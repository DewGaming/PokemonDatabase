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
        [Route("add_evolution/{pokemonId:int}")]
        public IActionResult Evolution(int pokemonId)
        {
            Pokemon evolutionPokemon = this._dataService.GetPokemonById(pokemonId);
            EvolutionViewModel model = new EvolutionViewModel()
            {
                AllEvolutionMethods = this._dataService.GetEvolutionMethods(),
                EvolutionPokemon = evolutionPokemon,
                EvolutionPokemonId = evolutionPokemon.Id,
            };

            List<Pokemon> pokemonList = this._dataService.GetAllPokemonIncludeIncomplete().Where(x => x.Id != pokemonId).ToList();
            foreach (var pokemon in pokemonList.Where(x => this._dataService.CheckIfAltForm(x.Id)))
            {
                pokemon.Name += " (" + this._dataService.GetPokemonFormName(pokemon.Id) + ")";
            }

            model.AllPokemon = pokemonList;
            
            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_evolution/{pokemonId:int}")]
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
                List<Pokemon> pokemonList = this._dataService.GetAllPokemonIncludeIncomplete().Where(x => x.Id != evolution.EvolutionPokemonId).ToList();
                foreach (var pokemon in pokemonList.Where(x => this._dataService.CheckIfAltForm(x.Id)))
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
        [Route("add_form_item")]
        public IActionResult FormItem()
        {
            FormItemViewModel model = new FormItemViewModel()
            {
                AllPokemon = this._dataService.GetAllPokemonOnlyForms(),
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_form_item")]
        public IActionResult FormItem(FormItem formItem)
        {
            if (!this.ModelState.IsValid)
            {
                FormItemViewModel model = new FormItemViewModel()
                {
                    AllPokemon = this._dataService.GetAllPokemonOnlyForms(),
                };

                return this.View(model);
            }

            this._dataService.AddFormItem(formItem);

            return this.RedirectToAction("FormItems", "Admin");
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
        [Route("add_egg_cycle")]
        public IActionResult EggCycle()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_egg_cycle")]
        public IActionResult EggCycle(EggCycle eggCycle)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this._dataService.AddEggCycle(eggCycle);

            return this.RedirectToAction("EggCycles", "Admin");
        }

        [HttpGet]
        [Route("add_experience_growth")]
        public IActionResult ExperienceGrowth()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_experience_growth")]
        public IActionResult ExperienceGrowth(ExperienceGrowth experienceGrowth)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this._dataService.AddExperienceGrowth(experienceGrowth);

            return this.RedirectToAction("ExperienceGrowths", "Admin");
        }

        [HttpGet]
        [Route("add_gender_ratio")]
        public IActionResult GenderRatio()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_gender_ratio")]
        public IActionResult GenderRatio(GenderRatio genderRatio)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this._dataService.AddGenderRatio(genderRatio);

            return this.RedirectToAction("GenderRatios", "Admin");
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
        [Route("add_move/{typeId:int}")]
        public IActionResult Move(int typeId)
        {
            MoveViewModel model = new MoveViewModel()
            {
                MoveTypeId = typeId,
                AllTypes = this._dataService.GetTypes(),
                AllMoveCategories = this._dataService.GetMoveCategories(),
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_move/{typeId:int}")]
        public IActionResult Move(Move move)
        {
            if (!this.ModelState.IsValid)
            {
                MoveViewModel model = new MoveViewModel()
                {
                    MoveTypeId = move.MoveTypeId,
                    AllTypes = this._dataService.GetTypes(),
                    AllMoveCategories = this._dataService.GetMoveCategories(),
                };
                
                return this.View(model);
            }

            this._dataService.AddMove(move);

            return this.RedirectToAction("Moves", "Admin");
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

            if(classification.Name.Contains("pokemon"))
            {
                classification.Name = classification.Name.Replace("pokemon", "Pokemon");
            }
            else if(!classification.Name.Contains("Pokemon") && !classification.Name.Contains("pokemon"))
            {
                classification.Name = classification.Name.Trim() + " Pokemon";
            }

            this._dataService.AddClassification(classification);

            return this.RedirectToAction("Classifications", "Admin");
        }

        [HttpGet]
        [Route("add_nature")]
        public IActionResult Nature()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_nature")]
        public IActionResult Nature(Nature nature)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this._dataService.AddNature(nature);

            return this.RedirectToAction("Natures", "Admin");
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
        [Route("add_evolution_method")]
        public IActionResult EvolutionMethod()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_evolution_method")]
        public IActionResult EvolutionMethod(EvolutionMethod evolutionMethod)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this._dataService.AddEvolutionMethod(evolutionMethod);

            return this.RedirectToAction("EvolutionMethods", "Admin");
        }

        [HttpGet]
        [Route("add_capture_rate")]
        public IActionResult CaptureRate()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_capture_rate")]
        public IActionResult CaptureRate(CaptureRate captureRate)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this._dataService.AddCaptureRate(captureRate);

            return this.RedirectToAction("CaptureRates", "Admin");
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
        [Route("add_base_happiness")]
        public IActionResult BaseHappiness()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_base_happiness")]
        public IActionResult BaseHappiness(BaseHappiness baseHappiness)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this._dataService.AddBaseHappiness(baseHappiness);

            return this.RedirectToAction("BaseHappinesses", "Admin");
        }

        [HttpGet]
        [Route("add_game")]
        public IActionResult Game()
        {
            GameViewModel model = new GameViewModel(){
                AllGenerations = this._dataService.GetGenerations(),
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_game")]
        public IActionResult Game(Game game)
        {
            if (!this.ModelState.IsValid)
            {
                GameViewModel model = new GameViewModel(){
                    AllGenerations = this._dataService.GetGenerations(),
                };

                return this.View(model);
            }

            this._dataService.AddGame(game);

            return this.RedirectToAction("Games", "Admin");
        }

        [HttpGet]
        [Route("add_battle_item")]
        public IActionResult BattleItem()
        {
            List<Pokemon> pokemonList = this._dataService.GetAllPokemon();
            foreach(var p in pokemonList.Where(x => this._dataService.CheckIfAltForm(x.Id)))
            {
                p.Name += " (" + this._dataService.GetFormByAltFormId(p.Id).Name + ")";
            }

            BattleItemViewModel model = new BattleItemViewModel(){
                AllGenerations = this._dataService.GetGenerations().Where(x => x.Id >= 1).ToList(),
                AllPokemon = pokemonList,
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_battle_item")]
        public IActionResult BattleItem(BattleItem battleItem)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this._dataService.AddBattleItem(battleItem);

            return this.RedirectToAction("BattleItems", "Admin");
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
                AllGames = this._dataService.GetGames(),
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

            model.GameId = this._dataService.GetGames().Last().Id;

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_pokemon")]
        public async Task<IActionResult> Pokemon(BasePokemonViewModel newPokemon, IFormFile upload)
        {
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
                    AllGames = this._dataService.GetGames(),
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
            else if (this._dataService.GetPokemonByPokedexNumber(newPokemon.PokedexNumber) != null)
            {
                BasePokemonViewModel model = new BasePokemonViewModel()
                {
                    AllBaseHappinesses = this._dataService.GetBaseHappinesses(),
                    AllClassifications = this._dataService.GetClassifications(),
                    AllCaptureRates = this._dataService.GetCaptureRates(),
                    AllEggCycles = this._dataService.GetEggCycles(),
                    AllExperienceGrowths = this._dataService.GetExperienceGrowths(),
                    AllGenderRatios = new List<GenderRatioViewModel>(),
                    AllGames = this._dataService.GetGames(),
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
            else if (!upload.FileName.Contains(".png"))
            {
                BasePokemonViewModel model = new BasePokemonViewModel()
                {
                    AllBaseHappinesses = this._dataService.GetBaseHappinesses(),
                    AllClassifications = this._dataService.GetClassifications(),
                    AllCaptureRates = this._dataService.GetCaptureRates(),
                    AllEggCycles = this._dataService.GetEggCycles(),
                    AllExperienceGrowths = this._dataService.GetExperienceGrowths(),
                    AllGenderRatios = new List<GenderRatioViewModel>(),
                    AllGames = this._dataService.GetGames(),
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
                
                this.ModelState.AddModelError("Picture", "The image must be in the .png format.");
                return this.View(model);
            }

            if(upload != null)
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(_appConfig.PokemonImageFTPUrl + newPokemon.Id.ToString() + upload.FileName.Substring(upload.FileName.LastIndexOf('.')));
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
            }
            else
            {
                WebClient request = new WebClient();
                request.Credentials = new NetworkCredential(_appConfig.FTPUsername, _appConfig.FTPPassword);

                byte[] file = request.DownloadData(_appConfig.WebUrl + "/images/general/tempPhoto.png");
                
                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(_appConfig.PokemonImageFTPUrl + newPokemon.Id.ToString() + ".png");
                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
                ftpRequest.Credentials = new NetworkCredential(_appConfig.FTPUsername, _appConfig.FTPPassword);

                using (var requestStream = ftpRequest.GetRequestStream())
                {
                    requestStream.Write(file, 0, file.Length);
                }

                using (FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse())
                {
                    Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
                }
            }

            this._dataService.AddPokemon(newPokemon);

            this._dataService.AddPokemonGameDetail(new PokemonGameDetail()
            {
                PokemonId = newPokemon.Id,
                GameId = newPokemon.GameId,
            });

            return this.RedirectToAction("Typing", "Add", new { pokemonId = newPokemon.Id });
        }

        [HttpGet]
        [Route("add_alternate_form/{pokemonId:int}")]
        public IActionResult AltForm(int pokemonId)
        {
            Pokemon originalPokemon = this._dataService.GetPokemonById(pokemonId);
            List<Game> games = this._dataService.GetGames().Where(x => x.ReleaseDate >= originalPokemon.Game.ReleaseDate).ToList();

            AlternateFormViewModel model = new AlternateFormViewModel()
            {
                AllForms = this._dataService.GetForms(),
                AllClassifications = this._dataService.GetClassifications(),
                AllGames = games,
                OriginalPokemon = originalPokemon,
                OriginalPokemonId = originalPokemon.Id,
            };

            model.GameId = this._dataService.GetGames().Last().Id;

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_alternate_form/{pokemonId:int}")]
        public async Task<IActionResult> AltForm(AlternateFormViewModel pokemon, IFormFile upload)
        {
            List<PokemonFormDetail> originalPokemonForms = this._dataService.GetPokemonFormsWithIncomplete(pokemon.OriginalPokemonId);
            if (!this.ModelState.IsValid)
            {
                Pokemon originalPokemon = this._dataService.GetPokemonById(pokemon.OriginalPokemonId);
                List<Game> games = this._dataService.GetGames().Where(x => x.ReleaseDate >= originalPokemon.Game.ReleaseDate).ToList();

                AlternateFormViewModel model = new AlternateFormViewModel()
                {
                    AllForms = this._dataService.GetForms(),
                    AllClassifications = this._dataService.GetClassifications(),
                    AllGames = games,
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
                    List<Game> games = this._dataService.GetGames().Where(x => x.ReleaseDate >= originalPokemon.Game.ReleaseDate).ToList();

                    AlternateFormViewModel model = new AlternateFormViewModel()
                    {
                        AllForms = this._dataService.GetForms(),
                        AllClassifications = this._dataService.GetClassifications(),
                        AllGames = games,
                        OriginalPokemon = originalPokemon,
                        OriginalPokemonId = originalPokemon.Id,
                    };
    
                    this.ModelState.AddModelError("Alternate Form Name", "Original Pokemon already has an alternate form of this type.");
                    return this.View(model);
                }
            }

            Pokemon alternatePokemon = this._dataService.GetPokemonNoIncludesById(pokemon.OriginalPokemonId);

            alternatePokemon.Id = 0;
            alternatePokemon.PokedexNumber = this._dataService.GetPokemonByIdNoIncludes(pokemon.OriginalPokemonId).PokedexNumber;
            alternatePokemon.Height = pokemon.Height;
            alternatePokemon.Weight = pokemon.Weight;
            alternatePokemon.ExpYield = pokemon.ExpYield;
            alternatePokemon.GameId = pokemon.GameId;
            alternatePokemon.ClassificationId = pokemon.ClassificationId;
            alternatePokemon.IsComplete = false;

            if(upload != null)
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(_appConfig.PokemonImageFTPUrl + alternatePokemon.Id.ToString() + upload.FileName.Substring(upload.FileName.LastIndexOf('.')));
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
            }
            else
            {
                WebClient request = new WebClient();
                request.Credentials = new NetworkCredential(_appConfig.FTPUsername, _appConfig.FTPPassword);

                byte[] file = request.DownloadData(_appConfig.WebUrl + "/images/general/tempPhoto.png");
                
                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(_appConfig.PokemonImageFTPUrl + alternatePokemon.Id.ToString() + ".png");
                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
                ftpRequest.Credentials = new NetworkCredential(_appConfig.FTPUsername, _appConfig.FTPPassword);

                using (var requestStream = ftpRequest.GetRequestStream())
                {
                    requestStream.Write(file, 0, file.Length);
                }

                using (FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse())
                {
                    Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
                }
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

            this._dataService.AddPokemonGameDetail(new PokemonGameDetail()
            {
                PokemonId = alternatePokemon.Id,
                GameId = alternatePokemon.GameId,
            });

            return this.RedirectToAction("Typing", "Add", new { pokemonId = alternatePokemon.Id });
        }

        [Route("add_game_availability/{pokemonId:int}/{gameId:int}")]
        public IActionResult PokemonGameDetail(int pokemonId, int gameId)
        {
            this._dataService.AddPokemonGameDetail(new PokemonGameDetail()
            {
                PokemonId = pokemonId,
                GameId = gameId,
            });

            return this.RedirectToAction("PokemonGameDetails", "Admin", new { pokemonId = pokemonId });
        }

        [HttpGet]
        [Route("add_typing/{pokemonId:int}")]
        public IActionResult Typing(int pokemonId)
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
        [Route("add_typing/{pokemonId:int}")]
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
        [Route("add_abilities/{pokemonId:int}")]
        public IActionResult Abilities(int pokemonId)
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
        [Route("add_abilities/{pokemonId:int}")]
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
            else if (this._dataService.CheckIfAltForm(abilities.PokemonId))
            {
                return this.RedirectToAction("BaseStats", "Add", new { pokemonId = abilities.PokemonId });
            }
            else
            {
                return this.RedirectToAction("EggGroups", "Add", new { pokemonId = abilities.PokemonId });
            }
        }

        [HttpGet]
        [Route("add_special_event_ability/{pokemonId:int}")]
        public IActionResult SpecialEventAbility(int pokemonId)
        {
            _cameFromAdminAltForms = Request.Headers["Referer"].ToString().Contains("admin/edit_alternate_forms");

            SpecialEventAbilityViewModel model = new SpecialEventAbilityViewModel()
            {
                AllAbilities = this._dataService.GetAbilities(),
                PokemonId = pokemonId,
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_special_event_ability/{pokemonId:int}")]
        public IActionResult SpecialEventAbility(SpecialEventAbilityViewModel ability)
        {
            if (!this.ModelState.IsValid)
            {
                SpecialEventAbilityViewModel model = new SpecialEventAbilityViewModel()
                {
                    AllAbilities = this._dataService.GetAbilities(),
                    PokemonId = ability.PokemonId,
                };

                return this.View(model);
            }

            PokemonAbilityDetail pokemonAbilities = this._dataService.GetPokemonWithAbilitiesNoIncludes(ability.PokemonId);

            pokemonAbilities.SpecialEventAbilityId = ability.AbilityId;

            this._dataService.UpdatePokemonAbilityDetail(pokemonAbilities);

            if (_cameFromAdminAltForms)
            {
                _cameFromAdminAltForms = false;
                PokemonFormDetail pokemon = this._dataService.GetPokemonFormDetailByAltFormId(ability.PokemonId);
                return this.RedirectToAction("AltForms", "Edit", new { pokemonId = pokemon.OriginalPokemonId });
            }
            else
            {
                return this.RedirectToAction("Pokemon", "Admin");
            }
        }

        [HttpGet]
        [Route("add_egg_groups/{pokemonId:int}")]
        public IActionResult EggGroups(int pokemonId)
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
        [Route("add_egg_groups/{pokemonId:int}")]
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
                return this.RedirectToAction("Pokemon", "Admin");
            }
            else
            {
                return this.RedirectToAction("BaseStats", "Add", new { pokemonId = eggGroups.PokemonId });
            }
        }

        [HttpGet]
        [Route("add_base_stats/{pokemonId:int}")]
        public IActionResult BaseStats(int pokemonId)
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
        [Route("add_base_stats/{pokemonId:int}")]
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

            if (this._dataService.CheckIfAltForm(baseStat.PokemonId))
            {
                return this.RedirectToAction("Pokemon", "Admin");
            }
            else
            {
                return this.RedirectToAction("EVYields", "Add", new { pokemonId = baseStat.PokemonId });
            }
        }

        [HttpGet]
        [Route("add_ev_yields/{pokemonId:int}")]
        public IActionResult EVYields(int pokemonId)
        {
            EVYield model = new EVYield()
            {
                PokemonId = pokemonId,
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_ev_yields/{pokemonId:int}")]
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

            return this.RedirectToAction("Pokemon", "Admin");
        }

        [HttpGet]
        [Route("set_legendary_type/{pokemonId:int}")]
        public IActionResult PokemonLegendaryDetails(int pokemonId)
        {
            PokemonLegendaryViewModel model = new PokemonLegendaryViewModel()
            {
                AllLegendaryTypes = this._dataService.GetLegendaryTypes(),
                PokemonId = pokemonId,
                Pokemon = this._dataService.GetPokemonById(pokemonId),
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("set_legendary_type/{pokemonId:int}")]
        public IActionResult PokemonLegendaryDetails(PokemonLegendaryViewModel pokemonLegendaryDetails)
        {
            if (!this.ModelState.IsValid)
            {
                PokemonLegendaryViewModel model = new PokemonLegendaryViewModel()
                {
                    AllLegendaryTypes = this._dataService.GetLegendaryTypes(),
                    PokemonId = pokemonLegendaryDetails.PokemonId,
                    Pokemon = this._dataService.GetPokemonById(pokemonLegendaryDetails.PokemonId),
                };

                return this.View(model);
            }

            this._dataService.AddPokemonLegendaryDetails(pokemonLegendaryDetails);

            return this.RedirectToAction("Pokemon", "Admin");
        }
    }
}