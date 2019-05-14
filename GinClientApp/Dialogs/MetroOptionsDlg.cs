using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using GinClientApp.Properties;
using GinClientLibrary;
using MetroFramework;
using MetroFramework.Controls;
using MetroFramework.Forms;
using Newtonsoft.Json;

namespace GinClientApp.Dialogs
{
    public partial class MetroOptionsDlg : MetroForm
    {
        public enum Page
        {
            Login = 0,
            GlobalOptions,
            Repositories,
            About
        }
        public event EventHandler RepoListingChanged;
        private readonly GinApplicationContext _parentContext;
        private readonly UserCredentials _storedCredentials;
        private readonly GlobalOptions _storedOptions;
        private Dictionary<string, ServerConf> serverMap;
        private BindingSource bs;

        protected virtual void OnRepoListingChanged()
        {
            RepoListingChanged?.Invoke(this, EventArgs.Empty);
        }
        private void serverChanged(object sender, EventArgs e)
        {
           
            var serv = mCBxServer.Text;
            var logins = UserCredentials.Instance.loginList;
            var selectedLogin = logins.Find(x => x.Server.Equals(serv));

            mTBAlias.Text = serv;
            if (selectedLogin != null)
            {
                mTxBUsername.Text = selectedLogin.Username;
                mTxBPassword.Text = selectedLogin.Password;
            }
            else
            {
                mTxBUsername.Text = "";
                mTxBPassword.Text = "";
            }
        }

        private void userOrPassChanged()
        {
            var logins = UserCredentials.Instance.loginList;
            var selectedLogin = logins.Find(x => x.Server == mTBAlias.Text);
            if (selectedLogin != null)
            {
                selectedLogin.Password = mTxBPassword.Text;
                selectedLogin.Username = mTxBUsername.Text;
            }
            else
            {
                var login = new UserCredentials.LoginSettings();
                login.Server = mTBAlias.Text;
                login.Password = mTxBPassword.Text;
                login.Username = mTxBUsername.Text;
                UserCredentials.Instance.loginList.Add(login);
            }
        }

        public MetroOptionsDlg(GinApplicationContext parentContext, Page startPage)
        {
            InitializeComponent();

            _parentContext = parentContext;

            mTabCtrl.SelectTab((int)startPage);

            mLblStatus.Visible = false;
            mLblWorking.Visible = false;
            mProgWorking.Visible = false;
            serverMap = GetServers();
            bs = new BindingSource(serverMap, null);
            mCBxServer.DataSource = bs;
            mCBxServer.DisplayMember = "Key";
            /*
            mTxBUsername.DataBindings.Add("Text", UserCredentials.Instance.loginList, "Username");
            mTxBPassword.DataBindings.Add("Text", UserCredentials.Instance.loginList, "Password");
            mTBAlias.DataBindings.Add("Text", UserCredentials.Instance.loginList, "Server");
            */
            mTBAlias.Text = mCBxServer.Text;
            var logins = UserCredentials.Instance.loginList;
            var selectedLogin = logins.Find(x => x.Server == mTBAlias.Text);
            mTxBPassword.Text = selectedLogin.Password;
            mTxBUsername.Text = selectedLogin.Username;

            mTxBDefaultCheckout.Text = GlobalOptions.Instance.DefaultCheckoutDir.FullName;
            mTxBDefaultMountpoint.Text = GlobalOptions.Instance.DefaultMountpointDir.FullName;
            mTglDownloadAnnex.Checked = GlobalOptions.Instance.RepositoryCheckoutOption ==
                                        GlobalOptions.CheckoutOption.FullCheckout;
            switch (GlobalOptions.Instance.RepositoryUpdateInterval)
            {
                case 0:
                    mCBxRepoUpdates.SelectedIndex = 0;
                    break;
                case 5:
                    mCBxRepoUpdates.SelectedIndex = 1;
                    break;
                case 15:
                    mCBxRepoUpdates.SelectedIndex = 2;
                    break;
                case 30:
                    mCBxRepoUpdates.SelectedIndex = 3;
                    break;
                case 60:
                    mCBxRepoUpdates.SelectedIndex = 4;
                    break;
                default:
                    mCBxRepoUpdates.SelectedIndex = 0;
                    break;
            }

            FillRepoList();

            mTxBLicense.Text = Resources.License_Text;
            mTxBGinCliVersion.Text = parentContext.ServiceClient.GetGinCliVersion()+"\n WinGIN version "+ Assembly.GetExecutingAssembly().GetName().Version;

            _storedOptions = (GlobalOptions) GlobalOptions.Instance.Clone();
            _storedCredentials = (UserCredentials) UserCredentials.Instance.Clone();
        }

        public void SetTab(Page page)
        {
            mTabCtrl.SelectTab((int) page);
        }

        private Dictionary<string, ServerConf> GetServers()
        {
            string serverJson = _parentContext.ServiceClient.GetServers();
            var map = JsonConvert.DeserializeObject<Dictionary<string,ServerConf>>(serverJson);
            return map;
        }

