using GinClientApp.Dialogs;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows;
using Application = System.Windows.Forms.Application;

namespace GinClientApp
{
    internal static class Program
    {
        /// <summary>
        /// mutex that provides one instance of WinGIN per user
        /// </summary>
        static readonly Mutex Mutex = new Mutex(true, "{AC8AB48D-C289-445D-B1EB-ABCFF24443ED}" + Environment.UserName);
        /// <summary>
        /// Directory for updater; latest msi is downloaded there
        /// </summary>
        private static readonly DirectoryInfo UpdaterBaseDirectory = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\g-node\WinGIN\Updates\");
        /// <summary>
        /// Link to AppVeyor project json; used to check latest released version: version.subversion.build
        /// </summary>
        private static readonly string AppVeyorProjectUrl = "https://web.gin.g-node.org/G-Node/wingin-installers/raw/master/build.json";

        #region Dokan Versions
        /// <summary>
        /// unsupported dokan versions from old WinGIN bundles
        /// </summary>
        private const string dokanAppOld = "Dokan Library 1.1.0.2000 Bundle";
        private const string dokanAppOld2 = "Dokan Library 1.3.0.1000 Bundle";
        private const string dokanAppOld3 = "Dokan Library 1.3.1.1000 Bundle";
        private static readonly List<string> oldDokanList = new List<string>(new string[] { dokanAppOld, dokanAppOld2, dokanAppOld3 });
        /// <summary>
        /// supported dokan version
        /// </summary>
        private const string dokanApp = "Dokan Library 1.4.0.1000 Bundle";
        /// <summary>
        /// supported dokan version
        /// </summary>
        #endregion

