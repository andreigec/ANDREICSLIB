using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ANDREICSLIB
{
    public static class EmbeddedResources
    {
        /// <summary>
        /// get the contents of an embedded resource image
        /// </summary>
        /// <param name="filename">pass the file name</param>
        /// <returns></returns>
        public static Image ReadEmbeddedImage(string filename)
        {
            StreamReader rs = GetResourceStream(filename, Assembly.GetCallingAssembly());
            if (rs == null)
                return null;
            Image s = Image.FromStream(rs.BaseStream);
            rs.Close();
            return s;
        }

        /// <summary>
        /// get the contents of an embedded resource
        /// </summary>
        /// <param name="filename">pass the file name</param>
        /// <returns></returns>
        public static string ReadEmbeddedResource(String filename)
        {
            StreamReader rs = GetResourceStream(filename, Assembly.GetCallingAssembly());
            if (rs == null)
                return null;
            string s = rs.ReadToEnd();
            rs.Close();
            return s;
        }

        /// <summary>
        /// get the underlying streamreader for the resource
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="a">assembly.getexecutingassembly</param>
        /// <returns>make sure to close the streamreader if not null</returns>
        private static StreamReader GetResourceStream(String filename, Assembly a)
        {
            if (a == null || string.IsNullOrEmpty(filename))
                return null;

            try
            {
                string[] n = a.GetManifestResourceNames();
                IEnumerable<string> n1 = n.Where(s => s.EndsWith(filename));
                if (n1.Count() == 1)
                {
                    string s1 = n1.First();
                    Stream s2 = a.GetManifestResourceStream(s1);
                    if (s2 != null)
                    {
                        var s3 = new StreamReader(s2);
                        return s3;
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}