using System;
using System.Linq;
using Xunit;

namespace DIMS.Core.Tests
{
    public class BoxTests
    {
        [Fact]
        public void Adding_new_item_to_box_should_have_correct_quantity()
        {
            var box = new Box();
            box.AddItem(3, new Item { Name = "Test Item" });

            Assert.Equal(3, box.GetContents().First().Quantity);
        }

        [Fact]
        public void Adding_same_item_more_than_once_should_sum_quanitities()
        {
            var box = new Box();
            var item = new Item { Name = "Test Item" };
            box.AddItem(3, item);

            Assert.Equal(3, box.GetContents().First().Quantity);

            box.AddItem(2, item);

            Assert.Equal(5, box.GetContents().First().Quantity);
            Assert.Equal(1, box.GetContents().Count);
        }

        [Fact]
        public void Adding_item_with_negative_quanity_throws_error()
        {
            var box = new Box();
            var item = new Item { Name = "Test Item" };

            Assert.Throws<InvalidOperationException>(() => box.AddItem(-3, item));
        }

        [Fact]
        public void Can_remove_an_item()
        {
            var box = new Box();
            var item = new Item {Name = "Test Item"};

            box.AddItem(1, item);

            Assert.Equal(1, box.GetContents().Count(x => x.Item == item));

            box.RemoveItem(item);

            Assert.Empty(box.GetContents());
        }

        [Fact]
        public void Can_reduce_an_item_without_removing_it()
        {
            var box = new Box();
            var item = new Item {Name = "Test Item"};

            box.AddItem(2, item);

            Assert.Equal(2, box.GetContents().First(x => x.Item == item).Quantity);

            box.ReduceItem(-1, item);

            Assert.Equal(1, box.GetContents().First(x => x.Item == item).Quantity);
        }

        [Fact]
        public void Reducing_an_item_by_equal_or_more_than_current_quantity_removes_item()
        {
            var box = new Box();
            var item = new Item {Name = "Test Item"};

            box.AddItem(2, item);

            Assert.Equal(2, box.GetContents().First(x => x.Item == item).Quantity);

            box.ReduceItem(-2, item);

            Assert.Equal(0, box.GetContents().Count(x => x.Item == item));
        }

        [Fact]
        public void Removing_an_item_that_is_not_in_box_throws_error()
        {
            var box = new Box();
            Assert.Throws<InvalidOperationException>(() => box.RemoveItem(new Item()));
        }

        [Fact]
        public void Passing_negative_quantity_to_reduce_item_throws_error()
        {
            var box = new Box();
            Assert.Throws<InvalidOperationException>(() => box.ReduceItem(1, new Item()));
        }

        [Fact]
        public void Can_assign_box_to_campus()
        {
            var box = new Box();
            Assert.Null(box.Campus);

            box.RelocateBox(Campus.StPius);

            Assert.Equal(Campus.StPius, box.Campus);
        }

        [Fact]
        public void Can_relocate_box_to_new_campus()
        {
            var box = new Box(campus: Campus.StPius);

            Assert.Equal(Campus.StPius, box.Campus);

            box.RelocateBox(Campus.StRose);

            Assert.Equal(Campus.StRose, box.Campus);
        }

    }
}
