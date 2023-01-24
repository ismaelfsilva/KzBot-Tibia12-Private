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

            foreach (Server server in Program.Config.Servers)
                comboBox3.Items.Add(server.name);

            foreach (Script script in Program.Config.Scripts)
                comboBox2.Items.Add(script.name);
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

                    Character? ch = Program.Characters.Find(c => c.server == lvConn.server && c.script == lvConn.script && c.level < lvConn.levelMax);

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

        private async void button2_Click(object sender, EventArgs e)
        {
            using (CreateCharacter createCharacter = new CreateCharacter())
            {
                Character ch = await createCharacter.CreateGetCharacter("Hades", "RP", "Leveling 1");
            }

            Debug.WriteLine(1);
        }
    }
}
