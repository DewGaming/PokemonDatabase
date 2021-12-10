using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Pokedex.DataAccess.Models;
using Pokedex.Models;
using System.Collections.Generic;
using System.Linq;

namespace Pokedex.Controllers
{
    /// <summary>
    /// The class that is used to represent the owner controller.
    /// </summary>
    [Authorize(Roles = "Owner")]
    [Route("admin")]
    public class OwnerController : Controller
    {
        private readonly DataService dataService;

        private readonly AppConfig appConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="OwnerController"/> class.
        /// </summary>
        /// <param name="dataContext">The database context.</param>
        /// <param name="appConfig">The application configuration.</param>
        public OwnerController(DataContext dataContext, IOptions<AppConfig> appConfig)
        {
            // Instantiate an instance of the data service.
            this.dataService = new DataService(dataContext);
            this.appConfig = appConfig.Value;
        }

        /// <summary>
        /// Allows owners to view all users on the site.
        /// </summary>
        /// <returns>The owner user page.</returns>
        [Route("users")]
        public IActionResult Users()
        {
            UserViewModel model = new UserViewModel()
            {
                UserList = this.dataService.GetObjects<User>().OrderByDescending(x => x.LastVisit).ThenBy(x => x.Id).ToList(),
                UsersWithPokemonTeams = new List<User>(),
            };

            List<PokemonTeam> pokemonTeams = this.dataService.GetPokemonTeams();

            foreach (var u in model.UserList)
            {
                if (pokemonTeams.Where(x => x.UserId == u.Id).ToList().Count > 0)
                {
                    model.UsersWithPokemonTeams.Add(u);
                }
            }

            return this.View(model);
        }

        /// <summary>
        /// Opens the page to send a message based off of a comment.
        /// </summary>
        /// <param name="commentId">The id of the comment being replied to.</param>
        /// <returns>The send message page.</returns>
        [HttpGet]
        [Route("send_message/{commentId:int}")]
        public IActionResult SendMessage(int commentId)
        {
            Comment comment = this.dataService.GetObjectByPropertyValue<Comment>("Id", commentId);

            Message model = new Message()
            {
                SenderId = this.dataService.GetObjectByPropertyValue<User>("Username", this.User.Identity.Name).Id,
                ReceiverId = comment.CommentorId,
                MessageTitle = string.Concat("Regaring your comment \"", comment.Name, "\" "),
            };

            return this.View(model);
        }

        /// <summary>
        /// Sends the message to the user.
        /// </summary>
        /// <param name="message">The message being sent.</param>
        /// <param name="commentId">The id of the comment being replied to.</param>
        /// <returns>The owner's comment page.</returns>
        [HttpPost]
        [Route("send_message/{commentId:int}")]
        public IActionResult SendMessage(Message message, int commentId)
        {
            if (!this.ModelState.IsValid)
            {
                Comment comment = this.dataService.GetObjectByPropertyValue<Comment>("Id", commentId);

                Message model = new Message()
                {
                    SenderId = this.dataService.GetObjectByPropertyValue<User>("Username", this.User.Identity.Name).Id,
                    ReceiverId = comment.CommentorId,
                    MessageTitle = string.Concat("Regaring your comment \"", comment.Name, "\""),
                };

                return this.View(model);
            }

            this.dataService.AddObject(message);

            return this.RedirectToAction("Comments", "Owner");
        }

        /// <summary>
        /// Opens the page to send a message without the need of a comment.
        /// </summary>
        /// <returns>The send message page.</returns>
        [HttpGet]
        [Route("send_message")]
        public IActionResult SendMessageNoComment()
        {
            List<User> users = this.dataService.GetObjects<User>();
            users.Remove(users.Find(x => x.Username == this.User.Identity.Name));

            MessageViewModel model = new MessageViewModel()
            {
                SenderId = this.dataService.GetObjectByPropertyValue<User>("Username", this.User.Identity.Name).Id,
                AllUsers = users,
            };

            return this.View(model);
        }

