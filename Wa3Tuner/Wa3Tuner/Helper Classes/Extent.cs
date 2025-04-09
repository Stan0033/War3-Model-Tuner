namespace Whim_GEometry_Editor.Misc
{
    public class Extent
    {
        internal float minX, minY, minZ, maxX, maxY, maxZ = 0;
        public Extent (float mx, float my, float mz, float xx, float xy, float xz)
        {
            minX = mx;
            minY = my;
            minZ = mz;
            maxX = xx;
            maxY = xy;
            maxZ = xz;
        }
        public Extent() { }
        public override string ToString()
        {
            return $"{minX} {minY} {minZ}, {maxX} {maxY} {maxZ}";
        }
    }
}