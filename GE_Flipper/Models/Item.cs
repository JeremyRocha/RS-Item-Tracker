using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GE_Flipper.Models
{
    public class Item
    {
        public int ItemId { get; set; }  // Primary Key

        public string? Name { get; set; } //Holds item name

        public string? Image { get; set; } //Hold url for image

        [Required]
        [Range(1, 100000)] //Set range for gameid (range potentially needs changing when items are added/removed)
        public int GameId { get; set; }  // Grand Exchange ID

        // Foreign key to ItemCategory
        [ForeignKey("ItemCategory")]
        public int ItemCategoryId { get; set; } //Holds item category id

        [ValidateNever]
        public ItemCategory ItemCategory { get; set; } = null!; //Holds entries for item categories
    }
}