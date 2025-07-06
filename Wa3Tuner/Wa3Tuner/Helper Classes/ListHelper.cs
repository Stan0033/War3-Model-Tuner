using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wa3Tuner.Helper_Classes
{
    public static class ListHelper
    {
        public static void MoveElement<T>(List<T> list, int index, bool up)
        {
            if (list == null || list.Count == 0 || index < 0 || index >= list.Count)
                return;

            int newIndex = up ? index - 1 : index + 1;

            if (newIndex < 0 || newIndex >= list.Count)
                return;

            // Swap the elements
            T temp = list[index];
            list[index] = list[newIndex];
            list[newIndex] = temp;
        }

    }
}
