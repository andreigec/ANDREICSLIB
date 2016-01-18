using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ANDREICSLIB.ClassExtras;

namespace ANDREICSLIB.Helpers
{

    /// <summary>
    /// example usage: https://github.com/andreigec/Meal-Chooser
    /// </summary>
    public class BTree
    {
        public static void SaveFileIntoTree<T>(string filename, Btree<T> root, string levelSeparator = "\t")
        {
            var fs = new FileStream(filename, FileMode.Create);
            var sw = new StreamWriter(fs);

            string ret = "";
            SaveTree(root, ref ret, 0, levelSeparator);
            sw.Write(ret);

            sw.Close();
            fs.Close();
        }

        private static void SaveTree<T>(Btree<T> node, ref string ret, int level, string levelSeparator = "\t")
        {
            if (node.Name != null)
            {
                ret += node.Name;
                ret += "\r\n";
            }
            if (node.Children != null && node.Children.Count > 0)
            {
                foreach (var c in node.Children)
                {
                    for (int a = 0; a < level; a++)
                        ret += levelSeparator;
                    SaveTree(c, ref ret, level + 1, levelSeparator);
                }
            }
        }

        /// <summary>
        /// default for strings
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="root"></param>
        /// <param name="levelSeparator"></param>
        /// <param name="RecreateFileIfInvalid"></param>
        public static void LoadFileIntoTree(string filename, Btree<string> root, string levelSeparator = "\t",
                                            bool RecreateFileIfInvalid = true)
        {
            LoadFileIntoTree(filename, root, s => s, levelSeparator, RecreateFileIfInvalid);
        }

        /// <summary>
        /// Load a file into a tree structure based on levels. by default '1 \n \t 2' in a file will create a parent with a child node
        /// </summary>
        /// <typeparam name="T">class type, usually string</typeparam>
        /// <param name="filename"></param>
        /// <param name="root"></param>
        /// <param name="addfunc">T must be able to be instantiated with a string. call with a=>new T(a) where T is your class, or the return string method</param>
        /// <param name="levelSeparator"></param>
        /// <param name="RecreateFileIfInvalid"></param>
        public static void LoadFileIntoTree<T>(string filename, Btree<T> root, Func<string, T> addfunc,
                                               string levelSeparator = "\t", bool RecreateFileIfInvalid = true)
        {
            root.Children = new List<Btree<T>>();

            FileStream fs = null;
            StreamReader sr = null;
            try
            {
                fs = new FileStream(filename, FileMode.OpenOrCreate);
                sr = new StreamReader(fs);

                string line = sr.ReadLine();
                Btree<T> parentT = root;
                int currentlevel = 0;
                while (line != null)
                {
                    int level = StringExtras.ContainsSubStringCount(line, levelSeparator);
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
                            parentT = parentT.Parent;
                            currentlevel--;
                        }
                    }

                    string rc = StringExtras.ReplaceAllChars(line, levelSeparator, "");

                    var t = new Btree<T> { Name = addfunc(rc), Parent = parentT };
                    if (parentT.Children == null)
                        parentT.Children = new List<Btree<T>>();

                    parentT.Children.Add(t);
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

            catch 
            {
                if (sr != null)
                    sr.Close();

                if (fs != null)
                    fs.Close();

                if (RecreateFileIfInvalid)
                {
                    if (File.Exists(filename))
                        File.Delete(filename);
                    File.Create(filename);
                }

                root.Children = new List<Btree<T>>();
            }
        }
    }

    public class Btree<T>
    {
        public List<Btree<T>> Children;
        public T Name;
        public Btree<T> Parent;

        public void ClearChildren()
        {
            if (Children == null)
                Children = new List<Btree<T>>();
            Children.Clear();
        }

        public Btree<T> GetChildByName(T nameC)
        {
            if (Children == null)
                return null;
            return Children.FirstOrDefault(v => v.Name.Equals(nameC));
        }

        public Btree<T> AddChild(T nameC)
        {
            var t = new Btree<T>();
            t.Name = nameC;
            t.Parent = this;
            if (Children == null)
                Children = new List<Btree<T>>();
            Children.Add(t);

            return Children.Last();
        }

        public void RemoveChild(T nameC)
        {
        redo:
            for (int a = 0; a < Children.Count; a++)
            {
                if (Children[a].Name.Equals(nameC))
                {
                    Children.RemoveAt(a);
                    goto redo;
                }
            }
        }
    }
}