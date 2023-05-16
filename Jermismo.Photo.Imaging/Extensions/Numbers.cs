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

using Microsoft.Xna.Framework;

namespace Jermismo.Photo.Imaging
{
    public static class Numbers
    {
        /// <summary>
        /// Clamps the value between 0 and 1.
        /// </summary>
        public static float Clamp(this float f)
        {
            if (f > 1f) return 1f;
            if (f < 0f) return 0f;
            return f;
        }

        /// <summary>
        /// Clamps the values between 0 and 1.
        /// </summary>
        public static Vector4 Clamp(this Vector4 v)
        {
            return new Vector4(v.X.Clamp(), v.Y.Clamp(), v.Z.Clamp(), v.W.Clamp());
        }

        /// <summary>
        /// Clamps the value between 0 and 255.
        /// </summary>
        public static byte ClampToByte(this int i)
        {
            if (i > 255) return 255;
            else if (i < 0) return 0;
            return (byte)i;
        }

        /// <summary>
        /// Clamps the value between 0 and 255.
        /// </summary>
        public static byte ClampToByte(this float f)
        {
            if (f > 255f) return 255;
            else if (f < 0f) return 0;
            return (byte)f;
        }

        /// <summary>
        /// Clamps the value between 0 and 255.
        /// </summary>
        public static byte ClampToByte(this double d)
        {
            if (d > 255f) return 255;
            else if (d < 0f) return 0;
            return (byte)d;
        }

        /// <summary>
        /// Scales the byte by the specified amount
        /// </summary>
        public static byte Scale(this byte b, byte amount)
        {
            int r1 = (b * amount) + 128;
            int r2 = ((r1 >> 8) + r1) >> 8;
            return (byte)r2;
        }

        /// <summary>
        /// Scales the byte by the specified amount
        /// </summary>
        public static byte Scale(this byte b, int amount)
        {
            int r1 = (b * amount) + 128;
            int r2 = ((r1 >> 8) + r1) >> 8;
            return (byte)r2;
        }

        public static float GetMaxXyz(this Vector4 vec)
        {
            if (vec.X > vec.Y)
            {
                if (vec.X > vec.Z) return vec.X;
                return vec.Z;
            }
            else
            {
                if (vec.Y > vec.Z) return vec.Y;
                return vec.Z;
            }
        }

        public static float GetMaxXyz(float x, float y, float z)
        {
            if (x > y)
            {
                if (x > z) return x;
                return z;
            }
            else
            {
                if (y > z) return y;
                return z;
            }
        }

        public static float GetMinXyz(this Vector4 vec)
        {
            if (vec.X < vec.Y)
            {
                if (vec.X < vec.Z) return vec.X;
                return vec.Z;
            }
            else
            {
                if (vec.Y < vec.Z) return vec.Y;
                return vec.Z;
            }
        }

        public static float GetMinXyz(float x, float y, float z)
        {
            if (x < y)
            {
                if (x < z) return x;
                return z;
            }
            else
            {
                if (y < z) return y;
                return z;
            }
        }

    }
}
