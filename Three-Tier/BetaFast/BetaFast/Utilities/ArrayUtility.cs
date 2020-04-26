using System;

namespace BetaFast.Utilities
{
    public static class ArrayUtility
    {
        public static byte[] MergeArrays(byte[] array1, byte[] array2)
        {
            byte[] outputArray = new byte[array1.Length + array2.Length];
            Array.Copy(array1, outputArray, array1.Length);
            Array.Copy(array2, 0, outputArray, array1.Length, array2.Length);
            return outputArray;
        }
    }
}
