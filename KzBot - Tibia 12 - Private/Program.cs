using System.Reflection;
using System.Xml.Serialization;

namespace KzBot
{
    public static class Program
    {
        public static Config Config = new Config();
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            using (Stream file = System.IO.File.Open(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Config.xml", FileMode.Open))
            {
                XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(Config), new XmlRootAttribute("KzTibia"));

                Config = (Config)writer.Deserialize(file);
                file.Close();
            }

            ApplicationConfiguration.Initialize();



            if (args.Length >= 1)
                Globals.Script = Config.Scripts.FirstOrDefault(s => s.id == args[0]);

            if (args.Length >= 3)
            {
                Globals.Username= args[1];
                Globals.Password= args[2];
            }

            if (args.Length >= 8)
            {
                Globals.AccCharName = args[3];
                Globals.AccName = args[4];
                Globals.AccPass = args[5];
                Globals.AccCharIndex = int.Parse(args[6]);
                Globals.AccVocation = (Vocation)Enum.Parse(typeof(Vocation), args[7]);
            }

            if (args.Length >= 9)
                Globals.Server = Config.Servers.FirstOrDefault(s => s.id == args[8]);

            if (args.Length >= 10)
                Globals.Client = Config.Clients.FirstOrDefault(c => c.id == args[9]);


            Main main = new Main();
            Application.Run(main);
        }
    }
}