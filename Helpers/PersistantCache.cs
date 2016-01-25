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
    /// example usage: https://github.com/andreigec/GithubSensitiveSearch
    /// </summary>
    public class PersistantCache
    {
        private string filename;
        private JsonSerializer js;

        private Dictionary<string, object> Storage { get; set; }

        public PersistantCache(string filename)
        {
            if (filename.EndsWith(".json") == false)
                filename += ".json";

            this.filename = filename;

            FileExtras.CreateFile(filename);

            using (var fs = new FileStream(filename, FileMode.Open))
            {
                Storage = DictionaryExtras.Deserialize(fs) ?? new Dictionary<string, object>();
            }

            js = new JsonSerializer();
            Set("", null);
        }

        public void Set(string key, object value)
        {
            Storage[key] = value;
            using (var fs = new FileStream(filename, FileMode.Create))
            {
                Storage.Serialise(js, fs);
            }
        }

        public T Get<T>(string cacheKey) where T : class
        {
            if (!Storage.ContainsKey(cacheKey))
                return null;

            //its either the object on current session
            if (Storage[cacheKey] is T)
                return (T)Storage[cacheKey];

            return ReturnType<T>(Storage[cacheKey]);
        }

        private T ReturnType<T>(object o) where T : class
        {
            //base type = return
            if (o is T)
                return (T)o;

            //object - straight cast
            if (o is JObject)
            {
                //or jobject when loaded from disk
                var ob = (JObject)o;
                var ty = ob.ToObject<T>();
                return ty;
            }
            // its a list
            if (o is JArray)
            {
                var oba = (o as JArray).ToObject<T>();
                return oba;
            }

            return null;
        }

        /// <summary>
        /// pass in a sync action
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="memberName"></param>
        /// <returns></returns>
        public T Cache<T>(Expression<Func<T>> action, [CallerMemberName] string memberName = "")
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
                    return Get<T>(cacheKey);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error on cache read:", ex);
            }

            var res = action.Compile().Invoke();

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

        /// <summary>
        /// pass in an async action
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="memberName"></param>
        /// <returns></returns>
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
                    return Get<T>(cacheKey);
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
