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
        private static List<PokemonTeam> _pokemonTeams;

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
        [Route("create_pokemon/{pokemonTeamId:int}")]
        public IActionResult CreatePokemon(int pokemonTeamId)
        {
            this.UpdatePokemonTeamList();
            if(_pokemonTeams.Count() < pokemonTeamId || _pokemonTeams[pokemonTeamId - 1].SixthPokemonId != null)
            {
                return this.RedirectToAction("PokemonTeams", "User");
            }
            else
            {
                PokemonTeam pokemonTeam = _pokemonTeams[pokemonTeamId - 1];
                List<Pokemon> pokemonList = this.FillPokemonList(pokemonTeam);
                CreateTeamPokemonViewModel model = new CreateTeamPokemonViewModel(){
                    AllPokemon = pokemonList,
                    AllAbilities = this._dataService.GetAbilities(),
                };

                return this.View(model);
            }
        }

        [HttpPost]
        [Route("create_pokemon/{pokemonTeamId:int}")]
        public IActionResult CreatePokemon(CreateTeamPokemonViewModel pokemonTeamDetail, int pokemonTeamId)
        {
            if (!this.ModelState.IsValid)
            {
                CreateTeamPokemonViewModel model = new CreateTeamPokemonViewModel(){
                    AllPokemon = this.FillPokemonList(_pokemonTeams[pokemonTeamId - 1]),
                    AllAbilities = this._dataService.GetAbilities(),
                };

                return this.View(model);
            }

            PokemonTeam pokemonTeam = _pokemonTeams[pokemonTeamId - 1];

            if(pokemonTeamDetail.PokemonId == "678" && pokemonTeamDetail.Gender == "Female")
            {
                pokemonTeamDetail.PokemonId = "678-1";
            }

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
        [Route("update_pokemon/{pokemonTeamId:int}/{pokemonTeamDetailId:int}")]
        public IActionResult EditPokemon(int pokemonTeamId, int pokemonTeamDetailId)
        {
            this.UpdatePokemonTeamList();
            if(_pokemonTeams.Count() < pokemonTeamId)
            {
                return this.RedirectToAction("PokemonTeams", "User");
            }
            else
            {
                PokemonTeam pokemonTeam = _pokemonTeams[pokemonTeamId - 1];
                PokemonTeamDetail pokemonTeamDetail = this._dataService.GetPokemonTeamDetail(pokemonTeam.GrabPokemonTeamDetailIds()[pokemonTeamDetailId - 1]);
                if(pokemonTeamDetail.PokemonId == "678-1")
                {
                    pokemonTeamDetail.PokemonId = "678";
                }
                
                List<Pokemon> pokemonList = this.FillPokemonList(pokemonTeam);
                UpdateTeamPokemonViewModel model = new UpdateTeamPokemonViewModel(){
                    PokemonTeamDetail = pokemonTeamDetail,
                    AllPokemon = pokemonList,
                    AllAbilities = this._dataService.GetAbilities(),
                };

                return this.View(model);
            }
        }

        [HttpPost]
        [Route("update_pokemon/{pokemonTeamId:int}/{pokemonTeamDetailId:int}")]
        public IActionResult EditPokemon(PokemonTeamDetail pokemonTeamDetail)
        {
            PokemonTeam pokemonTeam = this._dataService.GetPokemonTeamFromPokemon(pokemonTeamDetail.Id);
            if (!this.ModelState.IsValid)
            {
                List<Pokemon> pokemonList = this.FillPokemonList(pokemonTeam);

                UpdateTeamPokemonViewModel model = new UpdateTeamPokemonViewModel(){
                    PokemonTeamDetail = pokemonTeamDetail,
                    AllPokemon = pokemonList,
                    AllAbilities = this._dataService.GetAbilities(),
                };

                return this.View(model);
            }

            if(pokemonTeamDetail.PokemonId == "678" && pokemonTeamDetail.Gender == "Female")
            {
                pokemonTeamDetail.PokemonId = "678-1";
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
        [Route("delete_team/{pokemonTeamId:int}")]
        public IActionResult DeleteTeam(int pokemonTeamId)
        {
            this.UpdatePokemonTeamList();
            if(_pokemonTeams.Count() < pokemonTeamId)
            {
                return this.RedirectToAction("PokemonTeams", "User");
            }
            else
            {
                PokemonTeam pokemonTeam = _pokemonTeams[pokemonTeamId - 1];
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
        [Route("delete_team/{pokemonTeamId:int}")]
        public IActionResult DeleteTeam(PokemonTeam pokemonTeam)
        {
            this._dataService.DeletePokemonTeam(pokemonTeam.Id);

            return this.RedirectToAction("PokemonTeams", "User");
        }

        [HttpGet]
        [Route("update_ev/{pokemonTeamId:int}/{pokemonTeamDetailId:int}")]
        public IActionResult EditEV(int pokemonTeamId, int pokemonTeamDetailId)
        {
            this.UpdatePokemonTeamList();
            if(_pokemonTeams.Count() < pokemonTeamId)
            {
                return this.RedirectToAction("PokemonTeams", "User");
            }
            else
            {
                PokemonTeamEV pokemonEV = this._dataService.GetPokemonTeamEV((int)_pokemonTeams[pokemonTeamId - 1].GrabPokemonTeamDetails[pokemonTeamDetailId - 1].PokemonTeamEVId);
                PokemonTeamEVViewModel model = new PokemonTeamEVViewModel(){
                    Id = pokemonEV.Id,
                    Health = pokemonEV.Health,
                    Attack = pokemonEV.Attack,
                    Defense = pokemonEV.Defense,
                    SpecialAttack = pokemonEV.SpecialAttack,
                    SpecialDefense = pokemonEV.SpecialDefense,
                    Speed = pokemonEV.Speed,
                    PokemonId = pokemonEV.Id,
                };

                return this.View(model);
            }
        }

        [HttpPost]
        [Route("update_ev/{pokemonTeamId:int}/{pokemonTeamDetailId:int}")]
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
        [Route("update_iv/{pokemonTeamId:int}/{pokemonTeamDetailId:int}")]
        public IActionResult EditIV(int pokemonTeamId, int pokemonTeamDetailId)
        {
            this.UpdatePokemonTeamList();
            if(_pokemonTeams.Count() < pokemonTeamId)
            {
                return this.RedirectToAction("PokemonTeams", "User");
            }
            else
            {
                PokemonTeamIV pokemonIV = this._dataService.GetPokemonTeamIV((int)_pokemonTeams[pokemonTeamId - 1].GrabPokemonTeamDetails[pokemonTeamDetailId - 1].PokemonTeamIVId);
                PokemonTeamIVViewModel model = new PokemonTeamIVViewModel(){
                    Id = pokemonIV.Id,
                    Health = pokemonIV.Health,
                    Attack = pokemonIV.Attack,
                    Defense = pokemonIV.Defense,
                    SpecialAttack = pokemonIV.SpecialAttack,
                    SpecialDefense = pokemonIV.SpecialDefense,
                    Speed = pokemonIV.Speed,
                    PokemonId = pokemonIV.Id,
                };

                return this.View(model);
            }
        }

        [HttpPost]
        [Route("update_iv/{pokemonTeamId:int}/{pokemonTeamDetailId:int}")]
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
        [Route("delete_team_pokemon/{pokemonTeamId:int}/{pokemonTeamDetailId:int}")]
        public IActionResult DeletePokemon(int pokemonTeamId, int pokemonTeamDetailId)
        {
            this.UpdatePokemonTeamList();
            if(_pokemonTeams.Count() < pokemonTeamId)
            {
                return this.RedirectToAction("PokemonTeams", "User");
            }
            else
            {
                PokemonTeam pokemonTeam = _pokemonTeams[pokemonTeamId - 1];
                PokemonTeamDetail pokemonTeamDetail = this._dataService.GetPokemonTeamDetail(pokemonTeam.GrabPokemonTeamDetailIds()[pokemonTeamDetailId - 1]);
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
        [Route("delete_team_pokemon/{pokemonTeamId:int}/{pokemonTeamDetailId:int}")]
        public IActionResult DeletePokemon(PokemonTeamDetail pokemonTeamDetail)
        {
            this._dataService.DeletePokemonTeamDetail(pokemonTeamDetail.Id);

            return this.RedirectToAction("PokemonTeams", "User");
        }

        private void UpdatePokemonTeamList()
        {
            _pokemonTeams = this._dataService.GetAllPokemonTeams(User.Identity.Name);
        }

        private List<Pokemon> FillPokemonList(PokemonTeam pokemonTeam)
        {
            List<Pokemon> pokemonList = this._dataService.GetAllPokemon().Where(x => x.Id != "800-3" && x.Id != "718-2" && x.Id != "678-1").ToList();
            if(pokemonTeam.Generation != null)
            {
                pokemonList = pokemonList.Where(x => x.Generation.ReleaseDate <= pokemonTeam.Generation.ReleaseDate).ToList();
            }

            foreach(var p in pokemonList.Where(x => x.Name.Contains('_')))
            {
                p.Name = p.Name.Replace('_', ' ');
            }
            
            foreach(var p in pokemonList.Where(x => x.Id.Contains('-')).ToList())
            {
                p.Name += " (" + this._dataService.GetFormByAltFormId(p.Id).Name + ")";
            }

            pokemonList = pokemonList.OrderBy(x => x.Name).ToList();

            return pokemonList;
        }
    }
}