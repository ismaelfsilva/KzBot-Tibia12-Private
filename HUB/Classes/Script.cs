using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUB.Classes
{
    public class Script
    {
        public string id { get; set; }
        public string name { get; set; } = string.Empty;
        public string client { get; set; } = string.Empty;
        public string path { get; set; } = string.Empty;

        public Script()
        {
            id = Guid.NewGuid().ToString();
        }
    }
}
