using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace GinClientApp.Dialogs
{
    public partial class ServerForm : Form
    {

        public string web;
        public string git;
        public string alias;

        public ServerForm()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            web = cBxWebProtocol.Text + "://" + tBxWebHostname.Text+":"+ cBxWebPort.Text;
            alias = tBxAlias.Text;
            git = cBxGitUser.Text+"@"+tBxGitHostname+":"+cBxWebPort;
            Close();
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
    }
}
