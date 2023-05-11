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

namespace Pokedex.Controllers
{
    /// <summary>
    /// The class that handles all ShinyHunt related requests.
    /// </summary>
    [Authorize(Roles = "Admin")]
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
            List<ShinyHunt> shinyHunts = this.dataService.GetObjects<ShinyHunt>("Game.GenerationId, Pokemon.PokedexNumber, PokemonId, Id", "User, Pokemon, Pokemon.Game, Game, HuntingMethod, Mark, Pokeball", "User.Username", this.User.Identity.Name).Where(x => x.IsCaptured).ToList();
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
                AllGames = this.GetShinyHuntGames(),
                UserId = this.dataService.GetObjectByPropertyValue<User>("Username", this.User.Identity.Name).Id,
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
                    AllGames = this.GetShinyHuntGames(),
                    UserId = this.dataService.GetObjectByPropertyValue<User>("Username", this.User.Identity.Name).Id,
                    AppConfig = this.appConfig,
                };

                return this.View(model);
            }

            shinyHunt.Phases = 1;

            this.dataService.AddObject(shinyHunt);

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
            List<Pokemon> pokemonList = this.dataService.GetObjects<PokemonGameDetail>("Pokemon.PokedexNumber, Pokemon.Id", "Pokemon", "GameId", shinyHunt.GameId).ConvertAll(x => x.Pokemon);
            List<Pokemon> altFormsList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, Form").ConvertAll(x => x.AltFormPokemon);
            foreach (var p in pokemonList.Where(x => altFormsList.Any(y => y.Id == x.Id)))
            {
                p.Name = string.Concat(p.Name, " (", this.dataService.GetObjectByPropertyValue<PokemonFormDetail>("AltFormPokemonId", p.Id, "Form").Form.Name, ")");
            }

            List<string> genders = new List<string>();
            if (pokemon.GenderRatioId == 1)
            {
                genders.Add("Male");
            }
            else if (pokemon.GenderRatioId == 9)
            {
                genders.Add("Female");
            }
            else if (pokemon.GenderRatioId == 10)
            {
                genders.Add("Gender Unknown");
            }
            else
            {
                genders.Add("Male");
                genders.Add("Female");
            }

            shinyHunt.TotalEncounters += shinyHunt.CurrentPhaseEncounters;

            CompleteShinyHuntViewModel model = new CompleteShinyHuntViewModel(shinyHunt)
            {
                Id = shinyHunt.Id,
                PokemonHunted = pokemon,
                GameHuntedIn = this.dataService.GetObjectByPropertyValue<Game>("Id", shinyHunt.GameId),
                DateOfCapture = DateTime.Now.Date,
                AllPokeballs = this.GetPokeballs(shinyHunt.GameId, shinyHunt.HuntingMethodId),
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
            List<Pokemon> pokemonList = this.dataService.GetObjects<PokemonGameDetail>("Pokemon.PokedexNumber, Pokemon.Id", "Pokemon", "GameId", shinyHunt.GameId).ConvertAll(x => x.Pokemon);
            List<Pokemon> altFormsList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, Form").ConvertAll(x => x.AltFormPokemon);
            foreach (var p in pokemonList.Where(x => altFormsList.Any(y => y.Id == x.Id)))
            {
                p.Name = string.Concat(p.Name, " (", this.dataService.GetObjectByPropertyValue<PokemonFormDetail>("AltFormPokemonId", p.Id, "Form").Form.Name, ")");
            }

            List<string> genders = new List<string>();
            if (pokemon.GenderRatioId == 1)
            {
                genders.Add("Male");
            }
            else if (pokemon.GenderRatioId == 9)
            {
                genders.Add("Female");
            }
            else if (pokemon.GenderRatioId == 10)
            {
                genders.Add("Gender Unknown");
            }
            else
            {
                genders.Add("Male");
                genders.Add("Female");
            }

            CompleteShinyHuntViewModel model = new CompleteShinyHuntViewModel(shinyHunt)
            {
                PokemonHunted = pokemon,
                GameHuntedIn = this.dataService.GetObjectByPropertyValue<Game>("Id", shinyHunt.GameId),
                DateOfCapture = DateTime.Now.Date,
                AllPokemon = pokemonList,
                AllPokeballs = this.GetPokeballs(shinyHunt.GameId, shinyHunt.HuntingMethodId),
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
            List<Pokemon> pokemonList = this.dataService.GetObjects<PokemonGameDetail>("Pokemon.PokedexNumber, Pokemon.Id", "Pokemon", "GameId", shinyHunt.GameId).ConvertAll(x => x.Pokemon);
            List<Pokemon> altFormsList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, Form").ConvertAll(x => x.AltFormPokemon);
            foreach (var p in pokemonList.Where(x => altFormsList.Any(y => y.Id == x.Id)))
            {
                p.Name = string.Concat(p.Name, " (", this.dataService.GetObjectByPropertyValue<PokemonFormDetail>("AltFormPokemonId", p.Id, "Form").Form.Name, ")");
            }

            EditShinyHuntViewModel model = new EditShinyHuntViewModel(shinyHunt)
            {
                AllGames = this.GetShinyHuntGames(),
                AllPokemon = pokemonList,
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
                List<Pokemon> pokemonList = this.dataService.GetObjects<PokemonGameDetail>("Pokemon.PokedexNumber, Pokemon.Id", "Pokemon", "GameId", oldShinyHunt.GameId).ConvertAll(x => x.Pokemon);
                List<Pokemon> altFormsList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, Form").ConvertAll(x => x.AltFormPokemon);
                foreach (var p in pokemonList.Where(x => altFormsList.Any(y => y.Id == x.Id)))
                {
                    p.Name = string.Concat(p.Name, " (", this.dataService.GetObjectByPropertyValue<PokemonFormDetail>("AltFormPokemonId", p.Id, "Form").Form.Name, ")");
                }

                EditShinyHuntViewModel model = new EditShinyHuntViewModel(oldShinyHunt)
                {
                    AllGames = this.GetShinyHuntGames(),
                    AllPokemon = pokemonList,
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
            List<Pokemon> pokemonList = this.dataService.GetObjects<PokemonGameDetail>("Pokemon.PokedexNumber, Pokemon.Id", "Pokemon", "GameId", shinyHunt.GameId).ConvertAll(x => x.Pokemon);
            List<Pokemon> altFormsList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, Form").ConvertAll(x => x.AltFormPokemon);
            foreach (var p in pokemonList.Where(x => altFormsList.Any(y => y.Id == x.Id)))
            {
                p.Name = string.Concat(p.Name, " (", this.dataService.GetObjectByPropertyValue<PokemonFormDetail>("AltFormPokemonId", p.Id, "Form").Form.Name, ")");
            }

            List<string> genders = new List<string>();
            if (shinyHunt.Pokemon.GenderRatioId == 1)
            {
                genders.Add("Male");
            }
            else if (shinyHunt.Pokemon.GenderRatioId == 9)
            {
                genders.Add("Female");
            }
            else if (shinyHunt.Pokemon.GenderRatioId == 10)
            {
                genders.Add("Gender Unknown");
            }
            else
            {
                genders.Add("Male");
                genders.Add("Female");
            }

            EditShinyHuntViewModel model = new EditShinyHuntViewModel(shinyHunt)
            {
                PokemonHunted = shinyHunt.Pokemon,
                GameHuntedIn = shinyHunt.Game,
                AllGames = this.GetShinyHuntGames(),
                AllPokemon = pokemonList,
                AllHuntingMethods = this.dataService.GetObjects<HuntingMethodGameDetail>(includes: "HuntingMethod", whereProperty: "GameId", wherePropertyValue: shinyHunt.GameId).ConvertAll(x => x.HuntingMethod),
                AllPokeballs = this.GetPokeballs(shinyHunt.GameId, shinyHunt.HuntingMethodId),
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
                List<Pokemon> pokemonList = this.dataService.GetObjects<PokemonGameDetail>("Pokemon.PokedexNumber, Pokemon.Id", "Pokemon", "GameId", oldShinyHunt.GameId).ConvertAll(x => x.Pokemon);
                List<Pokemon> altFormsList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, Form").ConvertAll(x => x.AltFormPokemon);
                foreach (var p in pokemonList.Where(x => altFormsList.Any(y => y.Id == x.Id)))
                {
                    p.Name = string.Concat(p.Name, " (", this.dataService.GetObjectByPropertyValue<PokemonFormDetail>("AltFormPokemonId", p.Id, "Form").Form.Name, ")");
                }

                List<string> genders = new List<string>();
                if (oldShinyHunt.Pokemon.GenderRatioId == 1)
                {
                    genders.Add("Male");
                }
                else if (oldShinyHunt.Pokemon.GenderRatioId == 9)
                {
                    genders.Add("Female");
                }
                else if (oldShinyHunt.Pokemon.GenderRatioId == 10)
                {
                    genders.Add("Gender Unknown");
                }
                else
                {
                    genders.Add("Male");
                    genders.Add("Female");
                }

                EditShinyHuntViewModel model = new EditShinyHuntViewModel(oldShinyHunt)
                {
                    PokemonHunted = oldShinyHunt.Pokemon,
                    GameHuntedIn = oldShinyHunt.Game,
                    AllGames = this.GetShinyHuntGames(),
                    AllPokemon = pokemonList,
                    AllHuntingMethods = this.dataService.GetObjects<HuntingMethodGameDetail>(includes: "HuntingMethod", whereProperty: "GameId", wherePropertyValue: oldShinyHunt.GameId).ConvertAll(x => x.HuntingMethod),
                    AllPokeballs = this.GetPokeballs(oldShinyHunt.GameId, oldShinyHunt.HuntingMethodId),
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

        private List<Game> GetShinyHuntGames()
        {
            List<Game> gamesList = this.dataService.GetObjects<Game>("ReleaseDate, Id").Where(x => x.ReleaseDate <= DateTime.Now && x.GenerationId >= 2).ToList();
            List<Game> selectableGames = new List<Game>();
            List<Game> uniqueGames = gamesList.OrderBy(x => x.ReleaseDate).ThenBy(x => x.Id).DistinctBy(y => y.ReleaseDate).ToList();
            for (var i = 0; i < uniqueGames.Count; i++)
            {
                if (uniqueGames[i].Name == "Fire Red")
                {
                    selectableGames.Add(uniqueGames[i]);
                    selectableGames.Add(this.dataService.GetObjectByPropertyValue<Game>("Name", "Leaf Green"));
                }
                else if (i == uniqueGames.Count - 1)
                {
                    selectableGames.Add(new Game()
                    {
                        Id = uniqueGames[i].Id,
                        Name = string.Join(" / ", gamesList.Where(x => x.ReleaseDate >= uniqueGames[i].ReleaseDate).Select(x => x.Name)),
                        GenerationId = uniqueGames[i].GenerationId,
                    });
                }
                else
                {
                    List<Game> games = gamesList.Where(x => x.ReleaseDate >= uniqueGames[i].ReleaseDate && x.ReleaseDate < uniqueGames[i + 1].ReleaseDate && !selectableGames.Any(y => y.ReleaseDate == x.ReleaseDate)).ToList();
                    if (games.Count == 0)
                    {
                        selectableGames.Add(uniqueGames[i]);
                    }
                    else
                    {
                        selectableGames.Add(new Game()
                        {
                            Id = uniqueGames[i].Id,
                            Name = string.Join(" / ", games.ConvertAll(x => x.Name)),
                            GenerationId = uniqueGames[i].GenerationId,
                        });
                    }
                }
            }

            return selectableGames;
        }

        private List<Pokeball> GetPokeballs(int gameId, int huntingMethodId)
        {
            List<Pokeball> selectablePokeballs = this.dataService.GetObjects<Pokeball>();
            Game game = this.dataService.GetObjectByPropertyValue<Game>("Id", gameId);
            HuntingMethod huntingMethod = this.dataService.GetObjectByPropertyValue<HuntingMethod>("Id", huntingMethodId);

            switch (huntingMethod.Name)
            {
                case "Breeding":
                case "Masuda Method":
                    if (game.GenerationId <= 5)
                    {
                        selectablePokeballs = selectablePokeballs.Where(x => x.Id == 1).ToList();
                    }
                    else
                    {
                        selectablePokeballs = selectablePokeballs.Where(x => x.GenerationId <= game.GenerationId && x.Id != 4 && x.Id != 24).ToList();
                    }

                    break;
                case "Event":
                    if (game.GenerationId <= 3)
                    {
                        selectablePokeballs = selectablePokeballs.Where(x => x.Id == 1).ToList();
                    }
                    else
                    {
                        selectablePokeballs = selectablePokeballs.Where(x => x.Id == 24).ToList();
                    }

                    break;
                default:
                    if ((game.GenerationId != 5 && game.GenerationId < 8) || game.Id == 35 || game.Id == 36)
                    {
                        selectablePokeballs.Remove(selectablePokeballs.Find(x => x.Id == 25));
                    }

                    if ((game.GenerationId > 4 && game.GenerationId < 8) || game.GenerationId == 9)
                    {
                        selectablePokeballs.Remove(selectablePokeballs.Find(x => x.Id == 5));
                    }

                    if (game.GenerationId != 2 && game.Id != 9 && game.Id != 26 && game.Id != 17 && game.Id != 32)
                    {
                        selectablePokeballs.Remove(selectablePokeballs.Find(x => x.Id == 13));
                    }

                    if (game.Id == 37)
                    {
                        selectablePokeballs = selectablePokeballs.Where(x => x.Name.Contains("Hisui")).ToList();
                        foreach (var p in selectablePokeballs)
                        {
                            p.Name = p.Name.Replace(" (Hisui)", string.Empty);
                        }
                    }
                    else
                    {
                        selectablePokeballs = selectablePokeballs.Where(x => !x.Name.Contains("Hisui")).ToList();
                    }

                    selectablePokeballs = selectablePokeballs.Where(x => x.GenerationId <= game.GenerationId).ToList();
                    break;
            }

            return selectablePokeballs.OrderBy(x => x.GenerationId).ThenBy(x => x.Name).ToList();
        }
    }
}
