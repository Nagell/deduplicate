using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace Deduplicate;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        if (MicaController.IsSupported())
            SystemBackdrop = new MicaBackdrop();

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);

        AppWindow.Resize(new Windows.Graphics.SizeInt32(1100, 750));
    }
}
