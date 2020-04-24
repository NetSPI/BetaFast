using System;
using System.Collections.Generic;
using System.Text;

namespace BetaFast.Utilities
{
    public static class RequestUtility
    {
        public static byte[] ConstructBody(SortedDictionary<string, string> parameters, byte[] password)
        {
            string bodyString = string.Empty;

            foreach(KeyValuePair<string, string> pairs in parameters)
            {
                bodyString += (pairs.Key + "=" + pairs.Value + "&");
            }

            bodyString += "Password=";

            byte[] bodyArrayNoPassword = Encoding.UTF8.GetBytes(bodyString);

            byte[] bodyArray = Utilities.ArrayUtility.MergeArrays(bodyArrayNoPassword, password);

            Array.Clear(bodyArrayNoPassword, 0, bodyArrayNoPassword.Length);

            return bodyArray;
        }
    }
}
