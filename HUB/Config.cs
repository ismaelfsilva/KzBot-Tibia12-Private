using HUB.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUB
{
    public class Config
    {
        public List<Server> Servers = new List<Server>();
        public List<Client> Clients = new List<Client>();
        public List<Script> Scripts = new List<Script>();
        public List<LevelingConnection> LevelingConnections = new List<LevelingConnection>();
    }
}
