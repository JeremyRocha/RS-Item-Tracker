using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GE_Flipper.Models;

public class ItemCategory
{
    public int ItemCategoryId { get; set; }  // Primary Key

    [Required]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string IconUrl { get; set; } = string.Empty;

    public ICollection<Item> Items { get; set; } = [];
}
