using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KzBot.Addresses
{
    public static class Client
    {
        public static uint[] clientTime;
        public static uint[] lastWalkTime;

        public static uint attackCooldown = 0x0;
        public static uint healCooldown = 0x0;
        public static uint supportCooldown = 0x0;
    }
}
