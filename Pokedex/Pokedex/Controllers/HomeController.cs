using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MoreLinq;
using Pokedex.DataAccess.Models;
using Pokedex.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;

namespace Pokedex.Controllers
{
    /// <summary>
    /// The class that handles the main website pages.
    /// </summary>
    [Authorize]
    [Route("")]
    public class HomeController : Controller
    {
        private static int selectedPokemonId = 0;

        private static int selectedGenerationId = 0;

        private readonly DataService dataService;

        private readonly AppConfig appConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="appConfig">The application configuration.</param>
        /// <param name="dataContext">The database context.</param>
        public HomeController(IOptions<AppConfig> appConfig, DataContext dataContext)
        {
            this.appConfig = appConfig.Value;
            this.dataService = new DataService(dataContext);
        }

        /// <summary>
        /// The home page of the website.
        /// </summary>
        /// <returns>The view for the home page.</returns>
        [AllowAnonymous]
        [Route("")]
        public IActionResult Index()
        {
            return this.View(this.appConfig);
        }

        /// <summary>
        /// Searchs for the pokemon that contains the searched text.
        /// </summary>
        /// <param name="searchText">The text being searched.</param>
        /// <returns>Returns the all pokemon page if no pokemon fits the search, or continues to do a more in-depth search.</returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("search")]
        public IActionResult Search(string searchText)
        {
            searchText = HttpUtility.UrlDecode(searchText);
            searchText = string.IsNullOrEmpty(searchText) ? string.Empty : searchText.Replace("/", string.Empty).Replace("\\", string.Empty);
            if (string.IsNullOrEmpty(searchText))
            {
                return this.RedirectToAction("AllPokemon", "Home");
            }
            else
            {
                return this.RedirectToAction("SearchRedirect", "Home", new { search = searchText });
            }
        }

