using HUB.Classes;
using HUB.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace HUB.UI
{
    public partial class Connections : Form
    {
        public Connections()
        {
            InitializeComponent();

            this.TopLevel = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Dock = DockStyle.Fill;
            this.Show();
        }

        private void Connections_Load(object sender, EventArgs e)
        {
            foreach (LevelingConnection lvConn in Program.Config.LevelingConnections)
            {
                lvConn.enabled = false;
                listView1.Items.Add(lvConn.ListViewItem);
            }

            foreach (ScriptConnection scriptConn in Program.Config.ScriptConnections)
            {
                scriptConn.enabled = false;
                listView2.Items.Add(scriptConn.ListViewItem);
            }

            foreach (Server server in Program.Config.Servers)
            {
                comboBox3.Items.Add(server.name);
                comboBox1.Items.Add(server.name);
            }

            foreach (Script script in Program.Config.Scripts)
            {
                comboBox2.Items.Add(script.name);
                comboBox4.Items.Add(script.name);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LevelingConnection lvConnection = new LevelingConnection();
            Program.Config.LevelingConnections.Add(lvConnection);
            listView1.Items.Add(lvConnection.ListViewItem);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool hasSelectedItem = listView1.SelectedItems.Count > 0;

            checkBox2.Enabled = hasSelectedItem;
            comboBox2.Enabled = hasSelectedItem;
            comboBox3.Enabled = hasSelectedItem;

            if (hasSelectedItem)
            {
                LevelingConnection conn = Program.Config.LevelingConnections[listView1.SelectedIndices[0]];

                checkBox2.Checked = conn.createCharacter;
                comboBox2.Text = conn.script;
                comboBox3.Text = conn.server;
            }
        }

        private void listView1_CheckedChanged(object sender, ItemCheckedEventArgs e)
        {
            LevelingConnection conn = Program.Config.LevelingConnections[e.Item.Index];
            conn.enabled = e.Item.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            ListViewItem lvItem = listView1.SelectedItems[0];
            if (lvItem != null)
            {
                Program.Config.LevelingConnections[lvItem.Index].createCharacter = checkBox2.Checked;
                lvItem.SubItems[1].Text = checkBox2.Checked.ToString();
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListViewItem lvItem = listView1.SelectedItems[0];
            if (lvItem != null)
            {
                Program.Config.LevelingConnections[lvItem.Index].server = comboBox3.Text;
                lvItem.SubItems[2].Text = comboBox3.Text;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListViewItem lvItem = listView1.SelectedItems[0];
            if (lvItem != null)
            {
                Program.Config.LevelingConnections[lvItem.Index].script = comboBox2.Text;
                lvItem.SubItems[3].Text = comboBox2.Text;
            }
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Interval = 60000;

            foreach (LevelingConnection lvConn in Program.Config.LevelingConnections)
            {
                try
                {
                    if (!lvConn.enabled)
                        continue;

                    bool isScriptRunning = Program.Characters.Exists(c => c.server == lvConn.server && c.script == lvConn.script && (DateTime.Now - DateTime.Parse(c.last_online)).TotalMinutes <= 2);
                    if (isScriptRunning)
                        continue;

                    Character? ch = Program.Characters.Find(c => c.server == lvConn.server && c.script == lvConn.script && c.level < lvConn.levelMax && c.status >= 0);

                    if (lvConn.createCharacter && ch == null)
                    {
                        using (CreateCharacter createCharacter = new CreateCharacter())
                        {
                            ch = await createCharacter.CreateGetCharacter(lvConn.server, lvConn.vocation, lvConn.script);
                        }
                    }

                    if (ch == null)
                        continue;

                    Script script = Program.Config.Scripts.FirstOrDefault(s => s.name == lvConn.script);
                    Client client = Program.Config.Clients.FirstOrDefault(c => script.client == c.name);
                    Server server = Program.Config.Servers.FirstOrDefault(s => s.name == lvConn.server);
                    string clientPath = Path.Combine(server.path, "bin", client.file);

                    List<string> argms = new List<string>()
                    {
                        script.id,
                        Settings.Default.Username,
                        Settings.Default.Password,
                        string.Format("\"{0}\"", ch.name),
                        ch.account,
                        ch.password,
                        ch.char_index.ToString(),
                        ch.vocation,
                        server.id,
                        client.id,
                    };

                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.WindowStyle = ProcessWindowStyle.Normal;
                    startInfo.CreateNoWindow = false;
                    startInfo.UseShellExecute = false;
                    startInfo.FileName = Settings.Default.BotPath;
                    startInfo.Arguments = string.Join(" ", argms);
                    Process.Start(startInfo);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
        }

        #region "Scripts"
        #endregion
        private void button2_Click_1(object sender, EventArgs e)
        {
            ScriptConnection scriptConnection = new ScriptConnection();
            Program.Config.ScriptConnections.Add(scriptConnection);
            listView2.Items.Add(scriptConnection.ListViewItem);
        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool hasSelectedItem = listView2.SelectedItems.Count > 0;

            checkBox1.Enabled = hasSelectedItem;
            comboBox1.Enabled = hasSelectedItem;
            comboBox4.Enabled = hasSelectedItem;

            if (hasSelectedItem)
            {
                ScriptConnection conn = Program.Config.ScriptConnections[listView2.SelectedIndices[0]];

                checkBox1.Checked = conn.requiresImbuement;
                comboBox1.Text = conn.script;
                comboBox4.Text = conn.server;
            }
        }

        private void listView2_CheckedChanged(object sender, ItemCheckedEventArgs e)
        {
            ScriptConnection conn = Program.Config.ScriptConnections[e.Item.Index];
            conn.enabled = e.Item.Checked;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            ListViewItem lvItem = listView2.SelectedItems[0];
            if (lvItem != null)
            {
                Program.Config.ScriptConnections[lvItem.Index].requiresImbuement = checkBox1.Checked;
                lvItem.SubItems[1].Text = checkBox1.Checked.ToString();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListViewItem lvItem = listView2.SelectedItems[0];
            if (lvItem != null)
            {
                Program.Config.ScriptConnections[lvItem.Index].server = comboBox1.Text;
                lvItem.SubItems[2].Text = comboBox1.Text;
            }
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListViewItem lvItem = listView2.SelectedItems[0];
            if (lvItem != null)
            {
                Program.Config.ScriptConnections[lvItem.Index].script = comboBox4.Text;
                lvItem.SubItems[3].Text = comboBox4.Text;
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Interval = 60000;

            new Thread(async () =>
            {
                foreach (ScriptConnection scriptConn in Program.Config.ScriptConnections)
                {
                    try
                    {
                        if (!scriptConn.enabled)
                            continue;

                        if ((DateTime.Now - Program.lastSuccessfulUpdate).TotalMinutes > 2)
                            continue;

                        if ((DateTime.Now - scriptConn.lastConnection).TotalMinutes < scriptConn.minMinutesBetweenScripts)
                            continue;

                        bool isScriptRunning = Program.Characters.Exists(c => c.server == scriptConn.server && c.level >= scriptConn.minLevel && ((c.status != -1 && (DateTime.Now - DateTime.Parse(c.last_online)).TotalMinutes <= 2) || (c.status == -1 && (DateTime.Now - DateTime.Parse(c.last_online)).TotalMinutes < scriptConn.minutesToWaitOnBan)));
                        if (isScriptRunning)
                            continue;

                        Character? ch = Program.Characters.OrderByDescending(c=> c.actual_stamina).ThenBy(c=> c.level).ToList().Find(c => c.server == scriptConn.server && c.script == scriptConn.script && c.status >= 0 && (!scriptConn.requiresImbuement || (c.imbuement_time != 0 && (c.actual_stamina - TimeSpan.FromMinutes(c.imbuement_time > 0 ? c.imbuement_time : 20 * 60)).TotalHours > 14)));

                        if (ch == null)
                            continue;

                        Script script = Program.Config.Scripts.FirstOrDefault(s => s.name == scriptConn.script);
                        Client client = Program.Config.Clients.FirstOrDefault(c => script.client == c.name);
                        Server server = Program.Config.Servers.FirstOrDefault(s => s.name == scriptConn.server);
                        string clientPath = Path.Combine(server.path, "bin", client.file);

                        List<string> argms = new List<string>()
                    {
                        script.id,
                        Settings.Default.Username,
                        Settings.Default.Password,
                        string.Format("\"{0}\"", ch.name),
                        ch.account,
                        ch.password,
                        ch.char_index.ToString(),
                        ch.vocation,
                        server.id,
                        client.id,
                        scriptConn.maxBalance.ToString(),
                    };

                        ProcessStartInfo startInfo = new ProcessStartInfo();
                        startInfo.WindowStyle = ProcessWindowStyle.Normal;
                        startInfo.CreateNoWindow = false;
                        startInfo.UseShellExecute = false;
                        startInfo.FileName = Settings.Default.BotPath;
                        startInfo.Arguments = string.Join(" ", argms);
                        Process.Start(startInfo);

                        UpdateScript(ch.name, script.name);

                        ((Main)this.Parent).Log.listView1.Invoke((MethodInvoker)delegate
                        {
                            ListViewItem lvItem = new ListViewItem();

                            lvItem.Text = string.Empty;
                            lvItem.SubItems.Add(ch.name);
                            lvItem.SubItems.Add(ch.server);
                            lvItem.SubItems.Add(DateTime.Now.ToString());
                            lvItem.SubItems.Add(string.Format("Trying to Run"));

                            listView1.Items.Add(lvItem);
                        });

                        scriptConn.lastConnection = DateTime.Now;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }
                }
            }).Start();
        }

        public static async void UpdateScript(string charName, string script)
        {
            try
            {
                var client = new HttpClient();

                client.Timeout = TimeSpan.FromSeconds(5);
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
                client.DefaultRequestHeaders.Add("accept", "application/json, text/plain, */*");
                client.DefaultRequestHeaders.Add("accept-language", "en-US,en;q=0.9");

                string requestString = string.Format("https://tibia.kzsoft.com.br/script.php?username={0}&password={1}&char_name={2}&script={3}",
                    Settings.Default.Username,
                    Settings.Default.Password,
                    HttpUtility.UrlEncode(charName).Replace("+", "%20"),
                    HttpUtility.UrlEncode(script).Replace("+", "%20")
                    );

                var response = await client.GetAsync(requestString);
                string content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode && content == "1")
                {
                    //done
                }
                else
                {
                    // error
                }
            }
            catch (Exception ex)
            {
                // error
                return;
            }
        }
    }
}
