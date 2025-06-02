using MdxLib.Model;
 
using System.Collections.Generic;
 
namespace Wa3Tuner.Helper_Classes
{
    public enum PlayBackType
    {
        Default, Loop, Cycle, Paused, DontLoop
    }
    public static class Playback
    {
        public static   List<CSequence> ModelSequences;
        public static CModel Sequences
        {
            set { ModelSequences = value.Sequences.ObjectList; }
        }
        private static CSequence  sequence_;
        public static CSequence CurrentSequence
        {
            get {  return sequence_; }
            set { sequence_ = value; currentTrack = value.IntervalStart; }
        }
        public static PlayBackType Type = PlayBackType.Paused;
        private static int currentTrack = 0;
      public static int Track => currentTrack;
        
        public static int Next
        {
            get { 
               
                switch (Type)
                {
                    case PlayBackType.Paused:
                        return currentTrack;
                    case PlayBackType.Default:
                        if (currentTrack == sequence_.IntervalEnd)
                        {
                            if (sequence_.NonLooping)
                            {
                                return currentTrack;
                            }
                            else
                            {
                                currentTrack++; return currentTrack;
                            }
                        }
                        else
                        {
                            if (currentTrack < CurrentSequence.IntervalStart || currentTrack > CurrentSequence.IntervalEnd)
                            {
                                currentTrack = CurrentSequence.IntervalStart;return currentTrack;
                            }
                            currentTrack++; return currentTrack;
                        }
                            
                    case PlayBackType.Loop:
                        if (currentTrack == sequence_.IntervalEnd)
                        {

                            currentTrack = sequence_.IntervalStart; ; return currentTrack;
                            
                        }
                        else
                        {
                            if (currentTrack < CurrentSequence.IntervalStart || currentTrack > CurrentSequence.IntervalEnd)
                            {
                                currentTrack = CurrentSequence.IntervalStart; return currentTrack;
                            }
                            currentTrack++; return currentTrack;
                        }
                        
                    case PlayBackType.Cycle:
                        if (currentTrack == sequence_.IntervalEnd)
                        {
                            currentTrack++; return currentTrack;
                        }
                        else
                        {
                            int sIndex = ModelSequences.IndexOf(sequence_);
                            if (sIndex == ModelSequences.Count - 1) sIndex = 0;
                            CurrentSequence = ModelSequences[sIndex];
                            currentTrack = CurrentSequence.IntervalStart;
                            return currentTrack;
                        }
                    default: return currentTrack;
                }
                
            
            }

        }
    }
}
