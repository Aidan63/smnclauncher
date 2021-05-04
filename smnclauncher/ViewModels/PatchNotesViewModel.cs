using HtmlAgilityPack;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReverseMarkdown;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace smnclauncher.ViewModels
{
    public class PatchNotesViewModel : ReactiveObject, IActivatableViewModel
    {
        public ViewModelActivator Activator { get; }

        [ObservableAsProperty] public string PatchNotes { get; } = "";

        public readonly ReactiveCommand<Unit, string> fetchPatchNotes;

        public PatchNotesViewModel()
        {
            Activator = new ViewModelActivator();

            fetchPatchNotes = ReactiveCommand.CreateFromObservable(GetPatchNotes);

            this.WhenActivated(disposables =>
            {
                fetchPatchNotes
                    .ToPropertyEx(this, vm => vm.PatchNotes)
                    .DisposeWith(disposables);

                fetchPatchNotes
                    .Execute()
                    .Subscribe()
                    .DisposeWith(disposables);
            });
        }

        private static IObservable<string> GetPatchNotes() =>
            Observable
                .Create<Stream>(async obs =>
                {
                    // Download the websites html
                    using var client = new HttpClient();

                    var response = await client.GetStreamAsync("https://reboot.smnc.lennardf1989.com/rulechanges.htm");

                    obs.OnNext(response);

                    return Disposable.Empty;
                })
                .Select(stream =>
                {
                    // Attempt to extract the patch notes from the html
                    var htmlDoc = new HtmlDocument();

                    htmlDoc.Load(stream);

                    return htmlDoc
                        .DocumentNode
                        .Descendants("div")
                        .Where(node => node.HasClass("overlay"))
                        .First()
                        .Descendants("div")
                        .First()
                        .WriteContentTo();
                })
                .Select(html =>
                {
                    // Convert the patch notes html to markdown
                    var converter = new Converter();

                    return converter.Convert(html);
                })
                .Catch(Observable.Return(""));
    }
}
