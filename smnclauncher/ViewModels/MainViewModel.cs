using HtmlAgilityPack;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using smnclauncher.Models;
using smnclauncher.ViewModels.Patcher;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace smnclauncher.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        public readonly GameLocationViewModel gameLocation;

        public readonly CredentialsViewModel authentication;

        public readonly PatchNotesViewModel patchNotes;

        public readonly Interaction<PatcherViewModel, Unit> launchPatcher;

        public readonly Interaction<CredentialsViewModel, Unit> launchCredentials;

        public readonly Interaction<GameLocationViewModel, Unit> launchGameLocation;

        public readonly ReactiveCommand<Unit, Unit> launch;

        public readonly ReactiveCommand<Unit, Unit> update;

        public readonly ReactiveCommand<Unit, Unit> updateCredentials;

        public readonly ReactiveCommand<Unit, Unit> findGameLocation;

        public MainViewModel()
        {
            gameLocation       = new GameLocationViewModel();
            authentication     = new CredentialsViewModel();
            patchNotes         = new PatchNotesViewModel();
            launchPatcher      = new Interaction<PatcherViewModel, Unit>();
            launchCredentials  = new Interaction<CredentialsViewModel, Unit>();
            launchGameLocation = new Interaction<GameLocationViewModel, Unit>();

            // We can only start the game when non null credentials and a non null game install have been provided.
            var hasValidInstall     = this.WhenAnyValue(vm => vm.gameLocation.Install).Select(v => v is not null);
            var hasValidCredentials = this.WhenAnyValue(vm => vm.authentication.Username, vm => vm.authentication.Password).Select(args => !string.IsNullOrWhiteSpace(args.Item1) && !string.IsNullOrWhiteSpace(args.Item2));
            var canLaunchGame       = Observable
                .CombineLatest(
                    hasValidInstall,
                    hasValidCredentials,
                    (validInstall, validCredentials) => validInstall && validCredentials);

            launch = ReactiveCommand.CreateFromObservable(LaunchGame, canLaunchGame);
            update = ReactiveCommand.CreateFromObservable(LaunchPatcher, hasValidInstall);
            updateCredentials  = ReactiveCommand.CreateFromObservable(() => launchCredentials.Handle(authentication));
            findGameLocation = ReactiveCommand.CreateFromObservable(() => launchGameLocation.Handle(gameLocation));
        }

        private IObservable<Unit> LaunchPatcher() =>
            launchPatcher
                .Handle(new PatcherViewModel(gameLocation.Install));

        private IObservable<Unit> LaunchGame() =>
            Observable
                .Create<Unit>(async obs =>
                {
                    switch (gameLocation.Install)
                    {
                        case SteamInstall:
                            var steamInfo = new ProcessStartInfo
                            {
                                UseShellExecute = false,
                                FileName        = "C:/Program Files (x86)/Steam/Steam.exe",
                                Arguments       = $"-applaunch 104700 -autologin -Ticket=\"{ authentication.Username.Length }|{ authentication.Username }|{ authentication.Password }\""
                            };
                            using (var proc = new Process { StartInfo = steamInfo })
                            {
                                proc.Start();

                                await proc.WaitForExitAsync();
                            }
                            break;
                        case ManualInstall manual:
                            var exe        = Path.Combine(manual.Directory(), "Binaries", "Win32", "SuperMNCGameClient.exe");
                            var manualInfo = new ProcessStartInfo
                            {
                                UseShellExecute = false,
                                FileName        = exe,
                                Arguments       = $"-autologin -Ticket=\"{ authentication.Username.Length }|{ authentication.Username }|{ authentication.Password }\""
                            };
                            using (var proc = new Process { StartInfo = manualInfo })
                            {
                                proc.Start();

                                await proc.WaitForExitAsync();
                            }
                            break;
                        default: throw new NullReferenceException("game installation was null");
                    }

                    obs.OnCompleted();

                    return Disposable.Empty;
                });
    }
}
