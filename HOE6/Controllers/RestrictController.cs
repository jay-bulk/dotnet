using HandOnEx6.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HandOnEx6.Controllers
{
    public class RestrictController : Controller
    {
        private readonly TaraStoreContext _context;
        public RestrictController(TaraStoreContext context)
        {
            _context = context;

        }

        [Authorize]
        public async Task<ActionResult> MyOrders()
        {
            int userPk = Int32.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value);

            var orderDetails = _context.tblOrderDetails
                .Include(od => od.OrderFKNavigation)
                .Include(od => od.ProductFKNavigation)
                .Where(u => u.OrderFKNavigation.CustomerFK == userPk)
                .OrderBy(d => d.OrderFKNavigation.OrderDate);

            return View(await orderDetails.ToListAsync());

        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AllOrders()
        {

            var customer = _context.LoginInfos
                .Where(c => c.tblOrders.Count > 0)
                .Include(cust => cust.tblOrders)
                .ThenInclude(order => order.tblOrderDetails)
                .ThenInclude(detail => detail.ProductFKNavigation)
                .OrderBy(c => c.FullName);

            return View(await customer.ToListAsync());

        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
