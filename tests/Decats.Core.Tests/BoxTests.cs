using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Decats.Core.Enums;
using NUnit.Framework;

namespace Decats.Core.Tests
{
    [TestFixture]
    public class BoxTests
    {
        [Test]
        public void Can_create_box()
        {
            var box = new Box("str-a1", Campuses.NorthWoods);

            Assert.IsNotNull(box);
            Assert.True(box.Label == "str-a1");
            Assert.True(box.Campus == Campuses.NorthWoods);
        }

        [Test]
        public void Can_change_label()
        {
            var box = new Box("str-a1", Campuses.StPius);

            Assert.True(box.Label == "str-a1");
            
            box.ChangeLabel("str-b2");

            Assert.True(box.Label == "str-b2");
        }

        [Test]
        public void Can_move_to_new_campus()
        {
            var box = new Box("str-a1", Campuses.StPius);

            Assert.IsTrue(box.Campus == Campuses.StPius);

            box.MoveToNewCampus(Campuses.StRose);

            Assert.True(box.Campus == Campuses.StRose);
        }

        [Test]
        public void Can_set_current_quantity()
        {
            var box = new Box("str-a1", Campuses.StPius);

            box.SetCurrentQuantity(12);

            Assert.IsTrue(box.CurrentQuantity == 12);
        }

        [Test]
        public void Can_set_categories()
        {
            var box = new Box("str-a1", Campuses.NorthWoods, new string[] {"Office", "General"});

            Assert.IsTrue(box.Categories.Contains("Office"));
            Assert.IsTrue(box.Categories.Contains("General"));
            Assert.IsTrue(box.Categories.Count == 2);
        }

        [Test]
        public void Can_add_to_category()
        {
            var box = new Box("str-a1", Campuses.NorthWoods, new string[] {"Office"});

            Assert.IsTrue(box.Categories.Count == 1);
            Assert.IsTrue(box.Categories.Contains("Office"));

            box.AddToCategory("General");

            Assert.IsTrue(box.Categories.Count == 2);
            Assert.IsTrue(box.Categories.Contains("General"));
        }

        [Test]
        public void Can_remove_from_category()
        {
            var box = new Box("str-a1", Campuses.NorthWoods, new string[] { "Office", "General" });

            Assert.IsTrue(box.Categories.Contains("Office"));
            Assert.IsTrue(box.Categories.Contains("General"));
            Assert.IsTrue(box.Categories.Count == 2);

            box.RemoveFromCategory("Office");

            Assert.IsTrue(box.Categories.Count == 1);
            Assert.IsFalse(box.Categories.Contains("Office"));
        }

        [Test]
        public void Can_add_items_to_box()
        {
            var box = new Box("str-a1", Campuses.StRose);

            var item = new Item("name");

            box.AddItem(item);

            Assert.True(box.Items.Contains(item));
        }

        [Test]
        public void Can_remove_items_from_box()
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
