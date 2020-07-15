using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using IWshRuntimeLibrary;

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
            
            if (arguments.Length > 2)
                return;
            
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

            while (true)
            {
                Trace.Write("New Scanner Service");
                MyScannerService service = new MyScannerService();
                Thread thread = service.getThread();
                service.startListening();
                thread.Join();
                Thread.Sleep(100);
            }
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
