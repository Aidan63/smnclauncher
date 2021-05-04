using Akavache;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Markdown.Avalonia;
using ReactiveUI;
using smnclauncher.ViewModels;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace smnclauncher.Views
{
    public class MainWindow : ReactiveWindow<MainViewModel>
    {
        public Button BttnLanch => this.FindControl<Button>("BttnLaunch");

        public Button BttnUpdate => this.FindControl<Button>("BttnUpdate");

        public GameLocationView GameLocationView => this.FindControl<GameLocationView>("GameLocationView");

        public AuthenticateView AuthenticateView => this.FindControl<AuthenticateView>("AuthenticateView");

        public MarkdownScrollViewer PatchNotesDisplay => this.FindControl<MarkdownScrollViewer>("PatchNotes");

        public MainWindow()
        {
            this.InitializeComponent();
            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, vm => vm.gameLocation, v => v.GameLocationView.ViewModel)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.authentication, v => v.AuthenticateView.ViewModel)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.PatchNotes, v => v.PatchNotesDisplay.Markdown)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.launch, v => v.BttnLanch)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.update, v => v.BttnUpdate)
                    .DisposeWith(disposables);

                this.BindInteraction(ViewModel, vm => vm.launchPatcher, ctx =>
                    {
                        var popup = new PatcherWindow { Width = 800, Height = 300, WindowStartupLocation = WindowStartupLocation.CenterOwner, ViewModel = ctx.Input };

                        ctx.SetOutput(Unit.Default);

                        return Observable.Start(async () => await popup.ShowDialog(this), RxApp.MainThreadScheduler);
                    })
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

        protected override void OnClosing(CancelEventArgs e)
        {
            BlobCache.Shutdown().Wait();
        }
    }
}
