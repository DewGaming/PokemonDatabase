using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MoreLinq;
using Pokedex.DataAccess.Models;
using Pokedex.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;

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
                            model.PokemonList.Add(pokemonList.Find(x => x.Id == id));
                        }
                    }
                    catch
                    {
                        model.PokemonList.Add(pokemonList.Find(x => x.Id == id));
                    }
                }

                model.PokemonList.Where(x => altFormList.Any(y => y.Id == x.Id)).ToList().ForEach(x => x.Name = this.dataService.GetAltFormWithFormName(x.Id).Name);

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
                List<Pokemon> altFormList = this.dataService.GetObjects<PokemonFormDetail>().ConvertAll(x => x.AltFormPokemon);
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
                        webRequest = (HttpWebRequest)WebRequest.Create(string.Concat(this.appConfig.WebUrl, this.appConfig.ShinyPokemonImageUrl, id, ".png"));
                        imageRequest = (HttpWebResponse)webRequest.GetResponse();
                        if (imageRequest.StatusCode != HttpStatusCode.OK)
                        {
                            model.PokemonList.Add(pokemonList.Find(x => x.Id == id));
                        }
                    }
                    catch
                    {
                        model.PokemonList.Add(pokemonList.Find(x => x.Id == id));
                    }
                }

                model.PokemonList.Where(x => altFormList.Any(y => y.Id == x.Id)).ToList().ForEach(x => x.Name = this.dataService.GetAltFormWithFormName(x.Id).Name);

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
            PokemonTeam pokemonTeam = this.dataService.GetObjectByPropertyValue<PokemonTeam>("Id", teamId, "Game, FirstPokemon, FirstPokemon.Pokemon, FirstPokemon.Pokemon.Game, FirstPokemon.Ability, FirstPokemon.PokemonTeamEV, FirstPokemon.PokemonTeamIV, FirstPokemon.PokemonTeamMoveset, FirstPokemon.Nature, FirstPokemon.TeraType, SecondPokemon, SecondPokemon.Pokemon, SecondPokemon.Pokemon.Game, SecondPokemon.Ability, SecondPokemon.PokemonTeamEV, SecondPokemon.PokemonTeamIV, SecondPokemon.PokemonTeamMoveset, SecondPokemon.Nature, SecondPokemon.TeraType, ThirdPokemon, ThirdPokemon.Pokemon, ThirdPokemon.Pokemon.Game, ThirdPokemon.Ability, ThirdPokemon.PokemonTeamEV, ThirdPokemon.PokemonTeamIV, ThirdPokemon.PokemonTeamMoveset, ThirdPokemon.Nature, ThirdPokemon.TeraType, FourthPokemon, FourthPokemon.Pokemon, FourthPokemon.Pokemon.Game, FourthPokemon.Ability, FourthPokemon.PokemonTeamEV, FourthPokemon.PokemonTeamIV, FourthPokemon.PokemonTeamMoveset, FourthPokemon.Nature, FourthPokemon.TeraType, FifthPokemon, FifthPokemon.Pokemon, FifthPokemon.Pokemon.Game, FifthPokemon.Ability, FifthPokemon.PokemonTeamEV, FifthPokemon.PokemonTeamIV, FifthPokemon.PokemonTeamMoveset, FifthPokemon.Nature, FifthPokemon.TeraType, SixthPokemon, SixthPokemon.Pokemon, SixthPokemon.Pokemon.Game, SixthPokemon.Ability, SixthPokemon.PokemonTeamEV, SixthPokemon.PokemonTeamIV, SixthPokemon.PokemonTeamMoveset, SixthPokemon.Nature, SixthPokemon.TeraType, User");
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
                List<PokemonTeam> pokemonTeams = this.dataService.GetObjects<PokemonTeam>("Id", "Game, FirstPokemon, FirstPokemon.Pokemon, FirstPokemon.Pokemon.Game, FirstPokemon.Ability, FirstPokemon.PokemonTeamEV, FirstPokemon.PokemonTeamIV, FirstPokemon.PokemonTeamMoveset, FirstPokemon.Nature, FirstPokemon.TeraType, SecondPokemon, SecondPokemon.Pokemon, SecondPokemon.Pokemon.Game, SecondPokemon.Ability, SecondPokemon.PokemonTeamEV, SecondPokemon.PokemonTeamIV, SecondPokemon.PokemonTeamMoveset, SecondPokemon.Nature, SecondPokemon.TeraType, ThirdPokemon, ThirdPokemon.Pokemon, ThirdPokemon.Pokemon.Game, ThirdPokemon.Ability, ThirdPokemon.PokemonTeamEV, ThirdPokemon.PokemonTeamIV, ThirdPokemon.PokemonTeamMoveset, ThirdPokemon.Nature, ThirdPokemon.TeraType, FourthPokemon, FourthPokemon.Pokemon, FourthPokemon.Pokemon.Game, FourthPokemon.Ability, FourthPokemon.PokemonTeamEV, FourthPokemon.PokemonTeamIV, FourthPokemon.PokemonTeamMoveset, FourthPokemon.Nature, FourthPokemon.TeraType, FifthPokemon, FifthPokemon.Pokemon, FifthPokemon.Pokemon.Game, FifthPokemon.Ability, FifthPokemon.PokemonTeamEV, FifthPokemon.PokemonTeamIV, FifthPokemon.PokemonTeamMoveset, FifthPokemon.Nature, FifthPokemon.TeraType, SixthPokemon, SixthPokemon.Pokemon, SixthPokemon.Pokemon.Game, SixthPokemon.Ability, SixthPokemon.PokemonTeamEV, SixthPokemon.PokemonTeamIV, SixthPokemon.PokemonTeamMoveset, SixthPokemon.Nature, SixthPokemon.TeraType, User", "User.Username", this.User.Identity.Name);
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
        /// <param name="shinyPokemonList">The list of trues and falses if the pokemon generated is shiny.</param>
        /// <param name="abilityIdList">The list of abilities generated alongside the pokemon.</param>
        /// <param name="exportAbilities">Check to see if generated abilities are to be exported.</param>
        /// <param name="pokemonTeamName">The given pokemon team name. Defaults to "Save from Team Randomizer" if no name given.</param>
        /// <returns>The string confirming the team has been generated. Tells a user they need to be logged in if they aren't.</returns>
        [AllowAnonymous]
        [Route("save-pokemon-team")]
        public string SavePokemonTeam(int selectedGame, List<int> pokemonIdList, List<bool> shinyPokemonList, List<int> abilityIdList, bool exportAbilities, string pokemonTeamName)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                try
                {
                    // Used for testing errors.
                    // selectedGame = 0;
                    // pokemonIdList = new List<int>()
                    // {
                    //     1552, 1386, 1433, 1423, 1432, 1405
                    // };
                    // shinyPokemonList = new List<bool>()
                    // {
                    //     false, false, false, false, false, false
                    // };
                    // abilityIdList = new List<int>()
                    // {
                    //     107, 49, 51, 236, 161, 162
                    // };
                    // exportAbilities = false;
                    // pokemonTeamName = "test";
                    if (string.IsNullOrEmpty(pokemonTeamName))
                    {
                        pokemonTeamName = "Save from Team Randomizer";
                    }

                    if (this.dataService.GetCurrentUser(this.User) != null)
                    {
                        Game game = this.dataService.GetObjectByPropertyValue<Game>("Id", selectedGame);
                        this.dataService.AddPageView("Save Pokemon Team from Team Randomizer", this.User.IsInRole("Owner"));
                        PokemonTeam pokemonTeam = new PokemonTeam()
                        {
                            PokemonTeamName = pokemonTeamName,
                            UserId = this.dataService.GetCurrentUser(this.User).Id,
                        };

                        if (selectedGame != 0)
                        {
                            pokemonTeam.GameId = selectedGame;
                        }

                        Pokemon pokemon;
                        Ability ability = null;
                        PokemonTeamDetail pokemonTeamDetail;

                        for (var i = 0; i < pokemonIdList.Count; i++)
                        {
                            pokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", pokemonIdList[i], "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth");

                            pokemonTeamDetail = new PokemonTeamDetail()
                            {
                                PokemonId = pokemon.Id,
                                NatureId = this.dataService.GetObjectByPropertyValue<Nature>("Name", "Serious").Id,
                                Level = 100,
                            };

                            if (selectedGame == 0 || game.GenerationId >= 3)
                            {
                                if (exportAbilities)
                                {
                                    ability = this.dataService.GetObjectByPropertyValue<Ability>("Id", abilityIdList[i]);
                                }

                                if (ability != null)
                                {
                                    pokemonTeamDetail.AbilityId = ability.Id;
                                }
                            }

                            if ((selectedGame == 0 || game.GenerationId >= 2) && shinyPokemonList[i])
                            {
                                pokemonTeamDetail.IsShiny = true;
                            }

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
                catch (Exception e)
                {
                    if (!this.User.IsInRole("Owner"))
                    {
                        string commentBody;
                        Game game = this.dataService.GetObjectByPropertyValue<Game>("Id", selectedGame);

                        if (e != null)
                        {
                            commentBody = string.Concat(e.GetType().ToString(), " error while saving generated team.");
                        }
                        else
                        {
                            commentBody = "Unknown error while saving generated team.";
                        }

                        if (game != null)
                        {
                            commentBody = string.Concat(commentBody, " - Selected Game: ", game.Name);
                        }
                        else
                        {
                            commentBody = string.Concat(commentBody, " - Selected Game: ", selectedGame);
                        }

                        commentBody = string.Concat(commentBody, " - Generated Pokemon Ids: {", string.Join(", ", pokemonIdList), "}");
                        commentBody = string.Concat(commentBody, " - Generated Shiny Pokemon: {", string.Join(", ", shinyPokemonList), "}");
                        commentBody = string.Concat(commentBody, " - Generated Ability List: {", string.Join(", ", abilityIdList), "}");
                        commentBody = string.Concat(commentBody, " - Export Abilities: ", exportAbilities);
                        commentBody = string.Concat(commentBody, " - Pokemon Team Name: ", pokemonTeamName);

                        Comment comment = new Comment()
                        {
                            Name = commentBody,
                        };

                        if (this.User.Identity.Name != null)
                        {
                            comment.CommentorId = this.dataService.GetCurrentUser(this.User).Id;
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
        /// <param name="selectedTypeId">The specific type for generation.</param>
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
        /// <param name="monotypeOnly">Whether or not only monotypes are allowed for generation.</param>
        /// <param name="noRepeatType">Whether or not repeat types are allowed for generation.</param>
        /// <param name="allowIncomplete">Whether or not incomplete pokemon can appear.</param>
        /// <returns>The view model of the generated pokemon team.</returns>
        [Route("get-pokemon-team")]
        public TeamRandomizerViewModel GetPokemonTeam(int pokemonCount, List<int> selectedGens, int selectedGameId, int selectedTypeId, List<string> selectedLegendaries, List<string> selectedForms, List<string> selectedEvolutions, bool needsStarter, bool onlyLegendaries, bool onlyAltForms, bool multipleMegas, bool multipleGMax, bool onePokemonForm, bool randomAbility, bool monotypeOnly, bool noRepeatType, bool allowIncomplete)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<Pokemon> pokemonList = new List<Pokemon>();
                List<int> testPokemonIds = new List<int>();

                // Modify to test errors in team generation.
                // selectedGameId = this.dataService.GetObjectByPropertyValue<Game>("Name", "").Id;
                // selectedTypeId = this.dataService.GetObjectByPropertyValue<Pokedex.DataAccess.Models.Type>("Name", "").Id;
                // pokemonCount = 6;
                // testPokemonIds = new List<int>() {  };
                // selectedGens = new List<int>() {  };
                // selectedEvolutions = new List<string>() {  };
                // selectedForms = new List<string>() {  };
                // selectedLegendaries = new List<string>() {  };
                // needsStarter = false;
                // onlyLegendaries = false;
                // onlyAltForms = false;
                // multipleMegas = false;
                // multipleGMax = false;
                // onePokemonForm = true;
                // randomAbility = false;
                // monotypeOnly = false;
                // noRepeatType = false;
                // allowIncomplete = false;
                try
                {
                    Random rnd = new Random();
                    Game selectedGame = new Game();
                    List<Pokemon> starterList = new List<Pokemon>();
                    List<Pokemon> pokemonDetails = this.dataService.GetObjects<Pokemon>("PokedexNumber, Id", "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth");
                    List<PokemonGameDetail> availablePokemon = this.dataService.GetObjects<PokemonGameDetail>(includes: "Pokemon, Game");
                    List<PokemonTypeDetail> pokemonTypeDetails = this.dataService.GetObjects<PokemonTypeDetail>("GenerationId", "Pokemon, Pokemon.Game, PrimaryType, SecondaryType");
                    List<Pokemon> allPokemon = this.GetAllPokemonWithoutForms();
                    List<PokemonFormDetail> pokemonFormDetails = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, OriginalPokemon, Form");
                    Pokemon pokemon;
                    PokemonFormDetail pokemonForm;
                    List<Pokemon> altForms = new List<Pokemon>();
                    PokemonTypeDetail typing;

                    allPokemon = allPokemon.Where(x => selectedGens.Contains(x.Game.GenerationId)).ToList();
                    if (!allowIncomplete)
                    {
                        allPokemon = allPokemon.Where(x => x.IsComplete).ToList();
                    }

                    if (pokemonCount > 6)
                    {
                        pokemonCount = 6;
                    }

                    if (selectedGameId != 0)
                    {
                        selectedGame = this.dataService.GetObjectByPropertyValue<Game>("Id", selectedGameId);
                        availablePokemon = availablePokemon.Where(x => x.GameId == selectedGame.Id).ToList();
                        pokemonTypeDetails = pokemonTypeDetails.Where(x => x.GenerationId <= selectedGame.GenerationId).ToList();
                        if (needsStarter)
                        {
                            starterList = this.dataService.GetObjects<GameStarterDetail>("Pokemon.PokedexNumber, Pokemon.Id", "Pokemon", "GameId", selectedGame.Id).ConvertAll(x => x.Pokemon);
                        }
                    }
                    else
                    {
                        needsStarter = false;
                    }

                    pokemonTypeDetails = pokemonTypeDetails.GroupBy(x => new { x.PokemonId }).Select(x => x.LastOrDefault()).ToList();
                    allPokemon = allPokemon.Where(x => availablePokemon.Any(y => y.PokemonId == x.Id)).ToList();
                    allPokemon = this.FilterLegendaries(allPokemon, selectedLegendaries, onlyLegendaries);
                    (allPokemon, starterList) = this.FilterEvolutions(allPokemon, starterList, selectedEvolutions, selectedGame);
                    allPokemon = this.FilterForms(allPokemon, selectedForms, selectedGame, onlyAltForms, multipleMegas, multipleGMax);
                    (allPokemon, starterList) = this.FilterEvolutions(allPokemon, starterList, selectedEvolutions, selectedGame);
                    allPokemon = this.FilterTypes(allPokemon, selectedTypeId, selectedGame);

                    if (monotypeOnly)
                    {
                        allPokemon = allPokemon.Where(x => pokemonTypeDetails.Any(y => (y.PrimaryTypeId == selectedTypeId || y.SecondaryTypeId == selectedTypeId) && y.PokemonId == x.Id)).ToList();
                        List<Pokemon> startersWithType = starterList.Where(x => pokemonTypeDetails.Any(y => (y.PrimaryTypeId == selectedTypeId || y.SecondaryTypeId == selectedTypeId) && y.PokemonId == x.Id)).ToList();
                        if (startersWithType.Count() > 0)
                        {
                            starterList = startersWithType;
                        }
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
                                int originalPokemonId = pokemon.Id;
                                pokemonForm = pokemonFormDetails.Find(x => x.AltFormPokemonId == pokemon.Id);
                                if (pokemonForm != null)
                                {
                                    originalPokemonId = pokemonForm.OriginalPokemonId;
                                }

                                altForms = pokemonFormDetails.Where(x => x.OriginalPokemonId == originalPokemonId).Select(x => x.AltFormPokemon).ToList();

                                if (pokemonForm != null)
                                {
                                    altForms.Remove(altForms.Find(x => x.Id == pokemon.Id));
                                    altForms.Add(pokemonDetails.Find(x => x.Id == originalPokemonId));
                                }

                                allPokemon = allPokemon.Where(x => !altForms.Any(y => y.Id == x.Id)).ToList();
                            }

                            if (noRepeatType)
                            {
                                typing = pokemonTypeDetails.Find(x => x.PokemonId == pokemon.Id);

                                if (typing != null)
                                {
                                    allPokemon = allPokemon.Where(x => !pokemonTypeDetails.Any(y => (y.PrimaryTypeId == typing.PrimaryTypeId || y.SecondaryTypeId == typing.PrimaryTypeId) && y.PokemonId == x.Id)).ToList();

                                    if (typing.SecondaryType != null)
                                    {
                                        allPokemon = allPokemon.Where(x => !pokemonTypeDetails.Any(y => (y.PrimaryTypeId == typing.SecondaryTypeId || y.SecondaryTypeId == typing.SecondaryTypeId) && y.PokemonId == x.Id)).ToList();
                                    }
                                }
                            }

                            pokemonList.Add(pokemon);
                            if (needsStarter && pokemonList.Count() == 1)
                            {
                                allPokemon = allPokemon.Where(x => !starterList.Any(y => y.Id == x.Id)).ToList();
                            }
                            else
                            {
                                allPokemon.Remove(allPokemon.Find(x => x.Id == pokemon.Id));
                            }
                        }
                    }

                    List<int> pokemonIds = new List<int>();
                    if (testPokemonIds.Count() > 0)
                    {
                        pokemonIds = testPokemonIds;
                        pokemonList = this.GetAllPokemonWithoutForms().Where(x => pokemonIds.Any(y => y == x.Id)).ToList();
                    }
                    else
                    {
                        pokemonIds = pokemonList.ConvertAll(x => x.Id);
                    }

                    TeamRandomizerViewModel model = new TeamRandomizerViewModel()
                    {
                        AllPokemonChangedNames = pokemonList,
                        AllPokemonOriginalNames = pokemonDetails.Where(x => pokemonIds.Any(y => y == x.Id)).OrderBy(x => pokemonIds.IndexOf(x.Id)).ToList(),
                        PokemonAbilities = new List<Ability>(),
                        PokemonURLs = new List<string>(),
                        AppConfig = this.appConfig,
                    };

                    foreach (var p in model.AllPokemonOriginalNames)
                    {
                        if (selectedGame.Id != 0)
                        {
                            model.PokemonURLs.Add(this.Url.Action("PokemonWithId", "Home", new { pokemonName = p.Name.Replace(": ", "_").Replace(' ', '_').ToLower(), pokemonId = p.Id, selectedGame.GenerationId }));
                        }
                        else
                        {
                            model.PokemonURLs.Add(this.Url.Action("PokemonWithId", "Home", new { pokemonName = p.Name.Replace(": ", "_").Replace(' ', '_').ToLower(), pokemonId = p.Id, generationId = 0 }));
                        }
                    }

                    if (selectedGame.GenerationId != 1 && selectedGame.GenerationId != 2 && randomAbility)
                    {
                        List<Ability> abilities;
                        PokemonAbilityDetail pokemonAbilities;
                        List<PokemonAbilityDetail> pokemonAbilityDetails = this.dataService.GetObjects<PokemonAbilityDetail>(includes: "Pokemon, PrimaryAbility, SecondaryAbility, HiddenAbility, SpecialEventAbility");
                        foreach (var p in model.AllPokemonOriginalNames)
                        {
                            abilities = new List<Ability>();
                            if (pokemonAbilityDetails.Find(x => x.PokemonId == p.Id) != null)
                            {
                                if (selectedGame.Id != 0)
                                {
                                    pokemonAbilities = pokemonAbilityDetails.Where(x => x.PokemonId == p.Id).Last(x => x.GenerationId <= selectedGame.GenerationId);
                                }
                                else
                                {
                                    pokemonAbilities = pokemonAbilityDetails.Where(x => x.PokemonId == p.Id).Last();
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

                    List<string> abilityNames = new List<string>();
                    if (model.PokemonAbilities.Count() > 0)
                    {
                        abilityNames = model.PokemonAbilities.ConvertAll(x => x.Name);
                    }

                    model.ExportString = this.ExportPokemonTeam(model.AllPokemonOriginalNames.ConvertAll(x => x.Id), abilityNames, randomAbility);

                    this.dataService.AddPageView("Random Team Generated", this.User.IsInRole("Owner"));

                    return model;
                }
                catch (Exception e)
                {
                    if (!this.User.IsInRole("Owner"))
                    {
                        string commentBody;
                        Game game = this.dataService.GetObjectByPropertyValue<Game>("Id", selectedGameId);
                        DataAccess.Models.Type type = this.dataService.GetObjectByPropertyValue<DataAccess.Models.Type>("Id", selectedTypeId);

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
                            commentBody = string.Concat(commentBody, " - Selected Type: ", selectedTypeId);
                        }

                        commentBody = string.Concat(commentBody, " - Pokemon Count: ", pokemonCount);
                        commentBody = string.Concat(commentBody, " - Pokemon Ids Generated: {", string.Join(", ", pokemonList.Select(x => x.Id)), "}");
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
                        commentBody = string.Concat(commentBody, " - Allow Incomplete Pokemon: ", allowIncomplete);

                        Comment comment = new Comment()
                        {
                            Name = commentBody,
                        };

                        if (this.User.Identity.Name != null)
                        {
                            comment.CommentorId = this.dataService.GetCurrentUser(this.User).Id;
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
                List<Ability> pokemonAbilities = this.dataService.GetAbilitiesForPokemon(pokemonId, gameId, this.User, this.appConfig);
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

                return this.Json(this.Url.Action("Types", "Owner")).Value.ToString();
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
                    this.dataService.DeletePokemonTeam(t.Id, this.User, this.appConfig);
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

                return this.Json(this.Url.Action("FormGroups", "Owner")).Value.ToString();
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

                return this.Json(this.Url.Action("Pokemon", "Owner")).Value.ToString();
            }

            return null;
        }

        /// <summary>
        /// Updates the availablity of the specified hunting method in different games.
        /// </summary>
        /// <param name="huntingMethodId">The hunting method's id.</param>
        /// <param name="games">The games the pokemon is able to be used in.</param>
        /// <returns>The admin pokemon page.</returns>
        [Route("update-hunting-method-game-availability")]
        public string UpdateHuntingMethodGameAvailability(int huntingMethodId, List<int> games)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                HuntingMethodGameDetail huntingMethodGameDetail;
                List<HuntingMethodGameDetail> existingGameDetails = this.dataService.GetObjects<HuntingMethodGameDetail>("Game.GenerationId, GameId, Id", "HuntingMethod, Game", "HuntingMethodId", huntingMethodId);

                foreach (var g in games)
                {
                    huntingMethodGameDetail = new HuntingMethodGameDetail()
                    {
                        HuntingMethodId = huntingMethodId,
                        GameId = g,
                    };
                    this.dataService.AddObject(huntingMethodGameDetail);
                }

                foreach (var g in existingGameDetails)
                {
                    this.dataService.DeleteObject<HuntingMethodGameDetail>(g.Id);
                }

                return this.Json(this.Url.Action("HuntingMethods", "Owner")).Value.ToString();
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

                return this.Json(this.Url.Action("Game", "Owner")).Value.ToString();
            }

            return null;
        }

        /// <summary>
        /// Updates the availablity of the pokemon in a specified game.
        /// </summary>
        /// <param name="regionalDexId">The game's id.</param>
        /// <param name="pokemonList">The pokemon that are available in the given regional dex.</param>
        /// <returns>The admin pokemon page.</returns>
        [Route("update-regional-dex-entries")]
        public string UpdateRegionalDexEntries(int regionalDexId, List<string> pokemonList)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                RegionalDex regionalDex = this.dataService.GetObjectByPropertyValue<RegionalDex>("Id", regionalDexId);
                List<Pokemon> allPokemon = this.dataService.GetAllPokemon();
                List<PokemonFormDetail> altFormList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon");
                allPokemon = allPokemon.Where(x => pokemonList.Any(y => y == x.Name)).DistinctBy(x => x.Name).OrderBy(x => pokemonList.IndexOf(x.Name)).ToList();
                List<int> pokemonIds = allPokemon.Select(x => x.Id).ToList();
                List<RegionalDexEntry> existingDexEntries = this.dataService.GetObjects<RegionalDexEntry>(includes: "Pokemon, RegionalDex", whereProperty: "RegionalDexId", wherePropertyValue: regionalDexId);
                List<RegionalDexEntry> newDexEntries = new List<RegionalDexEntry>();
                if (allPokemon.First().Name == "Victini")
                {
                    if (existingDexEntries.Find(x => x.PokemonId == pokemonIds.First() && x.RegionalDexId == regionalDexId && x.RegionalPokedexNumber == 0) == null)
                    {
                        newDexEntries.Add(new RegionalDexEntry()
                        {
                            PokemonId = pokemonIds.First(),
                            RegionalDexId = regionalDexId,
                            RegionalPokedexNumber = 0,
                        });
                    }
                    else
                    {
                        existingDexEntries.Remove(existingDexEntries.Find(x => x.PokemonId == pokemonIds.First() && x.RegionalDexId == regionalDexId));
                    }

                    pokemonIds.Remove(pokemonIds.First());
                }

                for (var i = 0; i < pokemonIds.Count(); i++)
                {
                    if (existingDexEntries.Find(x => x.PokemonId == pokemonIds[i] && x.RegionalDexId == regionalDexId && x.RegionalPokedexNumber == i + 1) == null)
                    {
                        newDexEntries.Add(new RegionalDexEntry()
                        {
                            PokemonId = pokemonIds[i],
                            RegionalDexId = regionalDexId,
                            RegionalPokedexNumber = i + 1,
                        });
                        if (altFormList.Exists(x => x.OriginalPokemonId == pokemonIds[i]))
                        {
                            foreach (var a in altFormList.Where(x => x.OriginalPokemonId == pokemonIds[i]).Select(x => x.AltFormPokemon))
                            {
                                newDexEntries.Add(new RegionalDexEntry()
                                {
                                    PokemonId = a.Id,
                                    RegionalDexId = regionalDexId,
                                    RegionalPokedexNumber = i + 1,
                                });
                            }
                        }
                    }
                    else
                    {
                        existingDexEntries.Remove(existingDexEntries.Find(x => x.PokemonId == pokemonIds[i] && x.RegionalDexId == regionalDexId));
                        if (altFormList.Exists(x => x.OriginalPokemonId == pokemonIds[i]))
                        {
                            foreach (var a in altFormList.Where(x => x.OriginalPokemonId == pokemonIds[i]).Select(x => x.AltFormPokemon))
                            {
                                existingDexEntries.Remove(existingDexEntries.Find(x => x.PokemonId == a.Id && x.RegionalDexId == regionalDexId));
                            }
                        }
                    }
                }

                foreach (var e in newDexEntries)
                {
                    this.dataService.AddObject(e);
                }

                foreach (var e in existingDexEntries)
                {
                    this.dataService.DeleteObject<RegionalDexEntry>(e.Id);
                }

                return this.Json(this.Url.Action("RegionalDexes", "Owner")).Value.ToString();
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

                return this.Json(this.Url.Action("Games", "Owner")).Value.ToString();
            }

            return null;
        }

        /// <summary>
        /// Updates the availablity of the pokemon in a specified game.
        /// </summary>
        /// <param name="gameId">The game's id.</param>
        /// <param name="markList">The marks that are available in the given game.</param>
        /// <returns>The admin pokemon page.</returns>
        [Route("update-mark-games")]
        public string UpdateMarkGame(int gameId, List<int> markList)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<MarkGameDetail> existingMarkStarters = new List<MarkGameDetail>();
                List<MarkGameDetail> newMarkStarters = new List<MarkGameDetail>();
                List<Game> games = this.dataService.GetObjects<Game>("ReleaseDate, Id").Where(x => x.GenerationId >= 8).ToList();
                DateTime releaseDate = games.Find(x => x.Id == gameId).ReleaseDate;
                games = games.Where(x => x.ReleaseDate == releaseDate).ToList();
                int generationId = games.First().GenerationId;

                foreach (var game in games.ConvertAll(x => x.Id))
                {
                    newMarkStarters = new List<MarkGameDetail>();
                    existingMarkStarters = this.dataService.GetObjects<MarkGameDetail>(includes: "Mark, Game", whereProperty: "GameId", wherePropertyValue: game);

                    foreach (var m in markList)
                    {
                        if (existingMarkStarters.Find(x => x.MarkId == m && x.GameId == game) == null)
                        {
                            newMarkStarters.Add(new MarkGameDetail()
                            {
                                GameId = game,
                                MarkId = m,
                            });
                        }
                        else
                        {
                            existingMarkStarters.Remove(existingMarkStarters.Find(x => x.MarkId == m && x.GameId == game));
                        }
                    }

                    foreach (var g in newMarkStarters)
                    {
                        this.dataService.AddObject(g);
                    }

                    foreach (var g in existingMarkStarters)
                    {
                        this.dataService.DeleteObject<MarkGameDetail>(g.Id);
                    }
                }

                return this.Json(this.Url.Action("Games", "Owner")).Value.ToString();
            }

            return null;
        }

        /// <summary>
        /// Updates the availablity of the pokemon in a specified game.
        /// </summary>
        /// <param name="gameId">The game's id.</param>
        /// <param name="pokeballList">The pokeballs that are available in the given game.</param>
        /// <returns>The admin pokemon page.</returns>
        [Route("update-pokeball-games")]
        public string UpdatePokeballGame(int gameId, List<int> pokeballList)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<PokeballGameDetail> existingPokeballStarters = new List<PokeballGameDetail>();
                List<PokeballGameDetail> newPokeballStarters = new List<PokeballGameDetail>();
                List<Game> games = this.dataService.GetObjects<Game>("ReleaseDate, Id");
                DateTime releaseDate = games.Find(x => x.Id == gameId).ReleaseDate;
                games = games.Where(x => x.ReleaseDate == releaseDate).ToList();
                int generationId = games.First().GenerationId;

                foreach (var game in games.ConvertAll(x => x.Id))
                {
                    newPokeballStarters = new List<PokeballGameDetail>();
                    existingPokeballStarters = this.dataService.GetObjects<PokeballGameDetail>(includes: "Pokeball, Game", whereProperty: "GameId", wherePropertyValue: game);

                    foreach (var m in pokeballList)
                    {
                        if (existingPokeballStarters.Find(x => x.PokeballId == m && x.GameId == game) == null)
                        {
                            newPokeballStarters.Add(new PokeballGameDetail()
                            {
                                GameId = game,
                                PokeballId = m,
                            });
                        }
                        else
                        {
                            existingPokeballStarters.Remove(existingPokeballStarters.Find(x => x.PokeballId == m && x.GameId == game));
                        }
                    }

                    foreach (var g in newPokeballStarters)
                    {
                        this.dataService.AddObject(g);
                    }

                    foreach (var g in existingPokeballStarters)
                    {
                        this.dataService.DeleteObject<PokeballGameDetail>(g.Id);
                    }
                }

                return this.Json(this.Url.Action("Games", "Owner")).Value.ToString();
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
        /// <param name="useCase">The use case of this method.</param>
        /// <returns>A list of available genders.</returns>
        [Route("get-pokemon-genders")]
        public List<string> GetPokemonGenders(int pokemonId, string useCase)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return this.dataService.GrabGenders(pokemonId, useCase);
            }

            return null;
        }

        /// <summary>
        /// Gets the available sweets for a pokemon.
        /// </summary>
        /// <param name="pokemonId">The pokemon's id.</param>
        /// <returns>A list of available sweets.</returns>
        [Route("get-pokemon-sweets")]
        public List<Sweet> GetPokemonSweets(int pokemonId)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                Pokemon pokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", pokemonId);
                List<Sweet> model = new List<Sweet>();
                if (pokemon.Name == "Alcremie")
                {
                    model = this.dataService.GetObjects<Sweet>();
                }

                return model;
            }

            return null;
        }

        /// <summary>
        /// Gets the type chart for the given typing combination.
        /// </summary>
        /// <param name="pokemonId">The pokemon's id.</param>
        /// <param name="generationId">The generation used to specify the type chart.</param>
        /// <param name="teraType">The tera type of the pokemon.</param>
        /// <returns>The file type evaluator chart shared view.</returns>
        [Route("get-stellar-typing-chart")]
        public IActionResult GetStellarTypingChart(int pokemonId, int generationId, string teraType)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                PokemonTypeDetail pokemonTypeDetail = this.dataService.GetObjects<PokemonTypeDetail>(whereProperty: "PokemonId", wherePropertyValue: pokemonId).OrderByDescending(x => x.GenerationId).First(x => x.GenerationId <= generationId);
                int primaryId = pokemonTypeDetail.PrimaryTypeId ?? 0;
                int secondaryId = pokemonTypeDetail.SecondaryTypeId ?? 0;
                List<Game> games = this.dataService.GetObjects<Game>().Where(x => x.GenerationId == generationId).ToList();
                return this.PartialView("_FillTypeEvaluatorChart", this.GetTypeChartTyping(primaryId, secondaryId, games.Last().Id, teraType));
            }
            else
            {
                return this.RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Gets the type chart for the given typing combination.
        /// </summary>
        /// <param name="pokemonId">The pokemon's id.</param>
        /// <param name="generationId">The generation used to specify the type chart.</param>
        /// <returns>The file type evaluator chart shared view.</returns>
        [Route("get-typing-evaluator-chart-by-pokemon")]
        public IActionResult GetTypingEvaluatorChartByPokemon(int pokemonId, int generationId)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                PokemonTypeDetail pokemonTypeDetail = this.dataService.GetObjects<PokemonTypeDetail>(whereProperty: "PokemonId", wherePropertyValue: pokemonId).OrderByDescending(x => x.GenerationId).First(x => x.GenerationId <= generationId);
                int primaryId = pokemonTypeDetail.PrimaryTypeId ?? 0;
                int secondaryId = pokemonTypeDetail.SecondaryTypeId ?? 0;
                List<Game> games = this.dataService.GetObjects<Game>().Where(x => x.GenerationId == generationId).ToList();
                return this.PartialView("_FillTypeEvaluatorChart", this.GetTypeChartTyping(primaryId, secondaryId, games.Last().Id));
            }
            else
            {
                return this.RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Gets the type chart for the given typing combination.
        /// </summary>
        /// <param name="primaryTypeId">The primary type's id.</param>
        /// <param name="secondaryTypeId">The secondary type's id.</param>
        /// <param name="gameId">The generation used to specify the type chart.</param>
        /// <param name="teraType">The tera type of the pokemon.</param>
        /// <returns>The file type evaluator chart shared view.</returns>
        [Route("get-typing-evaluator-chart")]
        public IActionResult GetTypingEvaluatorChart(int primaryTypeId, int secondaryTypeId, int gameId, string teraType)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return this.PartialView("_FillTypeEvaluatorChart", this.GetTypeChartTyping(primaryTypeId, secondaryTypeId, gameId, teraType));
            }
            else
            {
                return this.RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Increments the phases for a shiny hunt.
        /// </summary>
        /// <param name="shinyHuntId">The shiny hunt's id.</param>
        /// <param name="phases">The amount of increments done when clicking "Increase Phases". Default is 1.</param>
        [Route("increment-shiny-hunt-phases")]
        public void IncrementShinyHuntPhases(int shinyHuntId, int phases = 1)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                ShinyHunt shinyHunt = this.dataService.GetObjectByPropertyValue<ShinyHunt>("Id", shinyHuntId);
                User user = this.dataService.GetCurrentUser(this.User);
                if (user != null && user.Id == shinyHunt.UserId)
                {
                    shinyHunt.Phases = phases;
                    shinyHunt.TotalEncounters += shinyHunt.CurrentPhaseEncounters;
                    shinyHunt.CurrentPhaseEncounters = 0;
                    this.dataService.UpdateObject(shinyHunt);
                }
            }
        }

        /// <summary>
        /// Updates the encounters for a shiny hunt.
        /// </summary>
        /// <param name="shinyHuntId">The shiny hunt's id.</param>
        /// <param name="encounters">The amount to encounters.</param>
        [Route("set-shiny-hunt-encounters")]
        public void SetShinyHuntEncounters(int shinyHuntId, int encounters)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                ShinyHunt shinyHunt = this.dataService.GetObjectByPropertyValue<ShinyHunt>("Id", shinyHuntId);
                User user = this.dataService.GetCurrentUser(this.User);
                if (user != null && user.Id == shinyHunt.UserId)
                {
                    shinyHunt.CurrentPhaseEncounters = Math.Max(encounters, 0);
                    this.dataService.UpdateObject(shinyHunt);
                }
            }
        }

        /// <summary>
        /// Updates the phases for a shiny hunt.
        /// </summary>
        /// <param name="shinyHuntId">The shiny hunt's id.</param>
        /// <param name="phases">The amount to phases.</param>
        [Route("set-shiny-hunt-phases")]
        public void SetShinyHuntPhases(int shinyHuntId, int phases)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                ShinyHunt shinyHunt = this.dataService.GetObjectByPropertyValue<ShinyHunt>("Id", shinyHuntId);
                User user = this.dataService.GetCurrentUser(this.User);
                if (user != null && user.Id == shinyHunt.UserId)
                {
                    shinyHunt.Phases = Math.Max(phases, 1);
                    this.dataService.UpdateObject(shinyHunt);
                }
            }
        }

        /// <summary>
        /// Updates the increments for a shiny hunt.
        /// </summary>
        /// <param name="shinyHuntId">The shiny hunt's id.</param>
        /// <param name="increments">The amount to increments.</param>
        [Route("set-shiny-hunt-increments")]
        public void SetShinyHuntIncrements(int shinyHuntId, int increments)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                ShinyHunt shinyHunt = this.dataService.GetObjectByPropertyValue<ShinyHunt>("Id", shinyHuntId);
                User user = this.dataService.GetCurrentUser(this.User);
                if (user != null && user.Id == shinyHunt.UserId)
                {
                    shinyHunt.IncrementAmount = Math.Max(increments, 1);
                    this.dataService.UpdateObject(shinyHunt);
                }
            }
        }

        /// <summary>
        /// Checks to see if shiny charm can be used.
        /// </summary>
        /// <param name="gameId">The game's id.</param>
        /// <param name="huntingMethodId">The hunting method's id.</param>
        /// <returns>The boolean determining if a shiny charm can be shown.</returns>
        [Route("check-shiny-charm")]
        public bool CheckShinyCharm(int gameId, int huntingMethodId)
        {
            try
            {
                if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    bool shinyCharm = true;
                    if (huntingMethodId == 8 || huntingMethodId == 14 || gameId == 43)
                    {
                        shinyCharm = false;
                    }
                    else
                    {
                        Game game = this.dataService.GetObjectByPropertyValue<Game>("Id", gameId);
                        if (game == null)
                        {
                            shinyCharm = false;
                        }
                        else if (game.GenerationId < 6 && gameId != 11 && gameId != 29)
                        {
                            shinyCharm = false;
                        }
                        else if ((gameId == 35 || gameId == 36) && huntingMethodId != 4 && huntingMethodId != 5)
                        {
                            shinyCharm = false;
                        }
                    }

                    return shinyCharm;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                if (!this.User.IsInRole("Owner") && e != null)
                {
                    string commentBody;
                    if (e != null)
                    {
                        commentBody = string.Concat(e.GetType().ToString(), " error during shiny charm check.");
                    }
                    else
                    {
                        commentBody = "Unknown error during shiny charm check.";
                    }

                    commentBody = string.Concat(commentBody, " - Selected Game Id: ", gameId);
                    commentBody = string.Concat(commentBody, " - Selected Hunting Method Id: ", huntingMethodId);

                    Comment comment = new Comment()
                    {
                        Name = commentBody,
                    };

                    if (this.User.Identity.Name != null)
                    {
                        comment.CommentorId = this.dataService.GetCurrentUser(this.User).Id;
                    }

                    this.dataService.EmailComment(this.appConfig, comment);
                }

                return false;
            }
        }

        /// <summary>
        /// Checks to see if sparkling power can be used.
        /// </summary>
        /// <param name="gameId">The game's id.</param>
        /// <param name="huntingMethodId">The hunting method's id.</param>
        /// <returns>The boolean determining if a sparkling power can be shown.</returns>
        [Route("check-sparkling-power")]
        public bool CheckSparklingPower(int gameId, int huntingMethodId)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest" && (gameId == 41 || gameId == 42) && huntingMethodId != 4 && huntingMethodId != 5 && huntingMethodId != 14)
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
        /// Toggles whether a shiny hunt is pinned or not.
        /// </summary>
        /// <param name="shinyHuntId">The shiny hunt's id.</param>
        /// <returns>The boolean determining if a mark can be shown.</returns>
        [Route("toggle-hunt-pin")]
        public bool ToggleHuntPin(int shinyHuntId)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                ShinyHunt hunt = this.dataService.GetObjectByPropertyValue<ShinyHunt>("Id", shinyHuntId);
                hunt.IsPinned = !hunt.IsPinned;
                this.dataService.UpdateObject(hunt);

                return hunt.IsPinned;
            }

            return false;
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
                        comment.CommentorId = this.dataService.GetCurrentUser(this.User).Id;
                    }

                    this.dataService.EmailComment(this.appConfig, comment);
                }

                return null;
            }
        }

        /// <summary>
        /// Gets a list of all pokemon by their type combination.
        /// </summary>
        /// <param name="primaryTypeId">The primary type's id.</param>
        /// <param name="secondaryTypeId">The secondary type's id.</param>
        /// <param name="gameId">The game used to specify the type chart.</param>
        /// <param name="regionalDexId">The game's regional dex used to specify the type chart.</param>
        /// <returns>The fill typing evaluator shared view.</returns>
        [Route("get-pokemon-by-typing")]
        public IActionResult GetPokemonByTyping(int primaryTypeId, int secondaryTypeId, int gameId, int regionalDexId)
        {
            List<Pokemon> altFormList = this.dataService.GetObjects<PokemonFormDetail>("AltFormPokemon.PokedexNumber, AltFormPokemon.Id", "AltFormPokemon").ConvertAll(x => x.AltFormPokemon);
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                Game game = new Game()
                {
                    Id = 0,
                    Name = "Any Game",
                };

                if (gameId != 0)
                {
                    game = this.dataService.GetObjectByPropertyValue<Game>("Id", gameId);
                }

                List<PokemonTypeDetail> typingList = this.GetAllPokemonWithSpecificTypes(primaryTypeId, secondaryTypeId, game, regionalDexId);

                typingList.Where(x => altFormList.Any(y => y.Id == x.PokemonId)).ToList().ForEach(x => x.Pokemon = this.dataService.GetAltFormWithFormName(x.PokemonId));

                List<Pokemon> pokemonList = typingList.ConvertAll(x => x.Pokemon);

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
        /// <param name="abilityId">The ability's id.</param>
        /// <param name="gameId">The game the ability is in.</param>
        /// <returns>The fill typing evaluator shared view.</returns>
        [Route("get-pokemon-by-ability")]
        public IActionResult GetPokemonByAbility(int abilityId, int gameId)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<Pokemon> altFormList = this.dataService.GetObjects<PokemonFormDetail>("AltFormPokemon.PokedexNumber, AltFormPokemon.Id", "AltFormPokemon").ConvertAll(x => x.AltFormPokemon);
                List<PokemonAbilityDetail> abilityList = this.GetPokemonByAbilityAndGame(abilityId, gameId);
                abilityList.Where(x => altFormList.Any(y => y.Id == x.PokemonId)).ToList().ForEach(x => x.Pokemon = this.dataService.GetAltFormWithFormName(x.PokemonId));
                List<Pokemon> pokemonList = abilityList.ConvertAll(x => x.Pokemon);
                int generationId = 0;
                if (gameId != 0)
                {
                    generationId = this.dataService.GetObjectByPropertyValue<Game>("Id", gameId).GenerationId;
                }

                AbilityEvaluatorPageViewModel model = new AbilityEvaluatorPageViewModel()
                {
                    AllPokemonWithAbility = abilityList,
                    AllPokemon = pokemonList,
                    AppConfig = this.appConfig,
                    GenerationId = generationId,
                    Ability = this.dataService.GetObjectByPropertyValue<Ability>("Id", abilityId),
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

                List<Pokemon> altFormList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form").ConvertAll(x => x.AltFormPokemon);

                eggGroupDetails.Where(x => altFormList.Any(y => y.Id == x.PokemonId)).ToList().ForEach(x => x.Pokemon = this.dataService.GetAltFormWithFormName(x.PokemonId));

                return this.PartialView("_FillDayCareEvaluatorDropdown", eggGroupDetails.ConvertAll(x => x.Pokemon));
            }
            else
            {
                return this.RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Adds a shiny hunt into the completed shiny hunt section.
        /// </summary>
        /// <param name="shinyHuntId">The shiny hunt that was completed.</param>
        /// <returns>The fill completed shiny hunt shared view.</returns>
        [Route("add-completed-shiny-hunt")]
        public IActionResult AddCompletedShinyHunt(int shinyHuntId)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                ShinyHunt shinyHunt = this.dataService.GetObjectByPropertyValue<ShinyHunt>("Id", shinyHuntId, "User, Pokemon, Game, HuntingMethod, Pokeball, Mark, PhaseOfHunt");
                List<Game> gamesList = this.dataService.GetObjects<Game>("ReleaseDate, Id").Where(x => x.ReleaseDate <= DateTime.UtcNow && x.GenerationId >= 2).ToList();
                if (gamesList.Exists(x => x.Id == shinyHunt.GameId))
                {
                    shinyHunt.Game = new Game()
                    {
                        Id = gamesList.First(x => x.ReleaseDate == shinyHunt.Game.ReleaseDate).Id,
                        Name = string.Join(" / ", gamesList.Where(x => x.ReleaseDate == shinyHunt.Game.ReleaseDate).Select(x => x.Name)),
                        GenerationId = gamesList.First(x => x.ReleaseDate == shinyHunt.Game.ReleaseDate).GenerationId,
                        ReleaseDate = shinyHunt.Game.ReleaseDate,
                        GameColor = gamesList.First(x => x.ReleaseDate == shinyHunt.Game.ReleaseDate).GameColor,
                    };
                }

                ShinyHuntsViewModel model = new ShinyHuntsViewModel()
                {
                    ShinyHunt = shinyHunt,
                    AppConfig = this.appConfig,
                    IsShared = true,
                };

                return this.PartialView("_FillCompletedShinyHunt", model);
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
                    List<Pokemon> altFormList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form").ConvertAll(x => x.AltFormPokemon);
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
                        eggGroupList.Where(x => altFormList.Any(y => y.Id == x.PokemonId)).ToList().ForEach(x => x.Pokemon = this.dataService.GetAltFormWithFormName(x.PokemonId));
                        pokemonList.AddRange(eggGroupList.ConvertAll(x => x.Pokemon));
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
                        List<PokemonEggGroupDetail> finalEggGroupList = new List<PokemonEggGroupDetail>(eggGroupList);

                        foreach (var p in eggGroupList.Where(x => x.Pokemon.GenderRatioId == 10 || ((genderRatio.Id == 1 || genderRatio.Id == 9) && x.Pokemon.GenderRatioId == genderRatio.Id)).ToList())
                        {
                            finalEggGroupList.Remove(p);
                        }

                        eggGroupList.Where(x => altFormList.Any(y => y.Id == x.PokemonId)).ToList().ForEach(x => x.Pokemon = this.dataService.GetAltFormWithFormName(x.PokemonId));
                        eggGroupList = finalEggGroupList;
                        eggGroupList.Add(this.dataService.GetObjectByPropertyValue<PokemonEggGroupDetail>("Pokemon.Name", "Ditto", "Pokemon, Pokemon.GenderRatio, PrimaryEggGroup, SecondaryEggGroup"));
                        originalPokemon = eggGroupList.ConvertAll(x => x.Pokemon);
                        pokemonList.AddRange(originalPokemon);
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
                        AllPokemonWithEggGroups = eggGroupList.OrderBy(x => x.Pokemon.PokedexNumber).ThenBy(x => x.PokemonId).ToList(),
                        AllPokemon = pokemonList.OrderBy(x => x.PokedexNumber).ThenBy(x => x.Id).ToList(),
                        AllAltForms = altFormList.OrderBy(x => x.PokedexNumber).ThenBy(x => x.Id).ToList(),
                        AllOriginalPokemon = originalPokemon.OrderBy(x => x.PokedexNumber).ThenBy(x => x.Id).ToList(),
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
                            comment.CommentorId = this.dataService.GetCurrentUser(this.User).Id;
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
        /// Gets a list of types that are available in the given game.
        /// </summary>
        /// <param name="gameId">The game's id.</param>
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
        /// Gets a list of regional dexes that are available in the given game.
        /// </summary>
        /// <param name="gameId">The game's id.</param>
        /// <returns>The fill typing evaluator regional dexes shared view.</returns>
        [Route("get-regional-dexes-by-game")]
        public IActionResult GrabTypingEvaluatorRegionalDexes(int gameId)
        {
            List<RegionalDex> model = this.dataService.GetObjects<RegionalDex>().Where(x => x.GameId == gameId).ToList();
            return this.PartialView("_FillTypingEvaluatorRegionalDexes", model);
        }

        /// <summary>
        /// Grabs the details about two pokemon given the games provided.
        /// </summary>
        /// <param name="firstPokemonId">The id of the first pokemon.</param>
        /// <param name="firstGameId">The game from which the first pokemon's details will be grabbed.</param>
        /// <param name="secondPokemonId">The id of the second pokemon.</param>
        /// <param name="secondGameId">The game from which the second pokemon's details will be grabbed.</param>
        /// <returns>The pokemon difference shared view.</returns>
        [Route("get-pokemon-differences")]
        public IActionResult GrabPokemonDifferences(int firstPokemonId, int firstGameId, int secondPokemonId, int secondGameId)
        {
            try
            {
                List<Pokemon> pokemonList = this.dataService.GetAllPokemon();
                List<PokemonFormDetail> altForms = this.dataService.GetObjects<PokemonFormDetail>(includes: "Form");
                Form form = null;
                if (altForms.Exists(x => x.AltFormPokemonId == firstPokemonId))
                {
                    form = altForms.Find(x => x.AltFormPokemonId == firstPokemonId).Form;
                }

                PokemonViewModel firstPokemonDetails = this.dataService.GetPokemonDetails(pokemonList.Find(x => x.Id == firstPokemonId), this.appConfig, form);
                if (altForms.Exists(x => x.AltFormPokemonId == secondPokemonId))
                {
                    form = altForms.Find(x => x.AltFormPokemonId == secondPokemonId).Form;
                }

                PokemonViewModel secondPokemonDetails = this.dataService.GetPokemonDetails(pokemonList.Find(x => x.Id == secondPokemonId), this.appConfig, form);
                List<Game> games = this.dataService.GetObjects<Game>();
                List<Game> selectedGames = new List<Game>
                {
                    games.Find(x => x.Id == firstGameId),
                    games.Find(x => x.Id == secondGameId),
                };

                List<PokemonViewModel> pokemonViewList = new List<PokemonViewModel>
                {
                    firstPokemonDetails,
                    secondPokemonDetails,
                };

                PokemonDifferenceSharedViewModel model = new PokemonDifferenceSharedViewModel()
                {
                    AllPokemon = pokemonViewList,
                    AllGames = selectedGames,
                    AppConfig = this.appConfig,
                };

                return this.PartialView("_FillPokemonDifferences", model);
            }
            catch (Exception e)
            {
                if (!this.User.IsInRole("Owner") && e != null)
                {
                    string commentBody;
                    if (e != null)
                    {
                        commentBody = string.Concat(e.GetType().ToString(), " error during the pokemon comparison.");
                    }
                    else
                    {
                        commentBody = "Unknown error during the pokemon comparison.";
                    }

                    commentBody = string.Concat(commentBody, " - First Pokemon Id: ", firstPokemonId);
                    commentBody = string.Concat(commentBody, " - First Game Id: ", firstGameId);
                    commentBody = string.Concat(commentBody, " - Second Pokemon Id: ", secondPokemonId);
                    commentBody = string.Concat(commentBody, " - Second Game Id: ", secondGameId);
                    Comment comment = new Comment()
                    {
                        Name = commentBody,
                    };

                    if (this.User.Identity.Name != null)
                    {
                        comment.CommentorId = this.dataService.GetCurrentUser(this.User).Id;
                    }

                    this.dataService.AddObject(comment);
                    this.dataService.EmailComment(this.appConfig, comment);
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a list of abilities that are available in the given game.
        /// </summary>
        /// <param name="gameId">The game's id.</param>
        /// <returns>The fill ability evaluator abilities shared view.</returns>
        [Route("get-abilities-by-game")]
        public IActionResult GrabAbilityEvaluatorAbilities(int gameId)
        {
            List<Ability> model = new List<Ability>();
            if (gameId != 0)
            {
                Game game = this.dataService.GetObjectByPropertyValue<Game>("Id", gameId);
                List<Pokemon> pokemonList = this.dataService.GetObjects<PokemonGameDetail>(includes: "Pokemon", whereProperty: "GameId", wherePropertyValue: gameId).Select(x => x.Pokemon).ToList();
                pokemonList = pokemonList.Distinct().ToList();
                List<PokemonAbilityDetail> pokemonAbilities = this.dataService.GetObjects<PokemonAbilityDetail>(includes: "PrimaryAbility, SecondaryAbility, HiddenAbility, SpecialEventAbility").Where(x => x.GenerationId <= game.GenerationId).ToList();
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
            DateTime month = new DateTime(year, 1, 1);
            DateTime finalMonth = new DateTime(year, 12, 1);
            if (DateTime.Now.Year == year && finalMonth.Month > DateTime.Now.Month)
            {
                finalMonth = DateTime.Now;
            }

            List<string> model = new List<string>()
            {
                month.ToString("MMMM"),
            };
            while (month.Month < finalMonth.Month)
            {
                month = month.AddMonths(1);
                model.Add(month.ToString("MMMM"));
            }

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
            int monthInt = DateTime.ParseExact(month, "MMMM", CultureInfo.CurrentCulture).Month;
            DateTime day = new DateTime(year, monthInt, 1);
            DateTime finalDay = new DateTime(year, monthInt, DateTime.DaysInMonth(year, monthInt));
            if (DateTime.Now.Year == year && DateTime.Now.Month == monthInt && finalDay.Day > DateTime.Now.Day)
            {
                finalDay = DateTime.Now;
            }

            List<string> model = new List<string>()
            {
                day.ToString("dd"),
            };
            while (day.Day < finalDay.Day)
            {
                day = day.AddDays(1);
                model.Add(day.ToString("dd"));
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
            List<PageStat> pageStats = this.dataService.GetObjects<PageStat>("VisitDate").Where(x => x.VisitDate.Year == year).ToList();
            if (month != "0")
            {
                pageStats = pageStats.Where(x => x.VisitDate.ToString("MMMM") == month).ToList();
                if (day != 0)
                {
                    pageStats = pageStats.Where(x => x.VisitDate.Day == day).ToList();
                }
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
        /// <param name="gameId">The id of the generation.</param>
        /// <param name="teraType">The tera type of the pokemon. Used only if terastallized.</param>
        /// <returns>Returns the type effectiveness given the typing and generation.</returns>
        [Route("get-type-chart-typing")]
        public TypeEffectivenessViewModel GetTypeChartTyping(int primaryTypeId, int secondaryTypeId, int gameId, string teraType = "")
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
            Game game;
            if (gameId != 0)
            {
                game = this.dataService.GetObjectByPropertyValue<Game>("Id", gameId);
            }
            else
            {
                game = this.dataService.GetObjects<Game>().OrderBy(x => x.ReleaseDate).Last();
            }

            pokemonTypes.Add(this.dataService.GetObjectByPropertyValue<DataAccess.Models.Type>("Id", primaryTypeId));

            if (secondaryTypeId != 0)
            {
                pokemonTypes.Add(this.dataService.GetObjectByPropertyValue<DataAccess.Models.Type>("Id", secondaryTypeId));
            }

            foreach (var type in pokemonTypes)
            {
                typeChart = this.dataService.GetObjects<TypeChart>(includes: "Attack, Defend", whereProperty: "DefendId", wherePropertyValue: type.Id);

                List<int> generations = typeChart.Select(x => x.GenerationId).Distinct().OrderByDescending(x => x).ToList();
                if (gameId != 0)
                {
                    typeChart = typeChart.Where(x => x.GenerationId == generations.First(x => x <= game.GenerationId)).ToList();
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

            if (!string.IsNullOrEmpty(teraType) && game.GenerationId == 9)
            {
                weakAgainst.Add("Stellar");
            }

            TypeEffectivenessViewModel effectivenessChart = new TypeEffectivenessViewModel()
            {
                ImmuneTo = immuneTo,
                StrongAgainst = strongAgainst,
                WeakAgainst = weakAgainst,
                SuperStrongAgainst = superStrongAgainst,
                SuperWeakAgainst = superWeakAgainst,
                AppConfig = this.appConfig,
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
        /// Grabs all of the games available for the selected pokemon.
        /// </summary>
        /// <param name="pokemonId">The selected pokemon Ids.</param>
        /// <returns>The list of available games.</returns>
        [Route("get-games-by-shiny-huntable-pokemon")]
        public List<Game> GetGamesByShinyHuntablePokemon(List<int> pokemonId)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return this.dataService.GetMultipleShinyHuntGames(pokemonId);
            }

            return null;
        }

        /// <summary>
        /// Grabs all of the games available for the selected pokemon.
        /// </summary>
        /// <param name="pokemonId">The selected pokemon's Id.</param>
        /// <returns>The list of available games.</returns>
        [Route("get-games-by-pokemon")]
        public List<Game> GetGamesByPokemon(int pokemonId)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<Game> pokemonGamesIn = this.dataService.GetObjects<PokemonGameDetail>("GameId", "Game", "PokemonId", pokemonId).ConvertAll(x => x.Game);
                List<Game> games = this.dataService.GetGamesGroupedByReleaseDate().Where(x => x.Id != 43).ToList();
                return games.Where(x => pokemonGamesIn.Any(y => y.Id == x.Id)).ToList();
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
                List<HuntingMethod> huntingMethods = this.dataService.GetObjects<HuntingMethodGameDetail>("HuntingMethod.Name", "HuntingMethod", "GameId", gameId).ConvertAll(x => x.HuntingMethod);

                return huntingMethods;
            }

            return null;
        }

        /// <summary>
        /// Grabs the given pokemon by the provided id.
        /// </summary>
        /// <param name="pokemonId">The selected pokemon's Id.</param>
        /// <returns>The selected pokemon.</returns>
        [Route("go-transfer-without-symbol")]
        public bool GoTransferWithoutSymbol(int pokemonId)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<Pokemon> pokemonList = this.dataService.GetObjects<Pokemon>(includes: "Game");
                List<Pokemon> updatedPokemonList = pokemonList.Where(x => (x.PokedexNumber <= 150 || x.Name == "Meltan" || x.Name == "Melmetal") && x.Game.GenerationId <= 7).ToList();
                List<Evolution> evolutions = this.dataService.GetObjects<Evolution>("EvolutionPokemon.PokedexNumber, EvolutionPokemonId", "EvolutionPokemon, EvolutionPokemon.Game");
                List<Pokemon> futureEvolutions = evolutions.Where(x => updatedPokemonList.Any(y => y.Id == x.PreevolutionPokemonId)).ToList().ConvertAll(x => x.EvolutionPokemon);
                List<PokemonFormDetail> formDetails = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, Form");
                futureEvolutions.Where(x => formDetails.ConvertAll(x => x.AltFormPokemon).Any(y => y.Id == x.Id)).ToList();
                updatedPokemonList.AddRange(futureEvolutions);
                if (updatedPokemonList.Exists(x => x.Id == pokemonId))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Grabs all of the pokeballs available for the selected game.
        /// </summary>
        /// <param name="gameId">The selected game's Id.</param>
        /// <param name="huntingMethodId">The selected hunting method's Id.</param>
        /// <returns>The list of available pokeballs.</returns>
        [Route("get-shiny-hunt-pokeballs")]
        public List<Pokeball> GetShinyHuntPokeballs(int gameId, int huntingMethodId)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<Pokeball> pokeballs = this.dataService.GetPokeballs(gameId, huntingMethodId, this.User, this.appConfig);

                return pokeballs;
            }

            return null;
        }

        /// <summary>
        /// Grabs all of the marks available for the selected game.
        /// </summary>
        /// <param name="gameId">The selected game's Id.</param>
        /// <param name="huntingMethodId">The selected hunting method's Id.</param>
        /// <returns>The list of available marks.</returns>
        [Route("get-shiny-hunt-marks")]
        public List<Mark> GetShinyHuntMarks(int gameId, int huntingMethodId)
        {
            Game game = this.dataService.GetObjectByPropertyValue<Game>("Id", gameId);
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest" && game.GenerationId >= 8 && gameId != 35 && gameId != 36 && gameId != 37 && huntingMethodId != 4 && huntingMethodId != 5)
            {
                List<Mark> marks = this.dataService.GetObjects<MarkGameDetail>("Mark.Name", "Mark", "GameId", gameId).ConvertAll(x => x.Mark);

                return marks;
            }

            return null;
        }

        /// <summary>
        /// Toggles whether or not a user will see alternate forms on the shiny dex progression page upon visiting it.
        /// </summary>
        /// <param name="altFormToggle">Tells the method whether or not the user wants shiny alternate forms to be shown.</param>
        [Route("toggle-shiny-alt-forms")]
        public void ToggleShinyAltForms(string altFormToggle)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest" && this.User.Identity.Name != null)
            {
                User user = this.dataService.GetCurrentUser(this.User);
                switch (altFormToggle)
                {
                    case "hide":
                        user.ShowShinyAltForms = false;
                        break;
                    case "show":
                        user.ShowShinyAltForms = true;
                        break;
                    default:
                        user.ShowShinyAltForms = false;
                        break;
                }

                this.dataService.UpdateObject<User>(user);
            }
        }

        /// <summary>
        /// Toggles whether or not a user will see gender differences on the shiny dex progression page upon visiting it.
        /// </summary>
        /// <param name="genderDifferenceToggle">Tells the method whether or not the user wants shiny gender differences to be shown.</param>
        [Route("toggle-shiny-gender-differences")]
        public void ToggleShinyGenderDifferences(string genderDifferenceToggle)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest" && this.User.Identity.Name != null)
            {
                User user = this.dataService.GetCurrentUser(this.User);
                switch (genderDifferenceToggle)
                {
                    case "hide":
                        user.ShowShinyGenderDifferences = false;
                        break;
                    case "show":
                        user.ShowShinyGenderDifferences = true;
                        break;
                    default:
                        user.ShowShinyGenderDifferences = false;
                        break;
                }

                this.dataService.UpdateObject<User>(user);
            }
        }

        /// <summary>
        /// Toggles whether or not a user will see pokemon already captured shiny on the shiny dex progression page upon visiting it.
        /// </summary>
        /// <param name="capturedShiniesToggle">Tells the method whether or not the user wants pokemon already captured shiny to be shown.</param>
        [Route("toggle-captured-shinies")]
        public void ToggleCapturedShinies(string capturedShiniesToggle)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest" && this.User.Identity.Name != null)
            {
                User user = this.dataService.GetCurrentUser(this.User);
                switch (capturedShiniesToggle)
                {
                    case "hide":
                        user.HideCapturedShinyPokemon = true;
                        break;
                    case "show":
                        user.HideCapturedShinyPokemon = false;
                        break;
                    default:
                        user.HideCapturedShinyPokemon = false;
                        break;
                }

                this.dataService.UpdateObject<User>(user);
            }
        }

        /// <summary>
        /// Inserts Ogerpon's Tera ability into the pokemon page.
        /// </summary>
        /// <param name="pokemonId">The Pokemon's Id. Used to modify the description of the ability.</param>
        /// <returns>Returns the description of the ability Embody Aspect based on Ogerpon's Tera.</returns>
        [Route("grab-ogerpon-tera-ability")]
        public Ability GrabOgerponTeraAbility(int pokemonId)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest" && this.User.Identity.Name != null)
            {
                // Embody Aspect's internal id.
                Ability teraAbility = this.dataService.GetObjectByPropertyValue<Ability>("Id", 314);
                switch (pokemonId)
                {
                    // Regular Ogerpon's internal id.
                    case 1768:
                        // Uses default description.
                        break;

                    // Wellspring Mask Ogerpon's internal id.
                    case 1879:
                        teraAbility.Description = "The Pokémon's heart fills with memories, causing the Wellspring Mask to shine and the Pokémon's Sp. Def stat to be boosted.";
                        break;

                    // Hearthflame Mask Ogerpon's internal id.
                    case 1880:

                        teraAbility.Description = "The Pokémon's heart fills with memories, causing the Hearthflame Mask to shine and the Pokémon's Attack stat to be boosted.";
                        break;

                    // Cornerstone Mask Ogerpon's internal id.
                    case 1881:
                        teraAbility.Description = "The Pokémon's heart fills with memories, causing the Cornerstone Mask to shine and the Pokémon's Defense stat to be boosted.";
                        break;
                }

                return teraAbility;
            }

            return null;
        }

        /// <summary>
        /// Gets the abilities for the given Ogerpon form combination.
        /// </summary>
        /// <param name="pokemonId">Ogerpon's id.</param>
        /// <returns>The regular ability of Ogerpon.</returns>
        [Route("grab-ogerpon-regular-ability")]
        public Ability GrabOgerponRegularAbility(int pokemonId)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                PokemonAbilityDetail pokemonAbilityDetail = this.dataService.GetObjects<PokemonAbilityDetail>(includes: "PrimaryAbility", whereProperty: "PokemonId", wherePropertyValue: pokemonId).Find(x => x.GenerationId == 9);
                return pokemonAbilityDetail.PrimaryAbility;
            }

            return null;
        }

        /// <summary>
        /// Abandon a shiny hunt.
        /// </summary>
        /// <param name="shinyHuntId">The abandoned shiny hunt's Id.</param>
        [Route("abandon-shiny-hunt")]
        public void DeleteShinyHunt(int shinyHuntId)
        {
            ShinyHunt shinyHunt = this.dataService.GetObjectByPropertyValue<ShinyHunt>("Id", shinyHuntId);
            User user = this.dataService.GetCurrentUser(this.User);
            if (user != null && user.Id == shinyHunt.UserId)
            {
                List<ShinyHunt> phaseHunts = this.dataService.GetObjects<ShinyHunt>(whereProperty: "PhaseOfHuntId", wherePropertyValue: shinyHuntId);
                foreach (var sh in phaseHunts)
                {
                    sh.PhaseOfHuntId = null;
                    this.dataService.UpdateObject(sh);
                }

                this.dataService.DeleteObject<ShinyHunt>(shinyHuntId);
            }
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

        private List<PokemonAbilityDetail> GetPokemonByAbilityAndGame(int abilityId, int gameId)
        {
            List<PokemonAbilityDetail> pokemonList = new List<PokemonAbilityDetail>();
            if (gameId != 0)
            {
                Game game = this.dataService.GetObjectByPropertyValue<Game>("Id", gameId);
                pokemonList = this.dataService.GetObjects<PokemonAbilityDetail>(includes: "Pokemon, Pokemon.Game")
                    .Where(x => x.Pokemon.Game.GenerationId <= game.GenerationId)
                    .Where(x => x.GenerationId <= game.GenerationId)
                    .OrderBy(x => x.GenerationId)
                    .GroupBy(x => new { x.PokemonId })
                    .Select(x => x.LastOrDefault())
                    .ToList();

                List<int> exclusionList = pokemonList.Select(x => x.PokemonId).Except(this.dataService.GetObjects<PokemonGameDetail>(includes: "Pokemon, Game", whereProperty: "GameId", wherePropertyValue: gameId).Select(x => x.PokemonId)).ToList();

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
                    if (abilityList.Count() <= 0)
                    {
                        exportAbilities = false;
                    }

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
                            comment.CommentorId = this.dataService.GetCurrentUser(this.User).Id;
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

        private List<Pokemon> FilterLegendaries(List<Pokemon> pokemonList, List<string> selectedLegendaries, bool onlyLegendaries)
        {
            try
            {
                List<PokemonLegendaryDetail> allLegendaries = this.dataService.GetObjects<PokemonLegendaryDetail>(includes: "Pokemon, LegendaryType");

                if (selectedLegendaries.Count() > 0)
                {
                    List<string> legendaryTypes = this.dataService.GetObjects<LegendaryType>("Type").ConvertAll(x => x.Type);
                    if (selectedLegendaries.Count() < legendaryTypes.Count())
                    {
                        foreach (var legendary in legendaryTypes.Except(selectedLegendaries).ToList())
                        {
                            pokemonList = pokemonList.Where(x => allLegendaries.Any(y => y.LegendaryType.Type != legendary && y.PokemonId == x.Id)).ToList();
                        }
                    }

                    if (onlyLegendaries)
                    {
                        pokemonList = pokemonList.Where(x => allLegendaries.Any(y => y.PokemonId == x.Id)).ToList();
                    }
                }
                else
                {
                    foreach (var p in allLegendaries)
                    {
                        pokemonList.Remove(pokemonList.Find(x => x.Id == p.PokemonId));
                    }
                }

                return pokemonList;
            }
            catch (Exception e)
            {
                if (!this.User.IsInRole("Owner"))
                {
                    string commentBody;

                    if (e != null)
                    {
                        commentBody = string.Concat(e.GetType().ToString(), " error during team generation. More specifically while filtering legendaries.");
                    }
                    else
                    {
                        commentBody = "Unknown error during team generation. More specifically while filtering legendaries.";
                    }

                    commentBody = string.Concat(commentBody, " - Selected Legendary Types: {", string.Join(", ", selectedLegendaries), "}");
                    commentBody = string.Concat(commentBody, " - Only Legendaries: ", onlyLegendaries);

                    Comment comment = new Comment()
                    {
                        Name = commentBody,
                    };

                    if (this.User.Identity.Name != null)
                    {
                        comment.CommentorId = this.dataService.GetCurrentUser(this.User).Id;
                    }

                    this.dataService.AddObject(comment);

                    this.dataService.EmailComment(this.appConfig, comment);
                }

                return new List<Pokemon>();
            }
        }

        private List<Pokemon> FilterForms(List<Pokemon> pokemonList, List<string> formList, Game game, bool onlyAltForms, bool multipleMegas, bool multipleGMax)
        {
            try
            {
                List<PokemonFormDetail> availableForms = new List<PokemonFormDetail>();
                List<PokemonFormDetail> altFormList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form");
                altFormList.ForEach(x => x.AltFormPokemon.Name = string.Concat(x.AltFormPokemon.Name, " (", x.Form.Name, ")"));

                if (formList.Count() != 0)
                {
                    List<PokemonFormDetail> altForm = new List<PokemonFormDetail>();
                    List<Pokemon> allAltForms = new List<Pokemon>();
                    if (formList.Where(x => x != "Other").Count() > 0)
                    {
                        FormGroup formGroup;
                        List<Form> forms;
                        foreach (var formGroupName in formList.Where(x => x != "Other"))
                        {
                            formGroup = this.dataService.GetObjectByPropertyValue<FormGroup>("Name", formGroupName);
                            forms = this.dataService.GetObjects<Form>(whereProperty: "FormGroupId", wherePropertyValue: formGroup.Id);
                            availableForms.AddRange(altFormList.Where(x => forms.Any(y => y.Id == x.FormId)));
                        }
                    }

                    if (formList.Contains("Other"))
                    {
                        List<Form> formsToRemove = this.dataService.GetObjects<Form>("Name").Where(x => !x.Randomizable).ToList();
                        availableForms.AddRange(altFormList.Where(x => !formsToRemove.Any(y => y.Name == x.Form.Name)));
                    }

                    foreach (var p in pokemonList)
                    {
                        altForm = availableForms.Where(x => x.OriginalPokemonId == p.Id).ToList();

                        if (altForm.Count > 0)
                        {
                            allAltForms.AddRange(altForm.Select(x => x.AltFormPokemon));
                        }
                    }

                    pokemonList.AddRange(allAltForms);
                    pokemonList = this.RemoveExtraPokemonForms(pokemonList, altFormList, multipleMegas, multipleGMax);
                }

                if (onlyAltForms)
                {
                    List<Pokemon> alternateForms = altFormList.ConvertAll(x => x.AltFormPokemon);
                    pokemonList = pokemonList.Where(x => alternateForms.Any(y => y.Id == x.Id)).ToList();
                }

                if (game.Id != 0)
                {
                    List<PokemonGameDetail> pokemonGameDetails = this.dataService.GetObjects<PokemonGameDetail>(whereProperty: "GameId", wherePropertyValue: game.Id);
                    pokemonList = pokemonList.Where(x => pokemonGameDetails.Any(y => y.PokemonId == x.Id)).ToList();
                }

                return pokemonList;
            }
            catch (Exception e)
            {
                if (!this.User.IsInRole("Owner"))
                {
                    string commentBody;

                    if (e != null)
                    {
                        commentBody = string.Concat(e.GetType().ToString(), " error during team generation. More specifically while filtering alternate forms.");
                    }
                    else
                    {
                        commentBody = "Unknown error during team generation. More specifically while filtering alternate forms.";
                    }

                    commentBody = string.Concat(commentBody, " - Only Alternate Forms: ", onlyAltForms);
                    commentBody = string.Concat(commentBody, " - Selected Forms: {", string.Join(", ", formList), "}");
                    commentBody = string.Concat(commentBody, " - Multiple Megas: ", multipleMegas);
                    commentBody = string.Concat(commentBody, " - Multiple Gigantamax: ", multipleGMax);

                    Comment comment = new Comment()
                    {
                        Name = commentBody,
                    };

                    if (this.User.Identity.Name != null)
                    {
                        comment.CommentorId = this.dataService.GetCurrentUser(this.User).Id;
                    }

                    this.dataService.AddObject(comment);

                    this.dataService.EmailComment(this.appConfig, comment);
                }

                return new List<Pokemon>();
            }
        }

        private List<Pokemon> RemoveExtraPokemonForms(List<Pokemon> pokemonList, List<PokemonFormDetail> altFormList, bool multipleMegas, bool multipleGMax)
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

            if (!multipleMegas)
            {
                List<Pokemon> megaList = new List<Pokemon>();
                foreach (var p in altFormList.Where(x => x.Form.Name.Contains("Mega")).ToList())
                {
                    if (pokemonList.Exists(x => x.Id == p.AltFormPokemonId))
                    {
                        megaList.Add(p.AltFormPokemon);
                    }
                }

                if (megaList.Count > 0)
                {
                    Pokemon mega = megaList[rnd.Next(megaList.Count)];
                    foreach (var p in megaList.Where(x => x.Id != mega.Id))
                    {
                        if (pokemonList.Exists(x => x.Id == p.Id))
                        {
                            pokemonList.Remove(pokemonList.Find(x => x.Id == p.Id));
                        }
                    }
                }
            }

            if (!multipleGMax)
            {
                List<Pokemon> allGigantamaxPokemon = altFormList.Where(x => x.Form.Name.Contains("Gigantamax")).Select(x => x.AltFormPokemon).ToList();
                List<Pokemon> filteredGigantamaxList = pokemonList.Where(x => allGigantamaxPokemon.Any(y => y.Id == x.Id)).ToList();

                if (filteredGigantamaxList.Count > 0)
                {
                    Pokemon gigantamax = filteredGigantamaxList[rnd.Next(filteredGigantamaxList.Count)];
                    foreach (var p in filteredGigantamaxList.Where(x => x.Id != gigantamax.Id))
                    {
                        if (pokemonList.Exists(x => x.Id == p.Id))
                        {
                            pokemonList.Remove(pokemonList.Find(x => x.Id == p.Id));
                        }
                    }
                }
            }

            return pokemonList;
        }

        private (List<Pokemon> pokemonList, List<Pokemon> starterList) FilterEvolutions(List<Pokemon> pokemonList, List<Pokemon> starterList, List<string> evolutionList, Game game)
        {
            try
            {
                List<Evolution> allEvolutions = this.dataService.GetObjects<Evolution>(includes: "PreevolutionPokemon, PreevolutionPokemon.Game, EvolutionPokemon, EvolutionPokemon.Game, EvolutionMethod");
                List<PokemonFormDetail> altForms = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, OriginalPokemon, Form");
                if (game.Id != 0)
                {
                    allEvolutions = allEvolutions.Where(x => x.PreevolutionPokemon.Game.ReleaseDate <= game.ReleaseDate && x.EvolutionPokemon.Game.ReleaseDate <= game.ReleaseDate).ToList();
                }

                if (game.Id == 16)
                {
                    allEvolutions = allEvolutions.Where(x => x.GenerationId == 1 || x.PreevolutionPokemonId == 1339).ToList();
                }

                if (evolutionList.Count > 0 && evolutionList.Count() < 3)
                {
                    List<Pokemon> evolutions = new List<Pokemon>();
                    List<Pokemon> starterEvolutions = new List<Pokemon>();
                    if (evolutionList.Contains("stage1Pokemon"))
                    {
                        foreach (var p in pokemonList)
                        {
                            if ((allEvolutions.Exists(x => x.PreevolutionPokemonId == p.Id) && !allEvolutions.Exists(x => x.EvolutionPokemonId == p.Id)) || (p.Id == 89 && game.Id == 20 && !starterEvolutions.Exists(x => x.Id == p.Id)) || ((p.Id == 1595 || p.Id == 1596) && game.Id == 16 && !starterEvolutions.Exists(x => x.Id == p.Id)))
                            {
                                evolutions.Add(p);
                            }
                        }

                        foreach (var p in starterList)
                        {
                            if ((allEvolutions.Exists(x => x.PreevolutionPokemonId == p.Id) && !allEvolutions.Exists(x => x.EvolutionPokemonId == p.Id)) || (p.Id == 89 && game.Id == 20 && !starterEvolutions.Exists(x => x.Id == p.Id)) || ((p.Id == 1595 || p.Id == 1596) && game.Id == 16 && !starterEvolutions.Exists(x => x.Id == p.Id)))
                            {
                                starterEvolutions.Add(p);
                            }
                        }
                    }

                    if (evolutionList.Contains("middleEvolution"))
                    {
                        foreach (var p in pokemonList)
                        {
                            if ((allEvolutions.Exists(x => x.PreevolutionPokemonId == p.Id) && allEvolutions.Exists(x => x.EvolutionPokemonId == p.Id)) || (p.Id == 89 && game.Id == 20 && !starterEvolutions.Exists(x => x.Id == p.Id)) || ((p.Id == 1595 || p.Id == 1596) && game.Id == 16 && !starterEvolutions.Exists(x => x.Id == p.Id)))
                            {
                                evolutions.Add(p);
                            }
                        }

                        foreach (var p in starterList)
                        {
                            if ((allEvolutions.Exists(x => x.PreevolutionPokemonId == p.Id) && allEvolutions.Exists(x => x.EvolutionPokemonId == p.Id)) || (p.Id == 89 && game.Id == 20 && !starterEvolutions.Exists(x => x.Id == p.Id)) || ((p.Id == 1595 || p.Id == 1596) && game.Id == 16 && !starterEvolutions.Exists(x => x.Id == p.Id)))
                            {
                                starterEvolutions.Add(p);
                            }
                        }
                    }

                    if (evolutionList.Contains("onlyFullyEvolved") || (game.Id != 16 && game.Id != 20 && evolutionList.Contains("noEvolutionLine")))
                    {
                        foreach (var p in pokemonList)
                        {
                            if ((!allEvolutions.Exists(x => x.PreevolutionPokemonId == p.Id) && allEvolutions.Exists(x => x.EvolutionPokemonId == p.Id)) || (p.Id == 89 && game.Id == 20 && !starterEvolutions.Exists(x => x.Id == p.Id)) || ((p.Id == 1595 || p.Id == 1596) && game.Id == 16 && !starterEvolutions.Exists(x => x.Id == p.Id)))
                            {
                                evolutions.Add(p);
                            }
                        }

                        foreach (var p in starterList)
                        {
                            if ((!allEvolutions.Exists(x => x.PreevolutionPokemonId == p.Id) && allEvolutions.Exists(x => x.EvolutionPokemonId == p.Id)) || (p.Id == 89 && game.Id == 20 && !starterEvolutions.Exists(x => x.Id == p.Id)) || ((p.Id == 1595 || p.Id == 1596) && game.Id == 16 && !starterEvolutions.Exists(x => x.Id == p.Id)))
                            {
                                starterEvolutions.Add(p);
                            }
                        }
                    }

                    if (evolutionList.Contains("noEvolutionLine"))
                    {
                        foreach (var p in pokemonList)
                        {
                            if ((!allEvolutions.Exists(x => x.PreevolutionPokemonId == p.Id) && !allEvolutions.Exists(x => x.EvolutionPokemonId == p.Id)) || (p.Id == 89 && game.Id == 20 && !starterEvolutions.Exists(x => x.Id == p.Id)) || ((p.Id == 1595 || p.Id == 1596) && game.Id == 16 && !starterEvolutions.Exists(x => x.Id == p.Id)))
                            {
                                evolutions.Add(p);
                            }
                        }

                        if (game.Id == 20 || game.Id == 16)
                        {
                            foreach (var p in starterList)
                            {
                                if ((!allEvolutions.Exists(x => x.PreevolutionPokemonId == p.Id) && !allEvolutions.Exists(x => x.EvolutionPokemonId == p.Id)) || (p.Id == 89 && game.Id == 20 && !starterEvolutions.Exists(x => x.Id == p.Id)) || ((p.Id == 1595 || p.Id == 1596) && game.Id == 16 && !starterEvolutions.Exists(x => x.Id == p.Id)))
                                {
                                    starterEvolutions.Add(p);
                                }
                            }
                        }
                    }

                    pokemonList = evolutions;
                    altForms = altForms.Where(x => pokemonList.Any(y => y.Id == x.OriginalPokemonId)).ToList();
                    altForms = altForms.Where(x => !pokemonList.Any(y => y.Id == x.AltFormPokemonId)).ToList();
                    pokemonList = pokemonList.Where(x => !altForms.Any(y => y.AltFormPokemonId == x.Id)).ToList();
                    starterList = starterEvolutions;
                }

                return (pokemonList, starterList);
            }
            catch (Exception e)
            {
                if (!this.User.IsInRole("Owner"))
                {
                    string commentBody;

                    if (e != null)
                    {
                        commentBody = string.Concat(e.GetType().ToString(), " error during team generation. More specifically while filtering evolutions.");
                    }
                    else
                    {
                        commentBody = "Unknown error during team generation. More specifically while filtering evolutions.";
                    }

                    commentBody = string.Concat(commentBody, " - Selected Evolutions: {", string.Join(", ", evolutionList), "}");
                    commentBody = string.Concat(commentBody, " - Selected Game: ", game.Id);
                    commentBody = string.Concat(commentBody, " - Needs Starter: ", starterList.Count() > 0);

                    Comment comment = new Comment()
                    {
                        Name = commentBody,
                    };

                    if (this.User.Identity.Name != null)
                    {
                        comment.CommentorId = this.dataService.GetCurrentUser(this.User).Id;
                    }

                    this.dataService.AddObject(comment);

                    this.dataService.EmailComment(this.appConfig, comment);
                }

                return (new List<Pokemon>(), new List<Pokemon>());
            }
        }

        private List<Pokemon> FilterTypes(List<Pokemon> pokemonList, int typeId, Game game)
        {
            try
            {
                if (typeId != 0)
                {
                    List<PokemonTypeDetail> allTypes = new List<PokemonTypeDetail>();
                    if (game.Id != 0)
                    {
                        allTypes = this.GetAllPokemonWithSpecificType(typeId, game.GenerationId, pokemonList);
                    }
                    else
                    {
                        allTypes = this.dataService.GetObjects<PokemonTypeDetail>("Pokemon.PokedexNumber, PokemonId", "Pokemon, Pokemon.Game, PrimaryType, SecondaryType").Where(x => pokemonList.Any(y => y.Id == x.PokemonId)).ToList();
                        allTypes = allTypes.Where(x => x.PrimaryTypeId == typeId || x.SecondaryTypeId == typeId).ToList();
                    }

                    pokemonList = pokemonList.Where(x => allTypes.Select(x => x.Pokemon).Any(y => y.Id == x.Id)).ToList();
                }

                return pokemonList;
            }
            catch (Exception e)
            {
                if (!this.User.IsInRole("Owner"))
                {
                    string commentBody;

                    if (e != null)
                    {
                        commentBody = string.Concat(e.GetType().ToString(), " error during team generation. More specifically while filtering types.");
                    }
                    else
                    {
                        commentBody = "Unknown error during team generation. More specifically while filtering types.";
                    }

                    commentBody = string.Concat(commentBody, " - Selected Type: ", typeId);
                    if (game.Id != 0)
                    {
                        commentBody = string.Concat(commentBody, " - Selected Game: ", game.Name);
                    }
                    else
                    {
                        commentBody = string.Concat(commentBody, " - No Selected Game");
                    }

                    Comment comment = new Comment()
                    {
                        Name = commentBody,
                    };

                    if (this.User.Identity.Name != null)
                    {
                        comment.CommentorId = this.dataService.GetCurrentUser(this.User).Id;
                    }

                    this.dataService.AddObject(comment);

                    this.dataService.EmailComment(this.appConfig, comment);
                }

                return new List<Pokemon>();
            }
        }

        private List<PokemonTypeDetail> GetAllPokemonWithSpecificTypes(int primaryTypeId, int secondaryTypeId, Game game, int regionalDexId)
        {
            List<PokemonTypeDetail> pokemonList = this.dataService.GetObjects<PokemonTypeDetail>("GenerationId", "Pokemon, Pokemon.Game, PrimaryType, SecondaryType").Where(x => x.GenerationId <= game.GenerationId).GroupBy(x => new { x.PokemonId }).Select(x => x.LastOrDefault()).ToList();
            if (regionalDexId != 0)
            {
                List<RegionalDexEntry> regionalDexEntries = this.dataService.GetObjects<RegionalDexEntry>(whereProperty: "RegionalDexId", wherePropertyValue: regionalDexId);
                pokemonList = pokemonList.Where(x => regionalDexEntries.Any(y => y.PokemonId == x.PokemonId)).ToList();
            }

            if (game.Id != 0)
            {
                pokemonList = pokemonList.Where(x => x.GenerationId <= game.GenerationId).ToList();
            }

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
                List<PokemonTypeDetail> pokemonList = this.dataService.GetObjects<PokemonTypeDetail>(includes: "Pokemon, Pokemon.Game, PrimaryType, SecondaryType").Where(x => allPokemon.Any(y => y.Id == x.PokemonId)).OrderBy(x => x.GenerationId).ToList();

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
                        comment.CommentorId = this.dataService.GetCurrentUser(this.User).Id;
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
            ExportPokemonViewModel pokemonTeam = new ExportPokemonViewModel
            {
                ExportString = string.Empty,
                TeamId = team.Id,
            };

            if (pokemonList.Count > 0)
            {
                pokemonTeam.ExportString = "=== ";

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
            }

            return pokemonTeam;
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
                else if (!string.IsNullOrEmpty(pokemonTeamDetail.BattleItemString) && generationId != 1)
                {
                    pokemonName = string.Concat(pokemonName, " @ ", pokemonTeamDetail.BattleItemString);
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

                if (generationId != 1 && pokemonTeamDetail.PokemonTeamMoveset.CheckMove("Frustration"))
                {
                    pokemonTeamString = string.Concat(pokemonTeamString, "\nHappiness: 0");
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
                        comment.CommentorId = this.dataService.GetCurrentUser(this.User).Id;
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
