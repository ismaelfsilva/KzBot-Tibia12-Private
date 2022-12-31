using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUB.Classes
{
    public class Client
    {
        public string id { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string file { get; set; } = string.Empty;

        public Client()
        {
            id = Guid.NewGuid().ToString();
        }
    }
}
