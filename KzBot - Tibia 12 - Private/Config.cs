using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KzBot
{
    public class Config
    {
        public bool auto_Reconnect { get; set; } = true;
        public bool manual_Looting { get; set; } = false;




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
        public int creature_Count_To_Skip_Lure { get; set; } = 3;
        public int creature_Count_To_End_Lure { get; set; } = 1;
        public int creatures_Left_Health_To_End_Lure { get; set; } = 50;

        // Cavebot Waypoints
        public List<Waypoint> Waypoints { get; set; } = new List<Waypoint>();






        // Targeting Options
        public bool ignore_Creature_Count_on_Combo { get; set; } = true;
        public bool stop_Walking_on_Target { get; set; } = false;
        public int max_Distance_To_Target { get; set; } = 99;
        public bool follow_Target { get; set; } = false;

        // Targeting Rules
        public List<TargetRule> Targeting { get; set; } = new List<TargetRule>();

        public int client_Data_Update_Rate { get; set; } = 100;
        public bool auto_Haste { get; set; } = true;
        public bool auto_Utito { get; set; } = true;

        public bool GeneralStatus { get; set; } = false;
        public bool HealerStatus { get; set; } = false;
        public bool CavebotStatus { get; set; } = false;
        public bool TargetingStatus { get; set; } = false;
        public bool AlarmStatus { get; set; } = false;
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
        public int Range { get; set; } = 1;
        [XmlAttribute]
        public int Mana { get; set; } = 0;
        [XmlAttribute]
        public int Level { get; set; } = 0;
        [XmlAttribute]
        public int Delay { get; set; } = 0;
        [XmlIgnore]
        public DateTime LastUse { get; set; } = DateTime.MinValue;

        public ListViewItem ListViewItem()
        {
            ListViewItem item = new ListViewItem();

            item.SubItems.Add(Type.ToString());
            item.SubItems.Add(Key.ToString());
            item.SubItems.Add(PlayerOnCenter.ToString());
            item.SubItems.Add(Range.ToString());
            item.SubItems.Add(CreatureCount.ToString());
            item.SubItems.Add(Mana.ToString());

            return item;
        }
    }

    public enum TargetType
    {
        Nothing,
        Item,
        Spell,
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
        public int Delay { get; set; } = 0;
        [XmlAttribute]
        public Keys Key { get; set; } = Keys.None;
        [XmlAttribute]
        public HealType Type { get; set; } = HealType.Nothing;

        public ListViewItem ListViewItem()
        {
            ListViewItem item = new ListViewItem();

            item.SubItems.Add(Type.ToString());
            item.SubItems.Add(String.Format("{0} to {1}", HpMin, HpMax));
            item.SubItems.Add(String.Format("{0} to {1}", MpMin, MpMax));
            item.SubItems.Add(Key.ToString());
            item.SubItems.Add(Delay.ToString());

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
            item.SubItems.Add(String.Format("x:{0}, y:{1}, z:{2}", X,Y,Z));
            item.SubItems.Add(Extra);

            return item;
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
        Sell_All,
        Buy_Refill,
        Deposit_All,
        Transfer,
        Wait_PZ,
        Disable_Targeting,
        Disable_Alerts,
        Enable_Targeting,
        Enable_Alerts,
        Use,
        Use_On,
        Not_Location_Goto_Label,
        Not_Location_Goback,
        Press,
        Click_Ok,
        Login_Next,
        Exit,
        Load,
        Alert,
        Nothing,
    }
}
