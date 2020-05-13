using BetaFast.Controllers;
using BetaFast.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace BetaFast.Utilities
{
    public static class EncryptionUtility
    {
        private static readonly int _iterations = 20000;
        private static byte[] _retrievedSalt;

        public static byte[] Encrypt(string plaintext, string password)
        {
            Rfc2898DeriveBytes rfc2898DeriveBytes;
            byte[] salt = CreateSalt(8);

            try
            {
                StoreSalt(salt);
            }
            catch
            {
                throw new Exception("Could not store encryption data");
            }

            using (rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, salt, _iterations))
            {
                byte[] ciphertextBytes;

                using (Aes aes = Aes.Create())
                {
                    aes.Key = rfc2898DeriveBytes.GetBytes(aes.KeySize / 8);
                    aes.IV = rfc2898DeriveBytes.GetBytes(aes.BlockSize / 8);

                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(plaintext);
                            }
                            ciphertextBytes = msEncrypt.ToArray();
                        }
                    }
                }
                return ciphertextBytes;
            }
        }

        public static string Decrypt(byte[] ciphertext, string password)
        {
            string plaintext = string.Empty;
            Rfc2898DeriveBytes rfc2898DeriveBytes;
            byte[] salt = null;

            try
            {
                GetSalt();
                salt = _retrievedSalt;
                if (salt == null)
                {
                    throw new Exception("Could not retrieve encryption data.");
                }
            }
            catch
            {
                throw new Exception("Could not retrieve encryption data.");
            }
            using (rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, salt, _iterations))
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = rfc2898DeriveBytes.GetBytes(aes.KeySize / 8);
                    aes.IV = rfc2898DeriveBytes.GetBytes(aes.BlockSize / 8);

                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (MemoryStream msDecrypt = new MemoryStream(ciphertext))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                plaintext = srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }

                return plaintext;
            }
        }

        private static byte[] CreateSalt(int size)
        {
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] buff = new byte[size];
                rng.GetBytes(buff);

                return buff;
            }
        }

        private static void StoreSalt(byte[] salt)
        {
            Task t = Task.Run(async () =>
            {
                await SetSaltAsync(salt);
            });
            TimeSpan ts = TimeSpan.FromMilliseconds(1000);
            if (!t.Wait(ts))
                throw new TimeoutException();
        }

        private static void GetSalt()
        {
            Task t = Task.Run(async () =>
            {
                _retrievedSalt = await GetSaltAsync();
            });
            TimeSpan ts = TimeSpan.FromMilliseconds(1000);
            if (!t.Wait(ts))
            {
                throw new TimeoutException();
            }
        }

        public static async Task<byte[]> GetSaltAsync()
        {
            int status;
            string body;

            using (HttpResponseMessage response = await EncryptionController.GetSalt())
            {
                status = (int)response.StatusCode;
                body = await response.Content.ReadAsStringAsync();
            }

            if (status == 200)
            {
                return System.Convert.FromBase64String(body);
            }
            else if (status == 400)
            {
                throw new Exception(body);
            }
            else if (status == 401)
            {
                throw new UnauthenticatedException();
            }
            else if (status == 500)
            {
                throw new ServerException();
            }
            else
            {
                throw new Exception("Uncaught status code");
            }
        }

        public static async Task SetSaltAsync(byte[] salt)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "Salt", System.Convert.ToBase64String(salt) } };
            FormUrlEncodedContent encodedContent = new FormUrlEncodedContent(parameters);
            using (HttpResponseMessage response = await EncryptionController.SetSalt(encodedContent))
            {
                int status = (int)response.StatusCode;
                string body = await response.Content.ReadAsStringAsync();

                if (status == 200)
                {
                    return;
                }
                else if (status == 400)
                {
                    throw new Exception(body);
                }
                else if (status == 401)
                {
                    throw new UnauthenticatedException();
                }
                else if (status == 500)
                {
                    throw new ServerException();
                }
                else
                {
                    throw new Exception("An uncaught error occurred.");
                }
            }
        }
    }
}