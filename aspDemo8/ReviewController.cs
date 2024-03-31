//Demo 8 - Complete Application; LV
#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Demo8.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Demo8.Controllers
{
    public class ReviewController : Controller
    {
        private readonly RWStudiosContext _context;

        public ReviewController(RWStudiosContext context)
        {
            _context = context;
        }

        // GET: Review/Create

        [Authorize]
        public IActionResult Create(int? id)
        {
            //if id is null

            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            //if id is not valid

            var film = _context.Films.FirstOrDefault(f => f.FilmPK == id);

            if (film == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // retrieve user's PK from the Claims collection
            // since the PK is stored as a string, it has to be parsed to an integer

            int userPK = Int32.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value);

            // Check if user already has a review for film

            var uReview = _context.FilmReviews
                          .FirstOrDefault(fr => fr.FilmFK == id && fr.ContactFK == userPK);

            // If user has a review, redirect to Edit

            if (uReview != null)
            {
                return RedirectToAction(nameof(Edit), new { id = uReview.ReviewPK });
            }

            //set ViewData to display film title in View

            ViewData["FilmTitle"] = film.MovieTitle;

            //Prepare a SelectList for the Star Rating (see StarList method below)

            ViewData["StarRating"] = StarList();

            // create a new FilmReview object and set FilmFK

            FilmReview aReview = new FilmReview { FilmFK = film.FilmPK };

            return View(aReview);
        }

        // POST: Review/Create

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FilmFK,ReviewSummary,ReviewRating")] FilmReview aReview)
        {
            //if filmpk is not valid

            var film = _context.Films.FirstOrDefault(f => f.FilmPK == aReview.FilmFK);

            if (film == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                // retrieve user's PK from the Claims collection
                // since the PK is stored as a string, it has to be parsed to an integer

                int userPK = Int32.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value);

                //set rest of the properties for the new review object

                aReview.ContactFK = userPK;
                aReview.ReviewDate = DateTime.Now;
                aReview.ReviewUpdateDate = DateTime.Now;

                //save new review object

                try
                {
                    _context.Add(aReview);
                    await _context.SaveChangesAsync();
                }
                catch
                {
                    TempData["failure"] = $"Your review of {film.MovieTitle} not added";
                    return RedirectToAction(nameof(MyReviews));
                }

                TempData["success"] = $"Your review of {film.MovieTitle} added";
                return RedirectToAction(nameof(MyReviews));
            }
            
            // if the data is not valid

            //set ViewData to display film title in View

            ViewData["FilmTitle"] = film.MovieTitle;

            //Prepare a SelectList for the Star Rating (see StarList method below)

            ViewData["StarRating"] = StarList(aReview.ReviewRating);

            return View(aReview);
        }

        // GET: Review/Edit/5

        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // retrieve user's PK from the Claims collection
            // since the PK is stored as a string, it has to be parsed to an integer

            int userPK = Int32.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value);

            // retrieve user's review

            var aReview = await _context.FilmReviews
                .Include(fr => fr.FilmFKNavigation)
                .FirstOrDefaultAsync(fr => fr.ReviewPK == id && fr.ContactFK == userPK);

            //if id is not valid

            if (aReview == null)
            {
                return RedirectToAction("Index", "Home");
            }

            //set ViewData to display film title in View

            ViewData["FilmTitle"] = aReview.FilmFKNavigation.MovieTitle;

            //Prepare a SelectList for the Star Rating (see StarList method below)

            ViewData["StarRating"] = StarList(aReview.ReviewRating);

            return View(aReview);
        }

        // POST: Review/Edit/5

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReviewPK,ReviewSummary,ReviewRating")] FilmReview aReview)
        {
            if (id != aReview.ReviewPK)
            {
                return RedirectToAction("Index", "Home");
            }

            // retrieve user's PK from the Claims collection
            // since the PK is stored as a string, it has to be parsed to an integer

            int userPK = Int32.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value);

            // retrieve original review

            var oReview = await _context.FilmReviews
                .Include(fr => fr.FilmFKNavigation)
                .FirstOrDefaultAsync(fr => fr.ReviewPK == id && fr.ContactFK == userPK);

            //if reviewpk is not valid

            if (oReview == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                // update original review

                oReview.ReviewSummary = aReview.ReviewSummary;
                oReview.ReviewRating = aReview.ReviewRating;
                oReview.ReviewUpdateDate = DateTime.Now;

                try
                {
                    _context.Update(oReview);
                    await _context.SaveChangesAsync();
                }
                catch
                {
                    TempData["failure"] = $"Your review of {oReview.FilmFKNavigation.MovieTitle} not updated";
                    return RedirectToAction(nameof(MyReviews));
                }

                TempData["success"] = $"Your review of {oReview.FilmFKNavigation.MovieTitle} updated";
                return RedirectToAction(nameof(MyReviews));
            }

            // if the data is not valid

            //set ViewData to display film title in View

            ViewData["FilmTitle"] = oReview.FilmFKNavigation.MovieTitle;

            //Prepare a SelectList for the Star Rating (see StarList method below)

            ViewData["StarRating"] = StarList(aReview.ReviewRating);

            return View(aReview);
        }

        // GET: Review/Delete/5

        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // retrieve user's PK from the Claims collection
            // since the PK is stored as a string, it has to be parsed to an integer

            int userPK = Int32.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value);

            // retrieve user's review

            var aReview = await _context.FilmReviews
                .Include(fr => fr.FilmFKNavigation)
                .FirstOrDefaultAsync(fr => fr.ReviewPK == id && fr.ContactFK == userPK);

            //if reviewpk is not valid

            if (aReview == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(aReview);
        }

        // POST: Review/Delete/5

        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // retrieve user's PK from the Claims collection
            // since the PK is stored as a string, it has to be parsed to an integer

            int userPK = Int32.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value);

            // retrieve user's review

            var aReview = await _context.FilmReviews
                .Include(fr => fr.FilmFKNavigation)
                .FirstOrDefaultAsync(fr => fr.ReviewPK == id && fr.ContactFK == userPK);

            //if reviewpk is not valid

            if (aReview == null)
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                _context.FilmReviews.Remove(aReview);
                await _context.SaveChangesAsync();
            }
            catch
            {
                 TempData["failure"] = $"Your review of {aReview.FilmFKNavigation.MovieTitle} not deleted";
                return RedirectToAction(nameof(MyReviews));
             }

            TempData["success"] = $"Your review of {aReview.FilmFKNavigation.MovieTitle} deleted";
            return RedirectToAction(nameof(MyReviews));
        }

        [Authorize]
        public async Task<IActionResult> MyReviews()
        {
            // retrieve the user's PK from the Claims collection
            // since the PK is stored as a string, it has to be parsed to an integer

            int userPK = Int32.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value);

            // retrieve the user's reviewers

            var userReviews = _context.FilmReviews
                .Include(fr => fr.FilmFKNavigation)
                .Where(fr => fr.ContactFK == userPK)
                .OrderByDescending(fr => fr.ReviewDate);

            return View(await userReviews.ToListAsync());
        }

        private List<SelectListItem> StarList(int aStar = 0)
        {
            //Prepare a SelectList for the Star Rating

            List<SelectListItem> starList = new List<SelectListItem>();

            for (int i = 1; i <= 10; i++)
            {
                if (i == aStar)
                {
                    starList.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    starList.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            starList.Insert(0, (new SelectListItem { Text = "Select a Rating", Value = "" }));

            return starList;
        }
    }
}
