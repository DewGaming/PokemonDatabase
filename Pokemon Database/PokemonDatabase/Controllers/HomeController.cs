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
using System.IO;

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
            List<PokemonTypeDetail> model = _dataService.GetPokemonWithTypes();

            return View(model);
        }

        [Route("team_generator")]
        public IActionResult TeamGenerator()
        {
            Pokemon pokemon;
            List<Pokemon> allPokemon = _dataService.GetAllPokemon().Where(x => x.Id.IndexOf('-') == -1).ToList();
            List<PokemonViewModel> model = new List<PokemonViewModel>();
            Random ram = new Random();
            for(var i = 0; i < 6; i++)
            {
                pokemon = allPokemon[ram.Next(allPokemon.Count)];
                model.Add(new PokemonViewModel(){
                    pokemon = pokemon,
                    baseStats = _dataService.GetBaseStat(pokemon.Id),
                    types = _dataService.GetPokemonTypes(pokemon.Id),
                    abilities = _dataService.GetPokemonAbilities(pokemon.Id),
                    effectiveness = _dataService.GetTypeChartPokemon(pokemon.Id)
                });
            }

            return View(model);
        }

        [Route("{Name}")]
        public IActionResult Pokemon(string Name)
        {
            if (!Name.Contains("nidoran"))
            {
                Name = Name.Replace('_', ' ');
            }
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            if (Name != textInfo.ToTitleCase(Name))
            {
                Name = textInfo.ToTitleCase(Name);
            }

            Pokemon pokemon = _dataService.GetPokemon(Name);
            List<Pokemon> altForms = _dataService.GetAltForms(pokemon);
            
            List<PokemonViewModel> model = new List<PokemonViewModel>();
            model.Add(new PokemonViewModel(){
                pokemon = pokemon,
                baseStats = _dataService.GetBaseStat(pokemon.Id),
                evYields = _dataService.GetEVYield(pokemon.Id),
                types = _dataService.GetPokemonTypes(pokemon.Id),
                abilities = _dataService.GetPokemonAbilities(pokemon.Id),
                eggGroups = _dataService.GetPokemonEggGroups(pokemon.Id),
                preEvolution = _dataService.GetPreEvolution(pokemon.Id),
                evolutions = _dataService.GetPokemonEvolutions(pokemon.Id),
                effectiveness = _dataService.GetTypeChartPokemon(pokemon.Id)
            });

            foreach(var p in altForms)
            {
                model.Add(new PokemonViewModel(){
                    pokemon = p,
                    baseStats = _dataService.GetBaseStat(p.Id),
                    evYields = _dataService.GetEVYield(p.Id),
                    types = _dataService.GetPokemonTypes(p.Id),
                    abilities = _dataService.GetPokemonAbilities(p.Id),
                    eggGroups = _dataService.GetPokemonEggGroups(p.Id),
                    preEvolution = _dataService.GetPreEvolution(p.Id),
                    evolutions = _dataService.GetPokemonEvolutions(p.Id),
                    effectiveness = _dataService.GetTypeChartPokemon(p.Id)
            });
            }

            return View(model);
        }

        [Route("type_chart")]
        public IActionResult TypeChart()
        {
            List<TypeChart> model = _dataService.GetTypeChart();

            return View(model);
        }

        [Route("error")]
        public IActionResult Error()
        {
            return View();
        }
    }
}
