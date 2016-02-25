using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using ANDREICSLIB.ClassExtras;

namespace ANDREICSLIB.Licensing
{
    /// <summary>
    /// 
    /// </summary>
    public static class LicensingHelpers
    {
        private static SolutionDetails _sd;

        /// <summary>
        ///     so we dont allow 2 instances of the about dialog
        /// </summary>
        internal static bool ShowingAbout;

        /// <summary>
        ///     so we dont allow 2 instances of the help dialog
        /// </summary>
        internal static bool ShowingHelp;

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <param name="ms">The ms.</param>
        /// <returns></returns>
        private static ToolStripMenuItem GetItem(MenuStrip ms)
        {
            //see if a help item exists
            foreach (ToolStripMenuItem i in ms.Items)
            {
                var t = i.Text;
                t = t.ToLower();
                t = t.Replace("&", "");

                if (t.Equals("help"))
                {
                    return i;
                }
            }

            return null;
        }

        private static void AddHelpOption(ToolStripMenuItem helpParent)
        {
            var helpitem = new ToolStripMenuItem("H&elp");
            helpitem.Click += Helpbox;
            helpParent.DropDownItems.Add(helpitem);
        }

        public static void InitLicensing(SolutionDetails newsd)
        {
            _sd = newsd;
        }

        /// <summary>
        /// INIT LICENSING FOR FORM - CALL THIS
        /// Inserts about tab on menu strip
        /// </summary>
        /// <param name="baseform">The baseform.</param>
        /// <param name="existingMenuStrip">The existing menu strip.</param>
        /// <param name="newsd">The newsd.</param>
        public static void InitLicensing(Form baseform, MenuStrip existingMenuStrip,
            SolutionDetails newsd)
        {
            InitLicensing(newsd);

            baseform.Text = _sd.AppName + " Version:" + _sd.Ld.CurrentVersion;

            var existed = false;
            const string help = "&Help";

            var helpToolStripItem = GetItem(existingMenuStrip);
            if (helpToolStripItem != null)
                existed = true;
            else
            //if it doesnt, create
            {
                existingMenuStrip.Items.Add(new ToolStripMenuItem(help));
                //should always be set now
                helpToolStripItem = GetItem(existingMenuStrip);
            }

            //add the help window
            if (!string.IsNullOrEmpty(newsd.HelpText))
            {
                AddHelpOption(helpToolStripItem);
            }

            //check for updates button
            var updateitem = new ToolStripMenuItem("&Check For Updates");
            updateitem.Click += async (a, b) => { await UpdateEvent(a, b); };
            helpToolStripItem.DropDownItems.Add(updateitem);

            //about item
            var aboutitem = new ToolStripMenuItem("&About");
            aboutitem.Click += Aboutbox;
            helpToolStripItem.DropDownItems.Add(aboutitem);

            //add all the items to the menu if help didnt exist
            if (existed == false)
            {
                existingMenuStrip.Items.Add(helpToolStripItem);
            }
        }

        /// <summary>
        /// Aboutboxes the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private static void Aboutbox(object sender, EventArgs e)
        {
            var AS = new AboutScreen
            {
                Text = "About " + _sd.AppName,
                appversionlabel = { Text = "Version " + _sd.Ld.CurrentVersion },
                apptitlelabel = { Text = _sd.AppName },
                otherapptext = { Text = _sd.AboutScreenOtherText }
            };

            if (ShowingAbout == false)
            {
                ShowingAbout = true;
                AS.ShowDialog();
            }
        }

        /// <summary>
        /// Helpboxes the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private static void Helpbox(object sender, EventArgs e)
        {
            var hs = new HelpScreen { Text = _sd.AppName + " Help", helpbox = { Text = _sd.HelpText } };

            if (ShowingHelp == false)
            {
                ShowingHelp = true;
                hs.Show();
            }
        }

        private static async Task<LicensingDetails> GetUpdateDetails()
        {
            bool error = false;
            string errormsg = null;
            var ld = _sd.Ld;
            try
            {
                await _sd.Callback();
            }
            catch (Exception ex)
            {
                error = true;
                errormsg = "Error while getting new version:" + ex;
            }

            if (ld?.OnlineVersion == null || error)
            {
                if (ld == null)
                    ld = new LicensingDetails();

                ld.Response = LicensingDetails.LicenseResponse.Error;
                ld.ResponseMessage = errormsg ?? "Error while getting new version";
            }
            else
            {
                if (_sd.Ld.CurrentVersion >= ld.OnlineVersion)
                {
                    ld.Response = LicensingDetails.LicenseResponse.UpToDate;
                    ld.ResponseMessage = "No update required, you already have an up to date version of " + _sd.AppName;
                }

                ld.ChangeLog = ld.ChangeLog;
                ld.Response = LicensingDetails.LicenseResponse.NewVersion;
                ld.ResponseMessage = "New update available.\r\n" + "Your version of " + _sd.AppName + ":" + _sd.Ld.CurrentVersion +
                           "\nNewest version online:" +
                           ld.OnlineVersion;
            }

            return ld;
        }

        private static async Task UpdateEvent(object sender, EventArgs e)
        {
            await UpdateApplicationForm();
        }

