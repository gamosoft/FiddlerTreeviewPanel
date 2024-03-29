﻿using Fiddler;
using System;
using System.Linq;
using System.Windows.Forms;

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
        /// <param name="tv">CustomTreeView</param>
        /// <param name="oSession">Fiddler session</param>
        public delegate void Add(CustomTreeView tv, Session oSession);

        /// <summary>
        /// Method that adds a session to a TreeView
        /// </summary>
        /// <param name="tv">CustomTreeView</param>
        /// <param name="oSession">Fiddler session</param>
        public void AddSession(CustomTreeView tv, Session oSession)
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
                result.ImageKey = Constants.ImageKeyHost;
                result.SelectedImageKey = Constants.ImageKeyHost;
            }

            string[] contents = oSession.PathAndQuery.Split(new char[] { '?' });
            string justPath = (contents.Length > 1) ? contents[0] : oSession.PathAndQuery;

            // Default node text to be displayed
            string nodeText = $"[{oSession.id}] /";

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
                            result.ImageKey = Constants.ImageKeyClosedFolder;
                            result.SelectedImageKey = Constants.ImageKeyClosedFolder;
                        }
                        else
                        {
                            result = found;
                        }
                    }
                }
                // The last one will be the actual page, image, script, etc...
                if (folders.Length > 0)
                    nodeText = $"[{oSession.id}] {folders[folders.Length - 1]}";
            }

            // Effectively create new node with the information so far
            TreeNode childNode = new TreeNode();
            childNode.Text = nodeText;
            if (contents.Length > 1)
                childNode.Text += "?" + contents[1];

            childNode.Tag = oSession; // Store the Session here as well
            childNode.ToolTipText = oSession.fullUrl;

            string nodeImage = Constants.ImageKeyHttpCode200;
            switch (oSession.responseCode)
            {
                case 200: // OK
                    if (Constants.ImageExtensions.Any(ext => nodeText.ToLower().EndsWith(ext)))
                    {
                        nodeImage = Constants.ImageKeyGenericImage;
                    }
                    break;
                case 301: // Moved permanently
                    nodeImage = Constants.ImageKeyHttpCode301;
                    break;
                case 304: // Unmodified
                    if (Constants.ImageExtensions.Any(ext => nodeText.ToLower().EndsWith(ext)))
                    {
                        nodeImage = Constants.ImageKeyGenericImageVisited;
                    }
                    else
                    {
                        nodeImage = Constants.ImageKeyHttpCode304;
                    }
                    break;
                case 401: // Unauthorized
                    nodeImage = Constants.ImageKeyHttpCode401;
                    break;
                case 404: // Not found
                    nodeImage = Constants.ImageKeyHttpCode404;
                    break;
                default:
                    break;
            }

            childNode.ImageKey = nodeImage;
            childNode.SelectedImageKey = nodeImage;

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
            if (oSession.ViewItem == null)
                return;

            CustomTreeView tv = FiddlerApplication.UI.pnlSessions.Controls[Constants.TreeViewName] as CustomTreeView;
            if (tv == null)
                return;

            tv.Invoke(new Add(AddSession), new object[] { tv, oSession });
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