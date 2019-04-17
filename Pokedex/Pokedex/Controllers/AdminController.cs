using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Pokedex.Models;
using Pokedex.DataAccess.Models;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace Pokedex.Controllers
{
    [Authorize(Roles = "Admin,Owner"), Route("admin")]
    public class AdminController : Controller
    {
        private readonly AppConfig _appConfig;

        private readonly DataService _dataService;

        public AdminController(IOptions<AppConfig> appConfig, DataContext dataContext)
        {
            _dataService = new DataService(dataContext);
            _appConfig = appConfig.Value;
        }

        [Route("")]
        public IActionResult Index()
        {
            ViewBag.ApplicationName = _appConfig.AppName;
            ViewBag.ApplicationVersion = _appConfig.AppVersion;

            return View();
        }

        [Route("pokemon")]
        public IActionResult Pokemon()
        {
            List<Pokemon> model = _dataService.GetAllPokemon().Where(x => x.Id.IndexOf('-') == -1).ToList();

            return View(model);
        }

        [Route("generation")]
        public IActionResult Generations()
        {
            GenerationViewModel model = new GenerationViewModel(){
                AllGenerations = _dataService.GetGenerationsWithArchive(),
                AllPokemon = _dataService.GetAllPokemon()
            };

            return View(model);
        }

        [Route("type")]
        public IActionResult Types()
        {
            TypeViewModel model = new TypeViewModel(){
                AllTypes = _dataService.GetTypesWithArchive(),
                AllPokemon = _dataService.GetAllPokemonWithTypes()
            };

            return View(model);
        }

        [Route("ability")]
        public IActionResult Abilities()
        {
            AbilityViewModel model = new AbilityViewModel(){
                AllAbilities = _dataService.GetAbilitiesWithArchive(),
                AllPokemon = _dataService.GetAllPokemonWithAbilities()
            };

            return View(model);
        }

        [Route("egg-group")]
        public IActionResult EggGroups()
        {
            EggGroupViewModel model = new EggGroupViewModel(){
                AllEggGroups = _dataService.GetEggGroupsWithArchive(),
                AllPokemon = _dataService.GetAllPokemonWithEggGroups()
            };

            return View(model);
        }

        [Route("classification")]
        public IActionResult Classifications()
        {
            ClassificationViewModel model = new ClassificationViewModel(){
                AllClassifications = _dataService.GetClassificationsWithArchive(),
                AllPokemon = _dataService.GetAllPokemonWithClassifications()
            };

            return View(model);
        }

        [HttpGet, Route("edit-generation/{id}")]
        public IActionResult EditGeneration(string id)
        {
            Generation model = _dataService.GetGeneration(id);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, Route("edit-generation/{id}")]
        public IActionResult EditGeneration(Generation generation)
        {
            if (!ModelState.IsValid)
            {
                Generation model = _dataService.GetGeneration(generation.Id);

                return View(model);
            }

            _dataService.UpdateGeneration(generation);

            return RedirectToAction("Generations");
        }

        [HttpGet, Route("edit-type/{id:int}")]
        public IActionResult EditType(int id)
        {
            Type model = _dataService.GetType(id);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, Route("edit-type/{id:int}")]
        public IActionResult EditType(Type type)
        {
            if (!ModelState.IsValid)
            {
                Type model = _dataService.GetType(type.Id);

                return View(model);
            }

            _dataService.UpdateType(type);

            return RedirectToAction("Types");
        }

        [HttpGet, Route("edit-egg-group/{id:int}")]
        public IActionResult EditEggGroup(int id)
        {
            EggGroup model = _dataService.GetEggGroup(id);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, Route("edit-egg-group/{id:int}")]
        public IActionResult EditEggGroup(EggGroup eggGroup)
        {
            if (!ModelState.IsValid)
            {
                EggGroup model = _dataService.GetEggGroup(eggGroup.Id);

                return View(model);
            }

            _dataService.UpdateEggGroup(eggGroup);

            return RedirectToAction("EggGroups");
        }

        [HttpGet, Route("edit-classification/{id:int}")]
        public IActionResult EditClassification(int id)
        {
            Classification model = _dataService.GetClassification(id);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, Route("edit-classification/{id:int}")]
        public IActionResult EditClassification(Classification classification)
        {
            if (!ModelState.IsValid)
            {
                Classification model = _dataService.GetClassification(classification.Id);

                return View(model);
            }

            _dataService.UpdateClassification(classification);

            return RedirectToAction("Classifications");
        }

        [HttpGet, Route("edit-ability/{id:int}")]
        public IActionResult EditAbility(int id)
        {
            Ability model = _dataService.GetAbility(id);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, Route("edit-ability/{id:int}")]
        public IActionResult EditAbility(Ability ability)
        {
            if (!ModelState.IsValid)
            {
                Ability model = _dataService.GetAbility(ability.Id);

                return View(model);
            }

            _dataService.UpdateAbility(ability);

            return RedirectToAction("Abilities");
        }

        [HttpGet, Route("archive-generation/{id}")]
        public IActionResult ArchiveGeneration(string id)
        {
            Generation model = _dataService.GetGeneration(id);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, Route("archive-generation/{id}")]
        public IActionResult ArchiveGeneration(Generation generation)
        {
            _dataService.ArchiveGeneration(generation.Id);

            return RedirectToAction("Generations");
        }

        [HttpGet, Route("archive-type/{id:int}")]
        public IActionResult ArchiveType(int id)
        {
            Type model = _dataService.GetType(id);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, Route("archive-type/{id:int}")]
        public IActionResult ArchiveType(Type type)
        {
            _dataService.ArchiveType(type.Id);

            return RedirectToAction("Types");
        }

        [HttpGet, Route("archive-egg-group/{id:int}")]
        public IActionResult ArchiveEggGroup(int id)
        {
            EggGroup model = _dataService.GetEggGroup(id);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, Route("archive-egg-group/{id:int}")]
        public IActionResult ArchiveEggGroup(EggGroup eggGroup)
        {
            _dataService.ArchiveEggGroup(eggGroup.Id);

            return RedirectToAction("EggGroups");
        }

        [HttpGet, Route("archive-classification/{id:int}")]
        public IActionResult ArchiveClassification(int id)
        {
            Classification model = _dataService.GetClassification(id);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, Route("archive-classification/{id:int}")]
        public IActionResult ArchiveClassification(Classification classification)
        {
            _dataService.ArchiveClassification(classification.Id);

            return RedirectToAction("Classifications");
        }

        [HttpGet, Route("archive-ability/{id:int}")]
        public IActionResult ArchiveAbility(int id)
        {
            Ability model = _dataService.GetAbility(id);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, Route("archive-ability/{id:int}")]
        public IActionResult ArchiveAbility(Ability ability)
        {
            _dataService.ArchiveAbility(ability.Id);

            return RedirectToAction("Abilities");
        }

        [HttpGet, Route("unarchive-generation/{id}")]
        public IActionResult UnarchiveGeneration(string id)
        {
            Generation model = _dataService.GetGeneration(id);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, Route("unarchive-generation/{id}")]
        public IActionResult UnarchiveGeneration(Generation generation)
        {
            _dataService.UnarchiveGeneration(generation.Id);

            return RedirectToAction("Generations");
        }

        [HttpGet, Route("unarchive-type/{id:int}")]
        public IActionResult UnarchiveType(int id)
        {
            Type model = _dataService.GetType(id);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, Route("unarchive-type/{id:int}")]
        public IActionResult UnarchiveType(Type type)
        {
            _dataService.UnarchiveType(type.Id);

            return RedirectToAction("Types");
        }

        [HttpGet, Route("unarchive-classification/{id:int}")]
        public IActionResult UnarchiveClassification(int id)
        {
            Classification model = _dataService.GetClassification(id);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, Route("unarchive-classification/{id:int}")]
        public IActionResult UnarchiveType(Classification classification)
        {
            _dataService.UnarchiveClassification(classification.Id);

            return RedirectToAction("Classifications");
        }

        [HttpGet, Route("unarchive-ability/{id:int}")]
        public IActionResult UnarchiveAbility(int id)
        {
            Ability model = _dataService.GetAbility(id);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, Route("unarchive-ability/{id:int}")]
        public IActionResult UnarchiveAbility(Ability ability)
        {
            _dataService.UnarchiveAbility(ability.Id);

            return RedirectToAction("Abilities");
        }

        [HttpGet, Route("unarchive-egg-group/{id:int}")]
        public IActionResult UnarchiveEggGroup(int id)
        {
            EggGroup model = _dataService.GetEggGroup(id);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, Route("unarchive-egg-group/{id:int}")]
        public IActionResult UnarchiveEggGroup(EggGroup eggGroup)
        {
            _dataService.UnarchiveEggGroup(eggGroup.Id);

            return RedirectToAction("EggGroups");
        }

        [Route("error")]
        public IActionResult Error()
        {
            return View();
        }
    }
}
