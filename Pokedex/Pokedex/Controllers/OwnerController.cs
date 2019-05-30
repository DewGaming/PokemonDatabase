using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pokedex.DataAccess.Models;
using Pokedex.Models;

namespace Pokedex.Controllers
{
    [Authorize(Roles = "Owner")]
    [Route("admin")]
    public class OwnerController : Controller
    {
        private readonly DataService _dataService;

        public OwnerController(DataContext dataContext)
        {
            // Instantiate an instance of the data service.
            this._dataService = new DataService(dataContext);
        }

        [Route("users")]
        public IActionResult Users()
        {
            List<User> model = this._dataService.GetUsers();

            return this.View(model);
        }
    }
}