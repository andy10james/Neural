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

        internal static FileHash Create(String file) {

            MD5 Encryptor = MD5.Create();
            String hash;
            using (FileStream fileStream = System.IO.File.OpenRead(file)) {
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
