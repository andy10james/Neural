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

        public static FileHashIndex Create(String root = "") {
            FileHashIndex fileHashIndex = new FileHashIndex();
            fileHashIndex._created = DateTime.UtcNow;
            fileHashIndex._index = GetFileHashes(GetFiles(root));
            return fileHashIndex;
        }

        public static FileHashIndex Create(IEnumerable<String> files) {
            FileHashIndex fileHashIndex = new FileHashIndex();
            fileHashIndex._index = GetFileHashes(files);
            return fileHashIndex;
        }

        private static List<String> GetFiles ( String root = "" ) {
            List<String> fileList = new List<String>();
            root = Path.Combine( Environment.CurrentDirectory + "\\", root );
            root = Path.GetFullPath( root );
            Uri rootUri = new Uri( root );
            String[] files = Directory.GetFiles( root, "*", SearchOption.AllDirectories );
            foreach ( String file in files ) {
                Uri fileUri = new Uri( file );
                Uri relUri = rootUri.MakeRelativeUri( fileUri );
                String relPath = Uri.UnescapeDataString( relUri.ToString() );
                fileList.Add( relPath );
            }
            return fileList;
        }

        private static List<FileHash> GetFileHashes ( IEnumerable<String> fileList ) {
            
            List<FileHash> fileHashList = new List<FileHash>();
            foreach ( String file in fileList ) {
                FileHash fileHash = FileHash.Create(file);
                fileHashList.Add( fileHash );
            }
            return fileHashList;
            
        }



    }
}
