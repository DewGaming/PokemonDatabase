using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pokedex.DataAccess.Models;
using Pokedex.Models;

namespace Pokedex.Controllers
{
    [Authorize(Roles = "Admin,Owner")]
    [Route("admin")]
    public class EditController : Controller
    {
        private readonly DataService _dataService;

        public EditController(DataContext dataContext)
        {
            // Instantiate an instance of the data service.
            this._dataService = new DataService(dataContext);
        }

        [Authorize(Roles = "Owner")]
        [HttpGet]
        [Route("edit_user/{id:int}")]
        public IActionResult User(int id)
        {
            User model = this._dataService.GetUserById(id);

            return this.View(model);
        }

        [HttpGet]
        [Route("edit_generation/{id}")]
        public IActionResult Generation(string id)
        {
            Generation model = this._dataService.GetGeneration(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_generation/{id}")]
        public IActionResult Generation(Generation generation)
        {
            if (!this.ModelState.IsValid)
            {
                Generation model = this._dataService.GetGeneration(generation.Id);

                return this.View(model);
            }

            this._dataService.UpdateGeneration(generation);

            return this.RedirectToAction("Generations");
        }

        [HttpGet]
        [Route("edit_pokemon/{id}")]
        public IActionResult Pokemon(string id)
        {
            BasePokemonViewModel model = new BasePokemonViewModel(){
                Pokemon = this._dataService.GetPokemonById(id),
                AllBaseHappinesses = this._dataService.GetBaseHappinesses(),
                AllClassifications = this._dataService.GetClassifications(),
                AllCaptureRates = this._dataService.GetCaptureRates(),
                AllEggCycles = this._dataService.GetEggCycles(),
                AllExperienceGrowths = this._dataService.GetExperienceGrowths(),
                AllGenderRatios = this._dataService.GetGenderRatios(),
                AllGenerations = this._dataService.GetGenerations(),
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_pokemon/{id}")]
        public IActionResult Pokemon(Pokemon pokemon)
        {
            if (!this.ModelState.IsValid)
            {
                BasePokemonViewModel model = new BasePokemonViewModel(){
                    Pokemon = this._dataService.GetPokemonById(pokemon.Id),
                    AllBaseHappinesses = this._dataService.GetBaseHappinesses(),
                    AllClassifications = this._dataService.GetClassifications(),
                    AllCaptureRates = this._dataService.GetCaptureRates(),
                    AllEggCycles = this._dataService.GetEggCycles(),
                    AllExperienceGrowths = this._dataService.GetExperienceGrowths(),
                    AllGenderRatios = this._dataService.GetGenderRatios(),
                    AllGenerations = this._dataService.GetGenerations(),
                };

                return this.View(model);
            }

            this._dataService.UpdatePokemon(pokemon);

            return this.RedirectToAction("Pokemon");
        }

        [HttpGet]
        [Route("edit_type/{id:int}")]
        public IActionResult Type(int id)
        {
            Type model = this._dataService.GetType(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_type/{id:int}")]
        public IActionResult Type(Type type)
        {
            if (!this.ModelState.IsValid)
            {
                Type model = this._dataService.GetType(type.Id);

                return this.View(model);
            }

            this._dataService.UpdateType(type);

            return this.RedirectToAction("Types");
        }

        [HttpGet]
        [Route("edit_shiny_hunting_technique/{id:int}")]
        public IActionResult ShinyHuntingTechnique(int id)
        {
            ShinyHuntingTechnique model = this._dataService.GetShinyHuntingTechnique(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_shiny_hunting_technique/{id:int}")]
        public IActionResult ShinyHuntingTechnique(ShinyHuntingTechnique shinyHuntingTechnique)
        {
            if (!this.ModelState.IsValid)
            {
                ShinyHuntingTechnique model = this._dataService.GetShinyHuntingTechnique(shinyHuntingTechnique.Id);

                return this.View(model);
            }

            this._dataService.UpdateShinyHuntingTechnique(shinyHuntingTechnique);

            return this.RedirectToAction("ShinyHuntingTechniques");
        }

        [HttpGet]
        [Route("edit_egg_group/{id:int}")]
        public IActionResult EggGroup(int id)
        {
            EggGroup model = this._dataService.GetEggGroup(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_egg_group/{id:int}")]
        public IActionResult EggGroup(EggGroup eggGroup)
        {
            if (!this.ModelState.IsValid)
            {
                EggGroup model = this._dataService.GetEggGroup(eggGroup.Id);

                return this.View(model);
            }

            this._dataService.UpdateEggGroup(eggGroup);

            return this.RedirectToAction("EggGroups");
        }

        [HttpGet]
        [Route("edit_classification/{id:int}")]
        public IActionResult Classification(int id)
        {
            Classification model = this._dataService.GetClassification(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_classification/{id:int}")]
        public IActionResult Classification(Classification classification)
        {
            if (!this.ModelState.IsValid)
            {
                Classification model = this._dataService.GetClassification(classification.Id);

                return this.View(model);
            }

            this._dataService.UpdateClassification(classification);

            return this.RedirectToAction("Classifications");
        }

        [HttpGet]
        [Route("edit_shiny_hunt/{id:int}")]
        public IActionResult ShinyHunt(int id)
        {
            ShinyHunt model = this._dataService.GetShinyHunt(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_shiny_hunt/{id:int}")]
        public IActionResult ShinyHunt(ShinyHunt shinyHunt)
        {
            if (!this.ModelState.IsValid)
            {
                ShinyHunt model = this._dataService.GetShinyHunt(shinyHunt.Id);

                return this.View(model);
            }

            this._dataService.UpdateShinyHunt(shinyHunt);

            return this.RedirectToAction("ShinyHunts");
        }

        [HttpGet]
        [Route("edit_ability/{id:int}")]
        public IActionResult Ability(int id)
        {
            Ability model = this._dataService.GetAbility(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_ability/{id:int}")]
        public IActionResult Ability(Ability ability)
        {
            if (!this.ModelState.IsValid)
            {
                Ability model = this._dataService.GetAbility(ability.Id);

                return this.View(model);
            }

            this._dataService.UpdateAbility(ability);

            return this.RedirectToAction("Abilities");
        }

        [Authorize(Roles = "Owner")]
        [HttpPost]
        [Route("edit_user/{id:int}")]
        public IActionResult User(User user)
        {
            if (!this.ModelState.IsValid)
            {
                User model = this._dataService.GetUserWithUsername(user.Username);

                return this.View(model);
            }

            this._dataService.UpdateUser(user);

            return this.RedirectToAction("Users");
        }

        [HttpGet]
        [Route("add_evolution")]
        public IActionResult AddEvolution()
        {
            EvolutionViewModel model = new EvolutionViewModel()
            {
                AllEvolutionMethods = this._dataService.GetEvolutionMethods(),
            };
            List<Pokemon> pokemonList = this._dataService.GetAllPokemon();
            foreach (var pokemon in pokemonList.Where(p => p.Id.Contains('-')))
            {
                pokemon.Name += " (" + this._dataService.GetPokemonFormName(pokemon.Id) + ")";
            }

            model.AllPokemon = pokemonList;
            
            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_evolution")]
        public IActionResult AddEvolution(Evolution evolution)
        {
            if (!this.ModelState.IsValid)
            {
                EvolutionViewModel model = new EvolutionViewModel()
                {
                    AllEvolutionMethods = this._dataService.GetEvolutionMethods(),
                };
                List<Pokemon> pokemonList = this._dataService.GetAllPokemon();
                foreach (var pokemon in pokemonList.Where(p => p.Id.Contains('-')))
                {
                    pokemon.Name += " (" + this._dataService.GetPokemonFormName(pokemon.Id) + ")";
                }

                model.AllPokemon = pokemonList;

                return this.View(model);
            }

            this._dataService.AddEvolution(evolution);

            return this.RedirectToAction("Pokemon", "Admin");
        }

        [HttpGet]
        [Route("add_shiny_hunting_technique")]
        public IActionResult AddShinyHuntingTechnique()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_shiny_hunting_technique")]
        public IActionResult AddShinyHuntingTechnique(ShinyHuntingTechnique shinyHuntingTechnique)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this._dataService.AddShinyHuntingTechnique(shinyHuntingTechnique);

            return this.RedirectToAction("ShinyHuntingTechniques", "Admin");
        }

        [HttpGet]
        [Route("add_generation")]
        public IActionResult AddGeneration()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_generation")]
        public IActionResult AddGeneration(Generation generation)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this._dataService.AddGeneration(generation);

            return this.RedirectToAction("Generations", "Admin");
        }

        [HttpGet]
        [Route("delete_generation/{id}")]
        public IActionResult DeleteGeneration(string id)
        {
            Generation model = this._dataService.GetGeneration(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_generation/{id}")]
        public IActionResult DeleteGeneration(Generation generation)
        {
            this._dataService.DeleteGeneration(generation.Id);

            return this.RedirectToAction("Generations", "Admin");
        }

        [HttpGet]
        [Route("delete_type/{id:int}")]
        public IActionResult DeleteType(int id)
        {
            Type model = this._dataService.GetType(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_type/{id:int}")]
        public IActionResult DeleteType(Type type)
        {
            this._dataService.DeleteType(type.Id);

            return this.RedirectToAction("Types", "Admin");
        }

        [HttpGet]
        [Route("delete_shiny_hunting_technique/{id:int}")]
        public IActionResult DeleteShinyHuntingTechnique(int id)
        {
            ShinyHuntingTechnique model = this._dataService.GetShinyHuntingTechnique(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_shiny_hunting_technique/{id:int}")]
        public IActionResult DeleteShinyHuntingTechnique(ShinyHuntingTechnique shinyHuntingTechnique)
        {
            this._dataService.DeleteShinyHuntingTechnique(shinyHuntingTechnique.Id);

            return this.RedirectToAction("ShinyHuntingTechniques", "Admin");
        }

        [HttpGet]
        [Route("delete_shiny_hunt/{id:int}")]
        public IActionResult DeleteShinyHunt(int id)
        {
            ShinyHunt model = this._dataService.GetShinyHunt(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_shiny_hunt/{id:int}")]
        public IActionResult DeleteShinyHunt(ShinyHunt shinyHunt)
        {
            this._dataService.DeleteShinyHunt(shinyHunt.Id);

            return this.RedirectToAction("ShinyHunts", "Admin");
        }

        [HttpGet]
        [Route("delete_ability/{id:int}")]
        public IActionResult DeleteAbility(int id)
        {
            Ability model = this._dataService.GetAbility(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_ability/{id:int}")]
        public IActionResult DeleteAbility(Ability ability)
        {
            this._dataService.DeleteAbility(ability.Id);

            return this.RedirectToAction("Abilities", "Admin");
        }

        [HttpGet]
        [Route("delete_user/{id:int}")]
        public IActionResult DeleteUser(int id)
        {
            User model = this._dataService.GetUserById(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_user/{id:int}")]
        public IActionResult DeleteUser(User user)
        {
            this._dataService.DeleteUser(user.Id);

            return this.RedirectToAction("Users", "Owner");
        }

        [HttpGet]
        [Route("delete_egg_group/{id:int}")]
        public IActionResult DeleteEggGroup(int id)
        {
            EggGroup model = this._dataService.GetEggGroup(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_egg_group/{id:int}")]
        public IActionResult DeleteEggGroup(EggGroup eggGroup)
        {
            this._dataService.DeleteEggGroup(eggGroup.Id);

            return this.RedirectToAction("EggGroups", "Admin");
        }

        [HttpGet]
        [Route("delete_classification/{id:int}")]
        public IActionResult DeleteClassification(int id)
        {
            Classification model = this._dataService.GetClassification(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_classification/{id:int}")]
        public IActionResult DeleteClassification(Classification classification)
        {
            this._dataService.DeleteClassification(classification.Id);

            return this.RedirectToAction("Classifications", "Admin");
        }

        [HttpGet]
        [Route("add_type")]
        public IActionResult AddType()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_type")]
        public IActionResult AddType(Type type)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this._dataService.AddType(type);

            return this.RedirectToAction("Types", "Admin");
        }

        [HttpGet]
        [Route("add_egg_group")]
        public IActionResult AddEggGroup()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_egg_group")]
        public IActionResult AddEggGroup(EggGroup eggGroup)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this._dataService.AddEggGroup(eggGroup);

            return this.RedirectToAction("EggGroups", "Admin");
        }

        [HttpGet]
        [Route("add_classification")]
        public IActionResult AddClassification()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_classification")]
        public IActionResult AddClassification(Classification classification)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this._dataService.AddClassification(classification);

            return this.RedirectToAction("Classifications", "Admin");
        }

        [HttpGet]
        [Route("add_ability")]
        public IActionResult AddAbility()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_ability")]
        public IActionResult AddAbility(Ability ability)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this._dataService.AddAbility(ability);

            return this.RedirectToAction("Abilities", "Admin");
        }

        [HttpGet]
        [Route("add_pokemon")]
        public IActionResult AddPokemon()
        {
            BasePokemonViewModel model = new BasePokemonViewModel()
            {
                AllBaseHappinesses = this._dataService.GetBaseHappinesses(),
                AllClassifications = this._dataService.GetClassifications(),
                AllCaptureRates = this._dataService.GetCaptureRates(),
                AllEggCycles = this._dataService.GetEggCycles(),
                AllExperienceGrowths = this._dataService.GetExperienceGrowths(),
                AllGenderRatios = this._dataService.GetGenderRatios(),
                AllGenerations = this._dataService.GetGenerations(),
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_pokemon")]
        public IActionResult AddPokemon(BasePokemonViewModel pokemon)
        {
            int pokedexNumber;
            if (!this.ModelState.IsValid)
            {
                BasePokemonViewModel model = new BasePokemonViewModel()
                {
                    AllBaseHappinesses = this._dataService.GetBaseHappinesses(),
                    AllClassifications = this._dataService.GetClassifications(),
                    AllCaptureRates = this._dataService.GetCaptureRates(),
                    AllEggCycles = this._dataService.GetEggCycles(),
                    AllExperienceGrowths = this._dataService.GetExperienceGrowths(),
                    AllGenderRatios = this._dataService.GetGenderRatios(),
                    AllGenerations = this._dataService.GetGenerations(),
                };

                return this.View(model);
            }
            else if (!int.TryParse(pokemon.Id, out pokedexNumber))
            {
                BasePokemonViewModel model = new BasePokemonViewModel()
                {
                    AllBaseHappinesses = this._dataService.GetBaseHappinesses(),
                    AllClassifications = this._dataService.GetClassifications(),
                    AllCaptureRates = this._dataService.GetCaptureRates(),
                    AllEggCycles = this._dataService.GetEggCycles(),
                    AllExperienceGrowths = this._dataService.GetExperienceGrowths(),
                    AllGenderRatios = this._dataService.GetGenderRatios(),
                    AllGenerations = this._dataService.GetGenerations(),
                };

                this.ModelState.AddModelError("Pokedex Number", "Pokedex Number must be a number.");
                return this.View(model);
            }
            else if (this._dataService.GetAllPokemon().Exists(x => x.Id == pokemon.Id))
            {
                BasePokemonViewModel model = new BasePokemonViewModel()
                {
                    AllBaseHappinesses = this._dataService.GetBaseHappinesses(),
                    AllClassifications = this._dataService.GetClassifications(),
                    AllCaptureRates = this._dataService.GetCaptureRates(),
                    AllEggCycles = this._dataService.GetEggCycles(),
                    AllExperienceGrowths = this._dataService.GetExperienceGrowths(),
                    AllGenderRatios = this._dataService.GetGenderRatios(),
                    AllGenerations = this._dataService.GetGenerations(),
                };

                this.ModelState.AddModelError("Pokedex Number", "Pokedex Number already exists.");
                return this.View(model);
            }

            this._dataService.AddPokemon(pokemon);

            return this.RedirectToAction("AddTyping", "Admin", new { pokemonId = pokemon.Id });
        }

        [HttpGet]
        [Route("add_typing/{pokemonId}")]
        public IActionResult AddTyping(string pokemonId)
        {
            PokemonTypingViewModel model = new PokemonTypingViewModel()
            {
                AllTypes = this._dataService.GetTypes(),
                PokemonId = pokemonId,
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_typing/{pokemonId}")]
        public IActionResult AddTyping(PokemonTypingViewModel typing)
        {
            if (!this.ModelState.IsValid)
            {
                PokemonTypingViewModel model = new PokemonTypingViewModel()
                {
                    AllTypes = this._dataService.GetTypes(),
                    PokemonId = typing.PokemonId,
                };

                return this.View(model);
            }

            this._dataService.AddPokemonTyping(typing);

            return this.RedirectToAction("AddAbilities", "Admin", new { pokemonId = typing.PokemonId });
        }

        [HttpGet]
        [Route("add_abilities/{pokemonId}")]
        public IActionResult AddAbilities(string pokemonId)
        {
            PokemonAbilitiesViewModel model = new PokemonAbilitiesViewModel()
            {
                AllAbilities = this._dataService.GetAbilities(),
                PokemonId = pokemonId,
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_abilities/{pokemonId}")]
        public IActionResult AddAbilities(PokemonAbilitiesViewModel abilities)
        {
            if (!this.ModelState.IsValid)
            {
                PokemonAbilitiesViewModel model = new PokemonAbilitiesViewModel()
                {
                    AllAbilities = this._dataService.GetAbilities(),
                    PokemonId = abilities.PokemonId,
                };

                return this.View(model);
            }

            this._dataService.AddPokemonAbilities(abilities);

            return this.RedirectToAction("AddEggGroups", "Admin", new { pokemonId = abilities.PokemonId });
        }

        [HttpGet]
        [Route("add_egg_groups/{pokemonId}")]
        public IActionResult AddEggGroups(string pokemonId)
        {
            PokemonEggGroupsViewModel model = new PokemonEggGroupsViewModel()
            {
                AllEggGroups = this._dataService.GetEggGroups(),
                PokemonId = pokemonId,
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_egg_groups/{pokemonId}")]
        public IActionResult AddEggGroups(PokemonEggGroupsViewModel eggGroups)
        {
            if (!this.ModelState.IsValid)
            {
                PokemonEggGroupsViewModel model = new PokemonEggGroupsViewModel()
                {
                    AllEggGroups = this._dataService.GetEggGroups(),
                    PokemonId = eggGroups.PokemonId,
                };

                return this.View(model);
            }

            this._dataService.AddPokemonEggGroups(eggGroups);

            return this.RedirectToAction("AddBaseStats", "Admin", new { pokemonId = eggGroups.PokemonId });
        }

        [HttpGet]
        [Route("add_base_stats/{pokemonId}")]
        public IActionResult AddBaseStats(string pokemonId)
        {
            BaseStat model = new BaseStat()
            {
                PokemonId = pokemonId,
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_base_stats/{pokemonId}")]
        public IActionResult AddBaseStats(BaseStat baseStat)
        {
            if (!this.ModelState.IsValid)
            {
                BaseStat model = new BaseStat()
                {
                    PokemonId = baseStat.PokemonId,
                };

                return this.View(model);
            }

            this._dataService.AddPokemonBaseStat(baseStat);

            return this.RedirectToAction("AddEVYields", "Admin", new { pokemonId = baseStat.PokemonId });
        }

        [HttpGet]
        [Route("add_ev_yields/{pokemonId}")]
        public IActionResult AddEVYields(string pokemonId)
        {
            EVYield model = new EVYield()
            {
                PokemonId = pokemonId,
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_ev_yields/{pokemonId}")]
        public IActionResult AddEVYields(EVYield evYield)
        {
            if (!this.ModelState.IsValid)
            {
                EVYield model = new EVYield()
                {
                    PokemonId = evYield.PokemonId,
                };

                return this.View(model);
            }
            else if (evYield.Health > 3)
            {
                EVYield model = new EVYield()
                {
                    PokemonId = evYield.PokemonId,
                };

                this.ModelState.AddModelError("EVTotal", "Health must be 3 or less.");
                return this.View(model);
            }
            else if (evYield.Attack > 3)
            {
                EVYield model = new EVYield()
                {
                    PokemonId = evYield.PokemonId,
                };

                this.ModelState.AddModelError("EVTotal", "Attack must be 3 or less.");
                return this.View(model);
            }
            else if (evYield.Defense > 3)
            {
                EVYield model = new EVYield()
                {
                    PokemonId = evYield.PokemonId,
                };

                this.ModelState.AddModelError("EVTotal", "Defense must be 3 or less.");
                return this.View(model);
            }
            else if (evYield.SpecialAttack > 3)
            {
                EVYield model = new EVYield()
                {
                    PokemonId = evYield.PokemonId,
                };

                this.ModelState.AddModelError("EVTotal", "Special Attack must be 3 or less.");
                return this.View(model);
            }
            else if (evYield.SpecialDefense > 3)
            {
                EVYield model = new EVYield()
                {
                    PokemonId = evYield.PokemonId,
                };

                this.ModelState.AddModelError("EVTotal", "Special Defense must be 3 or less.");
                return this.View(model);
            }
            else if (evYield.Speed > 3)
            {
                EVYield model = new EVYield()
                {
                    PokemonId = evYield.PokemonId,
                };

                this.ModelState.AddModelError("EVTotal", "Speed must be 3 or less.");
                return this.View(model);
            }
            else if (evYield.EVTotal > 3)
            {
                EVYield model = new EVYield()
                {
                    PokemonId = evYield.PokemonId,
                };

                this.ModelState.AddModelError("EVTotal", "EV Total must be 3 or less.");
                return this.View(model);
            }

            this._dataService.AddPokemonEVYield(evYield);

            Pokemon pokemon = this._dataService.GetPokemonById(evYield.PokemonId);

            return this.RedirectToAction("Pokemon", "Home", new { Name = pokemon.Name.Replace(' ', '_').ToLower() });
        }
    }
}