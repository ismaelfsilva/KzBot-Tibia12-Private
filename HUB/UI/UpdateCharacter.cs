using HUB.Classes;
using HUB.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HUB.UI
{
    public partial class UpdateCharacter : Form
    {
        public Character Character { get; set; }

        public UpdateCharacter(int characterId)
        {
            InitializeComponent();
            Character = Program.Characters.FirstOrDefault(c => c.id == characterId);
        }

        private void UpdateCharacter_Load(object sender, EventArgs e)
        {
            comboBox1.Items.AddRange(Program.Config.Servers.Select(s => s.name).ToArray());
            comboBox1.SelectedIndex = comboBox1.Items.IndexOf(Character.server);

            comboBox2.SelectedIndex = comboBox2.Items.IndexOf(Character.vocation);

            comboBox3.Items.Add(string.Empty);
            comboBox3.Items.AddRange(Program.Config.Scripts.Select(s=> s.name).ToArray());
            comboBox3.SelectedIndex = comboBox3.Items.IndexOf(Character.script);

            textBox1.Text = Character.name;
            textBox2.Text = Character.account;
            textBox3.Text = Character.password;
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

                string requestString = string.Format("https://tibia.kzsoft.com.br/create.php?username={0}&password={1}&char_name={2}&char_account={3}&char_password={4}&char_server={5}&char_vocation={6}{7}&char_id={8}",
                    Settings.Default.Username,
                    Settings.Default.Password,
                    textBox1.Text,
                    textBox2.Text,
                    textBox3.Text,
                    comboBox1.Text,
                    comboBox2.Text,
                    comboBox3.Text != string.Empty ? "&char_script=" + comboBox3.Text : string.Empty,
                    Character.id);

                var response = await client.GetAsync(requestString);
                string content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode && content == "1")
                {
                    if (MessageBox.Show("Character Updated.") == DialogResult.OK)
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
