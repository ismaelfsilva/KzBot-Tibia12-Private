using HUB.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace HUB
{
    public partial class CreateCharacter : Form
    {
        public CreateCharacter()
        {
            InitializeComponent();
        }

        private void CreateCharacter_Load(object sender, EventArgs e)
        {
            comboBox1.Items.AddRange(Program.Config.Servers.Select(s => s.name).ToArray());
            comboBox2.SelectedIndex = 0;
            comboBox3.Items.Add(string.Empty);
            comboBox3.Items.AddRange(Program.Config.Scripts.Select(s => s.name).ToArray());
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var client = new HttpClient();

                client.Timeout = TimeSpan.FromSeconds(5);
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
                client.DefaultRequestHeaders.Add("accept", "application/json, text/plain, */*");
                client.DefaultRequestHeaders.Add("accept-language", "en-US,en;q=0.9");

               string requestString = string.Format("https://tibia.kzsoft.com.br/create.php?username={0}&password={1}&char_name={2}&char_account={3}&char_password={4}&char_server={5}&char_vocation={6}{7}",
                    Settings.Default.Username,
                    Settings.Default.Password,
                    textBox1.Text,
                    textBox2.Text,
                    textBox3.Text,
                    comboBox1.Text,
                    comboBox2.Text,
                    comboBox3.Text != string.Empty ? "&char_script=" + comboBox3.Text : string.Empty
                    );

                var response = await client.GetAsync(requestString);
                string content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode && content == "1")
                {
                    if (MessageBox.Show("Character Added.") == DialogResult.OK)
                        this.Close();
                }
                else
                    MessageBox.Show("There was an error.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error.");
                return;
            }
        }
    }
}
