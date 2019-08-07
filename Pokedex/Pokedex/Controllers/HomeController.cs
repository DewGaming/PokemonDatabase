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
                        SlowConnection = slowConnection,
                        AppConfig = this._appConfig,
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
                SlowConnection = slowConnection,
                AppConfig = this._appConfig,
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
        public List<Pokemon> GetPokemonTeam(List<string> selectedGens, List<string> selectedForms, string selectedEvolutions, bool onlyAltForms)
        {
            List<Generation> unselectedGens = this._dataService.GetGenerations().Where(x => !x.Id.Contains('-')).ToList();
            foreach(var item in selectedGens)
            {
                unselectedGens.Remove(unselectedGens.Find(x => x.Id == item));
            }

            Pokemon pokemon;
            List<Pokemon> model = new List<Pokemon>();
            List<Pokemon> allPokemon = this._dataService.GetAllPokemon();
            List<Evolution> allEvolutions = this._dataService.GetEvolutions();
            Random rnd = new Random();

            foreach(var gen in unselectedGens)
            {
                allPokemon = allPokemon.Except(allPokemon.Where(x => (x.GenerationId == gen.Id) || (x.GenerationId.IndexOf('-') > -1 && x.GenerationId.Substring(0, x.GenerationId.IndexOf('-')) == gen.Id)).ToList()).ToList();
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

            if(selectedForms.Count() == 0)
            {
                List<PokemonFormDetail> pokemonFormList = this._dataService.GetPokemonFormDetails();

                foreach(var p in pokemonFormList)
                {
                    allPokemon.Remove(allPokemon.FirstOrDefault(x => x.Id == p.AltFormPokemonId));
                }
            }
            else
            {
                if (!selectedForms.Contains("Mega"))
                {
                    List<PokemonFormDetail> pokemonFormList = this._dataService.GetPokemonByFormName("Mega Evolution");
                    pokemonFormList.AddRange(this._dataService.GetPokemonByFormName("Mega X Evolution"));
                    pokemonFormList.AddRange(this._dataService.GetPokemonByFormName("Mega Y Evolution"));

                    foreach(var p in pokemonFormList)
                    {
                        allPokemon.Remove(allPokemon.FirstOrDefault(x => x.Id == p.AltFormPokemonId));
                    }
                }

                if (!selectedForms.Contains("Alolan"))
                {
                    List<PokemonFormDetail> pokemonFormList = this._dataService.GetPokemonByFormName("Alolan");

                    foreach(var p in pokemonFormList)
                    {
                        allPokemon.Remove(allPokemon.FirstOrDefault(x => x.Id == p.AltFormPokemonId));
                    }
                }

                if (!selectedForms.Contains("Other"))
                {
                    List<PokemonFormDetail> pokemonFormList = this._dataService.GetPokemonByFormName("Alolan");
                    pokemonFormList.AddRange(this._dataService.GetPokemonByFormName("Mega Evolution"));
                    pokemonFormList.AddRange(this._dataService.GetPokemonByFormName("Mega X Evolution"));
                    pokemonFormList.AddRange(this._dataService.GetPokemonByFormName("Mega Y Evolution"));

                    List<PokemonFormDetail> filteredFormList = this._dataService.GetPokemonFormDetails();

                    foreach(var p in pokemonFormList)
                    {
                        filteredFormList.Remove(filteredFormList.FirstOrDefault(x => x.Id == p.Id));
                    }

                    foreach(var p in filteredFormList)
                    {
                        allPokemon.Remove(allPokemon.FirstOrDefault(x => x.Id == p.AltFormPokemonId));
                    }
                }
            }

            if (onlyAltForms)
            {
                allPokemon = allPokemon.Where(x => x.Id.Contains('-')).ToList();
            }

            if(allPokemon.Count() > 0)
            {
                for (var i = 0; i < 6; i++)
                {
                    if (model.Count() == allPokemon.Count())
                    {
                        break;
                    }

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
        [Route("typing")]
        public IActionResult Typing()
        {
            List<Pokedex.DataAccess.Models.Type> model = this._dataService.GetTypes();

            return View(model);
        }

        [AllowAnonymous]
        [Route("get-typing-effectiveness")]
        public TypeEffectivenessViewModel GetTypingTypeChart(int primaryTypeId, int secondaryTypeId)
        {
            return this._dataService.GetTypeChartTyping(primaryTypeId, secondaryTypeId);
        }

        [AllowAnonymous]
        [Route("get-pokemon-by-typing")]
        public List<PokemonTypeDetail> GetPokemon(int primaryTypeId, int secondaryTypeId)
        {
            List<PokemonTypeDetail> pokemonList = this._dataService.GetAllPokemonWithSpecificTypes(primaryTypeId, secondaryTypeId);

            return pokemonList;
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
        [Route("error")]
        public IActionResult Error()
        {
            return this.View();
        }
    }
}
