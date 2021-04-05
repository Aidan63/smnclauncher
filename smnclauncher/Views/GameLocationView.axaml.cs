using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using smnclauncher.ViewModels;
using System.Reactive.Disposables;

namespace smnclauncher.Views
{
    public class GameLocationView : ReactiveUserControl<GameLocationViewModel>
    {
        public Button BttnAutoLocation => this.FindControl<Button>("BttnAutoDetect");

        public TextBox TxtGameDirectory => this.FindControl<TextBox>("TxtGameDirectory");

        public GameLocationView()
        {
            this.InitializeComponent();
            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, vm => vm.Directory, v => v.TxtGameDirectory.Text)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.findGameDirectory, v => v.BttnAutoLocation)
                    .DisposeWith(disposables);
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
