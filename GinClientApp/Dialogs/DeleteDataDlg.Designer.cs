using MetroFramework.Controls;

namespace GinClientApp.Dialogs
{
    partial class DeleteDataDlg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeleteDataDlg));
            this.label1 = new System.Windows.Forms.Label();
            this.chB_keepUserConfig = new MetroFramework.Controls.MetroCheckBox();
            this.chB_keepLoginInfo = new MetroFramework.Controls.MetroCheckBox();
            this.chB_keepCheckouts = new MetroFramework.Controls.MetroCheckBox();
            this.button1 = new MetroFramework.Controls.MetroButton();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 25);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 25);
            this.label1.TabIndex = 0;
            // 
            // chB_keepUserConfig
            // 
            this.chB_keepUserConfig.AutoSize = true;
            this.chB_keepUserConfig.Checked = true;
            this.chB_keepUserConfig.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chB_keepUserConfig.Location = new System.Drawing.Point(98, 121);
            this.chB_keepUserConfig.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.chB_keepUserConfig.Name = "chB_keepUserConfig";
            this.chB_keepUserConfig.Size = new System.Drawing.Size(123, 15);
            this.chB_keepUserConfig.TabIndex = 1;
            this.chB_keepUserConfig.Text = "User Configuration";
            this.chB_keepUserConfig.UseSelectable = true;
            this.chB_keepUserConfig.CheckedChanged += new System.EventHandler(this.chB_keepUserConfig_CheckedChanged);
            // 
            // chB_keepLoginInfo
            // 
            this.chB_keepLoginInfo.AutoSize = true;
            this.chB_keepLoginInfo.Checked = true;
            this.chB_keepLoginInfo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chB_keepLoginInfo.Location = new System.Drawing.Point(98, 165);
            this.chB_keepLoginInfo.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.chB_keepLoginInfo.Name = "chB_keepLoginInfo";
            this.chB_keepLoginInfo.Size = new System.Drawing.Size(145, 15);
            this.chB_keepLoginInfo.TabIndex = 2;
            this.chB_keepLoginInfo.Text = "User Login information";
            this.chB_keepLoginInfo.UseSelectable = true;
            this.chB_keepLoginInfo.CheckedChanged += new System.EventHandler(this.chB_keepLoginInfo_CheckedChanged);
            // 
            // chB_keepCheckouts
            // 
            this.chB_keepCheckouts.AutoSize = true;
            this.chB_keepCheckouts.Checked = true;
            this.chB_keepCheckouts.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chB_keepCheckouts.Location = new System.Drawing.Point(98, 210);
            this.chB_keepCheckouts.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.chB_keepCheckouts.Name = "chB_keepCheckouts";
            this.chB_keepCheckouts.Size = new System.Drawing.Size(79, 15);
            this.chB_keepCheckouts.TabIndex = 3;
            this.chB_keepCheckouts.Text = "Checkouts";
            this.chB_keepCheckouts.UseSelectable = true;
            this.chB_keepCheckouts.CheckedChanged += new System.EventHandler(this.chB_keepCheckouts_CheckedChanged);
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(758, 262);
            this.button1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(150, 44);
            this.button1.TabIndex = 4;
            this.button1.Text = "Ok";
            this.button1.UseSelectable = true;
            // 
            // DeleteDataDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(944, 335);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.chB_keepCheckouts);
            this.Controls.Add(this.chB_keepLoginInfo);
            this.Controls.Add(this.chB_keepUserConfig);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "DeleteDataDlg";
            this.Padding = new System.Windows.Forms.Padding(40, 115, 40, 38);
            this.Text = "Please mark any data you wish to keep:";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.DeleteDataDlg_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private MetroCheckBox chB_keepUserConfig;
        private MetroCheckBox chB_keepLoginInfo;
        private MetroCheckBox chB_keepCheckouts;
        private MetroButton button1;
    }
}