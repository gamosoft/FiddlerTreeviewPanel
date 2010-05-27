using System;
using System.Linq;
using System.Windows.Forms;
using Fiddler;

namespace TreeViewPanelExtension
{
    /// <summary>
    /// Class to generate the nodes using the Fiddler requests/responses
    /// </summary>
    public class TreeViewGenerator : IAutoTamper
    {
        #region "Methods"

        /// <summary>
        /// Default constructor
        /// </summary>
        public TreeViewGenerator()
        {
            /* NOTE: It's possible that Fiddler UI isn't fully loaded yet, so don't add any UI in the constructor.

               But it's also possible that AutoTamper* methods are called before OnLoad (below), so be
               sure any needed data structures are initialized to safe values here in this constructor */
        }

        /// <summary>
        /// Delegate to add nodes to the TreeView from another thread
        /// </summary>
        /// <param name="tv">TreeView</param>
        /// <param name="oSession">Fiddler session</param>
        public delegate void Add(TreeView tv, Session oSession);

        /// <summary>
        /// Method that adds a session to a TreeView
        /// </summary>
        /// <param name="tv">TreeView</param>
        /// <param name="oSession">Fiddler session</param>
        public void AddSession(TreeView tv, Session oSession)
        {
            string hostName = oSession.hostname;
            if (oSession.isHTTPS)
            {
                hostName = "https://" + hostName;
                if (oSession.port != 443) hostName += ":" + oSession.port.ToString();
            }
            else if (oSession.isFTP)
            {
                hostName = "ftp://" + hostName;
                if (oSession.port != 21) hostName += ":" + oSession.port.ToString();
            }
            else
            {
                hostName = "http://" + hostName;
                if (oSession.port == 443)
                {
                    hostName = "https://" + oSession.hostname;
                }
                else if (oSession.port != 80)
                {
                    hostName += ":" + oSession.port.ToString();
                }
            }

            TreeNode result = null;
            // First locate the host
            foreach (TreeNode host in tv.Nodes)
            {
                if (host.Text == hostName)
                {
                    result = host;
                    break; // Exit foreach
                }
            }
            // Not found, create a new one so we nest following sessions to the same host below
            if (result == null)
            {
                result = tv.Nodes.Add(hostName);
                result.ImageIndex = 1;
                result.SelectedImageIndex = 1;
            }

            string[] contents = oSession.PathAndQuery.Split(new char[] { '?' });
            string justPath = "";
            if (contents.Length > 1)
            {
                justPath = contents[0];
            }
            else
            {
                justPath = oSession.PathAndQuery;
            }

            // Default node text to be displayed
            string nodeText = "/";

            if (!String.IsNullOrEmpty(contents[0]))
            {
                string[] folders = contents[0].Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries); ;

                if (folders.Length > 1)
                {
                    // Go "down" in the hierarchy to locate the last subfolder where the session needs to be placed
                    for (int i = 0; i < folders.Length - 1; i++)
                    {
                        TreeNode found = null;
                        foreach (TreeNode child in result.Nodes)
                        {
                            if (child.Text == folders[i])
                            {
                                found = child;
                                break; // Exit foreach
                            }
                        }

                        if (found == null)
                        {
                            result = result.Nodes.Add(folders[i]);
                            result.ImageIndex = 2;
                            result.SelectedImageIndex = 2;
                        }
                        else
                        {
                            result = found;
                        }
                    }
                }
                // The last one will be the actual page, image, script, etc...
                if (folders.Length > 0)
                    nodeText = folders[folders.Length - 1];
            }

            // Effectively create new node with the information so far
            TreeNode childNode = new TreeNode();
            childNode.Text = nodeText;
            if (contents.Length > 1)
                childNode.Text += "?" + contents[1];

            childNode.Tag = oSession; // Store the Session here as well
            childNode.ToolTipText = oSession.fullUrl;

            int nodeImage = 4;
            switch (oSession.responseCode)
            {
                case 401: // Unauthorized
                    nodeImage = 5;
                    break;
                default:
                    break;
            }

            if (Constants.ImageExtensions.Any(ext => nodeText.ToLower().EndsWith(ext)))
            {
                nodeImage = 6;
            }

            childNode.ImageIndex = nodeImage;
            childNode.SelectedImageIndex = nodeImage;

            result.Nodes.Add(childNode);
        }

        #endregion "Methods"

        #region "IAutoTamper"

        /// <summary>
        /// After the response is retrieved, add it to the tree
        /// </summary>
        /// <param name="oSession">Returned session</param>
        public void AutoTamperResponseAfter(Session oSession)
        {
            // If it's null, it will have been intercepted by the filters
            if (oSession.ViewItem != null)
            {
                TreeView tv = FiddlerApplication.UI.pnlSessions.Controls[Constants.TreeViewName] as TreeView;
                if (tv != null)
                {
                    tv.Invoke(new Add(AddSession), new object[] { tv, oSession });
                }
            }
        }

        // Methods not used from the interface
        public void OnLoad() { }
        public void OnBeforeUnload() { }
        public void AutoTamperRequestBefore(Session oSession) { }
        public void AutoTamperRequestAfter(Session oSession) { }
        public void AutoTamperResponseBefore(Session oSession) { }
        public void OnBeforeReturningError(Session oSession) { }
        
        #endregion "IAutoTamper"
    }
}