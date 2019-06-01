using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pokedex.DataAccess.Models;
using Pokedex.Models;

namespace Pokedex.Controllers
{
    [Authorize(Roles = "Owner")]
    [Route("admin")]
    public class AddController : Controller
    {
        private readonly DataService _dataService;

        public AddController(DataContext dataContext)
        {
            // Instantiate an instance of the data service.
            this._dataService = new DataService(dataContext);
        }

        [HttpGet]
        [Route("add_evolution")]
        public IActionResult Evolution()
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
        public IActionResult Evolution(Evolution evolution)
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
        public IActionResult ShinyHuntingTechnique()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_shiny_hunting_technique")]
        public IActionResult ShinyHuntingTechnique(ShinyHuntingTechnique shinyHuntingTechnique)
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
        public IActionResult Generation()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_generation")]
        public IActionResult Generation(Generation generation)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this._dataService.AddGeneration(generation);

            return this.RedirectToAction("Generations", "Admin");
        }

        [HttpGet]
        [Route("add_type")]
        public IActionResult Type()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_type")]
        public IActionResult Type(Type type)
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

            this._dataService.AddEggGroup(eggGroup);

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

            this._dataService.AddClassification(classification);

            return this.RedirectToAction("Classifications", "Admin");
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

            this._dataService.AddAbility(ability);

            return this.RedirectToAction("Abilities", "Admin");
        }

        [HttpGet]
        [Route("add_pokemon")]
        public IActionResult Pokemon()
        {
            BasePokemonViewModel model = new BasePokemonViewModel()
            {
                AllBaseHappinesses = this._dataService.GetBaseHappinesses(),
                AllClassifications = this._dataService.GetClassifications(),
                AllCaptureRates = this._dataService.GetCaptureRates(),
                AllEggCycles = this._dataService.GetEggCycles(),
                AllExperienceGrowths = this._dataService.GetExperienceGrowths(),
                AllGenderRatios = new List<GenderRatioViewModel>(),
                AllGenerations = this._dataService.GetGenerations(),
            };

            foreach(GenderRatio genderRatio in this._dataService.GetGenderRatios())
            {
                GenderRatioViewModel viewModel = new GenderRatioViewModel() {
                    Id = genderRatio.Id
                };

                if (genderRatio.MaleRatio == genderRatio.FemaleRatio && genderRatio.MaleRatio == 0)
                {
                    viewModel.GenderRatioString = "Genderless";
                }
                else if (genderRatio.FemaleRatio == 0)
                {
                    viewModel.GenderRatioString = genderRatio.MaleRatio + "% Male";
                }
                else if (genderRatio.MaleRatio == 0)
                {
                    viewModel.GenderRatioString = genderRatio.FemaleRatio + "% Female";
                }
                else
                {
                    viewModel.GenderRatioString = genderRatio.MaleRatio + "% Male / " + genderRatio.FemaleRatio + "% Female";
                }

                model.AllGenderRatios.Add(viewModel);
            }

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_pokemon")]
        public IActionResult Pokemon(BasePokemonViewModel pokemon)
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
                    AllGenderRatios = new List<GenderRatioViewModel>(),
                    AllGenerations = this._dataService.GetGenerations(),
                };

                foreach(GenderRatio genderRatio in this._dataService.GetGenderRatios())
                {
                    GenderRatioViewModel viewModel = new GenderRatioViewModel() {
                        Id = genderRatio.Id
                    };

                    if (genderRatio.MaleRatio == genderRatio.FemaleRatio && genderRatio.MaleRatio == 0)
                    {
                        viewModel.GenderRatioString = "Genderless";
                    }
                    else if (genderRatio.FemaleRatio == 0)
                    {
                        viewModel.GenderRatioString = genderRatio.MaleRatio + "% Male";
                    }
                    else if (genderRatio.MaleRatio == 0)
                    {
                        viewModel.GenderRatioString = genderRatio.FemaleRatio + "% Female";
                    }
                    else
                    {
                        viewModel.GenderRatioString = genderRatio.MaleRatio + "% Male / " + genderRatio.FemaleRatio + "% Female";
                    }

                    model.AllGenderRatios.Add(viewModel);
                }

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
                    AllGenderRatios = new List<GenderRatioViewModel>(),
                    AllGenerations = this._dataService.GetGenerations(),
                };
    
                foreach(GenderRatio genderRatio in this._dataService.GetGenderRatios())
                {
                    GenderRatioViewModel viewModel = new GenderRatioViewModel() {
                        Id = genderRatio.Id
                    };
    
                    if (genderRatio.MaleRatio == genderRatio.FemaleRatio && genderRatio.MaleRatio == 0)
                    {
                        viewModel.GenderRatioString = "Genderless";
                    }
                    else if (genderRatio.FemaleRatio == 0)
                    {
                        viewModel.GenderRatioString = genderRatio.MaleRatio + "% Male";
                    }
                    else if (genderRatio.MaleRatio == 0)
                    {
                        viewModel.GenderRatioString = genderRatio.FemaleRatio + "% Female";
                    }
                    else
                    {
                        viewModel.GenderRatioString = genderRatio.MaleRatio + "% Male / " + genderRatio.FemaleRatio + "% Female";
                    }
    
                    model.AllGenderRatios.Add(viewModel);
                }

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
                    AllGenderRatios = new List<GenderRatioViewModel>(),
                    AllGenerations = this._dataService.GetGenerations(),
                };
    
                foreach(GenderRatio genderRatio in this._dataService.GetGenderRatios())
                {
                    GenderRatioViewModel viewModel = new GenderRatioViewModel() {
                        Id = genderRatio.Id
                    };
    
                    if (genderRatio.MaleRatio == genderRatio.FemaleRatio && genderRatio.MaleRatio == 0)
                    {
                        viewModel.GenderRatioString = "Genderless";
                    }
                    else if (genderRatio.FemaleRatio == 0)
                    {
                        viewModel.GenderRatioString = genderRatio.MaleRatio + "% Male";
                    }
                    else if (genderRatio.MaleRatio == 0)
                    {
                        viewModel.GenderRatioString = genderRatio.FemaleRatio + "% Female";
                    }
                    else
                    {
                        viewModel.GenderRatioString = genderRatio.MaleRatio + "% Male / " + genderRatio.FemaleRatio + "% Female";
                    }
    
                    model.AllGenderRatios.Add(viewModel);
                }

                this.ModelState.AddModelError("Pokedex Number", "Pokedex Number already exists.");
                return this.View(model);
            }

            this._dataService.AddPokemon(pokemon);

            return this.RedirectToAction("AddTyping", "Admin", new { pokemonId = pokemon.Id });
        }

        [HttpGet]
        [Route("add_typing/{pokemonId}")]
        public IActionResult Typing(string pokemonId)
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
        public IActionResult Typing(PokemonTypingViewModel typing)
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
        public IActionResult Abilities(string pokemonId)
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
        public IActionResult Abilities(PokemonAbilitiesViewModel abilities)
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
        public IActionResult EggGroups(string pokemonId)
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
        public IActionResult EggGroups(PokemonEggGroupsViewModel eggGroups)
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
        public IActionResult BaseStats(string pokemonId)
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
        public IActionResult BaseStats(BaseStat baseStat)
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
        public IActionResult EVYields(string pokemonId)
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
        public IActionResult EVYields(EVYield evYield)
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