        /// <summary>
        /// update process via console
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> IsUpdateConsoleRequired()
        {
            Console.WriteLine("--------------\r\nUPDATE PROCESS\r\n--------------");
            Console.WriteLine("This app will now connect to the internet to find  the newest version.\nDo you wish to continue? y/[n]");

            var ok = Console.ReadKey().Key.ToString();
            if (ok.ToLower() != "y")
            {
                return false;
            }

            var det = await GetUpdateDetails();
            Console.WriteLine("\r\n>>" + det.ResponseMessage);

            if (det.Response != LicensingDetails.LicenseResponse.NewVersion)
                return false;

            Console.WriteLine(
                det.OnlineVersion +
                "\n\nDo you wish to update to this version? y/[n] \n(Be aware that this program will restart; please save your data beforehand)");

            ok = Console.ReadKey().Key.ToString();
            if (ok.ToLower() != "y")
            {
                return false;
            }

            UpdateApplication(det);
            return true;
        }

        /// <summary>
        /// update process via winform
        /// </summary>
        /// <returns></returns>
        public static async Task UpdateApplicationForm()
        {
            var dr1 =
                MessageBox.Show(
                    _sd.AppName +
                    " will now connect to the internet to find  the newest version.\nDo you wish to continue?",
                    "Notification", MessageBoxButtons.YesNo);
            if (dr1 == DialogResult.No)
                return;

            var det = await GetUpdateDetails();
            MessageBox.Show(det.ResponseMessage);

            if (det.Response != LicensingDetails.LicenseResponse.NewVersion)
                return;

            var dr =
                MessageBox.Show(
                    det.OnlineVersion +
                    "\n\nDo you wish to update to this version? \n(Be aware that this program will restart; please save your data beforehand)",
                    "Do you wish to update?", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
                UpdateApplication(det);
        }

        /// <summary>
        /// Updates the application.
        /// </summary>
        /// <param name="dsd">The DSD.</param>
        private static void UpdateApplication(LicensingDetails dsd)
        {
            string folder;
            string localfile;
            //we need the exe file for later execution
            var exefile = "";
            string exefolder;

            try
            {
                //0: reset current directory in case it was changed
                Directory.SetCurrentDirectory(Application.StartupPath);
                //1: Get the online files
                folder = _sd.AppName + "v" + DateTime.Now.Ticks;
                Directory.CreateDirectory(folder);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while creating the temporary folder\n:" + ex);
                return;
            }

            try
            {
                var client = new WebClient();
                localfile = dsd.FileLocation.Substring(dsd.FileLocation.LastIndexOf('/') + 1);
                client.DownloadFile(dsd.FileLocation, localfile);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while downloading new files\n:" + ex);
                return;
            }

            try
            {
                var myname = AppDomain.CurrentDomain.FriendlyName;
                const string vshost = ".vshost.";
                //remove .vshost for testing
                if (myname.Contains(vshost))
                    myname = myname.Replace(vshost, ".");

                //2.1 unpack
                ZipExtras.ExtractZipFile(localfile, folder);

                //2.2 find exe
                foreach (var f in DirectoryExtras.GetFilesRecursive(folder))
                {
                    if (f.Contains(".exe"))
                        exefile = f;

                    if (f.EndsWith(myname))
                        break;
                }

                const string br = "\\";
                exefolder = "";
                //ignore everything above this dir
                while (exefile.Length > 0)
                {
                    var c = StringExtras.ContainsSubStringCount(exefile, br);
                    if (c > 0)
                    {
                        var s = exefile.Substring(0, exefile.IndexOf(br, StringComparison.Ordinal));
                        exefolder += s + br;
                        exefile = exefile.Substring(exefile.IndexOf(br, StringComparison.Ordinal) + 1);
                    }
                    else
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while unzipping new files\n:" + ex);
                return;
            }

            //move folder/* to the local dir
            //delete the zip file
            //delete folder remnants
            //start the exefile we found before

            var operations = "move /Y \"" + exefolder + "\"\\* . " +
                             "& del /Q \"" + localfile + "\" " +
                             "& rmdir /Q /S \"" + folder + "\" " +
                             "& start \"\" \"" + exefile + "\" ";

            RunCommands(operations);

            //4: Kill process			
            Application.Exit();
        }
        
        private static void RunCommands(string operations)
        {
            //3: Run async cmd prompt to move unpacked files and remove the folder in a second, and rerun the exe
            var osInfo = Environment.OSVersion;
            try
            {
                if (osInfo.Platform == PlatformID.Win32NT && osInfo.Version.Major > 5)
                {
                    //vista+

                    #region

                    /*
			 * The following is a description of what those parameters mean.  
			 * 1. Cmd /C causes a command window to appear and run the command specified.. 
			 * it then causes the window to close automatically.
			 * 2. Choice /C Y /N /D Y /T 3 displays an empty, flashing prompt. 
			 * However, the /T 3 means that the prompt will automatically select the default choice Y (/D Y) after 3 seconds. 
			 * 3. & is used to chain multiple commands together on a single line in a batch file. 
			 * 4. Del <Application.ExecutablePath>... Well, I'm sure you can imagine what that does.
			 * Everything after the & can be replaced with anything you want to happen after the three second delay.			 
			 */

                    #endregion

                    Process.Start("cmd.exe", "/C choice /C Y /N /D Y /T 1 & " + operations);
                }
                else
                {
                    //xp-

                    #region

                    /*
				 *This is fairly similar to the previous version except it uses the ping command to do the dirty work.
				 *-n 1 tells the command to only ping one time and -w 3000 tells the command to wait 3 seconds before performing the ping.
				 *> Nul basically just hides the output of the ping command. 
				 */

                    #endregion

                    Process.Start("cmd.exe", "/C ping 1.1.1.1 -n 1 -w 1000 > Nul & " + operations);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during final stages of update\n:" + ex);
                return;
            }
        }
    }
}