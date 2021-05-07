using Akavache;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace smnclauncher.ViewModels
{
    public class CredentialsViewModel : ReactiveObject
    {
        [Reactive] public string Username { get; set; }

        [Reactive] public string Password { get; set; }

        public CredentialsViewModel()
        {
            (Username, Password) = BlobCache
                .Secure
                .GetLoginAsync()
                .Select(info => (info.UserName, info.Password))
                .Catch(Observable.Return(("", "")))
                .Wait();

            this.WhenAnyValue(vm => vm.Username, vm => vm.Password)
                .Do(v => BlobCache.Secure.SaveLogin(v.Item1, v.Item2))
                .Subscribe();
        }
    }
}
 