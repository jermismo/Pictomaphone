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

using Microsoft.Xna.Framework;

namespace Jermismo.Photo.Imaging
{

    public struct CieLab : IPixel
    {
        public double L ,A ,B;

        // STATIC DATA
        public static double[][] cieR = new double[256][];
        public static double[][] cieG = new double[256][];
        public static double[][] cieB = new double[256][];

        static CieLab()
        {
            
            for (int i = 0; i < 256; i++)
            {
                // get sRGB value
                double v = i * Argb.FloatScale;
                if (v > 0.04045) v = Math.Pow((v + 0.055) * d1055, 2.4);
                else v = v * d1292;
                // get CIE values
                cieR[i] = new double[3];
                cieR[i][0] = v * 0.4124;
                cieR[i][1] = v * 0.2126;
                cieR[i][2] = v * 0.0193;

                cieG[i] = new double[3];
                cieG[i][0] = v * 0.3576;
                cieG[i][1] = v * 0.7152;
                cieG[i][2] = v * 0.1192;

                cieB[i] = new double[3];
                cieB[i][0] = v * 0.1805;
                cieB[i][1] = v * 0.0722;
                cieB[i][2] = v * 0.9505;
            }
        }

        /// <summary>
        /// Create a LAB struct from a color int.
        /// </summary>
        public CieLab(int val)
        {
            this = FromColorB((ColorB)val);    
        }
        
        /// <summary>
        /// Create a LAB struct for L, A, and B values.
        /// </summary>
        public CieLab(float l, float a, float b)
        {
            L = l; A = a; B = b;
        }

        public static CieLab FromColorB(ColorB col)
        {
            // get CIE XYZ values
            double[] r = cieR[col.Red];
            double[] g = cieG[col.Green];
            double[] b = cieB[col.Blue];

            double x = r[0] + g[0] + b[0];
            double y = r[1] + g[1] + b[1];
            double z = r[2] + g[2] + b[2];  

            // clamp at wb_65 levels
            //if (x > wb65_x) x = wb65_x;
            //if (y > wb65_y) y = wb65_y;
            //if (z > wb65_z) z = wb65_z;

            // convert to CIE LAB
            CieLab lab = new CieLab();
            x = x * iwb65_x;
            y = y * iwb65_y;
            z = z * iwb65_z;

            if (x > 0.008856) x = Math.Pow(x, d1_3);
            else x = 7.787 * x + d16_116;

            if (y > 0.008856) y = Math.Pow(y, d1_3);
            else y = 7.787 * y + d16_116;

            if (z > 0.008856) z = Math.Pow(z, d1_3);
            else z = 7.787 * z + d16_116;

            lab.L = 116.0 * y - 16;
            lab.A = 500.0 * (x - y);
            lab.B = 200.0 * (y - z);
            return lab;
        }

        public ColorB ToColorB()
        {
            // to cie xyz
            double y = (L + 16.0) * d116;
            double x = y + (A * d500);
            double z = y - (B * d200);

            if (x > d6_29) x = wb65_x * (x * x * x);
            else x = (x - d16_116) * 3 * d6_29x2 * wb65_x;

            if (y > d6_29) y = wb65_y * (y * y * y);
            else y = (y - d16_116) * 3 * d6_29x2 * wb65_y;

            if (z > d6_29) z = wb65_z * (z * z * z);
            else z = (z - d16_116) * 3 * d6_29x2 * wb65_z;

            // to colorb
            double r = x * 3.2406 + y * -1.5372 + z * -0.4986;
            double g = x * -0.9689 + y * 1.8758 + z * 0.0415;
            double b = x * 0.0557 + y * -0.2040 + z * 1.0570;

            if (r <= 0.0031308) r *= 12.92;
            else r = 1.055 * Math.Pow(r, d1_24) - 0.055;

            if (g <= 0.0031308f) g *= 12.92;
            else g = 1.055 * Math.Pow(g, d1_24) - 0.055;

            if (b <= 0.0031308) b *= 12.92;
            else b = 1.055 * Math.Pow(b, d1_24) - 0.055;
            
            return new ColorB(
                (r * 255).ClampToByte(),
                (g * 255).ClampToByte(),
                (b * 255).ClampToByte()
            );

        }

        public int GetInt()
        {
            return (int)this.ToColorB();
        }
        
        // white balance constants
        const double wb65_x = 0.9505;
        const double wb65_y = 1.0;
        const double wb65_z = 1.089;

        const double iwb65_x = 1 / wb65_x;
        const double iwb65_y = 1 / wb65_y;
        const double iwb65_z = 1 / wb65_z;

        // other
        const double d1292 = 1f / 12.92f;
        const double d1055 = 1f / 1.055f;
        const double d116 = 1f / 116f;
        const double d500 = 1f / 500f;
        const double d200 = 1f / 200f;

        const double d1_3 = 1.0f / 3.0f;
        const double d16_116 = 16.0f / 116.0f;
        const double d6_29 = 6f / 29f;
        const double d6_29x2 = d6_29 * d6_29;
        const double d1_24 = 1f / 2.4f;

    }
}
