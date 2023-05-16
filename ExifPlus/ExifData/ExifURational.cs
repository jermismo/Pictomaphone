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

    public class ExifURational : ExifDataBase<URational>, System.Xml.Serialization.IXmlSerializable
    {
        public ExifURational() { }

        public ExifURational(byte[] data, int offset, bool littleEndian)
        {
            this.Read(data, offset, littleEndian);
        }

        protected override void Read(byte[] data, int offset, bool littleEndian)
        {
            try
            {
                uint num = ExifIO.ReadUInt(data, offset * 8, littleEndian);
                uint den = ExifIO.ReadUInt(data, offset * 8 + 4, littleEndian);
                this.Value = new URational(num, den);
            }
            catch
            {
                this.readError = true;
            }
        }

        protected override void ParseValue(string s)
        {
            URational r;
            bool ret = URational.TryParse(s, out r);
            if (ret) this.value = r;
        }
    }
}
