using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.IO;
using Microsoft.Win32;

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
            /*HKEY_CLASSES_ROOT
                dekaron
                    (Default) = "URL:Dekaron Protocol"
                    URL Protocol = ""
                    DefaultIcon
                        (Default) = "Dekaron.exe, 1"
                shell
                    open
                        Command
                            (Default) = "C:/Program Files/Core-Games/Dekaron.exe" "%1"*/
            RegistryKey key = Registry.ClassesRoot.CreateSubKey("nlc"); // Setting up the parent registry key for the Neural Link Client.
                key.SetValue("", "URL:Neural Link Client Protocol"); // Setting the name of the Protocol to be called.
                key.SetValue("URL Protocol", ""); // Setting up the type of Protocol to be called.
            RegistryKey iconKey = key.CreateSubKey("DefaultIcon"); // Setting up the subkey for the Icon to be linked to the exe.
                iconKey.SetValue("", ""); // Setting up the filepath to connect the Icon.
            RegistryKey commandKey = key.CreateSubKey("shell").CreateSubKey("open").CreateSubKey("command"); // Setting up the subkey for the commandkey for when the shortcut is clicked on.
            commandKey.SetValue("", "C:/Program Files/Core-Games/Dekaron.exe"); // Setting up the exe filepath for when the shortcut is clicked on.
        }

        public static void install(Object installPath)
        {
            Directory.CreateDirectory((String)installPath); // Make the Directory for use in the install
            FileStream stream = File.Create(installPath + "/Cmd.exe"); // Set up a stream for writing the file to the full path including filename
            byte[] file = NL.Installer.Properties.Resources.cmd; // Prepare a RAM copy of the file to be copied
            stream.Write(file, 0, file.Length); // Write the file
            stream.Dispose(); // Clear the memory space of the stream to avoid unnecessary allocation.

            stream = File.Create(installPath + "/Notepad.exe"); // Change the stream for writing the second file.
            file = NL.Installer.Properties.Resources.notepad; // Change the RAM copy to the new file
            stream.Write(file, 0, file.Length); // Write the file
            stream.Dispose(); // Clear the memory space of the stream to avoid unnecessary allocation.
        }
    }
}
