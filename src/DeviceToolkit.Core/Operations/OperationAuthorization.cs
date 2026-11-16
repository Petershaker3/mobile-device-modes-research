using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace DeviceToolkit.Core.Operations;

public enum OperationImpact
{
    ReadOnly,
    LocalWrite,
    DeviceWrite,
    WindowsSystemChange,
    Destructive
}

public static class OperationImpactPolicy
{
    static readonly IReadOnlyDictionary<string, OperationImpact> Impacts =
        new Dictionary<string, OperationImpact>(StringComparer.Ordinal)
        {
            ["device.discover"] = OperationImpact.ReadOnly,
            ["device.wireless_discover"] = OperationImpact.ReadOnly,
            ["device.wireless_pair"] = OperationImpact.DeviceWrite,
            ["device.read_info"] = OperationImpact.ReadOnly,
            ["device.mode_transition"] = OperationImpact.DeviceWrite,
            ["app.list"] = OperationImpact.ReadOnly,
            ["app.details"] = OperationImpact.ReadOnly,
            ["app.install"] = OperationImpact.DeviceWrite,
            ["app.uninstall"] = OperationImpact.DeviceWrite,
            ["app.disable"] = OperationImpact.DeviceWrite,
            ["app.enable"] = OperationImpact.DeviceWrite,
            ["app.export_apk"] = OperationImpact.LocalWrite,
            ["file.list"] = OperationImpact.ReadOnly,
            ["file.pull"] = OperationImpact.LocalWrite,
            ["file.push"] = OperationImpact.DeviceWrite,
            ["file.delete"] = OperationImpact.DeviceWrite,
            ["file.rename"] = OperationImpact.DeviceWrite,
            ["diagnostics.logcat_capture"] = OperationImpact.ReadOnly,
            ["diagnostics.bugreport"] = OperationImpact.ReadOnly,
            ["diagnostics.dumpsys"] = OperationImpact.ReadOnly,
            ["storage.read_usage"] = OperationImpact.ReadOnly,
            ["firmware.inspect_package"] = OperationImpact.ReadOnly,
            ["firmware.inspect_image"] = OperationImpact.ReadOnly,
            ["backup.create"] = OperationImpact.LocalWrite,
            ["backup.restore"] = OperationImpact.DeviceWrite,
            ["network.dns_read"] = OperationImpact.ReadOnly,
            ["network.dns_apply"] = OperationImpact.DeviceWrite,
            ["drivers.diagnose"] = OperationImpact.ReadOnly,
            ["package.download"] = OperationImpact.LocalWrite,
            ["package.inspect"] = OperationImpact.ReadOnly
        };

    public static OperationImpact GetImpact(string operationType) =>
        Impacts.TryGetValue(operationType, out var impact)
            ? impact
            : throw new NotSupportedException($"Operation type '{operationType}' is not in the policy allowlist.");

    internal static void Validate(OperationPlan plan)
    {
        ArgumentNullException.ThrowIfNull(plan);
        if (GetImpact(plan.OperationType) != plan.ImpactLevel)
            throw new InvalidOperationException("The operation impact does not match the trusted policy for its type.");
        if (plan.ImpactLevel == OperationImpact.Destructive)
            throw new InvalidOperationException("Destructive operations are not available.");
        if (plan.TargetIdentityDigest.Length != 64 ||
            !plan.TargetIdentityDigest.All(Uri.IsHexDigit))
            throw new InvalidOperationException("The plan must target one explicit device identity.");
        if (plan.Inputs.Count > 64 || plan.Inputs.Any(input => input.Key.Length > 128 || input.Value.Length > 4096))
            throw new InvalidOperationException("Operation plan inputs exceed the supported bounds.");
        if (plan.VerificationRequirements.Count > 64 || plan.VerificationRequirements.Any(string.IsNullOrWhiteSpace))
            throw new InvalidOperationException("Operation verification requirements are invalid or exceed the supported bounds.");
        if (plan.VerificationRequirements.Count == 0 && plan.ImpactLevel is OperationImpact.LocalWrite or OperationImpact.DeviceWrite or OperationImpact.WindowsSystemChange)
            throw new InvalidOperationException("Write operations must define independent verification before approval.");
    }
}

public sealed class OperationPlan
{
    internal OperationPlan(
        Guid planId,
        string operationType,
        OperationImpact impactLevel,
        string targetIdentityDigest,
        DateTimeOffset createdAt,
        IReadOnlyDictionary<string, string> inputs,
        IReadOnlyList<string> verificationRequirements)
    {
        PlanId = planId;
        OperationType = operationType;
        ImpactLevel = impactLevel;
        TargetIdentityDigest = targetIdentityDigest;
        CreatedAt = createdAt;
        Inputs = inputs;
        VerificationRequirements = verificationRequirements;
    }

