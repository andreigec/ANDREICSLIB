using System;
using System.Threading.Tasks;

namespace ANDREICSLIB.Licensing
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="appRepo">The application repo.</param>
    /// <returns></returns>
    public delegate Task<LicensingDetails> LicenseCallback(LicensingDetails ld, string appRepo);

    /// <summary>
    /// 
    /// </summary>
    public class SolutionDetails
    {
        internal string AboutScreenOtherText;
        internal string AppName;
        internal string AppRepo;
        internal string HelpText;
        internal LicensingDetails Ld;

        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionDetails"/> class.
        /// </summary>
        /// <param name="dsd">The DSD.</param>
        /// <param name="helpText">The help text.</param>
        /// <param name="appName">Name of the application.</param>
        /// <param name="appRepoName">Name of the application repo.</param>
        /// <param name="version">The version.</param>
        /// <param name="aboutScreenOtherText">The about screen other text.</param>
        public SolutionDetails(LicenseCallback dsd, string helpText = null, string appName = null,
            string appRepoName = null, Version version = null, string aboutScreenOtherText = null)
        {
            AppRepo = appRepoName;
            HelpText = helpText;
            this.AppName = appName;
            AboutScreenOtherText = aboutScreenOtherText;
            Ld = new LicensingDetails() {Callback = dsd, CurrentVersion = version};
        }

        /// <summary>
        /// Callbacks this instance.
        /// </summary>
        /// <returns></returns>
        public async Task<LicensingDetails> Callback()
        {
            if (Ld.Callback != null)
                return await Ld.Callback(Ld, AppRepo);
            return null;
        }
    }
}