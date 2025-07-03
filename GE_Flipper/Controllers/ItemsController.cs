using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly IHttpClientFactory _httpClient;

        public ItemsController(ApplicationDbContext context, IHttpClientFactory httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        // GET: Items
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Items.Include(i => i.ItemCategory);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Items/Details/5
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
        public IActionResult Create()
        {
            ViewData["ItemCategoryId"] = new SelectList(_context.ItemCategories, "ItemCategoryId", "Name");
            return View();
        }

        // POST: Items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemId,GameId,ItemCategoryId")] Item item)
        {
            if (ModelState.IsValid)
            {
                bool itemExsists = await _context.Items.AnyAsync(i => i.GameId == item.GameId);
                if (itemExsists)
                {
                    ModelState.AddModelError("", "Item already Exists");
                    ViewData["ItemCategoryId"] = new SelectList(_context.ItemCategories, "ItemCategoryId", "Name", item.ItemCategoryId);
                    return View(item);
                }
                var client = _httpClient.CreateClient();
                var apiLink = await client.GetAsync($"https://services.runescape.com/m=itemdb_oldschool/api/catalogue/detail.json?item={item.GameId}");
                if (apiLink.IsSuccessStatusCode)
                {
                    var getAPI = await apiLink.Content.ReadAsStringAsync();
                    using var parseAPI = JsonDocument.Parse(getAPI);
                    var osrsItem = parseAPI.RootElement.GetProperty("item");
                    item.Name = osrsItem.GetProperty("name").GetString();
                    item.Image = $"https://secure.runescape.com/m=itemdb_oldschool/obj_big.gif?id={item.GameId}";
                }
                _context.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ItemCategoryId"] = new SelectList(_context.ItemCategories, "ItemCategoryId", "Name", item.ItemCategoryId);
            return View(item);
        }

        // GET: Items/Edit/5
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
    }
}
