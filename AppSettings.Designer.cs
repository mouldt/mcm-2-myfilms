namespace MCM2MyFilms
{
    partial class AppSettings
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtAMCatalog = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtFilmDirs = new System.Windows.Forms.TextBox();
            this.btnFindFilmDir = new System.Windows.Forms.Button();
            this.btnFindAMCCatalog = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.btnFindMePoThumbsDir = new System.Windows.Forms.Button();
            this.txtMePoThumbsDir = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtFilmExtensions = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(8, 236);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(89, 236);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "AMC Catalog :";
            // 
            // txtAMCatalog
            // 
            this.txtAMCatalog.Location = new System.Drawing.Point(87, 5);
            this.txtAMCatalog.Name = "txtAMCatalog";
            this.txtAMCatalog.Size = new System.Drawing.Size(360, 20);
            this.txtAMCatalog.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Film Directory :";
            // 
            // txtFilmDirs
            // 
            this.txtFilmDirs.Location = new System.Drawing.Point(87, 28);
            this.txtFilmDirs.Name = "txtFilmDirs";
            this.txtFilmDirs.Size = new System.Drawing.Size(360, 20);
            this.txtFilmDirs.TabIndex = 5;
            // 
            // btnFindFilmDir
            // 
            this.btnFindFilmDir.Location = new System.Drawing.Point(453, 27);
            this.btnFindFilmDir.Name = "btnFindFilmDir";
            this.btnFindFilmDir.Size = new System.Drawing.Size(27, 20);
            this.btnFindFilmDir.TabIndex = 6;
            this.btnFindFilmDir.Text = "...";
            this.btnFindFilmDir.UseVisualStyleBackColor = true;
            this.btnFindFilmDir.Click += new System.EventHandler(this.btnFindFilmDir_Click);
            // 
            // btnFindAMCCatalog
            // 
            this.btnFindAMCCatalog.Location = new System.Drawing.Point(453, 4);
            this.btnFindAMCCatalog.Name = "btnFindAMCCatalog";
            this.btnFindAMCCatalog.Size = new System.Drawing.Size(27, 20);
            this.btnFindAMCCatalog.TabIndex = 7;
            this.btnFindAMCCatalog.Text = "...";
            this.btnFindAMCCatalog.UseVisualStyleBackColor = true;
            this.btnFindAMCCatalog.Click += new System.EventHandler(this.btnFindAMCCatalog_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // btnFindMePoThumbsDir
            // 
            this.btnFindMePoThumbsDir.Location = new System.Drawing.Point(453, 51);
            this.btnFindMePoThumbsDir.Name = "btnFindMePoThumbsDir";
            this.btnFindMePoThumbsDir.Size = new System.Drawing.Size(27, 20);
            this.btnFindMePoThumbsDir.TabIndex = 10;
            this.btnFindMePoThumbsDir.Text = "...";
            this.btnFindMePoThumbsDir.UseVisualStyleBackColor = true;
            this.btnFindMePoThumbsDir.Click += new System.EventHandler(this.btnFindMePoThumbsDir_Click);
            // 
            // txtMePoThumbsDir
            // 
            this.txtMePoThumbsDir.Location = new System.Drawing.Point(87, 52);
            this.txtMePoThumbsDir.Name = "txtMePoThumbsDir";
            this.txtMePoThumbsDir.Size = new System.Drawing.Size(360, 20);
            this.txtMePoThumbsDir.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "MyFilms Fanart :";
            // 
            // txtFilmExtensions
            // 
            this.txtFilmExtensions.Location = new System.Drawing.Point(87, 78);
            this.txtFilmExtensions.Name = "txtFilmExtensions";
            this.txtFilmExtensions.Size = new System.Drawing.Size(360, 20);
            this.txtFilmExtensions.TabIndex = 12;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 81);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Film Extensions :";
            // 
            // AppSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(482, 262);
            this.ControlBox = false;
            this.Controls.Add(this.txtFilmExtensions);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnFindMePoThumbsDir);
            this.Controls.Add(this.txtMePoThumbsDir);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnFindAMCCatalog);
            this.Controls.Add(this.btnFindFilmDir);
            this.Controls.Add(this.txtFilmDirs);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtAMCatalog);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Name = "AppSettings";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configure Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtAMCatalog;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtFilmDirs;
        private System.Windows.Forms.Button btnFindFilmDir;
        private System.Windows.Forms.Button btnFindAMCCatalog;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button btnFindMePoThumbsDir;
        private System.Windows.Forms.TextBox txtMePoThumbsDir;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtFilmExtensions;
        private System.Windows.Forms.Label label4;
    }
}