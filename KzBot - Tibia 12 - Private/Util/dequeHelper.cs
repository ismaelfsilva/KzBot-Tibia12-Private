using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KzBot.Util;

namespace KzBot.Util
{
    public static class dequeHelper
    {
        public static List<uint> iterateDeque(uint parent, uint dequeStart, int dequeSize, int firstIndex)
        {
            mapNodesAddrs = new List<uint>();
            left = 0x0;
            parent = 0x4;
            right = 0x8;
            tries = 0;
            maxTries = 32;

            List<uint> childs = new List<uint>();
            for (int i = 0; i <= 0x1c; i += 4) //readind the dynamic size array, 0x1c elements
            {
                for (int j = 0; j <= 0x0c; j += 4) //reading the fixed size array of 4 elements 0x0, 0x4, 0x8 and 0xC
                {
                    //read deque position (i, j)
                    uint childrenAddr = WinApi.ReadUInt32(Globals.Handle, dequeStart + i);
                    childrenAddr = WinApi.ReadUInt32(Globals.Handle, childrenAddr + j);
                    childs.Add(childrenAddr);
                }
            }

            List<uint> validChilds = getOnlyValidAddresses(childs, dequeSize, firstIndex);
            return validChilds;

        }

        private static List<uint> getOnlyValidAddresses(List<uint> childs, int dequeSize, int firstIndex)
        {
            List<uint> validChilds = new List<uint>();
            int itemsRead = 0;
            int maxTries = 32;
            for (int i = 0; (i < childs.Count) && (itemsRead < dequeSize) && (i < maxTries); i++)
            {
                uint adr = childs[(firstIndex + i) % childs.Count];
                validChilds.Add(adr);
                itemsRead++;
            }

            return validChilds;
        }

        public static List<uint> iterateMap(uint rootPtr)
        {

            //Console.WriteLine("Rootnode address: " + (Convert.ToUInt32(rootPtr).ToString("X")));
            mapNodesAddrs.Clear();
            rootPtr = rootPtr;
            transverseMap(rootPtr + parent);
            return mapNodesAddrs;

        }

        private static List<uint> mapNodesAddrs = new List<uint>();
        private static uint rootPtr;
        private static uint left = 0x0;
        private static uint parent = 0x4;
        private static uint right = 0x8;
        private static int tries = 0;
        private static int maxTries = 32;

        private static void transverseMap(uint nodeAddress)
        {
            //Console.WriteLine("Tries: " + tries);
            //Console.WriteLine("Node address actual: " + (Convert.ToUInt32(nodeAddress).ToString("X")));
            if (nodeAddress == rootPtr || nodeAddress <= 0 || nodeAddress > 0x9999999 || tries >= maxTries)
                return;

            transverseMap(WinApi.ReadUInt32(Globals.Handle, nodeAddress + left));
            tries++;
            mapNodesAddrs.Add(nodeAddress);
            transverseMap(WinApi.ReadUInt32(Globals.Handle, nodeAddress + right));

        }
    }
}
