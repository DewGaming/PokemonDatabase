using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public HomeController(DataContext dataContext)
        {
            this._dataService = new DataService(dataContext);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("get-pokemon-list")]
        public List<Pokemon> GetPokemonList()
        {
            List<Pokemon> pokemonList = this._dataService.GetAllPokemonWithoutForms();

            foreach(var pokemon in pokemonList)
            {
                if (pokemon.Name.Contains("type null"))
                {
                    pokemon.Name = "Type: Null";
                }
                
                pokemon.Name = pokemon.Name.Replace('_', ' ');

                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                pokemon.Name = textInfo.ToTitleCase(pokemon.Name);

                if (pokemon.Name.Contains("-O") && pokemon.Name.Substring(pokemon.Name.Length - 2, 2) == "-O")
                {
                    pokemon.Name = pokemon.Name.Remove(pokemon.Name.Length - 2, 2) + "-o";
                }
            }

            return pokemonList;
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
        public IActionResult Search(string search, bool slowConnection)
        {
            TempData["SlowConnection"] = slowConnection;

            if(slowConnection)
            {
                return this.RedirectToAction("SearchRedirect", "Home", new { search = search, slowConnection = slowConnection } );
            }
            else
            {
                return this.RedirectToAction("SearchRedirect", "Home", new { search = search } );
            }
        }

        [AllowAnonymous]
        [Route("search/{search}")]
        public IActionResult SearchRedirect(string search, bool slowConnection)
        {
            this.ViewData["Search"] = search;

            if (!string.IsNullOrEmpty(search))
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

                List<PokemonTypeDetail> model = this._dataService.GetAllPokemonWithTypes();
                model = model
                    .Where(p => p.Pokemon.Name.ToLower().Contains(search.ToLower()))
                    .ToList();

                if (model.Count() == 1 || this._dataService.GetPokemon(search) != null)
                {
                    return this.RedirectToAction("Pokemon", "Home", new { Name = model[0].Pokemon.Name.Replace(": ", "_").Replace(' ', '_').ToLower() });
                }
                else if (model.Count() == 0)
                {
                    return this.RedirectToAction("AllPokemon", "Home");
                }
                else
                {
                    AllPokemonTypeViewModel viewModel = new AllPokemonTypeViewModel(){
                        AllPokemon = model,
                        SlowConnection = slowConnection
                    };

                    return this.View("AllPokemon", viewModel);
                }
            }

            return this.RedirectToAction("Error");
        }

        [AllowAnonymous]
        [Route("pokemon")]
        public IActionResult AllPokemon(bool slowConnection)
        {
            List<PokemonTypeDetail> pokemonList = this._dataService.GetAllPokemonWithTypes();

            AllPokemonTypeViewModel model = new AllPokemonTypeViewModel(){
                AllPokemon = pokemonList,
                SlowConnection = slowConnection
            };

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
        [Route("get-pokemon-team")]
        public List<Pokemon> GetPokemonTeam(List<string> selectedGens, bool useAlternateForms, string selectedEvolutions)
        {
            List<Generation> unselectedGens = this._dataService.GetGenerations().Where(x => !x.Id.Contains('-')).ToList();
            foreach(var item in selectedGens)
            {
                unselectedGens.Remove(unselectedGens.Find(x => x.Id == item));
            }

            Pokemon pokemon;
            List<Pokemon> model = new List<Pokemon>();
            List<Pokemon> allPokemon = this._dataService.GetAllPokemonWithoutForms();
            List<PokemonFormDetail> allAltForms = this._dataService.GetAllAltForms();
            List<Evolution> allEvolutions = this._dataService.GetEvolutions();
            Random rnd = new Random();

            foreach(var gen in unselectedGens)
            {
                allPokemon = allPokemon.Except(allPokemon.Where(x => (x.GenerationId == gen.Id) || (x.GenerationId.IndexOf('-') > -1 && x.GenerationId.Substring(0, x.GenerationId.IndexOf('-')) == gen.Id)).ToList()).ToList();
            }

            if (useAlternateForms && allPokemon.Count() == 0)
            {
                allPokemon = this._dataService.GetAllPokemon().Where(x => x.Id.IndexOf('-') != -1).ToList();
            }

            if (selectedEvolutions == "noEvolutions")
            {
                List<Pokemon> newPokemon = new List<Pokemon>();
                foreach(var p in allPokemon)
                {
                    if (!allEvolutions.Exists(x => x.EvolutionPokemonId == p.Id || x.PreevolutionPokemonId == p.Id))
                    {
                        newPokemon.Add(p);
                    }
                }

                allPokemon = newPokemon;
            }
            else if (selectedEvolutions == "stage1Pokemon")
            {
                List<Pokemon> newPokemon = new List<Pokemon>();
                foreach(var p in allPokemon)
                {
                    if (allEvolutions.Exists(x => x.PreevolutionPokemonId == p.Id) && !allEvolutions.Exists(x => x.EvolutionPokemonId == p.Id))
                    {
                        newPokemon.Add(p);
                    }
                }

                allPokemon = newPokemon;
            }
            else if (selectedEvolutions == "middleEvolution")
            {
                List<Pokemon> newPokemon = new List<Pokemon>();
                foreach(var p in allPokemon)
                {
                    if (allEvolutions.Exists(x => x.PreevolutionPokemonId == p.Id) && allEvolutions.Exists(x => x.EvolutionPokemonId == p.Id))
                    {
                        newPokemon.Add(p);
                    }
                }

                allPokemon = newPokemon;
            }
            else if (selectedEvolutions == "onlyFullyEvolved")
            {
                List<Pokemon> newPokemon = new List<Pokemon>();
                foreach(var p in allPokemon)
                {
                    if (!allEvolutions.Exists(x => x.PreevolutionPokemonId == p.Id) && allEvolutions.Exists(x => x.EvolutionPokemonId == p.Id))
                    {
                        newPokemon.Add(p);
                    }
                }

                allPokemon = newPokemon;
            }

            if (useAlternateForms)
            {
                List<Pokemon> altFormsList = new List<Pokemon>();
                foreach(var p in allPokemon)
                {
                    if (allAltForms.Exists(x => x.OriginalPokemonId == p.Id))
                    {
                        altFormsList.AddRange(this._dataService.GetAltForms(p));
                    }
                }

                allPokemon.AddRange(altFormsList);
            }

            if(allPokemon.Count() > 0)
            {
                for (var i = 0; i < 6; i++)
                {
                    pokemon = allPokemon[rnd.Next(allPokemon.Count)];
                    while (model.Contains(pokemon))
                    {
                        pokemon = allPokemon[rnd.Next(allPokemon.Count)];
                    }

                    model.Add(pokemon);
                }
            }

            return model;
        }

        [AllowAnonymous]
        [Route("{Name}")]
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
            List<Pokemon> altForms = this._dataService.GetAltForms(pokemon);
            PokemonTypeDetail pokemonTypes = this._dataService.GetPokemonWithTypes(pokemon.Id);
            PokemonAbilityDetail pokemonAbilities = this._dataService.GetPokemonWithAbilities(pokemon.Id);
            PokemonEggGroupDetail pokemonEggGroups = this._dataService.GetPokemonWithEggGroups(pokemon.Id);

            List<PokemonViewModel> model = new List<PokemonViewModel>();
            model.Add(new PokemonViewModel()
            {
                Pokemon = pokemon,
                BaseStats = this._dataService.GetBaseStat(pokemon.Id),
                EVYields = this._dataService.GetEVYield(pokemon.Id),
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
            });

            foreach (var p in altForms)
            {
                pokemonTypes = this._dataService.GetPokemonWithTypes(p.Id);
                pokemonAbilities = this._dataService.GetPokemonWithAbilities(p.Id);
                pokemonEggGroups = this._dataService.GetPokemonWithEggGroups(p.Id);
                var pokemonModel = new PokemonViewModel()
                {
                    Pokemon = p,
                    BaseStats = this._dataService.GetBaseStat(p.Id),
                    EVYields = this._dataService.GetEVYield(p.Id),
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
                };
                pokemonModel.Pokemon.Name += " (" + this._dataService.GetPokemonFormName(pokemonModel.Pokemon.Id) + ")";

                model.Add(pokemonModel);
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
        [Route("error")]
        public IActionResult Error()
        {
            return this.View();
        }
    }
}
