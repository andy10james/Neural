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
using System.Xml.Serialization;
using System.Windows.Controls;

namespace NL.Installer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // Test path = C:/Users/Hicks/Desktop/FileDump
        public App()
            : base()
        {
            //Start of application
            String installPath = System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Core-Games_Dekaron";
            Thread myThread = new Thread(install);
            myThread.Start(installPath);
            /*RegistryKey key = Registry.ClassesRoot.CreateSubKey("nlc");
                key.SetValue("", "URL:Neural Link Client Protocol");
                key.SetValue("URL Protocol", "");
            RegistryKey iconKey = key.CreateSubKey("DefaultIcon");
                iconKey.SetValue("", "");
            RegistryKey commandKey = key.CreateSubKey("shell").CreateSubKey("open").CreateSubKey("command");
            commandKey.SetValue("", "C:/Program Files/Core-Games/Dekaron.exe");*/
        }

        public void install(Object installPath)
        {
            Directory.CreateDirectory((String)installPath);
            FileStream stream = File.Create(installPath + "\\Cmd.exe");
            byte[] file = NL.Installer.Properties.Resources.cmd;
            for (int i = 0; i < file.Length; i++)
            {
                stream.Write(file, (0 + i), 1);
            }
            stream.Dispose();
            this.MainWindow.Close();

            stream = File.Create(installPath + "\\Notepad.exe");
            file = NL.Installer.Properties.Resources.notepad;
            for (int i = 0; i < file.Length; i++)
            {
                stream.Write(file, 0, file.Length);
                stream.Write(file, 0 + i, 1);
                //updateProgress();
            }
            stream.Dispose();
        }

        /*public void updateProgress()
        {
            if (this.progressBar.Value < 100)
            {
                this.progressBar.Value += 1;
            }
            else
            {
                return;
            }
        }*/
    }
}
