using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NLS {
    internal static class FileDirector {

        public static List<Uri> GetFiles(String root = "") {
            List<Uri> fileList = new List<Uri>();
            root = Path.Combine(Environment.CurrentDirectory + "\\", root);
            root = Path.GetFullPath(root);
            Uri rootUri = new Uri(root);
            String[] files = Directory.GetFiles(root, "*", SearchOption.AllDirectories);
            foreach (String file in files) {
                Uri fileUri = new Uri(file);
                Uri relUri = rootUri.MakeRelativeUri(fileUri);
                fileList.Add(relUri);
            }
            return fileList;
        }

        public static List<String> GetFileHashes(IEnumerable<Uri> fileList) {
            List<String> fileHashList = new List<String>();
            MD5 Encryptor = MD5.Create();
            foreach (Uri file in fileList) {
                using (FileStream fileStream = File.OpenRead(file.OriginalString)) {
                    Byte[] fileHashBytes = Encryptor.ComputeHash(fileStream);
                    String unformattedFileHash = BitConverter.ToString(fileHashBytes);
                    String fileHash = unformattedFileHash.Replace("-", "").ToLower();
                    fileHashList.Add(fileHash);
                }
            }
            return fileHashList;
        }

        public static Dictionary<Uri, String> GetFileHashIndex() {
            Dictionary<Uri, String> fileHashIndex = new Dictionary<Uri, String>();
            List<Uri> files = GetFiles();
            List<String> hashes = GetFileHashes(files);
            for (int i = 0; i < files.Count; i++) {
                fileHashIndex.Add(files[i], hashes[i]);
            }
            return fileHashIndex;
        }

    }
}
