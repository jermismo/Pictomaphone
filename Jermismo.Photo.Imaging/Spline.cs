using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Collections.ObjectModel;

namespace Jermismo.Photo.Imaging
{

    public class Spline
    {

        private List<Vector2> points = new List<Vector2>();
        private float[] y2;

        public int Count { get { return points.Count; } }

        public void Add(float x, float y)
        {
            points.Add(new Vector2(x, y));
            this.y2 = null;
        }

        public void Add(System.Windows.Point point)
        {
            points.Add(new Vector2((float)point.X, (float)point.Y));
            this.y2 = null;
        }

        public void Add(int x, int y)
        {
            points.Add(new Vector2(x, y));
            this.y2 = null;
        }

        public void AddRange(System.Windows.Point[] pointRange)
        {
            foreach (var point in pointRange)
            {
                this.Add(point);
            }
        }

        public void Clear()
        {
            this.points.Clear();
            this.y2 = null;
        }

        public float Interpolate(float x)
        {
            if (y2 == null) Initialize();
            int count = points.Count;
            int index1 = 0;
            int index2 = count - 1;

            while (index2 - index1 > 1)
            {
                int average = index2 + index1 >> 1;
                if (points[average].X > x)
                {
                    index2 = average;
                }
                else
                {
                    index1 = average;
                }
            }

            float num4 = points[index2].X - points[index1].X;
            float num5 = (points[index2].X - x) / num4;
            float num6 = (x - points[index1].X) / num4;

            // this is really long, I know
            return num5 * points[index1].Y + num6 * points[index2].Y + 
                   ((num5 * num5 * num5 - num5) * y2[index1] + 
                    (num6 * num6 * num6 - num6) * y2[index2]) * 
                   (num4 * num4) / 6;
        }

        private void Initialize()
        {
            // put points in X order
            points = points.OrderBy(p => p.X).ToList();

            int count = points.Count();
            float[] array = new float[count];
            this.y2 = new float[count];
            array[0] = 0f;
            y2[0] = 0f;

            for (int i = 1; i < count - 1; i++)
            {
                float x = points[i].X;
                float xm1 = points[i - 1].X;
                float xp1 = points[i + 1].X;

                float y = points[i].Y;
                float ym1 = points[i - 1].Y;
                float yp1 = points[i + 1].Y;

                float num = xp1 - xm1;
                float num2 = (x - xm1) / num;
                float num3 = num2 * y2[i - 1] + 2;
                y2[i] = (num2 - 1) / num3;
                float num4 = ((yp1 - y) / (xp1 - x)) - ((y - ym1) / (x - xm1));
                array[i] = (6 * num4 / num - num2 * array[i - 1]) / num3;
            }

            y2[count - 1] = 0;
            for (int j = count - 2; j >= 0; j--)
            {
                y2[j] = y2[j] * y2[j + 1] + array[j];
            }
        }

    }
}
