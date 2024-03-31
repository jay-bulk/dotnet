//Demo 7 - Shopping Cart; LV

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Demo7.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

//add the following namespace

using aBCryptNet = BCrypt.Net.BCrypt;

namespace Demo6.Controllers
{
    public class AccountController : Controller
    {
        private readonly TaraStoreContext _context;

        public AccountController(TaraStoreContext context)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Username,UserPassword,ReturnURL")] LoginInput loginInput)
        {
            if (ModelState.IsValid)
            {
                // retrieve user information

                var aUser = await _context.LoginInfos.FirstOrDefaultAsync(u => u.Uname == loginInput.Username);

                // if user exists and passwords match

                if (aUser != null && aBCryptNet.Verify(loginInput.UserPassword, aUser.UPass))
                {
                    // From Microsoft documentation - "A claim is a statement about a subject by an issuer. Claims represent attributes of the subject that are useful in the context of authentication and authorization operations"

                    // Examples of claims would be data on a Driver's License card (i.e., name, date of birth)

                    var claims = new List<Claim>();

                    // the Type property can be used to store information about the claim

                    claims.Add(new Claim(ClaimTypes.Name, aUser.FullName));
                    claims.Add(new Claim(ClaimTypes.Sid, aUser.UserPK.ToString()));

                    // role(s) are stored as a comma-delimited list in the "UserRoles" column in the LoginInfo table

                    string[] roles = aUser.URole.Split(",");

                    foreach (string role in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    // From Microsoft documentation - "The ClaimsIdentity class is a concrete implementation of a claims-based identity; that is, an identity described by a collection of claims."

                    // a collection of claims can be used to create a ClaimsIndentity along with the authentication scheme (in this case, cookie-based authentication)

                    // Example of identity would be a Driver's License card

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // multiple identities can be stored in a ClaimsPrincipal

                    // Example, a user's multiple identities (driver's license, employee ID, passport) can make up a ClaimsPrincipal

                    var principal = new ClaimsPrincipal(identity);

                    // the SignInAsync method issues the authentication cookie to the user

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    // return the user to the View they were originally trying to reach or Home/Index

                    return Redirect(loginInput?.ReturnURL ?? "~/");

                }

                // if credentials are not valid

                else
                {
                    ViewData["message"] = "Invalid credentials";
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
        public async Task<IActionResult> SignUp([Bind("Uname,UPass,FullName")] LoginInfo loginInfo)
        {
            if (ModelState.IsValid)
            {
                // check for duplicate username

                var aUser = await _context.LoginInfos.FirstOrDefaultAsync(u => u.Uname == loginInfo.Uname);

                // if no duplication

                if (aUser is null)
                {
                    // hash password

                    loginInfo.UPass = aBCryptNet.HashPassword(loginInfo.UPass);

                    // set default role to "user" and create new record in LoginInfo

                    loginInfo.URole = "User";

                    _context.Add(loginInfo);
                    await _context.SaveChangesAsync();

                    TempData["success"] = "Account created";

                    // redirect to Login View

                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    ViewData["message"] = "Choose a different username";
                }
            }
            // return user to SignUp View

            return View(loginInfo);
        }

        // method to log user out and redirect to Home View
        public async Task<RedirectToActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            return RedirectToAction("Index", "Home");
        }
    }
}
