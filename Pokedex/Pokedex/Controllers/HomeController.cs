using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Pokedex.DataAccess.Models;

using Pokedex.Models;

namespace Pokedex.Controllers
{
    [Authorize]
    [Route("")]
    public class HomeController : Controller
    {
        private readonly DataService _dataService;

        private readonly AppConfig _appConfig;

        public HomeController(IOptions<AppConfig> appConfig, DataContext dataContext)
        {
            this._appConfig = appConfig.Value;
            this._dataService = new DataService(dataContext);
        }

        [AllowAnonymous]
        [Route("")]
        public IActionResult Index()
        {
            return this.View();
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("search")]
        public IActionResult Search(string search)
        {
            return this.RedirectToAction("SearchRedirect", "Home", new { search = search } );
        }

        [AllowAnonymous]
        [Route("search/{search}")]
        public IActionResult SearchRedirect(string search)
        {
            this.ViewData["Search"] = search;

            if (!string.IsNullOrEmpty(search))
            {
                if (search.Contains("type null", StringComparison.OrdinalIgnoreCase))
                {
                    search = "Type: Null";
                }
                else if (search.Contains("nidoran", StringComparison.OrdinalIgnoreCase))
                {
                    search = search.Replace(' ', '_');
                }

                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                search = textInfo.ToTitleCase(search);

                if (search.Contains("-O") && search.Substring(search.Length - 2, 2) == "-O")
                {
                    search = search.Remove(search.Length - 2, 2) + "-o";
                }

                List<PokemonTypeDetail> model = this._dataService.GetAllPokemonWithTypes();
                model = model
                    .Where(p => p.Pokemon.Name.ToLower().Contains(search.ToLower()))
                    .ToList();

                Pokemon pokemonSearched = this._dataService.GetPokemon(search);

                if (model.Count() == 1 || pokemonSearched != null)
                {
                    if (pokemonSearched == null)
                    {
                        pokemonSearched = model[0].Pokemon;
                    }
                    return this.RedirectToAction("Pokemon", "Home", new { Name = pokemonSearched.Name.Replace(": ", "_").Replace(' ', '_').ToLower() });
                }
                else if (model.Count() == 0)
                {
                    return this.RedirectToAction("AllPokemon", "Home");
                }
                else
                {
                    AllPokemonTypeViewModel viewModel = new AllPokemonTypeViewModel(){
                        AllPokemon = model,
                        AppConfig = this._appConfig,
                    };

                    return this.View("Search", viewModel);
                }
            }

            return this.RedirectToAction("Error");
        }

        [AllowAnonymous]
        [Route("pokemon")]
        public IActionResult AllPokemon()
        {
            List<Pokemon> pokemonList = this._dataService.GetAllPokemon();
            List<string> model = pokemonList.Select(x => x.Generation.Id).Distinct().Where(x => x.IndexOf('-') < 0).OrderBy(x => x).ToList();

            return this.View(model);
        }

        [AllowAnonymous]
        [Route("team_generator")]
        public IActionResult TeamGenerator()
        {
            List<Pokemon> allPokemon = this._dataService.GetAllPokemon().Where(x => x.Id.IndexOf('-') == -1).ToList();
            List<Generation> generations = this._dataService.GetGenerations().Where(x => !x.Id.Contains('-')).ToList();
            List<Generation> model = new List<Generation>();

            foreach(var gen in generations)
            {
                if(allPokemon.Where(x => x.GenerationId == gen.Id).ToList().Count() != 0)
                {
                    model.Add(gen);
                }
            }

            return this.View(model);
        }

        [AllowAnonymous]
        [Route("typing")]
        public IActionResult Typing()
        {
            List<Pokedex.DataAccess.Models.Type> model = this._dataService.GetTypes();

            return View(model);
        }

