using System;

namespace ANDREICSLIB
{
    public static class MatrixOps
    {
        #region Delegates

        public delegate bool CompareDel<T>(T o1, T o2);

        #endregion

        private static void StartRight<T>(ref MatrixO<T> grid, int startx, int starty, T test, T set, bool testup)
        {
            if (starty < 0 || starty >= grid.h)
                return;

            for (int x = startx; x < grid.w; x++)
            {
                T v1 = grid.m[starty][x];
                T v2 = test;

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

        public static void FloodFill<T>(ref T[][] grid, int startX, int startY, T test, T set,
                                        CompareDel<T> compareF = null)
        {
            var g = new MatrixO<T> {m = grid, h = grid.Length, w = grid[0].Length, compareF = compareF};

            StartRight(ref g, startX, startY, test, set, true);
            StartRight(ref g, startX, startY + 1, test, set, false);
            grid = g.m;
        }

        public static T[][] CloneMatrix<T>(T[][] gridIN, int widthI, int heightI) where T : new()
        {
            T[][] outm = CreateMatrix<T>(widthI, heightI);

            for (int y = 0; y < heightI; y++)
            {
                for (int x = 0; x < widthI; x++)
                {
                    outm[y][x] = gridIN[y][x];
                }
            }
            return outm;
        }

        public static T[][] CreateMatrix<T>(int widthI, int heightI) where T : new()
        {
            var outm = new T[heightI][];

            for (int y = 0; y < heightI; y++)
            {
                outm[y] = new T[widthI];
                for (int x = 0; x < widthI; x++)
                {
                    outm[y][x] = new T();
                }
            }
            return outm;
        }

        public static String SerialiseMatrix<T>(T[][] matrix, int width, int height, String rowsep = ",",
                                                String linesep = "\r\n")
        {
            string ret = "";
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    ret += matrix[y][x] + rowsep;
                }
                if (y != (height - 1))
                    ret += linesep;
            }
            return ret;
        }

        public static void SetAll<T>(T[][] matrix, T val)
        {
            int h = matrix.Length;
            int w = matrix[0].Length;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    matrix[y][x] = val;
                }
            }
        }

        public static void Replace<T>(ref T[][] matrix, T test, T set, CompareDel<T> compare = null)
        {
            int h = matrix.Length;
            int w = matrix[0].Length;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if ((compare == null && matrix[y][x].Equals(test)) ||
                        (compare != null && compare(matrix[y][x], test)))
                        matrix[y][x] = set;
                }
            }
        }

        public static int Count<T>(T[][] matrix, T val)
        {
            int c = 0;
            int h = matrix.Length;
            int w = matrix[0].Length;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (matrix[y][x].Equals(val))
                        c++;
                }
            }
            return c;
        }

        #region Nested type: MatrixO

        public class MatrixO<T>
        {
            public CompareDel<T> compareF = null;
            public int h;
            public T[][] m;
            public int w;
        }

        #endregion
    }
}