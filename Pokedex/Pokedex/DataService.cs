using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Pokedex.DataAccess.Models;
using Pokedex.Models;

namespace Pokedex
{
    public class DataService
    {
        private readonly DataContext _dataContext;

        public DataService(DataContext dataContext)
        {
            this._dataContext = dataContext;

        }

        public bool CheckIfAltForm(int id)
        {
            return this._dataContext.PokemonFormDetails.ToList().Exists(x => x.AltFormPokemonId == id);
        }

        public Ability GetAbility(int id)
        {
            return this._dataContext.Abilities
                .ToList()
                .Find(x => x.Id == id);
        }

        public Ability GetAbilityByName(string name)
        {
            return this._dataContext.Abilities
                .ToList()
                .Find(x => x.Name == name);
        }

        public List<Ability> GetAbilities()
        {
            return this._dataContext.Abilities.OrderBy(x => x.Name).ToList();
        }

        public Game GetGame(int id)
        {
            return this._dataContext.Games
                .ToList()
                .Find(x => x.Id == id);
        }

        public List<Game> GetGames()
        {
            return this._dataContext.Games
                .ToList();
        }

        public LegendaryType GetLegendaryType(int id)
        {
            return this._dataContext.LegendaryTypes.OrderBy(x => x.Type).ToList().Find(x => x.Id == id);
        }

        public List<LegendaryType> GetLegendaryTypes()
        {
            return this._dataContext.LegendaryTypes.OrderBy(x => x.Type).ToList();
        }

        public PokemonLegendaryDetail GetLegendaryDetail(int pokemonId)
        {
            return this._dataContext.PokemonLegendaryDetails.ToList().Find(x => x.PokemonId == pokemonId);
        }

        public Type GetType(int id)
        {
            return this._dataContext.Types
                .ToList()
                .Find(x => x.Id == id);
        }

        public TypeChart GetTypeChart(int id)
        {
            return this._dataContext.TypeCharts
                .ToList()
                .Find(x => x.Id == id);
        }

        public List<Type> GetTypes()
        {
            return this._dataContext.Types.OrderBy(x => x.Name).ToList();
        }

        public FormItem GetFormItem(int id)
        {
            return this._dataContext.FormItems
                .ToList()
                .Find(x => x.Id == id);
        }

        public FormItem GetFormItemByPokemonId(int id)
        {
            return this._dataContext.FormItems
                .ToList()
                .Find(x => x.PokemonId == id);
        }

        public List<FormItem> GetFormItems()
        {
            List<FormItem> formItemList = this._dataContext.FormItems
                .Include(x => x.Pokemon)
                .OrderBy(x => x.Pokemon.PokedexNumber)
                .ToList();

            foreach(var f in formItemList)
            {
                f.Pokemon.Name += " (" + this.GetPokemonFormName(f.PokemonId) + ")";
            }

            return formItemList;
        }

        public List<Type> GetPokemonTypes(int pokemonId)
        {
            PokemonTypeDetail typeDetail = this._dataContext.PokemonTypeDetails
                                                .Include(x => x.Pokemon)
                                                .Include(x => x.PrimaryType)
                                                .Include(x => x.SecondaryType)
                                                .ToList()
                                                .Find(x => x.Pokemon.Id == pokemonId);
            List<Type> types = new List<Type>();
            types.Add(this.GetType(typeDetail.PrimaryType.Id));
            if (typeDetail.SecondaryType != null)
            {
                types.Add(this.GetType(typeDetail.SecondaryType.Id));
            }

            return types;
        }

        public EggGroup GetEggGroup(int id)
        {
            return this._dataContext.EggGroups
                .ToList()
                .Find(x => x.Id == id);
        }

        public List<EggGroup> GetEggGroups()
        {
            return this._dataContext.EggGroups.OrderBy(x => x.Name).ToList();
        }

        public Move GetMove(int id)
        {
            return this._dataContext.Moves
                .ToList()
                .Find(x => x.Id == id);
        }

        public List<Move> GetMoves()
        {
            return this._dataContext.Moves.OrderBy(x => x.Name).ToList();
        }

        public List<MoveCategory> GetMoveCategories()
        {
            return this._dataContext.MoveCategories.ToList();
        }

        public List<Evolution> GetEvolutions()
        {
            return this._dataContext.Evolutions
                .Include(x => x.PreevolutionPokemon)
                .Include(x => x.EvolutionPokemon)
                .Include(x => x.EvolutionMethod)
                .ToList();
        }

        public Evolution GetPreEvolution(int pokemonId)
        {
            Evolution preEvolution = this.GetEvolutions().Find(x => x.EvolutionPokemon.Id == pokemonId && x.PreevolutionPokemon.IsComplete);

            if (preEvolution != null && this.CheckIfAltForm(preEvolution.PreevolutionPokemonId))
            {
                preEvolution.PreevolutionPokemon.Name += " (" + this.GetPokemonFormName(preEvolution.PreevolutionPokemonId) + ")";
            }

            return preEvolution;
        }

        public Evolution GetPreEvolutionIncludeIncomplete(int pokemonId)
        {
            Evolution preEvolution = this.GetEvolutions().Find(x => x.EvolutionPokemon.Id == pokemonId);

            if (preEvolution != null && this.CheckIfAltForm(preEvolution.PreevolutionPokemonId))
            {
                preEvolution.PreevolutionPokemon.Name += " (" + this.GetPokemonFormName(preEvolution.PreevolutionPokemonId) + ")";
            }

            return preEvolution;
        }

        public Evolution GetPreEvolutionNoEdit(int pokemonId)
        {
            return this._dataContext.Evolutions
                .ToList()
                .Find(x => x.EvolutionPokemonId == pokemonId);
        }

