using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using smnclauncher.ViewModels;
using System.Reactive.Disposables;

namespace smnclauncher.Views
{
    public class MainWindow : ReactiveWindow<MainViewModel>
    {
        public Button BttnLanch => this.FindControl<Button>("BttnLaunch");

        public Button BttnUpdate => this.FindControl<Button>("BttnUpdate");

        public GameLocationView GameLocationView => this.FindControl<GameLocationView>("GameLocationView");

        public AuthenticateView AuthenticateView => this.FindControl<AuthenticateView>("AuthenticateView");

        public MainWindow()
        {
            this.InitializeComponent();
            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, vm => vm.gameLocation, v => v.GameLocationView.ViewModel)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.authentication, v => v.AuthenticateView.ViewModel)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.launch, v => v.BttnLanch)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.update, v => v.BttnUpdate)
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
