using KzBot.Objects;
using KzBot.UI;
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
        public UI.Refill Refill;
        public UI.Settings Settings;


        public UI.CavebotLite CavebotLite = new UI.CavebotLite();

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            this.Location = KzBot.Properties.Settings.Default.Location;
            this.Size = KzBot.Properties.Settings.Default.Size;

            Globals.Main = this;

            Globals.telegramBotToken = Properties.Settings.Default.TelegramBotToken;
            Globals.telegramUserId = Properties.Settings.Default.TelegramUserId;

            Globals.TelegramBot = new TelegramBotClient(Globals.telegramBotToken);

            foreach (AlarmType t in Enum.GetValues(typeof(AlarmType)))
                Globals.Config.Alarms.Add(new AlarmRule(t));

            Cavebot = new UI.Cavebot();
            Healer = new UI.Healer();
            Targeting = new UI.Targeting();
            Alerts = new UI.Alerts();
            Refill = new UI.Refill();
            Settings = new UI.Settings();

            TabPage cavebotPage = new TabPage(Cavebot.Text);
            cavebotPage.Controls.Add(Cavebot);

            TabPage healerPage = new TabPage(Healer.Text);
            healerPage.Controls.Add(Healer);

            TabPage targetingPage = new TabPage(Targeting.Text);
            targetingPage.Controls.Add(Targeting);

            TabPage alertsPage = new TabPage(Alerts.Text);
            alertsPage.Controls.Add(Alerts);

            TabPage refillPage = new TabPage(Refill.Text);
            refillPage.Controls.Add(Refill);

            TabPage settingsPage = new TabPage(Settings.Text);
            settingsPage.Controls.Add(Settings);

            tabControl1.Controls.Add(cavebotPage);
            tabControl1.Controls.Add(healerPage);
            tabControl1.Controls.Add(targetingPage);
            tabControl1.Controls.Add(alertsPage);
            tabControl1.Controls.Add(refillPage);
            tabControl1.Controls.Add(settingsPage);

            listView1.DoubleBuffering(true);
            Cavebot.listView1.DoubleBuffering(true);
            comboBox2.SelectedIndex = 0;
            button1.Text = ((Keys)Properties.Settings.Default[comboBox2.Text.Replace(" ", "_")]).ToString();

            comboBox1.Items.Clear();

            foreach (Process p in Process.GetProcessesByName("client").Where(p => p.MainWindowTitle != "Tibia"))
            {
                comboBox1.Items.Add(p.MainWindowTitle.Replace("Tibia - ", ""));
            }

            Threads.ClientData.Thread.Change(100, Timeout.Infinite);

            refreshToolStripMenuItem.PerformClick();
            Main_ResizeEnd(sender, e);
        }
        private void Main_ResizeEnd(object sender, EventArgs e)
        {
            decimal pixelPercent = (this.Size.Width - 160 - System.Windows.Forms.SystemInformation.VerticalScrollBarWidth) / 100;

            Refill.listView1.Columns[1].Width = (int)Math.Floor(pixelPercent * 20);
            Refill.listView1.Columns[2].Width = (int)Math.Floor(pixelPercent * 15);
            Refill.listView1.Columns[3].Width = (int)Math.Floor(pixelPercent * 35);
            Refill.listView1.Columns[4].Width = (int)Math.Floor(pixelPercent * 15);
            Refill.listView1.Columns[5].Width = (int)Math.Floor(pixelPercent * 15);

            if  (this.Size.Width > 700)
            {
                Healer.listView1.Columns[1].Width = (int)Math.Floor(pixelPercent * 20);
                Healer.listView1.Columns[2].Width = (int)Math.Floor(pixelPercent * 20);
                Healer.listView1.Columns[3].Width = (int)Math.Floor(pixelPercent * 20);
                Healer.listView1.Columns[4].Width = (int)Math.Floor(pixelPercent * 20);
                Healer.listView1.Columns[5].Width = (int)Math.Floor(pixelPercent * 10);
                Healer.listView1.Columns[6].Width = (int)Math.Floor(pixelPercent * 10);

                Cavebot.listView1.Columns[0].Width = 15;
                Cavebot.listView1.Columns[1].Width = (int)Math.Floor(pixelPercent * 15);
                Cavebot.listView1.Columns[2].Width = (int)Math.Floor(pixelPercent * 15);
                Cavebot.listView1.Columns[3].Width = (int)Math.Floor(pixelPercent * 20);
                Cavebot.listView1.Columns[4].Width = (int)Math.Floor(pixelPercent * 10);
                Cavebot.listView1.Columns[5].Width = (int)Math.Floor(pixelPercent * 35);

                Targeting.listView1.Columns[1].Width = (int)Math.Floor(pixelPercent * 20);
                Targeting.listView1.Columns[2].Width = (int)Math.Floor(pixelPercent * 10);
                Targeting.listView1.Columns[3].Width = (int)Math.Floor(pixelPercent * 20);
                Targeting.listView1.Columns[4].Width = (int)Math.Floor(pixelPercent * 10);
                Targeting.listView1.Columns[5].Width = (int)Math.Floor(pixelPercent * 10);
                Targeting.listView1.Columns[6].Width = (int)Math.Floor(pixelPercent * 10);
                Targeting.listView1.Columns[7].Width = (int)Math.Floor(pixelPercent * 10);
                Targeting.listView1.Columns[8].Width = (int)Math.Floor(pixelPercent * 10);
            }
            else
            {
                Healer.listView1.Columns[1].Width = (int)Math.Floor(pixelPercent * 20);
                Healer.listView1.Columns[2].Width = (int)Math.Floor(pixelPercent * 25);
                Healer.listView1.Columns[3].Width = (int)Math.Floor(pixelPercent * 25);
                Healer.listView1.Columns[4].Width = (int)Math.Floor(pixelPercent * 15);
                Healer.listView1.Columns[5].Width = (int)Math.Floor(pixelPercent * 15);
                Healer.listView1.Columns[6].Width = 0;

                Cavebot.listView1.Columns[0].Width = 15;
                Cavebot.listView1.Columns[1].Width = (int)Math.Floor(pixelPercent * 15);
                Cavebot.listView1.Columns[2].Width = (int)Math.Floor(pixelPercent * 20);
                Cavebot.listView1.Columns[3].Width = (int)Math.Floor(pixelPercent * 35);
                Cavebot.listView1.Columns[4].Width = 0;
                Cavebot.listView1.Columns[5].Width = (int)Math.Floor(pixelPercent * 25);

                Targeting.listView1.Columns[1].Width = (int)Math.Floor(pixelPercent * 15);
                Targeting.listView1.Columns[2].Width = (int)Math.Floor(pixelPercent * 15);
                Targeting.listView1.Columns[3].Width = (int)Math.Floor(pixelPercent * 20);
                Targeting.listView1.Columns[4].Width = (int)Math.Floor(pixelPercent * 15);
                Targeting.listView1.Columns[5].Width = (int)Math.Floor(pixelPercent * 15);
                Targeting.listView1.Columns[6].Width = (int)Math.Floor(pixelPercent * 15);
                Targeting.listView1.Columns[7].Width = 0;
                Targeting.listView1.Columns[8].Width = 0;
            }

            Debug.WriteLine(pixelPercent.ToString());
        }

        private bool canCloseForm = false;

        private void Main_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            if (!canCloseForm && MessageBox.Show("Are you sure you want to close KzBot?", "Are you sure?", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                e.Cancel = true;
                return;
            }
            else if (!canCloseForm)
            {
                canCloseForm = true;

                KzBot.Properties.Settings.Default.Location = this.Location;
                KzBot.Properties.Settings.Default.Size = this.Size;

                KzBot.Properties.Settings.Default.Save();

                this.Close();
            }

            Application.Exit();
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


            try
            {
                Globals.AccountId = -1;
                foreach (ClientList.Client client in Globals.ClientList.Clients)
                {
                    Globals.AccountId = client.Accounts.Accounts.FindIndex(a => a.Character.ToLower() == comboBox1.Text.Trim().ToLower());
                    Globals.Client = client;
                    if (Globals.AccountId != -1)
                        break;
                }
            }
            catch
            {
                Globals.AccountId = -1;
            }


            Threads.Alarms.safeMode = false;

            if (Globals.Client.Version == "12.90")
                Addresses.Version.v1290(Globals.Process);
            else if (Globals.Client.Version == "13.05")
                Addresses.Version.v1305(Globals.Process);
            else if (Globals.Client.Version == "13.20")
                Addresses.Version.v1320(Globals.Process);

            Globals.clientRect = new WinApi.RECT() { left = 0, top = 0, right = 0, bottom = 0 };

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
                if (Globals.Process != null && !Globals.Process.HasExited)
                {
                    Position playerPos = Objects.Player.Position;

                    if (WinApi.GetForegroundWindow() == this.Handle)
                    {
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
                    }

                    if  (Globals.Config.GeneralStatus && Globals.Config.CavebotStatus)
                    {
                        if (Globals.Config.Waypoints.Count > 0 && Globals.WaypointId >= 0 && Globals.WaypointId < Globals.Config.Waypoints.Count)
                        {
                            int lvItemId = 0;
                            foreach (ListViewItem lvItem in Cavebot.listView1.Items)
                            {
                                if (lvItemId++ == Globals.WaypointId)
                                    lvItem.Text = ">";
                                else
                                    lvItem.Text = "";
                            }
                        }
                    }

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
                Threads.Alarms.safeMode = false;
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

            using (Stream file = System.IO.File.Open("Clients.xml", FileMode.Open))
            {
                XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(ClientList), new XmlRootAttribute("KzTibia"));

                Globals.ClientList.Clients.Clear();
                Globals.ClientList = (ClientList)writer.Deserialize(file);

                foreach (ClientList.Client client in Globals.ClientList.Clients)
                {
                    ToolStripMenuItem item = new ToolStripMenuItem(client.Name);
                    client.Update(item);

                    item.Click += (sender, EventArgs) => {
                        Globals.Client = client;

                        if (Globals.Process == null)
                            return;

                        if (client.Version == "12.90")
                            Addresses.Version.v1290(Globals.Process);
                        else if (client.Version == "13.05")
                            Addresses.Version.v1305(Globals.Process);
                        else if (client.Version == "13.20")
                            Addresses.Version.v1320(Globals.Process);
                    };

                    accountsToolStripMenuItem.DropDownItems.Add(item);
                    // Login
                }

                file.Close();
            }
        }

        private void cavebotLiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
             CavebotLite.Show();
        }   

        private void charactersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //System.Diagnostics.Process.Start("explorer.exe", $"\"https://kzsoft.com.br/characters.php\"");
            MessageBox.Show(Objects.Client.hasCooldown(CooldownGroup.Heal).ToString());
        }

        private void sendToSafeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Threads.Alarms.safeMode = true;
        }

        private void accountCreatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AccCreator().Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
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