using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GE_Flipper.Models
{
    public class Item
    {
        public int ItemId { get; set; }  // Primary Key

        [Required]
        public string Name { get; set; }

        public int GEId { get; set; }  // Grand Exchange ID

        public decimal LatestPrice { get; set; }

        // Foreign key to ItemCategory
        [ForeignKey("ItemCategory")]
        public int ItemCategoryId { get; set; }

        public ItemCategory ItemCategory { get; set; }
    }
}