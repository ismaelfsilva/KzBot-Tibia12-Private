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
    public partial class Alerts : Form
    {
        public Alerts()
        {
            InitializeComponent();

            this.TopLevel = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Dock = DockStyle.Fill;
            this.Show();
        }

        private void Alerts_Load(object sender, EventArgs e)
        {
            AddAlarms();
        }

        public void AddAlarms()
        {
            this.tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.RowCount = 1;

            foreach (AlarmRule alarm in Globals.ScriptConfig.Alarms)
            {
                tableLayoutPanel1.RowCount += 1;

                CheckBox checkBox = new CheckBox();
                checkBox.Text = alarm.Type.ToString().Replace("_", " ");
                checkBox.Checked = alarm.Enabled;
                checkBox.Margin = new Padding(5, 3, 3, 3);
                checkBox.AutoSize = true;

                checkBox.CheckedChanged += (sender, e) => { alarm.Enabled = checkBox.Checked; };

                TextBox textBox = new TextBox();
                textBox.Text = alarm.Action;
                textBox.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

                textBox.TextChanged += (sender, e) => { alarm.Action = textBox.Text; };

                tableLayoutPanel1.Controls.Add(checkBox);
                tableLayoutPanel1.Controls.Add(textBox);
            }
        }
    }
}
