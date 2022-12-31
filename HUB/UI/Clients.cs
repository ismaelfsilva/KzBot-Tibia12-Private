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
    public partial class Clients : Form
    {

        public Clients()
        {
            InitializeComponent();

            this.TopLevel = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Dock = DockStyle.Fill;
            this.Show();
        }

        private void Clients_Load(object sender, EventArgs e)
        {
            foreach (Client c in Program.Config.Clients)
            {
                ListViewItem lvItem = new ListViewItem();

                lvItem.Text = c.id;
                lvItem.SubItems.Add(c.name);
                lvItem.SubItems.Add(c.file);
                lvItem.SubItems.Add(string.Empty);

                listView1.Items.Add(lvItem);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool hasSelectedItem = listView1.SelectedItems.Count > 0;

            textBox1.Enabled = hasSelectedItem;
            textBox2.Enabled = hasSelectedItem;

            if (hasSelectedItem)
            {
                Client client = Program.Config.Clients.FirstOrDefault(c => c.id == listView1.SelectedItems[0].Text);

                textBox1.Text = client.name;
                textBox2.Text = client.file;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Client client = new Client();
            Program.Config.Clients.Add(client);

            ListViewItem lvItem = new ListViewItem();

            lvItem.Text = client.id;
            lvItem.SubItems.Add(client.name);
            lvItem.SubItems.Add(client.file);
            lvItem.SubItems.Add(string.Empty);

            listView1.Items.Add(lvItem);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            ListViewItem lvItem = listView1.SelectedItems[0];

            if (lvItem != null)
            {
                Program.Config.Clients.FirstOrDefault(c => c.id == lvItem.Text).name = textBox1.Text;
                lvItem.SubItems[1].Text = textBox1.Text;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            ListViewItem lvItem = listView1.SelectedItems[0];
            if (lvItem != null)
            {
                Program.Config.Clients.FirstOrDefault(c => c.id == lvItem.Text).file = textBox2.Text;
                lvItem.SubItems[2].Text = textBox2.Text;
            }
        }
    }
}
