using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using ANDREICSLIB.ClassExtras;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace ANDREICSLIB
{
    public static class Licensing
    {
        private static String Title = "";
        private static double Version;
        private static String OtherText = "";
        private static String VersionPath = "";
        private static String UpdatePath = "";
        private static String ChangelogPath = "";
        private static String HelpText = "";
        /// <summary>
        /// so we dont allow 2 instances of the about dialog
        /// </summary>
        internal static bool ShowingAbout;
        /// <summary>
        /// so we dont allow 2 instances of the help dialog
        /// </summary>
        internal static bool ShowingHelp;

        private static ToolStripMenuItem GetItem(String text, MenuStrip ms)
        {
            ToolStripMenuItem helpToolStripItem = null;

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

            return helpToolStripItem;
        }

        private static void AddHelpOption(String helptext, ToolStripMenuItem helpParent)
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
        /// <param name="helptext"></param>
        /// <param name="newFormTitle"></param>
        /// <param name="appVersion"></param>
        /// <param name="aboutScreenOtherText"></param>
        /// <param name="versionPath"></param>
        /// <param name="applicationZIPFileLocation"></param>
        /// <param name="changelogPath"></param>
        /// <param name="ms"></param>
        public static void CreateLicense(Form baseform, String helptext, String newFormTitle, double appVersion, String aboutScreenOtherText,
                         String versionPath, String applicationZIPFileLocation, String changelogPath, MenuStrip ms)
        {
            Title = newFormTitle;
            Version = appVersion;
            OtherText = aboutScreenOtherText;
            VersionPath = versionPath;
            UpdatePath = applicationZIPFileLocation;
            ChangelogPath = changelogPath;
            HelpText = helptext;

            baseform.Text = Title + " Version:" + Version;

            ToolStripMenuItem helpToolStripItem;

            var existed = false;
            const string help = "&Help";

            helpToolStripItem = GetItem(help, ms);
            if (helpToolStripItem != null)
                existed = true;
            else
            //if it doesnt, create
            {
                ms.Items.Add(new ToolStripMenuItem(help));
                //should always be set now
                helpToolStripItem = GetItem(help, ms);
            }

            //add the help window
            if (HelpText.Length > 0)
            {
                AddHelpOption(helptext, helpToolStripItem);
            }

            //check for updates button
            var updateitem = new ToolStripMenuItem("&Check For Updates");
            updateitem.Click += UpdateApplication;
            helpToolStripItem.DropDownItems.Add(updateitem);

            //about item
            var aboutitem = new ToolStripMenuItem("&About");
            aboutitem.Click += aboutbox;
            helpToolStripItem.DropDownItems.Add(aboutitem);

            //add all the items to the menu if help didnt exist
            if (existed == false)
            {
                ms.Items.Add(helpToolStripItem);
            }
        }

        private static void aboutbox(object sender, EventArgs e)
        {
            var AS = new aboutScreen
                        {
                            Text = "About " + Title,
                            appversionlabel = { Text = "Version " + Version },
                            apptitlelabel = { Text = Title },
                            otherapptext = { Text = OtherText }
                        };

            if (ShowingAbout == false)
            {
                ShowingAbout = true;
                AS.ShowDialog();
            }
        }

        private static void Helpbox(object sender, EventArgs e)
        {
            var hs = new helpScreen { Text = Title + " Help", helpbox = { Text = HelpText } };

            if (ShowingHelp == false)
            {
                ShowingHelp = true;
                hs.Show();
            }
        }

        private static void UpdateApplication(object sender, EventArgs e)
        {
            var dr1 =
                MessageBox.Show(Title + " will now connect to the internet to find  the newest version.\nDo you wish to continue?",
                                "Notification", MessageBoxButtons.YesNo);
            if (dr1 == DialogResult.No)
                return;

            var result = Net.DownloadWebPage(VersionPath);
            if (String.IsNullOrEmpty(result))
            {
                MessageBox.Show("Error while getting new version file:" + VersionPath);
                return;
            }

            double newV;
            try
            {
                newV = double.Parse(result);
            }
            catch
            {
                MessageBox.Show("Online file:" + VersionPath + " has an invalid version:" + result);
                return;
            }

            var versionS = "Your version of " + Title + ":" + Version.ToString() + "\nNewest version online:" +
                              newV.ToString();

            if (Version >= newV)
            {
                MessageBox.Show(versionS + "\n\nNo update required, you already have an up to date version of " + Title,
                                "No action required");
                return;
            }

            var changelog = Net.DownloadWebPage(ChangelogPath);
            if (String.IsNullOrEmpty(changelog) == false)
            {
                changelog = StringUpdates.applyTrim(changelog, true, 500);
                versionS += "\n\nCHANGELOG:\n" + changelog;
            }
            else
            {
                MessageBox.Show("Error while getting updates");
                return;
            }

            var dr =
                MessageBox.Show(
                    versionS +
                    "\n\nDo you wish to update to this version? \n(Be aware that this program will restart; please save your data beforehand)",
                    "Do you wish to update?", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
                UpdateApplication(newV);
        }



        private static void UpdateApplication(double newVersion)
        {
            String folder;
            String localfile;
            var buffer = new byte[4096]; // 4K is optimum
            //we need the exe file for later execution
            var exefile = "";
            var exefolder = "";

            try
            {
                //0: reset current directory in case it was changed
                Directory.SetCurrentDirectory(Application.StartupPath);
                //1: Get the online files
                folder = Title + "v" + newVersion.ToString();
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
                localfile = UpdatePath.Substring(UpdatePath.LastIndexOf('/') + 1);
                client.DownloadFile(UpdatePath, localfile);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while downloading new files\n:" + ex);
                return;
            }

            try
            {
                var myname = AppDomain.CurrentDomain.FriendlyName;
                //remove .vshost for testing
                if (myname.Contains(".vshost."))
                    myname = myname.Replace(".vshost.", ".");
               
                //2.1 unpack
                Zip.ExtractZipFile(localfile, folder);

                //2.2 find exe
                foreach (var f in DirectoryUpdates.GetFilesRecursive(folder))
                {
                    if (f.Contains(".exe"))
                        exefile = f;

                    if (f.EndsWith(myname))
                        break;
                }

                var br = "\\";
                exefolder = "";
                //ignore everything above this dir
                while (exefile.Length > 0)
                {
                    var c = StringUpdates.ContainsSubStringCount(exefile, br);
                    if (c > 0)
                    {
                        var s = exefile.Substring(0, exefile.IndexOf(br));
                        exefolder += s + br;
                        exefile = exefile.Substring(exefile.IndexOf(br) + 1);
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
            var osInfo = Environment.OSVersion;
            try
            {
                //move folder/* to the local dir
                //delete the zip file
                //delete folder remnants
                //start the exefile we found before

                var operations = "move /Y \"" + exefolder + "\"\\* . " +
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
    }
}