using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void tBxAlias_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tBxAlias.Text))
            {
                e.Cancel = true;
                this.errorProvider1.SetError(tBxAlias,"Server alias cannot be empty!");
            }
        }

        private void tBxAlias_Validated(object sender, EventArgs e)
        {
            this.errorProvider1.SetError(tBxAlias, "");
        }

        private void cBxWebProtocol_Validating(object sender, CancelEventArgs e)
        {
            int number;
            try
            {
                int.TryParse(cBxWebPort.Text, out number);
            }
            catch
            {
                e.Cancel = true;
                this.errorProvider1.SetError(cBxWebPort, "Use valid number!");
            }
            
        }
    }
}
