using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.IO;

namespace NL.Installer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // Test path = C:/Program_Files/Core-Games_Dekaron

        public App()
            : base()
        {
            //Start of application
            String installPath = "C:/Users/Hicks/Desktop/FileDump"; // Set up the path to be used for installation
            Thread myThread = new Thread(install); // Set up a thread for method maintenance
            myThread.Start(installPath); // Start up the thread with the method programme loaded
        }

        public static void install(Object installPath)
        {
            Directory.CreateDirectory((String)installPath); // Make the Directory for use in the install
            FileStream stream = File.Create(installPath + "/Cmd.exe"); // Set up a stream for writing the file to the full path including filename
            byte[] file = NL.Installer.Properties.Resources.cmd; // Prepare a RAM copy of the file to be copied
            stream.Write(file, 0, file.Length); // Write the file

            stream = File.Create(installPath + "/Notepad.exe"); // Change the stream for writing the second file.
            file = NL.Installer.Properties.Resources.notepad; // Change the RAM copy to the new file
            stream.Write(file, 0, file.Length); // Write the file
        }
    }
}
