using System;
using System.Collections.Generic;
using System.Linq;
using DIMS.Core.Enumerations;

namespace DIMS.Core.Entities
{
    public class Box
    {
        private readonly IList<BoxedItem> _contents = new List<BoxedItem>();
        private Campus _campus;

        public virtual Guid Id { get; set; }
        public virtual string Label { get; set; }
        public virtual string Description { get; set; }
        public virtual BoxCategory BoxCategory { get; set; }

        public virtual Campus Campus
        {
            get { return _campus; }
        }

        public virtual IList<BoxedItem> Contents
        {
            get { return _contents; }
        }

        protected Box()
        {
            
        }

        public Box(string label = null, Campus campus = null)
        {
            Label = label;
            _campus = campus;
        }

        /// <summary>
        /// Adds an item to the boxes content if there are none in the box
        /// or it adds more of the item to the box if there are some already in the box
        /// </summary>
        /// <param name="quantity">Number of items to add</param>
        /// <param name="item">Item to add</param>
        public virtual void AddItem(int quantity, Item item)
        {
            if (quantity < 0)
                throw new InvalidOperationException("Quantity must be a positive number");

            if (Contents.Any(x => x.Item == item))
            {
                Contents.Single(x => x.Item == item).AddMore(quantity);
            }
            else
            {
                Contents.Add(new BoxedItem(quantity, item));
            }
        }

        /// <summary>
        /// Completely removes all of the given item from the box
        /// </summary>
        /// <param name="item">Item to remove</param>
        public virtual void RemoveItem(Item item)
        {
            if (Contents.Any(x => x.Item != item))
                throw new InvalidOperationException(string.Format("This box doesn't contain any {0}", item.Name));

            Contents.Remove(Contents.First(x => x.Item == item));
        }

        /// <summary>
        /// Reduces the quantity of the given item by the quantity specified.
        /// </summary>
        /// <param name="quantity">Number of items to remove</param>
        /// <param name="item">Item to reduce</param>
        public virtual void ReduceItem(int quantity, Item item)
        {
            if (quantity <= 0)
                throw new InvalidOperationException("Quantity must be greater than zero.");

            if (Contents.First(x => x.Item == item).Quantity <= Math.Abs(quantity))
            {
                RemoveItem(item);
            }
            else
            {
                Contents.First(x => x.Item == item).RemoveSome(quantity);
            }
        }

        /// <summary>
        /// Move the box to a new campus
        /// </summary>
        /// <param name="newLocation">Campus box is moving to</param>
        public virtual void RelocateBox(Campus newLocation)
        {
            if (Campus == null || Campus != newLocation)
            {
                _campus = newLocation;
            }
        }
    }
}
