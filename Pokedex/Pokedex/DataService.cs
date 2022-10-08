using ImageMagick;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Pokedex.DataAccess.Models;
using Pokedex.Models;
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
            this.dataContext.Set<TEntity>().Update(entity);
            this.dataContext.SaveChanges();
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
            return this.GetAllPokemon().Find(x => x.Name == name);
        }

        /// <summary>
        /// Gets the pokemon from a name. This pokemon will be an alternate form and will have the form name be used in the process of searching for it.
        /// </summary>
        /// <param name="pokemonName">The name of the pokemon.</param>
        /// <param name="formName">The name of the alternate form.</param>
        /// <returns>Returns the alternate pokemon.</returns>
        public Pokemon GetPokemonFromNameAndFormName(string pokemonName, string formName)
        {
            return this.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, OriginalPokemon, Form", whereProperty: "Form.Name", wherePropertyValue: formName).FirstOrDefault(x => x.AltFormPokemon.Name == pokemonName).AltFormPokemon;
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
        /// Gets a list of all pokemon that are able to be bred in the day care.
        /// </summary>
        /// <returns>The list of breedable pokemon.</returns>
        public List<PokemonEggGroupDetail> GetAllBreedablePokemon()
        {
            List<Pokemon> pokemonList = this.GetObjects<Pokemon>(whereProperty: "IsComplete", wherePropertyValue: true);

            List<PokemonFormDetail> formDetails = new List<PokemonFormDetail>();
            List<string> formNames = new List<string>()
            {
                "Mega",
                "Mega X",
                "Mega Y",
                "Gigantamax",
                "Low Key Gigantamax",
                "Sunny",
                "Rainy",
                "Snowy",
                "Zen",
                "Galar Zen",
                "Noice",
                "School",
                "Core",
                "Blade",
                "Crowned",
                "Ash",
                "Starter",
            };

            foreach (var n in formNames)
            {
                formDetails.AddRange(this.GetObjects<PokemonFormDetail>(includes: "OriginalPokemon, AltFormPokemon, Form", whereProperty: "Form.Name", wherePropertyValue: n));
            }

            List<PokemonEggGroupDetail> eggGroupDetails = this.GetObjects<PokemonEggGroupDetail>(includes: "Pokemon, PrimaryEggGroup, SecondaryEggGroup");

            List<Pokemon> unbreedablePokemon = this.GetAllPokemon().Where(x => !eggGroupDetails.Any(y => y.PokemonId == x.Id && y.PrimaryEggGroup.Name != "Undiscovered")).ToList();

            eggGroupDetails = eggGroupDetails.Where(x => !formDetails.Any(y => y.AltFormPokemonId == x.PokemonId)).ToList();
            eggGroupDetails = eggGroupDetails.Where(x => !unbreedablePokemon.Any(y => y.Id == x.PokemonId)).ToList();
            eggGroupDetails = eggGroupDetails.Where(x => x.Pokemon.IsComplete).ToList();
            eggGroupDetails = eggGroupDetails.OrderBy(x => x.Pokemon.Name).ToList();

            return eggGroupDetails;
        }

        /// <summary>
        /// Gets the complete list of pokemon teams. Specifically doesn't use the GetObjects method, due to the amount of includes being made.
        /// </summary>
        /// <returns>All pokemon teams created on the website.</returns>
        public List<PokemonTeam> GetPokemonTeams()
        {
            return this.dataContext.PokemonTeams
                .Include(x => x.Game)
                .Include(x => x.FirstPokemon)
                    .Include("FirstPokemon.Pokemon")
                        .Include("FirstPokemon.Pokemon.Game")
                    .Include("FirstPokemon.Ability")
                    .Include("FirstPokemon.PokemonTeamEV")
                    .Include("FirstPokemon.PokemonTeamIV")
                    .Include("FirstPokemon.PokemonTeamMoveset")
                    .Include("FirstPokemon.BattleItem")
                    .Include("FirstPokemon.Nature")
                .Include(x => x.SecondPokemon)
                    .Include("SecondPokemon.Pokemon")
                        .Include("SecondPokemon.Pokemon.Game")
                    .Include("SecondPokemon.Ability")
                    .Include("SecondPokemon.PokemonTeamEV")
                    .Include("SecondPokemon.PokemonTeamIV")
                    .Include("SecondPokemon.PokemonTeamMoveset")
                    .Include("SecondPokemon.BattleItem")
                    .Include("SecondPokemon.Nature")
                .Include(x => x.ThirdPokemon)
                    .Include("ThirdPokemon.Pokemon")
                        .Include("ThirdPokemon.Pokemon.Game")
                    .Include("ThirdPokemon.Ability")
                    .Include("ThirdPokemon.PokemonTeamEV")
                    .Include("ThirdPokemon.PokemonTeamIV")
                    .Include("ThirdPokemon.PokemonTeamMoveset")
                    .Include("ThirdPokemon.BattleItem")
                    .Include("ThirdPokemon.Nature")
                .Include(x => x.FourthPokemon)
                    .Include("FourthPokemon.Pokemon")
                        .Include("FourthPokemon.Pokemon.Game")
                    .Include("FourthPokemon.Ability")
                    .Include("FourthPokemon.PokemonTeamEV")
                    .Include("FourthPokemon.PokemonTeamIV")
                    .Include("FourthPokemon.PokemonTeamMoveset")
                    .Include("FourthPokemon.BattleItem")
                    .Include("FourthPokemon.Nature")
                .Include(x => x.FifthPokemon)
                    .Include("FifthPokemon.Pokemon")
                        .Include("FifthPokemon.Pokemon.Game")
                    .Include("FifthPokemon.Ability")
                    .Include("FifthPokemon.PokemonTeamEV")
                    .Include("FifthPokemon.PokemonTeamIV")
                    .Include("FifthPokemon.PokemonTeamMoveset")
                    .Include("FifthPokemon.BattleItem")
                    .Include("FifthPokemon.Nature")
                .Include(x => x.SixthPokemon)
                    .Include("SixthPokemon.Pokemon")
                        .Include("SixthPokemon.Pokemon.Game")
                    .Include("SixthPokemon.Ability")
                    .Include("SixthPokemon.PokemonTeamEV")
                    .Include("SixthPokemon.PokemonTeamIV")
                    .Include("SixthPokemon.PokemonTeamMoveset")
                    .Include("SixthPokemon.BattleItem")
                    .Include("SixthPokemon.Nature")
                .Include(x => x.User)
                .OrderBy(x => x.Id)
                .ToList();
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
        /// <returns>Returns the list of abilities.</returns>
        public List<Ability> GetAbilitiesForPokemon(int pokemonId, int gameId)
        {
            if (gameId == 0)
            {
                gameId = this.GetObjects<Game>("ReleaseDate, Id").Where(x => x.ReleaseDate <= System.DateTime.Now).Last().Id;
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
            List<Type> typeList = this.GetObjects<Type>("Name");
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
        public void DeletePokemonTeam(int id)
        {
            PokemonTeam pokemonTeam = this.GetObjectByPropertyValue<PokemonTeam>("Id", id);
            List<int> pokemonTeamDetailIds = pokemonTeam.GrabPokemonTeamDetailIds();
            this.DeleteObject<PokemonTeam>(pokemonTeam.Id);

            foreach (var p in pokemonTeamDetailIds)
            {
                this.DeleteObject<PokemonTeamDetail>(p);
            }
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
            IFormFile upload;

            if (imageType == "2d")
            {
                imageUrlPath = appConfig.OfficialPokemonImageFTPUrl;
            }
            else if (imageType == "3d")
            {
                imageUrlPath = appConfig.HomePokemonImageFTPUrl;
            }
            else if (imageType == "shiny")
            {
                imageUrlPath = appConfig.ShinyPokemonImageFTPUrl;
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
                upload = this.TrimImage(upload);

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(string.Concat(appConfig.FTPUrl, imageUrlPath, pokemonId.ToString(), ".png"));
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(appConfig.FTPUsername, appConfig.FTPPassword);

                using (Stream requestStream = request.GetRequestStream())
                {
                    await upload.CopyToAsync(requestStream).ConfigureAwait(false);
                }

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    System.Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
                }

                if (imageType == "2d")
                {
                    IFormFile favIconUpload = this.FormatFavIcon(upload);

                    request = (FtpWebRequest)WebRequest.Create(string.Concat(appConfig.FTPUrl, appConfig.FaviconImageFTPUrl, pokemonId.ToString(), ".png"));
                    request.Method = WebRequestMethods.Ftp.UploadFile;
                    request.Credentials = new NetworkCredential(appConfig.FTPUsername, appConfig.FTPPassword);

                    using (Stream requestStream = request.GetRequestStream())
                    {
                        await favIconUpload.CopyToAsync(requestStream).ConfigureAwait(false);
                    }

                    using FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                    System.Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
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
                    string body = "Comment";

                    if (comment.CommentorId != null)
                    {
                        body = string.Concat(body, " by ", this.GetObjectByPropertyValue<User>("Id", (int)comment.CommentorId).Username);
                    }
                    else
                    {
                        body = string.Concat(body, " by Anonymous User");
                    }

                    body = string.Concat(body, ": ", comment.Name);

                    SmtpClient smtp = new SmtpClient()
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(fromAddress.Address, appConfig.EmailAddressPassword),
                    };

                    using MailMessage message = new MailMessage(fromAddress, toAddress)
                    {
                        Subject = "New Comment for Pokémon Database",
                        Body = body,
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
        /// <returns>The formatted file.</returns>
        private IFormFile TrimImage(IFormFile file)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                file.CopyTo(ms);
                byte[] fileBytes = ms.ToArray();
                using MagickImage image = new MagickImage(fileBytes);
                image.Trim();
                MemoryStream strm = new MemoryStream();
                image.RePage();
                image.Quality = 100;
                image.Settings.SetDefine(MagickFormat.WebP, "lossless", true);
                image.Settings.SetDefine(MagickFormat.WebP, "method", "6");
                image.Write(strm, MagickFormat.WebP);
                file = new FormFile(strm, 0, strm.Length, file.Name, file.FileName);
            }

            return file;
        }

        /// <summary>
        /// Formats the image for use as a favicon.
        /// </summary>
        /// <param name="file">The file being formatted.</param>
        /// <returns>The formatted file.</returns>
        private IFormFile FormatFavIcon(IFormFile file)
        {
            using MemoryStream ms = new MemoryStream();
            file.CopyTo(ms);
            byte[] fileBytes = ms.ToArray();
            using (MagickImage image = new MagickImage(fileBytes))
            {
                image.Trim();
                image.VirtualPixelMethod = VirtualPixelMethod.Transparent;
                image.SetArtifact("distort:viewport", string.Concat(System.Math.Max(image.Width, image.Height).ToString(), 'x', System.Math.Max(image.Width, image.Height).ToString(), '-', System.Math.Max((image.Height - image.Width) / 2, 0).ToString(), '-', System.Math.Max((image.Width - image.Height) / 2, 0).ToString()));
                image.FilterType = FilterType.Point;
                image.Distort(DistortMethod.ScaleRotateTranslate, 0);
                image.Quality = 75;
                image.FilterType = FilterType.Catrom;
                image.Resize(64, 64);
                MemoryStream strm = new MemoryStream();
                image.RePage();
                image.Write(strm, MagickFormat.Png);
                file = new FormFile(strm, 0, strm.Length, file.Name, file.FileName);
            }

            return file;
        }
    }
}
