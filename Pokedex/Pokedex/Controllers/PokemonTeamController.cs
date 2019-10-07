using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Pokedex.DataAccess.Models;

namespace Pokedex.Controllers
{
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
    }
}