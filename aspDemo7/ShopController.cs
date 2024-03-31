//Demo 7 - Shopping Cart; LV

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Demo7.Models;

namespace Demo7.Controllers
{
    public class ShopController : Controller
    {
        private readonly TaraStoreContext _context;

        public ShopController(TaraStoreContext context)
        {
            _context = context;
        }

       public async Task<IActionResult> Search(string searchName, decimal? priceMin, decimal? priceMax)
        {
            // the ViewData elements are used to pass back the filter values to the FilterDemo View

            ViewData["NameFilter"] = searchName;
            ViewData["PriceMinFilter"] = priceMin;
            ViewData["PriceMaxFilter"] = priceMax;

            var products = from p in _context.Products select p;

            // depending on the filter values (received as parameters from the query string in the URL), where methods are used to filter the IQueryable object, products

            if (!String.IsNullOrEmpty(searchName))
            {
                products = products.Where(p => p.ModelName.Contains(searchName));
            }
            if (priceMin != null)
            {
                products = products.Where(p => p.UnitCost >= priceMin);
            }
            if (priceMax != null)
            {
                products = products.Where(p => p.UnitCost <= priceMax);
            }

            return View(await products.ToListAsync());
        }

        // prepare output to display details for a product
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return RedirectToAction(nameof(Search));
            }

            var product = await _context.Products
                .Include(p => p.CategoryFKNavigation)
                .Include(p => p.SubCategoryFKNavigation)
                .FirstOrDefaultAsync(m => m.ProductPK == id);
            if (product == null)
            {
                return RedirectToAction(nameof(Search));
            }

            return View(product);
        }

        // add a product to shopping cart
        public async Task<IActionResult> AddToCart(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Search));
            }

            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductPK == id);

            if (product == null)
            {
                return RedirectToAction(nameof(Search));
            }

            // call to method to retrieve cart object from session state

            Cart aCart = GetCart();

            aCart.AddItem(product);

            // call to method to save cart object to session state

            SaveCart(aCart);

            return RedirectToAction(nameof(MyCart));
        }

        // prepare output to display items in cart object
        public IActionResult MyCart()
        {
            Cart aCart = GetCart();

            if (aCart.CartItems().Any())
            {
                return View(aCart);
            }

            // if the cart is empty

            return RedirectToAction(nameof(Search));
        }

        // update cart - i.e., the quantity for a product in the cart
        public IActionResult UpdateCart(int? productPK, int qty)
        {
            if (productPK == null)
            {
                return RedirectToAction(nameof(Search));
            }

            Cart aCart = GetCart();

            CartItem? aItem = aCart.GetCartItem(productPK);
                        
            if (aItem == null)
            {
                return RedirectToAction(nameof(Search));
            }
 
            aCart.UpdateItem(aItem.Product?.ProductPK, qty);

            SaveCart(aCart);

            return RedirectToAction(nameof(MyCart));
        }

        // remove an item from the cart
        public IActionResult RemoveFromCart(int? productPK)
        {
            if (productPK == null)
            {
                return RedirectToAction(nameof(Search));
            }

            Cart aCart = GetCart();

            CartItem? aItem = aCart.GetCartItem(productPK);

            if (aItem == null)
            {
                return RedirectToAction(nameof(Search));
            }

            aCart.RemoveItem(aItem.Product?.ProductPK);

            SaveCart(aCart);

            return RedirectToAction(nameof(MyCart));
        }

        //method to retrieve cart object from session state
        private Cart GetCart()
        {
            // call the session extension method GetObject
            // if a cart object doesn't exist, create a new cart object

            Cart aCart = HttpContext.Session.GetObject<Cart>("Cart") ?? new Cart();
            return aCart;
        }

        //method to save cart object to session state
        private void SaveCart(Cart aCart)
        {
            // call the session extension method SetObject

            HttpContext.Session.SetObject("Cart", aCart);
        }
    }
}
