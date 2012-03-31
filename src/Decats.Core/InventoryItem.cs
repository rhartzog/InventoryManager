using System;

namespace Decats.Core
{
    public class InventoryItem
    {
        public InventoryItem(string name)
        {
            Name = name;
        }

        public InventoryItem(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public void ChangeItemName(string newName)
        {
            if (Name == newName)
                throw new ArgumentException(string.Format("This item is already named {0}", newName));

            Name = newName;
        }

        public void ModifyItemDescription(string newDescription)
        {
            if (Description == newDescription)
                throw new ArgumentException("The item description did not change");

            Description = newDescription;
        }
    }
}