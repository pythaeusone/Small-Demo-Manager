using SmallDemoManager.GUI;
using System.Globalization;

namespace SmallDemoManager
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {


            // Load new GUI Style
            try
            {
                Application.EnableVisualStyles();

#if NETCOREAPP3_1 || NET6_0 || NET7_0 || NET8_0 || NET9_0 || NET10_0

                Application.SetHighDpiMode(HighDpiMode.SystemAware);

#endif

                CultureInfo cultureInfo = new(CultureInfo.CurrentCulture.TextInfo.CultureName);
                Application.SetCompatibleTextRenderingDefault(false);

                NewGUI newGUI = new NewGUI();

                // If the first argument is an existing file with a ".dem" extension (case-insensitive)
                // pass the file path to the form so it can be processed on startup
                if (args.Length > 0 && File.Exists(args[0]) && Path.GetExtension(args[0]).Equals(".dem", StringComparison.OrdinalIgnoreCase))
                {
                    // Passes the file to the form at startup
                    newGUI.SetDemoFileOnStartup(args[0]);
                }

                Application.CurrentCulture = cultureInfo;
                Application.Run(newGUI);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace + "\n" + ex.Source + "\n" + ex.InnerException + "\n" + ex.Data + "\n" + ex.TargetSite);
            }
        }
    }
}