        private void FillRepoList()
        {
            mLVwRepositories.Items.Clear();
            var repos = JsonConvert.DeserializeObject<GinRepositoryData[]>(_parentContext.ServiceClient
                .GetRepositoryList());
            foreach (var repo in repos)
                mLVwRepositories.Items.Add(new ListViewItem(new[]
                    {repo.Name, repo.Mountpoint.FullName, repo.PhysicalDirectory.FullName, repo.Address}));
            if (repos.Length > 1)
            {
                mLVwRepositories.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            }
            else
            {
                mLVwRepositories.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
            OnRepoListingChanged();
        }


        private void mBtnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            GlobalOptions.Save();
            UserCredentials.Save();
            SaveRepoList();
            Close();
        }

        private void UpdateDefaultdir(ref DirectoryInfo directory, MetroTextBox txtBox)
        {
            var dlg = new FolderBrowserDialog
            {
                SelectedPath = directory.FullName
            };

            if (dlg.ShowDialog() == DialogResult.OK)
                directory = new DirectoryInfo(dlg.SelectedPath);

            txtBox.Text = directory.FullName;
        }

        private void mBtnPickDefaultCheckoutDir_Click(object sender, EventArgs e)
        {
            var directory = GlobalOptions.Instance.DefaultCheckoutDir;
            UpdateDefaultdir(ref directory, mTxBDefaultCheckout);
            GlobalOptions.Instance.DefaultCheckoutDir = directory;
        }

        private void ClickEditServer(object sender, EventArgs e)
        {
            var editSvrForm = new EditServerForm(_parentContext)
            {
                ServerDic = serverMap

            };
            editSvrForm.ShowDialog();
            serverMap = GetServers();
            bs.ResetBindings(false);
        }
        /// <summary>
        /// open ServerAddDlg to get necessary information about server
        /// all informations are saved in public strings alias, web, git
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickAddServer(object sender, EventArgs e)
        {
            var svrForm = new ServerForm();
            svrForm.ShowDialog();
            var result = svrForm.DialogResult;
            if (result == DialogResult.OK)
            {
                AddNewServer(svrForm.alias, svrForm.web, svrForm.git);
                serverMap = GetServers();
            }
            else
            {
                ///do nothing
            }
            bs.ResetBindings(false);
        }

