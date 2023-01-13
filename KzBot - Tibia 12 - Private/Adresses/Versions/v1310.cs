using System;
using System.Diagnostics;

namespace KzBot.Addresses
{
    public partial class Version
    {
        public static void v1310(Process p) // 13.10
        {
            uint BaseAddress = Convert.ToUInt32(p.MainModule.BaseAddress.ToInt32());

            Battlelist.Pointer = new uint[] { 0x158, 0x8};   // UPDATED
            Battlelist.creaturePointer = 0x14;  // UPDATED

            Creature.id = 0x10; // UPDATED
            Creature.X = 0x24; // UPDATED
            Creature.Y = 0x28; // UPDATED
            Creature.Z = 0x2C; // UPDATED
            Creature.name = 0x18; // + 10 Name; + 04 Length // UPDATED
            Creature.type = 0x30; // UPDATED
            Creature.guild = 0x38; // UPDATED
            Creature.skull = 0x3B; // UPDATED
            Creature.direction = 0x54; // UPDATED
            Creature.speed = 0x58; // UPDATED
            Creature.hpPc = 0x84; // UPDATED
            Creature.isNear = 0xC8; // UPDATED

            Player.Pointer = 0x10B6A0C + BaseAddress; // UPDATED
            Player.creaturePointer = 0x290; // UPDATED
            Player.redSquare = new uint[] { 0x158, 0x1C }; // UPDATED
            Player.greenSquare = new uint[] { 0x158, 0x18 }; // UPDATED
            Player.mouseHoverCreature = new uint[] { 0x158, 0x14 }; // UPDATED

            Client.clientTime = new uint[] { Player.creaturePointer, 0x64, 0x50 }; // UPDATED

            Player.gotoX = new uint[] { 0x4FC, 0xB8 }; // UPDATED
            Player.gotoY = new uint[] { 0x4FC, 0xBC }; // UPDATED
            Player.gotoZ = new uint[] { 0x4FC, 0xC0 }; // UPDATED
            Player.isWalking = new uint[] { 0x4FC, 0xA8 }; // UPDATED

            Player.hp = new uint[] { 0x6C, 0xC }; // UPDATED
            Player.hpMax = new uint[] { 0x6C, 0x10 }; // UPDATED
            Player.cap = new uint[] { 0x6C, 0x14 }; // UPDATED
            Player.experience = new uint[] { 0x6C, 0x20 }; // UPDATED
            Player.level = new uint[] { 0x6C, 0x28 }; // UPDATED
            Player.experiencePc = new uint[] { 0x6C, 0x2A }; // UPDATED
            Player.mana = new uint[] { 0x6C, 0x58 }; // UPDATED
            Player.manaMax = new uint[] { 0x6C, 0x5C }; // UPDATED
            Player.soul = new uint[] { 0x6C, 0x60 }; // UPDATED
            Player.stamina = new uint[] { 0x6C, 0x78 }; // UPDATED

            Player.items = new uint[] { 0xB0, 0x18, 0x20 }; // UPDATED
            Player.cooldowns = new uint[] { 0x74, 0x18 }; // UPDATED
        }
    }
}
