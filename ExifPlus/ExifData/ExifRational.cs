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

    public class ExifRational : ExifDataBase<Rational>, System.Xml.Serialization.IXmlSerializable
    {
        public ExifRational() { }

        public ExifRational(byte[] data, int offset, bool littleEndian)
        {
            this.Read(data, offset, littleEndian);
        }

        protected override void Read(byte[] data, int offset, bool littleEndian)
        {
            try
            {
                int num = ExifIO.ReadInt(data, offset * 8, littleEndian);
                int den = ExifIO.ReadInt(data, offset * 8 + 4, littleEndian);
                this.Value = new Rational(num, den);
            }
            catch
            {
                this.readError = true;
            }
        }

        protected override void ParseValue(string s)
        {
            Rational r;
            bool ret = Rational.TryParse(s, out r);
            if (ret) this.value = r;
        }
    }
}
