using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Pokedex.DataAccess.Models;
using Pokedex.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Pokedex.Controllers
{
    [Authorize]
    [Route("")]
    public class AjaxController : Controller
    {
        private readonly DataService dataService;

        private readonly AppConfig appConfig;

        public AjaxController(IOptions<AppConfig> appConfig, DataContext dataContext)
        {
            this.appConfig = appConfig.Value;
            this.dataService = new DataService(dataContext);
        }

        [Route("mark-as-read")]
        public void MarkAsRead()
        {
            List<Message> messages = this.dataService.GetObjects<Message>(whereProperty: "ReceiverId", wherePropertyValue: Convert.ToInt32(this.User.Claims.First(x => x.Type == "UserId").Value));
            foreach (var m in messages.Where(x => !x.IsRead))
            {
                m.IsRead = true;
                this.dataService.UpdateObject(m);
            }
        }

        [Route("check-unread-messages")]
        public int CheckUnreadMessages()
        {
            return this.dataService.GetObjects<Message>(whereProperty: "ReceiverId", wherePropertyValue: Convert.ToInt32(this.User.Claims.First(x => x.Type == "UserId").Value)).Where(x => !x.IsRead).ToList().Count;
        }

        [Route("update-last-visit")]
        public void UpdateLastVisit()
        {
            User user = this.dataService.GetObjectByPropertyValue<User>("Id", Convert.ToInt32(this.User.Claims.First(x => x.Type == "UserId").Value));
            user.LastVisit = DateTime.Now.ToUniversalTime();
            this.dataService.UpdateObject(user);
        }

        [Route("get-pokemon-by-generation-admin/{generationId}")]
        public IActionResult GetPokemonByGenerationAdmin(int generationId)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<Pokemon> pokemonList = this.dataService.GetObjects<Pokemon>("PokedexNumber, Id", "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth");
                List<Pokemon> altFormList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form").ConvertAll(x => x.AltFormPokemon);

                AllAdminPokemonViewModel allAdminPokemon = this.dataService.GetAllAdminPokemonDetails();

                DropdownViewModel dropdownViewModel = new DropdownViewModel()
                {
                    AllPokemon = allAdminPokemon,
                    AppConfig = this.appConfig,
                    GenerationId = generationId,
                };

                AdminGenerationTableViewModel model = new AdminGenerationTableViewModel()
                {
                    PokemonList = pokemonList.Where(x => !altFormList.Any(y => y.Id == x.Id)).Where(x => x.Game.GenerationId == generationId).ToList(),
                    DropdownViewModel = dropdownViewModel,
                    AppConfig = this.appConfig,
                };

                return this.PartialView("_FillAdminGenerationTable", model);
            }
            else
            {
                return this.RedirectToAction("Index", "Home");
            }
        }

        [Route("get-incomplete-pokemon-admin/")]
        public IActionResult GetPokemonByGenerationAdmin()
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                AllAdminPokemonViewModel allAdminPokemon = this.dataService.GetAllAdminPokemonDetails();

                DropdownViewModel dropdownViewModel = new DropdownViewModel()
                {
                    AllPokemon = allAdminPokemon,
                    AppConfig = this.appConfig,
                    GenerationId = this.dataService.GetObjects<Generation>().Last().Id,
                };

                AdminGenerationTableViewModel model = new AdminGenerationTableViewModel()
                {
                    PokemonList = this.dataService.GetObjects<Pokemon>("PokedexNumber, Id", "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth").Where(x => !x.IsComplete).ToList(),
                    ReviewedPokemon = new List<ReviewedPokemon>(),
                    DropdownViewModel = dropdownViewModel,
                    AppConfig = this.appConfig,
                };

                foreach (var r in this.dataService.GetObjects<ReviewedPokemon>("Pokemon.PokedexNumber", "Pokemon"))
                {
                    model.PokemonList.Remove(model.PokemonList.Find(x => x.Id == r.PokemonId));
                    model.ReviewedPokemon.Add(r);
                }

                return this.PartialView("_FillAdminGenerationTable", model);
            }
            else
            {
                return this.RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [Route("update-pokemon-list/{gameId}")]
        public UpdatePokemonListViewModel UpdatePokemonList(int gameId)
        {
            Game game = this.dataService.GetObjectByPropertyValue<Game>("Id", gameId);
            List<PokemonGameDetail> pokemonGameDetails = this.dataService.GetPokemonGameDetailsByGame(gameId);
            List<Pokemon> altFormsList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form").ConvertAll(x => x.AltFormPokemon);
            UpdatePokemonListViewModel pokemonList = new UpdatePokemonListViewModel()
            {
                PokemonList = this.dataService.GetAllPokemon().Where(x => pokemonGameDetails.Any(y => y.PokemonId == x.Id)).ToList(),
                Game = game,
            };

            foreach (var pokemon in pokemonList.PokemonList.Where(x => altFormsList.Any(y => y.Id == x.Id)))
            {
                pokemon.Name = this.dataService.GetAltFormWithFormName(pokemon.Id).Name;
            }

            return pokemonList;
        }

        [AllowAnonymous]
        [Route("grab-user-pokemon-team")]
        public List<ExportPokemonViewModel> ExportUserPokemonTeam(int teamId)
        {
            PokemonTeam pokemonTeam = this.dataService.GetPokemonTeams().Find(x => x.Id == teamId);
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest" && (pokemonTeam.User.Username == this.User.Identity.Name || this.User.IsInRole("Owner")))
            {
                List<ExportPokemonViewModel> exportList = new List<ExportPokemonViewModel>();
                List<PokemonTeamDetail> pokemonList = pokemonTeam.GrabPokemonTeamDetails;
                if (pokemonList.Count > 0)
                {
                    ExportPokemonViewModel pokemonTeamExport = new ExportPokemonViewModel()
                    {
                        ExportString = "=== ",
                        TeamId = pokemonTeam.Id,
                    };

                    if (pokemonTeam.Game != null)
                    {
                        pokemonTeamExport.ExportString = string.Concat(pokemonTeamExport.ExportString, " [gen", pokemonTeam.Game.GenerationId, "] ");
                    }

                    pokemonTeamExport.ExportString = string.Concat(pokemonTeamExport.ExportString, pokemonTeam.PokemonTeamName, " ===\n\n");

                    for (var i = 0; i < pokemonList.Count; i++)
                    {
                        if (i != 0)
                        {
                            pokemonTeamExport.ExportString = string.Concat(pokemonTeamExport.ExportString, "\n\n");
                        }

                        pokemonTeamExport.ExportString = string.Concat(pokemonTeamExport.ExportString, this.FillUserPokemonTeam(pokemonList[i], pokemonTeam.GameId));
                    }

                    exportList.Add(pokemonTeamExport);
                }

                return exportList;
            }
            else
            {
                this.RedirectToAction("Home", "Index");
            }

            return null;
        }

        [AllowAnonymous]
        [Route("grab-all-user-pokemon-teams")]
        public List<ExportPokemonViewModel> ExportAllUserPokemonTeams()
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<PokemonTeam> pokemonTeams = this.dataService.GetAllPokemonTeams(this.User.Identity.Name);
                List<PokemonTeamDetail> pokemonList;
                List<ExportPokemonViewModel> exportList = new List<ExportPokemonViewModel>();
                foreach (var team in pokemonTeams)
                {
                    pokemonList = team.GrabPokemonTeamDetails;
                    if (pokemonList.Count > 0)
                    {
                        ExportPokemonViewModel pokemonTeam = new ExportPokemonViewModel
                        {
                            ExportString = "=== ",
                            TeamId = team.Id,
                        };

                        if (team.Game != null)
                        {
                            pokemonTeam.ExportString = string.Concat(pokemonTeam.ExportString, " [gen", team.Game.GenerationId, "] ");
                        }

                        pokemonTeam.ExportString = string.Concat(pokemonTeam.ExportString, team.PokemonTeamName, " ===\n\n");

                        for (var i = 0; i < pokemonList.Count; i++)
                        {
                            if (i != 0)
                            {
                                pokemonTeam.ExportString = string.Concat(pokemonTeam.ExportString, "\n\n");
                            }

                            pokemonTeam.ExportString = string.Concat(pokemonTeam.ExportString, this.FillUserPokemonTeam(pokemonList[i], team.GameId));
                        }

                        exportList.Add(pokemonTeam);
                    }
                }

                return exportList;
            }
            else
            {
                this.RedirectToAction("Home", "Index");
            }

            return null;
        }

        [AllowAnonymous]
        [Route("fill-user-pokemon-team")]
        public string FillUserPokemonTeam(PokemonTeamDetail pokemonTeamDetail, int? generationId)
        {
            Pokemon pokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", pokemonTeamDetail.PokemonId, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth");
            List<string> pokemonForm = new List<string>();
            string pokemonName = string.Empty;
            if (this.dataService.CheckIfAltForm(pokemon.Id))
            {
                pokemonForm = this.GetUserFormDetails(pokemon.Id);
            }

            if (!string.IsNullOrEmpty(pokemonTeamDetail.Nickname))
            {
                pokemonName = string.Concat(pokemonTeamDetail.Nickname, " (");
            }

            if (pokemon.Name.Contains(" (Male)"))
            {
                pokemon.Name = pokemon.Name.Replace(" (Male)", "-M");
            }
            else if (pokemon.Name.Contains(" (Female)"))
            {
                pokemon.Name = pokemon.Name.Replace(" (Female)", "-F");
            }

            pokemonName = string.Concat(pokemonName, pokemon.Name);
            if (this.dataService.CheckIfAltForm(pokemon.Id))
            {
                pokemonName = string.Concat(pokemonName, "-", (pokemonForm[0] == "Female") ? "F" : pokemonForm[0]);
            }

            if (!string.IsNullOrEmpty(pokemonTeamDetail.Nickname))
            {
                pokemonName = string.Concat(pokemonName, ")");
            }

            if (!string.IsNullOrEmpty(pokemonTeamDetail.Gender) && generationId != 1)
            {
                pokemonName = string.Concat(pokemonName, " (", pokemonTeamDetail.Gender.Substring(0, 1), ")");
            }

            if (pokemonForm.Count == 2)
            {
                pokemonName = string.Concat(pokemonName, pokemonForm[1]);
            }
            else if (pokemonTeamDetail.BattleItemId != null && generationId != 1)
            {
                pokemonName = string.Concat(pokemonName, " @ ", pokemonTeamDetail.BattleItem.Name);
            }

            string pokemonTeamString = pokemonName;
            if (generationId != 1 && generationId != 2)
            {
                pokemonTeamString = string.Concat(pokemonTeamString, "\nAbility: ", pokemonTeamDetail.Ability.Name);
            }

            if (pokemonTeamDetail.Level < 100)
            {
                pokemonTeamString = string.Concat(pokemonTeamString, "\nLevel: ", pokemonTeamDetail.Level.ToString());
            }

            if (pokemonTeamDetail.IsShiny && generationId != 1)
            {
                pokemonTeamString = string.Concat(pokemonTeamString, "\nShiny: Yes");
            }

            if (pokemonTeamDetail.Happiness < 255 && generationId != 1)
            {
                pokemonTeamString = string.Concat(pokemonTeamString, "\nHappiness: ", pokemonTeamDetail.Happiness.ToString());
            }

            pokemonTeamString = string.Concat(pokemonTeamString, this.FillEVs(pokemonTeamDetail.PokemonTeamEV));

            if (pokemonTeamDetail.Nature != null && generationId != 1 && generationId != 2)
            {
                pokemonTeamString = string.Concat(pokemonTeamString, "\n", pokemonTeamDetail.Nature.Name, " Nature");
            }

            pokemonTeamString = string.Concat(pokemonTeamString, this.FillIVs(pokemonTeamDetail.PokemonTeamIV), this.FillMoveset(pokemonTeamDetail.PokemonTeamMoveset));

            return pokemonTeamString;
        }

        [AllowAnonymous]
        [Route("save-pokemon-team")]
        public string SavePokemonTeam(string pokemonTeamName, int selectedGame, List<int> pokemonIdList, List<int> abilityIdList, bool exportAbilities)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                if (this.dataService.GetObjectByPropertyValue<User>("Username", this.User.Identity.Name) != null)
                {
                    if (string.IsNullOrEmpty(pokemonTeamName))
                    {
                        pokemonTeamName = "Save from Team Randomizer";
                    }

                    PokemonTeam pokemonTeam = new PokemonTeam()
                    {
                        PokemonTeamName = pokemonTeamName,
                        UserId = this.dataService.GetObjectByPropertyValue<User>("Username", this.User.Identity.Name).Id,
                    };

                    if (selectedGame != 0)
                    {
                        pokemonTeam.GameId = selectedGame;
                    }

                    Pokemon pokemon;
                    Ability ability;
                    PokemonTeamDetail pokemonTeamDetail;

                    for (var i = 0; i < pokemonIdList.Count; i++)
                    {
                        pokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", pokemonIdList[i], "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth");

                        if (exportAbilities)
                        {
                            ability = this.dataService.GetObjectByPropertyValue<Ability>("Id", abilityIdList[i]);
                        }
                        else
                        {
                            ability = this.dataService.GetAbilitiesForPokemon(pokemon.Id, selectedGame)[0];
                        }

                        pokemonTeamDetail = new PokemonTeamDetail()
                        {
                            PokemonId = pokemon.Id,
                            AbilityId = ability.Id,
                            NatureId = this.dataService.GetObjectByPropertyValue<Nature>("Name", "Serious").Id,
                            Level = 100,
                            Happiness = 255,
                        };

                        this.dataService.AddPokemonTeamDetail(pokemonTeamDetail);

                        pokemonTeam.InsertPokemon(pokemonTeamDetail);
                    }

                    this.dataService.AddObject(pokemonTeam);

                    return string.Concat("Team \"", pokemonTeam.PokemonTeamName, "\" has been added successfully!");
                }
                else
                {
                    return "You must be logged in to save a team.";
                }
            }
            else
            {
                this.RedirectToAction("Home", "Index");
            }

            return null;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("get-pokemon-list")]
        public List<Pokemon> GetPokemonList()
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<Pokemon> pokemonList = this.GetAllPokemonWithoutForms();

                foreach (var pokemon in pokemonList)
                {
                    if (pokemon.Name.Contains("type null"))
                    {
                        pokemon.Name = "Type: Null";
                    }

                    TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                    pokemon.Name = textInfo.ToTitleCase(pokemon.Name);

                    if (pokemon.Name.Contains("-O") && pokemon.Name.Substring(pokemon.Name.Length - 2, 2) == "-O")
                    {
                        pokemon.Name = string.Concat(pokemon.Name.Remove(pokemon.Name.Length - 2, 2), "-o");
                    }
                }

                return pokemonList;
            }
            else
            {
                this.RedirectToAction("Error");
            }

            return null;
        }

        [AllowAnonymous]
        [Route("get-pokemon-by-generation/{generationId}")]
        public IActionResult GetPokemonByGeneration(int generationId)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                GenerationTableViewModel model = new GenerationTableViewModel()
                {
                    PokemonList = this.dataService.GetAllPokemonWithTypes().Where(x => x.Pokemon.Game.GenerationId == generationId).ToList(),
                    AppConfig = this.appConfig,
                };

                return this.PartialView("_FillGenerationTable", model);
            }
            else
            {
                return this.RedirectToAction("Home", "Index");
            }
        }

        [AllowAnonymous]
        [Route("get-available-pokemon-by-game/{gameId}")]
        public IActionResult GetAvailablePokemonByGame(int gameId)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<PokemonGameDetail> pokemonGameDetails = this.dataService.GetPokemonGameDetailsByGame(gameId);
                List<Pokemon> pokemonList = this.dataService.GetObjects<Pokemon>("PokedexNumber, Id", "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth");
                pokemonList = pokemonList.Where(x => pokemonGameDetails.Any(y => y.PokemonId == x.Id)).ToList();
                List<Pokemon> altFormsList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form").ConvertAll(x => x.AltFormPokemon);
                foreach (var p in pokemonList.Where(x => altFormsList.Any(y => y.Id == x.Id)))
                {
                    p.Name = string.Concat(p.Name, " (", this.dataService.GetObjectByPropertyValue<PokemonFormDetail>("AltFormPokemonId", p.Id, "Form").Form.Name, ")");
                }

                GenerationTableViewModel model = new GenerationTableViewModel()
                {
                    PokemonNoTypeList = pokemonList,
                    AltFormsList = altFormsList,
                    AppConfig = this.appConfig,
                    Generation = this.dataService.GetObjectByPropertyValue<Game>("Id", gameId, "Generation").Generation,
                };

                return this.PartialView("_FillAvailableGenerationTable", model);
            }
            else
            {
                return this.RedirectToAction("Home", "Index");
            }
        }

        [AllowAnonymous]
        [Route("get-capture-chance")]
        public string GetCaptureChange(int pokemonId, int generationId, float healthPercentage, int pokeballId, int statusId, float turnCount, float encounterLevel, float userLevel, bool surfing, bool fishing, bool previouslyCaught, bool caveOrNight, bool storyComplete, bool sameGender)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                bool isColosseum = false;
                bool isGaleOfDarkness = false;
                if (generationId == 99)
                {
                    isColosseum = true;
                    generationId = 3;
                }
                else if (generationId == 100)
                {
                    isGaleOfDarkness = true;
                    generationId = 3;
                }

                Pokemon pokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", pokemonId, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth");
                Pokeball pokeball = this.dataService.GetObjectByPropertyValue<Pokeball>("Id", pokeballId, "Generation");
                PokemonLegendaryDetail legendary = this.dataService.GetObjectByPropertyValue<PokemonLegendaryDetail>("PokemonId", pokemon.Id, "LegendaryType");
                List<PokemonCaptureRateDetail> captureRates = this.dataService.GetPokemonWithCaptureRates(pokemonId).OrderBy(x => x.GenerationId).ToList();
                List<PokeballCatchModifierDetail> pokeballDetails = this.dataService.GetObjects<PokeballCatchModifierDetail>(whereProperty: "PokeballId", wherePropertyValue: pokeballId);
                float statusEffect = 1;
                if (statusId != 0)
                {
                    statusEffect = this.dataService.GetObjectByPropertyValue<Status>("Id", statusId).Effect;
                }

                double successfulCapture;
                List<double> wobbles = new List<double>();
                string chanceText = string.Empty;

                if ((isColosseum && pokemon.Name == "Makuhita") || (isGaleOfDarkness && pokemon.Name == "Teddiursa"))
                {
                    successfulCapture = 101;
                }
                else
                {
                    double catchRate = 0;

                    if (isColosseum)
                    {
                        switch (pokemon.Name)
                        {
                            case "Bayleef": case "Croconaw": case "Quilava":
                                catchRate = 180;
                                break;
                            case "Mantine": case "Misdreavus":
                                catchRate = 90;
                                break;
                            case "Togetic": case "Tropius":
                                catchRate = 45;
                                break;
                            case "Entei": case "Metagross": case "Raikou": case "Skarmory": case "Suicune":
                                catchRate = 15;
                                break;
                            case "Tyranitar":
                                catchRate = 10;
                                break;
                            default:
                                break;
                        }
                    }
                    else if (isGaleOfDarkness)
                    {
                        switch (pokemon.Name)
                        {
                            case "Houndour":
                                catchRate = 255;
                                break;
                            case "Delcatty": case "Golduck": case "Magcargo": case "Mawile": case "Venomoth":
                                catchRate = 120;
                                break;
                            case "Magneton": case "Marowak": case "Rapidash": case "Starmie":
                                catchRate = 110;
                                break;
                            case "Dugtrio": case "Lunatone": case "Rhydon":
                                catchRate = 100;
                                break;
                            case "Banette": case "Beedrill": case "Butterfree": case "Dodrio": case "Electabuzz": case "Hitmonchan": case "Hitmonlee": case "Kangaskhan": case "Lickitung": case "Magmar": case "Mr. Mime": case "Pinsir": case "Poliwrath": case "Sableye": case "Scyther": case "Solrock": case "Swellow": case "Tangela":
                                catchRate = 90;
                                break;
                            case "Altaria": case "Exeggutor": case "Farfetch'd": case "Hypno": case "Lapras": case "Manectric": case "Primeape": case "Salamence": case "Tauros":
                                catchRate = 80;
                                break;
                            case "Chansey": case "Snorlax":
                                catchRate = 70;
                                break;
                            case "Articuno": case "Moltres": case "Zapdos":
                                catchRate = 25;
                                break;
                            default:
                                break;
                        }
                    }

                    if (catchRate == 0)
                    {
                        catchRate = captureRates.First(x => x.GenerationId <= generationId).CaptureRate.CatchRate;
                    }

                    double pokeballEffect = 1;
                    if (statusEffect == 0)
                    {
                        statusEffect = 1;
                    }

                    double heavyValue = 0;

                    switch (pokeball.Name)
                    {
                        case "Heavy Ball":
                            foreach (var w in pokeballDetails)
                            {
                                if (pokemon.Weight < Convert.ToInt16(w.Effect))
                                {
                                    heavyValue = (int)w.CatchModifier;
                                    break;
                                }
                            }

                            catchRate += heavyValue;
                            if (catchRate <= 0)
                            {
                                catchRate = 1;
                            }

                            break;
                        case "Nest Ball":
                            if (encounterLevel <= 29)
                            {
                                pokeballEffect = (41f - encounterLevel) / 10;
                            }

                            break;
                        case "Repeat Ball":
                            pokeballEffect = pokeballDetails.Find(x => bool.Parse(x.Effect) == previouslyCaught).CatchModifier;
                            if (generationId == 6)
                            {
                                pokeballEffect -= .5;
                            }

                            break;
                        case "Dusk Ball":
                            pokeballEffect = pokeballDetails.Find(x => bool.Parse(x.Effect) == caveOrNight).CatchModifier;
                            if (generationId == 6)
                            {
                                pokeballEffect += .5;
                            }

                            break;
                        case "Quick Ball":
                            pokeballEffect = pokeballDetails.Find(x => bool.Parse(x.Effect) == (turnCount == 1)).CatchModifier;
                            break;
                        case "Net Ball":
                            PokemonTypeDetail typeDetail = this.dataService.GetObjects<PokemonTypeDetail>(includes: "Pokemon, PrimaryType, SecondaryType, Generation", whereProperty: "PokemonId", wherePropertyValue: pokemon.Id).Last();
                            bool netBallCondition = false;
                            if (typeDetail.PrimaryType.Name == "Bug" || typeDetail.PrimaryType.Name == "Water" || typeDetail.SecondaryType.Name == "Bug" || typeDetail.SecondaryType.Name == "Water")
                            {
                                netBallCondition = true;
                            }

                            pokeballEffect = pokeballDetails.Find(x => bool.Parse(x.Effect) == netBallCondition).CatchModifier;
                            if (generationId == 6)
                            {
                                pokeballEffect -= .5;
                            }

                            break;
                        case "Dive Ball":
                            pokeballEffect = pokeballDetails.Find(x => bool.Parse(x.Effect) == surfing).CatchModifier;
                            break;
                        case "Lure Ball":
                            pokeballEffect = pokeballDetails.Find(x => bool.Parse(x.Effect) == fishing).CatchModifier;
                            break;
                        case "Dream Ball":
                            pokeballEffect = pokeballDetails.Find(x => bool.Parse(x.Effect) == (statusId == 2)).CatchModifier;
                            break;
                        case "Fast Ball":
                            BaseStat baseStats = this.dataService.GetBaseStat(pokemon.Id).Last();
                            pokeballEffect = pokeballDetails.Find(x => bool.Parse(x.Effect) == (baseStats.Speed >= 100)).CatchModifier;
                            break;
                        case "Level Ball":
                            float multiple = userLevel / encounterLevel;
                            pokeballEffect = pokeballDetails.Find(x => Convert.ToDouble(x.Effect) < multiple).CatchModifier;
                            break;
                        case "Love Ball":
                            pokeballEffect = pokeballDetails.Find(x => bool.Parse(x.Effect) == sameGender).CatchModifier;
                            break;
                        case "Timer Ball":
                            pokeballEffect = 1f + ((turnCount - 1) * (1229f / 4096f));
                            if (pokeballEffect > 4)
                            {
                                pokeballEffect = 4;
                            }

                            break;
                        case "Moon Ball":
                            List<Evolution> evolutions = this.dataService.GetPokemonEvolutions(pokemon.Id).Where(x => x.GenerationId == generationId && x.PreevolutionPokemon.IsComplete && x.EvolutionPokemon.IsComplete).ToList();
                            bool moonStoneEvolve = false;
                            foreach (var e in evolutions)
                            {
                                if (e.EvolutionMethod.Name == "Evolutionary Item" && e.EvolutionDetails == "Moon Stone")
                                {
                                    moonStoneEvolve = true;
                                    break;
                                }
                            }

                            pokeballEffect = pokeballDetails.Find(x => bool.Parse(x.Effect) == moonStoneEvolve).CatchModifier;
                            break;
                        case "Beast Ball":
                            pokeballEffect = pokeballDetails.Find(x => bool.Parse(x.Effect) == (legendary?.LegendaryType.Type == "Ultra Beast")).CatchModifier;
                            break;
                        default:
                            pokeballEffect = pokeballDetails[0].CatchModifier;
                            break;
                    }

                    double catchValue;

                    if (generationId == 1)
                    {
                        double greatBallModifier = 12;
                        if (statusId == 1 || statusId == 2)
                        {
                            statusEffect = 25;
                        }
                        else if (statusId == 3 || statusId == 4 || statusId == 5)
                        {
                            statusEffect = 12;
                        }
                        else
                        {
                            statusEffect = 0;
                        }

                        if (pokeballId == 1)
                        {
                            pokeballEffect = 256;
                        }
                        else if (pokeballId == 2)
                        {
                            pokeballEffect = 201;
                            greatBallModifier = 8;
                        }
                        else
                        {
                            pokeballEffect = 151;
                        }

                        double hpFactor = Math.Floor(Math.Min(1020f / (greatBallModifier * healthPercentage), 255f));
                        successfulCapture = (statusEffect + (Math.Min(catchRate + 1f, pokeballEffect - statusEffect) * ((hpFactor + 1) / 256))) / pokeballEffect;
                    }
                    else if (generationId == 2)
                    {
                        if (pokeball.Id == 24)
                        {
                            pokeballEffect = 1;
                        }
                        else if (pokeball.Id == 18)
                        {
                            if (pokemonId == 1347 || pokemonId == 46 || pokemonId == 148)
                            {
                                pokeballEffect = 4;
                            }
                            else
                            {
                                pokeballEffect = 1;
                            }
                        }
                        else if (pokeball.Id == 20)
                        {
                            if (pokemon.Weight >= 409.6m)
                            {
                                heavyValue = 40;
                            }
                            else if (pokemon.Weight >= 307.2m)
                            {
                                heavyValue = 30;
                            }
                            else if (pokemon.Weight >= 204.8m)
                            {
                                heavyValue = 20;
                            }
                            else if (pokemon.Weight < 102.4m)
                            {
                                heavyValue = -20;
                            }

                            catchRate += heavyValue;
                        }

                        if (statusId == 1 || statusId == 2)
                        {
                            statusEffect = 10;
                        }
                        else
                        {
                            statusEffect = 0;
                        }

                        catchRate *= pokeballEffect;

                        if (catchRate > 255f)
                        {
                            catchRate = 255;
                        }
                        else if (catchRate < 0f)
                        {
                            catchRate = 1;
                        }

                        if (pokeball.Id == 21)
                        {
                            successfulCapture = ((Math.Floor(Math.Max(catchRate, 1f)) % 256f) + statusEffect + 1) / 256;
                        }
                        else
                        {
                            successfulCapture = ((Math.Floor(Math.Max(catchRate * (3f - (2f * healthPercentage)) / 3f, 1f)) % 256f) + statusEffect + 1) / 256;
                        }
                    }
                    else if (generationId == 3 || generationId == 4)
                    {
                        if (statusId == 1 || statusId == 2)
                        {
                            statusEffect = 2;
                        }

                        if (pokeball.Id == 20)
                        {
                            if (pokemon.Weight >= 409.6m)
                            {
                                heavyValue = 40;
                            }
                            else if (pokemon.Weight >= 307.2m)
                            {
                                heavyValue = 30;
                            }
                            else if (pokemon.Weight >= 204.8m)
                            {
                                heavyValue = 20;
                            }
                            else if (pokemon.Weight < 102.4m)
                            {
                                heavyValue = -20;
                            }

                            catchRate += heavyValue;
                        }

                        catchValue = Math.Floor((1 - ((2 * healthPercentage) / 3)) * catchRate * statusEffect);
                        if (pokeball.Id != 20)
                        {
                            catchValue *= pokeballEffect;
                            if (catchRate > 255f)
                            {
                                catchRate = 255;
                            }
                            else if (catchRate < 0f)
                            {
                                catchRate = 1;
                            }
                        }

                        double captureChance = Math.Floor(1048560 / Math.Floor(Math.Sqrt(Math.Floor(Math.Sqrt(16711680 / catchValue))))) / 65536;
                        double chanceInverse = 1 - captureChance;

                        successfulCapture = Math.Pow(captureChance, 4);
                        wobbles = new List<double>()
                        {
                            chanceInverse,
                            captureChance * chanceInverse,
                            Math.Pow(captureChance, 2) * chanceInverse,
                            Math.Pow(captureChance, 3) * chanceInverse,
                        };

                        for (var i = 0; i < 4; i++)
                        {
                            wobbles[i] = Math.Round(wobbles[i] * 100, 3);
                        }
                    }
                    else if (generationId >= 5 && generationId <= 7)
                    {
                        if (legendary?.LegendaryType.Type == "Ultra Beast" && pokeball.Name != "Beast Ball")
                        {
                            pokeballEffect = 0.10009765625;
                        }

                        catchValue = Math.Min((1d - ((2d * healthPercentage) / 3d)) * catchRate * statusEffect, 255);
                        if (pokeball.Name != "Heavy Ball")
                        {
                            catchValue *= pokeballEffect;
                            if (catchValue > 255f)
                            {
                                catchValue = 255;
                            }
                            else if (catchValue < 0f)
                            {
                                catchValue = 1;
                            }
                        }

                        double captureChance = Math.Floor(65536f / Math.Sqrt(Math.Sqrt(255f / catchValue))) / 65536;
                        double chanceInverse = 1 - captureChance;

                        successfulCapture = Math.Pow(captureChance, 3);
                        wobbles = new List<double>()
                        {
                            chanceInverse,
                            captureChance * chanceInverse,
                            0,
                            Math.Pow(captureChance, 2) * chanceInverse,
                        };

                        for (var i = 0; i < 4; i++)
                        {
                            wobbles[i] = Math.Round(wobbles[i] * 100, 3);
                        }
                    }
                    else
                    {
                        double lowLevelModifier = 1, difficultyModifier = 1;
                        if (legendary?.LegendaryType.Type == "Ultra Beast" && pokeball.Name != "Beast Ball")
                        {
                            pokeballEffect = 0.10009765625;
                        }

                        if (encounterLevel <= 19)
                        {
                            lowLevelModifier = (30 - encounterLevel) / 10d;
                        }

                        if (encounterLevel > userLevel && !storyComplete)
                        {
                            difficultyModifier = 410d / 4096d;
                        }

                        catchValue = Math.Min((1d - ((2d * healthPercentage) / 3d)) * catchRate * statusEffect * lowLevelModifier * difficultyModifier, 255);
                        if (pokeball.Name != "Heavy Ball")
                        {
                            catchValue *= pokeballEffect;
                            if (catchValue > 255f)
                            {
                                catchValue = 255;
                            }
                            else if (catchValue < 0f)
                            {
                                catchValue = 1;
                            }
                        }

                        double captureChance = Math.Floor(65536f / Math.Sqrt(Math.Sqrt(255f / catchValue))) / 65536;
                        double chanceInverse = 1 - captureChance;

                        successfulCapture = Math.Pow(captureChance, 3);
                        wobbles = new List<double>()
                        {
                            chanceInverse,
                            captureChance * chanceInverse,
                            0,
                            Math.Pow(captureChance, 2) * chanceInverse,
                        };

                        for (var i = 0; i < 4; i++)
                        {
                            wobbles[i] = Math.Round(wobbles[i] * 100, 3);
                        }
                    }

                    successfulCapture *= 100;
                    chanceText = Math.Round(successfulCapture, 3) + "% chance";
                }

                if (successfulCapture >= 100)
                {
                    return string.Concat("Guaranteed to catch ", pokemon.Name, " at ", (healthPercentage * 100).ToString(), "% health in a ", pokeball.Name);
                }
                else
                {
                    string successString = string.Concat(chanceText, " to catch ", pokemon.Name, " at ", (healthPercentage * 100).ToString(), "% health in a ", pokeball.Name, ".");
                    if (wobbles.Count > 0)
                    {
                        successString = string.Concat(successString, "|", wobbles[3].ToString(), "% chance of it wobbling 3 times", ".|");
                        if (wobbles[2] > 0)
                        {
                            successString = string.Concat(successString, wobbles[2].ToString(), "% chance of it wobbling 2 times", ".|");
                        }

                        successString = string.Concat(successString, wobbles[1].ToString(), "% chance of it wobbling 1 time", ".|", wobbles[0].ToString(), "% chance of it not wobbling at all.");
                    }

                    return successString;
                }
            }
            else
            {
                return null;
            }
        }

        [AllowAnonymous]
        [Route("get-pokemon-team")]
        public TeamRandomizerViewModel GetPokemonTeam(List<int> selectedGens, int selectedGameId, int selectedType, List<string> selectedLegendaries, List<string> selectedForms, string selectedEvolutions, bool onlyLegendaries, bool onlyAltForms, bool multipleMegas, bool multipleGMax, bool onePokemonForm, bool randomAbility, bool noRepeatType)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<Generation> unselectedGens = this.dataService.GetObjects<Generation>();
                foreach (var item in selectedGens)
                {
                    unselectedGens.Remove(unselectedGens.Find(x => x.Id == item));
                }

                Pokemon pokemon;
                Game selectedGame = new Game();
                if (selectedGameId != 0)
                {
                    selectedGame = this.dataService.GetObjectByPropertyValue<Game>("Id", selectedGameId);
                }

                TeamRandomizerViewModel model = new TeamRandomizerViewModel()
                {
                    AllPokemonChangedNames = new List<Pokemon>(),
                    AllPokemonOriginalNames = new List<Pokemon>(),
                    PokemonAbilities = new List<Ability>(),
                    AppConfig = this.appConfig,
                };
                List<Pokemon> pokemonList = new List<Pokemon>();
                List<PokemonGameDetail> availablePokemon = new List<PokemonGameDetail>();

                if (selectedGame.Id != 0)
                {
                    availablePokemon = this.dataService.GetPokemonGameDetailsByGame(selectedGame.Id);
                }
                else
                {
                    availablePokemon = this.dataService.GetObjects<PokemonGameDetail>(includes: "Pokemon, Game");
                }

                List<Pokemon> allPokemon = this.GetAllPokemonWithoutForms();
                List<Evolution> allEvolutions = this.dataService.GetObjects<Evolution>(includes: "PreevolutionPokemon, PreevolutionPokemon.Game, EvolutionPokemon, EvolutionPokemon.Game, EvolutionMethod");
                if (selectedGame.Id != 0)
                {
                    allEvolutions = allEvolutions.Where(x => x.PreevolutionPokemon.Game.ReleaseDate <= selectedGame.ReleaseDate && x.EvolutionPokemon.Game.ReleaseDate <= selectedGame.ReleaseDate).ToList();
                }

                Random rnd = new Random();

                foreach (var gen in unselectedGens)
                {
                    allPokemon = allPokemon.Except(allPokemon.Where(x => x.Game.GenerationId == gen.Id)).ToList();
                }

                if (selectedLegendaries.Count == 0)
                {
                    List<PokemonLegendaryDetail> legendaryList = this.dataService.GetObjects<PokemonLegendaryDetail>(includes: "Pokemon, LegendaryType", whereProperty: "Pokemon.IsComplete", wherePropertyValue: true);

                    foreach (var p in legendaryList)
                    {
                        allPokemon.Remove(allPokemon.Find(x => x.Id == p.PokemonId));
                    }
                }
                else
                {
                    List<string> legendaryTypes = this.dataService.GetObjects<LegendaryType>("Type").ConvertAll(x => x.Type);
                    foreach (var legendary in legendaryTypes.Except(selectedLegendaries).ToList())
                    {
                        List<PokemonLegendaryDetail> legendaryList = this.dataService.GetObjects<PokemonLegendaryDetail>(includes: "Pokemon, LegendaryType", whereProperty: "Pokemon.IsComplete", wherePropertyValue: true).Where(x => x.LegendaryType.Type == legendary).ToList();

                        foreach (var l in legendaryList)
                        {
                            allPokemon.Remove(allPokemon.Find(x => x.Id == l.PokemonId));
                        }
                    }
                }

                if (onlyLegendaries)
                {
                    List<Pokemon> legendaryList = new List<Pokemon>();
                    List<PokemonLegendaryDetail> allLegendaries = this.dataService.GetObjects<PokemonLegendaryDetail>(includes: "Pokemon, LegendaryType", whereProperty: "Pokemon.IsComplete", wherePropertyValue: true);

                    foreach (var p in allLegendaries)
                    {
                        if (allPokemon.Exists(x => x.Id == p.PokemonId))
                        {
                            legendaryList.Add(p.Pokemon);
                        }
                    }

                    allPokemon = legendaryList;
                }

                if (selectedForms.Count != 0)
                {
                    List<Pokemon> altForms = new List<Pokemon>();

                    if (selectedForms.Contains("Mega"))
                    {
                        List<PokemonFormDetail> pokemonFormList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form", whereProperty: "AltFormPokemon.IsComplete", wherePropertyValue: true).Where(x => x.Form.Name.Contains("Mega")).ToList();

                        List<PokemonFormDetail> filteredFormList = new List<PokemonFormDetail>();

                        foreach (var p in allPokemon)
                        {
                            List<PokemonFormDetail> altForm = pokemonFormList.Where(x => x.OriginalPokemonId == p.Id).ToList();
                            foreach (var a in altForm)
                            {
                                a.AltFormPokemon.Name = string.Concat(a.AltFormPokemon.Name, " (", a.Form.Name, ")");
                            }

                            if (altForm.Count > 0)
                            {
                                filteredFormList.AddRange(altForm);
                            }
                        }

                        if (filteredFormList.Count > 0)
                        {
                            foreach (var p in filteredFormList)
                            {
                                altForms.Add(p.AltFormPokemon);
                            }
                        }
                    }

                    if (selectedForms.Contains("Alolan"))
                    {
                        List<PokemonFormDetail> pokemonFormList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form", whereProperty: "AltFormPokemon.IsComplete", wherePropertyValue: true).Where(x => x.Form.Name == "Alola").ToList();

                        List<PokemonFormDetail> filteredFormList = new List<PokemonFormDetail>();

                        foreach (var p in allPokemon)
                        {
                            if (selectedGame.Id != 37 || (selectedGame.Id == 37 && p.Id == 54))
                            {
                                List<PokemonFormDetail> altForm = pokemonFormList.Where(x => x.OriginalPokemonId == p.Id).ToList();
                                foreach (var a in altForm)
                                {
                                    a.AltFormPokemon.Name = string.Concat(a.AltFormPokemon.Name, " (", a.Form.Name, ")");
                                }

                                if (altForm.Count > 0)
                                {
                                    filteredFormList.AddRange(altForm);
                                }
                            }
                        }

                        if (filteredFormList.Count > 0)
                        {
                            foreach (var p in filteredFormList)
                            {
                                altForms.Add(p.AltFormPokemon);
                            }
                        }
                    }

                    if (selectedForms.Contains("Galarian"))
                    {
                        List<PokemonFormDetail> pokemonFormList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form", whereProperty: "AltFormPokemon.IsComplete", wherePropertyValue: true).Where(x => x.Form.Name == "Galar").ToList();

                        List<PokemonFormDetail> filteredFormList = new List<PokemonFormDetail>();

                        foreach (var p in allPokemon)
                        {
                            List<PokemonFormDetail> altForm = pokemonFormList.Where(x => x.OriginalPokemonId == p.Id).ToList();
                            foreach (var a in altForm)
                            {
                                a.AltFormPokemon.Name = string.Concat(a.AltFormPokemon.Name, " (", a.Form.Name, ")");
                            }

                            if (altForm.Count > 0)
                            {
                                filteredFormList.AddRange(altForm);
                            }
                        }

                        if (filteredFormList.Count > 0)
                        {
                            foreach (var p in filteredFormList)
                            {
                                altForms.Add(p.AltFormPokemon);
                            }
                        }
                    }

                    if (selectedForms.Contains("Gigantamax"))
                    {
                        List<PokemonFormDetail> pokemonFormList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form", whereProperty: "AltFormPokemon.IsComplete", wherePropertyValue: true).Where(x => x.Form.Name.Contains("Gigantamax")).ToList();

                        List<PokemonFormDetail> filteredFormList = new List<PokemonFormDetail>();

                        foreach (var p in allPokemon)
                        {
                            List<PokemonFormDetail> altForm = pokemonFormList.Where(x => x.OriginalPokemonId == p.Id).ToList();
                            foreach (var a in altForm)
                            {
                                a.AltFormPokemon.Name = string.Concat(a.AltFormPokemon.Name, " (", a.Form.Name, ")");
                            }

                            if (altForm.Count > 0)
                            {
                                filteredFormList.AddRange(altForm);
                            }
                        }

                        if (filteredFormList.Count > 0)
                        {
                            foreach (var p in filteredFormList)
                            {
                                altForms.Add(p.AltFormPokemon);
                            }
                        }
                    }

                    if (selectedForms.Contains("Hisuian"))
                    {
                        List<PokemonFormDetail> pokemonFormList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form", whereProperty: "AltFormPokemon.IsComplete", wherePropertyValue: true).Where(x => x.Form.Name == "Hisui").ToList();

                        List<PokemonFormDetail> filteredFormList = new List<PokemonFormDetail>();

                        foreach (var p in allPokemon)
                        {
                            List<PokemonFormDetail> altForm = pokemonFormList.Where(x => x.OriginalPokemonId == p.Id).ToList();
                            foreach (var a in altForm)
                            {
                                a.AltFormPokemon.Name = string.Concat(a.AltFormPokemon.Name, " (", a.Form.Name, ")");
                            }

                            if (altForm.Count > 0)
                            {
                                filteredFormList.AddRange(altForm);
                            }
                        }

                        if (filteredFormList.Count > 0)
                        {
                            foreach (var p in filteredFormList)
                            {
                                altForms.Add(p.AltFormPokemon);
                            }
                        }
                    }

                    if (selectedForms.Contains("Other"))
                    {
                        List<PokemonFormDetail> pokemonFormList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form", whereProperty: "AltFormPokemon.IsComplete", wherePropertyValue: true);

                        List<Form> formsToRemove = this.dataService.GetObjects<Form>("Name").Where(x => !x.Randomizable).ToList();

                        foreach (var f in formsToRemove)
                        {
                            pokemonFormList = pokemonFormList.Where(x => x.Form.Name != f.Name).ToList();
                        }

                        List<PokemonFormDetail> filteredFormList = new List<PokemonFormDetail>();

                        foreach (var p in allPokemon)
                        {
                            List<PokemonFormDetail> altForm = pokemonFormList.Where(x => x.OriginalPokemonId == p.Id).ToList();
                            foreach (var a in altForm)
                            {
                                a.AltFormPokemon.Name = string.Concat(a.AltFormPokemon.Name, " (", a.Form.Name, ")");
                            }

                            if (altForm.Count > 0)
                            {
                                filteredFormList.AddRange(altForm);
                            }
                        }

                        if (filteredFormList.Count > 0)
                        {
                            foreach (var p in filteredFormList)
                            {
                                altForms.Add(p.AltFormPokemon);
                            }
                        }
                    }

                    allPokemon.AddRange(altForms);

                    allPokemon = this.RemoveExtraPokemonForms(allPokemon);
                }

                if (onlyAltForms)
                {
                    List<Pokemon> altFormsList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form").ConvertAll(x => x.AltFormPokemon);
                    allPokemon = allPokemon.Where(x => altFormsList.Any(y => y.Id == x.Id)).ToList();
                }

                if (selectedType != 0)
                {
                    List<PokemonTypeDetail> allTypes = new List<PokemonTypeDetail>();
                    if (selectedGame.Id != 0)
                    {
                        allTypes = this.GetAllPokemonWithSpecificType(selectedType, selectedGame.GenerationId, allPokemon);
                    }
                    else
                    {
                        allTypes = this.dataService.GetObjects<PokemonTypeDetail>("Pokemon.PokedexNumber, PokemonId", "Pokemon, Pokemon.Game, PrimaryType, SecondaryType", "Pokemon.IsComplete", true).Where(x => allPokemon.Any(y => y.Id == x.PokemonId)).ToList();
                        allTypes = allTypes.Where(x => x.PrimaryTypeId == selectedType || x.SecondaryTypeId == selectedType).ToList();
                    }

                    allPokemon = allPokemon.Where(x => allTypes.Select(x => x.Pokemon).Any(y => y.Id == x.Id)).ToList();
                }

                if (!multipleMegas)
                {
                    List<Pokemon> megaList = new List<Pokemon>();
                    List<PokemonFormDetail> altFormList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form");
                    foreach (var p in altFormList.Where(x => x.Form.Name.Contains("Mega")).ToList())
                    {
                        if (allPokemon.Exists(x => x.Id == p.AltFormPokemonId))
                        {
                            megaList.Add(p.AltFormPokemon);
                        }
                    }

                    if (megaList.Count > 0)
                    {
                        Pokemon mega = megaList[rnd.Next(megaList.Count)];
                        foreach (var p in megaList.Where(x => x.Id != mega.Id))
                        {
                            if (allPokemon.Exists(x => x.Id == p.Id))
                            {
                                allPokemon.Remove(allPokemon.Find(x => x.Id == p.Id));
                            }
                        }
                    }
                }

                if (!multipleGMax)
                {
                    List<Pokemon> altFormList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form").Where(x => x.Form.Name.Contains("Gigantamax")).Select(x => x.AltFormPokemon).ToList();
                    List<Pokemon> gigantamaxList = allPokemon.Where(x => altFormList.Any(y => y.Id == x.Id)).ToList();

                    if (gigantamaxList.Count > 0)
                    {
                        Pokemon gigantamax = gigantamaxList[rnd.Next(gigantamaxList.Count)];
                        foreach (var p in gigantamaxList.Where(x => x.Id != gigantamax.Id))
                        {
                            if (allPokemon.Exists(x => x.Id == p.Id))
                            {
                                allPokemon.Remove(allPokemon.Find(x => x.Id == p.Id));
                            }
                        }
                    }
                }

                if (selectedEvolutions == "stage1Pokemon")
                {
                    List<Pokemon> newPokemon = new List<Pokemon>();
                    foreach (var p in allPokemon)
                    {
                        if (allEvolutions.Exists(x => x.PreevolutionPokemonId == p.Id) && !allEvolutions.Exists(x => x.EvolutionPokemonId == p.Id))
                        {
                            newPokemon.Add(p);
                        }
                    }

                    allPokemon = newPokemon;
                }
                else if (selectedEvolutions == "middleEvolution")
                {
                    List<Pokemon> newPokemon = new List<Pokemon>();
                    foreach (var p in allPokemon)
                    {
                        if (allEvolutions.Exists(x => x.PreevolutionPokemonId == p.Id) && allEvolutions.Exists(x => x.EvolutionPokemonId == p.Id))
                        {
                            newPokemon.Add(p);
                        }
                    }

                    allPokemon = newPokemon;
                }
                else if (selectedEvolutions == "onlyFullyEvolved")
                {
                    List<Pokemon> newPokemon = new List<Pokemon>();
                    foreach (var p in allPokemon)
                    {
                        if (!allEvolutions.Exists(x => x.PreevolutionPokemonId == p.Id))
                        {
                            newPokemon.Add(p);
                        }
                    }

                    allPokemon = newPokemon;
                }

                if (availablePokemon.Count > 1)
                {
                    allPokemon = allPokemon.Where(x => availablePokemon.Any(y => y.PokemonId == x.Id)).ToList();
                }

                if (allPokemon.Count > 0)
                {
                    for (var i = 0; i < 6; i++)
                    {
                        if (pokemonList.Count >= allPokemon.Count)
                        {
                            break;
                        }

                        pokemon = allPokemon[rnd.Next(allPokemon.Count)];
                        while (pokemonList.Contains(pokemon))
                        {
                            pokemon = allPokemon[rnd.Next(allPokemon.Count)];
                        }

                        if (onePokemonForm)
                        {
                            int originalPokemonId;
                            if (this.dataService.CheckIfAltForm(pokemon.Id))
                            {
                                originalPokemonId = this.dataService.GetObjectByPropertyValue<PokemonFormDetail>("AltFormPokemonId", pokemon.Id, "AltFormPokemon, OriginalPokemon, Form").OriginalPokemonId;
                            }
                            else
                            {
                                originalPokemonId = pokemon.Id;
                            }

                            List<Pokemon> altForms = this.dataService.GetAltForms(originalPokemonId);

                            if (this.dataService.CheckIfAltForm(pokemon.Id))
                            {
                                altForms.Remove(altForms.Find(x => x.Id == pokemon.Id));
                                altForms.Add(this.dataService.GetObjectByPropertyValue<Pokemon>("Id", originalPokemonId, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth"));
                            }

                            foreach (var p in altForms)
                            {
                                if (allPokemon.Exists(x => x.Id == p.Id))
                                {
                                    allPokemon.Remove(allPokemon.Find(x => x.Id == p.Id));
                                }
                            }
                        }

                        if (noRepeatType)
                        {
                            List<PokemonTypeDetail> pokemonTypeDetails = this.dataService.GetObjects<PokemonTypeDetail>(includes: "PrimaryType, SecondaryType");
                            List<DataAccess.Models.Type> selectedPokemonTyping = new List<DataAccess.Models.Type>()
                            {
                                pokemonTypeDetails.Find(x => x.PokemonId == pokemon.Id).PrimaryType,
                            };

                            if (pokemonTypeDetails.Find(x => x.PokemonId == pokemon.Id).SecondaryType != null)
                            {
                                selectedPokemonTyping.Add(pokemonTypeDetails.Find(x => x.PokemonId == pokemon.Id).SecondaryType);
                            }

                            foreach (var t in selectedPokemonTyping)
                            {
                                if (pokemonTypeDetails.Exists(x => x.PrimaryTypeId == t.Id))
                                {
                                    foreach (var ptd in pokemonTypeDetails.Where(x => x.PrimaryTypeId == t.Id))
                                    {
                                        allPokemon.Remove(allPokemon.Find(x => x.Id == ptd.PokemonId));
                                    }
                                }

                                if (pokemonTypeDetails.Exists(x => x.SecondaryTypeId == t.Id))
                                {
                                    foreach (var ptd in pokemonTypeDetails.Where(x => x.SecondaryTypeId == t.Id))
                                    {
                                        allPokemon.Remove(allPokemon.Find(x => x.Id == ptd.PokemonId));
                                    }
                                }
                            }
                        }

                        pokemonList.Add(pokemon);
                    }
                }

                model.AllPokemonChangedNames = pokemonList;
                foreach (var p in pokemonList)
                {
                    model.AllPokemonOriginalNames.Add(this.dataService.GetObjectByPropertyValue<Pokemon>("Id", p.Id, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth"));
                }

                int generationId = 0;

                if (selectedGame.Id != 0)
                {
                    generationId = selectedGame.GenerationId;
                }

                model.PokemonURLs = new List<string>();
                foreach (var p in model.AllPokemonOriginalNames)
                {
                    if (generationId != 0)
                    {
                        model.PokemonURLs.Add(this.Url.Action("PokemonWithId", "Home", new { pokemonName = p.Name.Replace(": ", "_").Replace(' ', '_').ToLower(), pokemonId = p.Id, generationId }));
                    }
                    else
                    {
                        model.PokemonURLs.Add(this.Url.Action("PokemonWithId", "Home", new { pokemonName = p.Name.Replace(": ", "_").Replace(' ', '_').ToLower(), pokemonId = p.Id, generationId = this.dataService.GetObjects<PokemonGameDetail>(includes: "Game", whereProperty: "PokemonId", wherePropertyValue: p.Id).Select(x => x.Game).Last().Id }));
                    }
                }

                if (randomAbility && selectedGame.Id != 1 && selectedGame.Id != 2)
                {
                    foreach (var p in model.AllPokemonOriginalNames)
                    {
                        PokemonAbilityDetail pokemonAbilities;
                        List<Ability> abilities = new List<Ability>();
                        if (generationId != 0)
                        {
                            pokemonAbilities = this.dataService.GetObjects<PokemonAbilityDetail>(includes: "Pokemon, PrimaryAbility, SecondaryAbility, HiddenAbility, SpecialEventAbility", whereProperty: "PokemonId", wherePropertyValue: p.Id).Last(x => x.GenerationId <= generationId);
                        }
                        else
                        {
                            pokemonAbilities = this.dataService.GetObjects<PokemonAbilityDetail>(includes: "Pokemon, PrimaryAbility, SecondaryAbility, HiddenAbility, SpecialEventAbility", whereProperty: "PokemonId", wherePropertyValue: p.Id).Last();
                        }

                        abilities.Add(pokemonAbilities.PrimaryAbility);
                        if (pokemonAbilities.SecondaryAbility != null)
                        {
                            abilities.Add(pokemonAbilities.SecondaryAbility);
                        }

                        if (pokemonAbilities.HiddenAbility != null)
                        {
                            abilities.Add(pokemonAbilities.HiddenAbility);
                        }

                        if (pokemonAbilities.SpecialEventAbility != null)
                        {
                            abilities.Add(pokemonAbilities.SpecialEventAbility);
                        }

                        model.PokemonAbilities.Add(abilities[rnd.Next(abilities.Count)]);
                    }
                }

                List<int> pokemonIds = new List<int>();
                foreach (var p in model.AllPokemonOriginalNames)
                {
                    pokemonIds.Add(p.Id);
                }

                List<string> abilityNames = new List<string>();
                foreach (var a in model.PokemonAbilities)
                {
                    abilityNames.Add(a.Name);
                }

                model.ExportString = this.ExportPokemonTeam(pokemonIds, abilityNames, randomAbility);

                this.dataService.AddPageView("Random Team Generated", this.User.IsInRole("Owner"));

                return model;
            }
            else
            {
                this.RedirectToAction("Home", "Index");
            }

            return null;
        }

        [AllowAnonymous]
        [Route("get-pokemon-abilities")]
        public List<Ability> GetPokemonAbilities(int pokemonId, int gameId)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<Ability> pokemonAbilities = this.dataService.GetAbilitiesForPokemon(pokemonId, gameId);
                return pokemonAbilities;
            }
            else
            {
                this.RedirectToAction("Home", "Index");
            }

            return null;
        }

        [AllowAnonymous]
        [Route("update-type-chart")]
        public string UpdateTypeChart(int typeId, int genId, List<int> resistances, List<int> weaknesses, List<int> immunities)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                TypeChart typeChart;
                List<int> duplicateIds = resistances.Where(x => weaknesses.Contains(x)).ToList();
                List<TypeChart> existingCharts = this.dataService.GetTypeChartByDefendType(typeId, genId);

                foreach (var t in duplicateIds)
                {
                    resistances.Remove(resistances.Find(x => x == t));
                    weaknesses.Remove(weaknesses.Find(x => x == t));
                }

                foreach (var t in immunities)
                {
                    if (resistances.IndexOf(t) != -1)
                    {
                        resistances.Remove(resistances.Find(x => x == t));
                    }

                    if (weaknesses.IndexOf(t) != -1)
                    {
                        weaknesses.Remove(weaknesses.Find(x => x == t));
                    }
                }

                foreach (var r in resistances)
                {
                    typeChart = new TypeChart()
                    {
                        AttackId = r,
                        DefendId = typeId,
                        Effective = 0.5m,
                        GenerationId = genId,
                    };
                    this.dataService.AddObject(typeChart);
                }

                foreach (var w in weaknesses)
                {
                    typeChart = new TypeChart()
                    {
                        AttackId = w,
                        DefendId = typeId,
                        Effective = 2m,
                        GenerationId = genId,
                    };
                    this.dataService.AddObject(typeChart);
                }

                foreach (var i in immunities)
                {
                    typeChart = new TypeChart()
                    {
                        AttackId = i,
                        DefendId = typeId,
                        Effective = 0m,
                        GenerationId = genId,
                    };
                    this.dataService.AddObject(typeChart);
                }

                foreach (var t in existingCharts)
                {
                    this.dataService.DeleteObject<TypeChart>(t.Id);
                }

                return this.Json(this.Url.Action("Types", "Admin")).Value.ToString();
            }
            else
            {
                this.RedirectToAction("Home", "Index");
            }

            return null;
        }

        [AllowAnonymous]
        [Route("delete-pokemon-teams")]
        public string DeletePokemonTeams(List<int> teamIds)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<PokemonTeam> pokemonTeams = this.dataService.GetPokemonTeamsByUserId(this.dataService.GetObjectByPropertyValue<User>("Username", this.User.Identity.Name).Id);
                pokemonTeams = pokemonTeams.Where(x => teamIds.Any(y => y == x.Id)).ToList();

                foreach (var t in pokemonTeams)
                {
                    this.dataService.DeletePokemonTeam(t.Id);
                }

                return this.Json(this.Url.Action("PokemonTeams", "User")).Value.ToString();
            }
            else
            {
                this.RedirectToAction("Home", "Index");
            }

            return null;
        }

        [AllowAnonymous]
        [Route("update-pokemon-game-availability")]
        public string UpdatePokemonGameAvailability(int pokemonId, List<int> games)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                PokemonGameDetail pokemonGameDetail;
                List<PokemonGameDetail> existingGameDetails = this.dataService.GetPokemonGameDetails(pokemonId);

                foreach (var g in games)
                {
                    pokemonGameDetail = new PokemonGameDetail()
                    {
                        PokemonId = pokemonId,
                        GameId = g,
                    };
                    this.dataService.AddObject(pokemonGameDetail);
                }

                foreach (var g in existingGameDetails)
                {
                    this.dataService.DeleteObject<PokemonGameDetail>(g.Id);
                }

                return this.Json(this.Url.Action("Pokemon", "Admin")).Value.ToString();
            }
            else
            {
                this.RedirectToAction("Home", "Index");
            }

            return null;
        }

        [AllowAnonymous]
        [Route("update-game-availability")]
        public string UpdateGameAvailability(int gameId, List<int> pokemonList)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<PokemonGameDetail> existingGameDetails = new List<PokemonGameDetail>();
                List<PokemonGameDetail> newGameDetails = new List<PokemonGameDetail>();
                List<Game> games = this.dataService.GetObjects<Game>("ReleaseDate, Id");
                DateTime releaseDate = games.Find(x => x.Id == gameId).ReleaseDate;
                games = games.Where(x => x.ReleaseDate == releaseDate).ToList();

                if (gameId == 4 || gameId == 5)
                {
                    games = games.Where(x => x.Id == gameId).ToList();
                }

                foreach (var game in games.ConvertAll(x => x.Id))
                {
                    existingGameDetails = this.dataService.GetPokemonGameDetailsByGame(game);

                    foreach (var p in pokemonList)
                    {
                        if (existingGameDetails.Find(x => x.PokemonId == p && x.GameId == game) == null)
                        {
                            newGameDetails.Add(new PokemonGameDetail()
                            {
                                GameId = game,
                                PokemonId = p,
                            });
                        }
                        else
                        {
                            existingGameDetails.Remove(existingGameDetails.Find(x => x.PokemonId == p && x.GameId == game));
                        }
                    }

                    foreach (var g in newGameDetails)
                    {
                        this.dataService.AddObject(g);
                    }

                    foreach (var g in existingGameDetails)
                    {
                        this.dataService.DeleteObject<PokemonGameDetail>(g.Id);
                    }
                }

                return this.Json(this.Url.Action("GameAvailability", "Home")).Value.ToString();
            }
            else
            {
                this.RedirectToAction("Home", "Index");
            }

            return null;
        }

        [AllowAnonymous]
        [Route("get-pokemon-battle-items")]
        public List<BattleItem> GetPokemonBattleItems(int pokemonId, int generationId)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<BattleItem> battleItems = new List<BattleItem>();
                List<FormItem> formItems = this.dataService.GetFormItems().Where(x => x.PokemonId == pokemonId).ToList();
                if (formItems.Count == 0)
                {
                    Generation generation = this.dataService.GetObjectByPropertyValue<Generation>("Id", generationId);
                    List<BattleItem> allBattleItems = this.dataService.GetObjects<BattleItem>("GenerationId, Name", "Generation, Pokemon");
                    battleItems.AddRange(allBattleItems.Where(x => !x.OnlyInThisGeneration && x.PokemonId == null).ToList());
                    if (generation != null)
                    {
                        battleItems = battleItems.Where(x => x.Generation.Id <= generation.Id).ToList();
                        if (allBattleItems.Where(x => x.OnlyInThisGeneration && x.GenerationId == generation.Id).ToList().Count > 0)
                        {
                            battleItems.AddRange(allBattleItems.Where(x => x.OnlyInThisGeneration && x.GenerationId == generation.Id).ToList());
                        }

                        if (allBattleItems.Where(x => x.PokemonId == pokemonId && x.Generation.Id <= generation.Id).ToList().Count > 0)
                        {
                            battleItems.AddRange(allBattleItems.Where(x => x.PokemonId == pokemonId && x.Generation.Id <= generation.Id).ToList());
                        }
                    }
                    else
                    {
                        if (allBattleItems.Where(x => x.PokemonId == pokemonId).ToList().Count > 0)
                        {
                            battleItems.AddRange(allBattleItems.Where(x => x.PokemonId == pokemonId).ToList());
                        }
                    }

                    battleItems = battleItems.OrderBy(x => x.Name).ToList();
                }

                return battleItems;
            }
            else
            {
                this.RedirectToAction("Home", "Index");
            }

            return null;
        }

        [AllowAnonymous]
        [Route("get-pokemon-genders")]
        public List<string> GetPokemonGenders(int pokemonId)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<string> genders = new List<string>();
                GenderRatio genderRatio = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", pokemonId, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth").GenderRatio;
                if (genderRatio.MaleRatio == 0 && genderRatio.FemaleRatio == 0)
                {
                    genders.Add("None");
                }
                else if (genderRatio.MaleRatio == 0)
                {
                    genders.Add("Female");
                }
                else if (genderRatio.FemaleRatio == 0)
                {
                    genders.Add("Male");
                }
                else
                {
                    genders.Add(string.Empty);
                    genders.Add("Male");
                    genders.Add("Female");
                }

                return genders;
            }
            else
            {
                this.RedirectToAction("Home", "Index");
            }

            return null;
        }

        [AllowAnonymous]
        [Route("get-typing-evaluator-chart")]
        public IActionResult GetTypingEvaluatorChart(int primaryTypeID, int secondaryTypeID, int generationID)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return this.PartialView("_FillTypeEvaluatorChart", this.GetTypeChartTyping(primaryTypeID, secondaryTypeID, generationID));
            }
            else
            {
                this.RedirectToAction("Home", "Index");
            }

            return null;
        }

        [AllowAnonymous]
        [Route("get-generations")]
        public TeamRandomizerListViewModel GetGenerations(int selectedGame)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                if (selectedGame != 0)
                {
                    List<Pokemon> altFormsList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form").ConvertAll(x => x.AltFormPokemon);
                    List<PokemonGameDetail> availablePokemon = this.dataService.GetPokemonGameDetailsByGame(selectedGame).Where(x => !altFormsList.Any(y => y.Id == x.PokemonId)).ToList();
                    List<Pokemon> allPokemon = this.dataService.GetAllPokemon().Where(x => availablePokemon.Any(y => y.PokemonId == x.Id)).ToList();
                    Generation selectedGen = this.dataService.GetObjectByPropertyValue<Game>("Id", selectedGame, "Generation").Generation;
                    List<Generation> generationList = this.dataService.GetObjects<Generation>().Where(x => x.Id <= selectedGen.Id).ToList();
                    List<DataAccess.Models.Type> typesList = this.dataService.GetObjects<DataAccess.Models.Type>("Name").Where(x => x.GenerationId <= selectedGen.Id).ToList();
                    List<LegendaryType> legendaryTypes = this.dataService.GetObjects<LegendaryType>("Type");
                    List<Generation> availableGenerations = new List<Generation>();
                    Game game = this.dataService.GetObjectByPropertyValue<Game>("Id", selectedGame);

                    foreach (var gen in generationList)
                    {
                        if (allPokemon.Where(x => x.Game.GenerationId == gen.Id).ToList().Count != 0)
                        {
                            availableGenerations.Add(gen);
                        }
                    }

                    if (game.ReleaseDate < new DateTime(2016, 11, 18))
                    {
                        legendaryTypes.Remove(legendaryTypes.Find(x => x.Type == "Ultra Beast"));
                    }

                    TeamRandomizerListViewModel model = new TeamRandomizerListViewModel()
                    {
                        AllGenerations = availableGenerations,
                        AllTypes = typesList,
                        AllLegendaryTypes = legendaryTypes,
                    };

                    return model;
                }
                else
                {
                    TeamRandomizerListViewModel model = new TeamRandomizerListViewModel()
                    {
                        AllGenerations = this.dataService.GetObjects<Generation>().Where(x => this.GetAllPokemonWithoutForms().Any(y => y.Game.GenerationId == x.Id)).ToList(),
                        AllTypes = this.dataService.GetObjects<DataAccess.Models.Type>("Name"),
                        AllLegendaryTypes = this.dataService.GetObjects<LegendaryType>("Type"),
                    };

                    return model;
                }
            }
            else
            {
                this.RedirectToAction("Home", "Index");
            }

            return null;
        }

        [AllowAnonymous]
        [Route("get-pokemon-by-typing")]
        public IActionResult GetPokemonByTyping(int primaryTypeID, int secondaryTypeID, int generationID)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<PokemonTypeDetail> typingList = this.GetAllPokemonWithSpecificTypes(primaryTypeID, secondaryTypeID, generationID);
                List<Pokemon> pokemonList = new List<Pokemon>();

                foreach (var p in typingList)
                {
                    if (this.dataService.CheckIfAltForm(p.PokemonId))
                    {
                        Pokemon pokemon = this.dataService.GetAltFormWithFormName(p.PokemonId);
                        pokemonList.Add(pokemon);
                    }
                    else
                    {
                        pokemonList.Add(p.Pokemon);
                    }
                }

                TypingEvaluatorViewModel model = new TypingEvaluatorViewModel()
                {
                    AllPokemonWithTypes = typingList,
                    AllPokemon = pokemonList,
                    AppConfig = this.appConfig,
                    GenerationId = generationID,
                };

                return this.PartialView("_FillTypingEvaluator", model);
            }
            else
            {
                this.RedirectToAction("Home", "Index");
            }

            return null;
        }

        [AllowAnonymous]
        [Route("get-pokemon-by-ability")]
        public IActionResult GetPokemonByAbility(int abilityID, int generationID)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                List<PokemonAbilityDetail> abilityList = this.GetPokemonByAbilityAndGeneration(abilityID, generationID);
                List<Pokemon> pokemonList = new List<Pokemon>();

                foreach (var p in abilityList)
                {
                    if (this.dataService.CheckIfAltForm(p.PokemonId))
                    {
                        Pokemon pokemon = this.dataService.GetAltFormWithFormName(p.PokemonId);
                        pokemonList.Add(pokemon);
                    }
                    else
                    {
                        pokemonList.Add(p.Pokemon);
                    }
                }

                AbilityEvaluatorPageViewModel model = new AbilityEvaluatorPageViewModel()
                {
                    AllPokemonWithAbility = abilityList ?? new List<PokemonAbilityDetail>(),
                    AllPokemon = pokemonList,
                    AppConfig = this.appConfig,
                    GenerationId = generationID,
                    Ability = this.dataService.GetObjectByPropertyValue<Ability>("Id", abilityID),
                };

                return this.PartialView("_FillAbilityEvaluator", model);
            }
            else
            {
                this.RedirectToAction("Home", "Index");
            }

            return null;
        }

        [AllowAnonymous]
        [Route("get-pokemon-by-egg-group")]
        public IActionResult GetPokemonByEggGroup(int pokemonId)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                PokemonEggGroupDetail searchedEggGroupDetails = this.dataService.GetObjects<PokemonEggGroupDetail>(includes: "Pokemon, PrimaryEggGroup, SecondaryEggGroup", whereProperty: "PokemonId", wherePropertyValue: pokemonId).Last();
                GenderRatio genderRatio = this.dataService.GetObjectByPropertyValue<GenderRatio>("Id", searchedEggGroupDetails.Pokemon.GenderRatioId);
                List<Pokemon> altFormsList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form").ConvertAll(x => x.AltFormPokemon);
                List<PokemonEggGroupDetail> eggGroupList = new List<PokemonEggGroupDetail>();
                List<Pokemon> pokemonList = new List<Pokemon>();
                List<Pokemon> originalPokemon = new List<Pokemon>();

                if (pokemonId == this.dataService.GetPokemon("Manaphy").Id || pokemonId == this.dataService.GetPokemon("Phione").Id || (genderRatio.MaleRatio == 0 && genderRatio.FemaleRatio == 0 && pokemonId != this.dataService.GetPokemon("Ditto").Id))
                {
                    eggGroupList.Add(this.dataService.GetObjectByPropertyValue<PokemonEggGroupDetail>("Pokemon.Name", "Ditto", "Pokemon, Pokemon.GenderRatio, PrimaryEggGroup, SecondaryEggGroup"));
                    pokemonList.Add(this.dataService.GetPokemon("Ditto"));
                }
                else if (pokemonId == this.dataService.GetPokemon("Ditto").Id)
                {
                    Pokemon pokemon;
                    eggGroupList = this.dataService.GetAllPokemonWithEggGroupsAndIncomplete();
                    List<PokemonEggGroupDetail> breedablePokemonList = this.dataService.GetAllBreedablePokemon();
                    eggGroupList = eggGroupList.Where(x => breedablePokemonList.Any(y => y.PokemonId == x.PokemonId)).OrderBy(x => x.Pokemon.PokedexNumber).ToList();
                    eggGroupList.Remove(eggGroupList.Find(x => x.PokemonId == pokemonId));

                    foreach (var p in eggGroupList)
                    {
                        if (altFormsList.Find(x => x.Id == p.PokemonId) != null)
                        {
                            pokemon = this.dataService.GetAltFormWithFormName(p.PokemonId);
                        }

                        pokemonList.Add(p.Pokemon);
                    }

                    pokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", pokemonId, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth");
                }
                else
                {
                    eggGroupList = this.GetAllPokemonWithSpecificEggGroups((int)searchedEggGroupDetails.PrimaryEggGroupId, searchedEggGroupDetails.SecondaryEggGroupId);
                    List<PokemonEggGroupDetail> breedablePokemonList = this.dataService.GetAllBreedablePokemon();
                    eggGroupList.Add(this.dataService.GetObjectByPropertyValue<PokemonEggGroupDetail>("Pokemon.Name", "Ditto", "Pokemon, Pokemon.GenderRatio, PrimaryEggGroup, SecondaryEggGroup"));
                    if (eggGroupList.Any(x => x.Pokemon.Name == "Manaphy"))
                    {
                        eggGroupList.Remove(eggGroupList.Find(x => x.Pokemon.Name == "Manaphy"));
                    }

                    if (eggGroupList.Any(x => x.Pokemon.Name == "Phione"))
                    {
                        eggGroupList.Remove(eggGroupList.Find(x => x.Pokemon.Name == "Phione"));
                    }

                    eggGroupList = eggGroupList.Where(x => breedablePokemonList.Any(y => y.PokemonId == x.PokemonId)).OrderBy(x => x.Pokemon.PokedexNumber).ToList();
                    originalPokemon = eggGroupList.ConvertAll(x => x.Pokemon);
                    List<PokemonEggGroupDetail> finalEggGroupList = new List<PokemonEggGroupDetail>(eggGroupList);

                    foreach (var p in eggGroupList)
                    {
                        if ((p.Pokemon.GenderRatio.MaleRatio == 0 && p.Pokemon.GenderRatio.FemaleRatio == 0) || (genderRatio.MaleRatio == 100 && p.Pokemon.GenderRatio.MaleRatio == 100) || (genderRatio.FemaleRatio == 100 && p.Pokemon.GenderRatio.FemaleRatio == 100))
                        {
                            finalEggGroupList.Remove(p);
                        }
                        else
                        {
                            if (altFormsList.Find(x => x.Id == p.PokemonId) != null)
                            {
                                p.Pokemon = this.dataService.GetAltFormWithFormName(p.PokemonId);
                            }

                            pokemonList.Add(p.Pokemon);
                        }
                    }

                    eggGroupList = finalEggGroupList;
                }

                (originalPokemon ??= eggGroupList.ConvertAll(x => x.Pokemon)).RemoveAll(x => !eggGroupList.Select(y => y.PokemonId).Contains(x.Id));

                List<EggGroup> pokemonEggGroupList = new List<EggGroup>
                {
                    searchedEggGroupDetails.PrimaryEggGroup,
                };

                if (searchedEggGroupDetails.SecondaryEggGroup != null)
                {
                    pokemonEggGroupList.Add(searchedEggGroupDetails.SecondaryEggGroup);
                }

                EggGroupEvaluatorViewModel model = new EggGroupEvaluatorViewModel()
                {
                    AllPokemonWithEggGroups = eggGroupList,
                    AllPokemon = pokemonList,
                    AllAltForms = altFormsList,
                    AllOriginalPokemon = originalPokemon.ToList(),
                    AppConfig = this.appConfig,
                    SearchedPokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", pokemonId, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth"),
                    PokemonEggGroups = pokemonEggGroupList,
                    GenerationId = this.dataService.GetObjects<Generation>().Last().Id,
                };

                return this.PartialView("_FillDayCareEvaluator", model);
            }
            else
            {
                this.RedirectToAction("Home", "Index");
            }

            return null;
        }

        [AllowAnonymous]
        [Route("get-types-by-generation")]
        public IActionResult GrabTypingEvaluatorTypes(int generationID)
        {
            if (generationID == 0)
            {
                generationID = this.dataService.GetObjects<Generation>().Last().Id;
            }

            List<Pokedex.DataAccess.Models.Type> model = this.dataService.GetObjects<DataAccess.Models.Type>("Name").Where(x => x.GenerationId <= generationID).ToList();
            return this.PartialView("_FillTypingEvaluatorTypes", model);
        }

        [AllowAnonymous]
        [Route("get-abilities-by-generation")]
        public IActionResult GrabAbilityEvaluatorAbilities(int generationID)
        {
            List<Ability> model = this.dataService.GetObjects<Ability>("Name").Where(x => x.GenerationId <= generationID).ToList();
            return this.PartialView("_FillAbilityEvaluatorAbilities", model);
        }

        [AllowAnonymous]
        [Route("get-month-by-year/{year:int}/{pokemonPageCheck}")]
        public IActionResult GrabMonthByYear(int year, string pokemonPageCheck)
        {
            List<PageStat> pageStats = this.dataService.GetObjects<PageStat>("VisitDate");
            if (pokemonPageCheck == "noPokemonPage")
            {
                pageStats = pageStats.Where(x => !x.Name.Contains("Pokemon Page -")).ToList();
            }
            else
            {
                pageStats = pageStats.Where(x => x.Name.Contains("Pokemon Page -")).ToList();
            }

            pageStats = pageStats.Where(x => Convert.ToInt16(x.VisitDate.ToString("yyyy")) <= year).ToList();
            List<string> model = pageStats.Select(x => x.VisitDate.ToString("MMMM")).Distinct().ToList();
            return this.PartialView("_FillMonthInYear", model);
        }

        [AllowAnonymous]
        [Route("get-day-by-month/{month}/{year:int}/{pokemonPageCheck}")]
        public IActionResult GrabDayByMonth(string month, int year, string pokemonPageCheck)
        {
            List<string> model = new List<string>();
            List<PageStat> pageStats = this.dataService.GetObjects<PageStat>("VisitDate").Where(x => Convert.ToInt16(x.VisitDate.ToString("yyyy")) <= year).ToList();
            pageStats = pageStats.Where(x => x.VisitDate.ToString("MMMM") == month).ToList();

            if (pokemonPageCheck == "noPokemonPage")
            {
                model = pageStats.Where(x => !x.Name.Contains("Pokemon Page -")).ToList().Select(x => x.VisitDate.ToString("dd")).Distinct().ToList();
            }
            else
            {
                model = pageStats.Where(x => x.Name.Contains("Pokemon Page -")).ToList().Select(x => x.VisitDate.ToString("dd")).Distinct().ToList();
            }

            return this.PartialView("_FillDayInMonth", model);
        }

        [AllowAnonymous]
        [Route("get-stats-by-date/{day:int}/{month}/{year:int}/{pokemonPageCheck}")]
        public IActionResult GrabStatsByDate(int day, string month, int year, string pokemonPageCheck)
        {
            List<PageStat> pageStats = this.dataService.GetObjects<PageStat>("VisitDate").Where(x => Convert.ToInt16(x.VisitDate.ToString("yyyy")) <= year).ToList();
            pageStats = pageStats.Where(x => x.VisitDate.ToString("MMMM") == month).ToList();
            pageStats = pageStats.Where(x => Convert.ToInt16(x.VisitDate.ToString("dd")) == day).ToList();

            if (pokemonPageCheck == "noPokemonPage")
            {
                pageStats = pageStats.Where(x => !x.Name.Contains("Pokemon Page -")).ToList();
                return this.PartialView("_FillStatsInDate", pageStats);
            }
            else
            {
                pageStats = pageStats.Where(x => x.Name.Contains("Pokemon Page -")).ToList();
                List<Pokemon> pokemonList = new List<Pokemon>();
                string altForm;
                Pokemon pokemon;
                foreach (var p in pageStats.Select(x => x.Name.Replace("Pokemon Page - ", string.Empty)).Distinct())
                {
                    if (p.Contains("(") && !p.Contains("Nidoran"))
                    {
                        altForm = p.Split("(")[1].Replace(")", string.Empty);
                        pokemon = this.dataService.GetPokemonFromNameAndFormName(p.Split(" (")[0], altForm);
                        pokemon.Name = string.Concat(pokemon.Name, " (", altForm, ")");
                        pokemonList.Add(pokemon);
                    }
                    else
                    {
                        pokemonList.Add(this.dataService.GetPokemon(p));
                    }
                }

                PokemonPageStatsViewModel model = new PokemonPageStatsViewModel()
                {
                    PokemonList = pokemonList.OrderBy(x => x.PokedexNumber).ThenBy(x => x.Id).ToList(),
                    PageStatList = pageStats,
                    AppConfig = this.appConfig,
                    Generation = this.dataService.GetObjects<Generation>().Last(),
                };

                return this.PartialView("_FillPokemonStatsInDate", model);
            }
        }

        /// <summary>
        /// Gets a list of all pokemon that are not alternate forms.
        /// </summary>
        /// <returns>Returns the list of original pokemon.</returns>
        public List<Pokemon> GetAllPokemonWithoutForms()
        {
            List<Pokemon> pokemonList = this.dataService.GetAllPokemon();
            List<Pokemon> altFormList = this.dataService.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form").ConvertAll(x => x.AltFormPokemon);
            return pokemonList.Where(x => !altFormList.Any(y => y.Id == x.Id)).ToList();
        }

        /// <summary>
        /// Gets the type chart for the given typing and generation.
        /// </summary>
        /// <param name="primaryTypeId">The id of the primary type.</param>
        /// <param name="secondaryTypeId">The id of the secondary type.</param>
        /// <param name="generationId">The id of the generation.</param>
        /// <returns>Returns the type effectiveness given the typing and generation.</returns>
        private TypeEffectivenessViewModel GetTypeChartTyping(int primaryTypeId, int secondaryTypeId, int generationId)
        {
            List<DataAccess.Models.Type> typeList = this.dataService.GetObjects<DataAccess.Models.Type>("Name");
            List<DataAccess.Models.Type> pokemonTypes = new List<DataAccess.Models.Type>();
            List<string> strongAgainst = new List<string>();
            List<string> superStrongAgainst = new List<string>();
            List<string> weakAgainst = new List<string>();
            List<string> superWeakAgainst = new List<string>();
            List<string> immuneTo = new List<string>();
            List<TypeChart> typeChart;
            string effectiveValue, attackType;

            pokemonTypes.Add(this.dataService.GetObjectByPropertyValue<DataAccess.Models.Type>("Id", primaryTypeId));

            if (secondaryTypeId != 0)
            {
                pokemonTypes.Add(this.dataService.GetObjectByPropertyValue<DataAccess.Models.Type>("Id", secondaryTypeId));
            }

            foreach (var type in pokemonTypes)
            {
                typeChart = this.dataService.GetObjects<TypeChart>(includes: "Attack, Defend", whereProperty: "Defend", wherePropertyValue: type);

                List<int> generations = typeChart.Select(x => x.GenerationId).Distinct().OrderByDescending(x => x).ToList();
                if (generationId != 0)
                {
                    typeChart = typeChart.Where(x => x.GenerationId == generations.First(x => x <= generationId)).ToList();
                }
                else
                {
                    typeChart = typeChart.Where(x => x.GenerationId == generations.Last()).ToList();
                }

                foreach (var t in typeList)
                {
                    if (typeChart.Exists(x => x.Attack.Name == t.Name))
                    {
                        attackType = t.Name;
                        effectiveValue = typeChart.Find(x => x.Attack.Name == attackType).Effective.ToString("0.####");
                        if (effectiveValue == "0")
                        {
                            strongAgainst.Remove(attackType);
                            weakAgainst.Remove(attackType);
                            immuneTo.Add(attackType);
                        }
                        else if (effectiveValue == "0.5" && immuneTo.Where(x => x == attackType).ToList().Count == 0)
                        {
                            if (strongAgainst.Exists(x => x == attackType))
                            {
                                strongAgainst.Remove(attackType);
                                superStrongAgainst.Add(string.Concat(attackType, " Quad"));
                            }
                            else if (weakAgainst.Exists(x => x == attackType))
                            {
                                weakAgainst.Remove(attackType);
                            }
                            else
                            {
                                strongAgainst.Add(attackType);
                            }
                        }
                        else if (effectiveValue == "2" && immuneTo.Where(x => x == attackType).ToList().Count == 0)
                        {
                            if (weakAgainst.Exists(x => x == attackType))
                            {
                                weakAgainst.Remove(attackType);
                                superWeakAgainst.Add(string.Concat(attackType, " Quad"));
                            }
                            else if (strongAgainst.Exists(x => x == attackType))
                            {
                                strongAgainst.Remove(attackType);
                            }
                            else
                            {
                                weakAgainst.Add(attackType);
                            }
                        }
                    }
                }
            }

            immuneTo.Sort();
            strongAgainst.Sort();
            superStrongAgainst.Sort();
            weakAgainst.Sort();
            superWeakAgainst.Sort();

            List<string> strongAgainstList = superStrongAgainst;
            strongAgainstList.AddRange(strongAgainst);

            List<string> weakAgainstList = superWeakAgainst;
            weakAgainstList.AddRange(weakAgainst);

            TypeEffectivenessViewModel effectivenessChart = new TypeEffectivenessViewModel()
            {
                ImmuneTo = immuneTo,
                StrongAgainst = strongAgainstList,
                WeakAgainst = weakAgainstList,
            };

            return effectivenessChart;
        }

        private List<PokemonEggGroupDetail> GetAllPokemonWithSpecificEggGroups(int primaryEggGroupId, int? secondaryEggGroupId)
        {
            List<PokemonEggGroupDetail> pokemonList = this.dataService.GetObjects<PokemonEggGroupDetail>(includes: "Pokemon, Pokemon.GenderRatio, PrimaryEggGroup, SecondaryEggGroup").OrderBy(x => x.GenerationId).GroupBy(x => new { x.PokemonId }).Select(x => x.LastOrDefault()).ToList();

            List<PokemonEggGroupDetail> finalPokemonList = pokemonList.Where(x => x.PrimaryEggGroupId == primaryEggGroupId || x.SecondaryEggGroupId == primaryEggGroupId).ToList();

            if (secondaryEggGroupId != null)
            {
                finalPokemonList.AddRange(pokemonList.Where(x => x.PrimaryEggGroupId == secondaryEggGroupId || x.SecondaryEggGroupId == secondaryEggGroupId).ToList());
            }

            return finalPokemonList.Distinct().ToList();
        }

        private List<PokemonAbilityDetail> GetPokemonByAbilityAndGeneration(int abilityId, int generationId)
        {
            List<PokemonAbilityDetail> pokemonList = this.dataService.GetObjects<PokemonAbilityDetail>(includes: "Pokemon, Pokemon.Game", whereProperty: "Pokemon.IsComplete", wherePropertyValue: true)
                .Where(x => x.Pokemon.Game.GenerationId <= generationId)
                .Where(x => x.GenerationId <= generationId)
                .OrderBy(x => x.GenerationId)
                .GroupBy(x => new { x.PokemonId })
                .Select(x => x.LastOrDefault())
                .ToList();

            List<int> exclusionList = pokemonList.Select(x => x.PokemonId).Except(this.dataService.GetObjects<PokemonGameDetail>(includes: "Pokemon, Game, Game.Generation", whereProperty: "Game.GenerationId", wherePropertyValue: generationId).Select(x => x.PokemonId)).ToList();

            foreach (var pokemon in exclusionList)
            {
                pokemonList.Remove(pokemonList.Find(x => x.PokemonId == pokemon));
            }

            return pokemonList.Where(x => x.PrimaryAbilityId == abilityId || x.SecondaryAbilityId == abilityId || x.HiddenAbilityId == abilityId || x.SpecialEventAbilityId == abilityId).OrderBy(x => x.Pokemon.PokedexNumber).ThenBy(x => x.PokemonId).ToList();
        }

        private string FillEVs(PokemonTeamEV evs)
        {
            string evString = string.Empty;
            if (evs.EVTotal == 0)
            {
                return "\nEVs: 1 HP / 1 Atk / 1 Def / 1 SpA / 1 SpD / 1 Spe";
            }
            else
            {
                if (evs.Health > 0)
                {
                    evString = string.Concat(evString, evs.Health.ToString(), " HP");
                }

                if (evs.Attack > 0)
                {
                    if (!string.IsNullOrEmpty(evString))
                    {
                        evString = string.Concat(evString, " / ");
                    }

                    evString = string.Concat(evString, evs.Attack.ToString(), " Atk");
                }

                if (evs.Defense > 0)
                {
                    if (!string.IsNullOrEmpty(evString))
                    {
                        evString = string.Concat(evString, " / ");
                    }

                    evString = string.Concat(evString, evs.Defense.ToString(), " Def");
                }

                if (evs.SpecialAttack > 0)
                {
                    if (!string.IsNullOrEmpty(evString))
                    {
                        evString = string.Concat(evString, " / ");
                    }

                    evString = string.Concat(evString, evs.SpecialAttack.ToString(), " SpA");
                }

                if (evs.SpecialDefense > 0)
                {
                    if (!string.IsNullOrEmpty(evString))
                    {
                        evString = string.Concat(evString, " / ");
                    }

                    evString = string.Concat(evString, evs.SpecialDefense.ToString(), " SpD");
                }

                if (evs.Speed > 0)
                {
                    if (!string.IsNullOrEmpty(evString))
                    {
                        evString = string.Concat(evString, " / ");
                    }

                    evString = string.Concat(evString, evs.Speed.ToString(), " Spe");
                }

                return string.Concat("\nEVs: ", evString);
            }
        }

        private string FillIVs(PokemonTeamIV ivs)
        {
            string ivString = string.Empty;
            if (ivs.Health < 31)
            {
                ivString = string.Concat(ivString, ivs.Health.ToString(), " HP");
            }

            if (ivs.Attack < 31)
            {
                if (!string.IsNullOrEmpty(ivString))
                {
                    ivString = string.Concat(ivString, " / ");
                }

                ivString = string.Concat(ivString, ivs.Attack.ToString(), " Atk");
            }

            if (ivs.Defense < 31)
            {
                if (!string.IsNullOrEmpty(ivString))
                {
                    ivString = string.Concat(ivString, " / ");
                }

                ivString = string.Concat(ivString, ivs.Defense.ToString(), " Def");
            }

            if (ivs.SpecialAttack < 31)
            {
                if (!string.IsNullOrEmpty(ivString))
                {
                    ivString = string.Concat(ivString, " / ");
                }

                ivString = string.Concat(ivString, ivs.SpecialAttack.ToString(), " SpA");
            }

            if (ivs.SpecialDefense < 31)
            {
                if (!string.IsNullOrEmpty(ivString))
                {
                    ivString = string.Concat(ivString, " / ");
                }

                ivString = string.Concat(ivString, ivs.SpecialDefense.ToString(), " SpD");
            }

            if (ivs.Speed < 31)
            {
                if (!string.IsNullOrEmpty(ivString))
                {
                    ivString = string.Concat(ivString, " / ");
                }

                ivString = string.Concat(ivString, ivs.Speed.ToString(), " Spe");
            }

            if (!string.IsNullOrEmpty(ivString))
            {
                ivString = string.Concat("\nIVs: ", ivString);
            }

            return ivString;
        }

        private string FillMoveset(PokemonTeamMoveset moveset)
        {
            string movesetString = string.Empty;
            if (!string.IsNullOrEmpty(moveset.FirstMove))
            {
                movesetString = string.Concat(movesetString, "\n- ", moveset.FirstMove);
            }

            if (!string.IsNullOrEmpty(moveset.SecondMove))
            {
                movesetString = string.Concat(movesetString, "\n- ", moveset.SecondMove);
            }

            if (!string.IsNullOrEmpty(moveset.ThirdMove))
            {
                movesetString = string.Concat(movesetString, "\n- ", moveset.ThirdMove);
            }

            if (!string.IsNullOrEmpty(moveset.FourthMove))
            {
                movesetString = string.Concat(movesetString, "\n- ", moveset.FourthMove);
            }

            return movesetString;
        }

        private List<string> GetUserFormDetails(int pokemonId)
        {
            string form = string.Empty, itemName = string.Empty;
            List<string> formDetails = new List<string>();
            PokemonFormDetail pokemonFormDetail = this.dataService.GetObjectByPropertyValue<PokemonFormDetail>("AltFormPokemonId", pokemonId, "AltFormPokemon, OriginalPokemon, Form");

            form = string.Concat(form, pokemonFormDetail.Form.Name.Replace(' ', '-').Replace("Gigantamax", "Gmax"));

            List<FormItem> formItems = this.dataService.GetObjects<FormItem>();
            FormItem formItem = formItems.Find(x => x.PokemonId == pokemonId);
            if (formItem != null)
            {
                itemName = string.Concat(" @ ", formItem.Name);
            }

            formDetails.Add(form);
            if (!string.IsNullOrEmpty(itemName))
            {
                formDetails.Add(itemName);
            }

            return formDetails;
        }

        private string ExportPokemonTeam(List<int> pokemonIdList, List<string> abilityList, bool exportAbilities)
        {
            if (this.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                string pokemonTeam = string.Empty;
                Pokemon pokemon;

                for (var i = 0; i < pokemonIdList.Count; i++)
                {
                    if (i != 0)
                    {
                        pokemonTeam = string.Concat(pokemonTeam, "\n\n");
                    }

                    pokemon = this.dataService.GetObjectByPropertyValue<Pokemon>("Id", pokemonIdList[i], "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth");
                    string pokemonName = pokemon.Name;
                    if (this.dataService.CheckIfAltForm(pokemon.Id))
                    {
                        string pokemonForm = this.GetFormDetails(pokemon.Id);
                        pokemonName = string.Concat(pokemonName, "-", pokemonForm);
                    }

                    pokemonTeam = string.Concat(pokemonTeam, pokemonName);
                    if (exportAbilities)
                    {
                        pokemonTeam = string.Concat(pokemonTeam, "\nAbility: ", abilityList[i]);
                    }

                    pokemonTeam = string.Concat(pokemonTeam, "\nEVs: 1 HP / 1 Atk / 1 Def / 1 SpA / 1 SpD / 1 Spe");
                }

                return pokemonTeam;
            }
            else
            {
                this.RedirectToAction("Home", "Index");
            }

            return null;
        }

        private string GetFormDetails(int pokemonId)
        {
            string formDetails = string.Empty, itemName = string.Empty;
            PokemonFormDetail pokemonFormDetail = this.dataService.GetObjectByPropertyValue<PokemonFormDetail>("AltFormPokemonId", pokemonId, "AltFormPokemon, OriginalPokemon, Form");

            formDetails = string.Concat(formDetails, pokemonFormDetail.Form.Name.Replace(' ', '-').Replace("Gigantamax", "Gmax"));

            List<FormItem> formItems = this.dataService.GetObjects<FormItem>();
            FormItem formItem = formItems.Find(x => x.PokemonId == pokemonId);
            if (formItem != null)
            {
                itemName = formItem.Name;
            }

            if (!string.IsNullOrEmpty(itemName))
            {
                formDetails = string.Concat(formDetails, " @ ", itemName);
            }

            return formDetails;
        }

        private List<Pokemon> RemoveExtraPokemonForms(List<Pokemon> pokemonList)
        {
            Random rnd = new Random();
            List<Pokemon> pumpkabooCount = pokemonList.Where(x => x.PokedexNumber == 710).ToList();
            while (pumpkabooCount.Count > 1)
            {
                pokemonList.Remove(pumpkabooCount[rnd.Next(pumpkabooCount.Count)]);
                pumpkabooCount = pokemonList.Where(x => x.PokedexNumber == 710).ToList();
            }

            List<Pokemon> gourgeistCount = pokemonList.Where(x => x.PokedexNumber == 711).ToList();
            while (gourgeistCount.Count > 1)
            {
                pokemonList.Remove(gourgeistCount[rnd.Next(gourgeistCount.Count)]);
                gourgeistCount = pokemonList.Where(x => x.PokedexNumber == 711).ToList();
            }

            List<Pokemon> starterForms = this.dataService.GetObjects<PokemonFormDetail>(includes: "OriginalPokemon, AltFormPokemon, Form", whereProperty: "Form.Name", wherePropertyValue: "Starter").ConvertAll(x => x.AltFormPokemon);
            List<Pokemon> lgpeStarterCount = pokemonList.Where(x => starterForms.Any(y => y.Id == x.Id)).ToList();
            while (lgpeStarterCount.Count > 1)
            {
                pokemonList.Remove(lgpeStarterCount[rnd.Next(lgpeStarterCount.Count)]);
                lgpeStarterCount = pokemonList.Where(x => starterForms.Any(y => y.Id == x.Id)).ToList();
            }

            return pokemonList;
        }

        private List<PokemonTypeDetail> GetAllPokemonWithSpecificTypes(int primaryTypeId, int secondaryTypeId, int generationId)
        {
            List<PokemonTypeDetail> pokemonList = this.dataService.GetObjects<PokemonTypeDetail>("GenerationId", "Pokemon, Pokemon.Game, PrimaryType, SecondaryType", "Pokemon.IsComplete", true)
                                                        .GroupBy(x => new { x.PokemonId })
                                                        .Select(x => x.LastOrDefault())
                                                        .ToList();

            if (secondaryTypeId != 0 && secondaryTypeId != 100)
            {
                pokemonList = pokemonList.Where(x => (x.PrimaryTypeId == primaryTypeId && x.SecondaryTypeId == secondaryTypeId) || (x.PrimaryTypeId == secondaryTypeId && x.SecondaryTypeId == primaryTypeId)).ToList();
            }
            else if (secondaryTypeId == 100)
            {
                pokemonList = pokemonList.Where(x => x.PrimaryTypeId == primaryTypeId || x.SecondaryTypeId == primaryTypeId).ToList();
            }
            else
            {
                pokemonList = pokemonList.Where(x => x.PrimaryTypeId == primaryTypeId && x.SecondaryType == null).ToList();
            }

            if (generationId != 0)
            {
                pokemonList = pokemonList.Where(x => x.Pokemon.Game.GenerationId == generationId).Where(x => x.GenerationId <= generationId).ToList();

                List<int> exclusionList = pokemonList.Select(x => x.PokemonId).Except(this.dataService.GetObjects<PokemonGameDetail>(includes: "Pokemon, Game, Game.Generation", whereProperty: "Game.GenerationId", wherePropertyValue: generationId).Select(x => x.PokemonId)).ToList();

                foreach (var pokemon in exclusionList)
                {
                    pokemonList.Remove(pokemonList.Find(x => x.PokemonId == pokemon));
                }
            }

            return pokemonList.OrderBy(x => x.Pokemon.PokedexNumber).ToList();
        }

        private List<PokemonTypeDetail> GetAllPokemonWithSpecificType(int typeId, int generationId, List<Pokemon> allPokemon)
        {
            List<PokemonTypeDetail> pokemonList = this.dataService.GetObjects<PokemonTypeDetail>(includes: "Pokemon, Pokemon.Game, PrimaryType, SecondaryType", whereProperty: "Pokemon.IsComplete", wherePropertyValue: true)
                                                        .Where(x => allPokemon.Any(y => y.Id == x.PokemonId))
                                                        .OrderBy(x => x.GenerationId)
                                                        .ToList();

            if (generationId != 0)
            {
                pokemonList = pokemonList.Where(x => x.GenerationId <= generationId).ToList();

                List<int> exclusionList = pokemonList.Select(x => x.PokemonId).Except(this.dataService.GetObjects<PokemonGameDetail>(includes: "Pokemon, Game, Game.Generation", whereProperty: "Game.GenerationId", wherePropertyValue: generationId).Select(x => x.PokemonId)).ToList();

                foreach (var pokemon in exclusionList)
                {
                    pokemonList.Remove(pokemonList.Find(x => x.PokemonId == pokemon));
                }
            }

            pokemonList = pokemonList.GroupBy(x => new { x.PokemonId }).Select(x => x.LastOrDefault()).Where(x => x.PrimaryTypeId == typeId || x.SecondaryTypeId == typeId).ToList();

            return pokemonList.OrderBy(x => x.Pokemon.PokedexNumber).ToList();
        }
    }
}
