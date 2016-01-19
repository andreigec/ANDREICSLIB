using System.Collections.Generic;
using System.Threading.Tasks;

namespace ANDREICSLIB.Extracters
{
    public interface IExtract
    {
        Dictionary<string, object> Extract(string content);

        Task<string> DownloadPage(string url, string @params, string method, List<KeyValuePair<string, string>> cookies);
    }
}
