using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;
using BetaFast.Utilities;

namespace BetaFast.SecureStringRequests
{
    public static class SecureStringRequests
    {
        public static async Task SecureStringLogin(string username, SecureString password, Func<string, byte[], Task> Login)
        {
            // Pass in Login(string username, byte[] password) as function
            byte[] plaintext = null;
            GCHandle gcHandle = GCHandle.Alloc(plaintext, GCHandleType.Pinned);

            IntPtr unmanagedPtr = IntPtr.Zero;

            try
            {
                SecureStringUtility.SecureStringToByteArray(unmanagedPtr, password, ref plaintext);
                await Login(username, plaintext);
            }
            finally
            {
                SecureStringUtility.ClearPasswordFromMemory(unmanagedPtr, ref plaintext, gcHandle);
            }
        }

        public static async Task SecureStringRegistration(string username, string firstName, string lastName, SecureString password, Func<string, string, string, byte[], Task> Register)
        {
            IntPtr unmanagedPtr = IntPtr.Zero;
            byte[] plaintext = null;
            GCHandle gcHandle = GCHandle.Alloc(plaintext, GCHandleType.Pinned);

            try
            {
                SecureStringUtility.SecureStringToByteArray(unmanagedPtr, password, ref plaintext);
                await Register(lastName, firstName, username, plaintext);
            }
            finally
            {
                SecureStringUtility.ClearPasswordFromMemory(unmanagedPtr, ref plaintext, gcHandle);
            }
        }

        public static async Task SecureStringUpdatePassword(SecureString currentPassword, SecureString newPassword, SecureString confirmPassword, Func<byte[], byte[], byte[], Task> UpdatePassword)
        {
            IntPtr unmanagedPtr1 = IntPtr.Zero;
            byte[] plaintext1 = null;
            GCHandle gcHandle1 = GCHandle.Alloc(plaintext1, GCHandleType.Pinned);

            IntPtr unmanagedPtr2 = IntPtr.Zero;
            byte[] plaintext2 = null;
            GCHandle gcHandle2 = GCHandle.Alloc(plaintext1, GCHandleType.Pinned);

            IntPtr unmanagedPtr3 = IntPtr.Zero;
            byte[] plaintext3 = null;
            GCHandle gcHandle3 = GCHandle.Alloc(plaintext1, GCHandleType.Pinned);

            try
            {
                SecureStringUtility.SecureStringToByteArray(unmanagedPtr1, currentPassword, ref plaintext1);
                SecureStringUtility.SecureStringToByteArray(unmanagedPtr2, newPassword, ref plaintext2);
                SecureStringUtility.SecureStringToByteArray(unmanagedPtr3, confirmPassword, ref plaintext3);
                await UpdatePassword(plaintext1, plaintext2, plaintext3);
            }
            finally
            {
                SecureStringUtility.ClearPasswordFromMemory(unmanagedPtr1, ref plaintext1, gcHandle1);
                SecureStringUtility.ClearPasswordFromMemory(unmanagedPtr2, ref plaintext2, gcHandle2);
                SecureStringUtility.ClearPasswordFromMemory(unmanagedPtr3, ref plaintext3, gcHandle3);
            }
        }
    }
}
