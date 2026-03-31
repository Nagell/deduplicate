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
        var picker = new Windows.Storage.Pickers.FolderPicker();
        picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
        picker.FileTypeFilter.Add("*");

        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

        var folder = await picker.PickSingleFolderAsync();
        if (folder != null)
            ViewModel.FolderPath = folder.Path;
    }

    private async void ScanButton_Click(object sender, RoutedEventArgs e)
    {
        await ViewModel.StartScanAsync();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.CancelScan();
    }

    private void OpenFile_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string path)
        {
            try
            {
                Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
            }
            catch { /* file may have been moved */ }
        }
    }

    private void OpenFolder_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string path)
        {
            try
            {
                Process.Start("explorer.exe", $"/select,\"{path}\"");
            }
            catch { /* ignore */ }
        }
    }

    private async void DeleteSelected_Click(object sender, RoutedEventArgs e)
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
        if (result == ContentDialogResult.Primary)
            ViewModel.DeleteFiles(selected);
    }
}
