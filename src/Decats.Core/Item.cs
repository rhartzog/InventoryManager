using System.Collections.Generic;
using Decats.Core.Enums;
using Decats.Core.Helpers;

namespace Decats.Core
{
    public class Item
    {
        public Item(string name, string description = null, ICollection<string> categories = null)
        {
            Name = name;
            Categories = new List<string>();
            Quantities = new Dictionary<Campuses, Range<int>>();

            if (!string.IsNullOrWhiteSpace(description))
                Description = description;

            if (categories != null && categories.Count > 0)
            {
                foreach (var category in categories)
                {
                    AddToCategory(category);
                }
            }
        }

        public string Id { get; private set; }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public IList<string> Categories { get; private set; }

        public IDictionary<Campuses, Range<int>> Quantities { get; private set; }

        public void ChangeItemName(string newName)
        {
            if (Name == newName)
                return;

            Name = newName;
        }

        public void ModifyItemDescription(string newDescription)
        {
            if (Description == newDescription)
                return;

            Description = newDescription;
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

        public void SetQuantitiesForCampus(Campuses campus, int minQuantity, int desiredQuantity )
        {
            if (Quantities.Contains(new KeyValuePair<Campuses, Range<int>>(campus, new Range<int>(minQuantity, desiredQuantity))))
                return;

            if (Quantities.ContainsKey(campus))
            {
                Quantities[campus] = new Range<int>(minQuantity, desiredQuantity);
                return;
            }

            Quantities.Add(campus, new Range<int>(minQuantity, desiredQuantity));
        }
    }
}