using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Pokedex.DataAccess.Models;
using Pokedex.Models;

namespace Pokedex.Controllers
{
    [Authorize(Roles = "Admin,Owner")]
    [Route("admin")]
    public class AdminController : Controller
    {
        private readonly DataService _dataService;

        public AdminController(DataContext dataContext)
        {
            this._dataService = new DataService(dataContext);
        }

        [Route("pokemon")]
        public IActionResult Pokemon(bool slowConnection)
        {
            List<Pokemon> pokemonList = this._dataService.GetAllPokemonWithoutForms();
            List<PokemonFormDetail> altFormsList = this._dataService.GetAllAltForms();
            List<Evolution> evolutionList = this._dataService.GetEvolutions();

            AllAdminPokemonViewModel allPokemon = new AllAdminPokemonViewModel(){
                AllPokemon = pokemonList,
                AllAltForms = altFormsList,
                AllEvolutions = evolutionList,
                SlowConnection = slowConnection
            };

            DropdownViewModel model = new DropdownViewModel(){
                AllPokemon = allPokemon,
            };

            return this.View(model);
        }

        [Route("generation")]
        public IActionResult Generations()
        {
            GenerationViewModel model = new GenerationViewModel()
            {
                AllGenerations = this._dataService.GetGenerationsWithArchive(),
                AllPokemon = this._dataService.GetAllPokemon(),
            };

            return this.View(model);
        }

        [Route("type")]
        public IActionResult Types()
        {
            TypeViewModel model = new TypeViewModel()
            {
                AllTypes = this._dataService.GetTypesWithArchive(),
                AllPokemon = this._dataService.GetAllPokemonWithTypes(),
            };

            return this.View(model);
        }

        [Route("ability")]
        public IActionResult Abilities()
        {
            AbilityViewModel model = new AbilityViewModel()
            {
                AllAbilities = this._dataService.GetAbilitiesWithArchive(),
                AllPokemon = this._dataService.GetAllPokemonWithAbilities(),
            };

            return this.View(model);
        }

        [Route("egg_group")]
        public IActionResult EggGroups()
        {
            EggGroupViewModel model = new EggGroupViewModel()
            {
                AllEggGroups = this._dataService.GetEggGroupsWithArchive(),
                AllPokemon = this._dataService.GetAllPokemonWithEggGroups(),
            };

            return this.View(model);
        }

        [Route("classification")]
        public IActionResult Classifications()
        {
            ClassificationViewModel model = new ClassificationViewModel()
            {
                AllClassifications = this._dataService.GetClassificationsWithArchive(),
                AllPokemon = this._dataService.GetAllPokemonWithClassifications(),
            };

            return this.View(model);
        }

        [Route("shiny_hunting_technique")]
        public IActionResult ShinyHuntingTechniques()
        {
            ShinyHuntViewModel model = new ShinyHuntViewModel()
            {
                AllShinyHunters = this._dataService.GetShinyHunters(),
                AllShinyHuntingTechniques = this._dataService.GetShinyHuntingTechniquesWithArchive(),
            };

            return this.View(model);
        }

        [Route("shiny_hunt")]
        public IActionResult ShinyHunts()
        {
            List<ShinyHunt> model = this._dataService.GetShinyHuntersWithArchive();

            return this.View(model);
        }

        [Route("error")]
        public IActionResult Error()
        {
            return this.View();
        }
    }
}
