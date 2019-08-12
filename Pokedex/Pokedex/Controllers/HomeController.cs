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

                Pokemon pokemonSearched = this._dataService.GetPokemon(search);

                if (model.Count() == 1 || pokemonSearched != null)
                {
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
        public TeamGeneratorViewModel GetPokemonTeam(List<string> selectedGens, List<string> selectedLegendaries, List<string> selectedForms, string selectedEvolutions, bool onlyLegendaries, bool onlyAltForms)
        {
            List<Generation> unselectedGens = this._dataService.GetGenerations().Where(x => !x.Id.Contains('-')).ToList();
            foreach(var item in selectedGens)
            {
                unselectedGens.Remove(unselectedGens.Find(x => x.Id == item));
            }

            Pokemon pokemon;
            TeamGeneratorViewModel model = new TeamGeneratorViewModel(){
                AllPokemonChangedNames = new List<Pokemon>(),
                AllPokemonOriginalNames = new List<Pokemon>(),
            };
            List<Pokemon> pokemonList = new List<Pokemon>();
            List<Pokemon> allPokemon = this._dataService.GetAllPokemonWithoutForms();
            List<Evolution> allEvolutions = this._dataService.GetEvolutions();
            Random rnd = new Random();

            foreach(var gen in unselectedGens)
            {
                allPokemon = allPokemon.Except(allPokemon.Where(x => (x.GenerationId == gen.Id) || (x.GenerationId.IndexOf('-') > -1 && x.GenerationId.Substring(0, x.GenerationId.IndexOf('-')) == gen.Id)).ToList()).ToList();
            }

            if (selectedEvolutions == "stage1Pokemon")
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
                    if (!allEvolutions.Exists(x => x.PreevolutionPokemonId == p.Id))
                    {
                        newPokemon.Add(p);
                    }
                }

                allPokemon = newPokemon;
            }

            if(selectedLegendaries.Count() == 0)
            {
                List<PokemonLegendaryDetail> legendaryList = this._dataService.GetAllPokemonWithLegendaryTypes();

                foreach(var p in legendaryList)
                {
                    allPokemon.Remove(allPokemon.FirstOrDefault(x => x.Id == p.PokemonId));
                }
            }
            else
            {
                if(!selectedLegendaries.Contains("Legendaries"))
                {
                    List<PokemonLegendaryDetail> legendaryList = this._dataService.GetAllPokemonWithLegendaryTypes().Where(x => x.LegendaryType.Type == "Legendary").ToList();

                    foreach(var l in legendaryList)
                    {
                        allPokemon.Remove(l.Pokemon);
                    }
                }
            }

            if(onlyLegendaries)
            {
                List<Pokemon> legendaryList = new List<Pokemon>();
                List<PokemonLegendaryDetail> allLegendaries = this._dataService.GetAllPokemonWithLegendaryTypes();

                foreach(var p in allLegendaries)
                {
                    if(allPokemon.Exists(x => x.Id == p.PokemonId))
                    {
                        legendaryList.Add(p.Pokemon);
                    }
                }

                allPokemon = legendaryList;
            }

            if(selectedForms.Count() != 0)
            {
                List<Pokemon> altForms = new List<Pokemon>();

                if (selectedForms.Contains("Mega"))
                {  
                    List<PokemonFormDetail> pokemonFormList = this._dataService.GetAllAltFormsOnlyComplete().Where(x => x.Form.Name == "Mega Evolution" || x.Form.Name == "Mega X Evolution" || x.Form.Name == "Mega Y Evolution").ToList();

                    List<PokemonFormDetail> filteredFormList = new List<PokemonFormDetail>();

                    foreach(var p in allPokemon)
                    {
                        List<PokemonFormDetail> altForm = pokemonFormList.Where(x => x.OriginalPokemonId == p.Id).ToList();
                        foreach(var a in altForm)
                        {
                            a.AltFormPokemon.Name += " (" + a.Form.Name + ")";
                        }
                        
                        if(altForm.Count() > 0)
                        {
                            filteredFormList.AddRange(altForm);
                        }
                    }

                    if (filteredFormList.Count() > 0)
                    {
                        foreach(var p in filteredFormList)
                        {
                            altForms.Add(p.AltFormPokemon);
                        }
                    }
                }

                if (selectedForms.Contains("Alolan"))
                {
                    List<PokemonFormDetail> pokemonFormList = this._dataService.GetAllAltFormsOnlyComplete().Where(x => x.Form.Name == "Alolan").ToList();

                    List<PokemonFormDetail> filteredFormList = new List<PokemonFormDetail>();

                    foreach(var p in allPokemon)
                    {
                        List<PokemonFormDetail> altForm = pokemonFormList.Where(x => x.OriginalPokemonId == p.Id).ToList();
                        foreach(var a in altForm)
                        {
                            a.AltFormPokemon.Name += " (" + a.Form.Name + ")";
                        }
                        
                        if(altForm.Count() > 0)
                        {
                            filteredFormList.AddRange(altForm);
                        }
                    }

                    if (filteredFormList.Count() > 0)
                    {
                        foreach(var p in filteredFormList)
                        {
                            altForms.Add(p.AltFormPokemon);
                        }
                    }
                }

                if (selectedForms.Contains("Galarian"))
                {
                    List<PokemonFormDetail> pokemonFormList = this._dataService.GetAllAltFormsOnlyComplete().Where(x => x.Form.Name == "Galarian").ToList();

                    List<PokemonFormDetail> filteredFormList = new List<PokemonFormDetail>();

                    foreach(var p in allPokemon)
                    {
                        List<PokemonFormDetail> altForm = pokemonFormList.Where(x => x.OriginalPokemonId == p.Id).ToList();
                        foreach(var a in altForm)
                        {
                            a.AltFormPokemon.Name += " (" + a.Form.Name + ")";
                        }

                        if(altForm.Count() > 0)
                        {
                            filteredFormList.AddRange(altForm);
                        }
                    }

                    if (filteredFormList.Count() > 0)
                    {
                        foreach(var p in filteredFormList)
                        {
                            altForms.Add(p.AltFormPokemon);
                        }
                    }
                }

                if (selectedForms.Contains("Other"))
                {
                    List<PokemonFormDetail> pokemonFormList = this._dataService.GetAllAltFormsOnlyComplete();

                    List<Form> formsToRemove = this.GatherRemovableForms();

                    foreach(var f in formsToRemove)
                    {
                        pokemonFormList = pokemonFormList.Where(x => x.Form.Name != f.Name).ToList();
                    }

                    List<PokemonFormDetail> filteredFormList = new List<PokemonFormDetail>();

                    foreach(var p in allPokemon)
                    {
                        List<PokemonFormDetail> altForm = pokemonFormList.Where(x => x.OriginalPokemonId == p.Id).ToList();
                        foreach(var a in altForm)
                        {
                            a.AltFormPokemon.Name += " (" + a.Form.Name + ")";
                        }

                        if(altForm.Count() > 0)
                        {
                            filteredFormList.AddRange(altForm);
                        }
                    }

                    if (filteredFormList.Count() > 0)
                    {
                        foreach(var p in filteredFormList)
                        {
                            altForms.Add(p.AltFormPokemon);
                        }
                    }
                }

                allPokemon.AddRange(altForms);
            }

            if (onlyAltForms)
            {
                allPokemon = allPokemon.Where(x => x.Id.Contains('-')).ToList();
            }

            if(allPokemon.Count() > 0)
            {
                allPokemon = this.RemoveExtraPokemonForms(allPokemon);
                for (var i = 0; i < 6; i++)
                {
                    if (pokemonList.Count() == allPokemon.Count())
                    {
                        break;
                    }

                    pokemon = allPokemon[rnd.Next(allPokemon.Count)];
                    while (pokemonList.Contains(pokemon))
                    {
                        pokemon = allPokemon[rnd.Next(allPokemon.Count)];
                    }

                    pokemonList.Add(pokemon);
                }
            }

            model.AllPokemonChangedNames = pokemonList;
            foreach(var p in pokemonList)
            {
                model.AllPokemonOriginalNames.Add(this._dataService.GetPokemonById(p.Id));
            }

            return model;
        }

        [AllowAnonymous]
        [Route("gather-removable-forms")]
        public List<Form> GatherRemovableForms()
        {
            List<Form> forms = new List<Form>();

            forms.Add(this._dataService.GetFormByName("Mega Evolution"));
            forms.Add(this._dataService.GetFormByName("Mega X Evolution"));
            forms.Add(this._dataService.GetFormByName("Mega Y Evolution"));
            forms.Add(this._dataService.GetFormByName("Alolan"));
            forms.Add(this._dataService.GetFormByName("Galarian"));
            forms.Add(this._dataService.GetFormByName("Rainy"));
            forms.Add(this._dataService.GetFormByName("Snowy"));
            forms.Add(this._dataService.GetFormByName("Sunny"));
            forms.Add(this._dataService.GetFormByName("Zen Mode"));
            forms.Add(this._dataService.GetFormByName("Pirouette Forme"));
            forms.Add(this._dataService.GetFormByName("Blue-Striped Form"));
            forms.Add(this._dataService.GetFormByName("Female"));
            forms.Add(this._dataService.GetFormByName("Blade Forme"));
            forms.Add(this._dataService.GetFormByName("Event"));
            forms.Add(this._dataService.GetFormByName("School Form"));
            forms.Add(this._dataService.GetFormByName("Core"));

            return forms;
        }

        [AllowAnonymous]
        [Route("remove-extra-pokemon-forms")]
        public List<Pokemon> RemoveExtraPokemonForms(List<Pokemon> pokemonList)
        {
            Random rnd = new Random();
            List<Pokemon> pumpkabooCount = pokemonList.Where(x => x.Id.StartsWith("710")).ToList();
            while(pumpkabooCount.Count() > 1)
            {
                pokemonList.Remove(pumpkabooCount[rnd.Next(pumpkabooCount.Count)]);
                pumpkabooCount = pokemonList.Where(x => x.Id.StartsWith("710")).ToList();
            }

            List<Pokemon> gourgeistCount = pokemonList.Where(x => x.Id.StartsWith("711")).ToList();
            while(gourgeistCount.Count() > 1)
            {
                pokemonList.Remove(gourgeistCount[rnd.Next(gourgeistCount.Count)]);
                gourgeistCount = pokemonList.Where(x => x.Id.StartsWith("711")).ToList();
            }

            return pokemonList;
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
        public TypingEvaluatorViewModel GetPokemon(int primaryTypeId, int secondaryTypeId)
        {
            List<PokemonTypeDetail> typingList = this._dataService.GetAllPokemonWithSpecificTypes(primaryTypeId, secondaryTypeId);
            List<Pokemon> pokemonList = new List<Pokemon>();

            foreach(var p in typingList)
            {
                if(p.PokemonId.Contains('-'))
                {
                    Pokemon pokemon = this._dataService.GetAltFormWithFormName(p.PokemonId);
                    pokemonList.Add(pokemon);
                }
                else
                {
                    pokemonList.Add(p.Pokemon);
                }
            }

            TypingEvaluatorViewModel model = new TypingEvaluatorViewModel(){
                AllPokemonWithTypes = typingList,
                AllPokemon = pokemonList,
            };

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
