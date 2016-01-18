using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;

namespace ANDREICSLIB.ClassExtras
{
    /// <summary>
    /// example usage: https://github.com/andreigec/Dwarf-Fortress-Mod-Merger
    /// </summary>
    public static class FileExtras
    {
        /// <summary>
        /// load a file from a path into a string
        /// </summary>
        /// <param name="filename"></param>
        public static string LoadFile(string filename)
        {
            try
            {
                var fs = new FileStream(filename, FileMode.Open);
                var sr = new StreamReader(fs);
                string filestr = sr.ReadToEnd();
                sr.Close();
                fs.Close();
                return filestr;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static bool SaveToFile(string filename, string text, bool append = false)
        {
            try
            {
                string exist = "";
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
            catch (Exception e)
            {
            }
            return false;
        }

        private static void CreateDirectoryTree(string[] dirs, string basedir)
        {
            string d = basedir;
            foreach (string s in dirs)
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

            List<string> mergefiles = Directory.GetFiles(merge, "*.*", SearchOption.AllDirectories).ToList();
            foreach (string file in mergefiles)
            {
                var mFile = new FileInfo(file);
                if (mFile.Directory == null)
                    continue;

                //create directory parents
                string d = mFile.Directory.FullName.Remove(0, merge.Length);
                string[] ds = d.Split(new[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                CreateDirectoryTree(ds, based);

                string dirt = based + d + "\\" + mFile.Name;
                mFile.CopyTo(dirt, true);
            }
        }

        /// <summary>
        /// delete directory if exists
        /// </summary>
        /// <param name="dir"></param>
        public static void DeleteDirectory(string dir)
        {
            int trycount = 5;
            int @try = 0;
            //wait for directory to be deleted
            while (Directory.Exists(dir))
            {
                try
                {
                    Directory.Delete(dir, true);
                }
                catch (Exception ex)
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
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static bool CreateFile(string filepath)
        {
            try
            {
                var fs = new FileStream(filepath, FileMode.OpenOrCreate);
                fs.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// create directory if doesnt exist
        /// </summary>
        /// <param name="dir"></param>
        public static void CreateDirectory(string dir)
        {
            int trycount = 5;
            int @try = 0;
            //wait for directory to be created
            while (Directory.Exists(dir) == false)
            {
                try
                {
                    Directory.CreateDirectory(dir);
                }
                catch (Exception ex)
                {
                    Thread.Sleep(100);
                }
                @try++;
                if (@try >= trycount)
                    throw new Exception("Can't create directory");
            }
        }

        public static string TrimFileName(string fn, bool basepath, bool filename, bool extension)
        {
            int i1 = fn.LastIndexOf('\\') + 1;
            int i2 = fn.LastIndexOf('.');
            string bp = fn.Substring(0, i1);
            string fin = fn.Substring(i1, i2 - i1);
            string ex = fn.Substring(i2);
            string ret = "";
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
        /// <param name="basedir"></param>
        /// <returns></returns>
        public static string GetAbsoluteFilePath(string partialFN, string basedir)
        {
            IEnumerable<string> f = DirectoryExtras.GetFilesRecursive(basedir);

            foreach (string f2 in f)
            {
                string f3 = TrimFileName(f2, false, true, false);
                if (f3.Contains(partialFN))
                    return f2;
            }
            return null;
        }

        public static string GenerateRandomFileName(string fileextension = "txt")
        {
            var seed = DateTime.Now.Ticks.ToString();
            return seed + "." + fileextension;
        }
    }
}