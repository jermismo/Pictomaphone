using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ExifPlus
{
    /// <summary>
    /// Holds classes to get info from the byte[]
    /// </summary>
    public static class ExifIO
    {
        /// <summary>
        /// Reads a Short from the specified offset.
        /// </summary>
        /// <param name="data">the byte array</param>
        /// <param name="offset">the offset the short is at</param>
        /// <param name="littleEndian">if the data is in little endian format</param>
        /// <returns></returns>
        public static short ReadShort(byte[] data, int offset, bool littleEndian)
        {
            short result;
            // if the system is the same endian
            if ((littleEndian && BitConverter.IsLittleEndian) || (!littleEndian && !BitConverter.IsLittleEndian))
            {
                result = BitConverter.ToInt16(data, offset);
            }
            else // if it isn't
            {
                // get bytes in reverse order
                byte[] value = new byte[]
                {
                    data[offset + 1], data[offset]
                };
                result = BitConverter.ToInt16(value, 0);
            }
            return result;
        }

        /// <summary>
        /// Reads a UShort from the specified offset.
        /// </summary>
        /// <param name="data">the byte array</param>
        /// <param name="offset">the offset the short is at</param>
        /// <param name="littleEndian">if the data is in little endian format</param>
        /// <returns></returns>
        public static ushort ReadUShort(byte[] data, int offset, bool littleEndian)
        {
            ushort result;
            // if the system is the same endian
            if ((littleEndian && BitConverter.IsLittleEndian) || (!littleEndian && !BitConverter.IsLittleEndian))
            {
                result = BitConverter.ToUInt16(data, offset);
            }
            else // if it isn't
            {
                // get bytes in reverse order
                byte[] value = new byte[]
                {
                    data[offset + 1], data[offset]
                };
                result = BitConverter.ToUInt16(value, 0);
            }
            return result;
        }

        /// <summary>
        /// Reads an Int32 from the specified offset.
        /// </summary>
        /// <param name="data">the byte array</param>
        /// <param name="offset">the offset the short is at</param>
        /// <param name="littleEndian">if the data is in little endian format</param>
        /// <returns></returns>
        public static int ReadInt(byte[] data, int offset, bool littleEndian)
        {
            int result;
            // if the system is the same endian
            if ((littleEndian && BitConverter.IsLittleEndian) || (!littleEndian && !BitConverter.IsLittleEndian))
            {
                result = BitConverter.ToInt32(data, offset);
            }
            else // if it isn't
            {
                // get bytes in reverse order
                byte[] value = new byte[]
                {
                    data[offset + 3], data[offset + 2], data[offset + 1], data[offset]
                };
                result = BitConverter.ToInt32(value, 0);
            }
            return result;
        }

        /// <summary>
        /// Reads a UInt32 from the specified offset.
        /// </summary>
        /// <param name="data">the byte array</param>
        /// <param name="offset">the offset the short is at</param>
        /// <param name="littleEndian">if the data is in little endian format</param>
        /// <returns></returns>
        public static uint ReadUInt(byte[] data, int offset, bool littleEndian)
        {
            uint result;
            // if the system is the same endian
            if ((littleEndian && BitConverter.IsLittleEndian) || (!littleEndian && !BitConverter.IsLittleEndian))
            {
                result = BitConverter.ToUInt32(data, offset);
            }
            else // if it isn't
            {
                // get bytes in reverse order
                byte[] value = new byte[]
                {
                    data[offset + 3], data[offset + 2], data[offset + 1], data[offset]
                };
                result = BitConverter.ToUInt32(value, 0);
            }
            return result;
        }

        /// Reads a Single from the specified offset.
        /// </summary>
        /// <param name="data">the byte array</param>
        /// <param name="offset">the offset the short is at</param>
        /// <param name="littleEndian">if the data is in little endian format</param>
        /// <returns></returns>
        public static float ReadSingle(byte[] data, int offset, bool littleEndian)
        {
            float result;
            // if the system is the same endian
            if ((littleEndian && BitConverter.IsLittleEndian) || (!littleEndian && !BitConverter.IsLittleEndian))
            {
                result = BitConverter.ToSingle(data, offset);
            }
            else // if it isn't
            {
                // get bytes in reverse order
                byte[] value = new byte[]
                {
                    data[offset + 3], data[offset + 2], data[offset + 1], data[offset]
                };
                result = BitConverter.ToSingle(value, 0);
            }
            return result;
        }

        /// Reads a Double from the specified offset.
        /// </summary>
        /// <param name="data">the byte array</param>
        /// <param name="offset">the offset the short is at</param>
        /// <param name="littleEndian">if the data is in little endian format</param>
        /// <returns></returns>
        public static double ReadDouble(byte[] data, int offset, bool littleEndian)
        {
            double result;
            // if the system is the same endian
            if ((littleEndian && BitConverter.IsLittleEndian) || (!littleEndian && !BitConverter.IsLittleEndian))
            {
                result = BitConverter.ToDouble(data, offset);
            }
            else // if it isn't
            {
                // get bytes in reverse order
                byte[] value = new byte[]
                {
                    data[offset + 7], data[offset + 6], data[offset + 5], data[offset + 4],
                    data[offset + 3], data[offset + 2], data[offset + 1], data[offset]
                };
                result = BitConverter.ToDouble(value, 0);
            }
            return result;
        }

    }
}
