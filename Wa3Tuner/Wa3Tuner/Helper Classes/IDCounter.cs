using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wa3Tuner.Helper_Classes
{
   public static class IDCounter
    {
        public static int Next
        {
            get
            {
                counter++; return counter;
            }
            
        }
        public static string Next_
        {
            get
            {
                counter++; return counter.ToString(); ;
            }

        }
        private static int counter = 0;
       public static int Value  => counter; 
        
    }
}
