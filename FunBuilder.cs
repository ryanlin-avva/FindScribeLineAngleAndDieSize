using System.Drawing;

namespace CsGetTgs
{
    public class FunBuilder
    {
        bool isHor;
        float m = 0;
        float b = 0;

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
            if (!isHor && Point2.X == Point1.X)
            {
                m = 1; b = -Point1.X;
            } 
            else
            {
                m = ((float)(Point2.Y - Point1.Y)) / ((float)(Point2.X - Point1.X));
                b = Point1.Y - m * Point1.X;
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
            if (float.IsInfinity(m))
            {
                return -b/m;
            }
            return (y - b) / m;
        }
        public override string ToString()
        {
            return "Function = " + m.ToString() + "*X + " + b.ToString();
        }
    }
}
