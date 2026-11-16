using DeviceToolkit.Core.Operations;
using Microsoft.Data.Sqlite;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Xunit;

namespace logcat.Tests;

public sealed class AuditStoreTests
{
    [Fact]
    public async Task AppendAndVerify_PersistsAnOrderedHashChainAcrossReopen()
    {
        var databasePath = CreateDatabasePath();
        try
        {
            await using (var store = await AuditStore.OpenAsync(databasePath))
            {
                var first = await store.AppendAsync(new AuditEventDraft(
                    "OperationStarted", new AuditActor("DesktopUser"), "Read started"));
                var second = await store.AppendAsync(new AuditEventDraft(
                    "OperationSucceeded", new AuditActor("DesktopUser"), "Read completed"));

                Assert.Equal(1, first.Sequence);
                Assert.Null(first.PreviousEventHash);
                Assert.Equal(2, second.Sequence);
                Assert.Matches("^[0-9a-f]{64}$", second.PreviousEventHash!);
                Assert.Equal(2, (await store.VerifyAsync()).EventCount);

                await using var connection = new SqliteConnection(new SqliteConnectionStringBuilder
                {
                    DataSource = databasePath,
                    Mode = SqliteOpenMode.ReadOnly,
                    Pooling = false
                }.ToString());
                await connection.OpenAsync();
                await using var read = connection.CreateCommand();
                read.CommandText = "SELECT event_json FROM audit_events WHERE seq = 1;";
                var storedEvent = (string?)await read.ExecuteScalarAsync();
                Assert.Contains("\"seq\":1", storedEvent, StringComparison.Ordinal);
                Assert.DoesNotContain("\"sequence\"", storedEvent, StringComparison.Ordinal);
                using var document = JsonDocument.Parse(storedEvent!);
                Assert.EndsWith("Z", document.RootElement.GetProperty("occurredAt").GetString(), StringComparison.Ordinal);
            }

            await using var reopened = await AuditStore.OpenAsync(databasePath);
            Assert.Equal(2, (await reopened.VerifyAsync()).EventCount);
        }
        finally
        {
            DeleteDatabaseFiles(databasePath);
        }
    }

    [Fact]
    public async Task Append_RejectsUnredactedDetailsAndUnboundApprovalEvents()
    {
        var databasePath = CreateDatabasePath();
        try
        {
            await using var store = await AuditStore.OpenAsync(databasePath);
            await Assert.ThrowsAsync<ArgumentException>(() => store.AppendAsync(new AuditEventDraft(
                "OperationFailed", new AuditActor("System"), "Failed",
                Detail: new AuditDetail(Redacted: false))));
            await Assert.ThrowsAsync<ArgumentException>(() => store.AppendAsync(new AuditEventDraft(
                "ApprovalGranted", new AuditActor("DesktopUser"), "Approved")));
            Assert.Equal(0, (await store.VerifyAsync()).EventCount);
        }
        finally
        {
            DeleteDatabaseFiles(databasePath);
        }
    }

    [Fact]
    public async Task AuditRows_CannotBeUpdatedOrDeleted()
    {
        var databasePath = CreateDatabasePath();
        try
        {
            await using var store = await AuditStore.OpenAsync(databasePath);
            await store.AppendAsync(new AuditEventDraft("OperationStarted", new AuditActor("System"), "Started"));

            await using var connection = new SqliteConnection(new SqliteConnectionStringBuilder
            {
                DataSource = databasePath,
                Mode = SqliteOpenMode.ReadWrite,
                Pooling = false
            }.ToString());
            await connection.OpenAsync();

            await using (var update = connection.CreateCommand())
            {
                update.CommandText = "UPDATE audit_events SET event_json = 'changed' WHERE seq = 1;";
                await Assert.ThrowsAsync<SqliteException>(() => update.ExecuteNonQueryAsync());
            }
            await using (var delete = connection.CreateCommand())
            {
                delete.CommandText = "DELETE FROM audit_events WHERE seq = 1;";
                await Assert.ThrowsAsync<SqliteException>(() => delete.ExecuteNonQueryAsync());
            }
            await connection.DisposeAsync();
            Assert.Equal(1, (await store.VerifyAsync()).EventCount);
        }
        finally
        {
            DeleteDatabaseFiles(databasePath);
        }
    }

    [Fact]
    public async Task Verify_ReportsTamperedEventJsonWithoutRepairingIt()
    {
        var databasePath = CreateDatabasePath();
        try
        {
            await using var store = await AuditStore.OpenAsync(databasePath);
            await store.AppendAsync(new AuditEventDraft("OperationStarted", new AuditActor("System"), "Started"));

            await using var connection = new SqliteConnection(new SqliteConnectionStringBuilder
            {
                DataSource = databasePath,
                Mode = SqliteOpenMode.ReadWrite,
                Pooling = false
            }.ToString());
            await connection.OpenAsync();
            await using (var dropTrigger = connection.CreateCommand())
            {
                dropTrigger.CommandText = "DROP TRIGGER audit_events_no_update;";
                await dropTrigger.ExecuteNonQueryAsync();
            }
            await using (var tamper = connection.CreateCommand())
            {
                tamper.CommandText = "UPDATE audit_events SET event_json = replace(event_json, 'Started', 'Altered') WHERE seq = 1;";
                await tamper.ExecuteNonQueryAsync();
            }
            await connection.DisposeAsync();

            var failure = await Assert.ThrowsAsync<AuditIntegrityViolation>(() => store.VerifyAsync());
            Assert.Equal(1, failure.Sequence);
        }
        finally
        {
            DeleteDatabaseFiles(databasePath);
        }
    }

