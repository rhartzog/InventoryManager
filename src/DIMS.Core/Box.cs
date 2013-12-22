using System;
using System.Collections.Generic;
using System.Linq;

namespace DIMS.Core
{
    public class Box
    {
        public Guid Id { get; set; }
        public string Label { get; set; }
        public Campus Campus { get; private set; }
        private IList<BoxedItem> Contents { get; set; }

        public Box(string label = null, Campus campus = null)
        {
            Label = label;
            Campus = campus;
            Contents = new List<BoxedItem>();
        }

        /// <summary>
        /// Returns the contents of the box.
        /// </summary>
        /// <returns></returns>
        public IList<BoxedItem> GetContents()
        {
            return Contents;
        }

        /// <summary>
        /// Adds an item to the boxes content if there are none in the box
        /// or it adds more of the item to the box if there are some already in the box
        /// </summary>
        /// <param name="quantity">Number of items to add</param>
        /// <param name="item">Item to add</param>
        public void AddItem(int quantity, Item item)
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
        public void RemoveItem(Item item)
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
        public void ReduceItem(int quantity, Item item)
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
        public void RelocateBox(Campus newLocation)
        {
            if (Campus == null || Campus != newLocation)
            {
                Campus = newLocation;
            }
        }
    }
}
