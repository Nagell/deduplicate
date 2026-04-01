namespace Deduplicate.Models;

public record ScanProgress(
    long BytesProcessed,
    long TotalBytes,
    int FilesScanned,
    double ThroughputBytesPerSec,
    double EtaSeconds)
{
    public ScanProgress
    {
        if (TotalBytes < 0) throw new ArgumentOutOfRangeException(nameof(TotalBytes));
        if (BytesProcessed < 0 || BytesProcessed > TotalBytes) throw new ArgumentOutOfRangeException(nameof(BytesProcessed));
        if (ThroughputBytesPerSec < 0) throw new ArgumentOutOfRangeException(nameof(ThroughputBytesPerSec));
        if (EtaSeconds < 0) throw new ArgumentOutOfRangeException(nameof(EtaSeconds));
    }
}
