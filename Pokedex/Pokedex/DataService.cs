using ImageMagick;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using Pokedex.DataAccess.Models;
using Pokedex.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Net.Mail;

namespace Pokedex
{
    /// <summary>
    /// The class that is used to represent methods to grab, modify, or delete data from the DataContext database.
    /// </summary>
    public class DataService
    {
        private readonly DataContext dataContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataService"/> class.
        /// </summary>
        /// <param name="dataContext">The data context for the main database.</param>
        public DataService(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        /// <summary>
        /// Records a page's view.
        /// </summary>
        /// <param name="pageName">The name of the page the view is being counted for.</param>
        /// <param name="isOwner">A check to ensure the user is not the owner.</param>
        public void AddPageView(string pageName, bool isOwner)
        {
            if (!isOwner)
            {
                PageStat pageStat = this.GetObjects<PageStat>().Find(x => x.Name == pageName);

                this.AddObject(new PageStat() { Name = pageName, VisitTime = System.DateTime.Now.ToUniversalTime(), VisitDate = System.DateTime.Now.ToUniversalTime() });
            }
        }

        /// <summary>
        /// Returns a list of objects from the passed-through TEntity class.
        /// </summary>
        /// <typeparam name="TEntity">The generic type parameter.</typeparam>
        /// <param name="orderedProperty">The property that is used if an order is needed. Separate properties by commas for multiples. I.E. "Id, Name".</param>
        /// <param name="includes">The property that is used if an include is needed. Separate includes by commas for multiples. I.E. "Game, Ability".</param>
        /// <param name="whereProperty">The property the object will be searched for.</param>
        /// <param name="wherePropertyValue">The property's value.</param>
        /// <returns>Returns the list of the class requested.</returns>
        public List<TEntity> GetObjects<TEntity>(string orderedProperty = "", string includes = "", string whereProperty = "", object wherePropertyValue = null)
            where TEntity : class
        {
            IQueryable<TEntity> objects = this.dataContext.Set<TEntity>();

            if (!string.IsNullOrEmpty(includes))
            {
                foreach (var i in includes.Split(", "))
                {
                    objects = objects.Include(i);
                }
            }

            if (!string.IsNullOrEmpty(orderedProperty))
            {
                objects = objects.OrderBy(orderedProperty);
            }

            if (!string.IsNullOrEmpty(whereProperty) && wherePropertyValue != null)
            {
                string searchString = string.Concat("x => x.", whereProperty, " == ");

                if (wherePropertyValue is string)
                {
                    searchString = string.Concat(searchString, '"', wherePropertyValue, '"');
                }
                else
                {
                    searchString = string.Concat(searchString, wherePropertyValue);
                }

                try
                {
                    return objects.Where(searchString).ToList();
                }
                catch
                {
                    return objects.ToList();
                }
            }
            else
            {
                return objects.ToList();
            }
        }

        /// <summary>
        /// Returns an object based off the passed-through TEntity class and the variable we are finding.
        /// </summary>
        /// <typeparam name="TEntity">The generic type parameter.</typeparam>
        /// <param name="property">The property the object will be searched for.</param>
        /// <param name="propertyValue">The property's value.</param>
        /// <param name="includes">The property that is used if an include is needed. Separate includes by commas for multiples. I.E. "Game, Ability".</param>
        /// <returns>Returns the object with the correct class and id.</returns>
        public TEntity GetObjectByPropertyValue<TEntity>(string property, object propertyValue, string includes = "")
            where TEntity : class
        {
            IQueryable<TEntity> objects = this.dataContext.Set<TEntity>();

            if (!string.IsNullOrEmpty(includes))
            {
                foreach (var i in includes.Split(", "))
                {
                    objects = objects.Include(i);
                }
            }

            string searchString = string.Concat("x => x.", property, " == ");
            if (propertyValue is string)
            {
                searchString = string.Concat(searchString, '"', propertyValue, '"');
            }
            else
            {
                searchString = string.Concat(searchString, propertyValue);
            }

            try
            {
                return objects.First(searchString);
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// Creates an entity in the database and saves the changes.
        /// </summary>
        /// <typeparam name="TEntity">The generic type parameter.</typeparam>
        /// <param name="entity">The entity being added.</param>
        public void AddObject<TEntity>(TEntity entity)
            where TEntity : class
        {
            this.dataContext.Set<TEntity>().Add(entity);
            this.dataContext.SaveChanges();
        }

        /// <summary>
        /// Updates an entity in the database and saves the changes.
        /// </summary>
        /// <typeparam name="TEntity">The generic type parameter.</typeparam>
        /// <param name="entity">The updated entity.</param>
        public void UpdateObject<TEntity>(TEntity entity)
            where TEntity : class
        {
            try
            {
                this.dataContext.Set<TEntity>().Update(entity);
                this.dataContext.SaveChanges();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Delets an entity in the database and saves the changes.
        /// </summary>
        /// <typeparam name="TEntity">The generic type parameter.</typeparam>
        /// <param name="id">The id of the entity to be deleted.</param>
        public void DeleteObject<TEntity>(int id)
            where TEntity : class
        {
            var entity = this.GetObjectByPropertyValue<TEntity>("Id", id);
            this.dataContext.Set<TEntity>().Remove(entity);
            this.dataContext.SaveChanges();
        }

        /// <summary>
        /// Checks to see if the provided pokemon id is an alternate form of another pokemon.
        /// </summary>
        /// <param name="id">The pokemon's id.</param>
        /// <returns>Returns whether the pokemon is an alternate form or not.</returns>
        public bool CheckIfAltForm(int id)
        {
            return this.GetObjects<PokemonFormDetail>().Exists(x => x.AltFormPokemonId == id);
        }

        /// <summary>
        /// Gets the list of form items with form names added to it.
        /// </summary>
        /// <returns>The list of formatted form items.</returns>
        public List<FormItem> GetFormItems()
        {
            List<FormItem> formItemList = this.GetObjects<FormItem>("Pokemon.PokedexNumber", "Pokemon");

            foreach (var f in formItemList)
            {
                f.Pokemon.Name = string.Concat(f.Pokemon.Name, " (", this.GetPokemonFormName(f.PokemonId), ")");
            }

            return formItemList;
        }

        /// <summary>
        /// Gets the list of pre-evolutions with form names added to it.
        /// </summary>
        /// <param name="pokemonId">The id of the evolved pokemon.</param>
        /// <returns>The list of formatted pre evolutions.</returns>
        public List<Evolution> GetPreEvolution(int pokemonId)
        {
            List<Evolution> preEvolution = this.GetObjects<Evolution>(includes: "PreevolutionPokemon, PreevolutionPokemon.Game, EvolutionPokemon, EvolutionPokemon.Game, EvolutionMethod", whereProperty: "EvolutionPokemon.Id", wherePropertyValue: pokemonId);

            foreach (var p in preEvolution)
            {
                if (this.CheckIfAltForm(p.PreevolutionPokemonId))
                {
                    p.PreevolutionPokemon.Name = string.Concat(p.PreevolutionPokemon.Name, " (", this.GetPokemonFormName(p.PreevolutionPokemonId), ")");
                }
            }

            return preEvolution;
        }

        /// <summary>
        /// Gets all of the evolutions available for a pokemon by their id.
        /// </summary>
        /// <param name="pokemonId">The preevolution's id.</param>
        /// <returns>The list of all available evolutions.</returns>
        public List<Evolution> GetPokemonEvolutions(int pokemonId)
        {
            List<Evolution> evolutions = this.GetObjects<Evolution>("EvolutionPokemon.PokedexNumber, EvolutionPokemon.Id",  "PreevolutionPokemon, PreevolutionPokemon.Game, EvolutionPokemon, EvolutionPokemon.Game, EvolutionMethod", "PreevolutionPokemon.Id", pokemonId);

            foreach (var e in evolutions)
            {
                if (this.CheckIfAltForm(e.EvolutionPokemonId))
                {
                    e.EvolutionPokemon.Name = string.Concat(e.EvolutionPokemon.Name, " (", this.GetPokemonFormName(e.EvolutionPokemonId), ")");
                }
            }

            return evolutions;
        }

        /// <summary>
        /// Finds the name of the alternate form given the pokemon id.
        /// </summary>
        /// <param name="pokemonId">The pokemon's id.</param>
        /// <returns>Returns the name of the alternate form.</returns>
        public string GetPokemonFormName(int pokemonId)
        {
            return this.GetObjects<PokemonFormDetail>(includes: "Form")
                .FirstOrDefault(x => x.AltFormPokemonId == pokemonId).Form.Name;
        }

        /// <summary>
        /// Gets the pokemon by the name.
        /// </summary>
        /// <param name="name">The name of the pokemon.</param>
        /// <returns>Returns the requested pokemon.</returns>
        public Pokemon GetPokemon(string name)
        {
            return this.GetObjectByPropertyValue<Pokemon>("Name", name);
        }

        /// <summary>
        /// Gets the pokemon from a name. This pokemon will be an alternate form and will have the form name be used in the process of searching for it.
        /// </summary>
        /// <param name="pokemonName">The name of the pokemon.</param>
        /// <param name="formId">The id of the alternate form.</param>
        /// <returns>Returns the alternate pokemon.</returns>
        public Pokemon GetPokemonFromNameAndFormName(string pokemonName, int formId)
        {
            return this.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, OriginalPokemon, Form", whereProperty: "FormId", wherePropertyValue: formId).FirstOrDefault(x => x.AltFormPokemon.Name == pokemonName).AltFormPokemon;
        }

        /// <summary>
        /// Gets all of the details required for admin actions.
        /// </summary>
        /// <returns>Returns all the details required for admins.</returns>
        public AllAdminPokemonViewModel GetAllAdminPokemonDetails()
        {
            return new AllAdminPokemonViewModel()
            {
                AllAltForms = this.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form"),
                AllEvolutions = this.GetObjects<Evolution>(includes: "PreevolutionPokemon, PreevolutionPokemon.Game, EvolutionPokemon, EvolutionPokemon.Game, EvolutionMethod, Generation"),
                AllTypings = this.GetObjects<PokemonTypeDetail>("PokemonId", "Pokemon, PrimaryType, SecondaryType"),
                AllAbilities = this.GetObjects<PokemonAbilityDetail>(includes: "Pokemon, PrimaryAbility, SecondaryAbility, HiddenAbility, SpecialEventAbility"),
                AllEggGroups = this.GetObjects<PokemonEggGroupDetail>(includes: "Pokemon, PrimaryEggGroup, SecondaryEggGroup"),
                AllBaseStats = this.GetObjects<BaseStat>(includes: "Pokemon"),
                AllEVYields = this.GetObjects<EVYield>(includes: "Pokemon"),
                AllLegendaryDetails = this.GetObjects<PokemonLegendaryDetail>(includes: "Pokemon, LegendaryType"),
                AllPokemonCaptureRates = this.GetAllPokemonWithCaptureRates(),
                AllPokemonBaseHappinesses = this.GetAllPokemonWithBaseHappinesses(),
            };
        }

        /// <summary>
        /// Gets a list of all pokemon.
        /// </summary>
        /// <returns>Returns the list of all pokemon.</returns>
        public List<Pokemon> GetAllPokemon()
        {
            return this.GetObjects<Pokemon>("PokedexNumber, Id", "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth");
        }

        /// <summary>
        /// Gets all of the details for a pokemon.
        /// </summary>
        /// <param name="pokemon">The pokemon needing their details.</param>
        /// <param name="appConfig">The application's configuration.</param>
        /// <param name="form">The alternate form if it applies.</param>
        /// <returns>Returns the pokemon's details.</returns>
        public PokemonViewModel GetPokemonDetails(Pokemon pokemon, AppConfig appConfig, Form form = null)
        {
            List<Game> games = this.GetObjects<PokemonGameDetail>("Game.ReleaseDate, GameId, Id", "Pokemon, Pokemon.Game, Game", "PokemonId", pokemon.Id).ConvertAll(x => x.Game);
            games.Remove(games.Find(x => x.Id == 43));

            PokemonViewModel pokemonViewModel = new PokemonViewModel()
            {
                Pokemon = pokemon,
                BaseHappinesses = this.GetObjects<PokemonBaseHappinessDetail>(includes: "BaseHappiness", whereProperty: "PokemonId", wherePropertyValue: pokemon.Id).OrderByDescending(x => x.GenerationId).ToList(),
                BaseStats = this.GetObjects<BaseStat>(includes: "Pokemon", whereProperty: "Pokemon.Id", wherePropertyValue: pokemon.Id),
                EVYields = this.GetObjects<EVYield>(includes: "Pokemon", whereProperty: "Pokemon.Id", wherePropertyValue: pokemon.Id),
                Typings = this.GetObjects<PokemonTypeDetail>(includes: "Pokemon, PrimaryType, SecondaryType, Generation", whereProperty: "PokemonId", wherePropertyValue: pokemon.Id),
                Abilities = this.GetObjects<PokemonAbilityDetail>(includes: "Pokemon, PrimaryAbility, SecondaryAbility, HiddenAbility, SpecialEventAbility", whereProperty: "PokemonId", wherePropertyValue: pokemon.Id),
                EggGroups = this.GetObjects<PokemonEggGroupDetail>(includes: "Pokemon, PrimaryEggGroup, SecondaryEggGroup", whereProperty: "PokemonId", wherePropertyValue: pokemon.Id),
                CaptureRates = this.GetPokemonWithCaptureRates(pokemon.Id),
                PreEvolutions = this.GetPreEvolution(pokemon.Id),
                Evolutions = this.GetPokemonEvolutions(pokemon.Id),
                Effectiveness = this.GetTypeChartPokemon(pokemon.Id),
                GamesAvailableIn = games,
                AppConfig = appConfig,
            };

            if (form != null)
            {
                pokemonViewModel.Form = form;
                pokemonViewModel.Pokemon.Name = string.Concat(pokemonViewModel.Pokemon.Name, " (", form.Name, ")");
            }

            HttpWebRequest webRequest;
            HttpWebResponse imageRequest;
            try
            {
                webRequest = (HttpWebRequest)HttpWebRequest.Create(string.Concat(appConfig.WebUrl, appConfig.ShinyPokemonImageUrl, pokemon.Id, ".png"));
                imageRequest = (HttpWebResponse)webRequest.GetResponse();
                if (imageRequest.StatusCode == HttpStatusCode.OK)
                {
                    pokemonViewModel.HasShiny = true;
                }
                else
                {
                    pokemonViewModel.HasShiny = false;
                }
            }
            catch
            {
                pokemonViewModel.HasShiny = false;
            }

            try
            {
                webRequest = (HttpWebRequest)HttpWebRequest.Create(string.Concat(appConfig.WebUrl, appConfig.HomePokemonImageUrl, pokemon.Id, ".png"));
                imageRequest = (HttpWebResponse)webRequest.GetResponse();
                if (imageRequest.StatusCode == HttpStatusCode.OK)
                {
                    pokemonViewModel.HasHome = true;
                }
                else
                {
                    pokemonViewModel.HasHome = false;
                }
            }
            catch
            {
                pokemonViewModel.HasHome = false;
            }

            PokemonLegendaryDetail legendaryType = this.GetObjectByPropertyValue<PokemonLegendaryDetail>("PokemonId", pokemon.Id, "LegendaryType");

            if (legendaryType != null)
            {
                pokemonViewModel.LegendaryType = legendaryType.LegendaryType;
            }

            return pokemonViewModel;
        }

        /// <summary>
        /// Gets a list of all pokemon with form names for alternate forms.
        /// </summary>
        /// <param name="gameId">The game's id. Optional.</param>
        /// <param name="noBattleOnlyForms">Whether or not battle only forms will be excluded. Optional.</param>
        /// <returns>Returns the list of all pokemon.</returns>
        public List<Pokemon> GetPokemonWithFormNames(int gameId = 0, bool noBattleOnlyForms = false)
        {
            List<Pokemon> pokemonList = new List<Pokemon>();
            Game game = new Game();
            if (gameId != 0)
            {
                pokemonList = this.GetObjects<PokemonGameDetail>(includes: "Pokemon, Pokemon.GenderRatio, Pokemon.Game", whereProperty: "GameId", wherePropertyValue: gameId).ConvertAll(x => x.Pokemon).ToList();
                game = this.GetObjectByPropertyValue<Game>("Id", gameId);
            }
            else
            {
                pokemonList = this.GetObjects<Pokemon>(includes: "GenderRatio, Game");
            }

            List<PokemonFormDetail> formDetails = this.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, Form");
            if (noBattleOnlyForms)
            {
                pokemonList = pokemonList.Where(x => !formDetails.Where(x => x.Form.OnlyDuringBattle || x.Form.FusionForm).Select(x => x.AltFormPokemon).Any(y => y.Id == x.Id)).ToList();
            }

            pokemonList.Where(x => formDetails.ConvertAll(x => x.AltFormPokemon).Any(y => y.Id == x.Id)).ToList().ForEach(x => x.Name = this.GetAltFormWithFormName(x.Id).Name);

            return pokemonList.OrderBy(x => x.PokedexNumber).ThenBy(x => x.Id).ToList();
        }

        /// <summary>
        /// Gets a list of all pokemon.
        /// </summary>
        /// <param name="gameId">The game's id. Optional.</param>
        /// <returns>Returns the list of all pokemon.</returns>
        public List<Pokemon> GetNonBattlePokemonWithFormNames(int gameId = 0)
        {
            List<Pokemon> pokemonList = this.GetPokemonWithFormNames(gameId, true);

            if (gameId != 0)
            {
                // Gets future evolutions that are possible.
                Game game = this.GetObjectByPropertyValue<Game>("Id", gameId);
                List<Evolution> evolutions = this.GetObjects<Evolution>("EvolutionPokemon.PokedexNumber, EvolutionPokemonId", "EvolutionPokemon, EvolutionPokemon.Game").Where(x => x.GenerationId > game.GenerationId).ToList();
                List<Pokemon> futureEvolutions = evolutions.Where(x => pokemonList.Any(y => y.Id == x.PreevolutionPokemonId)).ToList().ConvertAll(x => x.EvolutionPokemon);
                List<PokemonFormDetail> formDetails = this.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, Form");
                futureEvolutions = futureEvolutions.Where(x => x.Game.GenerationId > game.GenerationId).Distinct().ToList();
                futureEvolutions.Where(x => formDetails.ConvertAll(x => x.AltFormPokemon).Any(y => y.Id == x.Id)).ToList().ForEach(x => x.Name = this.GetAltFormWithFormName(x.Id).Name);
                pokemonList.AddRange(futureEvolutions.Distinct());
            }

            return pokemonList.OrderBy(x => x.PokedexNumber).ThenBy(x => x.Id).ToList();
        }

        /// <summary>
        /// Gets a list of all pokemon that are able to be bred in the day care.
        /// </summary>
        /// <param name="gameId">The game's id. Defaults to 0 when no game id is given.</param>
        /// <returns>The list of breedable pokemon.</returns>
        public List<PokemonEggGroupDetail> GetAllBreedablePokemon(int gameId = 0)
        {
            List<Pokemon> battleOnlyForms = this.GetObjects<PokemonFormDetail>(includes: "OriginalPokemon, AltFormPokemon, Form", whereProperty: "Form.OnlyDuringBattle", wherePropertyValue: true).ConvertAll(x => x.AltFormPokemon);
            List<PokemonEggGroupDetail> eggGroupDetails = this.GetObjects<PokemonEggGroupDetail>(includes: "Pokemon, PrimaryEggGroup, SecondaryEggGroup");
            List<Pokemon> unbreedablePokemon = this.GetAllPokemon().Where(x => !eggGroupDetails.Any(y => y.PokemonId == x.Id && y.PrimaryEggGroupId != 15)).ToList();

            eggGroupDetails = eggGroupDetails.Where(x => !battleOnlyForms.Any(y => y.Id == x.PokemonId)).ToList();
            eggGroupDetails = eggGroupDetails.Where(x => !unbreedablePokemon.Any(y => y.Id == x.PokemonId)).ToList();
            eggGroupDetails = eggGroupDetails.Where(x => x.Pokemon.IsComplete).ToList();

            if (gameId != 0)
            {
                Game game = this.GetObjectByPropertyValue<Game>("Id", gameId);
                List<Pokemon> gameAvailability = this.GetObjects<PokemonGameDetail>(includes: "Pokemon", whereProperty: "GameId", wherePropertyValue: game.Id).ConvertAll(y => y.Pokemon);
                eggGroupDetails = eggGroupDetails.Where(x => gameAvailability.Any(z => z.Id == x.PokemonId)).ToList();
            }

            eggGroupDetails = eggGroupDetails.OrderBy(x => x.Pokemon.Name).ToList();

            return eggGroupDetails;
        }

        /// <summary>
        /// Used to get all of the alternate forms of a pokemon.
        /// </summary>
        /// <param name="pokemonId">The id of a pokemon.</param>
        /// <returns>Returns a list of all alternate forms of a pokemon.</returns>
        public List<Pokemon> GetAltForms(int pokemonId)
        {
            List<PokemonFormDetail> pokemonList = this.GetObjects<PokemonFormDetail>("AltFormPokemon.Game.ReleaseDate, AltFormPokemon.PokedexNumber, AltFormPokemon.Id", "AltFormPokemon, AltFormPokemon.EggCycle, AltFormPokemon.GenderRatio, AltFormPokemon.Classification, AltFormPokemon.Game, AltFormPokemon.ExperienceGrowth, Form");
            if (this.CheckIfAltForm(pokemonId))
            {
                PokemonFormDetail formDetail = pokemonList.Find(x => x.AltFormPokemonId == pokemonId);

                pokemonList = pokemonList.Where(x => x.OriginalPokemonId == formDetail.OriginalPokemonId && x.AltFormPokemonId != pokemonId).ToList();

                pokemonList.Add(formDetail);
            }
            else
            {
                pokemonList = pokemonList.Where(x => x.OriginalPokemonId == pokemonId).ToList();
            }

            return pokemonList.Select(x => x.AltFormPokemon).ToList();
        }

        /// <summary>
        /// Gets the pokemon given an id. This pokemon will be an alternate form and will have its form name added to the pokemon's name.
        /// </summary>
        /// <param name="pokemonId">The pokemon id.</param>
        /// <returns>Returns the pokemon.</returns>
        public Pokemon GetAltFormWithFormName(int pokemonId)
        {
            PokemonFormDetail pokemonForm = this.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, Form")
                .Find(x => x.AltFormPokemonId == pokemonId);

            Pokemon pokemon = pokemonForm.AltFormPokemon;

            pokemon.Name = string.Concat(pokemon.Name, " (", pokemonForm.Form.Name, ")");

            return pokemon;
        }

        /// <summary>
        /// Gets a list of all pokemon, alongside all of their possible typings.
        /// </summary>
        /// <returns>Returns the pokemon type detail list.</returns>
        public List<PokemonTypeDetail> GetAllPokemonWithTypes()
        {
            List<Pokemon> altFormList = this.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon").Select(x => x.AltFormPokemon).ToList();
            List<PokemonTypeDetail> pokemonList = this.GetObjects<PokemonTypeDetail>("Pokemon.PokedexNumber, PokemonId", "Pokemon, Pokemon.Game, PrimaryType, SecondaryType").Where(x => !altFormList.Any(y => y.Id == x.PokemonId)).ToList();
            List<int> pokemonIds = pokemonList.Select(x => x.PokemonId).Distinct().ToList();

            return pokemonList.OrderBy(x => x.GenerationId).GroupBy(x => new { x.PokemonId }).Select(x => x.LastOrDefault()).ToList();
        }

        /// <summary>
        /// Gets the abilities of a pokemon. This is will then be cut down to the abilities of either the latest game, or of the game specified.
        /// </summary>
        /// <param name="pokemonId">The pokemon id.</param>
        /// <param name="gameId">The game id.</param>
        /// <param name="user">The user.</param>
        /// <param name="appConfig">The application config.</param>
        /// <returns>Returns the list of abilities.</returns>
        public List<Ability> GetAbilitiesForPokemon(int pokemonId, int gameId, System.Security.Claims.ClaimsPrincipal user, AppConfig appConfig)
        {
            try
            {
                if (gameId == 0)
                {
                    List<Game> games = this.GetObjects<PokemonGameDetail>("Game.ReleaseDate, Game.Id", "Game", "PokemonId", pokemonId).Select(x => x.Game).ToList();
                    games = games.Where(x => x.Id != 37).ToList();
                    gameId = games.Where(x => x.ReleaseDate <= System.DateTime.Now).Last().Id;
                }

                List<Ability> abilityList = new List<Ability>();
                if (gameId != 1 && gameId != 21 && gameId != 20 && gameId != 2 && gameId != 22 && gameId != 23 && gameId != 37)
                {
                    Game game = this.GetObjectByPropertyValue<Game>("Id", gameId);
                    List<PokemonAbilityDetail> availableAbilityDetails = this.GetObjects<PokemonAbilityDetail>(includes: "Pokemon, PrimaryAbility, SecondaryAbility, HiddenAbility, SpecialEventAbility", whereProperty: "PokemonId", wherePropertyValue: pokemonId).OrderByDescending(x => x.GenerationId).ToList();
                    PokemonAbilityDetail pokemonAbilityDetail = availableAbilityDetails.Find(x => x.GenerationId <= game.GenerationId);

                    abilityList.Add(pokemonAbilityDetail.PrimaryAbility);
                    if (pokemonAbilityDetail.SecondaryAbility != null)
                    {
                        abilityList.Add(pokemonAbilityDetail.SecondaryAbility);
                    }

                    if (pokemonAbilityDetail.HiddenAbility != null)
                    {
                        abilityList.Add(pokemonAbilityDetail.HiddenAbility);
                    }

                    if (pokemonAbilityDetail.SpecialEventAbility != null)
                    {
                        abilityList.Add(pokemonAbilityDetail.SpecialEventAbility);
                    }
                }

                return abilityList;
            }
            catch (Exception e)
            {
                if (!user.IsInRole("Owner"))
                {
                    string commentBody;
                    Game game = this.GetObjectByPropertyValue<Game>("Id", gameId);
                    Pokemon pokemon = this.GetObjectByPropertyValue<Pokemon>("Id", pokemonId);

                    if (e != null)
                    {
                        commentBody = string.Concat(e.GetType().ToString(), " error while grabbing pokemon abilities.");
                    }
                    else
                    {
                        commentBody = "Unknown error while grabbing pokemon abilities.";
                    }

                    if (game != null)
                    {
                        commentBody = string.Concat(commentBody, " - Selected Game: ", game.Name);
                    }
                    else
                    {
                        commentBody = string.Concat(commentBody, " - Selected Game: ", gameId);
                    }

                    if (pokemon != null)
                    {
                        commentBody = string.Concat(commentBody, " - Selected Pokemon: ", pokemon.Name, " (ID: ", pokemon.Id, ")");
                    }
                    else
                    {
                        commentBody = string.Concat(commentBody, " - Selected Pokemon: ", pokemonId);
                    }

                    Comment comment = new Comment()
                    {
                        Name = commentBody,
                    };

                    if (user.Identity.Name != null)
                    {
                        comment.CommentorId = this.GetObjectByPropertyValue<User>("Username", user.Identity.Name).Id;
                    }

                    this.AddObject(comment);
                    this.EmailComment(appConfig, comment);
                }

                return null;
            }
        }

        /// <summary>
        /// Gets a list of all pokemon, alongside their capture rates.
        /// </summary>
        /// <returns>Returns the list of pokemon capture rate details.</returns>
        public List<PokemonCaptureRateDetail> GetAllPokemonWithCaptureRates()
        {
            return this.GetObjects<PokemonCaptureRateDetail>(includes: "Pokemon, CaptureRate, Generation")
                .OrderByDescending(x => x.GenerationId)
                .ToList();
        }

        /// <summary>
        /// Gets a list of all pokemon, alongside their base happinesses.
        /// </summary>
        /// <returns>Returns the list of pokemon base happiness details.</returns>
        public List<PokemonBaseHappinessDetail> GetAllPokemonWithBaseHappinesses()
        {
            return this.GetObjects<PokemonBaseHappinessDetail>(includes: "Pokemon, BaseHappiness, Generation")
                .OrderByDescending(x => x.GenerationId)
                .ToList();
        }

        /// <summary>
        /// Gets the capture rates of a pokemon.
        /// </summary>
        /// <param name="pokemonId">The pokemon's id.</param>
        /// <returns>Returns the list of capture rates for a pokemon.</returns>
        public List<PokemonCaptureRateDetail> GetPokemonWithCaptureRates(int pokemonId)
        {
            return this.GetObjects<PokemonCaptureRateDetail>(includes: "Pokemon, CaptureRate, Generation", whereProperty: "Pokemon.Id", wherePropertyValue: pokemonId)
                .OrderByDescending(x => x.GenerationId)
                .ToList();
        }

        /// <summary>
        /// Gets the base stats of a pokemon depending on the generation.
        /// </summary>
        /// <param name="pokemonId">The id of the pokemon.</param>
        /// <param name="generationId">The id of the generation.</param>
        /// <returns>The base stats of the pokemon by generation.</returns>
        public BaseStat GetPokemonBaseStats(int pokemonId, int generationId)
        {
            return this.GetObjects<BaseStat>(includes: "Pokemon")
                .ToList()
                .Find(x => x.PokemonId == pokemonId && x.GenerationId == generationId);
        }

        /// <summary>
        /// Gets the ev yield of a pokemon depending on the generation.
        /// </summary>
        /// <param name="pokemonId">The id of the pokemon.</param>
        /// <param name="generationId">The id of the generation.</param>
        /// <returns>The ev yield of the pokemon by generation.</returns>
        public EVYield GetPokemonEVYields(int pokemonId, int generationId)
        {
            return this.GetObjects<EVYield>(includes: "Pokemon")
                .ToList()
                .Find(x => x.PokemonId == pokemonId && x.GenerationId == generationId);
        }

        /// <summary>
        /// Gets the type chart by the defending type.
        /// </summary>
        /// <param name="typeId">The type's id.</param>
        /// <param name="generationId">The generation's id.</param>
        /// <returns>The type chart by the defending type.</returns>
        public List<TypeChart> GetTypeChartByDefendType(int typeId, int generationId)
        {
            return this.GetObjects<TypeChart>("AttackId, DefendId", "Attack, Defend")
                .Where(x => x.DefendId == typeId && x.GenerationId == generationId)
                .ToList();
        }

        /// <summary>
        /// Gets the type chart of a particular pokemon.
        /// </summary>
        /// <param name="pokemonId">The id of the pokemon.</param>
        /// <returns>The view model used for the pokemon's type chart.</returns>
        public List<PokemonTypeChartViewModel> GetTypeChartPokemon(int pokemonId)
        {
            List<DataAccess.Models.Type> typeList = this.GetObjects<DataAccess.Models.Type>("Name");
            List<PokemonTypeDetail> pokemonTypes = this.GetObjects<PokemonTypeDetail>(includes: "Pokemon, PrimaryType, SecondaryType, Generation", whereProperty: "PokemonId", wherePropertyValue: pokemonId);
            List<TypeChart> typeChart = this.GetObjects<TypeChart>("AttackId, DefendId", "Attack, Defend");
            List<TypeChart> primaryTypeChart = new List<TypeChart>();
            List<TypeChart> secondaryTypeChart = new List<TypeChart>();
            List<PokemonTypeChartViewModel> pokemonTypeCharts = new List<PokemonTypeChartViewModel>();

            foreach (var t in pokemonTypes)
            {
                if (t.SecondaryType != null)
                {
                    secondaryTypeChart = typeChart.Where(x => x.DefendId == t.SecondaryTypeId).ToList();
                }

                primaryTypeChart = typeChart.Where(x => x.DefendId == t.PrimaryTypeId).ToList();

                if (secondaryTypeChart.Count > 0)
                {
                    primaryTypeChart.AddRange(secondaryTypeChart);
                }

                List<TypeChart> finalTypeChart = new List<TypeChart>();
                finalTypeChart.AddRange(primaryTypeChart);

                foreach (var i in primaryTypeChart.Where(x => x.Effective == 0))
                {
                    finalTypeChart.RemoveAll(x => x.AttackId == i.AttackId && x.Effective != 0 && x.GenerationId == i.GenerationId);
                }

                pokemonTypeCharts.Add(new PokemonTypeChartViewModel()
                {
                    TypeChart = finalTypeChart,
                    Generation = t.Generation,
                });
            }

            return pokemonTypeCharts;
        }

        /// <summary>
        /// Adds a pokemon team detail to the database, and ensures that there is an entry for the pokemon's EV, IV, and moveset.
        /// </summary>
        /// <param name="pokemonTeamDetail">The pokemon team detail that is being added.</param>
        /// <returns>The id of the new pokemon team detail.</returns>
        public int AddPokemonTeamDetail(PokemonTeamDetail pokemonTeamDetail)
        {
            if (pokemonTeamDetail.PokemonTeamEVId == null)
            {
                PokemonTeamEV pokemonTeamEV = new PokemonTeamEV();
                this.AddObject(pokemonTeamEV);
                pokemonTeamDetail.PokemonTeamEVId = pokemonTeamEV.Id;
            }

            if (pokemonTeamDetail.PokemonTeamIVId == null)
            {
                PokemonTeamIV pokemonTeamIV = new PokemonTeamIV();
                this.AddObject(pokemonTeamIV);
                pokemonTeamDetail.PokemonTeamIVId = pokemonTeamIV.Id;
            }

            if (pokemonTeamDetail.PokemonTeamMovesetId == null)
            {
                PokemonTeamMoveset pokemonTeamMoveset = new PokemonTeamMoveset();
                this.AddObject(pokemonTeamMoveset);
                pokemonTeamDetail.PokemonTeamMovesetId = pokemonTeamMoveset.Id;
            }

            this.AddObject(pokemonTeamDetail);
            return pokemonTeamDetail.Id;
        }

        /// <summary>
        /// Deletes a pokemon team.
        /// </summary>
        /// <param name="id">The id of the team being deleted.</param>
        /// <param name="user">The user that called this method. Only used if there is an error.</param>
        /// <param name="appConfig">The application's config. Only used if there is an error.</param>
        public void DeletePokemonTeam(int id, System.Security.Claims.ClaimsPrincipal user, AppConfig appConfig)
        {
            int teamDetailId = 0;
            List<int> pokemonTeamDetailIds = new List<int>();

            try
            {
                PokemonTeam pokemonTeam = this.GetObjectByPropertyValue<PokemonTeam>("Id", id);
                pokemonTeamDetailIds = pokemonTeam.GrabPokemonTeamDetailIds();
                this.DeleteObject<PokemonTeam>(pokemonTeam.Id);
            }
            catch (Exception e)
            {
                if (!user.IsInRole("Owner") && e != null)
                {
                    string commentBody;
                    if (e != null)
                    {
                        commentBody = string.Concat(e.GetType().ToString(), " error during the pokemon team's deletion.");
                    }
                    else
                    {
                        commentBody = "Unknown error during the pokemon team's deletion.";
                    }

                    commentBody = string.Concat(commentBody, " - Deleted Team Id: {", id, "}");
                    if (pokemonTeamDetailIds.Count() > 0)
                    {
                        commentBody = string.Concat(commentBody, " - Deleted Team Detail Id(s): {", string.Join(", ", pokemonTeamDetailIds), "}");
                    }

                    Comment comment = new Comment()
                    {
                        Name = commentBody,
                    };

                    if (user.Identity.Name != null)
                    {
                        comment.CommentorId = this.GetObjectByPropertyValue<User>("Username", user.Identity.Name).Id;
                    }

                    this.AddObject(comment);
                    this.EmailComment(appConfig, comment);
                }
            }

            if (pokemonTeamDetailIds.Count() > 0)
            {
                try
                {
                    foreach (var p in pokemonTeamDetailIds)
                    {
                        teamDetailId = p;
                        this.DeleteObject<PokemonTeamDetail>(teamDetailId);
                    }
                }
                catch (Exception e)
                {
                    if (!user.IsInRole("Owner") && e != null)
                    {
                        string commentBody;
                        if (e != null)
                        {
                            commentBody = string.Concat(e.GetType().ToString(), " error during the pokemon team detail's deletion.");
                        }
                        else
                        {
                            commentBody = "Unknown error during the pokemon team's deletion.";
                        }

                        commentBody = string.Concat(commentBody, " - Deleted Team Id: {", id, "}");
                        commentBody = string.Concat(commentBody, " - Deleted Team Detail Id: {", string.Join(", ", teamDetailId), "}");

                        Comment comment = new Comment()
                        {
                            Name = commentBody,
                        };

                        if (user.Identity.Name != null)
                        {
                            comment.CommentorId = this.GetObjectByPropertyValue<User>("Username", user.Identity.Name).Id;
                        }

                        this.AddObject(comment);
                        this.EmailComment(appConfig, comment);
                    }
                }
            }
        }

        /// <summary>
        /// Gets a list of all of the pokemon teams a user has, sorted with no games first, then by the game itself.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <returns>A list of sorted pokemon teams.</returns>
        public List<PokemonTeam> GetPokemonTeams(string username)
        {
            List<PokemonTeam> teams = this.GetObjects<PokemonTeam>("Id", "User, Game, FirstPokemon, FirstPokemon.Pokemon, SecondPokemon, SecondPokemon.Pokemon, ThirdPokemon, ThirdPokemon.Pokemon, FourthPokemon, FourthPokemon.Pokemon, FifthPokemon, FifthPokemon.Pokemon, SixthPokemon, SixthPokemon.Pokemon", "User.Username", username);
            List<PokemonTeam> teamsWithGames = teams.Where(x => x.Game != null).OrderBy(x => x.Game.ReleaseDate).ThenBy(x => x.GameId).ThenBy(x => x.Id).ToList();
            List<PokemonTeam> teamsWithoutGames = teams.Where(x => x.Game == null).OrderBy(x => x.Id).ToList();
            List<PokemonTeam> pokemonTeams = teamsWithoutGames;
            pokemonTeams.AddRange(teamsWithGames);

            return pokemonTeams;
        }

        /// <summary>
        /// Gets a list of huntable pokemon.
        /// </summary>
        /// <param name="gameId">The game's id. Defaults to 0.</param>
        /// <returns>A list of pokemon huntable.</returns>
        public List<Pokemon> GetHuntablePokemon(int gameId = 0)
        {
            List<Pokemon> pokemonList = this.GetNonBattlePokemonWithFormNames(gameId).Where(x => !x.IsShinyLocked).ToList();
            if (gameId != 0)
            {
                Game game = this.GetObjectByPropertyValue<Game>("Id", gameId);

                // Gets extra forms made available in future generations.
                if (game.GenerationId == 3)
                {
                    pokemonList.Remove(pokemonList.Find(x => x.Name.Contains("Deoxys")));
                    List<Pokemon> deoxysList = this.GetObjects<Pokemon>(includes: "GenderRatio").Where(x => x.Name == "Deoxys").ToList();
                    List<PokemonFormDetail> formDetails = this.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, Form").Where(x => x.AltFormPokemon.Name == "Deoxys").ToList();
                    deoxysList.Where(x => formDetails.ConvertAll(x => x.AltFormPokemon).Any(y => y.Id == x.Id)).ToList().ForEach(x => x.Name = this.GetAltFormWithFormName(x.Id).Name);
                    pokemonList.AddRange(deoxysList);
                }
                else if (game.GenerationId == 6)
                {
                    pokemonList.Remove(pokemonList.Find(x => x.Name.Contains("Zygarde")));
                    List<Pokemon> zygardeList = this.GetObjects<Pokemon>(includes: "GenderRatio").Where(x => x.Name == "Zygarde").ToList();
                    List<PokemonFormDetail> formDetails = this.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, Form").Where(x => x.AltFormPokemon.Name == "Zygarde").ToList();
                    zygardeList = zygardeList.Where(x => !formDetails.Where(x => x.Form.Name == "Complete").Select(x => x.AltFormPokemon).Any(y => y.Id == x.Id)).ToList();
                    zygardeList.Where(x => formDetails.ConvertAll(x => x.AltFormPokemon).Any(y => y.Id == x.Id)).ToList().ForEach(x => x.Name = this.GetAltFormWithFormName(x.Id).Name);
                    pokemonList.AddRange(zygardeList);
                }
            }

            return pokemonList.OrderBy(x => x.PokedexNumber).ThenBy(x => x.Id).ToList();
        }

        /// <summary>
        /// Gets the list of games possible to shiny hunt in and formats it for the shiny hunt pages.
        /// </summary>
        /// <param name="pokemonId">The pokemon's id.</param>
        /// <returns>The list of shiny huntable games.</returns>
        public List<Game> GetShinyHuntGames(int pokemonId)
        {
            List<Pokemon> pokemonList = this.GetObjects<Pokemon>(includes: "Game");
            Pokemon pokemon = pokemonList.Find(x => x.Id == pokemonId);
            List<PokemonGameDetail> pokemonGameDetails = this.GetObjects<PokemonGameDetail>("GameId", "Game");
            List<Game> pokemonGamesIn = pokemonGameDetails.Where(x => x.PokemonId == pokemon.Id).ToList().ConvertAll(x => x.Game);
            List<Game> gamesAvailable = this.GetGamesGroupedByReleaseDate().Where(x => x.GenerationId >= 2).ToList();
            List<Game> possibleGames = gamesAvailable.Where(x => pokemonGamesIn.Any(y => y.Id == x.Id)).ToList();

            // Gets other games for extra forms introduced in future generations.
            if (pokemon.Name == "Deoxys")
            {
                possibleGames = possibleGames.Where(x => x.GenerationId >= 4).ToList();
                possibleGames.AddRange(gamesAvailable.Where(x => x.GenerationId == 3 && x.Id != 39 && x.Id != 40));
            }
            else if (pokemon.Name == "Zygarde")
            {
                possibleGames = possibleGames.Where(x => x.GenerationId >= 7).ToList();
                possibleGames.AddRange(gamesAvailable.Where(x => x.GenerationId == 6));
            }

            // Gets other games for evolutions introduced in future generations.
            List<Evolution> evolutionList = this.GetObjects<Evolution>(includes: "PreevolutionPokemon");
            Evolution evolution = evolutionList.Find(x => x.EvolutionPokemonId == pokemon.Id);
            int preevolution = pokemon.Id;
            while (evolution != null && evolution.PreevolutionPokemon != null)
            {
                preevolution = evolution.PreevolutionPokemonId;
                evolution = evolutionList.Find(x => x.EvolutionPokemonId == preevolution);
            }

            if (preevolution != pokemon.Id)
            {
                pokemonGamesIn = pokemonGameDetails.Where(x => x.PokemonId == preevolution).ToList().ConvertAll(x => x.Game);
                gamesAvailable = gamesAvailable.Where(x => pokemonGamesIn.Any(y => y.Id == x.Id)).ToList();
                possibleGames.AddRange(gamesAvailable.Where(x => !possibleGames.Any(y => y.Id == x.Id)).ToList());
            }

            return possibleGames.OrderBy(x => x.GenerationId).ThenBy(x => x.Id).ToList();
        }

        /// <summary>
        /// Gets the list of games grouped by release date.
        /// </summary>
        /// <returns>The list of games grouped by release date.</returns>
        public List<Game> GetGamesGroupedByReleaseDate()
        {
            List<Game> gamesList = new List<Game>();
            List<Game> selectableGames = new List<Game>();
            gamesList = this.GetObjects<Game>("ReleaseDate, Id").Where(x => x.ReleaseDate <= DateTime.Now).ToList();
            List<Game> uniqueGames = gamesList.OrderBy(x => x.ReleaseDate).ThenBy(x => x.Id).DistinctBy(y => y.ReleaseDate).ToList();
            for (var i = 0; i < uniqueGames.Count; i++)
            {
                if (uniqueGames[i].Name == "Fire Red")
                {
                    selectableGames.Add(uniqueGames[i]);
                    selectableGames.Add(this.GetObjectByPropertyValue<Game>("Name", "Leaf Green"));
                }
                else if (i == uniqueGames.Count - 1)
                {
                    selectableGames.Add(new Game()
                    {
                        Id = uniqueGames[i].Id,
                        Name = string.Join(" / ", gamesList.Where(x => x.ReleaseDate >= uniqueGames[i].ReleaseDate).Select(x => x.Name)),
                        GenerationId = uniqueGames[i].GenerationId,
                    });
                }
                else
                {
                    List<Game> games = gamesList.Where(x => x.ReleaseDate >= uniqueGames[i].ReleaseDate && x.ReleaseDate < uniqueGames[i + 1].ReleaseDate && !selectableGames.Any(y => y.ReleaseDate == x.ReleaseDate)).ToList();
                    if (games.Count == 0)
                    {
                        selectableGames.Add(uniqueGames[i]);
                    }
                    else
                    {
                        selectableGames.Add(new Game()
                        {
                            Id = uniqueGames[i].Id,
                            Name = string.Join(" / ", games.ConvertAll(x => x.Name)),
                            GenerationId = uniqueGames[i].GenerationId,
                        });
                    }
                }
            }

            return selectableGames;
        }

        /// <summary>
        /// Gets a list of pokeballs available for shiny hunts.
        /// </summary>
        /// <param name="gameId">The game the shiny hunt is taking place in.</param>
        /// <param name="huntingMethodId">The type of shiny hunt.</param>
        /// <param name="user">The user.</param>
        /// <param name="appConfig">The application config.</param>
        /// <returns>The available pokeballs.</returns>
        public List<Pokeball> GetPokeballs(int gameId, int huntingMethodId, System.Security.Claims.ClaimsPrincipal user, AppConfig appConfig)
        {
            try
            {
                List<Pokeball> selectablePokeballs = this.GetObjects<Pokeball>();
                Game game = this.GetObjectByPropertyValue<Game>("Id", gameId);
                HuntingMethod huntingMethod = this.GetObjectByPropertyValue<HuntingMethod>("Id", huntingMethodId);
                if ((game.GenerationId != 5 && game.GenerationId < 8) || game.Id == 35 || game.Id == 36)
                {
                    selectablePokeballs.Remove(selectablePokeballs.Find(x => x.Id == 25));
                }

                if ((game.GenerationId > 4 && game.GenerationId < 8) || game.GenerationId == 9)
                {
                    selectablePokeballs.Remove(selectablePokeballs.Find(x => x.Id == 5));
                }

                if (game.GenerationId != 2 && game.Id != 9 && game.Id != 26 && game.Id != 17 && game.Id != 32)
                {
                    selectablePokeballs.Remove(selectablePokeballs.Find(x => x.Id == 13));
                }

                if (game.Id == 16 || game.Id == 28 || game.Id == 43)
                {
                    selectablePokeballs = selectablePokeballs.Where(x => x.GenerationId == 1 || x.Id == 20).ToList();
                }

                if (game.Id == 37)
                {
                    selectablePokeballs = selectablePokeballs.Where(x => x.Name.Contains("Hisui")).ToList();
                    foreach (var p in selectablePokeballs)
                    {
                        p.Name = p.Name.Replace(" (Hisui)", string.Empty);
                    }
                }
                else
                {
                    selectablePokeballs = selectablePokeballs.Where(x => !x.Name.Contains("Hisui")).ToList();
                }

                switch (huntingMethod.Name)
                {
                    case "Breeding":
                    case "Masuda Method":
                        if (game.GenerationId <= 5)
                        {
                            selectablePokeballs = selectablePokeballs.Where(x => x.Id == 1).ToList();
                        }
                        else
                        {
                            selectablePokeballs = selectablePokeballs.Where(x => x.GenerationId <= game.GenerationId && x.Id != 4 && x.Id != 24).ToList();
                        }

                        break;
                    case "Event":
                        if (game.GenerationId <= 3)
                        {
                            selectablePokeballs = selectablePokeballs.Where(x => x.Id == 1).ToList();
                        }
                        else
                        {
                            selectablePokeballs = selectablePokeballs.Where(x => x.Id == 24).ToList();
                        }

                        break;
                    default:
                        break;
                }

                return selectablePokeballs.Where(x => x.GenerationId <= game.GenerationId).OrderBy(x => x.Name).ToList();
            }
            catch (Exception e)
            {
                if (!user.IsInRole("Owner"))
                {
                    string commentBody;
                    Game game = this.GetObjectByPropertyValue<Game>("Id", gameId);
                    HuntingMethod huntingMethod = this.GetObjectByPropertyValue<HuntingMethod>("Id", huntingMethodId);

                    if (e != null)
                    {
                        commentBody = string.Concat(e.GetType().ToString(), " error while grabbing shiny hunting pokeballs.");
                    }
                    else
                    {
                        commentBody = "Unknown error while grabbing shiny hunting pokeballs.";
                    }

                    if (game != null)
                    {
                        commentBody = string.Concat(commentBody, " - Selected Game: ", game.Name);
                    }
                    else
                    {
                        commentBody = string.Concat(commentBody, " - Selected Game: ", gameId);
                    }

                    if (huntingMethod != null)
                    {
                        commentBody = string.Concat(commentBody, " - Selected Hunting Method: ", huntingMethod.Name, " (ID: ", huntingMethod.Id, ")");
                    }
                    else
                    {
                        commentBody = string.Concat(commentBody, " - Selected Hunting Method: ", huntingMethodId);
                    }

                    Comment comment = new Comment()
                    {
                        Name = commentBody,
                    };

                    if (user.Identity.Name != null)
                    {
                        comment.CommentorId = this.GetObjectByPropertyValue<User>("Username", user.Identity.Name).Id;
                    }

                    this.AddObject(comment);
                    this.EmailComment(appConfig, comment);
                }

                return null;
            }
        }

        /// <summary>
        /// Creates a list of strings for the available genders of a pokemon.
        /// </summary>
        /// <param name="pokemonId">The pokemon's id.</param>
        /// <param name="useCase">The use case of this method. Only used to determine what no gender is listed as.</param>
        /// <returns>A list of genders in string form.</returns>
        public List<string> GrabGenders(int pokemonId, string useCase)
        {
            List<string> genders = new List<string>();
            GenderRatio genderRatio = this.GetObjectByPropertyValue<Pokemon>("Id", pokemonId, "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth").GenderRatio;
            if (genderRatio.MaleRatio == 0 && genderRatio.FemaleRatio == 0)
            {
                if (useCase == "shinyHunt")
                {
                    genders.Add("Gender Unknown");
                }
                else
                {
                    genders.Add("None");
                }
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

        /// <summary>
        /// Uploades a given image to the pokemon images folder.
        /// </summary>
        /// <param name="fileUpload">The file being uploaded.</param>
        /// <param name="urlUpload">The URL of an image if no file is provided.</param>
        /// <param name="pokemonId">The id of the pokemon this image is for.</param>
        /// <param name="appConfig">The application config.</param>
        /// <param name="imageType">Whether this image is for the 2d artwork or the 3d render.</param>
        public async void UploadImages(IFormFile fileUpload, string urlUpload, int pokemonId, AppConfig appConfig, string imageType)
        {
            string imageUrlPath = string.Empty;
            string iconUrlPath = string.Empty;
            string gridUrlPath = string.Empty;
            IFormFile upload;
            IFormFile resizedUpload;

            if (imageType == "2d")
            {
                imageUrlPath = appConfig.OfficialPokemonImageFTPUrl;
                iconUrlPath = appConfig.IconImageFTPUrl;
                gridUrlPath = appConfig.GridImageFTPUrl;
            }
            else if (imageType == "3d")
            {
                imageUrlPath = appConfig.HomePokemonImageFTPUrl;
            }
            else if (imageType == "shiny")
            {
                imageUrlPath = appConfig.ShinyPokemonImageFTPUrl;
                iconUrlPath = appConfig.ShinyIconImageFTPUrl;
                gridUrlPath = appConfig.ShinyGridImageFTPUrl;
            }
            else if (imageType == "pokeball")
            {
                imageUrlPath = appConfig.OfficialPokeballImageFTPUrl;
            }
            else if (imageType == "mark")
            {
                imageUrlPath = appConfig.OfficialMarkImageFTPUrl;
            }

            if (fileUpload == null && urlUpload != null)
            {
                WebRequest webRequest = WebRequest.CreateHttp(urlUpload);

                using WebResponse webResponse = webRequest.GetResponse();
                Stream stream = webResponse.GetResponseStream();
                MemoryStream memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);

                upload = new FormFile(memoryStream, 0, memoryStream.Length, "image", "image.png");
            }
            else
            {
                upload = fileUpload;
            }

            if (upload != null)
            {
                if (imageType == "pokeball" || imageType == "mark")
                {
                    resizedUpload = this.TrimImage(upload, 50, 50, true);
                }
                else
                {
                    resizedUpload = this.TrimImage(upload, 350, 350);
                }

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(string.Concat(appConfig.FTPUrl, imageUrlPath, pokemonId.ToString(), ".png"));
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(appConfig.FTPUsername, appConfig.FTPPassword);

                using (Stream requestStream = request.GetRequestStream())
                {
                    await resizedUpload.CopyToAsync(requestStream).ConfigureAwait(false);
                }

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    System.Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
                }

                if (imageType == "2d" || imageType == "shiny")
                {
                    IFormFile iconUpload = this.TrimImage(upload, 64, 64, true);

                    request = (FtpWebRequest)WebRequest.Create(string.Concat(appConfig.FTPUrl, iconUrlPath, pokemonId.ToString(), ".png"));
                    request.Method = WebRequestMethods.Ftp.UploadFile;
                    request.Credentials = new NetworkCredential(appConfig.FTPUsername, appConfig.FTPPassword);

                    using (Stream requestStream = request.GetRequestStream())
                    {
                        await iconUpload.CopyToAsync(requestStream).ConfigureAwait(false);
                    }

                    using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                    {
                        System.Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
                    }

                    IFormFile gridUpload = this.TrimImage(upload, 150, 150);

                    request = (FtpWebRequest)WebRequest.Create(string.Concat(appConfig.FTPUrl, gridUrlPath, pokemonId.ToString(), ".png"));
                    request.Method = WebRequestMethods.Ftp.UploadFile;
                    request.Credentials = new NetworkCredential(appConfig.FTPUsername, appConfig.FTPPassword);

                    using (Stream requestStream = request.GetRequestStream())
                    {
                        await gridUpload.CopyToAsync(requestStream).ConfigureAwait(false);
                    }

                    using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                    {
                        System.Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
                    }
                }
            }
            else
            {
                WebClient webRequest = new WebClient
                {
                    Credentials = new NetworkCredential(appConfig.FTPUsername, appConfig.FTPPassword),
                };

                byte[] file = webRequest.DownloadData(string.Concat(appConfig.WebUrl, "/images/general/tempPhoto.png"));

                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(string.Concat(appConfig.FTPUrl, imageUrlPath, pokemonId.ToString(), ".png"));
                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
                ftpRequest.Credentials = new NetworkCredential(appConfig.FTPUsername, appConfig.FTPPassword);

                using (Stream requestStream = ftpRequest.GetRequestStream())
                {
                    requestStream.Write(file, 0, file.Length);
                }

                using FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse();
                System.Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
            }
        }

        /// <summary>
        /// Gets the User class for the current user using the ClaimsPrincipal object.
        /// </summary>
        /// <param name="user">The ClaimsPrincipal object of the current user.</param>
        /// <returns>The current user's User object.</returns>
        public User GetCurrentUser(System.Security.Claims.ClaimsPrincipal user)
        {
            return this.GetObjectByPropertyValue<User>("Username", user.Identity.Name);
        }

        /// <summary>
        /// Sends an email with a given comment.
        /// </summary>
        /// <param name="appConfig">The configuration for the application.</param>
        /// <param name="comment">The email's message.</param>
        public void EmailComment(AppConfig appConfig, Comment comment)
        {
            try
            {
                if (comment.CommentorId != 1)
                {
                    MailAddress fromAddress = new MailAddress(appConfig.EmailAddress, "Pokemon Database Website");
                    MailAddress toAddress = new MailAddress(appConfig.EmailAddress, "Pokemon Database Email");
                    MailMessage message = new MailMessage(fromAddress, toAddress)
                    {
                        Subject = string.Concat("New Comment for ", appConfig.AppName),
                        Body = "Comment",
                    };

                    if (comment.CommentorId != null)
                    {
                        message.Body += string.Concat(" by ", this.GetObjectByPropertyValue<User>("Id", (int)comment.CommentorId).Username);
                    }
                    else
                    {
                        message.Body += string.Concat(" by Anonymous User");
                    }

                    message.Body += string.Concat(": ", comment.Name);

                    SmtpClient smtp = new SmtpClient()
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(fromAddress.Address, appConfig.EmailAddressPassword),
                    };
                    smtp.Send(message);
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine("Email could not be sent. ", (ex.InnerException != null) ? ex.InnerException.ToString() : ex.Message);
            }
        }

        /// <summary>
        /// Formats the image for use as a main page image.
        /// </summary>
        /// <param name="file">The file being formatted.</param>
        /// <param name="width">The new width of the image. Defaults is 0.</param>
        /// <param name="height">The new height of the image. Defaults is 0.</param>
        /// <param name="icon">Whether or not this image is an icon. Default is false.</param>
        /// <returns>The formatted file.</returns>
        private IFormFile TrimImage(IFormFile file, int width = 0, int height = 0, bool icon = false)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                file.CopyTo(ms);
                byte[] fileBytes = ms.ToArray();
                using MagickImage image = new MagickImage(fileBytes);
                if (!icon)
                {
                    image.Trim();
                }

                image.Resize(new MagickGeometry(width, height));
                MemoryStream strm = new MemoryStream();
                image.RePage();
                if (icon)
                {
                    image.Quality = 75;
                }
                else
                {
                    image.Quality = 100;
                }

                image.Settings.SetDefine(MagickFormat.WebP, "lossless", true);
                image.Settings.SetDefine(MagickFormat.WebP, "method", "6");
                image.Write(strm, MagickFormat.Png);
                file = new FormFile(strm, 0, strm.Length, file.Name, file.FileName);
            }

            return file;
        }
    }
}
