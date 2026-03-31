using System.ComponentModel;
using System.Runtime.CompilerServices;
using Deduplicate.Helpers;

namespace Deduplicate.Models;

public class FileItem : INotifyPropertyChanged
{
    private bool _isSelected;

    public string Path { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Directory { get; init; } = string.Empty;
    public long SizeBytes { get; init; }
    public DateTime LastModified { get; init; }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value) return;
            _isSelected = value;
            OnPropertyChanged();
        }
    }

    public string SizeFormatted => FileSizeFormatter.FormatBytes(SizeBytes);
    public string LastModifiedFormatted => LastModified.ToString("yyyy-MM-dd HH:mm");

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
