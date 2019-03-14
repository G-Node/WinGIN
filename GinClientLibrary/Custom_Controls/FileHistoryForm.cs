using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GinClientLibrary.Custom_Controls
{
    public partial class FileHistoryForm : Form
    {
        public FileHistoryForm(List<GinRepository.FileVersion> fileHistory)
        {
            InitializeComponent();
            listView1.Items.Clear();
            foreach (var history in fileHistory)
            {
                listView1.Items.Add(new ListViewItem(new[] { history.date, history.authorname, history.subject, history.body, history.abbrevhash }));
            }
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.Title = "Select location for restortoration of file";
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.ShowDialog();
            var filepath = saveFileDialog1.FileName;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
