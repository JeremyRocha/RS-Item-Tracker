using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GE_Flipper.Models;

public class ItemCategory
{
    public int ItemCategoryId { get; set; }  // Primary Key

    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string Description { get; set; } = string.Empty;

    [ValidateNever]
    public ICollection<Item> Items { get; set; } = [];
}
