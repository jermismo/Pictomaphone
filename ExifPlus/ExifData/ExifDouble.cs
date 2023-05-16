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

    public class ExifDouble : ExifDataBase<double>
    {
        public ExifDouble() { }

        public ExifDouble(byte[] data, int offset, bool littleEndian)
        {
            this.Read(data, offset, littleEndian);
        }

        protected override void Read(byte[] data, int offset, bool littleEndian)
        {
            try
            {
                this.Value = ExifIO.ReadDouble(data, offset * 8, littleEndian);
            }
            catch
            {
                this.readError = true;
            }
        }

        protected override void ParseValue(string s)
        {
            double d;
            bool ret = double.TryParse(s, out d);
            if (ret) this.value = d;
        }
    }
}
