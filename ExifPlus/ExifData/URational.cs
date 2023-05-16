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

namespace ExifPlus.ExifData
{
    /// <summary>
    /// Represents an unsigned rational number.
    /// </summary>
    [System.Xml.Serialization.XmlRoot("URational")]
    public struct URational
    {
        [System.Xml.Serialization.XmlElement("Numerator")]
        public uint Numerator;
        [System.Xml.Serialization.XmlElement("Denominator")]
        public uint Denominator;

        /// <summary>
        /// Create a new URational.
        /// </summary>
        public URational(uint numerator, uint denominator)
        {
            this.Numerator = numerator;
            this.Denominator = denominator;
        }

        /// <summary>
        /// Gets the result of dividing the values.
        /// </summary>
        public double GetQuotient()
        {
            if (Denominator == 0) return double.NaN;
            else return (double)Numerator / (double)Denominator;
        }

        /// <summary>
        /// Gets the value as a string in format "N/D"
        /// </summary>
        public override string ToString()
        {
            return Numerator.ToString() + "/" + Denominator.ToString();
        }

        /// <summary>
        /// Parses a Rational String
        /// </summary>
        public static URational Parse(string value)
        {
            URational r = new URational();
            var parts = value.Split('/');
            r.Numerator = uint.Parse(parts[0]);
            r.Denominator = uint.Parse(parts[1]);
            return r;
        }

        /// <summary>
        /// Tries to parse a rational from a string.
        /// </summary>
        public static bool TryParse(string value, out URational r)
        {
            try
            {
                r = Parse(value);
                return true;
            }
            catch
            {
                r = new URational(0, 1);
                return false;
            }
        }
    }
}
