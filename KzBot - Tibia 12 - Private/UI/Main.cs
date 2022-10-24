using KzBot.Objects;
using System.Diagnostics;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using Telegram.Bot;

namespace KzBot
{
    public partial class Main : Form
    {
        public UI.Cavebot Cavebot;
        public UI.Healer Healer;
        public UI.Targeting Targeting;
        public UI.Alerts Alerts;
        public UI.Settings Settings;


        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            Globals.Main = this;

            Globals.exeOtLocation = Properties.Settings.Default.ExeLocation;
            Globals.telegramBotToken = Properties.Settings.Default.TelegramBotToken;
            Globals.telegramUserId = Properties.Settings.Default.TelegramUserId;
            Globals.characterToTransfer = Properties.Settings.Default.CharacterToTransfer;

            Globals.TelegramBot = new TelegramBotClient(Globals.telegramBotToken);

            foreach (AlarmType t in Enum.GetValues(typeof(AlarmType)))
                Globals.Config.Alarms.Add(new AlarmRule(t));

            Cavebot = new UI.Cavebot();
            Healer = new UI.Healer();
            Targeting = new UI.Targeting();
            Alerts = new UI.Alerts();
            Settings = new UI.Settings();

            TabPage cavebotPage = new TabPage(Cavebot.Text);
            cavebotPage.Controls.Add(Cavebot);

            TabPage healerPage = new TabPage(Healer.Text);
            healerPage.Controls.Add(Healer);

            TabPage targetingPage = new TabPage(Targeting.Text);
            targetingPage.Controls.Add(Targeting);

            TabPage alertsPage = new TabPage(Alerts.Text);
            alertsPage.Controls.Add(Alerts);

            TabPage settingsPage = new TabPage(Settings.Text);
            settingsPage.Controls.Add(Settings);

            tabControl1.Controls.Add(cavebotPage);
            tabControl1.Controls.Add(healerPage);
            tabControl1.Controls.Add(targetingPage);
            tabControl1.Controls.Add(alertsPage);
            tabControl1.Controls.Add(settingsPage);

            listView1.DoubleBuffering(true);
            comboBox2.SelectedIndex = 0;
            button1.Text = ((Keys)Properties.Settings.Default[comboBox2.Text.Replace(" ", "_")]).ToString();

            comboBox1.Items.Clear();

            foreach (Process p in Process.GetProcessesByName("client").Where(p => p.MainWindowTitle != "Tibia"))
            {
                comboBox1.Items.Add(p.MainWindowTitle.Replace("Tibia - ", ""));
            }

            Threads.ClientData.Thread.Change(100, Timeout.Infinite);

            refreshToolStripMenuItem.PerformClick();
        }

        private void comboBox1_Clicked(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();

            foreach (Process p in Process.GetProcessesByName("client").Where(p => p.MainWindowTitle != "Tibia"))
            {
                comboBox1.Items.Add(p.MainWindowTitle.Replace("Tibia - ", ""));
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Process p = Process.GetProcessesByName("client").FirstOrDefault(p => p.MainWindowTitle == "Tibia - " + comboBox1.Text);
            if (p != null)
                Globals.Process = p;
            Addresses.Version.HadesOt(Globals.Process);
            Threads.ClientData.setClient = false;
        }   

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fileDialog = new OpenFileDialog())
            {
                fileDialog.Filter = "Kz Tibia Script (*.kzTibia)|*.kzTibia";
                fileDialog.DefaultExt = "kzTibia";
                fileDialog.AddExtension = true;
                fileDialog.DefaultExt = ".kzTibia";

                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    Globals.Load(fileDialog.FileName);
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {

            using (SaveFileDialog fileDialog = new SaveFileDialog())
            {
                fileDialog.Filter = "Kz Tibia Script (*.kzTibia)|*.kzTibia";
                fileDialog.DefaultExt = "kzTibia";
                fileDialog.AddExtension = true;

                fileDialog.DefaultExt = ".kzTibia";

                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    Globals.Save(fileDialog.FileName);                    
                }
            }
        }

