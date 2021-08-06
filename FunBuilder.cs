using System;
using System.Drawing;
using System.Windows.Forms;

namespace CsGetTgs
{
    public class FunBuilder
    {
        bool isHor;
        float m = 0;
        float b = 0;
        public double ThetaDegree { get; set; }
        public FunBuilder(bool hor)
        {
            isHor = hor;
        }
        public bool BuildLine(Point Point1, Point Point2)
        {
            if (Point1 == Point2) return false;
            CompLineCoeff(Point1, Point2);
            return true;
        }
        private void CompLineCoeff(Point Point1, Point Point2)
        {
            /*
               y=mx+b             
            */
            if (Point2.X == Point1.X)
            {
                if (!isHor) //Vertical line
                {
                    m = 0; b = Point1.X;
                    ThetaDegree = 90;
                }
                else
                {
                    MessageBox.Show("[CompLineCoeff]: Invalid Points for Horizontal Line Building "
                                           + Point1.ToString() + Point2.ToString());
                }
            }
            else
            {
                if (Point2.Y == Point1.Y)
                {
                    if (isHor) //Horizontal line
                    {
                        m = 0; b = Point1.Y;
                        ThetaDegree = 0;
                    }
                    else
                    {
                        MessageBox.Show("[CompLineCoeff]: Invalid Points for Vertical Line Building "
                                               + Point1.ToString() + Point2.ToString());
                    }
                }
                else
                {
                    m = ((float)(Point2.Y - Point1.Y)) / ((float)(Point2.X - Point1.X));
                    b = Point1.Y - m * Point1.X;
                    //使用-m來計算，因為影像中的Y軸是往下 (越下面，值越大)
                    ThetaDegree = Math.Atan(-m) * 180.0 / Math.PI;
                    if (!isHor)
                        ThetaDegree = (ThetaDegree > 0) ? ThetaDegree - 90 : ThetaDegree + 90;
                }
            }
        }
        public float GetAssociateValue(float f)
        {
            if (isHor) return GetY(f);
            else return GetX(f);
        }
        private float GetY(float x)
        {
            if (float.IsInfinity(m))
            {
                return b;
            }
            return m * x + b;
        }
        private float GetX(float y)
        {
            /*
               y=mx+b             
            */
            if (m == 0)
            {
                return b;
            }
            return (y - b) / m;
        }
        public override string ToString()
        {
            return "Function = " + m.ToString() + "*X + " + b.ToString();
        }
    }
}