using System.Text.RegularExpressions;

namespace BetaFastAPI.Security
{
    public static class InputValidation
    {
        public static bool IsValidFileName(string name)
        {
            if (IsValidFormat(name) && ContainsValidCharacters(name))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool IsValidFormat(string name)
        {
            if ((name.Substring(name.Length - 4, 4).Equals(".jpg")) || (name.Substring(name.Length - 4, 4).Equals(".png")))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool ContainsValidCharacters(string name)
        {
            if (name.Contains("%") || name.Contains("/") || name.Contains(@"\") || (name.Substring(0, name.Length - 4).Contains(".")))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}