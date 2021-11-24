using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Pokedex.DataAccess.Models;
using Pokedex.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Pokedex.Controllers
{
    /// <summary>
    /// The class that is used to represent the add controller.
    /// </summary>
    [Authorize(Roles = "Owner")]
    [Route("admin")]
    public class AddController : Controller
    {
        private readonly DataService dataService;

        private readonly AppConfig appConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddController"/> class.
        /// </summary>
        /// <param name="appConfig">The application's configuration.</param>
        /// <param name="dataContext">The pokemon database's context.</param>
        public AddController(IOptions<AppConfig> appConfig, DataContext dataContext)
        {
            // Instantiate an instance of the data service.
            this.appConfig = appConfig.Value;
            this.dataService = new DataService(dataContext);
        }

        /// <summary>
        /// Adds an evolution for a pokemon.
        /// </summary>
        /// <param name="pokemonId">The pokemon's Id.</param>
        /// <param name="generationId">The generation's Id.</param>
        /// <returns>The view to add the pokemon's evolution.</returns>
        [HttpGet]
        [Route("add_evolution/{pokemonId:int}/{generationId:int}")]
        public IActionResult Evolution(int pokemonId, int generationId)
        {
            Pokemon evolutionPokemon = this.dataService.GetPokemonById(pokemonId);
            EvolutionViewModel model = new EvolutionViewModel()
            {
                AllEvolutionMethods = this.dataService.GetObjects<EvolutionMethod>("Name"),
                EvolutionPokemon = evolutionPokemon,
                EvolutionPokemonId = evolutionPokemon.Id,
                GenerationId = generationId,
            };

            List<Pokemon> pokemonList = this.dataService.GetObjects<Pokemon>("PokedexNumber, Id", "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth, BaseHappiness").Where(x => x.Id != pokemonId).ToList();
            List<Pokemon> altFormsList = this.dataService.GetAllAltForms().ConvertAll(x => x.AltFormPokemon);
            foreach (var pokemon in pokemonList.Where(x => altFormsList.Any(y => y.Id == x.Id)))
            {
                pokemon.Name = string.Concat(pokemon.Name, " (", this.dataService.GetPokemonFormName(pokemon.Id), ")");
            }

            model.AllPokemon = pokemonList;

            return this.View(model);
        }

        /// <summary>
        /// Adds an evolution for a pokemon.
        /// </summary>
        /// <param name="evolution">The generated evolution.</param>
        /// <returns>The admin pokemon view.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_evolution/{pokemonId:int}/{generationId:int}")]
        public IActionResult Evolution(Evolution evolution)
        {
            if (!this.ModelState.IsValid)
            {
                EvolutionViewModel model = new EvolutionViewModel()
                {
                    AllEvolutionMethods = this.dataService.GetObjects<EvolutionMethod>("Name"),
                    EvolutionPokemon = evolution.EvolutionPokemon,
                    EvolutionPokemonId = evolution.EvolutionPokemon.Id,
                    GenerationId = evolution.GenerationId,
                };
                List<Pokemon> pokemonList = this.dataService.GetObjects<Pokemon>("PokedexNumber, Id", "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth, BaseHappiness").Where(x => x.Id != evolution.EvolutionPokemonId).ToList();
                List<Pokemon> altFormsList = this.dataService.GetAllAltForms().ConvertAll(x => x.AltFormPokemon);
                foreach (var pokemon in pokemonList.Where(x => altFormsList.Any(y => y.Id == x.Id)))
                {
                    pokemon.Name = string.Concat(pokemon.Name, " (", this.dataService.GetPokemonFormName(pokemon.Id), ")");
                }

                model.AllPokemon = pokemonList;

                return this.View(model);
            }

            this.dataService.AddEvolution(evolution);

            if (this.dataService.GetPokemonById(evolution.EvolutionPokemonId).IsComplete)
            {
                return this.RedirectToAction("Pokemon", "Admin");
            }
            else
            {
                return this.RedirectToAction("ReviewPokemon", "Owner", new { pokemonId = evolution.EvolutionPokemonId });
            }
        }

        /// <summary>
        /// Adds an generation.
        /// </summary>
        /// <returns>The view to add the generation.</returns>
        [Route("add_generation")]
        public IActionResult Generation()
        {
            this.dataService.AddGeneration();

            return this.RedirectToAction("Generations", "Admin");
        }

        [HttpGet]
        [Route("add_region")]
        public IActionResult Region()
        {
            RegionAdminViewModel model = new RegionAdminViewModel()
            {
                AllGenerations = this.dataService.GetObjects<Generation>(),
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_region")]
        public IActionResult Region(Region region)
        {
            if (!this.ModelState.IsValid)
            {
                RegionAdminViewModel model = new RegionAdminViewModel()
                {
                    AllGenerations = this.dataService.GetObjects<Generation>(),
                };

                return this.View(model);
            }

            this.dataService.AddRegion(region);

            return this.RedirectToAction("Regions", "Admin");
        }

        [HttpGet]
        [Route("add_form_item")]
        public IActionResult FormItem()
        {
            FormItemViewModel model = new FormItemViewModel();
            List<Pokemon> altForms = this.dataService.GetPokemonFormDetails().Where(x => x.Form.NeedsItem).ToList().ConvertAll(x => x.AltFormPokemon);

            foreach (var p in this.dataService.GetFormItems().Select(x => x.Pokemon))
            {
                altForms.Remove(altForms.Find(x => x.PokedexNumber == p.PokedexNumber));
            }

            altForms.Remove(altForms.Find(x => x.Name == "Rayquaza"));

            foreach (var p in altForms)
            {
                p.Name = string.Concat(p.Name, " (", this.dataService.GetPokemonFormName(p.Id), ")");
            }

            model.AllPokemon = altForms;

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_form_item")]
        public IActionResult FormItem(FormItem formItem)
        {
            if (!this.ModelState.IsValid)
            {
                FormItemViewModel model = new FormItemViewModel()
                {
                    AllPokemon = this.dataService.GetAllPokemonOnlyForms(),
                };

                return this.View(model);
            }

            this.dataService.AddFormItem(formItem);

            return this.RedirectToAction("FormItems", "Admin");
        }

        [HttpGet]
        [Route("add_type")]
        public IActionResult Type()
        {
            TypeGenerationViewModel model = new TypeGenerationViewModel()
            {
                AllGenerations = this.dataService.GetObjects<Generation>(),
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_type")]
        public IActionResult Type(DataAccess.Models.Type type)
        {
            if (!this.ModelState.IsValid)
            {
                TypeGenerationViewModel model = new TypeGenerationViewModel()
                {
                    AllGenerations = this.dataService.GetObjects<Generation>(),
                };

                return this.View(model);
            }

            this.dataService.AddType(type);

            return this.RedirectToAction("Types", "Admin");
        }

        [HttpGet]
        [Route("add_egg_cycle")]
        public IActionResult EggCycle()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_egg_cycle")]
        public IActionResult EggCycle(EggCycle eggCycle)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this.dataService.AddEggCycle(eggCycle);

            return this.RedirectToAction("EggCycles", "Admin");
        }

        [HttpGet]
        [Route("add_experience_growth")]
        public IActionResult ExperienceGrowth()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_experience_growth")]
        public IActionResult ExperienceGrowth(ExperienceGrowth experienceGrowth)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this.dataService.AddExperienceGrowth(experienceGrowth);

            return this.RedirectToAction("ExperienceGrowths", "Admin");
        }

        [HttpGet]
        [Route("add_gender_ratio")]
        public IActionResult GenderRatio()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_gender_ratio")]
        public IActionResult GenderRatio(GenderRatio genderRatio)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this.dataService.AddGenderRatio(genderRatio);

            return this.RedirectToAction("GenderRatios", "Admin");
        }

        [HttpGet]
        [Route("add_legendary_type")]
        public IActionResult LegendaryType()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_legendary_type")]
        public IActionResult LegendaryType(LegendaryType legendaryType)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this.dataService.AddLegendaryType(legendaryType);

            return this.RedirectToAction("LegendaryTypes", "Admin");
        }

        [HttpGet]
        [Route("add_egg_group")]
        public IActionResult EggGroup()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_egg_group")]
        public IActionResult EggGroup(EggGroup eggGroup)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this.dataService.AddEggGroup(eggGroup);

            return this.RedirectToAction("EggGroups", "Admin");
        }

        [HttpGet]
        [Route("add_classification")]
        public IActionResult Classification()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_classification")]
        public IActionResult Classification(Classification classification)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            if (classification.Name.Contains("pokemon"))
            {
                classification.Name = classification.Name.Replace("pokemon", "Pokemon");
            }
            else if (!classification.Name.Contains("Pokemon") && !classification.Name.Contains("pokemon"))
            {
                classification.Name = string.Concat(classification.Name.Trim(), " Pokemon");
            }

            this.dataService.AddClassification(classification);

            return this.RedirectToAction("Classifications", "Admin");
        }

        [HttpGet]
        [Route("add_nature")]
        public IActionResult Nature()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_nature")]
        public IActionResult Nature(Nature nature)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this.dataService.AddNature(nature);

            return this.RedirectToAction("Natures", "Admin");
        }

        [HttpGet]
        [Route("add_form")]
        public IActionResult Form()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_form")]
        public IActionResult Form(Form form)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this.dataService.AddForm(form);

            return this.RedirectToAction("Forms", "Admin");
        }

        [HttpGet]
        [Route("add_evolution_method")]
        public IActionResult EvolutionMethod()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_evolution_method")]
        public IActionResult EvolutionMethod(EvolutionMethod evolutionMethod)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this.dataService.AddEvolutionMethod(evolutionMethod);

            return this.RedirectToAction("EvolutionMethods", "Admin");
        }

        [HttpGet]
        [Route("add_season")]
        public IActionResult Season()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_season")]
        public IActionResult Season(Season season)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this.dataService.AddSeason(season);

            return this.RedirectToAction("Seasons", "Admin");
        }

        [HttpGet]
        [Route("add_time")]
        public IActionResult Time()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_time")]
        public IActionResult Time(Time time)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this.dataService.AddTime(time);

            return this.RedirectToAction("Times", "Admin");
        }

        [HttpGet]
        [Route("add_capture_rate")]
        public IActionResult CaptureRate()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_capture_rate")]
        public IActionResult CaptureRate(CaptureRate captureRate)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this.dataService.AddCaptureRate(captureRate);

            return this.RedirectToAction("CaptureRates", "Admin");
        }

        [HttpGet]
        [Route("add_capture_method")]
        public IActionResult CaptureMethod()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_capture_method")]
        public IActionResult CaptureMethod(CaptureMethod captureMethod)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this.dataService.AddCaptureMethod(captureMethod);

            return this.RedirectToAction("CaptureMethods", "Admin");
        }

        [HttpGet]
        [Route("add_location")]
        public IActionResult Location()
        {
            LocationViewModel model = new LocationViewModel()
            {
                AllRegions = this.dataService.GetObjects<Region>("GenerationId, Id"),
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_location")]
        public IActionResult Location(Location location)
        {
            if (!this.ModelState.IsValid)
            {
                LocationViewModel model = new LocationViewModel()
                {
                    AllRegions = this.dataService.GetObjects<Region>("GenerationId, Id"),
                };

                return this.View(model);
            }

            this.dataService.AddLocation(location);

            return this.RedirectToAction("Locations", "Admin");
        }

        [HttpGet]
        [Route("add_pokemon_availability/{locationId:int}")]
        public IActionResult PokemonLocationDetail(int locationId)
        {
            Location location = this.dataService.GetObjectByPropertyValue<Location>("Id", locationId);
            PokemonLocationDetailViewModel model = new PokemonLocationDetailViewModel()
            {
                LocationId = location.Id,
                AllPokemon = this.dataService.GetPokemonForLocation(location.Id),
                AllCaptureMethods = this.dataService.GetObjects<CaptureMethod>("Name"),
                AllTimes = this.dataService.GetObjects<Time>("Name"),
                AllSeasons = this.dataService.GetObjects<Season>("Name"),
                Region = this.dataService.GetObjectByPropertyValue<Region>("Id", location.RegionId),
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_pokemon_availability/{locationId:int}")]
        public IActionResult PokemonLocationDetail(PokemonLocationDetail pokemonLocationDetail, int locationId)
        {
            if (!this.ModelState.IsValid)
            {
                Location location = this.dataService.GetObjectByPropertyValue<Location>("Id", locationId);
                PokemonLocationDetailViewModel model = new PokemonLocationDetailViewModel()
                {
                    LocationId = location.Id,
                    AllPokemon = this.dataService.GetPokemonForLocation(location.Id),
                    AllCaptureMethods = this.dataService.GetObjects<CaptureMethod>("Name"),
                    AllTimes = this.dataService.GetObjects<Time>("Name"),
                    AllSeasons = this.dataService.GetObjects<Season>("Name"),
                    Region = this.dataService.GetObjectByPropertyValue<Region>("Id", location.RegionId),
                };

                return this.View(model);
            }

            if (pokemonLocationDetail.SpecialSpawn && pokemonLocationDetail.ChanceOfEncounter > 0)
            {
                pokemonLocationDetail.ChanceOfEncounter = 0;
            }

            PokemonLocationDetail existingEntry = this.dataService.GetObjects<PokemonLocationDetail>()
                .Find(x => x.ChanceOfEncounter == pokemonLocationDetail.ChanceOfEncounter
                && x.MinimumLevel == pokemonLocationDetail.MinimumLevel
                && x.MaximumLevel == pokemonLocationDetail.MaximumLevel
                && x.PokemonId == pokemonLocationDetail.PokemonId
                && x.LocationId == pokemonLocationDetail.LocationId
                && x.CaptureMethodId == pokemonLocationDetail.CaptureMethodId
                && x.SOSBattleOnly == pokemonLocationDetail.SOSBattleOnly
                && x.SpecialSpawn == pokemonLocationDetail.SpecialSpawn
                && x.FailedSnag == pokemonLocationDetail.FailedSnag);

            if (existingEntry != null)
            {
                pokemonLocationDetail.Id = existingEntry.Id;
            }
            else
            {
                this.dataService.AddPokemonLocationDetail(pokemonLocationDetail);
            }

            return this.RedirectToAction("PokemonLocationGameDetail", "Edit", new { pokemonLocationDetailId = pokemonLocationDetail.Id });
        }

        [HttpGet]
        [Route("add_ability")]
        public IActionResult Ability()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_ability")]
        public IActionResult Ability(Ability ability)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this.dataService.AddAbility(ability);

            return this.RedirectToAction("Abilities", "Admin");
        }

        [HttpGet]
        [Route("add_comment_category")]
        public IActionResult CommentCategory()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_comment_category")]
        public IActionResult CommentCategory(CommentCategory commentCategory)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this.dataService.AddCommentCategory(commentCategory);

            return this.RedirectToAction("CommentCategories", "Admin");
        }

        [HttpGet]
        [Route("add_pokeball")]
        public IActionResult Pokeball()
        {
            PokeballAdminViewModel model = new PokeballAdminViewModel()
            {
                AllGenerations = this.dataService.GetObjects<Generation>(),
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_pokeball")]
        public IActionResult Pokeball(Pokeball pokeball)
        {
            if (!this.ModelState.IsValid)
            {
                PokeballAdminViewModel model = new PokeballAdminViewModel()
                {
                    AllGenerations = this.dataService.GetObjects<Generation>(),
                };

                return this.View(model);
            }

            this.dataService.AddPokeball(pokeball);

            return this.RedirectToAction("Pokeballs", "Admin");
        }

        [HttpGet]
        [Route("add_pokeball_catch_modifier_detail/{id:int}")]
        public IActionResult PokeballCatchModifierDetail()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_pokeball_catch_modifier_detail/{id:int}")]
        public IActionResult PokeballCatchModifierDetail(PokeballCatchModifierDetail pokeballCatchModifierDetail, int id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            pokeballCatchModifierDetail.PokeballId = id;
            pokeballCatchModifierDetail.Id = 0;

            this.dataService.AddPokeballCatchModifierDetail(pokeballCatchModifierDetail);

            return this.RedirectToAction("Pokeballs", "Admin");
        }

        [HttpGet]
        [Route("add_comment_page")]
        public IActionResult CommentPage()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_comment_page")]
        public IActionResult CommentPage(CommentPage commentPage)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this.dataService.AddCommentPage(commentPage);

            return this.RedirectToAction("CommentPages", "Admin");
        }

        [HttpGet]
        [Route("add_base_happiness")]
        public IActionResult BaseHappiness()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_base_happiness")]
        public IActionResult BaseHappiness(BaseHappiness baseHappiness)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this.dataService.AddBaseHappiness(baseHappiness);

            return this.RedirectToAction("BaseHappinesses", "Admin");
        }

        [HttpGet]
        [Route("add_game")]
        public IActionResult Game()
        {
            GameViewModel model = new GameViewModel()
            {
                AllGenerations = this.dataService.GetObjects<Generation>(),
                AllRegions = this.dataService.GetObjects<Region>("GenerationId, Id"),
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_game")]
        public IActionResult Game(Game game, List<int> regionIds)
        {
            if (!this.ModelState.IsValid)
            {
                GameViewModel model = new GameViewModel()
                {
                    AllGenerations = this.dataService.GetObjects<Generation>(),
                    AllRegions = this.dataService.GetObjects<Region>("GenerationId, Id"),
                };

                return this.View(model);
            }

            this.dataService.AddGame(game);

            foreach (var r in regionIds)
            {
                GameRegionDetail gameRegionDetail = new GameRegionDetail()
                {
                    GameId = game.Id,
                    RegionId = r,
                };

                this.dataService.AddGameRegionDetail(gameRegionDetail);
            }

            return this.RedirectToAction("Games", "Admin");
        }

        [HttpGet]
        [Route("add_status")]
        public IActionResult Status()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_status")]
        public IActionResult Status(Status status)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this.dataService.AddStatus(status);

            return this.RedirectToAction("Statuses", "Admin");
        }

        [HttpGet]
        [Route("add_pokemon_capture_rate/{pokemonId:int}/{generationId:int}")]
        public IActionResult CaptureRates(int pokemonId, int generationId)
        {
            PokemonCaptureRateViewModel model = new PokemonCaptureRateViewModel()
            {
                PokemonId = pokemonId,
                GenerationId = generationId,
                AllCaptureRates = this.dataService.GetObjects<CaptureRate>("CatchRate"),
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_pokemon_capture_rate/{pokemonId:int}/{generationId:int}")]
        public IActionResult CaptureRates(PokemonCaptureRateDetail pokemonCaptureRate)
        {
            if (!this.ModelState.IsValid)
            {
                PokemonCaptureRateViewModel model = new PokemonCaptureRateViewModel()
                {
                    PokemonId = pokemonCaptureRate.PokemonId,
                    GenerationId = pokemonCaptureRate.GenerationId,
                    AllCaptureRates = this.dataService.GetObjects<CaptureRate>("CatchRate"),
                };

                return this.View(model);
            }

            this.dataService.AddPokemonCaptureRateDetail(pokemonCaptureRate);

            return this.RedirectToAction("Pokemon", "Admin");
        }

        [HttpGet]
        [Route("add_battle_item")]
        public IActionResult BattleItem()
        {
            List<Pokemon> pokemonList = this.dataService.GetAllPokemon();
            List<Pokemon> altFormsList = this.dataService.GetAllAltForms().ConvertAll(x => x.AltFormPokemon);
            foreach (var p in pokemonList.Where(x => altFormsList.Any(y => y.Id == x.Id)))
            {
                p.Name = this.dataService.GetAltFormWithFormName(p.Id).Name;
            }

            BattleItemViewModel model = new BattleItemViewModel()
            {
                AllGenerations = this.dataService.GetObjects<Generation>().Where(x => x.Id > 1).ToList(),
                AllPokemon = pokemonList,
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_battle_item")]
        public IActionResult BattleItem(BattleItem battleItem)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this.dataService.AddBattleItem(battleItem);

            return this.RedirectToAction("BattleItems", "Admin");
        }

        [HttpGet]
        [Route("add_pokemon")]
        public IActionResult Pokemon()
        {
            BasePokemonViewModel model = new BasePokemonViewModel()
            {
                AllBaseHappinesses = this.dataService.GetObjects<BaseHappiness>("Happiness"),
                AllClassifications = this.dataService.GetObjects<Classification>("Name"),
                AllCaptureRates = this.dataService.GetObjects<CaptureRate>("CatchRate"),
                AllEggCycles = this.dataService.GetObjects<EggCycle>("CycleCount"),
                AllExperienceGrowths = this.dataService.GetObjects<ExperienceGrowth>("Name"),
                AllGenderRatios = new List<GenderRatioViewModel>(),
                AllGames = this.dataService.GetObjects<Game>("ReleaseDate, Id"),
            };

            foreach (GenderRatio genderRatio in this.dataService.GetObjects<GenderRatio>())
            {
                GenderRatioViewModel viewModel = new GenderRatioViewModel()
                {
                    Id = genderRatio.Id,
                };

                if (genderRatio.MaleRatio == genderRatio.FemaleRatio && genderRatio.MaleRatio == 0)
                {
                    viewModel.GenderRatioString = "Genderless";
                }
                else if (genderRatio.FemaleRatio == 0)
                {
                    viewModel.GenderRatioString = string.Concat(genderRatio.MaleRatio, "% Male");
                }
                else if (genderRatio.MaleRatio == 0)
                {
                    viewModel.GenderRatioString = string.Concat(genderRatio.FemaleRatio, "% Female");
                }
                else
                {
                    viewModel.GenderRatioString = string.Concat(genderRatio.MaleRatio, "% Male / ", genderRatio.FemaleRatio, "% Female");
                }

                model.AllGenderRatios.Add(viewModel);
            }

            model.GameId = this.dataService.GetObjects<Game>("ReleaseDate, Id").Last().Id;

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_pokemon")]
        public IActionResult Pokemon(BasePokemonViewModel newPokemon, IFormFile normalUpload, string normalUrlUpload, IFormFile shinyUpload, string shinyUrlUpload)
        {
            if (!this.ModelState.IsValid)
            {
                BasePokemonViewModel model = new BasePokemonViewModel()
                {
                    AllBaseHappinesses = this.dataService.GetObjects<BaseHappiness>("Happiness"),
                    AllClassifications = this.dataService.GetObjects<Classification>("Name"),
                    AllCaptureRates = this.dataService.GetObjects<CaptureRate>("CatchRate"),
                    AllEggCycles = this.dataService.GetObjects<EggCycle>("CycleCount"),
                    AllExperienceGrowths = this.dataService.GetObjects<ExperienceGrowth>("Name"),
                    AllGenderRatios = new List<GenderRatioViewModel>(),
                    AllGames = this.dataService.GetObjects<Game>("ReleaseDate, Id"),
                };

                foreach (GenderRatio genderRatio in this.dataService.GetObjects<GenderRatio>())
                {
                    GenderRatioViewModel viewModel = new GenderRatioViewModel()
                    {
                        Id = genderRatio.Id,
                    };

                    if (genderRatio.MaleRatio == genderRatio.FemaleRatio && genderRatio.MaleRatio == 0)
                    {
                        viewModel.GenderRatioString = "Genderless";
                    }
                    else if (genderRatio.FemaleRatio == 0)
                    {
                        viewModel.GenderRatioString = string.Concat(genderRatio.MaleRatio, "% Male");
                    }
                    else if (genderRatio.MaleRatio == 0)
                    {
                        viewModel.GenderRatioString = string.Concat(genderRatio.FemaleRatio, "% Female");
                    }
                    else
                    {
                        viewModel.GenderRatioString = string.Concat(genderRatio.MaleRatio, "% Male / ", genderRatio.FemaleRatio, "% Female");
                    }

                    model.AllGenderRatios.Add(viewModel);
                }

                return this.View(model);
            }
            else if (this.dataService.GetObjectByPropertyValue<Pokemon>("PokedexNumber", newPokemon.PokedexNumber) != null)
            {
                BasePokemonViewModel model = new BasePokemonViewModel()
                {
                    AllBaseHappinesses = this.dataService.GetObjects<BaseHappiness>("Happiness"),
                    AllClassifications = this.dataService.GetObjects<Classification>("Name"),
                    AllCaptureRates = this.dataService.GetObjects<CaptureRate>("CatchRate"),
                    AllEggCycles = this.dataService.GetObjects<EggCycle>("CycleCount"),
                    AllExperienceGrowths = this.dataService.GetObjects<ExperienceGrowth>("Name"),
                    AllGenderRatios = new List<GenderRatioViewModel>(),
                    AllGames = this.dataService.GetObjects<Game>("ReleaseDate, Id"),
                };

                foreach (GenderRatio genderRatio in this.dataService.GetObjects<GenderRatio>())
                {
                    GenderRatioViewModel viewModel = new GenderRatioViewModel()
                    {
                        Id = genderRatio.Id,
                    };

                    if (genderRatio.MaleRatio == genderRatio.FemaleRatio && genderRatio.MaleRatio == 0)
                    {
                        viewModel.GenderRatioString = "Genderless";
                    }
                    else if (genderRatio.FemaleRatio == 0)
                    {
                        viewModel.GenderRatioString = string.Concat(genderRatio.MaleRatio, "% Male");
                    }
                    else if (genderRatio.MaleRatio == 0)
                    {
                        viewModel.GenderRatioString = string.Concat(genderRatio.FemaleRatio, "% Female");
                    }
                    else
                    {
                        viewModel.GenderRatioString = string.Concat(genderRatio.MaleRatio, "% Male / ", genderRatio.FemaleRatio, "% Female");
                    }

                    model.AllGenderRatios.Add(viewModel);
                }

                this.ModelState.AddModelError("Pokedex Number", "Pokedex Number already exists.");
                return this.View(model);
            }

            Game game = this.dataService.GetObjectByPropertyValue<Game>("Id", newPokemon.GameId);

            this.dataService.AddPokemon(newPokemon);

            this.UploadImages(normalUpload, normalUrlUpload, newPokemon);
            this.UploadImages(shinyUpload, shinyUrlUpload, newPokemon);

            this.dataService.AddPokemonGameDetail(new PokemonGameDetail()
            {
                PokemonId = newPokemon.Id,
                GameId = newPokemon.GameId,
            });

            return this.RedirectToAction("Typing", "Add", new { pokemonId = newPokemon.Id, generationId = game.GenerationId });
        }

        [HttpGet]
        [Route("add_alternate_form/{pokemonId:int}")]
        public IActionResult AltForm(int pokemonId)
        {
            Pokemon originalPokemon = this.dataService.GetPokemonById(pokemonId);
            List<Game> games = this.dataService.GetObjects<Game>("ReleaseDate, Id").Where(x => x.ReleaseDate >= originalPokemon.Game.ReleaseDate).ToList();

            AlternateFormViewModel model = new AlternateFormViewModel()
            {
                AllForms = this.dataService.GetObjects<Form>("Name"),
                AllClassifications = this.dataService.GetObjects<Classification>("Name"),
                AllGames = games,
                OriginalPokemon = originalPokemon,
                OriginalPokemonId = originalPokemon.Id,
            };

            model.GameId = this.dataService.GetObjects<Game>("ReleaseDate, Id").Last().Id;

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_alternate_form/{pokemonId:int}")]
        public IActionResult AltForm(AlternateFormViewModel pokemon, IFormFile normalUpload, string normalUrlUpload, IFormFile shinyUpload, string shinyUrlUpload)
        {
            if (!this.ModelState.IsValid)
            {
                Pokemon originalPokemon = this.dataService.GetPokemonById(pokemon.OriginalPokemonId);
                List<Game> games = this.dataService.GetObjects<Game>("ReleaseDate, Id").Where(x => x.ReleaseDate >= originalPokemon.Game.ReleaseDate).ToList();

                AlternateFormViewModel model = new AlternateFormViewModel()
                {
                    AllForms = this.dataService.GetObjects<Form>("Name"),
                    AllClassifications = this.dataService.GetObjects<Classification>("Name"),
                    AllGames = games,
                    OriginalPokemon = originalPokemon,
                    OriginalPokemonId = originalPokemon.Id,
                };

                return this.View(model);
            }

            List<PokemonFormDetail> originalPokemonForms = this.dataService.GetPokemonFormsWithIncomplete(pokemon.OriginalPokemonId);

            foreach (var p in originalPokemonForms)
            {
                if (p.FormId == pokemon.FormId)
                {
                    Pokemon originalPokemon = this.dataService.GetPokemonById(pokemon.OriginalPokemonId);
                    List<Game> games = this.dataService.GetObjects<Game>("ReleaseDate, Id").Where(x => x.ReleaseDate >= originalPokemon.Game.ReleaseDate).ToList();

                    AlternateFormViewModel model = new AlternateFormViewModel()
                    {
                        AllForms = this.dataService.GetObjects<Form>("Name"),
                        AllClassifications = this.dataService.GetObjects<Classification>("Name"),
                        AllGames = games,
                        OriginalPokemon = originalPokemon,
                        OriginalPokemonId = originalPokemon.Id,
                    };

                    this.ModelState.AddModelError("Alternate Form Name", "Original Pokemon already has an alternate form of this type.");
                    return this.View(model);
                }
            }

            Pokemon alternatePokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", pokemon.OriginalPokemonId);

            alternatePokemon.Id = 0;
            alternatePokemon.PokedexNumber = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", pokemon.OriginalPokemonId).PokedexNumber;
            alternatePokemon.Height = pokemon.Height;
            alternatePokemon.Weight = pokemon.Weight;
            alternatePokemon.ExpYield = pokemon.ExpYield;
            alternatePokemon.GameId = pokemon.GameId;
            alternatePokemon.ClassificationId = pokemon.ClassificationId;
            alternatePokemon.IsComplete = false;

            this.dataService.AddPokemon(alternatePokemon);

            this.UploadImages(normalUpload, normalUrlUpload, alternatePokemon);
            this.UploadImages(shinyUpload, shinyUrlUpload, alternatePokemon);

            PokemonEggGroupDetail eggGroups = this.dataService.GetPokemonWithEggGroups(pokemon.OriginalPokemonId).Last();
            PokemonEggGroupDetail alternatePokemonEggGroups = new PokemonEggGroupDetail()
            {
                PrimaryEggGroupId = eggGroups.PrimaryEggGroupId,
                SecondaryEggGroupId = eggGroups.SecondaryEggGroupId,
                PokemonId = alternatePokemon.Id,
                GenerationId = this.dataService.GetObjectByPropertyValue<Game>("Id", alternatePokemon.GameId).GenerationId,
            };
            this.dataService.AddPokemonEggGroups(alternatePokemonEggGroups);

            PokemonFormDetail alternateForm = new PokemonFormDetail()
            {
                OriginalPokemonId = pokemon.OriginalPokemonId,
                AltFormPokemonId = alternatePokemon.Id,
                FormId = pokemon.FormId,
            };
            this.dataService.AddPokemonFormDetails(alternateForm);

            this.dataService.AddPokemonGameDetail(new PokemonGameDetail()
            {
                PokemonId = alternatePokemon.Id,
                GameId = alternatePokemon.GameId,
            });

            return this.RedirectToAction("Typing", "Add", new { pokemonId = alternatePokemon.Id, generationId = alternatePokemonEggGroups.GenerationId });
        }

        [HttpGet]
        [Route("add_typing/{pokemonId:int}/{generationId:int}")]
        public IActionResult Typing(int pokemonId, int generationId)
        {
            PokemonTypingViewModel model = new PokemonTypingViewModel()
            {
                AllTypes = this.dataService.GetObjects<DataAccess.Models.Type>("Name"),
                PokemonId = pokemonId,
                Pokemon = this.dataService.GetPokemonById(pokemonId),
                GenerationId = generationId,
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_typing/{pokemonId:int}/{generationId:int}")]
        public IActionResult Typing(PokemonTypingViewModel typing)
        {
            if (!this.ModelState.IsValid)
            {
                PokemonTypingViewModel model = new PokemonTypingViewModel()
                {
                    AllTypes = this.dataService.GetObjects<DataAccess.Models.Type>("Name"),
                    PokemonId = typing.PokemonId,
                    Pokemon = this.dataService.GetPokemonById(typing.PokemonId),
                    GenerationId = typing.GenerationId,
                };

                return this.View(model);
            }

            this.dataService.AddPokemonTyping(typing);

            if (this.dataService.GetPokemonWithAbilities(typing.PokemonId) == null && !this.dataService.GetPokemonById(typing.PokemonId).IsComplete)
            {
                return this.RedirectToAction("Abilities", "Add", new { pokemonId = typing.PokemonId, generationId = typing.GenerationId });
            }
            else
            {
                return this.RedirectToAction("Pokemon", "Admin");
            }
        }

        [HttpGet]
        [Route("add_abilities/{pokemonId:int}/{generationId:int}")]
        public IActionResult Abilities(int pokemonId, int generationId)
        {
            PokemonAbilitiesViewModel model = new PokemonAbilitiesViewModel()
            {
                AllAbilities = this.dataService.GetObjects<Ability>("Name"),
                PokemonId = pokemonId,
                Pokemon = this.dataService.GetPokemonById(pokemonId),
                GenerationId = generationId,
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_abilities/{pokemonId:int}/{generationId:int}")]
        public IActionResult Abilities(PokemonAbilitiesViewModel abilities)
        {
            if (!this.ModelState.IsValid)
            {
                PokemonAbilitiesViewModel model = new PokemonAbilitiesViewModel()
                {
                    AllAbilities = this.dataService.GetObjects<Ability>("Name"),
                    PokemonId = abilities.PokemonId,
                    Pokemon = this.dataService.GetPokemonById(abilities.PokemonId),
                    GenerationId = abilities.GenerationId,
                };

                return this.View(model);
            }

            this.dataService.AddPokemonAbilities(abilities);

            if (this.dataService.GetPokemonWithEggGroups(abilities.PokemonId) == null && !this.dataService.GetPokemonById(abilities.PokemonId).IsComplete)
            {
                return this.RedirectToAction("EggGroups", "Add", new { pokemonId = abilities.PokemonId, generationId = abilities.GenerationId });
            }
            else if (this.dataService.CheckIfAltForm(abilities.PokemonId) && this.dataService.GetPokemonBaseStats(abilities.PokemonId, abilities.GenerationId) == null && !this.dataService.GetPokemonById(abilities.PokemonId).IsComplete)
            {
                return this.RedirectToAction("BaseStats", "Add", new { pokemonId = abilities.PokemonId, generationId = abilities.GenerationId });
            }
            else
            {
                return this.RedirectToAction("Pokemon", "Admin");
            }
        }

        [HttpGet]
        [Route("add_special_event_ability/{pokemonId:int}/{generationId:int}")]
        public IActionResult SpecialEventAbility(int pokemonId, int generationId)
        {
            SpecialEventAbilityViewModel model = new SpecialEventAbilityViewModel()
            {
                AllAbilities = this.dataService.GetObjects<Ability>("Name"),
                PokemonId = pokemonId,
                GenerationId = generationId,
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_special_event_ability/{pokemonId:int}/{generationId:int}")]
        public IActionResult SpecialEventAbility(SpecialEventAbilityViewModel ability, int generationId)
        {
            if (!this.ModelState.IsValid)
            {
                SpecialEventAbilityViewModel model = new SpecialEventAbilityViewModel()
                {
                    AllAbilities = this.dataService.GetObjects<Ability>("Name"),
                    PokemonId = ability.PokemonId,
                };

                return this.View(model);
            }

            PokemonAbilityDetail pokemonAbilities = this.dataService.GetPokemonWithAbilitiesNoIncludes(ability.PokemonId, generationId);

            pokemonAbilities.SpecialEventAbilityId = ability.AbilityId;

            this.dataService.UpdatePokemonAbilityDetail(pokemonAbilities);

            return this.RedirectToAction("Pokemon", "Admin");
        }

        [HttpGet]
        [Route("add_egg_groups/{pokemonId:int}/{generationId:int}")]
        public IActionResult EggGroups(int pokemonId, int generationId)
        {
            PokemonEggGroupsViewModel model = new PokemonEggGroupsViewModel()
            {
                AllEggGroups = this.dataService.GetObjects<EggGroup>("Name"),
                PokemonId = pokemonId,
                Pokemon = this.dataService.GetPokemonById(pokemonId),
                GenerationId = generationId,
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_egg_groups/{pokemonId:int}/{generationId:int}")]
        public IActionResult EggGroups(PokemonEggGroupsViewModel eggGroups)
        {
            if (!this.ModelState.IsValid)
            {
                PokemonEggGroupsViewModel model = new PokemonEggGroupsViewModel()
                {
                    AllEggGroups = this.dataService.GetObjects<EggGroup>("Name"),
                    PokemonId = eggGroups.PokemonId,
                    Pokemon = this.dataService.GetPokemonById(eggGroups.PokemonId),
                    GenerationId = eggGroups.GenerationId,
                };

                return this.View(model);
            }

            this.dataService.AddPokemonEggGroups(eggGroups);

            if (this.dataService.GetPokemonBaseStats(eggGroups.PokemonId, eggGroups.GenerationId) == null && !this.dataService.GetPokemonById(eggGroups.PokemonId).IsComplete)
            {
                return this.RedirectToAction("BaseStats", "Add", new { pokemonId = eggGroups.PokemonId, generationId = eggGroups.GenerationId });
            }
            else
            {
                return this.RedirectToAction("Pokemon", "Admin");
            }
        }

        [HttpGet]
        [Route("add_base_stats/{pokemonId:int}/{generationId:int}")]
        public IActionResult BaseStats(int pokemonId, int generationId)
        {
            BaseStat model = new BaseStat()
            {
                PokemonId = pokemonId,
                Pokemon = this.dataService.GetPokemonById(pokemonId),
                GenerationId = generationId,
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_base_stats/{pokemonId:int}/{generationId:int}")]
        public IActionResult BaseStats(BaseStat baseStat)
        {
            if (!this.ModelState.IsValid)
            {
                BaseStat model = new BaseStat()
                {
                    PokemonId = baseStat.PokemonId,
                    Pokemon = this.dataService.GetPokemonById(baseStat.PokemonId),
                    GenerationId = baseStat.GenerationId,
                };

                return this.View(model);
            }

            this.dataService.AddPokemonBaseStat(baseStat);

            if (this.dataService.GetPokemonEVYields(baseStat.PokemonId, baseStat.GenerationId) == null && !this.dataService.GetPokemonById(baseStat.PokemonId).IsComplete)
            {
                return this.RedirectToAction("EVYields", "Add", new { pokemonId = baseStat.PokemonId, generationId = baseStat.GenerationId });
            }
            else
            {
                return this.RedirectToAction("Pokemon", "Admin");
            }
        }

        [HttpGet]
        [Route("add_ev_yields/{pokemonId:int}/{generationId:int}")]
        public IActionResult EVYields(int pokemonId, int generationId)
        {
            EVYield model = new EVYield()
            {
                PokemonId = pokemonId,
                Pokemon = this.dataService.GetPokemonById(pokemonId),
                GenerationId = generationId,
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_ev_yields/{pokemonId:int}/{generationId:int}")]
        public IActionResult EVYields(EVYield evYield)
        {
            if (!this.ModelState.IsValid)
            {
                EVYield model = new EVYield()
                {
                    PokemonId = evYield.PokemonId,
                    Pokemon = this.dataService.GetPokemonById(evYield.PokemonId),
                };

                return this.View(model);
            }

            this.dataService.AddPokemonEVYield(evYield);

            if (this.User.IsInRole("Owner") && !this.dataService.GetPokemonById(evYield.PokemonId).IsComplete)
            {
                return this.RedirectToAction("Evolution", "Add", new { pokemonId = evYield.PokemonId, generationId = evYield.GenerationId });
            }
            else
            {
                return this.RedirectToAction("Pokemon", "Admin");
            }
        }

        [HttpGet]
        [Route("set_legendary_type/{pokemonId:int}")]
        public IActionResult PokemonLegendaryDetails(int pokemonId)
        {
            PokemonLegendaryViewModel model = new PokemonLegendaryViewModel()
            {
                AllLegendaryTypes = this.dataService.GetObjects<LegendaryType>("Type"),
                PokemonId = pokemonId,
                Pokemon = this.dataService.GetPokemonById(pokemonId),
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("set_legendary_type/{pokemonId:int}")]
        public IActionResult PokemonLegendaryDetails(PokemonLegendaryViewModel pokemonLegendaryDetails)
        {
            if (!this.ModelState.IsValid)
            {
                PokemonLegendaryViewModel model = new PokemonLegendaryViewModel()
                {
                    AllLegendaryTypes = this.dataService.GetObjects<LegendaryType>("Type"),
                    PokemonId = pokemonLegendaryDetails.PokemonId,
                    Pokemon = this.dataService.GetPokemonById(pokemonLegendaryDetails.PokemonId),
                };

                return this.View(model);
            }

            this.dataService.AddPokemonLegendaryDetails(pokemonLegendaryDetails);

            return this.RedirectToAction("Pokemon", "Admin");
        }

        private async void UploadImages(IFormFile fileUpload, string urlUpload, Pokemon pokemon)
        {
            IFormFile upload;

            if (fileUpload == null && urlUpload != null)
            {
                WebRequest webRequest = WebRequest.CreateHttp(urlUpload);

                using WebResponse webResponse = webRequest.GetResponse();
                Stream stream = webResponse.GetResponseStream();
                MemoryStream memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);

                upload = new FormFile(memoryStream, 0, memoryStream.Length, "image", "image.png");
            }
            else
            {
                upload = fileUpload;
            }

            if (upload != null)
            {
                upload = this.dataService.TrimImage(upload);

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(string.Concat(this.appConfig.FTPUrl, this.appConfig.PokemonImageFTPUrl, pokemon.Id.ToString(), upload.FileName.Substring(upload.FileName.LastIndexOf('.'))));
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(this.appConfig.FTPUsername, this.appConfig.FTPPassword);

                using (Stream requestStream = request.GetRequestStream())
                {
                    await upload.CopyToAsync(requestStream).ConfigureAwait(false);
                }

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
                }

                IFormFile spriteUpload = this.dataService.FormatFavIcon(upload);

                request = (FtpWebRequest)WebRequest.Create(string.Concat(this.appConfig.FTPUrl, this.appConfig.FaviconImageFTPUrl, pokemon.Id.ToString(), ".png"));
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(this.appConfig.FTPUsername, this.appConfig.FTPPassword);

                using (Stream requestStream = request.GetRequestStream())
                {
                    await spriteUpload.CopyToAsync(requestStream).ConfigureAwait(false);
                }

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    System.Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
                }
            }
            else
            {
                WebClient webRequest = new WebClient
                {
                    Credentials = new NetworkCredential(this.appConfig.FTPUsername, this.appConfig.FTPPassword),
                };

                byte[] file = webRequest.DownloadData(string.Concat(this.appConfig.WebUrl, "/images/general/tempPhoto.png"));
                MemoryStream strm = new MemoryStream();
                IFormFile tempUpload = new FormFile(strm, 0, strm.Length, "image", "image.png");

                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(string.Concat(this.appConfig.FTPUrl, this.appConfig.PokemonImageFTPUrl, pokemon.Id.ToString(), ".png"));
                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
                ftpRequest.Credentials = new NetworkCredential(this.appConfig.FTPUsername, this.appConfig.FTPPassword);

                using (Stream requestStream = ftpRequest.GetRequestStream())
                {
                    await tempUpload.CopyToAsync(requestStream).ConfigureAwait(false);
                }

                using FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse();
                Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
            }
        }
    }
}
