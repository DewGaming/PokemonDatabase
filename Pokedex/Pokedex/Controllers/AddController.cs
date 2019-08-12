using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Pokedex.DataAccess.Models;
using Pokedex.Models;

namespace Pokedex.Controllers
{
    [Authorize(Roles = "Owner")]
    [Route("admin")]
    public class AddController : Controller
    {
        private readonly DataService _dataService;

        private readonly AppConfig _appConfig;

        private static bool _cameFromAdminPokemon = false;

        private static bool _cameFromAdminAltForms = false;

        public AddController(IOptions<AppConfig> appConfig, DataContext dataContext)
        {
            // Instantiate an instance of the data service.
            this._appConfig = appConfig.Value;
            this._dataService = new DataService(dataContext);
        }

        [HttpGet]
        [Route("add_evolution/{pokemonId}")]
        public IActionResult Evolution(string pokemonId)
        {
            Pokemon evolutionPokemon = this._dataService.GetPokemonById(pokemonId);
            EvolutionViewModel model = new EvolutionViewModel()
            {
                AllEvolutionMethods = this._dataService.GetEvolutionMethods(),
                EvolutionPokemon = evolutionPokemon,
                EvolutionPokemonId = evolutionPokemon.Id,
            };

            List<Pokemon> pokemonList = this._dataService.GetAllPokemon().Where(x => x.Id != pokemonId).ToList();
            foreach (var pokemon in pokemonList.Where(p => p.Id.Contains('-')))
            {
                pokemon.Name += " (" + this._dataService.GetPokemonFormName(pokemon.Id) + ")";
            }

            model.AllPokemon = pokemonList;
            
            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_evolution/{pokemonId}")]
        public IActionResult Evolution(Evolution evolution)
        {
            if (!this.ModelState.IsValid)
            {
                EvolutionViewModel model = new EvolutionViewModel()
                {
                    AllEvolutionMethods = this._dataService.GetEvolutionMethods(),
                    EvolutionPokemon = evolution.EvolutionPokemon,
                    EvolutionPokemonId = evolution.EvolutionPokemon.Id,
                };
                List<Pokemon> pokemonList = this._dataService.GetAllPokemon().Where(x => x.Id != evolution.EvolutionPokemonId).ToList();
                foreach (var pokemon in pokemonList.Where(p => p.Id.Contains('-')))
                {
                    pokemon.Name += " (" + this._dataService.GetPokemonFormName(pokemon.Id) + ")";
                }

                model.AllPokemon = pokemonList;

                return this.View(model);
            }

            this._dataService.AddEvolution(evolution);

            return this.RedirectToAction("Pokemon", "Admin");
        }

        [HttpGet]
        [Route("add_shiny_hunting_technique")]
        public IActionResult ShinyHuntingTechnique()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_shiny_hunting_technique")]
        public IActionResult ShinyHuntingTechnique(ShinyHuntingTechnique shinyHuntingTechnique)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this._dataService.AddShinyHuntingTechnique(shinyHuntingTechnique);

            return this.RedirectToAction("ShinyHuntingTechniques", "Admin");
        }

        [HttpGet]
        [Route("add_generation")]
        public IActionResult Generation()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_generation")]
        public IActionResult Generation(Generation generation)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this._dataService.AddGeneration(generation);

            return this.RedirectToAction("Generations", "Admin");
        }

        [HttpGet]
        [Route("add_type")]
        public IActionResult Type()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_type")]
        public IActionResult Type(DataAccess.Models.Type type)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this._dataService.AddType(type);

