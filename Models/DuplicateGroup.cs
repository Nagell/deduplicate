using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Deduplicate.Helpers;

namespace Deduplicate.Models;

public class DuplicateGroup : INotifyPropertyChanged
{
    private readonly ObservableCollection<FileItem> _items = new();

    public string GroupKey { get; init; } = string.Empty;
    public ReadOnlyObservableCollection<FileItem> Items { get; }

    public long WastedBytes => _items.Count > 1
        ? (_items.Count - 1) * (_items.FirstOrDefault()?.SizeBytes ?? 0)
        : 0;

    public string GroupLabel => _items.Count > 0
        ? $"{_items.Count} copies · {FileSizeFormatter.FormatBytes(WastedBytes)} wasted"
        : string.Empty;

    public DuplicateGroup()
    {
        Items = new ReadOnlyObservableCollection<FileItem>(_items);
        _items.CollectionChanged += OnItemsChanged;
    }

    public void AddItem(FileItem item) => _items.Add(item);
    public void RemoveItem(FileItem item) => _items.Remove(item);

    private void OnItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(WastedBytes));
        OnPropertyChanged(nameof(GroupLabel));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
