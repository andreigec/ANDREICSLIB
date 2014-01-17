using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ANDREICSLIB.ClassExtras
{
    public class ArrayExtras
    {
        /// <summary>
        /// create a 2d array with a width and height
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static T[][] InstantiateArray<T>(int width, int height) where T : new()
        {
            var ret = new T[height][];
            for (int y = 0; y < height; y++)
            {
                ret[y] = new T[width];
            }
            return ret;
        }

        /// <summary>
        /// rotate a 2d array 90 degrees right
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inarr"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static T[][] RotateArray<T>(T[][] inarr, int width,int height) where T:new()
        {
            var newArray = InstantiateArray<T>(width, height);
            
            for (int y=0;y<height;y++)
            {
                for (int x=0;x<width;x++)
                {
                    newArray[x][y] = inarr[y][x];
                }
            }

            return newArray;
        }

        /// <summary>
        /// clone a 2d array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inarr"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static T[][] Clone<T>(T[][] inarr, int width,int height) where T:new()
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
