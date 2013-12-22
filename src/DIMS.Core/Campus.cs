using Headspring;

namespace DIMS.Core
{
    public class Campus : Enumeration<Campus>
    {
        public static readonly Campus StRose = new Campus(1, "St. Rose of Lima");
        public static readonly Campus StPius = new Campus(2, "St. Pius");

        public Campus(int value, string displayName) 
            : base(value, displayName)
        {

        }
    }
}