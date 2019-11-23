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

        [Route("get-pokemon-by-generation-admin/{generationId}")]
        public IActionResult GetPokemonByGenerationAdmin(string generationId)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                AllAdminPokemonViewModel allPokemon = new AllAdminPokemonViewModel(){
                    AllAltForms = this._dataService.GetAllAltForms(),
                    AllEvolutions = this._dataService.GetEvolutions(),
                    AllTypings = this._dataService.GetAllPokemonWithTypesAndIncomplete(),
                    AllAbilities = this._dataService.GetAllPokemonWithAbilitiesAndIncomplete(),
                    AllEggGroups = this._dataService.GetAllPokemonWithEggGroupsAndIncomplete(),
                    AllBaseStats = this._dataService.GetBaseStatsWithIncomplete(),
                    AllEVYields = this._dataService.GetEVYieldsWithIncomplete(),
                    AllLegendaryDetails = this._dataService.GetAllPokemonWithLegendaryTypes(),
                };

                DropdownViewModel dropdownViewModel = new DropdownViewModel(){
                    AllPokemon = allPokemon,
                    AppConfig = this._appConfig,
                };

                AdminGenerationTableViewModel model = new AdminGenerationTableViewModel()
                {
                    PokemonList = this._dataService.GetAllPokemonWithoutFormsWithIncomplete().Where(x => x.GenerationId == generationId || x.GenerationId.Contains(generationId + '-')).ToList(),
                    DropdownViewModel = dropdownViewModel,
                    AppConfig = _appConfig,
                };

                return this.PartialView("_FillAdminGenerationTable", model);
            }
            else
            {
                return this.RedirectToAction("Index", "Home");
            }
        }

        [Route("get-incomplete-pokemon-admin/")]
        public IActionResult GetPokemonByGenerationAdmin()
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                AllAdminPokemonViewModel allPokemon = new AllAdminPokemonViewModel(){
                    AllAltForms = this._dataService.GetAllAltForms(),
                    AllEvolutions = this._dataService.GetEvolutions(),
                    AllTypings = this._dataService.GetAllPokemonWithTypesAndIncomplete(),
                    AllAbilities = this._dataService.GetAllPokemonWithAbilitiesAndIncomplete(),
                    AllEggGroups = this._dataService.GetAllPokemonWithEggGroupsAndIncomplete(),
                    AllBaseStats = this._dataService.GetBaseStatsWithIncomplete(),
                    AllEVYields = this._dataService.GetEVYieldsWithIncomplete(),
                    AllLegendaryDetails = this._dataService.GetAllPokemonWithLegendaryTypesAndIncomplete(),
                };

                DropdownViewModel dropdownViewModel = new DropdownViewModel(){
                    AllPokemon = allPokemon,
                    AppConfig = this._appConfig,
                };

                AdminGenerationTableViewModel model = new AdminGenerationTableViewModel()
                {
                    PokemonList = this._dataService.GetAllPokemonIncludeIncomplete().Where(x => !x.IsComplete).ToList(),
                    ReviewedPokemon = new List<ReviewedPokemon>(),
                    DropdownViewModel = dropdownViewModel,
                    AppConfig = _appConfig,
                };

                foreach(var r in this._dataService.GetAllReviewedPokemon())
                {
                    model.PokemonList.Remove(model.PokemonList.Find(x => x.Id == r.PokemonId));
                    model.ReviewedPokemon.Add(r);
                }

                return this.PartialView("_FillAdminGenerationTable", model);
            }
            else
            {
                return this.RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [Route("get-shiny-hunting-technique")]
        public ShinyHuntingTechnique GetShinyHuntTechnique(int id)
        {
            ShinyHuntingTechnique technique = this._dataService.GetShinyHuntingTechnique(id);
            
            return technique;
        }

        [HttpPost]
        [Route("add-hunt-attempt/{huntId:int}")]
        public int AddShinyCounter(int huntId)
        {
            ShinyHunt hunt = this._dataService.GetShinyHunt(huntId);
            hunt.ShinyAttemptCount++;
            this._dataService.UpdateShinyHunt(hunt);
            return hunt.ShinyAttemptCount;
        }

        [HttpPost]
        [Route("subtract-hunt-attempt/{huntId:int}")]
        public int SubtractShinyCounter(int huntId)
        {
            ShinyHunt hunt = this._dataService.GetShinyHunt(huntId);
            if (hunt.ShinyAttemptCount > 0)
            {
                hunt.ShinyAttemptCount--;
                this._dataService.UpdateShinyHunt(hunt);
            }
            else
            {
                hunt.ShinyAttemptCount = 0;
            }

            return hunt.ShinyAttemptCount;
        }

        [HttpPost]
        [Route("update-hunt-attempt/{huntId:int}/{attemptCount:int}")]
        public int UpdateShinyCounter(int huntId, int attemptCount)
        {
            ShinyHunt hunt = this._dataService.GetShinyHunt(huntId);
            if (attemptCount > 0)
            {
                hunt.ShinyAttemptCount = attemptCount;
                this._dataService.UpdateShinyHunt(hunt);
            }
            else
            {
                hunt.ShinyAttemptCount = 0;
                this._dataService.UpdateShinyHunt(hunt);
            }

            return hunt.ShinyAttemptCount;
        }

        [HttpPost]
        [Route("update-pokemon-list/{generationId}")]
        public UpdatePokemonListViewModel UpdatePokemonList(string generationId)
        {
            Generation gen = this._dataService.GetGeneration(generationId);
            List<PokemonGameDetail> pokemonGameDetails = this._dataService.GetPokemonGameDetailsByGeneration(generationId);
            UpdatePokemonListViewModel pokemonList = new UpdatePokemonListViewModel(){
                PokemonList = this._dataService.GetAllPokemon().Where(x => pokemonGameDetails.Any(y => y.PokemonId == x.Id) && !x.Id.Contains('-')).ToList(),
                Generation = gen,
            };
            return pokemonList;
        }

        [AllowAnonymous]
        [Route("grab-all-user-pokemon-teams")]
        public List<ExportPokemonViewModel> ExportAllUserPokemonTeams()
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<PokemonTeam> pokemonTeams = this._dataService.GetAllPokemonTeams(User.Identity.Name);
                List<PokemonTeamDetail> pokemonList;
                List<ExportPokemonViewModel> exportList = new List<ExportPokemonViewModel>();
                foreach(var team in pokemonTeams)
                {
                    pokemonList = team.GrabPokemonTeamDetails;
                    if(pokemonList.Count() > 0)
                    {
                        ExportPokemonViewModel pokemonTeam = new ExportPokemonViewModel(){
                            ExportString = "=== " + team.PokemonTeamName + " ===\n\n",
                            TeamId = team.Id,
                        };

                        for(var i = 0; i < pokemonList.Count(); i++)
                        {
                            if(i != 0)
                            {
                                pokemonTeam.ExportString += "\n\n";
                            }

                            pokemonTeam.ExportString += this.FillUserPokemonTeam(pokemonList[i]);
                        }

                        exportList.Add(pokemonTeam);
                    }
                }

                return exportList;
            }
            else
            {
                this.RedirectToAction("Home", "Index");
            }

            return null;
        }

        [AllowAnonymous]
        [Route("fill-user-pokemon-team")]
        public string FillUserPokemonTeam(PokemonTeamDetail pokemonTeamDetail)
        {
            Pokemon pokemon = this._dataService.GetPokemonById(pokemonTeamDetail.PokemonId);
            List<string> pokemonForm = new List<string>();
            string pokemonName = string.Empty;
            if(pokemon.Id.Contains('-'))
            {
                pokemonForm = this.GetUserFormDetails(pokemon.Id);
            }

            if(!string.IsNullOrEmpty(pokemonTeamDetail.Nickname))
            {
                pokemonName = pokemonTeamDetail.Nickname + " (";
            }
            
            pokemonName += pokemon.Name;
            if(pokemon.Id.Contains('-'))
            {
                pokemonName += "-" + ((pokemon.Id == "678-1") ? "F" : pokemonForm[0]);
            }

            if(!string.IsNullOrEmpty(pokemonTeamDetail.Nickname))
            {
                pokemonName += ")";
            }

            if(!string.IsNullOrEmpty(pokemonTeamDetail.Gender))
            {
                pokemonName += " (" + pokemonTeamDetail.Gender.Substring(0,1) + ")";
            }

            if(pokemonForm.Count == 2)
            {
                pokemonName += pokemonForm[1];
            }
            else if(pokemonTeamDetail.BattleItemId != null)
            {
                pokemonName += " @ " + pokemonTeamDetail.BattleItem.Name;
            }

            string pokemonTeamString = pokemonName;
            pokemonTeamString += "\nAbility: " + pokemonTeamDetail.Ability.Name;
            if(pokemonTeamDetail.IsShiny)
            {
                pokemonTeamString += "\nShiny: Yes";
            }
            
            if(pokemonTeamDetail.PokemonTeamEV != null)
            {
                pokemonTeamString += this.FillEVs(pokemonTeamDetail.PokemonTeamEV);
            }

            if(pokemonTeamDetail.Nature != null)
            {
                pokemonTeamString += "\n" + pokemonTeamDetail.Nature.Name + " Nature";
            }
            
            if(pokemonTeamDetail.PokemonTeamIV != null)
            {
                pokemonTeamString += this.FillIVs(pokemonTeamDetail.PokemonTeamIV);
            }

            return pokemonTeamString;
        }

        private string FillEVs(PokemonTeamEV evs)
        {
            string evString = "\nEVs: ";
            if(evs.EVTotal == 0)
            {
                evString += "1 HP / 1 Atk / 1 Def / 1 SpA / 1 SpD / 1 Spe";
            }
            else
            {
                if(evs.Health > 0)
                {
                    evString += evs.Health.ToString() + " HP / ";
                }

                if(evs.Attack > 0)
                {
                    evString += evs.Attack.ToString() + " Atk / ";
                }

                if(evs.Defense > 0)
                {
                    evString += evs.Defense.ToString() + " Def / ";
                }

                if(evs.SpecialAttack > 0)
                {
                    evString += evs.SpecialAttack.ToString() + " SpA / ";
                }

                if(evs.SpecialDefense > 0)
                {
                    evString += evs.SpecialDefense.ToString() + " SpD / ";
                }

                if(evs.Speed > 0)
                {
                    evString += evs.Speed.ToString() + " Spe";
                }
            }

            return evString;
        }

        private string FillIVs(PokemonTeamIV ivs)
        {
            string ivString = string.Empty;
            if(ivs.Health < 31)
            {
                ivString += ivs.Health.ToString() + " HP";
            }

            if(ivs.Attack < 31)
            {
                if(!string.IsNullOrEmpty(ivString))
                {
                    ivString += " / ";
                }
                
                ivString += ivs.Attack.ToString() + " Atk";
            }

            if(ivs.Defense < 31)
            {
                if(!string.IsNullOrEmpty(ivString))
                {
                    ivString += " / ";
                }
                
                ivString += ivs.Defense.ToString() + " Def";
            }

            if(ivs.SpecialAttack < 31)
            {
                if(!string.IsNullOrEmpty(ivString))
                {
                    ivString += " / ";
                }
                
                ivString += ivs.SpecialAttack.ToString() + " SpA";
            }

            if(ivs.SpecialDefense < 31)
            {
                if(!string.IsNullOrEmpty(ivString))
                {
                    ivString += " / ";
                }
                
                ivString += ivs.SpecialDefense.ToString() + " SpD";
            }

            if(ivs.Speed < 31)
            {
                if(!string.IsNullOrEmpty(ivString))
                {
                    ivString += " / ";
                }
                
                ivString += ivs.Speed.ToString() + " Spe";
            }

            if(!string.IsNullOrEmpty(ivString))
            {
                ivString = "\nIVs: " + ivString;
            }

            return ivString;
        }

        private List<string> GetUserFormDetails(string pokemonId)
        {
            string form = string.Empty, itemName = string.Empty;
            List<string> formDetails = new List<string>();
            PokemonFormDetail pokemonFormDetail;

             pokemonFormDetail = this._dataService.GetPokemonFormDetailByAltFormId(pokemonId);

            form += pokemonFormDetail.Form.Name.Replace(' ', '-');
            
            FormItem formItem = this._dataService.GetFormItemByPokemonId(pokemonId);
            if(formItem != null)
            {
                itemName = formItem.Name;
            }
            else if(form.Contains("Mega") && pokemonFormDetail.AltFormPokemonId != "384-1")
            {
                itemName = "[Insert Mega Stone Here]";
            }

            if(!string.IsNullOrEmpty(itemName))
            {
                itemName = " @ " + itemName;
            }

            formDetails.Add(form);
            if(!string.IsNullOrEmpty(itemName))
            {
                formDetails.Add(itemName);
            }

            return formDetails;
        }

        [AllowAnonymous]
        [Route("save-pokemon-team")]
        public string SavePokemonTeam(string pokemonTeamName, string selectedGame, List<string> pokemonIdList, List<int> abilityIdList, bool exportAbilities)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                if(this._dataService.GetUserWithUsername(this.User.Identity.Name) != null)
                {
                    if(string.IsNullOrEmpty(pokemonTeamName))
                    {
                        pokemonTeamName = "Save from Team Randomizer";
                    }

                    PokemonTeam pokemonTeam= new PokemonTeam(){
                        PokemonTeamName = pokemonTeamName,
                        UserId = this._dataService.GetUserWithUsername(this.User.Identity.Name).Id,
                    };

                    if(selectedGame != "0")
                    {
                        pokemonTeam.GenerationId = selectedGame;
                    }

                    Pokemon pokemon;
                    Ability ability;
                    PokemonTeamDetail pokemonTeamDetail;

                    for(var i = 0; i < pokemonIdList.Count; i++)
                    {
                        pokemon = this._dataService.GetPokemonById(pokemonIdList[i]);

                        if(exportAbilities)
                        {
                            ability = (pokemonIdList[i] == "800-3") ? this._dataService.GetAbility(34) : this._dataService.GetAbility(abilityIdList[i]);
                        }
                        else
                        {
                            ability = this._dataService.GetAbilitiesForPokemon(pokemon.Id)[0];
                        }

                        pokemonTeamDetail = new PokemonTeamDetail()
                        {
                            PokemonId = pokemon.Id,
                            AbilityId = ability.Id,
                        };

                        this._dataService.AddPokemonTeamDetail(pokemonTeamDetail);

                        pokemonTeam.InsertPokemon(pokemonTeamDetail);
                    }

                    this._dataService.AddPokemonTeam(pokemonTeam);

                    return "Team \"" + pokemonTeam.PokemonTeamName + "\" has been added successfully!";
                }
                else
                {
                    return "You must be logged in to save a team.";
                }
            }
            else
            {
                this.RedirectToAction("Home", "Index");
            }

            return null;
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
        [Route("get-available-pokemon-by-generation/{generationId}")]
        public IActionResult GetAvailablePokemonByGeneration(string generationId)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<PokemonGameDetail> pokemonGameDetails = this._dataService.GetPokemonGameDetailsByGeneration(generationId);
                List<Pokemon> pokemonList = this._dataService.GetAllPokemonIncludeIncomplete();
                pokemonList = pokemonList.Where(x => pokemonGameDetails.Any(y => y.PokemonId == x.Id)).ToList();
                foreach(var p in pokemonList.Where(x => x.Id.Contains('-')))
                {
                    p.Name = p.Name + " (" + this._dataService.GetFormByAltFormId(p.Id).Name + ")";
                }

                GenerationTableViewModel model = new GenerationTableViewModel()
                {
                    PokemonNoTypeList = pokemonList,
                    AppConfig = _appConfig,
                };

                return this.PartialView("_FillAvailableGenerationTable", model);
            }
            else
            {
                return this.RedirectToAction("Home", "Index");
            }
        }

        private string ExportPokemonTeam(List<string> pokemonIdList, List<string> abilityList, bool exportAbilities, string necrozmaOriginalId, string zygardeOriginalId)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                string pokemonTeam = string.Empty, pokemonName = string.Empty, pokemonForm = string.Empty;
                Pokemon pokemon;

                for(var i = 0; i < pokemonIdList.Count; i++)
                {
                    if (i != 0)
                    {
                        pokemonTeam += "\n\n";
                    }

                    pokemon = this._dataService.GetPokemonById(pokemonIdList[i]);
                    pokemonName = pokemon.Name;
                    if(pokemon.Id.Contains('-') && !(pokemonIdList[i] == "718-2" && zygardeOriginalId == "718"))
                    {
                        pokemonForm = this.GetFormDetails(pokemon.Id, necrozmaOriginalId, zygardeOriginalId);
                        pokemonName += "-" + pokemonForm;
                    }

                    pokemonTeam += pokemonName;
                    if(exportAbilities)
                    {
                        pokemonTeam += "\nAbility: " + ((pokemonIdList[i] == "800-3") ? this._dataService.GetAbility(34).Name : abilityList[i]);
                    }

                    pokemonTeam += "\nEVs: 1 HP / 1 Atk / 1 Def / 1 SpA / 1 SpD / 1 Spe";
                }

                return pokemonTeam;
            }
            else
            {
                this.RedirectToAction("Home", "Index");
            }

            return null;
        }

        private string GetFormDetails(string pokemonId, string necrozmaOriginalId, string zygardeOriginalId)
        {
            string formDetails = string.Empty, itemName = string.Empty;
            PokemonFormDetail pokemonFormDetail;

            if(pokemonId == "800-3")
            {
                pokemonFormDetail = this._dataService.GetPokemonFormDetailByAltFormId(necrozmaOriginalId);
            }
            else if (pokemonId == "718-2")
            {
                pokemonFormDetail = this._dataService.GetPokemonFormDetailByAltFormId(zygardeOriginalId);
            }
            else
            {
                pokemonFormDetail = this._dataService.GetPokemonFormDetailByAltFormId(pokemonId);
            }

            formDetails += pokemonFormDetail.Form.Name.Replace(' ', '-');
            
            FormItem formItem = this._dataService.GetFormItemByPokemonId(pokemonId);
            if(formItem != null)
            {
                itemName = formItem.Name;
            }
            else if(formDetails.Contains("Mega") && pokemonFormDetail.AltFormPokemonId != "384-1")
            {
                itemName = "[Insert Mega Stone Here]";
            }

            if(!string.IsNullOrEmpty(itemName))
            {
                formDetails += " @ " + itemName;
            }

            return formDetails;
        }

        [AllowAnonymous]
        [Route("get-pokemon-team")]
        public TeamRandomizerViewModel GetPokemonTeam(List<string> selectedGens, string selectedGame, List<string> selectedLegendaries, List<string> selectedForms, string selectedEvolutions, bool onlyLegendaries, bool onlyAltForms, bool multipleMegas, bool onePokemonForm, bool randomAbility)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<Generation> unselectedGens = this._dataService.GetGenerations().Where(x => !x.Id.Contains('-')).ToList();
                foreach(var item in selectedGens)
                {
                    unselectedGens.Remove(unselectedGens.Find(x => x.Id == item));
                }

                Pokemon pokemon;
                TeamRandomizerViewModel model = new TeamRandomizerViewModel(){
                    AllPokemonChangedNames = new List<Pokemon>(),
                    AllPokemonOriginalNames = new List<Pokemon>(),
                    PokemonAbilities = new List<Ability>(),
                };
                List<Pokemon> pokemonList = new List<Pokemon>();
                List<PokemonGameDetail> availablePokemon = this._dataService.GetPokemonGameDetailsByGeneration(selectedGame);
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
                        List<PokemonFormDetail> pokemonFormList = this._dataService.GetAllAltFormsOnlyComplete().Where(x => x.Form.Id == 9 || x.Form.Id == 10 || x.Form.Id == 11).ToList();

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
                        List<PokemonFormDetail> pokemonFormList = this._dataService.GetAllAltFormsOnlyComplete().Where(x => x.Form.Id == 21).ToList();

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
                        List<PokemonFormDetail> pokemonFormList = this._dataService.GetAllAltFormsOnlyComplete().Where(x => x.Form.Id == 1001).ToList();

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

                    allPokemon = this.RemoveExtraPokemonForms(allPokemon);
                }

                if (onlyAltForms)
                {
                    allPokemon = allPokemon.Where(x => x.Id.Contains('-')).ToList();
                }
                
                if (!multipleMegas)
                {
                    List<Pokemon> megaList = new List<Pokemon>();
                    List<PokemonFormDetail> altFormList = this._dataService.GetAllAltForms();
                    foreach(var p in altFormList.Where(x => x.Form.Id == 9
                                                         || x.Form.Id == 10
                                                         || x.Form.Id == 11).ToList())
                    {
                        if(allPokemon.Exists(x => x.Id == p.AltFormPokemonId))
                        {
                            megaList.Add(p.AltFormPokemon);
                        }
                    }

                    if(megaList.Count > 0)
                    {
                        Pokemon mega = megaList[rnd.Next(megaList.Count)];
                        foreach(var p in megaList.Where(x => x.Id != mega.Id))
                        {
                            if (allPokemon.Exists(x => x.Id == p.Id))
                            {
                                allPokemon.Remove(allPokemon.Find(x => x.Id == p.Id));
                            }
                        }
                    }
                }

                if(availablePokemon.Count() > 1)
                {
                    allPokemon = allPokemon.Where(x => availablePokemon.Any(y => y.PokemonId == x.Id)).ToList();
                }

                if(allPokemon.Count() > 0)
                {
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

                        if (onePokemonForm)
                        {
                            string originalPokemonId;
                            if (pokemon.Id.Contains('-'))
                            {
                                originalPokemonId = pokemon.Id.Substring(0, pokemon.Id.IndexOf('-'));
                            }
                            else
                            {
                                originalPokemonId = pokemon.Id;
                            }

                            List<Pokemon> altForms = this._dataService.GetAltForms(originalPokemonId);

                            if (pokemon.Id.Contains('-'))
                            {
                                altForms.Remove(altForms.Find(x => x.Id == pokemon.Id));
                                altForms.Add(this._dataService.GetPokemonById(originalPokemonId));
                            }

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

                if(randomAbility)
                {
                    foreach(var p in model.AllPokemonOriginalNames)
                    {
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

                        if(p.Id == "744")
                        {
                            abilities.Add(this._dataService.GetAbility(174));
                        }

                        if(p.Id == "718" || p.Id == "718-1")
                        {
                            model.PokemonAbilities.Add(abilities[0]);
                        }
                        else
                        {
                            model.PokemonAbilities.Add(abilities[rnd.Next(abilities.Count)]);
                        }
                    }
                }

                string zygardeOriginalId, necrozmaOriginalId;

                List<string> pokemonIds = new List<string>();
                foreach(var p in model.AllPokemonOriginalNames)
                {
                    pokemonIds.Add(p.Id);
                }

                List<string> abilityNames = new List<string>();
                foreach(var a in model.PokemonAbilities)
                {
                    abilityNames.Add(a.Name);
                }

                List<string> list = new List<string>(){ "1", "2" };
                    
                if(pokemonIds.IndexOf("800-1") > -1 && pokemonIds.IndexOf("800-2") == -1)
                {
                    necrozmaOriginalId = "800-2";
                }
                else if(pokemonIds.IndexOf("800-1") == -1 && pokemonIds.IndexOf("800-2") > -1)
                {
                    necrozmaOriginalId = "800-1";
                }
                else
                {
                    necrozmaOriginalId = "800-" + list[rnd.Next(list.Count)];
                }

                if(pokemonIds.IndexOf("718") > -1 && pokemonIds.IndexOf("718-1") == -1)
                {
                    zygardeOriginalId = "718-1";
                }
                else if(pokemonIds.IndexOf("718") == -1 && pokemonIds.IndexOf("718-1") > -1)
                {
                    zygardeOriginalId = "718";
                }
                else
                {
                    if(list[rnd.Next(list.Count)] == "1")
                    {
                        zygardeOriginalId = "718";
                    }
                    else
                    {
                        zygardeOriginalId = "718-1";
                    }
                }

                model.ExportString = this.ExportPokemonTeam(pokemonIds, abilityNames, randomAbility, necrozmaOriginalId, zygardeOriginalId);

                return model;
            }
            else
            {
                this.RedirectToAction("Home", "Index");
            }

            return null;
        }

        private List<Form> GatherRemovableForms()
        {
            List<Form> forms = new List<Form>();

            forms.Add(this._dataService.GetForm(9));
            forms.Add(this._dataService.GetForm(10));
            forms.Add(this._dataService.GetForm(11));
            forms.Add(this._dataService.GetForm(21));
            forms.Add(this._dataService.GetForm(1001));
            forms.Add(this._dataService.GetForm(34));
            forms.Add(this._dataService.GetForm(35));
            forms.Add(this._dataService.GetForm(33));
            forms.Add(this._dataService.GetForm(47));
            forms.Add(this._dataService.GetForm(1002));
            forms.Add(this._dataService.GetForm(13));
            forms.Add(this._dataService.GetForm(46));
            forms.Add(this._dataService.GetForm(45));
            forms.Add(this._dataService.GetForm(44));
            forms.Add(this._dataService.GetForm(29));
            forms.Add(this._dataService.GetForm(22));

            return forms;
        }

        private List<Pokemon> RemoveExtraPokemonForms(List<Pokemon> pokemonList)
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
        [Route("get-pokemon-abilities")]
        public List<Ability> GetPokemonAbilities(string pokemonId, string gender)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<Ability> pokemonAbilities = this._dataService.GetAbilitiesForPokemon(((pokemonId == "678" && gender == "Female") ? "678-1" : pokemonId));
                return pokemonAbilities;
            }
            else
            {
                this.RedirectToAction("Home", "Index");
            }

            return null;
        }

        [AllowAnonymous]
        [Route("get-pokemon-battle-items")]
        public List<BattleItem> GetPokemonBattleItems(string pokemonId, string generationId)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<BattleItem> battleItems = new List<BattleItem>();
                List<FormItem> formItems = this._dataService.GetFormItems().Where(x => x.PokemonId == pokemonId).ToList();
                if(formItems.Count() == 0)
                {
                    Generation generation = this._dataService.GetGeneration(generationId);
                    List<BattleItem> allBattleItems = this._dataService.GetBattleItems();
                    battleItems.AddRange(allBattleItems.Where(x => !x.OnlyInThisGeneration && x.PokemonId == null).ToList());
                    if(generation != null)
                    {
                        battleItems = battleItems.Where(x => x.Generation.ReleaseDate <= generation.ReleaseDate).ToList();
                        if(allBattleItems.Where(x => x.OnlyInThisGeneration && x.GenerationId == generation.Id).ToList().Count() > 0)
                        {
                            battleItems.AddRange(allBattleItems.Where(x => x.OnlyInThisGeneration && x.GenerationId == generation.Id).ToList());
                        }

                        if(allBattleItems.Where(x => x.PokemonId == pokemonId && x.Generation.ReleaseDate <= generation.ReleaseDate).ToList().Count() > 0)
                        {
                            battleItems.AddRange(allBattleItems.Where(x => x.PokemonId == pokemonId && x.Generation.ReleaseDate <= generation.ReleaseDate).ToList());
                        }
                    }
                    else
                    {
                        if(allBattleItems.Where(x => x.PokemonId == pokemonId).ToList().Count() > 0)
                        {
                            battleItems.AddRange(allBattleItems.Where(x => x.PokemonId == pokemonId).ToList());
                        }
                    }

                    battleItems = battleItems.OrderBy(x => x.Name).ToList();
                }
                
                return battleItems;
            }
            else
            {
                this.RedirectToAction("Home", "Index");
            }

            return null;
        }

        [AllowAnonymous]
        [Route("get-pokemon-genders")]
        public List<string> GetPokemonGenders(string pokemonId)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<string> genders = new List<string>();
                GenderRatio genderRatio = this._dataService.GetPokemonById(pokemonId).GenderRatio;
                if(genderRatio.MaleRatio == 0 && genderRatio.FemaleRatio == 0)
                {
                    genders.Add("None");
                }
                else if(genderRatio.MaleRatio == 0)
                {
                    genders.Add("Female");
                }
                else if(genderRatio.FemaleRatio == 0)
                {
                    genders.Add("Male");
                }
                else
                {
                    if(pokemonId != "678")
                    {
                        genders.Add("");
                    }

                    genders.Add("Male");
                    genders.Add("Female");
                }

                return genders;
            }
            else
            {
                this.RedirectToAction("Home", "Index");
            }

            return null;
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
        [Route("get-generations")]
        public List<Generation> GetGenerations(string selectedGame)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                if(selectedGame != "0")
                {
                    List<PokemonGameDetail> availablePokemon = this._dataService.GetPokemonGameDetailsByGeneration(selectedGame).Where(x => !x.PokemonId.Contains('-')).ToList();
                    List<Pokemon> allPokemon = this._dataService.GetAllPokemon().Where(x => availablePokemon.Any(y => y.PokemonId == x.Id)).ToList();
                    Generation selectedGen = this._dataService.GetGeneration(selectedGame);
                    List<Generation> generationList = this._dataService.GetGenerations().Where(x => !x.Id.Contains('-') && x.ReleaseDate <= selectedGen.ReleaseDate).ToList();
                    List<Generation> availableGenerations = new List<Generation>();

                    foreach(var gen in generationList)
                    {
                        if(allPokemon.Where(x => x.GenerationId == gen.Id || x.GenerationId.Contains(string.Concat(gen.Id, "-"))).ToList().Count() != 0)
                        {
                            availableGenerations.Add(gen);
                        }
                    }
                    return availableGenerations;
                }
                else
                {
                    return this._dataService.GetGenerations().Where(x => !x.Id.Contains('-')).ToList();
                }
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
                    AppConfig = _appConfig,
                };

                return model;
            }
            else
            {
                this.RedirectToAction("Home", "Index");
            }

            return null;
        }

        [AllowAnonymous]
        [Route("get-pokemon-by-egg-group")]
        public EggGroupEvaluatorViewModel GetPokemonByEggGroup(int primaryEggGroupId, int secondaryEggGroupId)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<PokemonEggGroupDetail> eggGroupList = this._dataService.GetAllPokemonWithSpecificEggGroups(primaryEggGroupId, secondaryEggGroupId);
                List<Pokemon> pokemonList = new List<Pokemon>();

                foreach(var p in eggGroupList)
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

                EggGroupEvaluatorViewModel model = new EggGroupEvaluatorViewModel(){
                    AllPokemonWithEggGroups = eggGroupList,
                    AllPokemon = pokemonList,
                    AppConfig = _appConfig,
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