        [AllowAnonymous]
        [Route("pokemon/{Name}")]
        public IActionResult Pokemon(string name)
        {
            if (name.Contains("type_null"))
            {
                name = "Type: Null";
            }
            else if (!name.Contains("nidoran"))
            {
                name = name.Replace('_', ' ');
            }

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            name = textInfo.ToTitleCase(name);

            if (name.Substring(name.Length - 2, 2) == "-O")
            {
                name = name.Remove(name.Length - 2, 2) + "-o";
            }

            Pokemon pokemon = this._dataService.GetPokemon(name);
            BaseStat baseStat = this._dataService.GetBaseStat(pokemon.Id);
            EVYield evYield = this._dataService.GetEVYield(pokemon.Id);
            List<Pokemon> altForms = this._dataService.GetAltForms(pokemon.Id);
            PokemonTypeDetail pokemonTypes = this._dataService.GetPokemonWithTypes(pokemon.Id);
            PokemonAbilityDetail pokemonAbilities = this._dataService.GetPokemonWithAbilities(pokemon.Id);
            PokemonEggGroupDetail pokemonEggGroups = this._dataService.GetPokemonWithEggGroups(pokemon.Id);

            List<PokemonViewModel> model = new List<PokemonViewModel>();
            model.Add(new PokemonViewModel()
            {
                Pokemon = pokemon,
                BaseStats = baseStat,
                EVYields = evYield,
                PrimaryType = pokemonTypes.PrimaryType,
                SecondaryType = pokemonTypes.SecondaryType,
                PrimaryAbility = pokemonAbilities.PrimaryAbility,
                SecondaryAbility = pokemonAbilities.SecondaryAbility,
                HiddenAbility = pokemonAbilities.HiddenAbility,
                PrimaryEggGroup = pokemonEggGroups.PrimaryEggGroup,
                SecondaryEggGroup = pokemonEggGroups.SecondaryEggGroup,
                PreEvolution = this._dataService.GetPreEvolution(pokemon.Id),
                Evolutions = this._dataService.GetPokemonEvolutions(pokemon.Id),
                Effectiveness = this._dataService.GetTypeChartPokemon(pokemon.Id),
                AppConfig = this._appConfig,
            });

            foreach (var p in altForms)
            {
                if (p.IsComplete)
                {
                    baseStat = this._dataService.GetBaseStat(p.Id);
                    evYield = this._dataService.GetEVYield(p.Id);
                    pokemonTypes = this._dataService.GetPokemonWithTypes(p.Id);
                    pokemonAbilities = this._dataService.GetPokemonWithAbilities(p.Id);
                    pokemonEggGroups = this._dataService.GetPokemonWithEggGroups(p.Id);
                    var pokemonModel = new PokemonViewModel()
                    {
                        Pokemon = p,
                        BaseStats = baseStat,
                        EVYields = evYield,
                        PrimaryType = pokemonTypes.PrimaryType,
                        SecondaryType = pokemonTypes.SecondaryType,
                        PrimaryAbility = pokemonAbilities.PrimaryAbility,
                        SecondaryAbility = pokemonAbilities.SecondaryAbility,
                        HiddenAbility = pokemonAbilities.HiddenAbility,
                        PrimaryEggGroup = pokemonEggGroups.PrimaryEggGroup,
                        SecondaryEggGroup = pokemonEggGroups.SecondaryEggGroup,
                        PreEvolution = this._dataService.GetPreEvolution(p.Id),
                        Evolutions = this._dataService.GetPokemonEvolutions(p.Id),
                        Effectiveness = this._dataService.GetTypeChartPokemon(p.Id),
                        AppConfig = this._appConfig,
                    };
                    pokemonModel.Pokemon.Name += " (" + this._dataService.GetPokemonFormName(pokemonModel.Pokemon.Id) + ")";

                    model.Add(pokemonModel);
                }
            }

            return this.View(model);
        }

        [AllowAnonymous]
        [Route("type_chart")]
        public IActionResult TypeChart()
        {
            TypeChartViewModel model = new TypeChartViewModel()
            {
                TypeChart = this._dataService.GetTypeChart(),
                Types = this._dataService.GetTypeChartTypes(),
            };

            return this.View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("suggestion")]
        public IActionResult Suggestion()
        {
            Suggestion model = new Suggestion()
            {
                Commentor = "Anonymous",
            };
            return this.View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("suggestion")]
        public IActionResult Suggestion(Suggestion suggestion)
        {
            if (!this.ModelState.IsValid)
            {
                Suggestion model = new Suggestion()
                {
                    Commentor = "Anonymous",
                };
                return this.View(model);
            }

            if (User.Identity.Name != null)
            {
                suggestion.Commentor = User.Identity.Name;
            }

            return this.RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [Route("error")]
        public IActionResult Error()
        {
            return this.View();
        }
    }
}
