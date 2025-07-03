using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GE_Flipper.Models
{
    public class Price
    {
        public int PriceID { get; set; } //Primary Key
        [Required]
        public int CurrentPrice {  get; set; } //Holds current price of item
        [Required]
        public DateTime Date { get; set; } //Holds date

        [ForeignKey("Item")]
        public int ItemId { get; set; } //Holds item ids

        [ValidateNever]
        public Item Item{ get; set; } = null!; //Holds entries for items
    }
}
