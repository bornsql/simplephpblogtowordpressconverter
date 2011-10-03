using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace ITS.Tools.SimplePHPBlogToWordPressConverter
{
    partial class AboutBox : Form
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="AboutBox"/> class.
		/// </summary>
        public AboutBox()
        {
            InitializeComponent();
            GenerateLicence();
        }

		/// <summary>
		/// Generates the licence.
		/// </summary>
        public void GenerateLicence()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Copyright (c) 2009 Randolph Potter");
            sb.AppendLine();
            sb.Append("Permission is hereby granted, free of charge, to any person obtaining a copy ");
            sb.Append("of this software and associated documentation files (the \"Software\"), to deal ");
            sb.Append("in the Software without restriction, including without limitation the rights ");
            sb.Append("to use, copy, modify, merge, publish, distribute, sublicense, and/or sell ");
            sb.Append("copies of the Software, and to permit persons to whom the Software is ");
            sb.Append("furnished to do so, subject to the following conditions:");
            sb.AppendLine();
            sb.AppendLine();
            sb.Append("The above copyright notice and this permission notice shall be included in ");
            sb.Append("all copies or substantial portions of the Software.");
            sb.AppendLine();
            sb.AppendLine();
            sb.Append("THE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR ");
            sb.Append("IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, ");
            sb.Append("FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE ");
            sb.Append("AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER ");
            sb.Append("LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, ");
            sb.Append("OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN ");
            sb.Append("THE SOFTWARE.");

            rtbLicence.Text = sb.ToString();
        }

		/// <summary>
		/// Handles the Click event of the okButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}
