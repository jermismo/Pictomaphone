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
using System.Runtime.InteropServices;

namespace Jermismo.Photo.Imaging
{
    public static class MathHelper
    {

        /// <summary>
        /// Gets the distance between 2 points.
        /// </summary>
        public static float Distance(int x1, int y1, int x2, int y2)
        {
            int dx = x1 - x2;
            int dy = y1 - y2;
            return (float)Math.Sqrt((dx * dx) + (dy * dy));
        }
    }
}
