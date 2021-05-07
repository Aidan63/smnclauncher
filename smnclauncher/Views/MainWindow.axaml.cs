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

        public Button BttnAccount => this.FindControl<Button>("BttnAccount");

        public Button BttnDirectory => this.FindControl<Button>("BttnDirectory");

        public Button BttnUpdate => this.FindControl<Button>("BttnUpdate");

        public PatchNotesView PatchNotesView => this.FindControl<PatchNotesView>("PatchNotesView");

        public MainWindow()
        {
            this.InitializeComponent();
            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, vm => vm.patchNotes, v => v.PatchNotesView.ViewModel)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.launch, v => v.BttnLanch)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.updateCredentials, v => v.BttnAccount)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.findGameLocation, v => v.BttnDirectory)
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

                this.BindInteraction(ViewModel, vm => vm.launchCredentials, ctx =>
                    {
                        var popup = new CredentialsWindow { Width = 400, Height = 450, WindowStartupLocation = WindowStartupLocation.CenterOwner, ViewModel = ctx.Input };

                        ctx.SetOutput(Unit.Default);

                        return Observable.Start(async () => await popup.ShowDialog(this), RxApp.MainThreadScheduler);
                    })
                    .DisposeWith(disposables);

                this.BindInteraction(ViewModel, vm => vm.launchGameLocation, ctx =>
                    {
                        var popup = new GameLocationWindow { Width = 600, Height = 350, WindowStartupLocation = WindowStartupLocation.CenterOwner, ViewModel = ctx.Input };

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
