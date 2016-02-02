using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace ANDREICSLIB.Extracters
{
    /// <summary>
    ///     download a webpage as json, and convert json to a dictionary
    /// </summary>
    public class JSONExtract : IExtract
    {
        private readonly JavaScriptSerializer serializer = new JavaScriptSerializer();

        /// <summary>
        /// Downloads the page.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="params">The parameters.</param>
        /// <param name="method">The method.</param>
        /// <param name="cookies">The cookies.</param>
        /// <returns></returns>
        public async Task<string> DownloadPage(string url, string @params, string method = "POST",
            List<KeyValuePair<string, string>> cookies = null)
        {
            var jsr = new JsonRequest();
            var rex = await jsr.Execute(url, @params, method, cookies);
            return rex.ToString();
        }

        /// <summary>
        /// Extracts the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public Dictionary<string, object> Extract(string content)
        {
            serializer.RegisterConverters(new[] {new DynamicJsonConverter()});
            var obj = serializer.Deserialize(content, typeof (object));
            var d = ((DynamicJsonConverter.DynamicJsonObject) obj)._dictionary;
            return (Dictionary<string, object>) d;
        }
    }
}