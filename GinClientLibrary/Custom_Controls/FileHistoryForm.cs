using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GinClientLibrary.Custom_Controls
{
    /// <summary>
    /// Form used to show user available file version for checkout
    /// </summary>
    public partial class FileHistoryForm : Form
    {
        public string hashRestore = "";
        public FileHistoryForm(List<GinRepository.FileVersion> fileHistory)
        {
            InitializeComponent();
            listView1.Items.Clear();
            ///show loaded files history versions ins listView
            foreach (var history in fileHistory)
            {
                listView1.Items.Add(new ListViewItem(new[] { history.getDateTime().ToString("dd/MM/yyyy HH:mm:ss"), history.authorname, history.subject, history.body, history.abbrevhash }));
            }
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void Ok_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            if (listView1.SelectedItems.Count == 1)
            {
                var old = listView1.SelectedItems[0].SubItems[4].Text;
                hashRestore = old;
            }
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
