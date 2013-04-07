using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace ANDREICSLIB.ClassExtras
{
    public static class DirectoryExtras
    {
        public static string GetExePath()
        {
            return Path.GetDirectoryName(Application.ExecutablePath);
        }

        public static void SetCurrentDirectoryToDefault()
        {
            string p = GetExePath();
            Directory.SetCurrentDirectory(p);
        }

        /// <summary>
        /// get all the files under a folder
        /// </summary>
        /// <param name="absolutePath">must be the absolute path, not the relative path</param>
        /// <returns></returns>
        public static IEnumerable<string> GetFilesRecursive(string absolutePath)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(absolutePath);
            foreach (string fileName in fileEntries)
            {
                // do something with fileName
                yield return fileName;
            }

            // Recurse into subdirectories of this directory.
            string[] subdirEntries = Directory.GetDirectories(absolutePath);

            var ret = new List<string>();
            foreach (string subdir in subdirEntries)
            {
                // Do not iterate through reparse points
                if ((File.GetAttributes(subdir) &
                     FileAttributes.ReparsePoint) !=
                    FileAttributes.ReparsePoint)

                    ret.AddRange(GetFilesRecursive(subdir));
            }

            foreach (string r in ret)
                yield return r;
        }
    }
}