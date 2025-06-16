using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GE_Flipper.Models
{
    public class Item
    {
        public int ItemId { get; set; }  // Primary Key

        public string? Name { get; set; }

        public string? Image { get; set; }

        [Required]
        [Range(1, 100000)]
        public int GameId { get; set; }  // Grand Exchange ID

        // Foreign key to ItemCategory
        [ForeignKey("ItemCategory")]
        public int ItemCategoryId { get; set; }

        [ValidateNever]
        public ItemCategory ItemCategory { get; set; } = null!;
    }
}