        #region Forms Strings
        /// <summary>
        /// error messages
        /// </summary>
        private const string connectionError = "Cannot connect to G-Node server.";
        private const string dokanNotInstalled = "Dokan library is missing or wrong version is installed! Dokan is necessary for WinGIN to work. Do you want to install Dokan 1.4.0 now?";
        private const string oldDokanInstalled = "Old Dokan library is installed! Dokan 1.4.0 is necessary for WinGIN to work. Please uninstall old version.";
        private const string ginNotInstalled = "Local GIN binary is missing. Please reinstall application.";
        private const string winginIsRunning = "WinGIN is already running.";
        #endregion

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                if (args[0] == "-uninstall")
                {
                    Installer.DoUninstall();
                    return;
                }
            }
            if (!Mutex.WaitOne(TimeSpan.Zero, true))
            {
                MessageBox.Show(winginIsRunning, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var wb = new WebClient();
            try
            {
                ///download build result from GIN-installers-repository
                var response = wb.DownloadString(new Uri(AppVeyorProjectUrl));
                var rootObject = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(response);
                var remoteVersion = new Version(rootObject.build.version);
                ///get local assembly version
                var assemblyVer = Assembly.GetExecutingAssembly().GetName().Version;
                ///compare local and latest released version
                var verResult = remoteVersion.CompareTo(assemblyVer);
                if (verResult > 0)
                {
                    ///if new version available ask for installation
                    var result = System.Windows.MessageBox.Show(
                        "A new version " + remoteVersion + " of WinGIN is available. Do you want to update it now?",
                        "WinGIN", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        ///install new version of WinGIN
                        if (!Directory.Exists(UpdaterBaseDirectory.FullName))
                        {
                            Directory.CreateDirectory(UpdaterBaseDirectory.FullName);
                        }
                        File.Copy("Updater.exe", UpdaterBaseDirectory + "Updater.exe", true);
                        File.Copy("Newtonsoft.Json.dll", UpdaterBaseDirectory + "Newtonsoft.Json.dll", true);
                        var psInfo = new ProcessStartInfo
                        {
                            FileName = UpdaterBaseDirectory + "Updater.exe"
                        };
                        Process.Start(psInfo);
                        Environment.Exit(0);
                    }
                }
            }
            catch
            {
                ///connection issue; location change;
                MessageBox.Show(connectionError, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var curPath = AppDomain.CurrentDomain.BaseDirectory;
            ///check if supported dokan is installed
            MessageBoxResult dokanResult = MessageBoxResult.No;
            if (!CheckInstalled(dokanApp))
            {
                /// no supported dokan installed
                foreach (string dokan in oldDokanList)
                {
                    ///check if old version is installed
                    if (CheckInstalled(dokan))
                    {
                        ///installed old dokan. Show warning and exit.
                        MessageBox.Show(oldDokanInstalled, "WinGIN", MessageBoxButton.OK, MessageBoxImage.Error);
                        dokanResult = MessageBoxResult.No;
                        return;
                    }
                }
                {
                    ///No dokan installed,  ask for installation
                    dokanResult = MessageBox.Show(dokanNotInstalled, "WinGIN", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (dokanResult == MessageBoxResult.Yes)
                    {
                        ///try to install dokan
                        var procstartinfo = new ProcessStartInfo
                        {
                            FileName = curPath + @"dokan/DokanSetup.exe",
                            CreateNoWindow = true,
                            UseShellExecute = true,
                            Verb = "runas"
                        };
                        var process = Process.Start(procstartinfo);
                        Environment.Exit(0);
                    }
                    else
                    {
                        ///no dokan installed, exit application
                        return;
                    }
                }
            }
            ///check if local gin-cli is present
            
            if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\g-node\WinGIN\gin-cli\bin\gin.exe"))
            {
                GinCliDownloadDlg dlg = new GinCliDownloadDlg();
                dlg.ShowDialog();
            }
            ///add gin-cli to path
            var path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\g-node\WinGIN\";
            var value = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process);
            value = path + @"gin-cli\bin" + ";" + value;
            value = path + @"gin-cli\git\usr\bin" + ";" + value;
            value = path + @"gin-cli\git\bin" + ";" + value;
            Environment.SetEnvironmentVariable("PATH", value, EnvironmentVariableTarget.Process);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ///start WinGIN
            Application.Run(new GinApplicationContext());
        }

        /// <summary>
        /// Checks if application with provided name is installed
        /// </summary>
        /// <param name="c_name">name of application</param>
        /// <returns>true for installed</returns>
        public static bool CheckInstalled(string c_name)
        {
            string displayName;
            ///32bit installations
            string registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKey);
            if (key != null)
            {
                foreach (RegistryKey subkey in key.GetSubKeyNames().Select(keyName => key.OpenSubKey(keyName)))
                {
                    displayName = subkey.GetValue("DisplayName") as string;
                    if (displayName != null && displayName.Contains(c_name))
                        return true;
                }
                key.Close();
            }
            ///64bit installations
            registryKey = @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall";
            key = Registry.LocalMachine.OpenSubKey(registryKey);
            if (key != null)
            {
                foreach (RegistryKey subkey in key.GetSubKeyNames().Select(keyName => key.OpenSubKey(keyName)))
                {
                    displayName = subkey.GetValue("DisplayName") as string;
                    if (displayName != null && displayName.Contains(c_name))
                        return true;
                }
                key.Close();
            }
            ///not found
            return false;
        }
    }

    #region structures
    public class NuGetFeed
    {
        public string id { get; set; }
        public string name { get; set; }
        public int accountId { get; set; }
        public int projectId { get; set; }
        public bool isPrivateProject { get; set; }
        public bool publishingEnabled { get; set; }
        public DateTime created { get; set; }
    }

    public class AccessRightDefinition
    {
        public string name { get; set; }
        public string description { get; set; }
    }

    public class AccessRight
    {
        public string name { get; set; }
        public bool allowed { get; set; }
    }

    public class RoleAce
    {
        public int roleId { get; set; }
        public string name { get; set; }
        public bool isAdmin { get; set; }
        public List<AccessRight> accessRights { get; set; }
    }

    public class SecurityDescriptor
    {
        public List<AccessRightDefinition> accessRightDefinitions { get; set; }
        public List<RoleAce> roleAces { get; set; }
    }

    public class Project
    {
        public int projectId { get; set; }
        public int accountId { get; set; }
        public string accountName { get; set; }
        public List<object> builds { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public string repositoryType { get; set; }
        public string repositoryScm { get; set; }
        public string repositoryName { get; set; }
        public string repositoryBranch { get; set; }
        public bool isPrivate { get; set; }
        public bool skipBranchesWithoutAppveyorYml { get; set; }
        public bool enableSecureVariablesInPullRequests { get; set; }
        public bool enableSecureVariablesInPullRequestsFromSameRepo { get; set; }
        public bool enableDeploymentInPullRequests { get; set; }
        public bool saveBuildCacheInPullRequests { get; set; }
        public bool rollingBuilds { get; set; }
        public bool rollingBuildsDoNotCancelRunningBuilds { get; set; }
        public bool rollingBuildsOnlyForPullRequests { get; set; }
        public bool alwaysBuildClosedPullRequests { get; set; }
        public string tags { get; set; }
        public NuGetFeed nuGetFeed { get; set; }
        public SecurityDescriptor securityDescriptor { get; set; }
        public DateTime created { get; set; }
        public DateTime updated { get; set; }
    }

    public class Job
    {
        public string jobId { get; set; }
        public string name { get; set; }
        public string osType { get; set; }
        public bool allowFailure { get; set; }
        public int messagesCount { get; set; }
        public int compilationMessagesCount { get; set; }
        public int compilationErrorsCount { get; set; }
        public int compilationWarningsCount { get; set; }
        public int testsCount { get; set; }
        public int passedTestsCount { get; set; }
        public int failedTestsCount { get; set; }
        public int artifactsCount { get; set; }
        public string status { get; set; }
        public DateTime started { get; set; }
        public DateTime finished { get; set; }
        public DateTime created { get; set; }
        public DateTime updated { get; set; }
    }

    public class Build
    {
        public int buildId { get; set; }
        public List<Job> jobs { get; set; }
        public int buildNumber { get; set; }
        public string version { get; set; }
        public string message { get; set; }
        public string messageExtended { get; set; }
        public string branch { get; set; }
        public bool isTag { get; set; }
        public string commitId { get; set; }
        public string authorName { get; set; }
        public string authorUsername { get; set; }
        public string committerName { get; set; }
        public string committerUsername { get; set; }
        public DateTime committed { get; set; }
        public List<object> messages { get; set; }
        public string status { get; set; }
        public DateTime started { get; set; }
        public DateTime finished { get; set; }
        public DateTime created { get; set; }
        public DateTime updated { get; set; }
    }

    public class RootObject
    {
        public Project project { get; set; }
        public Build build { get; set; }
    }
    #endregion
}
