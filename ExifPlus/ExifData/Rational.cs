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
    /// Represents a signed rational number.
    /// </summary>
    [System.Xml.Serialization.XmlRoot("Rational")]
    public struct Rational
    {
        [System.Xml.Serialization.XmlElement("Numerator")]
        public int Numerator;
        [System.Xml.Serialization.XmlElement("Denominator")]
        public int Denominator;

        /// <summary>
        /// Create a new Rational.
        /// </summary>
        public Rational(int numerator, int denominator)
        {
            this.Numerator = numerator;
            this.Denominator = denominator;
        }

        /// <summary>
        /// Gets the result of dividing the values.
        /// </summary>
        /// <returns></returns>
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
        public static Rational Parse(string value)
        {
            Rational r = new Rational();
            var parts = value.Split('/');
            r.Numerator = int.Parse(parts[0]);
            r.Denominator = int.Parse(parts[1]);
            return r;
        }

        /// <summary>
        /// Tries to parse a rational from a string.
        /// </summary>
        public static bool TryParse(string value, out Rational r)
        {
            try
            {
                r = Parse(value);
                return true;
            }
            catch
            {
                r = new Rational(0, 1);
                return false;
            }
        }

    }
}
