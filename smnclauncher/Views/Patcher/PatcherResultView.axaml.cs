using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using smnclauncher.ViewModels.Patcher;
using System;
using System.Reactive.Disposables;

namespace smnclauncher.Views.Patcher
{
    public class PatcherResultView : ReactiveUserControl<PatcherResultViewModel>
    {
        public TextBlock TextTitle => this.FindControl<TextBlock>("TextTitle");

        public TextBlock TextResult => this.FindControl<TextBlock>("TextResult");

        public PatcherResultView()
        {
            this.InitializeComponent();
            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, vm => vm.Message, v => v.TextResult.Text)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.Result, v => v.TextTitle.Text, MakeTitleFromResult)
                    .DisposeWith(disposables);
            });
        }

        private void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            // Not sure if there's an easier way to find the window for a given control in avaloniaUI
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime application)
            {
                foreach (var window in application.Windows)
                {
                    if (window is PatcherWindow)
                    {
                        window.Close();

                        break;
                    }
                }
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private string MakeTitleFromResult(Result arg)
        {
            return arg switch
            {
                Result.Success => "Patching Succeeded",
                Result.Failure => "Patching Failed",
                _ => throw new NotImplementedException()
            };
        }
    }
}
