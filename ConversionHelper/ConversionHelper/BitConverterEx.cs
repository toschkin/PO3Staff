using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConversionHelper
{
    //From http://social.technet.microsoft.com/wiki/contents/articles/19055.convert-system-decimal-to-and-from-byte-arrays-vb-c.aspx
    public class BitConverterEx
    {
        public static byte[] GetBytes(decimal dec)
        {
            //Load four 32 bit integers from the Decimal.GetBits function
            int[] bits = decimal.GetBits(dec);
            //Create a temporary list to hold the bytes
            List<byte> bytes = new List<byte>();
            //iterate each 32 bit integer
            foreach (int i in bits)
            {
                //add the bytes of the current 32bit integer
                //to the bytes list
                bytes.AddRange(BitConverter.GetBytes(i));
            }
            //return the bytes list as an array
            return bytes.ToArray();
        }
        public static decimal ToDecimal(byte[] bytes, int offset)
        {
            if (bytes == null)
                throw new ArgumentNullException();
            //check that it is even possible to convert the array
            if (offset > bytes.Length - 16)
                throw new ArgumentException();
            return new decimal(
                                BitConverter.ToInt32(bytes, offset),
                                BitConverter.ToInt32(bytes, offset + 4),
                                BitConverter.ToInt32(bytes, offset + 8),
                                bytes[offset + 15] == (byte)128,
                                bytes[offset + 14]);
        }
    }
}
