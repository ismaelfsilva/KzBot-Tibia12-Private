using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telegram.Bot;

namespace KzBot.UI
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();

            this.TopLevel = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Dock = DockStyle.Fill;
            this.Show();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            Reload();
        }

        public void Reload()
        {
            this.tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.RowCount = 1;

            {
                Label label = new Label();
                label.Text = "Telegram Bot Token";
                label.Dock = DockStyle.Fill;

                tableLayoutPanel1.Controls.Add(label);
                ///
                TextBox textBox = new TextBox();
                textBox.PlaceholderText = "Telegram Bot Token";

                if (Globals.telegramBotToken != String.Empty)
                    textBox.Text = Globals.telegramBotToken;

                textBox.TextAlign = HorizontalAlignment.Center;
                textBox.Dock = DockStyle.Fill;

                textBox.TextChanged += (sender, EventArgs) => {
                    Globals.telegramBotToken = textBox.Text;

                    Properties.Settings.Default.TelegramBotToken = textBox.Text;
                    Properties.Settings.Default.Save();

                    Globals.TelegramBot = new TelegramBotClient(Globals.telegramBotToken);
                };

                tableLayoutPanel1.Controls.Add(textBox);
            }
            {
                Label label = new Label();
                label.Text = "Telegram User Id";
                label.Dock = DockStyle.Fill;

                tableLayoutPanel1.Controls.Add(label);
                ///
                TextBox textBox = new TextBox();
                textBox.PlaceholderText = "Telegram User Id";

                if (Globals.telegramUserId != String.Empty)
                    textBox.Text = Globals.telegramUserId;

                textBox.TextAlign = HorizontalAlignment.Center;
                textBox.Dock = DockStyle.Fill;

                textBox.TextChanged += (sender, EventArgs) => {
                    Globals.telegramUserId = textBox.Text;

                    Properties.Settings.Default.TelegramUserId = textBox.Text;
                    Properties.Settings.Default.Save();
                };

                tableLayoutPanel1.Controls.Add(textBox);
            }

            foreach (PropertyInfo prop in Globals.Config.GetType().GetProperties())
            {
                if (Char.IsUpper(prop.Name[0]))
                    continue;

                if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(bool))
                {
                    Label label = new Label();
                    label.Text = prop.Name;
                    label.Dock = DockStyle.Fill;

                    tableLayoutPanel1.Controls.Add(label);
                }

                if (prop.PropertyType == typeof(int))
                {
                    TextBox textBox = new TextBox();

                    textBox.Text = ((int)prop.GetValue(Globals.Config)).ToString();
                    textBox.PlaceholderText = prop.Name;
                    textBox.TextAlign = HorizontalAlignment.Center;
                    textBox.Dock = DockStyle.Fill;


                    textBox.TextChanged += (sender, EventArgs) => {
                        int output;
                        int.TryParse(textBox.Text, out output);

                        prop.SetValue(Globals.Config, output);
                    };

                    tableLayoutPanel1.Controls.Add(textBox);
                }
                else if (prop.PropertyType == typeof(bool))
                {
                    CheckBox checkBox = new CheckBox();

                    checkBox.Checked = (bool)prop.GetValue(Globals.Config);
                    checkBox.Dock = DockStyle.Fill;
                    checkBox.CheckAlign = ContentAlignment.MiddleCenter;

                    checkBox.CheckedChanged += (sender, EventArgs) => {
                        prop.SetValue(Globals.Config, checkBox.Checked);
                    };

                    tableLayoutPanel1.Controls.Add(checkBox);
                }


            }
        }
    }
}
