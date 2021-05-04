using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace smnclauncher.Models
{
    public class PatchFile
    {
        public List<string> Delete { get; set; } = new List<string>();

        public List<PatchEntry> Patch { get; set; } = new List<PatchEntry>();

        public List<PatchEntry> Add { get; set; } = new List<PatchEntry>();
    }
}
