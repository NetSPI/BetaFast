using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace BetaBank.Utilities
{
    public static class SecureStringUtility
    {
        public static void SecureStringToByteArray(IntPtr ptr, SecureString ciphertext, ref byte[] plaintext)
        {
            ptr = Marshal.SecureStringToBSTR(ciphertext);
            plaintext = new byte[ciphertext.Length];

            unsafe
            {
                // Copy without null bytes
                byte* bstr = (byte*)ptr;

                for (int i = 0; i < plaintext.Length; i++)
                {
                    plaintext[i] = *bstr++;
                    *bstr = *bstr++;
                }
            }
        }

        public static string SecureStringToString(SecureString secureString)
        {
            IntPtr unmanagedPtr = IntPtr.Zero;
            byte[] plaintext = null;
            GCHandle gcHandle = GCHandle.Alloc(plaintext, GCHandleType.Pinned);

            SecureStringToByteArray(unmanagedPtr, secureString, ref plaintext);

            string normalString = Encoding.UTF8.GetString(plaintext, 0, plaintext.Length);

            ClearPasswordFromMemory(unmanagedPtr, ref plaintext, gcHandle);

            return normalString;
        }

        public static void ClearPasswordFromMemory(IntPtr ptr, ref byte[] plaintext, GCHandle gcHandle)
        {
            Array.Clear(plaintext, 0, plaintext.Length);
            gcHandle.Free();
            if (ptr != IntPtr.Zero)
                Marshal.ZeroFreeBSTR(ptr);
        }

        public static void FillSecureString(SecureString destination, SecureString source)
        {
            destination.Clear();
            byte[] plaintext = null;
            char[] characters = null;
            GCHandle gcHandle = GCHandle.Alloc(plaintext, GCHandleType.Pinned);

            IntPtr unmanagedPtr = IntPtr.Zero;

            try
            {
                SecureStringUtility.SecureStringToByteArray(unmanagedPtr, source, ref plaintext);
                characters = System.Text.Encoding.UTF8.GetString(plaintext).ToCharArray();
                foreach (Char c in characters)
                {
                    destination.AppendChar(c);
                }
            }
            finally
            {
                Array.Clear(characters, 0, characters.Length);
                Array.Clear(plaintext, 0, plaintext.Length);
                gcHandle.Free();
                if (unmanagedPtr != IntPtr.Zero)
                    Marshal.ZeroFreeBSTR(unmanagedPtr);
            }
        }

        public static SecureString StringToSecureString(string input)
        {
            SecureString output = new SecureString();
            if (!string.IsNullOrEmpty(input))
            {
                for (int i = 0; i < input.Length; i++)
                {
                    output.AppendChar(input[i]);
                }
            }

            return output;
        }
    }
}