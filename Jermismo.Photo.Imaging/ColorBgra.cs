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

namespace Jermismo.Photo.Imaging
{
    /// <summary>
    /// Holds the raw bits from an int color.
    /// </summary>
    public class ColorBgra
    {
        public byte[] Bgra;
        public byte Alpha { get { return Bgra[3]; } }
        public byte Red { get { return Bgra[2]; } }
        public byte Green { get { return Bgra[1]; } }
        public byte Blue { get { return Bgra[0]; } }

        public ColorBgra()
        {
            Bgra = new byte[4] { 0, 0, 0, 0 };
        }
        public ColorBgra(int value)
        {
            Bgra = BitConverter.GetBytes(value);
        }
        public int ToInt()
        {
            return BitConverter.ToInt32(Bgra, 0);
        }
    }
}
