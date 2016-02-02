using System.Collections.Generic;
using System.Threading.Tasks;

namespace ANDREICSLIB.Extracters
{
    public interface IExtract
    {
        /// <summary>
        /// Extracts the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        Dictionary<string, object> Extract(string content);
        /// <summary>
        /// Downloads the page.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="params">The parameters.</param>
        /// <param name="method">The method.</param>
        /// <param name="cookies">The cookies.</param>
        /// <returns></returns>
        Task<string> DownloadPage(string url, string @params, string method, List<KeyValuePair<string, string>> cookies);
    }
}