        public List<Evolution> GetPokemonEvolutions(int pokemonId)
        {
            List<Evolution> evolutions = this.GetEvolutions()
                .Where(x => x.PreevolutionPokemon.Id == pokemonId && x.PreevolutionPokemon.IsComplete == true && x.EvolutionPokemon.IsComplete == true)
                .OrderBy(x => x.EvolutionPokemon.PokedexNumber)
                .ThenBy(x => x.EvolutionPokemon.Id)
                .ToList();

            foreach(var e in evolutions)
            {
                if (this.CheckIfAltForm(e.EvolutionPokemonId))
                {
                    e.EvolutionPokemon.Name += " (" + this.GetPokemonFormName(e.EvolutionPokemonId) + ")";
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

            foreach(var e in evolutions)
            {
                if (this.CheckIfAltForm(e.EvolutionPokemonId))
                {
                    e.EvolutionPokemon.Name += " (" + this.GetPokemonFormName(e.EvolutionPokemonId) + ")";
                }
            }

            return evolutions;
        }

        public List<Evolution> GetPokemonEvolutionsNoEdit(int pokemonId)
        {
            return this._dataContext.Evolutions
                .Where(x => x.PreevolutionPokemonId == pokemonId)
                .ToList();
        }

        public List<Form> GetForms()
        {
            return this._dataContext.Forms
                .OrderBy(x => x.Name)
                .ToList();
        }

        public Form GetForm(int id)
        {
            return this._dataContext.Forms
                .ToList()
                .Find(x => x.Id == id);
        }

        public Form GetFormByName(string formName)
        {
            return this._dataContext.Forms.ToList().Find(x => x.Name == formName);
        }

        public List<PokemonFormDetail> GetPokemonForms(int pokemonId)
        {
            return this._dataContext.PokemonFormDetails
                .Include(x => x.Form)
                .Include(x => x.OriginalPokemon)
                .Include(x => x.AltFormPokemon)
                .Where(x => x.OriginalPokemon.Id == pokemonId && x.OriginalPokemon.IsComplete == true && x.AltFormPokemon.IsComplete == true)
                .OrderBy(x => x.AltFormPokemon.Id)
                .ToList();
        }

        public List<PokemonFormDetail> GetPokemonFormsWithIncomplete(int pokemonId)
        {
            return this._dataContext.PokemonFormDetails
                .Include(x => x.Form)
                .Include(x => x.OriginalPokemon)
                .Include(x => x.AltFormPokemon)
                .Where(x => x.OriginalPokemon.Id == pokemonId)
                .OrderBy(x => x.AltFormPokemon.Id)
                .ToList();
        }

        public string GetPokemonFormName(int pokemonId)
        {
            PokemonFormDetail formDetail = this._dataContext.PokemonFormDetails
                .Include(x => x.Form)
                .Include(x => x.OriginalPokemon)
                .Include(x => x.AltFormPokemon)
                .ToList()
                .Find(x => x.AltFormPokemon.Id == pokemonId);
            return formDetail.Form.Name;
        }

        public PokemonGameDetail GetPokemonGameDetail(int id)
        {
            return this._dataContext.PokemonGameDetails
                .ToList()
                .Find(x => x.Id == id);
        }

        public List<PokemonGameDetail> GetPokemonGameDetails(int pokemonId)
        {
            return this._dataContext.PokemonGameDetails
                .Include(x => x.Pokemon)
                    .Include("Pokemon.Game")
                .Include(x => x.Game)
                .Where(x => x.PokemonId == pokemonId)
                .ToList();
        }

        public List<PokemonGameDetail> GetAllPokemonGameDetails()
        {
            return this._dataContext.PokemonGameDetails
                .Include(x => x.Pokemon)
                .Include(x => x.Game)
                .ToList();
        }

        public List<PokemonGameDetail> GetPokemonGameDetailsByGeneration(int generationId)
        {
            return this._dataContext.PokemonGameDetails
                .Include(x => x.Pokemon)
                .Include(x => x.Game)
                .Include("Game.Generation")
                .Where(x => x.Game.GenerationId == generationId)
                .ToList();
        }

        public List<PokemonGameDetail> GetPokemonGameDetailsByGame(int gameId)
        {
            return this._dataContext.PokemonGameDetails
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
            return this._dataContext.Pokemon
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
            return this._dataContext.Pokemon
                .Include(x => x.EggCycle)
                .Include(x => x.GenderRatio)
                .Include(x => x.Classification)
                .Include(x => x.Game)
                    .Include("Game.Generation")
                .Include(x => x.ExperienceGrowth)
                .Include(x => x.CaptureRate)
                .Include(x => x.BaseHappiness)
                .ToList()
                .Find(x => x.Id == id);
        }

        public Pokemon GetPokemonByIdNoIncludes(int id)
        {
            return this._dataContext.Pokemon
                .ToList()
                .Find(x => x.Id == id);
        }

        public Pokemon GetPokemonNoIncludesById(int id)
        {
            return this._dataContext.Pokemon
                .ToList()
                .Find(x => x.Id == id);
        }

        public PokemonViewModel GetPokemonDetails(Pokemon pokemon, Form form, AppConfig appConfig)
        {
            PokemonTypeDetail pokemonTypes = this.GetPokemonWithTypes(pokemon.Id);
            PokemonAbilityDetail pokemonAbilities = this.GetPokemonWithAbilities(pokemon.Id);
            PokemonEggGroupDetail pokemonEggGroups = this.GetPokemonWithEggGroups(pokemon.Id);
            
            PokemonViewModel pokemonViewModel = new PokemonViewModel()
            {
                Pokemon = pokemon,
                BaseStats = this.GetBaseStat(pokemon.Id),
                EVYields = this.GetEVYield(pokemon.Id),
                PrimaryType = pokemonTypes.PrimaryType,
                SecondaryType = pokemonTypes.SecondaryType,
                PrimaryAbility = pokemonAbilities.PrimaryAbility,
                SecondaryAbility = pokemonAbilities.SecondaryAbility,
                HiddenAbility = pokemonAbilities.HiddenAbility,
                SpecialEventAbility = pokemonAbilities.SpecialEventAbility,
                PrimaryEggGroup = pokemonEggGroups.PrimaryEggGroup,
                SecondaryEggGroup = pokemonEggGroups.SecondaryEggGroup,
                PreEvolution = this.GetPreEvolution(pokemon.Id),
                Evolutions = this.GetPokemonEvolutions(pokemon.Id),
                Effectiveness = this.GetTypeChartPokemon(pokemon.Id),
                AppConfig = appConfig,
            };

            if(form != null)
            {
                pokemonViewModel.Form = form;
            }

            return pokemonViewModel;
        }
        
        public AllAdminPokemonViewModel GetAllAdminPokemonDetails()
        {
            return new AllAdminPokemonViewModel(){
                AllAltForms = this.GetAllAltForms(),
                AllEvolutions = this.GetEvolutions(),
                AllTypings = this.GetAllPokemonWithTypesAndIncomplete(),
                AllAbilities = this.GetAllPokemonWithAbilitiesAndIncomplete(),
                AllEggGroups = this.GetAllPokemonWithEggGroupsAndIncomplete(),
                AllBaseStats = this.GetBaseStatsWithIncomplete(),
                AllEVYields = this.GetEVYieldsWithIncomplete(),
                AllLegendaryDetails = this.GetAllPokemonWithLegendaryTypes(),
            };
        }

        public List<Pokemon> GetAllPokemon()
        {
            return this._dataContext.Pokemon
                .Include(x => x.EggCycle)
                .Include(x => x.GenderRatio)
                .Include(x => x.Classification)
                .Include(x => x.Game)
                    .Include("Game.Generation")
                .Include(x => x.ExperienceGrowth)
                .Include(x => x.CaptureRate)
                .Include(x => x.BaseHappiness)
                .Where(x => x.IsComplete == true)
                .OrderBy(x => x.PokedexNumber)
                .ThenBy(x => x.Id)
                .ToList();
        }

        public List<Pokemon> GetAllPokemonNoIncludes()
        {
            return this._dataContext.Pokemon
                .Where(x => x.IsComplete == true)
                .OrderBy(x => x.PokedexNumber)
                .ThenBy(x => x.Id)
                .ToList();
        }

        public List<PokemonEggGroupDetail> GetAllBreedablePokemon()
        {
            List<Pokemon> pokemonList = this._dataContext.Pokemon
                .Where(x => x.IsComplete == true)
                .ToList();

            List<PokemonFormDetail> formDetails = new List<PokemonFormDetail>();
            List<string> formNames = new List<string>(){
                "Mega",
                "Mega X",
                "Mega Y",
                "Core",
                "Blade",
                "Crowned",
                "Ash",
            };

            foreach(var n in formNames)
            {
                formDetails.AddRange(this.GetPokemonFormDetailsByFormName(n));
            }
            
            List<PokemonEggGroupDetail> eggGroupDetails =  this._dataContext.PokemonEggGroupDetails
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
            return this._dataContext.Pokemon
                .Include(x => x.EggCycle)
                .Include(x => x.GenderRatio)
                .Include(x => x.Classification)
                .Include(x => x.Game)
                    .Include("Game.Generation")
                .Include(x => x.ExperienceGrowth)
                .Include(x => x.CaptureRate)
                .Include(x => x.BaseHappiness)
                .OrderBy(x => x.PokedexNumber)
                .ThenBy(x => x.Id)
                .ToList();
        }

        public List<Pokemon> GetAllPokemonWithoutForms()
        {
            List<Pokemon> pokemonList = this.GetAllPokemon();
            List<Pokemon> altFormList = this.GetAllAltForms().Select(x => x.AltFormPokemon).ToList();
            pokemonList = pokemonList.Where(x => !altFormList.Any(y => y.Id == x.Id)).ToList();

            return pokemonList;
        }

        public List<Pokemon> GetAllPokemonWithoutFormsWithIncomplete()
        {
            List<Pokemon> pokemonList = this.GetAllPokemonIncludeIncomplete();
            List<Pokemon> altFormList = this.GetAllAltForms().Select(x => x.AltFormPokemon).ToList();
            pokemonList = pokemonList.Where(x => !altFormList.Any(y => y.Id == x.Id)).ToList();
            return pokemonList;
        }

        public List<int> GetGenerationsFromPokemon()
        {
            List<Pokemon> pokemonList = this.GetAllPokemon();
            List<Pokemon> altFormList = this._dataContext.PokemonFormDetails.Select(x => x.AltFormPokemon).ToList();
            pokemonList = pokemonList.Where(x => !altFormList.Any(y => y.Id == x.Id)).ToList();

            List<int> generationList = pokemonList.Select(x => x.Game.GenerationId).Distinct().OrderBy(x => x).ToList();

            return generationList;
        }

        public List<int> GetGenerationsFromPokemonWithIncomplete()
        {
            List<Pokemon> pokemonList = this.GetAllPokemonIncludeIncomplete();
            List<Pokemon> altFormList = this._dataContext.PokemonFormDetails.Select(x => x.AltFormPokemon).ToList();
            pokemonList = pokemonList.Where(x => !altFormList.Any(y => y.Id == x.Id)).ToList();

            List<int> generationList = pokemonList.Select(x => x.Game.GenerationId).Distinct().OrderBy(x => x).ToList();

            return generationList;
        }

        public List<PokemonTeam> GetPokemonTeams()
        {
            return this._dataContext.PokemonTeams
                .Include(x => x.Game)
                .Include(x => x.FirstPokemon)
                    .Include("FirstPokemon.Pokemon")
                        .Include("FirstPokemon.Pokemon.Game")
                            .Include("FirstPokemon.Pokemon.Game.Generation")
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
            return this.GetPokemonTeams().Where(x => x.User.Username == username).ToList();
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
            return this._dataContext.PokemonTeams
                .ToList()
                .Find(x => x.Id == id);
        }

        public PokemonTeam GetPokemonTeamFromPokemon(int id)
        {
            return this._dataContext.PokemonTeams
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
            return this._dataContext.PokemonTeams
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
            return this._dataContext.PokemonTeamDetails
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
            return this._dataContext.PokemonTeamDetails
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
            return this._dataContext.PokemonTeamDetails
                .ToList()
                .Find(x => x.Id == id);
        }

        public PokemonTeamEV GetPokemonTeamEV(int id)
        {
            return this._dataContext.PokemonTeamEVs
                .ToList()
                .Find(x => x.Id == id);
        }

        public PokemonTeamIV GetPokemonTeamIV(int id)
        {
            return this._dataContext.PokemonTeamIVs
                .ToList()
                .Find(x => x.Id == id);
        }

        public PokemonTeamMoveset GetPokemonTeamMoveset(int id)
        {
            return this._dataContext.PokemonTeamMovesets
                .ToList()
                .Find(x => x.Id == id);
        }

        public List<Pokemon> GetAltForms(int pokemonId)
        {
            List<PokemonFormDetail> pokemonFormList = this._dataContext.PokemonFormDetails
                .Include(x => x.AltFormPokemon)
                    .Include("AltFormPokemon.EggCycle")
                    .Include("AltFormPokemon.GenderRatio")
                    .Include("AltFormPokemon.Classification")
                    .Include("AltFormPokemon.Game")
                    .Include("AltFormPokemon.ExperienceGrowth")
                    .Include("AltFormPokemon.CaptureRate")
                    .Include("AltFormPokemon.BaseHappiness")
                .Include(x => x.Form)
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
            PokemonFormDetail pokemonForm =  this._dataContext.PokemonFormDetails
                .Include(x => x.AltFormPokemon)
                .Include(x => x.Form)
                .ToList()
                .Find(x => x.AltFormPokemonId == pokemonId);

            Pokemon pokemon = pokemonForm.AltFormPokemon;

            pokemon.Name += string.Concat(" (", pokemonForm.Form.Name, ")");

            return pokemon;
        }

        public List<PokemonFormDetail> GetAllAltForms()
        {
            return this._dataContext.PokemonFormDetails
                .Include(x => x.AltFormPokemon)
                .Include(x => x.OriginalPokemon)
                .Include(x => x.Form)
                .ToList();
        }

        public List<Pokemon> GetAllPokemonOnlyForms()
        {
            List<Pokemon> pokemonList = this.GetAllAltForms().Select(x => x.AltFormPokemon).OrderBy(x => x.PokedexNumber).ToList();

            foreach(var p in pokemonList)
            {
                p.Name += " (" + this.GetPokemonFormName(p.Id) + ")";
            }

            return pokemonList;
        }

        public List<PokemonFormDetail> GetAllAltFormsOnlyComplete()
        {
            return this.GetAllAltForms().Where(x => x.AltFormPokemon.IsComplete).ToList();
        }

        public PokemonFormDetail GetPokemonFormDetailByAltFormId(int pokemonId)
        {
            return this._dataContext.PokemonFormDetails
                .Include(x => x.AltFormPokemon)
                .Include(x => x.OriginalPokemon)
                .Include(x => x.Form)
                .ToList()
                .Find(x => x.AltFormPokemonId == pokemonId);
        }

        public Pokemon GetOriginalPokemonByAltFormId(int pokemonId)
        {
            return this._dataContext.PokemonFormDetails
                .Include(x => x.AltFormPokemon)
                .Include(x => x.OriginalPokemon)
                .Include(x => x.Form)
                .ToList()
                .Find(x => x.AltFormPokemonId == pokemonId).OriginalPokemon;
        }

        public Form GetFormByAltFormId(int pokemonId)
        {
            PokemonFormDetail details = this._dataContext.PokemonFormDetails
                .Include(x => x.AltFormPokemon)
                .Include(x => x.Form)
                .ToList()
                .Find(x => x.AltFormPokemonId == pokemonId);

            return details.Form;
        }

        public List<PokemonFormDetail> GetPokemonFormDetailsForPokemon(int pokemonId)
        {
            return this._dataContext.PokemonFormDetails
                .Where(x => x.OriginalPokemonId == pokemonId)
                .ToList();
        }

        public List<PokemonFormDetail> GetPokemonFormDetails()
        {
            return this._dataContext.PokemonFormDetails
                .Include(x => x.OriginalPokemon)
                .Include(x => x.AltFormPokemon)
                .Include(x => x.Form)
                .ToList();
        }

        public List<PokemonFormDetail> GetPokemonFormDetailsByFormName(string formName)
        {
            return this._dataContext.PokemonFormDetails
                .Include(x => x.OriginalPokemon)
                .Include(x => x.AltFormPokemon)
                .Include(x => x.Form)
                .Where(x => x.Form.Name == formName)
                .ToList();
        }

        public PokemonTypeDetail GetPokemonWithTypes(int pokemonId)
        {
            return this._dataContext.PokemonTypeDetails
                .Include(x => x.Pokemon)
                .Include(x => x.PrimaryType)
                .Include(x => x.SecondaryType)
                .ToList()
                .Find(x => x.Pokemon.Id == pokemonId);
        }

        public PokemonTypeDetail GetPokemonWithTypesNoIncludes(int pokemonId)
        {
            return this._dataContext.PokemonTypeDetails
                .ToList()
                .Find(x => x.PokemonId == pokemonId);
        }

        public List<PokemonTypeDetail> GetAllPokemonWithTypes()
        {
            List<PokemonTypeDetail> pokemonList = this._dataContext.PokemonTypeDetails
                                                        .Include(x => x.Pokemon)
                                                            .Include("Pokemon.Game")
                                                        .Include(x => x.PrimaryType)
                                                        .Include(x => x.SecondaryType)
                                                        .Where(x => x.Pokemon.IsComplete == true)
                                                        .OrderBy(x => x.Pokemon.PokedexNumber)
                                                        .ThenBy(x => x.PokemonId)
                                                        .ToList();
            List<Pokemon> altFormList = this._dataContext.PokemonFormDetails.Select(x => x.AltFormPokemon).ToList();
            pokemonList = pokemonList.Where(x => !altFormList.Any(y => y.Id == x.PokemonId)).ToList();

            return pokemonList;
        }

        public List<PokemonTypeDetail> GetAllPokemonWithSpecificTypes(int primaryTypeId, int secondaryTypeId)
        {
            List<PokemonTypeDetail> pokemonList = this._dataContext.PokemonTypeDetails
                                                        .Include(x => x.Pokemon)
                                                            .Include("Pokemon.Game")
                                                        .Include(x => x.PrimaryType)
                                                        .Include(x => x.SecondaryType)
                                                        .Where(x => x.Pokemon.IsComplete == true)
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

            pokemonList = pokemonList.OrderBy(x => x.Pokemon.PokedexNumber).ToList();

            return pokemonList;
        }

        public List<PokemonTypeDetail> GetAllPokemonWithTypesAndIncomplete()
        {
            List<PokemonTypeDetail> pokemonList = this._dataContext.PokemonTypeDetails
                                                        .Include(x => x.Pokemon)
                                                        .Include(x => x.PrimaryType)
                                                        .Include(x => x.SecondaryType)
                                                        .ToList();

            pokemonList = pokemonList.OrderBy(x => x.Pokemon.Id).ThenBy(x => x.Pokemon.Id).ToList();

            return pokemonList;
        }

        public PokemonAbilityDetail GetPokemonWithAbilities(int pokemonId)
        {
            return this._dataContext.PokemonAbilityDetails.Include(x => x.Pokemon)
                .Include(x => x.PrimaryAbility)
                .Include(x => x.SecondaryAbility)
                .Include(x => x.HiddenAbility)
                .Include(x => x.SpecialEventAbility)
                .ToList()
                .Find(x => x.Pokemon.Id == pokemonId);
        }

        public List<Ability> GetAbilitiesForPokemon(int pokemonId)
        {
            List<Ability> abilityList = new List<Ability>();
            PokemonAbilityDetail pokemonAbilityDetail = this._dataContext.PokemonAbilityDetails.Include(x => x.Pokemon)
                .Include(x => x.PrimaryAbility)
                .Include(x => x.SecondaryAbility)
                .Include(x => x.HiddenAbility)
                .Include(x => x.SpecialEventAbility)
                .ToList()
                .Find(x => x.Pokemon.Id == pokemonId);

            abilityList.Add(pokemonAbilityDetail.PrimaryAbility);
            if(pokemonAbilityDetail.SecondaryAbility != null)
            {
                abilityList.Add(pokemonAbilityDetail.SecondaryAbility);
            }

            if(pokemonAbilityDetail.HiddenAbility != null)
            {
                abilityList.Add(pokemonAbilityDetail.HiddenAbility);
            }

            if(pokemonAbilityDetail.SpecialEventAbility != null)
            {
                abilityList.Add(pokemonAbilityDetail.SpecialEventAbility);
            }

            return abilityList;
        }

        public PokemonAbilityDetail GetPokemonWithAbilitiesNoIncludes(int pokemonId)
        {
            return this._dataContext.PokemonAbilityDetails.Include(x => x.Pokemon)
                .ToList()
                .Find(x => x.PokemonId == pokemonId);
        }

        public List<PokemonLegendaryDetail> GetAllPokemonWithLegendaryTypes()
        {
            return this._dataContext.PokemonLegendaryDetails
                .Include(x => x.Pokemon)
                .Include(x => x.LegendaryType)
                .ToList();
        }

        public List<PokemonLegendaryDetail> GetAllPokemonWithLegendaryTypesAndIncomplete()
        {
            return this._dataContext.PokemonLegendaryDetails
                .Include(x => x.Pokemon)
                .Include(x => x.LegendaryType)
                .ToList();
        }

        public List<PokemonAbilityDetail> GetAllPokemonWithAbilitiesAndIncomplete()
        {
            return this._dataContext.PokemonAbilityDetails
                .Include(x => x.Pokemon)
                .Include(x => x.PrimaryAbility)
                .Include(x => x.SecondaryAbility)
                .Include(x => x.HiddenAbility)
                .Include(x => x.SpecialEventAbility)
                .ToList();
        }

        public PokemonEggGroupDetail GetPokemonWithEggGroups(int pokemonId)
        {
            return this._dataContext.PokemonEggGroupDetails
                .Include(x => x.Pokemon)
                .Include(x => x.PrimaryEggGroup)
                .Include(x => x.SecondaryEggGroup)
                .ToList()
                .Find(x => x.Pokemon.Id == pokemonId);
        }

        public PokemonEggGroupDetail GetPokemonWithEggGroupsFromPokemonName(string pokemonName)
        {
            return this._dataContext.PokemonEggGroupDetails
                .Include(x => x.Pokemon)
                    .Include("Pokemon.GenderRatio")
                .Include(x => x.PrimaryEggGroup)
                .Include(x => x.SecondaryEggGroup)
                .ToList()
                .Find(x => x.Pokemon.Name == pokemonName);
        }

        public List<PokemonEggGroupDetail> GetAllPokemonWithSpecificEggGroups(int primaryEggGroupId, int? secondaryEggGroupId)
        {
            List<PokemonEggGroupDetail> pokemonList = this._dataContext.PokemonEggGroupDetails
                .Include(x => x.Pokemon)
                    .Include("Pokemon.GenderRatio")
                .Include(x => x.PrimaryEggGroup)
                .Include(x => x.SecondaryEggGroup)
                .ToList();

            List<PokemonEggGroupDetail> finalPokemonList = new List<PokemonEggGroupDetail>();

            finalPokemonList.AddRange(pokemonList.Where(x => x.PrimaryEggGroupId == primaryEggGroupId || x.SecondaryEggGroupId == primaryEggGroupId).ToList());

            if(secondaryEggGroupId != null)
            {
                finalPokemonList.AddRange(pokemonList.Where(x => x.PrimaryEggGroupId == secondaryEggGroupId || x.SecondaryEggGroupId == secondaryEggGroupId).ToList());
            }

            return finalPokemonList;
        }

        public List<Pokemon> GetSurroundingPokemon(int pokedexNumber)
        {
            List<Pokemon> surroundingPokemon = new List<Pokemon>();
            List<int> pokemonIds = this.GetPokedexNumbers();
            int previousId, nextId, index = pokemonIds.FindIndex(x => x == pokedexNumber);

            if(pokemonIds[index] == pokemonIds.First())
            {
                previousId = pokemonIds.Last();
            }
            else
            {
                previousId = pokemonIds[index - 1];
            }

            if(pokemonIds[index] == pokemonIds.Last())
            {
                nextId = pokemonIds.First();
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
            List<int> ids = this.GetAllPokemon().Select(x => x.Id).ToList();
            List<int> altIds = this.GetAllAltForms().Select(x => x.AltFormPokemonId).ToList();
            ids = ids.Where(x => !altIds.Any(y => y == x)).ToList();

            return ids;
        }

        public PokemonEggGroupDetail GetPokemonWithEggGroupsNoIncludes(int pokemonId)
        {
            return this._dataContext.PokemonEggGroupDetails
                .ToList()
                .Find(x => x.PokemonId == pokemonId);
        }

        public List<PokemonEggGroupDetail> GetAllPokemonWithEggGroupsAndIncomplete()
        {
            return this._dataContext.PokemonEggGroupDetails
                .Include(x => x.Pokemon)
                .Include(x => x.PrimaryEggGroup)
                .Include(x => x.SecondaryEggGroup)
                .ToList();
        }

        public BaseStat GetPokemonBaseStats(int pokemonId)
        {
            return this._dataContext.BaseStats
                .Include(x => x.Pokemon)
                .ToList()
                .Find(x => x.Pokemon.Id == pokemonId);
        }

        public BaseStat GetPokemonBaseStatsNoIncludes(int pokemonId)
        {
            return this._dataContext.BaseStats
                .ToList()
                .Find(x => x.PokemonId == pokemonId);
        }

        public EVYield GetPokemonEVYields(int pokemonId)
        {
            return this._dataContext.EVYields
                .Include(x => x.Pokemon)
                .ToList()
                .Find(x => x.Pokemon.Id == pokemonId);
        }

        public List<Pokemon> GetAllPokemonWithClassificationsAndIncomplete()
        {
            return this._dataContext.Pokemon
                .Include(x => x.Classification)
                .ToList();
        }

        public BaseStat GetBaseStat(int pokemonId)
        {
            return this._dataContext.BaseStats
                .Include(x => x.Pokemon)
                .ToList()
                .Find(x => x.Pokemon.Id == pokemonId);
        }

        public List<BaseStat> GetBaseStatsWithIncomplete()
        {
            return this._dataContext.BaseStats
                .Include(x => x.Pokemon)
                .ToList();
        }

        public EVYield GetEVYield(int pokemonId)
        {
            return this._dataContext.EVYields
                .Include(x => x.Pokemon)
                .ToList()
                .Find(x => x.Pokemon.Id == pokemonId);
        }

        public EVYield GetEVYieldNoIncludes(int pokemonId)
        {
            return this._dataContext.EVYields
                .ToList()
                .Find(x => x.PokemonId == pokemonId);
        }

        public List<EVYield> GetEVYieldsWithIncomplete()
        {
            return this._dataContext.EVYields
                .Include(x => x.Pokemon)
                .ToList();
        }

        public List<TypeChart> GetTypeCharts()
        {
            return this._dataContext.TypeCharts
                .Include(x => x.Attack)
                .Include(x => x.Defend)
                .OrderBy(x => x.AttackId)
                .ThenBy(x => x.DefendId)
                .ToList();
        }

        public List<TypeChart> GetTypeChartByAttackType(int id)
        {
            return this._dataContext.TypeCharts
                .Include(x => x.Attack)
                .Include(x => x.Defend)
                .Where(x => x.AttackId == id)
                .OrderBy(x => x.AttackId)
                .ThenBy(x => x.DefendId)
                .ToList();
        }

        public List<TypeChart> GetTypeChartByDefendType(int id)
        {
            return this._dataContext.TypeCharts
                .Include(x => x.Attack)
                .Include(x => x.Defend)
                .Where(x => x.DefendId == id)
                .OrderBy(x => x.AttackId)
                .ThenBy(x => x.DefendId)
                .ToList();
        }

        public TypeEffectivenessViewModel GetTypeChartPokemon(int pokemonId)
        {
            List<Type> typeList = this.GetTypes();
            List<Type> pokemonTypes = this.GetPokemonTypes(pokemonId);
            List<string> strongAgainst = new List<string>();
            List<string> superStrongAgainst = new List<string>();
            List<string> weakAgainst = new List<string>();
            List<string> superWeakAgainst = new List<string>();
            List<string> immuneTo = new List<string>();
            List<TypeChart> typeChart;
            string effectiveValue, attackType;

            foreach (var type in pokemonTypes)
            {
                typeChart = this._dataContext.TypeCharts
                    .Include(x => x.Attack)
                    .Include(x => x.Defend)
                    .Where(x => x.Defend == type)
                    .ToList();
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
                        else if (effectiveValue == "0.5" && immuneTo.Where(x => x == attackType).ToList().Count() == 0)
                        {
                            if (strongAgainst.Exists(x => x == attackType))
                            {
                                strongAgainst.Remove(attackType);
                                superStrongAgainst.Add(attackType + " Quad");
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
                        else if (effectiveValue == "2" && immuneTo.Where(x => x == attackType).ToList().Count() == 0)
                        {
                            if (weakAgainst.Exists(x => x == attackType))
                            {
                                weakAgainst.Remove(attackType);
                                superWeakAgainst.Add(attackType + " Quad");
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

        public TypeEffectivenessViewModel GetTypeChartTyping(int primaryTypeId, int secondaryTypeId)
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

            if(secondaryTypeId != 0)
            {
                pokemonTypes.Add(this.GetType(secondaryTypeId));
            }

            foreach (var type in pokemonTypes)
            {
                typeChart = this._dataContext.TypeCharts
                    .Include(x => x.Attack)
                    .Include(x => x.Defend)
                    .Where(x => x.Defend == type)
                    .ToList();
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
                        else if (effectiveValue == "0.5" && immuneTo.Where(x => x == attackType).ToList().Count() == 0)
                        {
                            if (strongAgainst.Exists(x => x == attackType))
                            {
                                strongAgainst.Remove(attackType);
                                superStrongAgainst.Add(attackType + " Quad");
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
                        else if (effectiveValue == "2" && immuneTo.Where(x => x == attackType).ToList().Count() == 0)
                        {
                            if (weakAgainst.Exists(x => x == attackType))
                            {
                                weakAgainst.Remove(attackType);
                                superWeakAgainst.Add(attackType + " Quad");
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
            return this._dataContext.BaseHappiness.ToList().Find(x => x.Id == id);
        }

        public List<BaseHappiness> GetBaseHappinesses()
        {
            return this._dataContext.BaseHappiness.OrderBy(x => x.Happiness).ToList();
        }

        public ShinyHuntingTechnique GetShinyHuntingTechnique(int id)
        {
            return this._dataContext.ShinyHuntingTechniques.ToList().Find(x => x.Id == id);
        }

        public List<ShinyHuntingTechnique> GetShinyHuntingTechniques()
        {
            return this._dataContext.ShinyHuntingTechniques.ToList();
        }

        public Classification GetClassification(int id)
        {
            return this._dataContext.Classifications.ToList().Find(x => x.Id == id);
        }

        public List<Classification> GetClassifications()
        {
            return this._dataContext.Classifications.OrderBy(x => x.Name).ToList();
        }

        public Nature GetNature(int id)
        {
            return this._dataContext.Natures.ToList().Find(x => x.Id == id);
        }

        public Nature GetNatureByName(string name)
        {
            return this._dataContext.Natures.ToList().Find(x => x.Name == name);
        }

        public List<Nature> GetNatures()
        {
            return this._dataContext.Natures.OrderBy(x => x.Name).ToList();
        }

        public ShinyHunt GetShinyHunt(int id)
        {
            return this._dataContext.ShinyHunts
                .Include(x => x.User)
                .Include(x => x.Pokemon)
                .Include(x => x.Game)
                .Include(x => x.ShinyHuntingTechnique)
                .ToList()
                .Find(x => x.Id == id);
        }

        public List<ShinyHunt> GetShinyHunter(string username)
        {
            return this._dataContext.ShinyHunts
                .Include(x => x.User)
                .Include(x => x.Pokemon)
                .Include(x => x.Game)
                .Include(x => x.ShinyHuntingTechnique)
                .Where(x => x.User.Username == username)
                .ToList();
        }

        public List<ShinyHunt> GetShinyHunterById(int id)
        {
            return this._dataContext.ShinyHunts
                .Include(x => x.User)
                .Include(x => x.Pokemon)
                .Include(x => x.Game)
                .Include(x => x.ShinyHuntingTechnique)
                .Where(x => x.User.Id == id)
                .ToList();
        }

        public List<ShinyHunt> GetShinyHunters()
        {
            return this._dataContext.ShinyHunts
                .Include(x => x.User)
                .Include(x => x.Pokemon)
                .Include(x => x.Game)
                .Include(x => x.ShinyHuntingTechnique)
                .OrderBy(x => x.User.Username)
                .ToList();
        }

        public List<BattleItem> GetBattleItems()
        {
            return this._dataContext.BattleItems
                .Include(x => x.Generation)
                .Include(x => x.Pokemon)
                .OrderBy(x => x.Generation.Id)
                .ThenBy(x => x.Name)
                .ToList();
        }

        public CaptureRate GetCaptureRate(int id)
        {
            return this._dataContext.CaptureRates.ToList().Find(x => x.Id == id);
        }

        public List<CaptureRate> GetCaptureRates()
        {
            return this._dataContext.CaptureRates.OrderBy(x => x.CatchRate).ToList();
        }

        public EggCycle GetEggCycle(int id)
        {
            return this._dataContext.EggCycles.ToList().Find(x => x.Id == id);
        }

        public List<EggCycle> GetEggCycles()
        {
            return this._dataContext.EggCycles.OrderBy(x => x.CycleCount).ToList();
        }

        public ExperienceGrowth GetExperienceGrowth(int id)
        {
            return this._dataContext.ExperienceGrowths.ToList().Find(x => x.Id == id);
        }

        public List<ExperienceGrowth> GetExperienceGrowths()
        {
            return this._dataContext.ExperienceGrowths.OrderBy(x => x.Name).ToList();
        }

        public GenderRatio GetGenderRatio(int id)
        {
            return this._dataContext.GenderRatios.ToList().Find(x => x.Id == id);
        }

        public List<GenderRatio> GetGenderRatios()
        {
            return this._dataContext.GenderRatios.ToList();
        }

        public List<ReviewedPokemon> GetAllReviewedPokemon()
        {
            return this._dataContext.ReviewedPokemons
                .Include(x => x.Pokemon)
                .OrderBy(x => x.Pokemon.PokedexNumber)
                .ToList();
        }

        public ReviewedPokemon GetReviewedPokemon(int id)
        {
            return this._dataContext.ReviewedPokemons.ToList().Find(x => x.Id == id);
        }

        public ReviewedPokemon GetReviewedPokemonByPokemonId(int id)
        {
            return this._dataContext.ReviewedPokemons.ToList().Find(x => x.PokemonId == id);
        }

        public List<EvolutionMethod> GetEvolutionMethods()
        {
            return this._dataContext.EvolutionMethods.OrderBy(x => x.Name).ToList();
        }

        public EvolutionMethod GetEvolutionMethod(int id)
        {
            return this._dataContext.EvolutionMethods.ToList().Find(x => x.Id == id);
        }

        public List<Game> GetGamesFromGeneration(Generation generation)
        {
            return this._dataContext.Games.Where(x => x.GenerationId == generation.Id).ToList();
        }

        public List<Generation> GetGenerations()
        {
            return this._dataContext.Generations.ToList();
        }

        public Generation GetLatestGeneration(int pokemonTeamId)
        {
            PokemonTeam pokemonTeam = this.GetPokemonTeam(pokemonTeamId);
            Generation latestGeneration = null;
            if(pokemonTeam.FirstPokemonId != null)
            {
                latestGeneration = pokemonTeam.FirstPokemon.Pokemon.Game.Generation;
            }

            if(pokemonTeam.SecondPokemonId != null)
            {
                if(pokemonTeam.SecondPokemon.Pokemon.Game.GenerationId > latestGeneration.Id)
                {
                    latestGeneration = pokemonTeam.SecondPokemon.Pokemon.Game.Generation;
                }
            }

            if(pokemonTeam.ThirdPokemonId != null)
            {
                if(pokemonTeam.ThirdPokemon.Pokemon.Game.GenerationId > latestGeneration.Id)
                {
                    latestGeneration = pokemonTeam.ThirdPokemon.Pokemon.Game.Generation;
                }
            }

            if(pokemonTeam.FourthPokemonId != null)
            {
                if(pokemonTeam.FourthPokemon.Pokemon.Game.GenerationId > latestGeneration.Id)
                {
                    latestGeneration = pokemonTeam.FourthPokemon.Pokemon.Game.Generation;
                }
            }

            if(pokemonTeam.FifthPokemonId != null)
            {
                if(pokemonTeam.FifthPokemon.Pokemon.Game.GenerationId > latestGeneration.Id)
                {
                    latestGeneration = pokemonTeam.FifthPokemon.Pokemon.Game.Generation;
                }
            }

            if(pokemonTeam.SixthPokemonId != null)
            {
                if(pokemonTeam.SixthPokemon.Pokemon.Game.GenerationId > latestGeneration.Id)
                {
                    latestGeneration = pokemonTeam.SixthPokemon.Pokemon.Game.Generation;
                }
            }
            
            return latestGeneration;
        }

        public Generation GetGeneration(int id)
        {
            return this._dataContext.Generations.ToList().Find(x => x.Id == id);
        }

        public Generation GetGenerationFromGame(int id)
        {
            return this._dataContext.Games
                .Include(x => x.Generation)
                .ToList()
                .Find(x => x.Id == id)
                .Generation;
        }

        public BattleItem GetBattleItem(int id)
        {
            return this._dataContext.BattleItems.ToList().Find(x => x.Id == id);
        }

        public BattleItem GetBattleItemByName(string name)
        {
            return this._dataContext.BattleItems.ToList().Find(x => x.Name == name);
        }

        public Generation GetGenerationByPokemon(int id)
        {
            return this.GetGeneration(this.GetPokemonById(id).Game.GenerationId);
        }

        public User GetUserWithUsername(string username)
        {
            return this._dataContext.Users.ToList().Find(x => x.Username == username);
        }

        public List<Message> GetMessagesToUsername(string username)
        {
            return this._dataContext.Messages.Where(x => x.ReceiverId == this.GetUserWithUsername(username).Id).ToList();
        }

        public Message GetMessage(int id)
        {
            return this._dataContext.Messages.ToList().Find(x => x.Id == id);
        }

        public User GetUserById(int id)
        {
            return this._dataContext.Users.ToList().Find(x => x.Id == id);
        }

        public List<User> GetUsers()
        {
            return this._dataContext.Users.ToList();
        }

        public List<Comment> GetComments()
        {
            return this._dataContext.Comments
                .Include(x => x.Commentor)
                .ToList();
        }

        public Comment GetComment(int id)
        {
            return this._dataContext.Comments.ToList().Find(x => x.Id == id);
        }

        public void AddComment(Comment comment)
        {
            this._dataContext.Comments.Add(comment);
            this._dataContext.SaveChanges();
        }

        public void AddMessage(Message message)
        {
            this._dataContext.Messages.Add(message);
            this._dataContext.SaveChanges();
        }

        public void AddUser(User user)
        {
            this._dataContext.Users.Add(user);
            this._dataContext.SaveChanges();
        }

        public void AddPokemonGameDetail(PokemonGameDetail pokemonGameDetail)
        {
            this._dataContext.PokemonGameDetails.Add(pokemonGameDetail);
            this._dataContext.SaveChanges();
        }

        public void AddEvolutionMethod(EvolutionMethod evolutionMethod)
        {
            this._dataContext.EvolutionMethods.Add(evolutionMethod);
            this._dataContext.SaveChanges();
        }

        public void AddCaptureRate(CaptureRate captureRate)
        {
            this._dataContext.CaptureRates.Add(captureRate);
            this._dataContext.SaveChanges();
        }

        public void AddBaseHappiness(BaseHappiness baseHappiness)
        {
            this._dataContext.BaseHappiness.Add(baseHappiness);
            this._dataContext.SaveChanges();
        }

        public void AddGeneration(Generation generation)
        {
            this._dataContext.Generations.Add(generation);
            this._dataContext.SaveChanges();
        }

        public void AddGame(Game game)
        {
            this._dataContext.Games.Add(game);
            this._dataContext.SaveChanges();
        }

        public void AddFormItem(FormItem formItem)
        {
            this._dataContext.FormItems.Add(formItem);
            this._dataContext.SaveChanges();
        }

        public void AddEvolution(Evolution evolution)
        {
            this._dataContext.Evolutions.Add(evolution);
            this._dataContext.SaveChanges();
        }

        public void AddType(Type type)
        {
            this._dataContext.Types.Add(type);
            this._dataContext.SaveChanges();
        }

        public void AddTypeChart(TypeChart typeChart)
        {
            this._dataContext.TypeCharts.Add(typeChart);
            this._dataContext.SaveChanges();
        }

        public void AddLegendaryType(LegendaryType legendaryType)
        {
            this._dataContext.LegendaryTypes.Add(legendaryType);
            this._dataContext.SaveChanges();
        }

        public void AddShinyHuntingTechnique(ShinyHuntingTechnique shinyHuntingTechnique)
        {
            this._dataContext.ShinyHuntingTechniques.Add(shinyHuntingTechnique);
            this._dataContext.SaveChanges();
        }

        public void AddShinyHunt(ShinyHunt shinyHunt)
        {
            this._dataContext.ShinyHunts.Add(shinyHunt);
            this._dataContext.SaveChanges();
        }

        public void AddReviewedPokemon(ReviewedPokemon reviewedPokemon)
        {
            this._dataContext.ReviewedPokemons.Add(reviewedPokemon);
            this._dataContext.SaveChanges();
        }

        public void AddEggGroup(EggGroup eggGroup)
        {
            this._dataContext.EggGroups.Add(eggGroup);
            this._dataContext.SaveChanges();
        }

        public void AddMove(Move move)
        {
            this._dataContext.Moves.Add(move);
            this._dataContext.SaveChanges();
        }

        public void AddAbility(Ability ability)
        {
            this._dataContext.Abilities.Add(ability);
            this._dataContext.SaveChanges();
        }

        public void AddBattleItem(BattleItem battleItem)
        {
            this._dataContext.BattleItems.Add(battleItem);
            this._dataContext.SaveChanges();
        }

        public void AddPokemon(Pokemon pokemon)
        {
            this._dataContext.Pokemon.Add(pokemon);
            this._dataContext.SaveChanges();
        }

        public void AddPokemonTeam(PokemonTeam pokemonTeam)
        {
            this._dataContext.PokemonTeams.Add(pokemonTeam);
            this._dataContext.SaveChanges();
        }

        public int AddPokemonTeamDetail(PokemonTeamDetail pokemonTeamDetail)
        {
            if(pokemonTeamDetail.PokemonTeamEVId == null)
            {
                int pokemonTeamEVId = this.AddPokemonTeamEV(new PokemonTeamEV());
                pokemonTeamDetail.PokemonTeamEVId = pokemonTeamEVId;
            }
            
            if(pokemonTeamDetail.PokemonTeamIVId == null)
            {
                int pokemonTeamIVId = this.AddPokemonTeamIV(new PokemonTeamIV());
                pokemonTeamDetail.PokemonTeamIVId = pokemonTeamIVId;
            }
            
            if(pokemonTeamDetail.PokemonTeamMovesetId == null)
            {
                int pokemonTeamMovesetId = this.AddPokemonTeamMoveset(new PokemonTeamMoveset());
                pokemonTeamDetail.PokemonTeamMovesetId = pokemonTeamMovesetId;
            }
            
            this._dataContext.PokemonTeamDetails.Add(pokemonTeamDetail);
            this._dataContext.SaveChanges();
            return pokemonTeamDetail.Id;
        }

        public int AddPokemonTeamEV(PokemonTeamEV pokemonTeamEV)
        {
            this._dataContext.PokemonTeamEVs.Add(pokemonTeamEV);
            this._dataContext.SaveChanges();
            return pokemonTeamEV.Id;
        }

        public int AddPokemonTeamIV(PokemonTeamIV pokemonTeamIV)
        {
            this._dataContext.PokemonTeamIVs.Add(pokemonTeamIV);
            this._dataContext.SaveChanges();
            return pokemonTeamIV.Id;
        }

        public int AddPokemonTeamMoveset(PokemonTeamMoveset pokemonTeamMoveset)
        {
            this._dataContext.PokemonTeamMovesets.Add(pokemonTeamMoveset);
            this._dataContext.SaveChanges();
            return pokemonTeamMoveset.Id;
        }

        public void AddPokemonFormDetails(PokemonFormDetail pokemonFormDetail)
        {
            this._dataContext.PokemonFormDetails.Add(pokemonFormDetail);
            this._dataContext.SaveChanges();
        }

        public void AddPokemonTyping(PokemonTypeDetail typing)
        {
            this._dataContext.PokemonTypeDetails.Add(typing);
            this._dataContext.SaveChanges();
        }

        public void AddEggCycle(EggCycle eggCycle)
        {
            this._dataContext.EggCycles.Add(eggCycle);
            this._dataContext.SaveChanges();
        }

        public void AddExperienceGrowth(ExperienceGrowth experienceGrowth)
        {
            this._dataContext.ExperienceGrowths.Add(experienceGrowth);
            this._dataContext.SaveChanges();
        }

        public void AddGenderRatio(GenderRatio genderRatio)
        {
            this._dataContext.GenderRatios.Add(genderRatio);
            this._dataContext.SaveChanges();
        }

        public void AddPokemonLegendaryDetails(PokemonLegendaryDetail pokemonLegendaryDetail)
        {
            this._dataContext.PokemonLegendaryDetails.Add(pokemonLegendaryDetail);
            this._dataContext.SaveChanges();
        }

        public void AddPokemonAbilities(PokemonAbilityDetail abilities)
        {
            this._dataContext.PokemonAbilityDetails.Add(abilities);
            this._dataContext.SaveChanges();
        }

        public void AddPokemonEggGroups(PokemonEggGroupDetail eggGroups)
        {
            this._dataContext.PokemonEggGroupDetails.Add(eggGroups);
            this._dataContext.SaveChanges();
        }

        public void AddPokemonBaseStat(BaseStat baseStat)
        {
            this._dataContext.BaseStats.Add(baseStat);
            this._dataContext.SaveChanges();
        }

        public void AddPokemonEVYield(EVYield evYield)
        {
            this._dataContext.EVYields.Add(evYield);
            this._dataContext.SaveChanges();
        }

        public void AddClassification(Classification classification)
        {
            this._dataContext.Classifications.Add(classification);
            this._dataContext.SaveChanges();
        }

        public void AddNature(Nature nature)
        {
            this._dataContext.Natures.Add(nature);
            this._dataContext.SaveChanges();
        }

        public void AddForm(Form form)
        {
            this._dataContext.Forms.Add(form);
            this._dataContext.SaveChanges();
        }

        public void UpdateUser(User user)
        {
            this._dataContext.Users.Update(user);
            this._dataContext.SaveChanges();
        }

        public void UpdateComment(Comment comment)
        {
            this._dataContext.Comments.Update(comment);
            this._dataContext.SaveChanges();
        }

        public void UpdateMessage(Message message)
        {
            this._dataContext.Messages.Update(message);
            this._dataContext.SaveChanges();
        }

        public void UpdatePokemonTypeDetail(PokemonTypeDetail pokemonTypeDetail)
        {
            this._dataContext.PokemonTypeDetails.Update(pokemonTypeDetail);
            this._dataContext.SaveChanges();
        }

        public void UpdateEvolutionMethod(EvolutionMethod evolutionMethod)
        {
            this._dataContext.EvolutionMethods.Update(evolutionMethod);
            this._dataContext.SaveChanges();
        }

        public void UpdatePokemonAbilityDetail(PokemonAbilityDetail pokemonAbilityDetail)
        {
            this._dataContext.PokemonAbilityDetails.Update(pokemonAbilityDetail);
            this._dataContext.SaveChanges();
        }

        public void UpdatePokemonTeam(PokemonTeam pokemonTeam)
        {
            this._dataContext.PokemonTeams.Update(pokemonTeam);
            this._dataContext.SaveChanges();
        }

        public void UpdatePokemonTeamDetail(PokemonTeamDetail pokemonTeamDetail)
        {
            this._dataContext.PokemonTeamDetails.Update(pokemonTeamDetail);
            this._dataContext.SaveChanges();
        }

        public void UpdatePokemonEggGroupDetail(PokemonEggGroupDetail pokemonEggGroupDetail)
        {
            this._dataContext.PokemonEggGroupDetails.Update(pokemonEggGroupDetail);
            this._dataContext.SaveChanges();
        }

        public void UpdateBaseStat(BaseStat baseStats)
        {
            this._dataContext.BaseStats.Update(baseStats);
            this._dataContext.SaveChanges();
        }

        public void UpdateEggCycle(EggCycle eggCycle)
        {
            this._dataContext.EggCycles.Update(eggCycle);
            this._dataContext.SaveChanges();
        }

        public void UpdateExperienceGrowth(ExperienceGrowth experienceGrowth)
        {
            this._dataContext.ExperienceGrowths.Update(experienceGrowth);
            this._dataContext.SaveChanges();
        }

        public void UpdateGenderRatio(GenderRatio genderRatio)
        {
            this._dataContext.GenderRatios.Update(genderRatio);
            this._dataContext.SaveChanges();
        }

        public void UpdateBaseHappiness(BaseHappiness baseHappiness)
        {
            this._dataContext.BaseHappiness.Update(baseHappiness);
            this._dataContext.SaveChanges();
        }

        public void UpdatePokemonTeamEV(PokemonTeamEV pokemonTeamEVs)
        {
            this._dataContext.PokemonTeamEVs.Update(pokemonTeamEVs);
            this._dataContext.SaveChanges();
        }

        public void UpdatePokemonTeamIV(PokemonTeamIV pokemonTeamIVs)
        {
            this._dataContext.PokemonTeamIVs.Update(pokemonTeamIVs);
            this._dataContext.SaveChanges();
        }

        public void UpdatePokemonTeamMoveset(PokemonTeamMoveset pokemonTeamMoveset)
        {
            pokemonTeamMoveset = this.SortMoveset(pokemonTeamMoveset);

            this._dataContext.PokemonTeamMovesets.Update(pokemonTeamMoveset);
            this._dataContext.SaveChanges();
        }

        public PokemonTeamMoveset SortMoveset(PokemonTeamMoveset moveset)
        {
            #region FirstSort
            if(string.IsNullOrEmpty(moveset.FirstMove) && !string.IsNullOrEmpty(moveset.SecondMove))
            {
                moveset.FirstMove = moveset.SecondMove;
                moveset.SecondMove = null;
            }
            
            if(string.IsNullOrEmpty(moveset.SecondMove) && !string.IsNullOrEmpty(moveset.ThirdMove))
            {
                moveset.SecondMove = moveset.ThirdMove;
                moveset.ThirdMove = null;
            }
            
            if(string.IsNullOrEmpty(moveset.ThirdMove) && !string.IsNullOrEmpty(moveset.FourthMove))
            {
                moveset.ThirdMove = moveset.FourthMove;
                moveset.FourthMove = null;
            }
            #endregion
            
            #region SecondSort
            if(string.IsNullOrEmpty(moveset.FirstMove) && !string.IsNullOrEmpty(moveset.SecondMove))
            {
                moveset.FirstMove = moveset.SecondMove;
                moveset.SecondMove = null;
            }
            
            if(string.IsNullOrEmpty(moveset.SecondMove) && !string.IsNullOrEmpty(moveset.ThirdMove))
            {
                moveset.SecondMove = moveset.ThirdMove;
                moveset.ThirdMove = null;
            }
            #endregion

            #region ThirdSort
            if(string.IsNullOrEmpty(moveset.FirstMove) && !string.IsNullOrEmpty(moveset.SecondMove))
            {
                moveset.FirstMove = moveset.SecondMove;
                moveset.SecondMove = null;
            }
            #endregion

            return moveset;
        }

        public void UpdateEVYield(EVYield evYields)
        {
            this._dataContext.EVYields.Update(evYields);
            this._dataContext.SaveChanges();
        }

        public void UpdateEvolution(Evolution evolution)
        {
            this._dataContext.Evolutions.Update(evolution);
            this._dataContext.SaveChanges();
        }

        public void UpdateFormItem(FormItem formItem)
        {
            this._dataContext.FormItems.Update(formItem);
            this._dataContext.SaveChanges();
        }

        public void UpdateCaptureRate(CaptureRate captureRate)
        {
            this._dataContext.CaptureRates.Update(captureRate);
            this._dataContext.SaveChanges();
        }

        public void UpdatePokemon(Pokemon pokemon)
        {
            this._dataContext.Pokemon.Update(pokemon);
            this._dataContext.SaveChanges();
        }

        public void UpdatePokemonFormDetail(PokemonFormDetail pokemonFormDetail)
        {
            this._dataContext.PokemonFormDetails.Update(pokemonFormDetail);
            this._dataContext.SaveChanges();
        }

        public void UpdateShinyHunt(ShinyHunt shinyHunt)
        {
            this._dataContext.ShinyHunts.Update(shinyHunt);
            this._dataContext.SaveChanges();
        }

        public void UpdateGeneration(Generation generation)
        {
            this._dataContext.Generations.Update(generation);
            this._dataContext.SaveChanges();
        }

        public void UpdateGame(Game game)
        {
            this._dataContext.Games.Update(game);
            this._dataContext.SaveChanges();
        }

        public void UpdateType(Type type)
        {
            this._dataContext.Types.Update(type);
            this._dataContext.SaveChanges();
        }

        public void UpdateEggGroup(EggGroup eggGroup)
        {
            this._dataContext.EggGroups.Update(eggGroup);
            this._dataContext.SaveChanges();
        }

        public void UpdateForm(Form form)
        {
            this._dataContext.Forms.Update(form);
            this._dataContext.SaveChanges();
        }

        public void UpdateBattleItem(BattleItem battleItem)
        {
            this._dataContext.BattleItems.Update(battleItem);
            this._dataContext.SaveChanges();
        }

        public void UpdateAbility(Ability ability)
        {
            this._dataContext.Abilities.Update(ability);
            this._dataContext.SaveChanges();
        }

        public void UpdateShinyHuntingTechnique(ShinyHuntingTechnique shinyHuntingTechnique)
        {
            this._dataContext.ShinyHuntingTechniques.Update(shinyHuntingTechnique);
            this._dataContext.SaveChanges();
        }

        public void UpdateClassification(Classification nature)
        {
            this._dataContext.Classifications.Update(nature);
            this._dataContext.SaveChanges();
        }

        public void UpdateNature(Nature classification)
        {
            this._dataContext.Natures.Update(classification);
            this._dataContext.SaveChanges();
        }

        public void UpdateMove(Move move)
        {
            this._dataContext.Moves.Update(move);
            this._dataContext.SaveChanges();
        }

        public void UpdateLegendaryType(LegendaryType legendaryType)
        {
            this._dataContext.LegendaryTypes.Update(legendaryType);
            this._dataContext.SaveChanges();
        }

        public void DeletePokemon(int id)
        {
            Pokemon pokemon = this.GetPokemonById(id);
            this._dataContext.Pokemon.Remove(pokemon);
            this._dataContext.SaveChanges();
        }

        public void DeleteGeneration(int id)
        {
            Generation generation = this.GetGeneration(id);
            this._dataContext.Generations.Remove(generation);
            this._dataContext.SaveChanges();
        }

        public void DeleteGame(int id)
        {
            Game game = this.GetGame(id);
            this._dataContext.Games.Remove(game);
            this._dataContext.SaveChanges();
        }

        public void DeletePokemonGameDetail(int id)
        {
            PokemonGameDetail pokemonGameDetail = this.GetPokemonGameDetail(id);
            this._dataContext.PokemonGameDetails.Remove(pokemonGameDetail);
            this._dataContext.SaveChanges();
        }

        public void DeleteType(int id)
        {
            Type type = this.GetType(id);
            this._dataContext.Types.Remove(type);
            this._dataContext.SaveChanges();
        }

        public void DeleteTypeChart(int id)
        {
            TypeChart typeChart = this.GetTypeChart(id);
            this._dataContext.TypeCharts.Remove(typeChart);
            this._dataContext.SaveChanges();
        }

        public void DeleteBattleItem(int id)
        {
            BattleItem battleItem = this.GetBattleItem(id);
            this._dataContext.BattleItems.Remove(battleItem);
            this._dataContext.SaveChanges();
        }

        public void DeleteAbility(int id)
        {
            Ability ability = this.GetAbility(id);
            this._dataContext.Abilities.Remove(ability);
            this._dataContext.SaveChanges();
        }

        public void DeleteLegendaryType(int id)
        {
            LegendaryType legendaryType = this.GetLegendaryType(id);
            this._dataContext.LegendaryTypes.Remove(legendaryType);
            this._dataContext.SaveChanges();
        }

        public void DeleteEggCycle(int id)
        {
            EggCycle eggCycle = this.GetEggCycle(id);
            this._dataContext.EggCycles.Remove(eggCycle);
            this._dataContext.SaveChanges();
        }

        public void DeleteExperienceGrowth(int id)
        {
            ExperienceGrowth experienceGrowth = this.GetExperienceGrowth(id);
            this._dataContext.ExperienceGrowths.Remove(experienceGrowth);
            this._dataContext.SaveChanges();
        }

        public void DeleteGenderRatio(int id)
        {
            GenderRatio genderRatio = this.GetGenderRatio(id);
            this._dataContext.GenderRatios.Remove(genderRatio);
            this._dataContext.SaveChanges();
        }

        public void DeleteCaptureRate(int id)
        {
            CaptureRate captureRate = this.GetCaptureRate(id);
            this._dataContext.CaptureRates.Remove(captureRate);
            this._dataContext.SaveChanges();
        }

        public void DeleteComment(int id)
        {
            Comment comment = this.GetComment(id);
            this._dataContext.Comments.Remove(comment);
            this._dataContext.SaveChanges();
        }

        public void DeleteEggGroup(int id)
        {
            EggGroup eggGroup = this.GetEggGroup(id);
            this._dataContext.EggGroups.Remove(eggGroup);
            this._dataContext.SaveChanges();
        }

        public void DeleteFormItem(int id)
        {
            FormItem formItem = this.GetFormItem(id);
            this._dataContext.FormItems.Remove(formItem);
            this._dataContext.SaveChanges();
        }

        public void DeleteEvolutionMethod(int id)
        {
            EvolutionMethod evolutionMethod = this.GetEvolutionMethod(id);
            this._dataContext.EvolutionMethods.Remove(evolutionMethod);
            this._dataContext.SaveChanges();
        }

        public void DeleteForm(int id)
        {
            Form form = this.GetForm(id);
            this._dataContext.Forms.Remove(form);
            this._dataContext.SaveChanges();
        }

        public void DeleteMove(int id)
        {
            Move move = this.GetMove(id);
            this._dataContext.Moves.Remove(move);
            this._dataContext.SaveChanges();
        }

        public void DeleteBaseHappiness(int id)
        {
            BaseHappiness baseHappiness = this.GetBaseHappiness(id);
            this._dataContext.BaseHappiness.Remove(baseHappiness);
            this._dataContext.SaveChanges();
        }

        public void DeleteReviewedPokemon(int id)
        {
            ReviewedPokemon reviewedPokemon = this.GetReviewedPokemon(id);
            this._dataContext.ReviewedPokemons.Remove(reviewedPokemon);
            this._dataContext.SaveChanges();
        }

        public void DeleteMessage(int id)
        {
            Message message = this.GetMessage(id);
            this._dataContext.Messages.Remove(message);
            this._dataContext.SaveChanges();
        }

        public void DeletePokemonTeam(int id)
        {
            PokemonTeam pokemonTeam = this.GetPokemonTeamNoIncludes(id);
            List<int> pokemonTeamDetailIds = pokemonTeam.GrabPokemonTeamDetailIds();
            this._dataContext.PokemonTeams.Remove(pokemonTeam);
            this._dataContext.SaveChanges();

            foreach(var p in pokemonTeamDetailIds)
            {
                this.DeletePokemonTeamDetail(p);
            }
        }

        public void RemovePokemonFromTeam(PokemonTeam team, PokemonTeamDetail teamDetail)
        {
            if(team.FirstPokemonId == teamDetail.Id)
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
            if(pokemonTeam.FirstPokemonId == null && pokemonTeam.SecondPokemonId != null)
            {
                pokemonTeam.FirstPokemonId = pokemonTeam.SecondPokemonId;
                pokemonTeam.SecondPokemonId = null;
            }

            if(pokemonTeam.SecondPokemonId == null && pokemonTeam.ThirdPokemonId != null)
            {
                pokemonTeam.SecondPokemonId = pokemonTeam.ThirdPokemonId;
                pokemonTeam.ThirdPokemonId = null;
            }

            if(pokemonTeam.ThirdPokemonId == null && pokemonTeam.FourthPokemonId != null)
            {
                pokemonTeam.ThirdPokemonId = pokemonTeam.FourthPokemonId;
                pokemonTeam.FourthPokemonId = null;
            }

            if(pokemonTeam.FourthPokemonId == null && pokemonTeam.FifthPokemonId != null)
            {
                pokemonTeam.FourthPokemonId = pokemonTeam.FifthPokemonId;
                pokemonTeam.FifthPokemonId = null;
            }

            if(pokemonTeam.FifthPokemonId == null && pokemonTeam.SixthPokemonId != null)
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
            if(pokemonTeam != null)
            {
                this.RemovePokemonFromTeam(pokemonTeam, pokemonTeamDetail);
            }

            int? evId = pokemonTeamDetail.PokemonTeamEVId;
            int? ivId = pokemonTeamDetail.PokemonTeamIVId;
            int? movesetId = pokemonTeamDetail.PokemonTeamMovesetId;
            this._dataContext.PokemonTeamDetails.Remove(pokemonTeamDetail);
            this._dataContext.SaveChanges();

            if(evId != null)
            {
                this.DeletePokemonTeamEV((int)evId);
            }

            if(ivId != null)
            {
                this.DeletePokemonTeamIV((int)ivId);
            }

            if(movesetId != null)
            {
                this.DeletePokemonTeamMoveset((int)movesetId);
            }
        }

        public void DeletePokemonTeamEV(int id)
        {
            PokemonTeamEV pokemonTeamDetailEV = this.GetPokemonTeamEV(id);
            this._dataContext.PokemonTeamEVs.Remove(pokemonTeamDetailEV);
            this._dataContext.SaveChanges();
        }

        public void DeletePokemonTeamIV(int id)
        {
            PokemonTeamIV pokemonTeamDetailIV = this.GetPokemonTeamIV(id);
            this._dataContext.PokemonTeamIVs.Remove(pokemonTeamDetailIV);
            this._dataContext.SaveChanges();
        }

        public void DeletePokemonTeamMoveset(int id)
        {
            PokemonTeamMoveset pokemonTeamDetailMoveset = this.GetPokemonTeamMoveset(id);
            this._dataContext.PokemonTeamMovesets.Remove(pokemonTeamDetailMoveset);
            this._dataContext.SaveChanges();
        }

        public void DeleteClassification(int id)
        {
            Classification classification = this.GetClassification(id);
            this._dataContext.Classifications.Remove(classification);
            this._dataContext.SaveChanges();
        }

        public void DeleteNature(int id)
        {
            Nature nature = this.GetNature(id);
            this._dataContext.Natures.Remove(nature);
            this._dataContext.SaveChanges();
        }

        public void DeleteShinyHuntingTechnique(int id)
        {
            ShinyHuntingTechnique shinyHuntingTechnique = this.GetShinyHuntingTechnique(id);
            this._dataContext.ShinyHuntingTechniques.Remove(shinyHuntingTechnique);
            this._dataContext.SaveChanges();
        }

        public void DeleteShinyHunt(int id)
        {
            ShinyHunt shinyHunt = this.GetShinyHunt(id);
            this._dataContext.ShinyHunts.Remove(shinyHunt);
            this._dataContext.SaveChanges();
        }

        public string FormatPokemonName(string pokemonName)
        {
            if (pokemonName.Contains("type_null"))
            {
                pokemonName = "Type: Null";
            }
            
            if (pokemonName.Contains('_'))
            {
                pokemonName = pokemonName.Replace('_', ' ');
            }

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            pokemonName = textInfo.ToTitleCase(pokemonName);

            if (pokemonName.Length > 1 && pokemonName.Substring(pokemonName.Length - 2, 2) == "-O")
            {
                pokemonName = pokemonName.Remove(pokemonName.Length - 2, 2) + "-o";
            }

            return pokemonName;
        }
    }
}