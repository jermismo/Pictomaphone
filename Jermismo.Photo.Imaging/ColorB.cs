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

using System.Runtime.InteropServices;

namespace Jermismo.Photo.Imaging
{

    /// <summary>
    /// Holds the Bytes of a Color.
    /// The int "Value" is the combined value of the pixel color (like from Writeable Bitmap)
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct ColorB : IPixel
    {
        // total int value
        [FieldOffset(0)]
        public int Value;

        // the bytes from the int
        [FieldOffset(0)]
        public byte Blue;
        [FieldOffset(1)]
        public byte Green;
        [FieldOffset(2)]
        public byte Red;
        [FieldOffset(3)]
        public byte Alpha;

        #region * Constructors *

        /// <summary>
        /// Creates a new ColorB from an int value.
        /// </summary>
        /// <param name="value">The pixel value.</param>
        public ColorB(int value)
        {
            // initialize bytes first so the compiler won't get mad
            Blue = Green = Red = Alpha = 0;

            this.Value = value;
        }

        /// <summary>
        /// Creates a new ColorB with the specified values.
        /// </summary>
        /// <param name="red">The Red Component.</param>
        /// <param name="green">The Green Component.</param>
        /// <param name="blue">The Blue Component.</param>
        public ColorB(byte red, byte green, byte blue)
        {
            Value = 0; // so the compiler won't get mad
            Alpha = 255;
            Red = red;
            Green = green;
            Blue = blue;
        }

        /// <summary>
        /// Creates a new ColorB with the specified grayscale value.
        /// </summary>
        /// <param name="gray">The Value for the RGB Components.</param>
        public ColorB(byte gray)
        {
            Value = 0; // so the compiler won't get mad
            Alpha = 255;
            Red = Green = Blue = gray;
        }

        /// <summary>
        /// Creates a new ColorB with the specified grayscale value.
        /// </summary>
        public ColorB(byte alpha, byte gray)
        {
            Value = 0;
            Alpha = alpha;
            Red = Green = Blue = gray;
        }

        /// <summary>
        /// Creates a new ColorB with the specified values.
        /// </summary>
        /// <param name="alpha">The Alpha Component.</param>
        /// <param name="red">The Red Component.</param>
        /// <param name="green">The Green Component.</param>
        /// <param name="blue">The Blue Component.</param>
        public ColorB(int alpha, int red, int green, int blue)
        {
            Value = 0; // so the compiler won't get mad
            Alpha = (byte)alpha;
            Red = (byte)red;
            Green = (byte)green;
            Blue = (byte)blue;
        }

        /// <summary>
        /// Creates a new ColorB from a Vector4 color.
        /// </summary>
        /// <param name="color">Vector4 (w=alpha, x=red, y=green, z=blue)</param>
        public ColorB(Microsoft.Xna.Framework.Vector4 color)
        {
            Value = 0; // again with the compiler
            Alpha = (byte)(color.W * 255);
            Red = (byte)(color.X * 255);
            Green = (byte)(color.Y * 255);
            Blue = (byte)(color.Z * 255);
        }

        #endregion

        #region * Operators *

        public static implicit operator ColorB(int value)
        {
            return new ColorB(value);
        }

        public static implicit operator int(ColorB color)
        {
            return color.Value;
        }

        public static ColorB operator +(ColorB first, ColorB second)
        {
            return new ColorB(
                 first.Alpha,
                 (first.Red + second.Red).ClampToByte(),
                 (first.Green + second.Green).ClampToByte(),
                 (first.Blue + second.Blue).ClampToByte()
            );
        }

        public static ColorB operator +(ColorB first, int second)
        {
            return new ColorB(
                first.Alpha,
                (first.Red + second).ClampToByte(),
                (first.Green + second).ClampToByte(),
                (first.Blue + second).ClampToByte()
           );
        }

        public static ColorB operator -(ColorB first, ColorB second)
        {
            return new ColorB(
                first.Alpha,
                (first.Red - second.Red).ClampToByte(),
                (first.Green - second.Green).ClampToByte(),
                (first.Blue - second.Blue).ClampToByte()
           );
        }

        public static ColorB operator -(ColorB first, int second)
        {
            return new ColorB(
                first.Alpha,
                (first.Red - second).ClampToByte(),
                (first.Green - second).ClampToByte(),
                (first.Blue - second).ClampToByte()
           );
        }

        public static ColorB operator *(ColorB first, ColorB second)
        {
            // multiply then divide by 255 (bit shift, because its faster)
            return new ColorB(
                first.Alpha,
                first.Red * second.Red >> 8,
                first.Green * second.Green >> 8,
                first.Blue * second.Blue >> 8
           );
        }

        public static ColorB operator *(ColorB first, int second)
        {
            // multiply then divide by 255 (bit shift, because its faster)
            return new ColorB(
                first.Alpha,
                first.Red * second >> 8,
                first.Green * second >> 8,
                first.Blue * second >> 8
           );
        }

        public static ColorB operator <<(ColorB first, int second)
        {
            return new ColorB(
                first.Alpha,
                first.Red << second,
                first.Green << second,
                first.Blue << second
                );
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.Value;
        }

        public override string ToString()
        {
            return "A:" + Alpha + " R:" + Red + " G:" + Green + " B:" + Blue;
        }

        #endregion

        #region * Public Static *

        public static ColorB White = new ColorB(255, 255);
        public static ColorB Black = new ColorB(255, 0);

        #endregion

        /// <summary>
        /// For IPixel.
        /// </summary>
        public int GetInt() { return Value; }

    }
}
