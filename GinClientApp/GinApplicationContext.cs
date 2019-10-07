using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using GinClientApp.Dialogs;
using GinClientApp.Properties;
using GinClientLibrary;
using GinService;
using Microsoft.Win32;
using Newtonsoft.Json;
using IGinService = GinClientApp.GinService.IGinService;
using Timer = System.Timers.Timer;

namespace GinClientApp
{
    /// <summary>
    ///     The main application context for the WinGIN.
    /// </summary>
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class GinApplicationContext : ApplicationContext
    {
        private readonly NotifyIcon _trayIcon;
        public IGinService ServiceClient;
        private Timer _updateIntervalTimer;
        private Thread _serviceThread;

        public GinApplicationContext()
        {
            _trayIcon = new NotifyIcon
            {
                Visible = true,
                Icon = Resources.gin_icon_desaturated
            };

            var service = new GinClientWindowsService(_trayIcon);

            _serviceThread = new Thread(service.Start);
            _serviceThread.Start();

            SystemEvents.SessionEnded += SystemEvents_SessionEnded;

            var myBinding = new WSDualHttpBinding
            {
                ClientBaseAddress = new Uri(@"http://localhost:8738/GinService/GinUI/" + Environment.UserName),
                MaxBufferPoolSize = int.MaxValue,
                MaxReceivedMessageSize = int.MaxValue,
                OpenTimeout = TimeSpan.FromMinutes(1.0),
                CloseTimeout = TimeSpan.FromMinutes(1.0),
                SendTimeout = TimeSpan.FromHours(1),
                ReceiveTimeout = TimeSpan.FromHours(1),
                ReaderQuotas = new XmlDictionaryReaderQuotas
                {
                    MaxArrayLength = int.MaxValue,
                    MaxBytesPerRead = int.MaxValue,
                    MaxDepth = int.MaxValue,
                    MaxNameTableCharCount = int.MaxValue,
                    MaxStringContentLength = int.MaxValue
                }
            };
            var endpointIdentity = EndpointIdentity.CreateDnsIdentity("localhost");
            var myEndpoint = new EndpointAddress(new Uri("http://localhost:8733/GinService/"), endpointIdentity);

            var myChannelFactory = new ChannelFactory<IGinService>(myBinding, myEndpoint);

            ServiceClient = myChannelFactory.CreateChannel();
            var saveFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                @"\g-node\WinGIN";
            if (!Directory.Exists(saveFilePath))
                Directory.CreateDirectory(saveFilePath);

            #region Environment Variables

            //Tell the service to use the current users' AppData folders for logging and config data
            ServiceClient.SetEnvironmentVariables(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\g-node\gin\",
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\g-node\gin\");

            #endregion

            #region Login

            if (!UserCredentials.Load())
            {
                var getUserCreds = new MetroGetUserCredentialsDlg(this);
                var result = getUserCreds.ShowDialog(); //The Dialog will log us in and save the user credentials

                if (result == DialogResult.Cancel)
                {
                    Exit(this, EventArgs.Empty);
                    return;
                }
            }
            else
            {
                var servString = ServiceClient.GetServers();
                var ServerDic = JsonConvert.DeserializeObject<Dictionary<string, ServerConf>>(servString);
                foreach (var server in ServerDic)
                {
                    ///search for login for server
                    var selectedLogin = UserCredentials.Instance.loginList.Find(x => x.Server == server.Key);
                    if (selectedLogin != null)
                    {
                        ///try login for servers
                        if (!ServiceClient.Login(selectedLogin.Username, selectedLogin.Password, selectedLogin.Server))
                        {
                            if (server.Value.Default)
                            {
                                ///if login fails for default server, try to get new login info
                                MessageBox.Show(Resources.GinApplicationContext_Error_while_trying_to_log_in_to_GIN,
                                Resources.GinApplicationContext_Gin_Client_Error,
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                                var getUserCreds = new MetroGetUserCredentialsDlg(this);
                                var result = getUserCreds.ShowDialog(); ///The Dialog will log us in and save the user credentials
                                if (result == DialogResult.Cancel)
                                {
                                    Exit(this, EventArgs.Empty);
                                    return;
                                }
                            }
                        }
                    }
                }
            }

            UserCredentials.Save();

            #endregion

            #region Read options

            if (!GlobalOptions.Load())
            {
                var optionsDlg = new MetroOptionsDlg(this, MetroOptionsDlg.Page.GlobalOptions);
                optionsDlg.RepoListingChanged += (o, args) => { _trayIcon.ContextMenu = new ContextMenu(BuildContextMenu()); };
                var result = optionsDlg.ShowDialog();

                if (result == DialogResult.Cancel)
                    Exit(this, EventArgs.Empty);
            }

            if (GlobalOptions.Instance.RepositoryUpdateInterval > 0)
            {
                _updateIntervalTimer =
                    new Timer(GlobalOptions.Instance.RepositoryUpdateInterval * 1000 * 60) { AutoReset = true };
                _updateIntervalTimer.Elapsed += (sender, args) => { ServiceClient.DownloadAllUpdateInfo(); };
            }

            GlobalOptions.Save();

            #endregion

            #region Set up repositories

            if (File.Exists(saveFilePath + @"\SavedRepositories.json"))
                try
                {
                    using (var saveFile = File.OpenText(saveFilePath + @"\SavedRepositories.json"))
                    {
                        var text = saveFile.ReadToEnd();
                        var repos = JsonConvert.DeserializeObject<GinRepositoryData[]>(text);

                        foreach (var repo in repos)
                            ServiceClient.AddRepository(repo.PhysicalDirectory.FullName, repo.Mountpoint.FullName,
                                repo.Name,
                                repo.Address,
                                GlobalOptions.Instance.RepositoryCheckoutOption ==
                                GlobalOptions.CheckoutOption.FullCheckout, false);
                    }
                }
                catch
                {
                }
            else
                ManageRepositoriesMenuItemHandler(null, EventArgs.Empty);

            #endregion


            _trayIcon.DoubleClick += _trayIcon_DoubleClick;
            _trayIcon.ContextMenu = new ContextMenu(BuildContextMenu());
            _trayIcon.Icon = Resources.gin_icon;
            _updateIntervalTimer?.Start();

        }

        private void SystemEvents_SessionEnded(object sender, SessionEndedEventArgs e)
        {
            SystemEvents.SessionEnded -= SystemEvents_SessionEnded;

            Exit(this, EventArgs.Empty);
        }

        private MenuItem[] BuildContextMenu()
        {
            var menuitems = new List<MenuItem>();

            var repositories = JsonConvert.DeserializeObject<GinRepositoryData[]>(ServiceClient.GetRepositoryList());
            foreach (var repo in repositories)
            {
                var mitem = new MenuItem(repo.Name) { Tag = repo };
                mitem.MenuItems.Add(Resources.GinApplicationContext_Upload, UploadRepoMenuItemHandler);
                mitem.MenuItems.Add(Resources.GinApplicationContext_Update, UpdateRepoMenuItemHandler);

                menuitems.Add(mitem);
            }

            if (repositories.Length != 0)
                menuitems.Add(new MenuItem("-"));

            menuitems.Add(new MenuItem(Resources.GinApplicationContext_Manage_Repositories,
                ManageRepositoriesMenuItemHandler));
            menuitems.Add(new MenuItem(Resources.GinApplicationContext_Options, ShowOptionsMenuItemHandler));
            menuitems.Add(new MenuItem(Resources.GinApplicationContext_About, ShowAboutMenuItemHandler));
            menuitems.Add(new MenuItem("-"));
            menuitems.Add(new MenuItem(Resources.GinApplicationContext_Exit, Exit));

            return menuitems.ToArray();
        }

        private void ShowAboutMenuItemHandler(object sender, EventArgs e)
        {
            if (Application.OpenForms.OfType<MetroOptionsDlg>().Count() > 0)
            {
                var form = Application.OpenForms.OfType<MetroOptionsDlg>().First();
                form.SetTab(MetroOptionsDlg.Page.About);
            }
            else
            {
                var optionsdlg = new MetroOptionsDlg(this, MetroOptionsDlg.Page.About);
                optionsdlg.RepoListingChanged += (o, args) => { _trayIcon.ContextMenu = new ContextMenu(BuildContextMenu()); };
                optionsdlg.Closed += (o, args) =>
                {
                    if (_trayIcon != null) _trayIcon.ContextMenu = new ContextMenu(BuildContextMenu());
                };
                optionsdlg.Show();
            }
        }

        private void UploadRepoMenuItemHandler(object sender, EventArgs e)
        {
            var repo = (GinRepositoryData)((MenuItem)sender).Parent.Tag;
            var fstatus = JsonConvert.DeserializeObject<
                Dictionary<string, GinRepository.FileStatus>>(ServiceClient.GetRepositoryFileInfo(repo.Name));

            var alteredFiles = from kvp in fstatus
                               where kvp.Value == GinRepository.FileStatus.OnDiskModified ||
                                     kvp.Value == GinRepository.FileStatus.Unknown || kvp.Value == GinRepository.FileStatus.Removed
                               select kvp;

            var files = alteredFiles as KeyValuePair<string, GinRepository.FileStatus>[] ?? alteredFiles.ToArray();
            if (!files.Any())
            {
                ///No new or changed files. Show notification.
                try
                {
                    _trayIcon.ShowBalloonTip(500, "WinGIN", "Nothing to do.", ToolTipIcon.Info);
                }
                catch
                {
                }
                return; //Nothing to upload here
            }

            var uploadfiledlg = new MetroUploadFilesDlg(files);
            var res = uploadfiledlg.ShowDialog();
            string commitMessage = uploadfiledlg.CommitTextBox.Text;
            if (res == DialogResult.Cancel) return;
            if (String.IsNullOrEmpty(commitMessage))
                //WCF requires that non-optional arguments be non-empty, so we provide a placeholder value
                ServiceClient.UploadFile(repo.Name, "%EMPTYSTRING%");
            else
                ServiceClient.UploadFileWithMessage(repo.Name, "%EMPTYSTRING%", commitMessage);
        }

        private void ShowOptionsMenuItemHandler(object sender, EventArgs e)
        {
            if (Application.OpenForms.OfType<MetroOptionsDlg>().Count() > 0)
            {
                var form = Application.OpenForms.OfType<MetroOptionsDlg>().First();
                form.SetTab(MetroOptionsDlg.Page.GlobalOptions);
            }
            else
            {
                var optionsDlg = new MetroOptionsDlg(this, MetroOptionsDlg.Page.GlobalOptions);
                optionsDlg.RepoListingChanged += (o, args) => { _trayIcon.ContextMenu = new ContextMenu(BuildContextMenu()); };
                optionsDlg.Closed += (o, args) =>
                {
                    if (_trayIcon != null) _trayIcon.ContextMenu = new ContextMenu(BuildContextMenu());
                };
                var res = optionsDlg.ShowDialog();

                if (res != DialogResult.OK) return;

                if (GlobalOptions.Instance.RepositoryUpdateInterval <= 0)
                {
                    _updateIntervalTimer?.Stop();
                    return;
                }

                if (_updateIntervalTimer == null)
                {
                    _updateIntervalTimer =
                        new Timer(GlobalOptions.Instance.RepositoryUpdateInterval * 1000) { AutoReset = true };
                    _updateIntervalTimer.Elapsed += (sender1, args) => { ServiceClient.DownloadAllUpdateInfo(); };
                }
                _updateIntervalTimer.Stop();
                _updateIntervalTimer.Interval = GlobalOptions.Instance.RepositoryUpdateInterval * 1000;
                _updateIntervalTimer.Start();
            }
        }

        private void UpdateRepoMenuItemHandler(object sender, EventArgs e)
        {
            var repo = (GinRepositoryData)((MenuItem)sender).Parent.Tag;
            //show status dialog
            ServiceClient.DownloadUpdateInfo(repo.Name);
        }

        private void ManageRepositoriesMenuItemHandler(object sender, EventArgs e)
        {
            if (Application.OpenForms.OfType<MetroOptionsDlg>().Count() > 0)
            {
                var form = Application.OpenForms.OfType<MetroOptionsDlg>().First();
                form.SetTab(MetroOptionsDlg.Page.Repositories);
            }
            else
            {
                var repomanager = new MetroOptionsDlg(this, MetroOptionsDlg.Page.Repositories);
                repomanager.RepoListingChanged += (o, args) => { _trayIcon.ContextMenu = new ContextMenu(BuildContextMenu()); };
                repomanager.Closed += (o, args) =>
                {
                    if (_trayIcon != null) _trayIcon.ContextMenu = new ContextMenu(BuildContextMenu());
                };
                repomanager.ShowDialog();
            }
        }

        private void _trayIcon_DoubleClick(object sender, EventArgs e)
        {
            ///app is not opened yet, open new form
            if (Application.OpenForms.OfType<MetroOptionsDlg>().Count() < 1)
            {
                var repomanager = new MetroOptionsDlg(this, MetroOptionsDlg.Page.Repositories);
                repomanager.RepoListingChanged += (o, args) => { _trayIcon.ContextMenu = new ContextMenu(BuildContextMenu()); };
                repomanager.Closed += (o, args) =>
                {
                    if (_trayIcon != null) _trayIcon.ContextMenu = new ContextMenu(BuildContextMenu());
                };
                repomanager.ShowDialog();
            }
            else
            {
                ///App is already open, bring the app form on top
                var form = Application.OpenForms.OfType<MetroOptionsDlg>().First();
                form.TopMost = true;
                form.Show();
                form.Focus();
                form.BringToFront();
                form.TopMost = false;
            }
        }

        private void Exit(object sender, EventArgs e)
        {
            /// Hide tray icon, otherwise it will remain shown until user mouses over it
            if (_trayIcon != null)
                _trayIcon.Visible = false;

            ServiceClient?.EndSession();

            _serviceThread.Abort();
            _serviceThread.Join();
            Environment.Exit(0);
        }
    }
}