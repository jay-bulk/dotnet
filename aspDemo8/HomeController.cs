//Demo 8 - Complete Application; LV;

using Demo8.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Demo8.Controllers
{
    public class HomeController : Controller
    {
        private readonly RWStudiosContext _context;

        public HomeController(RWStudiosContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }


        //Method to search and filter films

        public async Task<IActionResult> Search(string searchTitle, string searchTag, int searchRating = 0)
        {
            ViewData["TitleFilter"] = searchTitle;
            ViewData["TagFilter"] = searchTag;

            List<SelectListItem> ratingList = new SelectList(_context.FilmRatings.OrderBy(r => r.Rating), "RatingPK", "Rating", searchRating).ToList();

            ratingList.Insert(0, (new SelectListItem { Text = "Select a Rating", Value = "0" }));

            ViewData["RatingFilter"] = ratingList;

            var films = from f in _context.Films select f;

            if (!String.IsNullOrEmpty(searchTitle))
            {
                films = films.Where(f => f.MovieTitle.Contains(searchTitle));
            }
            if (!String.IsNullOrEmpty(searchTag))
            {
                films = films.Where(f => f.PitchText.Contains(searchTag));
            }
            if (searchRating > 0)
            {
                films = films.Where(f => f.RatingFK == searchRating);
            }

            return View(await films.Include(f => f.RatingFKNavigation).OrderBy(f => f.MovieTitle).ToListAsync());
        }

        //Method to retrieve reviews for a film
        public async Task<IActionResult> Reviews(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Search));
            }

            var film = _context.Films.FirstOrDefault(f => f.FilmPK == id);

            if (film == null)
            {
                return RedirectToAction(nameof(Search));
            }

            ViewData["FilmTitle"] = film.MovieTitle;
            ViewData["FilmPK"] = film.FilmPK;

            var filmReviews = _context.FilmReviews
                .Include(fr => fr.FilmFKNavigation)
                .Include(fr => fr.ContactFKNavigation)
                .Where(fr => fr.FilmFK == id)
                .OrderByDescending(fr => fr.ReviewDate);

            return View(await filmReviews.ToListAsync());
        }

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
