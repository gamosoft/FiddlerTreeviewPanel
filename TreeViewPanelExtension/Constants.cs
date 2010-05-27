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

        #endregion "Variables"
    }
}