using System;

namespace KzBot
{

    public enum FightMode
    {
        OFFENSIVE = 0,
        BALANCED = 1,
        DEFENSIVE = 2
    }
    public enum CooldownGroup
    {
        Attack = 1,
        Heal = 2,
        Support = 3
    }

    public enum CreatureType
    {
        Player = 0,
        Monster,
        Npc,
        SummonOwn,
        SummonOther,
        Unknown = 0xFF
    };

    public enum GuildEmblem
    {
        None = 0,
        Green,
        Red,
        Blue,
        Member,
        Other
    };

    public enum PlayerSkulls
    {
        None = 0,
        Yellow,
        Green,
        White,
        Red,
        Black,
        Orange
    };

    public enum Direction
    {
        NULL = -1,
        NORTH = 0,
        EAST = 1,
        SOUTH = 2,
        WEST = 3,
        NORTH_EAST = 4,
		SOUTH_EAST = 5,
		SOUTH_WEST = 6,
		NORTH_WEST = 7
    }

    public enum KeyModifiers
    {
        KeyboardNoModifier = 0,
        KeyboardCtrlModifier = 1,
        KeyboardAltModifier = 2,
        KeyboardShiftModifier = 4
    }

    public enum PlayerStates
    {
        None = 0,
        Poisoned = 1,
        Burning = 2,
        Electrified = 4,
        Drunk = 8,
        ManaShield = 16,
        Paralysed = 32,
        Hasted = 64,
        InBattle = 128,
        Drowning = 256,
        Freezing = 512,
        Dazzled = 1024,
        Cursed = 2048,
        Buffed = 4096,
        CannotLogoutOrEnterProtectionZone = 8192,
        WithinPZ = 16384,
        Bleeding = 32768,
    }

    public struct Position
    {	
        public int X, Y, Z;

		public int distanceTo(Position p)
        {
            int distX = (p.X) - (X);
            int distY = (p.Y) - (Y);

            double result = Math.Sqrt(Math.Pow(distX, 2) + Math.Pow(distY, 2));
            return (int)result;
        }


        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var objectToCompareWith = (Position)obj;

            return objectToCompareWith.X == X && objectToCompareWith.Y == Y &&
                   objectToCompareWith.Z == Z;

        }

        public override string ToString()
        {
            return String.Format("x: {0} y: {1} z: {2}", X, Y, Z);
        }

        public bool isNull()
        {
            return Z == 0;
        }

        public static bool operator ==(Position c1, Position c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(Position c1, Position c2)
        {
            return !c1.Equals(c2);
        }
    }
}
