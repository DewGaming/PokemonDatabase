using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using MoreLinq;
using Pokedex.DataAccess.Models;
using Pokedex.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

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

        private readonly IHubContext<ShinyHuntHub> hubContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShinyHuntController"/> class.
        /// </summary>
        /// <param name="appConfig">The application's configuration.</param>
        /// <param name="dataContext">The pokemon database context.</param>
        /// <param name="hubContext">The shiny hunting hub.</param>
        public ShinyHuntController(IOptions<AppConfig> appConfig, DataContext dataContext, IHubContext<ShinyHuntHub> hubContext)
        {
            // Instantiate an instance of the data service.
            this.appConfig = appConfig.Value;
            this.dataService = new DataService(dataContext);
            this.hubContext = hubContext;
        }

        /// <summary>
        /// Shows shiny dex progress.
        /// </summary>
        /// <returns>The shiny dex progress page.</returns>
        [Route("shiny_dex_progress")]
        public IActionResult ShinyDexProgress()
        {
            this.dataService.AddPageView("Shiny Dex Progression Page", this.User.IsInRole("Owner"));
            int userId = this.dataService.GetCurrentUser(this.User).Id;
            List<ShinyHunt> shinyHunts = this.dataService.GetObjects<ShinyHunt>("Game.GenerationId, Pokemon.PokedexNumber, PokemonId, Id", "User, Pokemon, Pokemon.Game, Game, HuntingMethod, Mark, Pokeball", "UserId", userId).Where(x => x.IsCaptured && !x.ExcludeFromShinyDex).ToList();
            List<Pokemon> pokemonCaptured = shinyHunts.ConvertAll(x => x.Pokemon).DistinctBy(x => x.Id).ToList();
            List<Pokemon> pokemonList = this.dataService.GetNonBattlePokemonWithFormNames().Where(x => !x.IsShinyLocked).ToList();
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
                CurrentUser = this.dataService.GetCurrentUser(this.User),
                IsShared = false,
                AppConfig = this.appConfig,
            };

            return this.View(model);
        }

        /// <summary>
        /// Shows shiny dex progress.
        /// </summary>
        /// <param name="username">The name of the user's shiny dex progress.</param>
        /// <returns>The shiny dex progress page.</returns>
        [AllowAnonymous]
        [Route("shiny_dex_progress/{username}")]
        public IActionResult SharableShinyDexProgress(string username)
        {
            User user = this.dataService.GetObjectByPropertyValue<User>("Username", username);
            if (user != null)
            {
                this.dataService.AddPageView("Shared Shiny Dex Progression Page", this.User.IsInRole("Owner"));
                List<ShinyHunt> shinyHunts = this.dataService.GetObjects<ShinyHunt>("Game.GenerationId, Pokemon.PokedexNumber, PokemonId, Id", "User, Pokemon, Pokemon.Game, Game, HuntingMethod, Mark, Pokeball", "UserId", user.Id).Where(x => x.IsCaptured).ToList();
                List<Pokemon> pokemonCaptured = shinyHunts.ConvertAll(x => x.Pokemon).DistinctBy(x => x.Id).ToList();
                List<Pokemon> pokemonList = this.dataService.GetNonBattlePokemonWithFormNames().Where(x => !x.IsShinyLocked).ToList();
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
                    CurrentUser = user,
                    IsShared = true,
                    AppConfig = this.appConfig,
                };

                return this.View("ShinyDexProgress", model);
            }
            else
            {
                return this.RedirectToAction("Index", "Home");
            }
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
                UserId = this.dataService.GetCurrentUser(this.User).Id,
                AppConfig = this.appConfig,
            };

            return this.View(model);
        }

        /// <summary>
        /// Starts a shiny hunt.
        /// </summary>
        /// <param name="shinyHunt">The started shiny hunt.</param>
        /// <returns>The user's shiny hunt page.</returns>
        [HttpPost]
        [Route("start_hunt")]
        public IActionResult StartHunt(StartShinyHuntViewModel shinyHunt)
        {
            if (!this.ModelState.IsValid)
            {
                StartShinyHuntViewModel model = new StartShinyHuntViewModel()
                {
                    AllPokemon = this.dataService.GetHuntablePokemon(),
                    UserId = this.dataService.GetCurrentUser(this.User).Id,
                    AppConfig = this.appConfig,
                };

                return this.View(model);
            }

            shinyHunt.IncrementAmount = 1;

            foreach (var p in shinyHunt.PokemonIds.Distinct())
            {
                shinyHunt.Id = 0;
                if (p != 0)
                {
                    shinyHunt.PokemonId = p;
                }

                this.dataService.AddObject(shinyHunt);
            }

            if (shinyHunt.HuntingMethodId == 8)
            {
                return this.RedirectToAction("ShinyFound", "ShinyHunt", new { shinyHuntId = shinyHunt.Id });
            }
            else
            {
                return this.RedirectToAction("ShinyHunts", "User");
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
            List<Pokemon> pokemonList = this.dataService.GetHuntablePokemon();
            pokemonList.RemoveAt(0);
            AddCompletedShinyHuntViewModel model = new AddCompletedShinyHuntViewModel()
            {
                AllPokemon = pokemonList,
                DateOfCapture = DateTime.Now.ToLocalTime().Date,
                IncrementAmount = 1,
                UserId = this.dataService.GetCurrentUser(this.User).Id,
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
                List<Pokemon> pokemonList = this.dataService.GetHuntablePokemon();
                pokemonList.RemoveAt(0);
                AddCompletedShinyHuntViewModel model = new AddCompletedShinyHuntViewModel()
                {
                    AllPokemon = pokemonList,
                    AllMarks = this.dataService.GetObjects<Mark>("Name"),
                    DateOfCapture = DateTime.Now.ToLocalTime().Date,
                    IncrementAmount = 1,
                    UserId = this.dataService.GetCurrentUser(this.User).Id,
                    AppConfig = this.appConfig,
                };

                return this.View(model);
            }

            shinyHunt.IsCaptured = true;

            // If game is Pokemon GO.
            if (shinyHunt.GameId == 43)
            {
                List<Pokemon> pokemonList = this.dataService.GetObjects<Pokemon>(includes: "Game");
                List<Pokemon> updatedPokemonList = pokemonList.Where(x => (x.PokedexNumber <= 150 || x.Name == "Meltan" || x.Name == "Melmetal") && x.Game.GenerationId <= 7).ToList();
                List<Evolution> evolutions = this.dataService.GetObjects<Evolution>("EvolutionPokemon.PokedexNumber, EvolutionPokemonId", "EvolutionPokemon, EvolutionPokemon.Game");
                List<Pokemon> futureEvolutions = evolutions.Where(x => updatedPokemonList.Any(y => y.Id == x.PreevolutionPokemonId)).ToList().ConvertAll(x => x.EvolutionPokemon);
                List<PokemonFormDetail> formDetails = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, Form");
                futureEvolutions.Where(x => formDetails.ConvertAll(x => x.AltFormPokemon).Any(y => y.Id == x.Id)).ToList();
                updatedPokemonList.AddRange(futureEvolutions);
                if (!updatedPokemonList.Exists(x => x.Id == shinyHunt.PokemonId))
                {
                    shinyHunt.DirectHOMETransfer = true;
                }
            }

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
            List<Pokemon> pokemonList = this.dataService.GetHuntablePokemon(shinyHunt.GameId);
            pokemonList.RemoveAt(0);
            Pokemon pokemon;
            List<string> genders = new List<string>();
            if (shinyHunt.PokemonId == null)
            {
                pokemon = pokemonList.First();
                genders = this.dataService.GrabGenders(pokemonList.First().Id, "shinyHunt");
            }
            else
            {
                pokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", shinyHunt.PokemonId);
                genders = this.dataService.GrabGenders(shinyHunt.PokemonId, "shinyHunt");
            }

            shinyHunt.TotalEncounters += shinyHunt.CurrentPhaseEncounters;

            CompleteShinyHuntViewModel model = new CompleteShinyHuntViewModel(shinyHunt)
            {
                Id = shinyHunt.Id,
                PokemonHunted = pokemon,
                GameHuntedIn = this.dataService.GetObjectByPropertyValue<Game>("Id", shinyHunt.GameId),
                DateOfCapture = DateTime.Now.ToLocalTime().Date,
                AllPokemon = pokemonList,
                AllPokeballs = this.dataService.GetPokeballs(shinyHunt.GameId, shinyHunt.HuntingMethodId, this.User, this.appConfig),
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
        public async Task<ActionResult> ShinyFound(CompleteShinyHuntViewModel shinyHunt)
        {
            if (this.ModelState.IsValid)
            {
                shinyHunt.IsCaptured = true;
                shinyHunt.TotalEncounters = shinyHunt.CurrentPhaseEncounters;
                List<ShinyHunt> phases = this.dataService.GetObjects<ShinyHunt>(whereProperty: "PhaseOfHuntId", wherePropertyValue: shinyHunt.Id);
                if (phases.Count() > 0)
                {
                    shinyHunt.TotalEncounters += phases.Select(x => x.CurrentPhaseEncounters).Sum();
                }

                // If game is Pokemon GO.
                if (shinyHunt.GameId == 43)
                {
                    Pokemon pokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", shinyHunt.PokemonId, "Game");
                    if ((pokemon.PokedexNumber > 150 && pokemon.Name != "Meltan" && pokemon.Name != "Melmetal") || (pokemon.PokedexNumber <= 150 && pokemon.Game.GenerationId >= 8))
                    {
                        shinyHunt.DirectHOMETransfer = true;
                    }
                }

                this.dataService.UpdateObject(shinyHunt);
            }

            await this.hubContext.Clients.All.SendAsync("FinishShinyHunt", shinyHunt.Id);

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
            List<Pokemon> pokemonList = this.dataService.GetHuntablePokemon(shinyHunt.GameId);
            pokemonList.RemoveAt(0);
            Pokemon pokemon;
            List<string> genders = new List<string>();
            if (shinyHunt.PokemonId == null)
            {
                pokemon = pokemonList.First();
                genders = this.dataService.GrabGenders(pokemonList.First().Id, "shinyHunt");
            }
            else
            {
                pokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", (int)shinyHunt.PokemonId);
                genders = this.dataService.GrabGenders(shinyHunt.PokemonId, "shinyHunt");
            }

            CompleteShinyHuntViewModel model = new CompleteShinyHuntViewModel(shinyHunt)
            {
                PokemonHunted = pokemon,
                GameHuntedIn = this.dataService.GetObjectByPropertyValue<Game>("Id", shinyHunt.GameId),
                DateOfCapture = DateTime.Now.Date,
                AllPokemon = pokemonList,
                AllPokeballs = this.dataService.GetPokeballs(shinyHunt.GameId, shinyHunt.HuntingMethodId, this.User, this.appConfig),
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
                shinyHunt.TotalEncounters = shinyHunt.CurrentPhaseEncounters;
                shinyHunt.PhaseOfHuntId = shinyHuntId;
                if (!shinyHunt.IsCaptured)
                {
                    shinyHunt.PokeballId = null;
                }

                // If game is Pokemon GO.
                if (shinyHunt.GameId == 43)
                {
                    List<Pokemon> pokemonList = this.dataService.GetObjects<Pokemon>(includes: "Game");
                    List<Pokemon> updatedPokemonList = pokemonList.Where(x => (x.PokedexNumber <= 150 || x.Name == "Meltan" || x.Name == "Melmetal") && x.Game.GenerationId <= 7).ToList();

                    List<Evolution> evolutions = this.dataService.GetObjects<Evolution>("EvolutionPokemon.PokedexNumber, EvolutionPokemonId", "EvolutionPokemon, EvolutionPokemon.Game");
                    List<Pokemon> futureEvolutions = evolutions.Where(x => updatedPokemonList.Any(y => y.Id == x.PreevolutionPokemonId)).ToList().ConvertAll(x => x.EvolutionPokemon);
                    List<PokemonFormDetail> formDetails = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, Form");
                    futureEvolutions.Where(x => formDetails.ConvertAll(x => x.AltFormPokemon).Any(y => y.Id == x.Id)).ToList();
                    updatedPokemonList.AddRange(futureEvolutions);
                    if (!updatedPokemonList.Exists(x => x.Id == shinyHunt.PokemonId))
                    {
                        shinyHunt.DirectHOMETransfer = true;
                    }
                }

                this.dataService.AddObject(shinyHunt);

                ShinyHunt originalShinyHunt = this.dataService.GetObjectByPropertyValue<ShinyHunt>("Id", shinyHuntId);
                originalShinyHunt.TotalEncounters += originalShinyHunt.CurrentPhaseEncounters;
                originalShinyHunt.CurrentPhaseEncounters = 0;
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
            List<Game> gamesList = this.dataService.GetShinyHuntGames(shinyHunt.PokemonId);

            EditShinyHuntViewModel model = new EditShinyHuntViewModel(shinyHunt)
            {
                AllPokemon = this.dataService.GetHuntablePokemon(),
                AllGames = gamesList,
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
                List<Game> gamesList = this.dataService.GetShinyHuntGames(oldShinyHunt.PokemonId);

                EditShinyHuntViewModel model = new EditShinyHuntViewModel(oldShinyHunt)
                {
                    AllPokemon = this.dataService.GetHuntablePokemon(),
                    AllGames = gamesList,
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
            List<Game> gamesList = this.dataService.GetShinyHuntGames(shinyHunt.PokemonId);
            List<string> genders = this.dataService.GrabGenders(shinyHunt.PokemonId, "shinyHunt");

            EditShinyHuntViewModel model = new EditShinyHuntViewModel(shinyHunt)
            {
                PokemonHunted = shinyHunt.Pokemon,
                GameHuntedIn = shinyHunt.Game,
                AllPokemon = this.dataService.GetHuntablePokemon(),
                AllGames = gamesList,
                AllHuntingMethods = this.dataService.GetObjects<HuntingMethodGameDetail>(includes: "HuntingMethod", whereProperty: "GameId", wherePropertyValue: shinyHunt.GameId).ConvertAll(x => x.HuntingMethod),
                AllPokeballs = this.dataService.GetPokeballs(shinyHunt.GameId, shinyHunt.HuntingMethodId, this.User, this.appConfig),
                AllMarks = this.dataService.GetObjects<Mark>(),
                AllSweets = this.dataService.GetObjects<Sweet>(),
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
                List<Game> gamesList = this.dataService.GetShinyHuntGames(oldShinyHunt.PokemonId);
                List<string> genders = this.dataService.GrabGenders(oldShinyHunt.PokemonId, "shinyHunt");

                EditShinyHuntViewModel model = new EditShinyHuntViewModel(oldShinyHunt)
                {
                    PokemonHunted = oldShinyHunt.Pokemon,
                    GameHuntedIn = oldShinyHunt.Game,
                    AllPokemon = this.dataService.GetHuntablePokemon(),
                    AllGames = gamesList,
                    AllHuntingMethods = this.dataService.GetObjects<HuntingMethodGameDetail>(includes: "HuntingMethod", whereProperty: "GameId", wherePropertyValue: oldShinyHunt.GameId).ConvertAll(x => x.HuntingMethod),
                    AllPokeballs = this.dataService.GetPokeballs(oldShinyHunt.GameId, oldShinyHunt.HuntingMethodId, this.User, this.appConfig),
                    AllMarks = this.dataService.GetObjects<Mark>(),
                    AllSweets = this.dataService.GetObjects<Sweet>(),
                    AllGenders = genders,
                    UserId = this.dataService.GetCurrentUser(this.User).Id,
                    AppConfig = this.appConfig,
                };

                return this.View(model);
            }

            if (!shinyHunt.IsCaptured)
            {
                shinyHunt.PokeballId = null;
            }

            shinyHunt.TotalEncounters = shinyHunt.CurrentPhaseEncounters;
            List<ShinyHunt> allHunts = this.dataService.GetObjects<ShinyHunt>();
            List<ShinyHunt> phases = allHunts.Where(x => x.PhaseOfHuntId == shinyHunt.Id).ToList();
            if (phases.Count() > 0)
            {
                shinyHunt.TotalEncounters += phases.Select(x => x.CurrentPhaseEncounters).Sum();
            }

            this.dataService.UpdateObject(shinyHunt);

            if (shinyHunt.PhaseOfHuntId != null)
            {
                ShinyHunt parentHunt = allHunts.Find(x => x.Id == shinyHunt.PhaseOfHuntId);
                parentHunt.TotalEncounters = parentHunt.CurrentPhaseEncounters;
                parentHunt.TotalEncounters += shinyHunt.CurrentPhaseEncounters;
                phases = allHunts.Where(x => x.PhaseOfHuntId == parentHunt.Id && x.Id != shinyHunt.Id).ToList();
                if (phases.Count() > 0)
                {
                    parentHunt.TotalEncounters += phases.Select(x => x.CurrentPhaseEncounters).Sum();
                }

                this.dataService.UpdateObject(parentHunt);
            }

            return this.RedirectToAction("ShinyHunts", "User");
        }

        /// <summary>
        /// Grabs all of the prior phases for a shiny hunt.
        /// </summary>
        /// <param name="shinyHuntId">The shiny hunt.</param>
        /// <returns>The shiny hunt phases page.</returns>
        [HttpGet]
        [Route("shiny_hunt_phases/{shinyHuntId:int}")]
        public IActionResult ShinyHuntPhases(int shinyHuntId)
        {
            List<ShinyHunt> shinyHunts = this.dataService.GetObjects<ShinyHunt>(includes: "Pokemon, Mark, Pokeball, Sweet, Game, HuntingMethod");

            ShinyHuntsViewModel model = new ShinyHuntsViewModel()
            {
                AllShinyHunts = shinyHunts.Where(x => x.PhaseOfHuntId == shinyHuntId).ToList(),
                ShinyHunt = shinyHunts.Find(x => x.Id == shinyHuntId),
                IsShared = true,
                AppConfig = this.appConfig,
            };

            this.dataService.AddPageView("Shiny Hunt Phases Page", this.User.IsInRole("Owner"));
            return this.View("ShinyHuntPhases", model);
        }
    }
}
