using DIMS.Core;
using DIMS.Core.Entities;
using FluentNHibernate.Mapping;

namespace DIMS.Data
{
    public class BoxedItemMap : ClassMap<BoxedItem>
    {
        public BoxedItemMap()
        {
            Table("BoxedItems");
            Id(x => x.Id);
            Map(x => x.Quantity).Access.ReadOnlyPropertyThroughCamelCaseField(Prefix.Underscore);
            References(x => x.Item).Access.ReadOnlyPropertyThroughCamelCaseField(Prefix.Underscore)
                .Not.LazyLoad()
                .Column("Item");
        }
    }
}