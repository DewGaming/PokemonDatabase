using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Pokedex.DataAccess.Models;

namespace Pokedex.Controllers
{
    [Authorize(Roles = "Owner")]
    [Route("admin")]
    public class DeleteController : Controller
    {
        private readonly DataService _dataService;

        public DeleteController(DataContext dataContext)
        {
            // Instantiate an instance of the data service.
            this._dataService = new DataService(dataContext);
        }

        [HttpGet]
        [Route("delete_generation/{id}")]
        public IActionResult Generation(string id)
        {
            Generation model = this._dataService.GetGeneration(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_generation/{id}")]
        public IActionResult Generation(Generation generation)
        {
            this._dataService.DeleteGeneration(generation.Id);

            return this.RedirectToAction("Generations", "Admin");
        }

        [HttpGet]
        [Route("delete_type/{id:int}")]
        public IActionResult Type(int id)
        {
            Type model = this._dataService.GetType(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_type/{id:int}")]
        public IActionResult Type(Type type)
        {
            this._dataService.DeleteType(type.Id);

            return this.RedirectToAction("Types", "Admin");
        }

        [HttpGet]
        [Route("delete_shiny_hunting_technique/{id:int}")]
        public IActionResult ShinyHuntingTechnique(int id)
        {
            ShinyHuntingTechnique model = this._dataService.GetShinyHuntingTechnique(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_shiny_hunting_technique/{id:int}")]
        public IActionResult ShinyHuntingTechnique(ShinyHuntingTechnique shinyHuntingTechnique)
        {
            this._dataService.DeleteShinyHuntingTechnique(shinyHuntingTechnique.Id);

            return this.RedirectToAction("ShinyHuntingTechniques", "Admin");
        }

        [HttpGet]
        [Route("delete_shiny_hunt/{id:int}")]
        public IActionResult ShinyHunt(int id)
        {
            ShinyHunt model = this._dataService.GetShinyHunt(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_shiny_hunt/{id:int}")]
        public IActionResult ShinyHunt(ShinyHunt shinyHunt)
        {
            this._dataService.DeleteShinyHunt(shinyHunt.Id);

            return this.RedirectToAction("ShinyHunts", "Admin");
        }

        [HttpGet]
        [Route("delete_ability/{id:int}")]
        public IActionResult Ability(int id)
        {
            Ability model = this._dataService.GetAbility(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_ability/{id:int}")]
        public IActionResult Ability(Ability ability)
        {
            this._dataService.DeleteAbility(ability.Id);

            return this.RedirectToAction("Abilities", "Admin");
        }

        [HttpGet]
        [Route("delete_form/{id:int}")]
        public IActionResult Form(int id)
        {
            Form model = this._dataService.GetForm(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_form/{id:int}")]
        public IActionResult Form(Form form)
        {
            this._dataService.DeleteForm(form.Id);

            return this.RedirectToAction("Forms", "Admin");
        }

        [HttpGet]
        [Route("delete_legendary_type/{id:int}")]
        public IActionResult LegendaryType(int id)
        {
            LegendaryType model = this._dataService.GetLegendaryType(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_legendary_type/{id:int}")]
        public IActionResult LegendaryType(LegendaryType legendaryType)
        {
            this._dataService.DeleteLegendaryType(legendaryType.Id);

            return this.RedirectToAction("LegendaryTypes", "Admin");
        }

        [HttpGet]
        [Route("delete_user/{id:int}")]
        public new IActionResult User(int id)
        {
            User model = this._dataService.GetUserById(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_user/{id:int}")]
        public new IActionResult User(User user)
        {
            this._dataService.DeleteUser(user.Id);

            return this.RedirectToAction("Users", "Owner");
        }
        
        [HttpGet]
        [Route("delete_comment/{id:int}")]
        public IActionResult Comment(int id)
        {
            Comment comment = this._dataService.GetComment(id);
            this._dataService.DeleteComment(comment.Id);

            return this.RedirectToAction("Comments", "Owner");
        }

        [HttpGet]
        [Route("delete_egg_group/{id:int}")]
        public IActionResult EggGroup(int id)
        {
            EggGroup model = this._dataService.GetEggGroup(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_egg_group/{id:int}")]
        public IActionResult EggGroup(EggGroup eggGroup)
        {
            this._dataService.DeleteEggGroup(eggGroup.Id);

            return this.RedirectToAction("EggGroups", "Admin");
        }

        [HttpGet]
        [Route("delete_form_item/{id:int}")]
        public IActionResult FormItem(int id)
        {
            FormItem model = this._dataService.GetFormItem(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_form_item/{id:int}")]
        public IActionResult FormItem(FormItem formItem)
        {
            this._dataService.DeleteFormItem(formItem.Id);

            return this.RedirectToAction("FormItems", "Admin");
        }

        [HttpGet]
        [Route("delete_classification/{id:int}")]
        public IActionResult Classification(int id)
        {
            Classification model = this._dataService.GetClassification(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_classification/{id:int}")]
        public IActionResult Classification(Classification classification)
        {
            this._dataService.DeleteClassification(classification.Id);

            return this.RedirectToAction("Classifications", "Admin");
        }
    }
}