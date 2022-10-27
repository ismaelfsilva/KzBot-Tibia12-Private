using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KzBot.Objects
{
    public static class Battlelist
    {

        public static uint BattlelistCollectionAddress()
        {
            return WinApi.ReadOffsetUInt32(Globals.Handle, Addresses.Player.Pointer, Addresses.Battlelist.Pointer);
        }

        public static List<Creature> getCreatures()
        {
            List<Creature> creatures = new List<Creature>();
            List<uint> creatureList = Util.QtCollectionHelper.Read(BattlelistCollectionAddress());

            foreach (uint creaturePointer in creatureList)
            {
                if (creaturePointer < Addresses.Player.Pointer)
                    continue;

                uint creatureAddress = WinApi.ReadUInt32(Globals.Handle, creaturePointer + Addresses.Battlelist.creaturePointer);

                Creature cr = new Creature(creatureAddress);
                creatures.Add(cr);
            }

            return creatures;
        }

        public static List<Creature> getCreaturesOnScreen()
        {
            List<Creature> creatures = new List<Creature>();
            List<uint> creatureList = Util.QtCollectionHelper.Read(BattlelistCollectionAddress());

            Position playerPos = Player.Position;

            foreach (uint creaturePointer in creatureList)
            {
                if (creaturePointer < Addresses.Player.Pointer)
                    continue;

                uint creatureAddress = WinApi.ReadUInt32(Globals.Handle, creaturePointer + Addresses.Battlelist.creaturePointer);

                Creature cr = new Creature(creatureAddress);
                if (!cr.isNear || cr.Z != playerPos.Z)
                    continue;

                creatures.Add(cr);
            }

            return creatures;
        }
    }
}
