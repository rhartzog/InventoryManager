using System;

namespace DIMS.Core
{
    public class BoxedItem
    {
        public BoxedItem(int quantity, Item item)
        {
            Quantity = quantity;
            Item = item;
        }

        public Item Item { get; private set; }
        public int Quantity { get; private set; }

        public void AddMore(int quantity)
        {
            if (quantity <= 0)
                throw new InvalidOperationException("Quantity must be a positive number when adding more.");

            Quantity += quantity;
        }

        public void RemoveSome(int quantity)
        {
            if (quantity <= 0)
                throw new InvalidOperationException("Quanity must be than zero when removing some.");

            Quantity += quantity;
        }
    }
}