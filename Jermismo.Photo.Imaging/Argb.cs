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
using xna = Microsoft.Xna.Framework;

namespace Jermismo.Photo.Imaging
{
    public struct Argb
    {

        public const float LumaRed = 0.299f;
        public const float LumaGreen = 0.587f;
        public const float LumaBlue = 0.114f;

        /// <summary>
        /// Used instead of (int / 255) for converting to float.
        /// </summary>
        public const float FloatScale = 0.003921568627451f;

        private Vector4 color;

        #region * Properties *

        /// <summary>
        /// The Alpha component of the color.
        /// </summary>
        public float Alpha 
        { 
            get { return color.W; }
            set { color.W = value; }
        }

        /// <summary>
        /// The Red component of the color.
        /// </summary>
        public float Red 
        { 
            get { return color.X; }
            set { color.X = value; }
        }

        /// <summary>
        /// The Red component as an Int32.
        /// Note: this requires a math step.
        /// </summary>
        public int RedInt
        {
            get { return (int)(color.X * 255f); }
            set { color.X = value * FloatScale; }
        }

        /// <summary>
        /// The Green component of the color.
        /// </summary>
        public float Green 
        { 
            get { return color.Y; }
            set { color.Y = value; }
        }

        /// <summary>
        /// The Green component as an Int32.
        /// Note: this requires a math step.
        /// </summary>
        public int GreenInt
        {
            get { return (int)(color.Y * 255f); }
            set { color.Y = value * FloatScale; }
        }

        /// <summary>
        /// The Blue component of the color.
        /// </summary>
        public float Blue 
        { 
            get { return color.Z; }
            set { color.Z = value; }
        }

        /// <summary>
        /// The Blue component as an Int32.
        /// Note: this requires a math step.
        /// </summary>
        public int BlueInt
        {
            get { return (int)(color.Z * 255f); }
            set { color.Z = value * FloatScale; }
        }

        /// <summary>
        /// The Color as a Vector4. 
        /// W=Alpha, X=Red, Y=Green, Z=Blue
        /// </summary>
        public Vector4 Color { get { return color; } set { color = value; } }

        #endregion

        #region * Constructors *

        /// <summary>
        /// Create a new Argb from a Vector4.
        /// </summary>
        public Argb(Vector4 vector)
        {
            this.color = vector;
        }

        /// <summary>
        /// Create a new Argb instance.
        /// </summary>
        public Argb(int alpha, int red, int green, int blue)
        {
            color = new Vector4(
                red * FloatScale,
                green * FloatScale,
                blue * FloatScale,
                alpha * FloatScale);
        }

        /// <summary>
        /// Creates a new Argb instance from an int color.
        /// </summary>
        /// <param name="color">The int that represents the Argb value.</param>
        public Argb(int color)
        {
            this.color = Vector4.Zero;
            FromInt(color);
        }

        /// <summary>
        /// Creates a new Argb instance from the alpha and lightness values.
        /// </summary>
        /// <param name="alpha">The alpha component of the color.</param>
        /// <param name="lightness">The value for R,G and B.</param>
        public Argb(float alpha, float lightness)
        {
            color = new Vector4(lightness, lightness, lightness, alpha);
        }

        /// <summary>
        /// Creates a new Argb instance from the color components.
        /// Values between 0 and 1.
        /// </summary>
        /// <param name="alpha">Alpha.</param>
        /// <param name="red">Red.</param>
        /// <param name="green">Green.</param>
        /// <param name="blue">Blue.</param>
        public Argb(float alpha, float red, float green, float blue)
        {
            color = new Vector4(red, green, blue, alpha);
        }

        #endregion

        #region * Public Methods *

        /// <summary>
        /// Get the Argb values from an Int
        /// </summary>
        public void FromInt(int value)
        {
            color.W = (value >> 24 & 255) * FloatScale;
            color.X = (value >> 16 & 255) * FloatScale;
            color.Y = (value >> 8 & 255) * FloatScale;
            color.Z = (value & 255) * FloatScale;
        }

        /// <summary>
        /// Gets the value as an int.
        /// </summary>
        public int ToInt()
        {
            return Argb.ToInt(this.color);
        }

        /// <summary>
        /// Get the minimum channel value.
        /// </summary>
        public float GetMin()
        {
            return Math.Min(color.X, Math.Min(color.Y, color.Z));
        }

        /// <summary>
        /// Get the maximum channel value.
        /// </summary>
        public float GetMax()
        {
            return Math.Max(color.X, Math.Max(color.Y, color.Z));
        }

