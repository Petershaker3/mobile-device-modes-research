using System.Security.Cryptography;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Data.Sqlite;

namespace DeviceToolkit.Core.Operations;

public sealed record AuditActor(string Surface, string? ClientId = null, string? SessionId = null);

public sealed record AuditDetail(
    bool Redacted,
    bool Truncated = false,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    IReadOnlyDictionary<string, string?>? Fields = null);

public sealed record AuditEventDraft(
    string EventType,
    AuditActor Actor,
    string Summary,
    Guid? OperationId = null,
    string? ImpactLevel = null,
    AuditDetail? Detail = null);

public sealed record AuditEvent(
    Guid EventId,
    [property: JsonPropertyName("seq")]
    long Sequence,
    DateTimeOffset OccurredAt,
    string EventType,
    AuditActor Actor,
    Guid? OperationId,
    string? ImpactLevel,
    string Summary,
    AuditDetail? Detail,
    string? PreviousEventHash);

public sealed record AuditVerificationResult(long EventCount, string? LatestEventHash);

public sealed class AuditIntegrityViolation : Exception
{
    public AuditIntegrityViolation(long sequence, string reason)
        : base($"Audit integrity check failed at sequence {sequence}: {reason}") => Sequence = sequence;

    public long Sequence { get; }
}

public sealed class AuditOperationRecordException : IOException
{
    public AuditOperationRecordException(bool operationStarted, bool operationCompleted,
        Exception? operationError, Exception auditError)
        : base(GetMessage(operationStarted, operationCompleted),
            operationError is null ? auditError : new AggregateException(operationError, auditError))
    {
        OperationStarted = operationStarted;
        OperationCompleted = operationCompleted;
    }

    public bool OperationStarted { get; }
    public bool OperationCompleted { get; }

    static string GetMessage(bool operationStarted, bool operationCompleted) =>
        !operationStarted
            ? "The operation was not run because its audit start record could not be saved."
            : operationCompleted
                ? "The operation completed and was verified, but its audit outcome could not be saved. Verify state before retrying."
                : "The operation may have changed state, but its audit outcome could not be saved. Reconcile state before retrying.";
}

/// <summary>
/// SQLite-backed append-only audit log. Each event is canonical JSON whose SHA-256
/// hash is linked from the next event. Integrity failures are surfaced and never repaired.
/// </summary>
public sealed class AuditStore : IDisposable, IAsyncDisposable
{
    const int SchemaVersion = 1;
    const int MaxSummaryLength = 2000;
    const int MaxFieldCount = 64;
    const int MaxFieldLength = 4000;

    static readonly JsonSerializerOptions JsonOptions = CreateJsonOptions();
    static readonly HashSet<string> EventTypes = new(StringComparer.Ordinal)
    {
        "DeviceDiscovered", "DeviceLost", "DeviceStateChanged", "CapabilityObserved",
        "OperationCreated", "PlanCreated", "PreviewShown", "ApprovalRequested",
        "ApprovalGranted", "ApprovalDenied", "ApprovalExpired", "OperationStarted",
        "OperationProgressed", "OperationCancelled", "OperationSucceeded", "OperationFailed",
        "OperationOutcomeUnknown", "OperationReconciled", "VerificationPerformed", "PolicyBlocked",
        "LimitOverridden", "ArtifactCreated", "CleanupPerformed", "ToolInvoked", "AuditIntegrityViolation"
    };

    readonly string _connectionString;
    readonly TimeProvider _timeProvider;
    bool _disposed;

    AuditStore(string connectionString, TimeProvider timeProvider)
    {
        _connectionString = connectionString;
        _timeProvider = timeProvider;
    }

    public static async Task<AuditStore> OpenAsync(string databasePath, TimeProvider? timeProvider = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);
        if (string.Equals(databasePath, ":memory:", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("AuditStore requires a persistent database path.", nameof(databasePath));

        var fullPath = Path.GetFullPath(databasePath);
        var directory = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
        var connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = fullPath,
            Mode = SqliteOpenMode.ReadWriteCreate,
            Cache = SqliteCacheMode.Shared,
            Pooling = false,
            DefaultTimeout = 5
        }.ToString();

        var store = new AuditStore(connectionString, timeProvider ?? TimeProvider.System);
        try
        {
            await store.InitializeAsync(cancellationToken).ConfigureAwait(false);
            await store.VerifyAsync(cancellationToken).ConfigureAwait(false);
            return store;
        }
        catch
        {
            await store.DisposeAsync().ConfigureAwait(false);
            throw;
        }
    }

