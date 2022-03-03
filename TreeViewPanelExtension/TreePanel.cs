using System;
using System.Windows.Forms;
using Fiddler;
using Microsoft.Win32;
using System.Drawing;

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
        CustomTreeView tv = null;

        /// <summary>
        /// OOB list view that handles original Fiddler sessions
        /// </summary>
        SessionListView lvSessions = null;

        /// <summary>
        /// New menu item
        /// </summary>
        private System.Windows.Forms.MenuItem mnuShowPanel;

        /// <summary>
        /// New menu item to show/hide the panel
        /// </summary>
        private System.Windows.Forms.MenuItem miShowPanelEnabled;

        /// <summary>
        /// New item to show about information
        /// </summary>
        private System.Windows.Forms.MenuItem miAbout;

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
            this.miAbout = new System.Windows.Forms.MenuItem();
            this.mnuShowPanel.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] { this.miShowPanelEnabled, this.miAbout });

            this.mnuShowPanel.Text = "TreeView&Panel";
            this.miShowPanelEnabled.Index = 0;
            this.miShowPanelEnabled.Text = "V&isible";
            this.miShowPanelEnabled.Shortcut = Shortcut.CtrlF1;
            this.miShowPanelEnabled.Checked = this.ShowPanel;
            this.miShowPanelEnabled.Click += new EventHandler(miShowPanelEnabled_Click);

            this.miAbout.Index = 1;
            this.miAbout.Text = "&About...";
            this.miAbout.Click += new EventHandler(miAbout_Click);

            // Add our menu option before the last option (Help) to keep Windows consistency
            FiddlerApplication.UI.mnuMain.MenuItems.Add(FiddlerApplication.UI.mnuMain.MenuItems.Count - 1, mnuShowPanel);

            // Initialize images from the embedded resource file
            this.imageList = new ImageList();
            this.imageList.Images.Add(Constants.ImageKeyGlobe, Resources._1403_Globe_16x16);
            this.imageList.Images.Add(Constants.ImageKeyHost, Resources._1409_Monitor_16x16); // Host
            this.imageList.Images.Add(Constants.ImageKeyClosedFolder, Resources.Folder_16x16); // Closed folder
            this.imageList.Images.Add(Constants.ImageKeyOpenFolder, Resources.FolderOpen_16x16_72); // Open folder
            this.imageList.Images.Add(Constants.ImageKeyHttpCode200, Resources._109_AllAnnotations_Default_16x16_72); // 200, OK
            this.imageList.Images.Add(Constants.ImageKeyHttpCode301, Resources._010_LowPriority_16x16_72); // 301, Moved permanently
            this.imageList.Images.Add(Constants.ImageKeyHttpCode304, Resources._109_AllAnnotations_Complete_16x16_72); // 304, Unmodified
            this.imageList.Images.Add(Constants.ImageKeyHttpCode401, Resources._109_AllAnnotations_donotenter_16x16); // 401, Forbidden
            this.imageList.Images.Add(Constants.ImageKeyHttpCode404, Resources._109_AllAnnotations_Error_16x16_72); // 404, Not found
            this.imageList.Images.Add(Constants.ImageKeyGenericImage, Resources.generic_picture_16x16); // Images
            this.imageList.Images.Add(Constants.ImageKeyGenericImageVisited, Resources.generic_picture_16x16_visited); // Images
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
        /// Show an small about dialog box
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">EventArgs</param>
        private void miAbout_Click(object sender, EventArgs e)
        {
            using (AboutForm about = new AboutForm())
            {
                about.ShowDialog();
            }
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
        /// Before a node is selected, change the previously selected node colors
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">TreeViewCancelEventArgs</param>
        private void tv_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (tv.SelectedNode != null)
            {
                tv.SelectedNode.ForeColor = tv.ForeColor;
                tv.SelectedNode.BackColor = tv.BackColor;
            }
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
        /// When the treeview loses focus, highlight the currently selected node (if any)
        /// so it's easier to track
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">EventArgs</param>
        private void tv_LostFocus(object sender, EventArgs e)
        {
            if (tv.SelectedNode != null)
            {
                tv.SelectedNode.ForeColor = SystemColors.HighlightText;
                tv.SelectedNode.BackColor = SystemColors.Highlight;
            }
        }

        /// <summary>
        /// After expanding a node, change the icon if it's a folder
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">TreeViewEventArgs</param>
        private void tv_AfterExpand(object sender, TreeViewEventArgs e)
        {
            if (e.Node.ImageKey == Constants.ImageKeyClosedFolder)
            {
                e.Node.ImageKey = Constants.ImageKeyOpenFolder;
                e.Node.SelectedImageKey = Constants.ImageKeyOpenFolder;
            }
        }

        /// <summary>
        /// Double clicking a node will expand/collapse all child nodes
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">TreeNodeMouseClickEventArgs</param>
        private void tv_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.IsExpanded)
            {
                e.Node.Collapse(false);
            }
            else
            {
                e.Node.ExpandAll();
            }
        }

        /// <summary>
        /// After collapsing a node, change the icon if it's a folder
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">TreeViewEventArgs</param>
        private void tv_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            if (e.Node.ImageKey == Constants.ImageKeyOpenFolder)
            {
                e.Node.ImageKey = Constants.ImageKeyClosedFolder;
                e.Node.SelectedImageKey = Constants.ImageKeyClosedFolder;
            }
        }

        /// <summary>
        /// Intercepts keystrokes
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">KeyEventArgs</param>
        private void tv_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Delete:
                    MessageBox.Show("Delete");
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Intercepts keystrokes
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">PreviewKeyDownEventArgs</param>
        private void tv_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Delete:
                    if (lvSessions.SelectedItems.Count != 0)
                    {
                        MessageBox.Show(e.Shift.ToString());
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Delete sessions from the TreeView
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">ToolStripItemClickedEventArgs</param>
        private void btnRemove_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Text.ToLower())
            {
                case "remove all":
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

            lvSessions.Invalidated += LvSessions_Invalidated;

            // Create our TreeView
            tv = new CustomTreeView();
            tv.Name = Constants.TreeViewName;
            tv.ImageList = this.imageList;
            tv.Location = lvSessions.Location;
            tv.Size = lvSessions.Size;
            tv.HideSelection = false; // If "false" the highlight is too "light", so we'll keep the selection "manually"
            tv.ContextMenu = FiddlerApplication.UI.mnuSessionContext; // Assign same context menu
            //tv.BeforeSelect += new TreeViewCancelEventHandler(tv_BeforeSelect);
            //tv.LostFocus += new EventHandler(tv_LostFocus);
            tv.AfterSelect += new TreeViewEventHandler(tv_AfterSelect);
            tv.AfterCollapse += new TreeViewEventHandler(tv_AfterCollapse);
            tv.AfterExpand += new TreeViewEventHandler(tv_AfterExpand);
            tv.NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(tv_NodeMouseDoubleClick);
            //tv.KeyDown += new KeyEventHandler(tv_KeyDown);
            //tv.PreviewKeyDown += new PreviewKeyDownEventHandler(tv_PreviewKeyDown);
            FiddlerApplication.UI.pnlSessions.Controls.Add(tv);

            // Show or hide depending on the registry value
            this.ShowHidePanel();

            // TODO: Avoid hardcoded reference to the actual button
            // TODO: Research possible error executing the following (code after this would not execute)
            foreach (Control c in FiddlerApplication.UI.Controls)
            {
                if (c is ToolStrip)
                {
                    ToolStrip menuBar = c as ToolStrip;
                    foreach (ToolStripItem btn in menuBar.Items)
                    {
                        //if (btn.Text.Equals(Constants.RemoveButtonText, StringComparison.CurrentCultureIgnoreCase))
                        //if (btn.ToolTipText.Equals(Constants.RemoveButtonToolTipText, StringComparison.CurrentCultureIgnoreCase))
                        if (btn.ImageKey.Equals(Constants.RemoveButtonImageKey, StringComparison.CurrentCultureIgnoreCase))
                        {
                            ToolStripDropDownButton btnRemove = btn as ToolStripDropDownButton;
                            btnRemove.DropDownItemClicked += new ToolStripItemClickedEventHandler(btnRemove_DropDownItemClicked);
                        }
                    }
                }
            }
        }

        private void LvSessions_Invalidated(object sender, InvalidateEventArgs e)
        {
            var sessionsList = sender as SessionListView;
            if (sessionsList == null)
                return;

            // Hack to clear the treeview panel if the original list has been cleared and invalidated
            if (sessionsList.Items.Count == 0)
                this.tv.Nodes.Clear();
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