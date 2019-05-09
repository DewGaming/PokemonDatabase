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
        public IActionResult Index(string search)
        {
            ViewData["Search"] = search;

            if (!String.IsNullOrEmpty(search))
            {
                if (search.Contains("type null"))
                {
                    search = "Type: Null";
                }
                else if (search.Contains("nidoran"))
                {
                    search = search.Replace(' ', '_');
                }

                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                search = textInfo.ToTitleCase(search);

                if (search.Contains("-O") && search.Substring(search.Length - 2, 2) == "-O")
                {
                    search = search.Remove(search.Length - 2, 2) + "-o";
                }

                List<PokemonTypeDetail> model = _dataService.GetPokemonWithTypes();
                model = model
                    .Where(p => p.Pokemon.Name.ToLower().Contains(search.ToLower()))
                    .ToList();

                if(model.Count() == 1 && _dataService.GetPokemon(model[0].Pokemon.Name) != null)
                {
                    return RedirectToAction("Pokemon", "Home", new { Name = model[0].Pokemon.Name.Replace(": ", "_").Replace(' ', '_').ToLower() });
                }
                else
                {
                    return View("AllPokemon", model);
                }
            }

            return View();
        }

        [HttpPost, Route("add-hunt-attempt/{huntId:int}")]
        public int AddShinyCounter(int huntId)
        {
            ShinyHunt hunt = _dataService.GetShinyHunt(huntId);
            hunt.ShinyAttemptCount++;
            _dataService.UpdateShinyHunt(hunt);
            return hunt.ShinyAttemptCount;
        }

        [HttpPost, Route("subtract-hunt-attempt/{huntId:int}")]
        public int SubtractShinyCounter(int huntId)
        {
            ShinyHunt hunt = _dataService.GetShinyHunt(huntId);
            if(hunt.ShinyAttemptCount > 0)
            {
                hunt.ShinyAttemptCount--;
            }
            else
            {
                hunt.ShinyAttemptCount = 0;
            }
            _dataService.UpdateShinyHunt(hunt);
            return hunt.ShinyAttemptCount;
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

        [Route("shiny_hunt/{id:int}")]
        public IActionResult ContinueHunt(int id)
        {
            ShinyHunt model = _dataService.GetShinyHunt(id);

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
            List<Generation> generations = _dataService.GetGenerations().OrderBy(p => p.Id).ToList();
            if(generations.IndexOf(_dataService.GetGeneration(shinyHunt.GenerationId)) < generations.IndexOf(_dataService.GetGenerationByPokemon(shinyHunt.PokemonId)))
            {
                BeginShinyHuntViewModel model = new BeginShinyHuntViewModel(){
                    UserId = _dataService.GetUserWithUsername(User.Identity.Name).Id,
                    AllShinyHuntingTechniques = _dataService.GetShinyHuntingTechniques(),
                    AllPokemon = _dataService.GetAllPokemon(),
                    AllGenerations = _dataService.GetGenerations()
                };

                ModelState.AddModelError("GenerationId", "Pokemon does not exist in this generation");

                return View(model);
            }

            _dataService.AddShinyHunt(shinyHunt);

            return RedirectToAction("ShinyHuntingCounter", "Home");
        }

        [HttpGet, Route("end_shiny_hunt/{id:int}")]
        public IActionResult EndShinyHunt(int id)
        {
            EndShinyHuntViewModel model = new EndShinyHuntViewModel(){
                shinyHuntId = id
            };

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, Route("end_shiny_hunt/{id:int}")]
        public IActionResult EndShinyHunt(EndShinyHuntViewModel endShinyHuntViewModel)
        {
            ShinyHunt shinyHunt = _dataService.GetShinyHunt(endShinyHuntViewModel.shinyHuntId);
            shinyHunt.HuntComplete = true;
            shinyHunt.IsPokemonCaught = endShinyHuntViewModel.HuntSuccessful;

            _dataService.UpdateShinyHunt(shinyHunt);

            return RedirectToAction("ShinyHuntingCounter", "Home");
        }

        [Route("remove_hunt/{id:int}")]
        public IActionResult RemoveHunt(int id)
        {
            ShinyHunt shinyHunt = _dataService.GetShinyHunt(id);

            if(shinyHunt != null)
            {
                _dataService.ArchiveShinyHunt(shinyHunt.Id);
            }

            return RedirectToAction("ShinyHuntingCounter");
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

        [HttpPost, Route("subtract-attempt-count/{id:int}")]
        public int SubtractAttemptCount(int id)
        {
            ShinyHunt shinyHunt = _dataService.GetShinyHunt(id);
            if (shinyHunt.ShinyAttemptCount == 0)
            {
                return 0;
            }
            else
            {
                shinyHunt.ShinyAttemptCount -= 1;
                _dataService.UpdateShinyHunt(shinyHunt);
                return shinyHunt.ShinyAttemptCount;
            }
        }

        [AllowAnonymous, Route("error")]
        public IActionResult Error()
        {
            return View();
        }
    }
}
