using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PokemonDatabase.Models;
using PokemonDatabase.DataAccess.Models;
using Microsoft.AspNetCore.Authorization;

namespace PokemonDatabase.Controllers
{
    [Authorize(Roles = "Owner"), Route("admin")]
    public class OwnerController : Controller  
    {
        private readonly DataService _dataService;

        public OwnerController(DataContext dataContext)
        {
            // Instantiate an instance of the data service.
            _dataService = new DataService(dataContext);
        }

        [Route("users")]
        public IActionResult Users()
        {
            List<User> model = _dataService.GetUsers();

            return View(model);
        }

        [HttpGet, Route("edit-user/{id:int}")]
        public IActionResult EditUser(int id)
        {
            User model = _dataService.GetUserById(id);

            return View(model);
        }

        [HttpPost, Route("edit-user/{id:int}")]
        public IActionResult EditUser(User user)
        {
            if (!ModelState.IsValid)
            {
                User model = _dataService.GetUserWithUsername(user.Username);

                return View(model);
            }

            _dataService.UpdateUser(user);

            return RedirectToAction("Users");
        }

        [HttpGet, Route("archive-user/{id:int}")]
        public IActionResult ArchiveUser(int id)
        {
            User model = _dataService.GetUserById(id);

            return View(model);
        }

        [HttpPost, Route("archive-user/{id:int}")]
        public IActionResult ArchiveUser(User user)
        {
            _dataService.ArchiveUser(user.Id);

            return RedirectToAction("Users");
        }

        [HttpGet, Route("unarchive-user/{id:int}")]
        public IActionResult UnarchiveUser(int id)
        {
            User model = _dataService.GetUserById(id);

            return View(model);
        }

        [HttpPost, Route("unarchive-user/{id:int}")]
        public IActionResult UnarchiveUser(User user)
        {
            _dataService.UnarchiveUser(user.Id);

            return RedirectToAction("Users");
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
    }
}