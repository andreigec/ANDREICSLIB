using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ANDREICSLIB.ClassExtras;

namespace ANDREICSLIB.Licensing
{
    /// <summary>
    /// 
    /// </summary>
    public class Licensing
    {
        /// <summary>
        /// Call this to prompt and update app via console
        /// </summary>
        /// <param name="helpString">The help string.</param>
        /// <param name="otherText">The other text.</param>
        /// <returns></returns>
        public static async Task UpdateConsole(string helpString, string otherText)
        {
            var a = AssemblyExtras.GetCallingAssemblyInfo();
            LicensingHelpers.InitLicensing(
                new LicensingHelpers.SolutionDetails(
                    GitHubLicensing.GetGitHubReleaseDetails,
                    helpString,
                    a.AppName, a.RepoName, a.CurrentVersion,
                    otherText));

            await LicensingHelpers.UpdateEventConsole();
        }

        /// <summary>
        /// Initialises the form. Add about/update toolstrip
        /// </summary>
        /// <param name="baseform">The baseform.</param>
        /// <param name="existingMenuStrip">The existing menu strip.</param>
        /// <param name="newsd">The newsd.</param>
        /// <returns></returns>
        public static void InitialiseForm(Form baseform, MenuStrip existingMenuStrip,
            LicensingHelpers.SolutionDetails newsd)
        {
            LicensingHelpers.InitLicensing(baseform, existingMenuStrip, newsd);
        }
    }
}
