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

    public class ExifUShort : ExifDataBase<ushort>, System.Xml.Serialization.IXmlSerializable
    {

        public ExifUShort() { }

        public ExifUShort(byte[] data, int offset, bool littleEndian)
        {
            this.Read(data, offset, littleEndian);
        }

        protected override void Read(byte[] data, int offset, bool littleEndian)
        {
            try
            {
                this.Value = ExifIO.ReadUShort(data, offset * 2, littleEndian);
            }
            catch
            {
                this.readError = true;
            }
        }

        protected override void ParseValue(string s)
        {
            ushort i;
            bool ret = ushort.TryParse(s, out i);
            if (ret) this.value = i;
        }

    }

}
