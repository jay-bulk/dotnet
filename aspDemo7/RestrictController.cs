//Demo 7 - Shopping Cart; LV
#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Demo7.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Demo7.Controllers
{
    public class RestrictController : Controller
    {
        private readonly TaraStoreContext _context;

        public RestrictController(TaraStoreContext context)
        {
            _context = context;
        }

        [Authorize]
        public async Task<IActionResult> MyOrders()
        {
            // retrieve the user's PK from the Claims collection
            // since the PK is stored as a string, it has to be parsed to an integer

            int userPK = Int32.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value);

            // retrieve the user's orders

            var orderDetails = _context.tblOrderDetails
                .Include(od => od.OrderFKNavigation)
                .Include(od => od.ProductFKNavigation)
                .Where(u => u.OrderFKNavigation.CustomerFK == userPK)
                .OrderBy(d => d.OrderFKNavigation.OrderDate);

            return View(await orderDetails.ToListAsync());
        }

        [Authorize]
        public IActionResult CheckOut()
        {
            return RedirectToAction("MyCart", "Shop");
        }

        [Authorize]
        public IActionResult PlaceOrder()
        {
            Cart aCart = GetCart();

            if (aCart.CartItems().Any())
            {
                int userPK = Int32.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value);

                // insert order

                tblOrder aOrder = new () { CustomerFK = userPK, OrderDate = DateTime.Now };

                _context.Add(aOrder);
                _context.SaveChanges();

                // get the PK of the newly inserted order

                int orderPK = aOrder.OrderPK;

                // insert a orderdetail for each item in the cart

                foreach (CartItem aItem in aCart.CartItems())
                {
                    tblOrderDetail aDetail = new () { ProductFK = aItem.Product.ProductPK, Quantity = aItem.Quantity, OrderFK = orderPK };
                    _context.Add(aDetail);
                }

                _context.SaveChanges();

                // remove all items from cart

                aCart.ClearCart();

                SaveCart(aCart);

                return View(nameof(OrderConfirmation));
            }

            return RedirectToAction("Search", "Shop");
        }

        private IActionResult OrderConfirmation()
        {
            return View();
        }

        private Cart GetCart()
        {
            Cart aCart = HttpContext.Session.GetObject<Cart>("Cart") ?? new Cart();
            return aCart;
        }

        private void SaveCart(Cart aCart)
        {
            HttpContext.Session.SetObject("Cart", aCart);
        }

    }
}
