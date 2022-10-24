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
    public partial class Cavebot : Form
    {

        public Position updatedPosition = new Position() { X=0, Y=0,Z=0};
        public WaypointType lastWaypointType = WaypointType.Nothing;

        public Cavebot()
        {
            InitializeComponent();

            this.TopLevel = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Dock = DockStyle.Fill;
            this.Show();
        }

        private void Cavebot_Load(object sender, EventArgs e)
        {
            comboBox1.Items.AddRange(Enum.GetNames(typeof(WaypointType)).Select(type => type.Replace("_", " ")).ToArray());
            comboBox1.SelectedIndex = 0;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            Waypoint waypoint = new Waypoint() { X = Convert.ToInt32(textBox1.Text), Y = Convert.ToInt32(textBox2.Text) , Z = Convert.ToInt32(textBox3.Text),  Label = textBox4.Text, Extra = textBox5.Text, Type = (WaypointType)Enum.Parse(typeof(WaypointType), comboBox1.Text.Replace(" ", "_")), rangeX = Convert.ToInt32(textBox6.Text), rangeY = Convert.ToInt32(textBox7.Text) };
            textBox4.Text = string.Empty;
            Globals.Config.Waypoints.Add(waypoint);
            listView1.Items.Add(waypoint.ListViewItem());
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0)
                return;

            Globals.WaypointId = listView1.SelectedIndices[0];
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            WaypointType t = (WaypointType)comboBox1.SelectedIndex;
            switch (t)
            {
                case WaypointType.Say:
                    textBox5.PlaceholderText = "Text To Say";
                    break;
                case WaypointType.Check_Cap:
                    textBox5.PlaceholderText = "Cap;Label";
                    break;
                case WaypointType.Check_Stamina:
                    textBox5.PlaceholderText = "Hours;Label";
                    break;
                case WaypointType.Check_Level:
                    textBox5.PlaceholderText = "Level;Label";
                    break;
                case WaypointType.Check_Refill:
                    textBox5.PlaceholderText = "Item Id;Qty;Label";
                    textBox5.Text = "Item Id;Qty;Label";
                    return;
                case WaypointType.Buy_Refill:
                    textBox5.PlaceholderText = "Type;Item Name;Qty;Item Id";
                    textBox5.Text = "Type;Item Name;Qty;Item Id";
                    return;
                case WaypointType.Press:
                case WaypointType.Use_On:
                    textBox5.PlaceholderText = "Key Number";
                    break;
                default:
                    textBox5.PlaceholderText = "Extra";
                    break;
            }

            if (lastWaypointType != t)
                textBox5.Text = string.Empty;

            lastWaypointType = t;
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to clear?", "Are you sure?", MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            Globals.Config.Waypoints.Clear();
            listView1.Items.Clear();
            Globals.WaypointId = 0;            
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0)
                return;

            Globals.Config.Waypoints.RemoveAt(listView1.SelectedIndices[0]);
            listView1.Items.Remove(listView1.SelectedItems[0]);            
        }

        private void updatePositionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0)
                return;

            Waypoint waypoint = Globals.Config.Waypoints[listView1.SelectedIndices[0]];
            ListViewItem lvItem = listView1.Items[listView1.SelectedIndices[0]];

            waypoint.X = int.Parse(textBox1.Text);
            waypoint.Y = int.Parse(textBox2.Text);
            waypoint.Z = int.Parse(textBox3.Text);

            lvItem.SubItems[3].Text = String.Format("x:{0}, y:{1}, z:{2}", waypoint.X, waypoint.Y, waypoint.Z);
        }
    }
}