    public async Task<AuditEvent> AppendAsync(AuditEventDraft draft, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ValidateDraft(draft);

        await using var connection = await OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        await using var transaction = connection.BeginTransaction(deferred: false);

        long sequence = 1;
        string? previousHash = null;
        await using (var select = connection.CreateCommand())
        {
            select.Transaction = transaction;
            select.CommandText = "SELECT seq, event_hash FROM audit_events ORDER BY seq DESC LIMIT 1;";
            await using var reader = await select.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
            if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                sequence = checked(reader.GetInt64(0) + 1);
                previousHash = reader.GetString(1);
            }
        }

        var auditEvent = new AuditEvent(Guid.NewGuid(), sequence, _timeProvider.GetUtcNow(), draft.EventType,
            draft.Actor, draft.OperationId, draft.ImpactLevel, draft.Summary, draft.Detail, previousHash);
        var json = SerializeCanonical(auditEvent);
        var hash = Hash(json);

        await using (var insert = connection.CreateCommand())
        {
            insert.Transaction = transaction;
            insert.CommandText = """
                INSERT INTO audit_events(seq, event_id, occurred_at, event_json, event_hash)
                VALUES ($seq, $eventId, $occurredAt, $eventJson, $eventHash);
                """;
            insert.Parameters.AddWithValue("$seq", auditEvent.Sequence);
            insert.Parameters.AddWithValue("$eventId", auditEvent.EventId.ToString("D"));
            insert.Parameters.AddWithValue("$occurredAt", auditEvent.OccurredAt.ToUniversalTime().ToString("O"));
            insert.Parameters.AddWithValue("$eventJson", json);
            insert.Parameters.AddWithValue("$eventHash", hash);
            await insert.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
        return auditEvent;
    }

