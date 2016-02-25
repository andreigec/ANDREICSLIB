using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANDREICSLIB.Licensing
{
    /// <summary>
    /// 
    /// </summary>
    public class LicensingDetails
    {
        /// <summary>
        /// Gets or sets the response.
        /// </summary>
        /// <value>
        /// The response.
        /// </value>
        public enum LicenseResponse
        {
            /// <summary>
            /// error
            /// </summary>
            Error = 0,
            /// <summary>
            /// Up to date
            /// </summary>
            UpToDate = 1,
            /// <summary>
            /// The new version
            /// </summary>
            NewVersion = 2
        }

        /// <summary>
        /// Gets or sets the response.
        /// </summary>
        /// <value>
        /// The response.
        /// </value>
        public LicenseResponse Response { get; set; }
        /// <summary>
        /// Gets or sets the response message.
        /// </summary>
        /// <value>
        /// The response message.
        /// </value>
        public string ResponseMessage { get; set; }

        /// <summary>
        /// Gets or sets the current version.
        /// </summary>
        /// <value>
        /// The current version.
        /// </value>
        public Version CurrentVersion { get; set; }
        /// <summary>
        /// Gets or sets the online version.
        /// </summary>
        /// <value>
        /// The online version.
        /// </value>
        public Version OnlineVersion { get; set; }

        /// <summary>
        /// Gets or sets the change log.
        /// </summary>
        /// <value>
        /// The change log.
        /// </value>
        public string ChangeLog { get; set; }

        public string FileLocation { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="LicensingDetails"/> class.
        /// </summary>
        public LicensingDetails()
        {

        }
    }
}
