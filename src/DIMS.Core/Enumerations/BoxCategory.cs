using DIMS.Core.Entities;

namespace DIMS.Core.Enumerations
{
    public class BoxCategory : Enumeration<BoxCategory>
    {
        private BoxCategory(int value, string displayName) : base(value, displayName)
        {
            
        }

        public static readonly BoxCategory Admin = new BoxCategory(1, "Admin");
        public static readonly BoxCategory BetterAt = new BoxCategory(2, "Better at");
        public static readonly BoxCategory Course = new BoxCategory(3, "Course Materials");
        public static readonly BoxCategory Decor = new BoxCategory(4, "Decor");
        public static readonly BoxCategory Electronics = new BoxCategory(5, "Electronics");
    }
}