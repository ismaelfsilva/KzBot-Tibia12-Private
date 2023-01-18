using HUB.Properties;
using HUB.UI;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Threading.Channels;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Xml;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using ToolTip = System.Windows.Forms.ToolTip;
using HUB.Classes;

namespace HUB
{
    public partial class Main : Form
    {

        Clients Clients;
        Characters Characters;
        Servers Servers;
        Scripts Scripts;
        Log Log;

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            textBox1.Text = Settings.Default.Username;
            textBox2.Text = Settings.Default.Password;
            textBox3.Text = Settings.Default.BotPath;


            Characters = new Characters();
            Clients = new Clients();
            Servers = new Servers();
            Scripts = new Scripts();
            Log = new Log();


            TabPage characteresTabPage = new TabPage(Characters.Text);
            TabPage clientsTabPage = new TabPage(Clients.Text);
            TabPage serversTabPage = new TabPage(Servers.Text);
            TabPage scriptsTabPage = new TabPage(Scripts.Text);
            TabPage logTabPage = new TabPage(Log.Text);


            characteresTabPage.Controls.Add(Characters);
            clientsTabPage.Controls.Add(Clients);
            serversTabPage.Controls.Add(Servers);
            scriptsTabPage.Controls.Add(Scripts);
            logTabPage.Controls.Add(Log);


            tabControl1.Controls.Add(characteresTabPage);
            tabControl1.Controls.Add(scriptsTabPage);
            tabControl1.Controls.Add(clientsTabPage);
            tabControl1.Controls.Add(serversTabPage);
            tabControl1.Controls.Add(logTabPage);


            Characters.listView1.DoubleBuffering(true);
            Clients.listView1.DoubleBuffering(true);
            Servers.listView1.DoubleBuffering(true);
            Scripts.listView1.DoubleBuffering(true);
            Log.listView1.DoubleBuffering(true);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.Username = textBox1.Text;
            Settings.Default.Save();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.Password = textBox2.Text;
            Settings.Default.Save();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.BotPath = textBox3.Text;
            Settings.Default.Save();
        }

        private void button1_Click(object sender, EventArgs e)
        {   
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            XmlSerializer configSerializer = new System.Xml.Serialization.XmlSerializer(typeof(Config), new XmlRootAttribute("KzTibia"));

            using (var writer = System.IO.File.OpenWrite(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Config.xml"))
            {
                using (var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings { Indent = true }))
                {
                    configSerializer.Serialize(xmlWriter, Program.Config);
                }
            }
        }
    }

    public static class ControlExtensions
    {
        public static void DoubleBuffering(this Control control, bool enable)
        {
            var doubleBufferPropertyInfo = control.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            doubleBufferPropertyInfo.SetValue(control, enable, null);
        }
    }
}