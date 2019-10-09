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
            else if(string.IsNullOrEmpty(pokemonTeamDetail.Gender))
            {
                pokemonTeamDetail.Gender = pokemonTeamDetail.Genders[rnd.Next(pokemonTeamDetail.Genders.Count)];
            }

            int pokemonTeamDetailId = this._dataService.AddPokemonTeamDetail(pokemonTeamDetail);

            pokemonTeam.InsertPokemon(this._dataService.GetPokemonTeamDetail(pokemonTeamDetailId));

            this._dataService.UpdatePokemonTeam(pokemonTeam);

            return this.RedirectToAction("PokemonTeams", "User");
        }

        [HttpGet]
        [Route("delete_team/{teamId:int}")]
        public IActionResult DeleteTeam(int teamId)
        {
            PokemonTeam pokemonTeam = this._dataService.GetPokemonTeam(teamId);

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete_team/{teamId:int}")]
        public IActionResult DeleteTeam(PokemonTeam pokemonTeam)
        {
            this._dataService.DeletePokemonTeam(pokemonTeam.Id);

            return this.RedirectToAction("PokemonTeams", "User");
        }

        [HttpGet]
        [Route("add_ev/{pokemonId:int}")]
        public IActionResult AddEV(int pokemonId)
        {
            PokemonTeamEVViewModel model = new PokemonTeamEVViewModel(){
                PokemonId = pokemonId,
            };
            
            return this.View(model);
        }

        [HttpPost]
        [Route("add_ev/{pokemonId:int}")]
        public IActionResult AddEV(PokemonTeamEVViewModel pokemonTeamEV)
        {
            if (!this.ModelState.IsValid)
            {
                PokemonTeamEVViewModel model = new PokemonTeamEVViewModel(){
                    PokemonId = pokemonTeamEV.PokemonId,
                };
            
                return this.View(model);
            }
            else if (pokemonTeamEV.Health > 252)
            {
                PokemonTeamEVViewModel model = new PokemonTeamEVViewModel(){
                    PokemonId = pokemonTeamEV.PokemonId,
                };

                this.ModelState.AddModelError("EVTotal", "Health must be 252 or less.");
                return this.View(model);
            }
            else if (pokemonTeamEV.Attack > 252)
            {
                PokemonTeamEVViewModel model = new PokemonTeamEVViewModel(){
                    PokemonId = pokemonTeamEV.PokemonId,
                };

                this.ModelState.AddModelError("EVTotal", "Attack must be 252 or less.");
                return this.View(model);
            }
            else if (pokemonTeamEV.Defense > 252)
            {
                PokemonTeamEVViewModel model = new PokemonTeamEVViewModel(){
                    PokemonId = pokemonTeamEV.PokemonId,
                };

                this.ModelState.AddModelError("EVTotal", "Defense must be 252 or less.");
                return this.View(model);
            }
            else if (pokemonTeamEV.SpecialAttack > 252)
            {
                PokemonTeamEVViewModel model = new PokemonTeamEVViewModel(){
                    PokemonId = pokemonTeamEV.PokemonId,
                };

                this.ModelState.AddModelError("EVTotal", "Special Attack must be 252 or less.");
                return this.View(model);
            }
            else if (pokemonTeamEV.SpecialDefense > 252)
            {
                PokemonTeamEVViewModel model = new PokemonTeamEVViewModel(){
                    PokemonId = pokemonTeamEV.PokemonId,
                };

                this.ModelState.AddModelError("EVTotal", "Special Defense must be 252 or less.");
                return this.View(model);
            }
            else if (pokemonTeamEV.Speed > 252)
            {
                PokemonTeamEVViewModel model = new PokemonTeamEVViewModel(){
                    PokemonId = pokemonTeamEV.PokemonId,
                };

                this.ModelState.AddModelError("EVTotal", "Speed must be 252 or less.");
                return this.View(model);
            }
            else if (pokemonTeamEV.EVTotal == 0)
            {
                PokemonTeamEVViewModel model = new PokemonTeamEVViewModel(){
                    PokemonId = pokemonTeamEV.PokemonId,
                };

                this.ModelState.AddModelError("EVTotal", "You need to add some EVs to continue.");
                return this.View(model);
            }
            else if (pokemonTeamEV.EVTotal > 510)
            {
                PokemonTeamEVViewModel model = new PokemonTeamEVViewModel(){
                    PokemonId = pokemonTeamEV.PokemonId,
                };

                this.ModelState.AddModelError("EVTotal", "Total EVs max at 510.");
                return this.View(model);
            }

            int pokemonTeamEVId = this._dataService.AddPokemonTeamEV(pokemonTeamEV);

            PokemonTeamDetail pokemon = this._dataService.GetPokemonTeamDetail(pokemonTeamEV.PokemonId);
            pokemon.PokemonTeamEVId = pokemonTeamEVId;
            this._dataService.UpdatePokemonTeamDetail(pokemon);

            return this.RedirectToAction("PokemonTeams", "User");
        }

        [HttpGet]
        [Route("update_ev/{pokemonId:int}")]
        public IActionResult EditEV(int pokemonId)
        {
            PokemonTeamEVViewModel model = new PokemonTeamEVViewModel(){
                PokemonId = pokemonId,
            };
            
            return this.View(model);
        }

        [HttpPost]
        [Route("update_ev/{pokemonId:int}")]
        public IActionResult EditEV(PokemonTeamEVViewModel pokemonTeamEV)
        {
            if (!this.ModelState.IsValid)
            {
                PokemonTeamEVViewModel model = new PokemonTeamEVViewModel(){
                    PokemonId = pokemonTeamEV.PokemonId,
                };
            
                return this.View(model);
            }
            else if (pokemonTeamEV.Health > 252)
            {
                PokemonTeamEVViewModel model = new PokemonTeamEVViewModel(){
                    PokemonId = pokemonTeamEV.PokemonId,
                };

                this.ModelState.AddModelError("EVTotal", "Health must be 252 or less.");
                return this.View(model);
            }
            else if (pokemonTeamEV.Attack > 252)
            {
                PokemonTeamEVViewModel model = new PokemonTeamEVViewModel(){
                    PokemonId = pokemonTeamEV.PokemonId,
                };

                this.ModelState.AddModelError("EVTotal", "Attack must be 252 or less.");
                return this.View(model);
            }
            else if (pokemonTeamEV.Defense > 252)
            {
                PokemonTeamEVViewModel model = new PokemonTeamEVViewModel(){
                    PokemonId = pokemonTeamEV.PokemonId,
                };

                this.ModelState.AddModelError("EVTotal", "Defense must be 252 or less.");
                return this.View(model);
            }
            else if (pokemonTeamEV.SpecialAttack > 252)
            {
                PokemonTeamEVViewModel model = new PokemonTeamEVViewModel(){
                    PokemonId = pokemonTeamEV.PokemonId,
                };

                this.ModelState.AddModelError("EVTotal", "Special Attack must be 252 or less.");
                return this.View(model);
            }
            else if (pokemonTeamEV.SpecialDefense > 252)
            {
                PokemonTeamEVViewModel model = new PokemonTeamEVViewModel(){
                    PokemonId = pokemonTeamEV.PokemonId,
                };

                this.ModelState.AddModelError("EVTotal", "Special Defense must be 252 or less.");
                return this.View(model);
            }
            else if (pokemonTeamEV.Speed > 252)
            {
                PokemonTeamEVViewModel model = new PokemonTeamEVViewModel(){
                    PokemonId = pokemonTeamEV.PokemonId,
                };

                this.ModelState.AddModelError("EVTotal", "Speed must be 252 or less.");
                return this.View(model);
            }
            else if (pokemonTeamEV.EVTotal == 0)
            {
                PokemonTeamEVViewModel model = new PokemonTeamEVViewModel(){
                    PokemonId = pokemonTeamEV.PokemonId,
                };

                this.ModelState.AddModelError("EVTotal", "You need to add some EVs to continue.");
                return this.View(model);
            }
            else if (pokemonTeamEV.EVTotal > 510)
            {
                PokemonTeamEVViewModel model = new PokemonTeamEVViewModel(){
                    PokemonId = pokemonTeamEV.PokemonId,
                };

                this.ModelState.AddModelError("EVTotal", "Total EVs max at 510.");
                return this.View(model);
            }

            this._dataService.UpdatePokemonTeamEV(pokemonTeamEV);

            return this.RedirectToAction("PokemonTeams", "User");
        }
    }
}