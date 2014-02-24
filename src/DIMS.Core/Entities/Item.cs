using System;

namespace DIMS.Core.Entities
{
    public class Item
    {
        public virtual Guid Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
    }
}