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
    public class ExifString : ExifDataBase<string>, System.Xml.Serialization.IXmlSerializable
    {

        public ExifString() { }

        internal static char[] badChars = new char[] { '\t', '\r', '\n', '\0' };

        public ExifString(string value)
        {
            this.Value = value;
        }

        public ExifString(byte[] data, int offset, bool littleEndian)
        {
            Read(data, offset, littleEndian);
        }

        protected override void Read(byte[] data, int offset, bool littleEndian)
        {
            try
            {
                string val = System.Text.Encoding.UTF8.GetString(data, offset, data.Length);
                this.Value = val.Trim(ExifString.badChars);
            }
            catch
            {
                this.readError = true;
            }
        }

        protected override void ParseValue(string s)
        {
            this.value = s;
        }

    }
}
