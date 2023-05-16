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

namespace Jermismo.Photo.Imaging
{
    public struct ColorI
    {
        public int Alpha;
        public int Red;
        public int Green;
        public int Blue;

        public ColorI(byte red, byte green, byte blue)
        {
            Alpha = 255;
            Red = red;
            Green = green;
            Blue = blue;
        }

        public ColorI(int alpha, int red, int green, int blue)
        {
            Alpha = alpha;
            Red = red;
            Green = green;
            Blue = blue;
        }

        public static implicit operator ColorB(ColorI color)
        {
            return new ColorB(color.Alpha, color.Red, color.Green, color.Blue);
        }

        public static implicit operator ColorI(ColorB color)
        {
            return new ColorI(color.Alpha, color.Red, color.Green, color.Blue);
        }

        public static ColorI operator +(ColorI first, ColorI second)
        {
            return new ColorI(
                 first.Alpha,
                 first.Red + second.Red,
                 first.Green + second.Green,
                 first.Blue + second.Blue
            );
        }

        public static ColorI operator +(ColorI first, int second)
        {
            return new ColorI(
                 first.Alpha,
                 first.Red + second,
                 first.Green + second,
                 first.Blue + second
            );
        }

        public static ColorI operator -(ColorI first, ColorI second)
        {
            return new ColorI(
                 first.Alpha,
                 first.Red - second.Red,
                 first.Green - second.Green,
                 first.Blue - second.Blue
            );
        }

        public static ColorI operator -(ColorI first, int second)
        {
            return new ColorI(
                 first.Alpha,
                 first.Red - second,
                 first.Green - second,
                 first.Blue - second
            );
        }

        public static ColorI operator *(ColorI first, ColorI second)
        {
            return new ColorI(
                 first.Alpha,
                 first.Red * second.Red >> 8,
                 first.Green * second.Green >> 8,
                 first.Blue * second.Blue >> 8
            );
        }

        public static ColorI operator *(ColorI first, int second)
        {
            return new ColorI(
                 first.Alpha,
                 first.Red * second >> 8,
                 first.Green * second >> 8,
                 first.Blue * second >> 8
            );
        }

        public static ColorI operator /(ColorI first, ColorI second)
        {
            return new ColorI(
                 first.Alpha,
                 first.Red / second.Red,
                 first.Green / second.Green,
                 first.Blue / second.Blue
            );
        }

        public static ColorI operator /(ColorI first, int second)
        {
            return new ColorI(
                 first.Alpha,
                 first.Red / second,
                 first.Green / second,
                 first.Blue / second
            );
        }

    }
}
