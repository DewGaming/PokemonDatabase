using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Pokedex.DataAccess.Models;
using Pokedex.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Pokedex.Controllers
{
    /// <summary>
    /// The class that is used to represent the add controller.
    /// </summary>
    [Authorize(Roles = "Owner")]
    [Route("admin")]
    public class AddController : Controller
    {
        private readonly DataService dataService;

        private readonly AppConfig appConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddController"/> class.
        /// </summary>
        /// <param name="appConfig">The application's configuration.</param>
        /// <param name="dataContext">The pokemon database's context.</param>
        public AddController(IOptions<AppConfig> appConfig, DataContext dataContext)
        {
            // Instantiate an instance of the data service.
            this.appConfig = appConfig.Value;
            this.dataService = new DataService(dataContext);
        }

        [HttpGet]
        [Route("add_evolution/{pokemonId:int}")]
        public IActionResult Evolution(int pokemonId)
        {
            Pokemon evolutionPokemon = this.dataService.GetPokemonById(pokemonId);
            EvolutionViewModel model = new EvolutionViewModel()
            {
                AllEvolutionMethods = this.dataService.GetEvolutionMethods(),
                EvolutionPokemon = evolutionPokemon,
                EvolutionPokemonId = evolutionPokemon.Id,
            };

            List<Pokemon> pokemonList = this.dataService.GetAllPokemonIncludeIncomplete().Where(x => x.Id != pokemonId).ToList();
            List<Pokemon> altFormsList = this.dataService.GetAllAltForms().Select(x => x.AltFormPokemon).ToList();
            foreach (var pokemon in pokemonList.Where(x => altFormsList.Any(y => y.Id == x.Id)))
            {
                pokemon.Name = string.Concat(pokemon.Name, " (", this.dataService.GetPokemonFormName(pokemon.Id), ")");
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
                    AllEvolutionMethods = this.dataService.GetEvolutionMethods(),
                    EvolutionPokemon = evolution.EvolutionPokemon,
                    EvolutionPokemonId = evolution.EvolutionPokemon.Id,
                };
                List<Pokemon> pokemonList = this.dataService.GetAllPokemonIncludeIncomplete().Where(x => x.Id != evolution.EvolutionPokemonId).ToList();
                List<Pokemon> altFormsList = this.dataService.GetAllAltForms().Select(x => x.AltFormPokemon).ToList();
                foreach (var pokemon in pokemonList.Where(x => altFormsList.Any(y => y.Id == x.Id)))
                {
                    pokemon.Name = string.Concat(pokemon.Name, " (", this.dataService.GetPokemonFormName(pokemon.Id), ")");
                }

                model.AllPokemon = pokemonList;

                return this.View(model);
            }

            this.dataService.AddEvolution(evolution);

            if (this.dataService.GetPokemonById(evolution.EvolutionPokemonId).IsComplete)
            {
                return this.RedirectToAction("Pokemon", "Admin");
            }
            else
            {
                return this.RedirectToAction("ReviewPokemon", "Owner", new { pokemonId = evolution.EvolutionPokemonId });
            }
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

            this.dataService.AddShinyHuntingTechnique(shinyHuntingTechnique);

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

            this.dataService.AddGeneration(generation);

            return this.RedirectToAction("Generations", "Admin");
        }

        [HttpGet]
        [Route("add_form_item")]
        public IActionResult FormItem()
        {
            FormItemViewModel model = new FormItemViewModel();
            List<Pokemon> altForms = this.dataService.GetPokemonFormDetailsByFormName("Mega").Select(x => x.AltFormPokemon).ToList();
            altForms.AddRange(this.dataService.GetPokemonFormDetailsByFormName("Mega X").Select(x => x.AltFormPokemon).ToList());
            altForms.AddRange(this.dataService.GetPokemonFormDetailsByFormName("Mega Y").Select(x => x.AltFormPokemon).ToList());
            altForms.AddRange(this.dataService.GetPokemonFormDetailsByFormName("Primal").Select(x => x.AltFormPokemon).ToList());
            altForms.AddRange(this.dataService.GetPokemonFormDetailsByFormName("Origin").Select(x => x.AltFormPokemon).ToList());
            altForms.AddRange(this.dataService.GetPokemonFormDetailsByFormName("Ultra").Select(x => x.AltFormPokemon).ToList());
            altForms.AddRange(this.dataService.GetPokemonFormDetailsByFormName("Crowned").Select(x => x.AltFormPokemon).ToList());

            foreach (var p in this.dataService.GetFormItems().Select(x => x.Pokemon))
            {
                altForms.Remove(altForms.Find(x => x.PokedexNumber == p.PokedexNumber));
            }

            altForms.Remove(altForms.Find(x => x.Name == "Rayquaza"));

            foreach (var p in altForms)
            {
                p.Name = string.Concat(p.Name, " (", this.dataService.GetPokemonFormName(p.Id), ")");
            }

            model.AllPokemon = altForms;

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
                    AllPokemon = this.dataService.GetAllPokemonOnlyForms(),
                };

                return this.View(model);
            }

            this.dataService.AddFormItem(formItem);

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

            this.dataService.AddType(type);

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

            this.dataService.AddEggCycle(eggCycle);

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

            this.dataService.AddExperienceGrowth(experienceGrowth);

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

            this.dataService.AddGenderRatio(genderRatio);

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

            this.dataService.AddLegendaryType(legendaryType);

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

            this.dataService.AddEggGroup(eggGroup);

            return this.RedirectToAction("EggGroups", "Admin");
        }

        [HttpGet]
        [Route("add_move/{typeId:int}")]
        public IActionResult Move(int typeId)
        {
            MoveViewModel model = new MoveViewModel()
            {
                MoveTypeId = typeId,
                AllTypes = this.dataService.GetTypes(),
                AllMoveCategories = this.dataService.GetMoveCategories(),
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
                    AllTypes = this.dataService.GetTypes(),
                    AllMoveCategories = this.dataService.GetMoveCategories(),
                };

                return this.View(model);
            }

            this.dataService.AddMove(move);

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

            if (classification.Name.Contains("pokemon"))
            {
                classification.Name = classification.Name.Replace("pokemon", "Pokemon");
            }
            else if (!classification.Name.Contains("Pokemon") && !classification.Name.Contains("pokemon"))
            {
                classification.Name = string.Concat(classification.Name.Trim(), " Pokemon");
            }

            this.dataService.AddClassification(classification);

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

            this.dataService.AddNature(nature);

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

            this.dataService.AddForm(form);

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

            this.dataService.AddEvolutionMethod(evolutionMethod);

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

            this.dataService.AddCaptureRate(captureRate);

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

            this.dataService.AddAbility(ability);

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

            this.dataService.AddBaseHappiness(baseHappiness);

            return this.RedirectToAction("BaseHappinesses", "Admin");
        }

        [HttpGet]
        [Route("add_game")]
        public IActionResult Game()
        {
            GameViewModel model = new GameViewModel()
            {
                AllGenerations = this.dataService.GetGenerations(),
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
                GameViewModel model = new GameViewModel()
                {
                    AllGenerations = this.dataService.GetGenerations(),
                };

                return this.View(model);
            }

            this.dataService.AddGame(game);

            return this.RedirectToAction("Games", "Admin");
        }

        [HttpGet]
        [Route("add_battle_item")]
        public IActionResult BattleItem()
        {
            List<Pokemon> pokemonList = this.dataService.GetAllPokemon();
            List<Pokemon> altFormsList = this.dataService.GetAllAltForms().Select(x => x.AltFormPokemon).ToList();
            foreach (var p in pokemonList.Where(x => altFormsList.Any(y => y.Id == x.Id)))
            {
                p.Name = this.dataService.GetAltFormWithFormName(p.Id).Name;
            }

            BattleItemViewModel model = new BattleItemViewModel()
            {
                AllGenerations = this.dataService.GetGenerations().Where(x => x.Id > 1).ToList(),
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

            this.dataService.AddBattleItem(battleItem);

            return this.RedirectToAction("BattleItems", "Admin");
        }

        [HttpGet]
        [Route("add_pokemon")]
        public IActionResult Pokemon()
        {
            BasePokemonViewModel model = new BasePokemonViewModel()
            {
                AllBaseHappinesses = this.dataService.GetBaseHappinesses(),
                AllClassifications = this.dataService.GetClassifications(),
                AllCaptureRates = this.dataService.GetCaptureRates(),
                AllEggCycles = this.dataService.GetEggCycles(),
                AllExperienceGrowths = this.dataService.GetExperienceGrowths(),
                AllGenderRatios = new List<GenderRatioViewModel>(),
                AllGames = this.dataService.GetGames(),
            };

            foreach (GenderRatio genderRatio in this.dataService.GetGenderRatios())
            {
                GenderRatioViewModel viewModel = new GenderRatioViewModel()
                {
                    Id = genderRatio.Id,
                };

                if (genderRatio.MaleRatio == genderRatio.FemaleRatio && genderRatio.MaleRatio == 0)
                {
                    viewModel.GenderRatioString = "Genderless";
                }
                else if (genderRatio.FemaleRatio == 0)
                {
                    viewModel.GenderRatioString = string.Concat(genderRatio.MaleRatio, "% Male");
                }
                else if (genderRatio.MaleRatio == 0)
                {
                    viewModel.GenderRatioString = string.Concat(genderRatio.FemaleRatio, "% Female");
                }
                else
                {
                    viewModel.GenderRatioString = string.Concat(genderRatio.MaleRatio, "% Male / ", genderRatio.FemaleRatio, "% Female");
                }

                model.AllGenderRatios.Add(viewModel);
            }

            model.GameId = this.dataService.GetGames().Last().Id;

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_pokemon")]
        public async Task<IActionResult> Pokemon(BasePokemonViewModel newPokemon, IFormFile fileUpload, string urlUpload)
        {
            if (!this.ModelState.IsValid)
            {
                BasePokemonViewModel model = new BasePokemonViewModel()
                {
                    AllBaseHappinesses = this.dataService.GetBaseHappinesses(),
                    AllClassifications = this.dataService.GetClassifications(),
                    AllCaptureRates = this.dataService.GetCaptureRates(),
                    AllEggCycles = this.dataService.GetEggCycles(),
                    AllExperienceGrowths = this.dataService.GetExperienceGrowths(),
                    AllGenderRatios = new List<GenderRatioViewModel>(),
                    AllGames = this.dataService.GetGames(),
                };

                foreach (GenderRatio genderRatio in this.dataService.GetGenderRatios())
                {
                    GenderRatioViewModel viewModel = new GenderRatioViewModel()
                    {
                        Id = genderRatio.Id,
                    };

                    if (genderRatio.MaleRatio == genderRatio.FemaleRatio && genderRatio.MaleRatio == 0)
                    {
                        viewModel.GenderRatioString = "Genderless";
                    }
                    else if (genderRatio.FemaleRatio == 0)
                    {
                        viewModel.GenderRatioString = string.Concat(genderRatio.MaleRatio, "% Male");
                    }
                    else if (genderRatio.MaleRatio == 0)
                    {
                        viewModel.GenderRatioString = string.Concat(genderRatio.FemaleRatio, "% Female");
                    }
                    else
                    {
                        viewModel.GenderRatioString = string.Concat(genderRatio.MaleRatio, "% Male / ", genderRatio.FemaleRatio, "% Female");
                    }

                    model.AllGenderRatios.Add(viewModel);
                }

                return this.View(model);
            }
            else if (this.dataService.GetPokemonByPokedexNumber(newPokemon.PokedexNumber) != null)
            {
                BasePokemonViewModel model = new BasePokemonViewModel()
                {
                    AllBaseHappinesses = this.dataService.GetBaseHappinesses(),
                    AllClassifications = this.dataService.GetClassifications(),
                    AllCaptureRates = this.dataService.GetCaptureRates(),
                    AllEggCycles = this.dataService.GetEggCycles(),
                    AllExperienceGrowths = this.dataService.GetExperienceGrowths(),
                    AllGenderRatios = new List<GenderRatioViewModel>(),
                    AllGames = this.dataService.GetGames(),
                };

                foreach (GenderRatio genderRatio in this.dataService.GetGenderRatios())
                {
                    GenderRatioViewModel viewModel = new GenderRatioViewModel()
                    {
                        Id = genderRatio.Id,
                    };

                    if (genderRatio.MaleRatio == genderRatio.FemaleRatio && genderRatio.MaleRatio == 0)
                    {
                        viewModel.GenderRatioString = "Genderless";
                    }
                    else if (genderRatio.FemaleRatio == 0)
                    {
                        viewModel.GenderRatioString = string.Concat(genderRatio.MaleRatio, "% Male");
                    }
                    else if (genderRatio.MaleRatio == 0)
                    {
                        viewModel.GenderRatioString = string.Concat(genderRatio.FemaleRatio, "% Female");
                    }
                    else
                    {
                        viewModel.GenderRatioString = string.Concat(genderRatio.MaleRatio, "% Male / ", genderRatio.FemaleRatio, "% Female");
                    }

                    model.AllGenderRatios.Add(viewModel);
                }

                this.ModelState.AddModelError("Pokedex Number", "Pokedex Number already exists.");
                return this.View(model);
            }

            this.dataService.AddPokemon(newPokemon);

            IFormFile upload;

            if (fileUpload == null)
            {
                WebRequest webRequest = WebRequest.CreateHttp(urlUpload);

                using (WebResponse webResponse = webRequest.GetResponse())
                {
                    Stream stream = webResponse.GetResponseStream();
                    MemoryStream memoryStream = new MemoryStream();
                    stream.CopyTo(memoryStream);

                    upload = new FormFile(memoryStream, 0, memoryStream.Length, "image", "image.png");
                }
            }
            else
            {
                upload = fileUpload;
            }

            if (upload != null)
            {
                upload = this.dataService.TrimImage(upload);

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(string.Concat(this.appConfig.FTPUrl, this.appConfig.PokemonImageFTPUrl, newPokemon.Id.ToString(), upload.FileName.Substring(upload.FileName.LastIndexOf('.'))));
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(this.appConfig.FTPUsername, this.appConfig.FTPPassword);

                using (var requestStream = request.GetRequestStream())
                {
                    await upload.CopyToAsync(requestStream);
                }

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
                }

                IFormFile spriteUpload = this.dataService.FormatFavIcon(upload);

                request = (FtpWebRequest)WebRequest.Create(string.Concat(this.appConfig.FTPUrl, this.appConfig.FaviconImageFTPUrl, newPokemon.Id.ToString(), ".png"));
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(this.appConfig.FTPUsername, this.appConfig.FTPPassword);

                using (var requestStream = request.GetRequestStream())
                {
                    await spriteUpload.CopyToAsync(requestStream);
                }

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    System.Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
                }
            }
            else
            {
                WebClient webRequest = new WebClient();
                webRequest.Credentials = new NetworkCredential(this.appConfig.FTPUsername, this.appConfig.FTPPassword);

                byte[] file = webRequest.DownloadData(string.Concat(this.appConfig.WebUrl, "/images/general/tempPhoto.png"));

                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(string.Concat(this.appConfig.FTPUrl, this.appConfig.PokemonImageFTPUrl, newPokemon.Id.ToString(), ".png"));
                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
                ftpRequest.Credentials = new NetworkCredential(this.appConfig.FTPUsername, this.appConfig.FTPPassword);

                using (var requestStream = ftpRequest.GetRequestStream())
                {
                    requestStream.Write(file, 0, file.Length);
                }

                using (FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse())
                {
                    Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
                }
            }

            this.dataService.AddPokemonGameDetail(new PokemonGameDetail()
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
            Pokemon originalPokemon = this.dataService.GetPokemonById(pokemonId);
            List<Game> games = this.dataService.GetGames().Where(x => x.ReleaseDate >= originalPokemon.Game.ReleaseDate).ToList();

            AlternateFormViewModel model = new AlternateFormViewModel()
            {
                AllForms = this.dataService.GetForms(),
                AllClassifications = this.dataService.GetClassifications(),
                AllGames = games,
                OriginalPokemon = originalPokemon,
                OriginalPokemonId = originalPokemon.Id,
            };

            model.GameId = this.dataService.GetGames().Last().Id;

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add_alternate_form/{pokemonId:int}")]
        public async Task<IActionResult> AltForm(AlternateFormViewModel pokemon, IFormFile fileUpload, string urlUpload)
        {
            List<PokemonFormDetail> originalPokemonForms = this.dataService.GetPokemonFormsWithIncomplete(pokemon.OriginalPokemonId);
            if (!this.ModelState.IsValid)
            {
                Pokemon originalPokemon = this.dataService.GetPokemonById(pokemon.OriginalPokemonId);
                List<Game> games = this.dataService.GetGames().Where(x => x.ReleaseDate >= originalPokemon.Game.ReleaseDate).ToList();

                AlternateFormViewModel model = new AlternateFormViewModel()
                {
                    AllForms = this.dataService.GetForms(),
                    AllClassifications = this.dataService.GetClassifications(),
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
                    Pokemon originalPokemon = this.dataService.GetPokemonById(pokemon.OriginalPokemonId);
                    List<Game> games = this.dataService.GetGames().Where(x => x.ReleaseDate >= originalPokemon.Game.ReleaseDate).ToList();

                    AlternateFormViewModel model = new AlternateFormViewModel()
                    {
                        AllForms = this.dataService.GetForms(),
                        AllClassifications = this.dataService.GetClassifications(),
                        AllGames = games,
                        OriginalPokemon = originalPokemon,
                        OriginalPokemonId = originalPokemon.Id,
                    };

                    this.ModelState.AddModelError("Alternate Form Name", "Original Pokemon already has an alternate form of this type.");
                    return this.View(model);
                }
            }

            Pokemon alternatePokemon = this.dataService.GetPokemonNoIncludesById(pokemon.OriginalPokemonId);

            alternatePokemon.Id = 0;
            alternatePokemon.PokedexNumber = this.dataService.GetPokemonByIdNoIncludes(pokemon.OriginalPokemonId).PokedexNumber;
            alternatePokemon.Height = pokemon.Height;
            alternatePokemon.Weight = pokemon.Weight;
            alternatePokemon.ExpYield = pokemon.ExpYield;
            alternatePokemon.GameId = pokemon.GameId;
            alternatePokemon.ClassificationId = pokemon.ClassificationId;
            alternatePokemon.IsComplete = false;

            this.dataService.AddPokemon(alternatePokemon);

            IFormFile upload;

            if (fileUpload == null)
            {
                WebRequest webRequest = WebRequest.CreateHttp(urlUpload);

                using (WebResponse webResponse = webRequest.GetResponse())
                {
                    Stream stream = webResponse.GetResponseStream();
                    MemoryStream memoryStream = new MemoryStream();
                    stream.CopyTo(memoryStream);

                    upload = new FormFile(memoryStream, 0, memoryStream.Length, "image", "image.png");
                }
            }
            else
            {
                upload = fileUpload;
            }

            if (upload != null)
            {
                upload = this.dataService.TrimImage(upload);

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(string.Concat(this.appConfig.FTPUrl, this.appConfig.PokemonImageFTPUrl, alternatePokemon.Id.ToString(), upload.FileName.Substring(upload.FileName.LastIndexOf('.'))));
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(this.appConfig.FTPUsername, this.appConfig.FTPPassword);

                using (var requestStream = request.GetRequestStream())
                {
                    await upload.CopyToAsync(requestStream);
                }

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
                }

                IFormFile spriteUpload = this.dataService.FormatFavIcon(upload);

                request = (FtpWebRequest)WebRequest.Create(string.Concat(this.appConfig.FTPUrl, this.appConfig.FaviconImageFTPUrl, alternatePokemon.Id.ToString(), ".png"));
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(this.appConfig.FTPUsername, this.appConfig.FTPPassword);

                using (var requestStream = request.GetRequestStream())
                {
                    await spriteUpload.CopyToAsync(requestStream);
                }

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
                }
            }
            else
            {
                WebClient webRequest = new WebClient
                {
                    Credentials = new NetworkCredential(this.appConfig.FTPUsername, this.appConfig.FTPPassword),
                };

                byte[] file = webRequest.DownloadData(string.Concat(this.appConfig.WebUrl, "/images/general/tempPhoto.png"));

                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(string.Concat(this.appConfig.FTPUrl, this.appConfig.PokemonImageFTPUrl, alternatePokemon.Id.ToString(), ".png"));
                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
                ftpRequest.Credentials = new NetworkCredential(this.appConfig.FTPUsername, this.appConfig.FTPPassword);

                using (var requestStream = ftpRequest.GetRequestStream())
                {
                    requestStream.Write(file, 0, file.Length);
                }

                using (FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse())
                {
                    Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
                }
            }

            PokemonEggGroupDetail eggGroups = this.dataService.GetPokemonWithEggGroups(pokemon.OriginalPokemonId);
            PokemonEggGroupDetail alternatePokemonEggGroups = new PokemonEggGroupDetail()
            {
                PrimaryEggGroupId = eggGroups.PrimaryEggGroupId,
                SecondaryEggGroupId = eggGroups.SecondaryEggGroupId,
                PokemonId = alternatePokemon.Id,
            };
            this.dataService.AddPokemonEggGroups(alternatePokemonEggGroups);

            PokemonFormDetail alternateForm = new PokemonFormDetail()
            {
                OriginalPokemonId = pokemon.OriginalPokemonId,
                AltFormPokemonId = alternatePokemon.Id,
                FormId = pokemon.FormId,
            };
            this.dataService.AddPokemonFormDetails(alternateForm);

            this.dataService.AddPokemonGameDetail(new PokemonGameDetail()
            {
                PokemonId = alternatePokemon.Id,
                GameId = alternatePokemon.GameId,
            });

            return this.RedirectToAction("Typing", "Add", new { pokemonId = alternatePokemon.Id });
        }

        [HttpGet]
        [Route("add_typing/{pokemonId:int}")]
        public IActionResult Typing(int pokemonId)
        {
            PokemonTypingViewModel model = new PokemonTypingViewModel()
            {
                AllTypes = this.dataService.GetTypes(),
                PokemonId = pokemonId,
                Pokemon = this.dataService.GetPokemonById(pokemonId),
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
                    AllTypes = this.dataService.GetTypes(),
                    PokemonId = typing.PokemonId,
                    Pokemon = this.dataService.GetPokemonById(typing.PokemonId),
                };

                return this.View(model);
            }

            this.dataService.AddPokemonTyping(typing);

            if (this.dataService.GetPokemonWithAbilities(typing.PokemonId) == null)
            {
                return this.RedirectToAction("Abilities", "Add", new { pokemonId = typing.PokemonId });
            }
            else
            {
                return this.RedirectToAction("Pokemon", "Admin");
            }
        }

        [HttpGet]
        [Route("add_abilities/{pokemonId:int}")]
        public IActionResult Abilities(int pokemonId)
        {
            PokemonAbilitiesViewModel model = new PokemonAbilitiesViewModel()
            {
                AllAbilities = this.dataService.GetAbilities(),
                PokemonId = pokemonId,
                Pokemon = this.dataService.GetPokemonById(pokemonId),
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
                    AllAbilities = this.dataService.GetAbilities(),
                    PokemonId = abilities.PokemonId,
                    Pokemon = this.dataService.GetPokemonById(abilities.PokemonId),
                };

                return this.View(model);
            }

            this.dataService.AddPokemonAbilities(abilities);

            if (this.dataService.GetPokemonWithEggGroups(abilities.PokemonId) == null)
            {
                return this.RedirectToAction("EggGroups", "Add", new { pokemonId = abilities.PokemonId });
            }
            else if (this.dataService.CheckIfAltForm(abilities.PokemonId) && this.dataService.GetPokemonBaseStats(abilities.PokemonId) == null)
            {
                return this.RedirectToAction("BaseStats", "Add", new { pokemonId = abilities.PokemonId });
            }
            else
            {
                return this.RedirectToAction("Pokemon", "Admin");
            }
        }

        [HttpGet]
        [Route("add_special_event_ability/{pokemonId:int}")]
        public IActionResult SpecialEventAbility(int pokemonId)
        {
            SpecialEventAbilityViewModel model = new SpecialEventAbilityViewModel()
            {
                AllAbilities = this.dataService.GetAbilities(),
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
                    AllAbilities = this.dataService.GetAbilities(),
                    PokemonId = ability.PokemonId,
                };

                return this.View(model);
            }

            PokemonAbilityDetail pokemonAbilities = this.dataService.GetPokemonWithAbilitiesNoIncludes(ability.PokemonId);

            pokemonAbilities.SpecialEventAbilityId = ability.AbilityId;

            this.dataService.UpdatePokemonAbilityDetail(pokemonAbilities);

            return this.RedirectToAction("Pokemon", "Admin");
        }

        [HttpGet]
        [Route("add_egg_groups/{pokemonId:int}")]
        public IActionResult EggGroups(int pokemonId)
        {
            PokemonEggGroupsViewModel model = new PokemonEggGroupsViewModel()
            {
                AllEggGroups = this.dataService.GetEggGroups(),
                PokemonId = pokemonId,
                Pokemon = this.dataService.GetPokemonById(pokemonId),
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
                    AllEggGroups = this.dataService.GetEggGroups(),
                    PokemonId = eggGroups.PokemonId,
                    Pokemon = this.dataService.GetPokemonById(eggGroups.PokemonId),
                };

                return this.View(model);
            }

            this.dataService.AddPokemonEggGroups(eggGroups);

            if (this.dataService.GetPokemonBaseStats(eggGroups.PokemonId) == null)
            {
                return this.RedirectToAction("BaseStats", "Add", new { pokemonId = eggGroups.PokemonId });
            }
            else
            {
                return this.RedirectToAction("Pokemon", "Admin");
            }
        }

        [HttpGet]
        [Route("add_base_stats/{pokemonId:int}")]
        public IActionResult BaseStats(int pokemonId)
        {
            BaseStat model = new BaseStat()
            {
                PokemonId = pokemonId,
                Pokemon = this.dataService.GetPokemonById(pokemonId),
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
                    Pokemon = this.dataService.GetPokemonById(baseStat.PokemonId),
                };

                return this.View(model);
            }

            this.dataService.AddPokemonBaseStat(baseStat);

            if (this.dataService.GetPokemonEVYields(baseStat.PokemonId) == null)
            {
                return this.RedirectToAction("EVYields", "Add", new { pokemonId = baseStat.PokemonId });
            }
            else
            {
                return this.RedirectToAction("Pokemon", "Admin");
            }
        }

        [HttpGet]
        [Route("add_ev_yields/{pokemonId:int}")]
        public IActionResult EVYields(int pokemonId)
        {
            EVYield model = new EVYield()
            {
                PokemonId = pokemonId,
                Pokemon = this.dataService.GetPokemonById(pokemonId),
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
                    Pokemon = this.dataService.GetPokemonById(evYield.PokemonId),
                };

                return this.View(model);
            }

            this.dataService.AddPokemonEVYield(evYield);

            if (this.User.IsInRole("Owner"))
            {
                return this.RedirectToAction("Evolution", "Add", new { pokemonId = evYield.PokemonId });
            }
            else
            {
                return this.RedirectToAction("Pokemon", "Admin");
            }
        }

        [HttpGet]
        [Route("set_legendary_type/{pokemonId:int}")]
        public IActionResult PokemonLegendaryDetails(int pokemonId)
        {
            PokemonLegendaryViewModel model = new PokemonLegendaryViewModel()
            {
                AllLegendaryTypes = this.dataService.GetLegendaryTypes(),
                PokemonId = pokemonId,
                Pokemon = this.dataService.GetPokemonById(pokemonId),
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
                    AllLegendaryTypes = this.dataService.GetLegendaryTypes(),
                    PokemonId = pokemonLegendaryDetails.PokemonId,
                    Pokemon = this.dataService.GetPokemonById(pokemonLegendaryDetails.PokemonId),
                };

                return this.View(model);
            }

            this.dataService.AddPokemonLegendaryDetails(pokemonLegendaryDetails);

            return this.RedirectToAction("Pokemon", "Admin");
        }
    }
}