        /// <summary>
        /// Searchs for the pokemon that contains the searched text.
        /// </summary>
        /// <param name="search">The text being searched.</param>
        /// <returns>Returns the pokemon's page if only one pokemon fits the search, or shows all pokemon with the given text.</returns>
        [AllowAnonymous]
        [Route("search/{search}")]
        public IActionResult SearchRedirect(string search)
        {
            this.dataService.AddPageView("Search Page", this.User.IsInRole("Owner"));
            search = HttpUtility.UrlDecode(search);

            if (!string.IsNullOrEmpty(search))
            {
                this.ViewData["Search"] = search;
                bool pokedexNumber = false;
                search = this.FormatPokemonName(search);

                List<PokemonTypeDetail> typeDetails = this.dataService.GetObjects<PokemonTypeDetail>(includes: "Pokemon, Pokemon.Game, PrimaryType, SecondaryType")
                                                                 .Where(x => x.Pokemon.Name.Contains(search, StringComparison.OrdinalIgnoreCase))
                                                                 .OrderByDescending(x => x.GenerationId)
                                                                 .ToList();

                if (typeDetails.Count() == 0)
                {
                    pokedexNumber = true;
                    typeDetails = this.dataService.GetObjects<PokemonTypeDetail>(includes: "Pokemon, Pokemon.Game, PrimaryType, SecondaryType")
                        .Where(x => x.Pokemon.PokedexNumber.ToString().Contains(search, StringComparison.OrdinalIgnoreCase))
                        .OrderByDescending(x => x.GenerationId)
                        .ToList();
                }

                List<PokemonTypeDetail> model = new List<PokemonTypeDetail>();
                foreach (var t in typeDetails)
                {
                    if (model.Find(x => x.PokemonId == t.PokemonId) == null)
                    {
                        model.Add(t);
                    }
                }

                model = model.OrderBy(x => x.Pokemon.PokedexNumber).ThenBy(x => x.PokemonId).ToList();

                List<PokemonFormDetail> altForms = new List<PokemonFormDetail>();
                if (pokedexNumber)
                {
                    altForms = this.dataService.GetObjects<PokemonFormDetail>("AltFormPokemon.PokedexNumber, AltFormPokemonId", "OriginalPokemon, AltFormPokemon, Form").Where(x => x.OriginalPokemon.PokedexNumber.ToString().Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
                }
                else
                {
                    altForms = this.dataService.GetObjects<PokemonFormDetail>("AltFormPokemon.PokedexNumber, AltFormPokemonId", "OriginalPokemon, AltFormPokemon, Form").Where(x => x.OriginalPokemon.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                if (model.Count == 1)
                {
                    return this.RedirectToAction("Pokemon", "Home", new { Name = model[0].Pokemon.Name.Replace(": ", "_").Replace(' ', '_').ToLower() });
                }
                else if (model.Count == 0)
                {
                    return this.RedirectToAction("AllPokemon", "Home");
                }
                else
                {
                    AllPokemonTypeViewModel viewModel = new AllPokemonTypeViewModel()
                    {
                        AllPokemon = model,
                        AllAltForms = altForms,
                        AppConfig = this.appConfig,
                    };

                    return this.View("Search", viewModel);
                }
            }

            return this.RedirectToAction("AllPokemon", "Home");
        }

        /// <summary>
        /// The page that lists off all of the completed pokemon.
        /// </summary>
        /// <returns>Returns the all pokemon's page.</returns>
        [AllowAnonymous]
        [Route("pokemon")]
        public IActionResult AllPokemon()
        {
            this.dataService.AddPageView("All Pokemon Page", this.User.IsInRole("Owner"));
            List<Pokemon> pokemonList = this.dataService.GetAllPokemon();
            List<Pokemon> altFormList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon").ConvertAll(x => x.AltFormPokemon);
            pokemonList = pokemonList.Where(x => !altFormList.Any(y => y.Id == x.Id)).ToList();

            List<Game> model = this.dataService.GetObjects<Game>("ReleaseDate, Id");

            return this.View(model);
        }

        /// <summary>
        /// The page that calculates how long.
        /// </summary>
        /// <returns>Returns the all pokemon's page.</returns>
        [AllowAnonymous]
        [Route("exp_leveling")]
        public IActionResult ExpLeveling()
        {
            this.dataService.AddPageView("Exp Leveling Page", this.User.IsInRole("Owner"));
            List<Pokemon> pokemonList = this.dataService.GetNonBattlePokemonWithFormNames();
            ExpLevelingViewModel model = new ExpLevelingViewModel()
            {
                PokemonList = pokemonList,
                AppConfig = this.appConfig,
            };

            return this.View(model);
        }

        /// <summary>
        /// The page that allows users to generate randomized teams.
        /// </summary>
        /// <returns>Returns the team randomizer's page.</returns>
        [AllowAnonymous]
        [Route("team_randomizer")]
        public IActionResult TeamRandomizer()
        {
            this.dataService.AddPageView("Team Randomizer Page", this.User.IsInRole("Owner"));
            List<Pokemon> allPokemon = this.dataService.GetAllPokemon();
            List<Generation> generations = this.dataService.GetObjects<Generation>();
            List<DataAccess.Models.Type> types = this.dataService.GetObjects<DataAccess.Models.Type>("Name");
            List<Game> selectableGames = this.dataService.GetGamesGroupedByReleaseDate().Where(x => x.Name != "Colosseum" && x.Name != "XD: Gale of Darkness" && x.Name != "Pokémon GO").ToList();

            foreach (var gen in generations)
            {
                List<Pokemon> pokemonInGen = allPokemon.Where(x => x.Game.GenerationId == gen.Id).ToList();
                if (pokemonInGen.Count == 0)
                {
                    selectableGames.RemoveAll(x => x.GenerationId == gen.Id);
                }
            }

            TeamRandomizerListViewModel model = new TeamRandomizerListViewModel()
            {
                AllGames = selectableGames,
                AllTypes = types,
                AllLegendaryTypes = this.dataService.GetObjects<LegendaryType>("Type"),
                AllFormGroups = this.dataService.GetObjects<FormGroup>("Name", whereProperty: "AppearInTeamRandomizer", wherePropertyValue: true),
                AllFormGroupGameDetails = this.dataService.GetObjects<FormGroupGameDetail>("FormGroup.Name", "FormGroup", "AppearInTeamRandomizer", true),
            };

            return this.View(model);
        }

        /// <summary>
        /// The page that is used to evaluate the different possible pokemon type combinations. This will also showcase any completed pokemon with the selected type combination.
        /// </summary>
        /// <returns>Returns the typing evaluator page.</returns>
        [AllowAnonymous]
        [Route("typing_lookup")]
        public IActionResult TypingEvaluator()
        {
            this.dataService.AddPageView("Typing Lookup Page", this.User.IsInRole("Owner"));
            List<Generation> generations = this.dataService.GetObjects<Generation>().ToList();
            List<Game> gamesList = this.dataService.GetObjects<Game>("ReleaseDate, Id").ToList();
            List<Game> selectableGames = new List<Game>();

            foreach (var gen in generations)
            {
                List<Game> allGames = gamesList.Where(x => x.GenerationId == gen.Id && x.Id != 43).ToList();
                List<Game> uniqueGames = allGames.OrderBy(x => x.ReleaseDate).ThenBy(x => x.Id).DistinctBy(y => y.ReleaseDate).ToList();
                for (var i = 0; i < uniqueGames.Count; i++)
                {
                    if (i == uniqueGames.Count - 1)
                    {
                        selectableGames.Add(new Game()
                        {
                            Id = uniqueGames[i].Id,
                            Name = string.Join(" / ", allGames.Where(x => x.ReleaseDate >= uniqueGames[i].ReleaseDate).Select(x => x.Name)),
                            GenerationId = gen.Id,
                            ReleaseDate = uniqueGames[i].ReleaseDate,
                        });
                    }
                    else
                    {
                        List<Game> games = allGames.Where(x => x.ReleaseDate >= uniqueGames[i].ReleaseDate && x.ReleaseDate < uniqueGames[i + 1].ReleaseDate && !selectableGames.Any(y => y.ReleaseDate == x.ReleaseDate)).ToList();
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
                                GenerationId = gen.Id,
                                ReleaseDate = uniqueGames[i].ReleaseDate,
                            });
                        }
                    }
                }
            }

            selectableGames = selectableGames.OrderBy(x => x.ReleaseDate).ThenBy(x => x.Id).ToList();

            TypeEvaluatorViewModel model = new TypeEvaluatorViewModel()
            {
                AllTypes = this.dataService.GetObjects<DataAccess.Models.Type>("Name"),
                AllGames = selectableGames,
            };

            return this.View(model);
        }

