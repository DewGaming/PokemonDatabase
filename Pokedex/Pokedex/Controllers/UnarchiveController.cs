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
    public class UnarchiveController : Controller
    {
        private readonly DataService _dataService;

        public UnarchiveController(DataContext dataContext)
        {
            this._dataService = new DataService(dataContext);
        }

        [HttpGet]
        [Route("unarchive_generation/{id}")]
        public IActionResult Generation(string id)
        {
            Generation model = this._dataService.GetGeneration(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("unarchive_generation/{id}")]
        public IActionResult Generation(Generation generation)
        {
            this._dataService.UnarchiveGeneration(generation.Id);

            return this.RedirectToAction("Generations");
        }

        [HttpGet]
        [Route("unarchive_type/{id:int}")]
        public IActionResult Type(int id)
        {
            Type model = this._dataService.GetType(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("unarchive_type/{id:int}")]
        public IActionResult Type(Type type)
        {
            this._dataService.UnarchiveType(type.Id);

            return this.RedirectToAction("Types");
        }

        [HttpGet]
        [Route("unarchive_shiny_hunt/{id:int}")]
        public IActionResult ShinyHunt(int id)
        {
            ShinyHunt model = this._dataService.GetShinyHunt(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("unarchive_shiny_hunt/{id:int}")]
        public IActionResult ShinyHunt(ShinyHunt shinyHunt)
        {
            this._dataService.UnarchiveShinyHunt(shinyHunt.Id);

            return this.RedirectToAction("ShinyHunts");
        }

        [HttpGet]
        [Route("unarchive_classification/{id:int}")]
        public IActionResult Classification(int id)
        {
            Classification model = this._dataService.GetClassification(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("unarchive_classification/{id:int}")]
        public IActionResult Type(Classification classification)
        {
            this._dataService.UnarchiveClassification(classification.Id);

            return this.RedirectToAction("Classifications");
        }

        [HttpGet]
        [Route("unarchive_ability/{id:int}")]
        public IActionResult Ability(int id)
        {
            Ability model = this._dataService.GetAbility(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("unarchive_ability/{id:int}")]
        public IActionResult Ability(Ability ability)
        {
            this._dataService.UnarchiveAbility(ability.Id);

            return this.RedirectToAction("Abilities");
        }

        [HttpGet]
        [Route("unarchive_shiny_hunting_technique/{id:int}")]
        public IActionResult ShinyHuntingTechnique(int id)
        {
            ShinyHuntingTechnique model = this._dataService.GetShinyHuntingTechnique(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("unarchive_shiny_hunting_technique/{id:int}")]
        public IActionResult ShinyHuntingTechnique(ShinyHuntingTechnique shinyHuntingTechnique)
        {
            this._dataService.UnarchiveShinyHuntingTechnique(shinyHuntingTechnique.Id);

            return this.RedirectToAction("ShinyHuntingTechniques");
        }

        [HttpGet]
        [Route("unarchive_egg_group/{id:int}")]
        public IActionResult EggGroup(int id)
        {
            EggGroup model = this._dataService.GetEggGroup(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("unarchive_egg_group/{id:int}")]
        public IActionResult EggGroup(EggGroup eggGroup)
        {
            this._dataService.UnarchiveEggGroup(eggGroup.Id);

            return this.RedirectToAction("EggGroups");
        }
    }
}
