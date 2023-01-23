using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUB.Classes
{
    public class Server
    {
        public string id { get; set; }
        public string name { get; set; } = string.Empty;
        public string version { get; set; } = string.Empty;
        public string path { get; set; } = string.Empty;
        public string website { get; set; } = string.Empty;
        public string loginServer { get; set; } = string.Empty;
        public int staminaRecoveryDelay { get; set; } = 6;
        public int autoLootId { get; set; } = -1;

        public Server()
        {
            id = Guid.NewGuid().ToString();
        }
    }
}
