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
    /// The class that handles the requests for the owner.
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
        /// Opens the page to view the stats of all pages.
        /// </summary>
        /// <returns>The page stat page.</returns>
        [Route("page_stats")]
        public IActionResult PageStats()
        {
            List<PageStat> model = this.dataService.GetObjects<PageStat>().Where(x => !x.Name.Contains("Pokemon Page -")).ToList();

            return this.View(model);
        }

        /// <summary>
        /// Opens the page to view the stats of all pages.
        /// </summary>
        /// <returns>The page stat page.</returns>
        [Route("pokemon_page_stats")]
        public IActionResult PokemonPageStats()
        {
            List<PageStat> model = this.dataService.GetObjects<PageStat>().Where(x => x.Name.Contains("Pokemon Page -")).ToList();

            return this.View(model);
        }

        [Route("pokemon")]
        public IActionResult Pokemon()
        {
            List<Pokemon> pokemonList = this.dataService.GetObjects<Pokemon>("PokedexNumber, Id", "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth");
            List<Pokemon> altFormList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon").Select(x => x.AltFormPokemon).ToList();
            pokemonList = pokemonList.Where(x => !altFormList.Any(y => y.Id == x.Id)).ToList();

            List<int> model = pokemonList.Select(x => x.Game.GenerationId).Distinct().OrderBy(x => x).ToList();

            return this.View(model);
        }

        [Route("generations")]
        public IActionResult Generations()
        {
            GenerationViewModel model = new GenerationViewModel()
            {
                AllGenerations = this.dataService.GetObjects<Generation>(),
                AllGames = this.dataService.GetObjects<Game>("ReleaseDate, Id"),
            };

            return this.View(model);
        }

        [Route("regions")]
        public IActionResult Regions()
        {
            RegionViewModel model = new RegionViewModel()
            {
                AllRegions = this.dataService.GetObjects<Region>("GenerationId, Id", "Generation"),
            };

            return this.View(model);
        }

        [Route("games")]
        public IActionResult Games()
        {
            AdminGameViewModel model = new AdminGameViewModel()
            {
                AllGames = this.dataService.GetObjects<Game>("ReleaseDate, Id", "Generation").ToList(),
                AllGameRegionDetails = this.dataService.GetObjects<GameRegionDetail>(includes: "Region").ToList(),
                AllPokemon = this.dataService.GetObjects<Pokemon>("PokedexNumber, Id", "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth"),
            };

            return this.View(model);
        }

        [Route("type")]
        public IActionResult Types()
        {
            TypeViewModel model = new TypeViewModel()
            {
                AllTypes = this.dataService.GetObjects<Type>("Name"),
                AllPokemon = this.dataService.GetObjects<PokemonTypeDetail>("PokemonId", "Pokemon, PrimaryType, SecondaryType"),
            };

            return this.View(model);
        }

        [Route("egg_cycle")]
        public IActionResult EggCycles()
        {
            EggCycleViewModel model = new EggCycleViewModel()
            {
                AllEggCycles = this.dataService.GetObjects<EggCycle>("CycleCount"),
                AllPokemon = this.dataService.GetObjects<Pokemon>("PokedexNumber, Id", "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth"),
            };

            return this.View(model);
        }

        [Route("experience_growth")]
        public IActionResult ExperienceGrowths()
        {
            ExperienceGrowthViewModel model = new ExperienceGrowthViewModel()
            {
                AllExperienceGrowths = this.dataService.GetObjects<ExperienceGrowth>("Name"),
                AllPokemon = this.dataService.GetObjects<Pokemon>("PokedexNumber, Id", "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth"),
            };

            return this.View(model);
        }

        [Route("gender_ratio")]
        public IActionResult GenderRatios()
        {
            GenderRatiosViewModel model = new GenderRatiosViewModel()
            {
                AllGenderRatios = this.dataService.GetObjects<GenderRatio>(),
                AllPokemon = this.dataService.GetObjects<Pokemon>("PokedexNumber, Id", "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth"),
            };

            return this.View(model);
        }

        [Route("form_items")]
        public IActionResult FormItems()
        {
            List<FormItem> model = this.dataService.GetFormItems();

            return this.View(model);
        }

        [Route("form_groups")]
        public IActionResult FormGroups()
        {
            FormGroupViewModel model = new FormGroupViewModel()
            {
                AllFormGroups = this.dataService.GetObjects<FormGroup>(),
                AllForms = this.dataService.GetObjects<Form>("Name"),
                AllFormGroupGameDetails = this.dataService.GetObjects<FormGroupGameDetail>(includes: "Game"),
            };

            return this.View(model);
        }

        [Route("ability")]
        public IActionResult Abilities()
        {
            AbilityViewModel model = new AbilityViewModel()
            {
                AllAbilities = this.dataService.GetObjects<Ability>("GenerationId, Name"),
                AllPokemon = this.dataService.GetObjects<PokemonAbilityDetail>(includes: "Pokemon, PrimaryAbility, SecondaryAbility, HiddenAbility, SpecialEventAbility"),
            };

            return this.View(model);
        }

        [Route("legendary_type")]
        public IActionResult LegendaryTypes()
        {
            LegendaryTypeViewModel model = new LegendaryTypeViewModel()
            {
                AllLegendaryTypes = this.dataService.GetObjects<LegendaryType>("Type"),
                AllPokemon = this.dataService.GetObjects<PokemonLegendaryDetail>(includes: "Pokemon, LegendaryType"),
            };

            return this.View(model);
        }

        [Route("form")]
        public IActionResult Forms()
        {
            FormViewModel model = new FormViewModel()
            {
                AllForms = this.dataService.GetObjects<Form>("Name", "FormGroup"),
                AllPokemon = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, OriginalPokemon, Form"),
            };

            return this.View(model);
        }

        [Route("egg_group")]
        public IActionResult EggGroups()
        {
            EggGroupViewModel model = new EggGroupViewModel()
            {
                AllEggGroups = this.dataService.GetObjects<EggGroup>("Name"),
                AllPokemon = this.dataService.GetObjects<PokemonEggGroupDetail>(includes: "Pokemon, PrimaryEggGroup, SecondaryEggGroup"),
            };

            return this.View(model);
        }

        [Route("evolution_methods")]
        public IActionResult EvolutionMethods()
        {
            EvolutionMethodViewModel model = new EvolutionMethodViewModel()
            {
                AllEvolutionMethods = this.dataService.GetObjects<EvolutionMethod>("Name"),
                AllEvolutions = this.dataService.GetObjects<Evolution>(includes: "PreevolutionPokemon, PreevolutionPokemon.Game, EvolutionPokemon, EvolutionPokemon.Game, EvolutionMethod"),
            };

            return this.View(model);
        }

        [Route("capture_rates")]
        public IActionResult CaptureRates()
        {
            CaptureRateViewModel model = new CaptureRateViewModel()
            {
                AllCaptureRates = this.dataService.GetObjects<CaptureRate>("CatchRate"),
                AllPokemonCaptureRates = this.dataService.GetAllPokemonWithCaptureRates(),
            };

            return this.View(model);
        }

        [Route("base_happinesses")]
        public IActionResult BaseHappinesses()
        {
            BaseHappinessViewModel model = new BaseHappinessViewModel()
            {
                AllBaseHappinesses = this.dataService.GetObjects<BaseHappiness>("Happiness"),
                AllPokemon = this.dataService.GetObjects<PokemonBaseHappinessDetail>(),
            };

            return this.View(model);
        }

        [Route("classification")]
        public IActionResult Classifications()
        {
            ClassificationViewModel model = new ClassificationViewModel()
            {
                AllClassifications = this.dataService.GetObjects<Classification>("Name"),
                AllPokemon = this.dataService.GetObjects<Pokemon>(includes: "Classification"),
            };

            return this.View(model);
        }

        [Route("nature")]
        public IActionResult Natures()
        {
            List<Nature> model = this.dataService.GetObjects<Nature>("Name");

            return this.View(model);
        }

        [Route("mark")]
        public IActionResult Marks()
        {
            List<Mark> model = this.dataService.GetObjects<Mark>("Name");

            return this.View(model);
        }

        [Route("pokeball")]
        public IActionResult Pokeballs()
        {
            List<Pokeball> model = this.dataService.GetObjects<Pokeball>("Name");

            return this.View(model);
        }

        [Route("hunting_method")]
        public IActionResult HuntingMethods()
        {
            List<HuntingMethod> model = this.dataService.GetObjects<HuntingMethod>("Name");

            return this.View(model);
        }

        [Route("battle_item")]
        public IActionResult BattleItems()
        {
            List<Pokemon> pokemonList = this.dataService.GetAllPokemon();
            List<Pokemon> altFormList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form").ConvertAll(x => x.AltFormPokemon);
            pokemonList.Where(x => altFormList.Any(y => y.Id == x.Id)).ToList().ForEach(x => x = this.dataService.GetAltFormWithFormName(x.Id));

            BattleItemViewModel model = new BattleItemViewModel()
            {
                AllBattleItems = this.dataService.GetObjects<BattleItem>("GenerationId, Name", "Generation, Pokemon"),
                AllPokemonTeamDetails = this.dataService.GetObjects<PokemonTeamDetail>(includes: "Pokemon, Pokemon.Game.Generation, Ability, PokemonTeamEV, PokemonTeamIV, PokemonTeamMoveset, BattleItem, Nature"),
                AllPokemon = pokemonList,
            };

            return this.View(model);
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

            List<PokemonTeam> pokemonTeams = this.dataService.GetObjects<PokemonTeam>();

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
        /// Opens the page to view comments left by users.
        /// </summary>
        /// <returns>The comments page.</returns>
        [Route("comments")]
        public IActionResult Comments()
        {
            List<Comment> model = this.dataService.GetObjects<Comment>("Id", "Commentor");

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
            bool pokemonIsComplete = this.dataService.GetObjects<PokemonTypeDetail>("PokemonId", "Pokemon, PrimaryType, SecondaryType").Exists(x => x.PokemonId == pokemonId) &&
                   this.dataService.GetObjects<PokemonAbilityDetail>(includes: "Pokemon, PrimaryAbility, SecondaryAbility, HiddenAbility, SpecialEventAbility").Exists(x => x.PokemonId == pokemonId) &&
                   this.dataService.GetObjects<PokemonEggGroupDetail>(includes: "Pokemon, PrimaryEggGroup, SecondaryEggGroup").Exists(x => x.PokemonId == pokemonId) &&
                   this.dataService.GetObjects<BaseStat>(includes: "Pokemon").Exists(x => x.PokemonId == pokemonId) &&
                   this.dataService.GetObjects<EVYield>(includes: "Pokemon").Exists(x => x.PokemonId == pokemonId);

            Pokemon pokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", pokemonId, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth");

            if (pokemonIsComplete && !pokemon.IsComplete)
            {
                PokemonViewModel model = new PokemonViewModel()
                {
                    Pokemon = pokemon,
                    BaseStats = this.dataService.GetObjects<BaseStat>(includes: "Pokemon", whereProperty: "Pokemon.Id", wherePropertyValue: pokemon.Id),
                    EVYields = this.dataService.GetObjects<EVYield>(includes: "Pokemon", whereProperty: "Pokemon.Id", wherePropertyValue: pokemon.Id),
                    Typings = this.dataService.GetObjects<PokemonTypeDetail>(includes: "Pokemon, PrimaryType, SecondaryType, Generation", whereProperty: "PokemonId", wherePropertyValue: pokemon.Id),
                    Abilities = this.dataService.GetObjects<PokemonAbilityDetail>(includes: "Pokemon, PrimaryAbility, SecondaryAbility, HiddenAbility, SpecialEventAbility", whereProperty: "PokemonId", wherePropertyValue: pokemon.Id),
                    EggGroups = this.dataService.GetObjects<PokemonEggGroupDetail>(includes: "Pokemon, PrimaryEggGroup, SecondaryEggGroup", whereProperty: "PokemonId", wherePropertyValue: pokemon.Id),
                    CaptureRates = this.dataService.GetPokemonWithCaptureRates(pokemon.Id),
                    BaseHappinesses = this.dataService.GetObjects<PokemonBaseHappinessDetail>(includes: "Pokemon, BaseHappiness", whereProperty: "PokemonId", wherePropertyValue: pokemon.Id).OrderByDescending(x => x.GenerationId).ToList(),
                    PreEvolutions = this.dataService.GetPreEvolution(pokemon.Id),
                    Evolutions = this.dataService.GetPokemonEvolutions(pokemon.Id),
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
                return this.RedirectToAction("Pokemon", "Owner");
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
            pokemon.IsComplete = true;
            this.dataService.UpdateObject(pokemon);

            return this.RedirectToAction("Pokemon", "Owner");
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

            return this.RedirectToAction("Pokemon", "Owner");
        }
    }
}
