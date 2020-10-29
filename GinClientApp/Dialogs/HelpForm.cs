using System;
using System.Windows.Forms;

namespace GinClientApp.Dialogs
{
    public partial class HelpForm : Form
    {
        public HelpForm()
        {
            InitializeComponent();
            webBrowser1.Navigate("https://gin.g-node.org/G-Node/Info/wiki/WinGINTutorial");
        }

        private void HelpForm_Load(object sender, EventArgs e)
        {
            if (webBrowser1.Document == null)
                return;
        }
    }
}
