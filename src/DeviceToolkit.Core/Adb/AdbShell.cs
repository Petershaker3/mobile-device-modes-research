// NEW FILE — not copied. See ATTRIBUTION.md §5.
//
// Why this exists
// ---------------
// Upstream's `AdbManager.Shell(serial, cmd, timeoutSec = 30)` accepts a timeout
// and never uses it:
//
//     public string Shell(string serial, string cmd, int timeoutSec = 30)
//     {
//         var device = FindDevice(serial);
//         var receiver = new ConsoleOutputReceiver();
//         _client.ExecuteRemoteCommand(cmd, device, receiver, Encoding.UTF8);
//         return receiver.ToString();          // <- timeoutSec never referenced
//     }
//
// And AdvancedSharpAdbClient's command channel has no timeout of its own, so a
// device command that never returns - a `su` prompt waiting for a tap, a wedged
// `logcat`, a `find /` on a full partition - blocks the calling thread forever.
// Upstream knows this: the comment on RootProbeTimeoutSec says exactly that
// ("设备的 su 弹授权框等待用户操作时调用会无限阻塞") and works around it with a
// hand-rolled 8-second race that only covers the root probe.
//
// Why not patch AdbManager in place? Because the transplant rule for this repo
// is that vendored files stay byte-identical, and the verification script now
// proves that structurally (scripts/check-subsequence.py). One shim for an
// unavoidable System.Drawing removal is defensible; a second shim that also
// changes behaviour would turn "we vendored upstream" into "we vendored
// something upstream-shaped".
//
// So this file routes around the bug instead of hiding it: it does not call
// Shell at all. It composes two primitives upstream already has and already
// gets right -
//   * AdbManager.AdbArgs   - adds "-s <serial>" to device-scoped commands
//   * AdbManager.RunAdbAsync - spawns adb, streams stdout/stderr line by line,
//     and kills the whole process tree on cancellation
// - behind a CancellationTokenSource whose deadline is the timeout the caller
// asked for. When the deadline passes, the token fires, RunAdbAsync kills adb,
// and the caller gets a TimeoutException instead of a hung thread.
//
// This is verifyable without a phone: the runner is an injected seam, so the
// test drives a fake that never returns and asserts the timeout actually fires.
//
// Message language: English, not localised through Loc. Upstream's own
// convention is that exception messages stay in the source language rather than
// going through the UI translation table (see the "异常消息按约定保持中文，
// 不本地化" comment on the adb-startup failure), and a Loc key that upstream
// does not have would fail scripts/check-loc-keys.py. Consistency with the
// codebase beats inventing a third convention.

using System.Text;

namespace logcat.Services;

/// <summary>
/// Runs a shell command on one device with a timeout that is enforced.
/// </summary>
public sealed class AdbShell
{
    /// <summary>What AdbManager.RunAdbAsync looks like, as a seam we can fake.</summary>
    internal delegate Task<int> Runner(string arguments, IProgress<string>? onLine, CancellationToken ct);

    readonly Runner _run;

    public AdbShell() : this((args, onLine, ct) => AdbManager.RunAdbAsync(args, onLine, ct))
    {
    }

    /// <summary>Test seam: production always uses the adb process runner above.</summary>
    internal AdbShell(Runner run) => _run = run ?? throw new ArgumentNullException(nameof(run));

    /// <summary>
    /// Runs <paramref name="command"/> through `adb shell` and returns stdout.
    /// </summary>
    /// <param name="serial">Target device serial; passed through <see cref="AdbManager.AdbArgs"/>.</param>
    /// <param name="command">Device-side command, e.g. <c>getprop ro.product.model</c>.</param>
    /// <param name="timeout">Hard deadline. Must be positive.</param>
    /// <param name="ct">Optional caller cancellation; cancelling it is not a timeout.</param>
    /// <exception cref="TimeoutException">The deadline passed before adb exited.</exception>
    /// <exception cref="ArgumentException">The timeout was zero or negative.</exception>
    public async Task<string> RunAsync(
        string serial,
        string command,
        TimeSpan timeout,
        CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(serial);
        ArgumentException.ThrowIfNullOrWhiteSpace(command);
        if (timeout <= TimeSpan.Zero)
            throw new ArgumentException(
                "A timeout was required but was not positive. Upstream's Shell silently ignores its " +
                "timeoutSec argument; this type refuses the same mistake by rejecting a non-positive deadline.",
                nameof(timeout));

        // Append synchronously: Progress<T> posts to the captured SynchronizationContext,
        // which would both lose ordering and still be delivering lines after we return.
        var output = new StringBuilder();
        var sink = new SynchronousLineSink(output);

        using var deadline = CancellationTokenSource.CreateLinkedTokenSource(ct);
        deadline.CancelAfter(timeout);

        try
        {
            await _run(AdbManager.AdbArgs($"shell {command}", serial), sink, deadline.Token)
                .ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            // Only the deadline fired - the caller did not give up. This is the
            // case that used to hang forever.
            throw new TimeoutException(
                $"adb shell timed out after {timeout.TotalSeconds:0.#}s on {serial}: {command}");
        }

        return output.ToString();
    }

    sealed class SynchronousLineSink(StringBuilder sink) : IProgress<string>
    {
        public void Report(string? value)
        {
            if (value is null) return;
            lock (sink) sink.AppendLine(value);
        }
    }
}
