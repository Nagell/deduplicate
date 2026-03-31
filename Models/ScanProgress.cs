namespace Deduplicate.Models;

public record ScanProgress(
    long BytesProcessed,
    long TotalBytes,
    int FilesScanned,
    double ThroughputBytesPerSec,
    double EtaSeconds
);
