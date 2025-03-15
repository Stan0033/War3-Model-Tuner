using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Wa3Tuner.Helper_Classes
{
    public class Range
    {
        public int Track;
        public Vector3 Value;
        public Range(int track, float X, float Y, float Z)
        {
            Track = track;
            Value = new Vector3(X, Y, Z);
        }
       
    }
    public static class RangeMaker
    {
        public static List<Range> CreateInterpolatedRange(int from, int to, Vector3 startValue, Vector3 endValue)
        {
            List<Range> ranges = new List<Range>();
            int count = to - from + 1;
            if (count <= 1) return ranges;

            Vector3 delta = (endValue - startValue) / (count - 1);

            for (int i = 0; i < count; i++)
            {
                Vector3 value = startValue + delta * i;
                ranges.Add(new Range(from + i, value.X, value.Y, value.Z));
            }

            return ranges;
        }

        // Function for step-based range (no predefined end value, stops when a condition is met)
        public static List<Range> CreateStepRange(int from, int to, Vector3 startValue, Vector3 step)
        {
            List<Range> ranges = new List<Range>();

            Vector3 currentValue = startValue;
            for (int i = from; i <= to; i++)
            {
                ranges.Add(new Range(i, currentValue.X, currentValue.Y, currentValue.Z));
                currentValue += step;
            }

            return ranges;
        }
    }
}
