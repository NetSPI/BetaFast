using System;
using System.Security.Cryptography;
using System.Text;

namespace BetaFastAPI.Security
{
    public static class Hashing
    {
        public static string GenerateSaltedHash(string password, string saltString)
        {
            byte[] plainText = Encoding.UTF8.GetBytes(password);
            byte[] salt = Encoding.UTF8.GetBytes(saltString);

            byte[] plainTextWithSaltBytes =
              new byte[plainText.Length + salt.Length];

            for (int i = 0; i < plainText.Length; i++)
            {
                plainTextWithSaltBytes[i] = plainText[i];
            }
            for (int i = 0; i < salt.Length; i++)
            {
                plainTextWithSaltBytes[plainText.Length + i] = salt[i];
            }

            int hashValue = 0;

            for (int i = 0; i < plainTextWithSaltBytes.Length; i++)
            {
                hashValue = 31 * hashValue + plainTextWithSaltBytes[i];
            }

            return Convert.ToBase64String(BitConverter.GetBytes(hashValue));
        }

        public static string CreateSalt(int size)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[size];
            rng.GetBytes(buff);

            return Convert.ToBase64String(buff);
        }
    }
}
