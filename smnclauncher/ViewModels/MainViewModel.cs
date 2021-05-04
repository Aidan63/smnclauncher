using ReactiveUI;
using smnclauncher.Models;
using smnclauncher.ViewModels.Patcher;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace smnclauncher.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        public readonly GameLocationViewModel gameLocation;

        public readonly AuthenticateViewModel authentication;

        public readonly Interaction<PatcherViewModel, Unit> launchPatcher;

        public readonly ReactiveCommand<Unit, Unit> launch;

        public readonly ReactiveCommand<Unit, Unit> update;

        public MainViewModel()
        {
            gameLocation   = new GameLocationViewModel();
            authentication = new AuthenticateViewModel();
            launchPatcher  = new Interaction<PatcherViewModel, Unit>();

            // We can only start the game when non null credentials and a non null game install have been provided.
            var hasValidInstall = this.WhenAnyValue(vm => vm.gameLocation.Install).Select(v => v is not null);
            var canLaunchGame   = this
                .WhenAnyValue(vm => vm.authentication.Username, vm => vm.authentication.Password, vm => vm.gameLocation.Install)
                .Select(((string username, string password, IInstall? install) args) => !string.IsNullOrWhiteSpace(args.username) && !string.IsNullOrWhiteSpace(args.password) && args.install is not null);

            launch = ReactiveCommand.CreateFromTask(LaunchGame, canLaunchGame);
            update = ReactiveCommand.CreateFromObservable<Unit, Unit>(_ => launchPatcher.Handle(new PatcherViewModel(gameLocation.Install)), hasValidInstall);
        }

        private async Task LaunchGame()
        {
            switch (gameLocation.Install)
            {
                case SteamInstall:
                    var steamInfo = new ProcessStartInfo
                    {
                        UseShellExecute = false,
                        FileName        = "C:/Program Files (x86)/Steam/Steam.exe",
                        Arguments       = $"-applaunch 104700 -autologin -Ticket=\"\"\"{ authentication.Username.Length }|{ authentication.Username }|{ authentication.Password }\"\"\""
                    };
                    using (var proc = new Process { StartInfo = steamInfo })
                    {
                        proc.Start();

                        await proc.WaitForExitAsync();
                    }
                    break;
                case ManualInstall manual:
                    var exe = Path.Combine(manual.Directory(), "Binaries", "Win32", "SuperMNCGameClient.exe");
                    if (!File.Exists(exe))
                    {
                        throw new FileNotFoundException($"Executable { exe } not found");
                    }

                    var manualInfo = new ProcessStartInfo
                    {
                        UseShellExecute = false,
                        FileName        = exe,
                        Arguments       = $"-autologin -Ticket=\"\"\"{ authentication.Username.Length }|{ authentication.Username }|{ authentication.Password }\"\"\""
                    };
                    using (var proc = new Process { StartInfo = manualInfo })
                    {
                        proc.Start();

                        await proc.WaitForExitAsync();
                    }
                    break;
                default:
                    throw new NullReferenceException("game installation was null");
            }
        }

        private void PatchGame()
        {
            //
        }
    }
}
