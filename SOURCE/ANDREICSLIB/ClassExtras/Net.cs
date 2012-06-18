using System;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace ANDREICSLIB
{
	public abstract class Net
	{
		public static string DownloadWebPage(string url)
		{
			// Open a connection
			var webRequestObject = (HttpWebRequest) WebRequest.Create(url);

			// You can also specify additional header values like 
			// the user agent or the referer:
			webRequestObject.UserAgent = ".NET Framework/2.0";
			webRequestObject.Referer = "http://www.example.com/";

			// Request response:
		    WebResponse response;
		    try
		    {
                response = webRequestObject.GetResponse();
		    }
		    catch (Exception ex)
		    {
		        return null;
		    }
            

			// Open data stream:
			var webStream = response.GetResponseStream();

			// Create reader object:
			if (webStream != null)
			{
				var reader = new StreamReader(webStream);

				// Read the entire stream content:
				var pageContent = reader.ReadToEnd();

				// Cleanup
				reader.Close();
				webStream.Close();
				response.Close();

				return pageContent;
			}
			return null;
		}
	}
}