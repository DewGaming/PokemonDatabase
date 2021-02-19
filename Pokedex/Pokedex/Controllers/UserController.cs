using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Pokedex.DataAccess.Models;
using Pokedex.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pokedex.Controllers
{
    /// <summary>
    /// The class that is used to represent the user controller.
    /// </summary>
    [Authorize]
    [Route("")]
    public class UserController : Controller
    {
        private readonly DataService dataService;

        private readonly AppConfig appConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="appConfig">The application configuration.</param>
        /// <param name="dataContext">The database context.</param>
        public UserController(IOptions<AppConfig> appConfig, DataContext dataContext)
        {
            this.appConfig = appConfig.Value;
            this.dataService = new DataService(dataContext);
        }

        /// <summary>
        /// Gets a list of messages for the user that is logged in.
        /// </summary>
        /// <returns>The view messages page.</returns>
        [Route("messages")]
        public IActionResult ViewMessages()
        {
            List<Message> model = this.dataService.GetMessagesToUser(Convert.ToInt32(this.User.Claims.First(x => x.Type == "UserId").Value));

            return this.View(model);
        }

        /// <summary>
        /// Deletes the selected message.
        /// </summary>
        /// <param name="messageId">The id of the message being deleted.</param>
        /// <returns>The delete message page.</returns>
        [HttpGet]
        [Route("delete_message/{messageId:int}")]
        public IActionResult DeleteMessage(int messageId)
        {
            Message model = this.dataService.GetMessagesToUser(Convert.ToInt32(this.User.Claims.First(x => x.Type == "UserId").Value))[messageId - 1];

            return this.View(model);
        }

        /// <summary>
        /// Deletes the selected message.
        /// </summary>
        /// <param name="message">The message being deleted.</param>
        /// <returns>The messages page for the user.</returns>
        [HttpPost]
        [Route("delete_message/{messageId:int}")]
        public IActionResult DeleteMessage(Message message)
        {
            this.dataService.DeleteMessage(message.Id);

            return this.RedirectToAction("ViewMessages", "User");
        }

        /// <summary>
        /// Replies to the provided message.
        /// </summary>
        /// <param name="messageId">The id for the message.</param>
        /// <returns>The reply message page.</returns>
        [HttpGet]
        [Route("reply_to_message/{messageId:int}")]
        public IActionResult ReplyMessage(int messageId)
        {
            Message originalMessage = this.dataService.GetMessagesToUser(Convert.ToInt32(this.User.Claims.First(x => x.Type == "UserId").Value))[messageId - 1];
            Message model = new Message()
            {
                ReceiverId = originalMessage.SenderId,
                SenderId = originalMessage.ReceiverId,
                MessageTitle = string.Concat("Re: ", originalMessage.MessageTitle),
            };

            return this.View(model);
        }

        /// <summary>
        /// Replies to the provided message.
        /// </summary>
        /// <param name="message">The message being sent.</param>
        /// <param name="messageId">The id for the message.</param>
        /// <returns>The user's message page.</returns>
        [HttpPost]
        [Route("reply_to_message/{messageId:int}")]
        public IActionResult ReplyMessage(Message message, int messageId)
        {
            if (!this.ModelState.IsValid)
            {
                Message originalMessage = this.dataService.GetMessagesToUser(Convert.ToInt32(this.User.Claims.First(x => x.Type == "UserId").Value))[messageId - 1];

                Message model = new Message()
                {
                    ReceiverId = originalMessage.SenderId,
                    SenderId = originalMessage.ReceiverId,
                    MessageTitle = string.Concat("Re: ", originalMessage.MessageTitle),
                };

                return this.View(model);
            }

            message.Id = 0;
            this.dataService.AddMessage(message);

            return this.RedirectToAction("ViewMessages", "User");
        }

        /// <summary>
        /// Allows the user to change the password they use with the site.
        /// </summary>
        /// <returns>The edit password page.</returns>
        [HttpGet]
        [Route("edit_password")]
        public IActionResult EditPassword()
        {
            NewPasswordViewModel model = new NewPasswordViewModel()
            {
                UserId = this.dataService.GetUserWithUsername(this.User.Identity.Name).Id,
            };

            return this.View(model);
        }

        /// <summary>
        /// Allows the user to change the password they use with the site.
        /// </summary>
        /// <param name="newPasswordViewModel">The newly created password from the user.</param>
        /// <returns>The home page.</returns>
        [HttpPost]
        [Route("edit_password")]
        public IActionResult EditPassword(NewPasswordViewModel newPasswordViewModel)
        {
            User user = this.dataService.GetUserById(newPasswordViewModel.UserId);
            PasswordHasher<string> passwordHasher = new PasswordHasher<string>();
            PasswordVerificationResult passwordVerificationResult =
                passwordHasher.VerifyHashedPassword(null, user.PasswordHash, newPasswordViewModel.OldPassword);

            if (!this.ModelState.IsValid || passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                NewPasswordViewModel model = new NewPasswordViewModel()
                {
                    UserId = newPasswordViewModel.UserId,
                };

                // Set invalid password error message.
                this.ModelState.AddModelError("Error", "Invalid password.");

                return this.View(model);
            }

            user.PasswordHash = passwordHasher.HashPassword(null, newPasswordViewModel.NewPassword);

            this.dataService.UpdateUser(user);

            return this.RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Transport the user to their pokemon team page.
        /// </summary>
        /// <returns>The pokemon team page.</returns>
        [Route("pokemon_teams")]
        public IActionResult PokemonTeams()
        {
            PokemonTeamsViewModel model = new PokemonTeamsViewModel()
            {
                AllPokemonTeams = this.dataService.GetAllPokemonTeams(this.User.Identity.Name),
                AppConfig = this.appConfig,
            };

            return this.View(model);
        }
    }
}
