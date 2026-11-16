namespace DeviceToolkit.Desktop.Services;

public enum FileTransferDirection
{
    Download,
    Upload
}

public sealed record FileTransferPreview(
    FileTransferDirection Direction,
    string DeviceModel,
    string DeviceSerial,
    string Source,
    string Destination,
    string LocalPath,
    string RemotePath,
    ulong SizeBytes,
    bool DestinationExists);
