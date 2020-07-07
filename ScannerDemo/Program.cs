using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using IWshRuntimeLibrary;
using File = System.IO.File;

namespace ScannerDemo
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string startupFolder = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            Trace.Write(startupFolder + "\n");
            string[] arguments = Environment.GetCommandLineArgs();

            if (arguments.Length == 2)
            {
                createShortCut();
                Properties.Settings.Default.SHORTCUTCREATED = true;
                Properties.Settings.Default.Save();
            }

            if (arguments.Length > 1)
            {
                return;
            }

            MyScannerService service = new MyScannerService();
            Thread thread = service.getThread();
            service.startListening();
            thread.Join();

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
        }

        private static void createShortCut()
        {
            string startupFolder = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            WshShell shell = new WshShell();
            string shortcutAddress = startupFolder + @"\GitaristScannerDemo.lnk";
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);
            shortcut.Description = "A startup shortcut. If you delete this shortcut from your computer, LaunchOnStartup.exe will not launch on Windows Startup"; // set the description of the shortcut
            shortcut.WorkingDirectory = Application.StartupPath; /* working directory */
            shortcut.TargetPath = Application.ExecutablePath; /* path of the executable */
            shortcut.Save(); // save the shortcut
            Trace.WriteLine("SHORTCUT CREATED");
        }
    }
}
