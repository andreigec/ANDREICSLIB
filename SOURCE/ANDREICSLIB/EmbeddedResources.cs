using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

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
            var rs = GetResourceStream(filename, Assembly.GetCallingAssembly());
            if (rs == null)
                return null;
            var s = Image.FromStream(rs.BaseStream);
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
            var rs = GetResourceStream(filename, Assembly.GetCallingAssembly());
            if (rs == null)
                return null;
            var s = rs.ReadToEnd();
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
                var n = a.GetManifestResourceNames();
                var n1 = n.Where(s => s.EndsWith(filename));
                if (n1.Count() == 1)
                {
                    var s1 = n1.First();
                    var s2 = a.GetManifestResourceStream(s1);
                    var s3 = new StreamReader(s2);
                    return s3;
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
