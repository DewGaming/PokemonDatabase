using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Pokedex.DataAccess.Models;

namespace Pokedex.Controllers
{
    [Authorize(Roles = "Owner")]
    [Route("admin")]
    public class OwnerController : Controller
    {
        private readonly DataService _dataService;

        public OwnerController(DataContext dataContext)
        {
            // Instantiate an instance of the data service.
            this._dataService = new DataService(dataContext);
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

        [HttpGet]
        [Route("complete_comment/{id:int}")]
        public IActionResult CompleteComment(int id)
        {
            Comment comment = this._dataService.GetComment(id);
            comment.IsCompleted = true;

            this._dataService.UpdateComment(comment);

            return this.RedirectToAction("Comments", "Owner");
        }

        [HttpGet]
        [Route("undo_completion/{id:int}")]
        public IActionResult UndoComment(int id)
        {
            Comment comment = this._dataService.GetComment(id);
            comment.IsCompleted = false;

            this._dataService.UpdateComment(comment);

            return this.RedirectToAction("Comments", "Owner");
        }

        [HttpGet]
        [Route("add_update")]
        public IActionResult Update()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_update")]
        public IActionResult Update(Update update)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            update.DateCreated = DateTime.Today;

            this._dataService.AddUpdate(update);

            return this.RedirectToAction("Index", "Home");
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
    }
}