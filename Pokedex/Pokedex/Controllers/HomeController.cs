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

            List<int> model = pokemonList.Select(x => x.Game.GenerationId).Distinct().OrderBy(x => x).ToList();

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
            List<Game> gamesList = this.dataService.GetObjects<Game>("ReleaseDate, Id");
            gamesList = gamesList.Where(x => x.Name != "Colosseum" && x.Name != "XD: Gale of Darkness").ToList();
            List<Game> selectableGames = new List<Game>();

            foreach (var gen in generations)
            {
                List<Pokemon> pokemonInGen = allPokemon.Where(x => x.Game.GenerationId == gen.Id).ToList();
                if (pokemonInGen.Count != 0)
                {
                    List<Game> uniqueGames = gamesList.Where(x => x.GenerationId == gen.Id).OrderBy(x => x.ReleaseDate).ThenBy(x => x.Id).GroupBy(y => y.ReleaseDate).Select(z => z.First()).ToList();
                    List<Game> allGames = gamesList.Where(x => x.GenerationId == gen.Id).ToList();
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
                AllFormGroups = this.dataService.GetObjects<FormGroup>("Name").ToList(),
            };

            return this.View(model);
        }

        /// <summary>
        /// The page that is used to evaluate the different possible pokemon type combinations. This will also showcase any completed pokemon with the selected type combination.
        /// </summary>
        /// <returns>Returns the typing evaluator page.</returns>
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

        /// <summary>
        /// The page that is used to evaluate the different abilities, providing a description of the selected ability and showcasing any completed pokemon with said ability.
        /// </summary>
        /// <returns>Returns the ability evaluator page.</returns>
        [AllowAnonymous]
        [Route("ability_evaluator")]
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
            foreach (var g in generations.Where(x => x.Id != 1 && x.Id != 2))
            {
                model.AllGenerations.Add(g);
            }

            return this.View(model);
        }

        /// <summary>
        /// The page that is used to evaluate the pokemon available for use in day cares.
        /// </summary>
        /// <returns>Returns the day care evaluator.</returns>
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

        /// <summary>
        /// The page that is used to showcase what pokemon are available for use in a generation. This is not showing what pokemon are catchable in a game.
        /// </summary>
        /// <returns>Returns the game availability page.</returns>
        [AllowAnonymous]
        [Route("game_availability")]
        public IActionResult GameAvailability()
        {
            this.dataService.AddPageView("Game Availability Page", this.User.IsInRole("Owner"));
            List<Game> model = this.GetGamesForEachReleaseDate();

            return this.View(model);
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

            Pokemon pokemon = this.dataService.GetPokemon(name);
            if (pokemon == null)
            {
                return this.RedirectToAction("AllPokemon", "Home");
            }
            else
            {
                string pokemonName = pokemon.Name;
                if (this.dataService.CheckIfAltForm(pokemonId))
                {
                    pokemonName = this.dataService.GetAltFormWithFormName(pokemonId).Name;
                }

                this.dataService.AddPageView(string.Concat("Pokemon Page - ", pokemonName), this.User.IsInRole("Owner"));
                this.dataService.AddPageView(string.Concat("Pokemon Page"), this.User.IsInRole("Owner"));
                if (pokemonId == 0)
                {
                    pokemonId = pokemon.Id;
                }

                if (generationId == 0)
                {
                    generationId = this.dataService.GetPokemonGameDetails(pokemonId).Last().Game.GenerationId;
                }

                List<PokemonViewModel> pokemonList = new List<PokemonViewModel>();
                PokemonViewModel pokemonDetails = this.GetPokemonDetails(pokemon);
                pokemonDetails.SurroundingPokemon = this.GetSurroundingPokemon(pokemonId);

                pokemonList.Add(pokemonDetails);

                List<Pokemon> altForms = this.dataService.GetAltForms(pokemonId);
                if (altForms.Count > 0)
                {
                    Form form;
                    foreach (var p in altForms)
                    {
                        if (p.IsComplete)
                        {
                            form = this.dataService.GetObjectByPropertyValue<PokemonFormDetail>("AltFormPokemonId", p.Id, "Form").Form;
                            pokemonDetails = this.GetPokemonDetails(p, form);

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
                    LatestGenerationId = this.dataService.GetObjects<Generation>(orderedProperty: "Id").Last().Id,
                };

                if (this.User.IsInRole("Admin"))
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
                TypeChart = this.dataService.GetTypeCharts(),
                AllTypes = this.dataService.GetObjects<DataAccess.Models.Type>("Name"),
                AllGenerations = this.dataService.GetObjects<Generation>("Id"),
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
                comment.CommentorId = this.dataService.GetObjectByPropertyValue<User>("Username", this.User.Identity.Name).Id;
            }

            this.dataService.AddObject(comment);

            this.EmailComment(comment);

            return this.RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Only viewed if the user gets an error while using the website. An email with the exact error will be sent to the owner's specified email.
        /// </summary>
        /// <returns>Returns the error page.</returns>
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

        /// <summary>
        /// Gets all of the details for a pokemon.
        /// </summary>
        /// <param name="pokemon">The pokemon needing their details.</param>
        /// <param name="form">The alternate form if it applies.</param>
        /// <returns>Returns the pokemon's details.</returns>
        private PokemonViewModel GetPokemonDetails(Pokemon pokemon, Form form = null)
        {
            PokemonViewModel pokemonViewModel = new PokemonViewModel()
            {
                Pokemon = pokemon,
                BaseHappinesses = this.dataService.GetObjects<PokemonBaseHappinessDetail>(includes: "BaseHappiness", whereProperty: "PokemonId", wherePropertyValue: pokemon.Id).OrderByDescending(x => x.GenerationId).ToList(),
                BaseStats = this.dataService.GetBaseStat(pokemon.Id),
                EVYields = this.dataService.GetEVYields(pokemon.Id),
                Typings = this.dataService.GetObjects<PokemonTypeDetail>(includes: "Pokemon, PrimaryType, SecondaryType, Generation", whereProperty: "PokemonId", wherePropertyValue: pokemon.Id),
                Abilities = this.dataService.GetObjects<PokemonAbilityDetail>(includes: "Pokemon, PrimaryAbility, SecondaryAbility, HiddenAbility, SpecialEventAbility", whereProperty: "PokemonId", wherePropertyValue: pokemon.Id),
                EggGroups = this.dataService.GetObjects<PokemonEggGroupDetail>(includes: "Pokemon, PrimaryEggGroup, SecondaryEggGroup", whereProperty: "PokemonId", wherePropertyValue: pokemon.Id),
                CaptureRates = this.dataService.GetPokemonWithCaptureRates(pokemon.Id),
                PreEvolutions = this.dataService.GetPreEvolution(pokemon.Id).Where(x => x.PreevolutionPokemon.IsComplete).ToList(),
                Evolutions = this.dataService.GetPokemonEvolutions(pokemon.Id).Where(x => x.PreevolutionPokemon.IsComplete && x.EvolutionPokemon.IsComplete).ToList(),
                Effectiveness = this.dataService.GetTypeChartPokemon(pokemon.Id),
                GamesAvailableIn = this.dataService.GetPokemonGameDetails(pokemon.Id).ConvertAll(x => x.Game),
                AppConfig = this.appConfig,
            };

            if (form != null)
            {
                pokemonViewModel.Form = form;
                pokemonViewModel.Pokemon.Name = string.Concat(pokemonViewModel.Pokemon.Name, " (", form.Name, ")");
            }

            PokemonLegendaryDetail legendaryType = this.dataService.GetObjectByPropertyValue<PokemonLegendaryDetail>("PokemonId", pokemon.Id, "LegendaryType");

            if (legendaryType != null)
            {
                pokemonViewModel.LegendaryType = legendaryType.LegendaryType;
            }

            return pokemonViewModel;
        }

        private List<Pokemon> GetSurroundingPokemon(int pokemonId)
        {
            List<Pokemon> surroundingPokemon = new List<Pokemon>();
            List<int> ids = this.dataService.GetAllPokemon().ConvertAll(x => x.Id);
            List<int> altIds = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form").ConvertAll(x => x.AltFormPokemonId);
            List<int> pokemonIds = ids.Where(x => !altIds.Any(y => y == x)).ToList();
            if (altIds.Where(x => x == pokemonId).Count() > 0 && pokemonIds.Where(x => x == pokemonId).Count() == 0)
            {
                pokemonId = this.dataService.GetObjectByPropertyValue<PokemonFormDetail>("AltFormPokemonId", pokemonId).OriginalPokemonId;
            }

            int previousId, nextId, index = pokemonIds.FindIndex(x => x == pokemonId);

            if (pokemonIds[index] == pokemonIds[0])
            {
                previousId = pokemonIds.Last();
            }
            else
            {
                previousId = pokemonIds[index - 1];
            }

            if (pokemonIds[index] == pokemonIds.Last())
            {
                nextId = pokemonIds[0];
            }
            else
            {
                nextId = pokemonIds[index + 1];
            }

            surroundingPokemon.Add(this.dataService.GetObjectByPropertyValue<Pokemon>("Id", previousId));
            surroundingPokemon.Add(this.dataService.GetObjectByPropertyValue<Pokemon>("Id", nextId));

            return surroundingPokemon;
        }

        private void EmailComment(Comment comment)
        {
            try
            {
                if (comment.CommentorId != 1)
                {
                    MailAddress fromAddress = new MailAddress(this.appConfig.EmailAddress, "Pokemon Database Website");
                    MailAddress toAddress = new MailAddress(this.appConfig.EmailAddress, "Pokemon Database Email");
                    string body = "Comment";

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
