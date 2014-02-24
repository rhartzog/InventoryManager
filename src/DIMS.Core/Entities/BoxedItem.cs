using System;

namespace DIMS.Core.Entities
{
    public class BoxedItem
    {
        private Item _item;
        private int _quantity;

        protected BoxedItem()
        {
            
        }

        public BoxedItem(int quantity, Item item)
        {
            _quantity = quantity;
            _item = item;
        }

        public virtual Guid Id { get; set; }

        public virtual Item Item
        {
            get { return _item; }
        }

        public virtual int Quantity
        {
            get { return _quantity; }
        }

        public virtual void AddMore(int quantity)
        {
            if (quantity <= 0)
                throw new InvalidOperationException("Quantity must be a positive number when adding more.");

            _quantity += quantity;
        }

        public virtual void RemoveSome(int quantity)
        {
            if (quantity <= 0)
                throw new InvalidOperationException("Quanity must be than zero when removing some.");

            _quantity -= quantity;
        }
    }
}