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
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PokemonDatabase.Controllers
{
    [Authorize(Roles = "Owner"), Route("admin")]
    public class OwnerController : Controller  
    {
        private readonly DataService _dataService;

        public OwnerController(DataContext dataContext)
        {
            // Instantiate an instance of the data service.
            _dataService = new DataService(dataContext);
        }

        [Route("users")]
        public IActionResult Users()
        {
            List<User> model = _dataService.GetUsers();

            return View(model);
        }

        [HttpGet, Route("edit-user/{id:int}")]
        public IActionResult EditUser(int id)
        {
            User model = _dataService.GetUserById(id);

            return View(model);
        }

        [HttpPost, Route("edit-user/{id:int}")]
        public IActionResult EditUser(User user)
        {
            if (!ModelState.IsValid)
            {
                User model = _dataService.GetUser(user.EmailAddress);

                return View(model);
            }

            _dataService.UpdateUser(user);

            return RedirectToAction("Users");
        }

        [HttpGet, Route("delete-user/{id:int}")]
        public IActionResult DeleteUser(int id)
        {
            User model = _dataService.GetUserById(id);

            return View(model);
        }

        [HttpPost, Route("delete-user/{id:int}")]
        public IActionResult DeleteUser(User user)
        {
            _dataService.DeleteUser(user.Id);

            return RedirectToAction("Users");
        }
    }
}