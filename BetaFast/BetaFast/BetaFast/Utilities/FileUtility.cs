using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace BetaFast.Utilities
{
    public static class FileUtility
    {
        // Code from https://docs.microsoft.com/en-us/dotnet/api/system.security.accesscontrol.filesecurity?redirectedfrom=MSDN&view=netframework-4.8

        public static void AddFileSecurity(string fileName, string account, FileSystemRights rights, AccessControlType controlType)
        {
            FileSecurity fSecurity = File.GetAccessControl(fileName);

            fSecurity.AddAccessRule(new FileSystemAccessRule(account,
                rights, controlType));

            File.SetAccessControl(fileName, fSecurity);
        }

        public static void RemoveFileSecurity(string fileName, string account, FileSystemRights rights, AccessControlType controlType)
        {
            FileSecurity fSecurity = File.GetAccessControl(fileName);

            fSecurity.RemoveAccessRule(new FileSystemAccessRule(account,
                rights, controlType));

            File.SetAccessControl(fileName, fSecurity);
        }

        public static void SetReadWriteCurrentUser(string fileName)
        {
            AddFileSecurity(fileName, System.Security.Principal.WindowsIdentity.GetCurrent().Name, FileSystemRights.ReadData, AccessControlType.Allow);
        }

        public static void SetReadAllUsers(string fileName)
        {
            FileSecurity fSecurity = File.GetAccessControl(fileName);

            fSecurity.AddAccessRule(new FileSystemAccessRule( new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.ReadData, AccessControlType.Allow));

            File.SetAccessControl(fileName, fSecurity);
        }
    }
}
