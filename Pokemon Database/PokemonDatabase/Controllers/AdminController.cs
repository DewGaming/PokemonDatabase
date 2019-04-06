using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using PokemonDatabase.Models;
using PokemonDatabase.DataAccess.Models;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace PokemonDatabase.Controllers
{
    [Authorize(Roles = "Admin,Owner"), Route("admin")]
    public class AdminController : Controller
    {
        private readonly AppConfig _appConfig;

        private readonly DataService _dataService;

        public AdminController(IOptions<AppConfig> appConfig, DataContext dataContext)
        {
            _dataService = new DataService(dataContext);
            _appConfig = appConfig.Value;
        }

        [Route("")]
        public IActionResult Index()
        {
            ViewBag.ApplicationName = _appConfig.AppName;
            ViewBag.ApplicationVersion = _appConfig.AppVersion;

            return View();
        }

        [Route("pokemon")]
        public IActionResult Pokemon()
        {
            List<PokemonTypeDetail> model = _dataService.GetPokemonWithTypes();

            return View(model);
        }

        [Route("generation")]
        public IActionResult Generations()
        {
            List<Generation> model = _dataService.GetGenerations();

            return View(model);
        }

        [Route("type")]
        public IActionResult Types()
        {
            List<Type> model = _dataService.GetTypes();

            return View(model);
        }

        [Route("ability")]
        public IActionResult Abilities()
        {
            List<Ability> model = _dataService.GetAbilities();

            return View(model);
        }

        [Route("egggroup")]
        public IActionResult EggGroups()
        {
            List<EggGroup> model = _dataService.GetEggGroups();

            return View(model);
        }

        [HttpGet, Route("add-generation")]
        public IActionResult AddGeneration()
        {
            return View();
        }

        [HttpPost, Route("add-generation")]
        public IActionResult AddGeneration(Generation generation)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            _dataService.AddGeneration(generation);

            return RedirectToAction("Generations");
        }

        [HttpGet, Route("edit-generation/{id}")]
        public IActionResult EditGeneration(string id)
        {
            Generation model = _dataService.GetGeneration(id);

            return View(model);
        }

        [HttpPost, Route("edit-generation/{id}")]
        public IActionResult EditGeneration(Generation generation)
        {
            if (!ModelState.IsValid)
            {
                Generation model = _dataService.GetGeneration(generation.Id);

                return View(model);
            }

            _dataService.UpdateGeneration(generation);

            return RedirectToAction("Generations");
        }

        [HttpGet, Route("delete-generation/{id}")]
        public IActionResult DeleteGeneration(string id)
        {
            Generation model = _dataService.GetGeneration(id);

            return View(model);
        }

        [HttpPost, Route("delete-generation/{id}")]
        public IActionResult DeleteGeneration(Generation generation)
        {
            _dataService.DeleteGeneration(generation.Id);

            return RedirectToAction("Generations");
        }

        [Route("error")]
        public IActionResult Error()
        {
            return View();
        }
    }
}
