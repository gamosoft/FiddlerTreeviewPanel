using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TreeViewPanelExtension
{
    /// <summary>
    /// Class to hold constants
    /// </summary>
    public static class Constants
    {
        #region "Variables"

        /// <summary>
        /// List of allowed image extensions
        /// </summary>
        public static List<string> ImageExtensions = new List<string>() { "png", "gif", "jpg", "jpeg" };

        /// <summary>
        /// Registry key name (HKCU\Software\Microsoft\Fiddler2\TreeViewPanelExtension
        /// </summary>
        public static string RegistryExtensionKeyName = "TreeViewPanelExtension";

        /// <summary>
        /// Name of the entry that store the visibility of the panel
        /// </summary>
        public static string RegistryShowPanel = "ShowPanel";

        /// <summary>
        /// Name of OOB Fiddlers ListView
        /// </summary>
        public static string ListViewName = "lvSessions";

        /// <summary>
        /// Name of the TreeView
        /// </summary>
        public static string TreeViewName = "sessionsTV";

        /// <summary>
        /// Text of the remove button
        /// </summary>
        public static string RemoveButtonText = "Remove";

        /// <summary>
        /// ImageKey of the remove button
        /// </summary>
        public static string RemoveButtonImageKey = "remove";

        /// <summary>
        /// Image key for the globe
        /// </summary>
        public static string ImageKeyGlobe = "Globe";

        /// <summary>
        /// Image key for a host
        /// </summary>
        public static string ImageKeyHost = "Host";

        /// <summary>
        /// Image key for a closed folder
        /// </summary>
        public static string ImageKeyClosedFolder = "ClosedFolder";

        /// <summary>
        /// Image key for an open folder
        /// </summary>
        public static string ImageKeyOpenFolder = "OpenFolder";

        /// <summary>
        /// Image key for http code 200
        /// </summary>
        public static string ImageKeyHttpCode200 = "HttpCode200";

        /// <summary>
        /// Image key for http code 301
        /// </summary>
        public static string ImageKeyHttpCode301 = "HttpCode301";

        /// <summary>
        /// Image key for http code 304
        /// </summary>
        public static string ImageKeyHttpCode304 = "HttpCode304";

        /// <summary>
        /// Image key for http code 401
        /// </summary>
        public static string ImageKeyHttpCode401 = "HttpCode401";

        /// <summary>
        /// Image key for http code 404
        /// </summary>
        public static string ImageKeyHttpCode404 = "HttpCode404";

        /// <summary>
        /// Image key for a generic image
        /// </summary>
        public static string ImageKeyGenericImage = "GenericImage";

        /// <summary>
        /// Image key for a generic image (visited, with 304 code)
        /// </summary>
        public static string ImageKeyGenericImageVisited = "GenericImageVisited";

        #endregion "Variables"
    }
}