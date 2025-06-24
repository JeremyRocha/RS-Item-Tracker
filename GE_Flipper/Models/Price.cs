using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GE_Flipper.Models
{
    public class Price
    {
        [Required]
        public int PriceID { get; set; }
        [Required]
        public int CurrentPrice {  get; set; }
        [Required]
        public DateTime Date { get; set; }

        [ForeignKey("Item")]
        public int ItemId { get; set; }

        [ValidateNever]
        public Item Item{ get; set; } = null!;
    }
}
