using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Pokedex.DataAccess.Models;
using Pokedex.Models;

namespace Pokedex.Controllers
{
    [Authorize]
    [Route("")]
    public class PokemonTeamController : Controller
    {
        private readonly DataService _dataService;

        private readonly AppConfig _appConfig;

        public PokemonTeamController(IOptions<AppConfig> appConfig, DataContext dataContext)
        {
            // Instantiate an instance of the data service.
            this._appConfig = appConfig.Value;
            this._dataService = new DataService(dataContext);
        }

        [HttpGet]
        [Route("create_team")]
        public IActionResult CreateTeam()
        {
            CreatePokemonTeamViewModel model = new CreatePokemonTeamViewModel(){
                AllGenerations = this._dataService.GetGenerations(),
                UserId = this._dataService.GetUserWithUsername(User.Identity.Name).Id,
            };
            
            return this.View(model);
        }

        [HttpPost]
        [Route("create_team")]
        public IActionResult CreateTeam(CreatePokemonTeamViewModel pokemonTeam)
        {
            if (!this.ModelState.IsValid)
            {
                CreatePokemonTeamViewModel model = new CreatePokemonTeamViewModel(){
                    AllGenerations = this._dataService.GetGenerations(),
                    UserId = this._dataService.GetUserWithUsername(User.Identity.Name).Id,
                };
            
                return this.View(model);
            }

            this._dataService.AddPokemonTeam(pokemonTeam);

            return this.RedirectToAction("PokemonTeams", "User");
        }
    }
}