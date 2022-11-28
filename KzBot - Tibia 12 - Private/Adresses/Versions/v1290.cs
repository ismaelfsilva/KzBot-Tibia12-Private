using System;
using System.Diagnostics;

namespace KzBot.Addresses
{
    public partial class Version
    {
        public static void v1290(Process p) // 12.90.12248
        {
            uint BaseAddress = Convert.ToUInt32(p.MainModule.BaseAddress.ToInt32());

            Battlelist.Pointer = new uint[] { 0x138, 0x8};   //
            Battlelist.creaturePointer = 0x14;  //

            Client.clientTime = new uint[] { 0x260, 0x64, 0x50 };

            Creature.id = 0x10;
            Creature.X = 0x24;
            Creature.Y = 0x28;
            Creature.Z = 0x2C;
            Creature.name = 0x18; // + 10 Name; + 04 Length
            Creature.type = 0x30;
            Creature.guild = 0x38;
            Creature.skull = 0x3B;
            Creature.direction = 0x54;
            Creature.speed = 0x58;
            Creature.hpPc = 0x84;
            Creature.isNear = 0xC8;

            Player.Pointer = 0xF1EB7C + BaseAddress;
            Player.creaturePointer = 0x260;
            Player.redSquare = new uint[] { 0x138, 0x1C };
            Player.greenSquare = new uint[] { 0x138, 0x18 };
            Player.mouseHoverCreature = new uint[] { 0x138, 0x14 };
            Player.loggedIn = 0x2D0;

            Player.gotoX = new uint[] { 0x494, 0xB8 };
            Player.gotoY = new uint[] { 0x494, 0xBC };
            Player.gotoZ = new uint[] { 0x494, 0xC0 };
            Player.isWalking = new uint[] { 0x494, 0xA8 };

            Player.hp = new uint[] { 0x6C, 0xC };
            Player.hpMax = new uint[] { 0x6C, 0xE };
            Player.cap = new uint[] { 0x6C, 0x10 };
            Player.experience = new uint[] { 0x6C, 0x20 };
            Player.level = new uint[] { 0x6C, 0x28 };
            Player.experiencePc = new uint[] { 0x6C, 0x2A };
            Player.mana = new uint[] { 0x6C, 0x58 };
            Player.manaMax = new uint[] { 0x6C, 0x5A };
            Player.soul = new uint[] { 0x6C, 0x5C };
            Player.stamina = new uint[] { 0x6C, 0x70 };

            Player.items = new uint[] { 0xA0, 0x18, 0x20 };
            Player.cooldowns = new uint[] { 0x74, 0x18 };
        }
    }
}
