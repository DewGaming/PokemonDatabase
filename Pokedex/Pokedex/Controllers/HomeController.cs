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
using System.Web;

namespace Pokedex.Controllers
{
    /// <summary>
    /// The class that is used to represent the HomeController.
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
                search = this.FormatPokemonName(search);

                List<PokemonTypeDetail> model = this.dataService.GetAllPokemonWithTypes()
                                                                 .Where(x => x.Pokemon.Name.Contains(search, StringComparison.OrdinalIgnoreCase))
                                                                 .ToList();

                Pokemon pokemonSearched = this.dataService.GetPokemon(search);

                if (model.Count == 1 || pokemonSearched != null)
                {
                    if (pokemonSearched == null)
                    {
                        pokemonSearched = model[0].Pokemon;
                    }

                    return this.RedirectToAction("Pokemon", "Home", new { Name = pokemonSearched.Name.Replace(": ", "_").Replace(' ', '_').ToLower() });
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
                        AppConfig = this.appConfig,
                    };

                    return this.View("Search", viewModel);
                }
            }

            return this.RedirectToAction("AllPokemon", "Home");
        }

        [AllowAnonymous]
        [Route("pokemon")]
        public IActionResult AllPokemon()
        {
            this.dataService.AddPageView("All Pokemon Page", this.User.IsInRole("Owner"));
            List<int> model = this.dataService.GetGenerationsFromPokemon();

            return this.View(model);
        }

        [AllowAnonymous]
        [Route("team_randomizer")]
        public IActionResult TeamRandomizer()
        {
            this.dataService.AddPageView("Team Randomizer Page", this.User.IsInRole("Owner"));
            List<Pokemon> allPokemon = this.dataService.GetAllPokemon();
            List<Generation> generations = this.dataService.GetObjects<Generation>();
            List<DataAccess.Models.Type> types = this.dataService.GetObjects<DataAccess.Models.Type>("Name");
            List<Game> gamesList = this.dataService.GetObjects<Game>("ReleaseDate, Id");
            gamesList = gamesList.Where(x => x.Name != "Colosseum" && x.Name != "XD: Gale of Darkness").ToList();
            List<Game> selectableGames = new List<Game>();

            foreach (var gen in generations)
            {
                List<Pokemon> pokemonInGen = allPokemon.Where(x => x.Game.GenerationId == gen.Id).ToList();
                if (pokemonInGen.Count != 0)
                {
                    List<Game> uniqueGames = gamesList.Where(x => x.GenerationId == gen.Id && DateTime.Compare(DateTime.Today, x.ReleaseDate) >= 0).OrderBy(x => x.ReleaseDate).ThenBy(x => x.Id).GroupBy(y => y.ReleaseDate).Select(z => z.First()).ToList();
                    List<Game> allGames = gamesList.Where(x => x.GenerationId == gen.Id && DateTime.Compare(DateTime.Today, x.ReleaseDate) >= 0).ToList();
                    for (var i = 0; i < uniqueGames.Count; i++)
                    {
                        if (uniqueGames[i].Abbreviation == "FR")
                        {
                            selectableGames.Add(uniqueGames[i]);
                            selectableGames.Add(this.dataService.GetObjectByPropertyValue<Game>("Abbreviation", "LG"));
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
            }

            TeamRandomizerListViewModel model = new TeamRandomizerListViewModel()
            {
                AllGames = selectableGames,
                AllTypes = types,
                AllLegendaryTypes = this.dataService.GetObjects<LegendaryType>("Type"),
            };

            return this.View(model);
        }

        [AllowAnonymous]
        [Route("typing_evaluator")]
        public IActionResult TypingEvaluator()
        {
            this.dataService.AddPageView("Typing Evaluator Page", this.User.IsInRole("Owner"));
            TypeEvaluatorViewModel model = new TypeEvaluatorViewModel()
            {
                AllTypes = this.dataService.GetObjects<DataAccess.Models.Type>("Name"),
                AllGenerations = this.dataService.GetObjects<Generation>(),
            };

            return this.View(model);
        }

        [AllowAnonymous]
        [Route("ability_evaluator")]
        public IActionResult AbilityEvaluator()
        {
            this.dataService.AddPageView("Ability Evalutator Page", this.User.IsInRole("Owner"));
            AbilityEvaluatorViewModel model = new AbilityEvaluatorViewModel()
            {
                AllAbilities = this.dataService.GetObjects<Ability>("Name"),
                AllGenerations = this.dataService.GetObjects<Generation>(),
            };

            // Removes the first and second generation from the list, as abilities were introduced in the third generation.
            model.AllGenerations.RemoveRange(0, 2);

            return this.View(model);
        }

        [AllowAnonymous]
        [Route("day_care_evaluator")]
        public IActionResult DayCareEvaluator()
        {
            this.dataService.AddPageView("Day Care Evaluator Page", this.User.IsInRole("Owner"));
            List<PokemonEggGroupDetail> eggGroupDetails = this.dataService.GetAllBreedablePokemon();
            EggGroupEvaluatorViewModel model = new EggGroupEvaluatorViewModel()
            {
                AllPokemonWithEggGroups = eggGroupDetails,
                AppConfig = this.appConfig,
                GenerationId = this.dataService.GetObjects<Generation>().Last().Id,
            };

            List<Pokemon> altForms = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form").ConvertAll(x => x.AltFormPokemon);

            foreach (var e in eggGroupDetails.Where(x => altForms.Any(y => y.Id == x.PokemonId)))
            {
                e.Pokemon.Name = this.dataService.GetAltFormWithFormName(e.PokemonId).Name;
            }

            model.AllPokemon = eggGroupDetails.ConvertAll(x => x.Pokemon);

            return this.View(model);
        }

        [AllowAnonymous]
        [Route("game_availability")]
        public IActionResult GameAvailability()
        {
            this.dataService.AddPageView("Game Availability Page", this.User.IsInRole("Owner"));
            List<Game> model = this.GetGamesForEachReleaseDate();

            return this.View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("location_availability")]
        public IActionResult LocationAvailability()
        {
            this.dataService.AddPageView("Location Availability Page", this.User.IsInRole("Owner"));
            List<Game> model = this.dataService.GetObjects<PokemonLocationGameDetail>("Game.ReleaseDate, Game.Id", "Game").Select(x => x.Game).Where(x => x.ReleaseDate <= DateTime.Now).GroupBy(x => x.Id).Select(x => x.First()).ToList();

            return this.View(model);
        }

        [AllowAnonymous]
        [Route("pokemon/{pokemonName}/{pokemonId:int}/{generationId:int}")]
        public IActionResult PokemonWithId(string pokemonName, int pokemonId, int generationId)
        {
            selectedPokemonId = pokemonId;
            if (generationId > this.dataService.GetObjects<PokemonGameDetail>(includes: "Game", whereProperty: "PokemonId", wherePropertyValue: pokemonId).Select(x => x.Game).Last().GenerationId)
            {
                selectedGenerationId = 0;
            }
            else
            {
                selectedGenerationId = generationId;
            }

            return this.RedirectToAction("Pokemon", "Home", new { name = pokemonName });
        }

        [AllowAnonymous]
        [Route("pokemon/{Name}")]
        public IActionResult Pokemon(string name)
        {
            this.dataService.AddPageView("Pokemon Page", this.User.IsInRole("Owner"));
            int pokemonId = selectedPokemonId;
            int generationId = selectedGenerationId;
            selectedPokemonId = 0;
            selectedGenerationId = 0;
            name = this.FormatPokemonName(name);

            Pokemon pokemon = this.dataService.GetPokemon(name);
            if (pokemonId == 0)
            {
                pokemonId = pokemon.Id;
            }

            if (generationId == 0)
            {
                generationId = this.dataService.GetPokemonGameDetails(pokemonId).Last().Game.GenerationId;
            }

            if (pokemon?.IsComplete == true)
            {
                List<PokemonViewModel> pokemonList = new List<PokemonViewModel>();
                PokemonViewModel pokemonDetails = this.dataService.GetPokemonDetails(pokemon, null, this.appConfig);
                pokemonDetails.SurroundingPokemon = this.dataService.GetSurroundingPokemon(pokemon.Id);

                pokemonList.Add(pokemonDetails);

                List<Pokemon> altForms = this.dataService.GetAltForms(pokemon.Id);
                if (altForms.Count > 0)
                {
                    Form form;
                    foreach (var p in altForms)
                    {
                        if (p.IsComplete)
                        {
                            form = this.dataService.GetObjectByPropertyValue<PokemonFormDetail>("AltFormPokemonId", p.Id, "Form").Form;
                            pokemonDetails = this.dataService.GetPokemonDetails(p, form, this.appConfig);

                            pokemonList.Add(pokemonDetails);
                        }
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

                return this.View(model);
            }
            else
            {
                return this.RedirectToAction("AllPokemon", "Home");
            }
        }

        [AllowAnonymous]
        [Route("pokemon_locations/{pokemonName}/{pokemonId:int}/{generationId:int}")]
        public IActionResult PokemonLocationsWithId(string pokemonName, int pokemonId, int generationId)
        {
            selectedPokemonId = pokemonId;
            if (generationId > this.dataService.GetObjects<PokemonLocationGameDetail>("Game.ReleaseDate, GameId", "PokemonLocationDetail, Game", "PokemonLocationDetail.PokemonId", pokemonId).Select(x => x.Game).Last().GenerationId)
            {
                selectedGenerationId = 0;
            }
            else
            {
                selectedGenerationId = generationId;
            }

            return this.RedirectToAction("PokemonLocations", "Home", new { name = pokemonName });
        }

        [AllowAnonymous]
        [Route("pokemon_locations/{name}")]
        public IActionResult PokemonLocations(string name)
        {
            this.dataService.AddPageView("Pokemon Locations Page", this.User.IsInRole("Owner"));
            int pokemonId = selectedPokemonId;
            int generationId = selectedGenerationId;
            selectedPokemonId = 0;
            selectedGenerationId = 0;
            Pokemon pokemon;
            name = this.FormatPokemonName(name);

            if (name.Contains('(') && !name.Contains("Nidoran"))
            {
                string[] names = name.Split(" (");
                pokemon = this.dataService.GetPokemonFromNameAndFormName(names[0], names[1].Replace(")", string.Empty));
            }
            else
            {
                pokemon = this.dataService.GetPokemon(name);
            }

            if (pokemonId == 0)
            {
                pokemonId = pokemon.Id;
            }

            if (generationId == 0 && this.dataService.GetObjects<PokemonLocationGameDetail>("Game.ReleaseDate, GameId", "PokemonLocationDetail, Game", "PokemonLocationDetail.PokemonId", pokemonId).Count() > 0)
            {
                generationId = this.dataService.GetObjects<PokemonLocationGameDetail>("Game.ReleaseDate, GameId", "PokemonLocationDetail, Game", "PokemonLocationDetail.PokemonId", pokemonId).Select(x => x.Game).Last().GenerationId;
            }

            if (pokemon?.IsComplete == true)
            {
                List<PokemonLocationDetail> pokemonLocationDetails = this.dataService.GetObjects<PokemonLocationDetail>().Where(x => x.PokemonId == pokemon.Id).ToList();
                List<Game> gamesAvailableIn = this.dataService.GetObjects<PokemonLocationGameDetail>(includes: "Game").Where(x => pokemonLocationDetails.Any(y => y.Id == x.PokemonLocationDetailId)).Select(x => x.Game).ToList();
                gamesAvailableIn = gamesAvailableIn.GroupBy(x => x.Id).Select(x => x.First()).OrderBy(x => x.ReleaseDate).ThenBy(x => x.Id).ToList();
                List<PokemonLocationGameDetail> pokemonLocation = this.dataService.GetObjects<PokemonLocationGameDetail>(includes: "PokemonLocationDetail, PokemonLocationDetail.Location, PokemonLocationDetail.CaptureMethod").Where(x => pokemonLocationDetails.Any(y => y.Id == x.PokemonLocationDetailId)).ToList();
                PokemonLocationViewModel model = new PokemonLocationViewModel()
                {
                    Pokemon = pokemon,
                    GamesAvailableIn = gamesAvailableIn,
                    PokemonLocations = pokemonLocation,
                    PokemonTimes = this.dataService.GetObjects<PokemonLocationTimeDetail>("TimeId", "PokemonLocationDetail, Time").Where(x => x.PokemonLocationDetail.PokemonId == pokemon.Id).ToList(),
                    PokemonSeasons = this.dataService.GetObjects<PokemonLocationSeasonDetail>("SeasonId", "PokemonLocationDetail, Season").Where(x => x.PokemonLocationDetail.PokemonId == pokemon.Id).ToList(),
                    PokemonId = pokemonId,
                    GenerationId = generationId,
                    AppConfig = this.appConfig,
                };

                return this.View(model);
            }
            else
            {
                return this.RedirectToAction("AllPokemon", "Home");
            }
        }

        [AllowAnonymous]
        [Route("location_evaluator/{locationId:int}/{gameId:int}")]
        public IActionResult PokemonByLocations(int locationId, int gameId)
        {
            this.dataService.AddPageView("Location Evaluator Page", this.User.IsInRole("Owner"));
            List<PokemonLocationDetail> pokemonLocationDetails = this.dataService.GetObjects<PokemonLocationDetail>().Where(x => x.LocationId == locationId).ToList();
            List<PokemonLocationGameDetail> pokemonLocationGames = this.dataService.GetObjects<PokemonLocationGameDetail>("PokemonLocationDetail.Pokemon.PokedexNumber, PokemonLocationDetail.PokemonId", "PokemonLocationDetail, PokemonLocationDetail.Pokemon, PokemonLocationDetail.CaptureMethod, PokemonLocationDetail.Location, PokemonLocationDetail.CaptureMethod", "GameId", gameId).Where(x => pokemonLocationDetails.Any(y => y.Id == x.PokemonLocationDetailId)).ToList();
            LocationEvaluatorViewModel model = new LocationEvaluatorViewModel()
            {
                PokemonLocationGames = pokemonLocationGames,
                PokemonTimes = this.dataService.GetObjects<PokemonLocationTimeDetail>("TimeId", "PokemonLocationDetail, Time").Where(x => pokemonLocationDetails.Any(y => y.Id == x.PokemonLocationDetailId)).ToList(),
                PokemonSeasons = this.dataService.GetObjects<PokemonLocationSeasonDetail>("SeasonId", "PokemonLocationDetail, Season").Where(x => pokemonLocationDetails.Any(y => y.Id == x.PokemonLocationDetailId)).ToList(),
                Location = this.dataService.GetObjectByPropertyValue<Location>("Id", locationId),
                Game = this.dataService.GetObjectByPropertyValue<Game>("Id", gameId),
                AppConfig = this.appConfig,
            };

            return this.View(model);
        }

        [AllowAnonymous]
        [Route("type_chart")]
        public IActionResult TypeChart()
        {
            this.dataService.AddPageView("Type Chart Page", this.User.IsInRole("Owner"));
            TypeChartViewModel model = new TypeChartViewModel()
            {
                TypeChart = this.dataService.GetTypeCharts(),
                AllTypes = this.dataService.GetObjects<DataAccess.Models.Type>("Name"),
                AllGenerations = this.dataService.GetObjects<Generation>("Id"),
            };

            return this.View(model);
        }

        [AllowAnonymous]
        [Route("capture_calculator")]
        public IActionResult CaptureCalculator()
        {
            this.dataService.AddPageView("Capture Calculator Page", this.User.IsInRole("Owner"));
            CaptureCalculatorViewModel model = new CaptureCalculatorViewModel()
            {
                AllPokemon = this.GetAllPokemonForCaptureCalculator(),
                AllPokeballs = this.dataService.GetObjects<Pokeball>("Name"),
                AllStatuses = this.dataService.GetObjects<Status>("Name"),
                AllGenerations = this.dataService.GetObjects<Generation>(),
            };

            model.AllGenerations.Insert(3, new Generation() { Id = 99 });
            model.AllGenerations.Insert(4, new Generation() { Id = 100 });

            return this.View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("comment")]
        public IActionResult Comment()
        {
            this.dataService.AddPageView("Comment Page", this.User.IsInRole("Owner"));
            CommentViewModel model = new CommentViewModel()
            {
                AllCategories = this.dataService.GetObjects<CommentCategory>(),
                AllPages = this.dataService.GetCommentPages(),
            };
            return this.View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("comment")]
        public IActionResult Comment(Comment comment)
        {
            if (!this.ModelState.IsValid)
            {
                CommentViewModel model = new CommentViewModel()
                {
                    AllCategories = this.dataService.GetObjects<CommentCategory>(),
                    AllPages = this.dataService.GetCommentPages(),
                };
                return this.View(model);
            }

            if (!string.IsNullOrEmpty(comment.PokemonName))
            {
                Pokemon pokemon = this.dataService.GetPokemon(this.FormatPokemonName(comment.PokemonName));
                if (pokemon == null)
                {
                    comment.PokemonName = null;
                }
            }

            if (!string.IsNullOrEmpty(comment.PokemonName) && comment.PageId != 3)
            {
                comment.PageId = 3;
            }

            if (this.User.Identity.Name != null)
            {
                comment.CommentorId = this.dataService.GetObjectByPropertyValue<User>("Username", this.User.Identity.Name).Id;
            }

            this.dataService.AddObject(comment);

            comment.Category = this.dataService.GetObjectByPropertyValue<CommentCategory>("Id", comment.CategoryId);
            comment.Page = this.dataService.GetObjectByPropertyValue<CommentPage>("Id", comment.PageId);

            this.EmailComment(comment);

            return this.RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [Route("error")]
        public IActionResult Error()
        {
            this.dataService.AddPageView("Error Page", this.User.IsInRole("Owner"));
            Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature exceptionHandlerFeature = this.HttpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>() !;

            if (!this.User.IsInRole("Owner") && exceptionHandlerFeature != null)
            {
                Comment comment = new Comment()
                {
                    CategoryId = 1,
                    PageId = 13,
                    Name = string.Concat(exceptionHandlerFeature.Error.GetType().ToString(), " (", exceptionHandlerFeature.Error.Message, ")"),
                };

                if (exceptionHandlerFeature.Error.InnerException != null)
                {
                    comment.Name = string.Concat(comment.Name, " (", exceptionHandlerFeature.Error.InnerException.Message, ")");
                }

                comment.Name = string.Concat(comment.Name, exceptionHandlerFeature.Error.StackTrace.Replace("   ", " "));

                if (this.User.Identity.Name != null)
                {
                    comment.CommentorId = this.dataService.GetObjectByPropertyValue<User>("Username", this.User.Identity.Name).Id;
                }

                this.EmailComment(comment);

                this.dataService.AddObject(comment);
            }
            else if (exceptionHandlerFeature != null)
            {
                string name = string.Concat(exceptionHandlerFeature.Error.GetType().ToString(), " (", exceptionHandlerFeature.Error.Message, ")");

                if (exceptionHandlerFeature.Error.InnerException != null)
                {
                    name = string.Concat(name, " (", exceptionHandlerFeature.Error.InnerException.Message, ")");
                }

                name = string.Concat(name, exceptionHandlerFeature.Error.StackTrace.Replace("   ", " "));
                return this.Problem(detail: name, title: exceptionHandlerFeature.Error.Message);
            }

            return this.View();
        }

        private void EmailComment(Comment comment)
        {
            try
            {
                if (comment.CommentorId != 1)
                {
                    MailAddress fromAddress = new MailAddress(this.appConfig.EmailAddress, "Pokemon Database Website");
                    MailAddress toAddress = new MailAddress(this.appConfig.EmailAddress, "Pokemon Database Email");
                    string body = this.dataService.GetObjectByPropertyValue<CommentCategory>("Id", comment.CategoryId).Name;
                    body = string.Concat(body, " for ", this.dataService.GetObjectByPropertyValue<CommentPage>("Id", comment.PageId).Name);

                    if (comment.OtherPage != null)
                    {
                        body = string.Concat(body, " (", comment.OtherPage, ")");
                    }

                    if (comment.PokemonName != null)
                    {
                        body = string.Concat(body, " (", comment.PokemonName, ")");
                    }

                    if (comment.CommentorId != null)
                    {
                        body = string.Concat(body, " by ", this.dataService.GetObjectByPropertyValue<User>("Id", (int)comment.CommentorId).Username);
                    }
                    else
                    {
                        body = string.Concat(body, " by Anonymous User");
                    }

                    body = string.Concat(body, ": ", comment.Name);

                    SmtpClient smtp = new SmtpClient()
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(fromAddress.Address, this.appConfig.EmailAddressPassword),
                    };

                    using MailMessage message = new MailMessage(fromAddress, toAddress)
                    {
                        Subject = "New Comment for Pokémon Database",
                        Body = body,
                    };
                    smtp.Send(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Email could not be sent. ", (ex.InnerException != null) ? ex.InnerException.ToString() : ex.Message);
            }
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
        /// Gets all of the pokemon used for the capture calculator.
        /// </summary>
        /// <returns>The list of capturable pokemon.</returns>
        private List<Pokemon> GetAllPokemonForCaptureCalculator()
        {
            List<Pokemon> pokemonList = this.dataService.GetObjects<Pokemon>("PokedexNumber, Id", "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth, BaseHappiness", "IsComplete", true);

            List<PokemonFormDetail> pokemonForm = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, Form", whereProperty: "AltFormPokemon.IsComplete", wherePropertyValue: true);

            List<PokemonFormDetail> altFormList = new List<PokemonFormDetail>();

            foreach (var p in pokemonForm)
            {
                p.AltFormPokemon.Name = string.Concat(p.AltFormPokemon.Name, " (", p.Form.Name, ")");
                altFormList.Add(p);
            }

            Pokemon pokemon;

            foreach (var a in altFormList)
            {
                pokemon = pokemonList.Find(x => x.Id == a.AltFormPokemonId);
                if (a.Form.Catchable)
                {
                    pokemon.Name = a.AltFormPokemon.Name;
                }
                else
                {
                    pokemonList.Remove(pokemon);
                }
            }

            return pokemonList;
        }

        /// <summary>
        /// Grabs a list of games, separated by the release dates.
        /// </summary>
        /// <returns>Returns a list of games, based on the release dates.</returns>
        private List<Game> GetGamesForEachReleaseDate()
        {
            List<Game> gameList = this.dataService.GetObjects<Game>("ReleaseDate, Id");
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
                        Abbreviation = string.Concat(gameList.Where(x => x.ReleaseDate == r).Select(x => x.Abbreviation)),
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
