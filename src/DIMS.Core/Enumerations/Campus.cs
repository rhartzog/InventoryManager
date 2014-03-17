using System;

namespace DIMS.Core.Enumerations
{
    public class Campus : Enumeration<Campus>
    {
        public static readonly Campus Northwoods = new Campus(1, "Northwoods");
        public static readonly Campus StPius = new Campus(2, "St. Pius");
        public static readonly Campus StRose = new Campus(3, "St. Rose of Lima");

        private Campus(int value, string displayName)
            : base(value, displayName)
        {
            
        }
    }
}