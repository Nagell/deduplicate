using Deduplicate.Helpers;

namespace Deduplicate.Models;

public class FileItem : ObservableObject
{
    private bool _isSelected;

    public string Path { get; init; } = string.Empty;
    public string Name => System.IO.Path.GetFileName(Path);
    public string Directory => System.IO.Path.GetDirectoryName(Path) ?? string.Empty;
    public long SizeBytes { get; init; }
    public DateTime LastModified { get; init; }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    public string SizeFormatted => FileSizeFormatter.FormatBytes(SizeBytes);
    public string LastModifiedFormatted => LastModified.ToString("yyyy-MM-dd HH:mm");
}
