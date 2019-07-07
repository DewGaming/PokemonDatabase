using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pokedex.DataAccess.Models;
using Pokedex.Models;

namespace Pokedex.Controllers
{
    [Authorize]
    [Route("")]
    public class AccountController : Controller
    {
        private readonly DataService _dataService;

        public AccountController(DataContext dataContext)
        {
            // Instantiate an instance of the data service.
            this._dataService = new DataService(dataContext);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("signup")]
        public IActionResult Register()
        {
            return this.View();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("signup")]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            User existingUser = this._dataService.GetUserWithUsername(registerViewModel.Username);
            if (existingUser != null && !existingUser.IsArchived)
            {
                this.ModelState.AddModelError("Error", "An account already exists with that username.");

                return this.View();
            }
            else if (existingUser != null && existingUser.IsArchived)
            {
                User updateUser = this.CompareUsers(existingUser, registerViewModel);

                existingUser.IsArchived = false;

                existingUser.IsAdmin = false;

                this._dataService.UpdateUser(updateUser);

                return this.RedirectToAction("Login", "Account");
            }

            PasswordHasher<string> passwordHasher = new PasswordHasher<string>();

            User user = new User()
            {
                Username = registerViewModel.Username,
                PasswordHash = passwordHasher.HashPassword(null, registerViewModel.Password),
            };

            this._dataService.AddUser(user);

            var claims = new List<Claim>
            {
                new Claim("UserId", user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.IsOwner ? "Owner" : user.IsAdmin ? "Admin" : "User"),
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new Microsoft.AspNetCore.Authentication.AuthenticationProperties { };

            await this.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return this.RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("login")]
        public IActionResult Login()
        {
            return this.View();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel, string returnUrl)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            User user = this._dataService.GetUserWithUsername(loginViewModel.Username);

            if (user == null || user.IsArchived)
            {
                // Set username not registered error message.
                this.ModelState.AddModelError("Error", "An account does not exist with that username.");

                return this.View();
            }

            PasswordHasher<string> passwordHasher = new PasswordHasher<string>();
            PasswordVerificationResult passwordVerificationResult =
                passwordHasher.VerifyHashedPassword(null, user.PasswordHash, loginViewModel.Password);

            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                // Set invalid password error message.
                this.ModelState.AddModelError("Error", "Invalid password.");

                return this.View();
            }

            var claims = new List<Claim>
            {
                new Claim("UserId", user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.IsOwner ? "Owner" : user.IsAdmin ? "Admin" : "User"),
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new Microsoft.AspNetCore.Authentication.AuthenticationProperties { };

            await this.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            if (string.IsNullOrEmpty(returnUrl))
            {
                return this.RedirectToAction("Index", "Home");
            }
            else
            {
                return this.Redirect(returnUrl);
            }
        }

        [HttpGet]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await this.HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return this.RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("access_denied")]
        public IActionResult AccessDenied()
        {
            return this.RedirectToAction("Index", "Home");
        }

        private User CompareUsers(User existingUser, RegisterViewModel newUser)
        {
            PasswordHasher<string> passwordHasher = new PasswordHasher<string>();
            string passwordHash = passwordHasher.HashPassword(null, newUser.Password);
            if (existingUser.Username != newUser.Username)
            {
                existingUser.Username = newUser.Username;
            }

            if (existingUser.PasswordHash != passwordHash)
            {
                existingUser.PasswordHash = passwordHash;
            }

            return existingUser;
        }
    }
}