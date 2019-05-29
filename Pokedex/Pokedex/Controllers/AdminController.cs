using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public AdminController(DataContext dataContext)
        {
            this._dataService = new DataService(dataContext);
        }

        [Route("")]
        public IActionResult Index()
        {
            return this.View();
        }

        [Route("pokemon")]
        public IActionResult Pokemon()
        {
            List<Pokemon> model = this._dataService.GetAllPokemon().ToList();

            foreach(var altForm in model)
            {
                if (altForm.Id.Contains('-'))
                {
                    altForm.Name += " (" + this._dataService.GetPokemonFormName(altForm.Id) + ")";
                }
            }

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

        [HttpGet]
        [Route("edit_generation/{id}")]
        public IActionResult EditGeneration(string id)
        {
            Generation model = this._dataService.GetGeneration(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_generation/{id}")]
        public IActionResult EditGeneration(Generation generation)
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
        public IActionResult EditPokemon(string id)
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
        public IActionResult EditPokemon(Pokemon pokemon)
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
        public IActionResult EditType(int id)
        {
            Type model = this._dataService.GetType(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_type/{id:int}")]
        public IActionResult EditType(Type type)
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
        public IActionResult EditShinyHuntingTechnique(int id)
        {
            ShinyHuntingTechnique model = this._dataService.GetShinyHuntingTechnique(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_shiny_hunting_technique/{id:int}")]
        public IActionResult EditShinyHuntingTechnique(ShinyHuntingTechnique shinyHuntingTechnique)
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
        public IActionResult EditEggGroup(int id)
        {
            EggGroup model = this._dataService.GetEggGroup(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_egg_group/{id:int}")]
        public IActionResult EditEggGroup(EggGroup eggGroup)
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
        public IActionResult EditClassification(int id)
        {
            Classification model = this._dataService.GetClassification(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_classification/{id:int}")]
        public IActionResult EditClassification(Classification classification)
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
        public IActionResult EditShinyHunt(int id)
        {
            ShinyHunt model = this._dataService.GetShinyHunt(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_shiny_hunt/{id:int}")]
        public IActionResult EditShinyHunt(ShinyHunt shinyHunt)
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
        public IActionResult EditAbility(int id)
        {
            Ability model = this._dataService.GetAbility(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_ability/{id:int}")]
        public IActionResult EditAbility(Ability ability)
        {
            if (!this.ModelState.IsValid)
            {
                Ability model = this._dataService.GetAbility(ability.Id);

                return this.View(model);
            }

            this._dataService.UpdateAbility(ability);

            return this.RedirectToAction("Abilities");
        }

        [HttpGet]
        [Route("archive_generation/{id}")]
        public IActionResult ArchiveGeneration(string id)
        {
            Generation model = this._dataService.GetGeneration(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("archive_generation/{id}")]
        public IActionResult ArchiveGeneration(Generation generation)
        {
            this._dataService.ArchiveGeneration(generation.Id);

            return this.RedirectToAction("Generations");
        }

        [HttpGet]
        [Route("archive_type/{id:int}")]
        public IActionResult ArchiveType(int id)
        {
            Type model = this._dataService.GetType(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("archive_type/{id:int}")]
        public IActionResult ArchiveType(Type type)
        {
            this._dataService.ArchiveType(type.Id);

            return this.RedirectToAction("Types");
        }

        [HttpGet]
        [Route("archive_shiny_hunt/{id:int}")]
        public IActionResult ArchiveShinyHunt(int id)
        {
            ShinyHunt model = this._dataService.GetShinyHunt(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("archive_shiny_hunt/{id:int}")]
        public IActionResult ArchiveShinyHunt(Type type)
        {
            this._dataService.ArchiveShinyHunt(type.Id);

            return this.RedirectToAction("ShinyHunts");
        }

        [HttpGet]
        [Route("archive_egg_group/{id:int}")]
        public IActionResult ArchiveEggGroup(int id)
        {
            EggGroup model = this._dataService.GetEggGroup(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("archive_egg_group/{id:int}")]
        public IActionResult ArchiveEggGroup(EggGroup eggGroup)
        {
            this._dataService.ArchiveEggGroup(eggGroup.Id);

            return this.RedirectToAction("EggGroups");
        }

        [HttpGet]
        [Route("archive_classification/{id:int}")]
        public IActionResult ArchiveClassification(int id)
        {
            Classification model = this._dataService.GetClassification(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("archive_classification/{id:int}")]
        public IActionResult ArchiveClassification(Classification classification)
        {
            this._dataService.ArchiveClassification(classification.Id);

            return this.RedirectToAction("Classifications");
        }

        [HttpGet]
        [Route("archive_ability/{id:int}")]
        public IActionResult ArchiveAbility(int id)
        {
            Ability model = this._dataService.GetAbility(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("archive_ability/{id:int}")]
        public IActionResult ArchiveAbility(Ability ability)
        {
            this._dataService.ArchiveAbility(ability.Id);

            return this.RedirectToAction("Abilities");
        }

        [HttpGet]
        [Route("archive_shiny_hunting_technique/{id:int}")]
        public IActionResult ArchiveShinyHuntingTechnique(int id)
        {
            ShinyHuntingTechnique model = this._dataService.GetShinyHuntingTechnique(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("archive_shiny_hunting_technique/{id:int}")]
        public IActionResult ArchiveShinyHuntingTechnique(ShinyHuntingTechnique shinyHuntingTechnique)
        {
            this._dataService.ArchiveShinyHuntingTechnique(shinyHuntingTechnique.Id);

            return this.RedirectToAction("ShinyHuntingTechniques");
        }

        [HttpGet]
        [Route("unarchive_generation/{id}")]
        public IActionResult UnarchiveGeneration(string id)
        {
            Generation model = this._dataService.GetGeneration(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("unarchive_generation/{id}")]
        public IActionResult UnarchiveGeneration(Generation generation)
        {
            this._dataService.UnarchiveGeneration(generation.Id);

            return this.RedirectToAction("Generations");
        }

        [HttpGet]
        [Route("unarchive_type/{id:int}")]
        public IActionResult UnarchiveType(int id)
        {
            Type model = this._dataService.GetType(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("unarchive_type/{id:int}")]
        public IActionResult UnarchiveType(Type type)
        {
            this._dataService.UnarchiveType(type.Id);

            return this.RedirectToAction("Types");
        }

        [HttpGet]
        [Route("unarchive_shiny_hunt/{id:int}")]
        public IActionResult UnarchiveShinyHunt(int id)
        {
            ShinyHunt model = this._dataService.GetShinyHunt(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("unarchive_shiny_hunt/{id:int}")]
        public IActionResult UnarchiveShinyHunt(ShinyHunt shinyHunt)
        {
            this._dataService.UnarchiveShinyHunt(shinyHunt.Id);

            return this.RedirectToAction("ShinyHunts");
        }

        [HttpGet]
        [Route("unarchive_classification/{id:int}")]
        public IActionResult UnarchiveClassification(int id)
        {
            Classification model = this._dataService.GetClassification(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("unarchive_classification/{id:int}")]
        public IActionResult UnarchiveType(Classification classification)
        {
            this._dataService.UnarchiveClassification(classification.Id);

            return this.RedirectToAction("Classifications");
        }

        [HttpGet]
        [Route("unarchive_ability/{id:int}")]
        public IActionResult UnarchiveAbility(int id)
        {
            Ability model = this._dataService.GetAbility(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("unarchive_ability/{id:int}")]
        public IActionResult UnarchiveAbility(Ability ability)
        {
            this._dataService.UnarchiveAbility(ability.Id);

            return this.RedirectToAction("Abilities");
        }

        [HttpGet]
        [Route("unarchive_shiny_hunting_technique/{id:int}")]
        public IActionResult UnarchiveShinyHuntingTechnique(int id)
        {
            ShinyHuntingTechnique model = this._dataService.GetShinyHuntingTechnique(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("unarchive_shiny_hunting_technique/{id:int}")]
        public IActionResult UnarchiveShinyHuntingTechnique(ShinyHuntingTechnique shinyHuntingTechnique)
        {
            this._dataService.UnarchiveShinyHuntingTechnique(shinyHuntingTechnique.Id);

            return this.RedirectToAction("ShinyHuntingTechniques");
        }

        [HttpGet]
        [Route("unarchive_egg_group/{id:int}")]
        public IActionResult UnarchiveEggGroup(int id)
        {
            EggGroup model = this._dataService.GetEggGroup(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("unarchive_egg_group/{id:int}")]
        public IActionResult UnarchiveEggGroup(EggGroup eggGroup)
        {
            this._dataService.UnarchiveEggGroup(eggGroup.Id);

            return this.RedirectToAction("EggGroups");
        }

        [Route("error")]
        public IActionResult Error()
        {
            return this.View();
        }
    }
}