        /// <summary>
        /// Sends the message to the user.
        /// </summary>
        /// <param name="message">The message being sent.</param>
        /// <returns>The owner's view message page.</returns>
        [HttpPost]
        [Route("send_message")]
        public IActionResult SendMessageNoComment(MessageViewModel message)
        {
            if (!this.ModelState.IsValid)
            {
                List<User> users = this.dataService.GetObjects<User>();
                users.Remove(users.Find(x => x.Username == this.User.Identity.Name));

                MessageViewModel model = new MessageViewModel()
                {
                    SenderId = this.dataService.GetObjectByPropertyValue<User>("Username", this.User.Identity.Name).Id,
                    AllUsers = users,
                };

                return this.View(model);
            }

            this.dataService.AddObject(message);

            return this.RedirectToAction("ViewMessages", "User");
        }

        /// <summary>
        /// Opens the page to view the stats of all pages.
        /// </summary>
        /// <returns>The page stat page.</returns>
        [Route("page_stats")]
        public IActionResult PageStats()
        {
            List<PageStat> model = this.dataService.GetObjects<PageStat>();

            return this.View(model);
        }

        /// <summary>
        /// Clears the view count of all pages and then returns owner to home page.
        /// </summary>
        /// <returns>The home page.</returns>
        [Route("clear_page_views")]
        public IActionResult ClearPageViews()
        {
            this.dataService.ClearPageViews();

            return this.RedirectToAction("PageStats", "Owner");
        }

        /// <summary>
        /// Opens the page to view comments left by users.
        /// </summary>
        /// <returns>The comments page.</returns>
        [Route("comments")]
        public IActionResult Comments()
        {
            AllCommentsViewModel model = new AllCommentsViewModel()
            {
                AllComments = this.dataService.GetObjects<Comment>(includes: "Commentor, Category, Page"),
                AllCategories = this.dataService.GetObjects<CommentCategory>(),
            };

            return this.View(model);
        }

        /// <summary>
        /// Marks a comment as being completed.
        /// </summary>
        /// <param name="id">The comment's id.</param>
        /// <returns>The comments page.</returns>
        [Route("complete_comment/{id:int}")]
        public IActionResult CompleteComment(int id)
        {
            Comment comment = this.dataService.GetObjectByPropertyValue<Comment>("Id", id);
            comment.IsCompleted = true;

            this.dataService.UpdateObject(comment);

            return this.RedirectToAction("Comments", "Owner");
        }

        /// <summary>
        /// Opens the review incomplete pokemon page.
        /// </summary>
        /// <param name="pokemonId">The pokemon's id.</param>
        /// <returns>The pokemon review page.</returns>
        [HttpGet]
        [Route("review_pokemon/{pokemonId:int}")]
        public IActionResult ReviewPokemon(int pokemonId)
        {
            // Ensuring that the pokemon really has all of these added.
            bool pokemonIsComplete = this.dataService.GetObjects<PokemonTypeDetail>("PokemonId", "AltFormPokemon, OriginalPokemon, Form").Exists(x => x.PokemonId == pokemonId) &&
                   this.dataService.GetAllPokemonWithAbilitiesAndIncomplete().Exists(x => x.PokemonId == pokemonId) &&
                   this.dataService.GetAllPokemonWithEggGroupsAndIncomplete().Exists(x => x.PokemonId == pokemonId) &&
                   this.dataService.GetObjects<BaseStat>(includes: "Pokemon").Exists(x => x.PokemonId == pokemonId) &&
                   this.dataService.GetObjects<EVYield>(includes: "Pokemon").Exists(x => x.PokemonId == pokemonId);

            Pokemon pokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", pokemonId, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth, BaseHappiness");

