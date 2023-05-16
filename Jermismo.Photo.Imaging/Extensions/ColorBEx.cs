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
    /// <summary>
    /// Extension methods for ColorB
    /// </summary>
    public static class ColorBEx
    {

        /// <summary>
        /// The byte values for luminance.
        /// </summary>
        public static readonly ColorB LumaColor = new ColorB(76, 150, 29);
        /// <summary>
        /// The byte values for luminance.
        /// </summary>
        public static readonly ColorI LumaColorI = new ColorI(76, 150, 29);

        #region * Min / Max *

        /// <summary>
        /// Returns the maximum color component value.
        /// </summary>
        public static byte GetMax(this ColorB color)
        {
            byte max = (color.Red > color.Green) ? color.Red : color.Green;
            if (color.Blue > max) max = color.Blue;
            return max;
        }

        /// <summary>
        /// Returns the maximum color component value.
        /// </summary>
        public static int GetMax(this ColorI color)
        {
            int max = (color.Red > color.Green) ? color.Red : color.Green;
            if (color.Blue > max) max = color.Blue;
            return max;
        }

        /// <summary>
        /// Returns the minimum color component value.
        /// </summary>
        public static byte GetMin(this ColorB color)
        {
            byte min = (color.Red < color.Green) ? color.Red : color.Green;
            if (color.Blue < min) min = color.Blue;
            return min;
        }

        /// <summary>
        /// Returns the minimum color component value.
        /// </summary>
        public static int GetMin(this ColorI color)
        {
            int min = (color.Red < color.Green) ? color.Red : color.Green;
            if (color.Blue < min) min = color.Blue;
            return min;
        }

        #endregion

        #region * Blend Modes *

        /// <summary>
        /// Alpha Blend.
        /// </summary>
        public static ColorB BlendAlpha(this ColorB bottom, ColorB top, int alpha)
        {
            if (alpha > 0 && alpha < 255)
                return new ColorB(
                    BlendAlpha(bottom.Alpha, top.Alpha, alpha),
                    BlendAlpha(bottom.Red, top.Red, alpha),
                    BlendAlpha(bottom.Green, top.Green, alpha),
                    BlendAlpha(bottom.Blue, top.Blue, alpha)
                    );
            else if (alpha == 255)
                return top;
            else
                return bottom;
        }

        /// <summary>
        /// Calculates Alpha Blending for a single color channel.
        /// </summary>
        public static int BlendAlpha(byte bottom, byte top, int alpha)
        {
            if (alpha > 0 && alpha < 255)
            {
                return ((alpha * top) + ((255 - alpha) * bottom) >> 8) + 1;
            }
            else if (alpha == 255) return top;
            else return bottom;
        }

        /// <summary>
        /// Does an Overlay Blend.
        /// </summary>
        public static ColorB BlendOverlay(this ColorB bottom, ColorB top, int alpha)
        {
            ColorB color = new ColorB(
                BlendOverlay(bottom.Red, top.Red),
                BlendOverlay(bottom.Green, top.Green),
                BlendOverlay(bottom.Blue, top.Blue)
                );
            return bottom.BlendAlpha(color, alpha);
        }

        /// <summary>
        /// Calculates an Overlay Blend for a single color component.
        /// </summary>
        /// <param name="bottom">The bottom color.</param>
        /// <param name="top">The top color.</param>
        /// <returns>New color.</returns>
        public static byte BlendOverlay(byte bottom, byte top)
        {
            return (top < 128) ? (byte)(((2 * bottom) * top) >> 8) : (byte)(255 - ((((255 - bottom) << 1) * (255 - top)) >> 8));
        }

        /// <summary>
        /// Does a Screen blend.
        /// </summary>
        public static ColorB BlendScreen(this ColorB bottom, ColorB top, int alpha)
        {
            ColorB color = new ColorB(
                (byte)(255 - (((255 - bottom.Red) * (255 - top.Red)) >> 8)),
                (byte)(255 - (((255 - bottom.Green) * (255 - top.Green)) >> 8)),
                (byte)(255 - (((255 - bottom.Blue) * (255 - top.Blue)) >> 8))
                );
            return bottom.BlendAlpha(color, alpha);
        }

        /// <summary>
        /// Does a Subtract blend.
        /// </summary>
        public static ColorB BlendSubtract(this ColorB bottom, ColorB top, int alpha)
        {
            ColorB color = new ColorB(
                (bottom.Red + top.Red - 255).ClampToByte(),
                (bottom.Green + top.Green - 255).ClampToByte(),
                (bottom.Blue + top.Blue - 255).ClampToByte()
                );
            return bottom.BlendAlpha(color, alpha);
        }

        public static ColorB BlendLinearBurn(this ColorB bottom, ColorB top, int alpha)
        {
            return top.BlendSubtract(bottom, alpha);
        }

        /// <summary>
        /// Linear Interpolation
        /// </summary>
        public static ColorB Lerp(this ColorB from, ColorB to, byte fraction)
        {
            byte inv = (byte)(255 - fraction);
            int a = from.Alpha.Scale(inv) + to.Alpha.Scale(fraction);
            int r = from.Red.Scale(inv) + to.Red.Scale(fraction);
            int g = from.Green.Scale(inv) + to.Green.Scale(fraction);
            int b = from.Blue.Scale(inv) + to.Green.Scale(fraction);
            return new ColorB(a, r, g, b);
        }

        /// <summary>
        /// Linear Interpolation
        /// </summary>
        public static ColorB Lerp(this ColorB from, ColorB to, int fraction)
        {
            int inv = (255 - fraction);
            int a = from.Alpha.Scale(inv) + to.Alpha.Scale(fraction);
            int r = from.Red.Scale(inv) + to.Red.Scale(fraction);
            int g = from.Green.Scale(inv) + to.Green.Scale(fraction);
            int b = from.Blue.Scale(inv) + to.Green.Scale(fraction);
            return new ColorB(a, r, g, b);
        }

        #endregion

        #region * Color Lightness *

        /// <summary>
        /// Gets the Lightness of the color.
        /// </summary>
        public static byte GetLightness(this ColorB color)
        {
            return (byte)((color.GetMax() + color.GetMin()) >> 1);
        }

        /// <summary>
        /// Gets the Lightness of the color.
        /// </summary>
        public static int GetLightness(this ColorI color)
        {
            return ((color.GetMax() + color.GetMin()) >> 1);
        }

        /// <summary>
        /// Gets the Intensity of the color.
        /// </summary>
        public static byte GetIntensity(this ColorB color)
        {
            return (byte)((color.Red + color.Green + color.Blue) / 3);
        }

        /// <summary>
        /// Gets the Intensity of the color.
        /// </summary>
        public static int GetIntensity(this ColorI color)
        {
            return ((color.Red + color.Green + color.Blue) / 3);
        }

        /// <summary>
        /// Gets the Luminance of the color.
        /// </summary>
        public static byte GetLuma(this ColorB color)
        {
            ColorB temp = color * LumaColor;
            return (byte)(temp.Red + temp.Green + temp.Blue);
        }

        /// <summary>
        /// Gets the Luminance of the color.
        /// </summary>
        public static int GetLuma(this ColorI color)
        {
            ColorI temp = color * LumaColorI;
            return temp.Red + temp.Green + temp.Blue;
        }

        #endregion

        //public static void BlendDifference(ref int bottom, ref int top, out int o)
        //{
        //    o = Math.Abs((int)(bottom - top));
        //}

        //public static void BlendExclusion(ref int bottom, ref int top, out int o)
        //{
        //    o = (bottom + top) - (((2 * bottom) * top) / 0xff);
        //}

        //public static void BlendGlow(ref int bottom, ref int top, out int o)
        //{
        //    BlendReflect(ref top, ref bottom, out o);
        //}

        //public static void BlendHardLight(ref int bottom, ref int top, out int o)
        //{
        //    BlendOverlay(ref top, ref bottom, out o);
        //}

        //public static void BlendHardMix(ref int bottom, ref int top, out int o)
        //{
        //    BlendVividLight(ref bottom, ref top, out o);
        //    o = (o < 0x80) ? 0 : 0xff;
        //}

        //public static void BlendLighten(ref int bottom, ref int top, out int o)
        //{
        //    o = (top > bottom) ? top : bottom;
        //}

        //public static void BlendLinearBurn(ref int bottom, ref int top, out int o)
        //{
        //    BlendSubtract(ref bottom, ref top, out o);
        //}

        //public static void BlendLinearDodge(ref int bottom, ref int top, out int o)
        //{
        //    BlendAdd(ref bottom, ref top, out o);
        //}

        //public static void BlendLinearLight(ref int bottom, ref int top, out int o)
        //{
        //    if (top < 0x80)
        //    {
        //        int num = 2 * top;
        //        BlendLinearBurn(ref bottom, ref num, out o);
        //    }
        //    else
        //    {
        //        int num2 = 2 * (top - 0x80);
        //        BlendLinearDodge(ref bottom, ref num2, out o);
        //    }
        //}

        //public static void BlendMultiply(ref int bottom, ref int top, out int o)
        //{
        //    o = (bottom * top) / 0xff;
        //}

        //public static void BlendNegation(ref int bottom, ref int top, out int o)
        //{
        //    o = 0xff - Math.Abs((int)((0xff - bottom) - top));
        //}

        //public static void BlendPinLight(ref int bottom, ref int top, out int o)
        //{
        //    if (top < 0x80)
        //    {
        //        int num = 2 * top;
        //        BlendDarken(ref bottom, ref num, out o);
        //    }
        //    else
        //    {
        //        int num2 = 2 * (top - 0x80);
        //        BlendLighten(ref bottom, ref num2, out o);
        //    }
        //}

        //public static void BlendReflect(ref int bottom, ref int top, out int o)
        //{
        //    o = (top == 0xff) ? top : Math.Min(0xff, (bottom * bottom) / (0xff - top));
        //}

        //public static void BlendSoftLight(ref int bottom, ref int top, out int o)
        //{
        //    o = (top < 0x80) ? ((int)((2 * ((bottom >> 1) + 0x40)) * (((float)top) / 255f))) : ((int)(255f - (((2 * (0xff - ((bottom >> 1) * 0x40))) * (0xff - top)) / 255f)));
        //}

        //public static void BlendVividLight(ref int bottom, ref int top, out int o)
        //{
        //    if (top < 0x80)
        //    {
        //        BlendColorBurn(ref bottom, ref top, out o);
        //    }
        //    else
        //    {
        //        int num = 2 * (top - 0x80);
        //        BlendColorDodge(ref bottom, ref num, out o);
        //    }
        //}

    }
}
