using Deduplicate.Models;
using Deduplicate.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;

namespace Deduplicate.Views;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel { get; } = new();

    public MainPage()
    {
        InitializeComponent();
    }

    private async void BrowseButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var picker = new Windows.Storage.Pickers.FolderPicker();
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            picker.FileTypeFilter.Add("*");

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            var folder = await picker.PickSingleFolderAsync();
            if (folder != null)
                ViewModel.FolderPath = folder.Path;
        }
        catch (Exception ex)
        {
            await ShowErrorDialog("Could not open folder picker", ex.Message);
        }
    }

    private async void ScanButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewModel.StartScanAsync();
        }
        catch (Exception ex)
        {
            await ShowErrorDialog("Scan error", ex.Message);
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.CancelScan();
    }

    private async void OpenFile_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button btn || btn.Tag is not string path) return;
        try
        {
            if (!File.Exists(path))
            {
                await ShowErrorDialog("File not found", $"The file no longer exists:\n{path}");
                return;
            }
            Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            await ShowErrorDialog("Could not open file", ex.Message);
        }
    }

    private async void OpenFolder_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button btn || btn.Tag is not string path) return;
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = $"/select,\"{path}\"",
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            await ShowErrorDialog("Could not open folder", ex.Message);
        }
    }

    private async void DeleteSelected_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var selected = ViewModel.GetSelectedFiles();
            if (selected.Count == 0) return;

            var allCopiesWarning = ViewModel.HasAllCopiesInAnyGroup();

            var totalSize = Helpers.FileSizeFormatter.FormatBytes(selected.Sum(f => f.SizeBytes));
            var bodyText = $"Permanently delete {selected.Count} file{(selected.Count == 1 ? "" : "s")} ({totalSize})?\n\nThis cannot be undone. Files will NOT be sent to the Recycle Bin.";

            if (allCopiesWarning)
                bodyText += "\n\n⚠ Warning: You have selected ALL copies of one or more duplicate groups. Those files will be permanently lost.";

            var dialog = new ContentDialog
            {
                Title = "Confirm Permanent Deletion",
                Content = bodyText,
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = XamlRoot
            };

            var result = await dialog.ShowAsync();
            if (result != ContentDialogResult.Primary) return;

            var failed = ViewModel.DeleteFiles(selected);
            if (failed.Count == 0) return;

            const int maxShow = 15;
            var lines = failed.Take(maxShow).Select(p => $"• {p}");
            var failBodyText = string.Join("\n", lines);
            if (failed.Count > maxShow)
                failBodyText += $"\n...and {failed.Count - maxShow} more";

            var failDialog = new ContentDialog
            {
                Title = $"{failed.Count} file{(failed.Count == 1 ? "" : "s")} could not be deleted",
                Content = new TextBlock
                {
                    Text = failBodyText,
                    FontSize = 12,
                    TextWrapping = Microsoft.UI.Xaml.TextWrapping.Wrap
                },
                CloseButtonText = "OK",
                XamlRoot = XamlRoot
            };
            await failDialog.ShowAsync();
        }
        catch (Exception ex)
        {
            await ShowErrorDialog("Error", ex.Message);
        }
    }

    private async Task ShowErrorDialog(string title, string message)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            CloseButtonText = "OK",
            XamlRoot = XamlRoot
        };
        await dialog.ShowAsync();
    }
}
