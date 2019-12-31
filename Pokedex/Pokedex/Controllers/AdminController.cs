using System.Collections.Generic;
using System.Linq;

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
            List<int> model = this._dataService.GetGenerations().Select(x => x.Id).OrderBy(x => x).ToList();

            return this.View(model);
        }

        [Route("generation")]
        public IActionResult Generations()
        {
            GenerationViewModel model = new GenerationViewModel()
            {
                AllGenerations = this._dataService.GetGenerations(),
                AllGames = this._dataService.GetGames(),
            };

            return this.View(model);
        }

        [Route("type")]
        public IActionResult Types()
        {
            TypeViewModel model = new TypeViewModel()
            {
                AllTypes = this._dataService.GetTypes(),
                AllPokemon = this._dataService.GetAllPokemonWithTypesAndIncomplete(),
            };

            return this.View(model);
        }

        [Route("egg_cycle")]
        public IActionResult EggCycles()
        {
            EggCycleViewModel model = new EggCycleViewModel()
            {
                AllEggCycles = this._dataService.GetEggCycles(),
                AllPokemon = this._dataService.GetAllPokemonIncludeIncomplete(),
            };

            return this.View(model);
        }

        [Route("experience_growth")]
        public IActionResult ExperienceGrowths()
        {
            ExperienceGrowthViewModel model = new ExperienceGrowthViewModel()
            {
                AllExperienceGrowths = this._dataService.GetExperienceGrowths(),
                AllPokemon = this._dataService.GetAllPokemonIncludeIncomplete(),
            };

            return this.View(model);
        }

        [Route("gender_ratio")]
        public IActionResult GenderRatios()
        {
            GenderRatiosViewModel model = new GenderRatiosViewModel()
            {
                AllGenderRatios = this._dataService.GetGenderRatios(),
                AllPokemon = this._dataService.GetAllPokemonIncludeIncomplete(),
            };

            return this.View(model);
        }

        [Route("form_items")]
        public IActionResult FormItems()
        {
            List<FormItem> model = this._dataService.GetFormItems();

            return this.View(model);
        }

        [Route("moves")]
        public IActionResult Moves()
        {
            AdminMoveViewModel model = new AdminMoveViewModel()
            {
                AllMoves = this._dataService.GetMoves(),
                AllTypes = this._dataService.GetTypes(),
                AllMoveCategories = this._dataService.GetMoveCategories(),
            };

            return this.View(model);
        }

        [Route("ability")]
        public IActionResult Abilities()
        {
            AbilityViewModel model = new AbilityViewModel()
            {
                AllAbilities = this._dataService.GetAbilities(),
                AllPokemon = this._dataService.GetAllPokemonWithAbilitiesAndIncomplete(),
            };

            return this.View(model);
        }

        [Route("legendary_type")]
        public IActionResult LegendaryTypes()
        {
            LegendaryTypeViewModel model = new LegendaryTypeViewModel()
            {
                AllLegendaryTypes = this._dataService.GetLegendaryTypes(),
                AllPokemon = this._dataService.GetAllPokemonWithLegendaryTypesAndIncomplete(),
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
                AllEggGroups = this._dataService.GetEggGroups(),
                AllPokemon = this._dataService.GetAllPokemonWithEggGroupsAndIncomplete(),
            };

            return this.View(model);
        }

        [Route("evolution_methods")]
        public IActionResult EvolutionMethods()
        {
            EvolutionMethodViewModel model = new EvolutionMethodViewModel()
            {
                AllEvolutionMethods = this._dataService.GetEvolutionMethods(),
                AllEvolutions = this._dataService.GetEvolutions(),
            };

            return this.View(model);
        }

        [Route("capture_rates")]
        public IActionResult CaptureRates()
        {
            CaptureRateViewModel model = new CaptureRateViewModel()
            {
                AllCaptureRates = this._dataService.GetCaptureRates(),
                AllPokemon = this._dataService.GetAllPokemonIncludeIncomplete(),
            };

            return this.View(model);
        }

        [Route("base_happinesses")]
        public IActionResult BaseHappinesses()
        {
            BaseHappinessViewModel model = new BaseHappinessViewModel()
            {
                AllBaseHappinesses = this._dataService.GetBaseHappinesses(),
                AllPokemon = this._dataService.GetAllPokemonIncludeIncomplete(),
            };

            return this.View(model);
        }

        [Route("classification")]
        public IActionResult Classifications()
        {
            ClassificationViewModel model = new ClassificationViewModel()
            {
                AllClassifications = this._dataService.GetClassifications(),
                AllPokemon = this._dataService.GetAllPokemonWithClassificationsAndIncomplete(),
            };

            return this.View(model);
        }

        [Route("game_availability/{pokemonId:int}")]
        public IActionResult PokemonGameDetails(int pokemonId)
        {
            Pokemon pokemon = this._dataService.GetPokemonById(pokemonId);
            if(this._dataService.CheckIfAltForm(pokemonId))
            {
                pokemon.Name += " (" + this._dataService.GetFormByAltFormId(pokemonId).Name + ")";
            }

            PokemonGameViewModel model = new PokemonGameViewModel()
            {
                Pokemon = pokemon,
                PokemonGameDetails = this._dataService.GetPokemonGameDetails(pokemonId),
                AllGames = this._dataService.GetGames().Where(x => x.ReleaseDate >= pokemon.Game.ReleaseDate).ToList(),
            };

            return this.View(model);
        }

        [Route("available_pokemon")]
        public IActionResult AvailablePokemon()
        {
            List<Generation> model = this._dataService.GetGenerations();

            return this.View(model);
        }

        [Route("nature")]
        public IActionResult Natures()
        {
            List<Nature> model = this._dataService.GetNatures();

            return this.View(model);
        }

        [Route("shiny_hunting_technique")]
        public IActionResult ShinyHuntingTechniques()
        {
            ShinyHuntViewModel model = new ShinyHuntViewModel()
            {
                AllShinyHunters = this._dataService.GetShinyHunters(),
                AllShinyHuntingTechniques = this._dataService.GetShinyHuntingTechniques(),
            };

            return this.View(model);
        }

        [Route("battle_item")]
        public IActionResult BattleItems()
        {
            List<Pokemon> pokemonList = this._dataService.GetAllPokemon();
            foreach(var p in pokemonList.Where(x => this._dataService.CheckIfAltForm(x.Id)))
            {
                p.Name += " (" + this._dataService.GetFormByAltFormId(p.Id).Name + ")";
            }

            BattleItemViewModel model = new BattleItemViewModel()
            {
                AllBattleItems = this._dataService.GetBattleItems(),
                AllPokemonTeamDetails = this._dataService.GetPokemonTeamDetails(),
                AllPokemon = pokemonList,
            };

            return this.View(model);
        }
    }
}
