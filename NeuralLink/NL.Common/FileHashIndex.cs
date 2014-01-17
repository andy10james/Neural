using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NL.Common {

    [Serializable]
    public class FileHashIndex : IEnumerable<FileHash> {

        public FileHash[] Index { get { return _index.ToArray(); } }
        public String[] Files { get { return _index.Select( i => i.File ).ToArray(); } }
        public String[] Hashes { get { return _index.Select( i => i.Hash ).ToArray(); } }
        public DateTime Created { get { return _created; } }
        
        private List<FileHash> _index = new List<FileHash>();
        private DateTime _created = DateTime.UtcNow;

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public IEnumerator<FileHash> GetEnumerator() {
            return _index.GetEnumerator();
        }

        public override String ToString() {
            StringBuilder output = new StringBuilder();
            foreach (FileHash fileHash in _index) {
                output.AppendFormat("{0,-40}{1,-32}", fileHash.File, fileHash.Hash);
                if (fileHash != _index.Last()) output.AppendLine();
            }
            return output.ToString();
        }

        public Dictionary<String, String> ToDictionary() {
            Dictionary<String, String> dictionary = new Dictionary<String,String>();
            foreach (FileHash fileHash in _index) {
                dictionary.Add(fileHash.File, fileHash.Hash);
            }
            return dictionary;
        }

        public static FileHashIndex Create(Uri root) {
            List<Uri> files = GetFiles(root);
            return Create(root, files);
        }

        public static FileHashIndex Create(Uri root, IEnumerable<Uri> files) {
            root = root.AbsolutePath.EndsWith("/") ?
                root : new Uri(root.AbsolutePath + '/');
            FileHashIndex fileHashIndex = new FileHashIndex();
            fileHashIndex._created = DateTime.UtcNow;
            fileHashIndex._index = GetFileHashes(root, files);
            return fileHashIndex;
        }

        private static List<Uri> GetFiles (Uri root) {
            List<Uri> fileList = new List<Uri>();
            String path = Uri.UnescapeDataString(root.AbsolutePath);
            String[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            foreach (String file in files) fileList.Add(new Uri(file));
            return fileList;
        }

        private static List<FileHash> GetFileHashes ( Uri root, IEnumerable<Uri> fileList ) {
            List<FileHash> fileHashList = new List<FileHash>();
            foreach (Uri file in fileList) {
                FileHash fileHash = FileHash.Create(root, file);
                fileHashList.Add(fileHash);
            }
            return fileHashList;
        }

    }
}