        /// <summary>
        /// Clamps the values into the appropriate range.
        /// </summary>
        public Argb Clamp()
        {
            this.color = this.color.Clamp();
            return this;
        }

        /// <summary>
        /// Does an operation to each color channel.
        /// The operation takes the value of the channel and 
        /// returns the new value.
        /// </summary>
        public void ForEachChannel(Func<float, float> func)
        {
            color.X = func(color.X);
            color.Y = func(color.Y);
            color.Z = func(color.Z);
        }

        #endregion

        #region * Light *

        /// <summary>
        /// Get the lightness of the color.
        /// </summary>
        public float Lightness()
        {
            return 0.5f * (GetMax() + GetMin());
        }

        /// <summary>
        /// Get the 'value' of the color.
        /// </summary>
        public float Value()
        {
            return GetMax();
        }

        /// <summary>
        /// Get the intensity of the color.
        /// </summary>
        public float Intensity()
        {
            return (color.X + color.Y + color.Z) * 0.3333333333333333f;
        }

        /// <summary>
        /// Get the gama corrected luminance of the color.
        /// </summary>
        public float Luma()
        {
            return (LumaRed * Red) + (LumaGreen * Green) + (LumaBlue * Blue);
        }

        /// <summary>
        /// Get the chroma of the color.
        /// </summary>
        public float Chroma()
        {
            return GetMax() - GetMin();
        }

        #endregion

        #region * Blend *

        /// <summary>
        /// Alpha blend. Colors must not be premultiplied.
        /// </summary>
        /// <param name="top">The color to blend.</param>
        /// <param name="alpha">The amount to blend.</param>
        public void BlendAlpha(Argb top, float alpha)
        {
            BlendAlpha(top.color, alpha);
        }

        /// <summary>
        /// Alpha blend. Colors must not be premultiplied.
        /// </summary>
        /// <param name="top">The color to blend.</param>
        /// <param name="alpha">The amount to blend.</param>
        public void BlendAlpha(Vector4 col, float alpha)
        {
            if (alpha < 1)
            {
                this.color = alpha * col + (1 - alpha) * color;
            }
            else
            {
                this.color = col;
            }
        }

        /// <summary>
        /// Takes the lighter of each channel.
        /// </summary>
        /// <param name="top">The color to blend.</param>
        /// <param name="alpha">The amount to blend.</param>
        public void BlendLighten(Argb top, float alpha)
        {
            var c = new Argb();
            c.color.W = 1;
            c.color.X = (color.X > top.color.X) ? color.X : top.color.X;
            c.color.Y = (color.Y > top.color.Y) ? color.Y : top.color.Y;
            c.color.Z = (color.Z > top.color.Z) ? color.Z : top.color.Z;
            BlendAlpha(c, alpha);
        }

        /// <summary>
        /// Takes the darker of each channel.
        /// </summary>
        /// <param name="top">The color to blend.</param>
        /// <param name="alpha">The amount to blend.</param>
        public void BlendDarken(Argb top, float alpha)
        {
            var c = new Argb();
            c.color.W = 1;
            c.color.X = (color.X > top.color.X) ? top.color.X : color.X;
            c.color.Y = (color.Y > top.color.Y) ? top.color.Y : color.Y;
            c.color.Z = (color.Z > top.color.Z) ? top.color.Z : color.Z;
            BlendAlpha(c, alpha);
        }

        /// <summary>
        /// Multiplies the color channels.
        /// </summary>
        /// <param name="top">The color to blend.</param>
        /// <param name="alpha">The amount to blend.</param>
        public void BlendMultiply(Argb top, float alpha)
        {
            Vector4 c = this * top;
            BlendAlpha(c, alpha);
        }

        /// <summary>
        /// Averages the color channels.
        /// </summary>
        /// <param name="top">The color to blend.</param>
        /// <param name="alpha">The amount to blend.</param>
        public void BlendAverage(Argb top, float alpha)
        {
            Vector4 c = (this + top) * 0.5f;
            BlendAlpha(c, alpha);
        }

        /// <summary>
        /// Adds the color channels.
        /// </summary>
        /// <param name="top">The color to blend.</param>
        /// <param name="alpha">The amount to blend.</param>
        public void BlendAdd(Argb top, float alpha)
        {
            Vector4 c = this + top;
            BlendAlpha(c.Clamp(), alpha);
        }

