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
    /// The class that handles the admin actions.
    /// </summary>
    [Authorize(Roles = "Admin,Owner")]
    [Route("admin")]
    public class AdminController : Controller
    {
        private readonly DataService dataService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminController"/> class.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        public AdminController(DataContext dataContext)
        {
            this.dataService = new DataService(dataContext);
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

        [Route("battle_item")]
        public IActionResult BattleItems()
        {
            List<Pokemon> pokemonList = this.dataService.GetAllPokemon();
            List<Pokemon> altFormsList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form").ConvertAll(x => x.AltFormPokemon);
            foreach (var p in pokemonList.Where(x => altFormsList.Any(y => y.Id == x.Id)))
            {
                p.Name = this.dataService.GetAltFormWithFormName(p.Id).Name;
            }

            BattleItemViewModel model = new BattleItemViewModel()
            {
                AllBattleItems = this.dataService.GetObjects<BattleItem>("GenerationId, Name", "Generation, Pokemon"),
                AllPokemonTeamDetails = this.dataService.GetObjects<PokemonTeamDetail>(includes: "Pokemon, Pokemon.Game.Generation, Ability, PokemonTeamEV, PokemonTeamIV, PokemonTeamMoveset, BattleItem, Nature"),
                AllPokemon = pokemonList,
            };

            return this.View(model);
        }
    }
}
