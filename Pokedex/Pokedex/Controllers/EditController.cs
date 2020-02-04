using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using ImageMagick;

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

        [Route("edit_type_effectiveness/{id:int}")]
        public IActionResult TypeEffectiveness(int id)
        {
            EditTypeChartViewModel model = new EditTypeChartViewModel()
            {
                TypeChart = this._dataService.GetTypeChartByDefendType(id),
                Types = this._dataService.GetTypes(),
                TypeId = id,
            };

            return this.View(model);
        }

        [Route("game_availability/{pokemonId:int}")]
        public IActionResult PokemonGameDetails(int pokemonId)
        {
            Pokemon pokemon = this._dataService.GetPokemonById(pokemonId);
            if(this._dataService.CheckIfAltForm(pokemonId))
            {
                pokemon.Name += " (" + this._dataService.GetFormByAltFormId(pokemonId).Name + ")";
            }

            PokemonGameViewModel model = new PokemonGameViewModel()
            {
                Pokemon = pokemon,
                PokemonGameDetails = this._dataService.GetPokemonGameDetails(pokemonId),
                AllGames = this._dataService.GetGames().Where(x => x.ReleaseDate >= pokemon.Game.ReleaseDate).ToList(),
            };

            return this.View(model);
        }

        [HttpGet]
        [Route("edit_generation/{id:int}")]
        public IActionResult Generation(int id)
        {
            Generation model = this._dataService.GetGeneration(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_generation/{id:int}")]
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
        [Route("edit_game/{id:int}")]
        public IActionResult Game(int id)
        {
            Game game = this._dataService.GetGame(id);
            GameViewModel model = new GameViewModel(){
                Id = game.Id,
                Name = game.Name,
                Abbreviation = game.Abbreviation,
                ReleaseDate = game.ReleaseDate,
                GenerationId = game.GenerationId,
                AllGenerations = this._dataService.GetGenerations(),
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_game/{id:int}")]
        public IActionResult Game(Game game)
        {
            if (!this.ModelState.IsValid)
            {
                Game oldGame = this._dataService.GetGame(game.Id);
                GameViewModel model = new GameViewModel(){
                    Id = oldGame.Id,
                    Name = oldGame.Name,
                    Abbreviation = oldGame.Abbreviation,
                    ReleaseDate = oldGame.ReleaseDate,
                    GenerationId = oldGame.GenerationId,
                    AllGenerations = this._dataService.GetGenerations(),
                };

                return this.View(model);
            }

            this._dataService.UpdateGame(game);

            return this.RedirectToAction("Games", "Admin");
        }

        [HttpGet]
        [Route("edit_battle_item/{id:int}")]
        public IActionResult BattleItem(int id)
        {
            List<Pokemon> pokemonList = this._dataService.GetAllPokemon();
            foreach(var p in pokemonList.Where(x => this._dataService.CheckIfAltForm(x.Id)))
            {
                p.Name += " (" + this._dataService.GetFormByAltFormId(p.Id).Name + ")";
            }

            BattleItem battleItem = this._dataService.GetBattleItem(id);
            BattleItemViewModel model = new BattleItemViewModel(){
                Id = battleItem.Id,
                Name = battleItem.Name,
                GenerationId = battleItem.GenerationId,
                AllGenerations = this._dataService.GetGenerations().Where(x => x.Id >= 1).ToList(),
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
                foreach(var p in pokemonList.Where(x => this._dataService.CheckIfAltForm(x.Id)))
                {
                    p.Name += " (" + this._dataService.GetFormByAltFormId(p.Id).Name + ")";
                }

                BattleItem item = this._dataService.GetBattleItem(battleItem.Id);
                BattleItemViewModel model = new BattleItemViewModel(){
                    Id = item.Id,
                    Name = item.Name,
                    GenerationId = item.GenerationId,
                    AllGenerations = this._dataService.GetGenerations().Where(x => x.Id >= 1).ToList(),
                    AllPokemon = pokemonList,
                };

                return this.View(model);
            }

            this._dataService.UpdateBattleItem(battleItem);

            return this.RedirectToAction("BattleItems", "Admin");
        }

        [HttpGet]
        [Route("edit_pokemon/{id:int}")]
        public IActionResult Pokemon(int id)
        {
            BasePokemonViewModel model = new BasePokemonViewModel(){
                Pokemon = this._dataService.GetPokemonById(id),
                AllGames = this._dataService.GetGames(),
                AllClassifications = this._dataService.GetClassifications(),
            };

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
            
            if(this._dataService.CheckIfAltForm(id))
            {
                model.Name = model.Pokemon.Name + " (" + this._dataService.GetPokemonFormName(id) + ")";
            }

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_pokemon/{id:int}")]
        public IActionResult Pokemon(Pokemon pokemon, int id)
        {
            if (!this.ModelState.IsValid)
            {
                BasePokemonViewModel model = new BasePokemonViewModel(){
                    Pokemon = this._dataService.GetPokemonById(pokemon.Id),
                    AllGames = this._dataService.GetGames(),
                };
    
                if(!this._dataService.CheckIfAltForm(pokemon.Id))
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
            else if (this._dataService.GetPokemonByPokedexNumber(pokemon.PokedexNumber) != null && this._dataService.GetPokemonById(pokemon.Id).PokedexNumber != pokemon.PokedexNumber)
            {
                BasePokemonViewModel model = new BasePokemonViewModel(){
                    Pokemon = this._dataService.GetPokemonById(pokemon.Id),
                    AllGames = this._dataService.GetGames(),
                };
    
                if(!this._dataService.CheckIfAltForm(pokemon.Id))
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

            Pokemon oldPokemon = this._dataService.GetPokemonById(pokemon.Id);
            List<Pokemon> altForms = this._dataService.GetAltForms(pokemon.Id);
            if(oldPokemon.PokedexNumber != pokemon.PokedexNumber)
            {
                foreach(var p in altForms)
                {
                    if(p.PokedexNumber != pokemon.PokedexNumber)
                    {
                        p.PokedexNumber = pokemon.PokedexNumber;
                        this._dataService.UpdatePokemon(p);
                    }
                }
            }

            this._dataService.UpdatePokemon(pokemon);

            return this.RedirectToAction("Pokemon", "Admin");
        }

        [HttpGet]
        [Route("edit_pokemon_image/{id:int}")]
        public IActionResult PokemonImage(int id)
        {
            Pokemon model = this._dataService.GetPokemonById(id);

            if(this._dataService.CheckIfAltForm(id))
            {
                model.Name = model.Name + " (" + this._dataService.GetPokemonFormName(id) + ")";
            }

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_pokemon_image/{id:int}")]
        public async Task<IActionResult> PokemonImage(Pokemon pokemon, int id, IFormFile fileUpload, string urlUpload)
        {
            if (!this.ModelState.IsValid)
            {
                Pokemon model = this._dataService.GetPokemonById(id);

                if(this._dataService.CheckIfAltForm(id))
                {
                    model.Name = model.Name + " (" + this._dataService.GetPokemonFormName(id) + ")";
                }
                
                return this.View(model);
            }
            else if (fileUpload == null && string.IsNullOrEmpty(urlUpload))
            {
                Pokemon model = this._dataService.GetPokemonById(id);

                if(this._dataService.CheckIfAltForm(id))
                {
                    model.Name = model.Name + " (" + this._dataService.GetPokemonFormName(id) + ")";
                }
                
                this.ModelState.AddModelError("Picture", "An image is needed to update.");
                return this.View(model);
            }
            else if ((fileUpload != null && !fileUpload.FileName.Contains(".png")) || (!string.IsNullOrEmpty(urlUpload) && !urlUpload.Contains(".png")))
            {
                Pokemon model = this._dataService.GetPokemonById(id);

                if(this._dataService.CheckIfAltForm(id))
                {
                    model.Name = model.Name + " (" + this._dataService.GetPokemonFormName(id) + ")";
                }
                
                this.ModelState.AddModelError("Picture", "The image must be in the .png format.");
                return this.View(model);
            }

            IFormFile upload;

            if(fileUpload == null)
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

            IFormFile trimmedUpload;

            using (var ms = new MemoryStream())
            {
                upload.CopyTo(ms);
                byte[] uploadBytes = ms.ToArray();
                using(MagickImage image = new MagickImage(uploadBytes))
                {
                    image.Trim();
                    MemoryStream strm = new MemoryStream();
                    image.RePage();
                    image.Write(strm, MagickFormat.Png);
                    trimmedUpload = new FormFile(strm, 0, strm.Length, upload.Name, upload.FileName);
                }
            }

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(_appConfig.PokemonImageFTPUrl + id.ToString() + ".png");
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(_appConfig.FTPUsername, _appConfig.FTPPassword);

            using (var requestStream = request.GetRequestStream())  
            {  
                await trimmedUpload.CopyToAsync(requestStream);  
            }

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                System.Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
            }

            return this.RedirectToAction("Pokemon", "Admin");
        }

        [HttpGet]
        [Route("edit_pokemon_sprite/{id:int}")]
        public IActionResult SpriteImage(int id)
        {
            Pokemon model = this._dataService.GetPokemonById(id);

            if(this._dataService.CheckIfAltForm(id))
            {
                model.Name = model.Name + " (" + this._dataService.GetPokemonFormName(id) + ")";
            }

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_pokemon_sprite/{id:int}")]
        public async Task<IActionResult> SpriteImage(Pokemon pokemon, int id, IFormFile fileUpload, string urlUpload)
        {
            if (!this.ModelState.IsValid)
            {
                Pokemon model = this._dataService.GetPokemonById(id);

                if(this._dataService.CheckIfAltForm(id))
                {
                    model.Name = model.Name + " (" + this._dataService.GetPokemonFormName(id) + ")";
                }
                
                return this.View(model);
            }
            else if (fileUpload == null && string.IsNullOrEmpty(urlUpload))
            {
                Pokemon model = this._dataService.GetPokemonById(id);

                if(this._dataService.CheckIfAltForm(id))
                {
                    model.Name = model.Name + " (" + this._dataService.GetPokemonFormName(id) + ")";
                }
                
                this.ModelState.AddModelError("Picture", "An image is needed to update.");
                return this.View(model);
            }
            else if ((fileUpload != null && !fileUpload.FileName.Contains(".png")) || (!string.IsNullOrEmpty(urlUpload) && !urlUpload.Contains(".png")))
            {
                Pokemon model = this._dataService.GetPokemonById(id);

                if(this._dataService.CheckIfAltForm(id))
                {
                    model.Name = model.Name + " (" + this._dataService.GetPokemonFormName(id) + ")";
                }
                
                this.ModelState.AddModelError("Picture", "The image must be in the .png format.");
                return this.View(model);
            }

            IFormFile upload;

            if(fileUpload == null)
            {
                WebRequest webRequest = WebRequest.CreateHttp(urlUpload);

                using (WebResponse webResponse = webRequest.GetResponse())
                {
                    Stream stream = webResponse.GetResponseStream();
                    MemoryStream memoryStream = new MemoryStream();
                    stream.CopyTo(memoryStream);

                    upload = new FormFile(memoryStream, 0, memoryStream.Length, "sprite", "sprite.png");
                }
            }
            else
            {
                upload = fileUpload;
            }

            IFormFile trimmedUpload, squaredImage;

            using (var ms = new MemoryStream())
            {
                upload.CopyTo(ms);
                byte[] uploadBytes = ms.ToArray();
                using(MagickImage image = new MagickImage(uploadBytes))
                {
                    image.Trim();
                    MemoryStream strm = new MemoryStream();
                    image.RePage();
                    image.Write(strm, MagickFormat.Png);
                    trimmedUpload = new FormFile(strm, 0, strm.Length, upload.Name, upload.FileName);
                }
            }

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(_appConfig.SpriteImageFTPUrl + id.ToString() + ".png");
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(_appConfig.FTPUsername, _appConfig.FTPPassword);

            using (var requestStream = request.GetRequestStream())  
            {  
                await trimmedUpload.CopyToAsync(requestStream);  
            }

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                System.Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
            }
            
            using (var ms = new MemoryStream())
            {
                trimmedUpload.CopyTo(ms);
                byte[] uploadBytes = ms.ToArray();
                using(MagickImage image = new MagickImage(uploadBytes))
                {
                    image.VirtualPixelMethod = VirtualPixelMethod.Transparent;
                    image.SetArtifact("distort:viewport", string.Concat(System.Math.Max(image.Width, image.Height).ToString(), 'x', System.Math.Max(image.Width, image.Height).ToString(), '-', System.Math.Max((image.Height - image.Width)/2,0).ToString(), '-', System.Math.Max((image.Width - image.Height)/2,0).ToString()));
                    image.FilterType = FilterType.Point;
                    image.Distort(DistortMethod.ScaleRotateTranslate, 0);
                    MemoryStream strm = new MemoryStream();
                    image.RePage();
                    image.Write(strm, MagickFormat.Png);
                    squaredImage = new FormFile(strm, 0, strm.Length, upload.Name, upload.FileName);
                }
            }

            request = (FtpWebRequest)WebRequest.Create(_appConfig.FaviconImageFtpUrl + id.ToString() + ".png");
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(_appConfig.FTPUsername, _appConfig.FTPPassword);

            using (var requestStream = request.GetRequestStream())  
            {  
                await squaredImage.CopyToAsync(requestStream);  
            }

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                System.Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
            }

            return this.RedirectToAction("Pokemon", "Admin");
        }

        [HttpGet]
        [Route("edit_typing/{pokemonId:int}")]
        public IActionResult Typing(int pokemonId)
        {
            PokemonTypeDetail typeDetail = this._dataService.GetPokemonWithTypes(pokemonId);
            PokemonTypingViewModel model = new PokemonTypingViewModel()
            {
                Id = typeDetail.Id,
                AllTypes = this._dataService.GetTypes(),
                PokemonId = typeDetail.PokemonId,
                Pokemon = typeDetail.Pokemon,
                PrimaryTypeId = typeDetail.PrimaryTypeId,
                SecondaryTypeId = typeDetail.SecondaryTypeId
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_typing/{id:int}")]
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
                    Pokemon = typeDetail.Pokemon,
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

            return this.RedirectToAction("Pokemon", "Admin");
        }

        [HttpGet]
        [Route("edit_abilities/{pokemonId:int}")]
        public IActionResult Abilities(int pokemonId)
        {
            PokemonAbilityDetail abilityDetail = this._dataService.GetPokemonWithAbilities(pokemonId);
            PokemonAbilitiesViewModel model = new PokemonAbilitiesViewModel()
            {
                Id = abilityDetail.Id,
                AllAbilities = this._dataService.GetAbilities(),
                PokemonId = abilityDetail.PokemonId,
                Pokemon = abilityDetail.Pokemon,
                PrimaryAbilityId = abilityDetail.PrimaryAbilityId,
                SecondaryAbilityId = abilityDetail.SecondaryAbilityId,
                HiddenAbilityId = abilityDetail.HiddenAbilityId
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_abilities/{pokemonId:int}")]
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
                    Pokemon = abilityDetail.Pokemon,
                    PrimaryAbilityId = abilityDetail.PrimaryAbilityId,
                    SecondaryAbilityId = abilityDetail.SecondaryAbilityId,
                    HiddenAbilityId = abilityDetail.HiddenAbilityId
                };

                return this.View(model);
            }

            this._dataService.UpdatePokemonAbilityDetail(pokemonAbilityDetail);

            return this.RedirectToAction("Pokemon", "Admin");
        }

        [HttpGet]
        [Route("edit_egg_groups/{pokemonId:int}")]
        public IActionResult EggGroups(int pokemonId)
        {
            PokemonEggGroupDetail eggGroupDetail = this._dataService.GetPokemonWithEggGroups(pokemonId);
            PokemonEggGroupsViewModel model = new PokemonEggGroupsViewModel()
            {
                Id = eggGroupDetail.Id,
                AllEggGroups = this._dataService.GetEggGroups(),
                PokemonId = eggGroupDetail.PokemonId,
                Pokemon = eggGroupDetail.Pokemon,
                PrimaryEggGroupId = eggGroupDetail.PrimaryEggGroupId,
                SecondaryEggGroupId = eggGroupDetail.SecondaryEggGroupId
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_egg_groups/{pokemonId:int}")]
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
                    Pokemon = eggGroupDetail.Pokemon,
                    PrimaryEggGroupId = eggGroupDetail.PrimaryEggGroupId,
                    SecondaryEggGroupId = eggGroupDetail.SecondaryEggGroupId
                };

                return this.View(model);
            }

            this._dataService.UpdatePokemonEggGroupDetail(pokemonEggGroupDetail);

            return this.RedirectToAction("Pokemon", "Admin");
        }

        [HttpGet]
        [Route("edit_base_stats/{pokemonId:int}")]
        public IActionResult BaseStats(int pokemonId)
        {
            BaseStat model = this._dataService.GetPokemonBaseStats(pokemonId);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_base_stats/{pokemonId:int}")]
        public IActionResult BaseStats(BaseStat baseStat)
        {
            if (!this.ModelState.IsValid)
            {
                BaseStat model = this._dataService.GetPokemonBaseStats(baseStat.PokemonId);

                return this.View(model);
            }

            this._dataService.UpdateBaseStat(baseStat);

            return this.RedirectToAction("Pokemon", "Admin");
        }

        [HttpGet]
        [Route("edit_ev_yields/{pokemonId:int}")]
        public IActionResult EVYields(int pokemonId)
        {
            EVYield model = this._dataService.GetPokemonEVYields(pokemonId);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_ev_yields/{pokemonId:int}")]
        public IActionResult EVYields(EVYield evYield)
        {
            if (!this.ModelState.IsValid)
            {
                EVYield model = this._dataService.GetPokemonEVYields(evYield.PokemonId);

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
        [Route("edit_egg_cycle/{id:int}")]
        public IActionResult EggCycle(int id)
        {
            EggCycle model = this._dataService.GetEggCycle(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_egg_cycle/{id:int}")]
        public IActionResult EggCycle(EggCycle eggCycle)
        {
            if (!this.ModelState.IsValid)
            {
                EggCycle model = this._dataService.GetEggCycle(eggCycle.Id);

                return this.View(model);
            }

            this._dataService.UpdateEggCycle(eggCycle);

            return this.RedirectToAction("EggCycles", "Admin");
        }

        [HttpGet]
        [Route("edit_experience_growth/{id:int}")]
        public IActionResult ExperienceGrowth(int id)
        {
            ExperienceGrowth model = this._dataService.GetExperienceGrowth(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_experience_growth/{id:int}")]
        public IActionResult ExperienceGrowth(ExperienceGrowth experienceGrowth)
        {
            if (!this.ModelState.IsValid)
            {
                ExperienceGrowth model = this._dataService.GetExperienceGrowth(experienceGrowth.Id);

                return this.View(model);
            }

            this._dataService.UpdateExperienceGrowth(experienceGrowth);

            return this.RedirectToAction("ExperienceGrowths", "Admin");
        }

        [HttpGet]
        [Route("edit_gender_ratio/{id:int}")]
        public IActionResult GenderRatio(int id)
        {
            GenderRatio model = this._dataService.GetGenderRatio(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_gender_ratio/{id:int}")]
        public IActionResult GenderRatio(GenderRatio genderRatio)
        {
            if (!this.ModelState.IsValid)
            {
                GenderRatio model = this._dataService.GetGenderRatio(genderRatio.Id);

                return this.View(model);
            }

            this._dataService.UpdateGenderRatio(genderRatio);

            return this.RedirectToAction("GenderRatios", "Admin");
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
            FormItem item = this._dataService.GetFormItem(id);
            FormItemViewModel model = new FormItemViewModel()
            {
                PokemonId = item.PokemonId,
                Name = item.Name,
            };
            
            List<Pokemon> altForms = this._dataService.GetPokemonFormDetailsByFormName("Mega").Select(x => x.AltFormPokemon).ToList();
            altForms.AddRange(this._dataService.GetPokemonFormDetailsByFormName("Mega X").Select(x => x.AltFormPokemon).ToList());
            altForms.AddRange(this._dataService.GetPokemonFormDetailsByFormName("Mega Y").Select(x => x.AltFormPokemon).ToList());
            altForms.AddRange(this._dataService.GetPokemonFormDetailsByFormName("Primal").Select(x => x.AltFormPokemon).ToList());
            altForms.AddRange(this._dataService.GetPokemonFormDetailsByFormName("Origin").Select(x => x.AltFormPokemon).ToList());
            altForms.AddRange(this._dataService.GetPokemonFormDetailsByFormName("Ultra").Select(x => x.AltFormPokemon).ToList());
            altForms.AddRange(this._dataService.GetPokemonFormDetailsByFormName("Crowned").Select(x => x.AltFormPokemon).ToList());

            foreach(var p in altForms)
            {
                p.Name += " (" + this._dataService.GetPokemonFormName(p.Id) + ")";
            }

            model.AllPokemon = altForms;

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_form_item/{id:int}")]
        public IActionResult FormItem(FormItem formItem)
        {
            if (!this.ModelState.IsValid)
            {
                FormItem item = this._dataService.GetFormItem(formItem.Id);
                FormItemViewModel model = new FormItemViewModel()
                {
                    PokemonId = item.PokemonId,
                    Name = item.Name,
                };
                
                List<Pokemon> altForms = this._dataService.GetPokemonFormDetailsByFormName("Mega").Select(x => x.AltFormPokemon).ToList();
                altForms.AddRange(this._dataService.GetPokemonFormDetailsByFormName("Mega X").Select(x => x.AltFormPokemon).ToList());
                altForms.AddRange(this._dataService.GetPokemonFormDetailsByFormName("Mega Y").Select(x => x.AltFormPokemon).ToList());
                altForms.AddRange(this._dataService.GetPokemonFormDetailsByFormName("Primal").Select(x => x.AltFormPokemon).ToList());
                altForms.AddRange(this._dataService.GetPokemonFormDetailsByFormName("Origin").Select(x => x.AltFormPokemon).ToList());
                altForms.AddRange(this._dataService.GetPokemonFormDetailsByFormName("Ultra").Select(x => x.AltFormPokemon).ToList());
                altForms.AddRange(this._dataService.GetPokemonFormDetailsByFormName("Crowned").Select(x => x.AltFormPokemon).ToList());

                foreach(var p in altForms)
                {
                    p.Name += " (" + this._dataService.GetPokemonFormName(p.Id) + ")";
                }

                model.AllPokemon = altForms;
    
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
        [Route("edit_evolution_method/{id:int}")]
        public IActionResult EvolutionMethod(int id)
        {
            EvolutionMethod model = this._dataService.GetEvolutionMethod(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_evolution_method/{id:int}")]
        public IActionResult EvolutionMethod(EvolutionMethod evolutionMethod)
        {
            if (!this.ModelState.IsValid)
            {
                EvolutionMethod model = this._dataService.GetEvolutionMethod(evolutionMethod.Id);

                return this.View(model);
            }

            this._dataService.UpdateEvolutionMethod(evolutionMethod);

            return this.RedirectToAction("EvolutionMethods", "Admin");
        }

        [HttpGet]
        [Route("edit_capture_rate/{id:int}")]
        public IActionResult CaptureRate(int id)
        {
            CaptureRate model = this._dataService.GetCaptureRate(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_capture_rate/{id:int}")]
        public IActionResult CaptureRate(CaptureRate captureRate)
        {
            if (!this.ModelState.IsValid)
            {
                CaptureRate model = this._dataService.GetCaptureRate(captureRate.Id);

                return this.View(model);
            }

            this._dataService.UpdateCaptureRate(captureRate);

            return this.RedirectToAction("CaptureRates", "Admin");
        }

        [HttpGet]
        [Route("edit_base_happiness/{id:int}")]
        public IActionResult BaseHappiness(int id)
        {
            BaseHappiness model = this._dataService.GetBaseHappiness(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_base_happiness/{id:int}")]
        public IActionResult BaseHappiness(BaseHappiness baseHappiness)
        {
            if (!this.ModelState.IsValid)
            {
                BaseHappiness model = this._dataService.GetBaseHappiness(baseHappiness.Id);

                return this.View(model);
            }

            this._dataService.UpdateBaseHappiness(baseHappiness);

            return this.RedirectToAction("BaseHappinesses", "Admin");
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
        [Route("edit_move/{id:int}")]
        public IActionResult Move(int id)
        {
            Move move = this._dataService.GetMove(id);
            MoveViewModel model = new MoveViewModel()
            {
                Id = move.Id,
                Name = move.Name,
                Description = move.Description,
                BasePower = move.BasePower,
                PP = move.PP,
                Accuracy = move.Accuracy,
                MoveTypeId = move.MoveTypeId,
                MoveCategoryId = move.MoveCategoryId,
                AllTypes = this._dataService.GetTypes(),
                AllMoveCategories = this._dataService.GetMoveCategories(),
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_move/{id:int}")]
        public IActionResult Move(Move move)
        {
            if (!this.ModelState.IsValid)
            {
                Move moveReset = this._dataService.GetMove(move.Id);
                MoveViewModel model = new MoveViewModel()
                {
                    Id = moveReset.Id,
                    Name = moveReset.Name,
                    Description = moveReset.Description,
                    BasePower = moveReset.BasePower,
                    PP = moveReset.PP,
                    Accuracy = moveReset.Accuracy,
                    MoveTypeId = move.MoveTypeId,
                    MoveCategoryId = move.MoveCategoryId,
                    AllTypes = this._dataService.GetTypes(),
                    AllMoveCategories = this._dataService.GetMoveCategories(),
                };

                return this.View(model);
            }

            this._dataService.UpdateMove(move);

            return this.RedirectToAction("Moves", "Admin");
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

            if(classification.Name.Contains("pokemon"))
            {
                classification.Name = classification.Name.Replace("pokemon", "Pokemon");
            }
            else if(!classification.Name.Contains("Pokemon") && !classification.Name.Contains("pokemon"))
            {
                classification.Name = classification.Name.Trim() + " Pokemon";
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
        [Route("edit_evolution/{pokemonId:int}")]
        public IActionResult Evolution(int pokemonId)
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
            foreach (var pokemon in pokemonList.Where(x => this._dataService.CheckIfAltForm(x.Id)))
            {
                pokemon.Name += " (" + this._dataService.GetPokemonFormName(pokemon.Id) + ")";
            }

            model.AllPokemon = pokemonList;

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_evolution/{pokemonId:int}")]
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
                foreach (var pokemon in pokemonList.Where(x => this._dataService.CheckIfAltForm(x.Id)))
                {
                    pokemon.Name += " (" + this._dataService.GetPokemonFormName(pokemon.Id) + ")";
                }

                model.AllPokemon = pokemonList;

                return this.View(model);
            }

            this._dataService.UpdateEvolution(evolution);

            return this.RedirectToAction("Pokemon", "Admin");
        }

        [Route("edit_alternate_forms/{pokemonId:int}")]
        public IActionResult AltForms(int pokemonId)
        {
            Pokemon originalPokemon = this._dataService.GetPokemonById(pokemonId);
            List<Pokemon> altFormList = this._dataService.GetAllPokemonIncludeIncomplete().Where(x => x.Name == originalPokemon.Name && this._dataService.CheckIfAltForm(x.Id)).ToList();
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
        [Route("edit_alternate_form/{pokemonId:int}")]
        public IActionResult AltFormsForm(int pokemonId)
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
        [Route("edit_alternate_form/{pokemonId:int}")]
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
    }
}