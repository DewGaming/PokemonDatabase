using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Pokedex.DataAccess.Models;
using Pokedex.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Pokedex.Controllers
{
    /// <summary>
    /// The class that handles the editing of the database.
    /// </summary>
    [Authorize(Roles = "Admin,Owner")]
    [Route("admin")]
    public class EditController : Controller
    {
        private readonly DataService dataService;

        private readonly AppConfig appConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditController"/> class.
        /// </summary>
        /// <param name="appConfig">The configuration for the application.</param>
        /// <param name="dataContext">The data context.</param>
        public EditController(IOptions<AppConfig> appConfig, DataContext dataContext)
        {
            this.appConfig = appConfig.Value;
            this.dataService = new DataService(dataContext);
        }

        [Authorize(Roles = "Owner")]
        [HttpGet]
        [Route("edit_user/{id:int}")]
        public new IActionResult User(int id)
        {
            User model = this.dataService.GetObjectByPropertyValue<User>("Id", id);

            return this.View(model);
        }

        [Authorize(Roles = "Owner")]
        [HttpPost]
        [Route("edit_user/{id:int}")]
        public new IActionResult User(User user)
        {
            if (!this.ModelState.IsValid)
            {
                User model = this.dataService.GetObjectByPropertyValue<User>("Username", user.Username);

                return this.View(model);
            }

            if (user.IsOwner && !user.IsAdmin)
            {
                user.IsAdmin = true;
            }

            this.dataService.UpdateObject(user);

            return this.RedirectToAction("Users", "Owner");
        }

        [Route("edit_type_effectiveness/{id:int}/{genId:int}")]
        public IActionResult TypeEffectiveness(int id, int genId)
        {
            EditTypeChartViewModel model = new EditTypeChartViewModel()
            {
                TypeChart = this.dataService.GetTypeChartByDefendType(id, genId),
                Types = this.dataService.GetObjects<Type>("Name").Where(x => x.GenerationId <= genId).ToList(),
                TypeId = id,
                GenerationId = genId,
            };

            return this.View(model);
        }

        [Route("game_availability/{pokemonId:int}")]
        public IActionResult PokemonGameDetails(int pokemonId)
        {
            Pokemon pokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", pokemonId, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth");
            if (this.dataService.CheckIfAltForm(pokemonId))
            {
                pokemon.Name = this.dataService.GetAltFormWithFormName(pokemon.Id).Name;
            }

            PokemonGameViewModel model = new PokemonGameViewModel()
            {
                Pokemon = pokemon,
                PokemonGameDetails = this.dataService.GetObjects<PokemonGameDetail>("Game.GenerationId, GameId, Id", "Pokemon, Pokemon.Game, Game", "PokemonId", pokemonId),
                AllGames = this.dataService.GetObjects<Game>("ReleaseDate, Id").Where(x => x.ReleaseDate >= pokemon.Game.ReleaseDate).ToList(),
            };

            return this.View(model);
        }

        [HttpGet]
        [Route("edit_game/{id:int}")]
        public IActionResult Game(int id)
        {
            Game game = this.dataService.GetObjectByPropertyValue<Game>("Id", id);
            GameViewModel model = new GameViewModel()
            {
                Id = game.Id,
                Name = game.Name,
                ReleaseDate = game.ReleaseDate,
                GenerationId = game.GenerationId,
                AllGenerations = this.dataService.GetObjects<Generation>(),
                AllRegions = this.dataService.GetObjects<Region>("GenerationId, Id"),
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_game/{id:int}")]
        public IActionResult Game(Game game, List<int> regionIds)
        {
            if (!this.ModelState.IsValid)
            {
                Game oldGame = this.dataService.GetObjectByPropertyValue<Game>("Id", game.Id);
                GameViewModel model = new GameViewModel()
                {
                    Id = oldGame.Id,
                    Name = oldGame.Name,
                    ReleaseDate = oldGame.ReleaseDate,
                    GenerationId = oldGame.GenerationId,
                    AllGenerations = this.dataService.GetObjects<Generation>(),
                    AllRegions = this.dataService.GetObjects<Region>("GenerationId, Id"),
                };

                return this.View(model);
            }

            GameRegionDetail gameRegionDetail;
            List<GameRegionDetail> existingEntries = this.dataService.GetObjects<GameRegionDetail>().Where(x => x.GameId == game.Id).ToList();
            foreach (var r in regionIds)
            {
                gameRegionDetail = new GameRegionDetail()
                {
                    RegionId = r,
                    GameId = game.Id,
                };
                this.dataService.AddObject(gameRegionDetail);
            }

            foreach (var e in existingEntries)
            {
                this.dataService.DeleteObject<GameRegionDetail>(e.Id);
            }

            this.dataService.UpdateObject(game);

            return this.RedirectToAction("Games", "Admin");
        }

        [Route("edit_game_availability/{id:int}")]
        public IActionResult GameAvailability(int id)
        {
            Game game = this.dataService.GetObjectByPropertyValue<Game>("Id", id);
            List<Pokemon> pokemonList = this.GetAllPokemonWithFormNames().Where(x => x.Game.ReleaseDate <= game.ReleaseDate).ToList();
            EditGameAvailabilityViewModel model = new EditGameAvailabilityViewModel()
            {
                Game = game,
                Games = this.dataService.GetObjects<Game>("ReleaseDate, Id"),
                PokemonList = pokemonList,
                GameAvailability = this.dataService.GetObjects<PokemonGameDetail>(includes: "Pokemon, Game"),
            };

            return this.View(model);
        }

        [HttpGet]
        [Route("edit_battle_item/{id:int}")]
        public IActionResult BattleItem(int id)
        {
            List<Pokemon> pokemonList = this.dataService.GetAllPokemon();
            List<Pokemon> altFormsList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form").ConvertAll(x => x.AltFormPokemon);
            foreach (var p in pokemonList.Where(x => altFormsList.Any(y => y.Id == x.Id)))
            {
                p.Name = this.dataService.GetAltFormWithFormName(p.Id).Name;
            }

            BattleItem battleItem = this.dataService.GetObjectByPropertyValue<BattleItem>("Id", id);
            BattleItemViewModel model = new BattleItemViewModel()
            {
                Id = battleItem.Id,
                Name = battleItem.Name,
                GenerationId = battleItem.GenerationId,
                AllGenerations = this.dataService.GetObjects<Generation>().Where(x => x.Id >= 1).ToList(),
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
                List<Pokemon> pokemonList = this.dataService.GetAllPokemon();
                List<Pokemon> altFormsList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form").ConvertAll(x => x.AltFormPokemon);
                foreach (var p in pokemonList.Where(x => altFormsList.Any(y => y.Id == x.Id)))
                {
                    p.Name = this.dataService.GetAltFormWithFormName(p.Id).Name;
                }

                BattleItem item = this.dataService.GetObjectByPropertyValue<BattleItem>("Id", battleItem.Id);
                BattleItemViewModel model = new BattleItemViewModel()
                {
                    Id = item.Id,
                    Name = item.Name,
                    GenerationId = item.GenerationId,
                    AllGenerations = this.dataService.GetObjects<Generation>().Where(x => x.Id >= 1).ToList(),
                    AllPokemon = pokemonList,
                };

                return this.View(model);
            }

            this.dataService.UpdateObject(battleItem);

            return this.RedirectToAction("BattleItems", "Admin");
        }

        [HttpGet]
        [Route("edit_pokemon/{id:int}")]
        public IActionResult Pokemon(int id)
        {
            BasePokemonViewModel model = new BasePokemonViewModel
            {
                Pokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", id, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth"),
                AllGames = this.dataService.GetObjects<Game>("ReleaseDate, Id"),
                AllClassifications = this.dataService.GetObjects<Classification>("Name"),
                AllBaseHappinesses = this.dataService.GetObjects<BaseHappiness>("Happiness"),
                AllCaptureRates = this.dataService.GetObjects<CaptureRate>("CatchRate"),
                AllEggCycles = this.dataService.GetObjects<EggCycle>("CycleCount"),
                AllExperienceGrowths = this.dataService.GetObjects<ExperienceGrowth>("Name"),
                AllGenderRatios = new List<GenderRatioViewModel>(),
            };

            foreach (GenderRatio genderRatio in this.dataService.GetObjects<GenderRatio>())
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

            if (this.dataService.CheckIfAltForm(id))
            {
                model.Name = string.Concat(model.Pokemon.Name, " (", this.dataService.GetPokemonFormName(id), ")");
            }

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_pokemon/{id:int}")]
        public IActionResult Pokemon(Pokemon pokemon)
        {
            if (!this.ModelState.IsValid)
            {
                BasePokemonViewModel model = new BasePokemonViewModel()
                {
                    Pokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", pokemon.Id, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth"),
                    AllGames = this.dataService.GetObjects<Game>("ReleaseDate, Id"),
                };

                if (!this.dataService.CheckIfAltForm(pokemon.Id))
                {
                    model.AllBaseHappinesses = this.dataService.GetObjects<BaseHappiness>("Happiness");
                    model.AllClassifications = this.dataService.GetObjects<Classification>("Name");
                    model.AllCaptureRates = this.dataService.GetObjects<CaptureRate>("CatchRate");
                    model.AllEggCycles = this.dataService.GetObjects<EggCycle>("CycleCount");
                    model.AllExperienceGrowths = this.dataService.GetObjects<ExperienceGrowth>("Name");
                    model.AllGenderRatios = new List<GenderRatioViewModel>();

                    foreach (GenderRatio genderRatio in this.dataService.GetObjects<GenderRatio>())
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
                }
                else
                {
                    model.Name = string.Concat(model.Pokemon.Name, " (", this.dataService.GetPokemonFormName(pokemon.Id), ")");
                }

                return this.View(model);
            }
            else if (this.dataService.GetObjectByPropertyValue<Pokemon>("PokedexNumber", pokemon.PokedexNumber) != null && this.dataService.GetObjectByPropertyValue<Pokemon>("Id", pokemon.Id, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth").PokedexNumber != pokemon.PokedexNumber)
            {
                BasePokemonViewModel model = new BasePokemonViewModel()
                {
                    Pokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", pokemon.Id, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth"),
                    AllGames = this.dataService.GetObjects<Game>("ReleaseDate, Id"),
                };

                if (!this.dataService.CheckIfAltForm(pokemon.Id))
                {
                    model.AllBaseHappinesses = this.dataService.GetObjects<BaseHappiness>("Happiness");
                    model.AllClassifications = this.dataService.GetObjects<Classification>("Name");
                    model.AllCaptureRates = this.dataService.GetObjects<CaptureRate>("CatchRate");
                    model.AllEggCycles = this.dataService.GetObjects<EggCycle>("CycleCount");
                    model.AllExperienceGrowths = this.dataService.GetObjects<ExperienceGrowth>("Name");
                    model.AllGenderRatios = new List<GenderRatioViewModel>();

                    foreach (GenderRatio genderRatio in this.dataService.GetObjects<GenderRatio>())
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
                }
                else
                {
                    model.Name = string.Concat(model.Pokemon.Name, " (", this.dataService.GetPokemonFormName(pokemon.Id), ")");
                }

                this.ModelState.AddModelError("PokedexNumber", "A pokemon with a this pokedex number already exists.");

                return this.View(model);
            }

            Pokemon oldPokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", pokemon.Id, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth");
            List<Pokemon> altForms = this.dataService.GetObjects<PokemonFormDetail>("AltFormPokemon.Game.ReleaseDate, AltFormPokemon.PokedexNumber, AltFormPokemon.Id", "AltFormPokemon, Form", "OriginalPokemonId", pokemon.Id).Select(x => x.AltFormPokemon).ToList();
            if (oldPokemon.PokedexNumber != pokemon.PokedexNumber)
            {
                foreach (var p in altForms)
                {
                    if (p.PokedexNumber != pokemon.PokedexNumber)
                    {
                        p.PokedexNumber = pokemon.PokedexNumber;
                        this.dataService.UpdateObject(p);
                    }
                }
            }

            this.dataService.UpdateObject(pokemon);

            return this.RedirectToAction("Pokemon", "Admin");
        }

        [HttpGet]
        [Route("edit_official_pokemon_image/{id:int}")]
        public IActionResult OfficialPokemonImage(int id)
        {
            Pokemon model = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", id, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth");

            if (this.dataService.CheckIfAltForm(id))
            {
                model.Name = string.Concat(model.Name, " (", this.dataService.GetPokemonFormName(id), ")");
            }

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_official_pokemon_image/{id:int}")]
        public IActionResult OfficialPokemonImage(Pokemon pokemon, int id, IFormFile fileUpload, string urlUpload)
        {
            if (!this.ModelState.IsValid && pokemon.Name.Length <= 25)
            {
                Pokemon model = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", id, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth");

                if (this.dataService.CheckIfAltForm(id))
                {
                    model.Name = string.Concat(model.Name, " (", this.dataService.GetPokemonFormName(id), ")");
                }

                return this.View(model);
            }
            else if (fileUpload == null && string.IsNullOrEmpty(urlUpload))
            {
                Pokemon model = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", id, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth");

                if (this.dataService.CheckIfAltForm(id))
                {
                    model.Name = string.Concat(model.Name, " (", this.dataService.GetPokemonFormName(id), ")");
                }

                this.ModelState.AddModelError("Picture", "An image is needed to update.");
                return this.View(model);
            }
            else if ((fileUpload?.FileName.Contains(".png") == false) || (!string.IsNullOrEmpty(urlUpload) && !urlUpload.Contains(".png")))
            {
                Pokemon model = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", id, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth");

                if (this.dataService.CheckIfAltForm(id))
                {
                    model.Name = string.Concat(model.Name, " (", this.dataService.GetPokemonFormName(id), ")");
                }

                this.ModelState.AddModelError("Picture", "The image must be in the .png format.");
                return this.View(model);
            }

            this.dataService.UploadImages(fileUpload, urlUpload, id, this.appConfig, "2d");

            return this.RedirectToAction("Pokemon", "Admin");
        }

        [HttpGet]
        [Route("edit_home_pokemon_image/{id:int}")]
        public IActionResult HomePokemonImage(int id)
        {
            Pokemon model = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", id, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth");

            if (this.dataService.CheckIfAltForm(id))
            {
                model.Name = string.Concat(model.Name, " (", this.dataService.GetPokemonFormName(id), ")");
            }

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_home_pokemon_image/{id:int}")]
        public IActionResult HomePokemonImage(Pokemon pokemon, int id, IFormFile fileUpload, string urlUpload)
        {
            if (!this.ModelState.IsValid && pokemon.Name.Length <= 25)
            {
                Pokemon model = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", id, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth");

                if (this.dataService.CheckIfAltForm(id))
                {
                    model.Name = string.Concat(model.Name, " (", this.dataService.GetPokemonFormName(id), ")");
                }

                return this.View(model);
            }
            else if (fileUpload == null && string.IsNullOrEmpty(urlUpload))
            {
                Pokemon model = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", id, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth");

                if (this.dataService.CheckIfAltForm(id))
                {
                    model.Name = string.Concat(model.Name, " (", this.dataService.GetPokemonFormName(id), ")");
                }

                this.ModelState.AddModelError("Picture", "An image is needed to update.");
                return this.View(model);
            }
            else if ((fileUpload?.FileName.Contains(".png") == false) || (!string.IsNullOrEmpty(urlUpload) && !urlUpload.Contains(".png")))
            {
                Pokemon model = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", id, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth");

                if (this.dataService.CheckIfAltForm(id))
                {
                    model.Name = string.Concat(model.Name, " (", this.dataService.GetPokemonFormName(id), ")");
                }

                this.ModelState.AddModelError("Picture", "The image must be in the .png format.");
                return this.View(model);
            }

            this.dataService.UploadImages(fileUpload, urlUpload, id, this.appConfig, "3d");

            pokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", id);
            pokemon.HasThreeDImage = true;
            this.dataService.UpdateObject(pokemon);

            return this.RedirectToAction("Pokemon", "Admin");
        }

        [HttpGet]
        [Route("edit_shiny_pokemon_image/{id:int}")]
        public IActionResult ShinyPokemonImage(int id)
        {
            Pokemon model = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", id, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth");

            if (this.dataService.CheckIfAltForm(id))
            {
                model.Name = string.Concat(model.Name, " (", this.dataService.GetPokemonFormName(id), ")");
            }

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_shiny_pokemon_image/{id:int}")]
        public IActionResult ShinyPokemonImage(Pokemon pokemon, int id, IFormFile fileUpload, string urlUpload)
        {
            if (!this.ModelState.IsValid && pokemon.Name.Length <= 25)
            {
                Pokemon model = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", id, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth");

                if (this.dataService.CheckIfAltForm(id))
                {
                    model.Name = string.Concat(model.Name, " (", this.dataService.GetPokemonFormName(id), ")");
                }

                return this.View(model);
            }
            else if (fileUpload == null && string.IsNullOrEmpty(urlUpload))
            {
                Pokemon model = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", id, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth");

                if (this.dataService.CheckIfAltForm(id))
                {
                    model.Name = string.Concat(model.Name, " (", this.dataService.GetPokemonFormName(id), ")");
                }

                this.ModelState.AddModelError("Picture", "An image is needed to update.");
                return this.View(model);
            }
            else if ((fileUpload?.FileName.Contains(".png") == false) || (!string.IsNullOrEmpty(urlUpload) && !urlUpload.Contains(".png")))
            {
                Pokemon model = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", id, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth");

                if (this.dataService.CheckIfAltForm(id))
                {
                    model.Name = string.Concat(model.Name, " (", this.dataService.GetPokemonFormName(id), ")");
                }

                this.ModelState.AddModelError("Picture", "The image must be in the .png format.");
                return this.View(model);
            }

            this.dataService.UploadImages(fileUpload, urlUpload, id, this.appConfig, "shiny");

            pokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", id);
            pokemon.HasShinyImage = true;
            this.dataService.UpdateObject(pokemon);

            return this.RedirectToAction("Pokemon", "Admin");
        }

        [HttpGet]
        [Route("edit_typing/{pokemonId:int}/{generationId:int}")]
        public IActionResult Typing(int pokemonId, int generationId)
        {
            PokemonTypeDetail typeDetail = this.dataService.GetObjects<PokemonTypeDetail>(includes: "Pokemon, PrimaryType, SecondaryType, Generation", whereProperty: "PokemonId", wherePropertyValue: pokemonId).Find(x => x.GenerationId == generationId);
            PokemonTypingViewModel model = new PokemonTypingViewModel()
            {
                Id = typeDetail.Id,
                AllTypes = this.dataService.GetObjects<Type>("Name"),
                PokemonId = typeDetail.PokemonId,
                Pokemon = typeDetail.Pokemon,
                PrimaryTypeId = typeDetail.PrimaryTypeId,
                SecondaryTypeId = typeDetail.SecondaryTypeId,
                GenerationId = typeDetail.GenerationId,
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_typing/{id:int}/{generationId:int}")]
        public IActionResult Typing(PokemonTypeDetail pokemonTypeDetail)
        {
            if (!this.ModelState.IsValid)
            {
                PokemonTypeDetail typeDetail = this.dataService.GetObjects<PokemonTypeDetail>(includes: "Pokemon, PrimaryType, SecondaryType, Generation", whereProperty: "PokemonId", wherePropertyValue: pokemonTypeDetail.PokemonId).Find(x => x.GenerationId == pokemonTypeDetail.GenerationId);
                PokemonTypingViewModel model = new PokemonTypingViewModel()
                {
                    Id = typeDetail.Id,
                    AllTypes = this.dataService.GetObjects<Type>("Name"),
                    PokemonId = typeDetail.PokemonId,
                    Pokemon = typeDetail.Pokemon,
                    PrimaryTypeId = typeDetail.PrimaryTypeId,
                    SecondaryTypeId = typeDetail.SecondaryTypeId,
                    GenerationId = typeDetail.GenerationId,
                };

                return this.View(model);
            }

            if (pokemonTypeDetail.PrimaryTypeId == pokemonTypeDetail.SecondaryTypeId)
            {
                pokemonTypeDetail.SecondaryTypeId = null;
            }

            this.dataService.UpdateObject(pokemonTypeDetail);

            return this.RedirectToAction("Pokemon", "Admin");
        }

        [HttpGet]
        [Route("edit_abilities/{pokemonId:int}/{generationId:int}")]
        public IActionResult Abilities(int pokemonId, int generationId)
        {
            PokemonAbilityDetail abilityDetail = this.dataService.GetObjects<PokemonAbilityDetail>(includes: "Pokemon, PrimaryAbility, SecondaryAbility, HiddenAbility, SpecialEventAbility", whereProperty: "PokemonId", wherePropertyValue: pokemonId).Find(x => x.GenerationId == generationId);
            PokemonAbilitiesViewModel model = new PokemonAbilitiesViewModel()
            {
                Id = abilityDetail.Id,
                AllAbilities = this.dataService.GetObjects<Ability>("Name"),
                PokemonId = abilityDetail.PokemonId,
                Pokemon = abilityDetail.Pokemon,
                PrimaryAbilityId = abilityDetail.PrimaryAbilityId,
                SecondaryAbilityId = abilityDetail.SecondaryAbilityId,
                HiddenAbilityId = abilityDetail.HiddenAbilityId,
                GenerationId = generationId,
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_abilities/{pokemonId:int}/{generationId:int}")]
        public IActionResult Abilities(PokemonAbilityDetail pokemonAbilityDetail)
        {
            if (!this.ModelState.IsValid)
            {
                PokemonAbilityDetail abilityDetail = this.dataService.GetObjects<PokemonAbilityDetail>(includes: "Pokemon, PrimaryAbility, SecondaryAbility, HiddenAbility, SpecialEventAbility", whereProperty: "PokemonId", wherePropertyValue: pokemonAbilityDetail.PokemonId).Find(x => x.GenerationId == pokemonAbilityDetail.GenerationId);
                PokemonAbilitiesViewModel model = new PokemonAbilitiesViewModel()
                {
                    Id = abilityDetail.Id,
                    AllAbilities = this.dataService.GetObjects<Ability>("Name"),
                    PokemonId = abilityDetail.PokemonId,
                    Pokemon = abilityDetail.Pokemon,
                    PrimaryAbilityId = abilityDetail.PrimaryAbilityId,
                    SecondaryAbilityId = abilityDetail.SecondaryAbilityId,
                    HiddenAbilityId = abilityDetail.HiddenAbilityId,
                    GenerationId = abilityDetail.GenerationId,
                };

                return this.View(model);
            }

            this.dataService.UpdateObject(pokemonAbilityDetail);

            return this.RedirectToAction("Pokemon", "Admin");
        }

        [HttpGet]
        [Route("edit_egg_groups/{pokemonId:int}/{generationId:int}")]
        public IActionResult EggGroups(int pokemonId, int generationId)
        {
            PokemonEggGroupDetail eggGroupDetail = this.dataService.GetObjects<PokemonEggGroupDetail>(includes: "Pokemon, PrimaryEggGroup, SecondaryEggGroup", whereProperty: "PokemonId", wherePropertyValue: pokemonId).Find(x => x.GenerationId == generationId);
            PokemonEggGroupsViewModel model = new PokemonEggGroupsViewModel()
            {
                Id = eggGroupDetail.Id,
                AllEggGroups = this.dataService.GetObjects<EggGroup>("Name"),
                PokemonId = eggGroupDetail.PokemonId,
                Pokemon = eggGroupDetail.Pokemon,
                PrimaryEggGroupId = eggGroupDetail.PrimaryEggGroupId,
                SecondaryEggGroupId = eggGroupDetail.SecondaryEggGroupId,
                GenerationId = generationId,
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_egg_groups/{pokemonId:int}/{generationId:int}")]
        public IActionResult EggGroups(PokemonEggGroupDetail pokemonEggGroupDetail)
        {
            if (!this.ModelState.IsValid)
            {
                PokemonEggGroupDetail eggGroupDetail = this.dataService.GetObjects<PokemonEggGroupDetail>(includes: "Pokemon, PrimaryEggGroup, SecondaryEggGroup", whereProperty: "PokemonId", wherePropertyValue: pokemonEggGroupDetail.PokemonId).Find(x => x.GenerationId == pokemonEggGroupDetail.GenerationId);
                PokemonEggGroupsViewModel model = new PokemonEggGroupsViewModel()
                {
                    Id = eggGroupDetail.Id,
                    AllEggGroups = this.dataService.GetObjects<EggGroup>("Name"),
                    PokemonId = eggGroupDetail.PokemonId,
                    Pokemon = eggGroupDetail.Pokemon,
                    PrimaryEggGroupId = eggGroupDetail.PrimaryEggGroupId,
                    SecondaryEggGroupId = eggGroupDetail.SecondaryEggGroupId,
                    GenerationId = eggGroupDetail.GenerationId,
                };

                return this.View(model);
            }

            this.dataService.UpdateObject(pokemonEggGroupDetail);

            return this.RedirectToAction("Pokemon", "Admin");
        }

        [HttpGet]
        [Route("edit_base_stats/{pokemonId:int}/{generationId:int}")]
        public IActionResult BaseStats(int pokemonId, int generationId)
        {
            BaseStat model = this.dataService.GetPokemonBaseStats(pokemonId, generationId);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_base_stats/{pokemonId:int}/{generationId:int}")]
        public IActionResult BaseStats(BaseStat baseStat)
        {
            if (!this.ModelState.IsValid)
            {
                BaseStat model = this.dataService.GetPokemonBaseStats(baseStat.PokemonId, baseStat.GenerationId);

                return this.View(model);
            }

            this.dataService.UpdateObject(baseStat);

            return this.RedirectToAction("Pokemon", "Admin");
        }

        [HttpGet]
        [Route("edit_ev_yields/{pokemonId:int}/{generationId:int}")]
        public IActionResult EVYields(int pokemonId, int generationId)
        {
            EVYield model = this.dataService.GetPokemonEVYields(pokemonId, generationId);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_ev_yields/{pokemonId:int}/{generationId:int}")]
        public IActionResult EVYields(EVYield evYield)
        {
            if (!this.ModelState.IsValid)
            {
                EVYield model = this.dataService.GetPokemonEVYields(evYield.PokemonId, evYield.GenerationId);

                return this.View(model);
            }

            this.dataService.UpdateObject(evYield);

            return this.RedirectToAction("Pokemon", "Admin");
        }

        [HttpGet]
        [Route("edit_type/{id:int}")]
        public IActionResult Type(int id)
        {
            Type type = this.dataService.GetObjectByPropertyValue<Type>("Id", id);
            TypeGenerationViewModel model = new TypeGenerationViewModel()
            {
                Name = type.Name,
                GenerationId = type.GenerationId,
                AllGenerations = this.dataService.GetObjects<Generation>(),
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_type/{id:int}")]
        public IActionResult Type(Type type)
        {
            if (!this.ModelState.IsValid)
            {
                Type newType = this.dataService.GetObjectByPropertyValue<Type>("Id", type.Id);
                TypeGenerationViewModel model = new TypeGenerationViewModel()
                {
                    Name = newType.Name,
                    GenerationId = newType.GenerationId,
                    AllGenerations = this.dataService.GetObjects<Generation>(),
                };

                return this.View(model);
            }

            this.dataService.UpdateObject(type);

            return this.RedirectToAction("Types", "Admin");
        }

        [HttpGet]
        [Route("edit_egg_cycle/{id:int}")]
        public IActionResult EggCycle(int id)
        {
            EggCycle model = this.dataService.GetObjectByPropertyValue<EggCycle>("Id", id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_egg_cycle/{id:int}")]
        public IActionResult EggCycle(EggCycle eggCycle)
        {
            if (!this.ModelState.IsValid)
            {
                EggCycle model = this.dataService.GetObjectByPropertyValue<EggCycle>("Id", eggCycle.Id);

                return this.View(model);
            }

            this.dataService.UpdateObject(eggCycle);

            return this.RedirectToAction("EggCycles", "Admin");
        }

        [HttpGet]
        [Route("edit_experience_growth/{id:int}")]
        public IActionResult ExperienceGrowth(int id)
        {
            ExperienceGrowth model = this.dataService.GetObjectByPropertyValue<ExperienceGrowth>("Id", id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_experience_growth/{id:int}")]
        public IActionResult ExperienceGrowth(ExperienceGrowth experienceGrowth)
        {
            if (!this.ModelState.IsValid)
            {
                ExperienceGrowth model = this.dataService.GetObjectByPropertyValue<ExperienceGrowth>("Id", experienceGrowth.Id);

                return this.View(model);
            }

            this.dataService.UpdateObject(experienceGrowth);

            return this.RedirectToAction("ExperienceGrowths", "Admin");
        }

        [HttpGet]
        [Route("edit_gender_ratio/{id:int}")]
        public IActionResult GenderRatio(int id)
        {
            GenderRatio model = this.dataService.GetObjectByPropertyValue<GenderRatio>("Id", id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_gender_ratio/{id:int}")]
        public IActionResult GenderRatio(GenderRatio genderRatio)
        {
            if (!this.ModelState.IsValid)
            {
                GenderRatio model = this.dataService.GetObjectByPropertyValue<GenderRatio>("Id", genderRatio.Id);

                return this.View(model);
            }

            this.dataService.UpdateObject(genderRatio);

            return this.RedirectToAction("GenderRatios", "Admin");
        }

        [HttpGet]
        [Route("edit_egg_group/{id:int}")]
        public IActionResult EggGroup(int id)
        {
            EggGroup model = this.dataService.GetObjectByPropertyValue<EggGroup>("Id", id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_egg_group/{id:int}")]
        public IActionResult EggGroup(EggGroup eggGroup)
        {
            if (!this.ModelState.IsValid)
            {
                EggGroup model = this.dataService.GetObjectByPropertyValue<EggGroup>("Id", eggGroup.Id);

                return this.View(model);
            }

            this.dataService.UpdateObject(eggGroup);

            return this.RedirectToAction("EggGroups", "Admin");
        }

        [HttpGet]
        [Route("edit_form_item/{id:int}")]
        public IActionResult FormItem(int id)
        {
            FormItem item = this.dataService.GetObjectByPropertyValue<FormItem>("Id", id);
            FormItemViewModel model = new FormItemViewModel()
            {
                PokemonId = item.PokemonId,
                Name = item.Name,
            };

            List<Pokemon> altForms = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, OriginalPokemon, Form").Where(x => x.Form.NeedsItem).ToList().ConvertAll(x => x.AltFormPokemon);

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
        [Route("edit_form_item/{id:int}")]
        public IActionResult FormItem(FormItem formItem)
        {
            if (!this.ModelState.IsValid)
            {
                FormItem item = this.dataService.GetObjectByPropertyValue<FormItem>("Id", formItem.Id);
                FormItemViewModel model = new FormItemViewModel()
                {
                    PokemonId = item.PokemonId,
                    Name = item.Name,
                };

                List<Pokemon> altForms = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, OriginalPokemon, Form").Where(x => x.Form.NeedsItem).ToList().ConvertAll(x => x.AltFormPokemon);

                altForms.Remove(altForms.Find(x => x.Name == "Rayquaza"));

                foreach (var p in altForms)
                {
                    p.Name = string.Concat(p.Name, " (", this.dataService.GetPokemonFormName(p.Id), ")");
                }

                model.AllPokemon = altForms;

                return this.View(model);
            }

            this.dataService.UpdateObject(formItem);

            return this.RedirectToAction("FormItems", "Admin");
        }

        [HttpGet]
        [Route("edit_form_group/{id:int}")]
        public IActionResult FormGroup(int id)
        {
            FormGroup model = this.dataService.GetObjectByPropertyValue<FormGroup>("Id", id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_form_group/{id:int}")]
        public IActionResult FormGroup(FormGroup formGroup)
        {
            if (!this.ModelState.IsValid)
            {
                FormGroup model = this.dataService.GetObjectByPropertyValue<FormGroup>("Id", formGroup.Id);

                return this.View(model);
            }

            this.dataService.UpdateObject(formGroup);

            return this.RedirectToAction("FormGroups", "Admin");
        }

        [HttpGet]
        [Route("edit_form/{id:int}")]
        public IActionResult Form(int id)
        {
            Form form = this.dataService.GetObjectByPropertyValue<Form>("Id", id);
            FormModelViewModel model = new FormModelViewModel()
            {
                Id = form.Id,
                Name = form.Name,
                Randomizable = form.Randomizable,
                FormGroupId = form.FormGroupId,
                NeedsItem = form.NeedsItem,
                AllFormGroups = this.dataService.GetObjects<FormGroup>(),
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_form/{id:int}")]
        public IActionResult Form(Form form)
        {
            if (!this.ModelState.IsValid)
            {
                Form newForm = this.dataService.GetObjectByPropertyValue<Form>("Id", form.Id);
                FormModelViewModel model = new FormModelViewModel()
                {
                    Id = newForm.Id,
                    Name = newForm.Name,
                    Randomizable = newForm.Randomizable,
                    FormGroupId = newForm.FormGroupId,
                    NeedsItem = newForm.NeedsItem,
                    AllFormGroups = this.dataService.GetObjects<FormGroup>(),
                };

                return this.View(model);
            }

            this.dataService.UpdateObject(form);

            return this.RedirectToAction("Forms", "Admin");
        }

        [HttpGet]
        [Route("edit_evolution_method/{id:int}")]
        public IActionResult EvolutionMethod(int id)
        {
            EvolutionMethod model = this.dataService.GetObjectByPropertyValue<EvolutionMethod>("Id", id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_evolution_method/{id:int}")]
        public IActionResult EvolutionMethod(EvolutionMethod evolutionMethod)
        {
            if (!this.ModelState.IsValid)
            {
                EvolutionMethod model = this.dataService.GetObjectByPropertyValue<EvolutionMethod>("Id", evolutionMethod.Id);

                return this.View(model);
            }

            this.dataService.UpdateObject(evolutionMethod);

            return this.RedirectToAction("EvolutionMethods", "Admin");
        }

        [HttpGet]
        [Route("edit_capture_rate/{id:int}")]
        public IActionResult CaptureRate(int id)
        {
            CaptureRate model = this.dataService.GetObjectByPropertyValue<CaptureRate>("Id", id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_capture_rate/{id:int}")]
        public IActionResult CaptureRate(CaptureRate captureRate)
        {
            if (!this.ModelState.IsValid)
            {
                CaptureRate model = this.dataService.GetObjectByPropertyValue<CaptureRate>("Id", captureRate.Id);

                return this.View(model);
            }

            this.dataService.UpdateObject(captureRate);

            return this.RedirectToAction("CaptureRates", "Admin");
        }

        [HttpGet]
        [Route("edit_base_happiness/{id:int}")]
        public IActionResult BaseHappiness(int id)
        {
            BaseHappiness model = this.dataService.GetObjectByPropertyValue<BaseHappiness>("Id", id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_base_happiness/{id:int}")]
        public IActionResult BaseHappiness(BaseHappiness baseHappiness)
        {
            if (!this.ModelState.IsValid)
            {
                BaseHappiness model = this.dataService.GetObjectByPropertyValue<BaseHappiness>("Id", baseHappiness.Id);

                return this.View(model);
            }

            this.dataService.UpdateObject(baseHappiness);

            return this.RedirectToAction("BaseHappinesses", "Admin");
        }

        [HttpGet]
        [Route("edit_region/{id:int}")]
        public IActionResult Region(int id)
        {
            Region region = this.dataService.GetObjectByPropertyValue<Region>("Id", id, "Generation");
            RegionAdminViewModel model = new RegionAdminViewModel()
            {
                AllGenerations = this.dataService.GetObjects<Generation>(),
                Id = region.Id,
                Name = region.Name,
                GenerationId = region.GenerationId,
                Generation = region.Generation,
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_region/{id:int}")]
        public IActionResult Region(Region region)
        {
            if (!this.ModelState.IsValid)
            {
                Region newRegion = this.dataService.GetObjectByPropertyValue<Region>("Id", region.Id, "Generation");
                RegionAdminViewModel model = new RegionAdminViewModel()
                {
                    AllGenerations = this.dataService.GetObjects<Generation>(),
                    Id = newRegion.Id,
                    Name = newRegion.Name,
                    GenerationId = newRegion.GenerationId,
                    Generation = newRegion.Generation,
                };

                return this.View(model);
            }

            this.dataService.UpdateObject(region);

            return this.RedirectToAction("Regions", "Admin");
        }

        [HttpGet]
        [Route("edit_pokemon_capture_rate/{pokemonId:int}/{generationId:int}")]
        public IActionResult CaptureRates(int pokemonId, int generationId)
        {
            PokemonCaptureRateDetail captureRate = this.GetPokemonWithCaptureRatesFromGenerationId(pokemonId, generationId);
            PokemonCaptureRateViewModel model = new PokemonCaptureRateViewModel()
            {
                Id = captureRate.Id,
                PokemonId = captureRate.PokemonId,
                GenerationId = captureRate.GenerationId,
                CaptureRateId = captureRate.CaptureRateId,
                AllCaptureRates = this.dataService.GetObjects<CaptureRate>("CatchRate"),
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_pokemon_capture_rate/{id:int}/{generationId:int}")]
        public IActionResult CaptureRates(PokemonCaptureRateDetail pokemonCaptureRate)
        {
            if (!this.ModelState.IsValid)
            {
                PokemonCaptureRateDetail captureRate = this.GetPokemonWithCaptureRatesFromGenerationId(pokemonCaptureRate.PokemonId, pokemonCaptureRate.GenerationId);
                PokemonCaptureRateViewModel model = new PokemonCaptureRateViewModel()
                {
                    Id = captureRate.Id,
                    PokemonId = captureRate.PokemonId,
                    GenerationId = captureRate.GenerationId,
                    CaptureRateId = captureRate.CaptureRateId,
                    AllCaptureRates = this.dataService.GetObjects<CaptureRate>("CatchRate"),
                };

                return this.View(model);
            }

            this.dataService.UpdateObject(pokemonCaptureRate);

            return this.RedirectToAction("Pokemon", "Admin");
        }

        [HttpGet]
        [Route("edit_pokemon_base_happiness/{pokemonId:int}/{generationId:int}")]
        public IActionResult BaseHappinesses(int pokemonId, int generationId)
        {
            PokemonBaseHappinessDetail bassHappiness = this.GetPokemonWithBaseHappinesssFromGenerationId(pokemonId, generationId);
            PokemonBaseHappinessViewModel model = new PokemonBaseHappinessViewModel()
            {
                Id = bassHappiness.Id,
                PokemonId = bassHappiness.PokemonId,
                GenerationId = bassHappiness.GenerationId,
                BaseHappinessId = bassHappiness.BaseHappinessId,
                AllBaseHappinesses = this.dataService.GetObjects<BaseHappiness>("Happiness"),
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_pokemon_base_happiness/{id:int}/{generationId:int}")]
        public IActionResult BaseHappinesses(PokemonBaseHappinessDetail pokemonBaseHappiness)
        {
            if (!this.ModelState.IsValid)
            {
                PokemonBaseHappinessDetail baseHappiness = this.GetPokemonWithBaseHappinesssFromGenerationId(pokemonBaseHappiness.PokemonId, pokemonBaseHappiness.GenerationId);
                PokemonBaseHappinessViewModel model = new PokemonBaseHappinessViewModel()
                {
                    Id = baseHappiness.Id,
                    PokemonId = baseHappiness.PokemonId,
                    GenerationId = baseHappiness.GenerationId,
                    BaseHappinessId = baseHappiness.BaseHappinessId,
                    AllBaseHappinesses = this.dataService.GetObjects<BaseHappiness>("Happiness"),
                };

                return this.View(model);
            }

            this.dataService.UpdateObject(pokemonBaseHappiness);

            return this.RedirectToAction("Pokemon", "Admin");
        }

        [HttpGet]
        [Route("edit_nature/{id:int}")]
        public IActionResult Nature(int id)
        {
            Nature model = this.dataService.GetObjectByPropertyValue<Nature>("Id", id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_nature/{id:int}")]
        public IActionResult Nature(Nature nature)
        {
            if (!this.ModelState.IsValid)
            {
                Nature model = this.dataService.GetObjectByPropertyValue<Nature>("Id", nature.Id);

                return this.View(model);
            }

            this.dataService.UpdateObject(nature);

            return this.RedirectToAction("Natures", "Admin");
        }

        [HttpGet]
        [Route("edit_legendary_type/{id:int}")]
        public IActionResult LegendaryType(int id)
        {
            LegendaryType model = this.dataService.GetObjectByPropertyValue<LegendaryType>("Id", id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_legendary_type/{id:int}")]
        public IActionResult LegendaryType(LegendaryType legendaryType)
        {
            if (!this.ModelState.IsValid)
            {
                LegendaryType model = this.dataService.GetObjectByPropertyValue<LegendaryType>("Id", legendaryType.Id);

                return this.View(model);
            }

            this.dataService.UpdateObject(legendaryType);

            return this.RedirectToAction("LegendaryTypes", "Admin");
        }

        [HttpGet]
        [Route("edit_classification/{id:int}")]
        public IActionResult Classification(int id)
        {
            Classification model = this.dataService.GetObjectByPropertyValue<Classification>("Id", id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_classification/{id:int}")]
        public IActionResult Classification(Classification classification)
        {
            if (!this.ModelState.IsValid)
            {
                Classification model = this.dataService.GetObjectByPropertyValue<Classification>("Id", classification.Id);

                return this.View(model);
            }

            if (classification.Name.Contains("pokemon"))
            {
                classification.Name = classification.Name.Replace("pokemon", "Pokemon");
            }
            else if (!classification.Name.Contains("Pokemon") && !classification.Name.Contains("pokemon"))
            {
                classification.Name = string.Concat(classification.Name.Trim(), " Pokemon");
            }

            this.dataService.UpdateObject(classification);

            return this.RedirectToAction("Classifications", "Admin");
        }

        [HttpGet]
        [Route("edit_ability/{id:int}")]
        public IActionResult Ability(int id)
        {
            Ability ability = this.dataService.GetObjectByPropertyValue<Ability>("Id", id);
            AbilityAdminViewModel model = new AbilityAdminViewModel()
            {
                Id = ability.Id,
                Description = ability.Description,
                GenerationId = ability.GenerationId,
                AllGenerations = this.dataService.GetObjects<Generation>(),
            };
            return this.View(model);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_ability/{id:int}")]
        public IActionResult Ability(Ability ability)
        {
            if (!this.ModelState.IsValid)
            {
                Ability model = this.dataService.GetObjectByPropertyValue<Ability>("Id", ability.Id);

                return this.View(model);
            }

            this.dataService.UpdateObject(ability);

            return this.RedirectToAction("Abilities", "Admin");
        }

        [HttpGet]
        [Route("edit_evolution/{pokemonId:int}/{generationId:int}")]
        public IActionResult Evolution(int pokemonId, int generationId)
        {
            Evolution preEvolution = this.dataService.GetObjects<Evolution>(includes: "PreevolutionPokemon, PreevolutionPokemon.Game, EvolutionPokemon, EvolutionPokemon.Game, EvolutionMethod").Find(x => x.EvolutionPokemonId == pokemonId && x.GenerationId == generationId);
            EvolutionViewModel model = new EvolutionViewModel()
            {
                AllEvolutionMethods = this.dataService.GetObjects<EvolutionMethod>("Name"),
                Id = preEvolution.Id,
                EvolutionDetails = preEvolution.EvolutionDetails,
                EvolutionMethodId = preEvolution.EvolutionMethodId,
                EvolutionMethod = preEvolution.EvolutionMethod,
                PreevolutionPokemon = preEvolution.PreevolutionPokemon,
                PreevolutionPokemonId = preEvolution.PreevolutionPokemonId,
                EvolutionPokemonId = preEvolution.EvolutionPokemonId,
                EvolutionPokemon = preEvolution.EvolutionPokemon,
                GenerationId = preEvolution.GenerationId,
            };

            List<Pokemon> pokemonList = this.dataService.GetObjects<Pokemon>("PokedexNumber, Id", "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth").Where(x => x.Id != pokemonId).ToList();
            List<Pokemon> altFormsList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form").ConvertAll(x => x.AltFormPokemon);
            foreach (var pokemon in pokemonList.Where(x => altFormsList.Any(y => y.Id == x.Id)))
            {
                pokemon.Name = string.Concat(pokemon.Name, " (", this.dataService.GetPokemonFormName(pokemon.Id), ")");
            }

            model.AllPokemon = pokemonList;

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit_evolution/{pokemonId:int}/{generationId:int}")]
        public IActionResult Evolution(EvolutionViewModel evolution)
        {
            if (!this.ModelState.IsValid)
            {
                Evolution preEvolution = this.dataService.GetObjects<Evolution>(includes: "PreevolutionPokemon, PreevolutionPokemon.Game, EvolutionPokemon, EvolutionPokemon.Game, EvolutionMethod").Find(x => x.EvolutionPokemonId == evolution.EvolutionPokemonId && x.GenerationId == evolution.GenerationId);
                EvolutionViewModel model = new EvolutionViewModel()
                {
                    AllEvolutionMethods = this.dataService.GetObjects<EvolutionMethod>("Name"),
                    Id = evolution.Id,
                    EvolutionDetails = preEvolution.EvolutionDetails,
                    EvolutionMethodId = preEvolution.EvolutionMethodId,
                    EvolutionMethod = preEvolution.EvolutionMethod,
                    PreevolutionPokemon = preEvolution.PreevolutionPokemon,
                    PreevolutionPokemonId = preEvolution.PreevolutionPokemonId,
                    EvolutionPokemonId = preEvolution.EvolutionPokemonId,
                    EvolutionPokemon = preEvolution.EvolutionPokemon,
                    GenerationId = preEvolution.GenerationId,
                };

                List<Pokemon> pokemonList = this.dataService.GetAllPokemon().Where(x => x.Id != evolution.EvolutionPokemonId).ToList();
                List<Pokemon> altFormsList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form").ConvertAll(x => x.AltFormPokemon);
                foreach (var pokemon in pokemonList.Where(x => altFormsList.Any(y => y.Id == x.Id)))
                {
                    pokemon.Name = string.Concat(pokemon.Name, " (", this.dataService.GetPokemonFormName(pokemon.Id), ")");
                }

                model.AllPokemon = pokemonList;

                return this.View(model);
            }

            this.dataService.UpdateObject(evolution);

            return this.RedirectToAction("Pokemon", "Admin");
        }

        [Route("edit_alternate_forms/{pokemonId:int}")]
        public IActionResult AltForms(int pokemonId)
        {
            Pokemon originalPokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", pokemonId, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth");
            List<Pokemon> altFormList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form").Select(x => x.AltFormPokemon).Where(x => x.Name == originalPokemon.Name).ToList();
            foreach (var pokemon in altFormList)
            {
                pokemon.Name = string.Concat(pokemon.Name, " (", this.dataService.GetPokemonFormName(pokemon.Id), ")");
            }

            AllAdminPokemonViewModel allPokemon = new AllAdminPokemonViewModel()
            {
                AllPokemon = altFormList,
                AllAltForms = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form"),
                AllEvolutions = this.dataService.GetObjects<Evolution>(includes: "PreevolutionPokemon, PreevolutionPokemon.Game, EvolutionPokemon, EvolutionPokemon.Game, EvolutionMethod"),
                AllTypings = this.dataService.GetObjects<PokemonTypeDetail>("PokemonId", "Pokemon, PrimaryType, SecondaryType"),
                AllAbilities = this.dataService.GetObjects<PokemonAbilityDetail>(includes: "Pokemon, PrimaryAbility, SecondaryAbility, HiddenAbility, SpecialEventAbility"),
                AllEggGroups = this.dataService.GetObjects<PokemonEggGroupDetail>(includes: "Pokemon, PrimaryEggGroup, SecondaryEggGroup"),
                AllBaseStats = this.dataService.GetObjects<BaseStat>(includes: "Pokemon"),
                AllEVYields = this.dataService.GetObjects<EVYield>(includes: "Pokemon"),
                AllLegendaryDetails = this.dataService.GetObjects<PokemonLegendaryDetail>(includes: "Pokemon, LegendaryType"),
                AllPokemonCaptureRates = this.dataService.GetAllPokemonWithCaptureRates(),
                AllPokemonBaseHappinesses = this.dataService.GetAllPokemonWithBaseHappinesses(),
            };

            DropdownViewModel model = new DropdownViewModel()
            {
                AllPokemon = allPokemon,
                AppConfig = this.appConfig,
            };

            return this.View(model);
        }

        [HttpGet]
        [Route("edit_alternate_form/{pokemonId:int}")]
        public IActionResult AltFormsForm(int pokemonId)
        {
            PokemonFormDetail pokemonForm = this.dataService.GetObjectByPropertyValue<PokemonFormDetail>("AltFormPokemonId", pokemonId, "AltFormPokemon, OriginalPokemon, Form");
            AlternateFormsFormViewModel model = new AlternateFormsFormViewModel()
            {
                PokemonFormDetail = pokemonForm,
                AllForms = this.dataService.GetObjects<Form>("Name"),
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
                PokemonFormDetail pokemonForm = this.dataService.GetObjectByPropertyValue<PokemonFormDetail>("AltFormPokemonId", pokemonFormDetail.AltFormPokemonId, "AltFormPokemon, OriginalPokemon, Form");
                AlternateFormsFormViewModel model = new AlternateFormsFormViewModel()
                {
                    PokemonFormDetail = pokemonForm,
                    AllForms = this.dataService.GetObjects<Form>("Name"),
                };

                return this.View(model);
            }

            this.dataService.UpdateObject(pokemonFormDetail);

            return this.RedirectToAction("AltForms", "Edit", new { pokemonId = pokemonFormDetail.OriginalPokemonId });
        }

        private PokemonCaptureRateDetail GetPokemonWithCaptureRatesFromGenerationId(int pokemonId, int generationId)
        {
            return this.dataService.GetObjects<PokemonCaptureRateDetail>(includes: "Pokemon, CaptureRate, Generation").First(x => x.Pokemon.Id == pokemonId && x.Generation.Id == generationId);
        }

        private PokemonBaseHappinessDetail GetPokemonWithBaseHappinesssFromGenerationId(int pokemonId, int generationId)
        {
            return this.dataService.GetObjects<PokemonBaseHappinessDetail>(includes: "Pokemon, BaseHappiness, Generation").First(x => x.Pokemon.Id == pokemonId && x.Generation.Id == generationId);
        }

        /// <summary>
        /// Gets a list of all pokemon and adds the form name to it if it is an alternate form.
        /// </summary>
        /// <returns>Returns all pokemon with the form name attached.</returns>
        private List<Pokemon> GetAllPokemonWithFormNames()
        {
            List<Pokemon> pokemonList = this.dataService.GetObjects<Pokemon>("PokedexNumber, Id", "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth");
            List<PokemonFormDetail> pokemonForm = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, Form");
            List<Pokemon> altFormList = pokemonForm.ConvertAll(x => x.AltFormPokemon);

            foreach (var p in pokemonForm)
            {
                p.AltFormPokemon.Name = string.Concat(p.AltFormPokemon.Name, " (", p.Form.Name, ")");
                altFormList.Add(p.AltFormPokemon);
            }

            Pokemon pokemon;

            foreach (var a in altFormList)
            {
                pokemon = pokemonList.Find(x => x.Id == a.Id);
                pokemon.Name = a.Name;
            }

            return pokemonList;
        }
    }
}
