using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANDREICSLIB.ClassExtras
{
    public static class AssemblyExtras
    {
        public static Version GetAssemblyFileVersionInfo()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetCallingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            var ret = Version.Parse(fvi.FileVersion);
            return ret;
        }
    }
}
