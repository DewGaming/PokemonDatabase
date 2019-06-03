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

            this._dataService.UpdatePokemon(pokemon);

            return this.RedirectToAction("Pokemon", "Admin");
        }

        [HttpGet]
        [Route("edit_typing/{pokemonId}")]
        public IActionResult Typing(string pokemonId)
        {
            PokemonTypeDetail typeDetail = this._dataService.GetPokemonWithTypes().Find(x => x.PokemonId == pokemonId);
            PokemonTypingViewModel model = new PokemonTypingViewModel()
            {
                Id = typeDetail.Id,
                AllTypes = this._dataService.GetTypes(),
                PokemonId = typeDetail.PokemonId,
                PrimaryTypeId = typeDetail.PrimaryTypeId,
                SecondaryTypeId = typeDetail.SecondaryTypeId
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_typing/{id}")]
        public IActionResult Typing(PokemonTypeDetail pokemonTypeDetail)
        {
            if (!this.ModelState.IsValid)
            {
                PokemonTypeDetail typeDetail = this._dataService.GetPokemonWithTypes().Find(x => x.PokemonId == pokemonTypeDetail.PokemonId);
                PokemonTypingViewModel model = new PokemonTypingViewModel()
                {
                    Id = typeDetail.Id,
                    AllTypes = this._dataService.GetTypes(),
                    PokemonId = typeDetail.PokemonId,
                    PrimaryTypeId = typeDetail.PrimaryTypeId,
                    SecondaryTypeId = typeDetail.SecondaryTypeId
                };

                return this.View(model);
            }

            pokemonTypeDetail.Id = this._dataService.GetPokemonWithTypes().Find(x => x.PokemonId == pokemonTypeDetail.PokemonId).Id;

            this._dataService.UpdatePokemonTypeDetail(pokemonTypeDetail);

            return this.RedirectToAction("Pokemon", "Admin");
        }

        [HttpGet]
        [Route("edit_abilities/{pokemonId}")]
        public IActionResult Abilities(string pokemonId)
        {
            PokemonAbilityDetail abilityDetail = this._dataService.GetPokemonWithAbilities(pokemonId);
            PokemonAbilitiesViewModel model = new PokemonAbilitiesViewModel()
            {
                Id = abilityDetail.Id,
                AllAbilities = this._dataService.GetAbilities(),
                PokemonId = abilityDetail.PokemonId,
                PrimaryAbilityId = abilityDetail.PrimaryAbilityId,
                SecondaryAbilityId = abilityDetail.SecondaryAbilityId,
                HiddenAbilityId = abilityDetail.HiddenAbilityId
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_abilities/{pokemonId}")]
        public IActionResult Abilities(PokemonAbilityDetail pokemonAbilityDetail)
        {
            if (!this.ModelState.IsValid)
            {
                PokemonAbilityDetail abilityDetail = this._dataService.GetPokemonWithAbilities(pokemonAbilityDetail.PokemonId);
                PokemonAbilitiesViewModel model = new PokemonAbilitiesViewModel()
                {
                    Id = abilityDetail.Id,
                    AllAbilities = this._dataService.GetAbilities(),
                    PokemonId = abilityDetail.PokemonId,
                    PrimaryAbilityId = abilityDetail.PrimaryAbilityId,
                    SecondaryAbilityId = abilityDetail.SecondaryAbilityId,
                    HiddenAbilityId = abilityDetail.HiddenAbilityId
                };

                return this.View(model);
            }

            pokemonAbilityDetail.Id = this._dataService.GetPokemonWithAbilities(pokemonAbilityDetail.PokemonId).Id;

            this._dataService.UpdatePokemonAbilityDetail(pokemonAbilityDetail);

            return this.RedirectToAction("Pokemon", "Admin");
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
    }
}