    public async Task<T> ExecuteOperationAsync<T>(OperationPlan plan,
        Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(operation);
        OperationImpactPolicy.Validate(plan);

        AuditEventDraft CreateEvent(string eventType, string verb) => new(
            eventType,
            new AuditActor("DesktopUser"),
            $"{plan.OperationType} {verb}.",
            plan.PlanId,
            plan.ImpactLevel.ToString());

        try
        {
            await AppendAsync(CreateEvent("OperationStarted", "started"), cancellationToken).ConfigureAwait(false);
        }
        catch (Exception auditError)
        {
            throw new AuditOperationRecordException(operationStarted: false, operationCompleted: false,
                operationError: null, auditError);
        }

        T result;
        try
        {
            result = await operation(cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException operationError)
        {
            await AppendOutcomeOrThrowAsync(CreateEvent("OperationCancelled", "was cancelled"),
                operationError, operationCompleted: false).ConfigureAwait(false);
            throw;
        }
        catch (Exception operationError)
        {
            await AppendOutcomeOrThrowAsync(CreateEvent("OperationFailed", "failed"),
                operationError, operationCompleted: false).ConfigureAwait(false);
            throw;
        }

        try
        {
            await AppendAsync(CreateEvent("OperationSucceeded", "completed"), CancellationToken.None)
                .ConfigureAwait(false);
        }
        catch (Exception auditError)
        {
            throw new AuditOperationRecordException(operationStarted: true, operationCompleted: true,
                operationError: null, auditError);
        }

        return result;
    }

    public async Task ExecuteOperationAsync(OperationPlan plan,
        Func<CancellationToken, Task> operation, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(operation);
        _ = await ExecuteOperationAsync(plan, async token =>
        {
            await operation(token).ConfigureAwait(false);
            return true;
        }, cancellationToken).ConfigureAwait(false);
    }

    public async IAsyncEnumerable<T> ExecuteStreamOperationAsync<T>(OperationPlan plan,
        Func<CancellationToken, IAsyncEnumerable<T>> operation,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(operation);
        OperationImpactPolicy.Validate(plan);
        AuditEventDraft CreateEvent(string eventType, string verb) => new(
            eventType,
            new AuditActor("DesktopUser"),
            $"{plan.OperationType} {verb}.",
            plan.PlanId,
            plan.ImpactLevel.ToString());

        try
        {
            await AppendAsync(CreateEvent("OperationStarted", "started"), cancellationToken).ConfigureAwait(false);
        }
        catch (Exception auditError)
        {
            throw new AuditOperationRecordException(operationStarted: false, operationCompleted: false,
                operationError: null, auditError: auditError);
        }

        IAsyncEnumerator<T>? enumerator = null;
        Exception? operationError = null;
        var completed = false;
        try
        {
            enumerator = operation(cancellationToken).GetAsyncEnumerator(cancellationToken);
            while (true)
            {
                bool hasNext;
                try
                {
                    hasNext = await enumerator.MoveNextAsync().ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    operationError = exception;
                    break;
                }

                if (!hasNext)
                {
                    completed = true;
                    break;
                }

                yield return enumerator.Current;
            }
        }
        finally
        {
            if (enumerator is not null)
            {
                try { await enumerator.DisposeAsync().ConfigureAwait(false); }
                catch (Exception exception) { operationError ??= exception; }
            }

            var outcomeType = completed && operationError is null
                ? "OperationSucceeded"
                : operationError is OperationCanceledException || cancellationToken.IsCancellationRequested
                    ? "OperationCancelled"
                    : operationError is null
                        ? "OperationOutcomeUnknown"
                        : "OperationFailed";
            try
            {
                await AppendAsync(CreateEvent(outcomeType, outcomeType switch
                {
                    "OperationSucceeded" => "completed",
                    "OperationCancelled" => "was cancelled",
                    "OperationOutcomeUnknown" => "ended before completion",
                    _ => "failed"
                }), CancellationToken.None).ConfigureAwait(false);
            }
            catch (Exception auditError)
            {
                throw new AuditOperationRecordException(operationStarted: true,
                    operationCompleted: completed && operationError is null, operationError, auditError);
            }
        }

        if (operationError is not null) ExceptionDispatchInfo.Capture(operationError).Throw();
    }

    async Task AppendOutcomeOrThrowAsync(AuditEventDraft draft, Exception operationError, bool operationCompleted)
    {
        try
        {
            await AppendAsync(draft, CancellationToken.None).ConfigureAwait(false);
        }
        catch (Exception auditError)
        {
            throw new AuditOperationRecordException(operationStarted: true, operationCompleted,
                operationError, auditError);
        }
    }

    public async Task<AuditVerificationResult> VerifyAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        await using var connection = await OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT seq, event_id, occurred_at, event_json, event_hash FROM audit_events ORDER BY seq;";
        await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

        long expectedSequence = 1;
        string? previousHash = null;
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            var sequence = reader.GetInt64(0);
            var storedEventId = reader.GetString(1);
            var storedOccurredAt = reader.GetString(2);
            var json = reader.GetString(3);
            var storedHash = reader.GetString(4);
            if (sequence != expectedSequence)
                throw new AuditIntegrityViolation(sequence, $"Expected sequence {expectedSequence}.");

            AuditEvent? auditEvent;
            try
            {
                auditEvent = JsonSerializer.Deserialize<AuditEvent>(json, JsonOptions);
            }
            catch (JsonException exception)
            {
                throw new AuditIntegrityViolation(sequence, $"Event JSON is invalid ({exception.GetType().Name}).");
            }

            if (auditEvent is null || auditEvent.Sequence != sequence)
                throw new AuditIntegrityViolation(sequence, "Stored event sequence does not match its row.");
            if (!string.Equals(SerializeCanonical(auditEvent), json, StringComparison.Ordinal))
                throw new AuditIntegrityViolation(sequence, "Stored event JSON is not canonical.");
            if (!string.Equals(auditEvent.EventId.ToString("D"), storedEventId, StringComparison.Ordinal) ||
                !string.Equals(auditEvent.OccurredAt.ToUniversalTime().ToString("O"), storedOccurredAt, StringComparison.Ordinal))
                throw new AuditIntegrityViolation(sequence, "Stored event metadata does not match its row.");
            if (!string.Equals(auditEvent.PreviousEventHash, previousHash, StringComparison.Ordinal))
                throw new AuditIntegrityViolation(sequence, "Previous event hash does not match the chain.");
            if (storedHash.Length != 64 || storedHash.Any(character => !Uri.IsHexDigit(character)))
                throw new AuditIntegrityViolation(sequence, "Stored event hash is not a SHA-256 hex digest.");
            if (!CryptographicOperations.FixedTimeEquals(Convert.FromHexString(Hash(json)), Convert.FromHexString(storedHash)))
                throw new AuditIntegrityViolation(sequence, "Event hash does not match its canonical JSON.");

            previousHash = storedHash;
            expectedSequence++;
        }

