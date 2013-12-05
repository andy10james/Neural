using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NLS {

    internal class FileHashIndex : IEnumerable {

        public List<String> Files { get { return files; } }
        public List<String> Hashes { get { return hashes; } }
        private List<String> hashes = new List<string>();
        private List<String> files = new List<string>();

        public static FileHashIndex Create( String root = "" ) {
            FileHashIndex fileHashIndex = new FileHashIndex();
            fileHashIndex.files = GetFiles( root );
            fileHashIndex.hashes = GetFileHashes( fileHashIndex.files );
            return fileHashIndex;
        }

        public override String ToString() {
            StringBuilder output = new StringBuilder();
            for (int i = 0; i < files.Count; i++) {
                output.AppendFormat("{0,-40}{1,-32}", files[i], hashes[i]);
                if (i + 1 != files.Count) output.AppendLine();
            }
            return output.ToString();
        }

        public String ToJson() {
            String jsonString;
            Dictionary<String, String> dictionary = this.ToDictionary();
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(dictionary.GetType(), 
                new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true });
            using (MemoryStream stream = new MemoryStream()) {
                jsonSerializer.WriteObject(stream, dictionary);
                jsonString = Encoding.UTF8.GetString(stream.ToArray());
            }
            return jsonString;
        }

        public Dictionary<String, String> ToDictionary() {
            Dictionary<String, String> dictionary = new Dictionary<String,String>();
            for (int i = 0; i < files.Count; i++) {
                dictionary.Add(files[i], hashes[i]);
            }
            return dictionary;
        }

        public static List<String> GetFiles ( String root = "" ) {
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

        public static List<String> GetFileHashes ( IEnumerable<String> fileList ) {
            List<String> fileHashList = new List<String>();
            MD5 Encryptor = MD5.Create();
            foreach ( String file in fileList ) {
                using ( FileStream fileStream = File.OpenRead( file ) ) {
                    Byte[] fileHashBytes = Encryptor.ComputeHash( fileStream );
                    String unformattedFileHash = BitConverter.ToString( fileHashBytes );
                    String fileHash = unformattedFileHash.Replace("-", "").ToLower();
                    fileHashList.Add( fileHash );
                }
            }
            return fileHashList;
        }

        public static Dictionary<String, String> GetFileHashDictionary( String root = "" ) {
            Dictionary<String, String> fileHashIndex = new Dictionary<String, String>();
            List<String> files = GetFiles( root );
            List<String> hashes = GetFileHashes( files );
            for (int i = 0; i < files.Count; i++) {
                fileHashIndex.Add( files[i], hashes[i] );
            }
            return fileHashIndex;
        }

    }
}
