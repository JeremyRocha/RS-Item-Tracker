using Microsoft.AspNetCore.Mvc;
using GE_Flipper.Models;
using System.Linq;
using GE_Flipper.Data;
using Microsoft.AspNetCore.Authorization;

namespace GE_Flipper.Controllers
{
    public class PriceController : Controller
    {
        private readonly ApplicationDbContext _context; //Variable for databe base

        public PriceController(ApplicationDbContext context) //Consturctor for intialize db variable
        {
            _context = context; //Intializes variable to get data from db
        }
        [Authorize]
        public IActionResult Index(int id)
        {
            //Variable for holding item name. Selects the name of item where item id equals the id passed
            var itemName = _context.Items
                .Where(i => i.ItemId == id)
                .Select(i => i.Name)
                .FirstOrDefault();

            if (itemName == null) //If item name is null
            {
                return NotFound(); //Returns not found
            }

            //Variable for prices. Get all price for an item where item id equals the id that was passed, ordered by date.
            var prices = _context.Prices
                .Where(p => p.ItemId == id)
                .OrderBy(p => p.Date)
                .ToList();

            ViewData["ItemName"] = itemName; //Store item in view data
           
            return View(prices); //returns the view
        }
    }
}