        return new AuditVerificationResult(expectedSequence - 1, previousHash);
    }

    async Task InitializeAsync(CancellationToken cancellationToken)
    {
        await using var connection = await OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        await using (var pragmas = connection.CreateCommand())
        {
            pragmas.CommandText = "PRAGMA journal_mode = WAL; PRAGMA synchronous = FULL; PRAGMA foreign_keys = ON;";
            await pragmas.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }
        await using var versionCommand = connection.CreateCommand();
        versionCommand.CommandText = "PRAGMA user_version;";
        var version = Convert.ToInt32(await versionCommand.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false),
            System.Globalization.CultureInfo.InvariantCulture);
        if (version > SchemaVersion)
            throw new InvalidOperationException($"Audit database schema {version} is newer than supported version {SchemaVersion}.");

        await using var schema = connection.CreateCommand();
        schema.CommandText = """
            CREATE TABLE IF NOT EXISTS audit_events (
                seq INTEGER PRIMARY KEY CHECK (seq >= 1),
                event_id TEXT NOT NULL UNIQUE,
                occurred_at TEXT NOT NULL,
                event_json TEXT NOT NULL,
                event_hash TEXT NOT NULL CHECK (length(event_hash) = 64)
            );
            CREATE TRIGGER IF NOT EXISTS audit_events_no_update
            BEFORE UPDATE ON audit_events
            BEGIN
                SELECT RAISE(ABORT, 'audit_events is append-only');
            END;
            CREATE TRIGGER IF NOT EXISTS audit_events_no_delete
            BEFORE DELETE ON audit_events
            BEGIN
                SELECT RAISE(ABORT, 'audit_events is append-only');
            END;
            PRAGMA user_version = 1;
            """;
        await schema.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    async Task<SqliteConnection> OpenConnectionAsync(CancellationToken cancellationToken)
    {
        var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        return connection;
    }

    static void ValidateDraft(AuditEventDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(draft.Actor);
        if (!EventTypes.Contains(draft.EventType))
            throw new ArgumentException("Audit event type is not in the shared contract.", nameof(draft));
        if (draft.Actor.Surface is not ("DesktopUser" or "McpClient" or "Scheduler" or "System"))
            throw new ArgumentException("Audit actor surface is not in the shared contract.", nameof(draft));
        if (draft.Actor.ClientId?.Length > 128 || draft.Actor.SessionId?.Length > 128)
            throw new ArgumentException("Audit actor identifiers exceed the shared contract bounds.", nameof(draft));
        if (draft.Summary.Length > MaxSummaryLength)
            throw new ArgumentException("Audit summary exceeds the shared contract bound.", nameof(draft));
        if (draft.ImpactLevel is not null && draft.ImpactLevel is not ("ReadOnly" or "LocalWrite" or "DeviceWrite" or "WindowsSystemChange" or "Destructive"))
            throw new ArgumentException("Audit impact level is not in the shared contract.", nameof(draft));
        if (draft.EventType.StartsWith("Approval", StringComparison.Ordinal) && draft.OperationId is null)
            throw new ArgumentException("Approval events must reference an operation.", nameof(draft));
        if (draft.Detail is not { } detail) return;
        if (!detail.Redacted)
            throw new ArgumentException("Audit detail must be marked redacted.", nameof(draft));
        if (detail.Fields is { } fields && (fields.Count > MaxFieldCount || fields.Any(field => field.Key.Length > 128 || field.Value?.Length > MaxFieldLength)))
            throw new ArgumentException("Audit detail fields exceed the shared contract bounds.", nameof(draft));
    }

    static string SerializeCanonical(AuditEvent auditEvent)
    {
        var json = JsonSerializer.SerializeToElement(auditEvent, JsonOptions);
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream)) WriteCanonical(writer, json);
        return Encoding.UTF8.GetString(stream.ToArray());
    }

    static void WriteCanonical(Utf8JsonWriter writer, JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                writer.WriteStartObject();
                foreach (var property in element.EnumerateObject().OrderBy(property => property.Name, StringComparer.Ordinal))
                {
                    writer.WritePropertyName(property.Name);
                    WriteCanonical(writer, property.Value);
                }
                writer.WriteEndObject();
                break;
            case JsonValueKind.Array:
                writer.WriteStartArray();
                foreach (var item in element.EnumerateArray()) WriteCanonical(writer, item);
                writer.WriteEndArray();
                break;
            default:
                element.WriteTo(writer);
                break;
        }
    }

    static string Hash(string json) => Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(json)));

    static JsonSerializerOptions CreateJsonOptions()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.Converters.Add(new UtcDateTimeOffsetConverter());
        return options;
    }

    sealed class UtcDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
    {
        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = DateTimeOffset.Parse(reader.GetString()!, System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.RoundtripKind);
            if (value.Offset != TimeSpan.Zero || !reader.GetString()!.EndsWith('Z'))
                throw new JsonException("Audit timestamps must use the UTC Z suffix.");
            return value;
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            var utc = value.ToUniversalTime().ToString("O", System.Globalization.CultureInfo.InvariantCulture);
            writer.WriteStringValue(utc[..^6] + "Z");
        }
    }

    public ValueTask DisposeAsync()
    {
        Dispose();
        return ValueTask.CompletedTask;
    }

    public void Dispose() => _disposed = true;
}
