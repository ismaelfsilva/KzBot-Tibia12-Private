using HUB.Classes;
using HUB.Properties;
using HUB.UI;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using ToolTip = System.Windows.Forms.ToolTip;

namespace HUB
{
    public partial class Characters : Form
    {
        private readonly HttpClient _httpClient = new HttpClient();
        public int orderByColumnId = 7;
        public bool descendingOrder = true;

        public Characters()
        {
            InitializeComponent();

            this.TopLevel = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Dock = DockStyle.Fill;            
            this.Show();
        }

        private void Characters_Load(object sender, EventArgs e)
        {

        }

        public void UpdateCharacteres()
        {
            try
            {
                switch (orderByColumnId)
                {
                    case 1:
                        if (descendingOrder)
                            Program.Characters = Program.Characters.OrderByDescending(c => c.name).ToList();
                        else
                            Program.Characters = Program.Characters.OrderBy(c => c.name).ToList();

                        break;
                    case 2:
                        if (descendingOrder)
                            Program.Characters = Program.Characters.OrderByDescending(c => c.level).ToList();
                        else
                            Program.Characters = Program.Characters.OrderBy(c => c.level).ToList();
                        break;
                    case 3:
                        if (descendingOrder)
                            Program.Characters = Program.Characters.OrderByDescending(c => c.vocation).ToList();
                        else
                            Program.Characters = Program.Characters.OrderBy(c => c.vocation).ToList();
                        break;
                    case 4:
                        if (descendingOrder)
                            Program.Characters = Program.Characters.OrderByDescending(c => c.script).ToList();
                        else
                            Program.Characters = Program.Characters.OrderBy(c => c.script).ToList();
                        break;
                    case 5:
                        if (descendingOrder)
                            Program.Characters = Program.Characters.OrderByDescending(c => c.balance).ToList();
                        else
                            Program.Characters = Program.Characters.OrderBy(c => c.balance).ToList();
                        break;
                    case 6:
                        if (descendingOrder)
                            Program.Characters = Program.Characters.OrderByDescending(c => c.actual_stamina).ToList();
                        else
                            Program.Characters = Program.Characters.OrderBy(c => c.actual_stamina).ToList();
                        break;
                    case 7:
                        if (descendingOrder)
                        {
                            List<Character> newCharList = new List<Character>();
                            newCharList = Program.Characters.Where(c => (DateTime.Now - DateTime.Parse(c.last_online)).TotalMinutes < 5).OrderBy(c => c.id).ToList();
                            newCharList.AddRange(Program.Characters.Where(c => (DateTime.Now - DateTime.Parse(c.last_online)).TotalMinutes >= 5).OrderByDescending(c => c.last_online).ToList());

                            Program.Characters = newCharList;
                        }
                        else
                            Program.Characters = Program.Characters.OrderBy(c => c.last_online).ToList();
                        break;
                    default:
                        break;
                }

                listView1.Items.Clear();

                foreach (Character ch in Program.Characters)
                {
                    DateTime lastOnlineTime = DateTime.Parse(ch.last_online);

                    if (!checkBox1.Checked && (DateTime.Now - lastOnlineTime).TotalMinutes >= 2)
                        continue;

                    if (!checkBox2.Checked && ch.status < 0)
                        continue;

                    if (textBox3.Text.Length > 0 && !ch.script.Contains(textBox3.Text))
                        continue;

                    if (comboBox2.Text != "Any" && !ch.server.Contains(comboBox2.Text))
                        continue;

                    int minLevel = 0;
                    if (int.TryParse(textBox4.Text, out minLevel) && ch.level < minLevel)
                        continue;

                    int minHours = 0;
                    int minMinutes = 0;
                    if (textBox5.Text.Contains(":"))
                    {
                        string[] timeSplit = textBox5.Text.Split(':');

                        int.TryParse(timeSplit[0], out minHours);
                        int.TryParse(timeSplit[1], out minMinutes);
                    }
                    else
                        int.TryParse(textBox5.Text, out minHours);

                    TimeSpan minStamina = new TimeSpan(minHours, minMinutes, 0);
                    if (ch.actual_stamina < minStamina)
                        continue;

                    ListViewItem lvItem = ch.ListViewItem;

                    if (ch.warning.Length > 0)
                    {
                        lvItem.ToolTipText += (lvItem.ToolTipText.Length > 0 ? Environment.NewLine : string.Empty) + "SCRIPT WARNINGS:" + Environment.NewLine + " - " + string.Join(Environment.NewLine + " - ", ch.warning.Split("|"));
                    }
                    if (ch.system_warning.Count > 0)
                    {
                        lvItem.ToolTipText += (lvItem.ToolTipText.Length > 0 ? Environment.NewLine : string.Empty) + "SYSTEM WARNINGS:" + Environment.NewLine + " - " + string.Join(Environment.NewLine + " - ", ch.system_warning);
                    }
                    if (ch.script_status.Length > 0)
                    {
                        //lvItem.ToolTipText += (lvItem.ToolTipText.Length > 0 ? Environment.NewLine : string.Empty) + "STATUS: " + ch.script_status;
                    }


                    if (ch.status <= -1)
                        lvItem.BackColor = Color.Gray;
                    else if (ch.system_warning.Count > 0)
                        lvItem.BackColor = Color.Red;
                    else if (ch.imbuement_time == 0)
                        lvItem.BackColor = Color.Blue;
                    else if (ch.warning.Length > 0)
                        lvItem.BackColor = Color.Yellow;
                    else if ((DateTime.Now - DateTime.Parse(ch.last_online)).TotalMinutes < 2)
                        lvItem.BackColor = Color.Green;

                    listView1.Items.Add(lvItem);
                }
            }
            catch
            {

            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Interval = 10000;

            try
            {
                Thread workerThread = new Thread(new ThreadStart(
                () =>
                {
                    try
                    {
                        this.Invoke((MethodInvoker)(
                            () =>
                            {
                                using (HttpResponseMessage response = _httpClient.GetAsync(string.Format("http://tibia.kzsoft.com.br/index.php?username={0}&password={1}&banned=true&json=true", Settings.Default.Username, Settings.Default.Password)).Result)
                                {
                                    if (response.IsSuccessStatusCode)
                                        Program.lastSuccessfulUpdate = DateTime.Now;

                                    using (HttpContent content = response.Content)
                                    {
                                        string result = content.ReadAsStringAsync().Result;

                                        List<Character> charList = JsonSerializer.Deserialize<List<Character>>(result);
                                        Program.Characters = charList;

                                        string selectedServer = comboBox2.Text;
                                        comboBox2.Items.Clear();
                                        comboBox2.Items.Add("Any");
                                        comboBox2.Items.AddRange(Program.Characters?.Where(c => c.server.Trim() != string.Empty).Select(c => c.server).Distinct().ToArray());
                                        comboBox2.Text = selectedServer;

                                        foreach (Character character in Program.Characters)
                                        {
                                            if (!Program.Config.Scripts.Exists(s => s.name == character.script))
                                                character.system_warning.Add("Script not found.");

                                            if (!Program.Config.Servers.Exists(s => s.name == character.server))
                                                character.system_warning.Add("Server not found.");
                                        }

                                        UpdateCharacteres();
                                    }
                                }
                            }));
                    }
                    catch
                    {

                    }
                }
                ));

                workerThread.Start();
            }
            catch
            {

            }
        }

        private void refreshSearch_Tick(object sender, EventArgs e)
        {
            UpdateCharacteres();
            refreshSearch.Enabled = false;
        }


        ToolTip mTooltip = new ToolTip() {  UseAnimation= false, UseFading=false};
        Point mLastPos = new Point(-1, -1);
        private void listview_ItemHover(object sender, ListViewItemMouseHoverEventArgs e)
        {
            mLastPos = listView1.PointToClient(Cursor.Position);
            mTooltip.Show(e.Item.ToolTipText, e.Item.ListView, mLastPos.X + 15, mLastPos.Y);
        }
        private void listview_MouseMove(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo info = listView1.HitTest(e.X, e.Y);

            if (info.Item == null && info.SubItem == null)
            {
                mTooltip.Show(string.Empty, listView1, e.X, e.Y);
            }
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == orderByColumnId)
                descendingOrder = !descendingOrder;
            else
                orderByColumnId = e.Column;
            UpdateCharacteres();
        }

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmd);

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo info = listView1.HitTest(e.X, e.Y);

