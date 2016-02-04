using System;
using System.Diagnostics;
using System.Reflection;

namespace ANDREICSLIB.ClassExtras
{
    /// <summary>
    /// 
    /// </summary>
    public static class AssemblyExtras
    {
        /// <summary>
        /// Gets the assembly file version information.
        /// </summary>
        /// <returns></returns>
        public static Version GetAssemblyFileVersionInfo()
        {
            var assembly = Assembly.GetCallingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            var ret = Version.Parse(fvi.FileVersion);
            return ret;
        }
    }
}