            if (pokemonIsComplete && !pokemon.IsComplete)
            {
                PokemonViewModel model = new PokemonViewModel()
                {
                    Pokemon = pokemon,
                    BaseStats = this.dataService.GetBaseStat(pokemon.Id),
                    EVYields = this.dataService.GetEVYields(pokemon.Id),
                    Typings = this.dataService.GetPokemonWithTypes(pokemon.Id),
                    Abilities = this.dataService.GetPokemonWithAbilities(pokemon.Id),
                    EggGroups = this.dataService.GetPokemonWithEggGroups(pokemon.Id),
                    CaptureRates = this.dataService.GetPokemonWithCaptureRates(pokemon.Id),
                    PreEvolutions = this.dataService.GetPreEvolutionIncludeIncomplete(pokemon.Id),
                    Evolutions = this.dataService.GetPokemonEvolutionsIncludeIncomplete(pokemon.Id),
                    Effectiveness = this.dataService.GetTypeChartPokemon(pokemon.Id),
                    AppConfig = this.appConfig,
                };

                if (this.dataService.CheckIfAltForm(pokemonId))
                {
                    model.OriginalPokemon = this.dataService.GetObjectByPropertyValue<PokemonFormDetail>("AltFormPokemonId", pokemon.Id, "AltFormPokemon, OriginalPokemon").OriginalPokemon;
                }

                return this.View(model);
            }
            else
            {
                return this.RedirectToAction("Pokemon", "Admin");
            }
        }

        /// <summary>
        /// Marks a newly finished pokemon as reviewed.
        /// </summary>
        /// <param name="pokemon">The newly finished pokemon.</param>
        /// <returns>The admin pokemon page.</returns>
        [HttpPost]
        [Route("review_pokemon/{pokemonId:int}")]
        public IActionResult ReviewPokemon(Pokemon pokemon)
        {
            ReviewedPokemon reviewedPokemon = this.dataService.GetObjectByPropertyValue<ReviewedPokemon>("PokemonId", pokemon.Id);
            if (reviewedPokemon == null)
            {
                reviewedPokemon = new ReviewedPokemon() { PokemonId = pokemon.Id };
                this.dataService.AddObject(reviewedPokemon);
            }

            return this.RedirectToAction("Pokemon", "Admin");
        }

        /// <summary>
        /// Opens the page to view all reviewed pokemon.
        /// </summary>
        /// <returns>The reviewed pokemon page.</returns>
        [Route("reviewed_pokemon")]
        public IActionResult ReviewedPokemon()
        {
            List<ReviewedPokemon> model = this.dataService.GetAllReviewedPokemon();

            return this.View(model);
        }

        /// <summary>
        /// Opens the page to mark all reviewed pokemon as complete.
        /// </summary>
        /// <returns>The admin pokemon page.</returns>
        [Route("complete_reviewed_pokemon")]
        public IActionResult CompleteReviewedPokemon()
        {
            List<ReviewedPokemon> reviewedPokemonList = this.dataService.GetAllReviewedPokemon();
            Pokemon pokemon;
            foreach (var r in reviewedPokemonList)
            {
                pokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", r.PokemonId);
                pokemon.IsComplete = true;
                this.dataService.UpdateObject(pokemon);
                this.dataService.DeleteObject<ReviewedPokemon>(r.Id);
            }

            return this.RedirectToAction("Pokemon", "Admin");
        }

        /// <summary>
        /// Marks a newly finished pokemon as completed.
        /// </summary>
        /// <param name="pokemonId">The pokemon's id.</param>
        /// <returns>The admin pokemon page.</returns>
        [Route("complete_pokemon/{pokemonId:int}")]
        public IActionResult CompletePokemon(int pokemonId)
        {
            Pokemon pokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", pokemonId);
            pokemon.IsComplete = true;
            this.dataService.UpdateObject(pokemon);

            return this.RedirectToAction("Pokemon", "Admin");
        }

        /// <summary>
        /// Opens the page to view a specific saved pokemon team.
        /// </summary>
        /// <param name="id">The pokemon team's id.</param>
        /// <returns>The pokemon team page.</returns>
        [Route("pokemon_teams/{id:int}")]
        public IActionResult PokemonTeams(int id)
        {
            PokemonTeamsViewModel model = new PokemonTeamsViewModel()
            {
                AllPokemonTeams = this.dataService.GetPokemonTeamsByUserId(id),
                AppConfig = this.appConfig,
            };

            return this.View(model);
        }
    }
}
