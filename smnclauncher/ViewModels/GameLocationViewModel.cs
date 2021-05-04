using Gameloop.Vdf;
using Gameloop.Vdf.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using smnclauncher.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace smnclauncher.ViewModels
{
    public class GameLocationViewModel : ReactiveObject
    {
        [ObservableAsProperty] public IInstall? Install { get; }

        public readonly Interaction<Unit, string> getDirectory;

        public readonly ReactiveCommand<Unit, IInstall> findGameDirectory;

        public readonly ReactiveCommand<Unit, IInstall> selectGameDirectory;

        public GameLocationViewModel()
        {
            getDirectory        = new Interaction<Unit, string>();
            findGameDirectory   = ReactiveCommand.CreateFromObservable(FindSteamInstall);
            selectGameDirectory = ReactiveCommand.CreateFromObservable<IInstall>(() => getDirectory.Handle(Unit.Default).Where(Directory.Exists).Select(v => new ManualInstall(v)));

            Observable
                .Merge(findGameDirectory, selectGameDirectory)
                .ToPropertyEx(this, vm => vm.Install);
        }

        private static IObservable<IInstall> FindSteamInstall()
        {
            return Observable
                .Create<SteamInstall>(obs =>
                {
                    var steamDirectory = "C:/Program Files (x86)/Steam";
                    if (!Directory.Exists(steamDirectory))
                    {
                        throw new Exception("Unable to find steam install");
                    }

                    // Find all game libraries steam knows about.
                    var libraryFoldersVdfPath = "C:/Program Files (x86)/Steam/steamapps/libraryfolders.vdf";
                    var allLibraryFolders     = new List<string> { steamDirectory };
                    if (File.Exists(libraryFoldersVdfPath))
                    {
                        var libraries = VdfConvert.Deserialize(File.ReadAllText(libraryFoldersVdfPath));
                        if (libraries.Key == "LibraryFolders")
                        {
                            foreach (var item in libraries.Value.Where(v => v.Type == VTokenType.Property))
                            {
                                var prop  = item as VProperty;
                                var value = prop.Value as VValue;

                                if (Directory.Exists(value.Value as string))
                                {
                                    allLibraryFolders.Add(value.Value as string);
                                }
                            }
                        }
                    }

                    // Check for SMNCs APPID folder in all the linstall directories
                    foreach (var library in allLibraryFolders)
                    {
                        var steamapps = Path.Combine(library, "steamapps");
                        foreach (var file in Directory.GetFiles(steamapps, "*.acf"))
                        {
                            dynamic acf  = VdfConvert.Deserialize(File.ReadAllText(file));
                            VValue appid = acf.Value.appid;

                            if (appid.Value as string == "104700")
                            {
                                VValue installdir = acf.Value.installdir;

                                obs.OnNext(new SteamInstall(Path.Combine(steamapps, "common", installdir.Value as string)));
                                obs.OnCompleted();

                                return Disposable.Empty;
                            }
                        }
                    }

                    obs.OnError(new DirectoryNotFoundException("Unable to find a steam install of SMNC"));

                    return Disposable.Empty;
                });
        }
    }
}
