using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace smnclauncher.ViewModels.Patcher
{
    public class PatcherURLInputViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
    {
        public string? UrlPathSegment => "input";

        public IScreen HostScreen { get; }

        public ViewModelActivator Activator { get; }

        [Reactive] public string DriveUrl { get; set; }

        [ObservableAsProperty] public string? ItemID { get; }

        public readonly string installDirectory;

        public readonly ReactiveCommand<string, Unit> startPatch;

        public PatcherURLInputViewModel(string installDir, IScreen? screen = null)
        {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();
            Activator  = new ViewModelActivator();
            DriveUrl   = "";

            installDirectory = installDir;
            startPatch       = ReactiveCommand.Create<string>(
                item => HostScreen.Router.Navigate.Execute(new PatcherProgressViewModel(ItemID, installDir, HostScreen)).Subscribe(),
                this.WhenAnyValue(vm => vm.ItemID).Select(v => !string.IsNullOrWhiteSpace(v)));

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(vm => vm.DriveUrl)
                    .Select(ExtractGoogleDriveFileID)
                    .ToPropertyEx(this, vm => vm.ItemID)
                    .DisposeWith(disposables);
            });
        }

        public static string? ExtractGoogleDriveFileID(string input)
        {
            try
            {
                var firstSplit  = input.Split("/file/d/");
                var secondSplit = firstSplit[1].Split("/view");

                return secondSplit[0];
            }
            catch
            {
                return null;
            }
        }
    }
}
