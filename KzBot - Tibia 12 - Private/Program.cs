namespace KzBot
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            Main main = new Main();

            if (args.Length >= 1)
                main.filePath = args[0];

            if (args.Length >= 3)
            {
                main.config_username= args[1];
                main.config_password= args[2];
            }

            if (args.Length >= 5)
            {
                main.account_username = args[3];
                main.account_password = args[4];
            }

            if (args.Length >= 6)
                main.clientPath = args[5];

            Application.Run(main);
        }
    }
}