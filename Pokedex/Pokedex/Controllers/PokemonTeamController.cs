using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Pokedex.DataAccess.Models;
using Pokedex.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pokedex.Controllers
{
    /// <summary>
    /// The class that is used to represent the controller for a user's pokemon team.
    /// </summary>
    [Authorize]
    [Route("")]
    public class PokemonTeamController : Controller
    {
        private static List<PokemonTeam> pokemonTeams;

        private readonly DataService dataService;

        private readonly AppConfig appConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="PokemonTeamController"/> class.
        /// </summary>
        /// <param name="appConfig">The application's configuration.</param>
        /// <param name="dataContext">The pokemon database context.</param>
        public PokemonTeamController(IOptions<AppConfig> appConfig, DataContext dataContext)
        {
            // Instantiate an instance of the data service.
            this.appConfig = appConfig.Value;
            this.dataService = new DataService(dataContext);
        }

        /// <summary>
        /// Creates a pokemon team.
        /// </summary>
        /// <returns>The create pokemon team page.</returns>
        [HttpGet]
        [Route("create_team")]
        public IActionResult CreateTeam()
        {
            CreatePokemonTeamViewModel model = new CreatePokemonTeamViewModel()
            {
                AllGames = this.dataService.GetGames().Where(x => x.ReleaseDate <= DateTime.Now).ToList(),
                UserId = this.dataService.GetUserWithUsername(this.User.Identity.Name).Id,
            };

            return this.View(model);
        }

        /// <summary>
        /// Creates a pokemon team.
        /// </summary>
        /// <param name="pokemonTeam">The created pokemon team.</param>
        /// <returns>The user's pokemon team page.</returns>
        [HttpPost]
        [Route("create_team")]
        public IActionResult CreateTeam(CreatePokemonTeamViewModel pokemonTeam)
        {
            if (!this.ModelState.IsValid)
            {
                CreatePokemonTeamViewModel model = new CreatePokemonTeamViewModel()
                {
                    AllGames = this.dataService.GetGames().Where(x => x.ReleaseDate <= DateTime.Now).ToList(),
                    UserId = this.dataService.GetUserWithUsername(this.User.Identity.Name).Id,
                };

                return this.View(model);
            }

            this.dataService.AddPokemonTeam(pokemonTeam);

            return this.RedirectToAction("PokemonTeams", "User");
        }

        /// <summary>
        /// Imports pokemon teams from Pokemon Showdown.
        /// </summary>
        /// <returns>The import pokemon teams page.</returns>
        [HttpGet]
        [Route("import_teams")]
        public IActionResult ImportTeams()
        {
            return this.View();
        }

        /// <summary>
        /// Imports pokemon teams from Pokemon Showdown.
        /// </summary>
        /// <param name="importedTeams">The Pokemon Showdown teama export string.</param>
        /// <returns>The user's pokemon team page.</returns>
        [HttpPost]
        [Route("import_teams")]
        public IActionResult ImportTeams(string importedTeams)
        {
            int userId = this.dataService.GetUserWithUsername(this.User.Identity.Name).Id;
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            List<string> pokemonTeams = importedTeams.Split("\r\n===").ToList();
            for (var i = 1; i < pokemonTeams.Count(); i++)
            {
                pokemonTeams[i] = string.Concat("===", pokemonTeams[i]);
            }

            foreach (var p in pokemonTeams)
            {
                this.CreateTeamFromImport(p, userId);
            }

            return this.RedirectToAction("PokemonTeams", "User");
        }

        /// <summary>
        /// Creates a pokemon for a user's pokemon team.
        /// </summary>
        /// <param name="pokemonTeamId">The pokemon team id.</param>
        /// <returns>The create pokemon for team page.</returns>
        [HttpGet]
        [Route("create_pokemon/{pokemonTeamId:int}")]
        public IActionResult CreatePokemon(int pokemonTeamId)
        {
            this.UpdatePokemonTeamList();
            if (pokemonTeams.Count() < pokemonTeamId || pokemonTeams[pokemonTeamId - 1].SixthPokemonId != null)
            {
                return this.RedirectToAction("PokemonTeams", "User");
            }
            else
            {
                PokemonTeam pokemonTeam = pokemonTeams[pokemonTeamId - 1];
                List<Pokemon> pokemonList = this.FillPokemonList(pokemonTeam);
                CreateTeamPokemonViewModel model = new CreateTeamPokemonViewModel()
                {
                    AllPokemon = pokemonList,
                    AllNatures = this.dataService.GetNatures(),
                    NatureId = this.dataService.GetNatureByName("Serious").Id,
                    GameId = pokemonTeam.GameId,
                    Level = 100,
                    Happiness = 255,
                };

                return this.View(model);
            }
        }

        /// <summary>
        /// Creates a pokemon for a user's pokemon team.
        /// </summary>
        /// <param name="pokemonTeamDetail">The created pokemon.</param>
        /// <param name="pokemonTeamId">The pokemon team id.</param>
        /// <returns>The user's pokemon team page.</returns>
        [HttpPost]
        [Route("create_pokemon/{pokemonTeamId:int}")]
        public IActionResult CreatePokemon(CreateTeamPokemonViewModel pokemonTeamDetail, int pokemonTeamId)
        {
            if (!this.ModelState.IsValid)
            {
                CreateTeamPokemonViewModel model = new CreateTeamPokemonViewModel()
                {
                    AllPokemon = this.FillPokemonList(pokemonTeams[pokemonTeamId - 1]),
                    AllNatures = this.dataService.GetNatures(),
                    NatureId = this.dataService.GetNatureByName("Serious").Id,
                    GameId = pokemonTeamDetail.GameId,
                    Level = 100,
                    Happiness = 255,
                };

                return this.View(model);
            }

            PokemonTeam pokemonTeam = pokemonTeams[pokemonTeamId - 1];

            Pokemon pokemon = this.dataService.GetPokemonById(pokemonTeamDetail.PokemonId);

            if (pokemon.GenderRatioId == 10)
            {
                pokemonTeamDetail.Gender = null;
            }

            int pokemonTeamDetailId = this.dataService.AddPokemonTeamDetail(pokemonTeamDetail);

            pokemonTeam.InsertPokemon(this.dataService.GetPokemonTeamDetail(pokemonTeamDetailId));

            this.dataService.UpdatePokemonTeam(pokemonTeam);

            return this.RedirectToAction("PokemonTeams", "User");
        }

        /// <summary>
        /// Edits a pokemon in a user's pokemon team.
        /// </summary>
        /// <param name="pokemonTeamId">The pokemon team id.</param>
        /// <param name="pokemonTeamDetailId">The team's pokemon id.</param>
        /// <returns>The edit pokemon in team page.</returns>
        [HttpGet]
        [Route("update_pokemon/{pokemonTeamId:int}/{pokemonTeamDetailId:int}")]
        public IActionResult EditPokemon(int pokemonTeamId, int pokemonTeamDetailId)
        {
            this.UpdatePokemonTeamList();
            if (pokemonTeams.Count() < pokemonTeamId)
            {
                return this.RedirectToAction("PokemonTeams", "User");
            }
            else
            {
                PokemonTeam pokemonTeam = pokemonTeams[pokemonTeamId - 1];
                PokemonTeamDetail pokemonTeamDetail = this.dataService.GetPokemonTeamDetail(pokemonTeam.GrabPokemonTeamDetailIds()[pokemonTeamDetailId - 1]);

                if (pokemonTeamDetail.Nature == null)
                {
                    pokemonTeamDetail.NatureId = this.dataService.GetNatureByName("Serious").Id;
                }

                List<Pokemon> pokemonList = this.FillPokemonList(pokemonTeam);
                UpdateTeamPokemonViewModel model = new UpdateTeamPokemonViewModel()
                {
                    PokemonTeamDetail = pokemonTeamDetail,
                    AllPokemon = pokemonList,
                    AllNatures = this.dataService.GetNatures(),
                    AllAbilities = this.dataService.GetAbilities(),
                    AllBattleItems = this.dataService.GetBattleItems().OrderBy(x => x.Name).ToList(),
                    GameId = pokemonTeam.GameId,
                };

                return this.View(model);
            }
        }

        /// <summary>
        /// Edits a pokemon in a user's pokemon team.
        /// </summary>
        /// <param name="pokemonTeamDetail">The team's updated pokemon.</param>
        /// <returns>The user's pokemon team page.</returns>
        [HttpPost]
        [Route("update_pokemon/{pokemonTeamId:int}/{pokemonTeamDetailId:int}")]
        public IActionResult EditPokemon(PokemonTeamDetail pokemonTeamDetail)
        {
            PokemonTeam pokemonTeam = this.dataService.GetPokemonTeamFromPokemon(pokemonTeamDetail.Id);
            if (!this.ModelState.IsValid)
            {
                List<Pokemon> pokemonList = this.FillPokemonList(pokemonTeam);

                UpdateTeamPokemonViewModel model = new UpdateTeamPokemonViewModel()
                {
                    PokemonTeamDetail = pokemonTeamDetail,
                    AllPokemon = pokemonList,
                    AllNatures = this.dataService.GetNatures(),
                    AllAbilities = this.dataService.GetAbilities(),
                    AllBattleItems = this.dataService.GetBattleItems().OrderBy(x => x.Name).ToList(),
                    GameId = pokemonTeam.GameId,
                };

                return this.View(model);
            }

            Pokemon pokemon = this.dataService.GetPokemonById(pokemonTeamDetail.PokemonId);

            if (pokemon.GenderRatioId == 10)
            {
                pokemonTeamDetail.Gender = null;
            }

            this.dataService.UpdatePokemonTeamDetail(pokemonTeamDetail);

            return this.RedirectToAction("PokemonTeams", "User");
        }

        /// <summary>
        /// Deletes the selected pokemon team.
        /// </summary>
        /// <param name="pokemonTeamId">The pokemon team id.</param>
        /// <returns>The delete pokemon team confirmation page.</returns>
        [HttpGet]
        [Route("delete_team/{pokemonTeamId:int}")]
        public IActionResult DeleteTeam(int pokemonTeamId)
        {
            this.UpdatePokemonTeamList();
            if (pokemonTeams.Count() < pokemonTeamId)
            {
                return this.RedirectToAction("PokemonTeams", "User");
            }
            else
            {
                PokemonTeam pokemonTeam = pokemonTeams[pokemonTeamId - 1];
                PokemonTeamViewModel model = new PokemonTeamViewModel()
                {
                    Id = pokemonTeam.Id,
                    PokemonTeamName = pokemonTeam.PokemonTeamName,
                    FirstPokemon = pokemonTeam.FirstPokemon,
                    SecondPokemon = pokemonTeam.SecondPokemon,
                    ThirdPokemon = pokemonTeam.ThirdPokemon,
                    FourthPokemon = pokemonTeam.FourthPokemon,
                    FifthPokemon = pokemonTeam.FifthPokemon,
                    SixthPokemon = pokemonTeam.SixthPokemon,
                    AppConfig = this.appConfig,
                };

                return this.View(model);
            }
        }

        /// <summary>
        /// Deletes the selected pokemon team.
        /// </summary>
        /// <param name="pokemonTeam">The pokemon team being deleted.</param>
        /// <returns>The user's pokemon teams page.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_team/{pokemonTeamId:int}")]
        public IActionResult DeleteTeam(PokemonTeam pokemonTeam)
        {
            this.dataService.DeletePokemonTeam(pokemonTeam.Id);

            return this.RedirectToAction("PokemonTeams", "User");
        }

        /// <summary>
        /// Edits the EVs of a pokemon.
        /// </summary>
        /// <param name="pokemonTeamId">The pokemon team's id.</param>
        /// <param name="pokemonTeamDetailId">The pokemon's id.</param>
        /// <returns>The update pokemon's EVs page.</returns>
        [HttpGet]
        [Route("update_ev/{pokemonTeamId:int}/{pokemonTeamDetailId:int}")]
        public IActionResult EditEV(int pokemonTeamId, int pokemonTeamDetailId)
        {
            this.UpdatePokemonTeamList();
            if (pokemonTeams.Count() < pokemonTeamId)
            {
                return this.RedirectToAction("PokemonTeams", "User");
            }
            else
            {
                PokemonTeamEV pokemonEV = this.dataService.GetPokemonTeamEV((int)pokemonTeams[pokemonTeamId - 1].GrabPokemonTeamDetails[pokemonTeamDetailId - 1].PokemonTeamEVId);
                PokemonTeamEVViewModel model = new PokemonTeamEVViewModel()
                {
                    Id = pokemonEV.Id,
                    Health = pokemonEV.Health,
                    Attack = pokemonEV.Attack,
                    Defense = pokemonEV.Defense,
                    SpecialAttack = pokemonEV.SpecialAttack,
                    SpecialDefense = pokemonEV.SpecialDefense,
                    Speed = pokemonEV.Speed,
                    PokemonId = pokemonEV.Id,
                };

                return this.View(model);
            }
        }

        /// <summary>
        /// Edits the EVs of a pokemon.
        /// </summary>
        /// <param name="pokemonTeamEV">The pokemon's EVs.</param>
        /// <returns>The user's pokemon team page.</returns>
        [HttpPost]
        [Route("update_ev/{pokemonTeamId:int}/{pokemonTeamDetailId:int}")]
        public IActionResult EditEV(PokemonTeamEVViewModel pokemonTeamEV)
        {
            if (!this.ModelState.IsValid)
            {
                PokemonTeamEV pokemon = this.dataService.GetPokemonTeamEV(pokemonTeamEV.PokemonId);
                PokemonTeamEVViewModel model = new PokemonTeamEVViewModel()
                {
                    Id = pokemon.Id,
                    Health = pokemon.Health,
                    Attack = pokemon.Attack,
                    Defense = pokemon.Defense,
                    SpecialAttack = pokemon.SpecialAttack,
                    SpecialDefense = pokemon.SpecialDefense,
                    Speed = pokemon.Speed,
                    PokemonId = pokemonTeamEV.PokemonId,
                };

                return this.View(model);
            }
            else if (pokemonTeamEV.EVTotal > 510)
            {
                PokemonTeamEV pokemon = this.dataService.GetPokemonTeamEV(pokemonTeamEV.PokemonId);
                PokemonTeamEVViewModel model = new PokemonTeamEVViewModel()
                {
                    Id = pokemon.Id,
                    Health = pokemon.Health,
                    Attack = pokemon.Attack,
                    Defense = pokemon.Defense,
                    SpecialAttack = pokemon.SpecialAttack,
                    SpecialDefense = pokemon.SpecialDefense,
                    Speed = pokemon.Speed,
                    PokemonId = pokemonTeamEV.PokemonId,
                };

                this.ModelState.AddModelError("EVTotal", "Total EVs max at 510.");
                return this.View(model);
            }

            this.dataService.UpdatePokemonTeamEV(pokemonTeamEV);

            return this.RedirectToAction("PokemonTeams", "User");
        }

        /// <summary>
        /// Edits the moveset of a pokemon.
        /// </summary>
        /// <param name="pokemonTeamId">The pokemon team's id.</param>
        /// <param name="pokemonTeamDetailId">The pokemon's id.</param>
        /// <returns>The update pokemon's moveset page.</returns>
        [HttpGet]
        [Route("update_moveset/{pokemonTeamId:int}/{pokemonTeamDetailId:int}")]
        public IActionResult EditMoveset(int pokemonTeamId, int pokemonTeamDetailId)
        {
            this.UpdatePokemonTeamList();
            if (pokemonTeams.Count() < pokemonTeamId)
            {
                return this.RedirectToAction("PokemonTeams", "User");
            }
            else
            {
                PokemonTeamMoveset pokemonMoveset = this.dataService.GetPokemonTeamMoveset((int)pokemonTeams[pokemonTeamId - 1].GrabPokemonTeamDetails[pokemonTeamDetailId - 1].PokemonTeamMovesetId);
                PokemonTeamMovesetViewModel model = new PokemonTeamMovesetViewModel()
                {
                    Id = pokemonMoveset.Id,
                    FirstMove = pokemonMoveset.FirstMove,
                    SecondMove = pokemonMoveset.SecondMove,
                    ThirdMove = pokemonMoveset.ThirdMove,
                    FourthMove = pokemonMoveset.FourthMove,
                    PokemonId = pokemonMoveset.Id,
                };

                return this.View(model);
            }
        }

        /// <summary>
        /// Edits the moveset of a pokemon.
        /// </summary>
        /// <param name="pokemonTeamMoveset">The pokemon's moveset.</param>
        /// <returns>The user's pokemon team page.</returns>
        [HttpPost]
        [Route("update_moveset/{pokemonTeamId:int}/{pokemonTeamDetailId:int}")]
        public IActionResult EditMoveset(PokemonTeamMovesetViewModel pokemonTeamMoveset)
        {
            if (!this.ModelState.IsValid)
            {
                PokemonTeamMoveset pokemon = this.dataService.GetPokemonTeamMoveset(pokemonTeamMoveset.PokemonId);
                PokemonTeamMovesetViewModel model = new PokemonTeamMovesetViewModel()
                {
                    Id = pokemon.Id,
                    FirstMove = pokemon.FirstMove,
                    SecondMove = pokemon.SecondMove,
                    ThirdMove = pokemon.ThirdMove,
                    FourthMove = pokemon.FourthMove,
                    PokemonId = pokemon.Id,
                };

                return this.View(model);
            }

            this.dataService.UpdatePokemonTeamMoveset(pokemonTeamMoveset);

            return this.RedirectToAction("PokemonTeams", "User");
        }

        /// <summary>
        /// Edits the IVs of a pokemon.
        /// </summary>
        /// <param name="pokemonTeamId">The pokemon team's id.</param>
        /// <param name="pokemonTeamDetailId">The pokemon's id.</param>
        /// <returns>The update pokemon's IVs page.</returns>
        [HttpGet]
        [Route("update_iv/{pokemonTeamId:int}/{pokemonTeamDetailId:int}")]
        public IActionResult EditIV(int pokemonTeamId, int pokemonTeamDetailId)
        {
            this.UpdatePokemonTeamList();
            if (pokemonTeams.Count() < pokemonTeamId)
            {
                return this.RedirectToAction("PokemonTeams", "User");
            }
            else
            {
                PokemonTeamIV pokemonIV = this.dataService.GetPokemonTeamIV((int)pokemonTeams[pokemonTeamId - 1].GrabPokemonTeamDetails[pokemonTeamDetailId - 1].PokemonTeamIVId);
                PokemonTeamIVViewModel model = new PokemonTeamIVViewModel()
                {
                    Id = pokemonIV.Id,
                    Health = pokemonIV.Health,
                    Attack = pokemonIV.Attack,
                    Defense = pokemonIV.Defense,
                    SpecialAttack = pokemonIV.SpecialAttack,
                    SpecialDefense = pokemonIV.SpecialDefense,
                    Speed = pokemonIV.Speed,
                    PokemonId = pokemonIV.Id,
                };

                return this.View(model);
            }
        }

        /// <summary>
        /// Edits the IVs of a pokemon.
        /// </summary>
        /// <param name="pokemonTeamIV">The pokemon's IVs.</param>
        /// <returns>The user's pokemon team page.</returns>
        [HttpPost]
        [Route("update_iv/{pokemonTeamId:int}/{pokemonTeamDetailId:int}")]
        public IActionResult EditIV(PokemonTeamIVViewModel pokemonTeamIV)
        {
            if (!this.ModelState.IsValid)
            {
                PokemonTeamEV pokemon = this.dataService.GetPokemonTeamEV(pokemonTeamIV.PokemonId);
                PokemonTeamIVViewModel model = new PokemonTeamIVViewModel()
                {
                    Id = pokemon.Id,
                    Health = pokemon.Health,
                    Attack = pokemon.Attack,
                    Defense = pokemon.Defense,
                    SpecialAttack = pokemon.SpecialAttack,
                    SpecialDefense = pokemon.SpecialDefense,
                    Speed = pokemon.Speed,
                    PokemonId = pokemonTeamIV.PokemonId,
                };

                return this.View(model);
            }

            this.dataService.UpdatePokemonTeamIV(pokemonTeamIV);

            return this.RedirectToAction("PokemonTeams", "User");
        }

        /// <summary>
        /// Deletes a pokemon from a pokemon team.
        /// </summary>
        /// <param name="pokemonTeamId">The id of the pokemon team being modified.</param>
        /// <param name="pokemonTeamDetailId">The id of the pokemon being deleted.</param>
        /// <returns>The delete pokemon team pokemon page.</returns>
        [HttpGet]
        [Route("delete_team_pokemon/{pokemonTeamId:int}/{pokemonTeamDetailId:int}")]
        public IActionResult DeletePokemon(int pokemonTeamId, int pokemonTeamDetailId)
        {
            this.UpdatePokemonTeamList();
            if (pokemonTeams.Count() < pokemonTeamId)
            {
                return this.RedirectToAction("PokemonTeams", "User");
            }
            else
            {
                PokemonTeam pokemonTeam = pokemonTeams[pokemonTeamId - 1];
                PokemonTeamDetail pokemonTeamDetail = this.dataService.GetPokemonTeamDetail(pokemonTeam.GrabPokemonTeamDetailIds()[pokemonTeamDetailId - 1]);
                PokemonTeamDetailViewModel model = new PokemonTeamDetailViewModel()
                {
                    Id = pokemonTeamDetail.Id,
                    Pokemon = pokemonTeamDetail.Pokemon,
                    AppConfig = this.appConfig,
                };

                return this.View(model);
            }
        }

        /// <summary>
        /// Deletes a pokemon from a pokemon team.
        /// </summary>
        /// <param name="pokemonTeamDetail">The pokemon being deleted.</param>
        /// <returns>The user's pokemon team page.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_team_pokemon/{pokemonTeamId:int}/{pokemonTeamDetailId:int}")]
        public IActionResult DeletePokemon(PokemonTeamDetail pokemonTeamDetail)
        {
            this.dataService.DeletePokemonTeamDetail(pokemonTeamDetail.Id);

            return this.RedirectToAction("PokemonTeams", "User");
        }

        /// <summary>
        /// Edits the pokemon team name or selected game.
        /// </summary>
        /// <param name="pokemonTeamId">The pokemon team id.</param>
        /// <returns>The update pokemon team page.</returns>
        [HttpGet]
        [Route("edit_team/{pokemonTeamId:int}")]
        public IActionResult EditTeam(int pokemonTeamId)
        {
            this.UpdatePokemonTeamList();

            PokemonTeam pokemonTeam = pokemonTeams[pokemonTeamId - 1];

            UpdatePokemonTeamViewModel model = new UpdatePokemonTeamViewModel()
            {
                Id = pokemonTeam.Id,
                PokemonTeamName = pokemonTeam.PokemonTeamName,
                GameId = pokemonTeam.GameId,
                UserId = pokemonTeam.UserId,
                AllGames = this.dataService.GetAvailableGames(pokemonTeam.Id),
            };

            return this.View(model);
        }

        /// <summary>
        /// Edits the pokemon team name or selected game.
        /// </summary>
        /// <param name="newPokemonTeam">The updated pokemon team.</param>
        /// <returns>The user's pokemon team page.</returns>
        [HttpPost]
        [Route("edit_team/{pokemonTeamId:int}")]
        public IActionResult EditTeam(PokemonTeam newPokemonTeam)
        {
            PokemonTeam originalPokemonTeam = this.dataService.GetPokemonTeamNoIncludes(newPokemonTeam.Id);
            if (!this.ModelState.IsValid)
            {
                UpdatePokemonTeamViewModel model = new UpdatePokemonTeamViewModel()
                {
                    Id = originalPokemonTeam.Id,
                    PokemonTeamName = originalPokemonTeam.PokemonTeamName,
                    GameId = originalPokemonTeam.GameId,
                    UserId = originalPokemonTeam.UserId,
                    AllGames = this.dataService.GetAvailableGames(newPokemonTeam.Id),
                };

                return this.View(model);
            }

            if (originalPokemonTeam.PokemonTeamName != newPokemonTeam.PokemonTeamName)
            {
                originalPokemonTeam.PokemonTeamName = newPokemonTeam.PokemonTeamName;
            }

            if (originalPokemonTeam.GameId != newPokemonTeam.GameId)
            {
                originalPokemonTeam.GameId = newPokemonTeam.GameId;
            }

            this.dataService.UpdatePokemonTeam(originalPokemonTeam);

            return this.RedirectToAction("PokemonTeams", "User");
        }

        /// <summary>
        /// Updates the static list with the given user's username.
        /// </summary>
        private void UpdatePokemonTeamList()
        {
            pokemonTeams = this.dataService.GetAllPokemonTeams(this.User.Identity.Name);
        }

        /// <summary>
        /// Creates a pokemon list from a selected pokemon team.
        /// </summary>
        /// <returns>The pokemon list.</returns>
        private List<Pokemon> FillPokemonList(PokemonTeam pokemonTeam)
        {
            List<Pokemon> pokemonList = this.dataService.GetAllPokemon().ToList();
            if (pokemonTeam.GameId != null)
            {
                List<PokemonGameDetail> pokemonGameDetails = this.dataService.GetPokemonGameDetailsByGame((int)pokemonTeam.GameId);
                pokemonList = pokemonList.Where(x => pokemonGameDetails.Any(y => y.PokemonId == x.Id)).ToList();
            }

            List<Pokemon> altForms = this.dataService.GetAllAltForms().Select(x => x.AltFormPokemon).ToList();

            foreach (var p in pokemonList.Where(x => altForms.Any(y => y.Id == x.Id)).ToList())
            {
                p.Name = this.dataService.GetAltFormWithFormName(p.Id).Name;
            }

            pokemonList = pokemonList.OrderBy(x => x.Name).ToList();

            return pokemonList;
        }

        /// <summary>
        /// Creates a pokemon team from a string in Pokemon Showndown's export format.
        /// </summary>
        /// <param name="importedTeam">The pokemon string in Pokemon Showdown's export format.</param>
        /// <param name="userId">The id of the user.</param>
        private void CreateTeamFromImport(string importedTeam, int userId)
        {
            string teamName = importedTeam.Split("===\r\n")[0];
            string pokemonString;
            if (teamName.IndexOf("===") != -1)
            {
                teamName = string.Concat(teamName, "===");
                int teamNameStart = teamName.IndexOf("] ");
                if (teamNameStart == -1)
                {
                    teamNameStart = 4;
                }
                else
                {
                    teamNameStart += 2;
                }

                int teamNameTo = teamName.LastIndexOf(" ===");
                teamName = teamName.Substring(teamNameStart, teamNameTo - teamNameStart);
                pokemonString = string.Concat("\r\n", importedTeam.Split("===\r\n")[1]);
            }
            else
            {
                teamName = "Import From Pokemon Showdown";
                pokemonString = importedTeam;
            }

            PokemonTeam pokemonTeam = new PokemonTeam()
            {
                PokemonTeamName = teamName,
                UserId = userId,
            };
            List<string> pokemonList = pokemonString.Split("\r\n\r\n").ToList();
            List<string> filteredPokemonList = new List<string>();
            foreach (var p in pokemonList)
            {
                if (!string.IsNullOrEmpty(p) && filteredPokemonList.Count() < 6)
                {
                    filteredPokemonList.Add(p);
                }
            }

            // Used to ensure that there are no issues when creating the pokemon detail.
            List<PokemonTeamDetailViewModel> pokemonDetailList = new List<PokemonTeamDetailViewModel>();

            foreach (var p in filteredPokemonList)
            {
                pokemonDetailList.Add(this.CreatePokemonDetailFromImport(p));
            }

            foreach (var p in pokemonDetailList)
            {
                if (p.EVs != null)
                {
                    p.PokemonTeamEVId = this.dataService.AddPokemonTeamEV(p.EVs);
                }

                if (p.IVs != null)
                {
                    p.PokemonTeamIVId = this.dataService.AddPokemonTeamIV(p.IVs);
                }

                if (p.Moveset != null)
                {
                    p.PokemonTeamMovesetId = this.dataService.AddPokemonTeamMoveset(p.Moveset);
                }

                int pokemonId = this.dataService.AddPokemonTeamDetail(p);
                pokemonTeam.InsertPokemon(this.dataService.GetPokemonTeamDetail(pokemonId));
            }

            string generationId = importedTeam.Split("===\r\n")[0];
            this.dataService.FillPokemonTeam(pokemonTeam);
            List<Game> availableGames = this.dataService.GetAvailableGames(pokemonTeam);
            if (generationId.IndexOf("[gen") != -1)
            {
                int generationStart = generationId.IndexOf(" [");
                int generationEnd = generationId.IndexOf("] ");
                generationId = generationId.Substring(generationStart, generationEnd - generationStart);
                int genId = Convert.ToInt32(System.Text.RegularExpressions.Regex.Replace(generationId, @"[^0-9]+", string.Empty));
                if (availableGames.Find(x => x.Id == genId) != null)
                {
                pokemonTeam.GameId = availableGames.Where(x => x.GenerationId == genId).Last().Id;
            }
            else
            {
                    pokemonTeam.GameId = availableGames.Last().Id;
                }
            }
            else
            {
                Game newestGame = availableGames.Last();
                pokemonTeam.GameId = newestGame.Id;
            }

            if (pokemonTeam.FirstPokemon != null)
            {
                pokemonTeam.FirstPokemon = null;
            }

            if (pokemonTeam.SecondPokemon != null)
            {
                pokemonTeam.SecondPokemon = null;
            }

            if (pokemonTeam.ThirdPokemon != null)
            {
                pokemonTeam.ThirdPokemon = null;
            }

            if (pokemonTeam.FourthPokemon != null)
            {
                pokemonTeam.FourthPokemon = null;
            }

            if (pokemonTeam.FifthPokemon != null)
            {
                pokemonTeam.FifthPokemon = null;
            }

            if (pokemonTeam.SixthPokemon != null)
            {
                pokemonTeam.SixthPokemon = null;
            }

            this.dataService.AddPokemonTeam(pokemonTeam);
        }

        /// <summary>
        /// Creates a pokemon for the imported team.
        /// </summary>
        /// <param name="importedPokemon">The pokemon string from the Pokemon Showdown export string.</param>
        /// <returns>The created PokemonTeamDetail object.</returns>
        private PokemonTeamDetailViewModel CreatePokemonDetailFromImport(string importedPokemon)
        {
            PokemonTeamDetailViewModel pokemonTeamDetail = new PokemonTeamDetailViewModel();
            string pokemonName = importedPokemon.Split("\r\n")[0];
            string remainingImportedText = importedPokemon.Replace(string.Concat(pokemonName, "\r\n"), string.Empty);
            pokemonName = pokemonName.Trim();

            // Held item converter.
            if (pokemonName.IndexOf(" @ ") != -1)
            {
                string itemName = pokemonName.Split(" @ ")[1];
                BattleItem battleItem = this.dataService.GetBattleItemByName(itemName);
                if (battleItem != null)
                {
                    pokemonTeamDetail.BattleItemId = battleItem.Id;
                }

                pokemonName = pokemonName.Split(string.Concat(" @ ", itemName))[0];
            }

            // Gender converter.
            int genderCheckStart = pokemonName.LastIndexOf('(');
            if (genderCheckStart != -1 && pokemonName.Substring(genderCheckStart + 2, 1) == ")")
            {
                string genderInitial = pokemonName.Substring(genderCheckStart + 1, 1);
                if (genderInitial == "M")
                {
                    pokemonTeamDetail.Gender = "Male";
                }
                else if (genderInitial == "F")
                {
                    pokemonTeamDetail.Gender = "Female";
                }

                pokemonName = pokemonName.Split(string.Concat(" (", genderInitial, ")"))[0];
            }

            // Nickname converter.
            if (pokemonName.IndexOf("(") != -1)
            {
                pokemonTeamDetail.Nickname = pokemonName.Substring(0, pokemonName.IndexOf("(") - 1);
                pokemonName = pokemonName.Replace(string.Concat(pokemonTeamDetail.Nickname, " ("), string.Empty);
                pokemonName = pokemonName.Replace(")", string.Empty);
            }

            // Pokemon converter.
            Pokemon pokemon;

            // Used to check for alternate form
            if (pokemonName.LastIndexOf('-') != -1)
            {
                if (pokemonName.Contains("Gmax"))
                {
                    pokemonName = pokemonName.Replace("-Gmax", "-Gigantamax");
                }

                if (pokemonName == "Meowstic-F" || pokemonName == "Indeedee-F")
                {
                    pokemonName = pokemonName.Replace("-F", "-Female");
                }

                List<Form> forms = this.dataService.GetForms();
                foreach (var f in forms)
                {
                    f.Name = f.Name.Replace(' ', '-');
                }

                Form form = forms.Find(x => pokemonName.Contains(x.Name));

                if (form != null)
                {
                    pokemon = this.dataService.GetPokemonFromNameAndFormName(pokemonName.Replace(string.Concat("-", form.Name), string.Empty), form.Name.Replace('-', ' '));
                }
                else
                {
                    pokemon = this.dataService.GetPokemon(pokemonName);
                }
            }
            else
            {
                pokemon = this.dataService.GetPokemon(pokemonName);
            }

            pokemonTeamDetail.PokemonId = pokemon.Id;

            // Ability converter.
            Ability ability;
            string abilityName = remainingImportedText.Split("\r\n")[0];
            if (abilityName.Contains("Ability: "))
            {
                remainingImportedText = remainingImportedText.Replace(string.Concat(abilityName, "\r\n"), string.Empty);
                abilityName = abilityName.Split("Ability: ")[1].Trim();
                ability = this.dataService.GetAbilityByName(abilityName);
            }
            else
            {
                ability = this.dataService.GetAbilitiesForPokemon(pokemon.Id).First();
            }

            pokemonTeamDetail.AbilityId = ability.Id;

            // Level converter.
            if (remainingImportedText.Contains("Level:"))
            {
                string pokemonLevel = remainingImportedText.Split("\r\n")[0];
                remainingImportedText = remainingImportedText.Replace(string.Concat(pokemonLevel, "\r\n"), string.Empty);
                pokemonLevel = pokemonLevel.Trim();
                pokemonTeamDetail.Level = Convert.ToByte(pokemonLevel.Substring(pokemonLevel.IndexOf(':') + 2, pokemonLevel.Length - (pokemonLevel.IndexOf(':') + 2)));
                if (pokemonTeamDetail.Level == 0)
                {
                    pokemonTeamDetail.Level = 1;
                }
                else if (pokemonTeamDetail.Level > 100 || string.Compare(pokemonTeamDetail.Level.ToString(), pokemonLevel.Substring(pokemonLevel.IndexOf(':') + 2)) != 0)
                {
                    pokemonTeamDetail.Level = 100;
                }
            }
            else
            {
                pokemonTeamDetail.Level = 100;
            }

            // Shiny converter.
            if (remainingImportedText.Contains("Shiny: Yes"))
            {
                remainingImportedText = remainingImportedText.Replace(string.Concat(remainingImportedText.Split("\r\n")[0], "\r\n"), string.Empty);
                pokemonTeamDetail.IsShiny = true;
            }

            // Happiness converter.
            if (remainingImportedText.Contains("Happiness:"))
            {
                string happiness = remainingImportedText.Split("\r\n")[0];
                remainingImportedText = remainingImportedText.Replace(string.Concat(happiness, "\r\n"), string.Empty);
                happiness = happiness.Trim();
                pokemonTeamDetail.Happiness = Convert.ToByte(happiness.Substring(happiness.IndexOf(':') + 2, happiness.Length - (happiness.IndexOf(':') + 2)));
                if (string.Compare(pokemonTeamDetail.Happiness.ToString(), happiness.Substring(happiness.IndexOf(':') + 2)) != 0)
                {
                    pokemonTeamDetail.Happiness = 255;
                }
            }
            else
            {
                pokemonTeamDetail.Happiness = 255;
            }

            // EV converter.
            if (remainingImportedText.Contains("EVs:"))
            {
                string evs = remainingImportedText.Split("\r\n")[0];
                remainingImportedText = remainingImportedText.Replace(string.Concat(evs, "\r\n"), string.Empty);
                PokemonTeamEV pokemonEVs = new PokemonTeamEV();
                if (evs.Contains("HP"))
                {
                    string health = evs.Substring(evs.IndexOf("HP") - 4, 3);
                    health = health.Replace(":", string.Empty).Replace("/", string.Empty).Trim();
                    pokemonEVs.Health = Convert.ToByte(health);
                }

                if (evs.Contains("Atk"))
                {
                    string attack = evs.Substring(evs.IndexOf("Atk") - 4, 3);
                    attack = attack.Replace(":", string.Empty).Replace("/", string.Empty).Trim();
                    pokemonEVs.Attack = Convert.ToByte(attack);
                }

                if (evs.Contains("Def"))
                {
                    string defense = evs.Substring(evs.IndexOf("Def") - 4, 3);
                    defense = defense.Replace(":", string.Empty).Replace("/", string.Empty).Trim();
                    pokemonEVs.Defense = Convert.ToByte(defense);
                }

                if (evs.Contains("SpA"))
                {
                    string specialAttack = evs.Substring(evs.IndexOf("SpA") - 4, 3);
                    specialAttack = specialAttack.Replace(":", string.Empty).Replace("/", string.Empty).Trim();
                    pokemonEVs.SpecialAttack = Convert.ToByte(specialAttack);
                }

                if (evs.Contains("SpD"))
                {
                    string specialDefense = evs.Substring(evs.IndexOf("SpD") - 4, 3);
                    specialDefense = specialDefense.Replace(":", string.Empty).Replace("/", string.Empty).Trim();
                    pokemonEVs.SpecialDefense = Convert.ToByte(specialDefense);
                }

                if (evs.Contains("Spe"))
                {
                    string speed = evs.Substring(evs.IndexOf("Spe") - 4, 3);
                    speed = speed.Replace(":", string.Empty).Replace("/", string.Empty).Trim();
                    pokemonEVs.Speed = Convert.ToByte(speed);
                }

                pokemonTeamDetail.EVs = pokemonEVs;
            }

            // Nature converter.
            if (remainingImportedText.Contains("Nature"))
            {
                string natureName = remainingImportedText.Split("\r\n")[0];
                remainingImportedText = remainingImportedText.Replace(string.Concat(natureName, "\r\n"), string.Empty);
                natureName = natureName.Replace("Nature", string.Empty).Trim();
                Nature nature = this.dataService.GetNatureByName(natureName);
                pokemonTeamDetail.NatureId = nature.Id;
            }
            else
            {
                pokemonTeamDetail.NatureId = this.dataService.GetNatureByName("Serious").Id;
            }

            // IV converter.
            if (remainingImportedText.Contains("IVs:"))
            {
                string ivs = remainingImportedText.Split("\r\n")[0];
                remainingImportedText = remainingImportedText.Replace(string.Concat(ivs, "\r\n"), string.Empty);
                PokemonTeamIV pokemonIVs = new PokemonTeamIV();
                if (ivs.Contains("HP"))
                {
                    string health = ivs.Substring(ivs.IndexOf("HP") - 3, 2).Trim();
                    pokemonIVs.Health = Convert.ToByte(health);
                }

                if (ivs.Contains("Atk"))
                {
                    string health = ivs.Substring(ivs.IndexOf("Atk") - 3, 2).Trim();
                    pokemonIVs.Attack = Convert.ToByte(health);
                }

                if (ivs.Contains("Def"))
                {
                    string health = ivs.Substring(ivs.IndexOf("Def") - 3, 2).Trim();
                    pokemonIVs.Defense = Convert.ToByte(health);
                }

                if (ivs.Contains("SpA"))
                {
                    string health = ivs.Substring(ivs.IndexOf("SpA") - 3, 2).Trim();
                    pokemonIVs.SpecialAttack = Convert.ToByte(health);
                }

                if (ivs.Contains("SpD"))
                {
                    string health = ivs.Substring(ivs.IndexOf("SpD") - 3, 2).Trim();
                    pokemonIVs.SpecialDefense = Convert.ToByte(health);
                }

                if (ivs.Contains("Spe"))
                {
                    string health = ivs.Substring(ivs.IndexOf("Spe") - 3, 2).Trim();
                    pokemonIVs.Speed = Convert.ToByte(health);
                }

                pokemonTeamDetail.IVs = pokemonIVs;
            }

            // Moveset converter.
            if (remainingImportedText.Contains("- "))
            {
                List<string> moves = remainingImportedText.Split("\r\n").ToList();
                List<string> validMoves = new List<string>();
                string move;
                PokemonTeamMoveset moveset = new PokemonTeamMoveset();

                foreach (var m in moves)
                {
                    if (!string.IsNullOrEmpty(m) && validMoves.Count() < 4)
                    {
                        validMoves.Add(m);
                    }
                }

                foreach (var m in validMoves)
                {
                    move = m.Substring(2, m.Length - 2).Trim();
                    moveset.FourthMove = move;
                    moveset = this.dataService.SortMoveset(moveset);
                }

                pokemonTeamDetail.Moveset = moveset;
            }

            return pokemonTeamDetail;
        }
    }
}
