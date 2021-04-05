using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using smnclauncher.ViewModels;
using System.Reactive.Disposables;

namespace smnclauncher.Views
{
    public class AuthenticateView : ReactiveUserControl<AuthenticateViewModel>
    {
        public Button BttnLogin => this.FindControl<Button>("BttnLogin");

        public TextBox InputUsername => this.FindControl<TextBox>("InputUsername");

        public TextBox InputPassword => this.FindControl<TextBox>("InputPassword");

        public AuthenticateView()
        {
            this.InitializeComponent();
            this.WhenActivated(disposables =>
            {
                this.BindCommand(ViewModel, vm => vm.login, v => v.BttnLogin, this.WhenAnyValue(v => v.ViewModel.Username, v => v.ViewModel.Password))
                    .DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.Username, v => v.InputUsername.Text)
                    .DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.Password, v => v.InputPassword.Text)
                    .DisposeWith(disposables);
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
