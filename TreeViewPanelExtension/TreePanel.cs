using System;
using System.Windows.Forms;
using Fiddler;
using Microsoft.Win32;

namespace TreeViewPanelExtension
{
    /// <summary>
    /// Creates a new control to be used in Fiddler, that represents the information in a TreeView
    /// </summary>
    public class TreePanel : IFiddlerExtension
    {
        #region "Variables"

        /// <summary>
        /// Holds the nodes with the sessions
        /// </summary>
        TreeView tv = null;

        /// <summary>
        /// OOB list view that handles original Fiddler sessions
        /// </summary>
        SessionListView lvSessions = null;

        /// <summary>
        /// New menu item
        /// </summary>
        private System.Windows.Forms.MenuItem mnuShowPanel;

        /// <summary>
        /// 
        /// </summary>
        private System.Windows.Forms.MenuItem miShowPanelEnabled;

        /// <summary>
        /// To keep images to be used in the TreeView
        /// </summary>
        ImageList imageList = null;

        #endregion "Variables"

        #region "Properties"

        /// <summary>
        /// Property that indicates if the new TreeViewPanel will be visible or not
        /// </summary>
        public bool ShowPanel { get; set; }

        #endregion "Properties"

        #region "Methods"

        /// <summary>
        /// Default constructor
        /// </summary>
        public TreePanel()
        {
            this.ShowPanel = false; // Default
            
            // Try to get registry configuration
            RegistryKey oReg = Registry.CurrentUser.OpenSubKey(String.Format(@"{0}\{1}\", CONFIG.GetRegPath("Root"), Constants.RegistryExtensionKeyName));
            if (null != oReg)
            {
                int showPanel = (int)oReg.GetValue(Constants.RegistryShowPanel, 0);
                this.ShowPanel = (showPanel == 1);
                oReg.Close();
            }

            // Initialize menu options
            this.mnuShowPanel = new System.Windows.Forms.MenuItem();
            this.miShowPanelEnabled = new System.Windows.Forms.MenuItem();
            this.mnuShowPanel.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] { this.miShowPanelEnabled });

            this.mnuShowPanel.Text = "TreeView&Panel";
            this.miShowPanelEnabled.Index = 0;
            this.miShowPanelEnabled.Text = "V&isible";
            this.miShowPanelEnabled.Shortcut = Shortcut.CtrlF1;
            this.miShowPanelEnabled.Checked = this.ShowPanel;
            this.miShowPanelEnabled.Click += new EventHandler(miShowPanelEnabled_Click);

            // Add our menu option before the last option (Help) to keep Windows consistency
            FiddlerApplication.UI.mnuMain.MenuItems.Add(FiddlerApplication.UI.mnuMain.MenuItems.Count - 1, mnuShowPanel);

            // Initialize images from the embedded resource file
            this.imageList = new ImageList();
            this.imageList.Images.Add(Resources._1403_Globe_16x16);
            this.imageList.Images.Add(Resources._1409_Monitor_16x16); // Host
            this.imageList.Images.Add(Resources.Folder_16x16); // Closed folder
            this.imageList.Images.Add(Resources.FolderOpen_16x16_72); // Open folder
            this.imageList.Images.Add(Resources._109_AllAnnotations_Default_16x16_72); // 200
            this.imageList.Images.Add(Resources._109_AllAnnotations_donotenter_16x16); // 401
            this.imageList.Images.Add(Resources.generic_picture_16x16); // Images
        }

        /// <summary>
        /// Menu option to show/hide the panel
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">EventArgs</param>
        private void miShowPanelEnabled_Click(object sender, EventArgs e)
        {
            this.miShowPanelEnabled.Checked = !this.miShowPanelEnabled.Checked;
            this.ShowPanel = this.miShowPanelEnabled.Checked;
            this.ShowHidePanel();
        }

        /// <summary>
        /// Shows or hides original Fiddler session container or the TreeView
        /// </summary>
        private void ShowHidePanel()
        {
            tv.Visible = this.ShowPanel;
            lvSessions.Visible = !this.ShowPanel;
        }

        /// <summary>
        /// Resize the TreeView when the panel changes it size
        /// TODO: This might not work correctly
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">EventArgs</param>
        private void pnlSessions_SizeChanged(object sender, EventArgs e)
        {
            tv.Height = FiddlerApplication.UI.pnlSessions.Height;
            tv.Width = FiddlerApplication.UI.pnlSessions.Width;
        }

