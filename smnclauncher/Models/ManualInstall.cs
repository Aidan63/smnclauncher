using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace smnclauncher.Models
{
    public class ManualInstall : IInstall
    {
        private readonly string directory;

        public ManualInstall(string directory)
        {
            this.directory = directory;
        }

        public string Directory()
        {
            return directory;
        }

        public IObservable<Unit> Launch()
        {
            throw new NotImplementedException();
        }
    }
}
