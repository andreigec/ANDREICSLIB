using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace ANDREICSLIB.Extracters
{
    /// <summary>
    ///     Create Http Request, using json, and read Http Response.
    /// </summary>
    public class JsonRequest
    {
        public CookieContainer CookieContainer = new CookieContainer();

        /// <summary>
        ///     Constructor Overload that allows passing URL and the VERB to be used.
        /// </summary>
        /// <param name="url">URL which request will be created</param>
        /// <param name="verb">Http Verb that will be userd in this request</param>
        public JsonRequest(string url, string verb)
        {
            URL = url;
            Verb = verb;
        }

        /// <summary>
        ///     Default constructor overload without any paramter
        /// </summary>
        public JsonRequest()
        {
            Verb = "GET";
        }

        /// <summary>
        ///     Url of http server wich request will be created to.
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        ///     HTTP Verb wich will be used. Eg. GET, POST, PUT, DELETE.
        /// </summary>
        public string Verb { get; set; }

        /// <summary>
        ///     Request content, Json by default.
        /// </summary>
        public string Content
        {
            get { return "text/json"; }
        }

        /// <summary>
        ///     User and Password for Basic Authentication
        /// </summary>
        public Credentials Credentials { get; set; }

        public HttpWebRequest HttpRequest { get; internal set; }
        public HttpWebResponse HttpResponse { get; internal set; }

        /// <summary>
        /// Executes the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="obj">The object.</param>
        /// <param name="verb">The verb.</param>
        /// <param name="Cookies">The cookies.</param>
        /// <returns></returns>
        public async Task<object> Execute(string url, object obj, string verb,
            List<KeyValuePair<string, string>> Cookies)
        {
            if (url != null)
                URL = url;

            if (verb != null)
                Verb = verb;
            var target = new Uri(url);
            HttpRequest = CreateRequest();
            HttpRequest.ContentType = "application/x-www-form-urlencoded";
            HttpRequest.UserAgent =
                "User-Agent: Mozilla/5.0";
            HttpRequest.KeepAlive = true;
            if (Cookies != null)
            {
                foreach (var kvp in Cookies)
                {
                    var c = new Cookie(kvp.Key, kvp.Value) {Domain = target.Host};
                    HttpRequest.CookieContainer.Add(c);
                }
            }

            if (verb == "POST")
                WriteStream(obj);

            try
            {
                HttpResponse = (HttpWebResponse) (await HttpRequest.GetResponseAsync());
            }
            catch (WebException error)
            {
                HttpResponse = (HttpWebResponse) error.Response;
                return ReadResponseFromError(error);
            }

            return ReadResponse();
        }

        /// <summary>
        /// Executes the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public async Task<object> Execute(string url)
        {
            if (url != null)
                URL = url;

            HttpRequest = CreateRequest();

            try
            {
                HttpResponse = (HttpWebResponse) (await HttpRequest.GetResponseAsync());
            }
            catch (WebException error)
            {
                HttpResponse = (HttpWebResponse) error.Response;
                return ReadResponseFromError(error);
            }

            return ReadResponse();
        }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException">URL cannot be null.</exception>
        public async Task<object> Execute()
        {
            if (URL == null)
                throw new ArgumentException("URL cannot be null.");

            HttpRequest = CreateRequest();

            try
            {
                HttpResponse = (HttpWebResponse) (await HttpRequest.GetResponseAsync());
            }
            catch (WebException error)
            {
                HttpResponse = (HttpWebResponse) error.Response;
                return ReadResponseFromError(error);
            }

            return ReadResponse();
        }

        /// <summary>
        /// Creates the request.
        /// </summary>
        /// <returns></returns>
        internal HttpWebRequest CreateRequest()
        {
            var basicRequest = (HttpWebRequest) WebRequest.Create(URL);
            basicRequest.ContentType = Content;
            basicRequest.Method = Verb;
            basicRequest.CookieContainer = CookieContainer;

            if (Credentials != null)
                basicRequest.Headers.Add("Authorization", "Basic" + " " + EncodeCredentials(Credentials));

            return basicRequest;
        }

        /// <summary>
        /// Writes the stream.
        /// </summary>
        /// <param name="obj">The object.</param>
        internal void WriteStream(object obj)
        {
            if (obj != null)
            {
                using (var streamWriter = new StreamWriter(HttpRequest.GetRequestStream()))
                {
                    if (obj is string)
                        streamWriter.Write(obj);
                    else
                    {
                        var x = new JavaScriptSerializer().Serialize(obj);
                        streamWriter.Write(x);
                    }
                }
            }
        }

        /// <summary>
        /// Reads the response.
        /// </summary>
        /// <returns></returns>
        internal string ReadResponse()
        {
            if (HttpResponse != null)
                using (var streamReader = new StreamReader(HttpResponse.GetResponseStream()))
                    return streamReader.ReadToEnd();

            return string.Empty;
        }

        /// <summary>
        /// Reads the response from error.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <returns></returns>
        internal string ReadResponseFromError(WebException error)
        {
            using (var streamReader = new StreamReader(error.Response.GetResponseStream()))
                return streamReader.ReadToEnd();
        }

        /// <summary>
        /// Encodes the credentials.
        /// </summary>
        /// <param name="credentials">The credentials.</param>
        /// <returns></returns>
        internal static string EncodeCredentials(Credentials credentials)
        {
            var strCredentials = string.Format("{0}:{1}", credentials.UserName, credentials.Password);
            var encodedCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(strCredentials));

            return encodedCredentials;
        }
    }

    public class Credentials
    {
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}