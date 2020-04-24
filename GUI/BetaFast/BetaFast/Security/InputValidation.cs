using System.Text.RegularExpressions;

namespace BetaFast.Security
{
    public static class InputValidation
    {
        public static bool IsAlphaNumeric(string text)
        {
            Regex regex = new Regex("^[a-zA-Z0-9]*$");
            if (regex.IsMatch(text))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