    public Guid PlanId { get; }
    public string OperationType { get; }
    public OperationImpact ImpactLevel { get; }
    public string TargetIdentityDigest { get; }
    public DateTimeOffset CreatedAt { get; }
    public IReadOnlyDictionary<string, string> Inputs { get; }
    public IReadOnlyList<string> VerificationRequirements { get; }

    internal string ComputeScopeDigest()
    {
        using var buffer = new MemoryStream();
        using (var json = new Utf8JsonWriter(buffer))
        {
            json.WriteStartObject();
            json.WriteString("createdAt", CreatedAt.ToUniversalTime().ToString("O", System.Globalization.CultureInfo.InvariantCulture));
            json.WriteString("impactLevel", ImpactLevel.ToString());
            json.WriteStartObject("inputs");
            foreach (var input in Inputs.OrderBy(pair => pair.Key, StringComparer.Ordinal))
                json.WriteString(input.Key, input.Value);
            json.WriteEndObject();
            json.WriteString("operationType", OperationType);
            json.WriteString("planId", PlanId.ToString("D"));
            json.WriteString("targetIdentityDigest", TargetIdentityDigest);
            json.WriteStartArray("verificationRequirements");
            foreach (var requirement in VerificationRequirements) json.WriteStringValue(requirement);
            json.WriteEndArray();
            json.WriteEndObject();
        }
        return Convert.ToHexStringLower(SHA256.HashData(buffer.ToArray()));
    }
}

public sealed class OperationApproval
{
    internal OperationApproval(Guid tokenId, Guid planId, OperationImpact impactLevel,
        DateTimeOffset issuedAt, DateTimeOffset expiresAt, string scopeDigest)
    {
        TokenId = tokenId;
        PlanId = planId;
        ImpactLevel = impactLevel;
        IssuedAt = issuedAt;
        ExpiresAt = expiresAt;
        ScopeDigest = scopeDigest;
    }

    public Guid TokenId { get; }
    public Guid PlanId { get; }
    public OperationImpact ImpactLevel { get; }
    public DateTimeOffset IssuedAt { get; }
    public DateTimeOffset ExpiresAt { get; }
    public string ScopeDigest { get; }
}

/// <summary>Issues short-lived, one-use approvals after the desktop user confirms a preview.</summary>
public sealed class ApprovalService : IDisposable
{
    static readonly TimeSpan ApprovalLifetime = TimeSpan.FromMinutes(5);
    readonly byte[] _targetKey = RandomNumberGenerator.GetBytes(32);
    readonly ConcurrentDictionary<Guid, OperationApproval> _issued = new();
    readonly TimeProvider _timeProvider;
    bool _disposed;

    public ApprovalService(TimeProvider? timeProvider = null) => _timeProvider = timeProvider ?? TimeProvider.System;

    public OperationPlan CreatePlan(
        string operationType,
        string targetSerial,
        IEnumerable<KeyValuePair<string, string>> inputs,
        IEnumerable<string> verificationRequirements)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentException.ThrowIfNullOrWhiteSpace(targetSerial);
        if (targetSerial.Length > 128 || targetSerial.Any(char.IsControl))
            throw new ArgumentException("The explicit device target is invalid.", nameof(targetSerial));
        ArgumentNullException.ThrowIfNull(inputs);
        ArgumentNullException.ThrowIfNull(verificationRequirements);

