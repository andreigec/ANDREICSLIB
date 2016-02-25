using System;
using System.Diagnostics;
using System.Reflection;

namespace ANDREICSLIB.ClassExtras
{
    public class AssemblyValues
    {
        public Version CurrentVersion { get; set; }
        /// <summary>
        /// Gets or sets the name of the repo. assembly name/ repo name
        /// </summary>
        /// <value>
        /// The name of the repo.
        /// </value>
        public string RepoName { get; set; }
        /// <summary>
        /// Gets or sets the name of the application. assembly title / app name
        /// </summary>
        /// <value>
        /// The name of the application.
        /// </value>
        public string AppName { get; set; }

        /// <summary>
        /// appname : version
        /// </summary>
        /// <returns></returns>
        public string GetAppString()
        {
            return $"{AppName}:${CurrentVersion}";
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public static class AssemblyExtras
    {
        /// <summary>
        /// get assembly info
        /// </summary>
        /// <returns></returns>
        public static AssemblyValues GetCallingAssemblyInfo()
        {
            var assembly = Assembly.GetCallingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            var v = Version.Parse(fvi.FileVersion);
            var ret = new AssemblyValues() { CurrentVersion = v, AppName = fvi.FileDescription, RepoName = fvi.InternalName.Substring(0, fvi.InternalName.IndexOf(".", StringComparison.Ordinal)) };
            return ret;
        }
    }
}