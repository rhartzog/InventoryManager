using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Decats.Core.Tests
{
    [TestFixture]
    public class BoxTests
    {
        [Test]
        public void Can_create_box()
        {
            var box = new Box("A1");
            Assert.IsNotNull(box);
        }

        [Test]
        public void Box_has_id_property()
        {
            var box = new Box("A1");
            Assert.IsNull(box.Id);
        }

        [Test]
        public void Box_has_label()
        {
            var box = new Box("B2");
            Assert.AreEqual("B2", box.Label);
        }

        [Test]
        public void Can_relabel_box()
        {
            var box = new Box("B2");
            box.RelabelBox("A1");

            Assert.AreEqual("A1", box.Label);
        }

        [Test]
        public void Can_not_relabel_box_with_same_label()
        {
            var box = new Box("A1");

            Assert.Throws<ArgumentException>(() => box.RelabelBox("A1"));
        }

        [Test]
        public void Can_relocate_box_to_new_campus()
        {
            var box = new Box("B2");

            Assert.IsNull(box.Campus);

            box.RelocateToNewCampus("St. Pius");

            Assert.AreEqual("St. Pius", box.Campus);
        }

        [Test]
        public void Can_relabel_when_relocating()
        {
            var box = new Box("B2");

            Assert.IsNull(box.Campus);

            box.RelocateToNewCampus("St. Pius", "A1");

            Assert.AreEqual("A1", box.Label);
        }

        [Test]
        public void Can_not_errantly_move_box_to_same_campus()
        {
            var box = new Box("A1");

            box.RelocateToNewCampus("St. Pius", box.Label);

            Assert.AreEqual("St. Pius", box.Campus);

            Assert.Throws<ArgumentException>(() => box.RelocateToNewCampus("St. Pius", box.Label));
        }

        [Test]
        public void Can_assign_to_category()
        {
            var box = new Box("A1");
            
            box.AddToCategory("Admin");

            Assert.AreEqual("Admin", box.Category);
        }

        [Test]
        public void Can_add_item_to_contents()
        {
            var box = new Box("A1");
            var item = new InventoryItem("Construction Paper");

            box.AddItemToBox(item);

            Assert.IsTrue(box.ListContents().ContainsKey(item));
        }

        [Test]
        public void Can_check_out_box()
        {
            var box = new Box("A1");

            box.Checkout(12);

            Assert.AreEqual(box.ClassroomCheckedOutTo, 12);
        }
    }
}
