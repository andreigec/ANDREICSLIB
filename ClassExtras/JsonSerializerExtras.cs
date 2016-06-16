using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ANDREICSLIB.Helpers;
using Newtonsoft.Json;

namespace ANDREICSLIB.ClassExtras
{
    /// <summary>
    /// 
    /// </summary>
    public static class JsonSerializerExtras
    {
        /// <summary>
        /// Creates the with standard resolver.
        /// </summary>
        /// <returns></returns>
        public static JsonSerializer CreateWithStandardResolver()
        {
            return new JsonSerializer();
        }

        /// <summary>
        /// Creates the with no private items resolver.
        /// </summary>
        /// <returns></returns>
        public static JsonSerializer CreateWithNoPrivateItemsResolver()
        {
            var ret = new JsonSerializer
            {
                ContractResolver = new NoPrivateItemsResolver(),
                NullValueHandling = NullValueHandling.Include,
                DefaultValueHandling = DefaultValueHandling.Include,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                DateFormatHandling = DateFormatHandling.IsoDateFormat
            };
            return ret;
        }
        /// <summary>
        /// Deserializes the json.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_serializer">The _serializer.</param>
        /// <param name="json">The json.</param>
        /// <returns></returns>
        public static T DeserializeJson<T>(this JsonSerializer _serializer, string json)
        {
            using (var stringReader = new StringReader(json))
            using (var jsonTextReader = new JsonTextReader(stringReader))
                return _serializer.Deserialize<T>(jsonTextReader);
        }

        /// <summary>
        /// Serializes the json.
        /// </summary>
        /// <param name="_serializer"></param>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static string SerializeJson(this JsonSerializer _serializer, object obj)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var jsonTextWriter = new JsonTextWriter(stringWriter))
                {
                    jsonTextWriter.QuoteChar = '"';
                    _serializer.Serialize(jsonTextWriter, obj);

                    var ret = stringWriter.ToString();
                    return ret;
                }
            }
        }
    }
}
