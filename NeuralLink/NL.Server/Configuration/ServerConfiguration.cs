﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms.VisualStyles;
using System.Xml;

namespace NL.Server.Configuration {
    internal static class ServerConfiguration {

        public static String[] StartUpCommands;
        public static Uri Repository = new Uri(Environment.CurrentDirectory, UriKind.Absolute);
        public static Boolean BeepOnConnection = true;
        public static Boolean BeepOnDisconnection = true;

        static ServerConfiguration() {
            Load(XmlStrings.SettingsFile);
        }

        public static void Load(String xmlPath) {
            FileStream fs = new FileStream(xmlPath, FileMode.Open, FileAccess.Read);
            XmlDocument settingsXml = new XmlDocument();
            settingsXml.Load(fs);
            XmlNode serverNode = settingsXml.GetElementsByTagName(XmlStrings.ServerRoute)[0];
            LoadStart(serverNode);
        }

        public static void LoadStart(XmlNode serverNode) {
            List<XmlNode> startUpNodes = GetNodes(serverNode, XmlStrings.StartRoute);
            StartUpCommands =
                startUpNodes.Where(n => n.Attributes[XmlStrings.StartEnabledAttribute].Value.ToLower().Equals("true"))
                    .Select(n => n.InnerText).ToArray();
        }

        public static List<XmlNode> GetNodes(XmlNode serverNode, String path) {
            String[] nodes = path.Split('\\');
            XmlNode currentNode = serverNode;
            List<XmlNode> matchedNodes = new List<XmlNode>();
            foreach (String node in nodes) {
                foreach (XmlNode child in currentNode) {
                    if (!nodes.Last().Equals(node) && child.Name.Equals(node)) {
                        currentNode = child;
                        break;
                    } else if (child.Name.Equals(node)) {
                        matchedNodes.Add(child);
                    }
                }
            }
            return matchedNodes;
        }


    }
}
