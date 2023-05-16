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

    public class ExifInt : ExifDataBase<int>
    {
        public ExifInt() { }

        public ExifInt(byte[] data, int offset, bool littleEndian)
        {
            this.Read(data, offset, littleEndian);
        }

        protected override void Read(byte[] data, int offset, bool littleEndian)
        {
            try
            {
                this.Value = ExifIO.ReadInt(data, offset * 4, littleEndian);
            }
            catch
            {
                this.readError = true;
            }
        }

        protected override void ParseValue(string s)
        {
            int i;
            bool ret = int.TryParse(s, out i);
            if (ret) this.value = i;
        }
    }
    
}
