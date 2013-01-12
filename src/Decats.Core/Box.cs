using System.Collections.Generic;
using Decats.Core.Enums;

namespace Decats.Core.Tests
{
    public class Box
    {
        public Box(string label, Campuses campus, ICollection<string> categories = null)
        {
            Label = label;
            Campus = campus;
            Categories = new List<string>();
            Items = new List<Item>();

            if (categories != null && categories.Count > 0)
            {
                foreach (var category in categories)
                {
                    AddToCategory(category);
                }
            }
        }

        public ICollection<string> Categories { get; private set; }

        public string Label { get; private set; }

        public Campuses Campus { get; private set; }

        public int CurrentQuantity { get; private set; }
        
        public ICollection<Item> Items { get; private set; }

        public void ChangeLabel(string newLabel)
        {
            if (Label == newLabel)
                return;

            Label = newLabel;
        }

        public void MoveToNewCampus(Campuses newCampus)
        {
            if (Campus == newCampus)
                return;

            Campus = newCampus;
        }

        public void SetCurrentQuantity(int quantity)
        {
            if (CurrentQuantity == quantity)
                return;

            CurrentQuantity = quantity;
        }

        public void AddToCategory(string category)
        {
            if (Categories.Contains(category))
                return;

            Categories.Add(category);
        }

        public void RemoveFromCategory(string category)
        {
            if (!Categories.Contains(category))
                return;

            Categories.Remove(category);
        }


        public void AddItem(Item item)
        {
            if (Items.Contains(item))
                return;

            Items.Add(item);
        }


        public void RemoveItem(Item item)
        {
            if (!Items.Contains(item))
                return;

            Items.Remove(item);
        }
    }
}