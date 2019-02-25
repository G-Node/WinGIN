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
        static readonly Mutex Mutex = new Mutex(true, "{AC8AB48D-C289-445D-B1EB-ABCFF24443ED}" + Environment.UserName);
        private static readonly DirectoryInfo UpdaterBaseDirectory = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\g-node\GinWindowsClient\Updates\");
        private static readonly string AppVeyorProjectUrl = "https://web.gin.g-node.org/G-Node/gin-ui-installers/raw/master/build.json";
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

            var wb = new WebClient();
            try
            {
                ///download build ressult from GIN-installers-repository
                var response = wb.DownloadString(new Uri(AppVeyorProjectUrl));
                var rootObject = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(response);
                var remoteVersion = new Version(rootObject.build.version);
                ///get local assembly version
                var assemblyVer = Assembly.GetExecutingAssembly().GetName().Version;
                var verResult = remoteVersion.CompareTo(assemblyVer);
                if (verResult > 0)
                {
                    var result = System.Windows.MessageBox.Show(
                        "A new version " + remoteVersion + " of the Gin client is available. Do you want to update now?",
                        "Gin Windows Client", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        if (!Directory.Exists(UpdaterBaseDirectory.FullName))
                            Directory.CreateDirectory(UpdaterBaseDirectory.FullName);

                        File.Copy("Updater.exe", UpdaterBaseDirectory + "Updater.exe", true);
                        File.Copy("Newtonsoft.Json.dll", UpdaterBaseDirectory + "Newtonsoft.Json.dll", true);
                        var psInfo = new ProcessStartInfo();
                        psInfo.FileName = UpdaterBaseDirectory + "Updater.exe";
                        Process.Start(psInfo);
                        Environment.Exit(0);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Cannot connect to GNode server.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!Mutex.WaitOne(TimeSpan.Zero, true))
            {
                MessageBox.Show("GIN client is already running.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var curPath = AppDomain.CurrentDomain.BaseDirectory;
            ///check if dokan is installed
            if (!CheckInstalled("Dokan Library 1.1.0.2000 Bundle"))
            {
                var result = MessageBox.Show(
                       "Dokan library is missing! Please install Dokan. Do you want to install Dokan now?",
                        "Gin Windows Client", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                ///try to install dokan
                if (result == MessageBoxResult.Yes)
                {                
                    var procstartinfo = new ProcessStartInfo();
                    procstartinfo.FileName = curPath + @"dokan/DokanSetup.exe";
                    procstartinfo.CreateNoWindow = true;
                    procstartinfo.UseShellExecute = true;
                    procstartinfo.Verb = "runas";
                    var process = Process.Start(procstartinfo);
                    Environment.Exit(0);
                }
                else
                {
                    ///no dokan installed, exit app
                    return;
                }
            }
            if (!File.Exists(curPath+@"gin-cli/bin/gin.exe")) {
                var result = MessageBox.Show(
                       "GIN binary is missing. Please reinstall application.",
                        "Gin Windows Client", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            var path = AppDomain.CurrentDomain.BaseDirectory;

            var value = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process);
            value += ";" + path + @"gin-cli\bin";
            value += ";" + path + @"gin-cli\git\usr\bin";
            value += ";" + path + @"gin-cli\git\bin";
            Environment.SetEnvironmentVariable("PATH", value, EnvironmentVariableTarget.Process);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
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
                    {
                        return true;
                    }
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
                    {
                        return true;
                    }
                }
                key.Close();
            }
            ///not found
            return false;
        }
    }


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

}