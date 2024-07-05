using System;
using System.IO;
using System.Security.Cryptography;

namespace Aes_Example
{
    public static class AesExample
    {
        // Source: https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.aes?view=net-8.0

        private static readonly byte[] AKey = new byte[] { 14, 178, 77, 188, 135, 48, 207, 183, 132, 60, 207, 172, 12, 97, 136, 137, 238, 255, 73, 56, 1, 128, 84, 239, 103, 223, 27, 254, 230, 53, 123, 110 };
        private static readonly byte[] AIV = new byte[] { 235, 12, 16, 208, 72, 140, 36, 219, 138, 6, 224, 147, 195, 1, 28, 173 };

        private static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {

            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
            {
                throw new ArgumentNullException("plainText");
            }

            if (Key == null || Key.Length <= 0)
            {
                throw new ArgumentNullException("Key");
            }

            if (IV == null || IV.Length <= 0)
            {
                throw new ArgumentNullException("IV");
            }

            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        public static string Encrypt(string plainText)
        {
            if (plainText.Length == 0)
            {
                return "";
            }

            byte[] a = EncryptStringToBytes_Aes(plainText, AKey, AIV);
            return Convert.ToBase64String(a);
        }

        private static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException("cipherText");
            }

            if (Key == null || Key.Length <= 0)
            {
                throw new ArgumentNullException("Key");
            }

            if (IV == null || IV.Length <= 0)
            {
                throw new ArgumentNullException("IV");
            }

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }

        public static string Decrypt(string cipherText)
        {
            if (cipherText.Length == 0)
            {
                return "";
            }

            byte[] a = Convert.FromBase64String(cipherText);
            return DecryptStringFromBytes_Aes(a, AKey, AIV);
        }

    }
}