            return this.RedirectToAction("Types", "Admin");
        }

        [HttpGet]
        [Route("add_legendary_type")]
        public IActionResult LegendaryType()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_legendary_type")]
        public IActionResult LegendaryType(LegendaryType legendaryType)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this._dataService.AddLegendaryType(legendaryType);

            return this.RedirectToAction("LegendaryTypes", "Admin");
        }

        [HttpGet]
        [Route("add_egg_group")]
        public IActionResult EggGroup()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_egg_group")]
        public IActionResult EggGroup(EggGroup eggGroup)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this._dataService.AddEggGroup(eggGroup);

            return this.RedirectToAction("EggGroups", "Admin");
        }

        [HttpGet]
        [Route("add_classification")]
        public IActionResult Classification()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_classification")]
        public IActionResult Classification(Classification classification)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this._dataService.AddClassification(classification);

            return this.RedirectToAction("Classifications", "Admin");
        }

        [HttpGet]
        [Route("add_form")]
        public IActionResult Form()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_form")]
        public IActionResult Form(Form form)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this._dataService.AddForm(form);

            return this.RedirectToAction("Forms", "Admin");
        }

        [HttpGet]
        [Route("add_ability")]
        public IActionResult Ability()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_ability")]
        public IActionResult Ability(Ability ability)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this._dataService.AddAbility(ability);

            return this.RedirectToAction("Abilities", "Admin");
        }

        [HttpGet]
        [Route("add_pokemon")]
        public IActionResult Pokemon()
        {
            BasePokemonViewModel model = new BasePokemonViewModel()
            {
                AllBaseHappinesses = this._dataService.GetBaseHappinesses(),
                AllClassifications = this._dataService.GetClassifications(),
                AllCaptureRates = this._dataService.GetCaptureRates(),
                AllEggCycles = this._dataService.GetEggCycles(),
                AllExperienceGrowths = this._dataService.GetExperienceGrowths(),
                AllGenderRatios = new List<GenderRatioViewModel>(),
                AllGenerations = this._dataService.GetGenerations(),
            };

            foreach(GenderRatio genderRatio in this._dataService.GetGenderRatios())
            {
                GenderRatioViewModel viewModel = new GenderRatioViewModel() {
                    Id = genderRatio.Id
                };

                if (genderRatio.MaleRatio == genderRatio.FemaleRatio && genderRatio.MaleRatio == 0)
                {
                    viewModel.GenderRatioString = "Genderless";
                }
                else if (genderRatio.FemaleRatio == 0)
                {
                    viewModel.GenderRatioString = genderRatio.MaleRatio + "% Male";
                }
                else if (genderRatio.MaleRatio == 0)
                {
                    viewModel.GenderRatioString = genderRatio.FemaleRatio + "% Female";
                }
                else
                {
                    viewModel.GenderRatioString = genderRatio.MaleRatio + "% Male / " + genderRatio.FemaleRatio + "% Female";
                }

                model.AllGenderRatios.Add(viewModel);
            }

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_pokemon")]
        public async Task<IActionResult> Pokemon(BasePokemonViewModel pokemon, IFormFile upload)
        {
            int pokedexNumber;
            if (!this.ModelState.IsValid)
            {
                BasePokemonViewModel model = new BasePokemonViewModel()
                {
                    AllBaseHappinesses = this._dataService.GetBaseHappinesses(),
                    AllClassifications = this._dataService.GetClassifications(),
                    AllCaptureRates = this._dataService.GetCaptureRates(),
                    AllEggCycles = this._dataService.GetEggCycles(),
                    AllExperienceGrowths = this._dataService.GetExperienceGrowths(),
                    AllGenderRatios = new List<GenderRatioViewModel>(),
                    AllGenerations = this._dataService.GetGenerations(),
                };

                foreach(GenderRatio genderRatio in this._dataService.GetGenderRatios())
                {
                    GenderRatioViewModel viewModel = new GenderRatioViewModel() {
                        Id = genderRatio.Id
                    };

                    if (genderRatio.MaleRatio == genderRatio.FemaleRatio && genderRatio.MaleRatio == 0)
                    {
                        viewModel.GenderRatioString = "Genderless";
                    }
                    else if (genderRatio.FemaleRatio == 0)
                    {
                        viewModel.GenderRatioString = genderRatio.MaleRatio + "% Male";
                    }
                    else if (genderRatio.MaleRatio == 0)
                    {
                        viewModel.GenderRatioString = genderRatio.FemaleRatio + "% Female";
                    }
                    else
                    {
                        viewModel.GenderRatioString = genderRatio.MaleRatio + "% Male / " + genderRatio.FemaleRatio + "% Female";
                    }

                    model.AllGenderRatios.Add(viewModel);
                }

                return this.View(model);
            }
            else if (!int.TryParse(pokemon.Id, out pokedexNumber))
            {
                BasePokemonViewModel model = new BasePokemonViewModel()
                {
                    AllBaseHappinesses = this._dataService.GetBaseHappinesses(),
                    AllClassifications = this._dataService.GetClassifications(),
                    AllCaptureRates = this._dataService.GetCaptureRates(),
                    AllEggCycles = this._dataService.GetEggCycles(),
                    AllExperienceGrowths = this._dataService.GetExperienceGrowths(),
                    AllGenderRatios = new List<GenderRatioViewModel>(),
                    AllGenerations = this._dataService.GetGenerations(),
                };
    
                foreach(GenderRatio genderRatio in this._dataService.GetGenderRatios())
                {
                    GenderRatioViewModel viewModel = new GenderRatioViewModel() {
                        Id = genderRatio.Id
                    };
    
                    if (genderRatio.MaleRatio == genderRatio.FemaleRatio && genderRatio.MaleRatio == 0)
                    {
                        viewModel.GenderRatioString = "Genderless";
                    }
                    else if (genderRatio.FemaleRatio == 0)
                    {
                        viewModel.GenderRatioString = genderRatio.MaleRatio + "% Male";
                    }
                    else if (genderRatio.MaleRatio == 0)
                    {
                        viewModel.GenderRatioString = genderRatio.FemaleRatio + "% Female";
                    }
                    else
                    {
                        viewModel.GenderRatioString = genderRatio.MaleRatio + "% Male / " + genderRatio.FemaleRatio + "% Female";
                    }
    
                    model.AllGenderRatios.Add(viewModel);
                }

                this.ModelState.AddModelError("Pokedex Number", "Pokedex Number must be a number.");
                return this.View(model);
            }
            else if (pokemon.Id.Contains('-'))
            {
                BasePokemonViewModel model = new BasePokemonViewModel()
                {
                    AllBaseHappinesses = this._dataService.GetBaseHappinesses(),
                    AllClassifications = this._dataService.GetClassifications(),
                    AllCaptureRates = this._dataService.GetCaptureRates(),
                    AllEggCycles = this._dataService.GetEggCycles(),
                    AllExperienceGrowths = this._dataService.GetExperienceGrowths(),
                    AllGenderRatios = new List<GenderRatioViewModel>(),
                    AllGenerations = this._dataService.GetGenerations(),
                };
    
                foreach(GenderRatio genderRatio in this._dataService.GetGenderRatios())
                {
                    GenderRatioViewModel viewModel = new GenderRatioViewModel() {
                        Id = genderRatio.Id
                    };
    
                    if (genderRatio.MaleRatio == genderRatio.FemaleRatio && genderRatio.MaleRatio == 0)
                    {
                        viewModel.GenderRatioString = "Genderless";
                    }
                    else if (genderRatio.FemaleRatio == 0)
                    {
                        viewModel.GenderRatioString = genderRatio.MaleRatio + "% Male";
                    }
                    else if (genderRatio.MaleRatio == 0)
                    {
                        viewModel.GenderRatioString = genderRatio.FemaleRatio + "% Female";
                    }
                    else
                    {
                        viewModel.GenderRatioString = genderRatio.MaleRatio + "% Male / " + genderRatio.FemaleRatio + "% Female";
                    }
    
                    model.AllGenderRatios.Add(viewModel);
                }

                this.ModelState.AddModelError("Pokedex Number", "Pokedex Number cannot contain a \"-\" in it.");
                return this.View(model);
            }
            else if (this._dataService.GetAllPokemon().Exists(x => x.Id == pokemon.Id))
            {
                BasePokemonViewModel model = new BasePokemonViewModel()
                {
                    AllBaseHappinesses = this._dataService.GetBaseHappinesses(),
                    AllClassifications = this._dataService.GetClassifications(),
                    AllCaptureRates = this._dataService.GetCaptureRates(),
                    AllEggCycles = this._dataService.GetEggCycles(),
                    AllExperienceGrowths = this._dataService.GetExperienceGrowths(),
                    AllGenderRatios = new List<GenderRatioViewModel>(),
                    AllGenerations = this._dataService.GetGenerations(),
                };
    
                foreach(GenderRatio genderRatio in this._dataService.GetGenderRatios())
                {
                    GenderRatioViewModel viewModel = new GenderRatioViewModel() {
                        Id = genderRatio.Id
                    };
    
                    if (genderRatio.MaleRatio == genderRatio.FemaleRatio && genderRatio.MaleRatio == 0)
                    {
                        viewModel.GenderRatioString = "Genderless";
                    }
                    else if (genderRatio.FemaleRatio == 0)
                    {
                        viewModel.GenderRatioString = genderRatio.MaleRatio + "% Male";
                    }
                    else if (genderRatio.MaleRatio == 0)
                    {
                        viewModel.GenderRatioString = genderRatio.FemaleRatio + "% Female";
                    }
                    else
                    {
                        viewModel.GenderRatioString = genderRatio.MaleRatio + "% Male / " + genderRatio.FemaleRatio + "% Female";
                    }
    
                    model.AllGenderRatios.Add(viewModel);
                }

                this.ModelState.AddModelError("Pokedex Number", "Pokedex Number already exists.");
                return this.View(model);
            }

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(_appConfig.FTPUrl + pokemon.Id.ToString() + upload.FileName.Substring(upload.FileName.LastIndexOf('.')));
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(_appConfig.FTPUsername, _appConfig.FTPPassword);

            using (var requestStream = request.GetRequestStream())  
            {  
                await upload.CopyToAsync(requestStream);  
            }

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
            }

            this._dataService.AddPokemon(pokemon);

            return this.RedirectToAction("Typing", "Add", new { pokemonId = pokemon.Id });
        }

        [HttpGet]
        [Route("add_alternate_form/{pokemonId}")]
        public IActionResult AltForm(string pokemonId)
        {
            Pokemon originalPokemon = this._dataService.GetPokemonById(pokemonId);
            List<Generation> generations = this._dataService.GetGenerations().Where(x => x.ReleaseDate >= originalPokemon.Generation.ReleaseDate).ToList();

            AlternateFormViewModel model = new AlternateFormViewModel()
            {
                AllForms = this._dataService.GetForms(),
                AllGenerations = generations,
                OriginalPokemon = originalPokemon,
                OriginalPokemonId = originalPokemon.Id,
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_alternate_form/{pokemonId}")]
        public async Task<IActionResult> AltForm(AlternateFormViewModel pokemon, IFormFile upload)
        {
            List<PokemonFormDetail> originalPokemonForms = this._dataService.GetPokemonForms(pokemon.OriginalPokemonId);
            if (!this.ModelState.IsValid)
            {
                Pokemon originalPokemon = this._dataService.GetPokemonById(pokemon.OriginalPokemonId);
                List<Generation> generations = this._dataService.GetGenerations().Where(x => x.ReleaseDate >= originalPokemon.Generation.ReleaseDate).ToList();

                AlternateFormViewModel model = new AlternateFormViewModel()
                {
                    AllForms = this._dataService.GetForms(),
                    AllGenerations = generations,
                    OriginalPokemon = originalPokemon,
                    OriginalPokemonId = originalPokemon.Id,
                };

                return this.View(model);
            }

            foreach (var p in originalPokemonForms)
            {
                if (p.FormId == pokemon.FormId)
                {
                    Pokemon originalPokemon = this._dataService.GetPokemonById(pokemon.OriginalPokemonId);
                    List<Generation> generations = this._dataService.GetGenerations().Where(x => x.ReleaseDate >= originalPokemon.Generation.ReleaseDate).ToList();

                    AlternateFormViewModel model = new AlternateFormViewModel()
                    {
                        AllForms = this._dataService.GetForms(),
                        AllGenerations = generations,
                        OriginalPokemon = originalPokemon,
                        OriginalPokemonId = originalPokemon.Id,
                    };
    
                    this.ModelState.AddModelError("Alternate Form Name", "Original Pokemon already has an alternate form of this type.");
                    return this.View(model);
                }
            }

            Pokemon alternatePokemon = this._dataService.GetPokemonNoIncludesById(pokemon.OriginalPokemonId);

            alternatePokemon.Id = pokemon.OriginalPokemonId + '-' + (originalPokemonForms.Count() + 1);
            alternatePokemon.Height = pokemon.Height;
            alternatePokemon.Weight = pokemon.Weight;
            alternatePokemon.ExpYield = pokemon.ExpYield;
            alternatePokemon.GenerationId = pokemon.GenerationId;
            alternatePokemon.IsComplete = false;

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(_appConfig.FTPUrl + alternatePokemon.Id.ToString() + upload.FileName.Substring(upload.FileName.LastIndexOf('.')));
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(_appConfig.FTPUsername, _appConfig.FTPPassword);

            using (var requestStream = request.GetRequestStream())  
            {  
                await upload.CopyToAsync(requestStream);  
            }

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
            }

            this._dataService.AddPokemon(alternatePokemon);

            PokemonEggGroupDetail eggGroups = this._dataService.GetPokemonEggGroups(pokemon.OriginalPokemonId);
            PokemonEggGroupDetail alternatePokemonEggGroups = new PokemonEggGroupDetail(){
                PrimaryEggGroupId = eggGroups.PrimaryEggGroupId,
                SecondaryEggGroupId = eggGroups.SecondaryEggGroupId,
                PokemonId = alternatePokemon.Id,
            };
            this._dataService.AddPokemonEggGroups(alternatePokemonEggGroups);

            EVYield evYields = this._dataService.GetEVYield(pokemon.OriginalPokemonId);
            EVYield alternatePokemonEVYields = new EVYield(){
                Health = evYields.Health,
                Attack = evYields.Attack,
                Defense = evYields.Defense,
                SpecialAttack = evYields.SpecialAttack,
                SpecialDefense = evYields.SpecialDefense,
                Speed = evYields.Speed,
                PokemonId = alternatePokemon.Id,
            };
            this._dataService.AddPokemonEVYield(alternatePokemonEVYields);

            PokemonFormDetail alternateForm = new PokemonFormDetail(){
                OriginalPokemonId = pokemon.OriginalPokemonId,
                AltFormPokemonId = alternatePokemon.Id,
                FormId = pokemon.FormId,
            };
            this._dataService.AddPokemonFormDetails(alternateForm);

            return this.RedirectToAction("Typing", "Add", new { pokemonId = alternatePokemon.Id });
        }

        [HttpGet]
        [Route("add_typing/{pokemonId}")]
        public IActionResult Typing(string pokemonId)
        {
            _cameFromAdminPokemon = Request.Headers["Referer"].ToString().Contains("admin/pokemon");
            _cameFromAdminAltForms = Request.Headers["Referer"].ToString().Contains("admin/edit_alternate_forms");

            PokemonTypingViewModel model = new PokemonTypingViewModel()
            {
                AllTypes = this._dataService.GetTypes(),
                PokemonId = pokemonId,
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_typing/{pokemonId}")]
        public IActionResult Typing(PokemonTypingViewModel typing)
        {
            if (!this.ModelState.IsValid)
            {
                PokemonTypingViewModel model = new PokemonTypingViewModel()
                {
                    AllTypes = this._dataService.GetTypes(),
                    PokemonId = typing.PokemonId,
                };

                return this.View(model);
            }

            this._dataService.AddPokemonTyping(typing);

            if(_cameFromAdminPokemon || _cameFromAdminAltForms)
            {
                if (this.CheckIfComplete(typing.PokemonId))
                {
                    Pokemon pokemon = this._dataService.GetPokemonById(typing.PokemonId);
                    pokemon.IsComplete = true;
                    this._dataService.UpdatePokemon(pokemon);
                }

                if (_cameFromAdminPokemon)
                {
                    _cameFromAdminPokemon = false;
                    return this.RedirectToAction("Pokemon", "Admin");
                }
                else
                {
                    _cameFromAdminAltForms = false;
                    PokemonFormDetail pokemon = this._dataService.GetPokemonFormDetailByAltFormId(typing.PokemonId);
                    return this.RedirectToAction("AltForms", "Edit", new { pokemonId = pokemon.OriginalPokemonId });
                }
            }
            else
            {
                return this.RedirectToAction("Abilities", "Add", new { pokemonId = typing.PokemonId });
            }
        }

        [HttpGet]
        [Route("add_abilities/{pokemonId}")]
        public IActionResult Abilities(string pokemonId)
        {
            _cameFromAdminPokemon = Request.Headers["Referer"].ToString().Contains("admin/pokemon");
            _cameFromAdminAltForms = Request.Headers["Referer"].ToString().Contains("admin/edit_alternate_forms");

            PokemonAbilitiesViewModel model = new PokemonAbilitiesViewModel()
            {
                AllAbilities = this._dataService.GetAbilities(),
                PokemonId = pokemonId,
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_abilities/{pokemonId}")]
        public IActionResult Abilities(PokemonAbilitiesViewModel abilities)
        {
            if (!this.ModelState.IsValid)
            {
                PokemonAbilitiesViewModel model = new PokemonAbilitiesViewModel()
                {
                    AllAbilities = this._dataService.GetAbilities(),
                    PokemonId = abilities.PokemonId,
                };

                return this.View(model);
            }

            this._dataService.AddPokemonAbilities(abilities);

            if(_cameFromAdminPokemon || _cameFromAdminAltForms)
            {
                if (this.CheckIfComplete(abilities.PokemonId))
                {
                    Pokemon pokemon = this._dataService.GetPokemonById(abilities.PokemonId);
                    pokemon.IsComplete = true;
                    this._dataService.UpdatePokemon(pokemon);
                }

                if (_cameFromAdminPokemon)
                {
                    _cameFromAdminPokemon = false;
                    return this.RedirectToAction("Pokemon", "Admin");
                }
                else
                {
                    _cameFromAdminAltForms = false;
                    PokemonFormDetail pokemon = this._dataService.GetPokemonFormDetailByAltFormId(abilities.PokemonId);
                    return this.RedirectToAction("AltForms", "Edit", new { pokemonId = pokemon.OriginalPokemonId });
                }
            }
            else if (abilities.PokemonId.Contains('-'))
            {
                return this.RedirectToAction("BaseStats", "Add", new { pokemonId = abilities.PokemonId });
            }else
            {
                return this.RedirectToAction("EggGroups", "Add", new { pokemonId = abilities.PokemonId });
            }
        }

        [HttpGet]
        [Route("add_egg_groups/{pokemonId}")]
        public IActionResult EggGroups(string pokemonId)
        {
            _cameFromAdminPokemon = Request.Headers["Referer"].ToString().Contains("admin/pokemon");

            PokemonEggGroupsViewModel model = new PokemonEggGroupsViewModel()
            {
                AllEggGroups = this._dataService.GetEggGroups(),
                PokemonId = pokemonId,
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_egg_groups/{pokemonId}")]
        public IActionResult EggGroups(PokemonEggGroupsViewModel eggGroups)
        {
            if (!this.ModelState.IsValid)
            {
                PokemonEggGroupsViewModel model = new PokemonEggGroupsViewModel()
                {
                    AllEggGroups = this._dataService.GetEggGroups(),
                    PokemonId = eggGroups.PokemonId,
                };

                return this.View(model);
            }

            this._dataService.AddPokemonEggGroups(eggGroups);

            if(_cameFromAdminPokemon)
            {
                _cameFromAdminPokemon = false;
                if (this.CheckIfComplete(eggGroups.PokemonId))
                {
                    Pokemon pokemon = this._dataService.GetPokemonById(eggGroups.PokemonId);
                    pokemon.IsComplete = true;
                    this._dataService.UpdatePokemon(pokemon);
                }

                return this.RedirectToAction("Pokemon", "Admin");
            }
            else
            {
                return this.RedirectToAction("BaseStats", "Add", new { pokemonId = eggGroups.PokemonId });
            }
        }

        [HttpGet]
        [Route("add_base_stats/{pokemonId}")]
        public IActionResult BaseStats(string pokemonId)
        {
            _cameFromAdminPokemon = Request.Headers["Referer"].ToString().Contains("admin/pokemon");
            _cameFromAdminAltForms = Request.Headers["Referer"].ToString().Contains("admin/edit_alternate_forms");

            BaseStat model = new BaseStat()
            {
                PokemonId = pokemonId,
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_base_stats/{pokemonId}")]
        public IActionResult BaseStats(BaseStat baseStat)
        {
            if (!this.ModelState.IsValid)
            {
                BaseStat model = new BaseStat()
                {
                    PokemonId = baseStat.PokemonId,
                };

                return this.View(model);
            }

            this._dataService.AddPokemonBaseStat(baseStat);

            if (baseStat.PokemonId.Contains('-'))
            {
                Pokemon pokemon = this._dataService.GetPokemonById(baseStat.PokemonId);
                pokemon.IsComplete = true;
                this._dataService.UpdatePokemon(pokemon);
                return this.RedirectToAction("Pokemon", "Home", new { Name = pokemon.Name.Replace(' ', '_').ToLower() });
            }
            else if(_cameFromAdminPokemon || _cameFromAdminAltForms)
            {
                if (this.CheckIfComplete(baseStat.PokemonId))
                {
                    Pokemon pokemon = this._dataService.GetPokemonById(baseStat.PokemonId);
                    pokemon.IsComplete = true;
                    this._dataService.UpdatePokemon(pokemon);
                }

                if (_cameFromAdminPokemon)
                {
                    _cameFromAdminPokemon = false;
                    return this.RedirectToAction("Pokemon", "Admin");
                }
                else
                {
                    _cameFromAdminAltForms = false;
                    PokemonFormDetail pokemon = this._dataService.GetPokemonFormDetailByAltFormId(baseStat.PokemonId);
                    return this.RedirectToAction("AltForms", "Edit", new { pokemonId = pokemon.OriginalPokemonId });
                }
            }
            else
            {
                return this.RedirectToAction("EVYields", "Add", new { pokemonId = baseStat.PokemonId });
            }
        }

        [HttpGet]
        [Route("add_ev_yields/{pokemonId}")]
        public IActionResult EVYields(string pokemonId)
        {
            _cameFromAdminPokemon = Request.Headers["Referer"].ToString().Contains("admin/pokemon");
            EVYield model = new EVYield()
            {
                PokemonId = pokemonId,
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_ev_yields/{pokemonId}")]
        public IActionResult EVYields(EVYield evYield)
        {
            if (!this.ModelState.IsValid)
            {
                EVYield model = new EVYield()
                {
                    PokemonId = evYield.PokemonId,
                };

                return this.View(model);
            }
            else if (evYield.Health > 3)
            {
                EVYield model = new EVYield()
                {
                    PokemonId = evYield.PokemonId,
                };

                this.ModelState.AddModelError("EVTotal", "Health must be 3 or less.");
                return this.View(model);
            }
            else if (evYield.Attack > 3)
            {
                EVYield model = new EVYield()
                {
                    PokemonId = evYield.PokemonId,
                };

                this.ModelState.AddModelError("EVTotal", "Attack must be 3 or less.");
                return this.View(model);
            }
            else if (evYield.Defense > 3)
            {
                EVYield model = new EVYield()
                {
                    PokemonId = evYield.PokemonId,
                };

                this.ModelState.AddModelError("EVTotal", "Defense must be 3 or less.");
                return this.View(model);
            }
            else if (evYield.SpecialAttack > 3)
            {
                EVYield model = new EVYield()
                {
                    PokemonId = evYield.PokemonId,
                };

                this.ModelState.AddModelError("EVTotal", "Special Attack must be 3 or less.");
                return this.View(model);
            }
            else if (evYield.SpecialDefense > 3)
            {
                EVYield model = new EVYield()
                {
                    PokemonId = evYield.PokemonId,
                };

                this.ModelState.AddModelError("EVTotal", "Special Defense must be 3 or less.");
                return this.View(model);
            }
            else if (evYield.Speed > 3)
            {
                EVYield model = new EVYield()
                {
                    PokemonId = evYield.PokemonId,
                };

                this.ModelState.AddModelError("EVTotal", "Speed must be 3 or less.");
                return this.View(model);
            }
            else if (evYield.EVTotal > 3)
            {
                EVYield model = new EVYield()
                {
                    PokemonId = evYield.PokemonId,
                };

                this.ModelState.AddModelError("EVTotal", "EV Total must be 3 or less.");
                return this.View(model);
            }

            this._dataService.AddPokemonEVYield(evYield);

            if(_cameFromAdminPokemon)
            {
                _cameFromAdminPokemon = false;
                if (this.CheckIfComplete(evYield.PokemonId))
                {
                    Pokemon pokemon = this._dataService.GetPokemonById(evYield.PokemonId);
                    pokemon.IsComplete = true;
                    this._dataService.UpdatePokemon(pokemon);
                }

                return this.RedirectToAction("Pokemon", "Admin");
            }
            else
            {
                Pokemon pokemon = this._dataService.GetPokemonById(evYield.PokemonId);

                pokemon.IsComplete = true;
                this._dataService.UpdatePokemon(pokemon);
                
                return this.RedirectToAction("Pokemon", "Home", new { Name = pokemon.Name.Replace(' ', '_').ToLower() });
            }
        }

        public bool CheckIfComplete(string pokemonId)
        {
            return this._dataService.GetAllPokemonWithTypesAndIncomplete().Exists(x => x.PokemonId == pokemonId) &&
                   this._dataService.GetAllPokemonWithAbilitiesAndIncomplete().Exists(x => x.PokemonId == pokemonId) &&
                   this._dataService.GetAllPokemonWithEggGroupsAndIncomplete().Exists(x => x.PokemonId == pokemonId) &&
                   this._dataService.GetBaseStatsWithIncomplete().Exists(x => x.PokemonId == pokemonId) &&
                   this._dataService.GetEVYieldsWithIncomplete().Exists(x => x.PokemonId == pokemonId);
        }
    }
}