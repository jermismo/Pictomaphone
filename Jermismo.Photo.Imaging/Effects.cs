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
using System.Collections;
using System.Collections.Generic;

using Jermismo.Photo.Imaging.Adjustments;
using Jermismo.Photo.Imaging.Borders;
using Jermismo.Photo.Imaging.Effects;
using Jermismo.Photo.Imaging.Filters;

namespace Jermismo.Photo.Imaging
{
    public static class EffectList
    {

        public static Dictionary<string, EffectBase> Items = new Dictionary<string, EffectBase>() {
            // edits

            // adjustments
            { BrightnessContrast.Name, new BrightnessContrast() },
            { Levels.Name, new Levels() },
            { ChannelOverlay.Name, new ChannelOverlay() },
            { ChannelMixer.Name, new ChannelMixer() },
            { ColorBalance.Name, new ColorBalance() },
            { TemperatureTone.Name, new TemperatureTone() },
            { Vibrance.Name, new Vibrance() },
            
            { HueSaturation.Name, new HueSaturation() },
            { Colorize.Name, new Colorize() },
            { ColorFilter.Name, new ColorFilter() },
            { Grayscale.Name, new Grayscale() },
            { Negative.Name, new Negative() },
            { Solarize.Name, new Solarize() },
            { Posterize.Name, new Posterize() },

            { Sepia.Name, new Sepia() }, // filters
            
            // effects
            //{ GaussianBlur.Name, new GaussianBlur() },
            //{ BoxBlur.Name, new BoxBlur() },
            { StackBlur.Name, new StackBlur() },
            { DreamyGlow.Name, new DreamyGlow() },
            { Vignette.Name, new Vignette() },
            { FrostedGlass.Name, new FrostedGlass() },
            //{ RedEyeFix.Name, new RedEyeFix() },
            { TiltShift.Name, new TiltShift() },
           
            // borders
            { SquareBorder.Name, new SquareBorder() },

            // filters
            { CrossProcess.Name, new CrossProcess() },
            { Country.Name, new Country() },
            { Brawny.Name, new Brawny() },
            { SundayMorning.Name, new SundayMorning() },
            { OldNewYork.Name, new OldNewYork() },
            { SummerForest.Name, new SummerForest() },
            { Lomo.Name, new Lomo() },
            { Velvian.Name, new Velvian() }

        };

    }
}