        /// <summary>
        /// Selecting an item in the TreeView will select the corresponding Session in the ListView
        /// so the information is sent to the inspector panels, etc...
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">TreeViewEventArgs</param>
        private void tv_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // First clear selection. If no valid session treenode was selected, it won't display anything :-)
            lvSessions.SelectedItems.Clear();
            Session oSession = e.Node.Tag as Session;
            if (oSession != null)
            {
                foreach (ListViewItem lvItem in lvSessions.Items)
                {
                    Session hiddenSession = lvItem.Tag as Session;
                    lvItem.Selected = (oSession.id == hiddenSession.id) && !lvItem.Selected; // This latter check is to avoid refiring events when item got selected
                }
            }
        }
        
        /// <summary>
        /// After expanding a node, change the icon if it's a folder
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">TreeViewEventArgs</param>
        private void tv_AfterExpand(object sender, TreeViewEventArgs e)
        {
            if (e.Node.ImageIndex == 2)
            {
                e.Node.ImageIndex = 3;
                e.Node.SelectedImageIndex = 3;
            }
        }

        /// <summary>
        /// After collapsing a node, change the icon if it's a folder
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">TreeViewEventArgs</param>
        private void tv_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            if (e.Node.ImageIndex == 3)
            {
                e.Node.ImageIndex = 2;
                e.Node.SelectedImageIndex = 2;
            }
        }

        /// <summary>
        /// Delete sessions from the TreeView
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">ToolStripItemClickedEventArgs</param>
        void btnRemove_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Text.ToLower())
            {
                case "all":
                    tv.Nodes.Clear();
                    break;
                case "images":
                    break;
                case "connects":
                    break;
                case "non-200s":
                    break;
                default:
                    // Other possible things to do here
                    break;
            }
        }


        #endregion "Methods"

        #region "IFiddlerExtension Members"

        /// <summary>
        /// Initialization after the extension loads
        /// </summary>
        public void OnLoad()
        {
            // Locate the original sessions listview
            lvSessions = FiddlerApplication.UI.pnlSessions.Controls[Constants.ListViewName] as SessionListView;
            FiddlerApplication.UI.pnlSessions.SizeChanged += new EventHandler(pnlSessions_SizeChanged);

            // Create our TreeView
            tv = new TreeView();
            tv.Name = Constants.TreeViewName;
            tv.ImageList = this.imageList;
            tv.Location = lvSessions.Location;
            tv.Size = lvSessions.Size;
            tv.ContextMenu = FiddlerApplication.UI.mnuSessionContext; // Assign same context menu
            tv.AfterSelect += new TreeViewEventHandler(tv_AfterSelect);
            tv.AfterCollapse += new TreeViewEventHandler(tv_AfterCollapse);
            tv.AfterExpand += new TreeViewEventHandler(tv_AfterExpand);
            FiddlerApplication.UI.pnlSessions.Controls.Add(tv);

            // TODO: Avoid hardcoded reference to the actual button
            foreach (Control c in FiddlerApplication.UI.Controls)
            {
                if (c is ToolStrip)
                {
                    ToolStrip menuBar = c as ToolStrip;
                    foreach (ToolStripItem btn in menuBar.Items)
                    {
                        if (btn.Text.Equals(Constants.RemoveButtonText, StringComparison.CurrentCultureIgnoreCase))
                        {
                            ToolStripDropDownButton btnRemove = btn as ToolStripDropDownButton;
                            btnRemove.DropDownItemClicked += new ToolStripItemClickedEventHandler(btnRemove_DropDownItemClicked);
                        }
                    }

                }
            }

            // Show or hide depending on the registry value
            this.ShowHidePanel();
        }

        /// <summary>
        /// Before unloading the extension
        /// </summary>
        public void OnBeforeUnload()
        {
            // Save the persistent values to the registry
            RegistryKey oReg = Registry.CurrentUser.CreateSubKey(String.Format(@"{0}\{1}\", CONFIG.GetRegPath("Root"), Constants.RegistryExtensionKeyName));
            int value = this.ShowPanel ? 1 : 0;
            oReg.SetValue(Constants.RegistryShowPanel, value, RegistryValueKind.DWord);
            oReg.Close();
        }
        
        #endregion "IFiddlerExtension Members"
    }
}