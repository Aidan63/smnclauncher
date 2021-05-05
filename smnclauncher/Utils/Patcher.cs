using Newtonsoft.Json;
using smnclauncher.Models;
using System;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Security.Cryptography;

namespace smnclauncher.Utils
{
    static class Patcher
    {
        public static IObservable<double> Patch(string patchDirectory, string gameDirectory)
        {
            var update  = JsonConvert.DeserializeObject<PatchFile>(File.ReadAllText(Path.Combine(patchDirectory, "patch.json")));
            var changes = update.Add.Count + update.Delete.Count + update.Patch.Count;

            var obsAdd = update
                .Add
                .ToObservable()
                .Do(entry =>
                {
                    var src = patchDirectory + entry.Key;
                    var dst = gameDirectory + entry.Value;

                    if (File.Exists(dst))
                    {
                        File.Delete(dst);
                    }

                    File.Copy(src, dst);
                })
                .Select(_ => Unit.Default);

            var obsDelete = update
                .Delete
                .ToObservable()
                .Do(entry =>
                {
                    var dst = gameDirectory + entry;

                    if (File.Exists(dst))
                    {
                        File.Delete(dst);
                    }
                })
                .Select(_ => Unit.Default);

            var obsPatch = update
                .Patch
                .ToObservable()
                .Do(entry =>
                {
                    var src = patchDirectory + entry.Key;
                    var dst = gameDirectory + entry.Value;

                    switch (HashFile(dst))
                    {
                        case string s when s == entry.NewHash: return;
                        case string s when s != entry.OldHash: throw new Exception($"{src} hash does not match the expected value");
                        case string:
                            var tempPatch    = dst + ".tmp";

                            using (var baseFile = File.OpenRead(dst))
                            using (var deltaFile = File.OpenRead(src))
                            using (var tempFile = File.OpenWrite(tempPatch))
                            {
                                OvermindRsync.RsyncInterface.PatchFile(baseFile, deltaFile, tempFile);
                            }

                            if (File.Exists(dst))
                            {
                                File.Delete(dst);
                            }

                            if (HashFile(tempPatch) != entry.NewHash)
                            {
                                throw new Exception("Patched files hash does not match the expected hash");
                            }

                            File.Move(tempPatch, dst);
                            break;
                    }
                })
                .Select(_ => Unit.Default);

            return Observable
                .Merge(obsAdd, obsDelete, obsPatch)
                .Scan(0d, (acc, _) => acc + 1)
                .Select(acc => acc / changes);
        }

        public static string HashFile(string path)
        {
            using var provider = new SHA1CryptoServiceProvider();
            using var stream   = File.OpenRead(path);

            return BitConverter.ToString(provider.ComputeHash(stream)).Replace("-", string.Empty);
        }
    }
}
