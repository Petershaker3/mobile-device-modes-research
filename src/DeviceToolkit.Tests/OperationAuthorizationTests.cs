using DeviceToolkit.Core.Operations;
using Xunit;

namespace logcat.Tests;

public sealed class OperationAuthorizationTests
{
    [Fact]
    public void ApprovalIsScopedToExactPlanAndCanBeConsumedOnlyOnce()
    {
        var clock = new TestTimeProvider(DateTimeOffset.Parse("2026-11-14T10:00:00Z"));
        using var service = new ApprovalService(clock);
        var plan = CreateInstallPlan(service);
        var approval = service.ApproveAfterUserConfirmation(plan);

        service.ValidateAndConsume(plan, approval);

        Assert.Throws<InvalidOperationException>(() => service.ValidateAndConsume(plan, approval));
    }

    [Fact]
    public void ChangingOneApprovedInputInvalidatesTheApproval()
    {
        using var service = new ApprovalService();
        var plan = CreateInstallPlan(service);
        var approval = service.ApproveAfterUserConfirmation(plan);
        var changedInputs = new Dictionary<string, string>(plan.Inputs, StringComparer.Ordinal)
        {
            ["sha256"] = new string('0', 64)
        };
        var changedPlan = Copy(plan, changedInputs);

        Assert.Throws<InvalidOperationException>(() => service.ValidateAndConsume(changedPlan, approval));
    }

    [Fact]
    public void ApprovalCannotBeRedirectedToAnotherDevice()
    {
        using var service = new ApprovalService();
        var plan = CreateInstallPlan(service);
        var approval = service.ApproveAfterUserConfirmation(plan);
        var inputs = new[]
        {
            KeyValuePair.Create("packageId", "com.example.app"),
            KeyValuePair.Create("sha256", new string('a', 64)),
            KeyValuePair.Create("versionCode", "3")
        };

        Assert.Throws<InvalidOperationException>(() => service.ValidateAndConsumeForOperation(
            plan, approval, "app.install", "different-device", inputs, ["VerifyPackageState"]));
        service.ValidateAndConsumeForOperation(
            plan, approval, "app.install", "test-device-serial", inputs, ["VerifyPackageState"]);
    }

    [Fact]
    public void ApprovalExpiresAfterFiveMinutes()
    {
        var clock = new TestTimeProvider(DateTimeOffset.Parse("2026-11-14T10:00:00Z"));
        using var service = new ApprovalService(clock);
        var plan = CreateInstallPlan(service);
        var approval = service.ApproveAfterUserConfirmation(plan);
        clock.Advance(TimeSpan.FromMinutes(5));

        Assert.Throws<InvalidOperationException>(() => service.ValidateAndConsume(plan, approval));
    }

    [Fact]
    public void ReadOnlyOperationsCannotReceiveAnApprovalToken()
    {
        using var service = new ApprovalService();
        var plan = service.CreatePlan("device.read_info", "test-device", [], []);

        Assert.Throws<InvalidOperationException>(() => service.ApproveAfterUserConfirmation(plan));
    }

    [Fact]
    public void WritePlanMustDeclareIndependentVerification()
    {
        using var service = new ApprovalService();

        Assert.Throws<InvalidOperationException>(() =>
            service.CreatePlan("app.install", "test-device", [], []));
    }

    [Fact]
    public void OperationTypeDeterminesImpactAndUnknownTypesAreDenied()
    {
        Assert.Equal(OperationImpact.DeviceWrite, OperationImpactPolicy.GetImpact("app.install"));
        Assert.Equal(OperationImpact.LocalWrite, OperationImpactPolicy.GetImpact("file.pull"));
        Assert.Throws<NotSupportedException>(() => OperationImpactPolicy.GetImpact("shell.execute"));
    }

    static OperationPlan CreateInstallPlan(ApprovalService service) => service.CreatePlan(
        "app.install",
        "test-device-serial",
        [
            KeyValuePair.Create("packageId", "com.example.app"),
            KeyValuePair.Create("sha256", new string('a', 64)),
            KeyValuePair.Create("versionCode", "3")
        ],
        ["VerifyPackageState"]);

    static OperationPlan Copy(OperationPlan original, IReadOnlyDictionary<string, string> inputs) => new(
        original.PlanId,
        original.OperationType,
        original.ImpactLevel,
        original.TargetIdentityDigest,
        original.CreatedAt,
        new System.Collections.ObjectModel.ReadOnlyDictionary<string, string>(new Dictionary<string, string>(inputs, StringComparer.Ordinal)),
        original.VerificationRequirements);

    sealed class TestTimeProvider(DateTimeOffset current) : TimeProvider
    {
        DateTimeOffset _current = current;
        public override DateTimeOffset GetUtcNow() => _current;
        public void Advance(TimeSpan duration) => _current += duration;
    }
}
