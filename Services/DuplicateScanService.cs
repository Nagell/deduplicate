using System.Security.Cryptography;
using Deduplicate.Models;

namespace Deduplicate.Services;

public class DuplicateScanService
{
    public IReadOnlyList<DuplicateGroup> Scan(
        string folder,
        DetectionMethod method,
        bool recursive,
        CancellationToken ct,
        IProgress<ScanProgress>? progress = null)
    {
        var enumOptions = new EnumerationOptions
        {
            RecurseSubdirectories = recursive,
            IgnoreInaccessible = true,
            AttributesToSkip = FileAttributes.System
        };
        var files = Directory.EnumerateFiles(folder, "*", enumOptions)
            .Select(path =>
            {
                try
                {
                    var info = new FileInfo(path);
                    return (info, ok: true);
                }
                catch { return (info: null!, ok: false); }
            })
            .Where(x => x.ok)
            .Select(x => x.info)
            .ToList();

        ct.ThrowIfCancellationRequested();

        return method switch
        {
            DetectionMethod.QuickNameSize => ScanByNameAndSize(files, ct),
            DetectionMethod.SmartSizeHash => ScanBySmartHash(files, ct, progress),
            DetectionMethod.FullMd5       => ScanByFullHash(files, ct, progress),
            _                             => ScanByNameAndSize(files, ct)
        };
    }

    private static IReadOnlyList<DuplicateGroup> ScanByNameAndSize(
        List<FileInfo> files, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        return files
            .GroupBy(f => (f.Name.ToLowerInvariant(), f.Length))
            .Where(g => g.Count() > 1)
            .Select(g =>
            {
                var group = new DuplicateGroup { GroupKey = $"{g.Key.Item1}|{g.Key.Item2}" };
                foreach (var fi in g.OrderBy(f => f.FullName))
                    group.Items.Add(ToFileItem(fi));
                return group;
            })
            .ToList();
    }

    private static IReadOnlyList<DuplicateGroup> ScanBySmartHash(
        List<FileInfo> files, CancellationToken ct, IProgress<ScanProgress>? progress)
    {
        // Pre-filter: only files that share a size with at least one other
        var candidates = files
            .GroupBy(f => f.Length)
            .Where(g => g.Count() > 1)
            .SelectMany(g => g)
            .ToList();

        return HashFiles(candidates, ct, progress);
    }

    private static IReadOnlyList<DuplicateGroup> ScanByFullHash(
        List<FileInfo> files, CancellationToken ct, IProgress<ScanProgress>? progress)
        => HashFiles(files, ct, progress);

    private static IReadOnlyList<DuplicateGroup> HashFiles(
        List<FileInfo> filesToHash, CancellationToken ct, IProgress<ScanProgress>? progress)
    {
        long totalBytes = filesToHash.Sum(f => f.Length);
        long bytesHashed = 0;
        int filesScanned = 0;
        var startTime = DateTime.UtcNow;

        var hashGroups = new Dictionary<string, List<FileInfo>>(StringComparer.Ordinal);

        try
        {
            foreach (var file in filesToHash)
            {
                ct.ThrowIfCancellationRequested();

                string hash;
                try
                {
                    hash = ComputeMd5(file.FullName);
                }
                catch
                {
                    bytesHashed += file.Length;
                    filesScanned++;
                    continue;
                }

                if (!hashGroups.TryGetValue(hash, out var list))
                {
                    list = new List<FileInfo>();
                    hashGroups[hash] = list;
                }
                list.Add(file);

                bytesHashed += file.Length;
                filesScanned++;

                if (progress != null)
                {
                    var elapsed = (DateTime.UtcNow - startTime).TotalSeconds;
                    var throughput = elapsed > 0 ? bytesHashed / elapsed : 0;
                    var eta = throughput > 0 ? (totalBytes - bytesHashed) / throughput : 0;
                    progress.Report(new ScanProgress(bytesHashed, totalBytes, filesScanned, throughput, eta));
                }
            }
        }
        catch (OperationCanceledException) { /* return partial results below */ }

        return hashGroups
            .Where(kv => kv.Value.Count > 1)
            .Select(kv =>
            {
                var group = new DuplicateGroup { GroupKey = kv.Key };
                foreach (var fi in kv.Value.OrderBy(f => f.FullName))
                    group.Items.Add(ToFileItem(fi));
                return group;
            })
            .ToList();
    }

    private static string ComputeMd5(string filePath)
    {
        using var md5 = MD5.Create();
        using var stream = File.OpenRead(filePath);
        var hash = md5.ComputeHash(stream);
        return Convert.ToHexString(hash);
    }

    private static FileItem ToFileItem(FileInfo fi) => new()
    {
        Path = fi.FullName,
        Name = fi.Name,
        Directory = fi.DirectoryName ?? string.Empty,
        SizeBytes = fi.Length,
        LastModified = fi.LastWriteTime
    };
}
