using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ANDREICSLIB.ClassExtras;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ANDREICSLIB.Helpers
{
    /// <summary>
    /// example usage:https://github.com/andreigec/GithubSensitiveSearch
    /// </summary>
    public class PersistantCache : IDisposable
    {
        private string filename;
        private JsonSerializer js;

        private Dictionary<string, object> storage;

        public Dictionary<string, object> Storage
        {
            get
            {
                if (storage != null)
                    return storage;

                using (var fs = new FileStream(filename, FileMode.Open))
                {
                    storage = DictionaryExtras.Deserialize(fs) ?? new Dictionary<string, object>();
                    return storage;
                }
            }
            private set { }
        }

        public void Set(string key, object value)
        {
            Storage[key] = value;
            using (var fs = new FileStream(filename, FileMode.Create))
            {
                DictionaryExtras.Serialise(storage, js, fs);
            }
        }

        public PersistantCache(string filename)
        {
            this.filename = filename;

            if (filename.EndsWith(".agdb") == false)
                throw new Exception("must end with .agdb");

            FileExtras.CreateFile(filename);
            js = new JsonSerializer();
            Set("", null);
        }

        public void Dispose()
        {
        }

        public async Task<T> Cache<T>(Expression<Func<Task<T>>> action, [CallerMemberName] string memberName = "")
            where T : class
        {
            var builder = new StringBuilder(100);
            builder.Append("AUTOCACHE.");
            builder.Append(memberName);

            (from MemberExpression expression in ((MethodCallExpression)action.Body).Arguments
             select ((FieldInfo)expression.Member).GetValue(((ConstantExpression)expression.Expression).Value))
                .ToList()
                .ForEach(x =>
                {
                    builder.Append("_");
                    builder.Append(x);
                });

            string cacheKey = builder.ToString();

            try
            {
                if (Storage.ContainsKey(cacheKey))
                {
                    //its either the object on current session
                    if (Storage[cacheKey] is T)
                        return (T)Storage[cacheKey];

                    //or jobject when loaded from disk
                    var ob = (JObject)Storage[cacheKey];
                    var ty = ob.ToObject<T>();
                    return ty;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error on cache read:", ex);
            }

            var res = await action.Compile().Invoke();

            try
            {
                Set(cacheKey, res);
            }
            catch (Exception ex)
            {
                throw new Exception("Error on cache write:", ex);
            }

            return res;
        }
    }
}
