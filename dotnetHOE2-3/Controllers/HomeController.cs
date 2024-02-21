using HandsOnEx.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HandsOnEx.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewBag.Message = "Mama, I just killed a man";
            return View("MyView");
        }

        [HttpGet]
        public IActionResult FavDestinationForm()
        {
            return View();
        }

        [HttpPost]
        public IActionResult FavDestinationForm(FavDestination aDestination)
        {
            if (ModelState.IsValid)
            {
                FavDestinationList.AddFavDestination(aDestination);
                return View("Confirmation", aDestination);
            }
            else
            {
                return View();
            }
        }
        public IActionResult ShowDestinations()
        {
            return View(FavDestinationList.FavDestinations());
        }

        // public IActionResult Index()
        // {
        //     return View();
        // }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
