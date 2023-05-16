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
    /// Doesn't do anything, just marks something as a usuable type of pixel color.
    /// </summary>
    public interface IPixel
    {
        int GetInt();
    }
}