            if (info.Item != null)
            {
                int accId = -1;
                int.TryParse(info.Item.Text, out accId);
                Character? ch = Program.Characters.FirstOrDefault(c => c.id == accId);

                if (ch != null)
                {
                    DateTime lastOnlineTime = DateTime.Parse(ch.last_online);
                    Process p = Process.GetProcesses().FirstOrDefault(p => p.MainWindowTitle == "Tibia - " + ch.name);
                    if (p !=null)
                    {
                        ShowWindow(p.MainWindowHandle, 3);
                        SetForegroundWindow(p.MainWindowHandle);
                    }
                    else if ((DateTime.Now - lastOnlineTime).TotalSeconds > 60)
                    {
                        Script script = Program.Config.Scripts.FirstOrDefault(s => s.name == ch.script);
                        Client client = Program.Config.Clients.FirstOrDefault(c => script.client == c.name);
                        Server server = Program.Config.Servers.FirstOrDefault(s => s.name == ch.server);
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
                        (10000000).ToString()
                    };

                        ProcessStartInfo startInfo = new ProcessStartInfo();
                        startInfo.WindowStyle = ProcessWindowStyle.Normal;
                        startInfo.CreateNoWindow = false;
                        startInfo.UseShellExecute = false;
                        startInfo.FileName = Settings.Default.BotPath;
                        startInfo.Arguments = string.Join(" ", argms);
                        Process.Start(startInfo);
                    }
                }
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateCharacteres();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCharacteres();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCharacteres();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            refreshSearch.Enabled = false;
            refreshSearch.Enabled = true;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            refreshSearch.Enabled = false;
            refreshSearch.Enabled = true;
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            refreshSearch.Enabled = false;
            refreshSearch.Enabled = true;
        }

        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new CreateCharacter().Show();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0) return;
            ListViewItem lvItem = listView1.SelectedItems[0];

            if (lvItem != null)
                new UpdateCharacter(int.Parse(lvItem.Text)).Show();
        }

        private async void banToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems.Count <= 0) return;
                ListViewItem lvItem = listView1.SelectedItems[0];
                if (lvItem == null) return;
                Character character = Program.Characters.FirstOrDefault(c => c.id == int.Parse(lvItem.Text));
                if (character == null) return;

                var client = new HttpClient();

                client.Timeout = TimeSpan.FromSeconds(5);
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
                client.DefaultRequestHeaders.Add("accept", "application/json, text/plain, */*");
                client.DefaultRequestHeaders.Add("accept-language", "en-US,en;q=0.9");

                string requestString = string.Format("https://tibia.kzsoft.com.br/status.php?username={0}&password={1}&char_name={2}&status={3}",
                     Settings.Default.Username,
                     Settings.Default.Password,
                     character.name,
                     character.status != -1 ? -1 : 1
                     );

                var response = await client.GetAsync(requestString);
                string content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode && content == "1")
                {
                    MessageBox.Show(string.Format("Character Set as {0}.", character.status != -1 ? "banned" : "unbanned"));
                }
                else
                    MessageBox.Show("There was an error.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error.");
                return;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<string> charNames = new List<string>();
            foreach (ListViewItem lvItem in listView1.Items)
                charNames.Add(lvItem.SubItems[1].Text);

            Clipboard.SetText(string.Join(Environment.NewLine, charNames));
        }
    }
}
