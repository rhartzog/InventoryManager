using System;
using Xunit;

namespace DIMS.Core.Tests
{
    public class BoxedItemTests
    {
        [Fact]
        public void Can_add_more_to_boxed_item_quantity()
        {
            var item = new Item();
            var boxedItem = new BoxedItem(1, item);

            Assert.Equal(1, boxedItem.Quantity);

            boxedItem.AddMore(1);

            Assert.Equal(2, boxedItem.Quantity);
        }

        [Fact]
        public void Can_remove_some_from_boxed_item_quantity()
        {
            var item = new Item();
            var boxedItem = new BoxedItem(2, item);

            Assert.Equal(2, boxedItem.Quantity);

            boxedItem.RemoveSome(-1);

            Assert.Equal(1, boxedItem.Quantity);
        }
        
        [Fact]
        public void Trying_to_add_negative_number_throws_error()
        {
            var boxedItem = new BoxedItem(1, new Item());
            Assert.Throws<InvalidOperationException>(() => boxedItem.AddMore(-1));
        }

        [Fact]
        public void Trying_to_remove_by_negative_number_throws_error()
        {
            var boxedItem = new BoxedItem(1, new Item());
            Assert.Throws<InvalidOperationException>(() => boxedItem.RemoveSome(-1));
        }
    }
}