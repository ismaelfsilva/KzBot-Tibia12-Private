using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KzBot.Objects
{
    public class Creature
    {
        private readonly uint addr;

        public Creature(uint address)
        {
            addr = address;
        }

        public int Id
        {
            get
            {
                return WinApi.ReadInt32(Globals.Handle, addr + Addresses.Creature.id);
            }
        }

        public int X
        {
            get
            {
                return WinApi.ReadInt32(Globals.Handle, addr + Addresses.Creature.X);
            }
        }

        public int Y
        {
            get
            {
                return WinApi.ReadInt32(Globals.Handle, addr + Addresses.Creature.Y);
            }
        }

        public int Z
        {
            get
            {
                return WinApi.ReadInt32(Globals.Handle, addr + Addresses.Creature.Z);
            }
        }

        public Position Position
        {
            get
            {
                return new Position() { X = X, Y=Y, Z = Z };
            }
        }

        public uint Address
        {
            get
            {
                return addr;
            }
        }

        public string Name
        {
            get
            {
                return WinApi.ReadOffsetString(Globals.Handle, addr + Addresses.Creature.name, 0x10, (uint)NameLen);
            }
        }

        public int NameLen
        {
            get
            {
                return WinApi.ReadOffsetInt32(Globals.Handle, addr + Addresses.Creature.name, 0x4);
            }
        }

        public CreatureType Type
        {
            get
            {
                return (CreatureType)WinApi.ReadByte(Globals.Handle, addr + Addresses.Creature.type);
            }
        }

        public GuildEmblem Guild
        {
            get
            {
                return (GuildEmblem)WinApi.ReadByte(Globals.Handle, addr + Addresses.Creature.guild);
            }
        }

        public PlayerSkulls Skull
        {
            get
            {
                return (PlayerSkulls)WinApi.ReadByte(Globals.Handle, addr + Addresses.Creature.skull);
            }
        }

        public bool isNear
        {
            get
            {
                return WinApi.ReadByte(Globals.Handle, addr + Addresses.Creature.isNear) == 1;
            }
        }

        public int Speed
        {
            get
            {
                return WinApi.ReadInt16(Globals.Handle, addr + Addresses.Creature.speed);
            }
        }

        public int Direction
        {
            get
            {
                return WinApi.ReadByte(Globals.Handle, addr + Addresses.Creature.direction);
            }
        }

        public int HealthPc
        {
            get
            {
                return WinApi.ReadByte(Globals.Handle, addr + Addresses.Creature.hpPc);
            }
        }
    }
}
