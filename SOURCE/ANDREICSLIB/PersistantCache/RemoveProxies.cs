using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ANDREICSLIB.ClassExtras;

namespace ANDREICSLIB.PersistantCache
{
    public static class RemoveProxiesHelpers
    {
        private static void AddQualifiedTypeNames(IEnumerable<ClassNames> classNames)
        {
            foreach (var c in classNames)
            {
                var types = (from asm in AppDomain.CurrentDomain.GetAssemblies()
                             from type in asm.GetTypes()
                             where type.Name == c.Type
                             select type);
                var typename = types.First();
                var asmname = Activator.CreateInstance(typename).GetType();
                if (asmname.AssemblyQualifiedName == null)
                    continue;

                //remove version
                string caststr = asmname.AssemblyQualifiedName;
                if (asmname.AssemblyQualifiedName.Contains(", Version"))
                    caststr = asmname.AssemblyQualifiedName.Substring(0, asmname.AssemblyQualifiedName.IndexOf(", Version", System.StringComparison.Ordinal));
                caststr = caststr.Trim(',', ' ');

                ClassNamesDict.Add(c.Type, new ClassNames(c.FullName.Trim('"'), c.ProxyRemoved, c.Type, caststr));
            }
        }

        private static string ReplaceTypes(string instr)
        {
            var ret = ClassNamesDict.Keys.Aggregate(instr, (current, i) => current.Replace(ClassNamesDict[i].FullName, ClassNamesDict[i].ReflectedType));
            return ret;
        }

        public class ClassNames
        {
            public string FullName;
            public string ProxyRemoved;
            public string Type;
            public string ReflectedType;

            public ClassNames(string fullName, string proxyRemoved, string type, string rt = null)
            {
                FullName = fullName;
                ProxyRemoved = proxyRemoved;
                Type = type;
                ReflectedType = rt;
            }
        }


        private static Dictionary<string, ClassNames> ClassNamesDict = new Dictionary<string, ClassNames>();

        public static string RemoveProxies(string oldblock)
        {
            var regex = new Regex(@"(System\.Data\.Entity\.DynamicProxies\.(.*?))_.*?""");
            var regs = regex.Matches(oldblock);
            //get types to be qualified
            var unseentypes = (from Match r in regs select new ClassNames(r.Groups[0].Value, r.Groups[1].Value, r.Groups[2].Value)).DistinctBy(s => s.Type).Where(s2 => ClassNamesDict.ContainsKey(s2.Type) == false).ToList();
            //populate type dictionary
            AddQualifiedTypeNames(unseentypes);
            //replace text
            var newblock = ReplaceTypes(oldblock);
            return newblock;
        }
    }
}
