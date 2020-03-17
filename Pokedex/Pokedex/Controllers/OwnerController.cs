using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Pokedex.DataAccess.Models;
using Pokedex.Models;
using System.Collections.Generic;
using System.Linq;

namespace Pokedex.Controllers
{
    [Authorize(Roles = "Owner")]
    [Route("admin")]
    public class OwnerController : Controller
    {
        private readonly DataService dataService;

        private readonly AppConfig appConfig;

        public OwnerController(DataContext dataContext, IOptions<AppConfig> appConfig)
        {
            // Instantiate an instance of the data service.
            this.dataService = new DataService(dataContext);
            this.appConfig = appConfig.Value;
        }

        [Route("users")]
        public IActionResult Users()
        {
            UserViewModel model = new UserViewModel()
            {
                UserList = this.dataService.GetUsers().Where(x => !x.IsOwner).OrderBy(x => x.LastVisit).ThenBy(x => x.Id).ToList(),
                UsersWithShinyHunts = new List<User>(),
                UsersWithPokemonTeams = new List<User>(),
            };

            List<ShinyHunt> shinyHunts = this.dataService.GetShinyHunters();
            List<PokemonTeam> pokemonTeams = this.dataService.GetPokemonTeams();

            foreach (var u in model.UserList)
            {
                if (shinyHunts.Where(x => x.UserId == u.Id).ToList().Count > 0)
                {
                    model.UsersWithShinyHunts.Add(u);
                }

                if (pokemonTeams.Where(x => x.UserId == u.Id).ToList().Count > 0)
                {
                    model.UsersWithPokemonTeams.Add(u);
                }
            }

            return this.View(model);
        }

        [HttpGet]
        [Route("send_message/{commentId:int}")]
        public IActionResult SendMessage(int commentId)
        {
            Comment comment = this.dataService.GetComment(commentId);

            Message model = new Message()
            {
                SenderId = this.dataService.GetUserWithUsername(this.User.Identity.Name).Id,
                ReceiverId = comment.CommentorId,
                MessageTitle = string.Concat("Regaring your comment \"", comment.Name, "\" "),
            };

            return this.View(model);
        }

        [HttpPost]
        [Route("send_message/{commentId:int}")]
        public IActionResult SendMessage(Message message, int commentId)
        {
            if (!this.ModelState.IsValid)
            {
                Comment comment = this.dataService.GetComment(commentId);

                Message model = new Message()
                {
                    SenderId = this.dataService.GetUserWithUsername(this.User.Identity.Name).Id,
                    ReceiverId = comment.CommentorId,
                    MessageTitle = string.Concat("Regaring your comment \"", comment.Name, "\""),
                };

                return this.View(model);
            }

            this.dataService.AddMessage(message);

            return this.RedirectToAction("Comments", "Owner");
        }

        [HttpGet]
        [Route("send_message")]
        public IActionResult SendMessageNoComment()
        {
            List<User> users = this.dataService.GetUsers();
            users.Remove(users.Find(x => x.Username == this.User.Identity.Name));

            MessageViewModel model = new MessageViewModel()
            {
                SenderId = this.dataService.GetUserWithUsername(this.User.Identity.Name).Id,
                AllUsers = users,
            };

            return this.View(model);
        }

        [HttpPost]
        [Route("send_message")]
        public IActionResult SendMessageNoComment(MessageViewModel message)
        {
            if (!this.ModelState.IsValid)
            {
                List<User> users = this.dataService.GetUsers();
                users.Remove(users.Find(x => x.Username == this.User.Identity.Name));

                MessageViewModel model = new MessageViewModel()
                {
                    SenderId = this.dataService.GetUserWithUsername(this.User.Identity.Name).Id,
                    AllUsers = users,
                };

                return this.View(model);
            }

            this.dataService.AddMessage(message);

            return this.RedirectToAction("ViewMessages", "User");
        }

        [Route("comments")]
        public IActionResult Comments()
        {
            AllCommentsViewModel model = new AllCommentsViewModel()
            {
                AllComments = this.dataService.GetComments(),
                CommentTypes = this.appConfig.CommentCategories,
            };

            return this.View(model);
        }

        [Route("complete_comment/{id:int}")]
        public IActionResult CompleteComment(int id)
        {
            Comment comment = this.dataService.GetComment(id);
            comment.IsCompleted = true;

            this.dataService.UpdateComment(comment);

            return this.RedirectToAction("Comments", "Owner");
        }

        [Route("undo_completion/{id:int}")]
        public IActionResult UndoComment(int id)
        {
            Comment comment = this.dataService.GetComment(id);
            comment.IsCompleted = false;

            this.dataService.UpdateComment(comment);

            return this.RedirectToAction("Comments", "Owner");
        }

