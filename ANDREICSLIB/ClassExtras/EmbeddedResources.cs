using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ANDREICSLIB
{
    public static class EmbeddedResources
    {
        /// <summary>
        /// get the test from an embedded resource
        /// </summary>
        /// <param name="filename">pass the file name</param>
        /// <param name="a">call with Assembly.GetExecutingAssembly()</param>
        /// <returns></returns>
        public static string ReadEmbeddedResource(String filename, Assembly a)
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
                    string s4 = s3.ReadToEnd();
                    s3.Close();
                    return s4;
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
