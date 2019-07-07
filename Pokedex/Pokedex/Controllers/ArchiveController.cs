using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pokedex.DataAccess.Models;

namespace Pokedex.Controllers
{
    [Authorize(Roles = "Admin,Owner")]
    [Route("admin")]
    public class ArchiveController : Controller
    {
        private readonly DataService _dataService;

        public ArchiveController(DataContext dataContext)
        {
            this._dataService = new DataService(dataContext);
        }

        [HttpGet]
        [Route("archive_generation/{id}")]
        public IActionResult Generation(string id)
        {
            Generation model = this._dataService.GetGeneration(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("archive_generation/{id}")]
        public IActionResult Generation(Generation generation)
        {
            this._dataService.ArchiveGeneration(generation.Id);

            return this.RedirectToAction("Generations");
        }

        [HttpGet]
        [Route("archive_type/{id:int}")]
        public IActionResult Type(int id)
        {
            Type model = this._dataService.GetType(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("archive_type/{id:int}")]
        public IActionResult Type(Type type)
        {
            this._dataService.ArchiveType(type.Id);

            return this.RedirectToAction("Types");
        }

        [HttpGet]
        [Route("archive_shiny_hunt/{id:int}")]
        public IActionResult ShinyHunt(int id)
        {
            ShinyHunt model = this._dataService.GetShinyHunt(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("archive_shiny_hunt/{id:int}")]
        public IActionResult ShinyHunt(Type type)
        {
            this._dataService.ArchiveShinyHunt(type.Id);

            return this.RedirectToAction("ShinyHunts");
        }

        [HttpGet]
        [Route("archive_egg_group/{id:int}")]
        public IActionResult EggGroup(int id)
        {
            EggGroup model = this._dataService.GetEggGroup(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("archive_egg_group/{id:int}")]
        public IActionResult EggGroup(EggGroup eggGroup)
        {
            this._dataService.ArchiveEggGroup(eggGroup.Id);

            return this.RedirectToAction("EggGroups");
        }

        [HttpGet]
        [Route("archive_classification/{id:int}")]
        public IActionResult Classification(int id)
        {
            Classification model = this._dataService.GetClassification(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("archive_classification/{id:int}")]
        public IActionResult Classification(Classification classification)
        {
            this._dataService.ArchiveClassification(classification.Id);

            return this.RedirectToAction("Classifications");
        }

        [HttpGet]
        [Route("archive_ability/{id:int}")]
        public IActionResult Ability(int id)
        {
            Ability model = this._dataService.GetAbility(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("archive_ability/{id:int}")]
        public IActionResult Ability(Ability ability)
        {
            this._dataService.ArchiveAbility(ability.Id);

            return this.RedirectToAction("Abilities");
        }

        [HttpGet]
        [Route("archive_shiny_hunting_technique/{id:int}")]
        public IActionResult ShinyHuntingTechnique(int id)
        {
            ShinyHuntingTechnique model = this._dataService.GetShinyHuntingTechnique(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("archive_shiny_hunting_technique/{id:int}")]
        public IActionResult ShinyHuntingTechnique(ShinyHuntingTechnique shinyHuntingTechnique)
        {
            this._dataService.ArchiveShinyHuntingTechnique(shinyHuntingTechnique.Id);

            return this.RedirectToAction("ShinyHuntingTechniques");
        }
    }
}
