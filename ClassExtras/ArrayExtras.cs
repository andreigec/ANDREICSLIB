using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ANDREICSLIB.ClassExtras
{
    /// <summary>
    /// example usage: https://github.com/andreigec/Crossword-Puzzle-Solver
    /// </summary>
    public class ArrayExtras
    {
        public static void AddItemToArray<T>(ref T[] inarr, T item)
        {
            var arr = inarr.ToList();
            arr.Add(item);
            
            inarr = arr.ToArray();
        }
        public static T[][] InstantiateArray<T>(int width, int height) where T : new()
        {
            var ret = new T[height][];
            for (int y = 0; y < height; y++)
            {
                ret[y] = new T[width];
            }
            return ret;
        }

        public static T[][] RotateArray<T>(T[][] inarr, int width, int height) where T : new()
        {
            var newArray = InstantiateArray<T>(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    newArray[x][y] = inarr[y][x];
                }
            }

            return newArray;
        }

        public static T[][] Clone<T>(T[][] inarr, int width, int height) where T : new()
        {
            var newArray = InstantiateArray<T>(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    newArray[x][y] = inarr[x][y];
                }
            }

            return newArray;
        }
    }
}
