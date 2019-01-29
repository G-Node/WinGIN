using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using GinClientApp.Properties;
using GinClientLibrary;
using GinClientLibrary.Extensions;
using MetroFramework;
using MetroFramework.Forms;
using Newtonsoft.Json;

namespace GinClientApp.Dialogs
{
    /// <summary>
    ///     Dialog to create/check out a new Repository
    /// </summary>
    public partial class MetroCreateNewRepoDlg : MetroForm
    {
        private readonly GinApplicationContext _appContext;
        private bool _createNew;

        public MetroCreateNewRepoDlg(GinRepositoryData data, GinApplicationContext appContext)
        {
            InitializeComponent();

            RepositoryData = data;

            mTxBRepoName.Text = data.Name;
            mTxBRepoAddress.Text = data.Address;
            mTxBRepoCheckoutDir.Text = data.PhysicalDirectory.FullName;
            mTxBRepoMountpoint.Text = data.Mountpoint.FullName;
            mTxBRepoAddress.WaterMark = Resources.Options__username___repository_;
            _createNew = data.CreateNew;

            createNewRepoToolTip.SetToolTip(mBtnRepoBrowser, Resources.CreateNewRepoDlg_Open_the_repository_browser);
            createNewRepoToolTip.SetToolTip(mBtnPickRepoCheckoutDir,
                Resources.MetroCreateNewRepoDlg_Choose_a_directory_for_the_checkout);
            createNewRepoToolTip.SetToolTip(mBtnPickRepoMountpointDir,
                Resources.MetroCreateNewRepoDlg_Choose_a_directory_for_the_mountpoint);

            _appContext = appContext;
        }

        public GinRepositoryData RepositoryData { get; }

