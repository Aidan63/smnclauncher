using Gameloop.Vdf;
using Gameloop.Vdf.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace smnclauncher.Models
{
    public class SteamInstall : IInstall
    {
        private readonly string directory;

        public SteamInstall(string directory)
        {
            this.directory = directory;
        }

        public string Directory()
        {
            return directory;
        }
    }
}
