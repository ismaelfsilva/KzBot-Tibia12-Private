using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KzBot.UI
{
    public partial class Refill : Form
    {
        public Refill()
        {
            InitializeComponent();

            this.TopLevel = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Dock = DockStyle.Fill;
            this.Show();
        }

        private void Refill_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            RefillRule rule = new RefillRule();
            Globals.Config.Refill.Add(rule);
            listView1.Items.Add(rule.ListViewItem());
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0)
                return;


            RefillRule rule = Globals.Config.Refill[listView1.SelectedIndices[0]];
            rule.Type = comboBox1.Text.ToString();
            listView1.SelectedItems[0].SubItems[1].Text = comboBox1.Text.ToString();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0)
                return;


            RefillRule rule = Globals.Config.Refill[listView1.SelectedIndices[0]];
            rule.Id = int.Parse(textBox1.Text.ToString());
            listView1.SelectedItems[0].SubItems[2].Text = textBox1.Text.ToString();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0)
                return;

            RefillRule rule = Globals.Config.Refill[listView1.SelectedIndices[0]];
            rule.Name = textBox2.Text.ToString();
            listView1.SelectedItems[0].SubItems[3].Text = textBox2.Text.ToString();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0)
                return;


            RefillRule rule = Globals.Config.Refill[listView1.SelectedIndices[0]];
            rule.ToBuy = int.Parse(textBox3.Text.ToString());
            listView1.SelectedItems[0].SubItems[4].Text = textBox3.Text.ToString();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0)
                return;


            RefillRule rule = Globals.Config.Refill[listView1.SelectedIndices[0]];
            rule.ToLeave = int.Parse(textBox4.Text.ToString());
            listView1.SelectedItems[0].SubItems[5].Text = textBox4.Text.ToString();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0)
            {
                comboBox1.Enabled = false;
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                textBox3.Enabled = false;
                textBox4.Enabled = false;
            }
            else
            {
                comboBox1.Enabled = true;
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                textBox3.Enabled = true;
                textBox4.Enabled = true;

                RefillRule rule = Globals.Config.Refill[listView1.SelectedIndices[0]];

                comboBox1.Text = rule.Type;
                textBox1.Text = rule.Id.ToString();
                textBox2.Text = rule.Name;
                textBox3.Text = rule.ToBuy.ToString();
                textBox4.Text = rule.ToLeave.ToString();
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to clear?", "Are you sure?", MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            Globals.Config.Refill.Clear();
            listView1.Items.Clear();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0)
                return;

            Globals.Config.Refill.RemoveAt(listView1.SelectedIndices[0]);
            listView1.Items.Remove(listView1.SelectedItems[0]);
        }

        private void knightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefillRule rule2 = new RefillRule() { Id = 23375, Name = "Supreme Health Potion", Type = "potions", Level=200, ToBuy = 50, ToLeave = 30, Vocation = Vocation.EK };
            Globals.Config.Refill.Add(rule2);
            listView1.Items.Add(rule2.ListViewItem());

            RefillRule rule3 = new RefillRule() { Id = 7643, Name = "Ultimate Health Potion", Type = "potions", Level = 130, MaxLevel = 200, ToBuy = 50, ToLeave = 30, Vocation = Vocation.EK };
            Globals.Config.Refill.Add(rule3);
            listView1.Items.Add(rule3.ListViewItem());

            RefillRule rule = new RefillRule() { Id = 237, Name = "Strong Mana Potion", Type = "potions", ToBuy = 800, ToLeave = 150, Vocation = Vocation.EK };
            Globals.Config.Refill.Add(rule);
            listView1.Items.Add(rule.ListViewItem());
        }

        private void paladinToolStripMenuItem_Click(object sender, EventArgs e)
        {

            RefillRule rule2 = new RefillRule() { Id = 23374, Name = "Ultimate Spirit Potion", Type = "potions", Level = 130, ToBuy = 200, ToLeave = 50, Vocation = Vocation.RP };
            Globals.Config.Refill.Add(rule2);
            listView1.Items.Add(rule2.ListViewItem());

            RefillRule rule6 = new RefillRule() { Id = 7642, Name = "Great Spirit Potion", Type = "potions", Level = 80, MaxLevel=130, ToBuy = 200, ToLeave = 50, Vocation = Vocation.RP };
            Globals.Config.Refill.Add(rule6);
            listView1.Items.Add(rule6.ListViewItem());

            RefillRule rule5 = new RefillRule() { Id = 3051, Name = "Energy Ring", Type = "trade", ToBuy = 2, ToLeave = 0, Vocation = Vocation.RP };
            Globals.Config.Refill.Add(rule5);
            listView1.Items.Add(rule5.ListViewItem());

            RefillRule rule3 = new RefillRule() { Id = 35901, Name = "Diamond Arrow", Type = "distance", ToBuy = 1200, ToLeave = 200, Vocation = Vocation.RP };
            Globals.Config.Refill.Add(rule3);
            listView1.Items.Add(rule3.ListViewItem());

            RefillRule rule4 = new RefillRule() { Id = 3161, Name = "Avalanche Rune", Type = "runes", ToBuy = 600, ToLeave = 100, Vocation = Vocation.RP };
            Globals.Config.Refill.Add(rule4);
            listView1.Items.Add(rule4.ListViewItem());

            RefillRule rule = new RefillRule() { Id = 238, Name = "Great Mana Potion", Type = "potions", ToBuy = 800, ToLeave = 200, Vocation = Vocation.RP };
            Globals.Config.Refill.Add(rule);
            listView1.Items.Add(rule.ListViewItem());
        }

        private void amuletOfLossToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefillRule rule = new RefillRule() { Id = 3057, Name = "Amulet of Loss", Type = "trade", ToBuy = 1, ToLeave = 0 };
            Globals.Config.Refill.Add(rule);
            listView1.Items.Add(rule.ListViewItem());
        }
    }
}
