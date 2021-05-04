using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using smnclauncher.ViewModels.Patcher;
using System.Reactive.Disposables;

namespace smnclauncher.Views.Patcher
{
    public class PatcherProgressView : ReactiveUserControl<PatcherProgressViewModel>
    {
        public ProgressBar BttnPatch => this.FindControl<ProgressBar>("Progress");

        public PatcherProgressView()
        {
            this.InitializeComponent();
            this.WhenActivated(disposables =>
            {
                this.WhenAnyObservable(v => v.ViewModel.patch)
                    .BindTo(this, v => v.BttnPatch.Value)
                    .DisposeWith(disposables);
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
