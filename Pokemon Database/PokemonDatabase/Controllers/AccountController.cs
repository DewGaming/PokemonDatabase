using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using PokemonDatabase.Models;
using PokemonDatabase.DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace PokemonDatabase.Controllers
{
    [Authorize]
    [Route("")]
    public class AccountController : Controller  
    {
        private readonly DataService _dataService;

        public AccountController(DataContext dataContext)
        {
            // Instantiate an instance of the data service.
            _dataService = new DataService(dataContext);
        }

        [AllowAnonymous, HttpGet, Route("signup")]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous, HttpPost, Route("signup")]
        public IActionResult Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            User existingUser = _dataService.GetUserWithUsername(registerViewModel.Username);
            if (existingUser != null) 
            {
                // Set email address already in use error message.
                ModelState.AddModelError("Error", "An account already exists with that username.");

                return View();
            }

            existingUser = _dataService.GetUserWithEmail(registerViewModel.EmailAddress);
            if (existingUser != null) 
            {
                // Set email address already in use error message.
                ModelState.AddModelError("Error", "An account already exists with that email address.");

                return View();
            }

            PasswordHasher<string> passwordHasher = new PasswordHasher<string>();

            User user = new User()
            {
                FirstName = registerViewModel.FirstName,
                LastName = registerViewModel.LastName,
                EmailAddress = registerViewModel.EmailAddress,
                Username = registerViewModel.Username,
                PasswordHash = passwordHasher.HashPassword(null, registerViewModel.Password)
            };
        
            _dataService.AddUser(user);

            return RedirectToAction("Login", "Account");
        }

        [AllowAnonymous, HttpGet, Route("login")]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous, HttpPost, Route("login")]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel, string returnUrl)
        {          
            if (!ModelState.IsValid)
            {
                return View();
            }

            User user = _dataService.GetUserWithUsername(loginViewModel.Username);

            if (user == null) 
            {
                // Set email address not registered error message.
                ModelState.AddModelError("Error", "An account does not exist with that email address.");
            
                return View();
            }

            PasswordHasher<string> passwordHasher = new PasswordHasher<string>();
            PasswordVerificationResult passwordVerificationResult = 
                passwordHasher.VerifyHashedPassword(null, user.PasswordHash, loginViewModel.Password);

            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                // Set invalid password error message.
                ModelState.AddModelError("Error", "Invalid password.");

                return View();
            }
            
            if (passwordVerificationResult == PasswordVerificationResult.SuccessRehashNeeded)
            {
                
            }

            var claims = new List<Claim>
            {
                new Claim("UserId", user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.IsOwner ? "Owner" : user.IsAdmin ? "Admin" : "User")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new Microsoft.AspNetCore.Authentication.AuthenticationProperties {};

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, 
                new ClaimsPrincipal(claimsIdentity), 
                authProperties);

            if (string.IsNullOrEmpty(returnUrl))
            {
                return RedirectToAction("Index", "Home");
            }
            else 
            {
                return Redirect(returnUrl);
            }
        }

        [HttpGet, Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet, Route("access-denied")]
        public IActionResult AccessDenied()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}