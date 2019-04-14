using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using PokemonDatabase.Models;
using PokemonDatabase.DataAccess.Models;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace PokemonDatabase.Controllers
{
    [Authorize(Roles = "Admin,Owner"), Route("admin")]
    public class AdminController : Controller
    {
        private readonly AppConfig _appConfig;

        private readonly DataService _dataService;

        public AdminController(IOptions<AppConfig> appConfig, DataContext dataContext)
        {
            _dataService = new DataService(dataContext);
            _appConfig = appConfig.Value;
        }

        [Route("")]
        public IActionResult Index()
        {
            ViewBag.ApplicationName = _appConfig.AppName;
            ViewBag.ApplicationVersion = _appConfig.AppVersion;

            return View();
        }

        [Route("pokemon")]
        public IActionResult Pokemon()
        {
            List<Pokemon> model = _dataService.GetAllPokemon().Where(x => x.Id.IndexOf('-') == -1).ToList();

            return View(model);
        }

        [Route("generation")]
        public IActionResult Generations()
        {
            GenerationViewModel model = new GenerationViewModel(){
                AllGenerations = _dataService.GetGenerations(),
                AllPokemon = _dataService.GetAllPokemon()
            };

            return View(model);
        }

        [Route("type")]
        public IActionResult Types()
        {
            TypeViewModel model = new TypeViewModel(){
                AllTypes = _dataService.GetTypesWithArchive(),
                AllPokemon = _dataService.GetAllPokemonWithTypes()
            };

            return View(model);
        }

        [Route("ability")]
        public IActionResult Abilities()
        {
            AbilityViewModel model = new AbilityViewModel(){
                AllAbilities = _dataService.GetAbilities(),
                AllPokemon = _dataService.GetAllPokemonWithAbilities()
            };

            return View(model);
        }

        [Route("egg-group")]
        public IActionResult EggGroups()
        {
            EggGroupViewModel model = new EggGroupViewModel(){
                AllEggGroups = _dataService.GetEggGroups(),
                AllPokemon = _dataService.GetAllPokemonWithEggGroups()
            };

            return View(model);
        }

        [Route("classification")]
        public IActionResult Classifications()
        {
            ClassificationViewModel model = new ClassificationViewModel(){
                AllClassifications = _dataService.GetClassificationsWithArchive(),
                AllPokemon = _dataService.GetAllPokemonWithClassifications()
            };

            return View(model);
        }

        [HttpGet, Route("add-evolution")]
        public IActionResult AddEvolution()
        {
            EvolutionViewModel model = new EvolutionViewModel(){
                AllEvolutionMethods = _dataService.GetEvolutionMethods()
            };
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, Route("add-evolution")]
        public IActionResult AddEvolution(Evolution evolution)
        {
            if (!ModelState.IsValid)
            {
                EvolutionViewModel model = new EvolutionViewModel(){
                    AllEvolutionMethods = _dataService.GetEvolutionMethods()
                };
                return View(model);
            }

            _dataService.AddEvolution(evolution);

            return RedirectToAction("Pokemon");
        }

        [HttpGet, Route("add-generation")]
        public IActionResult AddGeneration()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken, Route("add-generation")]
        public IActionResult AddGeneration(Generation generation)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            _dataService.AddGeneration(generation);

            return RedirectToAction("Generations");
        }

        [HttpGet, Route("add-type")]
        public IActionResult AddType()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken, Route("add-type")]
        public IActionResult AddType(Type type)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            _dataService.AddType(type);

            return RedirectToAction("Types");
        }

        [HttpGet, Route("add-egg-group")]
        public IActionResult AddEggGroup()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken, Route("add-egg-group")]
        public IActionResult AddEggGroup(EggGroup eggGroup)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            _dataService.AddEggGroup(eggGroup);

            return RedirectToAction("EggGroups");
        }

        [HttpGet, Route("add-classification")]
        public IActionResult AddClassification()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken, Route("add-classification")]
        public IActionResult AddClassification(Classification classification)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            _dataService.AddClassification(classification);

            return RedirectToAction("Classifications");
        }

        [HttpGet, Route("add-ability")]
        public IActionResult AddAbility()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken, Route("add-ability")]
        public IActionResult AddAbility(Ability ability)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            _dataService.AddAbility(ability);

            return RedirectToAction("Abilities");
        }

        [HttpGet, Route("add-pokemon")]
        public IActionResult AddPokemon()
        {
            BasePokemonViewModel model = new BasePokemonViewModel(){
                AllBaseHappinesses = _dataService.GetBaseHappinesses(),
                AllClassifications = _dataService.GetClassifications(),
                AllCaptureRates = _dataService.GetCaptureRates(),
                AllEggCycles = _dataService.GetEggCycles(),
                AllExperienceGrowths = _dataService.GetExperienceGrowths(),
                AllGenderRatios = _dataService.GetGenderRatios(),
                AllGenerations = _dataService.GetGenerations()
            };

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, Route("add-pokemon")]
        public IActionResult AddPokemon(BasePokemonViewModel pokemon)
        {
            int pokedexNumber;
            if (!ModelState.IsValid)
            {
                BasePokemonViewModel model = new BasePokemonViewModel(){
                    AllBaseHappinesses = _dataService.GetBaseHappinesses(),
                    AllClassifications = _dataService.GetClassifications(),
                    AllCaptureRates = _dataService.GetCaptureRates(),
                    AllEggCycles = _dataService.GetEggCycles(),
                    AllExperienceGrowths = _dataService.GetExperienceGrowths(),
                    AllGenderRatios = _dataService.GetGenderRatios(),
                    AllGenerations = _dataService.GetGenerations()
                };

                return View(model);
            }
            else if (!System.Int32.TryParse(pokemon.Id, out pokedexNumber))
            {
                BasePokemonViewModel model = new BasePokemonViewModel(){
                    AllBaseHappinesses = _dataService.GetBaseHappinesses(),
                    AllClassifications = _dataService.GetClassifications(),
                    AllCaptureRates = _dataService.GetCaptureRates(),
                    AllEggCycles = _dataService.GetEggCycles(),
                    AllExperienceGrowths = _dataService.GetExperienceGrowths(),
                    AllGenderRatios = _dataService.GetGenderRatios(),
                    AllGenerations = _dataService.GetGenerations()
                };

                ModelState.AddModelError("Pokedex Number", "Pokedex Number must be a number.");
                return View(model);
            }
            else if (_dataService.GetAllPokemon().Exists(x => x.Id == pokemon.Id))
            {
                BasePokemonViewModel model = new BasePokemonViewModel(){
                    AllBaseHappinesses = _dataService.GetBaseHappinesses(),
                    AllClassifications = _dataService.GetClassifications(),
                    AllCaptureRates = _dataService.GetCaptureRates(),
                    AllEggCycles = _dataService.GetEggCycles(),
                    AllExperienceGrowths = _dataService.GetExperienceGrowths(),
                    AllGenderRatios = _dataService.GetGenderRatios(),
                    AllGenerations = _dataService.GetGenerations()
                };

                ModelState.AddModelError("Pokedex Number", "Pokedex Number already exists.");
                return View(model);
            }

            _dataService.AddPokemon(pokemon);

            return RedirectToAction("AddTyping", "Admin", new { pokemonId = pokemon.Id });
        }

        [HttpGet, Route("add-typing/{pokemonId}")]
        public IActionResult AddTyping(string pokemonId)
        {
            PokemonTypingViewModel model = new PokemonTypingViewModel(){
                AllTypes = _dataService.GetTypes(),
                PokemonId = pokemonId
            };

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, Route("add-typing/{pokemonId}")]
        public IActionResult AddTyping(PokemonTypingViewModel typing)
        {
            if (!ModelState.IsValid)
            {
                PokemonTypingViewModel model = new PokemonTypingViewModel(){
                    AllTypes = _dataService.GetTypes(),
                    PokemonId = typing.PokemonId
                };

                return View(model);
            }

            _dataService.AddPokemonTyping(typing);

            return RedirectToAction("AddAbilities", "Admin", new { pokemonId = typing.PokemonId });
        }

        [HttpGet, Route("add-abilities/{pokemonId}")]
        public IActionResult AddAbilities(string pokemonId)
        {
            PokemonAbilitiesViewModel model = new PokemonAbilitiesViewModel(){
                AllAbilities = _dataService.GetAbilities(),
                PokemonId = pokemonId
            };

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, Route("add-abilities/{pokemonId}")]
        public IActionResult AddAbilities(PokemonAbilitiesViewModel abilities)
        {
            if (!ModelState.IsValid)
            {
                PokemonAbilitiesViewModel model = new PokemonAbilitiesViewModel(){
                    AllAbilities = _dataService.GetAbilities(),
                    PokemonId = abilities.PokemonId
                };

                return View(model);
            }

            _dataService.AddPokemonAbilities(abilities);

            return RedirectToAction("AddEggGroups", "Admin", new { pokemonId = abilities.PokemonId });
        }

        [HttpGet, Route("add-egg-groups/{pokemonId}")]
        public IActionResult AddEggGroups(string pokemonId)
        {
            PokemonEggGroupsViewModel model = new PokemonEggGroupsViewModel(){
                AllEggGroups = _dataService.GetEggGroups(),
                PokemonId = pokemonId
            };

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, Route("add-egg-groups/{pokemonId}")]
        public IActionResult AddEggGroups(PokemonEggGroupsViewModel eggGroups)
        {
            if (!ModelState.IsValid)
            {
                PokemonEggGroupsViewModel model = new PokemonEggGroupsViewModel(){
                    AllEggGroups = _dataService.GetEggGroups(),
                    PokemonId = eggGroups.PokemonId
                };

                return View(model);
            }

            _dataService.AddPokemonEggGroups(eggGroups);

            return RedirectToAction("AddBaseStats", "Admin", new { pokemonId = eggGroups.PokemonId });
        }

        [HttpGet, Route("add-base-stats/{pokemonId}")]
        public IActionResult AddBaseStats(string pokemonId)
        {
            BaseStat model = new BaseStat(){
                PokemonId = pokemonId
            };

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, Route("add-base-stats/{pokemonId}")]
        public IActionResult AddBaseStats(BaseStat baseStat)
        {
            if (!ModelState.IsValid)
            {
                BaseStat model = new BaseStat(){
                    PokemonId = baseStat.PokemonId
                };

                return View(model);
            }

            _dataService.AddPokemonBaseStat(baseStat);

            return RedirectToAction("AddEVYields", "Admin", new { pokemonId = baseStat.PokemonId });
        }

        [HttpGet, Route("add-ev-yields/{pokemonId}")]
        public IActionResult AddEVYields(string pokemonId)
        {
            EVYield model = new EVYield(){
                PokemonId = pokemonId
            };

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, Route("add-ev-yields/{pokemonId}")]
        public IActionResult AddEVYields(EVYield evYield)
        {
            if (!ModelState.IsValid)
            {
                EVYield model = new EVYield(){
                    PokemonId = evYield.PokemonId
                };

                return View(model);
            }
            else if (evYield.Health > 3)
            {
                EVYield model = new EVYield(){
                    PokemonId = evYield.PokemonId
                };

                ModelState.AddModelError("EVTotal", "Health must be 3 or less.");
                return View(model);
            }
            else if (evYield.Attack > 3)
            {
                EVYield model = new EVYield(){
                    PokemonId = evYield.PokemonId
                };

                ModelState.AddModelError("EVTotal", "Attack must be 3 or less.");
                return View(model);
            }
            else if (evYield.Defense > 3)
            {
                EVYield model = new EVYield(){
                    PokemonId = evYield.PokemonId
                };

                ModelState.AddModelError("EVTotal", "Defense must be 3 or less.");
                return View(model);
            }
            else if (evYield.SpecialAttack > 3)
            {
                EVYield model = new EVYield(){
                    PokemonId = evYield.PokemonId
                };

                ModelState.AddModelError("EVTotal", "Special Attack must be 3 or less.");
                return View(model);
            }
            else if (evYield.SpecialDefense > 3)
            {
                EVYield model = new EVYield(){
                    PokemonId = evYield.PokemonId
                };

                ModelState.AddModelError("EVTotal", "Special Defense must be 3 or less.");
                return View(model);
            }
            else if (evYield.Speed > 3)
            {
                EVYield model = new EVYield(){
                    PokemonId = evYield.PokemonId
                };

                ModelState.AddModelError("EVTotal", "Speed must be 3 or less.");
                return View(model);
            }
            else if (evYield.EVTotal > 3)
            {
                EVYield model = new EVYield(){
                    PokemonId = evYield.PokemonId
                };

                ModelState.AddModelError("EVTotal", "EV Total must be 3 or less.");
                return View(model);
            }

            _dataService.AddPokemonEVYield(evYield);

            Pokemon pokemon = _dataService.GetPokemonById(evYield.PokemonId);

            return RedirectToAction("Pokemon", "Home", new { Name = pokemon.Name.Replace(' ', '_').ToLower() });
        }

        [HttpGet, Route("edit-generation/{id}")]
        public IActionResult EditGeneration(string id)
        {
            Generation model = _dataService.GetGeneration(id);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, Route("edit-generation/{id}")]
        public IActionResult EditGeneration(Generation generation)
        {
            if (!ModelState.IsValid)
            {
                Generation model = _dataService.GetGeneration(generation.Id);

                return View(model);
            }

            _dataService.UpdateGeneration(generation);

            return RedirectToAction("Generations");
        }

        [HttpGet, Route("edit-type/{id:int}")]
        public IActionResult EditType(int id)
        {
            Type model = _dataService.GetType(id);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, Route("edit-type/{id:int}")]
        public IActionResult EditType(Type type)
        {
            if (!ModelState.IsValid)
            {
                Type model = _dataService.GetType(type.Id);

                return View(model);
            }

            _dataService.UpdateType(type);

            return RedirectToAction("Types");
        }

        [HttpGet, Route("edit-egg-group/{id:int}")]
        public IActionResult EditEggGroup(int id)
        {
            EggGroup model = _dataService.GetEggGroup(id);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, Route("edit-egg-group/{id:int}")]
        public IActionResult EditEggGroup(EggGroup eggGroup)
        {
            if (!ModelState.IsValid)
            {
                EggGroup model = _dataService.GetEggGroup(eggGroup.Id);

                return View(model);
            }

            _dataService.UpdateEggGroup(eggGroup);

            return RedirectToAction("EggGroups");
        }

        [HttpGet, Route("edit-classification/{id:int}")]
        public IActionResult EditClassification(int id)
        {
            Classification model = _dataService.GetClassification(id);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, Route("edit-classification/{id:int}")]
        public IActionResult EditClassification(Classification classification)
        {
            if (!ModelState.IsValid)
            {
                Classification model = _dataService.GetClassification(classification.Id);

                return View(model);
            }

            _dataService.UpdateClassification(classification);

            return RedirectToAction("Classifications");
        }

        [HttpGet, Route("edit-ability/{id:int}")]
        public IActionResult EditAbility(int id)
        {
            Ability model = _dataService.GetAbility(id);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, Route("edit-ability/{id:int}")]
        public IActionResult EditAbility(Ability ability)
        {
            if (!ModelState.IsValid)
            {
                Ability model = _dataService.GetAbility(ability.Id);

                return View(model);
            }

            _dataService.UpdateAbility(ability);

            return RedirectToAction("Abilities");
        }

        [HttpGet, Route("delete-generation/{id}")]
        public IActionResult DeleteGeneration(string id)
        {
            Generation model = _dataService.GetGeneration(id);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, Route("delete-generation/{id}")]
        public IActionResult DeleteGeneration(Generation generation)
        {
            _dataService.DeleteGeneration(generation.Id);

            return RedirectToAction("Generations");
        }

        [HttpGet, Route("delete-type/{id:int}")]
        public IActionResult DeleteType(int id)
        {
            Type model = _dataService.GetType(id);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, Route("delete-type/{id:int}")]
        public IActionResult DeleteType(Type type)
        {
            _dataService.DeleteType(type.Id);

            return RedirectToAction("Types");
        }

        [HttpGet, Route("delete-egg-group/{id:int}")]
        public IActionResult DeleteEggGroup(int id)
        {
            EggGroup model = _dataService.GetEggGroup(id);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, Route("delete-egg-group/{id:int}")]
        public IActionResult DeleteEggGroup(EggGroup eggGroup)
        {
            _dataService.DeleteEggGroup(eggGroup.Id);

            return RedirectToAction("EggGroups");
        }

        [HttpGet, Route("delete-classification/{id:int}")]
        public IActionResult DeleteClassification(int id)
        {
            Classification model = _dataService.GetClassification(id);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, Route("delete-classification/{id:int}")]
        public IActionResult DeleteClassification(Classification classification)
        {
            _dataService.DeleteClassification(classification.Id);

            return RedirectToAction("Classifications");
        }

        [HttpGet, Route("delete-ability/{id:int}")]
        public IActionResult DeleteAbility(int id)
        {
            Ability model = _dataService.GetAbility(id);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, Route("delete-ability/{id:int}")]
        public IActionResult DeleteAbility(Ability ability)
        {
            _dataService.DeleteAbility(ability.Id);

            return RedirectToAction("Abilities");
        }

        [HttpGet, Route("restore-generation/{id}")]
        public IActionResult RestoreGeneration(string id)
        {
            Generation model = _dataService.GetGeneration(id);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, Route("restore-generation/{id}")]
        public IActionResult RestoreGeneration(Generation generation)
        {
            _dataService.RestoreGeneration(generation.Id);

            return RedirectToAction("Generations");
        }

        [HttpGet, Route("restore-type/{id:int}")]
        public IActionResult RestoreType(int id)
        {
            Type model = _dataService.GetType(id);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, Route("restore-type/{id:int}")]
        public IActionResult RestoreType(Type type)
        {
            _dataService.RestoreType(type.Id);

            return RedirectToAction("Types");
        }

        [HttpGet, Route("restore-classification/{id:int}")]
        public IActionResult RestoreClassification(int id)
        {
            Classification model = _dataService.GetClassification(id);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, Route("restore-classification/{id:int}")]
        public IActionResult RestoreType(Classification classification)
        {
            _dataService.RestoreClassification(classification.Id);

            return RedirectToAction("Classifications");
        }

        [HttpGet, Route("restore-ability/{id:int}")]
        public IActionResult RestoreAbility(int id)
        {
            Ability model = _dataService.GetAbility(id);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, Route("restore-ability/{id:int}")]
        public IActionResult RestoreAbility(Ability ability)
        {
            _dataService.RestoreAbility(ability.Id);

            return RedirectToAction("Abilities");
        }

        [HttpGet, Route("restore-egg-group/{id:int}")]
        public IActionResult RestoreEggGroup(int id)
        {
            EggGroup model = _dataService.GetEggGroup(id);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, Route("restore-egg-group/{id:int}")]
        public IActionResult RestoreEggGroup(EggGroup eggGroup)
        {
            _dataService.RestoreEggGroup(eggGroup.Id);

            return RedirectToAction("EggGroups");
        }

        [Route("error")]
        public IActionResult Error()
        {
            return View();
        }
    }
}
