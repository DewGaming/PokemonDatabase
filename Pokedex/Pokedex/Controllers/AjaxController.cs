using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Pokedex.DataAccess.Models;
using Pokedex.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace Pokedex.Controllers
{
    /// <summary>
    /// The class that handles all AJAX requests.
    /// </summary>
    [Route("")]
    public class AjaxController : Controller
    {
        private readonly DataService dataService;

        private readonly AppConfig appConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="AjaxController"/> class.
        /// </summary>
        /// <param name="appConfig">The application's config.</param>
        /// <param name="dataContext">The database's context.</param>
        public AjaxController(IOptions<AppConfig> appConfig, DataContext dataContext)
        {
            this.appConfig = appConfig.Value;
            this.dataService = new DataService(dataContext);
        }

        /// <summary>
        /// Marks a message as read.
        /// </summary>
        [Route("mark-as-read")]
        public void MarkAsRead()
        {
            List<Message> messages = this.dataService.GetObjects<Message>(includes: "Receiver", whereProperty: "Receiver.Username", wherePropertyValue: this.User.Identity.Name);
            foreach (var m in messages.Where(x => !x.IsRead))
            {
                m.IsRead = true;
                this.dataService.UpdateObject(m);
            }
        }

        /// <summary>
        /// Checks to see if there are any unread messages for the user.
        /// </summary>
        /// <returns>The amount of unread messages.</returns>
        [Route("check-unread-messages")]
        public List<int> CheckUnreadMessages()
        {
            List<Message> messages = this.dataService.GetObjects<Message>(includes: "Receiver", whereProperty: "Receiver.Username", wherePropertyValue: this.User.Identity.Name).ToList();
            List<int> counts = new List<int>()
            {
                messages.Where(x => !x.IsSeen).Count(),
                messages.Where(x => !x.IsRead).Count(),
            };

            foreach (var m in messages.Where(x => !x.IsSeen).ToList())
            {
                m.IsSeen = true;
                this.dataService.UpdateObject(m);
            }

            return counts;
        }

        /// <summary>
        /// Updates the user's last visit time.
        /// </summary>
        [Route("update-last-visit")]
        public void UpdateLastVisit()
        {
            User user = this.dataService.GetObjectByPropertyValue<User>("Username", this.User.Identity.Name);
            user.LastVisit = DateTime.Now.ToUniversalTime();
            this.dataService.UpdateObject(user);
        }

        /// <summary>
        /// Prepares the required information for the admin to view the admin pokemon page.
        /// </summary>
        /// <param name="generationId">The generation requested by the admin.</param>
        /// <returns>The fill admin generation table shared view.</returns>
        [Route("get-pokemon-by-generation-admin/{generationId}")]
        public IActionResult GetPokemonByGenerationAdmin(int generationId)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<Pokemon> pokemonList = this.dataService.GetObjects<Pokemon>("PokedexNumber, Id", "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth");
                List<Pokemon> altFormList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form").ConvertAll(x => x.AltFormPokemon);

                AllAdminPokemonViewModel allAdminPokemon = this.dataService.GetAllAdminPokemonDetails();

                DropdownViewModel dropdownViewModel = new DropdownViewModel()
                {
                    AllPokemon = allAdminPokemon,
                    AppConfig = this.appConfig,
                    GenerationId = generationId,
                };

                AdminGenerationTableViewModel model = new AdminGenerationTableViewModel()
                {
                    AltFormList = altFormList,
                    PokemonList = pokemonList.Where(x => !altFormList.Any(y => y.Id == x.Id)).Where(x => x.Game.GenerationId == generationId).ToList(),
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

        /// <summary>
        /// Prepares the required information for the admin to view all incomplete pokemon.
        /// </summary>
        /// <returns>The fill admin generation table shared view.</returns>
        [Route("get-incomplete-pokemon-admin")]
        public IActionResult GetIncompletePokemonAdmin()
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                AllAdminPokemonViewModel allAdminPokemon = this.dataService.GetAllAdminPokemonDetails();

                DropdownViewModel dropdownViewModel = new DropdownViewModel()
                {
                    AllPokemon = allAdminPokemon,
                    AppConfig = this.appConfig,
                    GenerationId = this.dataService.GetObjects<Generation>().Last().Id,
                };

                AdminGenerationTableViewModel model = new AdminGenerationTableViewModel()
                {
                    PokemonList = this.dataService.GetObjects<Pokemon>("PokedexNumber, Id", "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth").Where(x => !x.IsComplete).ToList(),
                    AltFormList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon.EggCycle, AltFormPokemon.GenderRatio, AltFormPokemon.Classification, AltFormPokemon.Game, AltFormPokemon.Game.Generation, AltFormPokemon.ExperienceGrowth").ConvertAll(x => x.AltFormPokemon).Where(x => !x.IsComplete).ToList(),
                    DropdownViewModel = dropdownViewModel,
                    AppConfig = this.appConfig,
                };

                foreach (var a in model.AltFormList)
                {
                    a.Name = string.Concat(a.Name, " (", this.dataService.GetPokemonFormName(a.Id), ")");
                }

                return this.PartialView("_FillAdminGenerationTable", model);
            }
            else
            {
                return this.RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Prepares the required information for the admin to view all incomplete pokemon.
        /// </summary>
        /// <returns>The fill admin generation table shared view.</returns>
        [Route("get-missing-threed-pokemon-admin")]
        public IActionResult GetMissingThreeDPokemonAdmin()
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                HttpWebRequest webRequest;
                HttpWebResponse imageRequest;
                AllAdminPokemonViewModel allAdminPokemon = this.dataService.GetAllAdminPokemonDetails();
                List<Pokemon> pokemonList = this.dataService.GetObjects<Pokemon>("PokedexNumber, Id", "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth");
                List<Pokemon> altFormList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon.EggCycle, AltFormPokemon.GenderRatio, AltFormPokemon.Classification, AltFormPokemon.Game, AltFormPokemon.Game.Generation, AltFormPokemon.ExperienceGrowth").ConvertAll(x => x.AltFormPokemon);
                List<int> pokemonIds = pokemonList.ConvertAll(x => x.Id);

                DropdownViewModel dropdownViewModel = new DropdownViewModel()
                {
                    AllPokemon = allAdminPokemon,
                    AppConfig = this.appConfig,
                    GenerationId = this.dataService.GetObjects<Generation>().Last().Id,
                };

                AdminGenerationTableViewModel model = new AdminGenerationTableViewModel()
                {
                    PokemonList = new List<Pokemon>(),
                    AltFormList = new List<Pokemon>(),
                    DropdownViewModel = dropdownViewModel,
                    AppConfig = this.appConfig,
                };

                foreach (var id in pokemonIds)
                {
                    try
                    {
                        webRequest = (HttpWebRequest)HttpWebRequest.Create(string.Concat(this.appConfig.WebUrl, this.appConfig.HomePokemonImageUrl, id, ".png"));
                        imageRequest = (HttpWebResponse)webRequest.GetResponse();
                        if (imageRequest.StatusCode != HttpStatusCode.OK)
                        {
                            if (altFormList.Exists(x => x.Id == id))
                            {
                                model.AltFormList.Add(altFormList.Find(x => x.Id == id));
                            }
                            else
                            {
                                model.PokemonList.Add(pokemonList.Find(x => x.Id == id));
                            }
                        }
                    }
                    catch
                    {
                        if (altFormList.Exists(x => x.Id == id))
                        {
                            model.AltFormList.Add(altFormList.Find(x => x.Id == id));
                        }
                        else
                        {
                            model.PokemonList.Add(pokemonList.Find(x => x.Id == id));
                        }
                    }
                }

                foreach (var a in model.AltFormList)
                {
                    a.Name = string.Concat(a.Name, " (", this.dataService.GetPokemonFormName(a.Id), ")");
                }

                model.PokemonList.AddRange(model.AltFormList);

                return this.PartialView("_FillAdminGenerationTable", model);
            }
            else
            {
                return this.RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Prepares the required information for the admin to view all pokemon missing shiny artwork.
        /// </summary>
        /// <returns>The fill admin generation table shared view.</returns>
        [Route("get-missing-shiny-pokemon-admin")]
        public IActionResult GetMissingShinyPokemonAdmin()
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                HttpWebRequest webRequest;
                HttpWebResponse imageRequest;
                AllAdminPokemonViewModel allAdminPokemon = this.dataService.GetAllAdminPokemonDetails();
                List<Pokemon> pokemonList = this.dataService.GetObjects<Pokemon>("PokedexNumber, Id", "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth");
                List<Pokemon> altFormList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon.EggCycle, AltFormPokemon.GenderRatio, AltFormPokemon.Classification, AltFormPokemon.Game, AltFormPokemon.Game.Generation, AltFormPokemon.ExperienceGrowth").ConvertAll(x => x.AltFormPokemon);
                List<int> pokemonIds = pokemonList.ConvertAll(x => x.Id);

                DropdownViewModel dropdownViewModel = new DropdownViewModel()
                {
                    AllPokemon = allAdminPokemon,
                    AppConfig = this.appConfig,
                    GenerationId = this.dataService.GetObjects<Generation>().Last().Id,
                };

                AdminGenerationTableViewModel model = new AdminGenerationTableViewModel()
                {
                    PokemonList = new List<Pokemon>(),
                    AltFormList = new List<Pokemon>(),
                    DropdownViewModel = dropdownViewModel,
                    AppConfig = this.appConfig,
                };

                foreach (var id in pokemonIds)
                {
                    try
                    {
                        webRequest = (HttpWebRequest)HttpWebRequest.Create(string.Concat(this.appConfig.WebUrl, this.appConfig.ShinyPokemonImageUrl, id, ".png"));
                        imageRequest = (HttpWebResponse)webRequest.GetResponse();
                        if (imageRequest.StatusCode != HttpStatusCode.OK)
                        {
                            if (altFormList.Exists(x => x.Id == id))
                            {
                                model.AltFormList.Add(altFormList.Find(x => x.Id == id));
                            }
                            else
                            {
                                model.PokemonList.Add(pokemonList.Find(x => x.Id == id));
                            }
                        }
                    }
                    catch
                    {
                        if (altFormList.Exists(x => x.Id == id))
                        {
                            model.AltFormList.Add(altFormList.Find(x => x.Id == id));
                        }
                        else
                        {
                            model.PokemonList.Add(pokemonList.Find(x => x.Id == id));
                        }
                    }
                }

                foreach (var a in model.AltFormList)
                {
                    a.Name = string.Concat(a.Name, " (", this.dataService.GetPokemonFormName(a.Id), ")");
                }

                model.PokemonList.AddRange(model.AltFormList);

                return this.PartialView("_FillAdminGenerationTable", model);
            }
            else
            {
                return this.RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Exports a user's team into a format that can be imported into pokemon showdown.
        /// </summary>
        /// <param name="teamId">The team being exported.</param>
        /// <returns>A list of all the pokemon exported to be copied to clipboard.</returns>
        [Route("grab-user-pokemon-team")]
        public ExportPokemonViewModel ExportUserPokemonTeam(int teamId)
        {
            PokemonTeam pokemonTeam = this.dataService.GetObjectByPropertyValue<PokemonTeam>("Id", teamId, "Game, FirstPokemon, FirstPokemon.Pokemon, FirstPokemon.Pokemon.Game, FirstPokemon.Ability, FirstPokemon.PokemonTeamEV, FirstPokemon.PokemonTeamIV, FirstPokemon.PokemonTeamMoveset, FirstPokemon.BattleItem, FirstPokemon.Nature, FirstPokemon.TeraType, SecondPokemon, SecondPokemon.Pokemon, SecondPokemon.Pokemon.Game, SecondPokemon.Ability, SecondPokemon.PokemonTeamEV, SecondPokemon.PokemonTeamIV, SecondPokemon.PokemonTeamMoveset, SecondPokemon.BattleItem, SecondPokemon.Nature, SecondPokemon.TeraType, ThirdPokemon, ThirdPokemon.Pokemon, ThirdPokemon.Pokemon.Game, ThirdPokemon.Ability, ThirdPokemon.PokemonTeamEV, ThirdPokemon.PokemonTeamIV, ThirdPokemon.PokemonTeamMoveset, ThirdPokemon.BattleItem, ThirdPokemon.Nature, ThirdPokemon.TeraType, FourthPokemon, FourthPokemon.Pokemon, FourthPokemon.Pokemon.Game, FourthPokemon.Ability, FourthPokemon.PokemonTeamEV, FourthPokemon.PokemonTeamIV, FourthPokemon.PokemonTeamMoveset, FourthPokemon.BattleItem, FourthPokemon.Nature, FourthPokemon.TeraType, FifthPokemon, FifthPokemon.Pokemon, FifthPokemon.Pokemon.Game, FifthPokemon.Ability, FifthPokemon.PokemonTeamEV, FifthPokemon.PokemonTeamIV, FifthPokemon.PokemonTeamMoveset, FifthPokemon.BattleItem, FifthPokemon.Nature, FifthPokemon.TeraType, SixthPokemon, SixthPokemon.Pokemon, SixthPokemon.Pokemon.Game, SixthPokemon.Ability, SixthPokemon.PokemonTeamEV, SixthPokemon.PokemonTeamIV, SixthPokemon.PokemonTeamMoveset, SixthPokemon.BattleItem, SixthPokemon.Nature, SixthPokemon.TeraType, User");
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest" && (pokemonTeam.User.Username == this.User.Identity.Name || this.User.IsInRole("Owner")))
            {
                return this.ExportTeamString(pokemonTeam);
            }

            return null;
        }

        /// <summary>
        /// Exports all of a user's teams into a format that can be imported into pokemon showdown.
        /// </summary>
        /// <returns>A list of all the pokemon exported to be copied to clipboard.</returns>
        [Route("grab-all-user-pokemon-teams")]
        public List<ExportPokemonViewModel> ExportAllUserPokemonTeams()
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<PokemonTeam> pokemonTeams = this.dataService.GetObjects<PokemonTeam>("Id", "Game, FirstPokemon, FirstPokemon.Pokemon, FirstPokemon.Pokemon.Game, FirstPokemon.Ability, FirstPokemon.PokemonTeamEV, FirstPokemon.PokemonTeamIV, FirstPokemon.PokemonTeamMoveset, FirstPokemon.BattleItem, FirstPokemon.Nature, FirstPokemon.TeraType, SecondPokemon, SecondPokemon.Pokemon, SecondPokemon.Pokemon.Game, SecondPokemon.Ability, SecondPokemon.PokemonTeamEV, SecondPokemon.PokemonTeamIV, SecondPokemon.PokemonTeamMoveset, SecondPokemon.BattleItem, SecondPokemon.Nature, SecondPokemon.TeraType, ThirdPokemon, ThirdPokemon.Pokemon, ThirdPokemon.Pokemon.Game, ThirdPokemon.Ability, ThirdPokemon.PokemonTeamEV, ThirdPokemon.PokemonTeamIV, ThirdPokemon.PokemonTeamMoveset, ThirdPokemon.BattleItem, ThirdPokemon.Nature, ThirdPokemon.TeraType, FourthPokemon, FourthPokemon.Pokemon, FourthPokemon.Pokemon.Game, FourthPokemon.Ability, FourthPokemon.PokemonTeamEV, FourthPokemon.PokemonTeamIV, FourthPokemon.PokemonTeamMoveset, FourthPokemon.BattleItem, FourthPokemon.Nature, FourthPokemon.TeraType, FifthPokemon, FifthPokemon.Pokemon, FifthPokemon.Pokemon.Game, FifthPokemon.Ability, FifthPokemon.PokemonTeamEV, FifthPokemon.PokemonTeamIV, FifthPokemon.PokemonTeamMoveset, FifthPokemon.BattleItem, FifthPokemon.Nature, FifthPokemon.TeraType, SixthPokemon, SixthPokemon.Pokemon, SixthPokemon.Pokemon.Game, SixthPokemon.Ability, SixthPokemon.PokemonTeamEV, SixthPokemon.PokemonTeamIV, SixthPokemon.PokemonTeamMoveset, SixthPokemon.BattleItem, SixthPokemon.Nature, SixthPokemon.TeraType, User", "User.Username", this.User.Identity.Name);
                List<ExportPokemonViewModel> exportList = new List<ExportPokemonViewModel>();
                foreach (var team in pokemonTeams)
                {
                    exportList.Add(this.ExportTeamString(team));
                }

                return exportList;
            }

            return null;
        }

        /// <summary>
        /// Saves a team that was generated from the team randomizer.
        /// </summary>
        /// <param name="selectedGame">The game selected in the randomizer.</param>
        /// <param name="pokemonIdList">The list of ids of the pokemon generated.</param>
        /// <param name="abilityIdList">The list of abilities generated alongside the pokemon.</param>
        /// <param name="exportAbilities">Check to see if generated abilities are to be exported.</param>
        /// <param name="pokemonTeamName">The given pokemon team name. Defaults to "Save from Team Randomizer" if no name given.</param>
        /// <returns>The string confirming the team has been generated. Tells a user they need to be logged in if they aren't.</returns>
        [AllowAnonymous]
        [Route("save-pokemon-team")]
        public string SavePokemonTeam(int selectedGame, List<int> pokemonIdList, List<int> abilityIdList, bool exportAbilities, string pokemonTeamName)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                if (string.IsNullOrEmpty(pokemonTeamName))
                {
                    pokemonTeamName = "Save from Team Randomizer";
                }

                if (this.dataService.GetObjectByPropertyValue<User>("Username", this.User.Identity.Name) != null)
                {
                    this.dataService.AddPageView("Save Pokemon Team from Team Randomizer", this.User.IsInRole("Owner"));
                    PokemonTeam pokemonTeam = new PokemonTeam()
                    {
                        PokemonTeamName = pokemonTeamName,
                        UserId = this.dataService.GetObjectByPropertyValue<User>("Username", this.User.Identity.Name).Id,
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
                        pokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", pokemonIdList[i], "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth");

                        if (exportAbilities)
                        {
                            ability = this.dataService.GetObjectByPropertyValue<Ability>("Id", abilityIdList[i]);
                        }
                        else
                        {
                            ability = this.dataService.GetAbilitiesForPokemon(pokemon.Id, selectedGame)[0];
                        }

                        pokemonTeamDetail = new PokemonTeamDetail()
                        {
                            PokemonId = pokemon.Id,
                            AbilityId = ability.Id,
                            NatureId = this.dataService.GetObjectByPropertyValue<Nature>("Name", "Serious").Id,
                            Level = 100,
                            Happiness = 255,
                        };

                        this.dataService.AddPokemonTeamDetail(pokemonTeamDetail);

                        pokemonTeam.InsertPokemon(pokemonTeamDetail);
                    }

                    this.dataService.AddObject(pokemonTeam);

                    return string.Concat("Team \"", pokemonTeam.PokemonTeamName, "\" has been added successfully!");
                }
                else
                {
                    return "You must be logged in to save a team.";
                }
            }

            return null;
        }

        /// <summary>
        /// Prepares the required information for the user to view the pokemon page.
        /// </summary>
        /// <param name="generationId">The generation requested by the user.</param>
        /// <returns>The fill generation table shared view.</returns>
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

                List<PokemonTypeDetail> typingDetail = this.dataService.GetObjects<PokemonTypeDetail>(includes: "Pokemon, Pokemon.Game", whereProperty: "Pokemon.Game.GenerationId", wherePropertyValue: generationId).ToList();
                List<Pokemon> noTypingPokemon = this.dataService.GetAllPokemon().Where(x => x.Game.GenerationId == generationId).Except(typingDetail.Select(x => x.Pokemon)).ToList();
                List<Pokemon> altFormList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon").Select(x => x.AltFormPokemon).ToList();
                noTypingPokemon = noTypingPokemon.Where(x => !altFormList.Any(y => y.Id == x.Id)).ToList();

                foreach (var p in noTypingPokemon)
                {
                    model.PokemonList.Add(new PokemonTypeDetail()
                    {
                        Pokemon = p,
                        PokemonId = p.Id,
                        PrimaryTypeId = 0,
                        PrimaryType = new DataAccess.Models.Type()
                        {
                            Id = 0,
                            Name = "None",
                        },
                    });
                }

                model.PokemonList = model.PokemonList.OrderBy(x => x.Pokemon.PokedexNumber).ThenBy(x => x.PokemonId).ToList();

                return this.PartialView("_FillGenerationTable", model);
            }
            else
            {
                return this.RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Prepares the required information for the user to view the available pokemon page.
        /// </summary>
        /// <param name="gameId">The game requested by the user.</param>
        /// <returns>The fill available generation table shared view.</returns>
        [Route("get-available-pokemon-by-game/{gameId}")]
        public IActionResult GetAvailablePokemonByGame(int gameId)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<PokemonGameDetail> pokemonGameDetails = this.dataService.GetObjects<PokemonGameDetail>(includes: "Pokemon, Game", whereProperty: "GameId", wherePropertyValue: gameId);
                List<Pokemon> pokemonList = this.dataService.GetObjects<Pokemon>("PokedexNumber, Id", "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth");
                pokemonList = pokemonList.Where(x => pokemonGameDetails.Any(y => y.PokemonId == x.Id)).ToList();
                List<Pokemon> altFormsList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form").ConvertAll(x => x.AltFormPokemon);
                foreach (var p in pokemonList.Where(x => altFormsList.Any(y => y.Id == x.Id)))
                {
                    p.Name = string.Concat(p.Name, " (", this.dataService.GetObjectByPropertyValue<PokemonFormDetail>("AltFormPokemonId", p.Id, "Form").Form.Name, ")");
                }

                GenerationTableViewModel model = new GenerationTableViewModel()
                {
                    PokemonNoTypeList = pokemonList,
                    AltFormsList = altFormsList,
                    AppConfig = this.appConfig,
                    Generation = this.dataService.GetObjectByPropertyValue<Game>("Id", gameId, "Generation").Generation,
                };

                return this.PartialView("_FillAvailableGenerationTable", model);
            }
            else
            {
                return this.RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Generatres a pokemon team for the team randomizer.
        /// </summary>
        /// <param name="pokemonCount">The amount of pokemon requested.</param>
        /// <param name="selectedGens">The generations allowed for generation.</param>
        /// <param name="selectedGameId">The specific game used for generation.</param>
        /// <param name="selectedType">The specific type for generation.</param>
        /// <param name="selectedLegendaries">The specific legendary classifications available for generation.</param>
        /// <param name="selectedForms">The specific forms available for generation.</param>
        /// <param name="selectedEvolutions">The specific evolution classification available for generation.</param>
        /// <param name="needsStarter">Whether or not a starter needs to be included.</param>
        /// <param name="onlyLegendaries">Whether or not only legendaries are allowed for generation.</param>
        /// <param name="onlyAltForms">Whether or not only alternate forms are allowed for generation.</param>
        /// <param name="multipleMegas">Whether or not multiple mega pokemon are allowed for generation.</param>
        /// <param name="multipleGMax">Whether or not multiple gigantimax pokemon are allowed for generation.</param>
        /// <param name="onePokemonForm">Whether or not only one form of a pokemon is allowed for generation.</param>
        /// <param name="randomAbility">Whether or not randomized abilities will also be generated.</param>
        /// <param name="noRepeatType">Whether or not repeat types are allowed for generation.</param>
        /// <returns>The view model of the generated pokemon team.</returns>
        [Route("get-pokemon-team")]
        public TeamRandomizerViewModel GetPokemonTeam(int pokemonCount, List<int> selectedGens, int selectedGameId, int selectedType, List<string> selectedLegendaries, List<string> selectedForms, List<string> selectedEvolutions, bool needsStarter, bool onlyLegendaries, bool onlyAltForms, bool multipleMegas, bool multipleGMax, bool onePokemonForm, bool randomAbility, bool noRepeatType)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                try
                {
                    if (pokemonCount > 6)
                    {
                        pokemonCount = 6;
                    }

                    if (selectedType != 0 && noRepeatType)
                    {
                        noRepeatType = false;
                    }

                    List<Generation> unselectedGens = this.dataService.GetObjects<Generation>();
                    foreach (var item in selectedGens)
                    {
                        unselectedGens.Remove(unselectedGens.Find(x => x.Id == item));
                    }

                    Pokemon pokemon;
                    Game selectedGame = new Game();
                    if (selectedGameId != 0)
                    {
                        selectedGame = this.dataService.GetObjectByPropertyValue<Game>("Id", selectedGameId);
                    }
                    else
                    {
                        needsStarter = false;
                    }

                    TeamRandomizerViewModel model = new TeamRandomizerViewModel()
                    {
                        AllPokemonChangedNames = new List<Pokemon>(),
                        AllPokemonOriginalNames = new List<Pokemon>(),
                        PokemonAbilities = new List<Ability>(),
                        AppConfig = this.appConfig,
                    };
                    List<Pokemon> pokemonList = new List<Pokemon>();
                    List<PokemonGameDetail> availablePokemon = new List<PokemonGameDetail>();
                    List<GameStarterDetail> allStarters = new List<GameStarterDetail>();
                    List<Pokemon> starterList = new List<Pokemon>();
                    if (needsStarter && selectedGameId != 0)
                    {
                        starterList = this.dataService.GetObjects<GameStarterDetail>("Pokemon.PokedexNumber, Pokemon.Id", "Pokemon", "GameId", selectedGameId).ConvertAll(x => x.Pokemon);
                    }

                    if (selectedGame.Id != 0)
                    {
                        availablePokemon = this.dataService.GetObjects<PokemonGameDetail>(includes: "Pokemon, Game", whereProperty: "GameId", wherePropertyValue: selectedGame.Id);
                        if (needsStarter)
                        {
                            allStarters = this.dataService.GetObjects<GameStarterDetail>(includes: "Pokemon, Game", whereProperty: "GameId", wherePropertyValue: selectedGame.Id);
                        }
                    }
                    else
                    {
                        availablePokemon = this.dataService.GetObjects<PokemonGameDetail>(includes: "Pokemon, Game");
                    }

                    List<Pokemon> allPokemon = this.GetAllPokemonWithoutForms();
                    List<Evolution> allEvolutions = this.dataService.GetObjects<Evolution>(includes: "PreevolutionPokemon, PreevolutionPokemon.Game, EvolutionPokemon, EvolutionPokemon.Game, EvolutionMethod");
                    List<PokemonTypeDetail> pokemonTypeDetails = this.dataService.GetObjects<PokemonTypeDetail>(includes: "PrimaryType, SecondaryType");
                    if (selectedGame.Id != 0)
                    {
                        allEvolutions = allEvolutions.Where(x => x.PreevolutionPokemon.Game.ReleaseDate <= selectedGame.ReleaseDate && x.EvolutionPokemon.Game.ReleaseDate <= selectedGame.ReleaseDate).ToList();
                    }

                    Random rnd = new Random();

                    foreach (var gen in unselectedGens)
                    {
                        allPokemon = allPokemon.Except(allPokemon.Where(x => x.Game.GenerationId == gen.Id)).ToList();
                    }

                    if (selectedLegendaries.Count == 0)
                    {
                        List<PokemonLegendaryDetail> legendaryList = this.dataService.GetObjects<PokemonLegendaryDetail>(includes: "Pokemon, LegendaryType");

                        foreach (var p in legendaryList)
                        {
                            allPokemon.Remove(allPokemon.Find(x => x.Id == p.PokemonId));
                        }
                    }
                    else
                    {
                        List<string> legendaryTypes = this.dataService.GetObjects<LegendaryType>("Type").ConvertAll(x => x.Type);
                        foreach (var legendary in legendaryTypes.Except(selectedLegendaries).ToList())
                        {
                            List<PokemonLegendaryDetail> legendaryList = this.dataService.GetObjects<PokemonLegendaryDetail>(includes: "Pokemon, LegendaryType").Where(x => x.LegendaryType.Type == legendary).ToList();

                            foreach (var l in legendaryList)
                            {
                                allPokemon.Remove(allPokemon.Find(x => x.Id == l.PokemonId));
                            }
                        }
                    }

                    if (onlyLegendaries)
                    {
                        List<Pokemon> legendaryList = new List<Pokemon>();
                        List<PokemonLegendaryDetail> allLegendaries = this.dataService.GetObjects<PokemonLegendaryDetail>(includes: "Pokemon, LegendaryType");

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

                        if (selectedForms.Where(x => x != "Other").Count() > 0)
                        {
                            FormGroup formGroup;
                            List<Form> forms;
                            List<PokemonFormDetail> pokemonFormList, filteredFormList;
                            foreach (var formGroupName in selectedForms.Where(x => x != "Other"))
                            {
                                formGroup = this.dataService.GetObjectByPropertyValue<FormGroup>("Name", formGroupName);
                                forms = this.dataService.GetObjects<Form>(whereProperty: "FormGroupId", wherePropertyValue: formGroup.Id);
                                pokemonFormList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form").Where(x => forms.Any(y => y.Id == x.FormId)).ToList();
                                filteredFormList = new List<PokemonFormDetail>();

                                foreach (var p in allPokemon)
                                {
                                    if (formGroup.Name != "Alolan" || (formGroup.Name == "Alolan" && (selectedGame.Id != 37 || (selectedGame.Id == 37 && (p.Id == 54 || p.Id == 56)))))
                                    {
                                        List<PokemonFormDetail> altForm = pokemonFormList.Where(x => x.OriginalPokemonId == p.Id).ToList();
                                        foreach (var a in altForm)
                                        {
                                            a.AltFormPokemon.Name = string.Concat(a.AltFormPokemon.Name, " (", a.Form.Name, ")");
                                        }

                                        if (altForm.Count > 0)
                                        {
                                            filteredFormList.AddRange(altForm);
                                        }
                                    }
                                }

                                if (filteredFormList.Count > 0)
                                {
                                    foreach (var p in filteredFormList)
                                    {
                                        altForms.Add(p.AltFormPokemon);
                                    }
                                }
                            }
                        }

                        if (selectedForms.Contains("Other"))
                        {
                            List<PokemonFormDetail> pokemonFormList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form");

                            List<Form> formsToRemove = this.dataService.GetObjects<Form>("Name").Where(x => !x.Randomizable).ToList();

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

                                if (altForm.Count > 0)
                                {
                                    filteredFormList.AddRange(altForm);
                                }
                            }

                            if (filteredFormList.Count > 0)
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
                        List<Pokemon> altFormsList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form").ConvertAll(x => x.AltFormPokemon);
                        allPokemon = allPokemon.Where(x => altFormsList.Any(y => y.Id == x.Id)).ToList();
                    }

                    if (!multipleMegas)
                    {
                        List<Pokemon> megaList = new List<Pokemon>();
                        List<PokemonFormDetail> altFormList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form");
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
                        List<Pokemon> altFormList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form").Where(x => x.Form.Name.Contains("Gigantamax")).Select(x => x.AltFormPokemon).ToList();
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

                    if (selectedEvolutions.Count > 0 && selectedEvolutions.Count() < 3)
                    {
                        List<Pokemon> evolutions = new List<Pokemon>();
                        List<Pokemon> starterEvolutions = new List<Pokemon>();
                        if (selectedEvolutions.Contains("stage1Pokemon"))
                        {
                            foreach (var p in allPokemon)
                            {
                                if (allEvolutions.Exists(x => x.PreevolutionPokemonId == p.Id) && !allEvolutions.Exists(x => x.EvolutionPokemonId == p.Id))
                                {
                                    evolutions.Add(p);
                                }
                            }

                            foreach (var p in starterList)
                            {
                                if (allEvolutions.Exists(x => x.PreevolutionPokemonId == p.Id) && !allEvolutions.Exists(x => x.EvolutionPokemonId == p.Id))
                                {
                                    starterEvolutions.Add(p);
                                }
                            }
                        }

                        if (selectedEvolutions.Contains("middleEvolution"))
                        {
                            foreach (var p in allPokemon)
                            {
                                if ((allEvolutions.Exists(x => x.PreevolutionPokemonId == p.Id) && allEvolutions.Exists(x => x.EvolutionPokemonId == p.Id)) || (p.Id == 89 && selectedGame.Id == 20 && !starterEvolutions.Exists(x => x.Id == p.Id)))
                                {
                                    evolutions.Add(p);
                                }
                            }

                            foreach (var p in starterList)
                            {
                                if ((allEvolutions.Exists(x => x.PreevolutionPokemonId == p.Id) && allEvolutions.Exists(x => x.EvolutionPokemonId == p.Id)) || (p.Id == 89 && selectedGame.Id == 20 && !starterEvolutions.Exists(x => x.Id == p.Id)))
                                {
                                    starterEvolutions.Add(p);
                                }
                            }
                        }

                        if (selectedEvolutions.Contains("onlyFullyEvolved"))
                        {
                            foreach (var p in allPokemon)
                            {
                                if (!allEvolutions.Exists(x => x.PreevolutionPokemonId == p.Id) || (p.Id == 89 && selectedGame.Id == 20 && !starterEvolutions.Exists(x => x.Id == p.Id)))
                                {
                                    evolutions.Add(p);
                                }
                            }

                            foreach (var p in starterList)
                            {
                                if (!allEvolutions.Exists(x => x.PreevolutionPokemonId == p.Id) || (p.Id == 89 && selectedGame.Id == 20 && !starterEvolutions.Exists(x => x.Id == p.Id)))
                                {
                                    starterEvolutions.Add(p);
                                }
                            }
                        }

                        allPokemon = evolutions;
                        starterList = starterEvolutions;
                    }

                    if (selectedType != 0)
                    {
                        List<PokemonTypeDetail> allTypes = new List<PokemonTypeDetail>();
                        if (selectedGame.Id != 0)
                        {
                            allTypes = this.GetAllPokemonWithSpecificType(selectedType, selectedGame.GenerationId, allPokemon);
                        }
                        else
                        {
                            allTypes = this.dataService.GetObjects<PokemonTypeDetail>("Pokemon.PokedexNumber, PokemonId", "Pokemon, Pokemon.Game, PrimaryType, SecondaryType").Where(x => allPokemon.Any(y => y.Id == x.PokemonId)).ToList();
                            allTypes = allTypes.Where(x => x.PrimaryTypeId == selectedType || x.SecondaryTypeId == selectedType).ToList();
                        }

                        allPokemon = allPokemon.Where(x => allTypes.Select(x => x.Pokemon).Any(y => y.Id == x.Id)).ToList();
                    }

                    if (availablePokemon.Count > 1)
                    {
                        allPokemon = allPokemon.Where(x => availablePokemon.Any(y => y.PokemonId == x.Id)).ToList();
                    }

                    if (allPokemon.Count > 0)
                    {
                        while (allPokemon.Count() > 0 && pokemonList.Count() < pokemonCount)
                        {
                            if (needsStarter && pokemonList.Count() == 0)
                            {
                                pokemon = starterList[rnd.Next(starterList.Count)];
                            }
                            else
                            {
                                pokemon = allPokemon[rnd.Next(allPokemon.Count)];
                            }

                            if (onePokemonForm)
                            {
                                int originalPokemonId;
                                if (this.dataService.CheckIfAltForm(pokemon.Id))
                                {
                                    originalPokemonId = this.dataService.GetObjectByPropertyValue<PokemonFormDetail>("AltFormPokemonId", pokemon.Id, "AltFormPokemon, OriginalPokemon, Form").OriginalPokemonId;
                                }
                                else
                                {
                                    originalPokemonId = pokemon.Id;
                                }

                                List<Pokemon> altForms = this.dataService.GetAltForms(originalPokemonId);

                                if (this.dataService.CheckIfAltForm(pokemon.Id))
                                {
                                    altForms.Remove(altForms.Find(x => x.Id == pokemon.Id));
                                    altForms.Add(this.dataService.GetObjectByPropertyValue<Pokemon>("Id", originalPokemonId, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth"));
                                }

                                foreach (var p in altForms)
                                {
                                    if (allPokemon.Exists(x => x.Id == p.Id))
                                    {
                                        allPokemon.Remove(allPokemon.Find(x => x.Id == p.Id));
                                    }
                                }
                            }

                            if (noRepeatType)
                            {
                                pokemonTypeDetails = pokemonTypeDetails.Where(x => allPokemon.Any(y => x.PokemonId == y.Id)).ToList();

                                if (pokemonTypeDetails.Exists(x => x.PokemonId == pokemon.Id))
                                {
                                    List<DataAccess.Models.Type> selectedPokemonTyping = new List<DataAccess.Models.Type>()
                                    {
                                        pokemonTypeDetails.Find(x => x.PokemonId == pokemon.Id).PrimaryType,
                                    };

                                    if (pokemonTypeDetails.Find(x => x.PokemonId == pokemon.Id).SecondaryType != null)
                                    {
                                        selectedPokemonTyping.Add(pokemonTypeDetails.Find(x => x.PokemonId == pokemon.Id).SecondaryType);
                                    }

                                    pokemonTypeDetails.Remove(pokemonTypeDetails.Find(x => x.PokemonId == pokemon.Id));

                                    foreach (var t in selectedPokemonTyping)
                                    {
                                        if (pokemonTypeDetails.Exists(x => x.PrimaryTypeId == t.Id))
                                        {
                                            foreach (var ptd in pokemonTypeDetails.Where(x => x.PrimaryTypeId == t.Id))
                                            {
                                                allPokemon.Remove(allPokemon.Find(x => x.Id == ptd.PokemonId));
                                            }
                                        }

                                        if (pokemonTypeDetails.Exists(x => x.SecondaryTypeId == t.Id))
                                        {
                                            foreach (var ptd in pokemonTypeDetails.Where(x => x.SecondaryTypeId == t.Id))
                                            {
                                                allPokemon.Remove(allPokemon.Find(x => x.Id == ptd.PokemonId));
                                            }
                                        }
                                    }
                                }
                            }

                            pokemonList.Add(pokemon);
                            if (needsStarter && pokemonList.Count() == 1)
                            {
                                foreach (var s in starterList)
                                {
                                    if (allPokemon.Exists(x => x.Id == s.Id))
                                    {
                                        allPokemon.Remove(allPokemon.Find(x => x.Id == s.Id));
                                    }
                                }
                            }
                            else
                            {
                                allPokemon.Remove(allPokemon.Find(x => x.Id == pokemon.Id));
                            }
                        }
                    }

                    model.AllPokemonChangedNames = pokemonList;
                    foreach (var p in pokemonList)
                    {
                        model.AllPokemonOriginalNames.Add(this.dataService.GetObjectByPropertyValue<Pokemon>("Id", p.Id, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth"));
                    }

                    int generationId = 0;

                    if (selectedGame.Id != 0)
                    {
                        generationId = selectedGame.GenerationId;
                    }

                    model.PokemonURLs = new List<string>();
                    foreach (var p in model.AllPokemonOriginalNames)
                    {
                        if (generationId != 0)
                        {
                            model.PokemonURLs.Add(this.Url.Action("PokemonWithId", "Home", new { pokemonName = p.Name.Replace(": ", "_").Replace(' ', '_').ToLower(), pokemonId = p.Id, generationId }));
                        }
                        else
                        {
                            model.PokemonURLs.Add(this.Url.Action("PokemonWithId", "Home", new { pokemonName = p.Name.Replace(": ", "_").Replace(' ', '_').ToLower(), pokemonId = p.Id, generationId = this.dataService.GetObjects<PokemonGameDetail>(includes: "Game", whereProperty: "PokemonId", wherePropertyValue: p.Id).Select(x => x.Game).Last().Id }));
                        }
                    }

                    if (selectedGame.GenerationId != 1 && selectedGame.GenerationId != 2)
                    {
                        PokemonAbilityDetail pokemonAbilities;
                        List<Ability> abilities;
                        foreach (var p in model.AllPokemonOriginalNames)
                        {
                            abilities = new List<Ability>();
                            if (this.dataService.GetObjectByPropertyValue<PokemonAbilityDetail>("PokemonId", p.Id) != null)
                            {
                                if (generationId != 0)
                                {
                                    pokemonAbilities = this.dataService.GetObjects<PokemonAbilityDetail>(includes: "Pokemon, PrimaryAbility, SecondaryAbility, HiddenAbility, SpecialEventAbility", whereProperty: "PokemonId", wherePropertyValue: p.Id).Last(x => x.GenerationId <= generationId);
                                }
                                else
                                {
                                    pokemonAbilities = this.dataService.GetObjects<PokemonAbilityDetail>(includes: "Pokemon, PrimaryAbility, SecondaryAbility, HiddenAbility, SpecialEventAbility", whereProperty: "PokemonId", wherePropertyValue: p.Id).Last();
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
                            else
                            {
                                model.PokemonAbilities.Add(new Ability() { Id = 0, Name = "Unknown", Description = "Unknown" });
                            }
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

                    this.dataService.AddPageView("Random Team Generated", this.User.IsInRole("Owner"));

                    return model;
                }
                catch (Exception e)
                {
                    if (!this.User.IsInRole("Owner"))
                    {
                        string commentBody;
                        Game game = this.dataService.GetObjectByPropertyValue<Game>("Id", selectedGameId);
                        DataAccess.Models.Type type = this.dataService.GetObjectByPropertyValue<DataAccess.Models.Type>("Id", selectedType);

                        if (e != null)
                        {
                            commentBody = string.Concat(e.GetType().ToString(), " error during team generation.");
                        }
                        else
                        {
                            commentBody = "Unknown error during team generation.";
                        }

                        if (game != null)
                        {
                            commentBody = string.Concat(commentBody, " - Selected Game: ", game.Name);
                        }
                        else
                        {
                            commentBody = string.Concat(commentBody, " - Selected Game: ", selectedGameId);
                        }

                        if (type != null)
                        {
                            commentBody = string.Concat(commentBody, " - Selected Type: ", type.Name);
                        }
                        else
                        {
                            commentBody = string.Concat(commentBody, " - Selected Type: ", selectedType);
                        }

                        commentBody = string.Concat(commentBody, " - Pokemon Count: ", pokemonCount);
                        commentBody = string.Concat(commentBody, " - Selected Generation Ids: {", string.Join(", ", selectedGens), "}");
                        commentBody = string.Concat(commentBody, " - Selected Evolutions: {", string.Join(", ", selectedEvolutions), "}");
                        commentBody = string.Concat(commentBody, " - Selected Forms: {", string.Join(", ", selectedForms), "}");
                        commentBody = string.Concat(commentBody, " - Selected Legendary Types: {", string.Join(", ", selectedLegendaries), "}");
                        commentBody = string.Concat(commentBody, " - Needs Starter: ", needsStarter);
                        commentBody = string.Concat(commentBody, " - Only Legendaries: ", onlyLegendaries);
                        commentBody = string.Concat(commentBody, " - Only Alternate Forms: ", onlyAltForms);
                        commentBody = string.Concat(commentBody, " - Multiple Megas: ", multipleMegas);
                        commentBody = string.Concat(commentBody, " - Multiple Gigantamax: ", multipleGMax);
                        commentBody = string.Concat(commentBody, " - Only One Form per Pokemon: ", onePokemonForm);
                        commentBody = string.Concat(commentBody, " - Randomized Ability: ", randomAbility);
                        commentBody = string.Concat(commentBody, " - No Repeating Types: ", noRepeatType);

                        Comment comment = new Comment()
                        {
                            Name = commentBody,
                        };

                        if (this.User.Identity.Name != null)
                        {
                            comment.CommentorId = this.dataService.GetObjectByPropertyValue<User>("Username", this.User.Identity.Name).Id;
                        }

                        this.dataService.AddObject(comment);

                        this.dataService.EmailComment(this.appConfig, comment);
                    }

                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a list of abilities for a pokemon.
        /// </summary>
        /// <param name="pokemonId">The pokemon requested.</param>
        /// <param name="gameId">The game the search will take place in.</param>
        /// <returns>The list of available abilities for the specified pokemon in the specified game.</returns>
        [Route("get-pokemon-abilities")]
        public List<Ability> GetPokemonAbilities(int pokemonId, int gameId)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<Ability> pokemonAbilities = this.dataService.GetAbilitiesForPokemon(pokemonId, gameId);
                return pokemonAbilities;
            }

            return null;
        }

        /// <summary>
        /// Updates the type chart for the given type.
        /// </summary>
        /// <param name="typeId">The type being modified.</param>
        /// <param name="genId">The generation this modification is taking place.</param>
        /// <param name="resistances">The type's resistances.</param>
        /// <param name="weaknesses">The type's weaknesses.</param>
        /// <param name="immunities">The type's immunities.</param>
        /// <returns>The admin type page.</returns>
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
                    this.dataService.AddObject(typeChart);
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
                    this.dataService.AddObject(typeChart);
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
                    this.dataService.AddObject(typeChart);
                }

                foreach (var t in existingCharts)
                {
                    this.dataService.DeleteObject<TypeChart>(t.Id);
                }

                return this.Json(this.Url.Action("Types", "Admin")).Value.ToString();
            }

            return null;
        }

        /// <summary>
        /// Deletes the teams selected by the user.
        /// </summary>
        /// <param name="teamIds">The teams for deletion.</param>
        /// <returns>The pokemon teams page.</returns>
        [Route("delete-pokemon-teams")]
        public string DeletePokemonTeams(List<int> teamIds)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<PokemonTeam> pokemonTeams = this.dataService.GetObjects<PokemonTeam>("Id", "User", "User.Username", this.User.Identity.Name);
                pokemonTeams = pokemonTeams.Where(x => teamIds.Any(y => y == x.Id)).ToList();

                foreach (var t in pokemonTeams)
                {
                    this.dataService.DeletePokemonTeam(t.Id);
                }

                return this.Json(this.Url.Action("PokemonTeams", "User")).Value.ToString();
            }

            return null;
        }

        /// <summary>
        /// Updates the availablity of the specified form group in different games.
        /// </summary>
        /// <param name="formGroupId">The form group's id.</param>
        /// <param name="games">The games the pokemon is able to be used in.</param>
        /// <returns>The admin pokemon page.</returns>
        [Route("update-form-group-game-availability")]
        public string UpdateFormGroupGameAvailability(int formGroupId, List<int> games)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                FormGroupGameDetail formGroupGameDetail;
                List<FormGroupGameDetail> existingGameDetails = this.dataService.GetObjects<FormGroupGameDetail>(whereProperty: "FormGroupId", wherePropertyValue: formGroupId);

                foreach (var g in games)
                {
                    formGroupGameDetail = new FormGroupGameDetail()
                    {
                        FormGroupId = formGroupId,
                        GameId = g,
                    };
                    this.dataService.AddObject(formGroupGameDetail);
                }

                foreach (var g in existingGameDetails)
                {
                    this.dataService.DeleteObject<FormGroupGameDetail>(g.Id);
                }

                return this.Json(this.Url.Action("FormGroups", "Admin")).Value.ToString();
            }

            return null;
        }

        /// <summary>
        /// Updates the availablity of the specified pokemon in different games.
        /// </summary>
        /// <param name="pokemonId">The pokemon's id.</param>
        /// <param name="games">The games the pokemon is able to be used in.</param>
        /// <returns>The admin pokemon page.</returns>
        [Route("update-pokemon-game-availability")]
        public string UpdatePokemonGameAvailability(int pokemonId, List<int> games)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                PokemonGameDetail pokemonGameDetail;
                List<PokemonGameDetail> existingGameDetails = this.dataService.GetObjects<PokemonGameDetail>("Game.GenerationId, GameId, Id", "Pokemon, Pokemon.Game, Game", "PokemonId", pokemonId);

                foreach (var g in games)
                {
                    pokemonGameDetail = new PokemonGameDetail()
                    {
                        PokemonId = pokemonId,
                        GameId = g,
                    };
                    this.dataService.AddObject(pokemonGameDetail);
                }

                foreach (var g in existingGameDetails)
                {
                    this.dataService.DeleteObject<PokemonGameDetail>(g.Id);
                }

                return this.Json(this.Url.Action("Pokemon", "Admin")).Value.ToString();
            }

            return null;
        }

        /// <summary>
        /// Updates the availablity of the specified pokemon in different games.
        /// </summary>
        /// <param name="pokemonId">The pokemon's id.</param>
        /// <param name="games">The games the pokemon is able to be used in.</param>
        /// <returns>The admin pokemon page.</returns>
        [Route("update-pokemon-game-starters")]
        public string UpdatePokemonGameStarters(int pokemonId, List<int> games)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                GameStarterDetail gameStarterDetail;
                List<GameStarterDetail> existingGameDetails = this.dataService.GetObjects<GameStarterDetail>("Game.GenerationId, GameId, Id", "Pokemon, Pokemon.Game, Game", "PokemonId", pokemonId);

                foreach (var g in games)
                {
                    gameStarterDetail = new GameStarterDetail()
                    {
                        PokemonId = pokemonId,
                        GameId = g,
                    };
                    this.dataService.AddObject(gameStarterDetail);
                }

                foreach (var g in existingGameDetails)
                {
                    this.dataService.DeleteObject<GameStarterDetail>(g.Id);
                }

                return this.Json(this.Url.Action("Game", "Admin")).Value.ToString();
            }

            return null;
        }

        /// <summary>
        /// Updates the availablity of the pokemon in a specified game.
        /// </summary>
        /// <param name="gameId">The game's id.</param>
        /// <param name="pokemonList">The pokemon that are available in the given game.</param>
        /// <returns>The admin pokemon page.</returns>
        [Route("update-game-availability")]
        public string UpdateGameAvailability(int gameId, List<int> pokemonList)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<PokemonGameDetail> existingGameDetails = new List<PokemonGameDetail>();
                List<PokemonGameDetail> newGameDetails = new List<PokemonGameDetail>();
                List<int> pokemonIds = new List<int>();
                List<Game> games = this.dataService.GetObjects<Game>("ReleaseDate, Id");
                DateTime releaseDate = games.Find(x => x.Id == gameId).ReleaseDate;
                games = games.Where(x => x.ReleaseDate == releaseDate).ToList();
                int generationId = games.First().GenerationId;

                if (gameId == 4 || gameId == 5)
                {
                    games = games.Where(x => x.Id == gameId).ToList();
                }
                else if (gameId == 16 || gameId == 28)
                {
                    generationId = 1;
                }
                else if (gameId == 35 || gameId == 36)
                {
                    generationId = 4;
                }

                List<Evolution> evolutions = this.dataService.GetObjects<Evolution>().Where(x => x.GenerationId <= generationId).ToList();

                pokemonIds.AddRange(pokemonList.Except(evolutions.Select(x => x.EvolutionPokemonId).ToList()));

                foreach (var p in pokemonList.Intersect(evolutions.Select(x => x.EvolutionPokemonId).ToList()))
                {
                    Evolution evolutionDetails = evolutions.Find(x => x.EvolutionPokemonId == p);
                    if (!pokemonIds.Contains(evolutionDetails.PreevolutionPokemonId))
                    {
                        pokemonIds.Add(evolutionDetails.PreevolutionPokemonId);
                        if (evolutions.FirstOrDefault(x => x.EvolutionPokemonId == evolutionDetails.PreevolutionPokemonId) != null)
                        {
                            pokemonIds.Add(evolutions.First(x => x.EvolutionPokemonId == evolutionDetails.PreevolutionPokemonId).PreevolutionPokemonId);
                        }
                    }

                    pokemonIds.Add(p);
                }

                foreach (var game in games.ConvertAll(x => x.Id))
                {
                    newGameDetails = new List<PokemonGameDetail>();
                    existingGameDetails = this.dataService.GetObjects<PokemonGameDetail>(includes: "Pokemon, Game", whereProperty: "GameId", wherePropertyValue: game);

                    foreach (var p in pokemonIds)
                    {
                        if (existingGameDetails.Find(x => x.PokemonId == p && x.GameId == game) == null)
                        {
                            newGameDetails.Add(new PokemonGameDetail()
                            {
                                GameId = game,
                                PokemonId = p,
                            });
                        }
                        else
                        {
                            existingGameDetails.Remove(existingGameDetails.Find(x => x.PokemonId == p && x.GameId == game));
                        }
                    }

                    foreach (var g in newGameDetails)
                    {
                        this.dataService.AddObject(g);
                    }

                    foreach (var g in existingGameDetails)
                    {
                        this.dataService.DeleteObject<PokemonGameDetail>(g.Id);
                    }
                }

                return this.Json(this.Url.Action("GameAvailability", "Home")).Value.ToString();
            }

            return null;
        }

        /// <summary>
        /// Updates the availablity of the pokemon in a specified game.
        /// </summary>
        /// <param name="gameId">The game's id.</param>
        /// <param name="pokemonList">The pokemon that are available in the given game.</param>
        /// <returns>The admin pokemon page.</returns>
        [Route("update-game-starters")]
        public string UpdateGameStarter(int gameId, List<int> pokemonList)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<GameStarterDetail> existingGameStarters = new List<GameStarterDetail>();
                List<GameStarterDetail> newGameStarters = new List<GameStarterDetail>();
                List<Game> games = this.dataService.GetObjects<Game>("ReleaseDate, Id");
                DateTime releaseDate = games.Find(x => x.Id == gameId).ReleaseDate;
                games = games.Where(x => x.ReleaseDate == releaseDate).ToList();
                int generationId = games.First().GenerationId;

                if (gameId == 4 || gameId == 5)
                {
                    games = games.Where(x => x.Id == gameId).ToList();
                }
                else if (gameId == 16 || gameId == 28)
                {
                    generationId = 1;
                }
                else if (gameId == 35 || gameId == 36)
                {
                    generationId = 4;
                }

                foreach (var game in games.ConvertAll(x => x.Id))
                {
                    newGameStarters = new List<GameStarterDetail>();
                    existingGameStarters = this.dataService.GetObjects<GameStarterDetail>(includes: "Pokemon, Game", whereProperty: "GameId", wherePropertyValue: game);

                    foreach (var p in pokemonList)
                    {
                        if (existingGameStarters.Find(x => x.PokemonId == p && x.GameId == game) == null)
                        {
                            newGameStarters.Add(new GameStarterDetail()
                            {
                                GameId = game,
                                PokemonId = p,
                            });
                        }
                        else
                        {
                            existingGameStarters.Remove(existingGameStarters.Find(x => x.PokemonId == p && x.GameId == game));
                        }
                    }

                    foreach (var g in newGameStarters)
                    {
                        this.dataService.AddObject(g);
                    }

                    foreach (var g in existingGameStarters)
                    {
                        this.dataService.DeleteObject<GameStarterDetail>(g.Id);
                    }
                }

                return this.Json(this.Url.Action("Games", "Admin")).Value.ToString();
            }

            return null;
        }

        /// <summary>
        /// Gets a list of battle items for a pokemon.
        /// </summary>
        /// <param name="pokemonId">The pokemon the item belongs to.</param>
        /// <param name="generationId">The generation used to sort the available items.</param>
        /// <returns>The list of available battle items.</returns>
        [Route("get-pokemon-battle-items")]
        public List<BattleItem> GetPokemonBattleItems(int pokemonId, int generationId)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<BattleItem> battleItems = new List<BattleItem>();
                List<FormItem> formItems = this.dataService.GetFormItems().Where(x => x.PokemonId == pokemonId).ToList();
                if (formItems.Count == 0)
                {
                    Generation generation = this.dataService.GetObjectByPropertyValue<Generation>("Id", generationId);
                    List<BattleItem> allBattleItems = this.dataService.GetObjects<BattleItem>("GenerationId, Name", "Generation, Pokemon");
                    battleItems.AddRange(allBattleItems.Where(x => !x.OnlyInThisGeneration && x.PokemonId == null).ToList());
                    if (generation != null)
                    {
                        battleItems = battleItems.Where(x => x.Generation.Id <= generation.Id).ToList();
                        if (allBattleItems.Where(x => x.OnlyInThisGeneration && x.GenerationId == generation.Id).ToList().Count > 0)
                        {
                            battleItems.AddRange(allBattleItems.Where(x => x.OnlyInThisGeneration && x.GenerationId == generation.Id).ToList());
                        }

                        if (allBattleItems.Where(x => x.PokemonId == pokemonId && x.Generation.Id <= generation.Id).ToList().Count > 0)
                        {
                            battleItems.AddRange(allBattleItems.Where(x => x.PokemonId == pokemonId && x.Generation.Id <= generation.Id).ToList());
                        }
                    }
                    else
                    {
                        if (allBattleItems.Where(x => x.PokemonId == pokemonId).ToList().Count > 0)
                        {
                            battleItems.AddRange(allBattleItems.Where(x => x.PokemonId == pokemonId).ToList());
                        }
                    }

                    battleItems = battleItems.OrderBy(x => x.Name).ToList();
                }

                return battleItems;
            }

            return null;
        }

        /// <summary>
        /// Updates the pokemon list that give the specified evs.
        /// </summary>
        /// <param name="gameId">The generation's id.</param>
        /// <param name="health">The health ev.</param>
        /// <param name="attack">The attack ev.</param>
        /// <param name="defense">The defense ev.</param>
        /// <param name="specialAttack">The special attack ev.</param>
        /// <param name="specialDefense">The special defense ev.</param>
        /// <param name="speed">The speed ev.</param>
        /// <returns>The list of pokemon with this specific ev yield spread.</returns>
        [Route("get-pokemon-by-ev-yields")]
        public IActionResult GetPokemonByEVYields(int gameId, int health, int attack, int defense, int specialAttack, int specialDefense, int speed)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                int evYieldTotal = health + attack + defense + specialAttack + specialDefense + speed;
                Game game = this.dataService.GetObjectByPropertyValue<Game>("Id", gameId);
                List<PokemonFormDetail> altForms = this.dataService.GetObjects<PokemonFormDetail>(includes: "Form");
                List<Pokemon> pokemonList = this.dataService.GetObjects<PokemonGameDetail>("Pokemon.PokedexNumber, Pokemon.Id", "Pokemon", "GameId", game.Id).Select(x => x.Pokemon).ToList();
                List<EVYield> allEVYields = this.dataService.GetObjects<EVYield>("Pokemon.PokedexNumber, Pokemon.Id", "Pokemon").Where(x => x.GenerationId <= game.GenerationId).ToList();
                List<EVYield> evYields = new List<EVYield>();
                foreach (var ev in allEVYields)
                {
                    if (!evYields.Any(x => x.PokemonId == ev.PokemonId))
                    {
                        evYields.Add(allEVYields.Where(x => x.PokemonId == ev.PokemonId).OrderByDescending(x => x.GenerationId).First(x => x.GenerationId <= game.GenerationId));
                    }
                }

                pokemonList = evYields.Where(x => x.Health == health && x.Attack == attack && x.Defense == defense && x.SpecialAttack == specialAttack && x.SpecialDefense == specialDefense && x.Speed == speed && pokemonList.Any(y => y.Id == x.PokemonId)).Select(x => x.Pokemon).ToList();
                if (gameId == 10)
                {
                    if (attack == 1 && evYieldTotal == 1)
                    {
                        pokemonList.Add(this.dataService.GetObjectByPropertyValue<Pokemon>("Id", 839));
                    }
                    else if (attack == 2 && evYieldTotal == 2)
                    {
                        pokemonList.Remove(pokemonList.Find(x => x.Id == 839));
                    }
                }

                pokemonList = pokemonList.OrderBy(x => x.PokedexNumber).ThenBy(x => x.Id).ToList();

                EVTrainerViewModel pokemonEVYields = new EVTrainerViewModel()
                {
                    AllPokemon = pokemonList,
                    AllAltForms = new List<PokemonFormDetail>(),
                    AppConfig = this.appConfig,
                };

                foreach (var p in pokemonList)
                {
                    if (this.dataService.CheckIfAltForm(p.Id))
                    {
                        pokemonEVYields.AllAltForms.Add(altForms.Find(x => x.AltFormPokemonId == p.Id));
                    }
                }

                return this.PartialView("_FillEVTrainer", pokemonEVYields);
            }

            return this.RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Gets the available genders for a pokemon.
        /// </summary>
        /// <param name="pokemonId">The pokemon's id.</param>
        /// <returns>A list of available genders.</returns>
        [Route("get-pokemon-genders")]
        public List<string> GetPokemonGenders(int pokemonId)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<string> genders = new List<string>();
                GenderRatio genderRatio = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", pokemonId, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth").GenderRatio;
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

            return null;
        }

        /// <summary>
        /// Gets the type chart for the given typing combination.
        /// </summary>
        /// <param name="pokemonID">The pokemon's id.</param>
        /// <param name="generationID">The generation used to specify the type chart.</param>
        /// <returns>The file type evaluator chart shared view.</returns>
        [Route("get-typing-evaluator-chart-by-pokemon")]
        public IActionResult GetTypingEvaluatorChartByPokemon(int pokemonID, int generationID)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                PokemonTypeDetail pokemonTypeDetail = this.dataService.GetObjects<PokemonTypeDetail>(whereProperty: "PokemonId", wherePropertyValue: pokemonID).OrderByDescending(x => x.GenerationId).First(x => x.GenerationId <= generationID);
                int primaryId = pokemonTypeDetail.PrimaryTypeId ?? 0;
                int secondaryId = pokemonTypeDetail.SecondaryTypeId ?? 0;
                return this.PartialView("_FillTypeEvaluatorChart", this.GetTypeChartTyping(primaryId, secondaryId, generationID));
            }
            else
            {
                return this.RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Gets the type chart for the given typing combination.
        /// </summary>
        /// <param name="primaryTypeID">The primary type's id.</param>
        /// <param name="secondaryTypeID">The secondary type's id.</param>
        /// <param name="generationID">The generation used to specify the type chart.</param>
        /// <returns>The file type evaluator chart shared view.</returns>
        [Route("get-typing-evaluator-chart")]
        public IActionResult GetTypingEvaluatorChart(int primaryTypeID, int secondaryTypeID, int generationID)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return this.PartialView("_FillTypeEvaluatorChart", this.GetTypeChartTyping(primaryTypeID, secondaryTypeID, generationID));
            }
            else
            {
                return this.RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Updates the encounters for a shiny hunt.
        /// </summary>
        /// <param name="shinyHuntId">The shiny hunt's id.</param>
        /// <param name="increment">The amount to increment.</param>
        /// <returns>The amount of encounters for this shiny hunt.</returns>
        [Route("increment-shiny-hunt-encounters")]
        public int IncrementShinyHuntEncounters(int shinyHuntId, int increment)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                ShinyHunt shinyHunt = this.dataService.GetObjectByPropertyValue<ShinyHunt>("Id", shinyHuntId);
                if (increment == -1 && shinyHunt.Encounters == 0)
                {
                    return 0;
                }
                else
                {
                    shinyHunt.Encounters += increment;
                    this.dataService.UpdateObject<ShinyHunt>(shinyHunt);
                    return shinyHunt.Encounters;
                }
            }

            return 0;
        }

        /// <summary>
        /// Updates the encounters for a shiny hunt.
        /// </summary>
        /// <param name="shinyHuntId">The shiny hunt's id.</param>
        /// <param name="encounters">The amount to encounters.</param>
        /// <returns>The amount of encounters for this shiny hunt.</returns>
        [Route("set-shiny-hunt-encounters")]
        public int SetShinyHuntEncounters(int shinyHuntId, int encounters)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                ShinyHunt shinyHunt = this.dataService.GetObjectByPropertyValue<ShinyHunt>("Id", shinyHuntId);
                if (encounters < 0)
                {
                    return 0;
                }
                else
                {
                    shinyHunt.Encounters = encounters;
                    this.dataService.UpdateObject<ShinyHunt>(shinyHunt);
                    return shinyHunt.Encounters;
                }
            }

            return 0;
        }

        /// <summary>
        /// Checks to see if shiny charm can be used.
        /// </summary>
        /// <param name="gameId">The game's id.</param>
        /// <returns>The boolean determining if a shiny charm can be shown.</returns>
        [Route("check-shiny-charm")]
        public bool CheckShinyCharm(int gameId)
        {
            Game game = this.dataService.GetObjectByPropertyValue<Game>("Id", gameId);
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest" && gameId != 43 && (game.GenerationId >= 6 || gameId == 11 || gameId == 29))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks to see if sparkling power can be used.
        /// </summary>
        /// <param name="gameId">The game's id.</param>
        /// <returns>The boolean determining if a sparkling power can be shown.</returns>
        [Route("check-sparkling-power")]
        public bool CheckSparklingPower(int gameId)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest" && (gameId == 41 || gameId == 42))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks to see if marks can be used.
        /// </summary>
        /// <param name="gameId">The game's id.</param>
        /// <returns>The boolean determining if a mark can be shown.</returns>
        [Route("check-mark")]
        public bool CheckMark(int gameId)
        {
            Game game = this.dataService.GetObjectByPropertyValue<Game>("Id", gameId);
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest" && game.GenerationId >= 8 && gameId != 35 && gameId != 36)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the type chart for the given typing combination.
        /// </summary>
        /// <param name="pokemonId">The pokemon's id.</param>
        /// <param name="typeId">The tera/plate/memory type's id.</param>
        /// <param name="currentImage">The currently viewed image.</param>
        /// <returns>The file type evaluator chart shared view.</returns>
        [Route("get-pokemon-images-with-type")]
        public IActionResult GetTypingEvaluatorChart(int pokemonId, int typeId, string currentImage)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                if (typeId == 0)
                {
                    typeId = 5;
                }

                Pokemon pokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", pokemonId);
                PokemonImageViewModel model = new PokemonImageViewModel()
                {
                    Id = pokemon.Id,
                    Name = pokemon.Name,
                    CurrentImage = currentImage,
                    AppConfig = this.appConfig,
                    Type = this.dataService.GetObjectByPropertyValue<DataAccess.Models.Type>("Id", typeId),
                };

                HttpWebRequest webRequest;
                HttpWebResponse imageRequest;
                try
                {
                    webRequest = (HttpWebRequest)HttpWebRequest.Create(string.Concat(this.appConfig.WebUrl, this.appConfig.ShinyPokemonImageUrl, pokemon.Id, ".png"));
                    imageRequest = (HttpWebResponse)webRequest.GetResponse();
                    if (imageRequest.StatusCode == HttpStatusCode.OK)
                    {
                        model.HasShiny = true;
                    }
                    else
                    {
                        model.HasShiny = false;
                    }
                }
                catch
                {
                    model.HasShiny = false;
                }

                try
                {
                    webRequest = (HttpWebRequest)HttpWebRequest.Create(string.Concat(this.appConfig.WebUrl, this.appConfig.HomePokemonImageUrl, pokemon.Id, ".png"));
                    imageRequest = (HttpWebResponse)webRequest.GetResponse();
                    if (imageRequest.StatusCode == HttpStatusCode.OK)
                    {
                        model.HasHome = true;
                    }
                    else
                    {
                        model.HasHome = false;
                    }
                }
                catch
                {
                    model.HasHome = false;
                }

                return this.PartialView("_FillPokemonImages", model);
            }
            else
            {
                return this.RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Gets a list of generations based on what pokemon are available in the given game.
        /// </summary>
        /// <param name="selectedGame">The game used to filter the generations.</param>
        /// <returns>The refreshed list of generations.</returns>
        [Route("get-generations")]
        public TeamRandomizerListViewModel GetGenerations(int selectedGame)
        {
            try
            {
                if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    if (selectedGame != 0)
                    {
                        List<Pokemon> altFormsList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form").ConvertAll(x => x.AltFormPokemon);
                        List<PokemonGameDetail> availablePokemon = this.dataService.GetObjects<PokemonGameDetail>(includes: "Pokemon, Game", whereProperty: "GameId", wherePropertyValue: selectedGame).Where(x => !altFormsList.Any(y => y.Id == x.PokemonId)).ToList();
                        List<Pokemon> allPokemon = this.dataService.GetAllPokemon().Where(x => availablePokemon.Any(y => y.PokemonId == x.Id)).ToList();
                        Generation selectedGen = this.dataService.GetObjectByPropertyValue<Game>("Id", selectedGame, "Generation").Generation;
                        List<Generation> generationList = this.dataService.GetObjects<Generation>().Where(x => x.Id <= selectedGen.Id).ToList();
                        List<LegendaryType> legendaryTypes = this.dataService.GetObjects<LegendaryType>("Type");
                        List<Generation> availableGenerations = new List<Generation>();
                        Game game = this.dataService.GetObjectByPropertyValue<Game>("Id", selectedGame);

                        foreach (var gen in generationList)
                        {
                            if (allPokemon.Where(x => x.Game.GenerationId == gen.Id).ToList().Count != 0)
                            {
                                availableGenerations.Add(gen);
                            }
                        }

                        if (game.ReleaseDate < new DateTime(2016, 11, 18))
                        {
                            legendaryTypes.Remove(legendaryTypes.Find(x => x.Type == "Ultra Beast"));
                        }

                        TeamRandomizerListViewModel model = new TeamRandomizerListViewModel()
                        {
                            AllGenerations = availableGenerations,
                            AllTypes = this.dataService.GetObjects<DataAccess.Models.Type>("Name").Where(x => x.GenerationId <= selectedGen.Id).ToList(),
                            AllLegendaryTypes = legendaryTypes,
                            AllFormGroups = this.dataService.GetObjects<FormGroup>("Name", whereProperty: "AppearInTeamRandomizer", wherePropertyValue: true),
                            AllFormGroupGameDetails = this.dataService.GetObjects<FormGroupGameDetail>("FormGroup.Name", "FormGroup"),
                        };

                        return model;
                    }
                    else
                    {
                        TeamRandomizerListViewModel model = new TeamRandomizerListViewModel()
                        {
                            AllGenerations = this.dataService.GetObjects<Generation>().Where(x => this.GetAllPokemonWithoutForms().Any(y => y.Game.GenerationId == x.Id)).ToList(),
                            AllTypes = this.dataService.GetObjects<DataAccess.Models.Type>("Name"),
                            AllLegendaryTypes = this.dataService.GetObjects<LegendaryType>("Type"),
                            AllFormGroups = this.dataService.GetObjects<FormGroup>("Name", whereProperty: "AppearInTeamRandomizer", wherePropertyValue: true),
                            AllFormGroupGameDetails = this.dataService.GetObjects<FormGroupGameDetail>("FormGroup.Name", "FormGroup"),
                        };

                        return model;
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                if (!this.User.IsInRole("Owner") && e != null)
                {
                    string commentBody;
                    if (e != null)
                    {
                        commentBody = string.Concat(e.GetType().ToString(), " error during generation fetching.");
                    }
                    else
                    {
                        commentBody = "Unknown error during generation fetching.";
                    }

                    Comment comment = new Comment()
                    {
                        Name = string.Concat(commentBody, " - Selected Game Id: ", selectedGame),
                    };

                    if (this.User.Identity.Name != null)
                    {
                        comment.CommentorId = this.dataService.GetObjectByPropertyValue<User>("Username", this.User.Identity.Name).Id;
                    }

                    this.dataService.EmailComment(this.appConfig, comment);
                }

                return null;
            }
        }

        /// <summary>
        /// Gets a list of all pokemon by their type combination.
        /// </summary>
        /// <param name="primaryTypeID">The primary type's id.</param>
        /// <param name="secondaryTypeID">The secondary type's id.</param>
        /// <param name="gameID">The game used to specify the type chart.</param>
        /// <returns>The fill typing evaluator shared view.</returns>
        [Route("get-pokemon-by-typing")]
        public IActionResult GetPokemonByTyping(int primaryTypeID, int secondaryTypeID, int gameID)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                Game game = new Game()
                {
                    Id = 0,
                    Name = "Any Game",
                };

                if (gameID != 0)
                {
                    game = this.dataService.GetObjectByPropertyValue<Game>("Id", gameID);
                }

                List<PokemonTypeDetail> typingList = this.GetAllPokemonWithSpecificTypes(primaryTypeID, secondaryTypeID, game);
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
                    AppConfig = this.appConfig,
                    Game = game,
                };

                return this.PartialView("_FillTypingEvaluator", model);
            }
            else
            {
                return this.RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Gets a list of all pokemon by their ability.
        /// </summary>
        /// <param name="abilityID">The ability's id.</param>
        /// <param name="generationID">The generation used to specify the type chart.</param>
        /// <returns>The fill typing evaluator shared view.</returns>
        [Route("get-pokemon-by-ability")]
        public IActionResult GetPokemonByAbility(int abilityID, int generationID)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<PokemonAbilityDetail> abilityList = this.GetPokemonByAbilityAndGeneration(abilityID, generationID);

                List<Pokemon> pokemonList = new List<Pokemon>();

                foreach (var p in abilityList)
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

                AbilityEvaluatorPageViewModel model = new AbilityEvaluatorPageViewModel()
                {
                    AllPokemonWithAbility = abilityList ?? new List<PokemonAbilityDetail>(),
                    AllPokemon = pokemonList,
                    AppConfig = this.appConfig,
                    GenerationId = generationID,
                    Ability = this.dataService.GetObjectByPropertyValue<Ability>("Id", abilityID),
                };

                return this.PartialView("_FillAbilityEvaluator", model);
            }
            else
            {
                return this.RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Gets a list of all pokemon that is capable of breeding with the given pokemon.
        /// </summary>
        /// <param name="gameId">The game breeding is done in.</param>
        /// <returns>The fill day care evaluator dropdown shared view.</returns>
        [Route("get-pokemon-by-egg-group-dropdown")]
        public IActionResult GetPokemonByEggGroupDropdown(int gameId)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<PokemonEggGroupDetail> eggGroupDetails = this.dataService.GetAllBreedablePokemon(gameId);

                List<Pokemon> altForms = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form").ConvertAll(x => x.AltFormPokemon);

                foreach (var e in eggGroupDetails.Where(x => altForms.Any(y => y.Id == x.PokemonId)))
                {
                   e.Pokemon.Name = this.dataService.GetAltFormWithFormName(e.PokemonId).Name;
                }

                return this.PartialView("_FillDayCareEvaluatorDropdown", eggGroupDetails.ConvertAll(x => x.Pokemon));
            }
            else
            {
                return this.RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Gets a list of all pokemon that is capable of breeding with the given pokemon.
        /// </summary>
        /// <param name="pokemonId">The parent pokemon.</param>
        /// <param name="gameId">The game breeding is done in.</param>
        /// <returns>The fill day care evaluator shared view.</returns>
        [Route("get-pokemon-by-egg-group")]
        public IActionResult GetPokemonByEggGroup(int pokemonId, int gameId)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                try
                {
                    PokemonEggGroupDetail searchedEggGroupDetails = this.dataService.GetObjects<PokemonEggGroupDetail>(includes: "Pokemon, PrimaryEggGroup, SecondaryEggGroup", whereProperty: "PokemonId", wherePropertyValue: pokemonId).Last();
                    GenderRatio genderRatio = this.dataService.GetObjectByPropertyValue<GenderRatio>("Id", searchedEggGroupDetails.Pokemon.GenderRatioId);
                    List<Pokemon> altFormsList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form").ConvertAll(x => x.AltFormPokemon);
                    List<PokemonEggGroupDetail> eggGroupList = new List<PokemonEggGroupDetail>();
                    List<Pokemon> pokemonList = new List<Pokemon>();
                    List<Pokemon> originalPokemon = new List<Pokemon>();

                    if (pokemonId == this.dataService.GetPokemon("Manaphy").Id || pokemonId == this.dataService.GetPokemon("Phione").Id || (genderRatio.MaleRatio == 0 && genderRatio.FemaleRatio == 0 && pokemonId != this.dataService.GetPokemon("Ditto").Id))
                    {
                        eggGroupList.Add(this.dataService.GetObjectByPropertyValue<PokemonEggGroupDetail>("Pokemon.Name", "Ditto", "Pokemon, Pokemon.GenderRatio, PrimaryEggGroup, SecondaryEggGroup"));
                        pokemonList.Add(this.dataService.GetPokemon("Ditto"));
                        originalPokemon = eggGroupList.ConvertAll(x => x.Pokemon);
                    }
                    else if (pokemonId == this.dataService.GetPokemon("Ditto").Id)
                    {
                        eggGroupList = this.dataService.GetAllBreedablePokemon(gameId).OrderBy(x => x.Pokemon.PokedexNumber).ThenBy(x => x.PokemonId).ToList();
                        eggGroupList.Remove(eggGroupList.Find(x => x.PokemonId == pokemonId));

                        foreach (var p in eggGroupList)
                        {
                            if (altFormsList.Find(x => x.Id == p.PokemonId) != null)
                            {
                                p.Pokemon = this.dataService.GetAltFormWithFormName(p.PokemonId);
                            }

                            pokemonList.Add(p.Pokemon);
                        }

                        originalPokemon = eggGroupList.ConvertAll(x => x.Pokemon);
                    }
                    else
                    {
                        eggGroupList = this.GetAllPokemonWithSpecificEggGroups((int)searchedEggGroupDetails.PrimaryEggGroupId, searchedEggGroupDetails.SecondaryEggGroupId);
                        List<PokemonEggGroupDetail> breedablePokemonList = this.dataService.GetAllBreedablePokemon(gameId);
                        if (eggGroupList.Any(x => x.Pokemon.Name == "Manaphy"))
                        {
                            eggGroupList.Remove(eggGroupList.Find(x => x.Pokemon.Name == "Manaphy"));
                        }

                        if (eggGroupList.Any(x => x.Pokemon.Name == "Phione"))
                        {
                            eggGroupList.Remove(eggGroupList.Find(x => x.Pokemon.Name == "Phione"));
                        }

                        eggGroupList = eggGroupList.Where(x => breedablePokemonList.Any(y => y.PokemonId == x.PokemonId)).ToList();
                        eggGroupList.Add(this.dataService.GetObjectByPropertyValue<PokemonEggGroupDetail>("Pokemon.Name", "Ditto", "Pokemon, Pokemon.GenderRatio, PrimaryEggGroup, SecondaryEggGroup"));
                        eggGroupList = eggGroupList.OrderBy(x => x.Pokemon.PokedexNumber).ToList();
                        originalPokemon = eggGroupList.ConvertAll(x => x.Pokemon);
                        List<PokemonEggGroupDetail> finalEggGroupList = new List<PokemonEggGroupDetail>(eggGroupList);

                        foreach (var p in eggGroupList)
                        {
                            if ((p.Pokemon.GenderRatio.MaleRatio == 0 && p.Pokemon.GenderRatio.FemaleRatio == 0 && p.Pokemon.Name != "Ditto") || (genderRatio.MaleRatio == 100 && p.Pokemon.GenderRatio.MaleRatio == 100) || (genderRatio.FemaleRatio == 100 && p.Pokemon.GenderRatio.FemaleRatio == 100))
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

                    (originalPokemon ??= eggGroupList.ConvertAll(x => x.Pokemon)).RemoveAll(x => !eggGroupList.Select(y => y.PokemonId).Contains(x.Id));

                    List<EggGroup> pokemonEggGroupList = new List<EggGroup>
                    {
                        searchedEggGroupDetails.PrimaryEggGroup,
                    };

                    if (searchedEggGroupDetails.SecondaryEggGroup != null)
                    {
                        pokemonEggGroupList.Add(searchedEggGroupDetails.SecondaryEggGroup);
                    }

                    Game game = this.dataService.GetObjectByPropertyValue<Game>("Id", gameId);
                    List<Game> gamesWithSameReleaseDate = this.dataService.GetObjects<Game>().Where(x => x.ReleaseDate == game.ReleaseDate).ToList();
                    if (gamesWithSameReleaseDate.Count() > 1)
                    {
                        game.Name = gamesWithSameReleaseDate.First().Name;
                        for (var i = 1; i < gamesWithSameReleaseDate.Count(); i++)
                        {
                            game.Name += string.Concat(" / ", gamesWithSameReleaseDate[i].Name);
                        }
                    }

                    EggGroupEvaluatorViewModel model = new EggGroupEvaluatorViewModel()
                    {
                        AllPokemonWithEggGroups = eggGroupList,
                        AllPokemon = pokemonList,
                        AllAltForms = altFormsList,
                        AllOriginalPokemon = originalPokemon.ToList(),
                        AppConfig = this.appConfig,
                        SearchedPokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", pokemonId, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth"),
                        SearchedGame = game,
                        PokemonEggGroups = pokemonEggGroupList,
                        GenerationId = this.dataService.GetObjects<Generation>().Last().Id,
                    };

                    return this.PartialView("_FillDayCareEvaluator", model);
                }
                catch (Exception e)
                {
                    if (!this.User.IsInRole("Owner") && e != null)
                    {
                        string commentBody;
                        if (e != null)
                        {
                            commentBody = string.Concat(e.GetType().ToString(), " error while grabbing pokemon by egg group.");
                        }
                        else
                        {
                            commentBody = "Unknown error while grabbing pokemon by egg group.";
                        }

                        commentBody = string.Concat(commentBody, " - Pokemon Id: {", string.Join(", ", pokemonId), "}");
                        commentBody = string.Concat(commentBody, " - Game Id: {", string.Join(", ", gameId), "}");
                        Comment comment = new Comment()
                        {
                            Name = commentBody,
                        };
                        if (this.User.Identity.Name != null)
                        {
                            comment.CommentorId = this.dataService.GetObjectByPropertyValue<User>("Username", this.User.Identity.Name).Id;
                        }

                        this.dataService.AddObject(comment);
                        this.dataService.EmailComment(this.appConfig, comment);

                        return this.RedirectToAction("Error", "Index");
                    }
                    else
                    {
                        return this.RedirectToAction("Index", "Home");
                    }
                }
            }
            else
            {
                return this.RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Gets a list of pokemon with a specific form or form group.
        /// </summary>
        /// <param name="formId">The form or form group's id.</param>
        /// <returns>The fill form evaluator shared view.</returns>
        [Route("get-pokemon-by-alternate-form")]
        public IActionResult GetPokemonByAlternateForm(int formId)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<PokemonFormDetail> pokemonFormList = new List<PokemonFormDetail>();
                Form form = this.dataService.GetObjectByPropertyValue<Form>("Id", formId, "FormGroup");
                if (form.FormGroup != null)
                {
                    List<Form> formList = this.dataService.GetObjects<Form>(whereProperty: "FormGroupId", wherePropertyValue: form.FormGroupId);
                    foreach (var f in formList)
                    {
                        pokemonFormList.AddRange(this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, OriginalPokemon, Form", whereProperty: "FormId", wherePropertyValue: f.Id));
                    }
                }
                else
                {
                    pokemonFormList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, OriginalPokemon, Form", whereProperty: "FormId", wherePropertyValue: form.Id);
                }

                foreach (var f in pokemonFormList)
                {
                    f.AltFormPokemon.Name = string.Concat(f.AltFormPokemon.Name, " (", this.dataService.GetPokemonFormName(f.AltFormPokemonId), ")");
                }

                FormEvaluatorViewModel model = new FormEvaluatorViewModel()
                {
                    AllAltFormPokemon = pokemonFormList.OrderBy(x => x.AltFormPokemon.PokedexNumber).ThenBy(x => x.AltFormPokemon.Id).ToList(),
                    AppConfig = this.appConfig,
                    GenerationId = this.dataService.GetObjects<Generation>().Last().Id,
                };

                return this.PartialView("_FillFormEvaluator", model);
            }
            else
            {
                return this.RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Gets a list of types that are available in the given generation.
        /// </summary>
        /// <param name="gameId">The generation's id.</param>
        /// <returns>The fill typing evaluator types shared view.</returns>
        [Route("get-types-by-game")]
        public IActionResult GrabTypingEvaluatorTypes(int gameId)
        {
            if (gameId == 0)
            {
                gameId = this.dataService.GetObjects<Game>().Last().Id;
            }

            Game game = this.dataService.GetObjectByPropertyValue<Game>("Id", gameId);
            List<Pokedex.DataAccess.Models.Type> model = this.dataService.GetObjects<DataAccess.Models.Type>("Name").Where(x => x.GenerationId <= game.GenerationId).ToList();
            return this.PartialView("_FillTypingEvaluatorTypes", model);
        }

        /// <summary>
        /// Gets a list of abilities that are available in the given generation.
        /// </summary>
        /// <param name="generationId">The generation's id.</param>
        /// <returns>The fill ability evaluator abilities shared view.</returns>
        [Route("get-abilities-by-generation")]
        public IActionResult GrabAbilityEvaluatorAbilities(int generationId)
        {
            List<Ability> model = new List<Ability>();
            if (generationId != 0)
            {
                List<Pokemon> pokemonList = this.dataService.GetObjects<PokemonGameDetail>(includes: "Pokemon, Game", whereProperty: "Game.GenerationId", wherePropertyValue: generationId).Select(x => x.Pokemon).ToList();
                pokemonList = pokemonList.Distinct().ToList();
                List<PokemonAbilityDetail> pokemonAbilities = this.dataService.GetObjects<PokemonAbilityDetail>(includes: "PrimaryAbility, SecondaryAbility, HiddenAbility, SpecialEventAbility").Where(x => x.GenerationId <= generationId).ToList();
                pokemonAbilities = pokemonAbilities.Where(x => pokemonList.Any(y => y.Id == x.PokemonId)).ToList();
                if (pokemonAbilities.ConvertAll(x => x.PrimaryAbility).Any(x => x != null))
                {
                    model.AddRange(pokemonAbilities.ConvertAll(x => x.PrimaryAbility).Where(x => x != null).ToList());
                }

                if (pokemonAbilities.ConvertAll(x => x.SecondaryAbility).Any(x => x != null))
                {
                    model.AddRange(pokemonAbilities.ConvertAll(x => x.SecondaryAbility).Where(x => x != null).ToList());
                }

                if (pokemonAbilities.ConvertAll(x => x.HiddenAbility).Any(x => x != null))
                {
                    model.AddRange(pokemonAbilities.ConvertAll(x => x.HiddenAbility).Where(x => x != null).ToList());
                }

                if (pokemonAbilities.ConvertAll(x => x.SpecialEventAbility).Any(x => x != null))
                {
                    model.AddRange(pokemonAbilities.ConvertAll(x => x.SpecialEventAbility).Where(x => x != null).ToList());
                }

                model = model.Distinct().ToList();
            }
            else
            {
                model = this.dataService.GetObjects<Ability>().ToList();
            }

            return this.PartialView("_FillAbilityEvaluatorAbilities", model.OrderBy(x => x.Name).ToList());
        }

        /// <summary>
        /// Gets a list of months that any pages were viewed in by the year.
        /// </summary>
        /// <param name="year">The year in question.</param>
        /// <param name="pokemonPageCheck">Checks whether or not this pertains to the pokemon page stat page.</param>
        /// <returns>The fill month in year shared view.</returns>
        [Route("get-month-by-year/{year:int}/{pokemonPageCheck}")]
        public IActionResult GrabMonthByYear(int year, string pokemonPageCheck)
        {
            List<PageStat> pageStats = this.dataService.GetObjects<PageStat>("VisitDate");
            if (pokemonPageCheck == "noPokemonPage")
            {
                pageStats = pageStats.Where(x => !x.Name.Contains("Pokemon Page -")).ToList();
            }
            else
            {
                pageStats = pageStats.Where(x => x.Name.Contains("Pokemon Page -")).ToList();
            }

            pageStats = pageStats.Where(x => Convert.ToInt16(x.VisitDate.ToString("yyyy")) == year).ToList();
            List<string> model = pageStats.Select(x => x.VisitDate.ToString("MMMM")).Distinct().ToList();
            return this.PartialView("_FillMonthInYear", model);
        }

        /// <summary>
        /// Gets a list of days that any pages were viewed in by the month and year.
        /// </summary>
        /// <param name="month">The month in question.</param>
        /// <param name="year">The year in question.</param>
        /// <param name="pokemonPageCheck">Checks whether or not this pertains to the pokemon page stat page.</param>
        /// <returns>The fill day in month shared view.</returns>
        [Route("get-day-by-month/{month}/{year:int}/{pokemonPageCheck}")]
        public IActionResult GrabDayByMonth(string month, int year, string pokemonPageCheck)
        {
            List<string> model = new List<string>();
            List<PageStat> pageStats = this.dataService.GetObjects<PageStat>("VisitDate").Where(x => Convert.ToInt16(x.VisitDate.ToString("yyyy")) == year).ToList();
            pageStats = pageStats.Where(x => x.VisitDate.ToString("MMMM") == month).ToList();

            if (pokemonPageCheck == "noPokemonPage")
            {
                model = pageStats.Where(x => !x.Name.Contains("Pokemon Page -")).ToList().Select(x => x.VisitDate.ToString("dd")).Distinct().ToList();
            }
            else
            {
                model = pageStats.Where(x => x.Name.Contains("Pokemon Page -")).ToList().Select(x => x.VisitDate.ToString("dd")).Distinct().ToList();
            }

            return this.PartialView("_FillDayInMonth", model);
        }

        /// <summary>
        /// Gets a list of stats for any page that was viewed in by the day, month, and year.
        /// </summary>
        /// <param name="day">The day in question.</param>
        /// <param name="month">The month in question.</param>
        /// <param name="year">The year in question.</param>
        /// <param name="pokemonPageCheck">Checks whether or not this pertains to the pokemon page stat page.</param>
        /// <returns>The fill stats in date shared view.</returns>
        [Route("get-stats-by-date/{day:int}/{month}/{year:int}/{pokemonPageCheck}")]
        public IActionResult GrabStatsByDate(int day, string month, int year, string pokemonPageCheck)
        {
            List<PageStat> pageStats = this.dataService.GetObjects<PageStat>("VisitDate").Where(x => Convert.ToInt16(x.VisitDate.ToString("yyyy")) == year).ToList();
            pageStats = pageStats.Where(x => x.VisitDate.ToString("MMMM") == month).ToList();
            if (day != 0)
            {
                pageStats = pageStats.Where(x => Convert.ToInt16(x.VisitDate.ToString("dd")) == day).ToList();
            }

            if (pokemonPageCheck == "noPokemonPage")
            {
                pageStats = pageStats.Where(x => !x.Name.Contains("Pokemon Page -")).ToList();
                return this.PartialView("_FillStatsInDate", pageStats);
            }
            else
            {
                pageStats = pageStats.Where(x => x.Name.Contains("Pokemon Page -")).ToList();
                static PageStat Selector(PageStat x)
                {
                    x.Name = x.Name.Replace("Pokemon Page - ", string.Empty);
                    return x;
                }

                pageStats = pageStats.Select(Selector).ToList();
                List<Pokemon> allPokemon = this.dataService.GetObjects<Pokemon>("PokedexNumber, Id");
                List<Pokemon> pokemonList = new List<Pokemon>();
                Form altForm;
                Pokemon pokemon;
                foreach (var p in pageStats.Select(x => x.Name).Distinct())
                {
                    if (p.Contains("(") && !p.Contains("Nidoran"))
                    {
                        altForm = this.dataService.GetObjectByPropertyValue<Form>("Name", p.Split("(")[1].Replace(")", string.Empty));
                        pokemon = this.dataService.GetPokemonFromNameAndFormName(p.Split(" (")[0], altForm.Id);
                        pokemon.Name = string.Concat(pokemon.Name, " (", altForm.Name, ")");
                        pokemonList.Add(pokemon);
                    }
                    else
                    {
                        pokemonList.Add(allPokemon.First(x => x.Name == p));
                    }
                }

                PokemonPageStatsViewModel model = new PokemonPageStatsViewModel()
                {
                    PokemonList = pokemonList.OrderBy(x => x.PokedexNumber).ThenBy(x => x.Id).ToList(),
                    PageStatList = pageStats,
                    AppConfig = this.appConfig,
                    Generation = this.dataService.GetObjects<Generation>().Last(),
                };

                return this.PartialView("_FillPokemonStatsInDate", model);
            }
        }

        /// <summary>
        /// Gets and sorts a list of pokemon by their popularity on the site.
        /// </summary>
        /// <returns>The fill pokemon stats by popularity shared view.</returns>
        [Route("get-stats-by-popularity")]
        public IActionResult GrabStatsByPopularity()
        {
            List<PageStat> pageStats = this.dataService.GetObjects<PageStat>().Where(x => x.Name.Contains("Pokemon Page -")).ToList();
            static PageStat Selector(PageStat x)
            {
                x.Name = x.Name.Replace("Pokemon Page - ", string.Empty);
                return x;
            }

            pageStats = pageStats.Select(Selector).ToList();
            List<Pokemon> allPokemon = this.dataService.GetObjects<Pokemon>("PokedexNumber, Id");
            List<Pokemon> unOrderedPokemonList = new List<Pokemon>();
            List<Pokemon> pokemonList = new List<Pokemon>();
            Form altForm;
            Pokemon pokemon;
            int viewCount = 0;
            List<IGrouping<string, string>> pokemonNames = pageStats.Select(x => x.Name).ToList().GroupBy(x => x).OrderByDescending(x => x.Count()).ToList();
            foreach (var p in pokemonNames)
            {
                if (p.Count() != viewCount)
                {
                    viewCount = p.Count();
                    pokemonList = pokemonList.Concat(unOrderedPokemonList.OrderBy(x => x.PokedexNumber).ThenBy(x => x.Id).ToList()).ToList();
                    unOrderedPokemonList = new List<Pokemon>();
                }

                if (p.Key.Contains("(") && !p.Key.Contains("Nidoran"))
                {
                    altForm = this.dataService.GetObjectByPropertyValue<Form>("Name", p.Key.Split("(")[1].Replace(")", string.Empty));
                    pokemon = this.dataService.GetPokemonFromNameAndFormName(p.Key.Split(" (")[0], altForm.Id);
                    pokemon.Name = string.Concat(pokemon.Name, " (", altForm.Name, ")");
                    unOrderedPokemonList.Add(pokemon);
                }
                else
                {
                    unOrderedPokemonList.Add(allPokemon.First(x => x.Name == p.Key));
                }
            }

            pokemonList = pokemonList.Concat(unOrderedPokemonList.OrderBy(x => x.PokedexNumber).ThenBy(x => x.Id).ToList()).ToList();

            PokemonPageStatsViewModel model = new PokemonPageStatsViewModel()
            {
                PokemonList = pokemonList,
                PageStatList = pageStats,
                AppConfig = this.appConfig,
                Generation = this.dataService.GetObjects<Generation>().Last(),
            };

            return this.PartialView("_FillPokemonStatsByPopularity", model);
        }

        /// <summary>
        /// Gets the type chart for the given typing and generation.
        /// </summary>
        /// <param name="primaryTypeId">The id of the primary type.</param>
        /// <param name="secondaryTypeId">The id of the secondary type.</param>
        /// <param name="generationId">The id of the generation.</param>
        /// <returns>Returns the type effectiveness given the typing and generation.</returns>
        [Route("get-type-chart-typing")]
        public TypeEffectivenessViewModel GetTypeChartTyping(int primaryTypeId, int secondaryTypeId, int generationId)
        {
            List<DataAccess.Models.Type> typeList = this.dataService.GetObjects<DataAccess.Models.Type>("Name");
            List<DataAccess.Models.Type> pokemonTypes = new List<DataAccess.Models.Type>();
            List<string> strongAgainst = new List<string>();
            List<string> superStrongAgainst = new List<string>();
            List<string> weakAgainst = new List<string>();
            List<string> superWeakAgainst = new List<string>();
            List<string> immuneTo = new List<string>();
            List<TypeChart> typeChart;
            string effectiveValue, attackType;

            pokemonTypes.Add(this.dataService.GetObjectByPropertyValue<DataAccess.Models.Type>("Id", primaryTypeId));

            if (secondaryTypeId != 0)
            {
                pokemonTypes.Add(this.dataService.GetObjectByPropertyValue<DataAccess.Models.Type>("Id", secondaryTypeId));
            }

            foreach (var type in pokemonTypes)
            {
                typeChart = this.dataService.GetObjects<TypeChart>(includes: "Attack, Defend", whereProperty: "DefendId", wherePropertyValue: type.Id);

                List<int> generations = typeChart.Select(x => x.GenerationId).Distinct().OrderByDescending(x => x).ToList();
                if (generationId != 0)
                {
                    typeChart = typeChart.Where(x => x.GenerationId == generations.First(x => x <= generationId)).ToList();
                }
                else
                {
                    typeChart = typeChart.Where(x => x.GenerationId == generations.First()).ToList();
                }

                foreach (var t in typeList)
                {
                    if (typeChart.Exists(x => x.Attack.Name == t.Name))
                    {
                        attackType = t.Name;
                        effectiveValue = typeChart.Find(x => x.Attack.Name == attackType).Effective.ToString("0.####");
                        if (effectiveValue == "0")
                        {
                            strongAgainst.Remove(attackType);
                            weakAgainst.Remove(attackType);
                            immuneTo.Add(attackType);
                        }
                        else if (effectiveValue == "0.5" && immuneTo.Where(x => x == attackType).ToList().Count == 0)
                        {
                            if (strongAgainst.Exists(x => x == attackType))
                            {
                                strongAgainst.Remove(attackType);
                                superStrongAgainst.Add(string.Concat(attackType, " Quad"));
                            }
                            else if (weakAgainst.Exists(x => x == attackType))
                            {
                                weakAgainst.Remove(attackType);
                            }
                            else
                            {
                                strongAgainst.Add(attackType);
                            }
                        }
                        else if (effectiveValue == "2" && immuneTo.Where(x => x == attackType).ToList().Count == 0)
                        {
                            if (weakAgainst.Exists(x => x == attackType))
                            {
                                weakAgainst.Remove(attackType);
                                superWeakAgainst.Add(string.Concat(attackType, " Quad"));
                            }
                            else if (strongAgainst.Exists(x => x == attackType))
                            {
                                strongAgainst.Remove(attackType);
                            }
                            else
                            {
                                weakAgainst.Add(attackType);
                            }
                        }
                    }
                }
            }

            immuneTo.Sort();
            strongAgainst.Sort();
            superStrongAgainst.Sort();
            weakAgainst.Sort();
            superWeakAgainst.Sort();

            TypeEffectivenessViewModel effectivenessChart = new TypeEffectivenessViewModel()
            {
                ImmuneTo = immuneTo,
                StrongAgainst = strongAgainst,
                WeakAgainst = weakAgainst,
                SuperStrongAgainst = superStrongAgainst,
                SuperWeakAgainst = superWeakAgainst,
            };

            return effectivenessChart;
        }

        /// <summary>
        /// Marks down that a shiny pokemon was generated while randomizing a team.
        /// </summary>
        [Route("shiny-pokemon-found")]
        public void ShinyPokemonFound()
        {
            this.dataService.AddPageView("Shiny Pokemon found in Team Randomizer", false);
        }

        /// <summary>
        /// Grabs all of the pokemon available in the selected game.
        /// </summary>
        /// <param name="gameId">The selected game's Id.</param>
        /// <returns>The list of available pokemon.</returns>
        [Route("get-pokemon-by-game")]
        public List<Pokemon> GetPokemonByGame(int gameId)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<Pokemon> pokemonList = this.dataService.GetObjects<PokemonGameDetail>("Pokemon.PokedexNumber, Pokemon.Id", "Pokemon", "GameId", gameId).ConvertAll(x => x.Pokemon);
                List<Pokemon> altFormsList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, Form").ConvertAll(x => x.AltFormPokemon);
                foreach (var p in pokemonList.Where(x => altFormsList.Any(y => y.Id == x.Id)))
                {
                    p.Name = string.Concat(p.Name, " (", this.dataService.GetObjectByPropertyValue<PokemonFormDetail>("AltFormPokemonId", p.Id, "Form").Form.Name, ")");
                }

                return pokemonList;
            }

            return null;
        }

        /// <summary>
        /// Grabs all of the hunting methods available for the selected game.
        /// </summary>
        /// <param name="gameId">The selected game's Id.</param>
        /// <returns>The list of available hunting methods.</returns>
        [Route("get-hunting-methods")]
        public List<HuntingMethod> GetHuntingMethods(int gameId)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<HuntingMethod> huntingMethods = this.dataService.GetObjects<HuntingMethod>();

                return huntingMethods;
            }

            return null;
        }

        /// <summary>
        /// Gets a list of all pokemon that are not alternate forms.
        /// </summary>
        /// <returns>Returns the list of original pokemon.</returns>
        private List<Pokemon> GetAllPokemonWithoutForms()
        {
            List<Pokemon> pokemonList = this.dataService.GetAllPokemon();
            List<Pokemon> altFormList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form").ConvertAll(x => x.AltFormPokemon);
            return pokemonList.Where(x => !altFormList.Any(y => y.Id == x.Id)).ToList();
        }

        private List<PokemonEggGroupDetail> GetAllPokemonWithSpecificEggGroups(int primaryEggGroupId, int? secondaryEggGroupId)
        {
            List<PokemonEggGroupDetail> pokemonList = this.dataService.GetObjects<PokemonEggGroupDetail>(includes: "Pokemon, Pokemon.GenderRatio, PrimaryEggGroup, SecondaryEggGroup").OrderBy(x => x.GenerationId).GroupBy(x => new { x.PokemonId }).Select(x => x.LastOrDefault()).ToList();

            List<PokemonEggGroupDetail> finalPokemonList = pokemonList.Where(x => x.PrimaryEggGroupId == primaryEggGroupId || x.SecondaryEggGroupId == primaryEggGroupId).ToList();

            if (secondaryEggGroupId != null)
            {
                finalPokemonList.AddRange(pokemonList.Where(x => x.PrimaryEggGroupId == secondaryEggGroupId || x.SecondaryEggGroupId == secondaryEggGroupId).ToList());
            }

            return finalPokemonList.Distinct().ToList();
        }

        private List<PokemonAbilityDetail> GetPokemonByAbilityAndGeneration(int abilityId, int generationId)
        {
            List<PokemonAbilityDetail> pokemonList = new List<PokemonAbilityDetail>();
            if (generationId != 0)
            {
                pokemonList = this.dataService.GetObjects<PokemonAbilityDetail>(includes: "Pokemon, Pokemon.Game")
                    .Where(x => x.Pokemon.Game.GenerationId <= generationId)
                    .Where(x => x.GenerationId <= generationId)
                    .OrderBy(x => x.GenerationId)
                    .GroupBy(x => new { x.PokemonId })
                    .Select(x => x.LastOrDefault())
                    .ToList();

                List<int> exclusionList = pokemonList.Select(x => x.PokemonId).Except(this.dataService.GetObjects<PokemonGameDetail>(includes: "Pokemon, Game, Game.Generation", whereProperty: "Game.GenerationId", wherePropertyValue: generationId).Select(x => x.PokemonId)).ToList();

                foreach (var pokemon in exclusionList)
                {
                    pokemonList.Remove(pokemonList.Find(x => x.PokemonId == pokemon));
                }
            }
            else
            {
                pokemonList = this.dataService.GetObjects<PokemonAbilityDetail>(includes: "Pokemon, Pokemon.Game")
                    .GroupBy(x => new { x.PokemonId })
                    .Select(x => x.LastOrDefault())
                    .ToList();
            }

            return pokemonList.Where(x => x.PrimaryAbilityId == abilityId || x.SecondaryAbilityId == abilityId || x.HiddenAbilityId == abilityId || x.SpecialEventAbilityId == abilityId).OrderBy(x => x.Pokemon.PokedexNumber).ThenBy(x => x.PokemonId).ToList();
        }

        private string FillEVs(PokemonTeamEV evs)
        {
            string evString = string.Empty;
            if (evs.EVTotal == 0)
            {
                return "\nEVs: 1 HP / 1 Atk / 1 Def / 1 SpA / 1 SpD / 1 Spe";
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

                return string.Concat("\nEVs: ", evString);
            }
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

        private string ExportPokemonTeam(List<int> pokemonIdList, List<string> abilityList, bool exportAbilities)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                try
                {
                    string pokemonTeam = string.Empty;
                    Pokemon pokemon;

                    for (var i = 0; i < pokemonIdList.Count; i++)
                    {
                        if (i != 0)
                        {
                            pokemonTeam = string.Concat(pokemonTeam, "\n\n");
                        }

                        pokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", pokemonIdList[i], "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth");
                        string pokemonName = pokemon.Name;
                        if (this.dataService.CheckIfAltForm(pokemon.Id))
                        {
                            string pokemonForm = this.GetFormDetails(pokemon.Id);
                            pokemonName = string.Concat(pokemonName, "-", pokemonForm);
                        }

                        pokemonTeam = string.Concat(pokemonTeam, pokemonName);
                        if (exportAbilities && this.dataService.GetObjectByPropertyValue<PokemonAbilityDetail>("PokemonId", pokemonIdList[i]) != null)
                        {
                            pokemonTeam = string.Concat(pokemonTeam, "\nAbility: ", abilityList[i]);
                        }

                        pokemonTeam = string.Concat(pokemonTeam, "\nEVs: 1 HP / 1 Atk / 1 Def / 1 SpA / 1 SpD / 1 Spe");
                    }

                    return pokemonTeam;
                }
                catch (Exception e)
                {
                    if (!this.User.IsInRole("Owner") && e != null)
                    {
                        string commentBody;
                        if (e != null)
                        {
                            commentBody = string.Concat(e.GetType().ToString(), " error during the pokemon team's export.");
                        }
                        else
                        {
                            commentBody = "Unknown error during the pokemon team's export.";
                        }

                        commentBody = string.Concat(commentBody, " - Pokemon Id List: {", string.Join(", ", pokemonIdList), "}");
                        commentBody = string.Concat(commentBody, " - Ability List: {", string.Join(", ", abilityList), "}");
                        commentBody = string.Concat(commentBody, " - Export Abilities? ", exportAbilities.ToString());
                        Comment comment = new Comment()
                        {
                            Name = commentBody,
                        };
                        if (this.User.Identity.Name != null)
                        {
                            comment.CommentorId = this.dataService.GetObjectByPropertyValue<User>("Username", this.User.Identity.Name).Id;
                        }

                        this.dataService.AddObject(comment);
                        this.dataService.EmailComment(this.appConfig, comment);
                    }
                }

                return null;
            }

            return null;
        }

        private string GetFormDetails(int pokemonId)
        {
            string formDetails = string.Empty, itemName = string.Empty;
            PokemonFormDetail pokemonFormDetail = this.dataService.GetObjectByPropertyValue<PokemonFormDetail>("AltFormPokemonId", pokemonId, "AltFormPokemon, OriginalPokemon, Form");

            formDetails = string.Concat(formDetails, pokemonFormDetail.Form.Name.Replace(' ', '-').Replace("Gigantamax", "Gmax"));

            List<FormItem> formItems = this.dataService.GetObjects<FormItem>();
            FormItem formItem = formItems.Find(x => x.PokemonId == pokemonId);
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
            while (pumpkabooCount.Count > 1)
            {
                pokemonList.Remove(pumpkabooCount[rnd.Next(pumpkabooCount.Count)]);
                pumpkabooCount = pokemonList.Where(x => x.PokedexNumber == 710).ToList();
            }

            List<Pokemon> gourgeistCount = pokemonList.Where(x => x.PokedexNumber == 711).ToList();
            while (gourgeistCount.Count > 1)
            {
                pokemonList.Remove(gourgeistCount[rnd.Next(gourgeistCount.Count)]);
                gourgeistCount = pokemonList.Where(x => x.PokedexNumber == 711).ToList();
            }

            List<Pokemon> starterForms = this.dataService.GetObjects<PokemonFormDetail>(includes: "OriginalPokemon, AltFormPokemon, Form", whereProperty: "Form.Name", wherePropertyValue: "Starter").ConvertAll(x => x.AltFormPokemon);
            List<Pokemon> lgpeStarterCount = pokemonList.Where(x => starterForms.Any(y => y.Id == x.Id)).ToList();
            while (lgpeStarterCount.Count > 1)
            {
                pokemonList.Remove(lgpeStarterCount[rnd.Next(lgpeStarterCount.Count)]);
                lgpeStarterCount = pokemonList.Where(x => starterForms.Any(y => y.Id == x.Id)).ToList();
            }

            return pokemonList;
        }

        private List<PokemonTypeDetail> GetAllPokemonWithSpecificTypes(int primaryTypeId, int secondaryTypeId, Game game)
        {
            List<PokemonTypeDetail> pokemonList = this.dataService.GetObjects<PokemonTypeDetail>("GenerationId", "Pokemon, Pokemon.Game, PrimaryType, SecondaryType")
                                                        .Where(x => x.GenerationId <= game.GenerationId)
                                                        .GroupBy(x => new { x.PokemonId })
                                                        .Select(x => x.LastOrDefault())
                                                        .ToList();

            if (secondaryTypeId != 0 && secondaryTypeId != 100)
            {
                pokemonList = pokemonList.Where(x => (x.PrimaryTypeId == primaryTypeId && x.SecondaryTypeId == secondaryTypeId) || (x.PrimaryTypeId == secondaryTypeId && x.SecondaryTypeId == primaryTypeId)).ToList();
            }
            else if (secondaryTypeId == 100)
            {
                pokemonList = pokemonList.Where(x => x.PrimaryTypeId == primaryTypeId || x.SecondaryTypeId == primaryTypeId).ToList();
            }
            else
            {
                pokemonList = pokemonList.Where(x => x.PrimaryTypeId == primaryTypeId && x.SecondaryType == null).ToList();
            }

            if (game.Id != 0)
            {
                List<int> exclusionList = pokemonList.Select(x => x.PokemonId).Except(this.dataService.GetObjects<PokemonGameDetail>(includes: "Pokemon, Game", whereProperty: "GameId", wherePropertyValue: game.Id).Select(x => x.PokemonId)).ToList();

                foreach (var pokemon in exclusionList)
                {
                    pokemonList.Remove(pokemonList.Find(x => x.PokemonId == pokemon));
                }
            }

            return pokemonList.OrderBy(x => x.Pokemon.PokedexNumber).ToList();
        }

        private List<PokemonTypeDetail> GetAllPokemonWithSpecificType(int typeId, int generationId, List<Pokemon> allPokemon)
        {
            try
            {
                List<PokemonTypeDetail> pokemonList = this.dataService.GetObjects<PokemonTypeDetail>(includes: "Pokemon, Pokemon.Game, PrimaryType, SecondaryType")
                                                            .Where(x => allPokemon.Any(y => y.Id == x.PokemonId))
                                                            .OrderBy(x => x.GenerationId)
                                                            .ToList();

                if (generationId != 0)
                {
                    pokemonList = pokemonList.Where(x => x.GenerationId <= generationId).ToList();

                    List<int> exclusionList = pokemonList.Select(x => x.PokemonId).Except(this.dataService.GetObjects<PokemonGameDetail>(includes: "Pokemon, Game, Game.Generation", whereProperty: "Game.GenerationId", wherePropertyValue: generationId).Select(x => x.PokemonId)).ToList();

                    foreach (var pokemon in exclusionList)
                    {
                        pokemonList.Remove(pokemonList.Find(x => x.PokemonId == pokemon));
                    }
                }

                pokemonList = pokemonList.GroupBy(x => new { x.PokemonId }).Select(x => x.LastOrDefault()).Where(x => x.PrimaryTypeId == typeId || x.SecondaryTypeId == typeId).ToList();

                return pokemonList.OrderBy(x => x.Pokemon.PokedexNumber).ToList();
            }
            catch (Exception e)
            {
                if (!this.User.IsInRole("Owner") && e != null)
                {
                    string commentBody;
                    if (e != null)
                    {
                        commentBody = string.Concat(e.GetType().ToString(), " error during retrieval of pokemon's specific type.");
                    }
                    else
                    {
                        commentBody = "Unknown error during retrieval of pokemon's specific type.";
                    }

                    commentBody = string.Concat(commentBody, " - Type Id: ", typeId);
                    commentBody = string.Concat(commentBody, " - Generation Id: ", generationId);
                    commentBody = string.Concat(commentBody, " - Pokemon List: ", allPokemon.ToString());
                    Comment comment = new Comment()
                    {
                        Name = commentBody,
                    };
                    if (this.User.Identity.Name != null)
                    {
                        comment.CommentorId = this.dataService.GetObjectByPropertyValue<User>("Username", this.User.Identity.Name).Id;
                    }

                    this.dataService.AddObject(comment);
                    this.dataService.EmailComment(this.appConfig, comment);
                }
            }

            return null;
        }

        private ExportPokemonViewModel ExportTeamString(PokemonTeam team)
        {
            List<PokemonTeamDetail> pokemonList = team.GrabPokemonTeamDetails;
            if (pokemonList.Count > 0)
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

                for (var i = 0; i < pokemonList.Count; i++)
                {
                    if (i != 0)
                    {
                        pokemonTeam.ExportString = string.Concat(pokemonTeam.ExportString, "\n\n");
                    }

                    pokemonTeam.ExportString = string.Concat(pokemonTeam.ExportString, this.FillUserPokemonTeam(pokemonList[i], team.GameId));
                }

                return pokemonTeam;
            }
            else
            {
                return null;
            }
        }

        private string FillUserPokemonTeam(PokemonTeamDetail pokemonTeamDetail, int? gameId)
        {
            try
            {
                int generationId = 0;
                if (gameId != null)
                {
                    generationId = this.dataService.GetObjectByPropertyValue<Game>("Id", gameId).GenerationId;
                }

                Pokemon pokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", pokemonTeamDetail.PokemonId, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth");
                List<string> pokemonForm = new List<string>();
                string pokemonName = string.Empty;

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
                if ((pokemon.Id != 1760 && this.dataService.CheckIfAltForm(pokemon.Id)) || pokemon.Id == 1692)
                {
                    pokemonForm = this.GetUserFormDetails(pokemon.Id);
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
                if (generationId != 1 && generationId != 2 && gameId != 37)
                {
                    if (pokemonTeamDetail.Ability != null)
                    {
                        pokemonTeamString = string.Concat(pokemonTeamString, "\nAbility: ", pokemonTeamDetail.Ability.Name);
                    }
                    else
                    {
                        pokemonTeamString = string.Concat(pokemonTeamString, "\nAbility: No Ability");
                    }
                }

                if (pokemonTeamDetail.Level < 100)
                {
                    pokemonTeamString = string.Concat(pokemonTeamString, "\nLevel: ", pokemonTeamDetail.Level.ToString());
                }

                if (pokemonTeamDetail.IsShiny && generationId != 1)
                {
                    pokemonTeamString = string.Concat(pokemonTeamString, "\nShiny: Yes");
                }

                if ((gameId == 41 || gameId == 42) && pokemonTeamDetail.TeraType != null)
                {
                    pokemonTeamString = string.Concat(pokemonTeamString, "\nTera Type: ", pokemonTeamDetail.TeraType.Name);
                }

                if (pokemonTeamDetail.Happiness < 255 && generationId != 1)
                {
                    pokemonTeamString = string.Concat(pokemonTeamString, "\nHappiness: ", pokemonTeamDetail.Happiness.ToString());
                }

                pokemonTeamString = string.Concat(pokemonTeamString, this.FillEVs(pokemonTeamDetail.PokemonTeamEV));

                if (pokemonTeamDetail.Nature != null && generationId != 1 && generationId != 2)
                {
                    pokemonTeamString = string.Concat(pokemonTeamString, "\n", pokemonTeamDetail.Nature.Name, " Nature");
                }

                pokemonTeamString = string.Concat(pokemonTeamString, this.FillIVs(pokemonTeamDetail.PokemonTeamIV), this.FillMoveset(pokemonTeamDetail.PokemonTeamMoveset));

                return pokemonTeamString;
            }
            catch (Exception e)
            {
                if (!this.User.IsInRole("Owner") && e != null)
                {
                    string commentBody;
                    if (e != null)
                    {
                        commentBody = string.Concat(e.GetType().ToString(), " error during filling the user's pokemon team.");
                    }
                    else
                    {
                        commentBody = "Unknown error during filling the user's pokemon team.";
                    }

                    commentBody = string.Concat(commentBody, " - Pokemon Team Detail Id: ", pokemonTeamDetail.Id);
                    commentBody = string.Concat(commentBody, " - Game Id: ", gameId.HasValue ? gameId.Value.ToString() : "Null");
                    Comment comment = new Comment()
                    {
                        Name = commentBody,
                    };
                    if (this.User.Identity.Name != null)
                    {
                        comment.CommentorId = this.dataService.GetObjectByPropertyValue<User>("Username", this.User.Identity.Name).Id;
                    }

                    this.dataService.AddObject(comment);
                    this.dataService.EmailComment(this.appConfig, comment);
                }
            }

            return null;
        }

        private List<string> GetUserFormDetails(int pokemonId)
        {
            string form = string.Empty, itemName = string.Empty;
            List<string> formDetails = new List<string>();
            if (pokemonId == 1692)
            {
                form = "Four";
            }
            else
            {
                PokemonFormDetail pokemonFormDetail = this.dataService.GetObjectByPropertyValue<PokemonFormDetail>("AltFormPokemonId", pokemonId, "AltFormPokemon, OriginalPokemon, Form");
                form = string.Concat(form, pokemonFormDetail.Form.Name.Replace(' ', '-').Replace("Gigantamax", "Gmax"));
            }

            List<FormItem> formItems = this.dataService.GetObjects<FormItem>();
            FormItem formItem = formItems.Find(x => x.PokemonId == pokemonId);
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
    }
}
