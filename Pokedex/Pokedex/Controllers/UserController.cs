using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Pokedex.DataAccess.Models;

using Pokedex.Models;

namespace Pokedex.Controllers
{
    [Authorize]
    [Route("")]
    public class UserController : Controller
    {
        private readonly DataService _dataService;

        private readonly AppConfig _appConfig;

        public UserController(IOptions<AppConfig> appConfig, DataContext dataContext)
        {
            this._appConfig = appConfig.Value;
            this._dataService = new DataService(dataContext);
        }

        [HttpGet]
        [Route("edit_password")]
        public IActionResult EditPassword()
        {
            NewPasswordViewModel model = new NewPasswordViewModel()
            {
                UserId = this._dataService.GetUserWithUsername(this.User.Identity.Name).Id
            };

            return this.View(model);
        }

        [HttpPost]
        [Route("edit_password")]
        public IActionResult EditPassword(NewPasswordViewModel newPasswordViewModel)
        {
            User user = this._dataService.GetUserById(newPasswordViewModel.UserId);
            PasswordHasher<string> passwordHasher = new PasswordHasher<string>();
            PasswordVerificationResult passwordVerificationResult =
                passwordHasher.VerifyHashedPassword(null, user.PasswordHash, newPasswordViewModel.OldPassword);

            if (!this.ModelState.IsValid || passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                NewPasswordViewModel model = new NewPasswordViewModel()
                {
                    UserId = newPasswordViewModel.UserId
                };
                // Set invalid password error message.
                this.ModelState.AddModelError("Error", "Invalid password.");

                return this.View(model);
            }

            user.PasswordHash = passwordHasher.HashPassword(null, newPasswordViewModel.NewPassword);

            this._dataService.UpdateUser(user);

            return this.RedirectToAction("Index", "Home");
        }
        
        [Authorize]
        [Route("shiny_hunting_counter")]
        public IActionResult ShinyHuntingCounter()
        {
            List<ShinyHunt> model = this._dataService.GetShinyHunter(this.User.Identity.Name);
            return this.View(model);
        }

        [Route("shiny_hunt/{id:int}")]
        public IActionResult ContinueHunt(int id)
        {
            ShinyHunt shinyHunt = this._dataService.GetShinyHunt(id);

            ContinueHuntViewModel model = new ContinueHuntViewModel(){
                ShinyHunt = shinyHunt,
                AppConfig = this._appConfig,
            };

            return this.View(model);
        }

