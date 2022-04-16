using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pokedex.DataAccess.Models;
using Pokedex.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Pokedex.Controllers
{
    /// <summary>
    /// The class that handles the account related actions.
    /// </summary>
    [Authorize]
    [Route("")]
    public class AccountController : Controller
    {
        private readonly DataService dataService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        /// <param name="dataContext">The pokemon database's context.</param>
        public AccountController(DataContext dataContext)
        {
            // Instantiate an instance of the data service.
            this.dataService = new DataService(dataContext);
        }

        /// <summary>
        /// The method that is used when a user tries to sign up.
        /// </summary>
        /// <returns>The sign up page.</returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("signup")]
        public IActionResult Register()
        {
            return this.View();
        }

        /// <summary>
        /// The method that is used when a user tries to sign up.
        /// </summary>
        /// <param name="registerViewModel">The register credentials.</param>
        /// <returns>The home page.</returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("signup")]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            registerViewModel.Username = registerViewModel.Username.Trim();

            User existingUser = this.dataService.GetObjectByPropertyValue<User>("Username", registerViewModel.Username);
            if (existingUser != null)
            {
                this.ModelState.AddModelError("Error", "An account already exists with that username.");

                return this.View();
            }

            PasswordHasher<string> passwordHasher = new PasswordHasher<string>();

            User user = new User()
            {
                Username = registerViewModel.Username,
                PasswordHash = passwordHasher.HashPassword(null, registerViewModel.Password),
            };

            this.dataService.AddObject(user);

            List<Claim> claims = new List<Claim>
            {
                new Claim("UserId", user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.IsOwner ? "Owner" : user.IsAdmin ? "Admin" : "User"),
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await this.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    AllowRefresh = true,
                }).ConfigureAwait(false);

            return this.RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// The method that is used when a user tries to log in.
        /// </summary>
        /// <returns>The log in page.</returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("login")]
        public IActionResult Login()
        {
            return this.View();
        }

        /// <summary>
        /// The method that is used when a user tries to log in.
        /// </summary>
        /// <param name="loginViewModel">The login credentials.</param>
        /// <param name="returnUrl">The url the user came from.</param>
        /// <returns>The home page.</returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel, string returnUrl)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            User user = this.dataService.GetObjectByPropertyValue<User>("Username", loginViewModel.Username);

            if (user == null)
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

            List<Claim> claims = new List<Claim>
            {
                new Claim("UserId", user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
            };

            if (user.IsOwner)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Owner"));
            }

            if (user.IsAdmin)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }

            if (!user.IsOwner && !user.IsAdmin)
            {
                claims.Add(new Claim(ClaimTypes.Role, "User"));
            }

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await this.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    AllowRefresh = true,
                }).ConfigureAwait(false);

            if (string.IsNullOrEmpty(returnUrl))
            {
                return this.RedirectToAction("Index", "Home");
            }
            else
            {
                return this.Redirect(returnUrl);
            }
        }

        /// <summary>
        /// The method that is used when a user wants to log out.
        /// </summary>
        /// <returns>The home page.</returns>
        [HttpGet]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);

            return this.RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// The method that is used when a user tries to access a page without permissions.
        /// </summary>
        /// <returns>The access denied page.</returns>
        [HttpGet]
        [Route("access_denied")]
        public IActionResult AccessDenied()
        {
            return this.RedirectToAction("Index", "Home");
        }
    }
}
