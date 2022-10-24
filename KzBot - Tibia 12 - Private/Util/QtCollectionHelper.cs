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
            List<uint> list = new List<uint>();

            readChildrenRecursive(ref list, listAddress);

            return list;
        }
        public static List<uint> Read(uint listAddress, uint objectAddress)
        {
            List<uint> list = new List<uint>();

            readChildrenRecursive(ref list, listAddress, objectAddress);

            return list;
        }

        public static void readChildrenRecursive(ref List<uint> list, long parent = 0, uint objectAddress = 0x0)
        {
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    uint cTreeAddress = WinApi.ReadUInt32(Globals.Handle, parent + 0x4 * i);

                    if (cTreeAddress == 0x0)
                        continue;

                    if (list.Exists(c => c == cTreeAddress))
                        continue;

                    list.Add((uint)cTreeAddress);

                    readChildrenRecursive(ref list, cTreeAddress);
                }
                catch (Exception e)
                { }


                
            }
        }
    }
}