        /// <summary>
        /// Subtracts the color channels.
        /// </summary>
        /// <param name="top">The color to blend.</param>
        /// <param name="alpha">The amount to blend.</param>
        public void BlendSubtract(Argb top, float alpha)
        {
            Vector4 c = (this + top) - Vector4.One;
            BlendAlpha(c.Clamp(), alpha);
        }

        /// <summary>
        /// Gets the absolute difference between the color channels.
        /// </summary>
        /// <param name="top">The color to blend.</param>
        /// <param name="alpha">The amount to blend.</param>
        public void BlendDifference(Argb top, float alpha)
        {
            Vector4 c = ToAbsolute(this - top);
            BlendAlpha(c, alpha);
        }

        /// <summary>
        /// Gets the negative of the difference between the color channels.
        /// </summary>
        /// <param name="top">The color to blend.</param>
        /// <param name="alpha">The amount to blend.</param>
        public void BlendNegation(Argb top, float alpha)
        {
            Vector4 c = Vector4.One - ToAbsolute(Vector4.One - this.color - top);
            BlendAlpha(c, alpha);
        }

        /// <summary>
        /// Multiplies the negative of the color channels.
        /// </summary>
        /// <param name="top">The color to blend.</param>
        /// <param name="alpha">The amount to blend.</param>
        public void BlendScreen(Argb top, float alpha)
        {
            Vector4 c = Vector4.One - ((Vector4.One - this.color) * (Vector4.One - top.color));
            BlendAlpha(c, alpha);
        }

        public void BlendExclusion(Argb top, float alpha)
        {
            Vector4 c = this + top.color - 2f * this.color * top.color;
            BlendAlpha(c, alpha);
        }

        public void BlendOverlay(Argb top, float alpha)
        {
            Vector4 c = new Vector4();
            c.W = this.color.W;
            c.X = (top.color.X < 0.5f) ? (2f * this.color.X * top.color.X) : (1f - 2f * (1f - this.color.X) * (1f - top.color.X));
            c.Y = (top.color.Y < 0.5f) ? (2f * this.color.Y * top.color.Y) : (1f - 2f * (1f - this.color.Y) * (1f - top.color.Y));
            c.Z = (top.color.Z < 0.5f) ? (2f * this.color.Z * top.color.Z) : (1f - 2f * (1f - this.color.Z) * (1f - top.color.Z));
            BlendAlpha(c, alpha);
        }

        public void BlendSoftLight(Argb top, float alpha)
        {
            Vector4 c = new Vector4();
            c.W = this.color.W;
            c.X = (top.color.X < 0.5f) ? (2f * ((this.color.X * 0.5f) + 0.25f)) * (top.color.X / 1f) : (1f - (2f * (1f - ((this.color.X * 0.5f) * 0.25f)) * (1 - top.color.X) / 1f));
            c.Y = (top.color.Y < 0.5f) ? (2f * ((this.color.Y * 0.5f) + 0.25f)) * (top.color.Y / 1f) : (1f - (2f * (1f - ((this.color.Y * 0.5f) * 0.25f)) * (1 - top.color.Y) / 1f));
            c.Z = (top.color.Z < 0.5f) ? (2f * ((this.color.Z * 0.5f) + 0.25f)) * (top.color.Z / 1f) : (1f - (2f * (1f - ((this.color.Z * 0.5f) * 0.25f)) * (1 - top.color.Z) / 1f));
            BlendAlpha(c, alpha);
        }

        public void BlendHardLight(Argb top, float alpha)
        {
            Argb temp = new Argb(top.color);
            temp.BlendOverlay(this, alpha);
            this.color = temp.color;
        }

        public void BlendColorDodge(Argb top, float alpha)
        {
            Vector4 c = new Vector4();
            c.W = this.color.W;
            c.X = ColorDodge(this.color.X, top.color.X);
            c.Y = ColorDodge(this.color.Y, top.color.Y);
            c.Z = ColorDodge(this.color.Z, top.color.Z);
            BlendAlpha(c, alpha);
        }

        public void BlendColorBurn(Argb top, float alpha)
        {
            Vector4 c = new Vector4();
            c.W = this.color.W;
            c.X = ColorBurn(this.color.X, top.color.X);
            c.Y = ColorBurn(this.color.Y, top.color.Y);
            c.Z = ColorBurn(this.color.Z, top.color.Z);
            BlendAlpha(c, alpha);
        }

