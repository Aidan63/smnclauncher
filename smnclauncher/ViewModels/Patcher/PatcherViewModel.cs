using Google.Apis.Drive.v3;
using Google.Apis.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using smnclauncher.Models;
using smnclauncher.Utils;
using Splat;
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

namespace smnclauncher.ViewModels.Patcher
{
    public class PatcherViewModel : ReactiveObject, IScreen, IActivatableViewModel
    {
        public RoutingState Router { get; }

        public ViewModelActivator Activator { get; }

        public readonly IInstall install;

        public PatcherViewModel(IInstall install)
        {
            Router    = new RoutingState();
            Activator = new ViewModelActivator();

            this.install = install;
            this.WhenActivated(disposables =>
            {
                Router
                    .Navigate
                    .Execute(new PatcherURLInputViewModel(install.Directory(), this))
                    .Subscribe()
                    .DisposeWith(disposables);
            });
        }
    }
}
