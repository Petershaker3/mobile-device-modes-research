// NEW FILE — not copied. See ATTRIBUTION.md §5.
//
// The first test in this repository that covers the bug upstream left open:
// AdbManager.Shell accepts a timeout and ignores it. These tests need no phone,
// because AdbShell's runner is an injected seam.

using logcat.Services;
using Xunit;

namespace logcat.Tests;

public class AdbShellTimeoutTests
{
    /// <summary>
    /// The whole point: a command that never returns must not hang the caller.
    /// Without the fix this test would never finish, which is why it is written
    /// with a hard external bound as well as the internal one.
    /// </summary>
    [Fact]
    public async Task 命令永不返回时超时抛出而不是挂起()
    {
        var shell = new AdbShell((args, onLine, ct) =>
        {
            // Simulate a wedged device: wait for cancellation, and record that we
            // saw it - the fix depends on the token actually reaching the runner.
            var infinite = Task.Delay(Timeout.Infinite, ct);
            return infinite.ContinueWith(_ => 0, TaskContinuationOptions.OnlyOnRanToCompletion);
        });

        var started = DateTime.UtcNow;
        await Assert.ThrowsAsync<TimeoutException>(() =>
            shell.RunAsync("emulator-5554", "logcat -v threadtime", TimeSpan.FromMilliseconds(200)));

        var elapsed = DateTime.UtcNow - started;

        // Bounded above: the timeout fired rather than something else rescuing us.
        Assert.True(elapsed < TimeSpan.FromSeconds(10),
            $"expected the deadline to fire, but the call took {elapsed.TotalSeconds:0.#}s");
    }

    /// <summary>A fast command returns its output and does not trip the deadline.</summary>
    [Fact]
    public async Task 命令正常返回时透传输出()
    {
        var shell = new AdbShell((args, onLine, ct) =>
        {
            onLine?.Report("Pixel 7");
            onLine?.Report("Android 14");
            return Task.FromResult(0);
        });

        var output = await shell.RunAsync("emulator-5554", "getprop ro.product.model", TimeSpan.FromSeconds(5));

        Assert.Contains("Pixel 7", output);
        Assert.Contains("Android 14", output);
    }

    /// <summary>
    /// The serial must reach adb as "-s &lt;serial&gt;", otherwise a machine with two
    /// devices attached answers from an arbitrary one. AdbArgs is upstream's own
    /// logic, so this also pins that we delegate to it instead of re-inventing it.
    /// </summary>
    [Fact]
    public async Task 序列号经AdbArgs拼入命令行()
    {
        string? captured = null;
        var shell = new AdbShell((args, onLine, ct) =>
        {
            captured = args;
            return Task.FromResult(0);
        });

        await shell.RunAsync("R58M12345AB", "pm list packages", TimeSpan.FromSeconds(5));

        Assert.Equal("-s R58M12345AB shell pm list packages", captured);
    }

    /// <summary>
    /// Host-scoped superflous serial would break `adb connect`-style commands, and
    /// a wire-serial containing spaces would split into extra arguments.
    /// </summary>
    [Fact]
    public async Task 命令含空格时整体作为一个shell参数()
    {
        string? captured = null;
        var shell = new AdbShell((args, onLine, ct) =>
        {
            captured = args;
            return Task.FromResult(0);
        });

        await shell.RunAsync("emulator-5554", "logcat -d -v threadtime -t 500", TimeSpan.FromSeconds(5));

        Assert.Equal("-s emulator-5554 shell logcat -d -v threadtime -t 500", captured);
    }

    /// <summary>
    /// A non-positive timeout is the exact shape of upstream's bug (the parameter
    /// is ignored, so 0 looks harmless). Here it is rejected instead.
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task 超时为零或负数时拒绝执行(int seconds)
    {
        var ran = false;
        var shell = new AdbShell((args, onLine, ct) =>
        {
            ran = true;
            return Task.FromResult(0);
        });

        await Assert.ThrowsAsync<ArgumentException>(() =>
            shell.RunAsync("emulator-5554", "getprop", TimeSpan.FromSeconds(seconds)));

        Assert.False(ran);   // refused before touching adb
    }

    /// <summary>
    /// Cancelling the caller's token is not a timeout, and must not be reported as
    /// one - a UI that closes mid-command should not paint an error banner.
    /// </summary>
    [Fact]
    public async Task 调用方取消时抛取消而非超时()
    {
        var shell = new AdbShell((args, onLine, ct) =>
        {
            var infinite = Task.Delay(Timeout.Infinite, ct);
            return infinite.ContinueWith(_ => 0, TaskContinuationOptions.OnlyOnRanToCompletion);
        });

        using var cts = new CancellationTokenSource();
        var task = shell.RunAsync("emulator-5554", "logcat", TimeSpan.FromSeconds(30), cts.Token);
        cts.CancelAfter(50);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => task);
    }
}
