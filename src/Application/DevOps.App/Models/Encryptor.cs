using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DevOps.App.Encryption
{
    // Based on https://stackoverflow.com/questions/10168240/encrypting-decrypting-a-string-in-c-sharp
    public class Encryptor
    {
        private readonly string encryptionKey;
        private readonly byte[] salt = {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76};

        public Encryptor(string encryptionKey)
        {
            this.encryptionKey = encryptionKey;
        }
        public string Decrypt(string encryptedText)
        {
            encryptedText = encryptedText.Replace(" ", "+");
            var encryptedBytes = Convert.FromBase64String(encryptedText);

            var decryptedBytes = Execute(encryptedBytes, encrypt: false);

            var decryptedString = Encoding.Unicode.GetString(decryptedBytes);
            return decryptedString;
        }

        public string Encrypt(string text)
        {
            var bytes = Encoding.Unicode.GetBytes(text);

            var encryptedBytes = Execute(bytes, encrypt: true);
            var encryptedString = Convert.ToBase64String(encryptedBytes);
            return encryptedString;
        }

        private byte[] Execute(byte[] bytes, bool encrypt)
        {
            using (var aes = Aes.Create())
            {
                var derivedBytes = new Rfc2898DeriveBytes(encryptionKey, salt);
                aes.Key = derivedBytes.GetBytes(32);
                aes.IV = derivedBytes.GetBytes(16);
                
                using (var stream = new MemoryStream())
                {
                    var cryptoTransformer = encrypt ? aes.CreateEncryptor() : aes.CreateDecryptor();
                    using (var cryptoStream = new CryptoStream(stream, cryptoTransformer, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(bytes, 0, bytes.Length);
                    }

                    return stream.ToArray();
                }
            }
        }
    }
}