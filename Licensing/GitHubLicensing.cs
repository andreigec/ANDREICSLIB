using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ANDREICSLIB.ClassExtras;

namespace ANDREICSLIB.Licensing
{
    /// <summary>
    /// 
    /// </summary>
    public static class GitHubLicensing
    {
        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <returns></returns>
        private static Version GetVersion(string html)
        {
            var pattern =
                "<span class=\"release-label latest\">.*?<ul class=\"tag-references\">.*?<span class=\"css-truncate-target\">(.*?)</span>";
            var versionMatch = Regex.Match(html, pattern, RegexOptions.Singleline);
            var version = versionMatch.Groups[1].Value;

            if (version.Contains("v"))
                version = version.Substring(version.IndexOf("v") + 1);

            return Version.Parse(version);
        }

        /// <summary>
        /// Gets the download path.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <returns></returns>
        private static string GetDownloadPath(string html)
        {
            var pattern =
                "<div class=\"release label-latest\">.*release-downloads.*?<a href=\"(.*?)\"";
            var downloadPathMatch = Regex.Match(html, pattern, RegexOptions.Singleline);
            var path = downloadPathMatch.Groups[1].Value;
            path = "https://github.com" + path;
            return path;
        }

        /// <summary>
        /// Gets the change log.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <returns></returns>
        private static string GetChangeLog(string html)
        {
            var pattern =
                "<div class=\"release label-latest\">.*markdown-body\">(.*?)</div>";
            var changelogMatch = Regex.Match(html, pattern, RegexOptions.Singleline);
            var changelog = changelogMatch.Groups[1].Value;
            //remove html
            changelog = Regex.Replace(changelog, @"<[^>]+>|&nbsp;", "").Trim();
            changelog = changelog.Replace("  ", " ");
            return changelog;
        }

        /// <summary>
        /// Gets the git hub release details.
        /// </summary>
        /// <param name="appRepo">The application repo.</param>
        /// <returns></returns>
        public static async Task<Licensing.DownloadedSolutionDetails> GetGitHubReleaseDetails(string appRepo)
        {
            try
            {
                var det = NetExtras.DownloadWebPage($"https://github.com/andreigec/{appRepo}/releases/latest");
                if (det.IsFaulted)
                    return null;
                var htmlText = await det;

                var ret = new Licensing.DownloadedSolutionDetails();
                ret.Version = GetVersion(htmlText);
                ret.FileLocation = GetDownloadPath(htmlText);
                ret.ChangeLog = GetChangeLog(htmlText);
                return ret;
            }
            catch
            {
            }
            return null;
        }
    }
}