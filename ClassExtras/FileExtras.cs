using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace ANDREICSLIB.ClassExtras
{
    /// <summary>
    ///     example usage: https://github.com/andreigec/Dwarf-Fortress-Mod-Merger
    /// </summary>
    public static class FileExtras
    {
        /// <summary>
        /// load a file from a path into a string
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        public static string LoadFile(string filename)
        {
            var fs = new FileStream(filename, FileMode.Open);
            var sr = new StreamReader(fs);
            var filestr = sr.ReadToEnd();
            sr.Close();
            fs.Close();
            return filestr;
        }

        /// <summary>
        /// Saves to file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="text">The text.</param>
        /// <param name="append">if set to <c>true</c> [append].</param>
        /// <returns></returns>
        public static bool SaveToFile(string filename, string text, bool append = false)
        {
            var exist = "";
            if (append && File.Exists(filename))
                exist = LoadFile(filename);

            var fs = new FileStream(filename, FileMode.Create);
            var sw = new StreamWriter(fs);
            sw.Write(exist + Environment.NewLine);
            sw.Write(text);
            sw.Close();
            fs.Close();
            return true;
        }

        /// <summary>
        /// Creates the directory tree.
        /// </summary>
        /// <param name="dirs">The dirs.</param>
        /// <param name="basedir">The basedir.</param>
        private static void CreateDirectoryTree(string[] dirs, string basedir)
        {
            var d = basedir;
            foreach (var s in dirs)
            {
                d += "\\" + s;
                if (Directory.Exists(d) == false)
                    Directory.CreateDirectory(d);
            }
        }

        /// <summary>
        /// move all the children items from merge to based, overwriting existing
        /// </summary>
        /// <param name="based">dest directory,=</param>
        /// <param name="merge">directory to moverge into dest</param>
        public static void MergeDirectories(string based, string merge)
        {
            CreateDirectory(based);

            var mergefiles = Directory.GetFiles(merge, "*.*", SearchOption.AllDirectories).ToList();
            foreach (var file in mergefiles)
            {
                var mFile = new FileInfo(file);
                if (mFile.Directory == null)
                    continue;

                //create directory parents
                var d = mFile.Directory.FullName.Remove(0, merge.Length);
                var ds = d.Split(new[] {"\\"}, StringSplitOptions.RemoveEmptyEntries);
                CreateDirectoryTree(ds, based);

                var dirt = based + d + "\\" + mFile.Name;
                mFile.CopyTo(dirt, true);
                Application.DoEvents();
            }
        }

        /// <summary>
        /// delete directory if exists
        /// </summary>
        /// <param name="dir">The dir.</param>
        /// <exception cref="Exception">Can't delete directory</exception>
        public static void DeleteDirectory(string dir)
        {
            var trycount = 5;
            var @try = 0;
            //wait for directory to be deleted
            while (Directory.Exists(dir))
            {
                try
                {
                    Directory.Delete(dir, true);
                }
                catch
                {
                    Thread.Sleep(500);
                }
                @try++;
                if (@try >= trycount)
                    throw new Exception("Can't delete directory");
            }
        }

        /// <summary>
        /// create a file and then close the stream
        /// </summary>
        /// <param name="filepath">The filepath.</param>
        /// <returns></returns>
        public static bool CreateFile(string filepath)
        {
            try
            {
                using (var fs = new FileStream(filepath, FileMode.OpenOrCreate))
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// create directory if doesnt exist
        /// </summary>
        /// <param name="dir">The dir.</param>
        /// <exception cref="Exception">Can't create directory</exception>
        public static void CreateDirectory(string dir)
        {
            var trycount = 5;
            var @try = 0;
            //wait for directory to be created
            while (Directory.Exists(dir) == false)
            {
                try
                {
                    Directory.CreateDirectory(dir);
                }
                catch
                {
                    Thread.Sleep(100);
                }
                @try++;
                if (@try >= trycount)
                    throw new Exception("Can't create directory");
            }
        }

        /// <summary>
        /// Trims the name of the file.
        /// </summary>
        /// <param name="fn">The function.</param>
        /// <param name="basepath">if set to <c>true</c> [basepath].</param>
        /// <param name="filename">if set to <c>true</c> [filename].</param>
        /// <param name="extension">if set to <c>true</c> [extension].</param>
        /// <returns></returns>
        public static string TrimFileName(string fn, bool basepath, bool filename, bool extension)
        {
            var i1 = fn.LastIndexOf('\\') + 1;
            var i2 = fn.LastIndexOf('.');
            var bp = fn.Substring(0, i1);
            var fin = fn.Substring(i1, i2 - i1);
            var ex = fn.Substring(i2);
            var ret = "";
            if (basepath)
                ret = bp;
            if (filename)
                ret += fin;
            if (extension)
                ret += ex;

            return ret;
        }

        /// <summary>
        /// get the matching file for a substring of the file name
        /// </summary>
        /// <param name="partialFN">a part of the file name to look for</param>
        /// <param name="basedir">The basedir.</param>
        /// <returns></returns>
        public static string GetAbsoluteFilePath(string partialFN, string basedir)
        {
            var f = DirectoryExtras.GetFilesRecursive(basedir);

            foreach (var f2 in f)
            {
                var f3 = TrimFileName(f2, false, true, false);
                if (f3.Contains(partialFN))
                    return f2;
            }
            return null;
        }

        /// <summary>
        /// Generates the random name of the file.
        /// </summary>
        /// <param name="fileextension">The fileextension.</param>
        /// <returns></returns>
        public static string GenerateRandomFileName(string fileextension = "txt")
        {
            var seed = DateTime.Now.Ticks.ToString();
            return seed + "." + fileextension;
        }
    }
}