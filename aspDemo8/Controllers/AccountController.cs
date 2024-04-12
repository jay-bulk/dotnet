//Demo 8 - Complete Application; LV

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Demo8.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

//add the following namespace

using aBCryptNet = BCrypt.Net.BCrypt;

namespace Demo8.Controllers
{
    public class AccountController : Controller
    {
        private readonly RWStudiosContext _context;

        public AccountController(RWStudiosContext context)
        {
            _context = context;
        }

        // the returnURL captures the View the user was trying to reach before being redirected to the Login View

        public IActionResult Login(string returnURL)
        {
            // if returnURL is null or empty, it is set to "/" (i.e., Home/Index)

            returnURL = String.IsNullOrEmpty(returnURL) ? "~/" : returnURL;

            // create a new instance of LoginInput and pass it to the Login View

            return View(new LoginInput { ReturnURL = returnURL });
        }

        // Post action (when user submits the Login form)

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("UserLogin,UserPassword,ReturnURL")] LoginInput loginInput)
        {
            if (ModelState.IsValid)
            {
                // retrieve user information

                var aUser = await _context.Contacts
                    .Include(u => u.UserRoleFKNavigation)
                    .FirstOrDefaultAsync(u => u.UserLogin == loginInput.UserLogin);

                // if user exists and passwords match

                if (aUser != null && aBCryptNet.Verify(loginInput.UserPassword, aUser.UserPassword))
                {
                    var claims = new List<Claim>();

                   // the Type property can be used to store information about the claim

                    claims.Add(new Claim(ClaimTypes.Name, $"{aUser.FirstName} {aUser.LastName}"));
                    claims.Add(new Claim(ClaimTypes.Sid, aUser.ContactPK.ToString()));
                    claims.Add(new Claim(ClaimTypes.Role, aUser.UserRoleFKNavigation.UserRoleName));
                    
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var principal = new ClaimsPrincipal(identity);

                    // the SignInAsync method issues the authentication cookie to the user

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    // return the user to the View they were originally trying to reach or Home/Index

                    return Redirect(loginInput?.ReturnURL ?? "~/");
                }

                // if credentials are not valid

                else
                {
                    ViewData["failure"] = "Invalid credentials";
                }
            }

            // return user to Login View

            return View(loginInput);
        }

        // GET: Sign up for an Account
        public IActionResult SignUp()
        {
            return View();
        }

        // Post action (when user submits the SignUp form)

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp([Bind("FirstName,LastName,Address,City,State,Zip,Country,Email,Phone,UserLogin,UserPassword,MailingList")] Contact aContact)
        {
            if (ModelState.IsValid)
            {
                // check for duplicate username

                var aUser = await _context.Contacts.FirstOrDefaultAsync(u => u.UserLogin == aContact.UserLogin);

                // if no duplication

                if (aUser is null)
                {
                    // hash password

                    aContact.UserPassword = aBCryptNet.HashPassword(aContact.UserPassword);

                    // set default role to "user" and create new record in LoginInfo

                    aContact.UserRoleFK = 5;

                    _context.Add(aContact);
                    await _context.SaveChangesAsync();

                    TempData["success"] = "Account created";

                    // redirect to Login View

                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    ViewData["failure"] = "Choose a different username";
                }
            }
            // return user to SignUp View

            return View(aContact);
        }

        // method to log user out and redirect to Home View
        public async Task<RedirectToActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            return RedirectToAction("Index", "Home");
        }
    }
}
