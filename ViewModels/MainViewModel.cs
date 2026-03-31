using System.Collections.ObjectModel;
using Deduplicate.Helpers;
using Deduplicate.Models;
using Deduplicate.Services;

namespace Deduplicate.ViewModels;

public class MainViewModel : ObservableObject
{
    private readonly DuplicateScanService _scanService = new();
    private CancellationTokenSource? _cts;

    private string _folderPath = string.Empty;
    private int _selectedMethodIndex = 0;
    private bool _isRecursive = true;
    private bool _isScanning;
    private bool _hasBeenScanned;
    private bool _hasEmptyFolder;
    private ScanProgress? _currentProgress;
    private int _selectedFileCount;
    private long _selectedBytes;
    private string? _errorMessage;

    public ObservableCollection<DuplicateGroup> DuplicateGroups { get; } = new();

    public string FolderPath
    {
        get => _folderPath;
        set
        {
            if (SetProperty(ref _folderPath, value))
                OnPropertyChanged(nameof(CanScan));
        }
    }

    public int SelectedMethodIndex
    {
        get => _selectedMethodIndex;
        set
        {
            if (SetProperty(ref _selectedMethodIndex, value))
                OnPropertyChanged(nameof(IsProgressIndeterminate));
        }
    }

    public bool IsRecursive
    {
        get => _isRecursive;
        set => SetProperty(ref _isRecursive, value);
    }

    public bool IsScanning
    {
        get => _isScanning;
        private set
        {
            if (SetProperty(ref _isScanning, value))
            {
                OnPropertyChanged(nameof(CanScan));
                OnPropertyChanged(nameof(ShowInitialState));
                OnPropertyChanged(nameof(ShowNoDuplicates));
            }
        }
    }

    public bool CanScan => !IsScanning && !string.IsNullOrWhiteSpace(FolderPath);

    public bool IsProgressIndeterminate => SelectedMethodIndex == 0;

    public ScanProgress? CurrentProgress
    {
        get => _currentProgress;
        private set
        {
            if (SetProperty(ref _currentProgress, value))
            {
                OnPropertyChanged(nameof(ProgressPercent));
                OnPropertyChanged(nameof(ProgressText));
            }
        }
    }

    public double ProgressPercent
    {
        get
        {
            var p = _currentProgress;
            if (p == null || p.TotalBytes == 0) return 0;
            return Math.Min(100, p.BytesProcessed * 100.0 / p.TotalBytes);
        }
    }

    public string ProgressText
    {
        get
        {
            var p = _currentProgress;
            if (p == null) return "Scanning...";
            var throughput = FileSizeFormatter.FormatBytes((long)p.ThroughputBytesPerSec);
            if (p.EtaSeconds < 1) return $"Almost done · {throughput}/s";
            var eta = p.EtaSeconds < 60
                ? $"~{(int)p.EtaSeconds}s remaining"
                : $"~{(int)(p.EtaSeconds / 60)}m remaining";
            return $"{eta} · {throughput}/s";
        }
    }

    public bool HasResults => DuplicateGroups.Count > 0;

    public bool ShowInitialState => !_hasBeenScanned && !IsScanning;

    public bool ShowNoDuplicates => _hasBeenScanned && !IsScanning && !HasResults && !_hasEmptyFolder;

    public bool HasSelection => _selectedFileCount > 0;

    public string StatusText
    {
        get
        {
            if (_selectedFileCount == 0)
            {
                var total = DuplicateGroups.Sum(g => g.Items.Count - 1);
                var wasted = DuplicateGroups.Sum(g => g.WastedBytes);
                return $"{DuplicateGroups.Count} duplicate groups    ·    {total} extra files    ·    {FileSizeFormatter.FormatBytes(wasted)} reclaimable";
            }
            return $"{_selectedFileCount} files selected · {FileSizeFormatter.FormatBytes(_selectedBytes)} to free";
        }
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        private set => SetProperty(ref _errorMessage, value);
    }

    public DetectionMethod SelectedMethod => SelectedMethodIndex switch
    {
        0 => DetectionMethod.QuickNameSize,
        1 => DetectionMethod.SmartSizeHash,
        _ => DetectionMethod.FullMd5
    };

    public async Task StartScanAsync()
    {
        if (!CanScan) return;

        _cts?.Cancel();
        _cts?.Dispose();
        _cts = new CancellationTokenSource();

        IsScanning = true;
        _hasBeenScanned = false;
        _hasEmptyFolder = false;
        ErrorMessage = null;
        CurrentProgress = null;
        DuplicateGroups.Clear();
        _selectedFileCount = 0;
        _selectedBytes = 0;
        RefreshSelectionProperties();

        var method = SelectedMethod;
        var folder = FolderPath;
        var recursive = IsRecursive;
        var token = _cts.Token;
        var progress = IsProgressIndeterminate
            ? null
            : new Progress<ScanProgress>(p => CurrentProgress = p);

        try
        {
            var groups = await Task.Run(
                () => _scanService.Scan(folder, method, recursive, token, progress),
                token);

            foreach (var g in groups)
                DuplicateGroups.Add(g);

            _hasBeenScanned = true;

            // Check if folder was empty (no files found at all) vs no duplicates
            // We can't tell easily here without scanning file count, so just use HasResults
        }
        catch (OperationCanceledException) { _hasBeenScanned = true; }
        catch (DirectoryNotFoundException)
        {
            ErrorMessage = $"Directory not found: {folder}";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Scan failed: {ex.Message}";
        }
        finally
        {
            _cts?.Dispose();
            _cts = null;
            IsScanning = false;
            OnPropertyChanged(nameof(HasResults));
            OnPropertyChanged(nameof(ShowInitialState));
            OnPropertyChanged(nameof(ShowNoDuplicates));
            OnPropertyChanged(nameof(StatusText));
        }
    }

    public void CancelScan()
    {
        _cts?.Cancel();
    }

    public void UpdateSelectionCount()
    {
        _selectedFileCount = DuplicateGroups
            .SelectMany(g => g.Items)
            .Count(f => f.IsSelected);
        _selectedBytes = DuplicateGroups
            .SelectMany(g => g.Items)
            .Where(f => f.IsSelected)
            .Sum(f => f.SizeBytes);
        RefreshSelectionProperties();
    }

    public List<FileItem> GetSelectedFiles()
        => DuplicateGroups.SelectMany(g => g.Items).Where(f => f.IsSelected).ToList();

    // Returns true if any group has ALL its copies selected
    public bool HasAllCopiesInAnyGroup()
        => DuplicateGroups.Any(g => g.Items.Count > 0 && g.Items.All(f => f.IsSelected));

    public void DeleteFiles(List<FileItem> files)
    {
        foreach (var file in files)
        {
            try { File.Delete(file.Path); }
            catch { /* skip files that can't be deleted */ }
        }

        var fileSet = files.ToHashSet();
        foreach (var group in DuplicateGroups.ToList())
        {
            foreach (var file in group.Items.Where(f => fileSet.Contains(f)).ToList())
                group.Items.Remove(file);

            if (group.Items.Count < 2)
                DuplicateGroups.Remove(group);
        }

        _selectedFileCount = 0;
        _selectedBytes = 0;
        RefreshSelectionProperties();

        OnPropertyChanged(nameof(HasResults));
        OnPropertyChanged(nameof(ShowNoDuplicates));
        OnPropertyChanged(nameof(StatusText));
    }

    private void RefreshSelectionProperties()
    {
        OnPropertyChanged(nameof(HasSelection));
        OnPropertyChanged(nameof(StatusText));
    }
}
