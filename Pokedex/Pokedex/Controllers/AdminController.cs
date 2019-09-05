using System.Collections.Generic;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        private readonly AppConfig _appConfig;

        public AdminController(IOptions<AppConfig> appConfig, DataContext dataContext)
        {
            this._appConfig = appConfig.Value;
            this._dataService = new DataService(dataContext);
        }

        [Route("pokemon")]
        public IActionResult Pokemon()
        {
            AllAdminPokemonViewModel allPokemon = new AllAdminPokemonViewModel(){
                AllPokemon = this._dataService.GetAllPokemonWithoutFormsWithIncomplete(),
                AllAltForms = this._dataService.GetAllAltForms(),
                AllEvolutions = this._dataService.GetEvolutions(),
                AllTypings = this._dataService.GetAllPokemonWithTypesAndIncomplete(),
                AllAbilities = this._dataService.GetAllPokemonWithAbilitiesAndIncomplete(),
                AllEggGroups = this._dataService.GetAllPokemonWithEggGroupsAndIncomplete(),
                AllBaseStats = this._dataService.GetBaseStatsWithIncomplete(),
                AllEVYields = this._dataService.GetEVYieldsWithIncomplete(),
                AllLegendaryDetails = this._dataService.GetAllPokemonWithLegendaryTypes(),
            };

            DropdownViewModel model = new DropdownViewModel(){
                AllPokemon = allPokemon,
                AppConfig = this._appConfig,
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

        [Route("legendary_type")]
        public IActionResult LegendaryTypes()
        {
            LegendaryTypeViewModel model = new LegendaryTypeViewModel()
            {
                AllLegendaryTypes = this._dataService.GetLegendaryTypesWithArchive(),
                AllPokemon = this._dataService.GetAllPokemonWithLegendaryTypes(),
            };

            return this.View(model);
        }

        [Route("form")]
        public IActionResult Forms()
        {
            FormViewModel model = new FormViewModel()
            {
                AllForms = this._dataService.GetForms(),
                AllPokemon = this._dataService.GetPokemonFormDetails(),
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
