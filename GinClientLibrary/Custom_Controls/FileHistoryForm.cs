using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GinClientLibrary.Custom_Controls
{
    public partial class FileHistoryForm : Form
    {
        public string hashRestore = "";
        public FileHistoryForm(List<GinRepository.FileVersion> fileHistory)
        {
            InitializeComponent();
            listView1.Items.Clear();
            ///bring form to top
            TopMost = true;
            Show();
            Focus();
            BringToFront();
            TopMost = false;
            foreach (var history in fileHistory)
            {
                listView1.Items.Add(new ListViewItem(new[] { history.date, history.authorname, history.subject, history.body, history.abbrevhash }));
            }           
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            if (listView1.SelectedItems.Count == 1)
            {
                var old = listView1.SelectedItems[0].SubItems[4].Text;
                hashRestore = old;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
