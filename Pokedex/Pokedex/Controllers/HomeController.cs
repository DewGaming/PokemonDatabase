using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Pokedex.DataAccess.Models;

using Pokedex.Models;

namespace Pokedex.Controllers
{
    [Authorize]
    [Route("")]
    public class HomeController : Controller
    {
        private readonly DataService _dataService;

        private readonly AppConfig _appConfig;

        public HomeController(IOptions<AppConfig> appConfig, DataContext dataContext)
        {
            this._appConfig = appConfig.Value;
            this._dataService = new DataService(dataContext);
        }

        [AllowAnonymous]
        [Route("")]
        public IActionResult Index()
        {
            return this.View(this._appConfig);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("search")]
        public IActionResult Search(string search)
        {
            search = HttpUtility.UrlDecode(search);
            search = string.IsNullOrEmpty(search) ? "" : search.Replace("/", "").Replace("\\", "");
            if(string.IsNullOrEmpty(search))
            {
                return this.RedirectToAction("AllPokemon", "Home");
            }
            else
            {
                return this.RedirectToAction("SearchRedirect", "Home", new { search = search } );
            }
        }

        [AllowAnonymous]
        [Route("search/{search}")]
        public IActionResult SearchRedirect(string search)
        {
            search = HttpUtility.UrlDecode(search);

            if (!string.IsNullOrEmpty(search))
            {
                this.ViewData["Search"] = search;
                search = this._dataService.FormatPokemonName(search);

                List<PokemonTypeDetail> model = this._dataService.GetAllPokemonWithTypes()
                                                                 .Where(x => x.Pokemon.Name.ToLower().Contains(search.ToLower()))
                                                                 .ToList();

                Pokemon pokemonSearched = this._dataService.GetPokemon(search);

                if (model.Count() == 1 || pokemonSearched != null)
                {
                    if (pokemonSearched == null)
                    {
                        pokemonSearched = model[0].Pokemon;
                    }
                    return this.RedirectToAction("Pokemon", "Home", new { Name = pokemonSearched.Name.Replace(": ", "_").Replace(' ', '_').ToLower() });
                }
                else if (model.Count() == 0)
                {
                    return this.RedirectToAction("AllPokemon", "Home");
                }
                else
                {
                    AllPokemonTypeViewModel viewModel = new AllPokemonTypeViewModel(){
                        AllPokemon = model,
                        AppConfig = this._appConfig,
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
            List<int> model = this._dataService.GetGenerationsFromPokemon();

            return this.View(model);
        }

        [AllowAnonymous]
        [Route("team_randomizer")]
        public IActionResult TeamRandomizer()
        {
            List<Pokemon> allPokemon = this._dataService.GetAllPokemon();
            List<Generation> generations = this._dataService.GetGenerations();
            List<Game> model = new List<Game>();

            foreach(var gen in generations)
            {
                if(allPokemon.Where(x => x.Game.GenerationId == gen.Id).ToList().Count() != 0)
                {
                    model.AddRange(this._dataService.GetGamesFromGeneration(gen));
                }
            }

            return this.View(model);
        }

        [AllowAnonymous]
        [Route("typing_evaluator")]
        public IActionResult TypingEvaluator()
        {
            List<Pokedex.DataAccess.Models.Type> model = this._dataService.GetTypes();

            return View(model);
        }

        [AllowAnonymous]
        [Route("day_care_evaluator")]
        public IActionResult DayCareEvaluator()
        {
            List<PokemonEggGroupDetail> eggGroupDetails = this._dataService.GetAllBreedablePokemon();
            EggGroupEvaluatorViewModel model = new EggGroupEvaluatorViewModel(){
                AllPokemonWithEggGroups = eggGroupDetails,
                AppConfig = _appConfig,
            };

            List<Pokemon> altForms = this._dataService.GetAllAltForms().Select(x => x.AltFormPokemon).ToList();

            foreach(var e in eggGroupDetails.Where(x => altForms.Any(y => y.Id == x.PokemonId)))
            {
                e.Pokemon.Name = this._dataService.GetAltFormWithFormName(e.PokemonId).Name;
            }

            model.AllPokemon = eggGroupDetails.Select(x => x.Pokemon).ToList();

            return View(model);
        }

        [AllowAnonymous]
        [Route("game_availability")]
        public IActionResult GameAvailability()
        {
            List<Game> model = this._dataService.GetGames();

            return this.View(model);
        }

        [AllowAnonymous]
        [Route("pokemon/{Name}")]
        public IActionResult Pokemon(string name)
        {
            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();
            name = this._dataService.FormatPokemonName(name);

            Pokemon pokemon = this._dataService.GetPokemon(name);
            if(pokemon != null && pokemon.IsComplete)
            {
                List<PokemonViewModel> pokemonList = new List<PokemonViewModel>();
                PokemonViewModel pokemonDetails = this._dataService.GetPokemonDetails(pokemon, null, this._appConfig);
                pokemonDetails.SurroundingPokemon = this._dataService.GetSurroundingPokemon(pokemon.Id);

                pokemonList.Add(pokemonDetails);
                
                List<Pokemon> altForms = this._dataService.GetAltForms(pokemon.Id);
                if(altForms.Count() > 0)
                {
                    Form form;
                    foreach (var p in altForms)
                    {
                        if (p.IsComplete)
                        {
                            form = this._dataService.GetFormByAltFormId(p.Id);
                            pokemonDetails = this._dataService.GetPokemonDetails(p, form, this._appConfig);

                            pokemonList.Add(pokemonDetails);
                        }
                    }
                }

                AdminPokemonDropdownViewModel model = new AdminPokemonDropdownViewModel()
                {
                    PokemonList = pokemonList,
                };

                if(User.IsInRole("Owner"))
                {
                    AllAdminPokemonViewModel allAdminPokemon = this._dataService.GetAllAdminPokemonDetails();
                    DropdownViewModel dropdownViewModel = new DropdownViewModel(){
                        AllPokemon = allAdminPokemon,
                        AppConfig = this._appConfig,
                    };
                    AdminGenerationTableViewModel adminDropdown = new AdminGenerationTableViewModel()
                    {
                        PokemonList = new List<Pokemon>(),
                        DropdownViewModel = dropdownViewModel,
                        AppConfig = _appConfig,
                    };

                    foreach(var p in pokemonList)
                    {
                        adminDropdown.PokemonList.Add(p.Pokemon);
                    }

                    model.AdminDropdown = adminDropdown;
                }

                stopWatch.Stop();
                return this.View(model);
            }
            else
            {
                return this.RedirectToAction("AllPokemon", "Home");
            }
        }

        [AllowAnonymous]
        [Route("type_chart")]
        public IActionResult TypeChart()
        {
            TypeChartViewModel model = new TypeChartViewModel()
            {
                TypeChart = this._dataService.GetTypeCharts(),
                Types = this._dataService.GetTypes(),
            };

            return this.View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("comment")]
        public IActionResult Comment()
        {
            CommentViewModel model = new CommentViewModel()
            {
                TypeOfComment = _appConfig.CommentCategories,
                Page = _appConfig.PageCategories,
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
                    TypeOfComment = _appConfig.CommentCategories,
                    Page = _appConfig.PageCategories,
                };
                return this.View(model);
            }

            if (!string.IsNullOrEmpty(comment.PokemonName))
            {
                Pokemon pokemon = this._dataService.GetPokemon(this._dataService.FormatPokemonName(comment.PokemonName));
                if (pokemon == null)
                {
                    comment.PokemonName = null;
                }
            }
            
            if(!string.IsNullOrEmpty(comment.PokemonName) && comment.CommentedPage != "Pokemon Page")
            {
                comment.CommentedPage = "Pokemon Page";
            }

            if (User.Identity.Name != null)
            {
                comment.CommentorId = this._dataService.GetUserWithUsername(User.Identity.Name).Id;
            }

            this._dataService.AddComment(comment);

            this.EmailComment(comment);

            return this.RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [Route("about")]
        public IActionResult About()
        {
            return this.View();
        }

        [AllowAnonymous]
        [Route("error")]
        public IActionResult Error()
        {
            return this.View();
        }

        private void EmailComment(Comment comment)
        {
            try
            {
                if(comment.CommentorId != 1 || comment.CommentorId == null)
                {
                    MailAddress fromAddress = new MailAddress(_appConfig.EmailAddress, "Pokemon Database Website");
                    MailAddress toAddress = new MailAddress(_appConfig.EmailAddress, "Pokemon Database Email");
                    string body = comment.CommentType;
                    if(comment.CommentedPage != null)
                    {
                        body += " for " + comment.CommentedPage;
                    }

                    if(comment.PokemonName != null)
                    {
                        body += " (" + comment.PokemonName + ")";
                    }

                    if(comment.CommentorId != null)
                    {
                        body += " by " + this._dataService.GetUserById((int)comment.CommentorId).Username;
                    }

                    body += ": " + comment.Name;

                    SmtpClient smtp = new SmtpClient()
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(fromAddress.Address, _appConfig.EmailAddressPassword),
                    };

                    using (var message = new MailMessage(fromAddress, toAddress)
                    {
                        Subject = "New Comment for Pokemon Database",
                        Body = body,
                    })
                    {
                        smtp.Send(message);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Email could not be sent. " + ((ex.InnerException != null) ? ex.InnerException.ToString() : ex.Message));
            }
        }
    }
}