        public void BlendLinearDodge(Argb top, float alpha)
        {
            Argb temp = new Argb(top.color);
            temp.BlendAdd(this, alpha);
            this.color = temp.color;
        }

        public void BlendLinearBurn(Argb top, float alpha)
        {
            Argb temp = new Argb(top.color);
            temp.BlendSubtract(this, alpha);
            this.color = temp.color;
        }

        public void BlendLinearLight(Argb top, float alpha)
        {
            Vector4 c = new Vector4();
            c.W = this.color.W;
            c.X = (top.color.X < 0.5f) ? 
                ((top.color.X - this.color.X) - 1).Clamp() : // linear burn
                (top.color.X + this.color.X).Clamp();        // linear dodge
            c.Y = (top.color.Y < 0.5f) ?
                ((top.color.Y - this.color.Y) - 1).Clamp() : // linear burn
                (top.color.Y + this.color.Y).Clamp();        // linear dodge
            c.Z = (top.color.Z < 0.5f) ?
                ((top.color.Z - this.color.Z) - 1).Clamp() : // linear burn
                (top.color.Z + this.color.Z).Clamp();        // linear dodge

            BlendAlpha(c, alpha);
        }

        public void BlendVividLight(Argb top, float alpha)
        {
            Vector4 c = new Vector4();
            c.W = this.color.W;
            c.X = (top.color.X < 0.5f) ? ColorBurn(this.color.X, 2 * top.color.X) : ColorDodge(this.color.X, 2 * (top.color.X - 0.5f));
            c.Y = (top.color.Y < 0.5f) ? ColorBurn(this.color.Y, 2 * top.color.Y) : ColorDodge(this.color.Y, 2 * (top.color.Y - 0.5f));
            c.Z = (top.color.Z < 0.5f) ? ColorBurn(this.color.Z, 2 * top.color.Z) : ColorDodge(this.color.Z, 2 * (top.color.Z - 0.5f));
            BlendAlpha(c, alpha);
        }

        public void BlendPinLight(Argb top, float alpha)
        {
            Vector4 c = new Vector4();
            c.X = (top.color.X < 0.5f) ? Darken(this.color.X, 2 * top.color.X) : Lighten(this.color.X, 2 * (top.color.X - 0.5f));
            c.Y = (top.color.Y < 0.5f) ? Darken(this.color.Y, 2 * top.color.Y) : Lighten(this.color.Y, 2 * (top.color.Y - 0.5f));
            c.Z = (top.color.Z < 0.5f) ? Darken(this.color.Z, 2 * top.color.X) : Lighten(this.color.Z, 2 * (top.color.Z - 0.5f));
        }

        public void BlendHardMix(Argb top, float alpha)
        {
            // same as Vivid Light
            Vector4 c = new Vector4();
            c.W = this.color.W;
            c.X = (top.color.X < 0.5f) ? ColorBurn(this.color.X, 2 * top.color.X) : ColorDodge(this.color.X, 2 * (top.color.X - 0.5f));
            c.Y = (top.color.Y < 0.5f) ? ColorBurn(this.color.Y, 2 * top.color.Y) : ColorDodge(this.color.Y, 2 * (top.color.Y - 0.5f));
            c.Z = (top.color.Z < 0.5f) ? ColorBurn(this.color.Z, 2 * top.color.Z) : ColorDodge(this.color.Z, 2 * (top.color.Z - 0.5f));
            // this is new
            c.X = (c.X < 0.5f) ? 0 : 1;
            c.Y = (c.Y < 0.5f) ? 0 : 1;
            c.Z = (c.Z < 0.5f) ? 0 : 1;
            BlendAlpha(c, alpha);
        }

        public void BlendReflect(Argb top, float alpha)
        {
            Vector4 c = new Vector4();
            c.W = this.color.W;
            c.X = (top.color.X == 1) ? top.color.X : Math.Min(1, (this.color.X * this.color.X / (1 - top.color.X)));
            c.Y = (top.color.Y == 1) ? top.color.Y : Math.Min(1, (this.color.Y * this.color.Y / (1 - top.color.Y)));
            c.Z = (top.color.Z == 1) ? top.color.Z : Math.Min(1, (this.color.Z * this.color.Z / (1 - top.color.Z)));
            BlendAlpha(c, alpha);
        }

        public void BlendGlow(Argb top, float alpha)
        {
            Argb temp = new Argb(top.color);
            temp.BlendReflect(this, alpha);
            this.color = temp.color;
        }

