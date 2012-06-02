namespace ITS.Tools.SimplePHPBlogToWordPressConverter
{
	partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.grpMain = new System.Windows.Forms.GroupBox();
			this.lblCategoryList = new System.Windows.Forms.Label();
			this.txtCategoryList = new System.Windows.Forms.TextBox();
			this.txtHttpRoot = new System.Windows.Forms.TextBox();
			this.lblHttpRoot = new System.Windows.Forms.Label();
			this.btnTarget = new System.Windows.Forms.Button();
			this.txtTarget = new System.Windows.Forms.TextBox();
			this.lblTarget = new System.Windows.Forms.Label();
			this.btnSource = new System.Windows.Forms.Button();
			this.lblSource = new System.Windows.Forms.Label();
			this.txtSource = new System.Windows.Forms.TextBox();
			this.lblMain = new System.Windows.Forms.Label();
			this.lblCopyright = new System.Windows.Forms.Label();
			this.btnConvert = new System.Windows.Forms.Button();
			this.folderBrowserDialogSource = new System.Windows.Forms.FolderBrowserDialog();
			this.folderBrowserDialogTarget = new System.Windows.Forms.FolderBrowserDialog();
			this.btnLicence = new System.Windows.Forms.Button();
			this.lblCategoryDescription = new System.Windows.Forms.Label();
			this.linkXqrx = new System.Windows.Forms.LinkLabel();
			this.grpMain.SuspendLayout();
			this.SuspendLayout();
			// 
			// grpMain
			// 
			this.grpMain.Controls.Add(this.lblCategoryDescription);
			this.grpMain.Controls.Add(this.lblCategoryList);
			this.grpMain.Controls.Add(this.txtCategoryList);
			this.grpMain.Controls.Add(this.txtHttpRoot);
			this.grpMain.Controls.Add(this.lblHttpRoot);
			this.grpMain.Controls.Add(this.btnTarget);
			this.grpMain.Controls.Add(this.txtTarget);
			this.grpMain.Controls.Add(this.lblTarget);
			this.grpMain.Controls.Add(this.btnSource);
			this.grpMain.Controls.Add(this.lblSource);
			this.grpMain.Controls.Add(this.txtSource);
			this.grpMain.Location = new System.Drawing.Point(12, 30);
			this.grpMain.Name = "grpMain";
			this.grpMain.Size = new System.Drawing.Size(641, 172);
			this.grpMain.TabIndex = 0;
			this.grpMain.TabStop = false;
			this.grpMain.Text = "Conversion Settings";
			// 
			// lblCategoryList
			// 
			this.lblCategoryList.AutoSize = true;
			this.lblCategoryList.Location = new System.Drawing.Point(6, 110);
			this.lblCategoryList.Name = "lblCategoryList";
			this.lblCategoryList.Size = new System.Drawing.Size(68, 13);
			this.lblCategoryList.TabIndex = 10;
			this.lblCategoryList.Text = "Category List";
			// 
			// txtCategoryList
			// 
			this.txtCategoryList.Location = new System.Drawing.Point(128, 107);
			this.txtCategoryList.Multiline = true;
			this.txtCategoryList.Name = "txtCategoryList";
			this.txtCategoryList.Size = new System.Drawing.Size(507, 39);
			this.txtCategoryList.TabIndex = 9;
			this.txtCategoryList.Text = "1|General|0";
			// 
			// txtHttpRoot
			// 
			this.txtHttpRoot.Location = new System.Drawing.Point(128, 81);
			this.txtHttpRoot.Name = "txtHttpRoot";
			this.txtHttpRoot.Size = new System.Drawing.Size(507, 20);
			this.txtHttpRoot.TabIndex = 8;
			this.txtHttpRoot.Text = "http://www.randolphpotter.com/blog/";
			// 
			// lblHttpRoot
			// 
			this.lblHttpRoot.AutoSize = true;
			this.lblHttpRoot.Location = new System.Drawing.Point(6, 84);
			this.lblHttpRoot.Name = "lblHttpRoot";
			this.lblHttpRoot.Size = new System.Drawing.Size(108, 13);
			this.lblHttpRoot.TabIndex = 7;
			this.lblHttpRoot.Text = "Simple PHP Blog root";
			// 
			// btnTarget
			// 
			this.btnTarget.Location = new System.Drawing.Point(609, 52);
			this.btnTarget.Name = "btnTarget";
			this.btnTarget.Size = new System.Drawing.Size(26, 23);
			this.btnTarget.TabIndex = 6;
			this.btnTarget.Text = "...";
			this.btnTarget.UseVisualStyleBackColor = true;
			this.btnTarget.Click += new System.EventHandler(this.btnTarget_Click);
			// 
			// txtTarget
			// 
			this.txtTarget.Location = new System.Drawing.Point(128, 54);
			this.txtTarget.Name = "txtTarget";
			this.txtTarget.ReadOnly = true;
			this.txtTarget.Size = new System.Drawing.Size(475, 20);
			this.txtTarget.TabIndex = 5;
			this.txtTarget.Text = "C:\\Temp";
			// 
			// lblTarget
			// 
			this.lblTarget.AutoSize = true;
			this.lblTarget.Location = new System.Drawing.Point(6, 57);
			this.lblTarget.Name = "lblTarget";
			this.lblTarget.Size = new System.Drawing.Size(112, 13);
			this.lblTarget.TabIndex = 4;
			this.lblTarget.Text = "WordPress SQL folder";
			// 
			// btnSource
			// 
			this.btnSource.AutoSize = true;
			this.btnSource.Location = new System.Drawing.Point(609, 24);
			this.btnSource.Name = "btnSource";
			this.btnSource.Size = new System.Drawing.Size(26, 23);
			this.btnSource.TabIndex = 3;
			this.btnSource.Text = "...";
			this.btnSource.UseVisualStyleBackColor = true;
			this.btnSource.Click += new System.EventHandler(this.btnSource_Click);
			// 
			// lblSource
			// 
			this.lblSource.AutoSize = true;
			this.lblSource.Location = new System.Drawing.Point(6, 29);
			this.lblSource.Name = "lblSource";
			this.lblSource.Size = new System.Drawing.Size(116, 13);
			this.lblSource.TabIndex = 2;
			this.lblSource.Text = "Simple PHP Blog folder";
			// 
			// txtSource
			// 
			this.txtSource.Location = new System.Drawing.Point(128, 26);
			this.txtSource.Name = "txtSource";
			this.txtSource.ReadOnly = true;
			this.txtSource.Size = new System.Drawing.Size(475, 20);
			this.txtSource.TabIndex = 1;
			this.txtSource.Text = "<Select the \"content\" folder for your Simple PHP Blog>";
			// 
			// lblMain
			// 
			this.lblMain.Location = new System.Drawing.Point(12, 9);
			this.lblMain.Name = "lblMain";
			this.lblMain.Size = new System.Drawing.Size(628, 27);
			this.lblMain.TabIndex = 0;
			this.lblMain.Text = "Choose a folder where the Simple PHP Blog content is stored. Then choose a folder" +
				" where the generated SQL will be saved.";
			// 
			// lblCopyright
			// 
			this.lblCopyright.AutoSize = true;
			this.lblCopyright.Location = new System.Drawing.Point(9, 218);
			this.lblCopyright.Name = "lblCopyright";
			this.lblCopyright.Size = new System.Drawing.Size(170, 13);
			this.lblCopyright.TabIndex = 1;
			this.lblCopyright.Text = "Copyright © 2009 Randolph Potter";
			// 
			// btnConvert
			// 
			this.btnConvert.Location = new System.Drawing.Point(578, 208);
			this.btnConvert.Name = "btnConvert";
			this.btnConvert.Size = new System.Drawing.Size(75, 23);
			this.btnConvert.TabIndex = 2;
			this.btnConvert.Text = "Convert";
			this.btnConvert.UseVisualStyleBackColor = true;
			this.btnConvert.Click += new System.EventHandler(this.btnConvert_Click);
			// 
			// btnLicence
			// 
			this.btnLicence.Location = new System.Drawing.Point(497, 208);
			this.btnLicence.Name = "btnLicence";
			this.btnLicence.Size = new System.Drawing.Size(75, 23);
			this.btnLicence.TabIndex = 3;
			this.btnLicence.Text = "Licence";
			this.btnLicence.UseVisualStyleBackColor = true;
			this.btnLicence.Click += new System.EventHandler(this.btnLicence_Click);
			// 
			// lblCategoryDescription
			// 
			this.lblCategoryDescription.AutoSize = true;
			this.lblCategoryDescription.Location = new System.Drawing.Point(128, 149);
			this.lblCategoryDescription.Name = "lblCategoryDescription";
			this.lblCategoryDescription.Size = new System.Drawing.Size(321, 13);
			this.lblCategoryDescription.TabIndex = 11;
			this.lblCategoryDescription.Text = "Copy the contents of your categories.txt file into the textbox above.";
			// 
			// linkXqrx
			// 
			this.linkXqrx.AutoSize = true;
			this.linkXqrx.Location = new System.Drawing.Point(185, 218);
			this.linkXqrx.Name = "linkXqrx";
			this.linkXqrx.Size = new System.Drawing.Size(85, 13);
			this.linkXqrx.TabIndex = 4;
			this.linkXqrx.TabStop = true;
			this.linkXqrx.Text = "http://xqrx.com/";
			this.linkXqrx.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkXqrx_LinkClicked);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(665, 237);
			this.Controls.Add(this.linkXqrx);
			this.Controls.Add(this.btnLicence);
			this.Controls.Add(this.btnConvert);
			this.Controls.Add(this.lblCopyright);
			this.Controls.Add(this.grpMain);
			this.Controls.Add(this.lblMain);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Simple PHP Blog To WordPress Converter v0.3.0";
			this.grpMain.ResumeLayout(false);
			this.grpMain.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialogTarget;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialogSource;
		private System.Windows.Forms.Button btnConvert;
		private System.Windows.Forms.Label lblCopyright;
		private System.Windows.Forms.TextBox txtTarget;
		private System.Windows.Forms.Button btnTarget;
		private System.Windows.Forms.Label lblMain;
		private System.Windows.Forms.TextBox txtSource;
		private System.Windows.Forms.Label lblSource;
		private System.Windows.Forms.Button btnSource;
		private System.Windows.Forms.Label lblTarget;
        private System.Windows.Forms.GroupBox grpMain;
        private System.Windows.Forms.Button btnLicence;
        private System.Windows.Forms.Label lblCategoryList;
        private System.Windows.Forms.TextBox txtCategoryList;
        private System.Windows.Forms.TextBox txtHttpRoot;
        private System.Windows.Forms.Label lblHttpRoot;
		private System.Windows.Forms.Label lblCategoryDescription;
		private System.Windows.Forms.LinkLabel linkXqrx;
	}
}
