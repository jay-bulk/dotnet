using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HandOnEx6.Models;
using aBcrypt = BCrypt.Net.BCrypt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace HandOnEx6.Controllers
{
    public class AccountController : Controller
    {
        private readonly TaraStoreContext _context;

        public AccountController(TaraStoreContext context)
        {
            _context = context;
        }

        // GET: Account
        //public async Task<IActionResult> Index()
        //{
        //      return _context.LoginInfos != null ? 
        //                  View(await _context.LoginInfos.ToListAsync()) :
        //                  Problem("Entity set 'TaraStoreContext.LoginInfos'  is null.");
        //}

        // GET: Account/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null || _context.LoginInfos == null)
        //    {
        //        return NotFound();
        //    }

        //    var loginInfo = await _context.LoginInfos
        //        .FirstOrDefaultAsync(m => m.UserPK == id);
        //    if (loginInfo == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(loginInfo);
        //}


        // GET: Account/Login
        public IActionResult Login(string returnUrl)
        {
            returnUrl = string.IsNullOrEmpty(returnUrl) ? "~/" : returnUrl;
            return View(new LoginInput { ReturnURL = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Username, UserPassword, ReturnURL")] LoginInput input)
        {
            if (ModelState.IsValid)
            {
                var aUser = await _context.LoginInfos.FirstOrDefaultAsync(u => u.Uname == input.Username);

                if (aUser != null && aBcrypt.Verify(input.UserPassword, aUser.UPass))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, aUser.FullName),
                        new Claim(ClaimTypes.Sid, aUser.UserPK.ToString())
                    };
                    string[] roles = aUser.URole.Split(",");

                    foreach( string role in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return Redirect(input?.ReturnURL ?? "~/");
                }
                else
                {
                    ViewData["failure"] = "Invalid Credentials";
                }
            }
            return View(input);
        }

        // GET: Account/Create
        public IActionResult SignUp()
        {
            return View();
        }

        // POST: Account/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp([Bind("Uname,UPass,FullName")] LoginInfo loginInfo)
        {
            // Search account for user name to avoid duplication
            if (ModelState.IsValid)
            {
                var aUser = await _context.LoginInfos.FirstOrDefaultAsync(x => x.Uname == loginInfo.Uname);

                if (aUser is null)
                {
                    loginInfo.UPass = aBcrypt.HashPassword(loginInfo.UPass);
                    loginInfo.URole = "User";
                    _context.Add(loginInfo);
                    await _context.SaveChangesAsync();

                    TempData["success"] = "Account Created";
                    return RedirectToAction(nameof(Login));
                }

                ViewData["message"] = "Choose a different username";
            }
            return View(loginInfo);
        }

        public async Task<RedirectToActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }

        private bool LoginInfoExists(int id)
        {
          return (_context.LoginInfos?.Any(e => e.UserPK == id)).GetValueOrDefault();
        }
    }
}
