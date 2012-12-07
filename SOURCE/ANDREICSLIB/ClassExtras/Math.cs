using System;
using mm = System.Windows.Media.Matrix;
using dm = System.Drawing.Drawing2D.Matrix;

namespace ANDREICSLIB
{
    public static class MathUpdates
    {
        public static int Floor(int v)
        {
            var ret = (int) (Math.Floor((double) v));
            return ret;
        }

        public static int Ceiling(int v)
        {
            var ret = (int) (Math.Ceiling((double) v));
            return ret;
        }

        public static mm ConvertMatrix(dm DrawingMatrix)
        {
            float[] vars = DrawingMatrix.Elements;
            var ret = new mm(vars[0], vars[1], vars[2], vars[3], vars[4], vars[5]);
            return ret;
        }

        public static dm ConvertMatrix(mm MediaMatrix)
        {
            var ret = new dm((float) MediaMatrix.M11, (float) MediaMatrix.M12, (float) MediaMatrix.M21,
                             (float) MediaMatrix.M22, (float) MediaMatrix.OffsetX,
                             (float) MediaMatrix.OffsetY);
            return ret;
        }

        public static double Truncate(double inval, int length = 2)
        {
            return double.Parse(inval.ToString("F" + length.ToString()));
        }
    }
}