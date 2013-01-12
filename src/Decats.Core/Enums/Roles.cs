using System.ComponentModel;

namespace Decats.Core.Enums
{
    public enum Roles
    {
        Admin,

        [Description("Office Manager")]
        OfficeManager,

        Mentor
    }
}
