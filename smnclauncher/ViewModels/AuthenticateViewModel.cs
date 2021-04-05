using Artalk.Xmpp.Client;
using Artalk.Xmpp.Im;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace smnclauncher.ViewModels
{
    public class AuthenticateViewModel : ReactiveObject
    {
        [Reactive] public string Username { get; set; }

        [Reactive] public string Password { get; set; }

        public readonly ReactiveCommand<(string username, string password), bool> login;

        public AuthenticateViewModel()
        {
            Username = "";
            Password = "";
            login    = ReactiveCommand.CreateFromObservable<(string, string), bool>(AreCredentialsValid);
        }

        static IObservable<bool> AreCredentialsValid((string username, string password) credentials) =>
            Observable.Create<bool>(obs => {
                //using (var client = new XmppClient("xmpp.smnc.lennardf1989.com", credentials.username, credentials.password))
                //{
                //    try
                //    {
                //        client.Connect();

                //        obs.OnNext(client.Connected);
                //    }
                //    catch (Exception e)
                //    {
                //        obs.OnError(e);
                //    }
                //}

                obs.OnCompleted();

                return Disposable.Empty;
            });
    }
}
