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
    public partial class Log : Form
    {

        List<logMessage> uniqueLogMessages = new List<logMessage>();

        public Log()
        {
            InitializeComponent();

            this.TopLevel = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Dock = DockStyle.Fill;
            this.Show();
        }

        private void Log_Load(object sender, EventArgs e)
        {

        }

        public void addLog(string message, bool unique = false)
        {
            if (unique)
            {
                bool hasSameMessage = uniqueLogMessages.Exists(m => m.message == message && (DateTime.Now - m.time).TotalMinutes < 5);

                if (!hasSameMessage)
                {
                    uniqueLogMessages.Add(new logMessage(message));
                    // UPLOAD
                    Threads.ClientData.LogMessage(message);
                }
            }
            else
            {
                Threads.ClientData.LogMessage(message);
            }

            listBox1.Invoke((MethodInvoker)delegate
            {
                listBox1.Items.Add(string.Format("[{0}] {1}", DateTime.Now.ToString(), message));
            });
        }

        class logMessage
        {
            public string message;
            public DateTime time;

            public logMessage(string _message)
            {
                message = _message;
                time = DateTime.Now;
            }
        }
    }
}
