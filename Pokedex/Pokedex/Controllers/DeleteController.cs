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
            this.dataService.DeleteObject<ReviewedPokemon>(id);

            return this.RedirectToAction("ReviewedPokemon", "Owner");
        }

        [HttpGet]
        [Route("delete_generation/{id:int}")]
        public IActionResult Generation(int id)
        {
            Generation model = this.dataService.GetObjectByPropertyValue<Generation>("Id", id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_generation/{id:int}")]
        public IActionResult Generation(Generation generation)
        {
            this.dataService.DeleteObject<Generation>(generation.Id);

            return this.RedirectToAction("Generations", "Admin");
        }

        [HttpGet]
        [Route("delete_pokeball/{id:int}")]
        public IActionResult Pokeball(int id)
        {
            Pokeball pokeball = this.dataService.GetObjectByPropertyValue<Pokeball>("Id", id, "Generation");
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
            this.dataService.DeleteObject<Pokeball>(pokeball.Id);

            return this.RedirectToAction("Pokeballs", "Admin");
        }

        [HttpGet]
        [Route("delete_region/{id:int}")]
        public IActionResult Region(int id)
        {
            Region region = this.dataService.GetObjectByPropertyValue<Region>("Id", id, "Generation");
            RegionAdminViewModel model = new RegionAdminViewModel()
            {
                AllGenerations = this.dataService.GetObjects<Generation>(),
                Id = region.Id,
                Name = region.Name,
                GenerationId = region.GenerationId,
                Generation = region.Generation,
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_region/{id:int}")]
        public IActionResult Region(Region region)
        {
            this.dataService.DeleteObject<Region>(region.Id);

            return this.RedirectToAction("Regions", "Admin");
        }

        [HttpGet]
        [Route("delete_pageStat/{id:int}")]
        public IActionResult PageStat(int id)
        {
            PageStat model = this.dataService.GetObjectByPropertyValue<PageStat>("Id", id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_pageStat/{id:int}")]
        public IActionResult PageStat(PageStat pageStat)
        {
            this.dataService.DeleteObject<PageStat>(pageStat.Id);

            return this.RedirectToAction("PageStats", "Owner");
        }

        [HttpGet]
        [Route("delete_status/{id:int}")]
        public IActionResult Status(int id)
        {
            Status model = this.dataService.GetObjectByPropertyValue<Status>("Id", id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_status/{id:int}")]
        public IActionResult Status(Status status)
        {
            this.dataService.DeleteObject<Status>(status.Id);

            return this.RedirectToAction("Statuses", "Admin");
        }

        [HttpGet]
        [Route("delete_pokeball_catch_modifier_detail/{id:int}")]
        public IActionResult PokeballCatchModifierDetail(int id)
        {
            PokeballCatchModifierDetail model = this.dataService.GetObjectByPropertyValue<PokeballCatchModifierDetail>("Id", id, "Pokeball");

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_pokeball_catch_modifier_detail/{id:int}")]
        public IActionResult PokeballCatchModifierDetail(PokeballCatchModifierDetail pokeballCatchModifier)
        {
            this.dataService.DeleteObject<PokeballCatchModifierDetail>(pokeballCatchModifier.Id);

            return this.RedirectToAction("Pokeballs", "Admin");
        }

        [HttpGet]
        [Route("delete_game/{id:int}")]
        public IActionResult Game(int id)
        {
            Game model = this.dataService.GetObjectByPropertyValue<Game>("Id", id);

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
            Type model = this.dataService.GetObjectByPropertyValue<Type>("Id", id);

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
                this.dataService.DeleteObject<TypeChart>(t.Id);
            }

            this.dataService.DeleteObject<Type>(type.Id);

            return this.RedirectToAction("Types", "Admin");
        }

        [HttpGet]
        [Route("delete_nature/{id:int}")]
        public IActionResult Nature(int id)
        {
            Nature model = this.dataService.GetObjectByPropertyValue<Nature>("Id", id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_nature/{id:int}")]
        public IActionResult Nature(Nature nature)
        {
            this.dataService.DeleteObject<Nature>(nature.Id);

            return this.RedirectToAction("Natures", "Admin");
        }

        [HttpGet]
        [Route("delete_egg_cycle/{id:int}")]
        public IActionResult EggCycle(int id)
        {
            EggCycle model = this.dataService.GetObjectByPropertyValue<EggCycle>("Id", id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_egg_cycle/{id:int}")]
        public IActionResult EggCycle(EggCycle eggCycle)
        {
            this.dataService.DeleteObject<EggCycle>(eggCycle.Id);

            return this.RedirectToAction("EggCycles", "Admin");
        }

        [HttpGet]
        [Route("delete_experience_growth/{id:int}")]
        public IActionResult ExperienceGrowth(int id)
        {
            ExperienceGrowth model = this.dataService.GetObjectByPropertyValue<ExperienceGrowth>("Id", id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_experience_growth/{id:int}")]
        public IActionResult ExperienceGrowth(ExperienceGrowth experienceGrowth)
        {
            this.dataService.DeleteObject<ExperienceGrowth>(experienceGrowth.Id);

            return this.RedirectToAction("ExperienceGrowths", "Admin");
        }

        [HttpGet]
        [Route("delete_gender_ratio/{id:int}")]
        public IActionResult GenderRatio(int id)
        {
            GenderRatio model = this.dataService.GetObjectByPropertyValue<GenderRatio>("Id", id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_gender_ratio/{id:int}")]
        public IActionResult GenderRatio(GenderRatio genderRatio)
        {
            this.dataService.DeleteObject<GenderRatio>(genderRatio.Id);

            return this.RedirectToAction("GenderRatios", "Admin");
        }

        [HttpGet]
        [Route("delete_capture_rate/{id:int}")]
        public IActionResult CaptureRate(int id)
        {
            CaptureRate model = this.dataService.GetObjectByPropertyValue<CaptureRate>("Id", id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_capture_rate/{id:int}")]
        public IActionResult CaptureRate(CaptureRate captureRate)
        {
            this.dataService.DeleteObject<CaptureRate>(captureRate.Id);

            return this.RedirectToAction("CaptureRates", "Admin");
        }

        [HttpGet]
        [Route("delete_base_happiness/{id:int}")]
        public IActionResult BaseHappiness(int id)
        {
            BaseHappiness model = this.dataService.GetObjectByPropertyValue<BaseHappiness>("Id", id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_base_happiness/{id:int}")]
        public IActionResult BaseHappiness(BaseHappiness baseHappiness)
        {
            this.dataService.DeleteObject<BaseHappiness>(baseHappiness.Id);

            return this.RedirectToAction("BaseHappinesses", "Admin");
        }

        [HttpGet]
        [Route("delete_evolution_method/{id:int}")]
        public IActionResult EvolutionMethod(int id)
        {
            EvolutionMethod model = this.dataService.GetObjectByPropertyValue<EvolutionMethod>("Id", id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_evolution_method/{id:int}")]
        public IActionResult EvolutionMethod(EvolutionMethod evolutionMethod)
        {
            this.dataService.DeleteObject<EvolutionMethod>(evolutionMethod.Id);

            return this.RedirectToAction("EvolutionMethods", "Admin");
        }

        [HttpGet]
        [Route("delete_battle_item/{id:int}")]
        public IActionResult BattleItem(int id)
        {
            BattleItem model = this.dataService.GetObjectByPropertyValue<BattleItem>("Id", id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_battle_item/{id:int}")]
        public IActionResult BattleItem(BattleItem battleItem)
        {
            this.dataService.DeleteObject<BattleItem>(battleItem.Id);

            return this.RedirectToAction("BattleItems", "Admin");
        }

        [HttpGet]
        [Route("delete_ability/{id:int}")]
        public IActionResult Ability(int id)
        {
            Ability model = this.dataService.GetObjectByPropertyValue<Ability>("Id", id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_ability/{id:int}")]
        public IActionResult Ability(Ability ability)
        {
            this.dataService.DeleteObject<Ability>(ability.Id);

            return this.RedirectToAction("Abilities", "Admin");
        }

        [HttpGet]
        [Route("delete_comment_page/{id:int}")]
        public IActionResult CommentPage(int id)
        {
            CommentPage model = this.dataService.GetObjectByPropertyValue<CommentPage>("Id", id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_comment_page/{id:int}")]
        public IActionResult CommentPage(CommentPage commentPage)
        {
            this.dataService.DeleteObject<CommentPage>(commentPage.Id);

            return this.RedirectToAction("CommentPages", "Admin");
        }

        [HttpGet]
        [Route("delete_comment_category/{id:int}")]
        public IActionResult CommentCategory(int id)
        {
            CommentCategory model = this.dataService.GetObjectByPropertyValue<CommentCategory>("Id", id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_comment_category/{id:int}")]
        public IActionResult CommentCategory(CommentCategory commentCommentCategory)
        {
            this.dataService.DeleteObject<CommentCategory>(commentCommentCategory.Id);

            return this.RedirectToAction("CommentCategories", "Admin");
        }

        [HttpGet]
        [Route("delete_form/{id:int}")]
        public IActionResult Form(int id)
        {
            Form model = this.dataService.GetObjectByPropertyValue<Form>("Id", id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_form/{id:int}")]
        public IActionResult Form(Form form)
        {
            this.dataService.DeleteObject<Form>(form.Id);

            return this.RedirectToAction("Forms", "Admin");
        }

        [HttpGet]
        [Route("delete_legendary_type/{id:int}")]
        public IActionResult LegendaryType(int id)
        {
            LegendaryType model = this.dataService.GetObjectByPropertyValue<LegendaryType>("Id", id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_legendary_type/{id:int}")]
        public IActionResult LegendaryType(LegendaryType legendaryType)
        {
            this.dataService.DeleteObject<LegendaryType>(legendaryType.Id);

            return this.RedirectToAction("LegendaryTypes", "Admin");
        }

        [HttpGet]
        [Route("delete_comment/{id:int}")]
        public IActionResult Comment(int id)
        {
            Comment comment = this.dataService.GetObjectByPropertyValue<Comment>("Id", id);
            this.dataService.DeleteObject<Comment>(comment.Id);

            return this.RedirectToAction("Comments", "Owner");
        }

        [HttpGet]
        [Route("delete_egg_group/{id:int}")]
        public IActionResult EggGroup(int id)
        {
            EggGroup model = this.dataService.GetObjectByPropertyValue<EggGroup>("Id", id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_egg_group/{id:int}")]
        public IActionResult EggGroup(EggGroup eggGroup)
        {
            this.dataService.DeleteObject<EggGroup>(eggGroup.Id);

            return this.RedirectToAction("EggGroups", "Admin");
        }

        [HttpGet]
        [Route("delete_form_item/{id:int}")]
        public IActionResult FormItem(int id)
        {
            FormItem model = this.dataService.GetObjectByPropertyValue<FormItem>("Id", id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_form_item/{id:int}")]
        public IActionResult FormItem(FormItem formItem)
        {
            this.dataService.DeleteObject<FormItem>(formItem.Id);

            return this.RedirectToAction("FormItems", "Admin");
        }

        [HttpGet]
        [Route("delete_classification/{id:int}")]
        public IActionResult Classification(int id)
        {
            Classification model = this.dataService.GetObjectByPropertyValue<Classification>("Id", id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_classification/{id:int}")]
        public IActionResult Classification(Classification classification)
        {
            this.dataService.DeleteObject<Classification>(classification.Id);

            return this.RedirectToAction("Classifications", "Admin");
        }
    }
}
