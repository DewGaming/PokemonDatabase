using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Pokedex.DataAccess.Models;
using Pokedex.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Pokedex.Controllers
{
    [Authorize]
    [Route("")]
    public class AjaxController : Controller
    {
        private readonly DataService dataService;

        private readonly AppConfig appConfig;

        public AjaxController(IOptions<AppConfig> appConfig, DataContext dataContext)
        {
            this.appConfig = appConfig.Value;
            this.dataService = new DataService(dataContext);
        }

        [Route("mark-as-read")]
        public void MarkAsRead()
        {
            List<Message> messages = this.dataService.GetMessagesToUser(Convert.ToInt32(this.User.Claims.First(x => x.Type == "UserId").Value));
            foreach (var m in messages.Where(x => !x.IsRead))
            {
                m.IsRead = true;
                this.dataService.UpdateMessage(m);
            }
        }

        [Route("check-unread-messages")]
        public int CheckUnreadMessages()
        {
            return this.dataService.GetMessagesToUser(Convert.ToInt32(this.User.Claims.First(x => x.Type == "UserId").Value)).Where(x => !x.IsRead).ToList().Count;
        }

        [Route("update-last-visit")]
        public void UpdateLastVisit()
        {
            User user = this.dataService.GetUser(Convert.ToInt32(this.User.Claims.First(x => x.Type == "UserId").Value));
            user.LastVisit = DateTime.Now.ToUniversalTime();
            this.dataService.UpdateUser(user);
        }

        [Route("get-pokemon-by-generation-admin/{generationId}")]
        public IActionResult GetPokemonByGenerationAdmin(int generationId)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                AllAdminPokemonViewModel allAdminPokemon = this.dataService.GetAllAdminPokemonDetails();

                DropdownViewModel dropdownViewModel = new DropdownViewModel()
                {
                    AllPokemon = allAdminPokemon,
                    AppConfig = this.appConfig,
                    GenerationId = generationId,
                };

                AdminGenerationTableViewModel model = new AdminGenerationTableViewModel()
                {
                    PokemonList = this.dataService.GetAllPokemonWithoutFormsWithIncomplete().Where(x => x.Game.GenerationId == generationId).ToList(),
                    DropdownViewModel = dropdownViewModel,
                    AppConfig = this.appConfig,
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
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                AllAdminPokemonViewModel allAdminPokemon = this.dataService.GetAllAdminPokemonDetails();

                DropdownViewModel dropdownViewModel = new DropdownViewModel()
                {
                    AllPokemon = allAdminPokemon,
                    AppConfig = this.appConfig,
                    GenerationId = this.dataService.GetGenerations().Last().Id,
                };

                AdminGenerationTableViewModel model = new AdminGenerationTableViewModel()
                {
                    PokemonList = this.dataService.GetAllPokemonIncludeIncomplete().Where(x => !x.IsComplete).ToList(),
                    ReviewedPokemon = new List<ReviewedPokemon>(),
                    DropdownViewModel = dropdownViewModel,
                    AppConfig = this.appConfig,
                };

                foreach (var r in this.dataService.GetAllReviewedPokemon())
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
            ShinyHuntingTechnique technique = this.dataService.GetShinyHuntingTechnique(id);

            return technique;
        }

        [HttpPost]
        [Route("add-hunt-attempt/{huntId:int}")]
        public int AddShinyCounter(int huntId)
        {
            ShinyHunt hunt = this.dataService.GetShinyHunt(huntId);
            hunt.ShinyAttemptCount++;
            this.dataService.UpdateShinyHunt(hunt);
            return hunt.ShinyAttemptCount;
        }

        [HttpPost]
        [Route("subtract-hunt-attempt/{huntId:int}")]
        public int SubtractShinyCounter(int huntId)
        {
            ShinyHunt hunt = this.dataService.GetShinyHunt(huntId);
            if (hunt.ShinyAttemptCount > 0)
            {
                hunt.ShinyAttemptCount--;
                this.dataService.UpdateShinyHunt(hunt);
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
            ShinyHunt hunt = this.dataService.GetShinyHunt(huntId);
            if (attemptCount > 0)
            {
                hunt.ShinyAttemptCount = attemptCount;
                this.dataService.UpdateShinyHunt(hunt);
            }
            else
            {
                hunt.ShinyAttemptCount = 0;
                this.dataService.UpdateShinyHunt(hunt);
            }

            return hunt.ShinyAttemptCount;
        }

        [HttpPost]
        [Route("update-pokemon-list/{gameId}")]
        public UpdatePokemonListViewModel UpdatePokemonList(int gameId)
        {
            Game game = this.dataService.GetGame(gameId);
            List<PokemonGameDetail> pokemonGameDetails = this.dataService.GetPokemonGameDetailsByGame(gameId);
            List<Pokemon> altFormsList = this.dataService.GetAllAltForms().Select(x => x.AltFormPokemon).ToList();
            UpdatePokemonListViewModel pokemonList = new UpdatePokemonListViewModel()
            {
                PokemonList = this.dataService.GetAllPokemon().Where(x => pokemonGameDetails.Any(y => y.PokemonId == x.Id)).ToList(),
                Game = game,
            };

            foreach (var pokemon in pokemonList.PokemonList.Where(x => altFormsList.Any(y => y.Id == x.Id)))
            {
                pokemon.Name = this.dataService.GetAltFormWithFormName(pokemon.Id).Name;
            }

            return pokemonList;
        }

        [AllowAnonymous]
        [Route("grab-user-pokemon-team")]
        public List<ExportPokemonViewModel> ExportUserPokemonTeam(int teamId)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                PokemonTeam pokemonTeam = this.dataService.GetPokemonTeam(teamId);
                List<ExportPokemonViewModel> exportList = new List<ExportPokemonViewModel>();
                List<PokemonTeamDetail> pokemonList = pokemonTeam.GrabPokemonTeamDetails;
                if (pokemonList.Count() > 0)
                {
                    ExportPokemonViewModel pokemonTeamExport = new ExportPokemonViewModel()
                    {
                        ExportString = "=== ",
                        TeamId = pokemonTeam.Id,
                    };

                    if (pokemonTeam.Game != null)
                    {
                        pokemonTeamExport.ExportString = string.Concat(pokemonTeamExport.ExportString, " [gen", pokemonTeam.Game.GenerationId, "] ");
                    }

                    pokemonTeamExport.ExportString = string.Concat(pokemonTeamExport.ExportString, pokemonTeam.PokemonTeamName, " ===\n\n");

                    for (var i = 0; i < pokemonList.Count(); i++)
                    {
                        if (i != 0)
                        {
                            pokemonTeamExport.ExportString = string.Concat(pokemonTeamExport.ExportString, "\n\n");
                        }

                        pokemonTeamExport.ExportString = string.Concat(pokemonTeamExport.ExportString, this.FillUserPokemonTeam(pokemonList[i], pokemonTeam.GameId));
                    }

                    exportList.Add(pokemonTeamExport);
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
        [Route("grab-all-user-pokemon-teams")]
        public List<ExportPokemonViewModel> ExportAllUserPokemonTeams()
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<PokemonTeam> pokemonTeams = this.dataService.GetAllPokemonTeams(this.User.Identity.Name);
                List<PokemonTeamDetail> pokemonList;
                List<ExportPokemonViewModel> exportList = new List<ExportPokemonViewModel>();
                foreach (var team in pokemonTeams)
                {
                    pokemonList = team.GrabPokemonTeamDetails;
                    if (pokemonList.Count() > 0)
                    {
                        ExportPokemonViewModel pokemonTeam = new ExportPokemonViewModel
                        {
                            ExportString = "=== ",
                            TeamId = team.Id,
                        };

                        if (team.Game != null)
                        {
                            pokemonTeam.ExportString = string.Concat(pokemonTeam.ExportString, " [gen", team.Game.GenerationId, "] ");
                        }

                        pokemonTeam.ExportString = string.Concat(pokemonTeam.ExportString, team.PokemonTeamName, " ===\n\n");

                        for (var i = 0; i < pokemonList.Count(); i++)
                        {
                            if (i != 0)
                            {
                                pokemonTeam.ExportString = string.Concat(pokemonTeam.ExportString, "\n\n");
                            }

                            pokemonTeam.ExportString = string.Concat(pokemonTeam.ExportString, this.FillUserPokemonTeam(pokemonList[i], team.GameId));
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
            Pokemon pokemon = this.dataService.GetPokemonById(pokemonTeamDetail.PokemonId);
            List<string> pokemonForm = new List<string>();
            string pokemonName = string.Empty;
            if (this.dataService.CheckIfAltForm(pokemon.Id))
            {
                pokemonForm = this.GetUserFormDetails(pokemon.Id);
            }

            if (!string.IsNullOrEmpty(pokemonTeamDetail.Nickname))
            {
                pokemonName = string.Concat(pokemonTeamDetail.Nickname, " (");
            }

            if (pokemon.Name.Contains(" (Male)"))
            {
                pokemon.Name = pokemon.Name.Replace(" (Male)", "-M");
            }
            else if (pokemon.Name.Contains(" (Female)"))
            {
                pokemon.Name = pokemon.Name.Replace(" (Female)", "-F");
            }

            pokemonName = string.Concat(pokemonName, pokemon.Name);
            if (this.dataService.CheckIfAltForm(pokemon.Id))
            {
                pokemonName = string.Concat(pokemonName, "-", (pokemonForm[0] == "Female") ? "F" : pokemonForm[0]);
            }

            if (!string.IsNullOrEmpty(pokemonTeamDetail.Nickname))
            {
                pokemonName = string.Concat(pokemonName, ")");
            }

            if (!string.IsNullOrEmpty(pokemonTeamDetail.Gender) && generationId != 1)
            {
                pokemonName = string.Concat(pokemonName, " (", pokemonTeamDetail.Gender.Substring(0, 1), ")");
            }

            if (pokemonForm.Count == 2)
            {
                pokemonName = string.Concat(pokemonName, pokemonForm[1]);
            }
            else if (pokemonTeamDetail.BattleItemId != null && generationId != 1)
            {
                pokemonName = string.Concat(pokemonName, " @ ", pokemonTeamDetail.BattleItem.Name);
            }

            string pokemonTeamString = pokemonName;
            if (generationId != 1 && generationId != 2)
            {
                pokemonTeamString = string.Concat(pokemonTeamString, "\nAbility: ", pokemonTeamDetail.Ability.Name);
            }

            if (pokemonTeamDetail.Level < 100)
            {
                pokemonTeamString = string.Concat(pokemonTeamString, "\nLevel: ", pokemonTeamDetail.Level.ToString());
            }

            if (pokemonTeamDetail.IsShiny && generationId != 1)
            {
                pokemonTeamString = string.Concat(pokemonTeamString, "\nShiny: Yes");
            }

            if (pokemonTeamDetail.Happiness < 255 && generationId != 1)
            {
                pokemonTeamString = string.Concat(pokemonTeamString, "\nHappiness: ", pokemonTeamDetail.Happiness.ToString());
            }

            if (pokemonTeamDetail.PokemonTeamEV != null)
            {
                pokemonTeamString = string.Concat(pokemonTeamString, this.FillEVs(pokemonTeamDetail.PokemonTeamEV));
            }

            if (pokemonTeamDetail.Nature != null && generationId != 1 && generationId != 2)
            {
                pokemonTeamString = string.Concat(pokemonTeamString, "\n", pokemonTeamDetail.Nature.Name, " Nature");
            }

            if (pokemonTeamDetail.PokemonTeamIV != null)
            {
                pokemonTeamString = string.Concat(pokemonTeamString, this.FillIVs(pokemonTeamDetail.PokemonTeamIV));
            }

            if (pokemonTeamDetail.PokemonTeamMoveset != null)
            {
                pokemonTeamString = string.Concat(pokemonTeamString, this.FillMoveset(pokemonTeamDetail.PokemonTeamMoveset));
            }

            return pokemonTeamString;
        }

        [AllowAnonymous]
        [Route("save-pokemon-team")]
        public string SavePokemonTeam(string pokemonTeamName, int selectedGame, List<int> pokemonIdList, List<int> abilityIdList, bool exportAbilities)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                if (this.dataService.GetUserWithUsername(this.User.Identity.Name) != null)
                {
                    if (string.IsNullOrEmpty(pokemonTeamName))
                    {
                        pokemonTeamName = "Save from Team Randomizer";
                    }

                    PokemonTeam pokemonTeam = new PokemonTeam()
                    {
                        PokemonTeamName = pokemonTeamName,
                        UserId = this.dataService.GetUserWithUsername(this.User.Identity.Name).Id,
                    };

                    if (selectedGame != 0)
                    {
                        pokemonTeam.GameId = selectedGame;
                    }

                    Pokemon pokemon;
                    Ability ability;
                    PokemonTeamDetail pokemonTeamDetail;

                    for (var i = 0; i < pokemonIdList.Count; i++)
                    {
                        pokemon = this.dataService.GetPokemonById(pokemonIdList[i]);

                        if (exportAbilities)
                        {
                            ability = this.dataService.GetAbility(abilityIdList[i]);
                        }
                        else
                        {
                            ability = this.dataService.GetAbilitiesForPokemon(pokemon.Id)[0];
                        }

                        pokemonTeamDetail = new PokemonTeamDetail()
                        {
                            PokemonId = pokemon.Id,
                            AbilityId = ability.Id,
                            NatureId = this.dataService.GetNatureByName("Serious").Id,
                            Level = 100,
                            Happiness = 255,
                        };

                        this.dataService.AddPokemonTeamDetail(pokemonTeamDetail);

                        pokemonTeam.InsertPokemon(pokemonTeamDetail);
                    }

                    this.dataService.AddPokemonTeam(pokemonTeam);

                    return string.Concat("Team \"", pokemonTeam.PokemonTeamName, "\" has been added successfully!");
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
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<Pokemon> pokemonList = this.dataService.GetAllPokemonWithoutForms();

                foreach (var pokemon in pokemonList)
                {
                    if (pokemon.Name.Contains("type null"))
                    {
                        pokemon.Name = "Type: Null";
                    }

                    TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                    pokemon.Name = textInfo.ToTitleCase(pokemon.Name);

                    if (pokemon.Name.Contains("-O") && pokemon.Name.Substring(pokemon.Name.Length - 2, 2) == "-O")
                    {
                        pokemon.Name = string.Concat(pokemon.Name.Remove(pokemon.Name.Length - 2, 2), "-o");
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
        public IActionResult GetPokemonByGeneration(int generationId)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                GenerationTableViewModel model = new GenerationTableViewModel()
                {
                    PokemonList = this.dataService.GetAllPokemonWithTypes().Where(x => x.Pokemon.Game.GenerationId == generationId).ToList(),
                    AppConfig = this.appConfig,
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
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<PokemonGameDetail> pokemonGameDetails = this.dataService.GetPokemonGameDetailsByGame(gameId);
                List<Pokemon> pokemonList = this.dataService.GetAllPokemonIncludeIncomplete();
                pokemonList = pokemonList.Where(x => pokemonGameDetails.Any(y => y.PokemonId == x.Id)).ToList();
                List<Pokemon> altFormsList = this.dataService.GetAllAltForms().Select(x => x.AltFormPokemon).ToList();
                foreach (var p in pokemonList.Where(x => altFormsList.Any(y => y.Id == x.Id)))
                {
                    p.Name = string.Concat(p.Name, " (", this.dataService.GetFormByAltFormId(p.Id).Name, ")");
                }

                GenerationTableViewModel model = new GenerationTableViewModel()
                {
                    PokemonNoTypeList = pokemonList,
                    AltFormsList = altFormsList,
                    AppConfig = this.appConfig,
                    Generation = this.dataService.GetGenerationFromGame(gameId),
                };

                return this.PartialView("_FillAvailableGenerationTable", model);
            }
            else
            {
                return this.RedirectToAction("Home", "Index");
            }
        }

        [AllowAnonymous]
        [Route("get-capture-chance")]
        public string GetCaptureChange(int pokemonId, int generationId, float healthPercentage, int pokeballId, int statusId, float turnCount, float encounterLevel, float userLevel, bool surfing, bool fishing, bool previouslyCaught, bool caveOrNight, bool sameGender)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                Pokemon pokemon = this.dataService.GetPokemonById(pokemonId);
                Pokeball pokeball = this.dataService.GetPokeball(pokeballId);
                PokemonLegendaryDetail legendary = this.dataService.GetLegendaryDetail(pokemon.Id);
                List<PokemonCaptureRateDetail> captureRates = this.dataService.GetPokemonWithCaptureRates(pokemonId).OrderBy(x => x.GenerationId).ToList();
                List<PokeballCatchModifierDetail> pokeballDetails = this.dataService.GetCatchModifiersForPokeball(pokeballId);
                float statusEffect = 1;
                if (statusId != 0)
                {
                    statusEffect = this.dataService.GetStatus(statusId).Effect;
                }

                float catchRate = captureRates.First(x => x.GenerationId <= generationId).CaptureRate.CatchRate;
                float pokeballEffect = 1;
                if (statusEffect == 0)
                {
                    statusEffect = 1;
                }

                float heavyValue = 0f;

                switch (pokeball.Name)
                {
                    case "Heavy Ball":
                        foreach (var w in pokeballDetails)
                        {
                            if (pokemon.Weight < Convert.ToInt16(w.Effect))
                            {
                                heavyValue = (int)w.CatchModifier;
                                break;
                            }
                        }

                        catchRate += heavyValue;
                        if (catchRate <= 0)
                        {
                            catchRate = 1;
                        }

                        break;
                    case "Nest Ball":
                        if (encounterLevel <= 29)
                        {
                            pokeballEffect = (41f - encounterLevel) / 10f;
                        }

                        break;
                    case "Repeat Ball":
                        pokeballEffect = pokeballDetails.Find(x => bool.Parse(x.Effect) == previouslyCaught).CatchModifier;
                        break;
                    case "Dusk Ball":
                        pokeballEffect = pokeballDetails.Find(x => bool.Parse(x.Effect) == caveOrNight).CatchModifier;
                        break;
                    case "Quick Ball":
                        pokeballEffect = pokeballDetails.Find(x => bool.Parse(x.Effect) == (turnCount == 1)).CatchModifier;
                        break;
                    case "Net Ball":
                        PokemonTypeDetail typeDetail = this.dataService.GetPokemonWithTypes(pokemon.Id).Last();
                        bool netBallCondition = false;
                        if (typeDetail.PrimaryType.Name == "Bug" || typeDetail.PrimaryType.Name == "Water" || typeDetail.SecondaryType.Name == "Bug" || typeDetail.SecondaryType.Name == "Water")
                        {
                            netBallCondition = true;
                        }

                        pokeballEffect = pokeballDetails.Find(x => bool.Parse(x.Effect) == netBallCondition).CatchModifier;
                        break;
                    case "Dive Ball":
                        pokeballEffect = pokeballDetails.Find(x => bool.Parse(x.Effect) == surfing).CatchModifier;
                        break;
                    case "Lure Ball":
                        pokeballEffect = pokeballDetails.Find(x => bool.Parse(x.Effect) == fishing).CatchModifier;
                        break;
                    case "Dream Ball":
                        pokeballEffect = pokeballDetails.Find(x => bool.Parse(x.Effect) == (statusId == 2)).CatchModifier;
                        break;
                    case "Fast Ball":
                        BaseStat baseStats = this.dataService.GetBaseStat(pokemon.Id).Last();
                        pokeballEffect = pokeballDetails.Find(x => bool.Parse(x.Effect) == (baseStats.Speed >= 100)).CatchModifier;
                        break;
                    case "Level Ball":
                        float multiple = userLevel / encounterLevel;
                        pokeballEffect = pokeballDetails.Find(x => Convert.ToDouble(x.Effect) < multiple).CatchModifier;
                        break;
                    case "Love Ball":
                        pokeballEffect = pokeballDetails.Find(x => bool.Parse(x.Effect) == sameGender).CatchModifier;
                        break;
                    case "Timer Ball":
                        pokeballEffect = 1f + ((turnCount - 1) * (1229f / 4096f));
                        if (pokeballEffect > 4)
                        {
                            pokeballEffect = 4;
                        }

                        break;
                    case "Moon Ball":
                        List<Evolution> evolutions = this.dataService.GetPokemonEvolutions(pokemon.Id);
                        bool moonStoneEvolve = false;
                        foreach (var e in evolutions)
                        {
                            if (e.EvolutionMethod.Name == "Evolutionary Item" && e.EvolutionDetails == "Moon Stone")
                            {
                                moonStoneEvolve = true;
                                break;
                            }
                        }

                        pokeballEffect = pokeballDetails.Find(x => bool.Parse(x.Effect) == moonStoneEvolve).CatchModifier;
                        break;
                    case "Beast Ball":
                        pokeballEffect = pokeballDetails.Find(x => bool.Parse(x.Effect) == (legendary != null && legendary.LegendaryType.Type == "Ultra Beast")).CatchModifier;
                        break;
                    default:
                        pokeballEffect = pokeballDetails[0].CatchModifier;
                        break;
                }

                float catchValue;
                double captureChance;
                string chanceText = string.Empty;

                if (generationId == 1)
                {
                    float greatBallModifier = 12f;
                    if (statusId == 1 || statusId == 2)
                    {
                        statusEffect = 25f;
                    }
                    else if (statusId == 3 || statusId == 4 || statusId == 5)
                    {
                        statusEffect = 12f;
                    }
                    else
                    {
                        statusEffect = 0f;
                    }

                    if (pokeballId == 1)
                    {
                        pokeballEffect = 256f;
                    }
                    else if (pokeballId == 2)
                    {
                        pokeballEffect = 201f;
                        greatBallModifier = 8f;
                    }
                    else
                    {
                        pokeballEffect = 151f;
                    }

                    double hpFactor = Math.Floor(Math.Min(1020f / (greatBallModifier * healthPercentage), 255f));
                    captureChance = (statusEffect + (Math.Min(catchRate + 1f, pokeballEffect - statusEffect) * ((hpFactor + 1) / 256))) / pokeballEffect;
                }
                else if (generationId == 2)
                {
                    if (pokeball.Id == 24)
                    {
                        pokeballEffect = 1f;
                    }
                    else if (pokeball.Id == 18)
                    {
                        if (pokemonId == 1347 || pokemonId == 46 || pokemonId == 148)
                        {
                            pokeballEffect = 4f;
                        }
                        else
                        {
                            pokeballEffect = 1f;
                        }
                    }
                    else if (pokeball.Id == 20)
                    {
                        if (pokemon.Weight >= 409.6m)
                        {
                            heavyValue = 40f;
                        }
                        else if (pokemon.Weight >= 307.2m)
                        {
                            heavyValue = 30f;
                        }
                        else if (pokemon.Weight >= 204.8m)
                        {
                            heavyValue = 20f;
                        }
                        else if (pokemon.Weight < 102.4m)
                        {
                            heavyValue = -20f;
                        }

                        catchRate += heavyValue;
                    }

                    if (statusId == 1 || statusId == 2)
                    {
                        statusEffect = 10f;
                    }
                    else
                    {
                        statusEffect = 0f;
                    }

                    catchRate *= pokeballEffect;

                    if (catchRate > 255f)
                    {
                        catchRate = 255f;
                    }
                    else if (catchRate < 0f)
                    {
                        catchRate = 1f;
                    }

                    captureChance = ((Math.Floor(Math.Max(catchRate * (3f - (2f * healthPercentage)) / 3f, 1f)) % 256f) + statusEffect + 1) / 256;
                }
                else if (generationId == 3 || generationId == 4)
                {
                    if (statusId == 1 || statusId == 2)
                    {
                        statusEffect = 2f;
                    }

                    if (pokeball.Id == 20)
                    {
                        if (pokemon.Weight >= 409.6m)
                        {
                            heavyValue = 40f;
                        }
                        else if (pokemon.Weight >= 307.2m)
                        {
                            heavyValue = 30f;
                        }
                        else if (pokemon.Weight >= 204.8m)
                        {
                            heavyValue = 20f;
                        }
                        else if (pokemon.Weight < 102.4m)
                        {
                            heavyValue = -20f;
                        }

                        catchRate += heavyValue;
                    }

                    catchValue = (1f - ((2f * healthPercentage) / 3f)) * catchRate * statusEffect;
                    if (pokeball.Id != 20)
                    {
                        catchValue *= pokeballEffect;
                        if (catchRate > 255f)
                        {
                            catchRate = 255f;
                        }
                        else if (catchRate < 0f)
                        {
                            catchRate = 1f;
                        }
                    }

                    captureChance = Math.Pow(Math.Floor(1048560 / Math.Floor(Math.Sqrt(Math.Floor(Math.Sqrt(16711680 / catchValue))))) / 65536, 4);
                }
                else
                {
                    if (legendary != null && legendary.LegendaryType.Type == "Ultra Beast" && pokeball.Name != "Beast Ball")
                    {
                        pokeballEffect = 0.1f;
                    }

                    catchValue = Math.Min((1f - ((2f * healthPercentage) / 3f)) * catchRate * statusEffect, 255);
                    if (pokeball.Name != "Heavy Ball")
                    {
                        catchValue *= pokeballEffect;
                        if (catchRate > 255f)
                        {
                            catchRate = 255f;
                        }
                        else if (catchRate < 0f)
                        {
                            catchRate = 1f;
                        }
                    }

                    captureChance = Math.Pow(Math.Floor(65536 / Math.Pow(255 / catchValue, 3f / 16f)) / 65536, 4);
                }

                captureChance = captureChance * 100;
                chanceText = Math.Round(captureChance, 2) + "% chance";
                if (captureChance >= 100)
                {
                    chanceText = "Guaranteed";
                }

                return chanceText + " to catch " + pokemon.Name + " at " + (healthPercentage * 100) + "% health in a " + pokeball.Name;
            }
            else
            {
                return null;
            }
        }

        [AllowAnonymous]
        [Route("get-pokemon-team")]
        public TeamRandomizerViewModel GetPokemonTeam(List<int> selectedGens, int selectedGame, int selectedType, List<string> selectedLegendaries, List<string> selectedForms, string selectedEvolutions, bool onlyLegendaries, bool onlyAltForms, bool multipleMegas, bool multipleGMax, bool onePokemonForm, bool randomAbility)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<Generation> unselectedGens = this.dataService.GetGenerations().ToList();
                foreach (var item in selectedGens)
                {
                    unselectedGens.Remove(unselectedGens.Find(x => x.Id == item));
                }

                Pokemon pokemon;
                TeamRandomizerViewModel model = new TeamRandomizerViewModel()
                {
                    AllPokemonChangedNames = new List<Pokemon>(),
                    AllPokemonOriginalNames = new List<Pokemon>(),
                    PokemonAbilities = new List<Ability>(),
                    AppConfig = this.appConfig,
                };
                List<Pokemon> pokemonList = new List<Pokemon>();
                List<PokemonGameDetail> availablePokemon = new List<PokemonGameDetail>();

                if (selectedGame != 0)
                {
                    availablePokemon = this.dataService.GetPokemonGameDetailsByGame(selectedGame);
                }
                else
                {
                    availablePokemon = this.dataService.GetAllPokemonGameDetails();
                }

                List<Pokemon> allPokemon = this.dataService.GetAllPokemonWithoutForms();
                List<Evolution> allEvolutions = this.dataService.GetEvolutions();
                Random rnd = new Random();

                foreach (var gen in unselectedGens)
                {
                    allPokemon = allPokemon.Except(allPokemon.Where(x => x.Game.GenerationId == gen.Id)).ToList();
                }

                if (selectedLegendaries.Count() == 0)
                {
                    List<PokemonLegendaryDetail> legendaryList = this.dataService.GetAllPokemonWithLegendaryTypes();

                    foreach (var p in legendaryList)
                    {
                        allPokemon.Remove(allPokemon.FirstOrDefault(x => x.Id == p.PokemonId));
                    }
                }
                else
                {
                    List<string> legendaryTypes = this.dataService.GetLegendaryTypes().Select(x => x.Type).ToList();
                    foreach (var legendary in legendaryTypes.Except(selectedLegendaries).ToList())
                    {
                        List<PokemonLegendaryDetail> legendaryList = this.dataService.GetAllPokemonWithLegendaryTypes().Where(x => x.LegendaryType.Type == legendary).ToList();

                        foreach (var l in legendaryList)
                        {
                            allPokemon.Remove(allPokemon.FirstOrDefault(x => x.Id == l.PokemonId));
                        }
                    }
                }

                if (onlyLegendaries)
                {
                    List<Pokemon> legendaryList = new List<Pokemon>();
                    List<PokemonLegendaryDetail> allLegendaries = this.dataService.GetAllPokemonWithLegendaryTypes();

                    foreach (var p in allLegendaries)
                    {
                        if (allPokemon.Exists(x => x.Id == p.PokemonId))
                        {
                            legendaryList.Add(p.Pokemon);
                        }
                    }

                    allPokemon = legendaryList;
                }

                if (selectedForms.Count() != 0)
                {
                    List<Pokemon> altForms = new List<Pokemon>();

                    if (selectedForms.Contains("Mega"))
                    {
                        List<PokemonFormDetail> pokemonFormList = this.dataService.GetAllAltFormsOnlyComplete().Where(x => x.Form.Name.Contains("Mega")).ToList();

                        List<PokemonFormDetail> filteredFormList = new List<PokemonFormDetail>();

                        foreach (var p in allPokemon)
                        {
                            List<PokemonFormDetail> altForm = pokemonFormList.Where(x => x.OriginalPokemonId == p.Id).ToList();
                            foreach (var a in altForm)
                            {
                                a.AltFormPokemon.Name = string.Concat(a.AltFormPokemon.Name, " (", a.Form.Name, ")");
                            }

                            if (altForm.Count() > 0)
                            {
                                filteredFormList.AddRange(altForm);
                            }
                        }

                        if (filteredFormList.Count() > 0)
                        {
                            foreach (var p in filteredFormList)
                            {
                                altForms.Add(p.AltFormPokemon);
                            }
                        }
                    }

                    if (selectedForms.Contains("Alolan"))
                    {
                        List<PokemonFormDetail> pokemonFormList = this.dataService.GetAllAltFormsOnlyComplete().Where(x => x.Form.Name == "Alola").ToList();

                        List<PokemonFormDetail> filteredFormList = new List<PokemonFormDetail>();

                        foreach (var p in allPokemon)
                        {
                            List<PokemonFormDetail> altForm = pokemonFormList.Where(x => x.OriginalPokemonId == p.Id).ToList();
                            foreach (var a in altForm)
                            {
                                a.AltFormPokemon.Name = string.Concat(a.AltFormPokemon.Name, " (", a.Form.Name, ")");
                            }

                            if (altForm.Count() > 0)
                            {
                                filteredFormList.AddRange(altForm);
                            }
                        }

                        if (filteredFormList.Count() > 0)
                        {
                            foreach (var p in filteredFormList)
                            {
                                altForms.Add(p.AltFormPokemon);
                            }
                        }
                    }

                    if (selectedForms.Contains("Galarian"))
                    {
                        List<PokemonFormDetail> pokemonFormList = this.dataService.GetAllAltFormsOnlyComplete().Where(x => x.Form.Name == "Galar").ToList();

                        List<PokemonFormDetail> filteredFormList = new List<PokemonFormDetail>();

                        foreach (var p in allPokemon)
                        {
                            List<PokemonFormDetail> altForm = pokemonFormList.Where(x => x.OriginalPokemonId == p.Id).ToList();
                            foreach (var a in altForm)
                            {
                                a.AltFormPokemon.Name = string.Concat(a.AltFormPokemon.Name, " (", a.Form.Name, ")");
                            }

                            if (altForm.Count() > 0)
                            {
                                filteredFormList.AddRange(altForm);
                            }
                        }

                        if (filteredFormList.Count() > 0)
                        {
                            foreach (var p in filteredFormList)
                            {
                                altForms.Add(p.AltFormPokemon);
                            }
                        }
                    }

                    if (selectedForms.Contains("Gigantamax"))
                    {
                        List<PokemonFormDetail> pokemonFormList = this.dataService.GetAllAltFormsOnlyComplete().Where(x => x.Form.Name.Contains("Gigantamax")).ToList();

                        List<PokemonFormDetail> filteredFormList = new List<PokemonFormDetail>();

                        foreach (var p in allPokemon)
                        {
                            List<PokemonFormDetail> altForm = pokemonFormList.Where(x => x.OriginalPokemonId == p.Id).ToList();
                            foreach (var a in altForm)
                            {
                                a.AltFormPokemon.Name = string.Concat(a.AltFormPokemon.Name, " (", a.Form.Name, ")");
                            }

                            if (altForm.Count() > 0)
                            {
                                filteredFormList.AddRange(altForm);
                            }
                        }

                        if (filteredFormList.Count() > 0)
                        {
                            foreach (var p in filteredFormList)
                            {
                                altForms.Add(p.AltFormPokemon);
                            }
                        }
                    }

                    if (selectedForms.Contains("Other"))
                    {
                        List<PokemonFormDetail> pokemonFormList = this.dataService.GetAllAltFormsOnlyComplete();

                        List<Form> formsToRemove = this.dataService.GetForms().Where(x => x.Randomizable == false).ToList();

                        foreach (var f in formsToRemove)
                        {
                            pokemonFormList = pokemonFormList.Where(x => x.Form.Name != f.Name).ToList();
                        }

                        List<PokemonFormDetail> filteredFormList = new List<PokemonFormDetail>();

                        foreach (var p in allPokemon)
                        {
                            List<PokemonFormDetail> altForm = pokemonFormList.Where(x => x.OriginalPokemonId == p.Id).ToList();
                            foreach (var a in altForm)
                            {
                                a.AltFormPokemon.Name = string.Concat(a.AltFormPokemon.Name, " (", a.Form.Name, ")");
                            }

                            if (altForm.Count() > 0)
                            {
                                filteredFormList.AddRange(altForm);
                            }
                        }

                        if (filteredFormList.Count() > 0)
                        {
                            foreach (var p in filteredFormList)
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
                    List<Pokemon> altFormsList = this.dataService.GetAllAltForms().Select(x => x.AltFormPokemon).ToList();
                    allPokemon = allPokemon.Where(x => altFormsList.Any(y => y.Id == x.Id)).ToList();
                }

                if (selectedType != 0)
                {
                    List<PokemonTypeDetail> allPokemonTypes = this.dataService.GetAllPokemonWithTypesWithAltForms().Where(x => allPokemon.Any(y => y.Id == x.PokemonId)).ToList();
                    allPokemonTypes = allPokemonTypes.Where(x => x.PrimaryTypeId == selectedType || x.SecondaryTypeId == selectedType).ToList();
                    allPokemon = allPokemon.Where(x => allPokemonTypes.Select(x => x.Pokemon).Any(y => y.Id == x.Id)).ToList();
                }

                if (!multipleMegas)
                {
                    List<Pokemon> megaList = new List<Pokemon>();
                    List<PokemonFormDetail> altFormList = this.dataService.GetAllAltForms();
                    foreach (var p in altFormList.Where(x => x.Form.Name.Contains("Mega")).ToList())
                    {
                        if (allPokemon.Exists(x => x.Id == p.AltFormPokemonId))
                        {
                            megaList.Add(p.AltFormPokemon);
                        }
                    }

                    if (megaList.Count > 0)
                    {
                        Pokemon mega = megaList[rnd.Next(megaList.Count)];
                        foreach (var p in megaList.Where(x => x.Id != mega.Id))
                        {
                            if (allPokemon.Exists(x => x.Id == p.Id))
                            {
                                allPokemon.Remove(allPokemon.Find(x => x.Id == p.Id));
                            }
                        }
                    }
                }

                if (!multipleGMax)
                {
                    List<Pokemon> altFormList = this.dataService.GetAllAltForms().Where(x => x.Form.Name.Contains("Gigantamax")).Select(x => x.AltFormPokemon).ToList();
                    List<Pokemon> gigantamaxList = allPokemon.Where(x => altFormList.Any(y => y.Id == x.Id)).ToList();

                    if (gigantamaxList.Count > 0)
                    {
                        Pokemon gigantamax = gigantamaxList[rnd.Next(gigantamaxList.Count)];
                        foreach (var p in gigantamaxList.Where(x => x.Id != gigantamax.Id))
                        {
                            if (allPokemon.Exists(x => x.Id == p.Id))
                            {
                                allPokemon.Remove(allPokemon.Find(x => x.Id == p.Id));
                            }
                        }
                    }
                }

                if (selectedEvolutions == "stage1Pokemon")
                {
                    List<Pokemon> newPokemon = new List<Pokemon>();
                    foreach (var p in allPokemon)
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
                    foreach (var p in allPokemon)
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
                    foreach (var p in allPokemon)
                    {
                        if (!allEvolutions.Exists(x => x.PreevolutionPokemonId == p.Id))
                        {
                            newPokemon.Add(p);
                        }
                    }

                    allPokemon = newPokemon;
                }

                if (availablePokemon.Count() > 1)
                {
                    allPokemon = allPokemon.Where(x => availablePokemon.Any(y => y.PokemonId == x.Id)).ToList();
                }

                if (allPokemon.Count() > 0)
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
                            if (this.dataService.CheckIfAltForm(pokemon.Id))
                            {
                                originalPokemonId = this.dataService.GetOriginalPokemonByAltFormId(pokemon.Id).Id;
                            }
                            else
                            {
                                originalPokemonId = pokemon.Id;
                            }

                            List<Pokemon> altForms = this.dataService.GetAltForms(originalPokemonId);

                            if (this.dataService.CheckIfAltForm(pokemon.Id))
                            {
                                altForms.Remove(altForms.Find(x => x.Id == pokemon.Id));
                                altForms.Add(this.dataService.GetPokemonById(originalPokemonId));
                            }

                            foreach (var p in altForms)
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
                foreach (var p in pokemonList)
                {
                    model.AllPokemonOriginalNames.Add(this.dataService.GetPokemonById(p.Id));
                }

                int generationId = 0;

                if (selectedGame != 0)
                {
                    generationId = this.dataService.GetGenerationFromGame(selectedGame).Id;
                }

                model.PokemonURLs = new List<string>();
                foreach (var p in model.AllPokemonOriginalNames)
                {
                    if (generationId != 0)
                    {
                        model.PokemonURLs.Add(this.Url.Action("PokemonWithId", "Home", new { pokemonName = p.Name.Replace(": ", "_").Replace(' ', '_').ToLower(), pokemonId = p.Id, generationId = generationId }));
                    }
                    else
                    {
                        model.PokemonURLs.Add(this.Url.Action("PokemonWithId", "Home", new { pokemonName = p.Name.Replace(": ", "_").Replace(' ', '_').ToLower(), pokemonId = p.Id, generationId = this.dataService.GetAvailableGamesFromPokemonId(p.Id).Last().Id }));
                    }
                }

                if (randomAbility && selectedGame != 1 && selectedGame != 2)
                {
                    foreach (var p in model.AllPokemonOriginalNames)
                    {
                        PokemonAbilityDetail pokemonAbilities;
                        List<Ability> abilities = new List<Ability>();
                        if (generationId != 0)
                        {
                            pokemonAbilities = this.dataService.GetPokemonWithAbilities(p.Id).Where(x => x.GenerationId <= generationId).Last();
                        }
                        else
                        {
                            pokemonAbilities = this.dataService.GetPokemonWithAbilities(p.Id).Last();
                        }

                        abilities.Add(pokemonAbilities.PrimaryAbility);
                        if (pokemonAbilities.SecondaryAbility != null)
                        {
                            abilities.Add(pokemonAbilities.SecondaryAbility);
                        }

                        if (pokemonAbilities.HiddenAbility != null)
                        {
                            abilities.Add(pokemonAbilities.HiddenAbility);
                        }

                        if (pokemonAbilities.SpecialEventAbility != null)
                        {
                            abilities.Add(pokemonAbilities.SpecialEventAbility);
                        }

                        model.PokemonAbilities.Add(abilities[rnd.Next(abilities.Count)]);
                    }
                }

                List<int> pokemonIds = new List<int>();
                foreach (var p in model.AllPokemonOriginalNames)
                {
                    pokemonIds.Add(p.Id);
                }

                List<string> abilityNames = new List<string>();
                foreach (var a in model.PokemonAbilities)
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

        [AllowAnonymous]
        [Route("get-pokemon-abilities")]
        public List<Ability> GetPokemonAbilities(int pokemonId)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<Ability> pokemonAbilities = this.dataService.GetAbilitiesForPokemon(pokemonId);
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
        public string UpdateTypeChart(int typeId, int genId, List<int> resistances, List<int> weaknesses, List<int> immunities)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                TypeChart typeChart;
                List<int> duplicateIds = resistances.Where(x => weaknesses.Contains(x)).ToList();
                List<TypeChart> existingCharts = this.dataService.GetTypeChartByDefendType(typeId, genId);

                foreach (var t in duplicateIds)
                {
                    resistances.Remove(resistances.Find(x => x == t));
                    weaknesses.Remove(weaknesses.Find(x => x == t));
                }

                foreach (var t in immunities)
                {
                    if (resistances.IndexOf(t) != -1)
                    {
                        resistances.Remove(resistances.Find(x => x == t));
                    }

                    if (weaknesses.IndexOf(t) != -1)
                    {
                        weaknesses.Remove(weaknesses.Find(x => x == t));
                    }
                }

                foreach (var r in resistances)
                {
                    typeChart = new TypeChart()
                    {
                        AttackId = r,
                        DefendId = typeId,
                        Effective = 0.5m,
                        GenerationId = genId,
                    };
                    this.dataService.AddTypeChart(typeChart);
                }

                foreach (var w in weaknesses)
                {
                    typeChart = new TypeChart()
                    {
                        AttackId = w,
                        DefendId = typeId,
                        Effective = 2m,
                        GenerationId = genId,
                    };
                    this.dataService.AddTypeChart(typeChart);
                }

                foreach (var i in immunities)
                {
                    typeChart = new TypeChart()
                    {
                        AttackId = i,
                        DefendId = typeId,
                        Effective = 0m,
                        GenerationId = genId,
                    };
                    this.dataService.AddTypeChart(typeChart);
                }

                foreach (var t in existingCharts)
                {
                    this.dataService.DeleteTypeChart(t.Id);
                }

                return this.Json(this.Url.Action("Types", "Admin")).Value.ToString();
            }
            else
            {
                this.RedirectToAction("Home", "Index");
            }

            return null;
        }

        [AllowAnonymous]
        [Route("delete-pokemon-teams")]
        public string DeletePokemonTeams(List<int> teamIds)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<PokemonTeam> pokemonTeams = this.dataService.GetPokemonTeamsByUserId(this.dataService.GetUserWithUsername(this.User.Identity.Name).Id);
                pokemonTeams = pokemonTeams.Where(x => teamIds.Any(y => y == x.Id)).ToList();

                foreach (var t in pokemonTeams)
                {
                    this.dataService.DeletePokemonTeam(t.Id);
                }

                return this.Json(this.Url.Action("PokemonTeams", "User")).Value.ToString();
            }
            else
            {
                this.RedirectToAction("Home", "Index");
            }

            return null;
        }

        [AllowAnonymous]
        [Route("update-pokemon-game-availability")]
        public string UpdatePokemonGameAvailability(int pokemonId, List<int> games)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                PokemonGameDetail pokemonGameDetail;
                List<PokemonGameDetail> existingGameDetails = this.dataService.GetPokemonGameDetails(pokemonId);

                foreach (var g in games)
                {
                    pokemonGameDetail = new PokemonGameDetail()
                    {
                        PokemonId = pokemonId,
                        GameId = g,
                    };
                    this.dataService.AddPokemonGameDetail(pokemonGameDetail);
                }

                foreach (var g in existingGameDetails)
                {
                    this.dataService.DeletePokemonGameDetail(g.Id);
                }

                return this.Json(this.Url.Action("Pokemon", "Admin")).Value.ToString();
            }
            else
            {
                this.RedirectToAction("Home", "Index");
            }

            return null;
        }

        [AllowAnonymous]
        [Route("update-game-availability")]
        public string UpdateGameAvailability(int gameId, List<int> pokemonList)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                PokemonGameDetail pokemonGameDetail;
                List<PokemonGameDetail> existingGameDetails = this.dataService.GetPokemonGameDetailsByGame(gameId);

                foreach (var p in pokemonList)
                {
                    if (existingGameDetails.Find(x => x.PokemonId == p && x.GameId == gameId) == null)
                    {
                        pokemonGameDetail = new PokemonGameDetail()
                        {
                            GameId = gameId,
                            PokemonId = p,
                        };
                        this.dataService.AddPokemonGameDetail(pokemonGameDetail);
                    }
                    else
                    {
                        existingGameDetails.Remove(existingGameDetails.Find(x => x.PokemonId == p && x.GameId == gameId));
                    }
                }

                foreach (var g in existingGameDetails)
                {
                    this.dataService.DeletePokemonGameDetail(g.Id);
                }

                return this.Json(this.Url.Action("GameAvailability", "Home")).Value.ToString();
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
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<BattleItem> battleItems = new List<BattleItem>();
                List<FormItem> formItems = this.dataService.GetFormItems().Where(x => x.PokemonId == pokemonId).ToList();
                if (formItems.Count() == 0)
                {
                    Generation generation = this.dataService.GetGeneration(generationId);
                    List<BattleItem> allBattleItems = this.dataService.GetBattleItems();
                    battleItems.AddRange(allBattleItems.Where(x => !x.OnlyInThisGeneration && x.PokemonId == null).ToList());
                    if (generation != null)
                    {
                        battleItems = battleItems.Where(x => x.Generation.Id <= generation.Id).ToList();
                        if (allBattleItems.Where(x => x.OnlyInThisGeneration && x.GenerationId == generation.Id).ToList().Count() > 0)
                        {
                            battleItems.AddRange(allBattleItems.Where(x => x.OnlyInThisGeneration && x.GenerationId == generation.Id).ToList());
                        }

                        if (allBattleItems.Where(x => x.PokemonId == pokemonId && x.Generation.Id <= generation.Id).ToList().Count() > 0)
                        {
                            battleItems.AddRange(allBattleItems.Where(x => x.PokemonId == pokemonId && x.Generation.Id <= generation.Id).ToList());
                        }
                    }
                    else
                    {
                        if (allBattleItems.Where(x => x.PokemonId == pokemonId).ToList().Count() > 0)
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
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<string> genders = new List<string>();
                GenderRatio genderRatio = this.dataService.GetPokemonById(pokemonId).GenderRatio;
                if (genderRatio.MaleRatio == 0 && genderRatio.FemaleRatio == 0)
                {
                    genders.Add("None");
                }
                else if (genderRatio.MaleRatio == 0)
                {
                    genders.Add("Female");
                }
                else if (genderRatio.FemaleRatio == 0)
                {
                    genders.Add("Male");
                }
                else
                {
                    genders.Add(string.Empty);
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
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return this.PartialView("_FillTypeEffectiveness", this.dataService.GetTypeChartTyping(primaryTypeId, secondaryTypeId));
            }
            else
            {
                this.RedirectToAction("Home", "Index");
            }

            return null;
        }

        [AllowAnonymous]
        [Route("get-typing-evaluator-chart")]
        public IActionResult GetTypingEvaluatorChart(int primaryTypeId, int secondaryTypeId)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return this.PartialView("_FillTypeEvaluatorChart", this.dataService.GetTypeChartTyping(primaryTypeId, secondaryTypeId));
            }
            else
            {
                this.RedirectToAction("Home", "Index");
            }

            return null;
        }

        [AllowAnonymous]
        [Route("get-generations")]
        public TeamRandomizerListViewModel GetGenerations(int selectedGame)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                if (selectedGame != 0)
                {
                    List<Pokemon> altFormsList = this.dataService.GetAllAltForms().Select(x => x.AltFormPokemon).ToList();
                    List<PokemonGameDetail> availablePokemon = this.dataService.GetPokemonGameDetailsByGame(selectedGame).Where(x => !altFormsList.Any(y => y.Id == x.PokemonId)).ToList();
                    List<Pokemon> allPokemon = this.dataService.GetAllPokemon().Where(x => availablePokemon.Any(y => y.PokemonId == x.Id)).ToList();
                    Generation selectedGen = this.dataService.GetGenerationFromGame(selectedGame);
                    List<Generation> generationList = this.dataService.GetGenerations().Where(x => x.Id <= selectedGen.Id).ToList();
                    List<DataAccess.Models.Type> typesList = this.dataService.GetTypes();
                    List<LegendaryType> legendaryTypes = this.dataService.GetLegendaryTypes();
                    List<Generation> availableGenerations = new List<Generation>();
                    Game game = this.dataService.GetGame(selectedGame);

                    foreach (var gen in generationList)
                    {
                        if (allPokemon.Where(x => x.Game.GenerationId == gen.Id).ToList().Count() != 0)
                        {
                            availableGenerations.Add(gen);
                        }
                    }

                    if (selectedGame == 1)
                    {
                        typesList.Remove(typesList.Find(x => x.Name == "Dark"));
                        typesList.Remove(typesList.Find(x => x.Name == "Steel"));
                    }

                    if (game.ReleaseDate < new DateTime(2013, 10, 12))
                    {
                        typesList.Remove(typesList.Find(x => x.Name == "Fairy"));
                    }

                    if (game.ReleaseDate < new DateTime(2016, 11, 18))
                    {
                        legendaryTypes.Remove(legendaryTypes.Find(x => x.Type == "Ultra Beast"));
                    }

                    TeamRandomizerListViewModel model = new TeamRandomizerListViewModel()
                    {
                        AllGenerations = availableGenerations,
                        AllTypes = typesList,
                        AllLegendaryTypes = legendaryTypes,
                    };

                    return model;
                }
                else
                {
                    TeamRandomizerListViewModel model = new TeamRandomizerListViewModel()
                    {
                        AllGenerations = this.dataService.GetGenerations().Where(x => this.dataService.GetAllPokemonWithoutForms().Any(y => y.Game.GenerationId == x.Id)).ToList(),
                        AllTypes = this.dataService.GetTypes(),
                        AllLegendaryTypes = this.dataService.GetLegendaryTypes(),
                    };

                    return model;
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
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<PokemonTypeDetail> typingList = this.dataService.GetAllPokemonWithSpecificTypes(primaryTypeId, secondaryTypeId);
                List<Pokemon> pokemonList = new List<Pokemon>();

                foreach (var p in typingList)
                {
                    if (this.dataService.CheckIfAltForm(p.PokemonId))
                    {
                        Pokemon pokemon = this.dataService.GetAltFormWithFormName(p.PokemonId);
                        pokemonList.Add(pokemon);
                    }
                    else
                    {
                        pokemonList.Add(p.Pokemon);
                    }
                }

                TypingEvaluatorViewModel model = new TypingEvaluatorViewModel()
                {
                    AllPokemonWithTypes = typingList,
                    AllPokemon = pokemonList,
                    AllAltForms = this.dataService.GetAllAltForms().Select(x => x.AltFormPokemon).ToList(),
                    AppConfig = this.appConfig,
                    GenerationId = this.dataService.GetGenerations().Last().Id,
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
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                PokemonEggGroupDetail searchedEggGroupDetails = this.dataService.GetPokemonWithEggGroups(pokemonId).Last();
                GenderRatio genderRatio = this.dataService.GetGenderRatio(searchedEggGroupDetails.Pokemon.GenderRatioId);
                List<Pokemon> altFormsList = this.dataService.GetAllAltForms().Select(x => x.AltFormPokemon).ToList();
                List<PokemonEggGroupDetail> eggGroupList = new List<PokemonEggGroupDetail>();
                List<Pokemon> pokemonList = new List<Pokemon>();
                List<Pokemon> originalPokemon = new List<Pokemon>();

                if (pokemonId == this.dataService.GetPokemon("Manaphy").Id || pokemonId == this.dataService.GetPokemon("Phione").Id || (genderRatio.MaleRatio == 0 && genderRatio.FemaleRatio == 0 && pokemonId != this.dataService.GetPokemon("Ditto").Id))
                {
                    eggGroupList.Add(this.dataService.GetPokemonWithEggGroupsFromPokemonName("Ditto"));
                    pokemonList.Add(this.dataService.GetPokemon("Ditto"));
                }
                else if (pokemonId == this.dataService.GetPokemon("Ditto").Id)
                {
                    Pokemon pokemon;
                    eggGroupList = this.dataService.GetAllPokemonWithEggGroupsAndIncomplete();
                    List<PokemonEggGroupDetail> breedablePokemonList = this.dataService.GetAllBreedablePokemon();
                    eggGroupList = eggGroupList.Where(x => breedablePokemonList.Any(y => y.PokemonId == x.PokemonId)).OrderBy(x => x.Pokemon.PokedexNumber).ToList();
                    eggGroupList.Remove(eggGroupList.Find(x => x.PokemonId == pokemonId));

                    foreach (var p in eggGroupList)
                    {
                        if (altFormsList.Find(x => x.Id == p.PokemonId) != null)
                        {
                            pokemon = this.dataService.GetAltFormWithFormName(p.PokemonId);
                        }

                        pokemonList.Add(p.Pokemon);
                    }

                    pokemon = this.dataService.GetPokemonById(pokemonId);
                }
                else
                {
                    eggGroupList = this.dataService.GetAllPokemonWithSpecificEggGroups((int)searchedEggGroupDetails.PrimaryEggGroupId, searchedEggGroupDetails.SecondaryEggGroupId);
                    List<PokemonEggGroupDetail> breedablePokemonList = this.dataService.GetAllBreedablePokemon();
                    eggGroupList.Add(this.dataService.GetPokemonWithEggGroupsFromPokemonName("Ditto"));
                    if (eggGroupList.Where(x => x.Pokemon.Name == "Manaphy").Count() > 0)
                    {
                        eggGroupList.Remove(eggGroupList.Find(x => x.Pokemon.Name == "Manaphy"));
                    }

                    if (eggGroupList.Where(x => x.Pokemon.Name == "Phione").Count() > 0)
                    {
                        eggGroupList.Remove(eggGroupList.Find(x => x.Pokemon.Name == "Phione"));
                    }

                    eggGroupList = eggGroupList.Where(x => breedablePokemonList.Any(y => y.PokemonId == x.PokemonId)).OrderBy(x => x.Pokemon.PokedexNumber).ToList();
                    originalPokemon = eggGroupList.Select(x => x.Pokemon).ToList();
                    List<PokemonEggGroupDetail> finalEggGroupList = new List<PokemonEggGroupDetail>(eggGroupList);

                    foreach (var p in eggGroupList)
                    {
                        if ((p.Pokemon.GenderRatio.MaleRatio == 0 && p.Pokemon.GenderRatio.FemaleRatio == 0) || (genderRatio.MaleRatio == 100 && p.Pokemon.GenderRatio.MaleRatio == 100) || (genderRatio.FemaleRatio == 100 && p.Pokemon.GenderRatio.FemaleRatio == 100))
                        {
                            finalEggGroupList.Remove(p);
                        }
                        else
                        {
                            if (altFormsList.Find(x => x.Id == p.PokemonId) != null)
                            {
                                p.Pokemon = this.dataService.GetAltFormWithFormName(p.PokemonId);
                            }

                            pokemonList.Add(p.Pokemon);
                        }
                    }

                    eggGroupList = finalEggGroupList;
                }

                if (originalPokemon == null)
                {
                    originalPokemon = eggGroupList.Select(x => x.Pokemon).ToList();
                }

                originalPokemon.RemoveAll(x => !eggGroupList.Select(y => y.PokemonId).Contains(x.Id));

                List<EggGroup> pokemonEggGroupList = new List<EggGroup>
                {
                    searchedEggGroupDetails.PrimaryEggGroup,
                };

                if (searchedEggGroupDetails.SecondaryEggGroup != null)
                {
                    pokemonEggGroupList.Add(searchedEggGroupDetails.SecondaryEggGroup);
                }

                EggGroupEvaluatorViewModel model = new EggGroupEvaluatorViewModel()
                {
                    AllPokemonWithEggGroups = eggGroupList,
                    AllPokemon = pokemonList,
                    AllAltForms = altFormsList,
                    AllOriginalPokemon = originalPokemon.ToList(),
                    AppConfig = this.appConfig,
                    SearchedPokemon = this.dataService.GetPokemonById(pokemonId),
                    PokemonEggGroups = pokemonEggGroupList,
                    GenerationId = this.dataService.GetGenerations().Last().Id,
                };

                return this.PartialView("_FillDayCareEvaluator", model);
            }
            else
            {
                this.RedirectToAction("Home", "Index");
            }

            return null;
        }

        private string FillEVs(PokemonTeamEV evs)
        {
            string evString = string.Empty;
            if (evs.EVTotal == 0)
            {
                evString = "\nEVs: 1 HP / 1 Atk / 1 Def / 1 SpA / 1 SpD / 1 Spe";
            }
            else
            {
                if (evs.Health > 0)
                {
                    evString = string.Concat(evString, evs.Health.ToString(), " HP");
                }

                if (evs.Attack > 0)
                {
                    if (!string.IsNullOrEmpty(evString))
                    {
                        evString = string.Concat(evString, " / ");
                    }

                    evString = string.Concat(evString, evs.Attack.ToString(), " Atk");
                }

                if (evs.Defense > 0)
                {
                    if (!string.IsNullOrEmpty(evString))
                    {
                        evString = string.Concat(evString, " / ");
                    }

                    evString = string.Concat(evString, evs.Defense.ToString(), " Def");
                }

                if (evs.SpecialAttack > 0)
                {
                    if (!string.IsNullOrEmpty(evString))
                    {
                        evString = string.Concat(evString, " / ");
                    }

                    evString = string.Concat(evString, evs.SpecialAttack.ToString(), " SpA");
                }

                if (evs.SpecialDefense > 0)
                {
                    if (!string.IsNullOrEmpty(evString))
                    {
                        evString = string.Concat(evString, " / ");
                    }

                    evString = string.Concat(evString, evs.SpecialDefense.ToString(), " SpD");
                }

                if (evs.Speed > 0)
                {
                    if (!string.IsNullOrEmpty(evString))
                    {
                        evString = string.Concat(evString, " / ");
                    }

                    evString = string.Concat(evString, evs.Speed.ToString(), " Spe");
                }

                evString = string.Concat("\nEVs: ", evString);
            }

            return evString;
        }

        private string FillIVs(PokemonTeamIV ivs)
        {
            string ivString = string.Empty;
            if (ivs.Health < 31)
            {
                ivString = string.Concat(ivString, ivs.Health.ToString(), " HP");
            }

            if (ivs.Attack < 31)
            {
                if (!string.IsNullOrEmpty(ivString))
                {
                    ivString = string.Concat(ivString, " / ");
                }

                ivString = string.Concat(ivString, ivs.Attack.ToString(), " Atk");
            }

            if (ivs.Defense < 31)
            {
                if (!string.IsNullOrEmpty(ivString))
                {
                    ivString = string.Concat(ivString, " / ");
                }

                ivString = string.Concat(ivString, ivs.Defense.ToString(), " Def");
            }

            if (ivs.SpecialAttack < 31)
            {
                if (!string.IsNullOrEmpty(ivString))
                {
                    ivString = string.Concat(ivString, " / ");
                }

                ivString = string.Concat(ivString, ivs.SpecialAttack.ToString(), " SpA");
            }

            if (ivs.SpecialDefense < 31)
            {
                if (!string.IsNullOrEmpty(ivString))
                {
                    ivString = string.Concat(ivString, " / ");
                }

                ivString = string.Concat(ivString, ivs.SpecialDefense.ToString(), " SpD");
            }

            if (ivs.Speed < 31)
            {
                if (!string.IsNullOrEmpty(ivString))
                {
                    ivString = string.Concat(ivString, " / ");
                }

                ivString = string.Concat(ivString, ivs.Speed.ToString(), " Spe");
            }

            if (!string.IsNullOrEmpty(ivString))
            {
                ivString = string.Concat("\nIVs: ", ivString);
            }

            return ivString;
        }

        private string FillMoveset(PokemonTeamMoveset moveset)
        {
            string movesetString = string.Empty;
            if (!string.IsNullOrEmpty(moveset.FirstMove))
            {
                movesetString = string.Concat(movesetString, "\n- ", moveset.FirstMove);
            }

            if (!string.IsNullOrEmpty(moveset.SecondMove))
            {
                movesetString = string.Concat(movesetString, "\n- ", moveset.SecondMove);
            }

            if (!string.IsNullOrEmpty(moveset.ThirdMove))
            {
                movesetString = string.Concat(movesetString, "\n- ", moveset.ThirdMove);
            }

            if (!string.IsNullOrEmpty(moveset.FourthMove))
            {
                movesetString = string.Concat(movesetString, "\n- ", moveset.FourthMove);
            }

            return movesetString;
        }

        private List<string> GetUserFormDetails(int pokemonId)
        {
            string form = string.Empty, itemName = string.Empty;
            List<string> formDetails = new List<string>();
            PokemonFormDetail pokemonFormDetail;

            pokemonFormDetail = this.dataService.GetPokemonFormDetailByAltFormId(pokemonId);

            form = string.Concat(form, pokemonFormDetail.Form.Name.Replace(' ', '-').Replace("Gigantamax", "Gmax"));

            FormItem formItem = this.dataService.GetFormItemByPokemonId(pokemonId);
            if (formItem != null)
            {
                itemName = string.Concat(" @ ", formItem.Name);
            }

            formDetails.Add(form);
            if (!string.IsNullOrEmpty(itemName))
            {
                formDetails.Add(itemName);
            }

            return formDetails;
        }

        private string ExportPokemonTeam(List<int> pokemonIdList, List<string> abilityList, bool exportAbilities)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                string pokemonTeam = string.Empty;
                Pokemon pokemon;

                for (var i = 0; i < pokemonIdList.Count; i++)
                {
                    if (i != 0)
                    {
                        pokemonTeam = string.Concat(pokemonTeam, "\n\n");
                    }

                    pokemon = this.dataService.GetPokemonById(pokemonIdList[i]);
                    string pokemonName = pokemon.Name;
                    if (this.dataService.CheckIfAltForm(pokemon.Id))
                    {
                        string pokemonForm = this.GetFormDetails(pokemon.Id);
                        pokemonName = string.Concat(pokemonName, "-", pokemonForm);
                    }

                    pokemonTeam = string.Concat(pokemonTeam, pokemonName);
                    if (exportAbilities)
                    {
                        pokemonTeam = string.Concat(pokemonTeam, "\nAbility: ", abilityList[i]);
                    }

                    pokemonTeam = string.Concat(pokemonTeam, "\nEVs: 1 HP / 1 Atk / 1 Def / 1 SpA / 1 SpD / 1 Spe");
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

            pokemonFormDetail = this.dataService.GetPokemonFormDetailByAltFormId(pokemonId);

            formDetails = string.Concat(formDetails, pokemonFormDetail.Form.Name.Replace(' ', '-').Replace("Gigantamax", "Gmax"));

            FormItem formItem = this.dataService.GetFormItemByPokemonId(pokemonId);
            if (formItem != null)
            {
                itemName = formItem.Name;
            }

            if (!string.IsNullOrEmpty(itemName))
            {
                formDetails = string.Concat(formDetails, " @ ", itemName);
            }

            return formDetails;
        }

        private List<Pokemon> RemoveExtraPokemonForms(List<Pokemon> pokemonList)
        {
            Random rnd = new Random();
            List<Pokemon> pumpkabooCount = pokemonList.Where(x => x.PokedexNumber == 710).ToList();
            while (pumpkabooCount.Count() > 1)
            {
                pokemonList.Remove(pumpkabooCount[rnd.Next(pumpkabooCount.Count)]);
                pumpkabooCount = pokemonList.Where(x => x.PokedexNumber == 710).ToList();
            }

            List<Pokemon> gourgeistCount = pokemonList.Where(x => x.PokedexNumber == 711).ToList();
            while (gourgeistCount.Count() > 1)
            {
                pokemonList.Remove(gourgeistCount[rnd.Next(gourgeistCount.Count)]);
                gourgeistCount = pokemonList.Where(x => x.PokedexNumber == 711).ToList();
            }

            List<Pokemon> starterForms = this.dataService.GetPokemonFormDetailsByFormName("Starter").Select(x => x.AltFormPokemon).ToList();
            List<Pokemon> lgpeStarterCount = pokemonList.Where(x => starterForms.Any(y => y.Id == x.Id)).ToList();
            while (lgpeStarterCount.Count() > 1)
            {
                pokemonList.Remove(lgpeStarterCount[rnd.Next(lgpeStarterCount.Count)]);
                lgpeStarterCount = pokemonList.Where(x => starterForms.Any(y => y.Id == x.Id)).ToList();
            }

            return pokemonList;
        }
    }
}
