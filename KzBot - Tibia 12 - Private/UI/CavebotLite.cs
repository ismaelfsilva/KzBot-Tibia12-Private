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
    public partial class CavebotLite : Form
    {

        WaypointType defaultType = WaypointType.Stand;

        int xDiff = 0;
        int yDiff = 0;

        public CavebotLite()
        {
            InitializeComponent();
        }

        private void CavebotLite_Load(object sender, EventArgs e)
        {

        }

        private void CavebotLite_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void radioButton11_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton11.Checked)
            {
                xDiff = 0;
                yDiff = 0;
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                xDiff = 0;
                yDiff = -1;
            }
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton5.Checked)
            {
                xDiff = 1;
                yDiff = 0;
            }
        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton7.Checked)
            {
                xDiff = 0;
                yDiff = 1;
            }
        }

        private void radioButton9_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton9.Checked)
            {
                xDiff = -1;
                yDiff = 0;
            }
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked)
            {
                xDiff = 1;
                yDiff = -1;
            }
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton6.Checked)
            {
                xDiff = 1;
                yDiff = 1;
            }
        }

        private void radioButton8_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton8.Checked)
            {
                xDiff = -1;
                yDiff = 1;
            }
        }

        private void radioButton10_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton10.Checked)
            {
                xDiff = -1;
                yDiff = -1;
            }
        }

        Waypoint GenerateWaypoint(WaypointType type, int rangeX = -1, int rangeY = -1)
        {
            if (rangeX == -1)
                rangeX = Convert.ToInt32(numericUpDown1.Value);

            if (rangeY == -1)
                rangeY = Convert.ToInt32(numericUpDown2.Value);

            Position playerPos = Objects.Player.Position;
            Waypoint waypoint = new Waypoint() { X = playerPos.X + xDiff, Y = playerPos.Y + yDiff, Z = playerPos.Z, Type = type, rangeX = rangeX, rangeY = rangeY };
            return waypoint;
        }

        Waypoint GenerateWaypoint(WaypointType type, string extra, int rangeX = -1, int rangeY = -1)
        {
            if (rangeX == -1)
                rangeX = Convert.ToInt32(numericUpDown1.Value);

            if (rangeY == -1)
                rangeY = Convert.ToInt32(numericUpDown2.Value);

            Position playerPos = Objects.Player.Position;
            Waypoint waypoint = new Waypoint() { X = playerPos.X + xDiff, Y = playerPos.Y + yDiff, Z = playerPos.Z, Type = type, rangeX = rangeX, rangeY = rangeY, Extra = extra };
            return waypoint;
        }
        #region "Default"

        public static int lureId = 0;

        private void button1_Click(object sender, EventArgs e)
        {
            Waypoint waypoint = GenerateWaypoint(WaypointType.Stand);
            Globals.Config.Waypoints.Add(waypoint);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint.ListViewItem());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Waypoint waypoint = GenerateWaypoint(WaypointType.Node);
            Globals.Config.Waypoints.Add(waypoint);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint.ListViewItem());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Waypoint waypoint = GenerateWaypoint(WaypointType.Stand);
            waypoint.Label = "Lure" + ++lureId;
            Globals.Config.Waypoints.Add(waypoint);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint.ListViewItem());

            Waypoint waypoint2 = GenerateWaypoint(WaypointType.Lure);
            Globals.Config.Waypoints.Add(waypoint2);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint2.ListViewItem());

            Waypoint waypoint3 = GenerateWaypoint(WaypointType.Loot);
            Globals.Config.Waypoints.Add(waypoint3);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint3.ListViewItem());
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Waypoint waypoint = GenerateWaypoint(WaypointType.Not_Location_Goback);
            Globals.Config.Waypoints.Add(waypoint);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint.ListViewItem());
        }

        private void button25_Click(object sender, EventArgs e)
        {
            Waypoint waypoint = GenerateWaypoint(WaypointType.Use);
            Globals.Config.Waypoints.Add(waypoint);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint.ListViewItem());
        }
        #endregion

        #region "Labels"
        private void button13_Click(object sender, EventArgs e)
        {
            Waypoint waypoint = GenerateWaypoint(WaypointType.Enable_Targeting, 5, 5);
            waypoint.Label = button13.Text.Replace(" ", "_");
            Globals.Config.Waypoints.Add(waypoint);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint.ListViewItem());

            Waypoint waypoint2 = GenerateWaypoint(WaypointType.Enable_Alerts, 5, 5);
            Globals.Config.Waypoints.Add(waypoint2);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint2.ListViewItem());
        }

        private void button14_Click(object sender, EventArgs e)
        {
            Waypoint waypoint = new Waypoint() { X = 0, Y = 0, Z = 0, Type = WaypointType.Disable_Targeting, rangeX = Convert.ToInt32(numericUpDown1.Value), rangeY = Convert.ToInt32(numericUpDown2.Value) };
            waypoint.Label = button14.Text.Replace(" ", "_");
            Globals.Config.Waypoints.Add(waypoint);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint.ListViewItem());

            Waypoint waypoint2 = new Waypoint() { X = 0, Y = 0, Z = 0, Type = WaypointType.Disable_Alerts, rangeX = Convert.ToInt32(numericUpDown1.Value), rangeY = Convert.ToInt32(numericUpDown2.Value) };
            Globals.Config.Waypoints.Add(waypoint2);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint2.ListViewItem());
        }

        private void button15_Click(object sender, EventArgs e)
        {
            Waypoint waypoint = new Waypoint() { X = 0, Y = 0, Z = 0, Type = WaypointType.Nothing, rangeX = Convert.ToInt32(numericUpDown1.Value), rangeY = Convert.ToInt32(numericUpDown2.Value) };
            waypoint.Label = button15.Text.Replace(" ", "_");
            Globals.Config.Waypoints.Add(waypoint);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint.ListViewItem());
        }

        private void button16_Click(object sender, EventArgs e)
        {
            Waypoint waypoint = new Waypoint() { X = 0, Y = 0, Z = 0, Type = WaypointType.Nothing, rangeX = Convert.ToInt32(numericUpDown1.Value), rangeY = Convert.ToInt32(numericUpDown2.Value) };
            waypoint.Label = button16.Text.Replace(" ", "_");
            Globals.Config.Waypoints.Add(waypoint);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint.ListViewItem());
        }

        private void button17_Click(object sender, EventArgs e)
        {
            Waypoint waypoint = new Waypoint() { X = 0, Y = 0, Z = 0, Type = WaypointType.Nothing, rangeX = Convert.ToInt32(numericUpDown1.Value), rangeY = Convert.ToInt32(numericUpDown2.Value) };
            waypoint.Label = button17.Text.Replace(" ", "_");
            Globals.Config.Waypoints.Add(waypoint);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint.ListViewItem());
        }


        #endregion

        #region "Refill"
        private void button10_Click(object sender, EventArgs e)
        {
            Waypoint waypoint = GenerateWaypoint(WaypointType.Stand, 2, 2);
            waypoint.Label = "Sell_Items";
            Globals.Config.Waypoints.Add(waypoint);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint.ListViewItem());

            Waypoint waypoint2 = GenerateWaypoint(WaypointType.Sell_All, 2, 2);
            Globals.Config.Waypoints.Add(waypoint2);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint2.ListViewItem());

            Waypoint waypoint3 = GenerateWaypoint(WaypointType.Check_Cap, "2000; Sell_Items", 2, 2);
            Globals.Config.Waypoints.Add(waypoint3);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint3.ListViewItem());
        }

        private void button11_Click(object sender, EventArgs e)
        {
            Waypoint waypoint = GenerateWaypoint(WaypointType.Stand, 2, 2);
            waypoint.Label = "Refill";
            Globals.Config.Waypoints.Add(waypoint);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint.ListViewItem());

            Waypoint waypoint2 = GenerateWaypoint(WaypointType.Buy_Refill, 2, 2);
            Globals.Config.Waypoints.Add(waypoint2);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint2.ListViewItem());

            Waypoint waypoint3 = GenerateWaypoint(WaypointType.Check_Refill, "Refill", 2, 2);
            Globals.Config.Waypoints.Add(waypoint3);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint3.ListViewItem());
        }

        private void button18_Click(object sender, EventArgs e)
        {
            Waypoint waypoint = GenerateWaypoint(WaypointType.Stand, 1, 1);
            waypoint.Label = "Bank";
            Globals.Config.Waypoints.Add(waypoint);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint.ListViewItem());

            Waypoint waypoint2 = GenerateWaypoint(WaypointType.Deposit_All, 1, 1);
            Globals.Config.Waypoints.Add(waypoint2);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint2.ListViewItem());

            Waypoint waypoint3 = GenerateWaypoint(WaypointType.Transfer, 1, 1);
            Globals.Config.Waypoints.Add(waypoint3);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint3.ListViewItem());
        }

        private void button12_Click(object sender, EventArgs e)
        {
            Waypoint waypoint = GenerateWaypoint(WaypointType.Check_Stamina, "14;Exit_Trainer", 3, 3);
            waypoint.Label = "Check_Stamina";
            Globals.Config.Waypoints.Add(waypoint);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint.ListViewItem());

            Waypoint waypoint2 = GenerateWaypoint(WaypointType.Check_Safe, "Exit_Trainer", 3, 3);
            waypoint2.Label = "Check_Safe";
            Globals.Config.Waypoints.Add(waypoint2);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint2.ListViewItem());

            Waypoint waypoint3 = GenerateWaypoint(WaypointType.Check_Imbue, "Exit_Trainer", 3, 3);
            waypoint3.Label = "Check_Safe_Imbue";
            Globals.Config.Waypoints.Add(waypoint3);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint3.ListViewItem());
        }
        private void button3_Click(object sender, EventArgs e)
        {
            Waypoint waypoint = GenerateWaypoint(WaypointType.Goto_Label, "Goto_Hunt", 3, 3);
            Globals.Config.Waypoints.Add(waypoint);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint.ListViewItem());
        }
        #endregion

        #region "Hunt"
        public int doorId = 0;
        public int ladderId = 0;
        public int exaniTeraId = 0;
        public int holeId = 0;
        public int stairsId = 0;
        public int teleportId = 0;
        private void button20_Click(object sender, EventArgs e)
        {
            Position playerPos = Objects.Player.Position;
            Waypoint waypoint = new Waypoint() { X = playerPos.X, Y = playerPos.Y, Z = playerPos.Z, Type = WaypointType.Stand, rangeX = 1, rangeY = 1 };

            while (Globals.Config.Waypoints.Exists(w=> w.Label == waypoint.Label))
                waypoint.Label = "Door" + ++doorId;

            Globals.Config.Waypoints.Add(waypoint);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint.ListViewItem());

            Waypoint waypoint2 = GenerateWaypoint(WaypointType.Use, 1, 1);
            Globals.Config.Waypoints.Add(waypoint2);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint2.ListViewItem());
            
            Waypoint waypoint4 = GenerateWaypoint(WaypointType.Not_Location_Goto_Label, "Door" + doorId, 1, 1);
            Globals.Config.Waypoints.Add(waypoint4);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint4.ListViewItem());

            radioButton11.Checked = true;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Position playerPos = Objects.Player.Position;
            Waypoint waypoint = new Waypoint() { X = playerPos.X, Y = playerPos.Y, Z = playerPos.Z, Type = WaypointType.Stand, rangeX = 1, rangeY = 1 };

            while (Globals.Config.Waypoints.Exists(w => w.Label == waypoint.Label))
                waypoint.Label = "Door" + ++doorId;

            Globals.Config.Waypoints.Add(waypoint);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint.ListViewItem());

            Waypoint waypoint2 = GenerateWaypoint(WaypointType.Use, 1, 1);
            Globals.Config.Waypoints.Add(waypoint2);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint2.ListViewItem());

            Waypoint waypoint3 = new Waypoint() { X = playerPos.X, Y = playerPos.Y, Z = playerPos.Z, Type = WaypointType.Press, rangeX = 1, rangeY = 1 };
            if (radioButton3.Checked)
                waypoint3.Extra = Keys.Up.ToString();
            else if (radioButton5.Checked)
                waypoint3.Extra = Keys.Left.ToString();
            else if (radioButton7.Checked)
                waypoint3.Extra = Keys.Down.ToString();
            else if (radioButton9.Checked)
                waypoint3.Extra = Keys.Left.ToString();

            Globals.Config.Waypoints.Add(waypoint3);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint3.ListViewItem());

            Waypoint waypoint4 = GenerateWaypoint(WaypointType.Not_Location_Goto_Label, "Door" + doorId, 1, 1);
            Globals.Config.Waypoints.Add(waypoint4);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint4.ListViewItem());

            radioButton11.Checked = true;
        }

        private void button21_Click(object sender, EventArgs e)
        {
            Waypoint waypoint = GenerateWaypoint(WaypointType.Stand, 2, 2);

            while (Globals.Config.Waypoints.Exists(w => w.Label == waypoint.Label))
                waypoint.Label = "Ladder" + ++ladderId;

            Globals.Config.Waypoints.Add(waypoint);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint.ListViewItem());

            Waypoint waypoint2 = GenerateWaypoint(WaypointType.Use, 1, 1);
            Globals.Config.Waypoints.Add(waypoint2);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint2.ListViewItem());

            Position playerPos = Objects.Player.Position;
            Waypoint waypoint3 = new Waypoint() { X = playerPos.X + xDiff, Y = playerPos.Y + yDiff + 1, Z = playerPos.Z - 1, Type = WaypointType.Not_Location_Goto_Label, rangeX = 2, rangeY = 2, Extra = "Ladder" + ladderId };
            Globals.Config.Waypoints.Add(waypoint3);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint3.ListViewItem());

            radioButton11.Checked = true;
        }

        private void button22_Click(object sender, EventArgs e)
        {
            Waypoint waypoint = GenerateWaypoint(WaypointType.Stand, 1, 1);

            while (Globals.Config.Waypoints.Exists(w => w.Label == waypoint.Label))
                waypoint.Label = "ExaniTera" + ++exaniTeraId;

            Globals.Config.Waypoints.Add(waypoint);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint.ListViewItem());

            Waypoint waypoint2 = GenerateWaypoint(WaypointType.Say, "exani tera",1, 1);
            Globals.Config.Waypoints.Add(waypoint2);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint2.ListViewItem());

            Position playerPos = Objects.Player.Position;
            Waypoint waypoint3 = new Waypoint() { X = playerPos.X + xDiff, Y = playerPos.Y + yDiff + 1, Z = playerPos.Z - 1, Type = WaypointType.Not_Location_Goto_Label, rangeX = 2, rangeY = 2, Extra = "ExaniTera" + exaniTeraId };
            Globals.Config.Waypoints.Add(waypoint3);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint3.ListViewItem());

            radioButton11.Checked = true;
        }

        private void button23_Click(object sender, EventArgs e)
        {
            Waypoint waypoint = GenerateWaypoint(WaypointType.Check_Refill, "Goto_Refill", 5, 5);
            waypoint.Label = "Hunt_Check_Refill";
            Globals.Config.Waypoints.Add(waypoint);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint.ListViewItem());

            Waypoint waypoint2 = GenerateWaypoint(WaypointType.Check_Cap, "500;Goto_Refill", 5, 5);
            waypoint2.Label = "Hunt_Check_Cap";
            Globals.Config.Waypoints.Add(waypoint2);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint2.ListViewItem());

            Waypoint waypoint3 = GenerateWaypoint(WaypointType.Check_Stamina, "14;Goto_Refill", 5, 5);
            waypoint3.Label = "Hunt_Check_Stamina";
            Globals.Config.Waypoints.Add(waypoint3);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint3.ListViewItem());

            Waypoint waypoint4 = GenerateWaypoint(WaypointType.Check_Safe, "Goto_Refill", 5, 5);
            waypoint4.Label = "Hunt_Check_Safe";
            Globals.Config.Waypoints.Add(waypoint4);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint4.ListViewItem());

            Waypoint waypoint5 = GenerateWaypoint(WaypointType.Check_Imbue, "Goto_Refill", 5, 5);
            waypoint5.Label = "Hunt_Check_Imbue";
            Globals.Config.Waypoints.Add(waypoint5);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint5.ListViewItem());
        }

        private void button24_Click(object sender, EventArgs e)
        {
            Waypoint waypoint = GenerateWaypoint(WaypointType.Goto_Label, "Hunt_Start", 5, 5);
            waypoint.Label = "Loop_Hunt";
            Globals.Config.Waypoints.Add(waypoint);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint.ListViewItem());
        }
        private void button5_Click(object sender, EventArgs e)
        {
            Waypoint waypoint = GenerateWaypoint(WaypointType.Stand, 1, 1);

            while (Globals.Config.Waypoints.Exists(w => w.Label == waypoint.Label))
                waypoint.Label = "Hole" + ++holeId;

            Globals.Config.Waypoints.Add(waypoint);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint.ListViewItem());

            Position playerPos = Objects.Player.Position;
            Waypoint waypoint2 = new Waypoint() { X = playerPos.X + xDiff, Y = playerPos.Y + yDiff, Z = playerPos.Z + 1, Type = WaypointType.Not_Location_Goback, rangeX = 3, rangeY = 3, Extra = "Hole" + holeId };
            Globals.Config.Waypoints.Add(waypoint2);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint2.ListViewItem());

            radioButton11.Checked = true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Waypoint waypoint = GenerateWaypoint(WaypointType.Stand, 1, 1);
            while (Globals.Config.Waypoints.Exists(w => w.Label == waypoint.Label))
                waypoint.Label = "Stairs" + ++stairsId;

            Globals.Config.Waypoints.Add(waypoint);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint.ListViewItem());

            Waypoint waypoint2 = GenerateWaypoint(WaypointType.If_Location_Goback, 3, 3);
            Globals.Config.Waypoints.Add(waypoint2);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint2.ListViewItem());

            radioButton11.Checked = true;
        }

        private void button19_Click(object sender, EventArgs e)
        {
            Waypoint waypoint = GenerateWaypoint(WaypointType.Stand, 1, 1);

            while (Globals.Config.Waypoints.Exists(w => w.Label == waypoint.Label))
                waypoint.Label = "Teleport" + ++teleportId;

            Globals.Config.Waypoints.Add(waypoint);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint.ListViewItem());

            Waypoint waypoint2 = GenerateWaypoint(WaypointType.If_Location_Goback, 2, 2);
            Globals.Config.Waypoints.Add(waypoint2);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint2.ListViewItem());
        }
        private void button7_Click(object sender, EventArgs e)
        {
            Waypoint waypoint = GenerateWaypoint(WaypointType.Wait_PZ, 2, 2);
            waypoint.Label = "Wait_PZ";
            Globals.Config.Waypoints.Add(waypoint);
            Globals.Main.Cavebot.listView1.Items.Add(waypoint.ListViewItem());
        }
        #endregion
    }
}
