using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GE_Flipper.Models;

public class ItemCategory
{
    public int ItemCategoryId { get; set; }  // Primary Key

    [Required]
    public string Name { get; set; }

    public string Description { get; set; }

    public string IconUrl { get; set; }

    public ICollection<Item> Items { get; set; }
}
