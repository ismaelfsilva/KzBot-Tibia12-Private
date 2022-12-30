using HUB.Properties;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using System.Threading.Channels;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using ToolTip = System.Windows.Forms.ToolTip;

namespace HUB
{
    public partial class Main : Form
    {
        private readonly HttpClient _httpClient = new HttpClient();
        public List<Character> Characters = new List<Character>();
        public int orderByColumnId = 7;
        public bool descendingOrder = true;

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            textBox1.Text = Settings.Default.Username;
            textBox2.Text = Settings.Default.Password;
            comboBox1.SelectedIndex = 0;


        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.Username = textBox1.Text;
            Settings.Default.Save();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.Password = textBox2.Text;
            Settings.Default.Save();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateCharacteres();
        }

        public void UpdateCharacteres()
        {
            switch (orderByColumnId)
            {
                case 1:
                    if (descendingOrder)
                        Characters = Characters.OrderByDescending(c => c.name).ToList();
                    else
                        Characters = Characters.OrderBy(c => c.name).ToList();

                    break;
                case 2:
                    if (descendingOrder)
                        Characters = Characters.OrderByDescending(c => c.level).ToList();
                    else
                        Characters = Characters.OrderBy(c => c.level).ToList();
                    break;
                case 3:
                    if (descendingOrder)
                        Characters = Characters.OrderByDescending(c => c.vocation).ToList();
                    else
                        Characters = Characters.OrderBy(c => c.vocation).ToList();
                    break;
                case 4:
                    if (descendingOrder)
                        Characters = Characters.OrderByDescending(c => c.script).ToList();
                    else
                        Characters = Characters.OrderBy(c => c.script).ToList();
                    break;
                case 5:
                    if (descendingOrder)
                        Characters = Characters.OrderByDescending(c => c.balance).ToList();
                    else
                        Characters = Characters.OrderBy(c => c.balance).ToList();
                    break;
                case 6:
                    if (descendingOrder)
                        Characters = Characters = Characters.OrderByDescending(c => c.actual_stamina).ToList();
                    else
                        Characters = Characters = Characters.OrderBy(c => c.actual_stamina).ToList();
                    break;
                case 7:
                    if (descendingOrder)
                        Characters = Characters.OrderByDescending(c => c.last_online).ToList();
                    else
                        Characters = Characters.OrderBy(c => c.last_online).ToList();
                    break;
                default:
                    break;
            }

            listView1.Items.Clear();

            foreach (Character ch in Characters)
            {
                DateTime lastOnlineTime = DateTime.Parse(ch.last_online);

                if (!checkBox1.Checked && (DateTime.Now - lastOnlineTime).TotalMinutes >= 30)
                    continue;

                if (!checkBox2.Checked && ch.status < 0)
                    continue;

                if (textBox3.Text.Length > 0 && !ch.script.Contains(textBox3.Text))
                    continue;

                if (comboBox1.Text != "Any" && !ch.server.Contains(comboBox1.Text))
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

                lvItem.ToolTipText = ch.warning.Replace("; ", Environment.NewLine);

                if (lvItem.ToolTipText.Length > 0)
                    lvItem.BackColor = Color.Yellow;
                else if ((DateTime.Now - DateTime.Parse(ch.last_online)).TotalMinutes < 10)
                    lvItem.BackColor = Color.Green;

                listView1.Items.Add(lvItem);
            }
        }

        ToolTip mTooltip;
        Point mLastPos = new Point(-1, -1);
        private void listview_MouseMove(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo info = listView1.HitTest(e.X, e.Y);

            if (mTooltip == null)
                mTooltip = new ToolTip();

            if (mLastPos != e.Location)
            {
                if (info.Item != null && info.SubItem != null && info.Item.ToolTipText.Length > 0)
                {
                    mTooltip.Show(info.Item.ToolTipText, info.Item.ListView, e.X, e.Y, 20000);
                }
                else
                {
                    mTooltip.SetToolTip(listView1, string.Empty);
                }
            }

            mLastPos = e.Location;
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == orderByColumnId)
                descendingOrder = !descendingOrder;
            else
                orderByColumnId = e.Column;
            UpdateCharacteres();
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Interval = 10000;

            try
            {
                Thread workerThread = new Thread(new ThreadStart(
                () =>
                    {
                        this.Invoke((MethodInvoker)(
                            () =>
                            {
                                using (HttpResponseMessage response = _httpClient.GetAsync(string.Format("http://tibia.kzsoft.com.br/index.php?username={0}&password={1}&json=true", textBox1.Text, textBox2.Text)).Result)
                                {
                                    using (HttpContent content = response.Content)
                                    {
                                        string result = content.ReadAsStringAsync().Result;

                                        Characters = JsonSerializer.Deserialize<List<Character>>(result);

                                        string selectedServer = comboBox1.Text;
                                        comboBox1.Items.Clear();
                                        comboBox1.Items.Add("Any");
                                        comboBox1.Items.AddRange(Characters?.Select(c => c.server).Distinct().ToArray());
                                        comboBox1.Text = selectedServer;

                                        UpdateCharacteres();
                                    }
                                }
                            }));
                    }
                ));

                workerThread.Start();
            }
            catch
            {

            }
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

        private void refreshSearch_Tick(object sender, EventArgs e)
        {
            UpdateCharacteres();
            refreshSearch.Enabled = false;
        }
    }
}