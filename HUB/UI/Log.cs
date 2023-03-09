using HUB.Classes;
using HUB.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace HUB.UI
{
    public partial class Log : Form
    {

        private readonly HttpClient _httpClient = new HttpClient();
        public static int lastId = 0;

        public Log()
        {
            InitializeComponent();

            this.TopLevel = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Dock = DockStyle.Fill;
            this.Show();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Interval = 10000;

            try
            {
                Thread workerThread = new Thread(new ThreadStart(
                () =>
                {
                    try
                    {
                        this.Invoke((MethodInvoker)(
                        () =>
                        {
                            using (HttpResponseMessage response = _httpClient.GetAsync(string.Format("http://tibia.kzsoft.com.br/log.php?username={0}&password={1}&last_id={2}", Settings.Default.Username, Settings.Default.Password, lastId)).Result)
                            {
                                using (HttpContent content = response.Content)
                                {
                                    string result = content.ReadAsStringAsync().Result;

                                    List<LogMessage> LogMessages = JsonSerializer.Deserialize<List<LogMessage>>(result);

                                    foreach (LogMessage msg in LogMessages)
                                    {
                                        if (msg.id > lastId)
                                            lastId = msg.id;

                                        listView1.Invoke((MethodInvoker)delegate
                                        {
                                            listView1.Items.Add(msg.ListViewItem);
                                            listView1.EnsureVisible(listView1.Items.Count - 1);
                                        });
                                    }
                                }
                            }
                        }));
                    }
                    catch
                    {

                    }
                }
                ));

                workerThread.Start();
            }
            catch
            {

            }
        }
    }
}
