using System.Collections.Generic;

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
            List<User> model = this._dataService.GetUsers();

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

        [Route("complete_pokemon/{pokemonId}")]
        public IActionResult CompletePokemon(string pokemonId)
        {
            // Ensuring that the pokemon really has all of these added.
            bool PokemonIsComplete = this._dataService.GetAllPokemonWithTypesAndIncomplete().Exists(x => x.PokemonId == pokemonId) &&
                   this._dataService.GetAllPokemonWithAbilitiesAndIncomplete().Exists(x => x.PokemonId == pokemonId) &&
                   this._dataService.GetAllPokemonWithEggGroupsAndIncomplete().Exists(x => x.PokemonId == pokemonId) &&
                   this._dataService.GetBaseStatsWithIncomplete().Exists(x => x.PokemonId == pokemonId) &&
                   this._dataService.GetEVYieldsWithIncomplete().Exists(x => x.PokemonId == pokemonId);

            Pokemon pokemon = this._dataService.GetPokemonByIdNoIncludes(pokemonId);
            pokemon.IsComplete = true;
            this._dataService.UpdatePokemon(pokemon);    
            
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