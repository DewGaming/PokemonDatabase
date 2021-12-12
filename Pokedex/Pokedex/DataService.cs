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

                if (pageStat != null)
                {
                    pageStat.ViewCount += 1;
                    pageStat.LastVisit = System.DateTime.Now.ToUniversalTime();
                    this.UpdateObject(pageStat);
                }
                else
                {
                    this.AddObject(new PageStat() { Name = pageName, ViewCount = 1, LastVisit = System.DateTime.Now.ToUniversalTime() });
                }
            }
        }

        /// <summary>
        /// Resets the view counts of all current pages.
        /// </summary>
        public void ClearPageViews()
        {
            foreach (var p in this.GetObjects<PageStat>())
            {
                p.ViewCount = 0;
                this.UpdateObject(p);
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
            return this.dataContext.PokemonFormDetails.ToList().Exists(x => x.AltFormPokemonId == id);
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
            return this.dataContext.PokemonFormDetails
                .Include(x => x.Form)
                .FirstOrDefault(x => x.AltFormPokemonId == pokemonId).Form.Name;
        }

        public List<PokemonGameDetail> GetPokemonGameDetails(int pokemonId)
        {
            return this.dataContext.PokemonGameDetails
                .Include(x => x.Pokemon)
                    .Include("Pokemon.Game")
                .Include(x => x.Game)
                .Where(x => x.PokemonId == pokemonId)
                .OrderBy(x => x.Game.GenerationId)
                .ThenBy(x => x.GameId)
                .ThenBy(x => x.Id)
                .ToList();
        }

        public List<PokemonGameDetail> GetPokemonGameDetailsByGame(int gameId)
        {
            return this.dataContext.PokemonGameDetails
                .Include(x => x.Pokemon)
                .Include(x => x.Game)
                .Where(x => x.GameId == gameId)
                .ToList();
        }

        public Pokemon GetPokemon(string name)
        {
            return this.GetAllPokemon().Find(x => x.Name == name);
        }

        public Pokemon GetPokemonFromNameAndFormName(string pokemonName, string formName)
        {
            return this.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, OriginalPokemon, Form", whereProperty: "Form.Name", wherePropertyValue: formName).FirstOrDefault(x => x.AltFormPokemon.Name == pokemonName).AltFormPokemon;
        }

        public PokemonViewModel GetPokemonDetails(Pokemon pokemon, Form form, AppConfig appConfig)
        {
            List<PokemonLocationDetail> pokemonLocationDetails = this.GetObjects<PokemonLocationDetail>().Where(x => x.PokemonId == pokemon.Id).ToList();
            PokemonViewModel pokemonViewModel = new PokemonViewModel()
            {
                Pokemon = pokemon,
                BaseStats = this.GetBaseStat(pokemon.Id),
                EVYields = this.GetEVYields(pokemon.Id),
                Typings = this.GetObjects<PokemonTypeDetail>(includes: "Pokemon, PrimaryType, SecondaryType, Generation", whereProperty: "PokemonId", wherePropertyValue: pokemon.Id),
                Abilities = this.GetObjects<PokemonAbilityDetail>(includes: "Pokemon, PrimaryAbility, SecondaryAbility, HiddenAbility, SpecialEventAbility", whereProperty: "PokemonId", wherePropertyValue: pokemon.Id),
                EggGroups = this.GetObjects<PokemonEggGroupDetail>(includes: "Pokemon, PrimaryEggGroup, SecondaryEggGroup", whereProperty: "PokemonId", wherePropertyValue: pokemon.Id),
                CaptureRates = this.GetPokemonWithCaptureRates(pokemon.Id),
                PreEvolutions = this.GetPreEvolution(pokemon.Id).Where(x => x.PreevolutionPokemon.IsComplete).ToList(),
                Evolutions = this.GetPokemonEvolutions(pokemon.Id).Where(x => x.PreevolutionPokemon.IsComplete && x.EvolutionPokemon.IsComplete).ToList(),
                Effectiveness = this.GetTypeChartPokemon(pokemon.Id),
                GamesAvailableIn = this.GetPokemonGameDetails(pokemon.Id).ConvertAll(x => x.Game),
                PokemonLocationGameDetails = this.GetObjects<PokemonLocationGameDetail>().Where(x => pokemonLocationDetails.Any(y => y.Id == x.PokemonLocationDetailId)).ToList(),
                AppConfig = appConfig,
            };

            if (form != null)
            {
                pokemonViewModel.Form = form;
                pokemonViewModel.Pokemon.Name = string.Concat(pokemonViewModel.Pokemon.Name, " (", form.Name, ")");
            }

            PokemonLegendaryDetail legendaryType = this.GetObjectByPropertyValue<PokemonLegendaryDetail>("PokemonId", pokemon.Id, "LegendaryType");

            if (legendaryType != null)
            {
                pokemonViewModel.LegendaryType = legendaryType.LegendaryType;
            }

            return pokemonViewModel;
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
            };
        }

        public List<Pokemon> GetAllPokemon()
        {
            return this.dataContext.Pokemon
                .Include(x => x.EggCycle)
                .Include(x => x.GenderRatio)
                .Include(x => x.Classification)
                .Include(x => x.Game)
                    .Include("Game.Generation")
                .Include(x => x.ExperienceGrowth)
                .Include(x => x.BaseHappiness)
                .Where(x => x.IsComplete)
                .OrderBy(x => x.PokedexNumber)
                .ThenBy(x => x.Id)
                .ToList();
        }

        public List<Pokemon> GetAllPokemonWithIncompleteWithFormNames()
        {
            List<Pokemon> pokemonList = this.dataContext.Pokemon
                .Include(x => x.EggCycle)
                .Include(x => x.GenderRatio)
                .Include(x => x.Classification)
                .Include(x => x.Game)
                    .Include("Game.Generation")
                .Include(x => x.ExperienceGrowth)
                .Include(x => x.BaseHappiness)
                .OrderBy(x => x.PokedexNumber)
                .ThenBy(x => x.Id)
                .ToList();

            List<Pokemon> altFormList = this.GetAllAltFormsWithFormName();
            Pokemon pokemon;

            foreach (var a in altFormList)
            {
                pokemon = pokemonList.Find(x => x.Id == a.Id);
                pokemon.Name = a.Name;
            }

            return pokemonList;
        }

        public List<Pokemon> GetPokemonForLocation(int locationId)
        {
            Region region = this.GetObjectByPropertyValue<Location>("Id", locationId, "Region").Region;
            List<Game> games = this.GetObjects<GameRegionDetail>(includes: "Game").Where(x => x.RegionId == region.Id).Select(x => x.Game).ToList();
            List<Pokemon> pokemonList = new List<Pokemon>();
            foreach (var g in games)
            {
                pokemonList.AddRange(this.GetPokemonGameDetailsByGame(g.Id).Select(x => x.Pokemon).ToList());
            }

            pokemonList = pokemonList.GroupBy(x => x.Id).Select(x => x.First()).OrderBy(x => x.PokedexNumber).ToList();

            List<Pokemon> altFormList = this.GetAllAltFormsWithFormName().Where(x => x.IsComplete == true).ToList();
            Pokemon pokemon;

            foreach (var a in altFormList)
            {
                if (pokemonList.Find(x => x.Id == a.Id) != null)
                {
                    pokemon = pokemonList.Find(x => x.Id == a.Id);
                    pokemon.Name = a.Name;
                }
            }

            return pokemonList;
        }

        public List<PokemonEggGroupDetail> GetAllBreedablePokemon()
        {
            List<Pokemon> pokemonList = this.dataContext.Pokemon
                .Where(x => x.IsComplete)
                .ToList();

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

        public List<Pokemon> GetAllPokemonWithoutForms()
        {
            List<Pokemon> pokemonList = this.GetAllPokemon();
            List<Pokemon> altFormList = this.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form").ConvertAll(x => x.AltFormPokemon);
            return pokemonList.Where(x => !altFormList.Any(y => y.Id == x.Id)).ToList();
        }

        public List<Pokemon> GetAllPokemonWithoutFormsWithIncomplete()
        {
            List<Pokemon> pokemonList = this.GetObjects<Pokemon>("PokedexNumber, Id", "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth, BaseHappiness");
            List<Pokemon> altFormList = this.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form").ConvertAll(x => x.AltFormPokemon);
            return pokemonList.Where(x => !altFormList.Any(y => y.Id == x.Id)).ToList();
        }

        public List<int> GetGenerationsFromPokemon()
        {
            List<Pokemon> pokemonList = this.GetAllPokemon();
            List<Pokemon> altFormList = this.dataContext.PokemonFormDetails.Select(x => x.AltFormPokemon).ToList();
            pokemonList = pokemonList.Where(x => !altFormList.Any(y => y.Id == x.Id)).ToList();

            List<int> generationList = pokemonList.Select(x => x.Game.GenerationId).Distinct().OrderBy(x => x).ToList();

            return generationList;
        }

        public List<int> GetGenerationsFromPokemonWithIncomplete()
        {
            List<Pokemon> pokemonList = this.GetObjects<Pokemon>("PokedexNumber, Id", "EggCycle, GenderRatio, Classification, Game, Game.Generation, ExperienceGrowth, BaseHappiness");
            List<Pokemon> altFormList = this.dataContext.PokemonFormDetails.Select(x => x.AltFormPokemon).ToList();
            pokemonList = pokemonList.Where(x => !altFormList.Any(y => y.Id == x.Id)).ToList();

            List<int> generationList = pokemonList.Select(x => x.Game.GenerationId).Distinct().OrderBy(x => x).ToList();

            return generationList;
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
                .ToList();
        }

        public List<PokemonTeam> GetAllPokemonTeams(string username)
        {
            return this.GetPokemonTeams().Where(x => x.User.Username == username).OrderBy(x => x.Id).ToList();
        }

        public List<PokemonTeam> GetPokemonTeamsByUserId(int id)
        {
            return this.GetPokemonTeams()
                .Where(x => x.User.Id == id)
                .ToList();
        }

        public PokemonTeam GetPokemonTeamFromPokemon(int id)
        {
            return this.dataContext.PokemonTeams
                .Include(x => x.FirstPokemon)
                .Include(x => x.SecondPokemon)
                .Include(x => x.ThirdPokemon)
                .Include(x => x.FourthPokemon)
                .Include(x => x.FifthPokemon)
                .Include(x => x.SixthPokemon)
                .Include(x => x.User)
                .ToList()
                .Find(x => x.FirstPokemonId == id ||
                           x.SecondPokemonId == id ||
                           x.ThirdPokemonId == id ||
                           x.FourthPokemonId == id ||
                           x.FifthPokemonId == id ||
                           x.SixthPokemonId == id);
        }

        public PokemonTeam GetPokemonTeamFromPokemonNoIncludes(int id)
        {
            return this.dataContext.PokemonTeams
                .ToList()
                .Find(x => x.FirstPokemonId == id ||
                           x.SecondPokemonId == id ||
                           x.ThirdPokemonId == id ||
                           x.FourthPokemonId == id ||
                           x.FifthPokemonId == id ||
                           x.SixthPokemonId == id);
        }

        public PokemonTeamDetail GetPokemonTeamDetail(int id)
        {
            return this.dataContext.PokemonTeamDetails
                .Include(x => x.Pokemon)
                    .Include("Pokemon.Game.Generation")
                .Include(x => x.Ability)
                .Include(x => x.PokemonTeamEV)
                .Include(x => x.PokemonTeamIV)
                .Include(x => x.PokemonTeamMoveset)
                .Include(x => x.BattleItem)
                .Include(x => x.Nature)
                .ToList()
                .Find(x => x.Id == id);
        }

        public List<PokemonTeamDetail> GetPokemonTeamDetails()
        {
            return this.dataContext.PokemonTeamDetails
                .Include(x => x.Pokemon)
                    .Include("Pokemon.Game.Generation")
                .Include(x => x.Ability)
                .Include(x => x.PokemonTeamEV)
                .Include(x => x.PokemonTeamIV)
                .Include(x => x.PokemonTeamMoveset)
                .Include(x => x.BattleItem)
                .Include(x => x.Nature)
                .ToList();
        }

        public List<Pokemon> GetAltForms(int pokemonId)
        {
            List<PokemonFormDetail> pokemonFormList = this.dataContext.PokemonFormDetails
                .Include(x => x.AltFormPokemon)
                    .Include("AltFormPokemon.EggCycle")
                    .Include("AltFormPokemon.GenderRatio")
                    .Include("AltFormPokemon.Classification")
                    .Include("AltFormPokemon.Game")
                    .Include("AltFormPokemon.ExperienceGrowth")
                    .Include("AltFormPokemon.BaseHappiness")
                .Include(x => x.Form)
                .OrderBy(x => x.AltFormPokemon.Game.ReleaseDate)
                .ThenBy(x => x.AltFormPokemon.PokedexNumber)
                .ThenBy(x => x.AltFormPokemon.Id)
                .Where(x => x.OriginalPokemonId == pokemonId).ToList();
            List<Pokemon> pokemonList = new List<Pokemon>();
            foreach (var p in pokemonFormList)
            {
                pokemonList.Add(p.AltFormPokemon);
            }

            return pokemonList;
        }

        public List<Pokemon> GetAltFormsNoIncludes(int pokemonId)
        {
            List<PokemonFormDetail> pokemonFormList = this.dataContext.PokemonFormDetails
                .Include(x => x.AltFormPokemon)
                .Include(x => x.Form)
                .OrderBy(x => x.AltFormPokemon.Game.ReleaseDate)
                .ThenBy(x => x.AltFormPokemon.PokedexNumber)
                .ThenBy(x => x.AltFormPokemon.Id)
                .Where(x => x.OriginalPokemonId == pokemonId).ToList();
            List<Pokemon> pokemonList = new List<Pokemon>();
            foreach (var p in pokemonFormList)
            {
                pokemonList.Add(p.AltFormPokemon);
            }

            return pokemonList;
        }

        public Pokemon GetAltFormWithFormName(int pokemonId)
        {
            PokemonFormDetail pokemonForm = this.dataContext.PokemonFormDetails
                .Include(x => x.AltFormPokemon)
                .Include(x => x.Form)
                .ToList()
                .Find(x => x.AltFormPokemonId == pokemonId);

            Pokemon pokemon = pokemonForm.AltFormPokemon;

            pokemon.Name = string.Concat(pokemon.Name, " (", pokemonForm.Form.Name, ")");

            return pokemon;
        }

        public List<Pokemon> GetAllAltFormsWithFormName()
        {
            List<PokemonFormDetail> pokemonForm = this.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, Form");

            List<Pokemon> pokemonList = pokemonForm.ConvertAll(x => x.AltFormPokemon);

            foreach (var p in pokemonForm)
            {
                p.AltFormPokemon.Name = string.Concat(p.AltFormPokemon.Name, " (", p.Form.Name, ")");
                pokemonList.Add(p.AltFormPokemon);
            }

            return pokemonList;
        }

        public List<Pokemon> GetAllPokemonOnlyForms()
        {
            List<Pokemon> pokemonList = this.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form").Select(x => x.AltFormPokemon).OrderBy(x => x.PokedexNumber).ToList();

            foreach (var p in pokemonList)
            {
                p.Name = string.Concat(p.Name, " (", this.GetPokemonFormName(p.Id), ")");
            }

            return pokemonList;
        }

        public List<PokemonTypeDetail> GetAllPokemonWithTypes()
        {
            List<Pokemon> altFormList = this.dataContext.PokemonFormDetails.Select(x => x.AltFormPokemon).ToList();
            List<PokemonTypeDetail> pokemonList = this.GetObjects<PokemonTypeDetail>("Pokemon.PokedexNumber, PokemonId", "Pokemon, Pokemon.Game, PrimaryType, SecondaryType", "Pokemon.IsComplete", true).Where(x => !altFormList.Any(y => y.Id == x.PokemonId)).ToList();
            List<int> pokemonIds = pokemonList.Select(x => x.PokemonId).Distinct().ToList();

            return pokemonList.OrderBy(x => x.GenerationId).GroupBy(x => new { x.PokemonId }).Select(x => x.LastOrDefault()).ToList();
        }

        public List<PokemonTypeDetail> GetAllPokemonWithSpecificTypes(int primaryTypeId, int secondaryTypeId, int generationId)
        {
            List<PokemonTypeDetail> pokemonList = this.GetObjects<PokemonTypeDetail>("GenerationId", "Pokemon, Pokemon.Game, PrimaryType, SecondaryType", "Pokemon.IsComplete", true)
                                                        .Where(x => x.Pokemon.Game.GenerationId <= generationId)
                                                        .Where(x => x.GenerationId <= generationId)
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

            List<int> exclusionList = pokemonList.Select(x => x.PokemonId).Except(this.GetObjects<PokemonGameDetail>(includes: "Pokemon, Game, Game.Generation", whereProperty: "Game.GenerationId", wherePropertyValue: generationId).Select(x => x.PokemonId)).ToList();

            foreach (var pokemon in exclusionList)
            {
                pokemonList.Remove(pokemonList.Find(x => x.PokemonId == pokemon));
            }

            return pokemonList.OrderBy(x => x.Pokemon.PokedexNumber).ToList();
        }

        public List<Ability> GetAbilitiesForPokemon(int pokemonId)
        {
            List<Ability> abilityList = new List<Ability>();
            PokemonAbilityDetail pokemonAbilityDetail = this.GetObjectByPropertyValue<PokemonAbilityDetail>("PokemonId", pokemonId, "Pokemon, PrimaryAbility, SecondaryAbility, HiddenAbility, SpecialEventAbility");

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

        public List<PokemonAbilityDetail> GetPokemonByAbility(int abilityId, int generationId)
        {
            List<PokemonAbilityDetail> pokemonList = this.GetObjects<PokemonAbilityDetail>(includes: "Pokemon, Pokemon.Game", whereProperty: "Pokemon.IsComplete", wherePropertyValue: true)
                .Where(x => x.Pokemon.Game.GenerationId <= generationId)
                .Where(x => x.GenerationId <= generationId)
                .OrderBy(x => x.GenerationId)
                .GroupBy(x => new { x.PokemonId })
                .Select(x => x.LastOrDefault())
                .ToList();

            List<int> exclusionList = pokemonList.Select(x => x.PokemonId).Except(this.GetObjects<PokemonGameDetail>(includes: "Pokemon, Game, Game.Generation", whereProperty: "Game.GenerationId", wherePropertyValue: generationId).Select(x => x.PokemonId)).ToList();

            foreach (var pokemon in exclusionList)
            {
                pokemonList.Remove(pokemonList.Find(x => x.PokemonId == pokemon));
            }

            return pokemonList.Where(x => x.PrimaryAbilityId == abilityId || x.SecondaryAbilityId == abilityId || x.HiddenAbilityId == abilityId || x.SpecialEventAbilityId == abilityId).OrderBy(x => x.Pokemon.PokedexNumber).ThenBy(x => x.PokemonId).ToList();
        }

        public PokemonAbilityDetail GetPokemonWithAbilitiesNoIncludes(int pokemonId, int generationId)
        {
            return this.dataContext.PokemonAbilityDetails
                .Include(x => x.Pokemon)
                .FirstOrDefault(x => x.PokemonId == pokemonId && x.GenerationId == generationId);
        }

        public List<PokemonCaptureRateDetail> GetAllPokemonWithCaptureRates()
        {
            return this.dataContext.PokemonCaptureRateDetails
                .Include(x => x.Pokemon)
                .Include(x => x.CaptureRate)
                .Include(x => x.Generation)
                .OrderByDescending(x => x.GenerationId)
                .ToList();
        }

        public List<PokemonCaptureRateDetail> GetPokemonWithCaptureRates(int pokemonId)
        {
            return this.dataContext.PokemonCaptureRateDetails
                .Include(x => x.Pokemon)
                .Include(x => x.CaptureRate)
                .Include(x => x.Generation)
                .Where(x => x.Pokemon.Id == pokemonId)
                .OrderByDescending(x => x.GenerationId)
                .ToList();
        }

        public PokemonCaptureRateDetail GetPokemonWithCaptureRatesFromGenerationId(int pokemonId, int generationId)
        {
            return this.dataContext.PokemonCaptureRateDetails
                .Include(x => x.Pokemon)
                .Include(x => x.CaptureRate)
                .Include(x => x.Generation)
                .ToList()
                .First(x => x.Pokemon.Id == pokemonId && x.Generation.Id == generationId);
        }

        public List<PokemonEggGroupDetail> GetAllPokemonWithSpecificEggGroups(int primaryEggGroupId, int? secondaryEggGroupId)
        {
            List<PokemonEggGroupDetail> pokemonList = this.dataContext.PokemonEggGroupDetails
                .Include(x => x.Pokemon)
                    .Include("Pokemon.GenderRatio")
                .Include(x => x.PrimaryEggGroup)
                .Include(x => x.SecondaryEggGroup)
                .ToList();

            pokemonList = pokemonList.OrderBy(x => x.GenerationId).GroupBy(x => new { x.PokemonId }).Select(x => x.LastOrDefault()).ToList();

            List<PokemonEggGroupDetail> finalPokemonList = pokemonList.Where(x => x.PrimaryEggGroupId == primaryEggGroupId || x.SecondaryEggGroupId == primaryEggGroupId).ToList();

            if (secondaryEggGroupId != null)
            {
                finalPokemonList.AddRange(pokemonList.Where(x => x.PrimaryEggGroupId == secondaryEggGroupId || x.SecondaryEggGroupId == secondaryEggGroupId).ToList());
            }

            return finalPokemonList.Distinct().ToList();
        }

        public List<Pokemon> GetSurroundingPokemon(int pokedexNumber)
        {
            List<Pokemon> surroundingPokemon = new List<Pokemon>();
            List<int> ids = this.GetAllPokemon().ConvertAll(x => x.Id);
            List<int> altIds = this.GetObjects<PokemonFormDetail>(includes: "AltFormPokemon, AltFormPokemon.Game, OriginalPokemon, OriginalPokemon.Game, Form").ConvertAll(x => x.AltFormPokemonId);
            List<int> pokemonIds = ids.Where(x => !altIds.Any(y => y == x)).ToList();
            int previousId, nextId, index = pokemonIds.FindIndex(x => x == pokedexNumber);

            if (pokemonIds[index] == pokemonIds[0])
            {
                previousId = pokemonIds.Last();
            }
            else
            {
                previousId = pokemonIds[index - 1];
            }

            if (pokemonIds[index] == pokemonIds.Last())
            {
                nextId = pokemonIds[0];
            }
            else
            {
                nextId = pokemonIds[index + 1];
            }

            surroundingPokemon.Add(this.GetObjectByPropertyValue<Pokemon>("Id", previousId));
            surroundingPokemon.Add(this.GetObjectByPropertyValue<Pokemon>("Id", nextId));

            return surroundingPokemon;
        }

        public List<PokemonEggGroupDetail> GetAllPokemonWithEggGroupsAndIncomplete()
        {
            return this.dataContext.PokemonEggGroupDetails
                .Include(x => x.Pokemon)
                .Include(x => x.PrimaryEggGroup)
                .Include(x => x.SecondaryEggGroup)
                .ToList();
        }

        public BaseStat GetPokemonBaseStats(int pokemonId, int generationId)
        {
            return this.dataContext.BaseStats
                .Include(x => x.Pokemon)
                .ToList()
                .Find(x => x.PokemonId == pokemonId && x.GenerationId == generationId);
        }

        public EVYield GetPokemonEVYields(int pokemonId, int generationId)
        {
            return this.dataContext.EVYields
                .Include(x => x.Pokemon)
                .ToList()
                .Find(x => x.PokemonId == pokemonId && x.GenerationId == generationId);
        }

        public List<BaseStat> GetBaseStat(int pokemonId)
        {
            return this.dataContext.BaseStats
                .Include(x => x.Pokemon)
                .Where(x => x.Pokemon.Id == pokemonId)
                .ToList();
        }

        public List<EVYield> GetEVYields(int pokemonId)
        {
            return this.dataContext.EVYields
                .Include(x => x.Pokemon)
                .Where(x => x.Pokemon.Id == pokemonId)
                .ToList();
        }

        public List<TypeChart> GetTypeCharts()
        {
            return this.dataContext.TypeCharts
                .Include(x => x.Attack)
                .Include(x => x.Defend)
                .OrderBy(x => x.AttackId)
                .ThenBy(x => x.DefendId)
                .ToList();
        }

        public List<TypeChart> GetAllTypeChartByAttackType(int id)
        {
            return this.dataContext.TypeCharts
                .Include(x => x.Attack)
                .Include(x => x.Defend)
                .Where(x => x.AttackId == id)
                .OrderBy(x => x.AttackId)
                .ThenBy(x => x.DefendId)
                .ToList();
        }

        public List<TypeChart> GetAllTypeChartByDefendType(int id)
        {
            return this.dataContext.TypeCharts
                .Include(x => x.Attack)
                .Include(x => x.Defend)
                .Where(x => x.DefendId == id)
                .OrderBy(x => x.AttackId)
                .ThenBy(x => x.DefendId)
                .ToList();
        }

        public List<TypeChart> GetTypeChartByDefendType(int id, int genId)
        {
            return this.dataContext.TypeCharts
                .Include(x => x.Attack)
                .Include(x => x.Defend)
                .Where(x => x.DefendId == id && x.GenerationId == genId)
                .OrderBy(x => x.AttackId)
                .ThenBy(x => x.DefendId)
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
        /// Gets the type chart for the given typing and generation.
        /// </summary>
        /// <param name="primaryTypeId">The id of the primary type.</param>
        /// <param name="secondaryTypeId">The id of the secondary type.</param>
        /// <param name="generationId">The id of the generation.</param>
        /// <returns>Returns the type effectiveness given the typing and generation.</returns>
        public TypeEffectivenessViewModel GetTypeChartTyping(int primaryTypeId, int secondaryTypeId, int generationId)
        {
            List<Type> typeList = this.GetObjects<Type>("Name");
            List<Type> pokemonTypes = new List<Type>();
            List<string> strongAgainst = new List<string>();
            List<string> superStrongAgainst = new List<string>();
            List<string> weakAgainst = new List<string>();
            List<string> superWeakAgainst = new List<string>();
            List<string> immuneTo = new List<string>();
            List<TypeChart> typeChart;
            string effectiveValue, attackType;

            pokemonTypes.Add(this.GetObjectByPropertyValue<Type>("Id", primaryTypeId));

            if (secondaryTypeId != 0)
            {
                pokemonTypes.Add(this.GetObjectByPropertyValue<Type>("Id", secondaryTypeId));
            }

            foreach (var type in pokemonTypes)
            {
                typeChart = this.dataContext.TypeCharts
                    .Include(x => x.Attack)
                    .Include(x => x.Defend)
                    .Where(x => x.Defend == type)
                    .ToList();

                List<int> generations = typeChart.Select(x => x.GenerationId).Distinct().OrderByDescending(x => x).ToList();
                typeChart = typeChart.Where(x => x.GenerationId == generations.First(x => x <= generationId)).ToList();

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

        /// <summary>
        /// Gets the list of pages able to be commented on.
        /// </summary>
        /// <returns>The list of commentable pages.</param>
        public List<CommentPage> GetCommentPages()
        {
            List<CommentPage> pages = this.dataContext.CommentPages.ToList();
            List<CommentPage> pagesToBeMoved = pages.Where(x => x.Name.Contains("(Need to login to see)")).ToList();

            foreach (var p in pagesToBeMoved)
            {
                pages.Remove(p);
            }

            pages.AddRange(pagesToBeMoved);

            pagesToBeMoved = new List<CommentPage>
            {
                pages.Find(x => x.Name == "New Page"),
                pages.Find(x => x.Name == "Other"),
            };

            foreach (var p in pagesToBeMoved)
            {
                pages.Remove(p);
            }

            pages.AddRange(pagesToBeMoved);

            return pages;
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
        /// Deletes a game and all game region details.
        /// </summary>
        /// <param name="id">The id of the game being removed.</param>
        public void DeleteGame(int id)
        {
            Game game = this.GetObjectByPropertyValue<Game>("Id", id);
            List<GameRegionDetail> gameRegionDetails = this.GetObjects<GameRegionDetail>().Where(x => x.GameId == id).ToList();
            foreach (var r in gameRegionDetails)
            {
                this.DeleteObject<GameRegionDetail>(r.Id);
            }

            this.DeleteObject<Game>(game.Id);
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
