using System.Linq;

namespace ANDREICSLIB.ClassExtras
{
    /// <summary>
    ///     example usage: https://github.com/andreigec/Crossword-Puzzle-Solver
    /// </summary>
    public class ArrayExtras
    {

        /// <summary>
        /// Adds the item to array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">The array.</param>
        /// <param name="item">The item.</param>
        public static void AddItemToArray<T>(ref T[] array, T item)
        {
            var arr = array.ToList();
            arr.Add(item);

            array = arr.ToArray();
        }


        /// <summary>
        /// Instantiates the array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns></returns>
        public static T[][] InstantiateArray<T>(int width, int height) where T : new()
        {
            var ret = new T[height][];
            for (var y = 0; y < height; y++)
            {
                ret[y] = new T[width];
            }
            return ret;
        }


        /// <summary>
        /// Rotates the array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">The array.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns></returns>
        public static T[][] RotateArray<T>(T[][] array, int width, int height) where T : new()
        {
            var newArray = InstantiateArray<T>(width, height);

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    newArray[x][y] = array[y][x];
                }
            }

            return newArray;
        }


        /// <summary>
        /// Clones the specified array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">The array.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns></returns>
        public static T[][] Clone<T>(T[][] array, int width, int height) where T : new()
        {
            var newArray = InstantiateArray<T>(width, height);

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    newArray[x][y] = array[x][y];
                }
            }

            return newArray;
        }
    }
}