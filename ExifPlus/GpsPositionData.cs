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
    /// The Logitude Position information.
    /// </summary>
    public struct GpsLogitudePosition
    {
        public double Hours;
        public double Minutes;
        public double Seconds;
        public Exif.GpsLongitudeRef Reference;

        public override string ToString()
        {
            string ret = Hours.ToString("00") + ":" + Minutes.ToString("00") + ":" + Seconds.ToString();
            if (Reference == Exif.GpsLongitudeRef.West)
                ret += "W";
            else if (Reference == Exif.GpsLongitudeRef.East)
                ret += "E";
            return ret;
        }
    }

    /// <summary>
    /// The Latitude Position information
    /// </summary>
    public struct GpsLatitudePosition
    {
        public double Hours;
        public double Minutes;
        public double Seconds;
        public Exif.GpsLatitudeRef Reference;

        public override string ToString()
        {
            string ret = Hours.ToString("00") + ":" + Minutes.ToString("00") + ":" + Seconds.ToString();
            if (Reference == Exif.GpsLatitudeRef.North)
                ret += "N";
            else if (Reference == Exif.GpsLatitudeRef.South)
                ret += "S";
            return ret;
        }
    }
}
