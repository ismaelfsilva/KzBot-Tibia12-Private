using System;
using System.Diagnostics;

namespace KzBot.Addresses
{
    public partial class Version
    {
        public static void v1312(Process p) // 13.102
        {
            uint BaseAddress = Convert.ToUInt32(p.MainModule.BaseAddress.ToInt32());

            Battlelist.Pointer = new uint[] { 0x158, 0x8};   // UPDATED
            Battlelist.creaturePointer = 0x14;  // UPDATED

            Creature.id = 0x10; // UPDATED
            Creature.X = 0x24; // UPDATED
            Creature.Y = 0x28; // UPDATED
            Creature.Z = 0x2C; // UPDATED
            Creature.name = 0x18; // + 10 Name; + 04 Length // UPDATED
            Creature.type = 0x30; // 
            Creature.guild = 0x38; // 
            Creature.skull = 0x3B; // 
            Creature.direction = 0x54; // 
            Creature.speed = 0x58; // 
            Creature.hpPc = 0x84; // 
            Creature.isNear = 0xC8; // 

            Player.Pointer = 0x10BAD24 + BaseAddress; // UPDATED
            Player.creaturePointer = 0x290; // 
            Player.redSquare = new uint[] { 0x158, 0x1C }; // 
            Player.greenSquare = new uint[] { 0x158, 0x18 }; // 
            Player.mouseHoverCreature = new uint[] { 0x158, 0x14 }; // 

            Client.clientTime = new uint[] { Player.creaturePointer, 0x64, 0x50 }; // 
            Client.lastWalkTime = new uint[] { Player.creaturePointer, 0x64, 0x58 }; // 

            Player.gotoX = new uint[] { 0x4FC, 0xB8 }; // 
            Player.gotoY = new uint[] { 0x4FC, 0xBC }; // 
            Player.gotoZ = new uint[] { 0x4FC, 0xC0 }; // 
            Player.isWalking = new uint[] { 0x4FC, 0xA8 }; // 

            Player.hp = new uint[] { 0x6C, 0xC }; // 
            Player.hpMax = new uint[] { 0x6C, 0x10 }; // 
            Player.cap = new uint[] { 0x6C, 0x14 }; // 
            Player.experience = new uint[] { 0x6C, 0x20 }; // 
            Player.level = new uint[] { 0x6C, 0x28 }; // 
            Player.experiencePc = new uint[] { 0x6C, 0x2A }; // 
            Player.mana = new uint[] { 0x6C, 0x58 }; // 
            Player.manaMax = new uint[] { 0x6C, 0x5C }; // 
            Player.soul = new uint[] { 0x6C, 0x60 }; // 
            Player.stamina = new uint[] { 0x6C, 0x78 }; // 

            Player.items = new uint[] { 0xB0, 0x18, 0x20 }; // 
            Player.cooldowns = new uint[] { 0x74, 0x18 }; // 
        }
    }
}