        private void uiUpdater_Tick(object sender, EventArgs e)
        {
            try
            {
                if (Globals.Process != null && !Globals.Process.HasExited && WinApi.GetForegroundWindow() == this.Handle)
                {
                    Position playerPos = Objects.Player.Position;

                    listView1.Items[0].SubItems[1].Text = String.Format("{0}.{1} ({2})", Objects.Player.Level, Objects.Player.LevelPc, Objects.Player.Experience);
                    listView1.Items[1].SubItems[1].Text = String.Format("{0} / {1}", Objects.Player.Health, Objects.Player.HealthMax);
                    listView1.Items[2].SubItems[1].Text = String.Format("{0} / {1}", Objects.Player.Mana, Objects.Player.ManaMax);
                    listView1.Items[3].SubItems[1].Text = String.Format("{0} / {1}", 0, 0);
                    listView1.Items[4].SubItems[1].Text = String.Format("{0}", Objects.Player.Cap);
                    listView1.Items[5].SubItems[1].Text = String.Format("{0}", Objects.Player.Soul);
                    listView1.Items[6].SubItems[1].Text = String.Format("{0}", playerPos);
                    listView1.Items[7].SubItems[1].Text = String.Format("{0}", Objects.Player.Stamina);
                    listView1.Items[8].SubItems[1].Text = String.Format("{0}", (DateTime.Now - Globals.Process.StartTime));

                    this.Text = "KzBot - " + Objects.Player.Creature.Name;

                    if (Cavebot.updatedPosition != playerPos)
                    {
                        Cavebot.updatedPosition = playerPos;

                        Cavebot.textBox1.Text = playerPos.X.ToString();
                        Cavebot.textBox2.Text = playerPos.Y.ToString();
                        Cavebot.textBox3.Text = playerPos.Z.ToString();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Globals.Config.GeneralStatus = checkBox1.Checked;

            if (Globals.Config.GeneralStatus)
            {
                //Threads.ClientData.Thread.Change(100, Timeout.Infinite);
                Threads.Alarms.Thread.Change(100, Timeout.Infinite);

                if (Globals.Config.CavebotStatus) Threads.Cavebot.Thread.Change(100, Timeout.Infinite);
                if (Globals.Config.HealerStatus) Threads.Healer.Thread.Change(100, Timeout.Infinite);
                if (Globals.Config.TargetingStatus)
                {
                    Threads.Targeting.Target.Change(100, Timeout.Infinite);
                    Threads.Targeting.SpellCaster.Change(100, Timeout.Infinite);
                }
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Globals.Config.HealerStatus = checkBox2.Checked;
            Threads.Healer.Thread.Change(100, Timeout.Infinite);
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            Globals.Config.CavebotStatus = checkBox3.Checked;
            Threads.Cavebot.Thread.Change(100, Timeout.Infinite);
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            Globals.Config.TargetingStatus = checkBox4.Checked;
            Threads.Targeting.Target.Change(100, Timeout.Infinite);
            Threads.Targeting.SpellCaster.Change(100, Timeout.Infinite);
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            Globals.Config.AlarmStatus = checkBox5.Checked;
            Threads.Alarms.Thread.Change(100, Timeout.Infinite);
        }

        private void button1_KeyDown(object sender, KeyEventArgs e)
        {
            Properties.Settings.Default[comboBox2.Text.Replace(" ", "_")] = (int)e.KeyCode;
            Properties.Settings.Default.Save();
            button1.Text = e.KeyCode.ToString();    
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            button1.Text = ((Keys)Properties.Settings.Default[comboBox2.Text.Replace(" ", "_")]).ToString();
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            while (accountsToolStripMenuItem.DropDownItems.Count > 2)
                accountsToolStripMenuItem.DropDownItems.RemoveAt(2);

            using (Stream file = System.IO.File.Open("Accounts.xml", FileMode.Open))
            {
                XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(Accounts), new XmlRootAttribute("KzTibia"));

                Globals.Accounts.List.Clear();
                Globals.Accounts = (Accounts)writer.Deserialize(file);

                foreach (Accounts.Account account in Globals.Accounts.List)
                {
                    ToolStripItem item = accountsToolStripMenuItem.DropDownItems.Add(account.Character);
                    item.Click += (sender, EventArgs) => {
                        account.Start();
                    };


                    // Login
                }

                file.Close();
            }
        }


        private void testeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            {
                for (int tries = 0; tries < 5; tries++)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            Point sqmPosition = new Point();
                            sqmPosition.X = Objects.ClientData.GameMapCenter.X + x * Objects.ClientData.SqmSize.Width;
                            sqmPosition.Y = Objects.ClientData.GameMapCenter.Y + y * Objects.ClientData.SqmSize.Height;
                            Objects.Client.rightClickPos(sqmPosition.X, sqmPosition.Y);
                            //Debug.WriteLine(sqmPosition.ToString());
                            System.Threading.Thread.Sleep(10);
                        }
                    }
                }
            }
            //    MessageBox.Show(String.Format("Heal: {0} | Support: {1} | Attack: {2} | Diamond Arrow: {3}", Objects.Client.hasCooldown(CooldownGroup.Heal), Objects.Client.hasCooldown(CooldownGroup.Support), Objects.Client.hasCooldown(CooldownGroup.Attack), Objects.Client.getItemCount(-29635)));
        }

        private void alertaEsteBotãoFazCoisasEstranhasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Globals.WaypointId.ToString());
        }
    }   

    public static class ControlExtensions
    {
        public static void DoubleBuffering(this Control control, bool enable)
        {
            var doubleBufferPropertyInfo = control.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            doubleBufferPropertyInfo.SetValue(control, enable, null);
        }
    }
}