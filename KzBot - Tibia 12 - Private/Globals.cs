using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Telegram.Bot;

namespace KzBot
{
    public static class Globals
    {
        public static Main Main;
        public static Process Process { get; set; } = null;
        public static WinApi.RECT clientRect = new WinApi.RECT() { left = 0, top = 0, right = 0, bottom = 0 };
        public static IntPtr Handle 
        { 
            get
            { 
                return Process.Handle; 
            }
        }
        public static bool ComboStatus { get; set; } = false;
        public static bool HasAutoLoot { get; set; } = false;


        public static string telegramUserId { get; set; } = string.Empty;
        public static string telegramBotToken { get; set; } = string.Empty;
        public static TelegramBotClient TelegramBot;


        // SETUP AREA
        public static string Username { get; set; } = string.Empty;
        public static string Password { get; set; } = string.Empty;

        public static string AccCharName { get; set; } = string.Empty;
        public static string AccName { get; set; } = string.Empty;
        public static string AccPass { get; set; } = string.Empty;
        public static int AccCharIndex { get; set; } = 0;
        public static Vocation AccVocation { get; set; } = Vocation.None;

        public static Classes.Client Client { get; set; }
        public static Classes.Server Server { get; set; }
        public static Classes.Script Script { get; set; }

        // END SETUP AREA



        public static Script ScriptConfig { get; set; } = new Script();

        public static int WaypointId { get; set; } = 0;

        public static void Load(string filePath)
        {
            using (Stream file = System.IO.File.Open(filePath, FileMode.Open))
            {
                XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(Script), new XmlRootAttribute("KzTibia"));
                Globals.Main.Invoke((MethodInvoker)delegate {

                    Main.checkBox1.Checked = false;
                    Main.checkBox2.Checked = false;
                    Main.checkBox3.Checked = false;
                    Main.checkBox4.Checked = false;
                    Main.checkBox5.Checked = false;

                });

                Globals.ScriptConfig.AlarmStatus = false;
                Globals.ScriptConfig.HealerStatus = false;
                Globals.ScriptConfig.TargetingStatus = false;
                Globals.ScriptConfig.CavebotStatus = false;
                Globals.ScriptConfig.GeneralStatus = false;

                Globals.ScriptConfig.Waypoints.Clear();
                Globals.ScriptConfig.Targeting.Clear();
                Globals.ScriptConfig.Healer.Clear();
                Globals.ScriptConfig.Refill.Clear();

                Globals.Main.Invoke((MethodInvoker)delegate {
                    Main.Cavebot.listView1.Items.Clear();
                    Main.Targeting.listView1.Items.Clear();
                    Main.Healer.listView1.Items.Clear();
                    Main.Refill.listView1.Items.Clear();
                });                

                Globals.ScriptConfig = (Script)writer.Deserialize(file);

                foreach (AlarmType type in Enum.GetValues(typeof(AlarmType)))
                {
                    if (!Globals.ScriptConfig.Alarms.Exists(rule=> rule.Type == type))
                        Globals.ScriptConfig.Alarms.Add(new AlarmRule(type));
                }

                System.Threading.Thread.Sleep(1000);

                Globals.Main.Invoke((MethodInvoker)delegate {
                    foreach (Waypoint waypoint in Globals.ScriptConfig.Waypoints)
                        Main.Cavebot.listView1.Items.Add(waypoint.ListViewItem());

                    foreach (TargetRule targetRule in Globals.ScriptConfig.Targeting)
                        Main.Targeting.listView1.Items.Add(targetRule.ListViewItem());

                    foreach (HealRule healRule in Globals.ScriptConfig.Healer)
                        Main.Healer.listView1.Items.Add(healRule.ListViewItem());

                    foreach (RefillRule refillRule in Globals.ScriptConfig.Refill)
                        Main.Refill.listView1.Items.Add(refillRule.ListViewItem());

                    Main.Alerts.AddAlarms();
                    Main.Settings.Reload();

                    Main.checkBox2.Checked = Globals.ScriptConfig.HealerStatus;
                    Main.checkBox3.Checked = Globals.ScriptConfig.CavebotStatus;
                    Main.checkBox4.Checked = Globals.ScriptConfig.TargetingStatus;
                    Main.checkBox5.Checked = Globals.ScriptConfig.AlarmStatus;

                    System.Threading.Thread.Sleep(1000);

                    Main.checkBox1.Checked = Globals.ScriptConfig.GeneralStatus;
                });

                Globals.Main.CavebotLite.doorId = Globals.ScriptConfig.Waypoints.Count(w => w.Label.StartsWith("Door"));
                Globals.Main.CavebotLite.exaniTeraId = Globals.ScriptConfig.Waypoints.Count(w => w.Label.StartsWith("ExaniTera"));
                Globals.Main.CavebotLite.holeId = Globals.ScriptConfig.Waypoints.Count(w => w.Label.StartsWith("Hole"));
                Globals.Main.CavebotLite.ladderId = Globals.ScriptConfig.Waypoints.Count(w => w.Label.StartsWith("Ladder"));
                Globals.Main.CavebotLite.stairsId = Globals.ScriptConfig.Waypoints.Count(w => w.Label.StartsWith("Stairs"));
                Globals.Main.CavebotLite.teleportId = Globals.ScriptConfig.Waypoints.Count(w => w.Label.StartsWith("Teleport"));

                /*
                if (Globals.Config.CavebotStatus) Threads.Cavebot.Thread.Change(100, Timeout.Infinite);
                if (Globals.Config.HealerStatus) Threads.Healer.Thread.Change(100, Timeout.Infinite);
                if (Globals.Config.TargetingStatus)
                {
                    Threads.Targeting.Target.Change(100, Timeout.Infinite);
                    Threads.Targeting.SpellCaster.Change(100, Timeout.Infinite);
                }
                if (Globals.Config.AlarmStatus) Threads.Alarms.Thread.Change(100, Timeout.Infinite);
                */

                file.Close();
                Globals.Main.Log.addLog("Loaded Script " + filePath.Split(@"/").LastOrDefault(), true);
            }
        }

        public static void Save(string filePath)
        {
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            XmlSerializer configSerializer = new System.Xml.Serialization.XmlSerializer(typeof(Script), new XmlRootAttribute("KzTibia"));

            using (var writer = System.IO.File.OpenWrite(filePath))
            {
                using (var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings { Indent = true }))
                {
                    configSerializer.Serialize(xmlWriter, Globals.ScriptConfig);
                    Script.path = filePath;
                }
            }
        }
    }
}
