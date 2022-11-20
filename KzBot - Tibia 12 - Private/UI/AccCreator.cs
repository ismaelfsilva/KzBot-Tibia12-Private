using Newtonsoft.Json;
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
using Tesseract;

namespace KzBot.UI
{
    public partial class AccCreator : Form
    {


        public AccCreator()
        {
            InitializeComponent();
        }

        private void AccCreator_Load(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            Create(textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text, radioButton1.Checked);
        }

        public async Task<bool> Create()
        {
            Random r = new Random();
            string account = GenerateName(r.Next(6, 12)).ToLower();
            string password = new Util.PasswordGenerator().Generate();

            string email = account + "@" + GenerateName(r.Next(3, 5));
            int emailEndChance = r.Next(120);
            if (emailEndChance <= 30)
                email += ".net";
            else if (emailEndChance <= 60)
                email += ".pl";
            else
                email += ".com";

            bool male = r.Next(100) < 50;
            string flag = "br";
            string character = string.Empty;

            int characterNameChance = r.Next(100);
            if (characterNameChance <= 50)
            {
                flag = "pl";
                if (characterNameChance < 15)
                    flag = "br";

                character = GenerateName(r.Next(4, 7));
                if (r.Next(100) > 50)
                {
                    character += " " + GenerateName(r.Next(4, 7));
                }
            }
            else
            {
                flag = r.Next(100) <= 50 ? "br" : "gb";
                character = new Util.RandomName(r).Generate((Util.Sex)Convert.ToInt32(male), r.Next(0, 1));
            }

            return await Create(account, password, email, character, male, flag);
        }

        public string GenerateName(int len)
        {
            Random r = new Random();
            string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", "sh", "zh", "t", "v", "w", "x" };
            string[] vowels = { "a", "e", "i", "o", "u", "ae", "y" };
            string Name = "";
            Name += consonants[r.Next(consonants.Length)].ToUpper();
            Name += vowels[r.Next(vowels.Length)];
            int b = 2; //b tells how many times a new letter has been added. It's 2 right now because the first two letters are already in the name.
            while (b < len)
            {
                Name += consonants[r.Next(consonants.Length)];
                b++;
                Name += vowels[r.Next(vowels.Length)];
                b++;
            }

            return Name;
        }

