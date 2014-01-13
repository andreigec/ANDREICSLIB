using System;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Windows.Forms;
using ANDREICSLIB.ClassExtras;

namespace ANDREICSLIB
{
    public static class Licensing
    {
        private static SolutionDetails _sd;

        /// <summary>
        /// so we dont allow 2 instances of the about dialog
        /// </summary>
        internal static bool ShowingAbout;

        /// <summary>
        /// so we dont allow 2 instances of the help dialog
        /// </summary>
        internal static bool ShowingHelp;

        private static ToolStripMenuItem GetItem(MenuStrip ms)
        {
            //see if a help item exists
            foreach (ToolStripMenuItem i in ms.Items)
            {
                string t = i.Text;
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

        /// <summary>
        /// Adds functionality for auto updating application, and creation of about screen.
        /// Inserts about tab on menu strip
        /// </summary>
        /// <param name="baseform"></param>
        /// <param name="newsd"></param>
        /// <param name="existingMenuStrip"></param>
        public static void CreateLicense(Form baseform, MenuStrip existingMenuStrip, SolutionDetails newsd)
        {
            _sd = newsd;

            baseform.Text = _sd.FormTitle + " Version:" + _sd.AppVersion;

            bool existed = false;
            const string help = "&Help";

            ToolStripMenuItem helpToolStripItem = GetItem(existingMenuStrip);
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
            updateitem.Click += UpdateApplication;
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

        private static void Aboutbox(object sender, EventArgs e)
        {
            var AS = new aboutScreen
                         {
                             Text = "About " + _sd.FormTitle,
                             appversionlabel = { Text = "Version " + _sd.AppVersion },
                             apptitlelabel = { Text = _sd.FormTitle },
                             otherapptext = { Text = _sd.AboutScreenOtherText }
                         };

            if (ShowingAbout == false)
            {
                ShowingAbout = true;
                AS.ShowDialog();
            }
        }

        private static void Helpbox(object sender, EventArgs e)
        {
            var hs = new helpScreen { Text = _sd.FormTitle + " Help", helpbox = { Text = _sd.HelpText } };

            if (ShowingHelp == false)
            {
                ShowingHelp = true;
                hs.Show();
            }
        }

        private static void UpdateApplication(object sender, EventArgs e)
        {
            DialogResult dr1 =
                MessageBox.Show(
                    _sd.FormTitle +
                    " will now connect to the internet to find  the newest version.\nDo you wish to continue?",
                    "Notification", MessageBoxButtons.YesNo);
            if (dr1 == DialogResult.No)
                return;

            DownloadedSolutionDetails dsd = null;
            Exception ex = null;
            try
            {
                dsd = _sd.gd();
            }
            catch (Exception ex2)
            {
                ex = ex2;
            }

            if (dsd == null)
            {
                MessageBox.Show("Error while accessing server:" + ex ?? "");
                return;
            }

            string versionS = "Your version of " + _sd.FormTitle + ":" + _sd.AppVersion +
                              "\nNewest version online:" +
                              dsd.Version;

            if (_sd.AppVersion >= dsd.Version)
            {
                MessageBox.Show(
                    versionS + "\n\nNo update required, you already have an up to date version of " + _sd.FormTitle,
                    "No action required");
                return;
            }

            if (String.IsNullOrEmpty(dsd.ChangeLog) == false)
            {
                dsd.ChangeLog = StringExtras.ApplyTrim(dsd.ChangeLog, true, 500);
                versionS += "\n\nCHANGELOG:\n" + dsd.ChangeLog;
            }
            else
            {
                MessageBox.Show("Error while getting updates");
                return;
            }

            DialogResult dr =
                MessageBox.Show(
                    versionS +
                    "\n\nDo you wish to update to this version? \n(Be aware that this program will restart; please save your data beforehand)",
                    "Do you wish to update?", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
                UpdateApplication(dsd);
        }


        private static void UpdateApplication(DownloadedSolutionDetails dsd)
        {
            String folder;
            String localfile;
            //we need the exe file for later execution
            string exefile = "";
            string exefolder;

            try
            {
                //0: reset current directory in case it was changed
                Directory.SetCurrentDirectory(Application.StartupPath);
                //1: Get the online files
                folder = _sd.FormTitle + "v" + DateTime.Now.Ticks;
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
                localfile = StringExtras.RandomString(10);
                client.DownloadFile(dsd.ZipFileLocation, localfile);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while downloading new files\n:" + ex);
                return;
            }

            try
            {
                string myname = AppDomain.CurrentDomain.FriendlyName;
                const string vshost = ".vshost.";
                //remove .vshost for testing
                if (myname.Contains(vshost))
                    myname = myname.Replace(vshost, ".");

                //2.1 unpack
                ZipExtras.ExtractZipFile(localfile, folder);

                //2.2 find exe
                foreach (string f in DirectoryExtras.GetFilesRecursive(folder))
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
                    int c = StringExtras.ContainsSubStringCount(exefile, br);
                    if (c > 0)
                    {
                        string s = exefile.Substring(0, exefile.IndexOf(br, StringComparison.Ordinal));
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

            //3: Run async cmd prompt to move unpacked files and remove the folder in a second, and rerun the exe
            OperatingSystem osInfo = Environment.OSVersion;
            try
            {
                //move folder/* to the local dir
                //delete the zip file
                //delete folder remnants
                //start the exefile we found before

                string operations = "move /Y \"" + exefolder + "\"\\* . " +
                                    "& del /Q \"" + localfile + "\" " +
                                    "& rmdir /Q /S \"" + folder + "\" " +
                                    "& start \"\" \"" + exefile + "\" ";

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
            //4: Kill process			
            Application.Exit();
        }

        #region Nested type: SolutionDetails

        public class DownloadedSolutionDetails
        {
            public String ZipFileLocation;
            public String ChangeLog;
            public double Version;

            public DownloadedSolutionDetails()
            {

            }


        }

        public delegate DownloadedSolutionDetails GetDetails();

        public class SolutionDetails
        {
            public String AboutScreenOtherText;
            public double AppVersion;

            public String FormTitle;
            public String HelpText;
            public GetDetails gd;

            public SolutionDetails(GetDetails gd, string helpText = null, string formTitle = null, double appVersion = -1,
                                   string aboutScreenOtherText = null)
            {
                HelpText = helpText;
                FormTitle = formTitle;
                AppVersion = appVersion;
                AboutScreenOtherText = aboutScreenOtherText;
                this.gd = gd;
            }

            public SolutionDetails()
            {

            }
        }

        #endregion
    }
}