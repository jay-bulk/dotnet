using HandsOnEx4.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HandsOnEx4.Controllers
{
    public class HOE4Controller : Controller
    {
        private readonly TaraStoreContext _storeContext;
        public HOE4Controller(TaraStoreContext storeContext)
        {
            _storeContext = storeContext;
        }

        public IActionResult TableView()
        {
            var products = _storeContext.Products.Include(p => p.CategoryFKNavigation).Include(p => p.SubCategoryFKNavigation).ToList();

            return View(products);
        }

        public IActionResult ListView()
        {
            var products = _storeContext.Products.OrderBy(p => p.ModelName).ToList();
            return View(products);
        }

        [HttpGet]
        public IActionResult SearchView()
        {
            List<Product> productList = new();
            return View(productList);
        }

        [HttpPost]
        public IActionResult SearchView(string searchName, decimal? priceMin, decimal? priceMax)
        {
            var products = from p in _storeContext.Products select p;

            if (!string.IsNullOrEmpty(searchName) )
            {
                products = products.Where(p => p.ModelName.Contains(searchName));
            }
            if (priceMin != null )
            {
                products = products.Where(p => p.UnitCost >= priceMin);
            }
            if (priceMax != null )
            {
                products = products.Where(p => p.UnitCost <= priceMax);
            }
            ViewData["NameFilter"] = searchName;
            ViewData["PriceMinFilter"] = priceMin;
            ViewData["PriceMaxFilter"] = priceMax;

            return View(products.OrderBy(p => p.ModelName).ThenBy(p => p.UnitCost).ToList());
        }
    }
}
