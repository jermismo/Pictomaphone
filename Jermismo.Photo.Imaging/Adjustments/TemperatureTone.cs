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

namespace Jermismo.Photo.Imaging.Adjustments
{
    public class TemperatureTone : EffectBase
    {

        public const string Name = "TemperatureTone";

        private double temp = 0;
        private double tone = 0;

        public TemperatureTone()
        {
            Parameters = new System.Collections.ObjectModel.ReadOnlyCollection<EffectParamBase>(new EffectParamBase[] {
                new RangeParam("Temperature", -100, 100, 0),
                new RangeParam("Tone", -100, 100, 0)
            });
            this.colorspace = EffectColorSpace.LAB;
        }

        internal override void GetParameters()
        {
            temp = ((RangeParam)Parameters[0]).Value;
            tone = ((RangeParam)Parameters[1]).Value;
        }

        internal override IPixel PixelOperation(IPixel pixel)
        {
            CieLab lab = (CieLab)pixel;
            lab.B += temp;
            lab.A += tone;
            return lab;
        }

    }
}
