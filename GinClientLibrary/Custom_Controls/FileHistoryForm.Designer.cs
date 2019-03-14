namespace GinClientLibrary.Custom_Controls
{
    partial class FileHistoryForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileHistoryForm));
            this.listView1 = new System.Windows.Forms.ListView();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.dateColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.authorColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.subjectColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.bodyColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.hashColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.dateColumn,
            this.authorColumn,
            this.subjectColumn,
            this.bodyColumn,
            this.hashColumn});
            this.listView1.Location = new System.Drawing.Point(13, 13);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(775, 343);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(48, 380);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(101, 58);
            this.button1.TabIndex = 1;
            this.button1.Text = "Restore";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(245, 380);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(108, 58);
            this.button2.TabIndex = 2;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // dateColumn
            // 
            this.dateColumn.Text = "Date";
            // 
            // authorColumn
            // 
            this.authorColumn.Text = "Author";
            // 
            // subjectColumn
            // 
            this.subjectColumn.Text = "Subject";
            // 
            // bodyColumn
            // 
            this.bodyColumn.Text = "Body";
            // 
            // hashColumn
            // 
            this.hashColumn.Text = "Hash";
            // 
            // FileHistoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.listView1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FileHistoryForm";
            this.Text = "Select previous version";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ColumnHeader dateColumn;
        private System.Windows.Forms.ColumnHeader authorColumn;
        private System.Windows.Forms.ColumnHeader subjectColumn;
        private System.Windows.Forms.ColumnHeader bodyColumn;
        private System.Windows.Forms.ColumnHeader hashColumn;
    }
}