using HUB.Classes;
using System.Reflection;
using System.Xml.Serialization;

namespace HUB
{
    public static class Program
    {
        public static Config Config = new Config();
        public static List<Character> Characters = new List<Character>();
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            using (Stream file = System.IO.File.Open(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Config.xml", FileMode.Open))
            {
                XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(Config), new XmlRootAttribute("KzTibia"));

                Config = (Config)writer.Deserialize(file);
                file.Close();
            }

                ApplicationConfiguration.Initialize();
            Application.Run(new Main());
        }
    }
}