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

    public class ExifByte : ExifDataBase<byte>
    {

        public ExifByte() { }

        public ExifByte(byte[] data, int offset, bool littleEndian)
        {
            this.Read(data, offset, littleEndian);
        }

        protected override void Read(byte[] data, int offset, bool littleEndian)
        {
            try
            {
                this.Value = data[offset];
            }
            catch
            {
                this.readError = true;
            }
        }

        protected override void ParseValue(string s)
        {
            byte b;
            bool res = byte.TryParse(s, out b);
            if (res) this.value = b;
        }
    }
}
