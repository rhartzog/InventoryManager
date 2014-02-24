using DIMS.Core.Entities;
using FluentNHibernate.Mapping;

namespace DIMS.Data
{
    public class RoleMap : ClassMap<Role>
    {
        public RoleMap()
        {
            Id(x => x.Id).Access.ReadOnlyPropertyThroughCamelCaseField(Prefix.Underscore);
            Map(x => x.Name);
            Map(x => x.ApplicationName);
            HasManyToMany(x => x.UsersInRole)
                .Cascade.All()
                .Inverse()
                .Table("UsersInRoles");
        }
    }
}