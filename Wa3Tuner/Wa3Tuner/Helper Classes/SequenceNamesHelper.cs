using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wa3Tuner.Dialogs;

namespace Wa3Tuner.Helper_Classes
{
    internal static class SequenceNamesHelper
    {
        public static void Show()
        {

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Common sequence names:\n");
            sb.AppendLine("Stand");
            sb.AppendLine("Birth");
            sb.AppendLine("Death");
            sb.AppendLine("Dissipate");
            sb.AppendLine("Decay");
            sb.AppendLine("Walk");
            sb.AppendLine("Attack");
            sb.AppendLine("Morph");
            sb.AppendLine("Spell");
            sb.AppendLine("Spell {particular spell name}");
            sb.AppendLine("Portrait");
            sb.AppendLine("Portrait Talk");
           
            sb.AppendLine("\nSuffixes:\n");
            sb.AppendLine("Channel");
          
            sb.AppendLine("Defend");
            sb.AppendLine("Hit");
            sb.AppendLine("One");
            sb.AppendLine("Two");
            sb.AppendLine("Three");
            sb.AppendLine("Four");
            sb.AppendLine("Five");
            sb.AppendLine("Six");
            sb.AppendLine("First");
            sb.AppendLine("Second");
            sb.AppendLine("Third");
            sb.AppendLine("Fourth");
            sb.AppendLine("Fifth");
            sb.AppendLine("Sixth");
            sb.AppendLine("Upgrade");
            sb.AppendLine("Swim");
            sb.AppendLine("Spell");
            sb.AppendLine("Gold");
            sb.AppendLine("Lumber");
            sb.AppendLine("Fast");
            sb.AppendLine("Slow");
            sb.AppendLine("Slam");
            sb.AppendLine("Alternate");
            sb.AppendLine("Flesh");
            sb.AppendLine("Bone");
            sb.AppendLine("Cinematic");
            sb.AppendLine("Work");
            sb.AppendLine("Small");
            sb.AppendLine("Medium");
            sb.AppendLine("Large");
            sb.AppendLine("Ready");
            sb.AppendLine("Spin");
            sb.AppendLine("Throw");

            TextViewer tv = new TextViewer(sb.ToString());
            tv.ShowDialog();
        }
    }
}