        private bool AddNewServer(string serverAlias, string webConfiguration, string gitConfiguration)
        {
            ///get dictionary with servers
            serverMap = GetServers();
            ///check if alias exists
            if (serverMap.ContainsKey(serverAlias))
            {
                MessageBox.Show("Server with this alias already exist!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else
            {
                if (_parentContext.ServiceClient.NewServer(serverAlias, webConfiguration, gitConfiguration))
                {
                    MessageBox.Show("Server added.");
                }
                else
                {
                    MessageBox.Show("Error.");
                }
            }
            return true;
        }

        private void mBtnPickDefaultMountpointDir_Click(object sender, EventArgs e)
        {
            var directory = GlobalOptions.Instance.DefaultMountpointDir;
            UpdateDefaultdir(ref directory, mTxBDefaultMountpoint);
            GlobalOptions.Instance.DefaultMountpointDir = directory;
        }

        private void mTglDownloadAnnex_CheckedChanged(object sender, EventArgs e)
        {
            GlobalOptions.Instance.RepositoryCheckoutOption = mTglDownloadAnnex.Checked
                ? GlobalOptions.CheckoutOption.FullCheckout
                : GlobalOptions.CheckoutOption.AnnexCheckout;
        }

        private void mBtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;

            GlobalOptions.Instance = _storedOptions;
            UserCredentials.Instance = _storedCredentials;

            GlobalOptions.Save();
            UserCredentials.Save();

            Close();
        }

        private async void mBtnCheckout_Click(object sender, EventArgs e)
        {
            var repoData = new GinRepositoryData(GlobalOptions.Instance.DefaultCheckoutDir,
                GlobalOptions.Instance.DefaultMountpointDir, "", "", false);

            var createNewDlg = new MetroCreateNewRepoDlg(repoData, _parentContext);

            if (createNewDlg.ShowDialog() == DialogResult.Cancel) return;

            repoData = createNewDlg.RepositoryData;
            StartShowProgress();

            if (repoData.CreateNew)
            {
                await _parentContext.ServiceClient.CreateNewRepositoryAsync(repoData.Name);
                await _parentContext.ServiceClient.AddRepositoryAsync(repoData.PhysicalDirectory.FullName,
                repoData.Mountpoint.FullName, repoData.Name, repoData.Address,
                GlobalOptions.Instance.RepositoryCheckoutOption == GlobalOptions.CheckoutOption.FullCheckout,
                repoData.CreateNew);
            }
            else
            {
                await _parentContext.ServiceClient.AddRepositoryAsync(repoData.PhysicalDirectory.FullName,
                    repoData.Mountpoint.FullName, repoData.Name, repoData.Address,
                    GlobalOptions.Instance.RepositoryCheckoutOption == GlobalOptions.CheckoutOption.FullCheckout,
                    repoData.CreateNew);
            }

            StopShowProgress();

            FillRepoList();

            SaveRepoList();
        }

        private void SaveRepoList()
        {
            var repos = JsonConvert.DeserializeObject<GinRepositoryData[]>(_parentContext.ServiceClient
                .GetRepositoryList());

            var saveFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                           @"\g-node\GinWindowsClient\SavedRepositories.json";

            if (!Directory.Exists(Path.GetDirectoryName(saveFile)))
                Directory.CreateDirectory(Path.GetDirectoryName(saveFile));

            if (File.Exists(saveFile))
                File.Delete(saveFile);


            var fs = File.CreateText(saveFile);
            fs.Write(JsonConvert.SerializeObject(repos));
            fs.Flush();
            fs.Close();
        }

        private async void mBtnCreateNew_Click(object sender, EventArgs e)
        {
            var repoData = new GinRepositoryData(GlobalOptions.Instance.DefaultCheckoutDir,
                GlobalOptions.Instance.DefaultMountpointDir, "", "", true);

            var createNewDlg = new MetroCreateNewRepoDlg(repoData, _parentContext);

            if (createNewDlg.ShowDialog() == DialogResult.Cancel) return;

            repoData = createNewDlg.RepositoryData;
            StartShowProgress();

            await _parentContext.ServiceClient.CreateNewRepositoryAsync(repoData.Name);
            await _parentContext.ServiceClient.AddRepositoryAsync(repoData.PhysicalDirectory.FullName,
                repoData.Mountpoint.FullName, repoData.Name, repoData.Address,
                GlobalOptions.Instance.RepositoryCheckoutOption == GlobalOptions.CheckoutOption.FullCheckout,
                repoData.CreateNew);

            StopShowProgress();

            FillRepoList();
        }

        private void mBtnRemove_Click(object sender, EventArgs e)
        {
            if (mLVwRepositories.SelectedItems.Count == 0) return;

            var repo = mLVwRepositories.SelectedItems[0].SubItems[0].Text;
            var res = MetroMessageBox.Show(this,
                string.Format(Resources.Options_This_will_delete_the_repository, repo),
                Resources.GinClientApp_Gin_Client_Warning, MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
            if (res == DialogResult.Cancel) return;

            _parentContext.ServiceClient.DeleteRepository(repo);

            FillRepoList();
        }

        private void StartShowProgress()
        {
            mLblWorking.Visible = true;
            mProgWorking.Visible = true;
            mProgWorking.Spinning = true;

            mBtnOK.Enabled = false;
            mBtnCancel.Enabled = false;
        }

        private void StopShowProgress()
        {
            mLblWorking.Visible = false;
            mProgWorking.Visible = false;
            mProgWorking.Spinning = false;

            mBtnOK.Enabled = true;
            mBtnCancel.Enabled = true;
        }

        private bool AttemptLogin()
        {
            if (string.IsNullOrEmpty(mTxBUsername.Text) || string.IsNullOrEmpty(mTxBPassword.Text)) return false;
            _parentContext.ServiceClient.Logout();

            return _parentContext.ServiceClient.Login(mTxBUsername.Text, mTxBPassword.Text, mTBAlias.Text);
            //return _parentContext.ServiceClient.Login(mTxBUsername.Text, mTxBPassword.Text);
        }

        private void mTxBPassword_Leave(object sender, EventArgs e)
        {
            userOrPassChanged();
            mLblStatus.Visible = false;
            
            if (AttemptLogin()) return;

            mLblStatus.Text = Resources.GetUserCredentials_The_entered_Username_Password_combination_is_invalid;
            mLblStatus.Visible = true;
        }

        private void mTxBUsername_Leave(object sender, EventArgs e)
        {
            mLblStatus.Visible = false;
            userOrPassChanged();

            if (AttemptLogin()) return;

            mLblStatus.Text = Resources.GetUserCredentials_The_entered_Username_Password_combination_is_invalid;
            mLblStatus.Visible = true;
        }

        private void metroComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (mCBxRepoUpdates.SelectedIndex)
            {
                case 0:
                    GlobalOptions.Instance.RepositoryUpdateInterval = 0;
                    break;
                case 1:
                    GlobalOptions.Instance.RepositoryUpdateInterval = 5;
                    break;
                case 2:
                    GlobalOptions.Instance.RepositoryUpdateInterval = 15;
                    break;
                case 3:
                    GlobalOptions.Instance.RepositoryUpdateInterval = 30;
                    break;
                case 4:
                    GlobalOptions.Instance.RepositoryUpdateInterval = 60;
                    break;
                default:
                    GlobalOptions.Instance.RepositoryUpdateInterval = 0;
                    break;
            }
        }

        private void serverDefaultBtn_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Not implemented");
            string defServerAlias = mTBAlias.Text;
        }
    }
}