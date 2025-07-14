using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Wa3Tuner.Helper_Classes
{
    internal static class ListboxHelper
    {
        public static  void ReverseListBoxSelection(ListBox listBox)
        {
            if (listBox == null) return;

            // Store currently selected items in a HashSet for fast lookup
            var selectedItems = new HashSet<object>(listBox.SelectedItems.Cast<object>());

            listBox.SelectedItems.Clear(); // Clear the current selection

            // Select items that were previously unselected
            foreach (var item in listBox.Items)
            {
                if (!selectedItems.Contains(item))
                {
                    listBox.SelectedItems.Add(item);
                }
            }
        }
    }
}
