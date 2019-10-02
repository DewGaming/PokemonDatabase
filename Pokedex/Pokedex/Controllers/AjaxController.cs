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
    public class AjaxController : Controller
    {
        private readonly DataService _dataService;

        private readonly AppConfig _appConfig;

        public AjaxController(IOptions<AppConfig> appConfig, DataContext dataContext)
        {
            this._appConfig = appConfig.Value;
            this._dataService = new DataService(dataContext);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("get-pokemon-list")]
        public List<Pokemon> GetPokemonList()
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
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
            else
            {
                this.RedirectToAction("Error");
            }

            return null;
        }

        [AllowAnonymous]
        [Route("get-pokemon-by-generation/{generationId}")]
        public IActionResult GetPokemonByGeneration(string generationId)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                GenerationTableViewModel model = new GenerationTableViewModel()
                {
                    PokemonList = this._dataService.GetAllPokemonWithTypes().Where(x => x.Pokemon.GenerationId == generationId || x.Pokemon.GenerationId.Contains(generationId + '-')).ToList(),
                    AppConfig = _appConfig,
                };

                return this.PartialView("_FillGenerationTable", model);
            }
            else
            {
                return this.RedirectToAction("Home", "Index");
            }
        }

        [AllowAnonymous]
        [Route("get-pokemon-team")]
        public TeamGeneratorViewModel GetPokemonTeam(List<string> selectedGens, List<string> selectedLegendaries, List<string> selectedForms, string selectedEvolutions, bool onlyLegendaries, bool onlyAltForms, bool multipleMegas, bool oneAltForm, bool randomAbility)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
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
                    PokemonAbilities = new List<Ability>(),
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
                    if(!selectedLegendaries.Contains("Legendary"))
                    {
                        List<PokemonLegendaryDetail> legendaryList = this._dataService.GetAllPokemonWithLegendaryTypes().Where(x => x.LegendaryType.Type == "Legendary").ToList();

                        foreach(var l in legendaryList)
                        {
                            allPokemon.Remove(allPokemon.FirstOrDefault(x => x.Id == l.PokemonId));
                        }
                    }
                    if(!selectedLegendaries.Contains("Mythical"))
                    {
                        List<PokemonLegendaryDetail> legendaryList = this._dataService.GetAllPokemonWithLegendaryTypes().Where(x => x.LegendaryType.Type == "Mythical").ToList();

                        foreach(var l in legendaryList)
                        {
                            allPokemon.Remove(allPokemon.FirstOrDefault(x => x.Id == l.PokemonId));
                        }
                    }
                    if(!selectedLegendaries.Contains("UltraBeast"))
                    {
                        List<PokemonLegendaryDetail> legendaryList = this._dataService.GetAllPokemonWithLegendaryTypes().Where(x => x.LegendaryType.Type == "Ultra Beast").ToList();

                        foreach(var l in legendaryList)
                        {
                            allPokemon.Remove(allPokemon.FirstOrDefault(x => x.Id == l.PokemonId));
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
                    List<Pokemon> megaList = new List<Pokemon>();
                    List<PokemonFormDetail> altFormList = this._dataService.GetAllAltForms();
                    foreach(var p in altFormList.Where(x => x.Form.Name == "Mega Evolution"
                                                         || x.Form.Name == "Mega X Evolution"
                                                         || x.Form.Name == "Mega Y Evolution").ToList())
                    {
                        if(allPokemon.Exists(x => x.Id == p.AltFormPokemonId))
                        {
                            megaList.Add(p.AltFormPokemon);
                        }
                    }

                    allPokemon = this.RemoveExtraPokemonForms(allPokemon);
                    for (var i = 0; i < 6; i++)
                    {
                        if (pokemonList.Count() >= allPokemon.Count())
                        {
                            break;
                        }

                        pokemon = allPokemon[rnd.Next(allPokemon.Count)];
                        while (pokemonList.Contains(pokemon))
                        {
                            pokemon = allPokemon[rnd.Next(allPokemon.Count)];
                        }

                        if (megaList.Exists(x => x.Id == pokemon.Id) && !multipleMegas)
                        {
                            foreach(var p in megaList)
                            {
                                if (allPokemon.Exists(x => x.Id == p.Id))
                                {
                                    allPokemon.Remove(allPokemon.Find(x => x.Id == p.Id));
                                }
                            }
                        }

                        if (oneAltForm && pokemon.Id.Contains('-'))
                        {
                            List<Pokemon> altForms = this._dataService.GetAltForms(pokemon.Id.Substring(0, pokemon.Id.IndexOf('-')));

                            altForms.Remove(altForms.Find(x => x.Id == pokemon.Id));

                            foreach(var p in altForms)
                            {
                                if (allPokemon.Exists(x => x.Id == p.Id))
                                {
                                    allPokemon.Remove(allPokemon.Find(x => x.Id == p.Id));
                                }
                            }
                        }

                        pokemonList.Add(pokemon);
                    }
                }

                model.AllPokemonChangedNames = pokemonList;
                foreach(var p in pokemonList)
                {
                    model.AllPokemonOriginalNames.Add(this._dataService.GetPokemonById(p.Id));
                }

                model.PokemonURLs = new List<string>();
                foreach(var p in model.AllPokemonOriginalNames)
                {
                    model.PokemonURLs.Add(this.Url.Action("Pokemon", "Home", new { name = p.Name.Replace(": ", "_").Replace(' ', '_').ToLower() }));
                }

                foreach(var p in model.AllPokemonOriginalNames)
                {
                    pokemon = allPokemon[rnd.Next(allPokemon.Count)];
                    List<Ability> abilities = new List<Ability>();
                    PokemonAbilityDetail pokemonAbilities = this._dataService.GetPokemonWithAbilities(p.Id);
                    abilities.Add(pokemonAbilities.PrimaryAbility);
                    if(pokemonAbilities.SecondaryAbility != null)
                    {
                        abilities.Add(pokemonAbilities.SecondaryAbility);
                    }
                    if(pokemonAbilities.HiddenAbility != null)
                    {
                        abilities.Add(pokemonAbilities.HiddenAbility);
                    }
                    model.PokemonAbilities.Add(abilities[rnd.Next(abilities.Count)]);
                }

                return model;
            }
            else
            {
                this.RedirectToAction("Home", "Index");
            }

            return null;
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
        [Route("get-typing-effectiveness")]
        public TypeEffectivenessViewModel GetTypingTypeChart(int primaryTypeId, int secondaryTypeId)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return this._dataService.GetTypeChartTyping(primaryTypeId, secondaryTypeId);
            }
            else
            {
                this.RedirectToAction("Home", "Index");
            }

            return null;
        }

        [AllowAnonymous]
        [Route("get-pokemon-by-typing")]
        public TypingEvaluatorViewModel GetPokemon(int primaryTypeId, int secondaryTypeId)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
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
            else
            {
                this.RedirectToAction("Home", "Index");
            }

            return null;
        }
    }
}
