using System;
using System.Windows.Forms;
using System.IO;

namespace TTG_Tools
{
    static class Program
    {
        public static bool FirstTime = true;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string xmlPath = Application.StartupPath + "\\config.xml";

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (File.Exists(xmlPath))
            {
                FirstTime = false;
                AppData.LoadConfig();
                Application.Run(new FontEditor());
            }
            else
            {
                MessageBox.Show("Can't find config.xml!\r\nPlease set path for folders, save changes and restart the program!", "Error");
                Application.Run(new FormSettings());
            }
        }
    }
}
