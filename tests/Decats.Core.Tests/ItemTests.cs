using Decats.Core.Enums;
using NUnit.Framework;

namespace Decats.Core.Tests
{
    [TestFixture]
    public class ItemTests
    {
        [Test]
        public void Can_create_item()
        {
            var item = new Item("Printer Paper 8.5x11");
            Assert.IsNotNull(item);
        }

        [Test]
        public void Item_has_name()
        {
            var item = new Item("Printer Paper 8.5x11");
            Assert.AreEqual("Printer Paper 8.5x11", item.Name);
        }

        [Test]
        public void Item_has_description()
        {
            var item = new Item("Name", "This is used to print things");
            Assert.AreEqual("This is used to print things", item.Description);
        }

        [Test]
        public void Can_change_item_name()
        {
            var item = new Item("Name");
            item.ChangeItemName("New Name");

            Assert.AreEqual("New Name", item.Name);
        }

        [Test]
        public void Can_change_item_description()
        {
            var item = new Item("Name", "Description");
            item.ModifyItemDescription("New Description");

            Assert.AreEqual("New Description", item.Description);
        }

        [Test]
        public void Can_create_item_with_categories()
        {
            var item = new Item("Name", categories: new string[] {"Office", "General"});
            
            Assert.True(item.Categories.Contains("Office"));
            Assert.True(item.Categories.Contains("General"));
        }

        [Test]
        public void Can_add_category()
        {
            var item = new Item("Name");

            item.AddToCategory("Office");

            Assert.True(item.Categories.Contains("Office"));
        }

        [Test]
        public void Can_remove_category()
        {
            var item = new Item("Name", categories: new string[] {"General", "Office"});

            Assert.True(item.Categories.Contains("Office"));
            Assert.True(item.Categories.Contains("General"));

            item.RemoveFromCategory("Office");

            Assert.True(item.Categories.Count == 1);
            Assert.True(item.Categories.Contains("General"));
        }

        [Test]
        public void Can_add_quantity()
        {
            var item = new Item("Name");

            item.SetQuantitiesForCampus(Campuses.NorthWoods,3, 10);

            Assert.True(item.Quantities.ContainsKey(Campuses.NorthWoods));
            Assert.True(item.Quantities[Campuses.NorthWoods].LowerBound == 3);
            Assert.True(item.Quantities[Campuses.NorthWoods].UpperBound == 10);
        }

        [Test]
        public void Can_change_quantity()
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
