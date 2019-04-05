using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using GinClientLibrary;
using MetroFramework.Forms;

namespace GinClientApp.Dialogs
{
    public partial class MetroUploadFilesDlg : MetroForm
    {
        private readonly IEnumerable<KeyValuePair<string, GinRepository.FileStatus>> _alteredFiles;

        public MetroUploadFilesDlg(IEnumerable<KeyValuePair<string, GinRepository.FileStatus>> alteredFiles)
        {
            InitializeComponent();
            _alteredFiles = alteredFiles;

            foreach (var file in _alteredFiles)
            {
                var lvi = new ListViewItem(Path.GetFileName(file.Key));
                switch (file.Value)
                {
                    case GinRepository.FileStatus.InAnnex:
                        lvi.ForeColor = SystemColors.ControlText;
                        break;
                    case GinRepository.FileStatus.InAnnexModified:
                        lvi.ForeColor = SystemColors.ControlText;
                        break;
                    case GinRepository.FileStatus.OnDisk:
                        lvi.ForeColor = SystemColors.ControlText;
                        break;
                    case GinRepository.FileStatus.OnDiskModified:
                        lvi.ForeColor = Color.Black;
                        lvi.ToolTipText = "This file has been modified locally.";
                        break;
                    case GinRepository.FileStatus.Unknown:
                        lvi.ForeColor = Color.Blue;
                        lvi.ToolTipText = "This file is currently not tracked by gin";
                        break;
                    case GinRepository.FileStatus.Directory:
                        lvi.ForeColor = SystemColors.ControlText;
                        break;
                    case GinRepository.FileStatus.Unlocked:
                        lvi.ForeColor = SystemColors.ControlText;
                        break;
                    case GinRepository.FileStatus.Removed:
                        lvi.ForeColor = Color.Red;
                        lvi.ToolTipText =
                            "This file has been removed locally, but is still present in the remote repository.";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                mLvwFiles.Items.Add(lvi);
            }
            mLvwFiles.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void MetroUploadFilesDlg_Load(object sender, EventArgs e)
        {
           
        }

        private void CommitTextBox_Validating(object sender, CancelEventArgs e)
        {
            if (!ValidateMessage())
                e.Cancel = true;            
        }

        private void CommitTextBox_Validated(object sender, EventArgs e)
        {
            errorProvider1.SetError(CommitTextBox, "");
        }

        private bool ValidateMessage()
        {
            if (CommitTextBox.Text.Contains("\"") || CommitTextBox.Text.Contains("\'"))
            {
                errorProvider1.SetError(CommitTextBox, "Invalid characters in commit message. Message cannot contain \" or \'");
                return false;
            }
            else
            {
                errorProvider1.SetError(CommitTextBox, "");
                return true;
            }
        }

        private void mBtnOK_Click(object sender, EventArgs e)
        {
            if (!ValidateMessage())
            {
                return;
            }
            
            
        }
    }
}