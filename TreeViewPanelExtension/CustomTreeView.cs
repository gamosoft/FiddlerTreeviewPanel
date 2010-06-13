using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TreeViewPanelExtension
{
    /// <summary>
    /// New TreeView implementation to eliminate default double-click behavior (which expands/collapses the selected node)
    /// http://www.developersdex.com/gurus/code/831.asp
    /// </summary>
    public class CustomTreeView : TreeView
    {
        #region "Variables"

        /// <summary>
        /// Value for mouse double-click
        /// </summary>
        private const int WM_LBUTTONDBLCLK = 0x203; // 515

        #endregion "Variables"

        #region "Methods"

        /// <summary>
        /// Override this method to do nothing when double-click
        /// </summary>
        /// <param name="m">Window message</param>
        protected override void DefWndProc(ref Message m)
        {
            if (m.Msg == WM_LBUTTONDBLCLK)
            {
                // Do nothing
            }
            else
            {
                base.DefWndProc(ref m);
            }
        }

        #endregion "Methods"
    }
}