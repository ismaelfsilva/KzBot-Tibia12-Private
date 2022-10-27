using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KzBot.Util
{
    public static class QtCollectionHelper
    {

        public static List<uint> Read(uint listAddress)
        {
            Dictionary<uint, uint> list = new Dictionary<uint, uint>();

            readChildrenRecursive(ref list, listAddress);

            return list.Keys.ToList();
        }
        public static List<uint> Read(uint listAddress, uint objectAddress)
        {
            Dictionary<uint, uint> list = new Dictionary<uint, uint>();

            readChildrenRecursive(ref list, listAddress, objectAddress);

            return list.Keys.ToList();
        }

        public static void readChildrenRecursive(ref Dictionary<uint, uint> list, long parent = 0, uint objectAddress = 0x0)
        {
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    uint cTreeAddress = WinApi.ReadUInt32(Globals.Handle, parent + 0x4 * i);

                    if (cTreeAddress <= 0x0)
                        continue;

                    if (list.ContainsKey(cTreeAddress))
                        continue;

                    list.Add(cTreeAddress, cTreeAddress);

                    readChildrenRecursive(ref list, cTreeAddress);
                }
                catch (Exception e)
                { }                                
            }
        }
    }
}
