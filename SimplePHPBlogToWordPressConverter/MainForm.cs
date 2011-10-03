using System;
using System.IO;
using System.Windows.Forms;

namespace ITS.Tools.SimplePHPBlogToWordPressConverter
{
    /// <summary>
    /// Description of MainForm.
    /// </summary>
    public partial class MainForm : Form
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="MainForm"/> class.
		/// </summary>
        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Chooses the folder.
        /// </summary>
        /// <param name="fsd">The FolderBrowserDialog.</param>
        /// <param name="txt">The TextBox.</param>
        static void ChooseFolder(FolderBrowserDialog fsd, TextBox txt)
        {
            fsd.ShowNewFolderButton = false;
            fsd.ShowDialog();
            txt.Text = fsd.SelectedPath;
        }

        /// <summary>
        /// Handles the Click event of the btnSource control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnSource_Click(object sender, EventArgs e)
        {
            ChooseFolder(folderBrowserDialogSource, txtSource);
        }

        /// <summary>
        /// Handles the Click event of the btnTarget control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnTarget_Click(object sender, EventArgs e)
        {
            ChooseFolder(folderBrowserDialogTarget, txtTarget);
        }

        /// <summary>
        /// Handles the Click event of the btnConvert control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnConvert_Click(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now;
            TimeSpan ts = new TimeSpan();
            if (Directory.Exists(txtSource.Text) && Directory.Exists(txtTarget.Text))
            {
                Converter c = new Converter
                                  {
                                      HttpRoot = txtHttpRoot.Text.Trim(),
                                      CategoryList = txtCategoryList.Text.Trim()
                                  };
                c.ConvertBlogToSql(txtSource.Text, txtTarget.Text);
            }
            ts = DateTime.Now - dt;
            string s = string.Format("{0} minutes, {1} seconds, {2} milliseconds", ts.Minutes, ts.Seconds, ts.Milliseconds);
            MessageBox.Show(s);
        }

		/// <summary>
		/// Handles the Click event of the btnLicence control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnLicence_Click(object sender, EventArgs e)
        {
            AboutBox aboutBox = new AboutBox();
            aboutBox.ShowDialog();
        }

		/// <summary>
		/// Handles the LinkClicked event of the linkXqrx control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
		private void linkXqrx_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start(linkXqrx.Text);
		}
    }
}