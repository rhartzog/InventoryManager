using Decats.Core.Enums;
using NUnit.Framework;

namespace Decats.Core.Tests
{
    [TestFixture]
    public class ItemTests
    {
        [Test]
        public void CanCreateItem()
        {
            var item = new Item("Printer Paper 8.5x11");
            Assert.IsNotNull(item);
        }

        [Test]
        public void ItemHasName()
        {
            var item = new Item("Printer Paper 8.5x11");
            Assert.AreEqual("Printer Paper 8.5x11", item.Name);
        }

        [Test]
        public void ItemHasDescription()
        {
            var item = new Item("Name", "This is used to print things");
            Assert.AreEqual("This is used to print things", item.Description);
        }

        [Test]
        public void CanChangeItemName()
        {
            var item = new Item("Name");
            item.ChangeItemName("New Name");

            Assert.AreEqual("New Name", item.Name);
        }

        [Test]
        public void CanChangeItemDescription()
        {
            var item = new Item("Name", "Description");
            item.ModifyItemDescription("New Description");

            Assert.AreEqual("New Description", item.Description);
        }

        [Test]
        public void CanCreateItemWithCategories()
        {
            var item = new Item("Name", categories: new[] {"Office", "General"});
            
            Assert.True(item.Categories.Contains("Office"));
            Assert.True(item.Categories.Contains("General"));
        }

        [Test]
        public void CanAddCategory()
        {
            var item = new Item("Name");

            item.AddToCategory("Office");

            Assert.True(item.Categories.Contains("Office"));
        }

        [Test]
        public void CanRemoveCategory()
        {
            var item = new Item("Name", categories: new[] {"General", "Office"});

            Assert.True(item.Categories.Contains("Office"));
            Assert.True(item.Categories.Contains("General"));

            item.RemoveFromCategory("Office");

            Assert.True(item.Categories.Count == 1);
            Assert.True(item.Categories.Contains("General"));
        }

        [Test]
        public void CanAddQuantity()
        {
            var item = new Item("Name");

            item.SetQuantitiesForCampus(Campuses.NorthWoods,3, 10);

            Assert.True(item.Quantities.ContainsKey(Campuses.NorthWoods));
            Assert.True(item.Quantities[Campuses.NorthWoods].LowerBound == 3);
            Assert.True(item.Quantities[Campuses.NorthWoods].UpperBound == 10);
        }

        [Test]
        public void CanChangeQuantity()
        {
            var item = new Item("name");

            item.SetQuantitiesForCampus(Campuses.StPius, 1, 2);

            Assert.True(item.Quantities.ContainsKey(Campuses.StPius));
            Assert.True(item.Quantities[Campuses.StPius].LowerBound == 1);
            Assert.True(item.Quantities[Campuses.StPius].UpperBound == 2);

            item.SetQuantitiesForCampus(Campuses.StPius, 5, 10);

            Assert.True(item.Quantities.ContainsKey(Campuses.StPius));
            Assert.True(item.Quantities[Campuses.StPius].LowerBound == 5);
            Assert.True(item.Quantities[Campuses.StPius].UpperBound == 10);
        }
    }
}
