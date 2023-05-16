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
    /// <summary>
    /// Hue Saturation and Luminance
    /// </summary>
    public struct Hsl : IPixel
    {

        public const float HueScale = 0.0166666666666667f;
        public const float SatScale = 0.01f;

        /// <summary>
        /// Hue, Saturation, Lightness, Chroma
        /// </summary>
        private Vector4 color;

        /// <summary>
        /// Hue, Saturation, Lightness, Chroma (not used)
        /// </summary>
        public Vector4 Color { get { return color; } set { color = value; } }

        /// <summary>
        /// The Hue component of the color.
        /// </summary>
        public float Hue
        {
            get { return color.X; }
            set { OnHueChanged(value); }
        }

        /// <summary>
        /// The Saturation component of the color.
        /// </summary>
        public float Saturation
        {
            get { return color.Y; }
            set { color.Y = value.Clamp(); }
        }

        /// <summary>
        /// The Lightness component of the color.
        /// </summary>
        public float Lightness
        {
            get { return color.Z; }
            set { color.Z = value.Clamp(); }
        }

        /// <summary>
        /// The Chroma component of the color.
        /// Used internally for converting to RGB.
        /// </summary>
        public float Chroma
        {
            get { return color.W; }
        }

        /// <summary>
        /// Creates a new instance of the Hsl class from a Vector4.
        /// </summary>
        /// <param name="color">H,S,L and C values.</param>
        public Hsl(Vector4 color)
        {
            this.color = color;
        }

        /// <summary>
        /// Creates a new instance of the Hsl class from an Int32 pixel
        /// </summary>
        public Hsl(int color)
        {
            ColorB col = new ColorB(color);
            this.color = FromRgbF(
                col.Red * Argb.FloatScale,
                col.Green * Argb.FloatScale,
                col.Blue * Argb.FloatScale
                );
        }

        /// <summary>
        /// Creates a new instance of the Hsl class from an Argb.
        /// </summary>
        /// <param name="argb">Argb instance.</param>
        public Hsl(Argb argb)
        {
            this.color = FromRgbF(argb.Red, argb.Green, argb.Blue);
        }

        /// <summary>
        /// Creates a new instance of the Hsl class from a ColorB.
        /// </summary>
        public Hsl(ColorB color)
        {
            this.color = FromRgbF(
                color.Red * Argb.FloatScale,
                color.Green * Argb.FloatScale,
                color.Blue * Argb.FloatScale
                );
        }

        /// <summary>
        /// Creates a new Hsl instance from the H,S,L values.
        /// </summary>
        /// <param name="hue">Hue [0..360]</param>
        /// <param name="saturation">Saturation [0..100]</param>
        /// <param name="lightness">Lightness [0..100]</param>
        public Hsl(int hue, int saturation, int lightness)
        {
            color = new Vector4(
                hue * HueScale, 
                saturation * SatScale, 
                lightness * SatScale, 
                0);
        }

        private void OnHueChanged(float value)
        {
            if (value < 0) value += 6;
            else if (value > 6) value -= 6;
            color.X = value;
        }

        public static Vector4 FromRgbF(float red, float green, float blue)
        {
            Vector4 color = Vector4.Zero;
            float min = Numbers.GetMinXyz(red, green, blue);
            float max = Numbers.GetMaxXyz(red, green, blue);
            float chroma = color.W = max - min;

            color.Z = 0.5f * (max + min);

            if (color.W != 0) // 0 would mean gray, so hue and sat can be 0 as well
            {
                if (max == red) color.X = ((green - blue) / chroma) % 6f;
                else if (max == green) color.X = ((blue - red) / chroma) + 2f;
                else if (max == blue) color.X = ((red - green) / chroma) + 4f;

                color.Y = (color.Z <= 0.5f) ? chroma / (2f * color.Z) : chroma / (2 - (2f * color.Z));
            }

            if (color.X < 0f) color.X += 6f;

            return color;
        }

        public static Vector4 ToArgb(Hsl hsl)
        {
            Vector4 color = Vector4.One;
            
            float floor = (float)Math.Floor(hsl.Hue);
            float c = (hsl.Lightness <= 0.5f) ? (2f * hsl.Lightness * hsl.Saturation) : ((2f - (2f * hsl.Lightness)) * hsl.Saturation);
            float x = hsl.Hue - floor;
            float m = hsl.Lightness - (0.5f * c);

            if (floor == 0)      { x = x * c;       color.X = c + m; color.Y = x + m; color.Z = m; }
            else if (floor == 1) { x = (1 - x) * c; color.X = x + m; color.Y = c + m; color.Z = m; }
            else if (floor == 2) { x = x * c;       color.X = m;     color.Y = c + m; color.Z = x + m; }
            else if (floor == 3) { x = (1 - x) * c; color.X = m;     color.Y = x + m; color.Z = c + m; }
            else if (floor == 4) { x = x * c;       color.X = x + m; color.Y = m;     color.Z = c + m; }
            else                 { x = (1 - x) * c; color.X = c + m; color.Y = m;     color.Z = x + m; }

            return color.Clamp();
        }

        public static ColorB ToColorB(Hsl hsl)
        {
            float red, green, blue;

            float floor = (float)Math.Floor(hsl.Hue);
            float c = (hsl.Lightness <= 0.5f) ? (2f * hsl.Lightness * hsl.Saturation) : ((2f - (2f * hsl.Lightness)) * hsl.Saturation);
            float x = hsl.Hue - floor;
            float m = hsl.Lightness - (0.5f * c);

            if (floor == 0) { x = x * c; red = c + m; green = x + m; blue = m; }
            else if (floor == 1) { x = (1 - x) * c; red = x + m; green = c + m; blue = m; }
            else if (floor == 2) { x = x * c; red = m; green = c + m; blue = x + m; }
            else if (floor == 3) { x = (1 - x) * c; red = m; green = x + m; blue = c + m; }
            else if (floor == 4) { x = x * c; red = x + m; green = m; blue = c + m; }
            else { x = (1 - x) * c; red = c + m; green = m; blue = x + m; }

            return new ColorB((red * 255).ClampToByte(), (green * 255).ClampToByte(), (blue * 255).ClampToByte());
        }

        public Argb ToArgb()
        {
            return new Argb(Hsl.ToArgb(this));
        }

        public ColorB ToColorB()
        {
            return Hsl.ToColorB(this);
        }

        public int ToInt()
        {
            return Hsl.ToColorB(this).Value;
        }

        /// <summary>
        /// For IPixel.
        /// </summary>
        public int GetInt()
        {
            return ToInt();
        }
    }
}
