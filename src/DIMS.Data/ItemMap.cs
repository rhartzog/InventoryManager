using DIMS.Core.Entities;
using FluentNHibernate.Mapping;

namespace DIMS.Data
{
    public class ItemMap : ClassMap<Item>
    {
        public ItemMap()
        {
            Table("Items");
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.Description).Length(4001);
        }

    }
}