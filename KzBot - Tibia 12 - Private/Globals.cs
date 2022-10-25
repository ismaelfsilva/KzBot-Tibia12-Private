using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        public static IntPtr Handle 
        { 
            get
            { 
                return Process.Handle; 
            }
        }
        public static bool ComboStatus { get; set; } = false;
        public static string exeOtLocation { get; set; } = String.Empty;


        public static string telegramUserId { get; set; } = string.Empty;
        public static string telegramBotToken { get; set; } = string.Empty;
        public static TelegramBotClient TelegramBot;


        public static string characterToTransfer { get; set; } = String.Empty;



        public static int AccountId { get; set; } = -1;
        public static string ScriptFile { get; set; } = string.Empty;


        public static Config Config { get; set; } = new Config();
        public static Accounts Accounts { get; set; } = new Accounts();

        public static int WaypointId { get; set; } = 0;

        public static void Load(string filePath)
        {
            using (Stream file = System.IO.File.Open(filePath, FileMode.Open))
            {
                XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(Config), new XmlRootAttribute("KzTibia"));
                Globals.Main.Invoke((MethodInvoker)delegate {

                    Main.checkBox1.Checked = false;
                    Main.checkBox2.Checked = false;
                    Main.checkBox3.Checked = false;
                    Main.checkBox4.Checked = false;
                    Main.checkBox5.Checked = false;

                });


                if (ScriptFile != filePath)
                    Globals.WaypointId = 0;

                Globals.Config.Waypoints.Clear();
                Globals.Config.Targeting.Clear();
                Globals.Config.Healer.Clear();
                Globals.Config.Refill.Clear();

                Globals.Main.Invoke((MethodInvoker)delegate {
                    Main.Cavebot.listView1.Items.Clear();
                    Main.Targeting.listView1.Items.Clear();
                    Main.Healer.listView1.Items.Clear();
                    Main.Refill.listView1.Items.Clear();
                });                

                Globals.Config = (Config)writer.Deserialize(file);

                Globals.Main.Invoke((MethodInvoker)delegate {
                    foreach (Waypoint waypoint in Globals.Config.Waypoints)
                        Main.Cavebot.listView1.Items.Add(waypoint.ListViewItem());

                    foreach (TargetRule targetRule in Globals.Config.Targeting)
                        Main.Targeting.listView1.Items.Add(targetRule.ListViewItem());

                    foreach (HealRule healRule in Globals.Config.Healer)
                        Main.Healer.listView1.Items.Add(healRule.ListViewItem());

                    foreach (RefillRule refillRule in Globals.Config.Refill)
                        Main.Refill.listView1.Items.Add(refillRule.ListViewItem());

                    Main.Alerts.AddAlarms();
                    Main.Settings.Reload();

                    Main.checkBox1.Checked = Globals.Config.GeneralStatus;
                    Main.checkBox2.Checked = Globals.Config.HealerStatus;
                    Main.checkBox3.Checked = Globals.Config.CavebotStatus;
                    Main.checkBox4.Checked = Globals.Config.TargetingStatus;
                    Main.checkBox5.Checked = Globals.Config.AlarmStatus;

                });

                Globals.Main.CavebotLite.doorId = Globals.Config.Waypoints.Count(w => w.Label.StartsWith("Door"));
                Globals.Main.CavebotLite.exaniTeraId = Globals.Config.Waypoints.Count(w => w.Label.StartsWith("ExaniTera"));
                Globals.Main.CavebotLite.holeId = Globals.Config.Waypoints.Count(w => w.Label.StartsWith("Hole"));
                Globals.Main.CavebotLite.ladderId = Globals.Config.Waypoints.Count(w => w.Label.StartsWith("Ladder"));
                Globals.Main.CavebotLite.stairsId = Globals.Config.Waypoints.Count(w => w.Label.StartsWith("Stairs"));
                Globals.Main.CavebotLite.teleportId = Globals.Config.Waypoints.Count(w => w.Label.StartsWith("Teleport"));

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

                ScriptFile = filePath;
                file.Close();
            }
        }

        public static void Save(string filePath)
        {
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            XmlSerializer configSerializer = new System.Xml.Serialization.XmlSerializer(typeof(Config), new XmlRootAttribute("KzTibia"));

            using (var writer = System.IO.File.OpenWrite(filePath))
            {
                using (var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings { Indent = true }))
                {
                    configSerializer.Serialize(xmlWriter, Globals.Config);
                }
            }
        }
    }

    public class Accounts
    {
        public List<Account> List = new List<Account>();

        public class Account
        {
            [XmlAttribute]
            public string Character { get; set; } = string.Empty;
            [XmlAttribute]
            public string AccountName { get; set; } = string.Empty;
            [XmlAttribute]
            public string Password { get; set; } = string.Empty;
            [XmlAttribute]
            public string Script { get; set; } = string.Empty;
            [XmlAttribute]
            public int Index { get; set; } = 0;

            public void Start()
            {
                Globals.Main.Invoke((MethodInvoker)delegate {

                    Globals.Main.checkBox1.Checked = false;
                    Globals.Main.checkBox2.Checked = false;
                    Globals.Main.checkBox3.Checked = false;
                    Globals.Main.checkBox4.Checked = false;
                    Globals.Main.checkBox5.Checked = false;

                });

                Globals.Process?.Kill();

                System.Threading.Thread.Sleep(1000);

                Globals.Process = Process.Start(Globals.exeOtLocation);
                Globals.Process.WaitForInputIdle();

                Globals.Main.Invoke((MethodInvoker)delegate
                {
                    Globals.Main.comboBox1.Items.Clear();
                    Globals.Main.Text = "KzBot - " + Character;
                    Globals.Main.comboBox1.Items.Add(Character);
                    Globals.Main.comboBox1.Text = Character;
                });

                Globals.AccountId = Globals.Accounts.List.FindIndex(a=> a.Character == Character);

                if (Script.Length > 0)
                    Globals.Load(@".\Scripts\" + Script);
            }
        }
    }
}
