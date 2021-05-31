using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pokedex.DataAccess.Models;
using Pokedex.Models;
using System.Collections.Generic;

namespace Pokedex.Controllers
{
    [Authorize(Roles = "Owner")]
    [Route("admin")]
    public class DeleteController : Controller
    {
        private readonly DataService dataService;

        public DeleteController(DataContext dataContext)
        {
            // Instantiate an instance of the data service.
            this.dataService = new DataService(dataContext);
        }

        [Route("delete_reviewed_pokemon/{id:int}")]
        public IActionResult ReviewedPokemon(int id)
        {
            this.dataService.DeleteReviewedPokemon(id);

            return this.RedirectToAction("ReviewedPokemon", "Owner");
        }

        [HttpGet]
        [Route("delete_generation/{id:int}")]
        public IActionResult Generation(int id)
        {
            Generation model = this.dataService.GetObjectById<Generation>(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_generation/{id:int}")]
        public IActionResult Generation(Generation generation)
        {
            this.dataService.DeleteGeneration(generation.Id);

            return this.RedirectToAction("Generations", "Admin");
        }

        [HttpGet]
        [Route("delete_pokeball/{id:int}")]
        public IActionResult Pokeball(int id)
        {
            Pokeball pokeball = this.dataService.GetPokeball(id);
            PokeballAdminViewModel model = new PokeballAdminViewModel()
            {
                AllGenerations = this.dataService.GetObjects<Generation>(),
                Id = pokeball.Id,
                Name = pokeball.Name,
                GenerationId = pokeball.GenerationId,
                Generation = pokeball.Generation,
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_pokeball/{id:int}")]
        public IActionResult Pokeball(Pokeball pokeball)
        {
            this.dataService.DeletePokeball(pokeball.Id);

            return this.RedirectToAction("Pokeballs", "Admin");
        }

        [HttpGet]
        [Route("delete_status/{id:int}")]
        public IActionResult Status(int id)
        {
            Status model = this.dataService.GetObjectById<Status>(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_status/{id:int}")]
        public IActionResult Status(Status status)
        {
            this.dataService.DeleteStatus(status.Id);

            return this.RedirectToAction("Statuses", "Admin");
        }

        [HttpGet]
        [Route("delete_pokeball_catch_modifier_detail/{id:int}")]
        public IActionResult PokeballCatchModifierDetail(int id)
        {
            PokeballCatchModifierDetail model = this.dataService.GetPokeballCatchModifierDetail(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_pokeball_catch_modifier_detail/{id:int}")]
        public IActionResult PokeballCatchModifierDetail(PokeballCatchModifierDetail pokeballCatchModifier)
        {
            this.dataService.DeletePokeballCatchModifierDetail(pokeballCatchModifier.Id);

            return this.RedirectToAction("Pokeballs", "Admin");
        }

        [HttpGet]
        [Route("delete_game/{id:int}")]
        public IActionResult Game(int id)
        {
            Game model = this.dataService.GetObjectById<Game>(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_game/{id:int}")]
        public IActionResult Game(Game game)
        {
            this.dataService.DeleteGame(game.Id);

            return this.RedirectToAction("Games", "Admin");
        }

        [HttpGet]
        [Route("delete_type/{id:int}")]
        public IActionResult Type(int id)
        {
            Type model = this.dataService.GetObjectById<Type>(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_type/{id:int}")]
        public IActionResult Type(Type type)
        {
            List<TypeChart> typeCharts = this.dataService.GetAllTypeChartByDefendType(type.Id);
            typeCharts.AddRange(this.dataService.GetAllTypeChartByAttackType(type.Id));
            foreach (var t in typeCharts)
            {
                this.dataService.DeleteTypeChart(t.Id);
            }

            this.dataService.DeleteType(type.Id);

            return this.RedirectToAction("Types", "Admin");
        }

        [HttpGet]
        [Route("delete_nature/{id:int}")]
        public IActionResult Nature(int id)
        {
            Nature model = this.dataService.GetObjectById<Nature>(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_nature/{id:int}")]
        public IActionResult Nature(Nature nature)
        {
            this.dataService.DeleteNature(nature.Id);

            return this.RedirectToAction("Natures", "Admin");
        }

        [HttpGet]
        [Route("delete_egg_cycle/{id:int}")]
        public IActionResult EggCycle(int id)
        {
            EggCycle model = this.dataService.GetObjectById<EggCycle>(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_egg_cycle/{id:int}")]
        public IActionResult EggCycle(EggCycle eggCycle)
        {
            this.dataService.DeleteEggCycle(eggCycle.Id);

            return this.RedirectToAction("EggCycles", "Admin");
        }

        [HttpGet]
        [Route("delete_experience_growth/{id:int}")]
        public IActionResult ExperienceGrowth(int id)
        {
            ExperienceGrowth model = this.dataService.GetObjectById<ExperienceGrowth>(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_experience_growth/{id:int}")]
        public IActionResult ExperienceGrowth(ExperienceGrowth experienceGrowth)
        {
            this.dataService.DeleteExperienceGrowth(experienceGrowth.Id);

            return this.RedirectToAction("ExperienceGrowths", "Admin");
        }

        [HttpGet]
        [Route("delete_gender_ratio/{id:int}")]
        public IActionResult GenderRatio(int id)
        {
            GenderRatio model = this.dataService.GetObjectById<GenderRatio>(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_gender_ratio/{id:int}")]
        public IActionResult GenderRatio(GenderRatio genderRatio)
        {
            this.dataService.DeleteGenderRatio(genderRatio.Id);

            return this.RedirectToAction("GenderRatios", "Admin");
        }

        [HttpGet]
        [Route("delete_capture_rate/{id:int}")]
        public IActionResult CaptureRate(int id)
        {
            CaptureRate model = this.dataService.GetObjectById<CaptureRate>(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_capture_rate/{id:int}")]
        public IActionResult CaptureRate(CaptureRate captureRate)
        {
            this.dataService.DeleteCaptureRate(captureRate.Id);

            return this.RedirectToAction("CaptureRates", "Admin");
        }

        [HttpGet]
        [Route("delete_base_happiness/{id:int}")]
        public IActionResult BaseHappiness(int id)
        {
            BaseHappiness model = this.dataService.GetObjectById<BaseHappiness>(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_base_happiness/{id:int}")]
        public IActionResult BaseHappiness(BaseHappiness baseHappiness)
        {
            this.dataService.DeleteBaseHappiness(baseHappiness.Id);

            return this.RedirectToAction("BaseHappinesses", "Admin");
        }

        [HttpGet]
        [Route("delete_evolution_method/{id:int}")]
        public IActionResult EvolutionMethod(int id)
        {
            EvolutionMethod model = this.dataService.GetObjectById<EvolutionMethod>(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_evolution_method/{id:int}")]
        public IActionResult EvolutionMethod(EvolutionMethod evolutionMethod)
        {
            this.dataService.DeleteEvolutionMethod(evolutionMethod.Id);

            return this.RedirectToAction("EvolutionMethods", "Admin");
        }

        [HttpGet]
        [Route("delete_battle_item/{id:int}")]
        public IActionResult BattleItem(int id)
        {
            BattleItem model = this.dataService.GetObjectById<BattleItem>(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_battle_item/{id:int}")]
        public IActionResult BattleItem(BattleItem battleItem)
        {
            this.dataService.DeleteBattleItem(battleItem.Id);

            return this.RedirectToAction("BattleItems", "Admin");
        }

        [HttpGet]
        [Route("delete_ability/{id:int}")]
        public IActionResult Ability(int id)
        {
            Ability model = this.dataService.GetObjectById<Ability>(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_ability/{id:int}")]
        public IActionResult Ability(Ability ability)
        {
            this.dataService.DeleteAbility(ability.Id);

            return this.RedirectToAction("Abilities", "Admin");
        }

        [HttpGet]
        [Route("delete_comment_page/{id:int}")]
        public IActionResult CommentPage(int id)
        {
            CommentPage model = this.dataService.GetObjectById<CommentPage>(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_comment_page/{id:int}")]
        public IActionResult CommentPage(CommentPage commentPage)
        {
            this.dataService.DeleteCommentPage(commentPage.Id);

            return this.RedirectToAction("CommentPages", "Admin");
        }

        [HttpGet]
        [Route("delete_comment_category/{id:int}")]
        public IActionResult CommentCategory(int id)
        {
            CommentCategory model = this.dataService.GetObjectById<CommentCategory>(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_comment_category/{id:int}")]
        public IActionResult CommentCategory(CommentCategory commentCommentCategory)
        {
            this.dataService.DeleteCommentCategory(commentCommentCategory.Id);

            return this.RedirectToAction("CommentCategories", "Admin");
        }

        [HttpGet]
        [Route("delete_form/{id:int}")]
        public IActionResult Form(int id)
        {
            Form model = this.dataService.GetObjectById<Form>(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_form/{id:int}")]
        public IActionResult Form(Form form)
        {
            this.dataService.DeleteForm(form.Id);

            return this.RedirectToAction("Forms", "Admin");
        }

        [HttpGet]
        [Route("delete_legendary_type/{id:int}")]
        public IActionResult LegendaryType(int id)
        {
            LegendaryType model = this.dataService.GetObjectById<LegendaryType>(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_legendary_type/{id:int}")]
        public IActionResult LegendaryType(LegendaryType legendaryType)
        {
            this.dataService.DeleteLegendaryType(legendaryType.Id);

            return this.RedirectToAction("LegendaryTypes", "Admin");
        }

        [HttpGet]
        [Route("delete_comment/{id:int}")]
        public IActionResult Comment(int id)
        {
            Comment comment = this.dataService.GetObjectById<Comment>(id);
            this.dataService.DeleteComment(comment.Id);

            return this.RedirectToAction("Comments", "Owner");
        }

        [HttpGet]
        [Route("delete_egg_group/{id:int}")]
        public IActionResult EggGroup(int id)
        {
            EggGroup model = this.dataService.GetObjectById<EggGroup>(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_egg_group/{id:int}")]
        public IActionResult EggGroup(EggGroup eggGroup)
        {
            this.dataService.DeleteEggGroup(eggGroup.Id);

            return this.RedirectToAction("EggGroups", "Admin");
        }

        [HttpGet]
        [Route("delete_form_item/{id:int}")]
        public IActionResult FormItem(int id)
        {
            FormItem model = this.dataService.GetObjectById<FormItem>(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_form_item/{id:int}")]
        public IActionResult FormItem(FormItem formItem)
        {
            this.dataService.DeleteFormItem(formItem.Id);

            return this.RedirectToAction("FormItems", "Admin");
        }

        [HttpGet]
        [Route("delete_classification/{id:int}")]
        public IActionResult Classification(int id)
        {
            Classification model = this.dataService.GetObjectById<Classification>(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_classification/{id:int}")]
        public IActionResult Classification(Classification classification)
        {
            this.dataService.DeleteClassification(classification.Id);

            return this.RedirectToAction("Classifications", "Admin");
        }
    }
}
