using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace TreeViewPanelExtension
{
    public class ListViewItemCollectionWithEvents : System.Windows.Forms.ListView.ListViewItemCollection
    {
        private ListView parent;

        public delegate void ListViewItemDelegate(ListViewItem item);
        public delegate void ListViewItemRangeDelegate(ListViewItem[] item);
        public delegate void ListViewRemoveDelegate(ListViewItem item);
        public delegate void ListViewRemoveAtDelegate(int index, ListViewItem item);

        //Next come the event declarations:

        public event ListViewItemDelegate ItemAdded;
        public event ListViewItemRangeDelegate ItemRangeAdded;
        public event ListViewRemoveDelegate ItemRemoved;
        public event ListViewRemoveAtDelegate ItemRemovedAt;

        public ListViewItemCollectionWithEvents(System.Windows.Forms.ListView owner)
            : base(owner)
        {
            parent = owner;
        }
        public new void Add(ListViewItem Lvi)
        {
            base.Add(Lvi);
            //((ListViewNewEventCs)parent).AddedItem(Lvi);
        }
        public new void AddRange(ListViewItem[] Lvi)
        {
            base.AddRange(Lvi);
            //((ListViewNewEventCs)parent).AddedItemRange(Lvi);
        }
        public new void Remove(ListViewItem Lvi)
        {
            base.Remove(Lvi);
            //((ListViewNewEventCs)parent).RemovedItem(Lvi);
        }
        public new void RemoveAt(int index)
        {
            System.Windows.Forms.ListViewItem lvi = this[index];
            base.RemoveAt(index);
            //((ListViewNewEventCs)parent).RemovedItem(index, lvi);
        }
    }
}