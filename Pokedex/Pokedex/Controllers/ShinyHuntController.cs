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

            this.dataService.AddObject(shinyHunt);

            return this.RedirectToAction("ShinyHunts", "User");
        }

        /// <summary>
        /// Starts a shiny hunt.
        /// </summary>
        /// <param name="shinyHuntId">The completed shiny hunt's Id.</param>
        /// <returns>The shiny hunt page.</returns>
        [HttpGet]
        [Route("complete_shiny_hunt/{shinyHuntId:int}")]
        public IActionResult ShinyFound(int shinyHuntId)
        {
            ShinyHunt shinyHunt = this.dataService.GetObjectByPropertyValue<ShinyHunt>("Id", shinyHuntId, "Game");
            Pokemon pokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", shinyHunt.PokemonId);
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

            CompleteShinyHuntViewModel model = new CompleteShinyHuntViewModel()
            {
                Id = shinyHunt.Id,
                PokemonHunted = pokemon,
                GameHuntedIn = this.dataService.GetObjectByPropertyValue<Game>("Id", shinyHunt.GameId),
                UserId = shinyHunt.UserId,
                PokemonId = shinyHunt.PokemonId,
                GameId = shinyHunt.GameId,
                HuntingMethodId = shinyHunt.HuntingMethodId,
                Encounters = shinyHunt.Encounters,
                SparklingPowerLevel = shinyHunt.SparklingPowerLevel,
                HasShinyCharm = shinyHunt.HasShinyCharm,
                DateOfCapture = DateTime.Now.Date,
                AllPokeballs = this.GetPokeballs(shinyHunt.GameId, shinyHunt.HuntingMethodId),
                AllGenders = genders,
                AllMarks = this.dataService.GetObjects<Mark>("Name"),
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

            EditShinyHuntViewModel model = new EditShinyHuntViewModel()
            {
                AllGames = this.GetShinyHuntGames(),
                AllPokemon = pokemonList,
                AllHuntingMethods = this.GetHuntingMethods(shinyHunt.GameId),
                Id = shinyHunt.Id,
                PokemonId = shinyHunt.PokemonId,
                GameId = shinyHunt.GameId,
                HuntingMethodId = shinyHunt.HuntingMethodId,
                SparklingPowerLevel = shinyHunt.SparklingPowerLevel,
                HasShinyCharm = shinyHunt.HasShinyCharm,
                Encounters = shinyHunt.Encounters,
                UserId = this.dataService.GetObjectByPropertyValue<User>("Username", this.User.Identity.Name).Id,
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

                EditShinyHuntViewModel model = new EditShinyHuntViewModel()
                {
                    AllGames = this.GetShinyHuntGames(),
                    AllPokemon = pokemonList,
                    AllHuntingMethods = this.GetHuntingMethods(oldShinyHunt.GameId),
                    Id = oldShinyHunt.Id,
                    PokemonId = oldShinyHunt.PokemonId,
                    GameId = oldShinyHunt.GameId,
                    HuntingMethodId = oldShinyHunt.HuntingMethodId,
                    SparklingPowerLevel = oldShinyHunt.SparklingPowerLevel,
                    HasShinyCharm = oldShinyHunt.HasShinyCharm,
                    Encounters = oldShinyHunt.Encounters,
                    UserId = this.dataService.GetObjectByPropertyValue<User>("Username", this.User.Identity.Name).Id,
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

            EditShinyHuntViewModel model = new EditShinyHuntViewModel()
            {
                AllGames = this.GetShinyHuntGames(),
                AllPokemon = pokemonList,
                AllHuntingMethods = this.GetHuntingMethods(shinyHunt.GameId),
                AllPokeballs = this.GetPokeballs(shinyHunt.GameId, shinyHunt.HuntingMethodId),
                AllMarks = this.dataService.GetObjects<Mark>(),
                AllGenders = genders,
                Id = shinyHunt.Id,
                PokemonId = shinyHunt.PokemonId,
                GameId = shinyHunt.GameId,
                PokemonHunted = shinyHunt.Pokemon,
                GameHuntedIn = shinyHunt.Game,
                Nickname = shinyHunt.Nickname,
                DateOfCapture = shinyHunt.DateOfCapture,
                HuntingMethodId = shinyHunt.HuntingMethodId,
                PokeballId = shinyHunt.PokeballId,
                Gender = shinyHunt.Gender,
                MarkId = shinyHunt.MarkId,
                SparklingPowerLevel = shinyHunt.SparklingPowerLevel,
                HasShinyCharm = shinyHunt.HasShinyCharm,
                Encounters = shinyHunt.Encounters,
                IsCaptured = shinyHunt.IsCaptured,
                UserId = this.dataService.GetObjectByPropertyValue<User>("Username", this.User.Identity.Name).Id,
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

                EditShinyHuntViewModel model = new EditShinyHuntViewModel()
                {
                    AllGames = this.GetShinyHuntGames(),
                    AllPokemon = pokemonList,
                    AllHuntingMethods = this.GetHuntingMethods(oldShinyHunt.GameId),
                    AllPokeballs = this.GetPokeballs(oldShinyHunt.GameId, oldShinyHunt.HuntingMethodId),
                    AllMarks = this.dataService.GetObjects<Mark>(),
                    AllGenders = genders,
                    Id = oldShinyHunt.Id,
                    PokemonId = oldShinyHunt.PokemonId,
                    GameId = oldShinyHunt.GameId,
                    PokemonHunted = oldShinyHunt.Pokemon,
                    GameHuntedIn = oldShinyHunt.Game,
                    Nickname = oldShinyHunt.Nickname,
                    DateOfCapture = oldShinyHunt.DateOfCapture,
                    HuntingMethodId = oldShinyHunt.HuntingMethodId,
                    PokeballId = oldShinyHunt.PokeballId,
                    Gender = oldShinyHunt.Gender,
                    MarkId = oldShinyHunt.MarkId,
                    SparklingPowerLevel = oldShinyHunt.SparklingPowerLevel,
                    HasShinyCharm = oldShinyHunt.HasShinyCharm,
                    Encounters = oldShinyHunt.Encounters,
                    IsCaptured = oldShinyHunt.IsCaptured,
                    UserId = this.dataService.GetObjectByPropertyValue<User>("Username", this.User.Identity.Name).Id,
                    AppConfig = this.appConfig,
                };

                return this.View(model);
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

            DeleteShinyHuntViewModel model = new DeleteShinyHuntViewModel()
            {
                Id = shinyHunt.Id,
                Pokemon = shinyHunt.Pokemon,
                Game = shinyHunt.Game,
                Nickname = shinyHunt.Nickname,
                DateOfCapture = shinyHunt.DateOfCapture,
                HuntingMethod = shinyHunt.HuntingMethod,
                Pokeball = shinyHunt.Pokeball,
                Gender = shinyHunt.Gender,
                Mark = shinyHunt.Mark,
                HasShinyCharm = shinyHunt.HasShinyCharm,
                Encounters = shinyHunt.Encounters,
                IsCaptured = shinyHunt.IsCaptured,
                UserId = this.dataService.GetObjectByPropertyValue<User>("Username", this.User.Identity.Name).Id,
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
            this.dataService.DeleteObject<ShinyHunt>(shinyHunt.Id);

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

                    selectablePokeballs = selectablePokeballs.Where(x => x.GenerationId <= game.GenerationId).ToList();
                    break;
            }

            return selectablePokeballs.OrderBy(x => x.GenerationId).ThenBy(x => x.Name).ToList();
        }

        private List<HuntingMethod> GetHuntingMethods(int gameId)
        {
            List<HuntingMethod> selectableHuntingMethods = this.dataService.GetObjects<HuntingMethod>();
            Game game = this.dataService.GetObjectByPropertyValue<Game>("Id", gameId);

            if (game.Id != 17 && game.Id != 32)
            {
                selectableHuntingMethods.Remove(selectableHuntingMethods.Find(x => x.Id == 9));
            }

            return selectableHuntingMethods.OrderBy(x => x.Name).ToList();
        }
    }
}
