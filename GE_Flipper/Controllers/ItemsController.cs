using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GE_Flipper.Data;
using GE_Flipper.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GE_Flipper.Controllers
{
    public class ItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpClientFactory _httpClient; //Adds Httpclient for api requests

        public ItemsController(ApplicationDbContext context, IHttpClientFactory httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        // GET: Items
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = await _context.Items.Include(i => i.ItemCategory).ToListAsync(); //Gets items from databas and include item category
            //Gets the latest price from database 
            var prices = await _context.Prices.GroupBy(price => price.ItemId).Select(entry => new
            {
                ItemID = entry.Key,
                currentPrice = entry.OrderByDescending(prices => prices.Date).FirstOrDefault().CurrentPrice
            }).ToListAsync();
            ViewBag.CurrentPrice = prices.ToDictionary(p => p.ItemID, p => p.currentPrice); //Stores list of prices in view bag as dictionary with item id key
            return View(applicationDbContext); //returns view
        }

        // GET: Items/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .Include(i => i.ItemCategory)
                .FirstOrDefaultAsync(m => m.ItemId == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // GET: Items/Create
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            ViewData["ItemCategoryId"] = new SelectList(_context.ItemCategories, "ItemCategoryId", "Name");
            return View();
        }

        // POST: Items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemId,GameId,ItemCategoryId")] Item item)
        {
            if (ModelState.IsValid)
            {
                bool itemExsists = await _context.Items.AnyAsync(i => i.GameId == item.GameId); //Bool for checking if item already exists
                if (itemExsists) //If item does exists
                {
                    ModelState.AddModelError("", "Item already Exists"); //Adds error message so user know what happened
                    ViewData["ItemCategoryId"] = new SelectList(_context.ItemCategories, "ItemCategoryId", "Name", item.ItemCategoryId);
                    return View(item);
                }
                var client = _httpClient.CreateClient(); //Creates a new instance of httpClient
                var apiLink = await client.GetAsync($"https://services.runescape.com/m=itemdb_oldschool/api/catalogue/detail.json?item={item.GameId}"); //Makes request using the link to get API values
                if (apiLink.IsSuccessStatusCode) 
                {
                    var getAPI = await apiLink.Content.ReadAsStringAsync(); //Gets data from API as string
                    using var parseAPI = JsonDocument.Parse(getAPI); //Parse the string from API
                    var osrsItem = parseAPI.RootElement.GetProperty("item"); //Gets data associated with item 
                    item.Name = osrsItem.GetProperty("name").GetString(); //Stores name from API in db
                    item.Image = $"https://secure.runescape.com/m=itemdb_oldschool/obj_big.gif?id={item.GameId}"; //Store image string in db
                }
                _context.Add(item);
                await _context.SaveChangesAsync();
                try
                {
                    var apiPriceLink = await client.GetAsync($"https://secure.runescape.com/m=itemdb_oldschool/api/graph/{item.GameId}.json"); //Makes request using the link to get API values
                    if (apiPriceLink.IsSuccessStatusCode)
                    {
                        var getPriceAPI = await apiPriceLink.Content.ReadAsStringAsync(); //Gets data from API as string
                        using var parseAPI = JsonDocument.Parse(getPriceAPI); //Parse the string from API
                        var osrsItemPrice = parseAPI.RootElement.GetProperty("daily"); //Gets data associated with item 
                        var currentPriceString = osrsItemPrice.EnumerateObject().Last(); //Gets the latest price as string
                        int currentPrice = currentPriceString.Value.GetInt32(); //Converts string to int

                        var price = new Price //Variable for new price entry
                        {
                            ItemId = item.ItemId, //Set item id for FK
                            CurrentPrice = currentPrice, //Set current price
                            Date = DateTime.UtcNow //Sets the date with current date 
                        };
                        _context.Prices.Add(price); //Adds entry to the price table
                        await _context.SaveChangesAsync(); //save changes to db
                    }
                }catch (Exception)
                {
                    ModelState.AddModelError("", "An Error Occurred"); //Adds error message
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["ItemCategoryId"] = new SelectList(_context.ItemCategories, "ItemCategoryId", "Name", item.ItemCategoryId);
            return View(item);
        }

        // GET: Items/Edit/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            ViewData["ItemCategoryId"] = new SelectList(_context.ItemCategories, "ItemCategoryId", "Name", item.ItemCategoryId);
            return View(item);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ItemId,Name,Image,GameId,ItemCategoryId")] Item item)
        {
            if (id != item.ItemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(item.ItemId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ItemCategoryId"] = new SelectList(_context.ItemCategories, "ItemCategoryId", "Name", item.ItemCategoryId);
            return View(item);
        }

        // GET: Items/Delete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .Include(i => i.ItemCategory)
                .FirstOrDefaultAsync(m => m.ItemId == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // POST: Items/Delete/5
        [Authorize(Roles = "Administrator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item != null)
            {
                _context.Items.Remove(item);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemExists(int id)
        {
            return _context.Items.Any(e => e.ItemId == id);
        }

        [HttpGet]
        //Method for getting api to pass to javascript due to CORS issue
        public async Task<IActionResult> GetJsonForJs(int id)
        {
            var client = _httpClient.CreateClient(); //Creates a new instance of httpClient
            var apiLink = await client.GetAsync($"https://services.runescape.com/m=itemdb_oldschool/api/catalogue/detail.json?item={id}"); //Makes request using the link to get API values
            if (!apiLink.IsSuccessStatusCode) //Check if status code was a success
            {
                return NotFound(); //Return not found
            }
            var getAPI = await apiLink.Content.ReadAsStringAsync(); //Gets data from API as string
            using var parseAPI = JsonDocument.Parse(getAPI); //Parse the string from API
            var osrsItem = parseAPI.RootElement.GetProperty("item"); //Gets data associated with item 
            var infoForJson = new
            {
                name = osrsItem.GetProperty("name").GetString(), //Store item name to name key
                image = $"https://secure.runescape.com/m=itemdb_oldschool/obj_big.gif?id={id}" //Store image url to image key
            };
            return Json(infoForJson); //Returns the json file
        }
    }
}
