using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace ANDREICSLIB.Extracters
{
    /// <summary>
    /// download a webpage as json, and convert json to a dictionary
    /// </summary>
    public class JSONExtract : IExtract
    {
        readonly JavaScriptSerializer serializer = new JavaScriptSerializer();

        public async Task<string> DownloadPage(string url, string @params, string method = "POST", List<KeyValuePair<string, string>> cookies = null)
        {
            var jsr = new JsonRequest();
            var rex = await jsr.Execute(url, @params, method, cookies);
            return rex.ToString();
        }

        public Dictionary<string, object> Extract(string content)
        {
            serializer.RegisterConverters(new[] { new DynamicJsonConverter() });
            var obj = serializer.Deserialize(content, typeof(object));
            var d = ((DynamicJsonConverter.DynamicJsonObject)obj)._dictionary;
            return (Dictionary<string, object>)d;
        }
    }
}
