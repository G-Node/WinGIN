namespace GinClientApp.Dialogs
{
    partial class MetroUploadFilesDlg
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.ColumnHeader columnHeader1;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MetroUploadFilesDlg));
            this.mLvwFiles = new MetroFramework.Controls.MetroListView();
            this.mBtnOK = new MetroFramework.Controls.MetroButton();
            this.mBtnCancel = new MetroFramework.Controls.MetroButton();
            this.CommitLabel = new System.Windows.Forms.Label();
            this.CommitTextBox = new System.Windows.Forms.TextBox();
            columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // columnHeader1
            // 
            columnHeader1.Width = 200;
            // 
            // mLvwFiles
            // 
            this.mLvwFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mLvwFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            columnHeader1});
            this.mLvwFiles.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.mLvwFiles.FullRowSelect = true;
            this.mLvwFiles.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.mLvwFiles.Location = new System.Drawing.Point(48, 123);
            this.mLvwFiles.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.mLvwFiles.Name = "mLvwFiles";
            this.mLvwFiles.OwnerDraw = true;
            this.mLvwFiles.ShowItemToolTips = true;
            this.mLvwFiles.Size = new System.Drawing.Size(510, 485);
            this.mLvwFiles.TabIndex = 0;
            this.mLvwFiles.UseCompatibleStateImageBehavior = false;
            this.mLvwFiles.UseSelectable = true;
            this.mLvwFiles.View = System.Windows.Forms.View.Details;
            // 
            // mBtnOK
            // 
            this.mBtnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mBtnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mBtnOK.Location = new System.Drawing.Point(48, 698);
            this.mBtnOK.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.mBtnOK.Name = "mBtnOK";
            this.mBtnOK.Size = new System.Drawing.Size(150, 44);
            this.mBtnOK.TabIndex = 1;
            this.mBtnOK.Text = "OK";
            this.mBtnOK.UseSelectable = true;
            // 
            // mBtnCancel
            // 
            this.mBtnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mBtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mBtnCancel.Location = new System.Drawing.Point(212, 698);
            this.mBtnCancel.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.mBtnCancel.Name = "mBtnCancel";
            this.mBtnCancel.Size = new System.Drawing.Size(150, 44);
            this.mBtnCancel.TabIndex = 2;
            this.mBtnCancel.Text = "Cancel";
            this.mBtnCancel.UseSelectable = true;
            // 
            // CommitLabel
            // 
            this.CommitLabel.AutoSize = true;
            this.CommitLabel.Location = new System.Drawing.Point(48, 617);
            this.CommitLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.CommitLabel.Name = "CommitLabel";
            this.CommitLabel.Size = new System.Drawing.Size(178, 25);
            this.CommitLabel.TabIndex = 3;
            this.CommitLabel.Text = "Commit Message";
            this.CommitLabel.Visible = false;
            // 
            // CommitTextBox
            // 
            this.CommitTextBox.Location = new System.Drawing.Point(48, 648);
            this.CommitTextBox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.CommitTextBox.Name = "CommitTextBox";
            this.CommitTextBox.Size = new System.Drawing.Size(504, 31);
            this.CommitTextBox.TabIndex = 4;
            this.CommitTextBox.Visible = false;
            // 
            // MetroUploadFilesDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(608, 777);
            this.Controls.Add(this.CommitTextBox);
            this.Controls.Add(this.CommitLabel);
            this.Controls.Add(this.mBtnCancel);
            this.Controls.Add(this.mBtnOK);
            this.Controls.Add(this.mLvwFiles);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "MetroUploadFilesDlg";
            this.Padding = new System.Windows.Forms.Padding(40, 115, 40, 38);
            this.Text = "Files to upload";
            this.Load += new System.EventHandler(this.MetroUploadFilesDlg_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroListView mLvwFiles;
        private MetroFramework.Controls.MetroButton mBtnOK;
        private MetroFramework.Controls.MetroButton mBtnCancel;
        private System.Windows.Forms.Label CommitLabel;
        public System.Windows.Forms.TextBox CommitTextBox;
    }
}