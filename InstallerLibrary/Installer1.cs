using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using IWshRuntimeLibrary;
using Newtonsoft.Json;
using File = System.IO.File;

namespace InstallerLibrary
{
    [RunInstaller(true)]
    public partial class Installer1 : Installer
    {

        public Installer1()
        {
            InitializeComponent();

            Committed += Installer1_Committed;
            AfterUninstall += OnAfterUninstall;
        }

        private void OnAfterUninstall(object sender, InstallEventArgs installEventArgs)
        {
        }


        private void Installer1_Committed(object sender, InstallEventArgs e)
        {
            try
            {
                var path = new DirectoryInfo(Context.Parameters["assemblypath"]).Parent;

                if (Directory.Exists(path.FullName + @"\gin-cli\"))
                {
                    var dInfo = new DirectoryInfo(path.FullName + @"\gin-cli\");
                    dInfo.Empty();
                    Directory.Delete(path.FullName + @"\gin-cli\", true);
                }
                Directory.CreateDirectory(path.FullName + @"\dokan\");
                
                //Give the client the ability to register a URL to communicate with the service
                var everyone = new System.Security.Principal.SecurityIdentifier(
                    "S-1-1-0").Translate(typeof(System.Security.Principal.NTAccount)).ToString();
                var procStartInfo = new ProcessStartInfo("cmd.exe",
                    "/C netsh http add urlacl url=http://+:8738/GinService/ user=\\" + everyone + " delegate=yes")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                var process = new Process { StartInfo = procStartInfo };
                var Output = new StringBuilder();
                process.OutputDataReceived += (o, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data))
                        Output.AppendLine(args.Data);
                };
                Output.Clear();
                process.Start();
                process.BeginOutputReadLine();
                process.WaitForExit();

                //Give the user the ability to register the service URL
                procStartInfo = new ProcessStartInfo("cmd.exe",
                    "/C netsh http add urlacl url=http://+:8733/GinService/ user=\\" + everyone + " delegate=yes")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                process = new Process { StartInfo = procStartInfo };
                Output = new StringBuilder();
                process.OutputDataReceived += (o, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data))
                        Output.AppendLine(args.Data);
                };
                Output.Clear();
                process.Start();
                process.BeginOutputReadLine();
                process.WaitForExit();

                Output.Clear();

                //Create shortcuts in the Startup and Start Menu folders
                var wsh = new WshShell();
                var shortcut = wsh.CreateShortcut(
                        Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup) + @"\GinClientApp.lnk") as
                    IWshShortcut;
                shortcut.Arguments = "";
                shortcut.TargetPath = path.FullName + @"\GinClientApp.exe";
                shortcut.WindowStyle = 1;
                shortcut.Description = "Gin Client for Windows";
                shortcut.WorkingDirectory = path.FullName;
                shortcut.IconLocation = path.FullName + @"\gin_icon.ico";
                shortcut.Save();

                if (!Directory.Exists(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu) + @"\G-Node\"))
                    Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu) +
                                              @"\G-Node\");

                wsh = new WshShell();
                shortcut = wsh.CreateShortcut(
                        Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu) +
                        @"\G-Node\GinClientApp.lnk") as
                    IWshShortcut;
                shortcut.Arguments = "";
                shortcut.TargetPath = path.FullName + @"\GinClientApp.exe";
                shortcut.WindowStyle = 1;
                shortcut.Description = "Gin Client for Windows";
                shortcut.WorkingDirectory = path.FullName;
                shortcut.IconLocation = path.FullName + @"\gin_icon.ico";
                shortcut.Save();
            }
            catch (Exception exc)
            {
                ///print error into json in appdata folder
                var fs = File.CreateText(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\g-node\WinGIN\InstallError.json");
                fs.Write(JsonConvert.SerializeObject(exc));
                fs.Flush();
                fs.Close();
                throw new InstallException("Installation failed. Please check your internet connection and try it later.");
            }
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
        }

        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);
        }

        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
        }

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
        }
    }

    public static class DirectoryInfoExtension
    {
        public static void Empty(this DirectoryInfo directory)
        {
            File.SetAttributes(directory.FullName,
                File.GetAttributes(directory.FullName) & ~(FileAttributes.Hidden | FileAttributes.ReadOnly));

            foreach (var file in directory.GetFiles("*", SearchOption.AllDirectories))
                try
                {
                    File.SetAttributes(file.FullName, FileAttributes.Normal);
                    file.Delete();
                }
                catch
                {
                }

            foreach (var subDirectory in directory.GetDirectories())
                try
                {
                    File.SetAttributes(subDirectory.FullName, FileAttributes.Normal);
                    subDirectory.Delete(true);
                }
                catch
                {
                }
        }

        public static bool IsEmpty(this DirectoryInfo directory)
        {
            if (!Directory.Exists(directory.FullName))
                return true;

            return !Directory.EnumerateFileSystemEntries(directory.FullName).Any();
        }

        public static bool IsEqualTo(this DirectoryInfo left, DirectoryInfo right)
        {
            return string.Equals(left.FullName, right.FullName, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}