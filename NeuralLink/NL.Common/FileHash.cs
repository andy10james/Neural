using System;
using System.IO;
using System.Security.Cryptography;

namespace NL.Common {

    [Serializable]
    public class FileHash {

        public String File { get { return _file;  } }
        public String Hash { get { return _hash;  } }
        private String _file;
        private String _hash;

        internal static FileHash Create(Uri root, Uri path) {

            MD5 Encryptor = MD5.Create();
            String hash;



            String absPath = Uri.UnescapeDataString(path.AbsolutePath);
            String file = Uri.UnescapeDataString(root.MakeRelativeUri(path).ToString());
            using (FileStream fileStream = System.IO.File.OpenRead(absPath)) {
                Byte[] fileHashBytes = Encryptor.ComputeHash(fileStream);
                String unformattedFileHash = BitConverter.ToString(fileHashBytes);
                hash = unformattedFileHash.Replace("-", "").ToLower();
            }

            FileHash fileHash = new FileHash {
                _file = file,
                _hash = hash
            };

            return fileHash;

        }

    }

}
