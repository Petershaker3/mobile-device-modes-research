using System.Net;

namespace logcat.Services;

public static class WirelessAdbConnectDiscovery
{
    public static async Task<IPEndPoint> WaitForMatchingConnectServiceAsync(
        Func<CancellationToken, Task<IReadOnlyList<WirelessAdbService>>> readServices,
        IPAddress pairingAddress,
        TimeSpan timeout,
        TimeSpan pollInterval,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(readServices);
        ArgumentNullException.ThrowIfNull(pairingAddress);
        if (timeout <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(timeout));
        if (pollInterval <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(pollInterval));

        using var deadline = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        deadline.CancelAfter(timeout);

        while (true)
        {
            IReadOnlyList<WirelessAdbService> services;
            try
            {
                services = await readServices(deadline.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                throw new TimeoutException("The phone did not advertise its Wireless debugging connection service before the discovery deadline.");
            }

            var matches = services
                .Where(service => service.Kind == WirelessAdbServiceKind.Connect &&
                                  service.Endpoint.Address.Equals(pairingAddress))
                .Select(service => service.Endpoint)
                .DistinctBy(endpoint => endpoint.ToString())
                .ToArray();

            if (matches.Length == 1) return matches[0];
            if (matches.Length > 1)
                throw new InvalidOperationException("The phone advertised more than one connection port. Refresh Wireless debugging and retry.");

            try
            {
                await Task.Delay(pollInterval, deadline.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                throw new TimeoutException("The phone did not advertise its Wireless debugging connection service before the discovery deadline.");
            }
        }
    }
}
