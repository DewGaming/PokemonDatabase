using System;
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

        [Route("pokemon")]
        public IActionResult AllPokemon()
        {
            List<Pokemon> model = _dataService.GetAllPokemon();

            return View(model);
        }

        [Route("pokemon/{Name}")]
        public IActionResult Pokemon(string Name)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            if (Name != textInfo.ToTitleCase(Name))
            {
                Name = textInfo.ToTitleCase(Name);
            }

            PokemonViewModel model = new PokemonViewModel();
            model.pokemon = _dataService.GetPokemon(Name);
            model.baseStats = _dataService.GetBaseStat(model.pokemon.Id);
            model.evYields = _dataService.GetEVYield(model.pokemon.Id);
            model.types = _dataService.GetTyping(model.pokemon.Id);
            model.abilities = _dataService.GetAbilities(model.pokemon.Id);

            return View(model);
        }
    }
}
