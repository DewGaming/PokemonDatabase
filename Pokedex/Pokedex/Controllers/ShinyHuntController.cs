using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MoreLinq;
using Pokedex.DataAccess.Models;
using Pokedex.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pokedex.Controllers
{
    /// <summary>
    /// The class that handles all ShinyHunt related requests.
    /// </summary>
    [Authorize]
    [Route("")]
    public class ShinyHuntController : Controller
    {
        private readonly DataService dataService;

        private readonly AppConfig appConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShinyHuntController"/> class.
        /// </summary>
        /// <param name="appConfig">The application's configuration.</param>
        /// <param name="dataContext">The pokemon database context.</param>
        public ShinyHuntController(IOptions<AppConfig> appConfig, DataContext dataContext)
        {
            // Instantiate an instance of the data service.
            this.appConfig = appConfig.Value;
            this.dataService = new DataService(dataContext);
        }

        /// <summary>
        /// Shows shiny dex progress.
        /// </summary>
        /// <returns>The shiny dex progress page.</returns>
        [Route("shiny_dex_progress")]
        public IActionResult ShinyDexProgress()
        {
            this.dataService.AddPageView("Shiny Dex Progression Page", this.User.IsInRole("Owner"));
            int userId = this.dataService.GetObjectByPropertyValue<User>("Username", this.User.Identity.Name).Id;
            List<ShinyHunt> shinyHunts = this.dataService.GetObjects<ShinyHunt>("Game.GenerationId, Pokemon.PokedexNumber, PokemonId, Id", "User, Pokemon, Pokemon.Game, Game, HuntingMethod, Mark, Pokeball", "UserId", userId).Where(x => x.IsCaptured).ToList();
            List<Pokemon> pokemonCaptured = shinyHunts.ConvertAll(x => x.Pokemon).DistinctBy(x => x.Id).ToList();
            List<Pokemon> pokemonList = this.dataService.GetNonBattlePokemonWithFormNames().Where(x => x.IsComplete && !x.IsShinyLocked).ToList();
            List<Pokemon> altFormList = this.dataService.GetObjects<PokemonFormDetail>("AltFormPokemon.PokedexNumber, AltFormPokemon.Id", "AltFormPokemon, AltFormPokemon.Game").ConvertAll(x => x.AltFormPokemon);
            List<Pokemon> primals = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, Form", whereProperty: "Form.Name", wherePropertyValue: "Primal").ConvertAll(x => x.AltFormPokemon);
            pokemonList = pokemonList.Where(x => !primals.Any(y => y.Id == x.Id)).ToList();

            List<PokemonShinyHuntDetails> pokemonShinyHuntList = pokemonList.ConvertAll(x => new PokemonShinyHuntDetails() { Pokemon = x, IsCaptured = false, IsAltForm = altFormList.Exists(y => x.Id == y.Id) });
            pokemonShinyHuntList.Where(x => pokemonCaptured.Any(y => y.Id == x.Pokemon.Id)).ToList().ForEach(x => x.IsCaptured = true);

            ShinyDexViewModel model = new ShinyDexViewModel()
            {
                AllPokemon = pokemonShinyHuntList.OrderBy(x => x.Pokemon.PokedexNumber).ThenBy(x => x.Pokemon.Id).ToList(),
                AllShinyHunts = shinyHunts,
                AllGames = this.dataService.GetObjects<Game>(),
                AppConfig = this.appConfig,
            };

            return this.View(model);
        }

        /// <summary>
        /// Starts a shiny hunt.
        /// </summary>
        /// <returns>The shiny hunt page.</returns>
        [HttpGet]
        [Route("start_hunt")]
        public IActionResult StartHunt()
        {
            StartShinyHuntViewModel model = new StartShinyHuntViewModel()
            {
                AllPokemon = this.dataService.GetHuntablePokemon(),
                UserId = this.dataService.GetObjectByPropertyValue<User>("Username", this.User.Identity.Name).Id,
                AppConfig = this.appConfig,
            };

            return this.View(model);
        }

        /// <summary>
        /// Starts a shiny hunt.
        /// </summary>
        /// <param name="numberOfHunts">The number of hunts being added. Either one hunt or multiple.</param>
        /// <param name="shinyHunt">The started shiny hunt.</param>
        /// <returns>The user's shiny hunt page.</returns>
        [HttpPost]
        [Route("start_hunt")]
        public IActionResult StartHunt(string numberOfHunts, StartShinyHuntViewModel shinyHunt)
        {
            if (!this.ModelState.IsValid)
            {
                StartShinyHuntViewModel model = new StartShinyHuntViewModel()
                {
                    AllPokemon = this.dataService.GetHuntablePokemon(),
                    UserId = this.dataService.GetObjectByPropertyValue<User>("Username", this.User.Identity.Name).Id,
                    AppConfig = this.appConfig,
                };

                return this.View(model);
            }

            shinyHunt.Phases = 1;
            shinyHunt.IncrementAmount = 1;

            this.dataService.AddObject(shinyHunt);

            if (shinyHunt.HuntingMethodId == 8)
            {
                return this.RedirectToAction("ShinyFound", "ShinyHunt", new { shinyHuntId = shinyHunt.Id });
            }
            else
            {
                if (numberOfHunts == "oneHunt")
                {
                    return this.RedirectToAction("ShinyHunts", "User");
                }
                else if (numberOfHunts == "multipleHunts")
                {
                    return this.RedirectToAction("StartHunt", "ShinyHunt");
                }
                else
                {
                    return this.RedirectToAction("ShinyHunts", "User");
                }
            }
        }

        /// <summary>
        /// Starts a shiny hunt.
        /// </summary>
        /// <returns>The shiny hunt page.</returns>
        [HttpGet]
        [Route("add_completed_hunt")]
        public IActionResult AddCompletedHunt()
        {
            AddCompletedShinyHuntViewModel model = new AddCompletedShinyHuntViewModel()
            {
                AllPokemon = this.dataService.GetHuntablePokemon(),
                DateOfCapture = DateTime.Now.Date,
                Phases = 1,
                IncrementAmount = 1,
                UserId = this.dataService.GetObjectByPropertyValue<User>("Username", this.User.Identity.Name).Id,
                AppConfig = this.appConfig,
            };

            return this.View(model);
        }

        /// <summary>
        /// Starts a shiny hunt.
        /// </summary>
        /// <param name="numberOfHunts">The number of hunts being added. Either one hunt or multiple.</param>
        /// <param name="shinyHunt">The started shiny hunt.</param>
        /// <returns>The user's shiny hunt page.</returns>
        [HttpPost]
        [Route("add_completed_hunt")]
        public IActionResult AddCompletedHunt(string numberOfHunts, AddCompletedShinyHuntViewModel shinyHunt)
        {
            if (!this.ModelState.IsValid)
            {
                AddCompletedShinyHuntViewModel model = new AddCompletedShinyHuntViewModel()
                {
                    AllPokemon = this.dataService.GetHuntablePokemon(),
                    AllMarks = this.dataService.GetObjects<Mark>("Name"),
                    DateOfCapture = DateTime.Now.Date,
                    Phases = 1,
                    IncrementAmount = 1,
                    UserId = this.dataService.GetObjectByPropertyValue<User>("Username", this.User.Identity.Name).Id,
                    AppConfig = this.appConfig,
                };

                return this.View(model);
            }

            shinyHunt.IsCaptured = true;

            if (shinyHunt.CurrentPhaseEncounters > 0 && shinyHunt.TotalEncounters == 0)
            {
                shinyHunt.TotalEncounters = shinyHunt.CurrentPhaseEncounters;
            }
            else if (shinyHunt.TotalEncounters > 0 && shinyHunt.CurrentPhaseEncounters == 0)
            {
                shinyHunt.CurrentPhaseEncounters = shinyHunt.TotalEncounters;
            }

            this.dataService.AddObject(shinyHunt);

            if (numberOfHunts == "oneHunt")
            {
                return this.RedirectToAction("ShinyHunts", "User");
            }
            else if (numberOfHunts == "multipleHunts")
            {
                return this.RedirectToAction("AddCompletedHunt", "ShinyHunt");
            }
            else
            {
                return this.RedirectToAction("ShinyHunts", "User");
            }
        }

        /// <summary>
        /// Complete a shiny hunt.
        /// </summary>
        /// <param name="shinyHuntId">The completed shiny hunt's Id.</param>
        /// <returns>The shiny hunt page.</returns>
        [HttpGet]
        [Route("complete_shiny_hunt/{shinyHuntId:int}")]
        public IActionResult ShinyFound(int shinyHuntId)
        {
            ShinyHunt shinyHunt = this.dataService.GetObjectByPropertyValue<ShinyHunt>("Id", shinyHuntId, "Game");
            Pokemon pokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", shinyHunt.PokemonId);
            List<string> genders = this.dataService.GrabGenders(pokemon.Id, "shinyHunt");

            shinyHunt.TotalEncounters += shinyHunt.CurrentPhaseEncounters;
            if (shinyHunt.Phases < 1)
            {
                shinyHunt.Phases = 1;
            }

            CompleteShinyHuntViewModel model = new CompleteShinyHuntViewModel(shinyHunt)
            {
                Id = shinyHunt.Id,
                PokemonHunted = pokemon,
                GameHuntedIn = this.dataService.GetObjectByPropertyValue<Game>("Id", shinyHunt.GameId),
                DateOfCapture = DateTime.Now.Date,
                AllPokeballs = this.dataService.GetPokeballs(shinyHunt.GameId, shinyHunt.HuntingMethodId),
                AllGenders = genders,
                AllMarks = this.dataService.GetObjects<Mark>("Name"),
                AllHuntingMethods = this.dataService.GetObjects<HuntingMethodGameDetail>(includes: "HuntingMethod", whereProperty: "GameId", wherePropertyValue: shinyHunt.GameId).ConvertAll(x => x.HuntingMethod),
                AppConfig = this.appConfig,
            };

            return this.View(model);
        }

        /// <summary>
        /// Completed a shiny hunt.
        /// </summary>
        /// <param name="shinyHunt">The completed shiny hunt.</param>
        /// <returns>The user's shiny hunt page.</returns>
        [HttpPost]
        [Route("complete_shiny_hunt/{shinyHuntId:int}")]
        public IActionResult ShinyFound(CompleteShinyHuntViewModel shinyHunt)
        {
            if (this.ModelState.IsValid)
            {
                shinyHunt.IsCaptured = true;
                this.dataService.UpdateObject(shinyHunt);
            }

            return this.RedirectToAction("ShinyHunts", "User");
        }

        /// <summary>
        /// Complete a shiny hunt.
        /// </summary>
        /// <param name="shinyHuntId">The completed shiny hunt's Id.</param>
        /// <returns>The shiny hunt page.</returns>
        [HttpGet]
        [Route("found_phase_shiny/{shinyHuntId:int}")]
        public IActionResult PhaseShinyFound(int shinyHuntId)
        {
            ShinyHunt shinyHunt = this.dataService.GetObjectByPropertyValue<ShinyHunt>("Id", shinyHuntId, "Game");
            Pokemon pokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", shinyHunt.PokemonId);
            List<Pokemon> pokemonList = this.dataService.GetHuntablePokemon(shinyHunt.GameId);
            List<string> genders = this.dataService.GrabGenders(shinyHunt.PokemonId, "shinyHunt");

            CompleteShinyHuntViewModel model = new CompleteShinyHuntViewModel(shinyHunt)
            {
                PokemonHunted = pokemon,
                GameHuntedIn = this.dataService.GetObjectByPropertyValue<Game>("Id", shinyHunt.GameId),
                DateOfCapture = DateTime.Now.Date,
                AllPokemon = pokemonList,
                AllPokeballs = this.dataService.GetPokeballs(shinyHunt.GameId, shinyHunt.HuntingMethodId),
                AllGenders = genders,
                AllMarks = this.dataService.GetObjects<Mark>("Name"),
                AllHuntingMethods = this.dataService.GetObjects<HuntingMethodGameDetail>(includes: "HuntingMethod", whereProperty: "GameId", wherePropertyValue: shinyHunt.GameId).ConvertAll(x => x.HuntingMethod),
                AppConfig = this.appConfig,
            };

            return this.View(model);
        }

        /// <summary>
        /// Completed a shiny hunt.
        /// </summary>
        /// <param name="shinyHunt">The completed shiny hunt.</param>
        /// <param name="shinyHuntId">The original shiny hunt's Id.</param>
        /// <returns>The user's shiny hunt page.</returns>
        [HttpPost]
        [Route("found_phase_shiny/{shinyHuntId:int}")]
        public IActionResult PhaseShinyFound(CompleteShinyHuntViewModel shinyHunt, int shinyHuntId)
        {
            if (this.ModelState.IsValid)
            {
                shinyHunt.IsCaptured = true;
                shinyHunt.TotalEncounters = shinyHunt.CurrentPhaseEncounters;
                shinyHunt.PhaseOfHuntId = shinyHuntId;
                shinyHunt.Phases = 1;
                this.dataService.AddObject(shinyHunt);

                ShinyHunt originalShinyHunt = this.dataService.GetObjectByPropertyValue<ShinyHunt>("Id", shinyHuntId);
                originalShinyHunt.TotalEncounters += originalShinyHunt.CurrentPhaseEncounters;
                originalShinyHunt.CurrentPhaseEncounters = 0;
                originalShinyHunt.Phases++;
                this.dataService.UpdateObject(originalShinyHunt);
            }

            return this.RedirectToAction("ShinyHunts", "User");
        }

        /// <summary>
        /// Updates an incomplete shiny hunt.
        /// </summary>
        /// <param name="shinyHuntId">The shiny hunt's id.</param>
        /// <returns>The shiny hunt edit page.</returns>
        [HttpGet]
        [Route("edit_incomplete_hunt/{shinyHuntId:int}")]
        public IActionResult EditIncompleteShinyHunt(int shinyHuntId)
        {
            ShinyHunt shinyHunt = this.dataService.GetObjectByPropertyValue<ShinyHunt>("Id", shinyHuntId);
            List<Game> pokemonGamesIn = this.dataService.GetObjects<PokemonGameDetail>("GameId", "Game", "PokemonId", shinyHunt.PokemonId).ConvertAll(x => x.Game);
            List<Game> gamesList = this.dataService.GetShinyHuntGames();

            EditShinyHuntViewModel model = new EditShinyHuntViewModel(shinyHunt)
            {
                AllPokemon = this.dataService.GetHuntablePokemon(),
                AllGames = gamesList.Where(x => pokemonGamesIn.Any(y => y.Id == x.Id)).ToList(),
                AllHuntingMethods = this.dataService.GetObjects<HuntingMethodGameDetail>(includes: "HuntingMethod", whereProperty: "GameId", wherePropertyValue: shinyHunt.GameId).ConvertAll(x => x.HuntingMethod),
                AppConfig = this.appConfig,
            };

            return this.View(model);
        }

        /// <summary>
        /// Updates an incomplete shiny hunt.
        /// </summary>
        /// <param name="shinyHunt">The editted shiny hunt.</param>
        /// <returns>The user's shiny hunt page.</returns>
        [HttpPost]
        [Route("edit_incomplete_hunt/{shinyHuntId:int}")]
        public IActionResult EditIncompleteShinyHunt(EditShinyHuntViewModel shinyHunt)
        {
            if (!this.ModelState.IsValid)
            {
                ShinyHunt oldShinyHunt = this.dataService.GetObjectByPropertyValue<ShinyHunt>("Id", shinyHunt.Id);
                List<Game> pokemonGamesIn = this.dataService.GetObjects<PokemonGameDetail>("GameId", "Game", "PokemonId", oldShinyHunt.PokemonId).ConvertAll(x => x.Game);
                List<Game> gamesList = this.dataService.GetShinyHuntGames();

                EditShinyHuntViewModel model = new EditShinyHuntViewModel(oldShinyHunt)
                {
                    AllPokemon = this.dataService.GetHuntablePokemon(),
                    AllGames = gamesList.Where(x => pokemonGamesIn.Any(y => y.Id == x.Id)).ToList(),
                    AllHuntingMethods = this.dataService.GetObjects<HuntingMethodGameDetail>(includes: "HuntingMethod", whereProperty: "GameId", wherePropertyValue: oldShinyHunt.GameId).ConvertAll(x => x.HuntingMethod),
                    AppConfig = this.appConfig,
                };

                return this.View(model);
            }

            this.dataService.UpdateObject(shinyHunt);

            return this.RedirectToAction("ShinyHunts", "User");
        }

        /// <summary>
        /// Updates a complete shiny hunt.
        /// </summary>
        /// <param name="shinyHuntId">The shiny hunt's id.</param>
        /// <returns>The shiny hunt edit page.</returns>
        [HttpGet]
        [Route("edit_complete_hunt/{shinyHuntId:int}")]
        public IActionResult EditCompleteShinyHunt(int shinyHuntId)
        {
            ShinyHunt shinyHunt = this.dataService.GetObjectByPropertyValue<ShinyHunt>("Id", shinyHuntId, "Pokemon, Game");
            List<Game> pokemonGamesIn = this.dataService.GetObjects<PokemonGameDetail>("GameId", "Game", "PokemonId", shinyHunt.PokemonId).ConvertAll(x => x.Game);
            List<Game> gamesList = this.dataService.GetShinyHuntGames();
            List<string> genders = this.dataService.GrabGenders(shinyHunt.PokemonId, "shinyHunt");

            EditShinyHuntViewModel model = new EditShinyHuntViewModel(shinyHunt)
            {
                PokemonHunted = shinyHunt.Pokemon,
                GameHuntedIn = shinyHunt.Game,
                AllPokemon = this.dataService.GetHuntablePokemon(),
                AllGames = gamesList.Where(x => pokemonGamesIn.Any(y => y.Id == x.Id)).ToList(),
                AllHuntingMethods = this.dataService.GetObjects<HuntingMethodGameDetail>(includes: "HuntingMethod", whereProperty: "GameId", wherePropertyValue: shinyHunt.GameId).ConvertAll(x => x.HuntingMethod),
                AllPokeballs = this.dataService.GetPokeballs(shinyHunt.GameId, shinyHunt.HuntingMethodId),
                AllMarks = this.dataService.GetObjects<Mark>(),
                AllGenders = genders,
                AppConfig = this.appConfig,
            };

            return this.View(model);
        }

        /// <summary>
        /// Updates a complete shiny hunt.
        /// </summary>
        /// <param name="shinyHunt">The editted shiny hunt.</param>
        /// <returns>The user's shiny hunt page.</returns>
        [HttpPost]
        [Route("edit_complete_hunt/{shinyHuntId:int}")]
        public IActionResult EditCompleteShinyHunt(EditShinyHuntViewModel shinyHunt)
        {
            if (!this.ModelState.IsValid)
            {
                ShinyHunt oldShinyHunt = this.dataService.GetObjectByPropertyValue<ShinyHunt>("Id", shinyHunt.Id, "Pokemon, Game");
                List<Game> pokemonGamesIn = this.dataService.GetObjects<PokemonGameDetail>("GameId", "Game", "PokemonId", oldShinyHunt.PokemonId).ConvertAll(x => x.Game);
                List<Game> gamesList = this.dataService.GetShinyHuntGames();
                List<string> genders = this.dataService.GrabGenders(oldShinyHunt.PokemonId, "shinyHunt");

                EditShinyHuntViewModel model = new EditShinyHuntViewModel(oldShinyHunt)
                {
                    PokemonHunted = oldShinyHunt.Pokemon,
                    GameHuntedIn = oldShinyHunt.Game,
                    AllPokemon = this.dataService.GetHuntablePokemon(),
                    AllGames = gamesList.Where(x => pokemonGamesIn.Any(y => y.Id == x.Id)).ToList(),
                    AllHuntingMethods = this.dataService.GetObjects<HuntingMethodGameDetail>(includes: "HuntingMethod", whereProperty: "GameId", wherePropertyValue: oldShinyHunt.GameId).ConvertAll(x => x.HuntingMethod),
                    AllPokeballs = this.dataService.GetPokeballs(oldShinyHunt.GameId, oldShinyHunt.HuntingMethodId),
                    AllMarks = this.dataService.GetObjects<Mark>(),
                    AllGenders = genders,
                    UserId = this.dataService.GetObjectByPropertyValue<User>("Username", this.User.Identity.Name).Id,
                    AppConfig = this.appConfig,
                };

                return this.View(model);
            }

            if (shinyHunt.CurrentPhaseEncounters > 0 && shinyHunt.TotalEncounters == 0)
            {
                shinyHunt.TotalEncounters = shinyHunt.CurrentPhaseEncounters;
            }
            else if (shinyHunt.TotalEncounters > 0 && shinyHunt.CurrentPhaseEncounters == 0)
            {
                shinyHunt.CurrentPhaseEncounters = shinyHunt.TotalEncounters;
            }

            this.dataService.UpdateObject(shinyHunt);

            return this.RedirectToAction("ShinyHunts", "User");
        }

        /// <summary>
        /// Deletes a shiny hunt.
        /// </summary>
        /// <param name="shinyHuntId">The shiny hunt's id.</param>
        /// <returns>The shiny hunt delete page.</returns>
        [HttpGet]
        [Route("delete_shiny_hunt/{shinyHuntId:int}")]
        public IActionResult DeleteShinyHunt(int shinyHuntId)
        {
            ShinyHunt shinyHunt = this.dataService.GetObjectByPropertyValue<ShinyHunt>("Id", shinyHuntId, "Pokemon, Game, HuntingMethod, Pokeball, Mark");
            List<Pokemon> pokemonList = this.dataService.GetObjects<PokemonGameDetail>("Pokemon.PokedexNumber, Pokemon.Id", "Pokemon", "GameId", shinyHunt.GameId).ConvertAll(x => x.Pokemon);
            List<Pokemon> altFormsList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, Form").ConvertAll(x => x.AltFormPokemon);
            foreach (var p in pokemonList.Where(x => altFormsList.Any(y => y.Id == x.Id)))
            {
                p.Name = string.Concat(p.Name, " (", this.dataService.GetObjectByPropertyValue<PokemonFormDetail>("AltFormPokemonId", p.Id, "Form").Form.Name, ")");
            }

            DeleteShinyHuntViewModel model = new DeleteShinyHuntViewModel(shinyHunt)
            {
                AppConfig = this.appConfig,
            };

            return this.View(model);
        }

        /// <summary>
        /// Deletes a shiny hunt.
        /// </summary>
        /// <param name="shinyHunt">The deleteted shiny hunt.</param>
        /// <returns>The user's shiny hunt page.</returns>
        [HttpPost]
        [Route("delete_shiny_hunt/{shinyHuntId:int}")]
        public IActionResult DeleteShinyHunt(DeleteShinyHuntViewModel shinyHunt)
        {
            List<ShinyHunt> phaseHunts = this.dataService.GetObjects<ShinyHunt>(whereProperty: "PhaseOfHuntId", wherePropertyValue: shinyHunt.Id);

            this.dataService.DeleteObject<ShinyHunt>(shinyHunt.Id);

            foreach (var sh in phaseHunts)
            {
                sh.PhaseOfHuntId = null;
                this.dataService.UpdateObject(sh);
            }

            return this.RedirectToAction("ShinyHunts", "User");
        }
    }
}
