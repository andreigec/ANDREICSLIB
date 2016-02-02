namespace ANDREICSLIB.Helpers
{
    /// <summary>
    ///     example usage: https://github.com/andreigec/Sudoku-Solver
    /// </summary>
    public static class MatrixOps
    {
        #region Delegates

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o1">The o1.</param>
        /// <param name="o2">The o2.</param>
        /// <returns></returns>
        public delegate bool CompareDel<T>(T o1, T o2);

        #endregion

        /// <summary>
        /// Starts the right.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="grid">The grid.</param>
        /// <param name="startx">The startx.</param>
        /// <param name="starty">The starty.</param>
        /// <param name="test">The test.</param>
        /// <param name="set">The set.</param>
        /// <param name="testup">if set to <c>true</c> [testup].</param>
        private static void StartRight<T>(ref MatrixO<T> grid, int startx, int starty, T test, T set, bool testup)
        {
            if (starty < 0 || starty >= grid.h)
                return;

            for (var x = startx; x < grid.w; x++)
            {
                var v1 = grid.m[starty][x];
                var v2 = test;

                if ((grid.compareF == null && v1.Equals(v2)) || (grid.compareF != null && grid.compareF(v1, v2)))
                {
                    grid.m[starty][x] = set;

                    //test direction
                    if (testup && starty > 0 && grid.m[starty - 1][x].Equals(test))
                        StartRight(ref grid, startx, starty - 1, test, set, testup);
                    else if (testup == false && starty < (grid.h - 1) && grid.m[starty + 1][x].Equals(test))
                        StartRight(ref grid, startx, starty + 1, test, set, testup);
                }
                else
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Floods the fill.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="grid">The grid.</param>
        /// <param name="startX">The start x.</param>
        /// <param name="startY">The start y.</param>
        /// <param name="test">The test.</param>
        /// <param name="set">The set.</param>
        /// <param name="compareF">The compare f.</param>
        public static void FloodFill<T>(ref T[][] grid, int startX, int startY, T test, T set,
            CompareDel<T> compareF = null)
        {
            var g = new MatrixO<T> {m = grid, h = grid.Length, w = grid[0].Length, compareF = compareF};

            StartRight(ref g, startX, startY, test, set, true);
            StartRight(ref g, startX, startY + 1, test, set, false);
            grid = g.m;
        }

        /// <summary>
        /// Clones the matrix.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gridIN">The grid in.</param>
        /// <param name="widthI">The width i.</param>
        /// <param name="heightI">The height i.</param>
        /// <returns></returns>
        public static T[][] CloneMatrix<T>(T[][] gridIN, int widthI, int heightI) where T : new()
        {
            var outm = CreateMatrix<T>(widthI, heightI);

            for (var y = 0; y < heightI; y++)
            {
                for (var x = 0; x < widthI; x++)
                {
                    outm[y][x] = gridIN[y][x];
                }
            }
            return outm;
        }

        /// <summary>
        /// Creates the matrix.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="widthI">The width i.</param>
        /// <param name="heightI">The height i.</param>
        /// <returns></returns>
        public static T[][] CreateMatrix<T>(int widthI, int heightI) where T : new()
        {
            var outm = new T[heightI][];

            for (var y = 0; y < heightI; y++)
            {
                outm[y] = new T[widthI];
                for (var x = 0; x < widthI; x++)
                {
                    outm[y][x] = new T();
                }
            }
            return outm;
        }

        /// <summary>
        /// Serialises the matrix.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix">The matrix.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="rowsep">The rowsep.</param>
        /// <param name="linesep">The linesep.</param>
        /// <returns></returns>
        public static string SerialiseMatrix<T>(T[][] matrix, int width, int height, string rowsep = ",",
            string linesep = "\r\n")
        {
            var ret = "";
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    ret += matrix[y][x] + rowsep;
                }
                if (y != (height - 1))
                    ret += linesep;
            }
            return ret;
        }

        /// <summary>
        /// Sets all.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix">The matrix.</param>
        /// <param name="val">The value.</param>
        public static void SetAll<T>(T[][] matrix, T val)
        {
            var h = matrix.Length;
            var w = matrix[0].Length;

            for (var y = 0; y < h; y++)
            {
                for (var x = 0; x < w; x++)
                {
                    matrix[y][x] = val;
                }
            }
        }

        /// <summary>
        /// Replaces the specified matrix.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix">The matrix.</param>
        /// <param name="test">The test.</param>
        /// <param name="set">The set.</param>
        /// <param name="compare">The compare.</param>
        public static void Replace<T>(ref T[][] matrix, T test, T set, CompareDel<T> compare = null)
        {
            var h = matrix.Length;
            var w = matrix[0].Length;

            for (var y = 0; y < h; y++)
            {
                for (var x = 0; x < w; x++)
                {
                    if ((compare == null && matrix[y][x].Equals(test)) ||
                        (compare != null && compare(matrix[y][x], test)))
                        matrix[y][x] = set;
                }
            }
        }

        /// <summary>
        /// Counts the specified matrix.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix">The matrix.</param>
        /// <param name="val">The value.</param>
        /// <returns></returns>
        public static int Count<T>(T[][] matrix, T val)
        {
            var c = 0;
            var h = matrix.Length;
            var w = matrix[0].Length;

            for (var y = 0; y < h; y++)
            {
                for (var x = 0; x < w; x++)
                {
                    if (matrix[y][x].Equals(val))
                        c++;
                }
            }
            return c;
        }

        #region Nested type: MatrixO

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class MatrixO<T>
        {
            /// <summary>
            /// The compare f
            /// </summary>
            public CompareDel<T> compareF;
            public int h;
            public T[][] m;
            public int w;
        }

        #endregion
    }
}