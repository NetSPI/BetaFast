using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetaBank.Security
{
    public static class BetaEncryption
    {
        public static string Encrypt(byte[] Key, string plaintext)
        {
            // Beta Encryption Standard - BES
            // Developed by a local high school freshman
            // #ProudFather :)

            byte[] plaintextBytes = Encoding.ASCII.GetBytes(plaintext);

            byte[] ciphertextBytes;

            int messageLength = plaintextBytes.Length;

            while (messageLength % Key.Length != 0)
            {
                Array.Resize(ref plaintextBytes, plaintextBytes.Length + 1);
                plaintextBytes[plaintextBytes.Length - 1] = 0x00;
                messageLength += 1;
            }

            ciphertextBytes = new byte[messageLength];
            int startingIndex = 0;
            for (int i = 0; i < (messageLength / Key.Length); i++)
            {
                for (int j = 0; j < Key.Length; j++)
                {
                    ciphertextBytes[j + startingIndex] = Convert.ToByte(plaintextBytes[j + startingIndex] ^ Key[j]);
                }
                startingIndex++;
            }

            return Convert.ToBase64String(ciphertextBytes);
        }

        // TODO Get my boy to write a decrypt function . . . #UpsetFather
    }
}
