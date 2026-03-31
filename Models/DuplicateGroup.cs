using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Deduplicate.Helpers;

namespace Deduplicate.Models;

public class DuplicateGroup : INotifyPropertyChanged
{
    public string GroupKey { get; init; } = string.Empty;
    public ObservableCollection<FileItem> Items { get; } = new();

    public long WastedBytes => Items.Count > 1
        ? (Items.Count - 1) * (Items.FirstOrDefault()?.SizeBytes ?? 0)
        : 0;

    public string GroupLabel => Items.Count > 0
        ? $"{Items.Count} copies · {FileSizeFormatter.FormatBytes(WastedBytes)} wasted"
        : string.Empty;

    public DuplicateGroup()
    {
        Items.CollectionChanged += OnItemsChanged;
    }

    private void OnItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(WastedBytes));
        OnPropertyChanged(nameof(GroupLabel));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
