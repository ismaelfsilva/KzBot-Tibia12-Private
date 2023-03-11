using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HUB.Classes;

namespace HUB
{
    public partial class Servers : Form
    {


        public Servers()
        {
            InitializeComponent();

            this.TopLevel = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Dock = DockStyle.Fill;
            this.Show();
        }

        private void Servers_Load(object sender, EventArgs e)
        {
            foreach (Server server in Program.Config.Servers)
            {
                ListViewItem lvItem = new ListViewItem();

                lvItem.Text = server.id;
                lvItem.SubItems.Add(server.name);
                lvItem.SubItems.Add(server.version);
                lvItem.SubItems.Add(server.path);
                lvItem.SubItems.Add(server.websiteNoCF);
                lvItem.SubItems.Add(server.autoLootId.ToString());

                listView1.Items.Add(lvItem);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Server server = new Server();
            Program.Config.Servers.Add(server);

            ListViewItem lvItem = new ListViewItem();

            lvItem.Text = server.id;
            lvItem.SubItems.Add(server.name);
            lvItem.SubItems.Add(server.version);
            lvItem.SubItems.Add(server.path);
            lvItem.SubItems.Add(server.websiteNoCF);
            lvItem.SubItems.Add(server.autoLootId.ToString());

            listView1.Items.Add(lvItem);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool hasSelectedItem = listView1.SelectedItems.Count > 0;

            textBox1.Enabled = hasSelectedItem;
            textBox2.Enabled = hasSelectedItem;
            textBox3.Enabled = hasSelectedItem;
            textBox4.Enabled = hasSelectedItem;
            textBox5.Enabled = hasSelectedItem;

            if (hasSelectedItem)
            {
                Server server = Program.Config.Servers.FirstOrDefault(c => c.id == listView1.SelectedItems[0].Text);

                textBox1.Text = server.name;
                textBox2.Text = server.version;
                textBox3.Text = server.path;
                textBox4.Text = server.websiteNoCF;
                textBox5.Text = server.autoLootId.ToString();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            ListViewItem lvItem = listView1.SelectedItems[0];

            if (lvItem != null)
            {
                Program.Config.Servers.FirstOrDefault(c => c.id == lvItem.Text).name = textBox1.Text;
                lvItem.SubItems[1].Text = textBox1.Text;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            ListViewItem lvItem = listView1.SelectedItems[0];

            if (lvItem != null)
            {
                Program.Config.Servers.FirstOrDefault(c => c.id == lvItem.Text).version = textBox2.Text;
                lvItem.SubItems[2].Text = textBox2.Text;
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            ListViewItem lvItem = listView1.SelectedItems[0];

            if (lvItem != null)
            {
                Program.Config.Servers.FirstOrDefault(c => c.id == lvItem.Text).path = textBox3.Text;
                lvItem.SubItems[3].Text = textBox3.Text;
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            ListViewItem lvItem = listView1.SelectedItems[0];

            if (lvItem != null)
            {
                Program.Config.Servers.FirstOrDefault(c => c.id == lvItem.Text).websiteNoCF = textBox4.Text;
                lvItem.SubItems[4].Text = textBox4.Text;
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            ListViewItem lvItem = listView1.SelectedItems[0];

            if (lvItem != null)
            {
                int autoLootId = -1;
                if (!int.TryParse(textBox5.Text, out autoLootId))
                    autoLootId = -1;

                Program.Config.Servers.FirstOrDefault(c => c.id == lvItem.Text).autoLootId = autoLootId;
                lvItem.SubItems[5].Text = textBox5.Text;
            }
        }
    }
}
