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

    public class ExifShort : ExifDataBase<short>, System.Xml.Serialization.IXmlSerializable
    {
        public ExifShort() { }

        public ExifShort(byte[] data, int offset, bool littleEndian)
        {
            this.Read(data, offset, littleEndian);
        }

        protected override void Read(byte[] data, int offset, bool littleEndian)
        {
            try
            {
                this.Value = ExifIO.ReadShort(data, offset * 2, littleEndian);
            }
            catch
            {
                this.readError = true;
            }
        }

        protected override void ParseValue(string s)
        {
            short i;
            bool ret = short.TryParse(s, out i);
            if (ret) this.value = i;
        }
    }
}
