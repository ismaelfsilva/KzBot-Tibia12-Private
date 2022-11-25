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
    public partial class Healer : Form
    {
        public Healer()
        {
            InitializeComponent();

            this.TopLevel = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Dock = DockStyle.Fill;
            this.Show();
        }

        private void Healer_Load(object sender, EventArgs e)
        {
            comboBox1.Items.AddRange(Enum.GetNames(typeof(HealType)).Select(type => type.Replace("_", " ")).ToArray());
            comboBox1.SelectedIndex = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            HealRule HealRule = new HealRule() { HpMin = 0, HpMax = 80, MpMax = 30, MpMin = 5, Delay = 500, Type = HealType.Nothing};
            Globals.Config.Healer.Add(HealRule);
            listView1.Items.Add(HealRule.ListViewItem());
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0)
            {
                button1.Enabled = false;
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                textBox3.Enabled = false;
                textBox4.Enabled = false;
                textBox5.Enabled = false;
                comboBox1.Enabled = false;
            }
            else
            {
                HealRule rule = Globals.Config.Healer[listView1.SelectedIndices[0]];

                button1.Text = rule.Key.ToString();
                textBox1.Text = rule.HpMin.ToString();
                textBox2.Text = rule.HpMax.ToString();
                textBox3.Text = rule.MpMin.ToString();
                textBox4.Text = rule.MpMax.ToString();
                textBox5.Text = rule.Delay.ToString();

                comboBox1.Text = rule.Type.ToString();

                button1.Enabled = true;
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                textBox3.Enabled = true;
                textBox4.Enabled = true;
                textBox5.Enabled = true;
                comboBox1.Enabled = true;
            }
        }

        private void button1_KeyDown(object sender, KeyEventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0)
                return;

            HealRule rule = Globals.Config.Healer[listView1.SelectedIndices[0]];
            rule.Key = e.KeyCode;
            listView1.SelectedItems[0].SubItems[4].Text = e.KeyCode.ToString();
            button1.Text = e.KeyCode.ToString();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0)
                return;

            HealRule rule = Globals.Config.Healer[listView1.SelectedIndices[0]];
            rule.Type = (HealType)Enum.Parse(typeof(HealType), comboBox1.Text);
            listView1.SelectedItems[0].SubItems[1].Text = rule.Type.ToString();
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0)
                return;

            HealRule rule = Globals.Config.Healer[listView1.SelectedIndices[0]];
            rule.Delay = Convert.ToInt32(textBox5.Text);
            listView1.SelectedItems[0].SubItems[5].Text = rule.Delay.ToString();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0)
                return;

            HealRule rule = Globals.Config.Healer[listView1.SelectedIndices[0]];
            rule.HpMin = Convert.ToInt32(textBox1.Text);
            listView1.SelectedItems[0].SubItems[2].Text = String.Format("{0} to {1}", rule.HpMin, rule.HpMax);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0)
                return;

            HealRule rule = Globals.Config.Healer[listView1.SelectedIndices[0]];
            rule.HpMax = Convert.ToInt32(textBox2.Text);
            listView1.SelectedItems[0].SubItems[2].Text = String.Format("{0} to {1}", rule.HpMin, rule.HpMax);
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0)
                return;

            HealRule rule = Globals.Config.Healer[listView1.SelectedIndices[0]];
            rule.MpMin = Convert.ToInt32(textBox3.Text);
            listView1.SelectedItems[0].SubItems[3].Text = String.Format("{0} to {1}", rule.MpMin, rule.MpMax);
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0)
                return;

            HealRule rule = Globals.Config.Healer[listView1.SelectedIndices[0]];
            rule.MpMax = Convert.ToInt32(textBox4.Text);
            listView1.SelectedItems[0].SubItems[3].Text = String.Format("{0} to {1}", rule.MpMin, rule.MpMax);
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to clear?", "Are you sure?", MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            Globals.Config.Healer.Clear();
            listView1.Items.Clear();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0)
                return;

            Globals.Config.Healer.RemoveAt(listView1.SelectedIndices[0]);
            listView1.Items.Remove(listView1.SelectedItems[0]);
        }

        private void knightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // SUPREME HEALTH POTION
            HealRule HealRule1 = new HealRule() { HpMin = 0, HpMax = 50, MpMin = 0, MpMax = 100, Type = HealType.Item, Vocation = Vocation.EK, Key = Keys.F4 };
            Globals.Config.Healer.Add(HealRule1);
            listView1.Items.Add(HealRule1.ListViewItem());

            // EXURA GRAN ICO
            HealRule HealRule2 = new HealRule() { HpMin = 0, HpMax = 30, MpMin = 200, MpMax = 99999, Delay = 600000, Type = HealType.Spell, Level = 80, Vocation = Vocation.EK, Key=Keys.F1 };
            Globals.Config.Healer.Add(HealRule2);
            listView1.Items.Add(HealRule2.ListViewItem());

            // EXURA MED ICO / EXURA ICO
            HealRule HealRule3 = new HealRule() { HpMin = 0, HpMax = 90, MpMin = 40, MpMax = 99999, Type = HealType.Spell, Level = 10, Vocation = Vocation.EK, Key = Keys.F2 };
            Globals.Config.Healer.Add(HealRule3);
            listView1.Items.Add(HealRule3.ListViewItem());

            // STRONG MANA POTION
            HealRule HealRule4 = new HealRule() { HpMin = 50, HpMax = 100, MpMin = 0, MpMax = 80, Type = HealType.Item, Vocation = Vocation.EK, Key=Keys.F3 };
            Globals.Config.Healer.Add(HealRule4);
            listView1.Items.Add(HealRule4.ListViewItem());
        }

        private void paladinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // ULTIMATE SPIRIT POTION
            HealRule HealRule1 = new HealRule() { HpMin = 0, HpMax = 60, MpMin = 0, MpMax = 100, Type = HealType.Item, Vocation = Vocation.RP, Key = Keys.OemBackslash };
            Globals.Config.Healer.Add(HealRule1);
            listView1.Items.Add(HealRule1.ListViewItem());

            // EXURA GRAN SAN
            HealRule HealRule2 = new HealRule() { HpMin = 0, HpMax = 90, MpMin = 210, MpMax = 99999, Type = HealType.Spell, Level = 60, Vocation = Vocation.RP, Key=Keys.OemPeriod };
            Globals.Config.Healer.Add(HealRule2);
            listView1.Items.Add(HealRule2.ListViewItem());

            // ENERGY RING
            HealRule HealRule3 = new HealRule() { HpMin = 0, HpMax = 50, MpMin = 20, MpMax = 100, Type = HealType.EnergyRing, Vocation = Vocation.RP, Key = Keys.Home };
            Globals.Config.Healer.Add(HealRule3);
            listView1.Items.Add(HealRule3.ListViewItem());

            // GREAT MANA POTION
            HealRule HealRule4 = new HealRule() { HpMin = 60, HpMax = 100, MpMin = 0, MpMax = 90, Type = HealType.Item, Vocation = Vocation.RP, Key=Keys.OemQuestion };
            Globals.Config.Healer.Add(HealRule4);
            listView1.Items.Add(HealRule4.ListViewItem());
        }
    }
}
