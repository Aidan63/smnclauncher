using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using smnclauncher.ViewModels;
using smnclauncher.ViewModels.Patcher;
using smnclauncher.Views;
using smnclauncher.Views.Patcher;
using Splat;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;

namespace smnclauncher
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            Locator.CurrentMutable.Register(() => new PatcherProgressView(), typeof(IViewFor<PatcherProgressViewModel>));
            Locator.CurrentMutable.Register(() => new PatcherURLInputView(), typeof(IViewFor<PatcherURLInputViewModel>));
            Locator.CurrentMutable.Register(() => new PatcherResultView(), typeof(IViewFor<PatcherResultViewModel>));

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                Akavache.Registrations.Start("smnclauncher");

                SevenZip.SevenZipBase.SetLibraryPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Environment.Is64BitProcess ? "x64" : "x86", "7z.dll"));

                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