        public async Task<bool> Create(string account, string password, string email, string character, bool male, string flag = "br", bool printError = false)
        {
            try
            {
                var handler = new HttpClientHandler();
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.ServerCertificateCustomValidationCallback =
                    (httpRequestMessage, cert, cetChain, policyErrors) =>
                    {
                        return true;
                    };

                var client = new HttpClient(handler);
                string checkCodeStr;

                client.Timeout = TimeSpan.FromSeconds(15);
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
                client.DefaultRequestHeaders.Add("accept", "application/json, text/plain, */*");
                client.DefaultRequestHeaders.Add("accept-language", "en-US,en;q=0.9");

                using (TesseractEngine engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
                {
                    engine.SetVariable("tessedit_char_whitelist", "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");

                    var checkCodeResponse = await client.GetAsync(new Uri("https://45.141.214.47/?subtopic=imagebuilder&image_refresher=%27.rand(1,99999).%27"));
                    if (checkCodeResponse.IsSuccessStatusCode)
                    {
                        Bitmap checkCode = (Bitmap)Bitmap.FromStream(await checkCodeResponse.Content.ReadAsStreamAsync());
                        Bitmap finalImage = new Bitmap(checkCode.Size.Width, checkCode.Size.Height);

                        // TRATAR IMAGEM
                        for (int x = 0; x < checkCode.Width; x++)
                        {
                            for (int y = 0; y < checkCode.Height; y++)
                            {
                                if (checkCode.GetPixel(x, y).R >= 200)
                                    finalImage.SetPixel(x, y, Color.Black);
                                else
                                    finalImage.SetPixel(x, y, Color.White);
                            }
                        }

                        // LER IMAGEM
                        using (MemoryStream stream = new MemoryStream())
                        {
                            finalImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

                            using (var img = Pix.LoadFromMemory(stream.ToArray()).Scale(3, 3))
                            {
                                using (var page = engine.Process(img, PageSegMode.SingleWord))
                                {
                                    string text = page.GetText();
                                    checkCodeStr = text;
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("There was an unexpected error (2).");
                        return false;
                    }

                }

                var pairs = new List<KeyValuePair<string, string>>
    {
        new KeyValuePair<string, string>("account", account),
        new KeyValuePair<string, string>("reg_code", checkCodeStr),
        new KeyValuePair<string, string>("email", email),
        new KeyValuePair<string, string>("country", flag),
        new KeyValuePair<string, string>("password", password),
        new KeyValuePair<string, string>("password2", password),
        new KeyValuePair<string, string>("name", character),
        new KeyValuePair<string, string>("sex", Convert.ToInt32(male).ToString()),
        new KeyValuePair<string, string>("world", "0"),
        new KeyValuePair<string, string>("accept_rules", "true"),
        new KeyValuePair<string, string>("save", "1"),
        new KeyValuePair<string, string>("Submit.x", "105"),
        new KeyValuePair<string, string>("Submit.y", "13"),
    };

                var postContent = new FormUrlEncodedContent(pairs);

                var response = await client.PostAsync(new Uri("https://45.141.214.47/?account/create"), postContent);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    this.textBox5.Invoke((MethodInvoker)delegate {
                        if (content.Trim() == string.Empty)
                        {
                            if (printError)
                                textBox5.Text += "There was an unexpected error (0)." + Environment.NewLine;
                        }
                        else if (content.Contains("There was an error creating your character"))
                        {
                            if (printError)
                                textBox5.Text += "There was an error while creating the character (4)." + Environment.NewLine;
                        }
                        else if (content.Contains("Account Created"))
                        {
                            textBox5.Text += String.Format("<Account Character=\"{0}\" AccountName=\"{1}\" Password=\"{2}\" Script=\"Leveling.kzTibia\" Index=\"1\" />", character, account, password) + Environment.NewLine;
                        }
                        else
                        {
                            if (printError)
                                textBox5.Text += "There was an error while creating the account (3)." + Environment.NewLine;
                        }
                    });


                    if (content.Contains("Account Created") && !content.Contains("There was an error creating your character"))
                        return true;
                }
                else
                {
                    if (printError)
                        this.textBox5.Invoke((MethodInvoker)delegate
                        {
                            textBox5.Text += "There was an unexpected error (2)." + Environment.NewLine;
                        });
                    return false;
                }
            }            
            catch (Exception ex)
            {
                if (printError)
                    this.textBox5.Invoke((MethodInvoker)delegate
                    {
                        textBox5.Text += "There was an unexpected error (1)." + Environment.NewLine;
                    });
                return false;
            }
            
            return false;
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            List<Task<bool>> taskList = new List<Task<bool>>();
            int count = 0;

            for (int i = 0; i < 1; i++)
            {
                taskList.Add(Create());
            }

            for (int i = 0; i < taskList.Count; i++)
            {
                bool created = await taskList[i];

                if (created)
                    count++;
                else
                    taskList.Add(Create());

                if (count >= 1)
                    break;
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            List<Task<bool>> taskList = new List<Task<bool>>();
            int count = 0;

            for (int i = 0; i < 100; i++)
            {
                taskList.Add(Create());
            }

            for (int i = 0; i < taskList.Count; i++)
            {
                bool created = await taskList[i];

                if (created)
                    count++;
                else
                    taskList.Add(Create());

                if (count >= 100)
                    break;
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            List<Task<bool>> taskList = new List<Task<bool>>();
            int count = 0;

            for (int i = 0; i < 10; i++)
            {
                taskList.Add(Create());
            }

            for (int i = 0; i< taskList.Count; i++)
            {
                bool created = await taskList[i];

                if (created)
                    count++;
                else
                    taskList.Add(Create());

                if (count >= 10)
                    break;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();

            foreach (string dir in System.IO.Directory.GetParent(Globals.exeOtLocation).Parent.GetDirectories().Select(d=> d.FullName))
            {                
                try
                {
                    File.Copy(openFileDialog.FileName, dir + @"\" + openFileDialog.SafeFileName, true);
                }
                catch
                { }
            }
        }
    }
}
