using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wa3Tuner.Helper_Classes
{
   public static class IDCounter
    {
        private static int counter = 0;
        public static int Next() { counter++; return counter; }
        public static string Next_()
        {
            counter++; return counter.ToString();
        }
    }
}
