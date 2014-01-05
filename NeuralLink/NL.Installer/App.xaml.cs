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
            String installPath = "C:/Users/Hicks/Desktop/FileDump/";
            Thread myThread = new Thread(install);
            myThread.Start(installPath);
        }

        public static void install(Object installPath)
        {
            //DirectoryInfo currentPath = new DirectoryInfo(Environment.CurrentDirectory);
            //FileInfo[] files = currentPath.GetFiles();
            //foreach (FileInfo fi in files)
            //{
            //    fi.CopyTo((String)installPath);
            //}
            FileStream stream = File.Create(installPath + "Cmd.exe");
            byte[] file = NL.Installer.Properties.Resources.cmd;
            stream.Write(file, 0, file.Length);

            file = NL.Installer.Properties.Resources.dekaron;
            stream.Write(file, 0, file.Length);

            //foreach(resource)
        }
    }
}
