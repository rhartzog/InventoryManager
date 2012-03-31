using System;
using System.Collections.Generic;

namespace Decats.Core
{
    public class Box
    {
        private readonly IDictionary<InventoryItem, int> _contents = new Dictionary<InventoryItem, int>();

        public Box(string label)
        {
            Label = label;
        }

        public string Label { get; private set; }

        public string Campus { get; private set; }

        public string Category { get; private set; }

        public string Id { get; set; }

        public int ClassroomCheckedOutTo { get; private set; }

        public void RelocateToNewCampus(string campus, string newLabel = null)
        {
            if (Campus == campus)
                throw new ArgumentException("This box is already at the specified campus");

            Campus = campus;

            if (!string.IsNullOrWhiteSpace(newLabel))
                Label = newLabel;
        }

        public void RelabelBox(string newLabel)
        {
            if (Label == newLabel)
                throw new ArgumentException(string.Format("The box is already labeled {0}", newLabel));

            Label = newLabel;
        }

        public void AddToCategory(string category)
        {
            if (category == null)
                throw new ArgumentNullException("category");

            Category = category;
        }

        public void AddItemToBox(InventoryItem itemToAdd, int quantity = 1)
        {
            if (itemToAdd == null)
                throw new ArgumentNullException("itemToAdd");

            if (_contents.ContainsKey(itemToAdd))
                throw new ArgumentException("This item already exists in this box");

            _contents.Add(itemToAdd, quantity);
        }

        public IDictionary<InventoryItem, int> ListContents()
        {
            return _contents;
        }

        public void Checkout(int classNumber)
        {
            if (ClassroomCheckedOutTo == classNumber)
                throw new ArgumentException(string.Format("This box is already checked out to classroom {0}",
                                                          classNumber));

            ClassroomCheckedOutTo = classNumber;
        }
    }
}