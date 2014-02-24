using DIMS.Core;
using DIMS.Core.Entities;
using FluentNHibernate.Mapping;

namespace DIMS.Data
{
    public class CampusMap : ClassMap<Campus>
    {
        public CampusMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
        }
    }
}