using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using DIMS.Core.Enumerations;

namespace DIMS.UI.Models.Edit
{
    public class BoxForm
    {
        public Guid Id { get; set; }
        
        [Required]
        public string Label { get; set; }
        
        public string Description { get; set; }
        
        [Required]
        [DisplayName("Category")]
        public string SelectedCategory { get; set; }
        public IEnumerable<SelectListItem> AvailableBoxCategories { get; set; }
        
        [Required]
        public string Campus { get; set; }

        public BoxForm()
        {
            AvailableBoxCategories = BoxCategory.GetAll().Select(c => new SelectListItem
            {
                Selected = (c.DisplayName == SelectedCategory),
                Text = c.DisplayName,
                Value = c.Value.ToString()
            });
        }
    }
}