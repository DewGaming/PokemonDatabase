using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Pokedex.Models;
using Pokedex.DataAccess.Models;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Pokedex.Controllers
{
    [Authorize, Route("")]
    public class HomeController : Controller
    {
        private readonly AppConfig _appConfig;

        private readonly DataService _dataService;

        public HomeController(IOptions<AppConfig> appConfig, DataContext dataContext)
        {
            _dataService = new DataService(dataContext);
            _appConfig = appConfig.Value;
        }

        [AllowAnonymous, Route("")]
        public IActionResult Index()
        {
            ViewBag.ApplicationName = _appConfig.AppName;
            ViewBag.ApplicationVersion = _appConfig.AppVersion;

            return View();
        }

        [AllowAnonymous, Route("pokemon")]
        public IActionResult AllPokemon()
        {
            List<PokemonTypeDetail> model = _dataService.GetPokemonWithTypes();

            return View(model);
        }

        [AllowAnonymous, Route("team_generator")]
        public IActionResult TeamGenerator()
        {
            Pokemon pokemon;
            List<Pokemon> model = new List<Pokemon>();
            List<Pokemon> allPokemon = _dataService.GetAllPokemon().Where(x => x.Id.IndexOf('-') == -1).ToList();
            Random rnd = new Random();

            for(var i = 0; i < 6; i++)
            {
                pokemon = allPokemon[rnd.Next(allPokemon.Count)];
                while(model.Contains(pokemon))
                {
                    pokemon = allPokemon[rnd.Next(allPokemon.Count)];
                }

                model.Add(pokemon);
            }

            return View(model);
        }

        [Authorize, Route("shiny_hunting_counter")]
        public IActionResult ShinyHuntingCounter()
        {
            List<ShinyHunt> model = _dataService.GetShinyHunter(User.Identity.Name);
            return View(model);
        }

        [HttpGet, Route("begin_shiny_hunt")]
        public IActionResult BeginShinyHunt()
        {
            BeginShinyHuntViewModel model = new BeginShinyHuntViewModel(){
                UserId = _dataService.GetUserWithUsername(User.Identity.Name).Id,
                AllShinyHuntingTechniques = _dataService.GetShinyHuntingTechniques(),
                AllPokemon = _dataService.GetAllPokemon(),
                AllGenerations = _dataService.GetGenerations()
            };

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, Route("begin_shiny_hunt")]
        public IActionResult BeginShinyHunt(ShinyHunt shinyHunt)
        {
            _dataService.AddShinyHunt(shinyHunt);

            return RedirectToAction("ShinyHuntingCounter", "Home");
        }

        [AllowAnonymous, HttpPost]
        public IActionResult Results(string searchText)
        {
            searchText = searchText.ToLower();
            if (searchText.Contains("type null"))
            {
                searchText = "Type: Null";
            }
            else if (searchText.Contains("nidoran"))
            {
                searchText = searchText.Replace(' ', '_');
            }

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            searchText = textInfo.ToTitleCase(searchText);

            if (searchText.Substring(searchText.Length - 2, 2) == "-O")
            {
                searchText = searchText.Remove(searchText.Length - 2, 2) + "-o";
            }

            if (_dataService.GetPokemon(searchText) != null)
            {
                return RedirectToAction("Pokemon", "Home", new { Name = searchText.Replace(": ", "_").Replace(' ', '_').ToLower() });
            }

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous, Route("{Name}")]
        public IActionResult Pokemon(string Name)
        {
            if (Name.Contains("type_null"))
            {
                Name = "Type: Null";
            }
            else if (!Name.Contains("nidoran"))
            {
                Name = Name.Replace('_', ' ');
            }

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            Name = textInfo.ToTitleCase(Name);

            if (Name.Substring(Name.Length - 2, 2) == "-O")
            {
                Name = Name.Remove(Name.Length - 2, 2) + "-o";
            }

            Pokemon pokemon = _dataService.GetPokemon(Name);
            List<Pokemon> altForms = _dataService.GetAltForms(pokemon);
            PokemonTypeDetail pokemonTypes = _dataService.GetPokemonWithTypes(pokemon.Id);
            PokemonAbilityDetail pokemonAbilities = _dataService.GetPokemonWithAbilities(pokemon.Id);
            PokemonEggGroupDetail pokemonEggGroups = _dataService.GetPokemonWithEggGroups(pokemon.Id);
            
            List<PokemonViewModel> model = new List<PokemonViewModel>();
            model.Add(new PokemonViewModel(){
                pokemon = pokemon,
                baseStats = _dataService.GetBaseStat(pokemon.Id),
                evYields = _dataService.GetEVYield(pokemon.Id),
                PrimaryType = pokemonTypes.PrimaryType,
                SecondaryType = pokemonTypes.SecondaryType,
                PrimaryAbility = pokemonAbilities.PrimaryAbility,
                SecondaryAbility = pokemonAbilities.SecondaryAbility,
                HiddenAbility = pokemonAbilities.HiddenAbility,
                PrimaryEggGroup = pokemonEggGroups.PrimaryEggGroup,
                SecondaryEggGroup = pokemonEggGroups.SecondaryEggGroup,
                preEvolution = _dataService.GetPreEvolution(pokemon.Id),
                evolutions = _dataService.GetPokemonEvolutions(pokemon.Id),
                effectiveness = _dataService.GetTypeChartPokemon(pokemon.Id)
            });

            foreach(var p in altForms)
            {
                pokemonTypes = _dataService.GetPokemonWithTypes(p.Id);
                pokemonAbilities = _dataService.GetPokemonWithAbilities(p.Id);
                pokemonEggGroups = _dataService.GetPokemonWithEggGroups(p.Id);
                var pokemonModel = new PokemonViewModel(){
                    pokemon = p,
                    baseStats = _dataService.GetBaseStat(p.Id),
                    evYields = _dataService.GetEVYield(p.Id),
                    PrimaryType = pokemonTypes.PrimaryType,
                    SecondaryType = pokemonTypes.SecondaryType,
                    PrimaryAbility = pokemonAbilities.PrimaryAbility,
                    SecondaryAbility = pokemonAbilities.SecondaryAbility,
                    HiddenAbility = pokemonAbilities.HiddenAbility,
                    PrimaryEggGroup = pokemonEggGroups.PrimaryEggGroup,
                    SecondaryEggGroup = pokemonEggGroups.SecondaryEggGroup,
                    preEvolution = _dataService.GetPreEvolution(p.Id),
                    evolutions = _dataService.GetPokemonEvolutions(p.Id),
                    effectiveness = _dataService.GetTypeChartPokemon(p.Id)
                };
                pokemonModel.pokemon.Name += " (" + _dataService.GetPokemonFormName(pokemonModel.pokemon.Id) + ")";

                model.Add(pokemonModel);
            }

            return View(model);
        }

        [AllowAnonymous, Route("type_chart")]
        public IActionResult TypeChart()
        {
            TypeChartViewModel model = new TypeChartViewModel(){
                typeChart = _dataService.GetTypeChart(),
                types = _dataService.GetTypeChartTypes()
            };

            return View(model);
        }

        [AllowAnonymous, Route("error")]
        public IActionResult Error()
        {
            return View();
        }
    }
}
