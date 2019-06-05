using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using GinClientApp.Properties;
using GinClientLibrary;
using MetroFramework.Forms;
using Newtonsoft.Json;

namespace GinClientApp.Dialogs
{
    public partial class MetroGetUserCredentialsDlg : MetroForm
    {
        private readonly GinApplicationContext _parentContext;

        public MetroGetUserCredentialsDlg(GinApplicationContext parentContext)
        {
            InitializeComponent();
            _parentContext = parentContext;
            string serverJson = _parentContext.ServiceClient.GetServers();
            var ServerDic = JsonConvert.DeserializeObject<Dictionary<string, ServerConf>>(serverJson);
            mCBxServerAlias.DataSource = new BindingSource(ServerDic, null);
            mCBxServerAlias.DisplayMember = "Key";
            mCBxServerAlias.ValueMember = "Key";

            metroLabel1.TabStop = false;
            metroLabel2.TabStop = false;

            mLblWarning.Visible = false;
            //mCBxServerAlias.Text = "gin";
            //mCBxServerAlias.SelectedText = "gin";

            try
            {
                var login = UserCredentials.Instance.loginList.First();
                mTxBUsername.Text = login.Username;
                mTxBPassword.Text = login.Password;
                mCBxServerAlias.Text = login.Server;
            }
            catch
            {
                mTxBUsername.Text = "";
                mTxBPassword.Text = "";
                mCBxServerAlias.Text = "gin";
            }
            
            
        }

        private bool AttemptLogin()
        {
            if (string.IsNullOrEmpty(mTxBUsername.Text) || string.IsNullOrEmpty(mTxBPassword.Text) || string.IsNullOrEmpty((string)mCBxServerAlias.SelectedValue)) return false;

            _parentContext.ServiceClient.Logout();

            return _parentContext.ServiceClient.Login(mTxBUsername.Text, mTxBPassword.Text, (string)mCBxServerAlias.SelectedValue);
            //return _parentContext.ServiceClient.Login(mTxBUsername.Text, mTxBPassword.Text);
        }

        private void mBtnOk_Click(object sender, EventArgs e)
        {
            mLblWarning.Visible = false;

            if (AttemptLogin())
            {              
                var login = UserCredentials.Instance.loginList.Find(x => x.Server == mCBxServerAlias.SelectedText);
                if (login == null) {
                    login = UserCredentials.Instance.loginList.Find(x => x.Server == null);
                    if (login == null)
                    {
                    login = (new UserCredentials.LoginSettings());
                    UserCredentials.Instance.loginList.Add(login);

                    }
                }
                login.Username = mTxBUsername.Text;
                login.Password = mTxBPassword.Text;
                login.Server = mCBxServerAlias.Text;
                UserCredentials.Save();

                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                mLblWarning.Text = Resources.GetUserCredentials_The_entered_Username_Password_combination_is_invalid;
                mLblWarning.Visible = true;
            }
        }

        private void mBtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}