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
    [Authorize(Roles = "Admin,Owner")]
    [Route("admin")]
    public class EditController : Controller
    {
        private readonly DataService _dataService;

        private readonly AppConfig _appConfig;

        public EditController(IOptions<AppConfig> appConfig, DataContext dataContext)
        {
            this._appConfig = appConfig.Value;
            this._dataService = new DataService(dataContext);
        }

        [Authorize(Roles = "Owner")]
        [HttpGet]
        [Route("edit_user/{id:int}")]
        public new IActionResult User(int id)
        {
            User model = this._dataService.GetUserById(id);

            return this.View(model);
        }

        [Authorize(Roles = "Owner")]
        [HttpPost]
        [Route("edit_user/{id:int}")]
        public new IActionResult User(User user)
        {
            if (!this.ModelState.IsValid)
            {
                User model = this._dataService.GetUserWithUsername(user.Username);

                return this.View(model);
            }

            this._dataService.UpdateUser(user);

            return this.RedirectToAction("Users", "Owner");
        }

        [HttpGet]
        [Route("edit_generation/{id}")]
        public IActionResult Generation(string id)
        {
            Generation model = this._dataService.GetGeneration(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_generation/{id}")]
        public IActionResult Generation(Generation generation)
        {
            if (!this.ModelState.IsValid)
            {
                Generation model = this._dataService.GetGeneration(generation.Id);

                return this.View(model);
            }

            this._dataService.UpdateGeneration(generation);

            return this.RedirectToAction("Generations", "Admin");
        }

        [HttpGet]
        [Route("edit_battle_item/{id:int}")]
        public IActionResult BattleItem(int id)
        {
            List<Pokemon> pokemonList = this._dataService.GetAllPokemon();
            foreach(var p in pokemonList.Where(x => x.Id.Contains('-')))
            {
                p.Name += " (" + this._dataService.GetFormByAltFormId(p.Id).Name + ")";
            }

            BattleItem battleItem = this._dataService.GetBattleItem(id);
            BattleItemViewModel model = new BattleItemViewModel(){
                Id = battleItem.Id,
                Name = battleItem.Name,
                GenerationId = battleItem.GenerationId,
                AllGenerations = this._dataService.GetGenerations().Where(x => x.ReleaseDate >= new System.DateTime(2000, 10, 15)).ToList(),
                AllPokemon = pokemonList,
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_battle_item/{id:int}")]
        public IActionResult BattleItem(BattleItem battleItem)
        {
            if (!this.ModelState.IsValid)
            {
                List<Pokemon> pokemonList = this._dataService.GetAllPokemon();
                foreach(var p in pokemonList.Where(x => x.Id.Contains('-')))
                {
                    p.Name += " (" + this._dataService.GetFormByAltFormId(p.Id).Name + ")";
                }

                BattleItem item = this._dataService.GetBattleItem(battleItem.Id);
                BattleItemViewModel model = new BattleItemViewModel(){
                    Id = item.Id,
                    Name = item.Name,
                    GenerationId = item.GenerationId,
                    AllGenerations = this._dataService.GetGenerations().Where(x => x.ReleaseDate >= new System.DateTime(2000, 10, 15)).ToList(),
                    AllPokemon = pokemonList,
                };

                return this.View(model);
            }

            this._dataService.UpdateBattleItem(battleItem);

            return this.RedirectToAction("BattleItems", "Admin");
        }

        [HttpGet]
        [Route("edit_pokemon/{id}")]
        public IActionResult Pokemon(string id)
        {
            BasePokemonViewModel model = new BasePokemonViewModel(){
                Pokemon = this._dataService.GetPokemonById(id),
                AllGenerations = this._dataService.GetGenerations(),
                AllClassifications = this._dataService.GetClassifications(),
            };

            if(!id.Contains('-'))
            {
                model.AllBaseHappinesses = this._dataService.GetBaseHappinesses();
                model.AllCaptureRates = this._dataService.GetCaptureRates();
                model.AllEggCycles = this._dataService.GetEggCycles();
                model.AllExperienceGrowths = this._dataService.GetExperienceGrowths();
                model.AllGenderRatios = new List<GenderRatioViewModel>();
    
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
            }
            else
            {
                model.Name = model.Pokemon.Name + " (" + this._dataService.GetPokemonFormName(id) + ")";
            }

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_pokemon/{id}")]
        public IActionResult Pokemon(Pokemon pokemon, string id)
        {
            if (!this.ModelState.IsValid)
            {
                BasePokemonViewModel model = new BasePokemonViewModel(){
                    Pokemon = this._dataService.GetPokemonById(pokemon.Id),
                    AllGenerations = this._dataService.GetGenerations(),
                };
    
                if(!pokemon.Id.Contains('-'))
                {
                    model.AllBaseHappinesses = this._dataService.GetBaseHappinesses();
                    model.AllClassifications = this._dataService.GetClassifications();
                    model.AllCaptureRates = this._dataService.GetCaptureRates();
                    model.AllEggCycles = this._dataService.GetEggCycles();
                    model.AllExperienceGrowths = this._dataService.GetExperienceGrowths();
                    model.AllGenderRatios = new List<GenderRatioViewModel>();

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
                }
                else
                {
                    model.Name = model.Pokemon.Name + " (" + this._dataService.GetPokemonFormName(pokemon.Id) + ")";
                }

                return this.View(model);
            }
            else if (this._dataService.GetPokemonByPokedexNumber(pokemon.PokedexNumber) != null)
            {
                BasePokemonViewModel model = new BasePokemonViewModel(){
                    Pokemon = this._dataService.GetPokemonById(pokemon.Id),
                    AllGenerations = this._dataService.GetGenerations(),
                };
    
                if(!pokemon.Id.Contains('-'))
                {
                    model.AllBaseHappinesses = this._dataService.GetBaseHappinesses();
                    model.AllClassifications = this._dataService.GetClassifications();
                    model.AllCaptureRates = this._dataService.GetCaptureRates();
                    model.AllEggCycles = this._dataService.GetEggCycles();
                    model.AllExperienceGrowths = this._dataService.GetExperienceGrowths();
                    model.AllGenderRatios = new List<GenderRatioViewModel>();

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
                }
                else
                {
                    model.Name = model.Pokemon.Name + " (" + this._dataService.GetPokemonFormName(pokemon.Id) + ")";
                }

                this.ModelState.AddModelError("PokedexNumber", "A pokemon with a this pokedex number already exists.");

                return this.View(model);
            }

            this._dataService.UpdatePokemon(pokemon);

            if (!pokemon.Id.Contains('-'))
            {
                return this.RedirectToAction("Pokemon", "Admin");
            }
            else
            {
                return this.RedirectToAction("AltForms", "Edit", new { pokemonId = pokemon.Id.Substring(0, pokemon.Id.IndexOf('-')) });
            }
        }

        [HttpGet]
        [Route("edit_pokemon_image/{id}")]
        public IActionResult PokemonImage(string id)
        {
            Pokemon model = this._dataService.GetPokemonById(id);

            if(id.Contains('-'))
            {
                model.Name = model.Name + " (" + this._dataService.GetPokemonFormName(id) + ")";
            }

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_pokemon_image/{id}")]
        public async Task<IActionResult> PokemonImage(Pokemon pokemon, string id, IFormFile upload)
        {
            if (!this.ModelState.IsValid)
            {
                Pokemon model = this._dataService.GetPokemonById(id);

                if(id.Contains('-'))
                {
                    model.Name = model.Name + " (" + this._dataService.GetPokemonFormName(id) + ")";
                }
            }

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(_appConfig.FTPUrl + id.ToString() + upload.FileName.Substring(upload.FileName.LastIndexOf('.')));
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(_appConfig.FTPUsername, _appConfig.FTPPassword);

            using (var requestStream = request.GetRequestStream())  
            {  
                await upload.CopyToAsync(requestStream);  
            }

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                System.Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
            }

            if (!pokemon.Id.Contains('-'))
            {
                return this.RedirectToAction("Pokemon", "Admin");
            }
            else
            {
                return this.RedirectToAction("AltForms", "Edit", new { pokemonId = pokemon.Id.Substring(0, pokemon.Id.IndexOf('-')) });
            }
        }

        [HttpGet]
        [Route("edit_typing/{pokemonId}")]
        public IActionResult Typing(string pokemonId)
        {
            PokemonTypeDetail typeDetail = this._dataService.GetPokemonWithTypes(pokemonId);
            PokemonTypingViewModel model = new PokemonTypingViewModel()
            {
                Id = typeDetail.Id,
                AllTypes = this._dataService.GetTypes(),
                PokemonId = typeDetail.PokemonId,
                PrimaryTypeId = typeDetail.PrimaryTypeId,
                SecondaryTypeId = typeDetail.SecondaryTypeId
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_typing/{id}")]
        public IActionResult Typing(PokemonTypeDetail pokemonTypeDetail)
        {
            if (!this.ModelState.IsValid)
            {
                PokemonTypeDetail typeDetail = this._dataService.GetPokemonWithTypes(pokemonTypeDetail.PokemonId);
                PokemonTypingViewModel model = new PokemonTypingViewModel()
                {
                    Id = typeDetail.Id,
                    AllTypes = this._dataService.GetTypes(),
                    PokemonId = typeDetail.PokemonId,
                    PrimaryTypeId = typeDetail.PrimaryTypeId,
                    SecondaryTypeId = typeDetail.SecondaryTypeId
                };

                return this.View(model);
            }

            if (pokemonTypeDetail.PrimaryTypeId == pokemonTypeDetail.SecondaryTypeId)
            {
                pokemonTypeDetail.SecondaryTypeId = null;
            }

            this._dataService.UpdatePokemonTypeDetail(pokemonTypeDetail);

            if(!pokemonTypeDetail.PokemonId.Contains('-'))
            {
                return this.RedirectToAction("Pokemon", "Admin");
            }
            else
            {
                return this.RedirectToAction("AltForms", "Edit", new { pokemonId = pokemonTypeDetail.PokemonId.Substring(0, pokemonTypeDetail.PokemonId.IndexOf('-')) });
            }
        }

        [HttpGet]
        [Route("edit_abilities/{pokemonId}")]
        public IActionResult Abilities(string pokemonId)
        {
            PokemonAbilityDetail abilityDetail = this._dataService.GetPokemonWithAbilities(pokemonId);
            PokemonAbilitiesViewModel model = new PokemonAbilitiesViewModel()
            {
                Id = abilityDetail.Id,
                AllAbilities = this._dataService.GetAbilities(),
                PokemonId = abilityDetail.PokemonId,
                PrimaryAbilityId = abilityDetail.PrimaryAbilityId,
                SecondaryAbilityId = abilityDetail.SecondaryAbilityId,
                HiddenAbilityId = abilityDetail.HiddenAbilityId
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_abilities/{pokemonId}")]
        public IActionResult Abilities(PokemonAbilityDetail pokemonAbilityDetail)
        {
            if (!this.ModelState.IsValid)
            {
                PokemonAbilityDetail abilityDetail = this._dataService.GetPokemonWithAbilities(pokemonAbilityDetail.PokemonId);
                PokemonAbilitiesViewModel model = new PokemonAbilitiesViewModel()
                {
                    Id = abilityDetail.Id,
                    AllAbilities = this._dataService.GetAbilities(),
                    PokemonId = abilityDetail.PokemonId,
                    PrimaryAbilityId = abilityDetail.PrimaryAbilityId,
                    SecondaryAbilityId = abilityDetail.SecondaryAbilityId,
                    HiddenAbilityId = abilityDetail.HiddenAbilityId
                };

                return this.View(model);
            }

            this._dataService.UpdatePokemonAbilityDetail(pokemonAbilityDetail);

            if(!pokemonAbilityDetail.PokemonId.Contains('-'))
            {
                return this.RedirectToAction("Pokemon", "Admin");
            }
            else
            {
                return this.RedirectToAction("AltForms", "Edit", new { pokemonId = pokemonAbilityDetail.PokemonId.Substring(0, pokemonAbilityDetail.PokemonId.IndexOf('-')) });
            }
        }

        [HttpGet]
        [Route("edit_egg_groups/{pokemonId}")]
        public IActionResult EggGroups(string pokemonId)
        {
            PokemonEggGroupDetail eggGroupDetail = this._dataService.GetPokemonWithEggGroups(pokemonId);
            PokemonEggGroupsViewModel model = new PokemonEggGroupsViewModel()
            {
                Id = eggGroupDetail.Id,
                AllEggGroups = this._dataService.GetEggGroups(),
                PokemonId = eggGroupDetail.PokemonId,
                PrimaryEggGroupId = eggGroupDetail.PrimaryEggGroupId,
                SecondaryEggGroupId = eggGroupDetail.SecondaryEggGroupId
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_egg_groups/{pokemonId}")]
        public IActionResult EggGroups(PokemonEggGroupDetail pokemonEggGroupDetail)
        {
            if (!this.ModelState.IsValid)
            {
                PokemonEggGroupDetail eggGroupDetail = this._dataService.GetPokemonWithEggGroups(pokemonEggGroupDetail.PokemonId);
                PokemonEggGroupsViewModel model = new PokemonEggGroupsViewModel()
                {
                    Id = eggGroupDetail.Id,
                    AllEggGroups = this._dataService.GetEggGroups(),
                    PokemonId = eggGroupDetail.PokemonId,
                    PrimaryEggGroupId = eggGroupDetail.PrimaryEggGroupId,
                    SecondaryEggGroupId = eggGroupDetail.SecondaryEggGroupId
                };

                return this.View(model);
            }

            this._dataService.UpdatePokemonEggGroupDetail(pokemonEggGroupDetail);

            return this.RedirectToAction("Pokemon", "Admin");
        }

        [HttpGet]
        [Route("edit_base_stats/{pokemonId}")]
        public IActionResult BaseStats(string pokemonId)
        {
            BaseStat model = this._dataService.GetPokemonBaseStats(pokemonId);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_base_stats/{pokemonId}")]
        public IActionResult BaseStats(BaseStat baseStat)
        {
            if (!this.ModelState.IsValid)
            {
                BaseStat model = this._dataService.GetPokemonBaseStats(baseStat.PokemonId);

                return this.View(model);
            }

            this._dataService.UpdateBaseStat(baseStat);

            if(!baseStat.PokemonId.Contains('-'))
            {
                return this.RedirectToAction("Pokemon", "Admin");
            }
            else
            {
                return this.RedirectToAction("AltForms", "Edit", new { pokemonId = baseStat.PokemonId.Substring(0, baseStat.PokemonId.IndexOf('-')) });
            }
        }

        [HttpGet]
        [Route("edit_ev_yields/{pokemonId}")]
        public IActionResult EVYields(string pokemonId)
        {
            EVYield model = this._dataService.GetPokemonEVYields(pokemonId);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_ev_yields/{pokemonId}")]
        public IActionResult EVYields(EVYield evYield)
        {
            if (!this.ModelState.IsValid)
            {
                EVYield model = this._dataService.GetPokemonEVYields(evYield.PokemonId);

                return this.View(model);
            }
            else if (evYield.Health > 3)
            {
                EVYield model = this._dataService.GetPokemonEVYields(evYield.PokemonId);

                this.ModelState.AddModelError("EVTotal", "Health must be 3 or less.");
                return this.View(model);
            }
            else if (evYield.Attack > 3)
            {
                EVYield model = this._dataService.GetPokemonEVYields(evYield.PokemonId);

                this.ModelState.AddModelError("EVTotal", "Attack must be 3 or less.");
                return this.View(model);
            }
            else if (evYield.Defense > 3)
            {
                EVYield model = this._dataService.GetPokemonEVYields(evYield.PokemonId);

                this.ModelState.AddModelError("EVTotal", "Defense must be 3 or less.");
                return this.View(model);
            }
            else if (evYield.SpecialAttack > 3)
            {
                EVYield model = this._dataService.GetPokemonEVYields(evYield.PokemonId);

                this.ModelState.AddModelError("EVTotal", "Special Attack must be 3 or less.");
                return this.View(model);
            }
            else if (evYield.SpecialDefense > 3)
            {
                EVYield model = this._dataService.GetPokemonEVYields(evYield.PokemonId);

                this.ModelState.AddModelError("EVTotal", "Special Defense must be 3 or less.");
                return this.View(model);
            }
            else if (evYield.Speed > 3)
            {
                EVYield model = this._dataService.GetPokemonEVYields(evYield.PokemonId);

                this.ModelState.AddModelError("EVTotal", "Speed must be 3 or less.");
                return this.View(model);
            }
            else if (evYield.EVTotal > 3)
            {
                EVYield model = this._dataService.GetPokemonEVYields(evYield.PokemonId);

                this.ModelState.AddModelError("EVTotal", "EV Total must be 3 or less.");
                return this.View(model);
            }

            this._dataService.UpdateEVYield(evYield);

            return this.RedirectToAction("Pokemon", "Admin");
        }

        [HttpGet]
        [Route("edit_type/{id:int}")]
        public IActionResult Type(int id)
        {
            Type model = this._dataService.GetType(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_type/{id:int}")]
        public IActionResult Type(Type type)
        {
            if (!this.ModelState.IsValid)
            {
                Type model = this._dataService.GetType(type.Id);

                return this.View(model);
            }

            this._dataService.UpdateType(type);

            return this.RedirectToAction("Types", "Admin");
        }

        [HttpGet]
        [Route("edit_shiny_hunting_technique/{id:int}")]
        public IActionResult ShinyHuntingTechnique(int id)
        {
            ShinyHuntingTechnique model = this._dataService.GetShinyHuntingTechnique(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_shiny_hunting_technique/{id:int}")]
        public IActionResult ShinyHuntingTechnique(ShinyHuntingTechnique shinyHuntingTechnique)
        {
            if (!this.ModelState.IsValid)
            {
                ShinyHuntingTechnique model = this._dataService.GetShinyHuntingTechnique(shinyHuntingTechnique.Id);

                return this.View(model);
            }

            this._dataService.UpdateShinyHuntingTechnique(shinyHuntingTechnique);

            return this.RedirectToAction("ShinyHuntingTechniques", "Admin");
        }

        [HttpGet]
        [Route("edit_egg_group/{id:int}")]
        public IActionResult EggGroup(int id)
        {
            EggGroup model = this._dataService.GetEggGroup(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_egg_group/{id:int}")]
        public IActionResult EggGroup(EggGroup eggGroup)
        {
            if (!this.ModelState.IsValid)
            {
                EggGroup model = this._dataService.GetEggGroup(eggGroup.Id);

                return this.View(model);
            }

            this._dataService.UpdateEggGroup(eggGroup);

            return this.RedirectToAction("EggGroups", "Admin");
        }

        [HttpGet]
        [Route("edit_form_item/{id:int}")]
        public IActionResult FormItem(int id)
        {
            FormItem model = this._dataService.GetFormItem(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_form_item/{id:int}")]
        public IActionResult FormItem(FormItem formItem)
        {
            if (!this.ModelState.IsValid)
            {
                FormItem model = this._dataService.GetFormItem(formItem.Id);

                return this.View(model);
            }

            this._dataService.UpdateFormItem(formItem);

            return this.RedirectToAction("FormItems", "Admin");
        }

        [HttpGet]
        [Route("edit_form/{id:int}")]
        public IActionResult Form(int id)
        {
            Form model = this._dataService.GetForm(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_form/{id:int}")]
        public IActionResult Form(Form form)
        {
            if (!this.ModelState.IsValid)
            {
                Form model = this._dataService.GetForm(form.Id);

                return this.View(model);
            }

            this._dataService.UpdateForm(form);

            return this.RedirectToAction("Forms", "Admin");
        }

        [HttpGet]
        [Route("edit_nature/{id:int}")]
        public IActionResult Nature(int id)
        {
            Nature model = this._dataService.GetNature(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_nature/{id:int}")]
        public IActionResult Nature(Nature nature)
        {
            if (!this.ModelState.IsValid)
            {
                Nature model = this._dataService.GetNature(nature.Id);

                return this.View(model);
            }

            this._dataService.UpdateNature(nature);

            return this.RedirectToAction("Natures", "Admin");
        }

        [HttpGet]
        [Route("edit_legendary_type/{id:int}")]
        public IActionResult LegendaryType(int id)
        {
            LegendaryType model = this._dataService.GetLegendaryType(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_legendary_type/{id:int}")]
        public IActionResult LegendaryType(LegendaryType legendaryType)
        {
            if (!this.ModelState.IsValid)
            {
                LegendaryType model = this._dataService.GetLegendaryType(legendaryType.Id);

                return this.View(model);
            }

            this._dataService.UpdateLegendaryType(legendaryType);

            return this.RedirectToAction("LegendaryTypes", "Admin");
        }

        [HttpGet]
        [Route("edit_classification/{id:int}")]
        public IActionResult Classification(int id)
        {
            Classification model = this._dataService.GetClassification(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_classification/{id:int}")]
        public IActionResult Classification(Classification classification)
        {
            if (!this.ModelState.IsValid)
            {
                Classification model = this._dataService.GetClassification(classification.Id);

                return this.View(model);
            }

            this._dataService.UpdateClassification(classification);

            return this.RedirectToAction("Classifications", "Admin");
        }

        [HttpGet]
        [Route("edit_shiny_hunt/{id:int}")]
        public IActionResult ShinyHunt(int id)
        {
            ShinyHunt model = this._dataService.GetShinyHunt(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_shiny_hunt/{id:int}")]
        public IActionResult ShinyHunt(ShinyHunt shinyHunt)
        {
            if (!this.ModelState.IsValid)
            {
                ShinyHunt model = this._dataService.GetShinyHunt(shinyHunt.Id);

                return this.View(model);
            }

            this._dataService.UpdateShinyHunt(shinyHunt);

            return this.RedirectToAction("ShinyHunts", "Admin");
        }

        [HttpGet]
        [Route("edit_ability/{id:int}")]
        public IActionResult Ability(int id)
        {
            Ability model = this._dataService.GetAbility(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_ability/{id:int}")]
        public IActionResult Ability(Ability ability)
        {
            if (!this.ModelState.IsValid)
            {
                Ability model = this._dataService.GetAbility(ability.Id);

                return this.View(model);
            }

            this._dataService.UpdateAbility(ability);

            return this.RedirectToAction("Abilities", "Admin");
        }

        [HttpGet]
        [Route("edit_evolution/{pokemonId}")]
        public IActionResult Evolution(string pokemonId)
        {
            Evolution preEvolution = this._dataService.GetEvolutions().Find(x => x.EvolutionPokemonId == pokemonId);
            EvolutionViewModel model = new EvolutionViewModel()
            {
                AllEvolutionMethods = this._dataService.GetEvolutionMethods(),
                Id = preEvolution.Id,
                EvolutionDetails = preEvolution.EvolutionDetails,
                EvolutionMethodId = preEvolution.EvolutionMethodId,
                EvolutionMethod = preEvolution.EvolutionMethod,
                PreevolutionPokemon = preEvolution.PreevolutionPokemon,
                PreevolutionPokemonId = preEvolution.PreevolutionPokemonId,
                EvolutionPokemonId = preEvolution.EvolutionPokemonId,
                EvolutionPokemon = preEvolution.EvolutionPokemon,
            };

            List<Pokemon> pokemonList = this._dataService.GetAllPokemonIncludeIncomplete().Where(x => x.Id != pokemonId).ToList();
            foreach (var pokemon in pokemonList.Where(p => p.Id.Contains('-')))
            {
                pokemon.Name += " (" + this._dataService.GetPokemonFormName(pokemon.Id) + ")";
            }

            model.AllPokemon = pokemonList;

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_evolution/{pokemonId}")]
        public IActionResult Evolution(EvolutionViewModel evolution)
        {
            if (!this.ModelState.IsValid)
            {
                Evolution preEvolution = this._dataService.GetEvolutions().Find(x => x.EvolutionPokemonId == evolution.EvolutionPokemonId);
                EvolutionViewModel model = new EvolutionViewModel()
                {
                    AllEvolutionMethods = this._dataService.GetEvolutionMethods(),
                    Id = evolution.Id,
                    EvolutionDetails = preEvolution.EvolutionDetails,
                    EvolutionMethodId = preEvolution.EvolutionMethodId,
                    EvolutionMethod = preEvolution.EvolutionMethod,
                    PreevolutionPokemon = preEvolution.PreevolutionPokemon,
                    PreevolutionPokemonId = preEvolution.PreevolutionPokemonId,
                    EvolutionPokemonId = preEvolution.EvolutionPokemonId,
                    EvolutionPokemon = preEvolution.EvolutionPokemon,
                };

                List<Pokemon> pokemonList = this._dataService.GetAllPokemon().Where(x => x.Id != evolution.EvolutionPokemonId).ToList();
                foreach (var pokemon in pokemonList.Where(p => p.Id.Contains('-')))
                {
                    pokemon.Name += " (" + this._dataService.GetPokemonFormName(pokemon.Id) + ")";
                }

                model.AllPokemon = pokemonList;

                return this.View(model);
            }

            this._dataService.UpdateEvolution(evolution);

            return this.RedirectToAction("Pokemon", "Admin");
        }

        [Route("edit_alternate_forms/{pokemonId}")]
        public IActionResult AltForms(string pokemonId)
        {
            Pokemon originalPokemon = this._dataService.GetPokemonById(pokemonId);
            List<Pokemon> altFormList = this._dataService.GetAllPokemonIncludeIncomplete().Where(x => x.Name == originalPokemon.Name && x.Id.Contains('-')).ToList();
            foreach (var pokemon in altFormList)
            {
                pokemon.Name += " (" + this._dataService.GetPokemonFormName(pokemon.Id) + ")";
            }

            AllAdminPokemonViewModel allPokemon = new AllAdminPokemonViewModel(){
                AllPokemon = altFormList,
                AllAltForms = this._dataService.GetAllAltForms(),
                AllEvolutions = this._dataService.GetEvolutions(),
                AllTypings = this._dataService.GetAllPokemonWithTypesAndIncomplete(),
                AllAbilities = this._dataService.GetAllPokemonWithAbilitiesAndIncomplete(),
                AllEggGroups = this._dataService.GetAllPokemonWithEggGroupsAndIncomplete(),
                AllBaseStats = this._dataService.GetBaseStatsWithIncomplete(),
                AllEVYields = this._dataService.GetEVYieldsWithIncomplete(),
                AllLegendaryDetails = this._dataService.GetAllPokemonWithLegendaryTypes(),
            };

            DropdownViewModel model = new DropdownViewModel(){
                AllPokemon = allPokemon,
                AppConfig = this._appConfig,
            };

            return this.View(model);
        }

        [HttpGet]
        [Route("edit_alternate_form/{pokemonId}")]
        public IActionResult AltFormsForm(string pokemonId)
        {
            PokemonFormDetail pokemonForm = this._dataService.GetPokemonFormDetailByAltFormId(pokemonId);
            AlternateFormsFormViewModel model = new AlternateFormsFormViewModel()
            {
                pokemonFormDetail = pokemonForm,
                AllForms = this._dataService.GetForms(),
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_alternate_form/{pokemonId}")]
        public IActionResult AltFormsForm(PokemonFormDetail pokemonFormDetail)
        {
            if (!this.ModelState.IsValid)
            {
                PokemonFormDetail pokemonForm = this._dataService.GetPokemonFormDetailByAltFormId(pokemonFormDetail.AltFormPokemonId);
                AlternateFormsFormViewModel model = new AlternateFormsFormViewModel()
                {
                    pokemonFormDetail = pokemonForm,
                    AllForms = this._dataService.GetForms(),
                };

                return this.View(model);
            }

            this._dataService.UpdatePokemonFormDetail(pokemonFormDetail);

            return this.RedirectToAction("AltForms", "Edit", new { pokemonId = pokemonFormDetail.OriginalPokemonId });
        }

        private void UpdateAltForm(Pokemon newPokemon, string oldNumber, string originalNumber)
        {
            PokemonTypeDetail typing = this._dataService.GetPokemonWithTypesNoIncludes(oldNumber);
            PokemonAbilityDetail abilities = this._dataService.GetPokemonWithAbilitiesNoIncludes(oldNumber);
            PokemonEggGroupDetail eggGroups = this._dataService.GetPokemonWithEggGroupsNoIncludes(oldNumber);
            BaseStat baseStats = this._dataService.GetPokemonBaseStatsNoIncludes(oldNumber);
            EVYield evYields = this._dataService.GetEVYieldNoIncludes(oldNumber);
            Evolution preEvolution = this._dataService.GetPreEvolutionNoEdit(oldNumber);
            List<Evolution> evolutions = this._dataService.GetPokemonEvolutionsNoEdit(oldNumber);
            PokemonFormDetail altFormDetail = this._dataService.GetPokemonFormDetailByAltFormId(oldNumber);

            this._dataService.AddPokemon(newPokemon);

            if (typing != null)
            {
                typing.PokemonId = newPokemon.Id;
                this._dataService.UpdatePokemonTypeDetail(typing);
            }

            if (abilities != null)
            {
                abilities.PokemonId = newPokemon.Id;
                this._dataService.UpdatePokemonAbilityDetail(abilities);
            }

            if (eggGroups != null)
            {
                eggGroups.PokemonId = newPokemon.Id;
                this._dataService.UpdatePokemonEggGroupDetail(eggGroups);
            }

            if (baseStats != null)
            {
                baseStats.PokemonId = newPokemon.Id;
                this._dataService.UpdateBaseStat(baseStats);
            }

            if (evYields != null)
            {
                evYields.PokemonId = newPokemon.Id;
                this._dataService.UpdateEVYield(evYields);
            }

            if (preEvolution != null)
            {
                preEvolution.EvolutionPokemonId = newPokemon.Id;
                this._dataService.UpdateEvolution(preEvolution);
            }

            if (evolutions.Count() > 0)
            {
                foreach(var e in evolutions)
                {
                    e.PreevolutionPokemonId = newPokemon.Id;
                    this._dataService.UpdateEvolution(e);
                }
            }

            altFormDetail.AltFormPokemonId = newPokemon.Id;
            altFormDetail.OriginalPokemonId = originalNumber;
            this._dataService.UpdatePokemonFormDetail(altFormDetail);

            this._dataService.DeletePokemon(oldNumber);

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(_appConfig.FTPUrl + oldNumber + ".png");
            request.Credentials = new NetworkCredential(_appConfig.FTPUsername, _appConfig.FTPPassword);
            request.Method = WebRequestMethods.Ftp.Rename;
            request.RenameTo = newPokemon.Id + ".png";
            request.GetResponse();
        }
    }
}