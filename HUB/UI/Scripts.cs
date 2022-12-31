using HUB.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace HUB.UI
{
    public partial class Scripts : Form
    {

        public Scripts()
        {
            InitializeComponent();

            this.TopLevel = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Dock = DockStyle.Fill;
            this.Show();
        }

        private void Scripts_Load(object sender, EventArgs e)
        {
            foreach (Script script in Program.Config.Scripts)
            {
                ListViewItem lvItem = new ListViewItem();

                lvItem.Text = script.id;
                lvItem.SubItems.Add(script.name);
                lvItem.SubItems.Add(script.client);
                lvItem.SubItems.Add(script.path);

                listView1.Items.Add(lvItem);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool hasSelectedItem = listView1.SelectedItems.Count > 0;

            textBox1.Enabled = hasSelectedItem;
            textBox2.Enabled = hasSelectedItem;
            textBox3.Enabled = hasSelectedItem;

            if (hasSelectedItem)
            {
                Script client = Program.Config.Scripts.FirstOrDefault(c => c.id == listView1.SelectedItems[0].Text);

                textBox1.Text = client.name;
                textBox2.Text = client.client;
                textBox3.Text = client.path;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Script script = new Script();
            Program.Config.Scripts.Add(script);

            ListViewItem lvItem = new ListViewItem();

            lvItem.Text = script.id;
            lvItem.SubItems.Add(script.name);
            lvItem.SubItems.Add(script.client);
            lvItem.SubItems.Add(script.path);

            listView1.Items.Add(lvItem);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            ListViewItem lvItem = listView1.SelectedItems[0];

            if (lvItem != null)
            {
                Program.Config.Scripts.FirstOrDefault(c => c.id == lvItem.Text).name = textBox1.Text;
                lvItem.SubItems[1].Text = textBox1.Text;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            ListViewItem lvItem = listView1.SelectedItems[0];

            if (lvItem != null)
            {
                Program.Config.Scripts.FirstOrDefault(c => c.id == lvItem.Text).client = textBox2.Text;
                lvItem.SubItems[2].Text = textBox2.Text;

                checkValidClient.Enabled = false;
                checkValidClient.Enabled = true;
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            ListViewItem lvItem = listView1.SelectedItems[0];

            if (lvItem != null)
            {
                Program.Config.Scripts.FirstOrDefault(c => c.id == lvItem.Text).path = textBox3.Text;
                lvItem.SubItems[3].Text = textBox3.Text;
            }
        }

        private void checkValidClient_Tick(object sender, EventArgs e)
        {
            // CHECK IF SCRIPTS HAVE WORKING SETUP
            foreach (Script script in Program.Config.Scripts)
            {
                ListViewItem lvItem = null;
                foreach (ListViewItem item in listView1.Items)
                {
                    if (item.Text == script.id)
                    {
                        lvItem = item;
                        break;
                    }
                }

                if (lvItem == null)
                    continue;

                if (!Program.Config.Clients.Exists(c => c.name == script.client))
                    lvItem.BackColor = Color.Red;
                else
                    lvItem.BackColor = Color.White;
            }

            checkValidClient.Enabled = false;
        }
    }
}