        [HttpGet]
        [Route("begin_shiny_hunt")]
        public IActionResult BeginShinyHunt()
        {
            BeginShinyHuntViewModel model = new BeginShinyHuntViewModel()
            {
                UserId = this._dataService.GetUserWithUsername(this.User.Identity.Name).Id,
                AllShinyHuntingTechniques = this._dataService.GetShinyHuntingTechniques(),
                AllGenerations = this._dataService.GetGenerations(),
                AllPokemon = new List<Pokemon>(),
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("begin_shiny_hunt")]
        public IActionResult BeginShinyHunt(ShinyHunt shinyHunt)
        {
            List<Generation> generations = this._dataService.GetGenerations().OrderBy(p => p.Id).ToList();
            if (generations.IndexOf(this._dataService.GetGeneration(shinyHunt.GenerationId)) < generations.IndexOf(this._dataService.GetGenerationByPokemon(shinyHunt.PokemonId)))
            {
                BeginShinyHuntViewModel model = new BeginShinyHuntViewModel()
                {
                    UserId = this._dataService.GetUserWithUsername(this.User.Identity.Name).Id,
                    AllShinyHuntingTechniques = this._dataService.GetShinyHuntingTechniques(),
                    AllPokemon = this._dataService.GetAllPokemon(),
                    AllGenerations = this._dataService.GetGenerations(),
                };

                this.ModelState.AddModelError("GenerationId", "Pokemon does not exist in this generation");

                return this.View(model);
            }

            this._dataService.AddShinyHunt(shinyHunt);

            return this.RedirectToAction("ShinyHuntingCounter");
        }

        [HttpPost]
        [Route("update_pokemon_list/{generationId}")]
        public List<Pokemon> UpdatePokemonList(string generationId)
        {
            Generation gen = this._dataService.GetGeneration(generationId);
            List<Pokemon> pokemon = this._dataService.GetAllPokemon().Where(x => x.Generation.ReleaseDate <= gen.ReleaseDate && !x.Id.Contains('-')).ToList();
            return pokemon;
        }

        [HttpGet]
        [Route("end_shiny_hunt/{id:int}")]
        public IActionResult EndShinyHunt(int id)
        {
            EndShinyHuntViewModel model = new EndShinyHuntViewModel()
            {
                ShinyHuntId = id,
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("end_shiny_hunt/{id:int}")]
        public IActionResult EndShinyHunt(EndShinyHuntViewModel endShinyHuntViewModel)
        {
            ShinyHunt shinyHunt = this._dataService.GetShinyHunt(endShinyHuntViewModel.ShinyHuntId);
            shinyHunt.HuntComplete = true;
            shinyHunt.IsPokemonCaught = endShinyHuntViewModel.HuntSuccessful;

            this._dataService.UpdateShinyHunt(shinyHunt);

            return this.RedirectToAction("ShinyHuntingCounter");
        }

        [Route("remove_hunt/{id:int}")]
        public IActionResult RemoveHunt(int id)
        {
            ShinyHunt shinyHunt = this._dataService.GetShinyHunt(id);

            if (shinyHunt != null)
            {
                this._dataService.ArchiveShinyHunt(shinyHunt.Id);
            }

            return this.RedirectToAction("ShinyHuntingCounter");
        }

        [HttpPost]
        [Route("add-hunt-attempt/{huntId:int}")]
        public int AddShinyCounter(int huntId)
        {
            ShinyHunt hunt = this._dataService.GetShinyHunt(huntId);
            hunt.ShinyAttemptCount++;
            this._dataService.UpdateShinyHunt(hunt);
            return hunt.ShinyAttemptCount;
        }

        [HttpPost]
        [Route("subtract-hunt-attempt/{huntId:int}")]
        public int SubtractShinyCounter(int huntId)
        {
            ShinyHunt hunt = this._dataService.GetShinyHunt(huntId);
            if (hunt.ShinyAttemptCount > 0)
            {
                hunt.ShinyAttemptCount--;
                this._dataService.UpdateShinyHunt(hunt);
            }
            else
            {
                hunt.ShinyAttemptCount = 0;
            }

            return hunt.ShinyAttemptCount;
        }

        [HttpPost]
        [Route("update-hunt-attempt/{huntId:int}/{attemptCount:int}")]
        public int UpdateShinyCounter(int huntId, int attemptCount)
        {
            ShinyHunt hunt = this._dataService.GetShinyHunt(huntId);
            if (attemptCount > 0)
            {
                hunt.ShinyAttemptCount = attemptCount;
                this._dataService.UpdateShinyHunt(hunt);
            }
            else
            {
                hunt.ShinyAttemptCount = 0;
                this._dataService.UpdateShinyHunt(hunt);
            }

            return hunt.ShinyAttemptCount;
        }

        [Route("retry-hunt/{huntId:int}")]
        public IActionResult RetryHunt(int huntId)
        {
            ShinyHunt hunt = this._dataService.GetShinyHunt(huntId);
            hunt.HuntRetried = true;

            this._dataService.UpdateShinyHunt(hunt);

            ShinyHunt newHunt = new ShinyHunt()
            {
                PokemonId = hunt.PokemonId,
                ShinyHuntingTechniqueId = hunt.ShinyHuntingTechniqueId,
                GenerationId = hunt.GenerationId,
                UserId = hunt.UserId,
                HasShinyCharm = hunt.HasShinyCharm
            };

            this._dataService.AddShinyHunt(newHunt);

            return this.RedirectToAction("ContinueHunt", "User", new { id = newHunt.Id });
        }

        [AllowAnonymous]
        [Route("error")]
        public IActionResult Error()
        {
            return this.View();
        }
    }
}
