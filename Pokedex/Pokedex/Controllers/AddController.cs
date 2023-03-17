using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Pokedex.DataAccess.Models;
using Pokedex.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;

namespace Pokedex.Controllers
{
    /// <summary>
    /// The class that handles all additions to the database.
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
            Pokemon evolutionPokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", pokemonId, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth");
            EvolutionViewModel model = new EvolutionViewModel()
            {
                AllEvolutionMethods = this.dataService.GetObjects<EvolutionMethod>("Name"),
                EvolutionPokemon = evolutionPokemon,
                EvolutionPokemonId = evolutionPokemon.Id,
                GenerationId = generationId,
            };

            List<Pokemon> pokemonList = this.dataService.GetObjects<Pokemon>("PokedexNumber, Id", "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth").Where(x => x.Id != pokemonId).ToList();
            List<Pokemon> altFormsList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form").ConvertAll(x => x.AltFormPokemon);
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
                List<Pokemon> pokemonList = this.dataService.GetObjects<Pokemon>("PokedexNumber, Id", "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth").Where(x => x.Id != evolution.EvolutionPokemonId).ToList();
                List<Pokemon> altFormsList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form").ConvertAll(x => x.AltFormPokemon);
                foreach (var pokemon in pokemonList.Where(x => altFormsList.Any(y => y.Id == x.Id)))
                {
                    pokemon.Name = string.Concat(pokemon.Name, " (", this.dataService.GetPokemonFormName(pokemon.Id), ")");
                }

                model.AllPokemon = pokemonList;

                return this.View(model);
            }

            this.dataService.AddObject(evolution);

            return this.RedirectToAction("ReviewPokemon", "Owner", new { pokemonId = evolution.EvolutionPokemonId });
        }

        /// <summary>
        /// Adds a generation.
        /// </summary>
        /// <returns>The view to add the generation.</returns>
        [Route("add_generation")]
        public IActionResult Generation()
        {
            this.dataService.AddObject(new DataAccess.Models.Generation());

            return this.RedirectToAction("Generations", "Owner");
        }

        /// <summary>
        /// Adds a region.
        /// </summary>
        /// <returns>The view to add the region.</returns>
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

        /// <summary>
        /// Adds a region.
        /// </summary>
        /// <param name="region">The region being added.</param>
        /// <returns>The view to the region admin page.</returns>
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

            this.dataService.AddObject(region);

            return this.RedirectToAction("Regions", "Owner");
        }

        /// <summary>
        /// Adds a form item.
        /// </summary>
        /// <returns>The view to add the form item.</returns>
        [HttpGet]
        [Route("add_form_item")]
        public IActionResult FormItem()
        {
            FormItemViewModel model = new FormItemViewModel();
            List<Pokemon> altForms = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, OriginalPokemon, Form").Where(x => x.Form.NeedsItem).ToList().ConvertAll(x => x.AltFormPokemon);

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

        /// <summary>
        /// Adds a form item.
        /// </summary>
        /// <param name="formItem">The form item being added.</param>
        /// <returns>The view to the form item admin page.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_form_item")]
        public IActionResult FormItem(FormItem formItem)
        {
            if (!this.ModelState.IsValid)
            {
                List<Pokemon> pokemonList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form").Select(x => x.AltFormPokemon).OrderBy(x => x.PokedexNumber).ToList();

                foreach (var p in pokemonList)
                {
                    p.Name = string.Concat(p.Name, " (", this.dataService.GetPokemonFormName(p.Id), ")");
                }

                FormItemViewModel model = new FormItemViewModel()
                {
                    AllPokemon = pokemonList,
                };

                return this.View(model);
            }

            this.dataService.AddObject(formItem);

            return this.RedirectToAction("FormItems", "Owner");
        }

        /// <summary>
        /// Adds a form group.
        /// </summary>
        /// <returns>The view to add the form group.</returns>
        [HttpGet]
        [Route("add_form_group")]
        public IActionResult FormGroup()
        {
            return this.View();
        }

        /// <summary>
        /// Adds a form group.
        /// </summary>
        /// <param name="formGroup">The form group being added.</param>
        /// <returns>The view to the form group admin page.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_form_group")]
        public IActionResult FormGroup(FormGroup formGroup)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this.dataService.AddObject(formGroup);

            return this.RedirectToAction("FormGroups", "Owner");
        }

        /// <summary>
        /// Adds a type.
        /// </summary>
        /// <returns>The view to add the type.</returns>
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

        /// <summary>
        /// Adds a type.
        /// </summary>
        /// <param name="type">The type being added.</param>
        /// <returns>The view for the type admin page.</returns>
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

            this.dataService.AddObject(type);

            return this.RedirectToAction("Types", "Owner");
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

            this.dataService.AddObject(eggCycle);

            return this.RedirectToAction("EggCycles", "Owner");
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

            this.dataService.AddObject(experienceGrowth);

            return this.RedirectToAction("ExperienceGrowths", "Owner");
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

            this.dataService.AddObject(genderRatio);

            return this.RedirectToAction("GenderRatios", "Owner");
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

            this.dataService.AddObject(legendaryType);

            return this.RedirectToAction("LegendaryTypes", "Owner");
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

            this.dataService.AddObject(eggGroup);

            return this.RedirectToAction("EggGroups", "Owner");
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
            List<Classification> classifications = this.dataService.GetObjects<Classification>();
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            classification.Name = textInfo.ToTitleCase(classification.Name);

            if (classification.Name.Contains("pokemon"))
            {
                classification.Name = classification.Name.Replace("pokemon", "Pokémon");
            }
            else if (classification.Name.Contains("Pokemon"))
            {
                classification.Name = classification.Name.Replace("Pokemon", "Pokémon");
            }
            else if (classification.Name.Contains("pokémon"))
            {
                classification.Name = classification.Name.Replace("pokémon", "Pokémon");
            }
            else if (!classification.Name.Contains("Pokémon"))
            {
                classification.Name = string.Concat(classification.Name.Trim(), " Pokémon");
            }

            if (!this.ModelState.IsValid || classifications.Find(x => x.Name == classification.Name) != null)
            {
                return this.View();
            }

            this.dataService.AddObject(classification);

            return this.RedirectToAction("Classifications", "Owner");
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

            this.dataService.AddObject(nature);

            return this.RedirectToAction("Natures", "Owner");
        }

        [HttpGet]
        [Route("add_pokeball")]
        public IActionResult Pokeball()
        {
            PokeballViewModel model = new PokeballViewModel()
            {
                AllGenerations = this.dataService.GetObjects<Generation>(),
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_pokeball")]
        public IActionResult Pokeball(Pokeball pokeball, IFormFile officialUpload, string officialUrlUpload)
        {
            if (!this.ModelState.IsValid)
            {
                PokeballViewModel model = new PokeballViewModel()
                {
                    AllGenerations = this.dataService.GetObjects<Generation>(),
                };

                return this.View(model);
            }

            this.dataService.AddObject(pokeball);

            this.dataService.UploadImages(officialUpload, officialUrlUpload, pokeball.Id, this.appConfig, "pokeball");

            return this.RedirectToAction("Pokeballs", "Owner");
        }

        [HttpGet]
        [Route("add_mark")]
        public IActionResult Mark()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_mark")]
        public IActionResult Mark(Mark mark, IFormFile officialUpload, string officialUrlUpload)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this.dataService.AddObject(mark);

            this.dataService.UploadImages(officialUpload, officialUrlUpload, mark.Id, this.appConfig, "mark");

            return this.RedirectToAction("Marks", "Owner");
        }

        [HttpGet]
        [Route("add_hunting_method")]
        public IActionResult HuntingMethod()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_hunting_method")]
        public IActionResult HuntingMethod(HuntingMethod huntingMethod)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this.dataService.AddObject(huntingMethod);

            return this.RedirectToAction("HuntingMethods", "Owner");
        }

        [HttpGet]
        [Route("add_form")]
        public IActionResult Form()
        {
            FormModelViewModel model = new FormModelViewModel()
            {
                AllFormGroups = this.dataService.GetObjects<FormGroup>(),
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_form")]
        public IActionResult Form(Form form)
        {
            if (!this.ModelState.IsValid)
            {
                FormModelViewModel model = new FormModelViewModel()
                {
                    AllFormGroups = this.dataService.GetObjects<FormGroup>(),
                };

                return this.View(model);
            }

            this.dataService.AddObject(form);

            return this.RedirectToAction("Forms", "Owner");
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

            this.dataService.AddObject(evolutionMethod);

            return this.RedirectToAction("EvolutionMethods", "Owner");
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

            this.dataService.AddObject(captureRate);

            return this.RedirectToAction("CaptureRates", "Owner");
        }

        [HttpGet]
        [Route("add_ability")]
        public IActionResult Ability()
        {
            AbilityAdminViewModel model = new AbilityAdminViewModel()
            {
                AllGenerations = this.dataService.GetObjects<Generation>(),
            };
            return this.View(model);
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

            this.dataService.AddObject(ability);

            return this.RedirectToAction("Abilities", "Owner");
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

            this.dataService.AddObject(baseHappiness);

            return this.RedirectToAction("BaseHappinesses", "Owner");
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

            this.dataService.AddObject(game);

            foreach (var r in regionIds)
            {
                GameRegionDetail gameRegionDetail = new GameRegionDetail()
                {
                    GameId = game.Id,
                    RegionId = r,
                };

                this.dataService.AddObject(gameRegionDetail);
            }

            return this.RedirectToAction("Games", "Owner");
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

            this.dataService.AddObject(pokemonCaptureRate);

            if (this.dataService.GetObjects<PokemonEggGroupDetail>(includes: "Pokemon, PrimaryEggGroup, SecondaryEggGroup", whereProperty: "PokemonId", wherePropertyValue: pokemonCaptureRate.PokemonId).Count() == 0 && !this.dataService.GetObjectByPropertyValue<Pokemon>("Id", pokemonCaptureRate.PokemonId, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth").IsComplete)
            {
                return this.RedirectToAction("EggGroups", "Add", new { pokemonId = pokemonCaptureRate.PokemonId, generationId = pokemonCaptureRate.GenerationId });
            }
            else
            {
                return this.RedirectToAction("Pokemon", "Owner");
            }
        }

        [HttpGet]
        [Route("add_pokemon_base_happiness/{pokemonId:int}/{generationId:int}")]
        public IActionResult BaseHappinesses(int pokemonId, int generationId)
        {
            PokemonBaseHappinessViewModel model = new PokemonBaseHappinessViewModel()
            {
                PokemonId = pokemonId,
                GenerationId = generationId,
                AllBaseHappinesses = this.dataService.GetObjects<BaseHappiness>("Happiness"),
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_pokemon_base_happiness/{pokemonId:int}/{generationId:int}")]
        public IActionResult BaseHappinesses(PokemonBaseHappinessDetail pokemonBaseHappiness)
        {
            if (!this.ModelState.IsValid)
            {
                PokemonBaseHappinessViewModel model = new PokemonBaseHappinessViewModel()
                {
                    PokemonId = pokemonBaseHappiness.PokemonId,
                    GenerationId = pokemonBaseHappiness.GenerationId,
                    AllBaseHappinesses = this.dataService.GetObjects<BaseHappiness>("Happiness"),
                };

                return this.View(model);
            }

            this.dataService.AddObject(pokemonBaseHappiness);

            if (this.dataService.GetObjects<PokemonCaptureRateDetail>(includes: "Pokemon, CaptureRate", whereProperty: "PokemonId", wherePropertyValue: pokemonBaseHappiness.PokemonId).Count() == 0 && !this.dataService.GetObjectByPropertyValue<Pokemon>("Id", pokemonBaseHappiness.PokemonId, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth").IsComplete)
            {
                return this.RedirectToAction("CaptureRates", "Add", new { pokemonId = pokemonBaseHappiness.PokemonId, generationId = pokemonBaseHappiness.GenerationId });
            }
            else
            {
                return this.RedirectToAction("Pokemon", "Owner");
            }
        }

        [HttpGet]
        [Route("add_battle_item")]
        public IActionResult BattleItem()
        {
            List<Pokemon> pokemonList = this.dataService.GetAllPokemon();
            List<Pokemon> altFormsList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form").ConvertAll(x => x.AltFormPokemon);
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

            this.dataService.AddObject(battleItem);

            return this.RedirectToAction("BattleItems", "Owner");
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
                    viewModel.GenderRatioString = "Gender Unknown";
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
        public IActionResult Pokemon(BasePokemonViewModel newPokemon, IFormFile officialUpload, string officialUrlUpload)
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
                        viewModel.GenderRatioString = "Gender Unknown";
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
                        viewModel.GenderRatioString = "Gender Unknown";
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

            this.dataService.AddObject(newPokemon);

            this.dataService.UploadImages(officialUpload, officialUrlUpload, newPokemon.Id, this.appConfig, "2d");

            this.dataService.AddObject(new PokemonGameDetail()
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
            Pokemon originalPokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", pokemonId, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth");
            List<Game> games = this.dataService.GetObjects<Game>("ReleaseDate, Id");

            AlternateFormViewModel model = new AlternateFormViewModel
            {
                AllForms = this.dataService.GetObjects<Form>("Name"),
                AllClassifications = this.dataService.GetObjects<Classification>("Name"),
                AllGames = games.Where(x => x.ReleaseDate >= originalPokemon.Game.ReleaseDate).ToList(),
                OriginalPokemon = originalPokemon,
                OriginalPokemonId = originalPokemon.Id,
                GameId = games.Last().Id,
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_alternate_form/{pokemonId:int}")]
        public IActionResult AltForm(AlternateFormViewModel pokemon, IFormFile officialUpload, string officialUrlUpload)
        {
            if (!this.ModelState.IsValid)
            {
                Pokemon originalPokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", pokemon.OriginalPokemonId, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth");
                List<Game> games = this.dataService.GetObjects<Game>("ReleaseDate, Id");

                AlternateFormViewModel model = new AlternateFormViewModel()
                {
                    AllForms = this.dataService.GetObjects<Form>("Name"),
                    AllClassifications = this.dataService.GetObjects<Classification>("Name"),
                    AllGames = games.Where(x => x.ReleaseDate >= originalPokemon.Game.ReleaseDate).ToList(),
                    OriginalPokemon = originalPokemon,
                    OriginalPokemonId = originalPokemon.Id,
                    GameId = games.Last().Id,
                };

                return this.View(model);
            }

            List<PokemonFormDetail> originalPokemonForms = this.dataService.GetObjects<PokemonFormDetail>("AltFormPokemonId", "Form, OriginalPokemon, AltFormPokemon", "OriginalPokemonId", pokemon.OriginalPokemonId);

            foreach (var p in originalPokemonForms)
            {
                if (p.FormId == pokemon.FormId)
                {
                    Pokemon originalPokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", pokemon.OriginalPokemonId, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth");
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
            alternatePokemon.GameId = pokemon.GameId;
            alternatePokemon.ClassificationId = pokemon.ClassificationId;
            alternatePokemon.IsComplete = false;

            this.dataService.AddObject(alternatePokemon);

            this.dataService.UploadImages(officialUpload, officialUrlUpload, alternatePokemon.Id, this.appConfig, "2d");

            PokemonEggGroupDetail eggGroups = this.dataService.GetObjects<PokemonEggGroupDetail>(includes: "Pokemon, PrimaryEggGroup, SecondaryEggGroup", whereProperty: "PokemonId", wherePropertyValue: pokemon.OriginalPokemonId).Last();
            PokemonEggGroupDetail alternatePokemonEggGroups = new PokemonEggGroupDetail()
            {
                PrimaryEggGroupId = eggGroups.PrimaryEggGroupId,
                SecondaryEggGroupId = eggGroups.SecondaryEggGroupId,
                PokemonId = alternatePokemon.Id,
                GenerationId = this.dataService.GetObjectByPropertyValue<Game>("Id", alternatePokemon.GameId).GenerationId,
            };
            this.dataService.AddObject(alternatePokemonEggGroups);

            PokemonFormDetail alternateForm = new PokemonFormDetail()
            {
                OriginalPokemonId = pokemon.OriginalPokemonId,
                AltFormPokemonId = alternatePokemon.Id,
                FormId = pokemon.FormId,
            };
            this.dataService.AddObject(alternateForm);

            this.dataService.AddObject(new PokemonGameDetail()
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
                Pokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", pokemonId, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth"),
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
                    Pokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", typing.PokemonId, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth"),
                    GenerationId = typing.GenerationId,
                };

                return this.View(model);
            }

            this.dataService.AddObject(typing);

            if (this.dataService.GetObjects<PokemonAbilityDetail>(includes: "Pokemon, PrimaryAbility, SecondaryAbility, HiddenAbility, SpecialEventAbility", whereProperty: "PokemonId", wherePropertyValue: typing.PokemonId).Count() == 0 && !this.dataService.GetObjectByPropertyValue<Pokemon>("Id", typing.PokemonId, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth").IsComplete)
            {
                return this.RedirectToAction("Abilities", "Add", new { pokemonId = typing.PokemonId, generationId = typing.GenerationId });
            }
            else
            {
                return this.RedirectToAction("Pokemon", "Owner");
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
                Pokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", pokemonId, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth"),
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
                    Pokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", abilities.PokemonId, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth"),
                    GenerationId = abilities.GenerationId,
                };

                return this.View(model);
            }

            this.dataService.AddObject(abilities);

            if (this.dataService.GetObjects<PokemonBaseHappinessDetail>(includes: "Pokemon, BaseHappiness", whereProperty: "PokemonId", wherePropertyValue: abilities.PokemonId).Count() == 0 && !this.dataService.GetObjectByPropertyValue<Pokemon>("Id", abilities.PokemonId, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth").IsComplete)
            {
                return this.RedirectToAction("BaseHappinesses", "Add", new { pokemonId = abilities.PokemonId, generationId = abilities.GenerationId });
            }
            else if (this.dataService.CheckIfAltForm(abilities.PokemonId) && this.dataService.GetPokemonBaseStats(abilities.PokemonId, abilities.GenerationId) == null && !this.dataService.GetObjectByPropertyValue<Pokemon>("Id", abilities.PokemonId, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth").IsComplete)
            {
                return this.RedirectToAction("BaseStats", "Add", new { pokemonId = abilities.PokemonId, generationId = abilities.GenerationId });
            }
            else
            {
                return this.RedirectToAction("Pokemon", "Owner");
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

            PokemonAbilityDetail pokemonAbilities = this.dataService.GetObjects<PokemonAbilityDetail>(includes: "Pokemon").FirstOrDefault(x => x.PokemonId == ability.PokemonId && x.GenerationId == generationId);

            pokemonAbilities.SpecialEventAbilityId = ability.AbilityId;

            this.dataService.UpdateObject(pokemonAbilities);

            return this.RedirectToAction("Pokemon", "Owner");
        }

        [HttpGet]
        [Route("add_egg_groups/{pokemonId:int}/{generationId:int}")]
        public IActionResult EggGroups(int pokemonId, int generationId)
        {
            PokemonEggGroupsViewModel model = new PokemonEggGroupsViewModel()
            {
                AllEggGroups = this.dataService.GetObjects<EggGroup>("Name"),
                PokemonId = pokemonId,
                Pokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", pokemonId, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth"),
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
                    Pokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", eggGroups.PokemonId, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth"),
                    GenerationId = eggGroups.GenerationId,
                };

                return this.View(model);
            }

            this.dataService.AddObject(eggGroups);

            if (this.dataService.GetPokemonBaseStats(eggGroups.PokemonId, eggGroups.GenerationId) == null && !this.dataService.GetObjectByPropertyValue<Pokemon>("Id", eggGroups.PokemonId, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth").IsComplete)
            {
                return this.RedirectToAction("BaseStats", "Add", new { pokemonId = eggGroups.PokemonId, generationId = eggGroups.GenerationId });
            }
            else
            {
                return this.RedirectToAction("Pokemon", "Owner");
            }
        }

        [HttpGet]
        [Route("add_base_stats/{pokemonId:int}/{generationId:int}")]
        public IActionResult BaseStats(int pokemonId, int generationId)
        {
            BaseStat model = new BaseStat()
            {
                PokemonId = pokemonId,
                Pokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", pokemonId, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth"),
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
                    Pokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", baseStat.PokemonId, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth"),
                    GenerationId = baseStat.GenerationId,
                };

                return this.View(model);
            }

            this.dataService.AddObject(baseStat);

            if (this.dataService.GetPokemonEVYields(baseStat.PokemonId, baseStat.GenerationId) == null && !this.dataService.GetObjectByPropertyValue<Pokemon>("Id", baseStat.PokemonId, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth").IsComplete)
            {
                return this.RedirectToAction("EVYields", "Add", new { pokemonId = baseStat.PokemonId, generationId = baseStat.GenerationId });
            }
            else
            {
                return this.RedirectToAction("Pokemon", "Owner");
            }
        }

        [HttpGet]
        [Route("add_ev_yields/{pokemonId:int}/{generationId:int}")]
        public IActionResult EVYields(int pokemonId, int generationId)
        {
            EVYield model = new EVYield()
            {
                PokemonId = pokemonId,
                Pokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", pokemonId, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth"),
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
                    Pokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", evYield.PokemonId, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth"),
                };

                return this.View(model);
            }

            this.dataService.AddObject(evYield);

            if (this.User.IsInRole("Owner") && !this.dataService.GetObjectByPropertyValue<Pokemon>("Id", evYield.PokemonId, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth").IsComplete)
            {
                return this.RedirectToAction("Evolution", "Add", new { pokemonId = evYield.PokemonId, generationId = evYield.GenerationId });
            }
            else
            {
                return this.RedirectToAction("Pokemon", "Owner");
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
                Pokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", pokemonId, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth"),
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
                    Pokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", pokemonLegendaryDetails.PokemonId, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth"),
                };

                return this.View(model);
            }

            this.dataService.AddObject(pokemonLegendaryDetails);

            return this.RedirectToAction("Pokemon", "Owner");
        }
    }
}
