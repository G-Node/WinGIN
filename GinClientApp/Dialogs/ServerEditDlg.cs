using GinClientApp.GinService;
using GinClientLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace GinClientApp.Dialogs
{
    public partial class EditServerForm : Form
    {

        public string Web { get; set; }
        public string Git { get; set; }
        public string SelectedServer { get; set; }

        public Dictionary<string, ServerConf> ServerDic { get; set; }
        public string Alias { get; set; }

        private readonly GinApplicationContext _parentContext;

        public EditServerForm(GinApplicationContext Context)
        {
            InitializeComponent();
            _parentContext = Context;
            var text = _parentContext.ServiceClient.GetServers();          
            ServerDic  = JsonConvert.DeserializeObject<Dictionary<string, ServerConf>>(text);
            AutoValidate = AutoValidate.Disable;
            tBxAlias.DataSource = new BindingSource(ServerDic, null);
            tBxAlias.DisplayMember = "Key";
            tBxAlias.ValueMember = "Key";
            FillServerInfo();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (ValidateChildren())
            {
                Web = cBxWebProtocol.Text + "://" + tBxWebHostname.Text + ":" + cBxWebPort.Text;
                Git = cBxGitUser.Text + "@" + tBxGitHostname.Text + ":" + cBxGitPort.Text;
                SelectedServer = (string) tBxAlias.SelectedValue;
                _parentContext.ServiceClient.NewServer(SelectedServer, Web, Git);
                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Validation failed.");
            }
        }
        private bool tBxAlias_Validate()
        {
            if (string.IsNullOrWhiteSpace(tBxAlias.Text))
            {
                errorProvider1.SetError(tBxAlias, "Server alias cannot be empty!");
                return false;
            }
            else
            {
                return true;
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void tBxAlias_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tBxAlias.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(tBxAlias,"Server alias cannot be empty!");
            }
        }

        private void tBxAlias_Validated(object sender, EventArgs e)
        {
            errorProvider1.SetError(tBxAlias, "");
        }

        private void Port_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                int.Parse(cBxWebPort.Text);
            }
            catch
            {
                e.Cancel = true;
                errorProvider1.SetError(cBxWebPort, "Use valid number!");
            }          
        }

        private void cBxWebPort_Validated(object sender, EventArgs e)
        {
            errorProvider1.SetError(cBxWebPort,"");
        }

        private void cBxWebProtocol_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cBxWebProtocol.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(cBxWebProtocol, "Protocol cannot be empty!");
            }
        }

        private void cBxWebProtocol_Validated(object sender, EventArgs e)
        {
            errorProvider1.SetError(cBxWebProtocol, "");
        }

        private void tBxWebHostname_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tBxWebHostname.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(tBxWebHostname, "Hostname cannot be empty!");
            }
        }

        private void tBxWebHostname_Validated(object sender, EventArgs e)
        {
            errorProvider1.SetError(tBxWebHostname, "");
        }

        private void cBxGitUser_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cBxGitUser.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(cBxGitUser, "User name cannot be empty!");
            }
        }

        private void cBxGitUser_Validated(object sender, EventArgs e)
        {
            errorProvider1.SetError(cBxGitUser, "");
        }

        private void tBxGitHostname_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tBxGitHostname.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(tBxGitHostname, "Hostname cannot be empty!");
            }
        }

        private void tBxGitHostname_Validated(object sender, EventArgs e)
        {
            errorProvider1.SetError(tBxGitHostname, "");
        }

        private void cBxGitPort_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                int.Parse(cBxGitPort.Text);
            }
            catch
            {
                e.Cancel = true;
                errorProvider1.SetError(cBxGitPort, "Use valid number!");
            }
        }

        private void cBxGitPort_Validated(object sender, EventArgs e)
        {
            errorProvider1.SetError(cBxGitPort, "");
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Do you want to really delete "+ (string)tBxAlias.SelectedValue + " server configuration?","Warning!",MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                ///delete configuration
                _parentContext.ServiceClient.DeleteServer((string)tBxAlias.SelectedValue);
                var text = _parentContext.ServiceClient.GetServers();
                ServerDic = JsonConvert.DeserializeObject<Dictionary<string, ServerConf>>(text);
                Close();
            }
            else
            {
                ///dont delete
            }
        }

        private void tBxAlias_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillServerInfo();
        }

        private void FillServerInfo() {
            var selectedServer = ServerDic[(string)tBxAlias.SelectedValue];
            cBxWebProtocol.Text = selectedServer.Web.Protocol;
            tBxWebHostname.Text = selectedServer.Web.Host;
            cBxWebPort.Text = selectedServer.Web.Port;
            cBxGitPort.Text = selectedServer.Git.Port;
            cBxGitUser.Text = selectedServer.Git.User;
            tBxGitHostname.Text = selectedServer.Git.Host;
        }
    }
}
