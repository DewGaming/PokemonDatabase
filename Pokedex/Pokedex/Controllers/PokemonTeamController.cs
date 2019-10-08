using System;
using System.Collections.Generic;
using System.Linq;
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
        public Random rnd = new Random();

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

        [HttpGet]
        [Route("create_pokemon/{teamId:int}")]
        public IActionResult CreatePokemon(int teamId)
        {
            PokemonTeam pokemonTeam = this._dataService.GetPokemonTeam(teamId);
            if(pokemonTeam.SixthPokemonId != null || pokemonTeam.UserId != this._dataService.GetUserWithUsername(User.Identity.Name).Id)
            {
                return this.RedirectToAction("PokemonTeams", "Users");
            }
            else
            {
                List<Pokemon> pokemonList = this._dataService.GetAllPokemon()
                    .Where(x => x.Generation.ReleaseDate <= pokemonTeam.Generation.ReleaseDate &&
                           x.Id != "800-3" && x.Id != "718-2").ToList();
                foreach(var p in pokemonList.Where(x => x.Id.Contains('-')).ToList())
                {
                    p.Name += " (" + this._dataService.GetFormByAltFormId(p.Id).Name + ")";
                }

                foreach(var p in pokemonList.Where(x => x.Name.Contains('_')))
                {
                    p.Name = p.Name.Replace('_', ' ');
                }

                pokemonList = pokemonList.OrderBy(x => x.Name).ToList();
                CreateTeamPokemonViewModel model = new CreateTeamPokemonViewModel(){
                    AllPokemon = pokemonList,
                    AllAbilities = this._dataService.GetAbilities(),
                };

                return this.View(model);
            }
        }

        [HttpPost]
        [Route("create_pokemon/{teamId:int}")]
        public IActionResult CreatePokemon(CreateTeamPokemonViewModel pokemonTeamDetail, int teamId)
        {
            if (!this.ModelState.IsValid)
            {
                CreateTeamPokemonViewModel model = new CreateTeamPokemonViewModel(){
                    AllPokemon = this._dataService.GetAllPokemon(),
                    AllAbilities = this._dataService.GetAbilities(),
                };

                return this.View(model);
            }

            PokemonTeam pokemonTeam = this._dataService.GetPokemonTeam(teamId);

            Pokemon pokemon = this._dataService.GetPokemonById(pokemonTeamDetail.PokemonId);

            if(pokemon.GenderRatioId == 10)
            {
                pokemonTeamDetail.Gender = null;
            }
            else if(string.IsNullOrEmpty(pokemonTeamDetail.Gender))
            {
                pokemonTeamDetail.Gender = pokemonTeamDetail.Genders[rnd.Next(pokemonTeamDetail.Genders.Count)];
            }

            int pokemonTeamDetailId = this._dataService.AddPokemonTeamDetail(pokemonTeamDetail);

            if(pokemonTeam.FirstPokemonId == null)
            {
                pokemonTeam.FirstPokemonId = pokemonTeamDetailId;
            }
            else if(pokemonTeam.SecondPokemonId == null)
            {
                pokemonTeam.SecondPokemonId = pokemonTeamDetailId;
            }
            else if(pokemonTeam.ThirdPokemonId == null)
            {
                pokemonTeam.ThirdPokemonId = pokemonTeamDetailId;
            }
            else if(pokemonTeam.FourthPokemonId == null)
            {
                pokemonTeam.FourthPokemonId = pokemonTeamDetailId;
            }
            else if(pokemonTeam.FifthPokemonId == null)
            {
                pokemonTeam.FifthPokemonId = pokemonTeamDetailId;
            }
            else if(pokemonTeam.SixthPokemonId == null)
            {
                pokemonTeam.SixthPokemonId = pokemonTeamDetailId;
            }

            this._dataService.UpdatePokemonTeam(pokemonTeam);

            return this.RedirectToAction("PokemonTeams", "User");
        }
    }
}