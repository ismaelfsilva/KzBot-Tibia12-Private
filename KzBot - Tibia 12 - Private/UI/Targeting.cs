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
    public partial class Targeting : Form
    {
        public Targeting()
        {
            InitializeComponent();

            this.TopLevel = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Dock = DockStyle.Fill;
            this.Show();
        }

        private void Targeting_Load(object sender, EventArgs e)
        {
            comboBox1.Items.AddRange(Enum.GetNames(typeof(TargetType)).Select(type => type.Replace("_", " ")).ToArray());
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TargetRule TargetRule = new TargetRule();
            Globals.Config.Targeting.Add(TargetRule);
            listView1.Items.Add(TargetRule.ListViewItem());
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0)
            {
                button2.Enabled = false;
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                textBox3.Enabled = false;
                comboBox1.Enabled = false;
                comboBox2.Enabled = false;
            }
            else
            {
                TargetRule rule = Globals.Config.Targeting[listView1.SelectedIndices[0]];

                button2.Text = rule.Key.ToString();
                textBox1.Text = rule.Mana.ToString();
                textBox2.Text = rule.CreatureCount.ToString();
                textBox3.Text = rule.Range.ToString();
                comboBox1.Text = rule.Type.ToString();
                comboBox2.Text = rule.PlayerOnCenter ? "Player" : "Target";

                button2.Enabled = true;
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                textBox3.Enabled = true;
                comboBox1.Enabled = true;
                comboBox2.Enabled = true;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0)
                return;

            TargetRule rule = Globals.Config.Targeting[listView1.SelectedIndices[0]];
            rule.Type = (TargetType)Enum.Parse(typeof(TargetType), comboBox1.Text);
            listView1.SelectedItems[0].SubItems[1].Text = rule.Type.ToString();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0)
                return;

            TargetRule rule = Globals.Config.Targeting[listView1.SelectedIndices[0]];
            rule.Mana = Convert.ToInt32(textBox1.Text);
            listView1.SelectedItems[0].SubItems[6].Text = rule.Mana.ToString();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0)
                return;

            TargetRule rule = Globals.Config.Targeting[listView1.SelectedIndices[0]];
            rule.CreatureCount = Convert.ToInt32(textBox2.Text);
            listView1.SelectedItems[0].SubItems[5].Text = rule.CreatureCount.ToString();
        }

        private void button2_KeyDown(object sender, KeyEventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0)
                return;

            TargetRule rule = Globals.Config.Targeting[listView1.SelectedIndices[0]];
            rule.Key = e.KeyCode;
            listView1.SelectedItems[0].SubItems[2].Text = e.KeyCode.ToString();
            button2.Text = e.KeyCode.ToString();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0)
                return;

            TargetRule rule = Globals.Config.Targeting[listView1.SelectedIndices[0]];
            rule.PlayerOnCenter = comboBox2.Text == "Player";
            listView1.SelectedItems[0].SubItems[3].Text = comboBox2.Text;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0)
                return;

            TargetRule rule = Globals.Config.Targeting[listView1.SelectedIndices[0]];
            rule.Range = Convert.ToInt32(textBox3.Text);
            listView1.SelectedItems[0].SubItems[4].Text = rule.Range.ToString();
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to clear?", "Are you sure?", MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            Globals.Config.Targeting.Clear();
            listView1.Items.Clear();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0)
                return;

            Globals.Config.Targeting.RemoveAt(listView1.SelectedIndices[0]);
            listView1.Items.Remove(listView1.SelectedItems[0]);
        }
    }
}
