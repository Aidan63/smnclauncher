using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using smnclauncher.ViewModels.Patcher;
using System.Reactive.Disposables;

namespace smnclauncher.Views.Patcher
{
    public class PatcherURLInputView : ReactiveUserControl<PatcherURLInputViewModel>
    {
        public Button BttnPatch => this.FindControl<Button>("BttnPatch");

        public TextBox InputUrl => this.FindControl<TextBox>("InputUrl");

        public PatcherURLInputView()
        {
            this.InitializeComponent();
            this.WhenActivated(disposables =>
            {
                this.Bind(ViewModel, vm => vm.DriveUrl, v => v.InputUrl.Text)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.startPatch, v => v.BttnPatch, this.WhenAnyValue(v => v.ViewModel.ItemID))
                    .DisposeWith(disposables);
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