        private bool CheckSanity()
        {
           
            var repoListJson = _appContext.ServiceClient.GetRemoteRepositoryList();
            var repoList = JsonConvert.DeserializeObject<RepositoryListing[]>(repoListJson);
            var paths = repoList.Select(repoListing => repoListing.full_name).ToList();

            if (!paths.Exists(path => path == mTxBRepoAddress.Text))
            {
                //Before we give up, check if this is a public, but unlisted repo
                var repoInfoStr = _appContext.ServiceClient.GetRemoteRepositoryInfo(mTxBRepoAddress.Text);
                if (repoInfoStr.StartsWith("Error") && !_createNew)
                {
                    MetroMessageBox.Show(this, Resources.Options_CheckSanity_No_private_or_shared_repos,
                        Resources.GinClientApp_Gin_Client_Warning, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (_createNew && !repoInfoStr.StartsWith("Error"))
                {
                    MetroMessageBox.Show(this, Resources.Options_CheckSanity_A_repository_with_this_name_exists,
                        Resources.GinClientApp_Gin_Client_Warning, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            if (string.IsNullOrEmpty(mTxBRepoAddress.Text))
            {
                MetroMessageBox.Show(this, Resources.Options_CheckSanity_No_checkout_address_has_been_entered_,
                    Resources.GinClientApp_Gin_Client_Warning, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!mTxBRepoAddress.Text.Contains('/') && !RepositoryData.CreateNew)
            {
                var result = MetroMessageBox.Show(this,
                    string.Format(Resources.Options_CheckSanity_The_checkout_address_is_not_properly_formatted,
                        mTxBRepoAddress.Text),
                    Resources.GinClientApp_Gin_Client_Warning, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                RepositoryData.CreateNew = result == DialogResult.Yes;
                RepositoryData.Name = RepositoryData.Address;

                return result == DialogResult.Yes;
            }

            if (Directory.Exists(RepositoryData.PhysicalDirectory.FullName) && Directory.EnumerateFileSystemEntries(RepositoryData.PhysicalDirectory.FullName).Any())
            {
                MetroMessageBox.Show(this, string.Format(Resources.Options_CheckSanity_The_checkout_address_is_not_empty,
                    RepositoryData.PhysicalDirectory.FullName), Resources.GinClientApp_Gin_Client_Warning, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            var mpntUri = new Uri(RepositoryData.Mountpoint.FullName + @"\");
            var pdirUri = new Uri(RepositoryData.PhysicalDirectory.FullName + @"\");

            if (mpntUri.IsBaseOf(pdirUri) || pdirUri.IsBaseOf(mpntUri))
            {
                MetroMessageBox.Show(this,
                    Resources.Options_CheckSanity_The_mountpoint_and_checkout_directory_can_not_be_subdirectories,
                    Resources.GinClientApp_Gin_Client_Warning, MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }

            return true;
        }

        private void mBtnOK_Click(object sender, EventArgs e)
        {
            RepositoryData.PhysicalDirectory =
                        new DirectoryInfo(RepositoryData.PhysicalDirectory.FullName + @"\" + RepositoryData.Name);
            RepositoryData.Mountpoint =
                        new DirectoryInfo(RepositoryData.Mountpoint.FullName + @"\" + RepositoryData.Name);
            if (!CheckSanity()) return;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void mBtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void mTxBRepoName_TextChanged(object sender, EventArgs e)
        {
            RepositoryData.Name = mTxBRepoName.Text;
        }

        private void mTxBRepoAddress_TextChanged(object sender, EventArgs e)
        {
            RepositoryData.Address = mTxBRepoAddress.Text;
        }

        private void mTxBRepoAddress_Leave(object sender, EventArgs e)
        {
            if (!mTxBRepoAddress.Text.Contains('/')) return;

            var strings = mTxBRepoAddress.Text.Split('/');

            if (strings.Length == 2)
            {
                ///check special characters in repository name
                var regexItem = new Regex("^[a-zA-Z0-9-_.]+$");
                if (!regexItem.IsMatch(strings[1])) {
                    MetroMessageBox.Show(this, "Repository name must be valid alpha or numeric or dash(-_) or dot characters.",
                                          "Warning",
                                          MessageBoxButtons.OK, MessageBoxIcon.Error,200);
                    mTxBRepoAddress.Text = "";
                    return;
                }
                RepositoryData.Name = strings[1];



                if (RepositoryData.PhysicalDirectory.IsEqualTo(GlobalOptions.Instance.DefaultCheckoutDir))
                    RepositoryData.PhysicalDirectory =
                        new DirectoryInfo(RepositoryData.PhysicalDirectory.FullName);
               if (RepositoryData.Mountpoint.IsEqualTo(GlobalOptions.Instance.DefaultMountpointDir))
                    RepositoryData.Mountpoint =
                        new DirectoryInfo(RepositoryData.Mountpoint.FullName );

                mTxBRepoName.Text = RepositoryData.Name;
                mTxBRepoCheckoutDir.Text = RepositoryData.PhysicalDirectory.FullName + @"\" + RepositoryData.Name;
                mTxBRepoMountpoint.Text = RepositoryData.Mountpoint.FullName + @"\" + RepositoryData.Name;
            }
        }

        private void mBtnPickRepoCheckoutDir_Click(object sender, EventArgs e)
        {
            var folderBrowser = new FolderBrowserDialog
            {
                SelectedPath = RepositoryData.PhysicalDirectory.FullName
            };

            var res = folderBrowser.ShowDialog();

            if (res == DialogResult.OK)
                RepositoryData.PhysicalDirectory = new DirectoryInfo(folderBrowser.SelectedPath);

            mTxBRepoCheckoutDir.Text = RepositoryData.PhysicalDirectory.FullName + @"\" + RepositoryData.Name;
        }

        private void mBtnPickRepoMountpointDir_Click(object sender, EventArgs e)
        {
            var folderBrowser = new FolderBrowserDialog
            {
                SelectedPath = RepositoryData.Mountpoint.FullName
            };

            var res = folderBrowser.ShowDialog();

            if (res == DialogResult.OK)
                RepositoryData.Mountpoint = new DirectoryInfo(folderBrowser.SelectedPath);

            mTxBRepoMountpoint.Text = RepositoryData.Mountpoint.FullName + @"\" + RepositoryData.Name;
        }

        private void mBtnRepoBrowser_Click(object sender, EventArgs e)
        {
            var repoBrowser = new MetroRepoBrowser(_appContext);

            if (repoBrowser.ShowDialog() == DialogResult.OK)
            {
                mTxBRepoAddress.Text = repoBrowser.SelectedRepository;
                mTxBRepoAddress_Leave(null, EventArgs.Empty);
            }
        }
    }
}