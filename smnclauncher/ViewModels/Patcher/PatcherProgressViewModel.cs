using Google.Apis.Services;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using smnclauncher.Utils;
using Google.Apis.Drive.v3;

namespace smnclauncher.ViewModels.Patcher
{
    public class PatcherProgressViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
    {
        public string? UrlPathSegment => "progress";

        public ViewModelActivator Activator { get; }

        public IScreen HostScreen { get; }

        public readonly string itemID;

        public readonly string installDirectory;

        public readonly ReactiveCommand<Unit, double> patch;

        public PatcherProgressViewModel(string itemID, string installDirectory, IScreen? screen = null)
        {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();
            Activator  = new ViewModelActivator();

            patch = ReactiveCommand.CreateFromObservable(DownloadGoogleDriveFile);

            this.itemID           = itemID;
            this.installDirectory = installDirectory;
            this.WhenActivated(disposable =>
            {
                patch
                    .ThrownExceptions
                    .Subscribe(ex => HostScreen.Router.Navigate.Execute(new PatcherResultViewModel(Result.Failure, ex.Message, HostScreen)).Subscribe())
                    .DisposeWith(disposable);

                patch
                    .Where(v => v >= 100)
                    .Subscribe(ex => HostScreen.Router.Navigate.Execute(new PatcherResultViewModel(Result.Success, "The requested patch was downloaded and applied.", HostScreen)).Subscribe())
                    .DisposeWith(disposable);

                patch
                    .Execute()
                    .Subscribe()
                    .DisposeWith(disposable);
            });
        }

        private IObservable<double> DownloadGoogleDriveFile()
        {
            return Observable
                .Create<double>(async (obs, ct) =>
                {
                    // Since the google drive files are public we can use an API key instead of needing OAuth.
                    var client = new BaseClientService.Initializer
                    {
                        ApiKey          = ReadSecrets(),
                        ApplicationName = "smnclauncher"
                    };

                    // We need to manually request the size field else it will be null in the result.
                    var service = new DriveService(client);
                    var request = service.Files.Get(itemID);
                    request.Fields = "size";

                    var file = await request.ExecuteAsync(ct);
                    var size = file.Size ?? 1;

                    // Pipe the download progress into the observer.
                    // Download progress will account for the first 50% of the normalise progress ticker.
                    request.MediaDownloader.ProgressChanged += progress =>
                    {
                        if (progress.Status == Google.Apis.Download.DownloadStatus.Downloading)
                        {
                            obs.OnNext(0.5 * progress.BytesDownloaded / size);
                        }
                    };

                    // Copy the file into a file stream for extraction later.
                    var tmpDirs = new List<string>();
                    var tmpFile = Path.Combine(installDirectory, "tmp.exe");
                    var output  = File.OpenWrite(tmpFile);
                    var result  = await request.DownloadAsync(output, ct);
                    output.Close();

                    switch (result.Status)
                    {
                        case Google.Apis.Download.DownloadStatus.Completed:
                            var baseFolder = installDirectory;
                            var patchDir   = Path.Combine(baseFolder, "_patch");
                            var extractor  = new SevenZip.SevenZipExtractor(tmpFile);

                            extractor.ExtractArchive(baseFolder);

                            // Add any folder entry into a list so it can be cleaned up later.
                            foreach (var entry in extractor.ArchiveFileData)
                            {
                                if (entry.IsDirectory)
                                {
                                    tmpDirs.Add(Path.Combine(baseFolder, entry.FileName));
                                }
                            }

                            // Run the patcher
                            Utils.Patcher
                                .Patch(patchDir, baseFolder)
                                .Select(v => 0.5 + (v * 0.5))
                                .Subscribe(obs);
                            break;
                        case Google.Apis.Download.DownloadStatus.Failed:
                        case Google.Apis.Download.DownloadStatus.NotStarted:
                            obs.OnError(result.Exception);
                            break;
                    }

                    return Disposable.Create(() =>
                    {
                        if (File.Exists(tmpFile))
                        {
                            File.Delete(tmpFile);
                        }

                        foreach (var dir in tmpDirs)
                        {
                            if (Directory.Exists(dir))
                            {
                                Directory.Delete(dir, true);
                            }
                        }
                    });
                })
                .Select(norm => norm * 100)
                .SubscribeOn(RxApp.TaskpoolScheduler);
        }

        private static string ReadSecrets()
        {
            if (!File.Exists("Resources/secrets.txt"))
            {
                return "API-KEY";
            }

            return File.ReadAllText("Resources/secrets.txt");
        }
    }
}
