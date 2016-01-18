using System;
using System.Collections.Generic;

namespace ANDREICSLIB.ClassExtras
{
    /// <summary>
    /// example usage: https://github.com/andreigec/Histogram-OCR-Trainer
    /// </summary>
    public static class SaveFileDialogExtras
    {
        private const char sep = '|';

        /// <summary>
        /// create the SFD filter string
        /// </summary>
        /// <param name="descAndExt">tuple with description, fileext eg: JPeg Image, *.jpg</param>
        /// <returns></returns>
        public static string createFilter(List<Tuple<string, string>> descAndExt)
        {
            string ret = "";
            foreach (var v in descAndExt)
            {
                ret += sep + createFilter(v.Item1, v.Item2);
            }
            return ret;
        }

        /// <summary>
        /// create the SFD filter string
        /// </summary>
        /// <param name="description">eg: JPeg Image</param>
        /// <param name="extension">eg  *.jpg</param>
        /// <returns></returns>
        public static string createFilter(string description, string extension)
        {
            return description + sep + extension;
        }
    }
}