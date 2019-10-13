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
            if (this._dataService.GetUserWithUsername(User.Identity.Name) == null)
            {
                return this.RedirectToAction("Index", "Home");
            }
            else if(pokemonTeam.SixthPokemonId != null || pokemonTeam.UserId != this._dataService.GetUserWithUsername(User.Identity.Name).Id)
            {
                return this.RedirectToAction("PokemonTeams", "User");
            }
            else
            {
                List<Pokemon> pokemonList = new List<Pokemon>();
                if(pokemonTeam.Generation != null)
                {
                    pokemonList = this._dataService.GetAllPokemon()
                    .Where(x => x.Generation.ReleaseDate <= pokemonTeam.Generation.ReleaseDate &&
                           x.Id != "800-3" && x.Id != "718-2").ToList();
                }
                else
                {
                    pokemonList = this._dataService.GetAllPokemon()
                    .Where(x => x.Id != "800-3" && x.Id != "718-2").ToList();
                }

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

            int pokemonTeamDetailId = this._dataService.AddPokemonTeamDetail(pokemonTeamDetail);

            pokemonTeam.InsertPokemon(this._dataService.GetPokemonTeamDetail(pokemonTeamDetailId));

            this._dataService.UpdatePokemonTeam(pokemonTeam);

            return this.RedirectToAction("PokemonTeams", "User");
        }

        [HttpGet]
        [Route("update_pokemon/{pokemonId:int}")]
        public IActionResult EditPokemon(int pokemonId)
        {
            PokemonTeamDetail pokemonTeamDetail = this._dataService.GetPokemonTeamDetail(pokemonId);
            PokemonTeam pokemonTeam = this._dataService.GetPokemonTeamFromPokemon(pokemonTeamDetail.Id);
            if (pokemonTeam.UserId != this._dataService.GetUserWithUsername(User.Identity.Name).Id)
            {
                return this.RedirectToAction("PokemonTeams", "User");
            }
            else
            {
                List<Pokemon> pokemonList = new List<Pokemon>();
                if(pokemonTeam.Generation != null)
                {
                    pokemonList = this._dataService.GetAllPokemon()
                    .Where(x => x.Generation.ReleaseDate <= pokemonTeam.Generation.ReleaseDate &&
                           x.Id != "800-3" && x.Id != "718-2").ToList();
                }
                else
                {
                    pokemonList = this._dataService.GetAllPokemon()
                    .Where(x => x.Id != "800-3" && x.Id != "718-2").ToList();
                }

                foreach(var p in pokemonList.Where(x => x.Id.Contains('-')).ToList())
                {
                    p.Name += " (" + this._dataService.GetFormByAltFormId(p.Id).Name + ")";
                }

                foreach(var p in pokemonList.Where(x => x.Name.Contains('_')))
                {
                    p.Name = p.Name.Replace('_', ' ');
                }

                pokemonList = pokemonList.OrderBy(x => x.Name).ToList();
                UpdateTeamPokemonViewModel model = new UpdateTeamPokemonViewModel(){
                    PokemonTeamDetail = pokemonTeamDetail,
                    AllPokemon = pokemonList,
                    AllAbilities = this._dataService.GetAbilities(),
                };

                return this.View(model);
            }
        }

        [HttpPost]
        [Route("update_pokemon/{pokemonId:int}")]
        public IActionResult EditPokemon(PokemonTeamDetail pokemonTeamDetail)
        {
            PokemonTeam pokemonTeam = this._dataService.GetPokemonTeamFromPokemon(pokemonTeamDetail.Id);
            if (!this.ModelState.IsValid)
            {
                List<Pokemon> pokemonList = new List<Pokemon>();
                if(pokemonTeam.Generation != null)
                {
                    pokemonList = this._dataService.GetAllPokemon()
                    .Where(x => x.Generation.ReleaseDate <= pokemonTeam.Generation.ReleaseDate &&
                           x.Id != "800-3" && x.Id != "718-2").ToList();
                }
                else
                {
                    pokemonList = this._dataService.GetAllPokemon()
                    .Where(x => x.Id != "800-3" && x.Id != "718-2").ToList();
                }

                UpdateTeamPokemonViewModel model = new UpdateTeamPokemonViewModel(){
                    PokemonTeamDetail = pokemonTeamDetail,
                    AllPokemon = pokemonList,
                    AllAbilities = this._dataService.GetAbilities(),
                };

                return this.View(model);
            }


            Pokemon pokemon = this._dataService.GetPokemonById(pokemonTeamDetail.PokemonId);

            if(pokemon.GenderRatioId == 10)
            {
                pokemonTeamDetail.Gender = null;
            }

            this._dataService.UpdatePokemonTeamDetail(pokemonTeamDetail);

            return this.RedirectToAction("PokemonTeams", "User");
        }

        [HttpGet]
        [Route("delete_team/{teamId:int}")]
        public IActionResult DeleteTeam(int teamId)
        {
            PokemonTeam pokemonTeam = this._dataService.GetPokemonTeam(teamId);

            if(pokemonTeam.UserId != this._dataService.GetUserWithUsername(User.Identity.Name).Id)
            {
                return this.RedirectToAction("PokemonTeams", "User");
            }
            else
            {
                PokemonTeamViewModel model = new PokemonTeamViewModel(){
                    Id = pokemonTeam.Id,
                    PokemonTeamName = pokemonTeam.PokemonTeamName,
                    FirstPokemon = pokemonTeam.FirstPokemon,
                    SecondPokemon = pokemonTeam.SecondPokemon,
                    ThirdPokemon = pokemonTeam.ThirdPokemon,
                    FourthPokemon = pokemonTeam.FourthPokemon,
                    FifthPokemon = pokemonTeam.FifthPokemon,
                    SixthPokemon = pokemonTeam.SixthPokemon,
                    AppConfig = this._appConfig,
                };

                return this.View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_team/{teamId:int}")]
        public IActionResult DeleteTeam(PokemonTeam pokemonTeam)
        {
            this._dataService.DeletePokemonTeam(pokemonTeam.Id);

            return this.RedirectToAction("PokemonTeams", "User");
        }

        [HttpGet]
        [Route("update_ev/{evId:int}")]
        public IActionResult EditEV(int evId)
        {
            PokemonTeam pokemonTeam = this._dataService.GetPokemonTeamFromPokemonEV(evId);
            if(pokemonTeam.UserId != this._dataService.GetUserWithUsername(User.Identity.Name).Id)
            {
                return this.RedirectToAction("PokemonTeams", "User");
            }
            else
            {
                PokemonTeamEV pokemonEV = this._dataService.GetPokemonTeamEV(evId);
                PokemonTeamEVViewModel model = new PokemonTeamEVViewModel(){
                    Id = pokemonEV.Id,
                    Health = pokemonEV.Health,
                    Attack = pokemonEV.Attack,
                    Defense = pokemonEV.Defense,
                    SpecialAttack = pokemonEV.SpecialAttack,
                    SpecialDefense = pokemonEV.SpecialDefense,
                    Speed = pokemonEV.Speed,
                    PokemonId = evId,
                };

                return this.View(model);
            }
        }

        [HttpPost]
        [Route("update_ev/{evId:int}")]
        public IActionResult EditEV(PokemonTeamEVViewModel pokemonTeamEV)
        {
            if (!this.ModelState.IsValid)
            {
                PokemonTeamEV pokemon = this._dataService.GetPokemonTeamEV(pokemonTeamEV.PokemonId);
                PokemonTeamEVViewModel model = new PokemonTeamEVViewModel(){
                    Id = pokemon.Id,
                    Health = pokemon.Health,
                    Attack = pokemon.Attack,
                    Defense = pokemon.Defense,
                    SpecialAttack = pokemon.SpecialAttack,
                    SpecialDefense = pokemon.SpecialDefense,
                    Speed = pokemon.Speed,
                    PokemonId = pokemonTeamEV.PokemonId,
                };
            
                return this.View(model);
            }
            else if (pokemonTeamEV.EVTotal > 510)
            {
                PokemonTeamEV pokemon = this._dataService.GetPokemonTeamEV(pokemonTeamEV.PokemonId);
                PokemonTeamEVViewModel model = new PokemonTeamEVViewModel(){
                    Id = pokemon.Id,
                    Health = pokemon.Health,
                    Attack = pokemon.Attack,
                    Defense = pokemon.Defense,
                    SpecialAttack = pokemon.SpecialAttack,
                    SpecialDefense = pokemon.SpecialDefense,
                    Speed = pokemon.Speed,
                    PokemonId = pokemonTeamEV.PokemonId,
                };

                this.ModelState.AddModelError("EVTotal", "Total EVs max at 510.");
                return this.View(model);
            }

            this._dataService.UpdatePokemonTeamEV(pokemonTeamEV);

            return this.RedirectToAction("PokemonTeams", "User");
        }

        [HttpGet]
        [Route("update_iv/{ivId:int}")]
        public IActionResult EditIV(int ivId)
        {
            PokemonTeam pokemonTeam = this._dataService.GetPokemonTeamFromPokemonIV(ivId);
            if(pokemonTeam.UserId != this._dataService.GetUserWithUsername(User.Identity.Name).Id)
            {
                return this.RedirectToAction("PokemonTeams", "User");
            }
            else
            {
                PokemonTeamIV pokemonIV = this._dataService.GetPokemonTeamIV(ivId);
                PokemonTeamIVViewModel model = new PokemonTeamIVViewModel(){
                    Id = pokemonIV.Id,
                    Health = pokemonIV.Health,
                    Attack = pokemonIV.Attack,
                    Defense = pokemonIV.Defense,
                    SpecialAttack = pokemonIV.SpecialAttack,
                    SpecialDefense = pokemonIV.SpecialDefense,
                    Speed = pokemonIV.Speed,
                    PokemonId = ivId,
                };

                return this.View(model);
            }
        }

        [HttpPost]
        [Route("update_iv/{ivId:int}")]
        public IActionResult EditIV(PokemonTeamIVViewModel pokemonTeamIV)
        {
            if (!this.ModelState.IsValid)
            {
                PokemonTeamEV pokemon = this._dataService.GetPokemonTeamEV(pokemonTeamIV.PokemonId);
                PokemonTeamIVViewModel model = new PokemonTeamIVViewModel(){
                    Id = pokemon.Id,
                    Health = pokemon.Health,
                    Attack = pokemon.Attack,
                    Defense = pokemon.Defense,
                    SpecialAttack = pokemon.SpecialAttack,
                    SpecialDefense = pokemon.SpecialDefense,
                    Speed = pokemon.Speed,
                    PokemonId = pokemonTeamIV.PokemonId,
                };
            
                return this.View(model);
            }

            this._dataService.UpdatePokemonTeamIV(pokemonTeamIV);

            return this.RedirectToAction("PokemonTeams", "User");
        }

        [HttpGet]
        [Route("delete_team_pokemon/{pokemonId:int}")]
        public IActionResult DeletePokemon(int pokemonId)
        {
            PokemonTeamDetail pokemonTeamDetail = this._dataService.GetPokemonTeamDetail(pokemonId);
            PokemonTeam pokemonTeam = this._dataService.GetPokemonTeamFromPokemon(pokemonTeamDetail.Id);
            if(pokemonTeam.UserId != this._dataService.GetUserWithUsername(User.Identity.Name).Id)
            {
                return this.RedirectToAction("PokemonTeams", "User");
            }
            else
            {
                PokemonTeamDetailViewModel model = new PokemonTeamDetailViewModel(){
                    Id = pokemonTeamDetail.Id,
                    Pokemon = pokemonTeamDetail.Pokemon,
                    AppConfig = this._appConfig,
                };

                return this.View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_team_pokemon/{pokemonId:int}")]
        public IActionResult DeletePokemon(PokemonTeamDetail pokemonTeamDetail)
        {
            this._dataService.DeletePokemonTeamDetail(pokemonTeamDetail.Id);

            return this.RedirectToAction("PokemonTeams", "User");
        }
    }
}