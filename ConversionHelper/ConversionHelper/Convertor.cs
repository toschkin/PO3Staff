using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConversionHelper
{
    public static class Reverser
    {
        // reverse byte order (16-bit)
        public static short ReverseBytes(short value)
        {
            return (short)((value & 0xFFU) << 8 | (value & 0xFF00U) >> 8);
        }

        // reverse byte order (32-bit)

        public static int ReverseBytes(int value)
        {
            uint temp = unchecked((uint)value);
            temp = ReverseBytes(temp);
            return unchecked((int)temp); ;
        }

        // reverse byte order (16-bit)

        public static ushort ReverseBytes(ushort value)
        {
            return (ushort)((value & 0xFFU) << 8 | (value & 0xFF00U) >> 8);
        }

        // reverse byte order (32-bit)

        public static uint ReverseBytes(uint value)
        {
            return (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
                   (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;
        }

        // reverse byte order (64-bit)

        public static ulong ReverseBytes(ulong value)
        {
            return (value & 0x00000000000000FFUL) << 56 | (value & 0x000000000000FF00UL) << 40 |
                   (value & 0x0000000000FF0000UL) << 24 | (value & 0x00000000FF000000UL) << 8 |
                   (value & 0x000000FF00000000UL) >> 8 | (value & 0x0000FF0000000000UL) >> 24 |
                   (value & 0x00FF000000000000UL) >> 40 | (value & 0xFF00000000000000UL) >> 56;
        }

        public static long ReverseBytes(long value)
        {
            ulong temp = unchecked((ulong)value);
            temp = ReverseBytes(temp);
            return unchecked((long)temp); ;
        }
    }

    public static class Convertor
    {              
        /// <summary>
        /// Converts bytes from array to single floating point number
        /// </summary>
        /// <param name="array">input array of bytes</param>
        /// <param name="index">index of element of array from which to start convertion</param>
        /// <param name="reverseOrder">if true, elements from array will be took in reverse order</param>
        /// <returns>converted single floating point number</returns>
        public static float ConvertBytesToFloat(byte[] array, int index, bool reverseOrder = false)
        {
            if (index > array.Length - 4)
                throw new ArgumentOutOfRangeException();

            if (array == null)
                throw new ArgumentNullException();

            float floatValue = 0.0f;

            if (reverseOrder)
                Array.Reverse(array, index, 4);

            floatValue = BitConverter.ToSingle(array, index);

            return floatValue;
        }

        public static double ConvertBytesToDouble(byte[] array, int index, bool reverseOrder = false)
        {
            if (index > array.Length - 8)
                throw new ArgumentOutOfRangeException();

            if (array == null)
                throw new ArgumentNullException();

            double doubleValue = 0.0;

            if (reverseOrder)
                Array.Reverse(array, index, 8);

            doubleValue = BitConverter.ToDouble(array, index);

            return doubleValue;
        }

        public static decimal ConvertBytesToDecimal(byte[] array, int index, bool reverseOrder = false)
        {
            if (index > array.Length - 16)
                throw new ArgumentOutOfRangeException();

            if (array == null)
                throw new ArgumentNullException();

            decimal decimalValue = 0.0m;

            if (reverseOrder)
                Array.Reverse(array, index, 16);

            decimalValue = BitConverterEx.ToDecimal(array, index);

            return decimalValue;
        }

        public static string ConvertByteArrayToHexString(byte[] array)
        {
            StringBuilder hex = new StringBuilder(array.Length * 2);
            foreach (byte b in array)
                hex.AppendFormat("{0:x2} ", b);
            return hex.ToString();
        }

        public static float ConvertUShortsToFloat(ushort low, ushort high)
        {           
            byte[] bytes = new byte[4];
            bytes[0] = (byte)(low & 0xFF);
            bytes[1] = (byte)(low >> 8);
            bytes[2] = (byte)(high & 0xFF);
            bytes[3] = (byte)(high >> 8);
            return BitConverter.ToSingle(bytes, 0);
        }        
    }
}
