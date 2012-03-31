using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Decats.Core.Tests
{
    [TestFixture]
    public class InventoryItemTests
    {
        [Test]
        public void Can_create_inventory_item()
        {
            var item = new InventoryItem("Printer Paper 8.5x11");
            Assert.IsNotNull(item);
        }

        [Test]
        public void Inventory_item_has_name()
        {
            var item = new InventoryItem("Printer Paper 8.5x11");
            Assert.AreEqual("Printer Paper 8.5x11", item.Name);
        }

        [Test]
        public void Inventory_item_has_description()
        {
            var item = new InventoryItem("Name", "This is used to print things");
            Assert.AreEqual("This is used to print things", item.Description);
        }

        [Test]
        public void Can_change_item_name()
        {
            var item = new InventoryItem("Name");
            item.ChangeItemName("New Name");

            Assert.AreEqual("New Name", item.Name);
        }

        [Test]
        public void Can_not_change_item_name_to_same_name()
        {
            var item = new InventoryItem("Name");

            Assert.Throws<ArgumentException>(() => item.ChangeItemName("Name"));
        }

        [Test]
        public void Can_change_item_description()
        {
            var item = new InventoryItem("Name", "Description");
            item.ModifyItemDescription("New Description");

            Assert.AreEqual("New Description", item.Description);
        }

        [Test]
        public void Calling_modify_description_without_changing_description_throws_exception()
        {
            var item = new InventoryItem("Name", "Description");

            Assert.Throws<ArgumentException>(() => item.ModifyItemDescription("Description"));
        }
    }
}
