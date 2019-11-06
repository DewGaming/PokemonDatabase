using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Pokedex.DataAccess.Models;
using Pokedex.Models;

namespace Pokedex.Controllers
{
    [Authorize(Roles = "Owner")]
    [Route("admin")]
    public class OwnerController : Controller
    {
        private readonly DataService _dataService;

        private readonly AppConfig _appConfig;

        public OwnerController(DataContext dataContext, IOptions<AppConfig> appConfig)
        {
            // Instantiate an instance of the data service.
            this._dataService = new DataService(dataContext);
            this._appConfig = appConfig.Value;
        }

        [Route("users")]
        public IActionResult Users()
        {
            List<User> model = this._dataService.GetUsers().Where(x => !x.IsOwner).ToList();

            return this.View(model);
        }

        [Route("comments")]
        public IActionResult Comments()
        {
            List<Comment> model = this._dataService.GetComments();

            return this.View("Comments", model);
        }

        [Route("complete_comment/{id:int}")]
        public IActionResult CompleteComment(int id)
        {
            Comment comment = this._dataService.GetComment(id);
            comment.IsCompleted = true;

            this._dataService.UpdateComment(comment);

            return this.RedirectToAction("Comments", "Owner");
        }

        [Route("undo_completion/{id:int}")]
        public IActionResult UndoComment(int id)
        {
            Comment comment = this._dataService.GetComment(id);
            comment.IsCompleted = false;

            this._dataService.UpdateComment(comment);

            return this.RedirectToAction("Comments", "Owner");
        }

        [HttpGet]
        [Route("review_pokemon/{pokemonId}")]
        public IActionResult ReviewPokemon(string pokemonId)
        {
            // Ensuring that the pokemon really has all of these added.
            bool PokemonIsComplete = this._dataService.GetAllPokemonWithTypesAndIncomplete().Exists(x => x.PokemonId == pokemonId) &&
                   this._dataService.GetAllPokemonWithAbilitiesAndIncomplete().Exists(x => x.PokemonId == pokemonId) &&
                   this._dataService.GetAllPokemonWithEggGroupsAndIncomplete().Exists(x => x.PokemonId == pokemonId) &&
                   this._dataService.GetBaseStatsWithIncomplete().Exists(x => x.PokemonId == pokemonId) &&
                   this._dataService.GetEVYieldsWithIncomplete().Exists(x => x.PokemonId == pokemonId);

            Pokemon pokemon = this._dataService.GetPokemonById(pokemonId);

            if(PokemonIsComplete && !pokemon.IsComplete)
            {
                PokemonTypeDetail pokemonTypes = this._dataService.GetPokemonWithTypes(pokemon.Id);
                PokemonAbilityDetail pokemonAbilities = this._dataService.GetPokemonWithAbilities(pokemon.Id);
                PokemonEggGroupDetail pokemonEggGroups = this._dataService.GetPokemonWithEggGroups(pokemon.Id);

                PokemonViewModel model = new PokemonViewModel()
                {
                    Pokemon = pokemon,
                    BaseStats = this._dataService.GetBaseStat(pokemon.Id),
                    EVYields = this._dataService.GetEVYield(pokemon.Id),
                    PrimaryType = pokemonTypes.PrimaryType,
                    SecondaryType = pokemonTypes.SecondaryType,
                    PrimaryAbility = pokemonAbilities.PrimaryAbility,
                    SecondaryAbility = pokemonAbilities.SecondaryAbility,
                    HiddenAbility = pokemonAbilities.HiddenAbility,
                    PrimaryEggGroup = pokemonEggGroups.PrimaryEggGroup,
                    SecondaryEggGroup = pokemonEggGroups.SecondaryEggGroup,
                    PreEvolution = this._dataService.GetPreEvolution(pokemon.Id),
                    Evolutions = this._dataService.GetPokemonEvolutions(pokemon.Id),
                    Effectiveness = this._dataService.GetTypeChartPokemon(pokemon.Id),
                    AppConfig = this._appConfig,
                };

                if(pokemonId.Contains('-'))
                {
                    model.OriginalPokemon = this._dataService.GetOriginalPokemonByAltFormId(pokemon.Id);
                }

                return this.View(model);
            }
            else
            {
                return this.RedirectToAction("Pokemon", "Admin");   
            }
        }

        [HttpPost]
        [Route("review_pokemon/{pokemonId}")]
        public IActionResult ReviewPokemon(Pokemon pokemon, string pokemonId)
        {
            ReviewedPokemon reviewedPokemon = this._dataService.GetReviewedPokemonByPokemonId(pokemonId);
            this._dataService.AddReviewedPokemon(new ReviewedPokemon() { PokemonId = pokemonId });

            return this.RedirectToAction("Pokemon", "Admin");
        }

        [Route("reviewed_pokemon")]
        public IActionResult ReviewedPokemon()
        {
            List<ReviewedPokemon> model = this._dataService.GetAllReviewedPokemon();

            return this.View(model);
        }

        [Route("complete_reviewed_pokemon")]
        public IActionResult CompleteReviewedPokemon()
        {
            List<ReviewedPokemon> reviewedPokemonList = this._dataService.GetAllReviewedPokemon();
            Pokemon pokemon;
            foreach(var r in reviewedPokemonList)
            {
                pokemon = this._dataService.GetPokemonByIdNoIncludes(r.PokemonId);
                pokemon.IsComplete = true;
                this._dataService.UpdatePokemon(pokemon);
                this._dataService.DeleteReviewedPokemon(r.Id);
            }

            return this.RedirectToAction("Pokemon", "Admin");
        }

        [Route("shiny_hunting_counter/{id:int}")]
        public IActionResult ShinyHuntingCounter(int id)
        {
            List<ShinyHunt> model = this._dataService.GetShinyHunterById(id);

            return this.View(model);
        }

        [Route("pokemon_teams/{id:int}")]
        public IActionResult PokemonTeams(int id)
        {
            PokemonTeamsViewModel model = new PokemonTeamsViewModel(){
                AllPokemonTeams = this._dataService.GetPokemonTeamsByUserId(id),
                AppConfig = _appConfig,
            };

            return this.View(model);
        }
    }
}