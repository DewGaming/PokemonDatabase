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
        private static List<ShinyHunt> _shinyHunts;

        private readonly DataService _dataService;

        private readonly AppConfig _appConfig;

        public UserController(IOptions<AppConfig> appConfig, DataContext dataContext)
        {
            this._appConfig = appConfig.Value;
            this._dataService = new DataService(dataContext);
        }

        [Route("messages")]
        public IActionResult ViewMessages()
        {
            List<Message> model = this._dataService.GetMessagesToUsername(this.User.Identity.Name);

            return this.View(model);
        }

        [HttpGet]
        [Route("delete_message/{id:int}")]
        public IActionResult DeleteMessage(int id)
        {
            Message model = this._dataService.GetMessage(id);

            return this.View(model);
        }

        [HttpPost]
        [Route("delete_message/{id:int}")]
        public IActionResult DeleteMessage(Message message)
        {
            this._dataService.DeleteMessage(message.Id);

            return this.RedirectToAction("ViewMessages", "User");
        }

        [HttpGet]
        [Route("reply_to_message/{id:int}")]
        public IActionResult ReplyMessage(int id)
        {
            Message originalMessage = this._dataService.GetMessage(id);
            Message model = new Message() {
                ReceiverId = originalMessage.SenderId,
                SenderId = originalMessage.ReceiverId,
                MessageTitle = string.Concat("Re: ", originalMessage.MessageTitle),
            };

            return this.View(model);
        }

        [HttpPost]
        [Route("reply_to_message/{id:int}")]
        public IActionResult ReplyMessage(Message message, int id)
        {
            if (!this.ModelState.IsValid)
            {
                Message originalMessage = this._dataService.GetMessage(id);

                Message model = new Message()
                {
                    ReceiverId = originalMessage.SenderId,
                    SenderId = originalMessage.ReceiverId,
                    MessageTitle = string.Concat("Re: ", originalMessage.MessageTitle),
                };

                return this.View(model);
            }
            
            message.Id = 0;
            this._dataService.AddMessage(message);

            return this.RedirectToAction("ViewMessages", "User");
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
            List<ShinyHunt> shinyHunts = this._dataService.GetShinyHunter(this.User.Identity.Name);
            ShinyHuntingViewModel model = new ShinyHuntingViewModel(){
                InProgressHunts = shinyHunts.Where(x => !x.HuntComplete).ToList(),
                CompletedHunts = shinyHunts.Where(x => x.HuntComplete && x.IsPokemonCaught).ToList(),
                FailedHunts = shinyHunts.Where(x => x.HuntComplete && !x.IsPokemonCaught).ToList(),
            };

            return this.View(model);
        }

        [Route("shiny_hunt/{huntId:int}")]
        public IActionResult ContinueHunt(int huntId)
        {
            this.UpdateShinyHuntList();
            ShinyHunt shinyHunt = _shinyHunts.Where(x => !x.HuntComplete).ToList()[huntId - 1];

            ContinueHuntViewModel model = new ContinueHuntViewModel(){
                ShinyHunt = shinyHunt,
                AppConfig = this._appConfig,
                HuntIndex = huntId,
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

        [Route("complete_shiny_hunt/{huntId:int}")]
        public IActionResult CompleteShinyHunt(int huntId)
        {
            this.UpdateShinyHuntList();
            ShinyHunt shinyHunt = _shinyHunts.Where(x => !x.HuntComplete).ToList()[huntId - 1];
            shinyHunt.HuntComplete = true;
            shinyHunt.IsPokemonCaught = true;

            this._dataService.UpdateShinyHunt(shinyHunt);

            return this.RedirectToAction("ShinyHuntingCounter");
        }

        [Route("give_up_shiny_hunt/{huntId:int}")]
        public IActionResult GiveUpShinyHunt(int huntId)
        {
            this.UpdateShinyHuntList();
            ShinyHunt shinyHunt = _shinyHunts.Where(x => !x.HuntComplete).ToList()[huntId - 1];
            shinyHunt.HuntComplete = true;
            shinyHunt.IsPokemonCaught = false;

            this._dataService.UpdateShinyHunt(shinyHunt);

            return this.RedirectToAction("ShinyHuntingCounter");
        }

        [Route("remove_hunt/{huntProgress}/{huntId:int}")]
        public IActionResult RemoveHunt(string huntProgress, int huntId)
        {
            this.UpdateShinyHuntList();
            ShinyHunt shinyHunt = null;
            if(huntProgress == "InProgress")
            {
                shinyHunt = _shinyHunts.Where(x => !x.HuntComplete).ToList()[huntId - 1];
            }
            else if(huntProgress == "Completed")
            {
                shinyHunt = _shinyHunts.Where(x => x.HuntComplete && x.IsPokemonCaught).ToList()[huntId - 1];
            }
            else if(huntProgress == "Failed")
            {
                shinyHunt = _shinyHunts.Where(x => x.HuntComplete && !x.IsPokemonCaught).ToList()[huntId - 1];
            }

            if (shinyHunt != null)
            {
                this._dataService.DeleteShinyHunt(shinyHunt.Id);
            }

            return this.RedirectToAction("ShinyHuntingCounter");
        }

        [Route("retry_hunt/{huntId:int}")]
        public IActionResult RetryHunt(int huntId)
        {
            this.UpdateShinyHuntList();
            ShinyHunt hunt = _shinyHunts.Where(x => x.HuntComplete && !x.IsPokemonCaught).ToList()[huntId - 1];
            hunt.IsPokemonCaught = false;
            hunt.HuntComplete = false;

            this._dataService.UpdateShinyHunt(hunt);

            return this.RedirectToAction("ShinyHuntingCounter", "User");
        }

        [Route("pokemon_teams")]
        public IActionResult PokemonTeams()
        {
            PokemonTeamsViewModel model = new PokemonTeamsViewModel(){
                AllPokemonTeams = this._dataService.GetAllPokemonTeams(this.User.Identity.Name),
                AppConfig = _appConfig,
            };

            return this.View(model);
        }

        private void UpdateShinyHuntList()
        {
            _shinyHunts = this._dataService.GetShinyHunter(User.Identity.Name);
        }
    }
}
