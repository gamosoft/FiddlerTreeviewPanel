using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TreeViewPanelExtension
{
    public enum ListViewHookAction : int
	{
        WM_COMPAREITEM = 0x39,
        WM_DELETEITEM = 0x2D
        //HCBT_MOVESIZE = 0,
        //HCBT_MINMAX = 1,
        //HCBT_QS = 2,
        //HCBT_CREATEWND = 3,
        //HCBT_DESTROYWND = 4,
        //HCBT_ACTIVATE = 5,
        //HCBT_CLICKSKIPPED = 6,
        //HCBT_KEYSKIPPED = 7,
        //HCBT_SYSCOMMAND = 8,
        //HCBT_SETFOCUS = 9
	}

	public class ListViewEventArgs : EventArgs
	{
		public IntPtr Handle;			// Win32 handle of the window
		public string Title;			// caption of the window
		public string ClassName;		// class of the window
		public bool IsDialogWindow;		// whether is a popup dialog
	}

    public class ListViewHook : LocalWindowsHook
    {
        public delegate void ListViewEventHandler(object sender, ListViewEventArgs e);


        // ************************************************************************
		// Internal properties
		protected IntPtr m_hwnd = IntPtr.Zero;
		protected string m_title = "";
		protected string m_class = "";
		protected bool m_isDialog = false;
		// ************************************************************************

        // Class constructor(s)
        public ListViewHook()
            : base(HookType.WH_CALLWNDPROCRET)
		{
			this.HookInvoked += new HookEventHandler(ListViewHookInvoked);
		}
        public ListViewHook(HookProc func)
            : base(HookType.WH_CALLWNDPROCRET, func)
		{
			this.HookInvoked += new HookEventHandler(ListViewHookInvoked);
		}

        private void ListViewHookInvoked(object sender, HookEventArgs e)
		{
            ListViewHookAction code = (ListViewHookAction)e.HookCode; 
			IntPtr wParam = e.wParam; 
			IntPtr lParam = e.lParam;

            //// Handle hook events (only a few of available actions)
            switch (code)
            {
                case ListViewHookAction.WM_DELETEITEM:
                    System.Windows.Forms.MessageBox.Show("Session deleted");
                    //HandleCreateWndEvent(wParam, lParam);
                    break;
            //    case ListViewHookAction.HCBT_DESTROYWND:
            //        //HandleDestroyWndEvent(wParam, lParam);
            //        break;
            //    case ListViewHookAction.HCBT_ACTIVATE:
            //        //HandleActivateEvent(wParam, lParam);
            //        break;
                default:
                    break;
            }
			
			return;
		}

    }
}