        [HttpGet]
        [Route("review_pokemon/{pokemonId:int}")]
        public IActionResult ReviewPokemon(int pokemonId)
        {
            // Ensuring that the pokemon really has all of these added.
            bool pokemonIsComplete = this.dataService.GetAllPokemonWithTypesAndIncomplete().Exists(x => x.PokemonId == pokemonId) &&
                   this.dataService.GetAllPokemonWithAbilitiesAndIncomplete().Exists(x => x.PokemonId == pokemonId) &&
                   this.dataService.GetAllPokemonWithEggGroupsAndIncomplete().Exists(x => x.PokemonId == pokemonId) &&
                   this.dataService.GetBaseStatsWithIncomplete().Exists(x => x.PokemonId == pokemonId) &&
                   this.dataService.GetEVYieldsWithIncomplete().Exists(x => x.PokemonId == pokemonId);

            Pokemon pokemon = this.dataService.GetPokemonById(pokemonId);

            if (pokemonIsComplete && !pokemon.IsComplete)
            {
                PokemonTypeDetail pokemonTypes = this.dataService.GetPokemonWithTypes(pokemon.Id);
                PokemonAbilityDetail pokemonAbilities = this.dataService.GetPokemonWithAbilities(pokemon.Id);
                PokemonEggGroupDetail pokemonEggGroups = this.dataService.GetPokemonWithEggGroups(pokemon.Id);

                PokemonViewModel model = new PokemonViewModel()
                {
                    Pokemon = pokemon,
                    BaseStats = this.dataService.GetBaseStat(pokemon.Id),
                    EVYields = this.dataService.GetEVYield(pokemon.Id),
                    PrimaryType = pokemonTypes.PrimaryType,
                    SecondaryType = pokemonTypes.SecondaryType,
                    PrimaryAbility = pokemonAbilities.PrimaryAbility,
                    SecondaryAbility = pokemonAbilities.SecondaryAbility,
                    HiddenAbility = pokemonAbilities.HiddenAbility,
                    PrimaryEggGroup = pokemonEggGroups.PrimaryEggGroup,
                    SecondaryEggGroup = pokemonEggGroups.SecondaryEggGroup,
                    PreEvolution = this.dataService.GetPreEvolutionIncludeIncomplete(pokemon.Id),
                    Evolutions = this.dataService.GetPokemonEvolutionsIncludeIncomplete(pokemon.Id),
                    Effectiveness = this.dataService.GetTypeChartPokemon(pokemon.Id),
                    AppConfig = this.appConfig,
                };

                if (this.dataService.CheckIfAltForm(pokemonId))
                {
                    model.OriginalPokemon = this.dataService.GetOriginalPokemonByAltFormId(pokemon.Id);
                }

                return this.View(model);
            }
            else
            {
                return this.RedirectToAction("Pokemon", "Admin");
            }
        }

        [HttpPost]
        [Route("review_pokemon/{pokemonId:int}")]
        public IActionResult ReviewPokemon(Pokemon pokemon)
        {
            ReviewedPokemon reviewedPokemon = this.dataService.GetReviewedPokemonByPokemonId(pokemon.Id);
            if (reviewedPokemon != null)
            {
                this.dataService.AddReviewedPokemon(new ReviewedPokemon() { PokemonId = pokemon.Id });
            }

            return this.RedirectToAction("Pokemon", "Admin");
        }

        [Route("reviewed_pokemon")]
        public IActionResult ReviewedPokemon()
        {
            List<ReviewedPokemon> model = this.dataService.GetAllReviewedPokemon();

            return this.View(model);
        }

        [Route("complete_reviewed_pokemon")]
        public IActionResult CompleteReviewedPokemon()
        {
            List<ReviewedPokemon> reviewedPokemonList = this.dataService.GetAllReviewedPokemon();
            Pokemon pokemon;
            foreach (var r in reviewedPokemonList)
            {
                pokemon = this.dataService.GetPokemonByIdNoIncludes(r.PokemonId);
                pokemon.IsComplete = true;
                this.dataService.UpdatePokemon(pokemon);
                this.dataService.DeleteReviewedPokemon(r.Id);
            }

            return this.RedirectToAction("Pokemon", "Admin");
        }

        [Route("complete_pokemon/{pokemonId:int}")]
        public IActionResult CompletePokemon(int pokemonId)
        {
            Pokemon pokemon = this.dataService.GetPokemonByIdNoIncludes(pokemonId);
            pokemon.IsComplete = true;
            this.dataService.UpdatePokemon(pokemon);

            return this.RedirectToAction("Pokemon", "Admin");
        }

        [Route("shiny_hunting_counter/{id:int}")]
        public IActionResult ShinyHuntingCounter(int id)
        {
            List<ShinyHunt> model = this.dataService.GetShinyHunterById(id);

            return this.View(model);
        }

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
