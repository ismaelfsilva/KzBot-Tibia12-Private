using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KzBot.Objects
{
    public static class Player
    {


        public static int TargetId
        {
            get
            {
                return WinApi.ReadOffsetInt32(Globals.Handle, Addresses.Player.Pointer, Addresses.Player.redSquare);
            }
        }

        public static bool isAttacking
        {
            get
            {
                return TargetId != 0x0;
            }
        }

        public static bool isLoggedIn
        {
            get
            {
                System.Text.StringBuilder text = new System.Text.StringBuilder();
                WinApi.GetWindowText(Globals.Process.MainWindowHandle, text, 10);
                return text.ToString().StartsWith("Tibia -");
            }
        }

        public static Creature Creature
        {
            get
            {
                return new Creature((uint)WinApi.ReadOffsetInt32(Globals.Handle, Addresses.Player.Pointer, Addresses.Player.creaturePointer));
            }
        }

        public static Position Position
        {
            get
            {
                return Creature.Position;
            }
        }

        public static int Health
        {
            get
            {
                return WinApi.ReadOffsetInt16(Globals.Handle, Addresses.Player.Pointer, Addresses.Player.hp);
            }
        }

        public static int HealthMax
        {
            get
            {
                return WinApi.ReadOffsetInt16(Globals.Handle, Addresses.Player.Pointer, Addresses.Player.hpMax);
            }
        }

        public static int Mana
        {
            get
            {
                return WinApi.ReadOffsetInt16(Globals.Handle, Addresses.Player.Pointer, Addresses.Player.mana);
            }
        }

        public static int ManaMax
        {
            get
            {
                return WinApi.ReadOffsetInt16(Globals.Handle, Addresses.Player.Pointer, Addresses.Player.manaMax);
            }
        }

        public static bool isAlive()
        {
            return Health > 0;
        }

        public static int Level
        {
            get
            {
                return WinApi.ReadOffsetInt16(Globals.Handle, Addresses.Player.Pointer, Addresses.Player.level);
            }
        }

        public static int LevelPc
        {
            get
            {
                return WinApi.ReadOffsetByte(Globals.Handle, Addresses.Player.Pointer, Addresses.Player.experiencePc);
            }
        }

        public static int Experience
        {
            get
            {
                return WinApi.ReadOffsetInt32(Globals.Handle, Addresses.Player.Pointer, Addresses.Player.experience);
            }
        }

        public static TimeSpan Stamina
        {
            get
            {
                return new TimeSpan(0, 0, WinApi.ReadOffsetInt32(Globals.Handle, Addresses.Player.Pointer, Addresses.Player.stamina));
            }
        }

        public static int Cap
        {
            get
            {
                return WinApi.ReadOffsetInt32(Globals.Handle, Addresses.Player.Pointer, Addresses.Player.cap) / 100;
            }
        }

        public static int Soul
        {
            get
            {
                return WinApi.ReadOffsetInt16(Globals.Handle, Addresses.Player.Pointer, Addresses.Player.soul);
            }
        }

        public static int GotoX
        {
            get
            {
                return WinApi.ReadOffsetInt32(Globals.Handle, Addresses.Player.Pointer, Addresses.Player.gotoX);
            }
            set
            {
                WinApi.WriteOffsetInt(Globals.Handle, Addresses.Player.Pointer, Addresses.Player.gotoX, value);
            }
        }

        public static int GotoY
        {
            get
            {
                return WinApi.ReadOffsetInt32(Globals.Handle, Addresses.Player.Pointer, Addresses.Player.gotoY);
            }
            set
            {
                WinApi.WriteOffsetInt(Globals.Handle, Addresses.Player.Pointer, Addresses.Player.gotoY, value);
            }
        }

        public static int GotoZ
        {
            get
            {
                return WinApi.ReadOffsetInt32(Globals.Handle, Addresses.Player.Pointer, Addresses.Player.gotoZ);
            }
            set
            {
                WinApi.WriteOffsetInt(Globals.Handle, Addresses.Player.Pointer, Addresses.Player.gotoZ, value);
            }
        }

        public static bool isWalking
        {
            get
            {
                return WinApi.ReadOffsetInt32(Globals.Handle, Addresses.Player.Pointer, Addresses.Player.isWalking) == 1;
            }
            set
            {
                WinApi.WriteOffsetInt(Globals.Handle, Addresses.Player.Pointer, Addresses.Player.isWalking, Convert.ToInt32(value));
            }
        }

        public static void Goto(int X, int Y, int Z)
        {
            GotoX = X;
            GotoY = Y;
            GotoZ = Z;
            isWalking = true;
        }
        public static void Goto(Position pos)
        {
            GotoX = pos.X;
            GotoY = pos.Y;
            GotoZ = pos.Z;
            isWalking = true;
        }
    }
}