        public void Lerp(Argb value, float amount)
        {
            float a = this.Alpha;
            this.color = xna.Vector4.Lerp(this.color, value.color, amount);
            this.Alpha = a;
        }

        #endregion

        #region * Operators *

        public static implicit operator Argb(Vector4 vector)
        {
            return new Argb(vector);
        }

        public static explicit operator Vector4(Argb argb)
        {
            return argb.color;
        }

        public static Vector4 operator *(Argb first, Argb second)
        {
            return new Vector4(
                    first.color.X * second.color.X,
                    first.color.Y * second.color.Y,
                    first.color.Z * second.color.X,
                    first.color.W);
        }

        public static Vector4 operator *(Argb first, float second)
        {
            return new Vector4(
                first.color.X * second,
                first.color.Y * second,
                first.color.Z * second,
                first.color.W);
        }

        public static Vector4 operator /(Argb first, Argb second)
        {
            return new Vector4(
                first.color.X / second.color.X,
                first.color.Y / second.color.Y,
                first.color.Z / second.color.Z,
                first.color.W);
        }

        public static Vector4 operator /(Argb first, float second)
        {
            return new Vector4(
                first.color.X / second,
                first.color.Y / second,
                first.color.Z / second,
                first.color.W);
        }

        public static Vector4 operator +(Argb first, Argb second)
        {
            return new Vector4(
                first.color.X + second.color.X,
                first.color.Y + second.color.Y,
                first.color.Z + second.color.Z,
                first.color.W);
        }

        public static Vector4 operator +(Argb first, float second)
        {
            return new Vector4(
                first.color.X + second,
                first.color.Y + second,
                first.color.Z + second,
                first.color.W);
        }

        public static Vector4 operator -(Argb first, Argb second)
        {
            return new Vector4(
                first.color.X - second.color.X,
                first.color.Y - second.color.Y,
                first.color.Z - second.color.Z,
                first.color.W);
        }

        public static Vector4 operator -(Argb first, float second)
        {
            return new Vector4(
                first.color.X - second,
                first.color.Y - second,
                first.color.Z - second,
                first.color.W);
        }

        public static bool operator ==(Argb first, Argb second)
        {
            return (first.color == second.color);
        }

        public static bool operator !=(Argb first, Argb second)
        {
            return (first.color != second.color);
        }

        public override string ToString()
        {
            return "A:" + Alpha.ToString() + " R:" + Red.ToString() + " G:" + Green.ToString() + " B:" + Blue.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(Argb)) return false;
            Argb temp = (Argb)obj;
            return this == temp;
        }

        public override int GetHashCode()
        {
            return this.ToInt();
        }

        #endregion

        #region * Private Static Methods *

        private static Vector4 ToAbsolute(Vector4 vector)
        {
            if (vector.W < 0) vector.W *= -1;
            if (vector.X < 0) vector.X *= -1;
            if (vector.Y < 0) vector.Y *= -1;
            if (vector.Z < 0) vector.Z *= -1;
            return vector;
        }

        private static float ColorBurn(float a, float b)
        {
            return (b == 0) ? 0 : Math.Max(0, 1 - (1 - a) / b);
        }

        private static float ColorDodge(float a, float b)
        {
            return (b == 1) ? 1 : Math.Min(1, (a / 1) / (1 - b));
        }

        private static float Lighten(float a, float b)
        {
            return (a > b) ? a : b;
        }

        private static float Darken(float a, float b)
        {
            return (a > b) ? b : a;
        }

        #endregion

        /// <summary>
        /// Turns a Vector4 color into an int.
        /// </summary>
        public static int ToInt(Vector4 color)
        {
            return ((int)(color.W * 255) << 24 | (int)(color.X * 255) << 16 | (int)(color.Y * 255) << 8 | (int)(color.Z * 255));
        }

        /// <summary>
        /// Converts an Argb to a ColorB
        /// </summary>
        public static ColorB ToColorB(Argb color)
        {
            ColorB temp = new ColorB();
            temp.Alpha = (color.Alpha * 255).ClampToByte();
            temp.Red = (color.Red * 255).ClampToByte();
            temp.Green = (color.Green * 255).ClampToByte();
            temp.Blue = (color.Blue * 255).ClampToByte();
            return temp;
        }

        /// <summary>
        /// Converts a ColorB to an Argb.
        /// </summary>
        public static Argb FromColorB(ColorB color)
        {
            return new Argb(color.Alpha, color.Red, color.Green, color.Blue);
        }

    }
}