        /// <summary>
        /// The page that is used to evaluate the different abilities, providing a description of the selected ability and showcasing any completed pokemon with said ability.
        /// </summary>
        /// <returns>Returns the ability evaluator page.</returns>
        [AllowAnonymous]
        [Route("ability_lookup")]
        public IActionResult AbilityEvaluator()
        {
            this.dataService.AddPageView("Ability Evalutator Page", this.User.IsInRole("Owner"));
            List<Generation> generations = this.dataService.GetObjects<Generation>();
            AbilityEvaluatorViewModel model = new AbilityEvaluatorViewModel()
            {
                AllAbilities = this.dataService.GetObjects<Ability>("Name"),
                AllGenerations = new List<Generation>(),
            };

            // Removes the first and second generation from the list, as abilities were introduced in the third generation.
            model.AllGenerations = generations.Where(x => x.Id != 1 && x.Id != 2).ToList();

            return this.View(model);
        }

        /// <summary>
        /// The page that is used to evaluate the pokemon available for use in day cares.
        /// </summary>
        /// <returns>Returns the day care evaluator.</returns>
        [AllowAnonymous]
        [Route("day_care_combinations")]
        public IActionResult DayCareEvaluator()
        {
            this.dataService.AddPageView("Day Care Combinations Page", this.User.IsInRole("Owner"));
            List<Generation> generations = this.dataService.GetObjects<Generation>();
            List<Game> gamesList = this.dataService.GetObjects<Game>("ReleaseDate, Id").Where(x => x.IsBreedingPossible).ToList();
            List<Game> selectableGames = new List<Game>();

            foreach (var gen in generations)
            {
                List<Game> uniqueGames = gamesList.Where(x => x.GenerationId == gen.Id).OrderBy(x => x.ReleaseDate).ThenBy(x => x.Id).DistinctBy(y => y.ReleaseDate).ToList();
                List<Game> allGames = gamesList.Where(x => x.GenerationId == gen.Id).ToList();
                for (var i = 0; i < uniqueGames.Count; i++)
                {
                    if (i == uniqueGames.Count - 1)
                    {
                        selectableGames.Add(new Game()
                        {
                            Id = uniqueGames[i].Id,
                            Name = string.Join(" / ", allGames.Where(x => x.ReleaseDate >= uniqueGames[i].ReleaseDate).Select(x => x.Name)),
                            GenerationId = gen.Id,
                        });
                    }
                    else
                    {
                        List<Game> games = allGames.Where(x => x.ReleaseDate >= uniqueGames[i].ReleaseDate && x.ReleaseDate < uniqueGames[i + 1].ReleaseDate && !selectableGames.Any(y => y.ReleaseDate == x.ReleaseDate)).ToList();
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
                                GenerationId = gen.Id,
                            });
                        }
                    }
                }
            }

            EggGroupEvaluatorViewModel model = new EggGroupEvaluatorViewModel()
            {
                AllGames = selectableGames,
                AppConfig = this.appConfig,
                GenerationId = this.dataService.GetObjects<Generation>().Last().Id,
            };

            return this.View(model);
        }

        /// <summary>
        /// The page that is used to evaluate the alternate forms of pokemon.
        /// </summary>
        /// <returns>Returns the form evaluator.</returns>
        [AllowAnonymous]
        [Route("form_viewer")]
        public IActionResult FormEvaluator()
        {
            this.dataService.AddPageView("Form Viewer Page", this.User.IsInRole("Owner"));
            List<Form> model = this.dataService.GetObjects<Form>();
            List<FormGroup> formGroupList = this.dataService.GetObjects<FormGroup>();

            model.Remove(model.FirstOrDefault(x => x.Name == "Female"));

            foreach (var fg in formGroupList)
            {
                List<Form> formsFromGroupList = model.Where(x => x.FormGroupId == fg.Id).ToList();
                model.Add(new Form() { Id = formsFromGroupList.First().Id, Name = fg.Name, FormGroupId = fg.Id });
                foreach (var f in formsFromGroupList)
                {
                    model.Remove(f);
                }
            }

            model = model.OrderBy(x => x.Name).ToList();

            return this.View(model);
        }

        /// <summary>
        /// The page that is used to showcase what pokemon are available for use in a generation. This is not showing what pokemon are catchable in a game.
        /// </summary>
        /// <returns>Returns the game availability page.</returns>
        [AllowAnonymous]
        [Route("game_availability")]
        public IActionResult GameAvailability()
        {
            this.dataService.AddPageView("Game Availability Page", this.User.IsInRole("Owner"));
            GameAvailabilityViewModel model = new GameAvailabilityViewModel()
            {
                EdittedGames = this.GetGamesForEachReleaseDate(),
                UnedittedGames = this.dataService.GetObjects<Game>("ReleaseDate, Id"),
            };

            return this.View(model);
        }

        /// <summary>
        /// The page that is used to showcase the ev yield of opponent pokemon.
        /// </summary>
        /// <returns>Returns the ev trainer page.</returns>
        [AllowAnonymous]
        [Route("ev_trainer")]
        public IActionResult EVTrainer()
        {
            this.dataService.AddPageView("EV Trainer Page", this.User.IsInRole("Owner"));
            List<Generation> generations = this.dataService.GetObjects<Generation>();
            List<Game> gamesList = this.dataService.GetObjects<Game>("ReleaseDate, Id").Where(x => x.Name != "Colosseum" && x.Name != "XD: Gale of Darkness" && x.Name != "Pokémon GO" && x.GenerationId != 1 && x.GenerationId != 2).ToList();
            List<Game> selectableGames = new List<Game>();
            foreach (var gen in generations)
            {
                List<Game> uniqueGames = gamesList.Where(x => x.GenerationId == gen.Id).OrderBy(x => x.ReleaseDate).ThenBy(x => x.Id).DistinctBy(y => y.ReleaseDate).ToList();
                List<Game> allGames = gamesList.Where(x => x.GenerationId == gen.Id).ToList();
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
                            Name = string.Join(" / ", allGames.Where(x => x.ReleaseDate >= uniqueGames[i].ReleaseDate).Select(x => x.Name)),
                            GenerationId = gen.Id,
                        });
                    }
                    else
                    {
                        List<Game> games = allGames.Where(x => x.ReleaseDate >= uniqueGames[i].ReleaseDate && x.ReleaseDate < uniqueGames[i + 1].ReleaseDate && !selectableGames.Any(y => y.ReleaseDate == x.ReleaseDate)).ToList();
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
                                GenerationId = gen.Id,
                            });
                        }
                    }
                }
            }

            return this.View(selectableGames);
        }

        /// <summary>
        /// The method that is used to specify a generation and alternate form for the pokemon page.
        /// </summary>
        /// <param name="pokemonId">The ID of the pokemon.</param>
        /// <param name="generationId">The ID of the generation.</param>
        /// <returns>Returns the pokemon page's method.</returns>
        [AllowAnonymous]
        [Route("pokemon/{pokemonId:int}/{generationId:int}")]
        public IActionResult PokemonWithOnlyId(int pokemonId, int generationId)
        {
            Pokemon pokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", pokemonId);

            return this.RedirectToAction("PokemonWithId", "Home", new { pokemonName = pokemon.Name.ToLower(), pokemonId, generationId });
        }

        /// <summary>
        /// The method that is used to specify a generation and alternate form for the pokemon page.
        /// </summary>
        /// <param name="pokemonName">The name of the pokemon.</param>
        /// <param name="pokemonId">The ID of the pokemon.</param>
        /// <param name="generationId">The ID of the generation.</param>
        /// <returns>Returns the pokemon page's method.</returns>
        [AllowAnonymous]
        [Route("pokemon/{pokemonName}/{pokemonId:int}/{generationId:int}")]
        public IActionResult PokemonWithId(string pokemonName, int pokemonId, int generationId)
        {
            selectedPokemonId = pokemonId;
            if (generationId > this.dataService.GetObjects<PokemonGameDetail>(includes: "Game", whereProperty: "PokemonId", wherePropertyValue: pokemonId).Select(x => x.Game).Last().GenerationId || generationId < this.dataService.GetObjects<PokemonGameDetail>(includes: "Game", whereProperty: "PokemonId", wherePropertyValue: pokemonId).Select(x => x.Game).First().GenerationId)
            {
                selectedGenerationId = 0;
            }
            else
            {
                selectedGenerationId = generationId;
            }

            return this.RedirectToAction("Pokemon", "Home", new { name = pokemonName });
        }

        /// <summary>
        /// The page that is used to showcase all of the information pertaining to a particular pokemon. This includes any alternate forms relating to the selected pokemon.
        /// </summary>
        /// <param name="name">The name of the pokemon.</param>
        /// <returns>Returns the pokemon's page.</returns>
        [AllowAnonymous]
        [Route("pokemon/{Name}")]
        public IActionResult Pokemon(string name)
        {
            int pokemonId = selectedPokemonId;
            int generationId = selectedGenerationId;
            selectedPokemonId = 0;
            selectedGenerationId = 0;
            name = this.FormatPokemonName(name);

            try
            {
                Pokemon pokemon = this.dataService.GetAllPokemon().Find(x => x.Name == name);

                if (pokemon == null)
                {
                    return this.RedirectToAction("AllPokemon", "Home");
                }
                else
                {
                    string pokemonName = pokemon.Name;

                    if (pokemonId == 0)
                    {
                        pokemonId = pokemon.Id;
                    }

                    if (generationId == 0)
                    {
                        generationId = this.dataService.GetObjects<PokemonGameDetail>("Game.GenerationId, GameId, Id", "Pokemon, Pokemon.Game, Game", "PokemonId", pokemonId).Last().Game.GenerationId;
                    }

                    List<PokemonViewModel> pokemonList = new List<PokemonViewModel>();
                    PokemonViewModel pokemonDetails = this.dataService.GetPokemonDetails(pokemon, this.appConfig);
                    pokemonDetails.SurroundingPokemon = this.GetSurroundingPokemon(pokemon.Id);

                    pokemonList.Add(pokemonDetails);

                    List<Pokemon> altForms = this.dataService.GetAltForms(pokemonId);
                    if (altForms.Count > 0)
                    {
                        if (this.dataService.CheckIfAltForm(pokemonId))
                        {
                            pokemonName = this.dataService.GetAltFormWithFormName(pokemonId).Name;
                        }

                        Form form;
                        List<PokemonFormDetail> formDetails = this.dataService.GetObjects<PokemonFormDetail>(includes: "Form");
                        foreach (var p in altForms)
                        {
                            form = formDetails.Find(x => x.AltFormPokemonId == p.Id).Form;
                            pokemonDetails = this.dataService.GetPokemonDetails(p, this.appConfig, form);

                            pokemonList.Add(pokemonDetails);
                        }
                    }

                    List<int> pokemonIds = pokemonList.ConvertAll(x => x.Pokemon.Id);

                    if (pokemonIds.IndexOf(pokemonId) == -1)
                    {
                        pokemonId = pokemon.Id;
                    }

                    AdminPokemonDropdownViewModel model = new AdminPokemonDropdownViewModel()
                    {
                        PokemonList = pokemonList,
                        PokemonId = pokemonId,
                        GenerationId = generationId,
                        LatestGenerationId = this.dataService.GetObjects<Generation>(orderedProperty: "Id").Last().Id,
                        AllTypes = new List<DataAccess.Models.Type>(),
                    };

                    if (this.User.IsInRole("Owner"))
                    {
                        AllAdminPokemonViewModel allAdminPokemon = this.dataService.GetAllAdminPokemonDetails();
                        DropdownViewModel dropdownViewModel = new DropdownViewModel()
                        {
                            AllPokemon = allAdminPokemon,
                            AppConfig = this.appConfig,
                        };
                        AdminGenerationTableViewModel adminDropdown = new AdminGenerationTableViewModel()
                        {
                            PokemonList = new List<Pokemon>(),
                            DropdownViewModel = dropdownViewModel,
                            AppConfig = this.appConfig,
                        };

                        foreach (var p in pokemonList)
                        {
                            adminDropdown.PokemonList.Add(p.Pokemon);
                        }

                        model.AdminDropdown = adminDropdown;
                    }

                    this.dataService.AddPageView(string.Concat("Pokemon Page - ", pokemonName), this.User.IsInRole("Owner"));
                    this.dataService.AddPageView(string.Concat("Pokemon Page"), this.User.IsInRole("Owner"));
                    if (name == "Arceus")
                    {
                        model.AllTypes.Add(new Pokedex.DataAccess.Models.Type { Id = 0, Name = "No Plate" });
                    }
                    else if (name == "Silvally")
                    {
                        model.AllTypes.Add(new Pokedex.DataAccess.Models.Type { Id = 0, Name = "No Memory" });
                    }
                    else
                    {
                        model.AllTypes.Add(new Pokedex.DataAccess.Models.Type { Id = 0, Name = "Not Terastallized" });
                    }

                    if (generationId == 9)
                    {
                        model.AllTypes.Add(new Pokedex.DataAccess.Models.Type { Id = 99, Name = "Stellar" });
                    }

                    model.AllTypes.AddRange(this.dataService.GetObjects<DataAccess.Models.Type>("Name"));

                    return this.View(model);
                }
            }
            catch (Exception e)
            {
                if (!this.User.IsInRole("Owner"))
                {
                    string commentBody;
                    if (e != null)
                    {
                        commentBody = string.Concat(e.GetType().ToString(), " error while setting up pokemon page.");
                    }
                    else
                    {
                        commentBody = "Unknown error while setting up pokemon page.";
                    }

                    commentBody = string.Concat(commentBody, " - Selected Pokemon: ", name);
                    commentBody = string.Concat(commentBody, " - Selected Pokemon Id: ", pokemonId);
                    commentBody = string.Concat(commentBody, " - Selected Generation Id: ", generationId);

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

        /// <summary>
        /// The page that is used to showcase the different type charts throughout the generations.
        /// </summary>
        /// <returns>The type chart page.</returns>
        [AllowAnonymous]
        [Route("type_chart")]
        public IActionResult TypeChart()
        {
            this.dataService.AddPageView("Type Chart Page", this.User.IsInRole("Owner"));
            TypeChartViewModel model = new TypeChartViewModel()
            {
                TypeChart = this.dataService.GetObjects<TypeChart>("AttackId, DefendId", "Attack, Defend"),
                AllTypes = this.dataService.GetObjects<DataAccess.Models.Type>("Name"),
                AllGenerations = this.dataService.GetObjects<Generation>("Id"),
                AppConfig = this.appConfig,
            };

            return this.View(model);
        }

        /// <summary>
        /// The page that is used to showcase the different type charts throughout the generations.
        /// </summary>
        /// <returns>The type chart page.</returns>
        [AllowAnonymous]
        [Route("pokemon_difference")]
        public IActionResult PokemonDifference()
        {
            this.dataService.AddPageView("Pokemon Difference Page", this.User.IsInRole("Owner"));
            PokemonDifferenceViewModel model = new PokemonDifferenceViewModel()
            {
                AllPokemon = this.dataService.GetPokemonWithFormNames(),
                AppConfig = this.appConfig,
            };

            return this.View(model);
        }

        /// <summary>
        /// The page used for users to create comments regarding the website.
        /// </summary>
        /// <returns>Returns the comment page.</returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("comment")]
        public IActionResult Comment()
        {
            this.dataService.AddPageView("Comment Page", this.User.IsInRole("Owner"));
            Comment model = new Comment();
            return this.View(model);
        }

        /// <summary>
        /// Verifies if the comment is valid, then adds it to the database and sends the owner an email.
        /// </summary>
        /// <param name="comment">The comment the user made.</param>
        /// <returns>Returns the main page.</returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("comment")]
        public IActionResult Comment(Comment comment)
        {
            if (!this.ModelState.IsValid)
            {
                Comment model = new Comment();
                return this.View(model);
            }

            if (this.User.Identity.Name != null)
            {
                comment.CommentorId = this.dataService.GetCurrentUser(this.User).Id;
            }

            this.dataService.AddObject(comment);

            this.dataService.EmailComment(this.appConfig, comment);

            return this.RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// The page that is used to provide more information about the website.
        /// </summary>
        /// <returns>Returns the about page.</returns>
        [AllowAnonymous]
        [Route("about")]
        public IActionResult About()
        {
            this.dataService.AddPageView("About Page", this.User.IsInRole("Owner"));

            return this.View();
        }

        /// <summary>
        /// Only viewed if the user gets an unaccounted for error while using the website. An email with the exact error will be sent to the owner's specified email.
        /// </summary>
        /// <returns>Returns the error page.</returns>
        [AllowAnonymous]
        [Route("error")]
        public IActionResult Error()
        {
            this.dataService.AddPageView("Error Page", this.User.IsInRole("Owner"));
            Exception error = this.HttpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>().Error;
            StackTrace stackTrace = new StackTrace(error);
            StackFrame location = stackTrace.GetFrame(0);
            MethodBase sourceMethod = location.GetMethod();

            if (!this.User.IsInRole("Owner") && error != null)
            {
                Comment comment = new Comment()
                {
                    Name = string.Concat(error.GetType().ToString(), " (", error.Message, ")"),
                };

                if (error.InnerException != null)
                {
                    comment.Name = string.Concat(comment.Name, " (", error.InnerException.Message, ")");
                }

                comment.Name = string.Concat(comment.Name, " Method: ", sourceMethod.Name, ", Class: ", sourceMethod.DeclaringType.FullName, ", Location: ", location.GetILOffset());

                if (this.User.Identity.Name != null)
                {
                    comment.CommentorId = this.dataService.GetCurrentUser(this.User).Id;
                }

                this.dataService.AddObject(comment);

                this.dataService.EmailComment(this.appConfig, comment);
            }
            else if (error != null)
            {
                string name = string.Concat(error.GetType().ToString(), " (", error.Message, ")");

                if (error.InnerException != null)
                {
                    name = string.Concat(name, " (", error.InnerException.Message, ")");
                }

                name = string.Concat(name, error.StackTrace.Replace("   ", " "));
                return this.Problem(detail: name, title: error.Message);
            }

            return this.View();
        }

        private List<Pokemon> GetSurroundingPokemon(int pokemonId)
        {
            List<Pokemon> pokemonList = this.dataService.GetAllPokemon().OrderBy(x => x.PokedexNumber).ThenBy(x => x.Id).ToList();
            List<Pokemon> altForms = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon").Select(x => x.AltFormPokemon).OrderBy(x => x.PokedexNumber).ThenBy(x => x.Id).ToList();
            List<Pokemon> originalPokemonList = pokemonList.Where(x => !altForms.Contains(x)).ToList();
            Pokemon pokemon = pokemonList.Find(x => x.Id == pokemonId);
            Pokemon priorPokemon = originalPokemonList.FirstOrDefault(x => x.PokedexNumber == (pokemon.PokedexNumber - 1));
            Pokemon nextPokemon = originalPokemonList.FirstOrDefault(x => x.PokedexNumber == (pokemon.PokedexNumber + 1));

            if (priorPokemon == null)
            {
                if (originalPokemonList.First().PokedexNumber != pokemon.PokedexNumber)
                {
                    priorPokemon = originalPokemonList.Last(x => x.PokedexNumber < pokemon.PokedexNumber);
                }
                else
                {
                    priorPokemon = originalPokemonList.Last();
                }
            }

            if (nextPokemon == null)
            {
                if (originalPokemonList.Last().PokedexNumber != pokemon.PokedexNumber)
                {
                    nextPokemon = originalPokemonList.First(x => x.PokedexNumber > pokemon.PokedexNumber);
                }
                else
                {
                    nextPokemon = originalPokemonList.First();
                }
            }

            List<Pokemon> surroundingPokemon = new List<Pokemon>
            {
                priorPokemon,
                nextPokemon,
            };

            return surroundingPokemon;
        }

        /// <summary>
        /// Formats the name of the pokemon to how the database stores their names.
        /// </summary>
        /// <param name="pokemonName">The name being formatted.</param>
        /// <returns>The formatted pokemon name string.</returns>
        private string FormatPokemonName(string pokemonName)
        {
            pokemonName.ToLower();

            if (pokemonName.Contains("type"))
            {
                pokemonName = "Type: Null";
            }

            if (pokemonName.Contains("_(") && !pokemonName.Contains("nidoran"))
            {
                pokemonName = pokemonName.Split("_(")[0];
            }

            if (pokemonName.Contains('_'))
            {
                pokemonName = pokemonName.Replace('_', ' ');
            }

            if (pokemonName == "flabe" || pokemonName == "flabeb" || pokemonName == "flabebe")
            {
                pokemonName = "Flabébé";
            }

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            pokemonName = textInfo.ToTitleCase(pokemonName);

            if (pokemonName.Length > 1 && pokemonName.Substring(pokemonName.Length - 2, 2) == "-O")
            {
                pokemonName = string.Concat(pokemonName.Remove(pokemonName.Length - 2, 2), "-o");
            }

            return pokemonName;
        }

        /// <summary>
        /// Grabs a list of games, separated by the release dates.
        /// </summary>
        /// <returns>Returns a list of games, based on the release dates.</returns>
        private List<Game> GetGamesForEachReleaseDate()
        {
            List<Game> gameList = this.dataService.GetObjects<Game>("ReleaseDate, Id");
            if (!this.User.IsInRole("Owner"))
            {
                gameList = gameList.Where(x => x.ReleaseDate <= DateTime.UtcNow).ToList();
            }

            List<Game> games = new List<Game>();
            foreach (var r in gameList.ConvertAll(x => x.ReleaseDate).Distinct())
            {
                if (gameList.First(x => x.ReleaseDate == r).Id != 4)
                {
                    games.Add(new Game()
                    {
                        Id = gameList.First(x => x.ReleaseDate == r).Id,
                        Name = string.Join(" / ", gameList.Where(x => x.ReleaseDate == r).Select(x => x.Name)),
                        GenerationId = gameList.First(x => x.ReleaseDate == r).GenerationId,
                        ReleaseDate = r,
                        GameColor = gameList.First(x => x.ReleaseDate == r).GameColor,
                    });
                }
                else
                {
                    foreach (var g in gameList.Where(x => x.ReleaseDate == r).ToList())
                    {
                        games.Add(g);
                    }
                }
            }

            return games;
        }
    }
}
