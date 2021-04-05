using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace smnclauncher.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        public readonly GameLocationViewModel gameLocation;

        public readonly AuthenticateViewModel authentication;

        public readonly ReactiveCommand<Unit, Unit> launch;

        public readonly ReactiveCommand<Unit, Unit> update;

        public MainViewModel()
        {
            gameLocation   = new GameLocationViewModel();
            authentication = new AuthenticateViewModel();
            launch         = ReactiveCommand.Create(LaunchGame);
            update         = ReactiveCommand.Create(PatchGame);
        }

        private void LaunchGame()
        {
            if (!Directory.Exists(gameLocation.Directory))
            {
                return;
            }

            var executableLocation = Path.Combine(gameLocation.Directory, "Binaries", "Win32", "SuperMNCGameClient.exe");
            if (!File.Exists(executableLocation))
            {
                return;
            }

            using (var proc = new Process { StartInfo = new ProcessStartInfo { UseShellExecute = false, FileName = executableLocation, Arguments = $"-autologin -Ticket={ authentication.Username.Length }|{ authentication.Username }|{ authentication.Password }" } })
            {
                proc.Start();
                proc.WaitForExit();
            }
        }

        private void PatchGame()
        {
            //
        }
    }
}
