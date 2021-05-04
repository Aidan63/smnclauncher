using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace smnclauncher.Models
{
    public class PatchEntry
    {
        public string Key { get; set; } = "";

        public string Value { get; set; } = "";

        public string OldHash { get; set; } = "";

        public string NewHash { get; set; } = "";
    }
}
