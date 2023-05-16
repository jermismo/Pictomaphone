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

    public class ExifDateTime : ExifDataBase<DateTime>
    {

        public ExifDateTime() { }

        public ExifDateTime(byte[] data, int offset, bool littleEndian)
        {
            Read(data, offset, littleEndian);
        }

        protected override void Read(byte[] data, int offset, bool littleEndian)
        {
            try
            {
                string dt = System.Text.Encoding.UTF8.GetString(data, offset, data.Length);
                dt = dt.Trim(ExifString.badChars);

                ParseExifString(dt);
            }
            catch
            {
                this.readError = true;
            }
        }

        private void ParseExifString(string s)
        {
            if (!string.IsNullOrWhiteSpace(s) && s.Length == 19)
            {
                int year, month, day, hour, minute, second;

                int.TryParse(s.Substring(0, 4), out year);
                int.TryParse(s.Substring(5, 2), out month);
                int.TryParse(s.Substring(8, 2), out day);

                int.TryParse(s.Substring(11, 2), out hour);
                int.TryParse(s.Substring(14, 2), out minute);
                int.TryParse(s.Substring(17, 2), out second);

                this.Value = new DateTime(year, month, day, hour, minute, second);
            }
            else
            {
                this.Value = DateTime.MinValue;
                this.hasValue = false;
            }
        }

        protected override void ParseValue(string s)
        {
            DateTime d;
            bool res = DateTime.TryParse(s, out d);
            if (res) this.value = d;
        }

        public static ExifDateTime FromExifString(ExifString val)
        {
            ExifDateTime dt = new ExifDateTime();
            try
            {
                dt.ParseExifString(val.Value);
            }
            catch
            {
                dt.readError = true;
            }
            return dt;
        }

    }
}
