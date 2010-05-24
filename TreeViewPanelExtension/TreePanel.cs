using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Fiddler;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Drawing;
using System.ComponentModel;

//TODO: Allow multiple selection of nodes in TreeView (custom control)

namespace TreeViewPanelExtension
{
    /// <summary>
    /// Creates a new control to be used in Fiddler, that represents the information in a TreeView
    /// </summary>
    public class TreePanel : IFiddlerExtension
    {
        #region "Variables"

        ListViewHook _lvHook = new ListViewHook();

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
        /// 
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">EventArgs</param>
        private void lvSessions_SizeChanged(object sender, EventArgs e)
        {
            //TreeView tv = FiddlerApplication.UI.pnlSessions.Controls[Constants.TreeViewName] as TreeView;
            //if (tv != null)
            //{
            //    tv.Location = ((SessionListView)sender).Location;
            //    tv.Size = ((SessionListView)sender).Size;
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">EventArgs</param>
        private void pnlSessions_SizeChanged(object sender, EventArgs e)
        {
            // TODO: This might not be correct
            //tv.Location = ((SessionListView)sender).Location;
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
        
        //private void lvSessions_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        //{
        //    Session oSession = e.Item.Tag as Session;
        //    if (oSession != null && e.IsSelected)
        //    {
        //        TreeNode tn = this.FindNode(oSession);
        //        if (tn != null)
        //            tv.SelectedNode = tn;
        //    }
        //    //tv.
        //    //e.IsSelected
        //    //throw new NotImplementedException();
        //}

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
        /// When right-clicking a button, select the tree node so we get the correct context menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tv_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                TreeNode tn = tv.GetNodeAt(e.Location);
                if (tn != null) tv.SelectedNode = tn;
                //if (tn != null) this.DeleteNode(tn);
            }
        }

        private TreeNode FindNode(Session s)
        {
            foreach (TreeNode host in tv.Nodes)
            {
                TreeNode found = this.FindNode(s, host);
                if (found != null) return found;
            }
            return null;
        }

        private TreeNode FindNode(Session s, TreeNode tn)
        {
            foreach (TreeNode child in tn.Nodes)
            {
                Session oSession = child.Tag as Session;
                if (oSession != null && oSession.id == s.id) return child;
            }
            return null;
        }

        private void DeleteNode(TreeNode tn)
        {
            // TODO: Implement clearing of items from the treeview
            if (tn.Parent != null)
            {
                TreeNode parent = tn.Parent;
                parent.Nodes.Remove(tn);
                if (parent.Nodes.Count == 0) this.DeleteNode(parent);
            }
            else
            {
                if (tn.Nodes.Count == 0) tv.Nodes.Remove(tn);
            }
        }

        //private void lvSessions_Invalidated(object sender, InvalidateEventArgs e)
        //{
        //    foreach (ListViewItem lvItem in lvSessions.Items)
        //    {
        //        Session oSession = lvItem.Tag as Session;
        //        if (oSession != null)
        //        {
        //        }
        //    }
        //}

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
            tv.Nodes.Clear();
        }

        #endregion "Methods"

        #region "IFiddlerExtension Members"

        /// <summary>
        /// Initialization after the extension loads
        /// </summary>
        public void OnLoad()
        {
            //this._lvHook.Install();

            // Locate the original sessions listview
            lvSessions = FiddlerApplication.UI.pnlSessions.Controls[Constants.ListViewName] as SessionListView;

            // lvSessions.Items = new ListViewItemCollectionWithEvents(lvSessions);

            //lvSessions.ItemSelectionChanged += new ListViewItemSelectionChangedEventHandler(lvSessions_ItemSelectionChanged);
            //lvSessions.SizeChanged += new EventHandler(lvSessions_SizeChanged);
            // TODO: See status of deleted items
            //lvSessions.Invalidated += new InvalidateEventHandler(lvSessions_Invalidated);
            //lvSessions.ItemActivate += new EventHandler(lvSessions_ItemActivate);
            //lvSessions.ItemChecked += new ItemCheckedEventHandler(lvSessions_ItemChecked);
            FiddlerApplication.UI.pnlSessions.SizeChanged += new EventHandler(pnlSessions_SizeChanged);

            // Create our TreeView
            tv = new TreeView();
            //tv.Tag = new List<TreeNode>(); // Holds the list of nodes for faster lookup
            //tv.Dock = DockStyle.Fill;
            tv.Name = Constants.TreeViewName;
            tv.ImageList = this.imageList;
            tv.Location = lvSessions.Location;
            tv.Size = lvSessions.Size;
            tv.ContextMenu = FiddlerApplication.UI.mnuSessionContext; // Assign same context menu
            tv.MouseClick += new MouseEventHandler(tv_MouseClick);
            tv.AfterSelect += new TreeViewEventHandler(tv_AfterSelect);
            tv.AfterCollapse += new TreeViewEventHandler(tv_AfterCollapse);
            tv.AfterExpand += new TreeViewEventHandler(tv_AfterExpand);
            FiddlerApplication.UI.pnlSessions.Controls.Add(tv);

            ToolStrip menuBar = FiddlerApplication.UI.Controls[4] as ToolStrip;
            ToolStripDropDownButton btnRemove = menuBar.Items[2] as ToolStripDropDownButton;
            btnRemove.DropDownItems[0].Click += new EventHandler(btnRemoveAll_Click);
            //ALL
            //Images
            //Connects
            //Non-200

            // Show or hide depending on the value
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

            //this._lvHook.Uninstall();
        }
        
        #endregion "IFiddlerExtension Members"


        //private const int LVM_FIRST = 0x1000; // ListView messages
        //private const int LVM_DELETEALLITEMS = (LVM_FIRST + 9);

        //protected override void WndProc(ref Message m)
        //{
        //    switch (m.Msg)
        //    {
        //        case LVM_DELETEALLITEMS:
        //            tv.Nodes.Clear();
        //            break;
        //        default:
        //            // Nothing
        //            break;
        //    }
        //    base.WndProc(ref m);
        //}
    }
}