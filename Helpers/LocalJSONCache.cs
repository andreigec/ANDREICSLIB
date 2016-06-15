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
    /// <seealso cref="ANDREICSLIB.Helpers.ICache" />
    public class LocalJSONCache : ICache
    {
        private readonly string filename;
        private readonly JsonSerializer js;
        private readonly JsonSerializer privatejson = JsonSerializerExtras.CreateWithNoPrivateItemsResolver();
        private readonly object @lock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalJSONCache"/> class.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public LocalJSONCache(string filename)
        {
            if (filename.EndsWith(".json") == false)
                filename += ".json";

            this.filename = filename;

            lock (@lock)
            {
                FileExtras.CreateFile(filename);

                using (var fs = new FileStream(filename, FileMode.Open))
                {
                    Storage = DictionaryExtras.Deserialize(fs) ?? new Dictionary<string, object>();
                }
            }

            js = new JsonSerializer();
        }

        private Dictionary<string, object> Storage { get; }

        /// <summary>
        /// set a value using a key and a value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> Set<T>(string key, T value, bool obeyDataContracts)
        {
            Storage[key] = value;

            lock (@lock)
            {
                using (var fs = new FileStream(filename, FileMode.Create))
                {
                    Storage.Serialise(js, fs, obeyDataContracts);
                    return true;
                }
            }
        }

        /// <summary>
        /// return a value using a key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public async Task<T> Get<T>(string cacheKey) where T : class
        {
            if (!Storage.ContainsKey(cacheKey))
                return null;

            //its either the object on current session
            if (Storage[cacheKey] is T)
                return (T)Storage[cacheKey];

            return ReturnType<T>(Storage[cacheKey]);
        }

        /// <summary>
        /// pass in a sync action
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="memberName"></param>
        /// <returns></returns>
        /// <exception cref="Exception">Error on cache read:
        /// or
        /// Error on cache write:</exception>
        public async Task<T> Cache<T>(Expression<Func<T>> action, [CallerMemberName] string memberName = "", bool obeyDataContracts = true)
            where T : class
        {
            var cacheKey = GetKey<T>(memberName, ((MethodCallExpression)action.Body));

            try
            {
                if (Storage.ContainsKey(cacheKey))
                {
                    return await Get<T>(cacheKey);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error on cache read:", ex);
            }

            var res = action.Compile().Invoke();

            lock (@lock)
            {
                try
                {
                    AsyncHelpers.RunSync(() => Set(cacheKey, res, obeyDataContracts));
                }
                catch (Exception ex)
                {
                    throw new Exception("Error on cache write:", ex);
                }
            }

            return res;
        }

        /// <summary>
        /// pass in an async function that returns a string - can compress if type is known
        /// </summary>
        /// <param name="action"></param>
        /// <param name="compress"></param>
        /// <param name="memberName"></param>
        /// <returns></returns>
        /// <exception cref="Exception">
        /// Error on cache read:
        /// or
        /// Error on cache write:
        /// </exception>
        public async Task<string> Cache(Expression<Func<Task<string>>> action, bool compress = false,
            [CallerMemberName] string memberName = "", bool obeyDataContracts = true)
        {
            var cacheKey = GetKey<string>(memberName, ((MethodCallExpression)action.Body));

            try
            {
                if (Storage.ContainsKey(cacheKey))
                {
                    var val = await Get<string>(cacheKey);
                    if (compress == false)
                        return val;

                    var unzip = val.DecompressString();
                    return unzip;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error on cache read:", ex);
            }

            var res = await action.Compile().Invoke();

            lock (@lock)
            {
                try
                {
                    var zipped = res.CompressString();
                    AsyncHelpers.RunSync(() => Set(cacheKey, zipped, obeyDataContracts));
                }
                catch (Exception ex)
                {
                    throw new Exception("Error on cache write:", ex);
                }
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
        /// <exception cref="Exception">
        /// Error on cache read:
        /// or
        /// Error on cache write:
        /// </exception>
        public async Task<T> Cache<T>(Expression<Func<Task<T>>> action, [CallerMemberName] string memberName = "", bool obeyDataContracts = true)
            where T : class
        {
            var cacheKey = GetKey<T>(memberName, ((MethodCallExpression)action.Body));

            try
            {
                if (Storage.ContainsKey(cacheKey))
                {
                    return await Get<T>(cacheKey);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error on cache read:", ex);
            }

            var res = await action.Compile().Invoke();

            lock (@lock)
            {
                try
                {
                    AsyncHelpers.RunSync(() => Set(cacheKey, res, obeyDataContracts));
                }
                catch (Exception ex)
                {
                    throw new Exception("Error on cache write:", ex);
                }
            }

            return res;
        }

        /// <summary>
        /// Returns the type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o">The o.</param>
        /// <returns></returns>
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
                var ty = ob.ToObject<T>(privatejson);
                return ty;
            }
            // its a list
            if (o is JArray)
            {
                var oba = (o as JArray).ToObject<T>(privatejson);
                return oba;
            }

            return null;
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="action">((MethodCallExpression)action.Body)</param>
        /// <returns></returns>
        private string GetKey<T>(string memberName, MethodCallExpression action) where T : class
        {
            var builder = new StringBuilder(100);
            builder.Append("AUTOCACHE.");
            builder.Append(memberName);

            (from MemberExpression expression in action.Arguments
             select ((FieldInfo)expression.Member).GetValue(((ConstantExpression)expression.Expression).Value))
                .ToList()
                .ForEach(x =>
                {
                    builder.Append("_");
                    builder.Append(x);
                });

            var cacheKey = builder.ToString();
            return cacheKey;
        }
    }
}