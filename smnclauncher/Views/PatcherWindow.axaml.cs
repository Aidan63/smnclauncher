using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using smnclauncher.ViewModels;
using smnclauncher.ViewModels.Patcher;
using System.Reactive.Disposables;

namespace smnclauncher.Views
{
    public class PatcherWindow : ReactiveWindow<PatcherViewModel>
    {
        private RoutedViewHost ViewHost => this.FindControl<RoutedViewHost>("ViewHost");

        public PatcherWindow()
        {
            this.InitializeComponent();
            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, vm => vm.Router, v => v.ViewHost.Router)
                    .DisposeWith(disposables);
            });
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
