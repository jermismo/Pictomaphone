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

    public class ExifULong : ExifDataBase<uint>, System.Xml.Serialization.IXmlSerializable
    {

        public ExifULong() { }

        public ExifULong(byte[] data, int offset, bool littleEndian)
        {
            Read(data, offset, littleEndian);
        }

        protected override void Read(byte[] data, int offset, bool littleEndian)
        {
            try
            {
                this.Value = ExifIO.ReadUInt(data, offset * 4, littleEndian);
            }
            catch
            {
                this.readError = true;
            }
        }

        protected override void ParseValue(string s)
        {
            uint i;
            bool ret = uint.TryParse(s, out i);
            if (ret) this.value = i;
        }
    }
}
