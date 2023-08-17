using System;
using System.IO;
using System.Security.Cryptography;

namespace FireBrowserDataBase.BankC
{
    public class EnqHelper
    {

        private static Aes CreateAes(byte[] key)
        {
            Aes aes = Aes.Create();
            aes.Key = key;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.BlockSize = 128;
            aes.IV = new byte[16]; // Initialize with appropriate IV if using CBC mode
            return aes;
        }

        public static byte[] GenerateEncryptionKey()
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.KeySize = 256; // Use a valid key size (128, 192, or 256)
            aesAlg.GenerateKey();
            return aesAlg.Key;
        }

        public static byte[] Encrypt(byte[] plaintext, byte[] key)
        {
            using Aes aes = CreateAes(key);
            using MemoryStream ms = new MemoryStream();
            using CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(plaintext, 0, plaintext.Length);
            return ms.ToArray();
        }

        public static byte[] Decrypt(byte[] ciphertext, byte[] key)
        {
            using Aes aes = CreateAes(key);
            using MemoryStream ms = new MemoryStream(ciphertext);
            using CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using MemoryStream outputMs = new MemoryStream();

            byte[] buffer = new byte[1024];
            int bytesRead;

            while ((bytesRead = cs.Read(buffer, 0, buffer.Length)) > 0)
            {
                outputMs.Write(buffer, 0, bytesRead);
            }

            byte[] decryptedData = outputMs.ToArray();

            // Remove PKCS7 padding manually
            int paddingLength = decryptedData[decryptedData.Length - 1];
            int originalPlaintextLength = decryptedData.Length - paddingLength;

            byte[] plaintext = new byte[originalPlaintextLength];
            Buffer.BlockCopy(decryptedData, 0, plaintext, 0, originalPlaintextLength);

            return plaintext;
        }
    }
}
