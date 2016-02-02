using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ANDREICSLIB.ClassExtras;

namespace ANDREICSLIB.Licensing
{
    public static class GitHubLicensing
    {
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

        private static string GetDownloadPath(string html)
        {
            var pattern =
                 "<div class=\"release label-latest\">.*release-downloads.*?<a href=\"(.*?)\"";
            var downloadPathMatch = Regex.Match(html, pattern, RegexOptions.Singleline);
            var path = downloadPathMatch.Groups[1].Value;
            path = "https://github.com" + path;
            return path;
        }

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
            catch (Exception ex)
            {
            }
            return null;
        }
    }
}
