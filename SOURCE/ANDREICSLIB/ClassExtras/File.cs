using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using ANDREICSLIB;
using ANDREICSLIB.ClassExtras;
using tree = ANDREICSLIB.DataClasses.Btree<System.String>;

namespace ANDREICSLIB
{
    public abstract class FileUpdates
    {
        /// <summary>
        /// load a file from a path into a string
        /// </summary>
        /// <param name="filename"></param>
        public static string LoadFile(String filename)
        {
            var fs = new FileStream(filename, FileMode.Open);
            var sr = new StreamReader(fs);
            var filestr = sr.ReadToEnd();
            sr.Close();
            fs.Close();
            return filestr;
        }

        public static void SaveToFile(String filename, String text)
        {
            var fs = new FileStream(filename, FileMode.Create);
            var sw = new StreamWriter(fs);
            sw.Write(text);
            sw.Close();
            fs.Close();
        }

        public static void SaveFileIntoTree(String filename, DataClasses.Btree<string> root, String levelSeparator = "\t")
        {
            var fs = new FileStream(filename, FileMode.Create);
            var sw = new StreamWriter(fs);

            var ret = "";
            SaveTree(root, ref ret, 0, levelSeparator);
            sw.Write(ret);

            sw.Close();
            fs.Close();
        }

        private static void SaveTree(DataClasses.Btree<string> node, ref String ret, int level, String levelSeparator = "\t")
        {
            if (node.name != null)
            {
                ret += node.name;
                ret += "\r\n";
            }
            if (node.children != null && node.children.Count > 0)
            {
                foreach (var c in node.children)
                {
                    for (var a = 0; a < level; a++)
                        ret += levelSeparator;
                    SaveTree(c, ref ret, level + 1, levelSeparator);
                }
            }
        }

        /// <summary>
        /// Load a file into a tree structure based on levels. by default '1 \n \t 2' in a file will create a parent with a child node
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="root"></param>
        /// <param name="levelSeparator"></param>
        public static void LoadFileIntoTree(string filename, DataClasses.Btree<string> root, String levelSeparator = "\t",bool RecreateFileIfInvalid=true)
        {
            root.children = new List<tree>();

            FileStream fs=null;
            StreamReader sr = null;
            try
            {
                fs = new FileStream(filename, FileMode.OpenOrCreate);
                sr = new StreamReader(fs);

                var line = sr.ReadLine();
                var parentT = root;
                var currentlevel = 0;
                while (line != null)
                {
                    var level = StringUpdates.ContainsSubStringCount(line, levelSeparator);
                    if (level > (currentlevel + 1))
                    {
                        throw new Exception();
                    }
                    if (level == 0)
                    {
                        parentT = root;
                    }
                    else if (currentlevel > (level - 1))
                    {
                        while (currentlevel != (level - 1))
                        {
                            parentT = parentT.parent;
                            currentlevel--;
                        }
                    }

                    var t = new DataClasses.Btree<string>() { name = StringUpdates.replaceAllChars(line, levelSeparator, ""), parent = parentT };
                    if (parentT.children == null)
                        parentT.children = new List<DataClasses.Btree<string>>();

                    parentT.children.Add(t);
                    parentT = t;
                    currentlevel = level;
                redo:
                    line = sr.ReadLine();
                    if (line != null && line.Length == 0)
                        goto redo;
                }

                sr.Close();
                fs.Close();
            }

            catch (Exception ex)
            {
                if (sr!=null)
                    sr.Close();

                if (fs!=null)
                    fs.Close();

					if (RecreateFileIfInvalid)
					{
						if (File.Exists(filename))
							File.Delete(filename);
						File.Create(filename);
					}

                root.children = new List<tree>();
            }
        }

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
        public static void MergeDirectories(String based, string merge)
        {
            FileUpdates.CreateDirectory(based);

            var mergefiles = Directory.GetFiles(merge, "*.*", SearchOption.AllDirectories).ToList();
            foreach (var file in mergefiles)
            {
                var mFile = new FileInfo(file);
                if (mFile.Directory == null)
                    continue;

                //create directory parents
                var d = mFile.Directory.FullName.Remove(0, merge.Length);
                var ds = d.Split(new[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                CreateDirectoryTree(ds, based);

                var dirt = based + d + "\\" + mFile.Name;
                mFile.CopyTo(dirt, true);
            }
        }

        /// <summary>
        /// delete directory if exists
        /// </summary>
        /// <param name="dir"></param>
        public static void DeleteDirectory(String dir)
        {

            //wait for directory to be deleted
            while (Directory.Exists(dir))
            {
                try
                {
                    Directory.Delete(dir, true);
                }
                catch (Exception ex)
                {
                    Thread.Sleep(100);
                    continue;
                }
            }
        }

        /// <summary>
        /// create directory if doesnt exist
        /// </summary>
        /// <param name="dir"></param>
        public static void CreateDirectory(String dir)
        {
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
                    continue;
                }
            }
        }

        public static string TrimFileName(String fn,bool basepath,bool filename,bool extension)
        {
            int i1=fn.LastIndexOf('\\') + 1;
            int i2 = fn.LastIndexOf('.');
            String bp = fn.Substring(0, i1);
            string fin = fn.Substring(i1, i2-i1);
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
        public static string GetAbsoluteFilePath(String partialFN,String basedir)
        {
            var f = DirectoryUpdates.GetFilesRecursive(basedir);

            foreach(var f2 in f)
            {
                var f3 = TrimFileName(f2, false, true, false);
                if (f3.Contains(partialFN))
                    return f2;
            }
            return null;
        }
    }
}
