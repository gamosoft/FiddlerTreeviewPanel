using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fiddler;
using System.Windows.Forms;


namespace TreeViewPanelExtension
{
    /// <summary>
    /// Class to generate the nodes using the Fiddler requests/responses
    /// </summary>
    public class TreeViewGenerator : IAutoTamper    // Ensure class is public, or Fiddler won't see it!
    {

        #region "Variables"
        
        //string sUserAgent = "";
        
        #endregion "Variables"

        #region "Properties"

        #endregion "Properties"

        #region "Methods"

        /// <summary>
        /// Default constructor
        /// </summary>
        public TreeViewGenerator()
        {
            /* NOTE: It's possible that Fiddler UI isn't fully loaded yet, so don't add any UI in the constructor.

               But it's also possible that AutoTamper* methods are called before OnLoad (below), so be
               sure any needed data structures are initialized to safe values here in this constructor */

            //string sUserAgent = "Violin";
        }

        public delegate void Add(TreeView tv, Session oSession);

        public void Add1(TreeView tv, Session oSession)
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
                    //oSession.fullUrl;
                    //oSession.host;
                    //oSession.hostname;
                    //oSession.id;
                    //oSession.isFTP;
                    //oSession.isHTTPS;
                    //oSession.isTunnel;
                    //oSession.PathAndQuery;
                    //oSession.port;
                    //oSession.responseCode;
                    //oSession.state = SessionStates.Done;
                    //oSession.url;

                    //oSession.port
                    result = host;
                    break; // Exit foreach
                }
            }

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

            //string[] folders = oSession.PathAndQuery.Split(new string[] { @"\", "/" }, StringSplitOptions.RemoveEmptyEntries);

            string nodeText = "/";

            if (!String.IsNullOrEmpty(contents[0]))
            {
                string[] folders = contents[0].Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries); ;

                if (folders.Length > 1)
                {
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

                if (folders.Length > 0)
                    nodeText = folders[folders.Length - 1];
            }

            TreeNode childNode = new TreeNode();
            //childNode.Text = oSession.PathAndQuery;
            childNode.Text = nodeText;
            if (contents.Length > 1)
            {
                childNode.Text += "?" + contents[1];
            }
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

            // New to add the node to a separate list for faster performance
            //List<TreeNode> nodes = tv.Tag as List<TreeNode>;
            //nodes.Add(childNode);
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
                    tv.Invoke(new Add(Add1), new object[] { tv, oSession });
                }
            }
        }

        public void OnLoad() { }
        public void OnBeforeUnload() { }
        public void AutoTamperRequestBefore(Session oSession) { } // oSession.oRequest["User-Agent"] = sUserAgent;
        public void AutoTamperRequestAfter(Session oSession) { }
        public void AutoTamperResponseBefore(Session oSession) { }
        public void OnBeforeReturningError(Session oSession) { }
        
        #endregion "IAutoTamper"
    }
}