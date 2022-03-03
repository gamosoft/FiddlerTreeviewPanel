using System;
using System.Reflection;
using System.Windows.Forms;

namespace TreeViewPanelExtension
{
    /// <summary>
    /// New form for the About message
    /// </summary>
    public partial class AboutForm : Form
    {
        #region "Methods"

        /// <summary>
        /// Default constructor
        /// </summary>
        public AboutForm()
        {
            InitializeComponent();
            Assembly asm = Assembly.GetExecutingAssembly();
            AssemblyName asmName = asm.GetName();
            this.lblVersion.Text = asmName.Version.ToString();
        }

        /// <summary>
        /// Close the window
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">EventArgs</param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Open a new browser with the URL specified
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">LinkLabelLinkClickedEventArgs</param>
        private void lnkUrl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(this.lnkUrl.Text);
        }

        #endregion "Methods"
    }
}