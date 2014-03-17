using DIMS.Core.Entities;
using FluentNHibernate.Mapping;

namespace DIMS.Data
{
    public class BoxMap : ClassMap<Box>
    {
        public BoxMap()
        {
            Table("Boxes");
            Id(x => x.Id);
            Map(x => x.Label);
            Map(x => x.Description);
            Map(x => x.BoxCategory);
            Map(x => x.Campus)
                .Access.ReadOnlyPropertyThroughCamelCaseField(Prefix.Underscore)
                .Column("Campus");
            
            HasMany(x => x.Contents)
                .Access.ReadOnlyPropertyThroughCamelCaseField(Prefix.Underscore)
                .Table("BoxedItems")
                .Not.Inverse()
                .KeyColumn("Box")
                .Not.LazyLoad();
        }
    }
}
