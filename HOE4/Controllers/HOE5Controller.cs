using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HandsOnEx4.Models;

namespace HandsOnEx4.Controllers
{
    public class HOE5Controller : Controller
    {
        private readonly TaraStoreContext _context;

        public HOE5Controller(TaraStoreContext context)
        {
            _context = context;
        }

        // GET: HOE5
        public async Task<IActionResult> Index()
        {
            var taraStoreContext = _context.Products.Include(p => p.CategoryFKNavigation).Include(p => p.SubCategoryFKNavigation).OrderBy(p => p.ModelName);
            return View(await taraStoreContext.ToListAsync());
        }

        // GET: HOE5/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return RedirectToAction("Index");
            }

            var product = await _context.Products
                .Include(p => p.CategoryFKNavigation)
                .Include(p => p.SubCategoryFKNavigation)
                .FirstOrDefaultAsync(m => m.ProductPK == id);
            if (product == null)
            {
                return RedirectToAction("Index");
            }

            return View(product);
        }

        // GET: HOE5/Create
        public IActionResult Create()
        {
            ViewData["CategoryFK"] = new SelectList(_context.Categories.OrderBy(c => c.CategoryName), "CategoryPK", "CategoryName");
            ViewData["SubCategoryFK"] = new SelectList(_context.SubCategories.OrderBy(s => s.SubCategoryName), "SubCategoryPK", "SubCategoryName");
            return View();
        }

        // POST: HOE5/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryFK,SubCategoryFK,ModelNumber,ModelName,ProductImage,UnitCost,Description,Thumbnail,Availability")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                TempData["message"] = $"{product.ModelName} added";
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryFK"] = new SelectList(_context.Categories.OrderBy(c => c.CategoryName), "CategoryPK", "CategoryName", product.CategoryFK);
            ViewData["SubCategoryFK"] = new SelectList(_context.SubCategories.OrderBy(s => s.SubCategoryName), "SubCategoryPK", "SubCategoryName", product.SubCategoryFK);
            return View(product);
        }

        // GET: HOE5/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryFK"] = new SelectList(_context.Categories.OrderBy(c => c.CategoryName), "CategoryPK", "CategoryName", product.CategoryFK);
            ViewData["SubCategoryFK"] = new SelectList(_context.SubCategories.OrderBy(s => s.SubCategoryName), "SubCategoryPK", "SubCategoryName", product.SubCategoryFK);
            return View(product);
        }

        // POST: HOE5/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductPK,CategoryFK,SubCategoryFK,ModelNumber,ModelName,ProductImage,UnitCost,Description,Thumbnail,Availability")] Product product)
        {
            if (id != product.ProductPK)
            {
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductPK))
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
                TempData["message"] = $"{product.ModelName} updated";
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryFK"] = new SelectList(_context.Categories.OrderBy(c => c.CategoryName), "CategoryPK", "CategoryName", product.CategoryFK);
            ViewData["SubCategoryFK"] = new SelectList(_context.SubCategories.OrderBy(s => s.SubCategoryName), "SubCategoryPK", "SubCategoryName", product.SubCategoryFK);
            return View(product);
        }

        // GET: HOE5/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var product = await _context.Products
                .Include(p => p.CategoryFKNavigation)
                .Include(p => p.SubCategoryFKNavigation)
                .FirstOrDefaultAsync(m => m.ProductPK == id);
            if (product == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }

        // POST: HOE5/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return RedirectToAction(nameof(Index));
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);

                await _context.SaveChangesAsync();
                TempData["message"] = $"{product.ModelName} deleted";
                return RedirectToAction(nameof(Index));
            } 
            else
            {
                TempData["message"] = $"Delete Unsuccessful";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool ProductExists(int id)
        {
          return (_context.Products?.Any(e => e.ProductPK == id)).GetValueOrDefault();
        }
    }
}