    [Fact]
    public async Task ExecuteOperationAsync_RecordsStartAndVerifiedSuccessWithoutTargetInputs()
    {
        var databasePath = CreateDatabasePath();
        try
        {
            await using var store = await AuditStore.OpenAsync(databasePath);
            using var approvals = new ApprovalService();
            var plan = approvals.CreatePlan("app.install", "device-serial-private", [], ["VerifyPackageState"]);

            var result = await store.ExecuteOperationAsync(plan, _ => Task.FromResult("verified"));

            Assert.Equal("verified", result);
            Assert.Equal(new[] { "OperationStarted", "OperationSucceeded" }, await ReadEventTypesAsync(databasePath));
            var json = await ReadEventsJsonAsync(databasePath);
            Assert.All(json, eventJson =>
            {
                Assert.DoesNotContain("device-serial-private", eventJson, StringComparison.Ordinal);
                Assert.DoesNotContain("VerifyPackageState", eventJson, StringComparison.Ordinal);
            });
            Assert.Equal(2, (await store.VerifyAsync()).EventCount);
        }
        finally
        {
            DeleteDatabaseFiles(databasePath);
        }
    }

    [Fact]
    public async Task ExecuteOperationAsync_RecordsFailureThenRethrowsOperationError()
    {
        var databasePath = CreateDatabasePath();
        try
        {
            await using var store = await AuditStore.OpenAsync(databasePath);
            using var approvals = new ApprovalService();
            var plan = approvals.CreatePlan("file.push", "device-serial-private", [], ["VerifyRemoteSizeAndFinalPath"]);

            await Assert.ThrowsAsync<InvalidOperationException>(() => store.ExecuteOperationAsync<int>(plan,
                _ => Task.FromException<int>(new InvalidOperationException("simulated failure"))));

            Assert.Equal(new[] { "OperationStarted", "OperationFailed" }, await ReadEventTypesAsync(databasePath));
            Assert.Equal(2, (await store.VerifyAsync()).EventCount);
        }
        finally
        {
            DeleteDatabaseFiles(databasePath);
        }
    }

    [Fact]
    public async Task ExecuteOperationAsync_RecordsCancellation()
    {
        var databasePath = CreateDatabasePath();
        try
        {
            await using var store = await AuditStore.OpenAsync(databasePath);
            using var approvals = new ApprovalService();
            var plan = approvals.CreatePlan("app.uninstall", "device-serial-private", [], ["VerifyPackageAbsent"]);

            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => store.ExecuteOperationAsync<int>(plan,
                _ => Task.FromCanceled<int>(new CancellationToken(canceled: true))));

            Assert.Equal(new[] { "OperationStarted", "OperationCancelled" }, await ReadEventTypesAsync(databasePath));
            Assert.Equal(2, (await store.VerifyAsync()).EventCount);
        }
        finally
        {
            DeleteDatabaseFiles(databasePath);
        }
    }

    [Fact]
    public async Task ExecuteStreamOperationAsync_RecordsCompletionAndEarlyStop()
    {
        var databasePath = CreateDatabasePath();
        try
        {
            await using var store = await AuditStore.OpenAsync(databasePath);
            using var approvals = new ApprovalService();
            var completedPlan = approvals.CreatePlan("file.list", "device-serial-private", [], []);
            var values = new List<int>();
            await foreach (var value in store.ExecuteStreamOperationAsync(completedPlan, _ => ValuesAsync()))
                values.Add(value);
            Assert.Equal(new[] { 1, 2 }, values);

            var partialPlan = approvals.CreatePlan("file.list", "device-serial-private", [], []);
            await foreach (var _ in store.ExecuteStreamOperationAsync(partialPlan, _ => ValuesAsync())) break;

            Assert.Equal(new[]
            {
                "OperationStarted", "OperationSucceeded", "OperationStarted", "OperationOutcomeUnknown"
            }, await ReadEventTypesAsync(databasePath));
            Assert.Equal(4, (await store.VerifyAsync()).EventCount);
        }
        finally
        {
            DeleteDatabaseFiles(databasePath);
        }
    }

    static string CreateDatabasePath() => Path.Combine(Path.GetTempPath(), $"device-toolkit-audit-{Guid.NewGuid():N}.db");

    static async IAsyncEnumerable<int> ValuesAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        yield return 1;
        await Task.Yield();
        cancellationToken.ThrowIfCancellationRequested();
        yield return 2;
    }

    static async Task<string[]> ReadEventTypesAsync(string databasePath) =>
        (await ReadEventsJsonAsync(databasePath)).Select(eventJson => JsonDocument.Parse(eventJson).RootElement.GetProperty("eventType").GetString()!).ToArray();

    static async Task<string[]> ReadEventsJsonAsync(string databasePath)
    {
        await using var connection = new SqliteConnection(new SqliteConnectionStringBuilder
        {
            DataSource = databasePath,
            Mode = SqliteOpenMode.ReadOnly,
            Pooling = false
        }.ToString());
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT event_json FROM audit_events ORDER BY seq;";
        await using var reader = await command.ExecuteReaderAsync();
        var results = new List<string>();
        while (await reader.ReadAsync()) results.Add(reader.GetString(0));
        return results.ToArray();
    }

    static void DeleteDatabaseFiles(string databasePath)
    {
        foreach (var suffix in new[] { "", "-wal", "-shm" })
            if (File.Exists(databasePath + suffix)) File.Delete(databasePath + suffix);
    }
}
