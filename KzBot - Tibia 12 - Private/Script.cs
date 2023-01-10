using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KzBot
{
    public class Script
    {
        public bool auto_Reconnect { get; set; } = true;


        // Alarm Options

        // Alarm Rules
        public List<AlarmRule> Alarms { get; set; } = new List<AlarmRule>();





        // Healer Options

        // Healer Rules
        public List<HealRule> Healer { get; set; } = new List<HealRule>();





        // Cavebot Options
        public int qty_To_Transfer { get; set; } = 1000000;
        public bool check_Only_Near_Creatures_If_Player_on_Screen { get; set; } = true;
        public int time_To_Wait_Before_Checking_Creatures_If_Player_on_Screen { get; set; } = 3000;
        public int max_Lure_Time { get; set; } = -1;
        public int creature_Count_To_Skip_Lure { get; set; } = 3;
        public int creature_Count_To_End_Lure { get; set; } = 1;
        public int creatures_Left_Health_To_End_Lure { get; set; } = 50;

        public int min_Level_To_Check_Cap { get; set; } = 0;

        // Cavebot Waypoints
        public List<Waypoint> Waypoints { get; set; } = new List<Waypoint>();






        // Targeting Options
        public bool only_target_on_lures { get; set; } = false;
        public bool ignore_Creature_Count_on_Combo { get; set; } = true;
        public bool stop_Walking_on_Target { get; set; } = false;
        public int max_Distance_To_Target { get; set; } = 99;
        public bool follow_Target { get; set; } = false;
        public List<string> ignore_List { get; set; } = new List<string>();

        // Targeting Rules
        public List<TargetRule> Targeting { get; set; } = new List<TargetRule>();

        public List<RefillRule> Refill { get; set; } = new List<RefillRule>();


        //public int client_Data_Update_Rate { get; set; } = 100;
        public bool auto_Haste { get; set; } = true;
        public bool auto_Utito { get; set; } = true;

        public bool GeneralStatus { get; set; } = false;
        public bool HealerStatus { get; set; } = false;
        public bool CavebotStatus { get; set; } = false;
        public bool TargetingStatus { get; set; } = false;
        public bool AlarmStatus { get; set; } = false;
    }


    public class RefillRule
    {
        [XmlAttribute]
        public string Type { get; set; } = "trade";
        [XmlAttribute]
        public string Name { get; set; } = string.Empty;
        [XmlAttribute]
        public int Id { get; set; } = 0;
        [XmlAttribute]
        public int Level { get; set; } = 0;
        [XmlAttribute]
        public int MaxLevel { get; set; } = 0;
        [XmlAttribute]
        public int ToBuy { get; set; } = 0;
        [XmlAttribute]
        public int ToLeave { get; set; } = 0;
        [XmlAttribute]
        public Vocation Vocation { get; set; } = Vocation.None;

        public RefillRule()
        {
        }
        public ListViewItem ListViewItem()
        {
            ListViewItem item = new ListViewItem();

            item.SubItems.Add(Type.ToString());
            item.SubItems.Add(Id.ToString());
            item.SubItems.Add(Name.ToString());
            item.SubItems.Add(ToBuy.ToString());
            item.SubItems.Add(ToLeave.ToString());

            return item;
        }
    }

    public enum AlarmType
    {
        Player_On_Screen,
        PK_On_Screen,
        GM_On_Screen,
        Has_Skull,
        Has_Skull_Dangerous,
        Local_Chat_Message,
        Death,
        Disconnected,
        Low_Stamina,
        Low_Cap,
        Stuck,
        No_Imbue,
    }

    public class AlarmRule
    {
        [XmlAttribute]
        public AlarmType Type { get; set; }
        [XmlAttribute]
        public string Action { get; set; } = string.Empty;
        [XmlAttribute]
        public bool Enabled { get; set; } = false;

        public AlarmRule()
        {
        }

        public AlarmRule(AlarmType t)
        {
            Type = t;
        }
    }


    public class TargetRule
    {
        [XmlAttribute]
        public TargetType Type { get; set; } = TargetType.Nothing;
        [XmlAttribute]
        public int CreatureCount { get; set; } = 1;
        [XmlAttribute]
        public Keys Key { get; set; } = Keys.None;
        [XmlAttribute]
        public bool PlayerOnCenter { get; set; } = true;
        [XmlAttribute]
        public bool ComboOnly { get; set; } = false;
        [XmlAttribute]
        public int Range { get; set; } = 1;
        [XmlAttribute]
        public int Mana { get; set; } = 0;
        [XmlAttribute]
        public int Level { get; set; } = 0;
        [XmlAttribute]
        public int MaxLevel { get; set; } = 0;
        [XmlAttribute]
        public int Delay { get; set; } = 0;

        [XmlAttribute]
        public Vocation Vocation { get; set; } = Vocation.None;
        [XmlIgnore]
        public DateTime LastUse { get; set; } = DateTime.MinValue;

        public ListViewItem ListViewItem()
        {
            ListViewItem item = new ListViewItem();

            item.SubItems.Add(Type.ToString());
            item.SubItems.Add(Key.ToString());
            item.SubItems.Add(PlayerOnCenter ? "Player" : "Target");
            item.SubItems.Add(Range.ToString());
            item.SubItems.Add(CreatureCount.ToString());
            item.SubItems.Add(Mana.ToString());
            item.SubItems.Add(Level.ToString());
            item.SubItems.Add(Delay.ToString());

            return item;
        }
    }

    public enum TargetType
    {
        Nothing,
        Item,
        Spell,
        Support,
    }
    public class HealRule
    {
        [XmlAttribute]
        public int HpMin { get; set; } = 0;
        [XmlAttribute]
        public int HpMax { get; set; } = 0;
        [XmlAttribute]
        public int MpMin { get; set; } = 0;
        [XmlAttribute]
        public int MpMax { get; set; } = 0;
        [XmlAttribute]
        public int Level { get; set; } = 0;
        [XmlAttribute]
        public int MaxLevel { get; set; } = 0;
        [XmlAttribute]
        public int Delay { get; set; } = 0;
        [XmlAttribute]
        public Keys Key { get; set; } = Keys.None;
        [XmlAttribute]
        public HealType Type { get; set; } = HealType.Nothing;
        [XmlAttribute]
        public Vocation Vocation { get; set; } = Vocation.None;
        [XmlIgnore]
        public DateTime LastUse { get; set; } = DateTime.MinValue;

        public ListViewItem ListViewItem()
        {
            ListViewItem item = new ListViewItem();

            item.SubItems.Add(Type.ToString());
            item.SubItems.Add(String.Format("{0} to {1}", HpMin, HpMax));
            item.SubItems.Add(String.Format("{0} to {1}", MpMin, MpMax));
            item.SubItems.Add(Key.ToString());
            item.SubItems.Add(Delay.ToString());
            item.SubItems.Add(Level.ToString());

            return item;
        }
    }
    public enum HealType
    {
        Nothing,
        Item,
        Spell,
        SSA,
        EnergyRing,
    }

    public class Waypoint
    {
        [XmlAttribute]
        public WaypointType Type { get; set; } = WaypointType.Nothing;
        [XmlAttribute]
        public string Label { get; set; } = String.Empty;
        [XmlAttribute]
        public int X { get; set; } = 0;
        [XmlAttribute]
        public int Y { get; set; } = 0;
        [XmlAttribute]
        public int Z { get; set; } = 0;
        [XmlAttribute]
        public int rangeX { get; set; } = 1;
        [XmlAttribute]
        public int rangeY { get; set; } = 1;
        [XmlAttribute]
        public string Extra { get; set; } = String.Empty;

        public ListViewItem ListViewItem()
        {
            ListViewItem item = new ListViewItem();

            item.SubItems.Add(Label);
            item.SubItems.Add(Type.ToString());
            item.SubItems.Add(String.Format("x:{0}, y:{1}, z:{2}", X, Y, Z));
            item.SubItems.Add(String.Format("{0}x{1}", rangeX,rangeY));
            item.SubItems.Add(Extra);

            return item;
        }

        [XmlIgnore]
        public Position Position
        {
            get
            {
                return new Position() { X=X, Y=Y,Z=Z};
            }
        }
    }

    public enum WaypointType
    {
        Stand,
        Node,
        Lure,
        Loot,
        Say,
        Wait,
        Goto_Label,
        Check_Cap,
        Check_Stamina,
        Check_Level,
        Check_Refill,
        Check_Safe,
        Check_PZ,
        Check_Imbue,
        Check_Balance,
        Go_Near,
        Open_Npc,
        Sell_All,
        Buy_Refill,
        Deposit_All,
        Transfer,
        Balance,
        Buy_Market,
        Imbue,
        Take_Out_Equip,
        Setup_Config,
        Setup_Bag,
        Set_Offensive,
        Travel,
        Wait_PZ,
        Disable_Healer,
        Disable_Targeting,
        Disable_Alerts,
        Enable_Healer,
        Enable_Targeting,
        Enable_Alerts,
        Use,
        Use_On,
        Teleport,
        Step,
        Not_Location_Goto_Label,
        Not_Location_Goback,
        If_Location_Goto_Label,
        If_Location_Goback,
        If_Vocation_Goto_Label,
        Press,
        Click_Ok,
        Login_Next,
        Exit,
        Logout,
        Close_Bot,
        Load,
        Disable_Safe,
        Reset_FPS,
        Alert,
        Nothing,
    }
}
