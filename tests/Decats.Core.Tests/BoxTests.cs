using Decats.Core.Enums;
using NUnit.Framework;

namespace Decats.Core.Tests
{
    [TestFixture]
    public class BoxTests
    {
        [Test]
        public void CanCreateBox()
        {
            var box = new Box("str-a1", Campuses.NorthWoods);

            Assert.IsNotNull(box);
            Assert.True(box.Label == "str-a1");
            Assert.True(box.Campus == Campuses.NorthWoods);
        }

        [Test]
        public void CanChangeLabel()
        {
            var box = new Box("str-a1", Campuses.StPius);

            Assert.True(box.Label == "str-a1");
            
            box.ChangeLabel("str-b2");

            Assert.True(box.Label == "str-b2");
        }

        [Test]
        public void CanMoveToNewCampus()
        {
            var box = new Box("str-a1", Campuses.StPius);

            Assert.IsTrue(box.Campus == Campuses.StPius);

            box.MoveToNewCampus(Campuses.StRose);

            Assert.True(box.Campus == Campuses.StRose);
        }

        [Test]
        public void CanSetCurrentQuantity()
        {
            var box = new Box("str-a1", Campuses.StPius);

            box.SetCurrentQuantity(12);

            Assert.IsTrue(box.CurrentQuantity == 12);
        }

        [Test]
        public void CanSetCategories()
        {
            var box = new Box("str-a1", Campuses.NorthWoods, new[] {"Office", "General"});

            Assert.IsTrue(box.Categories.Contains("Office"));
            Assert.IsTrue(box.Categories.Contains("General"));
            Assert.IsTrue(box.Categories.Count == 2);
        }

        [Test]
        public void CanAddToCategory()
        {
            var box = new Box("str-a1", Campuses.NorthWoods, new[] {"Office"});

            Assert.IsTrue(box.Categories.Count == 1);
            Assert.IsTrue(box.Categories.Contains("Office"));

            box.AddToCategory("General");

            Assert.IsTrue(box.Categories.Count == 2);
            Assert.IsTrue(box.Categories.Contains("General"));
        }

        [Test]
        public void CanRemoveFromCategory()
        {
            var box = new Box("str-a1", Campuses.NorthWoods, new[] { "Office", "General" });

            Assert.IsTrue(box.Categories.Contains("Office"));
            Assert.IsTrue(box.Categories.Contains("General"));
            Assert.IsTrue(box.Categories.Count == 2);

            box.RemoveFromCategory("Office");

            Assert.IsTrue(box.Categories.Count == 1);
            Assert.IsFalse(box.Categories.Contains("Office"));
        }

        [Test]
        public void CanAddItemsToBox()
        {
            var box = new Box("str-a1", Campuses.StRose);

            var item = new Item("name");

            box.AddItem(item);

            Assert.True(box.Items.Contains(item));
        }

        [Test]
        public void CanRemoveItemsFromBox()
        {
            var box = new Box("str-a1", Campuses.StPius);

            var item = new Item("name");

            box.AddItem(item);

            Assert.True(box.Items.Contains(item));

            box.RemoveItem(item);

            Assert.False(box.Items.Contains(item));
        }
    }
}