        var impact = OperationImpactPolicy.GetImpact(operationType);
        var inputCopy = new SortedDictionary<string, string>(StringComparer.Ordinal);
        foreach (var input in inputs)
        {
            if (string.IsNullOrWhiteSpace(input.Key) || input.Value is null || !inputCopy.TryAdd(input.Key, input.Value))
                throw new ArgumentException("Plan inputs must have unique non-empty keys and non-null values.", nameof(inputs));
        }
        var verificationCopy = verificationRequirements.ToArray();
        var targetDigest = Convert.ToHexStringLower(HMACSHA256.HashData(_targetKey, Encoding.UTF8.GetBytes(targetSerial)));
        var plan = new OperationPlan(Guid.NewGuid(), operationType, impact, targetDigest, _timeProvider.GetUtcNow(),
            new System.Collections.ObjectModel.ReadOnlyDictionary<string, string>(inputCopy), Array.AsReadOnly(verificationCopy));
        OperationImpactPolicy.Validate(plan);
        return plan;
    }

    /// <summary>Call only after the desktop UI has shown the plan and the user has explicitly confirmed it.</summary>
    public OperationApproval ApproveAfterUserConfirmation(OperationPlan plan)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        OperationImpactPolicy.Validate(plan);
        if (plan.ImpactLevel == OperationImpact.ReadOnly)
            throw new InvalidOperationException("Read-only operations do not use approval tokens.");

        var issuedAt = _timeProvider.GetUtcNow();
        var approval = new OperationApproval(Guid.NewGuid(), plan.PlanId, plan.ImpactLevel,
            issuedAt, issuedAt + ApprovalLifetime, plan.ComputeScopeDigest());
        if (!_issued.TryAdd(approval.TokenId, approval)) throw new InvalidOperationException("Could not issue a unique approval token.");
        return approval;
    }

    public void ValidateAndConsumeForOperation(
        OperationPlan plan,
        OperationApproval approval,
        string expectedOperationType,
        string expectedTargetSerial,
        IEnumerable<KeyValuePair<string, string>> expectedInputs,
        IEnumerable<string> expectedVerificationRequirements)
    {
        ArgumentNullException.ThrowIfNull(expectedInputs);
        ArgumentNullException.ThrowIfNull(expectedVerificationRequirements);
        if (!string.Equals(plan.OperationType, expectedOperationType, StringComparison.Ordinal))
            throw new InvalidOperationException("The approval plan is for a different operation.");
        var expectedImpact = OperationImpactPolicy.GetImpact(expectedOperationType);
        if (plan.ImpactLevel != expectedImpact)
            throw new InvalidOperationException("The operation impact does not match the trusted policy for its type.");
        var expectedTargetDigest = Convert.ToHexStringLower(HMACSHA256.HashData(_targetKey, Encoding.UTF8.GetBytes(expectedTargetSerial)));
        if (!CryptographicOperations.FixedTimeEquals(Convert.FromHexString(plan.TargetIdentityDigest), Convert.FromHexString(expectedTargetDigest)))
            throw new InvalidOperationException("The approval plan targets a different device.");
        var expectedInputMap = new SortedDictionary<string, string>(StringComparer.Ordinal);
        foreach (var input in expectedInputs)
            if (!expectedInputMap.TryAdd(input.Key, input.Value))
                throw new ArgumentException("Expected operation inputs must have unique keys.", nameof(expectedInputs));
        if (plan.Inputs.Count != expectedInputMap.Count ||
            plan.Inputs.Any(pair => !expectedInputMap.TryGetValue(pair.Key, out var value) || !string.Equals(pair.Value, value, StringComparison.Ordinal)))
            throw new InvalidOperationException("The approved plan inputs do not match the operation about to run.");
        if (!plan.VerificationRequirements.SequenceEqual(expectedVerificationRequirements, StringComparer.Ordinal))
            throw new InvalidOperationException("The approved verification steps do not match the operation about to run.");
        ValidateAndConsume(plan, approval);
    }

    public void ValidateAndConsume(OperationPlan plan, OperationApproval approval)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(approval);
        OperationImpactPolicy.Validate(plan);
        if (plan.ImpactLevel == OperationImpact.ReadOnly)
            throw new InvalidOperationException("Read-only operations cannot consume write approval tokens.");
        if (!_issued.TryGetValue(approval.TokenId, out var issued) ||
            issued.PlanId != approval.PlanId || issued.ImpactLevel != approval.ImpactLevel ||
            issued.IssuedAt != approval.IssuedAt || issued.ExpiresAt != approval.ExpiresAt ||
            !CryptographicOperations.FixedTimeEquals(Convert.FromHexString(issued.ScopeDigest), Convert.FromHexString(approval.ScopeDigest)))
            throw new InvalidOperationException("The approval token was not issued by this service or has been altered.");
        if (approval.PlanId != plan.PlanId || approval.ImpactLevel != plan.ImpactLevel ||
            !CryptographicOperations.FixedTimeEquals(Convert.FromHexString(approval.ScopeDigest), Convert.FromHexString(plan.ComputeScopeDigest())))
            throw new InvalidOperationException("The approval does not match this exact operation plan.");
        var now = _timeProvider.GetUtcNow();
        if (now < approval.IssuedAt || now >= approval.ExpiresAt)
            throw new InvalidOperationException("The approval has expired. Review and confirm the plan again.");
        if (!_issued.TryRemove(approval.TokenId, out _))
            throw new InvalidOperationException("The approval has already been used.");
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _issued.Clear();
        CryptographicOperations.ZeroMemory(_targetKey);
    }
}
