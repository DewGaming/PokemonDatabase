using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using PokemonDatabase.Models;

namespace PokemonDatabase.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        private readonly AppConfig _appConfig;

        private readonly DataService _dataService;

        public HomeController(IOptions<AppConfig> appConfig, DataContext dataContext)
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

        [Route("ability/{ID:int}")]
        public IActionResult Ability(int ID)
        {
            Ability model = _dataService.GetAbility(ID);

            return View(model);
        }

        [Route("type")]
        public IActionResult Type()
        {
            List<PokemonDatabase.Models.Type> model = _dataService.GetTypes();

            return View(model);
        }

        [Route("egggroup")]
        public IActionResult EggGroup()
        {
            List<EggGroup> model = _dataService.GetEggGroups();

            return View(model);
        }
    }
}
