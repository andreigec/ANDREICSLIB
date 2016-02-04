using System;
using mm = System.Windows.Media.Matrix;
using dm = System.Drawing.Drawing2D.Matrix;

namespace ANDREICSLIB.ClassExtras
{
    /// <summary>
    ///     example usage: https://github.com/andreigec/Meal-Chooser
    /// </summary>
    public static class MathExtras
    {
        /// <summary>
        /// Floors the specified v.
        /// </summary>
        /// <param name="v">The v.</param>
        /// <returns></returns>
        public static int Floor(int v)
        {
            var ret = (int) (Math.Floor((double) v));
            return ret;
        }

        /// <summary>
        /// Ceilings the specified v.
        /// </summary>
        /// <param name="v">The v.</param>
        /// <returns></returns>
        public static int Ceiling(int v)
        {
            var ret = (int) (Math.Ceiling((double) v));
            return ret;
        }

        /// <summary>
        /// Converts the matrix.
        /// </summary>
        /// <param name="DrawingMatrix">The drawing matrix.</param>
        /// <returns></returns>
        public static mm ConvertMatrix(dm DrawingMatrix)
        {
            var vars = DrawingMatrix.Elements;
            var ret = new mm(vars[0], vars[1], vars[2], vars[3], vars[4], vars[5]);
            return ret;
        }

        /// <summary>
        /// Converts the matrix.
        /// </summary>
        /// <param name="MediaMatrix">The media matrix.</param>
        /// <returns></returns>
        public static dm ConvertMatrix(mm MediaMatrix)
        {
            var ret = new dm((float) MediaMatrix.M11, (float) MediaMatrix.M12, (float) MediaMatrix.M21,
                (float) MediaMatrix.M22, (float) MediaMatrix.OffsetX,
                (float) MediaMatrix.OffsetY);
            return ret;
        }

        /// <summary>
        /// Truncates the specified inval.
        /// </summary>
        /// <param name="inval">The inval.</param>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        public static double Truncate(double inval, int length = 2)
        {
            return double.Parse(inval.ToString("F" + length));
        }
    }
}