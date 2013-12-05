using System;
using System.IO;
using System.Security.Cryptography;

namespace NLS {

    [Serializable]
    internal class FileHash {

        public String File { get { return file;  } }
        public String Hash { get { return hash;  } }
        private String file;
        private String hash;

        public static FileHash Create(String file) {

            MD5 Encryptor = MD5.Create();
            String hash;
            using (FileStream fileStream = System.IO.File.OpenRead(file)) {
                Byte[] fileHashBytes = Encryptor.ComputeHash(fileStream);
                String unformattedFileHash = BitConverter.ToString(fileHashBytes);
                hash = unformattedFileHash.Replace("-", "").ToLower();
            }

            FileHash fileHash = new FileHash {
                file = file,
                hash = hash
            };

            return fileHash;

        }

    }

}
