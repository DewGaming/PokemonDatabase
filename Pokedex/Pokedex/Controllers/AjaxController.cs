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

        [Route("mark-as-read")]
        public void MarkAsRead()
        {
            List<Message> messages = this._dataService.GetMessagesToUsername(User.Identity.Name);
            foreach(var m in messages.Where(x => !x.IsRead))
            {
                m.IsRead = true;
                this._dataService.UpdateMessage(m);
            }
        }

        [Route("check-unread-messages")]
        public int CheckUnreadMessages()
        {
            return this._dataService.GetMessagesToUsername(User.Identity.Name).Where(x => !x.IsRead).ToList().Count;
        }

        [Route("get-pokemon-by-generation-admin/{generationId}")]
        public IActionResult GetPokemonByGenerationAdmin(int generationId)
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
                    PokemonList = this._dataService.GetAllPokemonWithoutFormsWithIncomplete().Where(x => x.Game.GenerationId == generationId).ToList(),
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
        [Route("update-pokemon-list/{gameId}")]
        public UpdatePokemonListViewModel UpdatePokemonList(int gameId)
        {
            Game game = this._dataService.GetGame(gameId);
            List<PokemonGameDetail> pokemonGameDetails = this._dataService.GetPokemonGameDetailsByGame(gameId);
            UpdatePokemonListViewModel pokemonList = new UpdatePokemonListViewModel(){
                PokemonList = this._dataService.GetAllPokemon().Where(x => pokemonGameDetails.Any(y => y.PokemonId == x.Id) && !this._dataService.CheckIfAltForm(x.Id)).ToList(),
                Game = game,
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
                            ExportString = "=== ",
                            TeamId = team.Id,
                        };

                        pokemonTeam.ExportString +=  string.Concat(team.PokemonTeamName, " ===\n\n");

                        for(var i = 0; i < pokemonList.Count(); i++)
                        {
                            if(i != 0)
                            {
                                pokemonTeam.ExportString += "\n\n";
                            }

                            pokemonTeam.ExportString += this.FillUserPokemonTeam(pokemonList[i], team.GameId);
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
        public string FillUserPokemonTeam(PokemonTeamDetail pokemonTeamDetail, int? generationId)
        {
            Pokemon pokemon = this._dataService.GetPokemonById(pokemonTeamDetail.PokemonId);
            List<string> pokemonForm = new List<string>();
            string pokemonName = string.Empty;
            if(this._dataService.CheckIfAltForm(pokemon.Id))
            {
                pokemonForm = this.GetUserFormDetails(pokemon.Id);
            }

            if(!string.IsNullOrEmpty(pokemonTeamDetail.Nickname))
            {
                pokemonName = pokemonTeamDetail.Nickname + " (";
            }
            
            pokemonName += pokemon.Name;
            if(this._dataService.CheckIfAltForm(pokemon.Id))
            {
                pokemonName += "-" + ((pokemonForm[0] == "Female") ? "F" : pokemonForm[0]);
            }

            if(!string.IsNullOrEmpty(pokemonTeamDetail.Nickname))
            {
                pokemonName += ")";
            }

            if(!string.IsNullOrEmpty(pokemonTeamDetail.Gender) && generationId != 1)
            {
                pokemonName += " (" + pokemonTeamDetail.Gender.Substring(0,1) + ")";
            }

            if(pokemonForm.Count == 2)
            {
                pokemonName += pokemonForm[1];
            }
            else if(pokemonTeamDetail.BattleItemId != null && generationId != 1)
            {
                pokemonName += " @ " + pokemonTeamDetail.BattleItem.Name;
            }

            string pokemonTeamString = pokemonName;
            if(generationId != 1 && generationId != 2)
            {
                pokemonTeamString += "\nAbility: " + pokemonTeamDetail.Ability.Name;
            }

            if(pokemonTeamDetail.Level < 100)
            {
                pokemonTeamString += "\nLevel: " + pokemonTeamDetail.Level.ToString();
            }

            if(pokemonTeamDetail.IsShiny && generationId != 1)
            {
                pokemonTeamString += "\nShiny: Yes";
            }

            if(pokemonTeamDetail.Happiness < 255 && generationId != 1)
            {
                pokemonTeamString += "\nHappiness: " + pokemonTeamDetail.Happiness.ToString();
            }
            
            if(pokemonTeamDetail.PokemonTeamEV != null)
            {
                pokemonTeamString += this.FillEVs(pokemonTeamDetail.PokemonTeamEV);
            }

            if(pokemonTeamDetail.Nature != null && generationId != 1 && generationId != 2)
            {
                pokemonTeamString += "\n" + pokemonTeamDetail.Nature.Name + " Nature";
            }
            
            if(pokemonTeamDetail.PokemonTeamIV != null)
            {
                pokemonTeamString += this.FillIVs(pokemonTeamDetail.PokemonTeamIV);
            }
            
            if(pokemonTeamDetail.PokemonTeamMoveset != null)
            {
                pokemonTeamString += this.FillMoveset(pokemonTeamDetail.PokemonTeamMoveset);
            }

            return pokemonTeamString;
        }

        private string FillEVs(PokemonTeamEV evs)
        {
            string evString = string.Empty;
            if(evs.EVTotal == 0)
            {
                evString = "\nEVs: 1 HP / 1 Atk / 1 Def / 1 SpA / 1 SpD / 1 Spe";
            }
            else
            {
                if(evs.Health > 0)
                {
                    evString += evs.Health.ToString() + " HP";
                }

                if(evs.Attack > 0)
                {
                    if(!string.IsNullOrEmpty(evString))
                    {
                        evString += " / ";
                    }
                    
                    evString += evs.Attack.ToString() + " Atk";
                }

                if(evs.Defense > 0)
                {
                    if(!string.IsNullOrEmpty(evString))
                    {
                        evString += " / ";
                    }
                    
                    evString += evs.Defense.ToString() + " Def";
                }

                if(evs.SpecialAttack > 0)
                {
                    if(!string.IsNullOrEmpty(evString))
                    {
                        evString += " / ";
                    }
                    
                    evString += evs.SpecialAttack.ToString() + " SpA";
                }

                if(evs.SpecialDefense > 0)
                {
                    if(!string.IsNullOrEmpty(evString))
                    {
                        evString += " / ";
                    }
                    
                    evString += evs.SpecialDefense.ToString() + " SpD";
                }

                if(evs.Speed > 0)
                {
                    if(!string.IsNullOrEmpty(evString))
                    {
                        evString += " / ";
                    }

                    evString += evs.Speed.ToString() + " Spe";
                }

                evString = string.Concat("\nEVs: ", evString);
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

        private string FillMoveset(PokemonTeamMoveset moveset)
        {
            string movesetString = string.Empty;
            if(!string.IsNullOrEmpty(moveset.FirstMove))
            {
                movesetString += string.Concat("\n- ", moveset.FirstMove);
            }
            
            if(!string.IsNullOrEmpty(moveset.SecondMove))
            {
                movesetString += string.Concat("\n- ", moveset.SecondMove);
            }
            
            if(!string.IsNullOrEmpty(moveset.ThirdMove))
            {
                movesetString += string.Concat("\n- ", moveset.ThirdMove);
            }
            
            if(!string.IsNullOrEmpty(moveset.FourthMove))
            {
                movesetString += string.Concat("\n- ", moveset.FourthMove);
            }

            return movesetString;
        }

        private List<string> GetUserFormDetails(int pokemonId)
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
        public string SavePokemonTeam(string pokemonTeamName, int selectedGame, List<int> pokemonIdList, List<int> abilityIdList, bool exportAbilities)
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

                    if(selectedGame != 0)
                    {
                        pokemonTeam.GameId = selectedGame;
                    }

                    Pokemon pokemon;
                    Ability ability;
                    PokemonTeamDetail pokemonTeamDetail;

                    for(var i = 0; i < pokemonIdList.Count; i++)
                    {
                        pokemon = this._dataService.GetPokemonById(pokemonIdList[i]);

                        if(exportAbilities)
                        {
                            ability = this._dataService.GetAbility(abilityIdList[i]);
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
        [Route("get-pokemon-by-generation/{generationId}/{showSprites:int}")]
        public IActionResult GetPokemonByGeneration(int generationId, int showSprites)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                GenerationTableViewModel model = new GenerationTableViewModel()
                {
                    PokemonList = this._dataService.GetAllPokemonWithTypes().Where(x => x.Pokemon.Game.GenerationId == generationId).ToList(),
                    ShowSprites = Convert.ToBoolean(showSprites),
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
        [Route("get-available-pokemon-by-game/{gameId}")]
        public IActionResult GetAvailablePokemonByGame(int gameId)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<PokemonGameDetail> pokemonGameDetails = this._dataService.GetPokemonGameDetailsByGame(gameId);
                List<Pokemon> pokemonList = this._dataService.GetAllPokemonIncludeIncomplete();
                pokemonList = pokemonList.Where(x => pokemonGameDetails.Any(y => y.PokemonId == x.Id)).ToList();
                foreach(var p in pokemonList.Where(x => this._dataService.CheckIfAltForm(x.Id)))
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

        private string ExportPokemonTeam(List<int> pokemonIdList, List<string> abilityList, bool exportAbilities)
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
                    if(this._dataService.CheckIfAltForm(pokemon.Id))
                    {
                        pokemonForm = this.GetFormDetails(pokemon.Id);
                        pokemonName += "-" + pokemonForm;
                    }

                    pokemonTeam += pokemonName;
                    if(exportAbilities)
                    {
                        pokemonTeam += "\nAbility: " + abilityList[i];
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

        private string GetFormDetails(int pokemonId)
        {
            string formDetails = string.Empty, itemName = string.Empty;
            PokemonFormDetail pokemonFormDetail;

            pokemonFormDetail = this._dataService.GetPokemonFormDetailByAltFormId(pokemonId);

            formDetails += pokemonFormDetail.Form.Name.Replace(' ', '-');
            
            FormItem formItem = this._dataService.GetFormItemByPokemonId(pokemonId);
            if(formItem != null)
            {
                itemName = formItem.Name;
            }

            if(!string.IsNullOrEmpty(itemName))
            {
                formDetails += " @ " + itemName;
            }

            return formDetails;
        }

        [AllowAnonymous]
        [Route("get-pokemon-team")]
        public TeamRandomizerViewModel GetPokemonTeam(List<int> selectedGens, int selectedGame, List<string> selectedLegendaries, List<string> selectedForms, string selectedEvolutions, bool onlyLegendaries, bool onlyAltForms, bool multipleMegas, bool onePokemonForm, bool randomAbility)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<Generation> unselectedGens = this._dataService.GetGenerations().ToList();
                foreach(var item in selectedGens)
                {
                    unselectedGens.Remove(unselectedGens.Find(x => x.Id == item));
                }

                Pokemon pokemon;
                TeamRandomizerViewModel model = new TeamRandomizerViewModel(){
                    AllPokemonChangedNames = new List<Pokemon>(),
                    AllPokemonOriginalNames = new List<Pokemon>(),
                    PokemonAbilities = new List<Ability>(),
                    AppConfig = _appConfig,
                };
                List<Pokemon> pokemonList = new List<Pokemon>();
                List<PokemonGameDetail> availablePokemon = new List<PokemonGameDetail>();

                if(selectedGame != 0)
                {
                    availablePokemon = this._dataService.GetPokemonGameDetailsByGeneration(this._dataService.GetGenerationFromGame(selectedGame).Id);
                }
                else
                {
                    availablePokemon = this._dataService.GetAllPokemonGameDetails();
                }

                List<Pokemon> allPokemon = this._dataService.GetAllPokemonWithoutForms();
                List<Evolution> allEvolutions = this._dataService.GetEvolutions();
                Random rnd = new Random();

                foreach(var gen in unselectedGens)
                {
                    allPokemon = allPokemon.Except(allPokemon.Where(x => x.Game.GenerationId == gen.Id)).ToList();
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
                        List<PokemonFormDetail> pokemonFormList = this._dataService.GetAllAltFormsOnlyComplete().Where(x => x.Form.Name == "Mega" || x.Form.Name == "Mega X" || x.Form.Name == "Mega Y").ToList();

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
                        List<PokemonFormDetail> pokemonFormList = this._dataService.GetAllAltFormsOnlyComplete().Where(x => x.Form.Name == "Alola").ToList();

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
                        List<PokemonFormDetail> pokemonFormList = this._dataService.GetAllAltFormsOnlyComplete().Where(x => x.Form.Name == "Galar").ToList();

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
                    allPokemon = allPokemon.Where(x => this._dataService.CheckIfAltForm(x.Id)).ToList();
                }
                
                if (!multipleMegas)
                {
                    List<Pokemon> megaList = new List<Pokemon>();
                    List<PokemonFormDetail> altFormList = this._dataService.GetAllAltForms();
                    foreach(var p in altFormList.Where(x => x.Form.Name.Contains("Mega")).ToList())
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
                            int originalPokemonId;
                            if (this._dataService.CheckIfAltForm(pokemon.Id))
                            {
                                originalPokemonId = this._dataService.GetOriginalPokemonByAltFormId(pokemon.Id).Id;
                            }
                            else
                            {
                                originalPokemonId = pokemon.Id;
                            }

                            List<Pokemon> altForms = this._dataService.GetAltForms(originalPokemonId);

                            if (this._dataService.CheckIfAltForm(pokemon.Id))
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

                        if(pokemonAbilities.SpecialEventAbility != null)
                        {
                            abilities.Add(pokemonAbilities.SpecialEventAbility);
                        }

                        model.PokemonAbilities.Add(abilities[rnd.Next(abilities.Count)]);
                    }
                }

                List<int> pokemonIds = new List<int>();
                foreach(var p in model.AllPokemonOriginalNames)
                {
                    pokemonIds.Add(p.Id);
                }

                List<string> abilityNames = new List<string>();
                foreach(var a in model.PokemonAbilities)
                {
                    abilityNames.Add(a.Name);
                }

                model.ExportString = this.ExportPokemonTeam(pokemonIds, abilityNames, randomAbility);

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
            List<string> formNames = new List<string>()
            {
                "Alola",
                "Galar",
                "Mega",
                "Mega X",
                "Mega Y",
                "Eternamax"
            };

            foreach(var formName in formNames)
            {
                forms.Add(this._dataService.GetFormByName(formName));
            }

            return forms.Where(x => x != null).ToList();
        }

        private List<Pokemon> RemoveExtraPokemonForms(List<Pokemon> pokemonList)
        {
            Random rnd = new Random();
            List<Pokemon> pumpkabooCount = pokemonList.Where(x => x.PokedexNumber == 710).ToList();
            while(pumpkabooCount.Count() > 1)
            {
                pokemonList.Remove(pumpkabooCount[rnd.Next(pumpkabooCount.Count)]);
                pumpkabooCount = pokemonList.Where(x => x.PokedexNumber == 710).ToList();
            }

            List<Pokemon> gourgeistCount = pokemonList.Where(x => x.PokedexNumber == 711).ToList();
            while(gourgeistCount.Count() > 1)
            {
                pokemonList.Remove(gourgeistCount[rnd.Next(gourgeistCount.Count)]);
                gourgeistCount = pokemonList.Where(x => x.PokedexNumber == 711).ToList();
            }

            return pokemonList;
        }

        [AllowAnonymous]
        [Route("get-pokemon-abilities")]
        public List<Ability> GetPokemonAbilities(int pokemonId, string gender)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<Ability> pokemonAbilities = this._dataService.GetAbilitiesForPokemon(pokemonId);
                return pokemonAbilities;
            }
            else
            {
                this.RedirectToAction("Home", "Index");
            }

            return null;
        }

        [AllowAnonymous]
        [Route("update-type-chart")]
        public string UpdateTypeChart(int typeId, List<int> resistances, List<int> weaknesses, List<int> immunities)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                TypeChart typeChart;
                List<int> duplicateIds = resistances.Where(x => weaknesses.Contains(x)).ToList();
                List<TypeChart> existingCharts = this._dataService.GetTypeChartByDefendType(typeId);

                foreach(var t in duplicateIds)
                {
                    resistances.Remove(resistances.Find(x => x == t));
                    weaknesses.Remove(weaknesses.Find(x => x == t));
                }

                foreach(var t in immunities)
                {
                    if(resistances.IndexOf(t) != -1)
                    {
                        resistances.Remove(resistances.Find(x => x == t));
                    }

                    if(weaknesses.IndexOf(t) != -1)
                    {
                        weaknesses.Remove(weaknesses.Find(x => x == t));
                    }
                }
                
                foreach(var r in resistances)
                {
                    typeChart = new TypeChart()
                    {
                        AttackId = r,
                        DefendId = typeId,
                        Effective = 0.5m,
                    };
                    this._dataService.AddTypeChart(typeChart);
                }
                
                foreach(var w in weaknesses)
                {
                    typeChart = new TypeChart()
                    {
                        AttackId = w,
                        DefendId = typeId,
                        Effective = 2m,
                    };
                    this._dataService.AddTypeChart(typeChart);
                }
                
                foreach(var i in immunities)
                {
                    typeChart = new TypeChart()
                    {
                        AttackId = i,
                        DefendId = typeId,
                        Effective = 0m,
                    };
                    this._dataService.AddTypeChart(typeChart);
                }

                foreach(var t in existingCharts)
                {
                    this._dataService.DeleteTypeChart(t.Id);
                }

                return Json(Url.Action("Types", "Admin")).Value.ToString();
            }
            else
            {
                this.RedirectToAction("Home", "Index");
            }

            return null;
        }

        [AllowAnonymous]
        [Route("update-game-availability")]
        public string UpdateGameAvailability(int pokemonId, List<int> games)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                PokemonGameDetail pokemonGameDetail;
                List<PokemonGameDetail> existingGameDetails = this._dataService.GetPokemonGameDetails(pokemonId);
                
                foreach(var g in games)
                {
                    pokemonGameDetail = new PokemonGameDetail()
                    {
                        PokemonId = pokemonId,
                        GameId = g,
                    };
                    this._dataService.AddPokemonGameDetail(pokemonGameDetail);
                }

                foreach(var g in existingGameDetails)
                {
                    this._dataService.DeletePokemonGameDetail(g.Id);
                }

                return Json(Url.Action("Pokemon", "Admin")).Value.ToString();
            }
            else
            {
                this.RedirectToAction("Home", "Index");
            }

            return null;
        }

        [AllowAnonymous]
        [Route("get-pokemon-battle-items")]
        public List<BattleItem> GetPokemonBattleItems(int pokemonId, int generationId)
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
                        battleItems = battleItems.Where(x => x.Generation.Id <= generation.Id).ToList();
                        if(allBattleItems.Where(x => x.OnlyInThisGeneration && x.GenerationId == generation.Id).ToList().Count() > 0)
                        {
                            battleItems.AddRange(allBattleItems.Where(x => x.OnlyInThisGeneration && x.GenerationId == generation.Id).ToList());
                        }

                        if(allBattleItems.Where(x => x.PokemonId == pokemonId && x.Generation.Id <= generation.Id).ToList().Count() > 0)
                        {
                            battleItems.AddRange(allBattleItems.Where(x => x.PokemonId == pokemonId && x.Generation.Id <= generation.Id).ToList());
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
        public List<string> GetPokemonGenders(int pokemonId)
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
        public IActionResult GetTypingTypeChart(int primaryTypeId, int secondaryTypeId)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return this.PartialView("_FillTypeEffectiveness", _dataService.GetTypeChartTyping(primaryTypeId, secondaryTypeId));
            }
            else
            {
                this.RedirectToAction("Home", "Index");
            }

            return null;
        }

        [AllowAnonymous]
        [Route("get-generations")]
        public List<Generation> GetGenerations(int selectedGame)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                if(selectedGame != 0)
                {
                    List<PokemonGameDetail> availablePokemon = this._dataService.GetPokemonGameDetailsByGame(selectedGame).Where(x => !this._dataService.CheckIfAltForm(x.PokemonId)).ToList();
                    List<Pokemon> allPokemon = this._dataService.GetAllPokemon().Where(x => availablePokemon.Any(y => y.PokemonId == x.Id)).ToList();
                    Generation selectedGen = this._dataService.GetGenerationFromGame(selectedGame);
                    List<Generation> generationList = this._dataService.GetGenerations().Where(x => x.Id <= selectedGen.Id).ToList();
                    List<Generation> availableGenerations = new List<Generation>();

                    foreach(var gen in generationList)
                    {
                        if(allPokemon.Where(x => x.Game.GenerationId == gen.Id).ToList().Count() != 0)
                        {
                            availableGenerations.Add(gen);
                        }
                    }
                    return availableGenerations;
                }
                else
                {
                    return this._dataService.GetGenerations().Where(x => this._dataService.GetAllPokemonWithoutForms().Any(y => y.Game.GenerationId == x.Id)).ToList();
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
        public IActionResult GetPokemon(int primaryTypeId, int secondaryTypeId)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<PokemonTypeDetail> typingList = this._dataService.GetAllPokemonWithSpecificTypes(primaryTypeId, secondaryTypeId);
                List<Pokemon> pokemonList = new List<Pokemon>();

                foreach(var p in typingList)
                {
                    if(this._dataService.CheckIfAltForm(p.PokemonId))
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

                return this.PartialView("_FillTypingEvaluator", model);
            }
            else
            {
                this.RedirectToAction("Home", "Index");
            }

            return null;
        }

        [AllowAnonymous]
        [Route("get-pokemon-by-egg-group")]
        public IActionResult GetPokemonByEggGroup(int pokemonId)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                PokemonEggGroupDetail searchedEggGroupDetails = this._dataService.GetPokemonWithEggGroups(pokemonId);
                GenderRatio genderRatio = this._dataService.GetGenderRatio(searchedEggGroupDetails.Pokemon.GenderRatioId);
                List<PokemonEggGroupDetail> eggGroupList = new List<PokemonEggGroupDetail>();
                List<Pokemon> pokemonList = new List<Pokemon>();
                if(pokemonId == this._dataService.GetPokemon("Manaphy").Id || pokemonId == this._dataService.GetPokemon("Phione").Id || (genderRatio.MaleRatio == 0 && genderRatio.FemaleRatio == 0 && pokemonId != this._dataService.GetPokemon("Ditto").Id))
                {
                    eggGroupList.Add(this._dataService.GetPokemonWithEggGroupsFromPokemonName("Ditto"));
                    pokemonList.Add(this._dataService.GetPokemon("Ditto"));
                }
                else if(pokemonId == this._dataService.GetPokemon("Ditto").Id)
                {
                    Pokemon pokemon;
                    eggGroupList = this._dataService.GetAllPokemonWithEggGroupsAndIncomplete();
                    List<PokemonEggGroupDetail> breedablePokemonList = this._dataService.GetAllBreedablePokemon();
                    eggGroupList = eggGroupList.Where(x => breedablePokemonList.Any(y => y.PokemonId == x.PokemonId)).OrderBy(x => x.Pokemon.Name).ToList();
                    eggGroupList.Remove(eggGroupList.Find(x => x.PokemonId == pokemonId));

                    foreach(var p in eggGroupList)
                    {
                        if(this._dataService.CheckIfAltForm(p.PokemonId))
                        {
                            pokemon = this._dataService.GetAltFormWithFormName(p.PokemonId);
                            pokemonList.Add(pokemon);
                        }
                        else
                        {
                            pokemonList.Add(p.Pokemon);
                        }
                    }

                    pokemon = this._dataService.GetPokemonById(pokemonId);
                }
                else
                {
                    Pokemon pokemon;
                    eggGroupList = this._dataService.GetAllPokemonWithSpecificEggGroups((int)searchedEggGroupDetails.PrimaryEggGroupId, searchedEggGroupDetails.SecondaryEggGroupId);
                    List<PokemonEggGroupDetail> breedablePokemonList = this._dataService.GetAllBreedablePokemon();
                    eggGroupList = eggGroupList.Where(x => breedablePokemonList.Any(y => y.PokemonId == x.PokemonId)).OrderBy(x => x.Pokemon.Name).ToList();

                    foreach(var p in eggGroupList)
                    {
                        if(this._dataService.CheckIfAltForm(p.PokemonId))
                        {
                            pokemon = this._dataService.GetAltFormWithFormName(p.PokemonId);
                            pokemonList.Add(pokemon);
                        }
                        else
                        {
                            pokemonList.Add(p.Pokemon);
                        }
                    }

                    pokemon = this._dataService.GetPokemonById(pokemonId);

                    if(genderRatio.MaleRatio == 100 || genderRatio.FemaleRatio == 100)
                    {
                        pokemonList.Remove(pokemon);
                    }
                }

                EggGroupEvaluatorViewModel model = new EggGroupEvaluatorViewModel(){
                    AllPokemonWithEggGroups = eggGroupList,
                    AllPokemon = pokemonList,
                    AppConfig = _appConfig,
                    SearchedPokemon = this._dataService.GetPokemonById(pokemonId),
                };

                return this.PartialView("_FillDayCareEvaluator", model);
            }
            else
            {
                this.RedirectToAction("Home", "Index");
            }

            return null;
        }
    }
}
