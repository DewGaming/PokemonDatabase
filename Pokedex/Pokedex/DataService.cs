using ImageMagick;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Pokedex.DataAccess.Models;
using Pokedex.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;

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

        public string GetPokemonFormName(int pokemonId)
        {
            return this.GetObjects<PokemonFormDetail>(includes: "Form")
                .FirstOrDefault(x => x.AltFormPokemonId == pokemonId).Form.Name;
        }

        public List<PokemonGameDetail> GetPokemonGameDetails(int pokemonId)
        {
            return this.GetObjects<PokemonGameDetail>("Game.GenerationId, GameId, Id", "Pokemon, Pokemon.Game, Game", "PokemonId", pokemonId);
        }

        public List<PokemonGameDetail> GetPokemonGameDetailsByGame(int gameId)
        {
            return this.GetObjects<PokemonGameDetail>(includes: "Pokemon, Game", whereProperty: "GameId", wherePropertyValue: gameId);
        }

        public Pokemon GetPokemon(string name)
        {
            return this.GetAllPokemon().Find(x => x.Name == name);
        }

        public Pokemon GetPokemonFromNameAndFormName(string pokemonName, string formName)
        {
            return this.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, OriginalPokemon, Form", whereProperty: "Form.Name", wherePropertyValue: formName).FirstOrDefault(x => x.AltFormPokemon.Name == pokemonName).AltFormPokemon;
        }

        public AllAdminPokemonViewModel GetAllAdminPokemonDetails()
        {
            return new AllAdminPokemonViewModel()
            {
                AllAltForms = this.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form"),
                AllEvolutions = this.GetObjects<Evolution>(includes: "PreevolutionPokemon, PreevolutionPokemon.Game, EvolutionPokemon, EvolutionPokemon.Game, EvolutionMethod, Generation"),
                AllTypings = this.GetObjects<PokemonTypeDetail>("PokemonId", "Pokemon, PrimaryType, SecondaryType"),
                AllAbilities = this.GetObjects<PokemonAbilityDetail>(includes: "Pokemon, PrimaryAbility, SecondaryAbility, HiddenAbility, SpecialEventAbility"),
                AllEggGroups = this.GetAllPokemonWithEggGroupsAndIncomplete(),
                AllBaseStats = this.GetObjects<BaseStat>(includes: "Pokemon"),
                AllEVYields = this.GetObjects<EVYield>(includes: "Pokemon"),
                AllLegendaryDetails = this.GetObjects<PokemonLegendaryDetail>(includes: "Pokemon, LegendaryType", whereProperty: "Pokemon.IsComplete", wherePropertyValue: true),
                AllPokemonCaptureRates = this.GetAllPokemonWithCaptureRates(),
                AllPokemonBaseHappinesses = this.GetAllPokemonWithBaseHappinesses(),
            };
        }

        public List<Pokemon> GetAllPokemon()
        {
            return this.GetObjects<Pokemon>("PokedexNumber, Id", "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth");
        }

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

        public List<PokemonTeam> GetAllPokemonTeams(string username)
        {
            return this.GetPokemonTeams().Where(x => x.User.Username == username).ToList();
        }

        public List<PokemonTeam> GetPokemonTeamsByUserId(int id)
        {
            return this.GetPokemonTeams().Where(x => x.User.Id == id).ToList();
        }

        public PokemonTeamDetail GetPokemonTeamDetail(int id)
        {
            return this.GetObjects<PokemonTeamDetail>(includes: "Pokemon, Pokemon.Game.Generation, Ability, PokemonTeamEV, PokemonTeamIV, PokemonTeamMoveset, BattleItem, Nature")
                .Find(x => x.Id == id);
        }

        public List<Pokemon> GetAltForms(int pokemonId)
        {
            List<Pokemon> pokemonList = new List<Pokemon>();
            List<PokemonFormDetail> pokemonFormList = new List<PokemonFormDetail>();
            if (this.CheckIfAltForm(pokemonId))
            {
                PokemonFormDetail formDetail = this.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.EggCycle, AltFormPokemon.GenderRatio, AltFormPokemon.Classification, AltFormPokemon.Game, AltFormPokemon.ExperienceGrowth, Form")
                    .Find(x => x.AltFormPokemonId == pokemonId);

                pokemonFormList = this.GetObjects<PokemonFormDetail>("AltFormPokemon.Game.ReleaseDate, AltFormPokemon.PokedexNumber, AltFormPokemon.Id", "AltFormPokemon, AltFormPokemon.EggCycle, AltFormPokemon.GenderRatio, AltFormPokemon.Classification, AltFormPokemon.Game, AltFormPokemon.ExperienceGrowth, Form").Where(x => x.OriginalPokemonId == formDetail.OriginalPokemonId && x.AltFormPokemonId != pokemonId).ToList();

                pokemonFormList.Add(formDetail);
            }
            else
            {
                pokemonFormList = this.GetObjects<PokemonFormDetail>("AltFormPokemon.Game.ReleaseDate, AltFormPokemon.PokedexNumber, AltFormPokemon.Id", "AltFormPokemon, AltFormPokemon.EggCycle, AltFormPokemon.GenderRatio, AltFormPokemon.Classification, AltFormPokemon.Game, AltFormPokemon.ExperienceGrowth, Form", "OriginalPokemonId", pokemonId);
            }

            foreach (var p in pokemonFormList)
            {
                pokemonList.Add(p.AltFormPokemon);
            }

            return pokemonList;
        }

        public Pokemon GetAltFormWithFormName(int pokemonId)
        {
            PokemonFormDetail pokemonForm = this.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, Form")
                .Find(x => x.AltFormPokemonId == pokemonId);

            Pokemon pokemon = pokemonForm.AltFormPokemon;

            pokemon.Name = string.Concat(pokemon.Name, " (", pokemonForm.Form.Name, ")");

            return pokemon;
        }

        public List<PokemonTypeDetail> GetAllPokemonWithTypes()
        {
            List<Pokemon> altFormList = this.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon").Select(x => x.AltFormPokemon).ToList();
            List<PokemonTypeDetail> pokemonList = this.GetObjects<PokemonTypeDetail>("Pokemon.PokedexNumber, PokemonId", "Pokemon, Pokemon.Game, PrimaryType, SecondaryType").Where(x => !altFormList.Any(y => y.Id == x.PokemonId)).ToList();
            List<int> pokemonIds = pokemonList.Select(x => x.PokemonId).Distinct().ToList();

            return pokemonList.OrderBy(x => x.GenerationId).GroupBy(x => new { x.PokemonId }).Select(x => x.LastOrDefault()).ToList();
        }

        public List<Ability> GetAbilitiesForPokemon(int pokemonId, int gameId)
        {
            if (gameId == 0)
            {
                gameId = this.GetObjects<Game>("ReleaseDate, Id").Where(x => x.ReleaseDate <= System.DateTime.Now).Last().Id;
            }

            Game game = this.GetObjectByPropertyValue<Game>("Id", gameId);
            List<Ability> abilityList = new List<Ability>();
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

            return abilityList;
        }

        public List<PokemonCaptureRateDetail> GetAllPokemonWithCaptureRates()
        {
            return this.GetObjects<PokemonCaptureRateDetail>(includes: "Pokemon, CaptureRate, Generation")
                .OrderByDescending(x => x.GenerationId)
                .ToList();
        }

        public List<PokemonBaseHappinessDetail> GetAllPokemonWithBaseHappinesses()
        {
            return this.GetObjects<PokemonBaseHappinessDetail>(includes: "Pokemon, BaseHappiness, Generation")
                .OrderByDescending(x => x.GenerationId)
                .ToList();
        }

        public List<PokemonCaptureRateDetail> GetPokemonWithCaptureRates(int pokemonId)
        {
            return this.GetObjects<PokemonCaptureRateDetail>(includes: "Pokemon, CaptureRate, Generation", whereProperty: "Pokemon.Id", wherePropertyValue: pokemonId)
                .OrderByDescending(x => x.GenerationId)
                .ToList();
        }

        public List<PokemonEggGroupDetail> GetAllPokemonWithEggGroupsAndIncomplete()
        {
            return this.GetObjects<PokemonEggGroupDetail>(includes: "Pokemon, PrimaryEggGroup, SecondaryEggGroup");
        }

        public BaseStat GetPokemonBaseStats(int pokemonId, int generationId)
        {
            return this.GetObjects<BaseStat>(includes: "Pokemon")
                .ToList()
                .Find(x => x.PokemonId == pokemonId && x.GenerationId == generationId);
        }

        public EVYield GetPokemonEVYields(int pokemonId, int generationId)
        {
            return this.GetObjects<EVYield>(includes: "Pokemon")
                .ToList()
                .Find(x => x.PokemonId == pokemonId && x.GenerationId == generationId);
        }

        public List<BaseStat> GetBaseStat(int pokemonId)
        {
            return this.GetObjects<BaseStat>(includes: "Pokemon", whereProperty: "Pokemon.Id", wherePropertyValue: pokemonId);
        }

        public List<EVYield> GetEVYields(int pokemonId)
        {
            return this.GetObjects<EVYield>(includes: "Pokemon", whereProperty: "Pokemon.Id", wherePropertyValue: pokemonId);
        }

        public List<TypeChart> GetTypeCharts()
        {
            return this.GetObjects<TypeChart>("AttackId, DefendId", "Attack, Defend");
        }

        public List<TypeChart> GetTypeChartByDefendType(int id, int genId)
        {
            return this.GetObjects<TypeChart>("AttackId, DefendId", "Attack, Defend")
                .Where(x => x.DefendId == id && x.GenerationId == genId)
                .ToList();
        }

        public List<PokemonTypeChartViewModel> GetTypeChartPokemon(int pokemonId)
        {
            List<Type> typeList = this.GetObjects<Type>("Name");
            List<PokemonTypeDetail> pokemonTypes = this.GetObjects<PokemonTypeDetail>(includes: "Pokemon, PrimaryType, SecondaryType, Generation", whereProperty: "PokemonId", wherePropertyValue: pokemonId);
            List<TypeChart> typeChart = this.GetTypeCharts();
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
        /// Formats the image for use as a main page image.
        /// </summary>
        /// <param name="file">The file being formatted.</param>
        /// <returns>The formatted file.</returns>
        public IFormFile TrimImage(IFormFile file)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                file.CopyTo(ms);
                byte[] fileBytes = ms.ToArray();
                using MagickImage image = new MagickImage(fileBytes);
                image.Trim();
                MemoryStream strm = new MemoryStream();
                image.RePage();
                image.Write(strm, MagickFormat.Png);
                file = new FormFile(strm, 0, strm.Length, file.Name, file.FileName);
            }

            return file;
        }

        /// <summary>
        /// Formats the image for use as a favicon.
        /// </summary>
        /// <param name="file">The file being formatted.</param>
        /// <returns>The formatted file.</returns>
        public IFormFile FormatFavIcon(IFormFile file)
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
