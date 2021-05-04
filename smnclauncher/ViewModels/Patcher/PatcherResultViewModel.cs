using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace smnclauncher.ViewModels.Patcher
{
    public enum Result
    {
        Success,
        Failure
    }

    public class PatcherResultViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
    {
        public string? UrlPathSegment => "result";

        public IScreen HostScreen { get; }

        public ViewModelActivator Activator { get; }

        public Result Result { get; }

        public string Message { get; }

        public PatcherResultViewModel(Result result, string message, IScreen hostScreen)
        {
            Activator  = new ViewModelActivator();
            HostScreen = hostScreen;
            Result     = result;
            Message    = message;
        }
    }
}
