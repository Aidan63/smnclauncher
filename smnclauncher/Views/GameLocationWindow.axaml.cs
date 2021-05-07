using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using smnclauncher.Models;
using smnclauncher.ViewModels;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace smnclauncher.Views
{
    public class GameLocationWindow : ReactiveWindow<GameLocationViewModel>
    {
        public Button BttnAutoLocation => this.FindControl<Button>("BttnAutoDetect");

        public Button BttnSelectDirectory => this.FindControl<Button>("BttnSelectDirectory");

        public Grid GridCurrentInstall => this.FindControl<Grid>("GridCurrentInstall");

        public TextBox InputGameDirectory => this.FindControl<TextBox>("InputGameDirectory");

        public TextBlock TxtInstallType => this.FindControl<TextBlock>("TxtInstallType");

        public TextBlock TxtInstallLocation => this.FindControl<TextBlock>("TxtInstallLocation");

        public GameLocationWindow()
        {
            this.InitializeComponent();
            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, vm => vm.Install, v => v.GridCurrentInstall.Opacity, v => v is not null ? 1 : 0.5)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.Install, v => v.TxtInstallType.Text, v => v switch {
                        SteamInstall => "Steam",
                        ManualInstall => "Manual",
                        _ => ""
                    })
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.Install, v => v.TxtInstallLocation.Text, v => v is null ? "" : v.Directory())
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.Install, v => v.InputGameDirectory.Text, MakeDirectoryName)
                    .DisposeWith(disposables);

                this.BindInteraction(ViewModel, vm => vm.getDirectory, PromptUserForDirectory)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.selectGameDirectory, v => v.BttnSelectDirectory)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.findGameDirectory, v => v.BttnAutoLocation)
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

        private string MakeDirectoryName(IInstall? install)
        {
            if (install is not ManualInstall)
            {
                return "";
            }
            else
            {
                return install.Directory();
            }
        }

        private async Task PromptUserForDirectory(InteractionContext<Unit, string> ctx)
        {
            var popup  = new OpenFolderDialog { Title = "Select Game Folder" };
            var folder = await popup.ShowAsync(GetWindow());

            ctx.SetOutput(folder);
        }

        private Window GetWindow()
        {
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                return desktopLifetime.MainWindow;
            }

            throw new InvalidOperationException("Application lifetime is not classic desktop style");
        }
    }
}
