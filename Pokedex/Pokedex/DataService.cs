using ImageMagick;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Pokedex.DataAccess.Models;
using Pokedex.Models;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

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
        /// Checks to see if the provided pokemon id is an alternate form of another pokemon.
        /// </summary>
        /// <param name="id">The pokemon's id.</param>
        /// <returns>Returns whether the pokemon is an alternate form or not.</returns>
        public bool CheckIfAltForm(int id)
        {
            return this.dataContext.PokemonFormDetails.ToList().Exists(x => x.AltFormPokemonId == id);
        }

        /// <summary>
        /// Gets the ability with the given id.
        /// </summary>
        /// <param name="id">The ability's id.</param>
        /// <returns>The requested ability.</returns>
        public Ability GetAbility(int id)
        {
            return this.dataContext.Abilities
                .ToList()
                .Find(x => x.Id == id);
        }

        /// <summary>
        /// Gets the ability with the given ability name.
        /// </summary>
        /// <param name="name">The ability's name.</param>
        /// <returns>The requested ability.</returns>
        public Ability GetAbilityByName(string name)
        {
            return this.dataContext.Abilities
                .ToList()
                .Find(x => x.Name == name);
        }

        /// <summary>
        /// Gets a list of abilities, ordered by the ability names.
        /// </summary>
        /// <returns>The list of all abilities.</returns>
        public List<Ability> GetAbilities()
        {
            return this.dataContext.Abilities.OrderBy(x => x.Name).ToList();
        }

        /// <summary>
        /// Gets the game with the given id.
        /// </summary>
        /// <param name="id">The game's id.</param>
        /// <returns>The requested game.</returns>
        public Game GetGame(int id)
        {
            return this.dataContext.Games
                .ToList()
                .Find(x => x.Id == id);
        }

        /// <summary>
        /// Gets all games.
        /// </summary>
        /// <returns>A list of all games.</returns>
        public List<Game> GetGames()
        {
            return this.dataContext.Games
                .OrderBy(x => x.ReleaseDate)
                .ThenBy(x => x.Id)
                .ToList();
        }

        public List<Game> GetGamesForEachReleaseDate()
        {
            List<Game> gameList = this.GetGames();
            List<Game> games = new List<Game>();
            foreach (var r in gameList.ConvertAll(x => x.ReleaseDate).Distinct())
            {
                if (gameList.First(x => x.ReleaseDate == r).Id != 4)
                {
                    games.Add(new Game()
                    {
                        Id = gameList.First(x => x.ReleaseDate == r).Id,
                        Name = string.Join(" / ", gameList.Where(x => x.ReleaseDate == r).Select(x => x.Name)),
                        GenerationId = gameList.First(x => x.ReleaseDate == r).GenerationId,
                        ReleaseDate = r,
                        Abbreviation = string.Concat(gameList.Where(x => x.ReleaseDate == r).Select(x => x.Abbreviation)),
                    });
                }
                else
                {
                    foreach (var g in gameList.Where(x => x.ReleaseDate == r).ToList())
                    {
                        games.Add(g);
                    }
                }
            }

            return games;
        }

        /// <summary>
        /// Gets a specific legendary type.
        /// </summary>
        /// <param name="id">The id of the legendary type.</param>
        /// <returns>The specified legendary type.</returns>
        public LegendaryType GetLegendaryType(int id)
        {
            return this.dataContext.LegendaryTypes.OrderBy(x => x.Type).ToList().Find(x => x.Id == id);
        }

        /// <summary>
        /// Gets all legendary types.
        /// </summary>
        /// <returns>A list of all legendary types.</returns>
        public List<LegendaryType> GetLegendaryTypes()
        {
            return this.dataContext.LegendaryTypes.OrderBy(x => x.Type).ToList();
        }

        public PokemonLegendaryDetail GetLegendaryDetail(int pokemonId)
        {
            return this.dataContext.PokemonLegendaryDetails
                .Include(x => x.LegendaryType)
                .ToList()
                .Find(x => x.PokemonId == pokemonId);
        }

        public Type GetType(int id)
        {
            return this.dataContext.Types
                .ToList()
                .Find(x => x.Id == id);
        }

        public TypeChart GetTypeChart(int id)
        {
            return this.dataContext.TypeCharts
                .ToList()
                .Find(x => x.Id == id);
        }

        public List<Type> GetTypes()
        {
            return this.dataContext.Types.OrderBy(x => x.Name).ToList();
        }

        public FormItem GetFormItem(int id)
        {
            return this.dataContext.FormItems
                .ToList()
                .Find(x => x.Id == id);
        }

        public FormItem GetFormItemByPokemonId(int id)
        {
            return this.dataContext.FormItems
                .ToList()
                .Find(x => x.PokemonId == id);
        }

        public List<FormItem> GetFormItems()
        {
            List<FormItem> formItemList = this.dataContext.FormItems
                .Include(x => x.Pokemon)
                .OrderBy(x => x.Pokemon.PokedexNumber)
                .ToList();

            foreach (var f in formItemList)
            {
                f.Pokemon.Name = string.Concat(f.Pokemon.Name, " (", this.GetPokemonFormName(f.PokemonId), ")");
            }

            return formItemList;
        }

        public EggGroup GetEggGroup(int id)
        {
            return this.dataContext.EggGroups
                .ToList()
                .Find(x => x.Id == id);
        }

        public Status GetStatus(int id)
        {
            return this.dataContext.Statuses.ToList().Find(x => x.Id == id);
        }

        public List<Status> GetStatuses()
        {
            return this.dataContext.Statuses.OrderBy(x => x.Name).ToList();
        }

        public List<EggGroup> GetEggGroups()
        {
            return this.dataContext.EggGroups.OrderBy(x => x.Name).ToList();
        }

        public List<Evolution> GetEvolutions()
        {
            return this.dataContext.Evolutions
                .Include(x => x.PreevolutionPokemon)
                    .Include("PreevolutionPokemon.Game")
                .Include(x => x.EvolutionPokemon)
                    .Include("EvolutionPokemon.Game")
                .Include(x => x.EvolutionMethod)
                .ToList();
        }

        public Evolution GetPreEvolution(int pokemonId)
        {
            Evolution preEvolution = this.GetEvolutions().Find(x => x.EvolutionPokemon.Id == pokemonId && x.PreevolutionPokemon.IsComplete);

            if (preEvolution != null && this.CheckIfAltForm(preEvolution.PreevolutionPokemonId))
            {
                preEvolution.PreevolutionPokemon.Name = string.Concat(preEvolution.PreevolutionPokemon.Name, " (", this.GetPokemonFormName(preEvolution.PreevolutionPokemonId), ")");
            }

            return preEvolution;
        }

        /// <summary>
        /// Gets the pre evolutions, including any incomplete pokemon.
        /// </summary>
        /// <param name="pokemonId">The pokemon's id.</param>
        /// <returns>Returns the pre evolution.</returns>
        public Evolution GetPreEvolutionIncludeIncomplete(int pokemonId)
        {
            Evolution preEvolution = this.GetEvolutions().Find(x => x.EvolutionPokemon.Id == pokemonId);

            if (preEvolution != null && this.CheckIfAltForm(preEvolution.PreevolutionPokemonId))
            {
                preEvolution.PreevolutionPokemon.Name = string.Concat(preEvolution.PreevolutionPokemon.Name, " (", this.GetPokemonFormName(preEvolution.PreevolutionPokemonId), ")");
            }

            return preEvolution;
        }

        public Evolution GetPreEvolutionNoEdit(int pokemonId)
        {
            return this.dataContext.Evolutions
                .ToList()
                .Find(x => x.EvolutionPokemonId == pokemonId);
        }

        public List<Evolution> GetPokemonEvolutions(int pokemonId)
        {
            List<Evolution> evolutions = this.GetEvolutions()
                .Where(x => x.PreevolutionPokemon.Id == pokemonId && x.PreevolutionPokemon.IsComplete && x.EvolutionPokemon.IsComplete)
                .OrderBy(x => x.EvolutionPokemon.PokedexNumber)
                .ThenBy(x => x.EvolutionPokemon.Id)
                .ToList();

            foreach (var e in evolutions)
            {
                if (this.CheckIfAltForm(e.EvolutionPokemonId))
                {
                    e.EvolutionPokemon.Name = string.Concat(e.EvolutionPokemon.Name, " (", this.GetPokemonFormName(e.EvolutionPokemonId), ")");
                }
            }

            return evolutions;
        }

        public List<Evolution> GetPokemonEvolutionsIncludeIncomplete(int pokemonId)
        {
            List<Evolution> evolutions = this.GetEvolutions()
                .Where(x => x.PreevolutionPokemon.Id == pokemonId)
                .OrderBy(x => x.EvolutionPokemon.Id)
                .ToList();

            foreach (var e in evolutions)
            {
                if (this.CheckIfAltForm(e.EvolutionPokemonId))
                {
                    e.EvolutionPokemon.Name = string.Concat(e.EvolutionPokemon.Name, " (", this.GetPokemonFormName(e.EvolutionPokemonId), ")");
                }
            }

            return evolutions;
        }

        public List<Evolution> GetPokemonEvolutionsNoEdit(int pokemonId)
        {
            return this.dataContext.Evolutions
                .Where(x => x.PreevolutionPokemonId == pokemonId)
                .ToList();
        }

        public List<Form> GetForms()
        {
            return this.dataContext.Forms
                .OrderBy(x => x.Name)
                .ToList();
        }

        public Form GetForm(int id)
        {
            return this.dataContext.Forms
                .ToList()
                .Find(x => x.Id == id);
        }

        public Form GetFormByName(string formName)
        {
            return this.dataContext.Forms.ToList().Find(x => x.Name == formName);
        }

        public List<PokemonFormDetail> GetPokemonForms(int pokemonId)
        {
            return this.dataContext.PokemonFormDetails
                .Include(x => x.Form)
                .Include(x => x.OriginalPokemon)
                .Include(x => x.AltFormPokemon)
                .Where(x => x.OriginalPokemon.Id == pokemonId && x.OriginalPokemon.IsComplete && x.AltFormPokemon.IsComplete)
                .OrderBy(x => x.AltFormPokemon.Id)
                .ToList();
        }

        public List<PokemonFormDetail> GetPokemonFormsWithIncomplete(int pokemonId)
        {
            return this.dataContext.PokemonFormDetails
                .Include(x => x.Form)
                .Include(x => x.OriginalPokemon)
                .Include(x => x.AltFormPokemon)
                .Where(x => x.OriginalPokemon.Id == pokemonId)
                .OrderBy(x => x.AltFormPokemon.Id)
                .ToList();
        }

        public string GetPokemonFormName(int pokemonId)
        {
            PokemonFormDetail formDetail = this.dataContext.PokemonFormDetails
                .Include(x => x.Form)
                .Include(x => x.OriginalPokemon)
                .Include(x => x.AltFormPokemon)
                .ToList()
                .Find(x => x.AltFormPokemon.Id == pokemonId);
            return formDetail.Form.Name;
        }

        public PokemonGameDetail GetPokemonGameDetail(int id)
        {
            return this.dataContext.PokemonGameDetails
                .ToList()
                .Find(x => x.Id == id);
        }

        public List<PokemonGameDetail> GetPokemonGameDetails(int pokemonId)
        {
            return this.dataContext.PokemonGameDetails
                .Include(x => x.Pokemon)
                    .Include("Pokemon.Game")
                .Include(x => x.Game)
                .Where(x => x.PokemonId == pokemonId)
                .ToList();
        }

        public List<PokemonGameDetail> GetAllPokemonGameDetails()
        {
            return this.dataContext.PokemonGameDetails
                .Include(x => x.Pokemon)
                .Include(x => x.Game)
                .ToList();
        }

        public List<PokemonGameDetail> GetPokemonGameDetailsByGeneration(int generationId)
        {
            return this.dataContext.PokemonGameDetails
                .Include(x => x.Pokemon)
                .Include(x => x.Game)
                    .Include("Game.Generation")
                .Where(x => x.Game.GenerationId == generationId)
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
            return this.GetAllPokemon()
                .Find(x => x.Name == name);
        }

        public Pokemon GetPokemonByPokedexNumber(int pokedexNumber)
        {
            return this.dataContext.Pokemon
                .ToList()
                .Find(x => x.PokedexNumber == pokedexNumber);
        }

        public Pokemon GetPokemonFromNameAndFormName(string pokemonName, string formName)
        {
            List<PokemonFormDetail> pokemon = this.GetPokemonFormDetails().Where(x => x.Form.Name == formName).ToList();
            return pokemon.Find(x => x.AltFormPokemon.Name == pokemonName).AltFormPokemon;
        }

        public Pokemon GetPokemonById(int id)
        {
            return this.dataContext.Pokemon
                .Include(x => x.EggCycle)
                .Include(x => x.GenderRatio)
                .Include(x => x.Classification)
                .Include(x => x.Game)
                    .Include("Game.Generation")
                .Include(x => x.ExperienceGrowth)
                .Include(x => x.BaseHappiness)
                .ToList()
                .Find(x => x.Id == id);
        }

        public Pokemon GetPokemonByIdNoIncludes(int id)
        {
            return this.dataContext.Pokemon
                .ToList()
                .Find(x => x.Id == id);
        }

        public Pokemon GetPokemonNoIncludesById(int id)
        {
            return this.dataContext.Pokemon
                .ToList()
                .Find(x => x.Id == id);
        }

        public PokemonViewModel GetPokemonDetails(Pokemon pokemon, Form form, AppConfig appConfig)
        {
            PokemonViewModel pokemonViewModel = new PokemonViewModel()
            {
                Pokemon = pokemon,
                BaseStats = this.GetBaseStat(pokemon.Id),
                EVYields = this.GetEVYields(pokemon.Id),
                Typings = this.GetPokemonWithTypes(pokemon.Id),
                Abilities = this.GetPokemonWithAbilities(pokemon.Id),
                EggGroups = this.GetPokemonWithEggGroups(pokemon.Id),
                CaptureRates = this.GetPokemonWithCaptureRates(pokemon.Id),
                PreEvolution = this.GetPreEvolution(pokemon.Id),
                Evolutions = this.GetPokemonEvolutions(pokemon.Id),
                Effectiveness = this.GetTypeChartPokemon(pokemon.Id),
                GamesAvailableIn = this.GetPokemonGameDetails(pokemon.Id).ConvertAll(x => x.Game),
                AppConfig = appConfig,
            };

            if (form != null)
            {
                pokemonViewModel.Form = form;
                pokemonViewModel.Pokemon.Name = string.Concat(pokemonViewModel.Pokemon.Name, " (", form.Name, ")");
            }

            PokemonLegendaryDetail legendaryType = this.GetLegendaryDetail(pokemon.Id);

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
                AllAltForms = this.GetAllAltForms(),
                AllEvolutions = this.GetEvolutions(),
                AllTypings = this.GetAllPokemonWithTypesAndIncomplete(),
                AllAbilities = this.GetAllPokemonWithAbilitiesAndIncomplete(),
                AllEggGroups = this.GetAllPokemonWithEggGroupsAndIncomplete(),
                AllBaseStats = this.GetBaseStatsWithIncomplete(),
                AllEVYields = this.GetEVYieldsWithIncomplete(),
                AllLegendaryDetails = this.GetAllPokemonWithLegendaryTypes(),
                AllPokemonCaptureRates = this.GetAllPokemonWithCaptureRates(),
            };
        }

        public List<Pokeball> GetPokeballs()
        {
            return this.dataContext.Pokeballs
                .Include(x => x.Generation)
                .OrderBy(x => x.GenerationId)
                .ThenBy(x => x.Name)
                .ToList();
        }

        public Pokeball GetPokeball(int id)
        {
            return this.dataContext.Pokeballs
                .Include(x => x.Generation)
                .ToList()
                .Find(x => x.Id == id);
        }

        public List<PokeballCatchModifierDetail> GetPokeballCatchModifierDetails()
        {
            return this.dataContext.PokeballCatchModifierDetails
                .Include(x => x.Pokeball)
                .ToList();
        }

        public PokeballCatchModifierDetail GetPokeballCatchModifierDetail(int id)
        {
            return this.dataContext.PokeballCatchModifierDetails
                .Include(x => x.Pokeball)
                .ToList()
                .Find(x => x.Id == id);
        }

        public List<Pokeball> GetPokeballsForCaptureCalculator()
        {
            return this.dataContext.Pokeballs
                .OrderBy(x => x.Name)
                .ToList();
        }

        public List<PokeballCatchModifierDetail> GetCatchModifiersForPokeball(int id)
        {
            return this.dataContext.PokeballCatchModifierDetails
                .Where(x => x.PokeballId == id)
                .ToList();
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

        public List<Pokemon> GetAllPokemonForCaptureCalculator()
        {
            List<Pokemon> pokemonList = this.dataContext.Pokemon
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

            List<PokemonFormDetail> altFormList = this.GetAllAltFormsForCaptureCalculator();
            Pokemon pokemon;

            foreach (var a in altFormList)
            {
                pokemon = pokemonList.Find(x => x.Id == a.AltFormPokemonId);
                if (a.Form.Catchable)
                {
                    pokemon.Name = a.AltFormPokemon.Name;
                }
                else
                {
                    pokemonList.Remove(pokemon);
                }
            }

            return pokemonList;
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

            List<Pokemon> altFormList = this.GetAllAltFormsWithIncompleteWithFormName();
            Pokemon pokemon;

            foreach (var a in altFormList)
            {
                pokemon = pokemonList.Find(x => x.Id == a.Id);
                pokemon.Name = a.Name;
            }

            return pokemonList;
        }

        public List<Pokemon> GetAllPokemonNoIncludes()
        {
            return this.dataContext.Pokemon
                .Where(x => x.IsComplete)
                .OrderBy(x => x.PokedexNumber)
                .ThenBy(x => x.Id)
                .ToList();
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
                formDetails.AddRange(this.GetPokemonFormDetailsByFormName(n));
            }

            List<PokemonEggGroupDetail> eggGroupDetails = this.dataContext.PokemonEggGroupDetails
                .Include(x => x.Pokemon)
                .Include(x => x.PrimaryEggGroup)
                .Include(x => x.SecondaryEggGroup)
                .ToList();

            List<Pokemon> unbreedablePokemon = this.GetAllPokemon().Where(x => !eggGroupDetails.Any(y => y.PokemonId == x.Id && y.PrimaryEggGroup.Name != "Undiscovered")).ToList();

            eggGroupDetails = eggGroupDetails.Where(x => !formDetails.Any(y => y.AltFormPokemonId == x.PokemonId)).ToList();
            eggGroupDetails = eggGroupDetails.Where(x => !unbreedablePokemon.Any(y => y.Id == x.PokemonId)).ToList();
            eggGroupDetails = eggGroupDetails.Where(x => x.Pokemon.IsComplete).ToList();

            eggGroupDetails = eggGroupDetails.OrderBy(x => x.Pokemon.Name).ToList();

            return eggGroupDetails;
        }

        public List<Pokemon> GetAllPokemonIncludeIncomplete()
        {
            return this.dataContext.Pokemon
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
        }

        public List<Pokemon> GetAllPokemonWithoutForms()
        {
            List<Pokemon> pokemonList = this.GetAllPokemon();
            List<Pokemon> altFormList = this.GetAllAltForms().ConvertAll(x => x.AltFormPokemon);
            return pokemonList.Where(x => !altFormList.Any(y => y.Id == x.Id)).ToList();
        }

        public List<Pokemon> GetAllPokemonWithoutFormsWithIncomplete()
        {
            List<Pokemon> pokemonList = this.GetAllPokemonIncludeIncomplete();
            List<Pokemon> altFormList = this.GetAllAltForms().ConvertAll(x => x.AltFormPokemon);
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
            List<Pokemon> pokemonList = this.GetAllPokemonIncludeIncomplete();
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

        public PokemonTeam GetPokemonTeam(int id)
        {
            return this.GetPokemonTeams().Find(x => x.Id == id);
        }

        public List<PokemonTeam> GetPokemonTeamsByUserId(int id)
        {
            return this.GetPokemonTeams()
                .Where(x => x.User.Id == id)
                .ToList();
        }

        public PokemonTeam GetPokemonTeamNoIncludes(int id)
        {
            return this.dataContext.PokemonTeams
                .ToList()
                .Find(x => x.Id == id);
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

        public PokemonTeamDetail GetPokemonTeamDetailNoIncludes(int id)
        {
            return this.dataContext.PokemonTeamDetails
                .ToList()
                .Find(x => x.Id == id);
        }

        public PokemonTeamEV GetPokemonTeamEV(int id)
        {
            return this.dataContext.PokemonTeamEVs
                .ToList()
                .Find(x => x.Id == id);
        }

        public PokemonTeamIV GetPokemonTeamIV(int id)
        {
            return this.dataContext.PokemonTeamIVs
                .ToList()
                .Find(x => x.Id == id);
        }

        public PokemonTeamMoveset GetPokemonTeamMoveset(int id)
        {
            return this.dataContext.PokemonTeamMovesets
                .ToList()
                .Find(x => x.Id == id);
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
            List<PokemonFormDetail> pokemonForm = this.dataContext.PokemonFormDetails
                .Include(x => x.AltFormPokemon)
                .Include(x => x.Form)
                .Where(x => x.AltFormPokemon.IsComplete)
                .ToList();

            List<Pokemon> pokemonList = pokemonForm.ConvertAll(x => x.AltFormPokemon);

            foreach (var p in pokemonForm)
            {
                p.AltFormPokemon.Name = string.Concat(p.AltFormPokemon.Name, " (", p.Form.Name, ")");
                pokemonList.Add(p.AltFormPokemon);
            }

            return pokemonList;
        }

        public List<PokemonFormDetail> GetAllAltFormsForCaptureCalculator()
        {
            List<PokemonFormDetail> pokemonForm = this.dataContext.PokemonFormDetails
                .Include(x => x.AltFormPokemon)
                .Include(x => x.Form)
                .Where(x => x.AltFormPokemon.IsComplete)
                .ToList();

            List<PokemonFormDetail> pokemonList = new List<PokemonFormDetail>();

            foreach (var p in pokemonForm)
            {
                p.AltFormPokemon.Name = string.Concat(p.AltFormPokemon.Name, " (", p.Form.Name, ")");
                pokemonList.Add(p);
            }

            return pokemonList;
        }

        public List<Pokemon> GetAllAltFormsWithIncompleteWithFormName()
        {
            List<PokemonFormDetail> pokemonForm = this.dataContext.PokemonFormDetails
                .Include(x => x.AltFormPokemon)
                .Include(x => x.Form)
                .ToList();

            List<Pokemon> pokemonList = pokemonForm.ConvertAll(x => x.AltFormPokemon);

            foreach (var p in pokemonForm)
            {
                p.AltFormPokemon.Name = string.Concat(p.AltFormPokemon.Name, " (", p.Form.Name, ")");
                pokemonList.Add(p.AltFormPokemon);
            }

            return pokemonList;
        }

        public List<PokemonFormDetail> GetAllAltForms()
        {
            return this.dataContext.PokemonFormDetails
                .Include(x => x.AltFormPokemon)
                    .Include("AltFormPokemon.Game")
                .Include(x => x.OriginalPokemon)
                    .Include("OriginalPokemon.Game")
                .Include(x => x.Form)
                .ToList();
        }

        public List<Pokemon> GetAllPokemonOnlyForms()
        {
            List<Pokemon> pokemonList = this.GetAllAltForms().Select(x => x.AltFormPokemon).OrderBy(x => x.PokedexNumber).ToList();

            foreach (var p in pokemonList)
            {
                p.Name = string.Concat(p.Name, " (", this.GetPokemonFormName(p.Id), ")");
            }

            return pokemonList;
        }

        public List<PokemonFormDetail> GetAllAltFormsOnlyComplete()
        {
            return this.GetAllAltForms().Where(x => x.AltFormPokemon.IsComplete).ToList();
        }

        public PokemonFormDetail GetPokemonFormDetailByAltFormId(int pokemonId)
        {
            return this.dataContext.PokemonFormDetails
                .Include(x => x.AltFormPokemon)
                .Include(x => x.OriginalPokemon)
                .Include(x => x.Form)
                .ToList()
                .Find(x => x.AltFormPokemonId == pokemonId);
        }

        public Pokemon GetOriginalPokemonByAltFormId(int pokemonId)
        {
            return this.dataContext.PokemonFormDetails
                .Include(x => x.AltFormPokemon)
                .Include(x => x.OriginalPokemon)
                .Include(x => x.Form)
                .ToList()
                .Find(x => x.AltFormPokemonId == pokemonId).OriginalPokemon;
        }

        public Form GetFormByAltFormId(int pokemonId)
        {
            PokemonFormDetail details = this.dataContext.PokemonFormDetails
                .Include(x => x.AltFormPokemon)
                .Include(x => x.Form)
                .ToList()
                .Find(x => x.AltFormPokemonId == pokemonId);

            return details.Form;
        }

        public List<PokemonFormDetail> GetPokemonFormDetailsForPokemon(int pokemonId)
        {
            return this.dataContext.PokemonFormDetails
                .Where(x => x.OriginalPokemonId == pokemonId)
                .ToList();
        }

        public List<PokemonFormDetail> GetPokemonFormDetails()
        {
            return this.dataContext.PokemonFormDetails
                .Include(x => x.OriginalPokemon)
                .Include(x => x.AltFormPokemon)
                .Include(x => x.Form)
                .ToList();
        }

        public List<PokemonFormDetail> GetPokemonFormDetailsByFormName(string formName)
        {
            return this.dataContext.PokemonFormDetails
                .Include(x => x.OriginalPokemon)
                .Include(x => x.AltFormPokemon)
                .Include(x => x.Form)
                .Where(x => x.Form.Name == formName)
                .ToList();
        }

        public List<PokemonTypeDetail> GetPokemonWithTypes(int pokemonId)
        {
            return this.dataContext.PokemonTypeDetails
                .Include(x => x.Pokemon)
                .Include(x => x.PrimaryType)
                .Include(x => x.SecondaryType)
                .Include(x => x.Generation)
                .Where(x => x.Pokemon.Id == pokemonId)
                .ToList();
        }

        public PokemonTypeDetail GetPokemonWithTypesNoIncludes(int pokemonId)
        {
            return this.dataContext.PokemonTypeDetails
                .ToList()
                .Find(x => x.PokemonId == pokemonId);
        }

        public List<PokemonTypeDetail> GetAllPokemonWithTypes()
        {
            List<PokemonTypeDetail> pokemonList = this.dataContext.PokemonTypeDetails
                                                        .Include(x => x.Pokemon)
                                                            .Include("Pokemon.Game")
                                                        .Include(x => x.PrimaryType)
                                                        .Include(x => x.SecondaryType)
                                                        .Where(x => x.Pokemon.IsComplete)
                                                        .OrderBy(x => x.Pokemon.PokedexNumber)
                                                        .ThenBy(x => x.PokemonId)
                                                        .ToList();
            List<Pokemon> altFormList = this.dataContext.PokemonFormDetails.Select(x => x.AltFormPokemon).ToList();
            pokemonList = pokemonList.Where(x => !altFormList.Any(y => y.Id == x.PokemonId)).ToList();
            List<int> pokemonIds = pokemonList.Select(x => x.PokemonId).Distinct().ToList();

            return pokemonList.OrderBy(x => x.GenerationId).GroupBy(x => new { x.PokemonId }).Select(x => x.LastOrDefault()).ToList();
        }

        public List<PokemonTypeDetail> GetAllPokemonWithTypesWithAltForms()
        {
            List<PokemonTypeDetail> pokemonList = this.dataContext.PokemonTypeDetails
                                                        .Include(x => x.Pokemon)
                                                            .Include("Pokemon.Game")
                                                        .Include(x => x.PrimaryType)
                                                        .Include(x => x.SecondaryType)
                                                        .Where(x => x.Pokemon.IsComplete)
                                                        .OrderBy(x => x.Pokemon.PokedexNumber)
                                                        .ThenBy(x => x.PokemonId)
                                                        .ToList();

            return pokemonList;
        }

        public List<PokemonTypeDetail> GetAllPokemonWithSpecificTypes(int primaryTypeId, int secondaryTypeId, int generationId)
        {
            List<PokemonTypeDetail> pokemonList = this.dataContext.PokemonTypeDetails
                                                        .Include(x => x.Pokemon)
                                                            .Include("Pokemon.Game")
                                                        .Include(x => x.PrimaryType)
                                                        .Include(x => x.SecondaryType)
                                                        .Where(x => x.Pokemon.Game.GenerationId <= generationId)
                                                        .Where(x => x.GenerationId <= generationId)
                                                        .Where(x => x.Pokemon.IsComplete)
                                                        .ToList();

            pokemonList = pokemonList.OrderBy(x => x.GenerationId).GroupBy(x => new { x.PokemonId }).Select(x => x.LastOrDefault()).ToList();

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

            return pokemonList.OrderBy(x => x.Pokemon.PokedexNumber).ToList();
        }

        public List<PokemonTypeDetail> GetAllPokemonWithTypesAndIncomplete()
        {
            List<PokemonTypeDetail> pokemonList = this.dataContext.PokemonTypeDetails
                                                        .Include(x => x.Pokemon)
                                                        .Include(x => x.PrimaryType)
                                                        .Include(x => x.SecondaryType)
                                                        .ToList();

            return pokemonList.OrderBy(x => x.Pokemon.Id).ThenBy(x => x.Pokemon.Id).ToList();
        }

        public List<PokemonAbilityDetail> GetPokemonWithAbilities(int pokemonId)
        {
            return this.dataContext.PokemonAbilityDetails.Include(x => x.Pokemon)
                .Include(x => x.PrimaryAbility)
                .Include(x => x.SecondaryAbility)
                .Include(x => x.HiddenAbility)
                .Include(x => x.SpecialEventAbility)
                .Where(x => x.Pokemon.Id == pokemonId)
                .ToList();
        }

        public List<Ability> GetAbilitiesForPokemon(int pokemonId)
        {
            List<Ability> abilityList = new List<Ability>();
            PokemonAbilityDetail pokemonAbilityDetail = this.dataContext.PokemonAbilityDetails.Include(x => x.Pokemon)
                .Include(x => x.PrimaryAbility)
                .Include(x => x.SecondaryAbility)
                .Include(x => x.HiddenAbility)
                .Include(x => x.SpecialEventAbility)
                .ToList()
                .Find(x => x.Pokemon.Id == pokemonId);

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

        public PokemonAbilityDetail GetPokemonWithAbilitiesNoIncludes(int pokemonId, int generationId)
        {
            return this.dataContext.PokemonAbilityDetails.Include(x => x.Pokemon)
                .ToList()
                .Find(x => x.PokemonId == pokemonId && x.GenerationId == generationId);
        }

        public List<PokemonLegendaryDetail> GetAllPokemonWithLegendaryTypes()
        {
            return this.dataContext.PokemonLegendaryDetails
                .Include(x => x.Pokemon)
                .Include(x => x.LegendaryType)
                .ToList();
        }

        public List<PokemonLegendaryDetail> GetAllPokemonWithLegendaryTypesAndIncomplete()
        {
            return this.dataContext.PokemonLegendaryDetails
                .Include(x => x.Pokemon)
                .Include(x => x.LegendaryType)
                .ToList();
        }

        public List<PokemonAbilityDetail> GetAllPokemonWithAbilitiesAndIncomplete()
        {
            return this.dataContext.PokemonAbilityDetails
                .Include(x => x.Pokemon)
                .Include(x => x.PrimaryAbility)
                .Include(x => x.SecondaryAbility)
                .Include(x => x.HiddenAbility)
                .Include(x => x.SpecialEventAbility)
                .ToList();
        }

        public List<PokemonEggGroupDetail> GetPokemonWithEggGroups(int pokemonId)
        {
            return this.dataContext.PokemonEggGroupDetails
                .Include(x => x.Pokemon)
                .Include(x => x.PrimaryEggGroup)
                .Include(x => x.SecondaryEggGroup)
                .Where(x => x.Pokemon.Id == pokemonId)
                .ToList();
        }

        public PokemonEggGroupDetail GetPokemonWithEggGroupsFromPokemonName(string pokemonName)
        {
            return this.dataContext.PokemonEggGroupDetails
                .Include(x => x.Pokemon)
                    .Include("Pokemon.GenderRatio")
                .Include(x => x.PrimaryEggGroup)
                .Include(x => x.SecondaryEggGroup)
                .ToList()
                .First(x => x.Pokemon.Name == pokemonName);
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
            List<int> pokemonIds = this.GetPokedexNumbers();
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

            surroundingPokemon.Add(this.GetPokemonByIdNoIncludes(previousId));
            surroundingPokemon.Add(this.GetPokemonByIdNoIncludes(nextId));

            return surroundingPokemon;
        }

        public List<int> GetPokedexNumbers()
        {
            List<int> ids = this.GetAllPokemon().ConvertAll(x => x.Id);
            List<int> altIds = this.GetAllAltForms().ConvertAll(x => x.AltFormPokemonId);
            return ids.Where(x => !altIds.Any(y => y == x)).ToList();
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

        public List<Pokemon> GetAllPokemonWithClassificationsAndIncomplete()
        {
            return this.dataContext.Pokemon
                .Include(x => x.Classification)
                .ToList();
        }

        public List<BaseStat> GetBaseStat(int pokemonId)
        {
            return this.dataContext.BaseStats
                .Include(x => x.Pokemon)
                .Where(x => x.Pokemon.Id == pokemonId)
                .ToList();
        }

        public List<BaseStat> GetBaseStatsWithIncomplete()
        {
            return this.dataContext.BaseStats
                .Include(x => x.Pokemon)
                .ToList();
        }

        public List<EVYield> GetEVYields(int pokemonId)
        {
            return this.dataContext.EVYields
                .Include(x => x.Pokemon)
                .Where(x => x.Pokemon.Id == pokemonId)
                .ToList();
        }

        public List<EVYield> GetEVYieldsWithIncomplete()
        {
            return this.dataContext.EVYields
                .Include(x => x.Pokemon)
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
            List<Type> typeList = this.GetTypes();
            List<PokemonTypeDetail> pokemonTypes = this.GetPokemonWithTypes(pokemonId);
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

        public TypeEffectivenessViewModel GetTypeChartTyping(int primaryTypeId, int secondaryTypeId, int generationId)
        {
            List<Type> typeList = this.GetTypes();
            List<Type> pokemonTypes = new List<Type>();
            List<string> strongAgainst = new List<string>();
            List<string> superStrongAgainst = new List<string>();
            List<string> weakAgainst = new List<string>();
            List<string> superWeakAgainst = new List<string>();
            List<string> immuneTo = new List<string>();
            List<TypeChart> typeChart;
            string effectiveValue, attackType;

            pokemonTypes.Add(this.GetType(primaryTypeId));

            if (secondaryTypeId != 0)
            {
                pokemonTypes.Add(this.GetType(secondaryTypeId));
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

        public BaseHappiness GetBaseHappiness(int id)
        {
            return this.dataContext.BaseHappiness.ToList().Find(x => x.Id == id);
        }

        public List<BaseHappiness> GetBaseHappinesses()
        {
            return this.dataContext.BaseHappiness.OrderBy(x => x.Happiness).ToList();
        }

        public Classification GetClassification(int id)
        {
            return this.dataContext.Classifications.ToList().Find(x => x.Id == id);
        }

        public List<Classification> GetClassifications()
        {
            return this.dataContext.Classifications.OrderBy(x => x.Name).ToList();
        }

        public Nature GetNature(int id)
        {
            return this.dataContext.Natures.ToList().Find(x => x.Id == id);
        }

        public Nature GetNatureByName(string name)
        {
            return this.dataContext.Natures.ToList().Find(x => x.Name == name);
        }

        public List<Nature> GetNatures()
        {
            return this.dataContext.Natures.OrderBy(x => x.Name).ToList();
        }

        public List<BattleItem> GetBattleItems()
        {
            return this.dataContext.BattleItems
                .Include(x => x.Generation)
                .Include(x => x.Pokemon)
                .OrderBy(x => x.Generation.Id)
                .ThenBy(x => x.Name)
                .ToList();
        }

        public CaptureRate GetCaptureRate(int id)
        {
            return this.dataContext.CaptureRates.ToList().Find(x => x.Id == id);
        }

        public List<CaptureRate> GetCaptureRates()
        {
            return this.dataContext.CaptureRates.OrderBy(x => x.CatchRate).ToList();
        }

        public EggCycle GetEggCycle(int id)
        {
            return this.dataContext.EggCycles.ToList().Find(x => x.Id == id);
        }

        public List<EggCycle> GetEggCycles()
        {
            return this.dataContext.EggCycles.OrderBy(x => x.CycleCount).ToList();
        }

        public ExperienceGrowth GetExperienceGrowth(int id)
        {
            return this.dataContext.ExperienceGrowths.ToList().Find(x => x.Id == id);
        }

        public List<ExperienceGrowth> GetExperienceGrowths()
        {
            return this.dataContext.ExperienceGrowths.OrderBy(x => x.Name).ToList();
        }

        public GenderRatio GetGenderRatio(int id)
        {
            return this.dataContext.GenderRatios.ToList().Find(x => x.Id == id);
        }

        public List<GenderRatio> GetGenderRatios()
        {
            return this.dataContext.GenderRatios.ToList();
        }

        public List<ReviewedPokemon> GetAllReviewedPokemon()
        {
            return this.dataContext.ReviewedPokemons
                .Include(x => x.Pokemon)
                .OrderBy(x => x.Pokemon.PokedexNumber)
                .ToList();
        }

        public ReviewedPokemon GetReviewedPokemon(int id)
        {
            return this.dataContext.ReviewedPokemons.ToList().Find(x => x.Id == id);
        }

        public ReviewedPokemon GetReviewedPokemonByPokemonId(int id)
        {
            return this.dataContext.ReviewedPokemons.ToList().Find(x => x.PokemonId == id);
        }

        public List<EvolutionMethod> GetEvolutionMethods()
        {
            return this.dataContext.EvolutionMethods.OrderBy(x => x.Name).ToList();
        }

        public EvolutionMethod GetEvolutionMethod(int id)
        {
            return this.dataContext.EvolutionMethods.ToList().Find(x => x.Id == id);
        }

        public List<Game> GetAvailableGamesFromPokemonId(int id)
        {
            return this.dataContext.PokemonGameDetails.Where(x => x.PokemonId == id).Select(x => x.Game).ToList();
        }

        public List<Generation> GetGenerations()
        {
            return this.dataContext.Generations.ToList();
        }

        public List<Generation> GetGenerationsForRazor()
        {
            List<Generation> generations = this.dataContext.Generations.ToList();
            foreach (var g in generations)
            {
                g.Region = string.Concat("Generation ", g.Id);
            }

            return generations;
        }

        public List<Game> GetAvailableGames(int pokemonTeamId)
        {
            PokemonTeam pokemonTeam = this.GetPokemonTeam(pokemonTeamId);
            List<Game> availableGames = new List<Game>();
            if (pokemonTeam.FirstPokemonId != null)
            {
                availableGames = this.GetPokemonGameDetails(pokemonTeam.FirstPokemon.PokemonId).ConvertAll(x => x.Game);
            }

            if (pokemonTeam.SecondPokemonId != null)
            {
                availableGames = availableGames.Where(x => this.GetPokemonGameDetails(pokemonTeam.SecondPokemon.PokemonId).Select(y => y.Game).Any(z => z.Id == x.Id)).ToList();
            }

            if (pokemonTeam.ThirdPokemonId != null)
            {
                availableGames = availableGames.Where(x => this.GetPokemonGameDetails(pokemonTeam.ThirdPokemon.PokemonId).Select(y => y.Game).Any(z => z.Id == x.Id)).ToList();
            }

            if (pokemonTeam.FourthPokemonId != null)
            {
                availableGames = availableGames.Where(x => this.GetPokemonGameDetails(pokemonTeam.FourthPokemon.PokemonId).Select(y => y.Game).Any(z => z.Id == x.Id)).ToList();
            }

            if (pokemonTeam.FifthPokemonId != null)
            {
                availableGames = availableGames.Where(x => this.GetPokemonGameDetails(pokemonTeam.FifthPokemon.PokemonId).Select(y => y.Game).Any(z => z.Id == x.Id)).ToList();
            }

            if (pokemonTeam.SixthPokemonId != null)
            {
                availableGames = availableGames.Where(x => this.GetPokemonGameDetails(pokemonTeam.SixthPokemon.PokemonId).Select(y => y.Game).Any(z => z.Id == x.Id)).ToList();
            }

            return availableGames;
        }

        public List<Game> GetAvailableGames(PokemonTeam pokemonTeam)
        {
            List<Game> availableGames = new List<Game>();
            if (pokemonTeam.FirstPokemonId != null)
            {
                availableGames = this.GetPokemonGameDetails(pokemonTeam.FirstPokemon.PokemonId).ConvertAll(x => x.Game);
            }

            if (pokemonTeam.SecondPokemonId != null)
            {
                availableGames = availableGames.Where(x => this.GetPokemonGameDetails(pokemonTeam.SecondPokemon.PokemonId).Select(y => y.Game).Any(z => z.Id == x.Id)).ToList();
            }

            if (pokemonTeam.ThirdPokemonId != null)
            {
                availableGames = availableGames.Where(x => this.GetPokemonGameDetails(pokemonTeam.ThirdPokemon.PokemonId).Select(y => y.Game).Any(z => z.Id == x.Id)).ToList();
            }

            if (pokemonTeam.FourthPokemonId != null)
            {
                availableGames = availableGames.Where(x => this.GetPokemonGameDetails(pokemonTeam.FourthPokemon.PokemonId).Select(y => y.Game).Any(z => z.Id == x.Id)).ToList();
            }

            if (pokemonTeam.FifthPokemonId != null)
            {
                availableGames = availableGames.Where(x => this.GetPokemonGameDetails(pokemonTeam.FifthPokemon.PokemonId).Select(y => y.Game).Any(z => z.Id == x.Id)).ToList();
            }

            if (pokemonTeam.SixthPokemonId != null)
            {
                availableGames = availableGames.Where(x => this.GetPokemonGameDetails(pokemonTeam.SixthPokemon.PokemonId).Select(y => y.Game).Any(z => z.Id == x.Id)).ToList();
            }

            return availableGames;
        }

        public void FillPokemonTeam(PokemonTeam pokemonTeam)
        {
            if (pokemonTeam.FirstPokemonId != null)
            {
                pokemonTeam.FirstPokemon = this.GetPokemonTeamDetail((int)pokemonTeam.FirstPokemonId);
            }

            if (pokemonTeam.SecondPokemonId != null)
            {
                pokemonTeam.SecondPokemon = this.GetPokemonTeamDetail((int)pokemonTeam.SecondPokemonId);
            }

            if (pokemonTeam.ThirdPokemonId != null)
            {
                pokemonTeam.ThirdPokemon = this.GetPokemonTeamDetail((int)pokemonTeam.ThirdPokemonId);
            }

            if (pokemonTeam.FourthPokemonId != null)
            {
                pokemonTeam.FourthPokemon = this.GetPokemonTeamDetail((int)pokemonTeam.FourthPokemonId);
            }

            if (pokemonTeam.FifthPokemonId != null)
            {
                pokemonTeam.FifthPokemon = this.GetPokemonTeamDetail((int)pokemonTeam.FifthPokemonId);
            }

            if (pokemonTeam.SixthPokemonId != null)
            {
                pokemonTeam.SixthPokemon = this.GetPokemonTeamDetail((int)pokemonTeam.SixthPokemonId);
            }
        }

        public Generation GetGeneration(int id)
        {
            return this.dataContext.Generations.ToList().Find(x => x.Id == id);
        }

        public Generation GetGenerationFromGame(int id)
        {
            return this.dataContext.Games
                .Include(x => x.Generation)
                .ToList()
                .Find(x => x.Id == id)
                .Generation;
        }

        public BattleItem GetBattleItem(int id)
        {
            return this.dataContext.BattleItems.ToList().Find(x => x.Id == id);
        }

        public BattleItem GetBattleItemByName(string name)
        {
            return this.dataContext.BattleItems.ToList().Find(x => x.Name == name);
        }

        public Generation GetGenerationByPokemon(int id)
        {
            return this.GetGeneration(this.GetPokemonById(id).Game.GenerationId);
        }

        public User GetUserWithUsername(string username)
        {
            return this.dataContext.Users.ToList().Find(x => x.Username == username);
        }

        public User GetUser(int id)
        {
            return this.dataContext.Users.ToList().Find(x => x.Id == id);
        }

        public List<Message> GetMessagesToUser(int id)
        {
            return this.dataContext.Messages.Where(x => x.ReceiverId == id).ToList();
        }

        public Message GetMessage(int id)
        {
            return this.dataContext.Messages.ToList().Find(x => x.Id == id);
        }

        public User GetUserById(int id)
        {
            return this.dataContext.Users.ToList().Find(x => x.Id == id);
        }

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

        public CommentPage GetCommentPage(int id)
        {
            return this.dataContext.CommentPages.ToList().Find(x => x.Id == id);
        }

        public CommentPage GetCommentPageByName(string name)
        {
            return this.dataContext.CommentPages.ToList().Find(x => x.Name == name);
        }

        public List<CommentCategory> GetCommentCategories()
        {
            return this.dataContext.CommentCategories.ToList();
        }

        public CommentCategory GetCommentCategory(int id)
        {
            return this.dataContext.CommentCategories.ToList().Find(x => x.Id == id);
        }

        public CommentCategory GetCommentCategoryByName(string name)
        {
            return this.dataContext.CommentCategories.ToList().Find(x => x.Name == name);
        }

        public List<User> GetUsers()
        {
            return this.dataContext.Users.ToList();
        }

        public List<Comment> GetComments()
        {
            return this.dataContext.Comments
                .Include(x => x.Commentor)
                .Include(x => x.Category)
                .Include(x => x.Page)
                .ToList();
        }

        public Comment GetComment(int id)
        {
            return this.dataContext.Comments.ToList().Find(x => x.Id == id);
        }

        public void AddComment(Comment comment)
        {
            this.dataContext.Comments.Add(comment);
            this.dataContext.SaveChanges();
        }

        public void AddMessage(Message message)
        {
            this.dataContext.Messages.Add(message);
            this.dataContext.SaveChanges();
        }

        public void AddUser(User user)
        {
            this.dataContext.Users.Add(user);
            this.dataContext.SaveChanges();
        }

        public void AddPokemonGameDetail(PokemonGameDetail pokemonGameDetail)
        {
            this.dataContext.PokemonGameDetails.Add(pokemonGameDetail);
            this.dataContext.SaveChanges();
        }

        public void AddPokemonGameDetails(List<PokemonGameDetail> details)
        {
            this.dataContext.PokemonGameDetails.AddRange(details);
            this.dataContext.SaveChanges();
        }

        public void AddEvolutionMethod(EvolutionMethod evolutionMethod)
        {
            this.dataContext.EvolutionMethods.Add(evolutionMethod);
            this.dataContext.SaveChanges();
        }

        public void AddCaptureRate(CaptureRate captureRate)
        {
            this.dataContext.CaptureRates.Add(captureRate);
            this.dataContext.SaveChanges();
        }

        public void AddBaseHappiness(BaseHappiness baseHappiness)
        {
            this.dataContext.BaseHappiness.Add(baseHappiness);
            this.dataContext.SaveChanges();
        }

        public void AddGeneration(Generation generation)
        {
            this.dataContext.Generations.Add(generation);
            this.dataContext.SaveChanges();
        }

        public void AddGame(Game game)
        {
            this.dataContext.Games.Add(game);
            this.dataContext.SaveChanges();
        }

        public void AddFormItem(FormItem formItem)
        {
            this.dataContext.FormItems.Add(formItem);
            this.dataContext.SaveChanges();
        }

        public void AddEvolution(Evolution evolution)
        {
            this.dataContext.Evolutions.Add(evolution);
            this.dataContext.SaveChanges();
        }

        public void AddType(Type type)
        {
            this.dataContext.Types.Add(type);
            this.dataContext.SaveChanges();
        }

        public void AddTypeChart(TypeChart typeChart)
        {
            this.dataContext.TypeCharts.Add(typeChart);
            this.dataContext.SaveChanges();
        }

        public void AddLegendaryType(LegendaryType legendaryType)
        {
            this.dataContext.LegendaryTypes.Add(legendaryType);
            this.dataContext.SaveChanges();
        }

        public void AddReviewedPokemon(ReviewedPokemon reviewedPokemon)
        {
            this.dataContext.ReviewedPokemons.Add(reviewedPokemon);
            this.dataContext.SaveChanges();
        }

        public void AddEggGroup(EggGroup eggGroup)
        {
            this.dataContext.EggGroups.Add(eggGroup);
            this.dataContext.SaveChanges();
        }

        public void AddAbility(Ability ability)
        {
            this.dataContext.Abilities.Add(ability);
            this.dataContext.SaveChanges();
        }

        public void AddBattleItem(BattleItem battleItem)
        {
            this.dataContext.BattleItems.Add(battleItem);
            this.dataContext.SaveChanges();
        }

        public void AddPokemon(Pokemon pokemon)
        {
            this.dataContext.Pokemon.Add(pokemon);
            this.dataContext.SaveChanges();
        }

        public void AddPokemonTeam(PokemonTeam pokemonTeam)
        {
            this.dataContext.PokemonTeams.Add(pokemonTeam);
            this.dataContext.SaveChanges();
        }

        public int AddPokemonTeamDetail(PokemonTeamDetail pokemonTeamDetail)
        {
            if (pokemonTeamDetail.PokemonTeamEVId == null)
            {
                int pokemonTeamEVId = this.AddPokemonTeamEV(new PokemonTeamEV());
                pokemonTeamDetail.PokemonTeamEVId = pokemonTeamEVId;
            }

            if (pokemonTeamDetail.PokemonTeamIVId == null)
            {
                int pokemonTeamIVId = this.AddPokemonTeamIV(new PokemonTeamIV());
                pokemonTeamDetail.PokemonTeamIVId = pokemonTeamIVId;
            }

            if (pokemonTeamDetail.PokemonTeamMovesetId == null)
            {
                int pokemonTeamMovesetId = this.AddPokemonTeamMoveset(new PokemonTeamMoveset());
                pokemonTeamDetail.PokemonTeamMovesetId = pokemonTeamMovesetId;
            }

            this.dataContext.PokemonTeamDetails.Add(pokemonTeamDetail);
            this.dataContext.SaveChanges();
            return pokemonTeamDetail.Id;
        }

        public int AddPokemonTeamEV(PokemonTeamEV pokemonTeamEV)
        {
            this.dataContext.PokemonTeamEVs.Add(pokemonTeamEV);
            this.dataContext.SaveChanges();
            return pokemonTeamEV.Id;
        }

        public int AddPokemonTeamIV(PokemonTeamIV pokemonTeamIV)
        {
            this.dataContext.PokemonTeamIVs.Add(pokemonTeamIV);
            this.dataContext.SaveChanges();
            return pokemonTeamIV.Id;
        }

        public int AddPokemonTeamMoveset(PokemonTeamMoveset pokemonTeamMoveset)
        {
            this.dataContext.PokemonTeamMovesets.Add(pokemonTeamMoveset);
            this.dataContext.SaveChanges();
            return pokemonTeamMoveset.Id;
        }

        public void AddPokemonFormDetails(PokemonFormDetail pokemonFormDetail)
        {
            this.dataContext.PokemonFormDetails.Add(pokemonFormDetail);
            this.dataContext.SaveChanges();
        }

        public void AddPokemonTyping(PokemonTypeDetail typing)
        {
            this.dataContext.PokemonTypeDetails.Add(typing);
            this.dataContext.SaveChanges();
        }

        public void AddEggCycle(EggCycle eggCycle)
        {
            this.dataContext.EggCycles.Add(eggCycle);
            this.dataContext.SaveChanges();
        }

        public void AddExperienceGrowth(ExperienceGrowth experienceGrowth)
        {
            this.dataContext.ExperienceGrowths.Add(experienceGrowth);
            this.dataContext.SaveChanges();
        }

        public void AddGenderRatio(GenderRatio genderRatio)
        {
            this.dataContext.GenderRatios.Add(genderRatio);
            this.dataContext.SaveChanges();
        }

        public void AddPokemonLegendaryDetails(PokemonLegendaryDetail pokemonLegendaryDetail)
        {
            this.dataContext.PokemonLegendaryDetails.Add(pokemonLegendaryDetail);
            this.dataContext.SaveChanges();
        }

        public void AddPokemonAbilities(PokemonAbilityDetail abilities)
        {
            this.dataContext.PokemonAbilityDetails.Add(abilities);
            this.dataContext.SaveChanges();
        }

        public void AddPokemonEggGroups(PokemonEggGroupDetail eggGroups)
        {
            this.dataContext.PokemonEggGroupDetails.Add(eggGroups);
            this.dataContext.SaveChanges();
        }

        public void AddPokemonBaseStat(BaseStat baseStat)
        {
            this.dataContext.BaseStats.Add(baseStat);
            this.dataContext.SaveChanges();
        }

        public void AddPokemonEVYield(EVYield evYield)
        {
            this.dataContext.EVYields.Add(evYield);
            this.dataContext.SaveChanges();
        }

        public void AddClassification(Classification classification)
        {
            this.dataContext.Classifications.Add(classification);
            this.dataContext.SaveChanges();
        }

        public void AddNature(Nature nature)
        {
            this.dataContext.Natures.Add(nature);
            this.dataContext.SaveChanges();
        }

        public void AddForm(Form form)
        {
            this.dataContext.Forms.Add(form);
            this.dataContext.SaveChanges();
        }

        public void AddCommentCategory(CommentCategory commentCategory)
        {
            this.dataContext.CommentCategories.Add(commentCategory);
            this.dataContext.SaveChanges();
        }

        public void AddCommentPage(CommentPage commentPage)
        {
            this.dataContext.CommentPages.Add(commentPage);
            this.dataContext.SaveChanges();
        }

        public void AddPokeball(Pokeball pokeball)
        {
            this.dataContext.Pokeballs.Add(pokeball);
            this.dataContext.SaveChanges();
        }

        public void AddPokeballCatchModifierDetail(PokeballCatchModifierDetail pokeballCatchModifierDetail)
        {
            this.dataContext.PokeballCatchModifierDetails.Add(pokeballCatchModifierDetail);
            this.dataContext.SaveChanges();
        }

        public void AddStatus(Status status)
        {
            this.dataContext.Statuses.Add(status);
            this.dataContext.SaveChanges();
        }

        public void AddPokemonCaptureRateDetail(PokemonCaptureRateDetail pokemonCaptureRate)
        {
            this.dataContext.PokemonCaptureRateDetails.Add(pokemonCaptureRate);
            this.dataContext.SaveChanges();
        }

        public void UpdatePokemonCaptureRateDetail(PokemonCaptureRateDetail pokemonCaptureRate)
        {
            this.dataContext.PokemonCaptureRateDetails.Update(pokemonCaptureRate);
            this.dataContext.SaveChanges();
        }

        public void UpdateStatus(Status status)
        {
            this.dataContext.Statuses.Update(status);
            this.dataContext.SaveChanges();
        }

        public void UpdatePokeballCatchModifierDetail(PokeballCatchModifierDetail pokeballCatchModifierDetail)
        {
            this.dataContext.PokeballCatchModifierDetails.Update(pokeballCatchModifierDetail);
            this.dataContext.SaveChanges();
        }

        public void UpdatePokeball(Pokeball pokeball)
        {
            this.dataContext.Pokeballs.Update(pokeball);
            this.dataContext.SaveChanges();
        }

        public void UpdateCommentPage(CommentPage commentPage)
        {
            this.dataContext.CommentPages.Update(commentPage);
            this.dataContext.SaveChanges();
        }

        public void UpdateCommentCategory(CommentCategory commentCategories)
        {
            this.dataContext.CommentCategories.Update(commentCategories);
            this.dataContext.SaveChanges();
        }

        public void UpdateUser(User user)
        {
            this.dataContext.Users.Update(user);
            this.dataContext.SaveChanges();
        }

        public void UpdateComment(Comment comment)
        {
            this.dataContext.Comments.Update(comment);
            this.dataContext.SaveChanges();
        }

        public void UpdateMessage(Message message)
        {
            this.dataContext.Messages.Update(message);
            this.dataContext.SaveChanges();
        }

        public void UpdatePokemonTypeDetail(PokemonTypeDetail pokemonTypeDetail)
        {
            this.dataContext.PokemonTypeDetails.Update(pokemonTypeDetail);
            this.dataContext.SaveChanges();
        }

        public void UpdateEvolutionMethod(EvolutionMethod evolutionMethod)
        {
            this.dataContext.EvolutionMethods.Update(evolutionMethod);
            this.dataContext.SaveChanges();
        }

        public void UpdatePokemonAbilityDetail(PokemonAbilityDetail pokemonAbilityDetail)
        {
            this.dataContext.PokemonAbilityDetails.Update(pokemonAbilityDetail);
            this.dataContext.SaveChanges();
        }

        public void UpdatePokemonTeam(PokemonTeam pokemonTeam)
        {
            this.dataContext.PokemonTeams.Update(pokemonTeam);
            this.dataContext.SaveChanges();
        }

        public void UpdatePokemonTeamDetail(PokemonTeamDetail pokemonTeamDetail)
        {
            this.dataContext.PokemonTeamDetails.Update(pokemonTeamDetail);
            this.dataContext.SaveChanges();
        }

        public void UpdatePokemonEggGroupDetail(PokemonEggGroupDetail pokemonEggGroupDetail)
        {
            this.dataContext.PokemonEggGroupDetails.Update(pokemonEggGroupDetail);
            this.dataContext.SaveChanges();
        }

        public void UpdateBaseStat(BaseStat baseStats)
        {
            this.dataContext.BaseStats.Update(baseStats);
            this.dataContext.SaveChanges();
        }

        public void UpdateEggCycle(EggCycle eggCycle)
        {
            this.dataContext.EggCycles.Update(eggCycle);
            this.dataContext.SaveChanges();
        }

        public void UpdateExperienceGrowth(ExperienceGrowth experienceGrowth)
        {
            this.dataContext.ExperienceGrowths.Update(experienceGrowth);
            this.dataContext.SaveChanges();
        }

        public void UpdateGenderRatio(GenderRatio genderRatio)
        {
            this.dataContext.GenderRatios.Update(genderRatio);
            this.dataContext.SaveChanges();
        }

        public void UpdateBaseHappiness(BaseHappiness baseHappiness)
        {
            this.dataContext.BaseHappiness.Update(baseHappiness);
            this.dataContext.SaveChanges();
        }

        public void UpdatePokemonTeamEV(PokemonTeamEV pokemonTeamEVs)
        {
            this.dataContext.PokemonTeamEVs.Update(pokemonTeamEVs);
            this.dataContext.SaveChanges();
        }

        public void UpdatePokemonTeamIV(PokemonTeamIV pokemonTeamIVs)
        {
            this.dataContext.PokemonTeamIVs.Update(pokemonTeamIVs);
            this.dataContext.SaveChanges();
        }

        public void UpdatePokemonTeamMoveset(PokemonTeamMoveset pokemonTeamMoveset)
        {
            pokemonTeamMoveset = this.SortMoveset(pokemonTeamMoveset);

            this.dataContext.PokemonTeamMovesets.Update(pokemonTeamMoveset);
            this.dataContext.SaveChanges();
        }

        public PokemonTeamMoveset SortMoveset(PokemonTeamMoveset moveset)
        {
            // First Sort
            if (string.IsNullOrEmpty(moveset.FirstMove) && !string.IsNullOrEmpty(moveset.SecondMove))
            {
                moveset.FirstMove = moveset.SecondMove;
                moveset.SecondMove = null;
            }

            if (string.IsNullOrEmpty(moveset.SecondMove) && !string.IsNullOrEmpty(moveset.ThirdMove))
            {
                moveset.SecondMove = moveset.ThirdMove;
                moveset.ThirdMove = null;
            }

            if (string.IsNullOrEmpty(moveset.ThirdMove) && !string.IsNullOrEmpty(moveset.FourthMove))
            {
                moveset.ThirdMove = moveset.FourthMove;
                moveset.FourthMove = null;
            }

            // Second Sort
            if (string.IsNullOrEmpty(moveset.FirstMove) && !string.IsNullOrEmpty(moveset.SecondMove))
            {
                moveset.FirstMove = moveset.SecondMove;
                moveset.SecondMove = null;
            }

            if (string.IsNullOrEmpty(moveset.SecondMove) && !string.IsNullOrEmpty(moveset.ThirdMove))
            {
                moveset.SecondMove = moveset.ThirdMove;
                moveset.ThirdMove = null;
            }

            // Third Sort.
            if (string.IsNullOrEmpty(moveset.FirstMove) && !string.IsNullOrEmpty(moveset.SecondMove))
            {
                moveset.FirstMove = moveset.SecondMove;
                moveset.SecondMove = null;
            }

            return moveset;
        }

        public void UpdateEVYield(EVYield evYields)
        {
            this.dataContext.EVYields.Update(evYields);
            this.dataContext.SaveChanges();
        }

        public void UpdateEvolution(Evolution evolution)
        {
            this.dataContext.Evolutions.Update(evolution);
            this.dataContext.SaveChanges();
        }

        public void UpdateFormItem(FormItem formItem)
        {
            this.dataContext.FormItems.Update(formItem);
            this.dataContext.SaveChanges();
        }

        public void UpdateCaptureRate(CaptureRate captureRate)
        {
            this.dataContext.CaptureRates.Update(captureRate);
            this.dataContext.SaveChanges();
        }

        public void UpdatePokemon(Pokemon pokemon)
        {
            this.dataContext.Pokemon.Update(pokemon);
            this.dataContext.SaveChanges();
        }

        public void UpdatePokemonFormDetail(PokemonFormDetail pokemonFormDetail)
        {
            this.dataContext.PokemonFormDetails.Update(pokemonFormDetail);
            this.dataContext.SaveChanges();
        }

        public void UpdateGeneration(Generation generation)
        {
            this.dataContext.Generations.Update(generation);
            this.dataContext.SaveChanges();
        }

        public void UpdateGame(Game game)
        {
            this.dataContext.Games.Update(game);
            this.dataContext.SaveChanges();
        }

        public void UpdateType(Type type)
        {
            this.dataContext.Types.Update(type);
            this.dataContext.SaveChanges();
        }

        public void UpdateEggGroup(EggGroup eggGroup)
        {
            this.dataContext.EggGroups.Update(eggGroup);
            this.dataContext.SaveChanges();
        }

        public void UpdateForm(Form form)
        {
            this.dataContext.Forms.Update(form);
            this.dataContext.SaveChanges();
        }

        public void UpdateBattleItem(BattleItem battleItem)
        {
            this.dataContext.BattleItems.Update(battleItem);
            this.dataContext.SaveChanges();
        }

        public void UpdateAbility(Ability ability)
        {
            this.dataContext.Abilities.Update(ability);
            this.dataContext.SaveChanges();
        }

        public void UpdateClassification(Classification nature)
        {
            this.dataContext.Classifications.Update(nature);
            this.dataContext.SaveChanges();
        }

        public void UpdateNature(Nature classification)
        {
            this.dataContext.Natures.Update(classification);
            this.dataContext.SaveChanges();
        }

        public void UpdateLegendaryType(LegendaryType legendaryType)
        {
            this.dataContext.LegendaryTypes.Update(legendaryType);
            this.dataContext.SaveChanges();
        }

        public void DeletePokemon(int id)
        {
            Pokemon pokemon = this.GetPokemonById(id);
            this.dataContext.Pokemon.Remove(pokemon);
            this.dataContext.SaveChanges();
        }

        public void DeleteGeneration(int id)
        {
            Generation generation = this.GetGeneration(id);
            this.dataContext.Generations.Remove(generation);
            this.dataContext.SaveChanges();
        }

        public void DeleteGame(int id)
        {
            Game game = this.GetGame(id);
            this.dataContext.Games.Remove(game);
            this.dataContext.SaveChanges();
        }

        public void DeletePokemonGameDetail(int id)
        {
            PokemonGameDetail pokemonGameDetail = this.GetPokemonGameDetail(id);
            this.dataContext.PokemonGameDetails.Remove(pokemonGameDetail);
            this.dataContext.SaveChanges();
        }

        public void DeletePokemonGameDetails(List<PokemonGameDetail> details)
        {
            this.dataContext.PokemonGameDetails.RemoveRange(this.dataContext.PokemonGameDetails.Where(x => details.Select(y => y.Id).Contains(x.Id)));
            this.dataContext.SaveChanges();
        }

        public void DeleteType(int id)
        {
            Type type = this.GetType(id);
            this.dataContext.Types.Remove(type);
            this.dataContext.SaveChanges();
        }

        public void DeleteTypeChart(int id)
        {
            TypeChart typeChart = this.GetTypeChart(id);
            this.dataContext.TypeCharts.Remove(typeChart);
            this.dataContext.SaveChanges();
        }

        public void DeleteBattleItem(int id)
        {
            BattleItem battleItem = this.GetBattleItem(id);
            this.dataContext.BattleItems.Remove(battleItem);
            this.dataContext.SaveChanges();
        }

        public void DeleteAbility(int id)
        {
            Ability ability = this.GetAbility(id);
            this.dataContext.Abilities.Remove(ability);
            this.dataContext.SaveChanges();
        }

        public void DeleteLegendaryType(int id)
        {
            LegendaryType legendaryType = this.GetLegendaryType(id);
            this.dataContext.LegendaryTypes.Remove(legendaryType);
            this.dataContext.SaveChanges();
        }

        public void DeleteEggCycle(int id)
        {
            EggCycle eggCycle = this.GetEggCycle(id);
            this.dataContext.EggCycles.Remove(eggCycle);
            this.dataContext.SaveChanges();
        }

        public void DeleteCommentCategory(int id)
        {
            CommentCategory commentCategory = this.GetCommentCategory(id);
            this.dataContext.CommentCategories.Remove(commentCategory);
            this.dataContext.SaveChanges();
        }

        public void DeleteCommentPage(int id)
        {
            CommentPage commentPage = this.GetCommentPage(id);
            this.dataContext.CommentPages.Remove(commentPage);
            this.dataContext.SaveChanges();
        }

        public void DeletePokeball(int id)
        {
            Pokeball pokeball = this.GetPokeball(id);
            this.dataContext.Pokeballs.Remove(pokeball);
            this.dataContext.SaveChanges();
        }

        public void DeletePokeballCatchModifierDetail(int id)
        {
            PokeballCatchModifierDetail pokeballCatchModifier = this.GetPokeballCatchModifierDetail(id);
            this.dataContext.PokeballCatchModifierDetails.Remove(pokeballCatchModifier);
            this.dataContext.SaveChanges();
        }

        public void DeleteExperienceGrowth(int id)
        {
            ExperienceGrowth experienceGrowth = this.GetExperienceGrowth(id);
            this.dataContext.ExperienceGrowths.Remove(experienceGrowth);
            this.dataContext.SaveChanges();
        }

        public void DeleteStatus(int id)
        {
            Status status = this.GetStatus(id);
            this.dataContext.Statuses.Remove(status);
            this.dataContext.SaveChanges();
        }

        public void DeleteGenderRatio(int id)
        {
            GenderRatio genderRatio = this.GetGenderRatio(id);
            this.dataContext.GenderRatios.Remove(genderRatio);
            this.dataContext.SaveChanges();
        }

        public void DeleteCaptureRate(int id)
        {
            CaptureRate captureRate = this.GetCaptureRate(id);
            this.dataContext.CaptureRates.Remove(captureRate);
            this.dataContext.SaveChanges();
        }

        public void DeleteComment(int id)
        {
            Comment comment = this.GetComment(id);
            this.dataContext.Comments.Remove(comment);
            this.dataContext.SaveChanges();
        }

        public void DeleteEggGroup(int id)
        {
            EggGroup eggGroup = this.GetEggGroup(id);
            this.dataContext.EggGroups.Remove(eggGroup);
            this.dataContext.SaveChanges();
        }

        public void DeleteFormItem(int id)
        {
            FormItem formItem = this.GetFormItem(id);
            this.dataContext.FormItems.Remove(formItem);
            this.dataContext.SaveChanges();
        }

        public void DeleteEvolutionMethod(int id)
        {
            EvolutionMethod evolutionMethod = this.GetEvolutionMethod(id);
            this.dataContext.EvolutionMethods.Remove(evolutionMethod);
            this.dataContext.SaveChanges();
        }

        public void DeleteForm(int id)
        {
            Form form = this.GetForm(id);
            this.dataContext.Forms.Remove(form);
            this.dataContext.SaveChanges();
        }

        public void DeleteBaseHappiness(int id)
        {
            BaseHappiness baseHappiness = this.GetBaseHappiness(id);
            this.dataContext.BaseHappiness.Remove(baseHappiness);
            this.dataContext.SaveChanges();
        }

        public void DeleteReviewedPokemon(int id)
        {
            ReviewedPokemon reviewedPokemon = this.GetReviewedPokemon(id);
            this.dataContext.ReviewedPokemons.Remove(reviewedPokemon);
            this.dataContext.SaveChanges();
        }

        public void DeleteMessage(int id)
        {
            Message message = this.GetMessage(id);
            this.dataContext.Messages.Remove(message);
            this.dataContext.SaveChanges();
        }

        public void DeletePokemonTeam(int id)
        {
            PokemonTeam pokemonTeam = this.GetPokemonTeamNoIncludes(id);
            List<int> pokemonTeamDetailIds = pokemonTeam.GrabPokemonTeamDetailIds();
            this.dataContext.PokemonTeams.Remove(pokemonTeam);
            this.dataContext.SaveChanges();

            foreach (var p in pokemonTeamDetailIds)
            {
                this.DeletePokemonTeamDetail(p);
            }
        }

        public void RemovePokemonFromTeam(PokemonTeam team, PokemonTeamDetail teamDetail)
        {
            if (team.FirstPokemonId == teamDetail.Id)
            {
                team.FirstPokemonId = null;
            }
            else if (team.SecondPokemonId == teamDetail.Id)
            {
                team.SecondPokemonId = null;
            }
            else if (team.ThirdPokemonId == teamDetail.Id)
            {
                team.ThirdPokemonId = null;
            }
            else if (team.FourthPokemonId == teamDetail.Id)
            {
                team.FourthPokemonId = null;
            }
            else if (team.FifthPokemonId == teamDetail.Id)
            {
                team.FifthPokemonId = null;
            }
            else if (team.SixthPokemonId == teamDetail.Id)
            {
                team.SixthPokemonId = null;
            }

            team = this.ShiftPokemonTeam(team);
            this.UpdatePokemonTeam(team);
        }

        public PokemonTeam ShiftPokemonTeam(PokemonTeam pokemonTeam)
        {
            if (pokemonTeam.FirstPokemonId == null && pokemonTeam.SecondPokemonId != null)
            {
                pokemonTeam.FirstPokemonId = pokemonTeam.SecondPokemonId;
                pokemonTeam.SecondPokemonId = null;
            }

            if (pokemonTeam.SecondPokemonId == null && pokemonTeam.ThirdPokemonId != null)
            {
                pokemonTeam.SecondPokemonId = pokemonTeam.ThirdPokemonId;
                pokemonTeam.ThirdPokemonId = null;
            }

            if (pokemonTeam.ThirdPokemonId == null && pokemonTeam.FourthPokemonId != null)
            {
                pokemonTeam.ThirdPokemonId = pokemonTeam.FourthPokemonId;
                pokemonTeam.FourthPokemonId = null;
            }

            if (pokemonTeam.FourthPokemonId == null && pokemonTeam.FifthPokemonId != null)
            {
                pokemonTeam.FourthPokemonId = pokemonTeam.FifthPokemonId;
                pokemonTeam.FifthPokemonId = null;
            }

            if (pokemonTeam.FifthPokemonId == null && pokemonTeam.SixthPokemonId != null)
            {
                pokemonTeam.FifthPokemonId = pokemonTeam.SixthPokemonId;
                pokemonTeam.SixthPokemonId = null;
            }

            return pokemonTeam;
        }

        public void DeletePokemonTeamDetail(int id)
        {
            PokemonTeamDetail pokemonTeamDetail = this.GetPokemonTeamDetailNoIncludes(id);
            PokemonTeam pokemonTeam = this.GetPokemonTeamFromPokemonNoIncludes(pokemonTeamDetail.Id);
            if (pokemonTeam != null)
            {
                this.RemovePokemonFromTeam(pokemonTeam, pokemonTeamDetail);
            }

            int? evId = pokemonTeamDetail.PokemonTeamEVId;
            int? ivId = pokemonTeamDetail.PokemonTeamIVId;
            int? movesetId = pokemonTeamDetail.PokemonTeamMovesetId;
            this.dataContext.PokemonTeamDetails.Remove(pokemonTeamDetail);
            this.dataContext.SaveChanges();

            if (evId != null)
            {
                this.DeletePokemonTeamEV((int)evId);
            }

            if (ivId != null)
            {
                this.DeletePokemonTeamIV((int)ivId);
            }

            if (movesetId != null)
            {
                this.DeletePokemonTeamMoveset((int)movesetId);
            }
        }

        public void DeletePokemonTeamEV(int id)
        {
            PokemonTeamEV pokemonTeamDetailEV = this.GetPokemonTeamEV(id);
            this.dataContext.PokemonTeamEVs.Remove(pokemonTeamDetailEV);
            this.dataContext.SaveChanges();
        }

        public void DeletePokemonTeamIV(int id)
        {
            PokemonTeamIV pokemonTeamDetailIV = this.GetPokemonTeamIV(id);
            this.dataContext.PokemonTeamIVs.Remove(pokemonTeamDetailIV);
            this.dataContext.SaveChanges();
        }

        public void DeletePokemonTeamMoveset(int id)
        {
            PokemonTeamMoveset pokemonTeamDetailMoveset = this.GetPokemonTeamMoveset(id);
            this.dataContext.PokemonTeamMovesets.Remove(pokemonTeamDetailMoveset);
            this.dataContext.SaveChanges();
        }

        public void DeleteClassification(int id)
        {
            Classification classification = this.GetClassification(id);
            this.dataContext.Classifications.Remove(classification);
            this.dataContext.SaveChanges();
        }

        public void DeleteNature(int id)
        {
            Nature nature = this.GetNature(id);
            this.dataContext.Natures.Remove(nature);
            this.dataContext.SaveChanges();
        }

        public string FormatPokemonName(string pokemonName)
        {
            pokemonName.ToLower();

            if (pokemonName.Contains("type"))
            {
                pokemonName = "Type: Null";
            }

            if (pokemonName.Contains('_'))
            {
                pokemonName = pokemonName.Replace('_', ' ');
            }

            if (pokemonName == "flabe" || pokemonName == "flabeb" || pokemonName == "flabebe")
            {
                pokemonName = "Flabébé";
            }

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            pokemonName = textInfo.ToTitleCase(pokemonName);

            if (pokemonName.Length > 1 && pokemonName.Substring(pokemonName.Length - 2, 2) == "-O")
            {
                pokemonName = string.Concat(pokemonName.Remove(pokemonName.Length - 2, 2), "-o");
            }

            return pokemonName;
        }

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
