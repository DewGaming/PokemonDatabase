using System.Collections.Generic;
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

        public Ability GetAbility(int id)
        {
            return this._dataContext.Abilities
                .ToList()
                .Find(x => x.Id == id);
        }

        public Ability GetAbilityDescription(string name)
        {
            return this._dataContext.Abilities
                .ToList()
                .Find(x => x.Name == name);
        }

        public List<Ability> GetAbilities()
        {
            return this._dataContext.Abilities.OrderBy(x => x.Name).Where(x => x.IsArchived == false).ToList();
        }

        public List<Ability> GetAbilitiesWithArchive()
        {
            return this._dataContext.Abilities.OrderBy(x => x.Name).ToList();
        }

        public LegendaryType GetLegendaryType(int id)
        {
            return this._dataContext.LegendaryTypes.OrderBy(x => x.Type).ToList().Find(x => x.Id == id);
        }

        public List<LegendaryType> GetLegendaryTypes()
        {
            return this._dataContext.LegendaryTypes.OrderBy(x => x.Type).Where(x => x.IsArchived == false).ToList();
        }

        public List<LegendaryType> GetLegendaryTypesWithArchive()
        {
            return this._dataContext.LegendaryTypes.OrderBy(x => x.Type).ToList();
        }

        public PokemonLegendaryDetail GetLegendaryDetail(string pokemonId)
        {
            return this._dataContext.PokemonLegendaryDetails.ToList().Find(x => x.PokemonId == pokemonId);
        }

        public Type GetType(int id)
        {
            return this._dataContext.Types
                .ToList()
                .Find(x => x.Id == id);
        }

        public List<Type> GetTypesWithArchive()
        {
            return this._dataContext.Types.OrderBy(x => x.Name).ToList();
        }

        public List<Type> GetTypes()
        {
            return this._dataContext.Types.OrderBy(x => x.Name).Where(x => x.IsArchived == false).ToList();
        }

        public FormItem GetFormItem(int id)
        {
            return this._dataContext.FormItems
                .ToList()
                .Find(x => x.Id == id);
        }

        public FormItem GetFormItemByPokemonId(string id)
        {
            return this._dataContext.FormItems
                .ToList()
                .Find(x => x.PokemonId == id);
        }

        public List<FormItem> GetFormItems()
        {
            return this._dataContext.FormItems.OrderBy(x => x.Name).ToList();
        }

        public List<Update> GetUpdates()
        {
            return this._dataContext.Updates.ToList();
        }

        public List<Update> GetUpdates(int count)
        {
            return this._dataContext.Updates.OrderByDescending(x => x.Id).Take(count).ToList();
        }

        public List<Type> GetTypeChartTypes()
        {
            return this._dataContext.Types.Where(x => x.IsArchived == false).ToList();
        }

        public List<Type> GetPokemonTypes(string pokemonId)
        {
            PokemonTypeDetail typeDetail = this._dataContext.PokemonTypeDetails
                                                .Include(x => x.Pokemon)
                                                .Include(x => x.PrimaryType)
                                                .Include(x => x.SecondaryType)
                                                .Where(x => x.Pokemon.IsComplete == true)
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
            return this._dataContext.EggGroups.OrderBy(x => x.Name).Where(x => x.IsArchived == false).ToList();
        }

        public List<EggGroup> GetEggGroupsWithArchive()
        {
            return this._dataContext.EggGroups.OrderBy(x => x.Name).ToList();
        }

        public PokemonEggGroupDetail GetPokemonEggGroups(string pokemonId)
        {
            PokemonEggGroupDetail eggGroupDetail = this._dataContext.PokemonEggGroupDetails
                                                        .Include(x => x.Pokemon)
                                                        .Include(x => x.PrimaryEggGroup)
                                                        .Include(x => x.SecondaryEggGroup)
                                                        .Where(x => x.Pokemon.IsComplete == true)
                                                        .ToList()
                                                        .Find(x => x.Pokemon.Id == pokemonId);

            return eggGroupDetail;
        }

        public List<Evolution> GetEvolutions()
        {
            return this._dataContext.Evolutions
                .Include(x => x.PreevolutionPokemon)
                .Include(x => x.EvolutionPokemon)
                .Include(x => x.EvolutionMethod)
                .ToList();
        }

        public Evolution GetPreEvolution(string pokemonId)
        {
            Evolution preEvolution = this.GetEvolutions().Find(x => x.EvolutionPokemon.Id == pokemonId);

            if (preEvolution != null && preEvolution.PreevolutionPokemonId.Contains('-'))
            {
                preEvolution.PreevolutionPokemon.Name += " (" + this.GetPokemonFormName(preEvolution.PreevolutionPokemonId) + ")";
            }

            return preEvolution;
        }

        public Evolution GetPreEvolutionNoEdit(string pokemonId)
        {
            return this._dataContext.Evolutions
                .ToList()
                .Find(x => x.EvolutionPokemonId == pokemonId);
        }

        public List<Evolution> GetPokemonEvolutions(string pokemonId)
        {
            List<Evolution> evolutions = this.GetEvolutions()
                .Where(x => x.PreevolutionPokemon.Id == pokemonId && x.PreevolutionPokemon.IsComplete == true && x.EvolutionPokemon.IsComplete == true)
                .OrderBy(x => x.EvolutionPokemon.Id.Length)
                .ThenBy(x => x.EvolutionPokemon.Id)
                .ToList();

            foreach(var e in evolutions)
            {
                if (e.EvolutionPokemonId.Contains('-'))
                {
                    e.EvolutionPokemon.Name += " (" + this.GetPokemonFormName(e.EvolutionPokemonId) + ")";
                }
            }
            
            if (pokemonId == "677")
            {
                evolutions.Remove(evolutions[1]);
            }

            return evolutions;
        }

        public List<Evolution> GetPokemonEvolutionsNoEdit(string pokemonId)
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

        public List<PokemonFormDetail> GetPokemonForms(string pokemonId)
        {
            return this._dataContext.PokemonFormDetails
                .Include(x => x.Form)
                .Include(x => x.OriginalPokemon)
                .Include(x => x.AltFormPokemon)
                .Where(x => x.OriginalPokemon.Id == pokemonId && x.OriginalPokemon.IsComplete == true && x.AltFormPokemon.IsComplete == true)
                .OrderBy(x => x.AltFormPokemon.Id.Substring(x.AltFormPokemon.Id.IndexOf("-") + 1).Length)
                .ThenBy(x => x.AltFormPokemon.Id.Substring(x.AltFormPokemon.Id.IndexOf("-") + 1))
                .ToList();
        }

        public string GetPokemonFormName(string pokemonId)
        {
            PokemonFormDetail formDetail = this._dataContext.PokemonFormDetails
                .Include(x => x.Form)
                .Include(x => x.OriginalPokemon)
                .Include(x => x.AltFormPokemon)
                .ToList()
                .Find(x => x.AltFormPokemon.Id == pokemonId);
            return formDetail.Form.Name;
        }

        public Pokemon GetPokemon(string name)
        {
            return this._dataContext.Pokemon
                .Include(x => x.EggCycle)
                .Include(x => x.GenderRatio)
                .Include(x => x.Classification)
                .Include(x => x.Generation)
                .Include(x => x.ExperienceGrowth)
                .Include(x => x.CaptureRate)
                .Include(x => x.BaseHappiness)
                .Where(x => x.IsComplete)
                .ToList()
                .Find(x => x.Name == name);
        }

        public Pokemon GetPokemonById(string id)
        {
            return this._dataContext.Pokemon
                .Include(x => x.EggCycle)
                .Include(x => x.GenderRatio)
                .Include(x => x.Classification)
                .Include(x => x.Generation)
                .Include(x => x.ExperienceGrowth)
                .Include(x => x.CaptureRate)
                .Include(x => x.BaseHappiness)
                .ToList()
                .Find(x => x.Id == id);
        }

        public Pokemon GetPokemonByIdNoIncludes(string id)
        {
            return this._dataContext.Pokemon
                .ToList()
                .Find(x => x.Id == id);
        }

        public Pokemon GetPokemonNoIncludes(string name)
        {
            return this._dataContext.Pokemon
                .ToList()
                .Find(x => x.Name == name);
        }

        public Pokemon GetPokemonNoIncludesById(string id)
        {
            return this._dataContext.Pokemon
                .ToList()
                .Find(x => x.Id == id);
        }

        public List<Pokemon> GetAllPokemon()
        {
            return this._dataContext.Pokemon
                .Include(x => x.EggCycle)
                .Include(x => x.GenderRatio)
                .Include(x => x.Classification)
                .Include(x => x.Generation)
                .Include(x => x.ExperienceGrowth)
                .Include(x => x.CaptureRate)
                .Include(x => x.BaseHappiness)
                .OrderBy(x => x.Id.Length)
                .ThenBy(x => x.Id)
                .Where(x => x.IsArchived == false && x.IsComplete == true)
                .ToList();
        }

        public List<Pokemon> GetAllPokemonIncludeIncomplete()
        {
            return this._dataContext.Pokemon
                .Include(x => x.EggCycle)
                .Include(x => x.GenderRatio)
                .Include(x => x.Classification)
                .Include(x => x.Generation)
                .Include(x => x.ExperienceGrowth)
                .Include(x => x.CaptureRate)
                .Include(x => x.BaseHappiness)
                .OrderBy(x => x.Id.Length)
                .ThenBy(x => x.Id)
                .Where(x => x.IsArchived == false)
                .ToList();
        }

        public List<Pokemon> GetAllPokemonWithoutForms()
        {
            List<Pokemon> pokemonList = this._dataContext.Pokemon
                .Include(x => x.EggCycle)
                .Include(x => x.GenderRatio)
                .Include(x => x.Classification)
                .Include(x => x.Generation)
                .Include(x => x.ExperienceGrowth)
                .Include(x => x.CaptureRate)
                .Include(x => x.BaseHappiness)
                .OrderBy(x => x.Id.Length)
                .ThenBy(x => x.Id)
                .Where(x => x.IsArchived == false && x.IsComplete == true)
                .ToList();
            List<Pokemon> altFormList = pokemonList.Where(x => x.Id.Contains("-")).ToList();
            pokemonList = pokemonList.Except(altFormList).ToList();

            pokemonList = pokemonList.OrderBy(x => x.Id.Length).ThenBy(x => x.Id).ToList();

            return pokemonList;
        }

        public List<Pokemon> GetAllPokemonWithoutFormsWithIncomplete()
        {
            List<Pokemon> pokemonList = this._dataContext.Pokemon
                .Include(x => x.EggCycle)
                .Include(x => x.GenderRatio)
                .Include(x => x.Classification)
                .Include(x => x.Generation)
                .Include(x => x.ExperienceGrowth)
                .Include(x => x.CaptureRate)
                .Include(x => x.BaseHappiness)
                .OrderBy(x => x.Id.Length)
                .ThenBy(x => x.Id)
                .Where(x => x.IsArchived == false)
                .ToList();
            List<Pokemon> altFormList = pokemonList.Where(x => x.Id.Contains("-")).ToList();
            pokemonList = pokemonList.Except(altFormList).ToList();

            pokemonList = pokemonList.OrderBy(x => x.Id.Length).ThenBy(x => x.Id).ToList();

            return pokemonList;
        }

        public List<PokemonTeam> GetAllPokemonTeams(string username)
        {
            return this._dataContext.PokemonTeams
                .Include(x => x.Generation)
                .Include(x => x.FirstPokemon)
                    .Include("FirstPokemon.Ability")
                    .Include("FirstPokemon.PokemonTeamEV")
                    .Include("FirstPokemon.PokemonTeamIV")
                .Include(x => x.SecondPokemon)
                    .Include("SecondPokemon.Ability")
                    .Include("SecondPokemon.PokemonTeamEV")
                    .Include("SecondPokemon.PokemonTeamIV")
                .Include(x => x.ThirdPokemon)
                    .Include("ThirdPokemon.Ability")
                    .Include("ThirdPokemon.PokemonTeamEV")
                    .Include("ThirdPokemon.PokemonTeamIV")
                .Include(x => x.FourthPokemon)
                    .Include("FourthPokemon.Ability")
                    .Include("FourthPokemon.PokemonTeamEV")
                    .Include("FourthPokemon.PokemonTeamIV")
                .Include(x => x.FifthPokemon)
                    .Include("FifthPokemon.Ability")
                    .Include("FifthPokemon.PokemonTeamEV")
                    .Include("FifthPokemon.PokemonTeamIV")
                .Include(x => x.SixthPokemon)
                    .Include("SixthPokemon.Ability")
                    .Include("SixthPokemon.PokemonTeamEV")
                    .Include("SixthPokemon.PokemonTeamIV")
                .Include(x => x.User)
                .Where(x => x.User.Username == username).ToList();
        }

        public PokemonTeam GetPokemonTeam(int id)
        {
            return this._dataContext.PokemonTeams
                .Include(x => x.Generation)
                .Include(x => x.FirstPokemon)
                    .Include("FirstPokemon.PokemonTypeDetail")
                        .Include("PokemonTypeDetail.Type")
                    .Include("FirstPokemon.Ability")
                    .Include("FirstPokemon.PokemonTeamEV")
                    .Include("FirstPokemon.PokemonTeamIV")
                .Include(x => x.SecondPokemon)
                    .Include("SecondPokemon.PokemonTypeDetail")
                        .Include("PokemonTypeDetail.Type")
                    .Include("SecondPokemon.Ability")
                    .Include("SecondPokemon.PokemonTeamEV")
                    .Include("SecondPokemon.PokemonTeamIV")
                .Include(x => x.ThirdPokemon)
                    .Include("ThirdPokemon.PokemonTypeDetail")
                        .Include("PokemonTypeDetail.Type")
                    .Include("ThirdPokemon.Ability")
                    .Include("ThirdPokemon.PokemonTeamEV")
                    .Include("ThirdPokemon.PokemonTeamIV")
                .Include(x => x.FourthPokemon)
                    .Include("FourthPokemon.PokemonTypeDetail")
                        .Include("PokemonTypeDetail.Type")
                    .Include("FourthPokemon.Ability")
                    .Include("FourthPokemon.PokemonTeamEV")
                    .Include("FourthPokemon.PokemonTeamIV")
                .Include(x => x.FifthPokemon)
                    .Include("FifthPokemon.PokemonTypeDetail")
                        .Include("PokemonTypeDetail.Type")
                    .Include("FifthPokemon.Ability")
                    .Include("FifthPokemon.PokemonTeamEV")
                    .Include("FifthPokemon.PokemonTeamIV")
                .Include(x => x.SixthPokemon)
                    .Include("SixthPokemon.PokemonTypeDetail")
                        .Include("PokemonTypeDetail.Type")
                    .Include("SixthPokemon.Ability")
                    .Include("SixthPokemon.PokemonTeamEV")
                    .Include("SixthPokemon.PokemonTeamIV")
                .Include(x => x.User)
                .ToList()
                .Find(x => x.Id == id);
        }

        public List<Pokemon> GetAltForms(string pokemonId)
        {
            List<PokemonFormDetail> pokemonFormList = this._dataContext.PokemonFormDetails
                .Include(x => x.AltFormPokemon)
                    .Include("AltFormPokemon.EggCycle")
                    .Include("AltFormPokemon.GenderRatio")
                    .Include("AltFormPokemon.Classification")
                    .Include("AltFormPokemon.Generation")
                    .Include("AltFormPokemon.ExperienceGrowth")
                    .Include("AltFormPokemon.CaptureRate")
                    .Include("AltFormPokemon.BaseHappiness")
                .Include(x => x.OriginalPokemon)
                    .Include("OriginalPokemon.EggCycle")
                    .Include("OriginalPokemon.GenderRatio")
                    .Include("OriginalPokemon.Classification")
                    .Include("OriginalPokemon.Generation")
                    .Include("OriginalPokemon.ExperienceGrowth")
                    .Include("OriginalPokemon.CaptureRate")
                    .Include("OriginalPokemon.BaseHappiness")
                .Include(x => x.Form)
                .Where(x => x.OriginalPokemonId == pokemonId).ToList();
            List<Pokemon> pokemonList = new List<Pokemon>();
            foreach (var p in pokemonFormList)
            {
                pokemonList.Add(p.AltFormPokemon);
            }

            return pokemonList;
        }

        public List<PokemonFormDetail> GetAltFormDetails(string pokemonId, string formName)
        {
            return this._dataContext.PokemonFormDetails
                .Include(x => x.AltFormPokemon)
                .Include(x => x.OriginalPokemon)
                .Include(x => x.Form)
                .Where(x => x.OriginalPokemonId == pokemonId && x.AltFormPokemon.IsComplete && x.Form.Name == formName)
                .ToList();
        }

        public Pokemon GetAltFormWithFormName(string pokemonId)
        {
            PokemonFormDetail pokemonForm =  this._dataContext.PokemonFormDetails
                .Include(x => x.AltFormPokemon)
                .Include(x => x.Form)
                .ToList()
                .Find(x => x.AltFormPokemonId == pokemonId && x.AltFormPokemon.IsComplete);

            Pokemon pokemon = pokemonForm.AltFormPokemon;

            pokemon.Name += " (" + pokemonForm.Form.Name + ")";

            return pokemon;
        }

        public List<PokemonFormDetail> GetAllAltForms()
        {
            return this._dataContext.PokemonFormDetails
                .Include(x => x.AltFormPokemon)
                    .Include("AltFormPokemon.EggCycle")
                    .Include("AltFormPokemon.GenderRatio")
                    .Include("AltFormPokemon.Classification")
                    .Include("AltFormPokemon.Generation")
                    .Include("AltFormPokemon.ExperienceGrowth")
                    .Include("AltFormPokemon.CaptureRate")
                    .Include("AltFormPokemon.BaseHappiness")
                .Include(x => x.OriginalPokemon)
                    .Include("OriginalPokemon.EggCycle")
                    .Include("OriginalPokemon.GenderRatio")
                    .Include("OriginalPokemon.Classification")
                    .Include("OriginalPokemon.Generation")
                    .Include("OriginalPokemon.ExperienceGrowth")
                    .Include("OriginalPokemon.CaptureRate")
                    .Include("OriginalPokemon.BaseHappiness")
                .Include(x => x.Form)
                .ToList();
        }

        public List<PokemonFormDetail> GetAllAltFormsOnlyComplete()
        {
            return this.GetAllAltForms().Where(x => x.AltFormPokemon.IsComplete).ToList();
        }

        public PokemonFormDetail GetPokemonFormDetailByAltFormId(string pokemonId)
        {
            return this._dataContext.PokemonFormDetails
                .Include(x => x.AltFormPokemon)
                .Include(x => x.OriginalPokemon)
                .Include(x => x.Form)
                .ToList()
                .Find(x => x.AltFormPokemonId == pokemonId);
        }

        public Form GetFormByAltFormId(string pokemonId)
        {
            PokemonFormDetail details = this._dataContext.PokemonFormDetails
                .Include(x => x.AltFormPokemon)
                .Include(x => x.Form)
                .ToList()
                .Find(x => x.AltFormPokemonId == pokemonId);

            return details.Form;
        }

        public List<PokemonFormDetail> GetPokemonFormDetailsForPokemon(string pokemonId)
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

        public PokemonTypeDetail GetPokemonWithTypes(string pokemonId)
        {
            return this._dataContext.PokemonTypeDetails
                .Include(x => x.Pokemon)
                .Include(x => x.PrimaryType)
                .Include(x => x.SecondaryType)
                .ToList()
                .Find(x => x.Pokemon.Id == pokemonId);
        }

        public PokemonTypeDetail GetPokemonWithTypesNoIncludes(string pokemonId)
        {
            return this._dataContext.PokemonTypeDetails
                .ToList()
                .Find(x => x.PokemonId == pokemonId);
        }

        public List<PokemonTypeDetail> GetAllPokemonWithTypes()
        {
            List<PokemonTypeDetail> pokemonList = this._dataContext.PokemonTypeDetails
                                                        .Include(x => x.Pokemon)
                                                            .Include("Pokemon.EggCycle")
                                                            .Include("Pokemon.BaseHappiness")
                                                            .Include("Pokemon.CaptureRate")
                                                            .Include("Pokemon.ExperienceGrowth")
                                                            .Include("Pokemon.Generation")
                                                            .Include("Pokemon.Classification")
                                                            .Include("Pokemon.GenderRatio")
                                                        .Include(x => x.PrimaryType)
                                                        .Include(x => x.SecondaryType)
                                                        .Where(x => x.Pokemon.IsComplete == true)
                                                        .ToList();
            List<PokemonTypeDetail> altFormList = pokemonList.Where(x => x.Pokemon.Id.Contains("-")).ToList();
            pokemonList = pokemonList.Except(altFormList).ToList();

            pokemonList = pokemonList.OrderBy(x => x.Pokemon.Id.Length).ThenBy(x => x.Pokemon.Id).ToList();

            return pokemonList;
        }

        public List<PokemonTypeDetail> GetAllPokemonWithSpecificTypes(int primaryTypeId, int secondaryTypeId)
        {
            List<PokemonTypeDetail> pokemonList = this._dataContext.PokemonTypeDetails
                                                        .Include(x => x.Pokemon)
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

            pokemonList = pokemonList.OrderBy(x => x.Pokemon.Name).ToList();

            return pokemonList;
        }

        public List<PokemonTypeDetail> GetAllPokemonWithTypesAndIncomplete()
        {
            List<PokemonTypeDetail> pokemonList = this._dataContext.PokemonTypeDetails
                                                        .Include(x => x.Pokemon)
                                                            .Include("Pokemon.EggCycle")
                                                            .Include("Pokemon.BaseHappiness")
                                                            .Include("Pokemon.CaptureRate")
                                                            .Include("Pokemon.ExperienceGrowth")
                                                            .Include("Pokemon.Generation")
                                                            .Include("Pokemon.Classification")
                                                            .Include("Pokemon.GenderRatio")
                                                        .Include(x => x.PrimaryType)
                                                        .Include(x => x.SecondaryType)
                                                        .ToList();

            pokemonList = pokemonList.OrderBy(x => x.Pokemon.Id.Length).ThenBy(x => x.Pokemon.Id).ToList();

            return pokemonList;
        }

        public PokemonAbilityDetail GetPokemonWithAbilities(string pokemonId)
        {
            return this._dataContext.PokemonAbilityDetails.Include(x => x.Pokemon)
                .Include(x => x.PrimaryAbility)
                .Include(x => x.SecondaryAbility)
                .Include(x => x.HiddenAbility)
                .Include(x => x.SpecialEventAbility)
                .ToList()
                .Find(x => x.Pokemon.Id == pokemonId);
        }

        public PokemonAbilityDetail GetPokemonWithAbilitiesNoIncludes(string pokemonId)
        {
            return this._dataContext.PokemonAbilityDetails.Include(x => x.Pokemon)
                .ToList()
                .Find(x => x.PokemonId == pokemonId);
        }

        public List<PokemonAbilityDetail> GetAllPokemonWithAbilities()
        {
            return this._dataContext.PokemonAbilityDetails
                .Include(x => x.Pokemon)
                .Include(x => x.PrimaryAbility)
                .Include(x => x.SecondaryAbility)
                .Include(x => x.HiddenAbility)
                .Where(x => x.Pokemon.IsComplete == true)
                .ToList();
        }

        public List<PokemonLegendaryDetail> GetAllPokemonWithLegendaryTypes()
        {
            return this._dataContext.PokemonLegendaryDetails
                .Include(x => x.Pokemon)
                .Include(x => x.LegendaryType)
                .Where(x => x.Pokemon.IsComplete == true)
                .ToList();
        }

        public List<PokemonAbilityDetail> GetAllPokemonWithAbilitiesAndIncomplete()
        {
            return this._dataContext.PokemonAbilityDetails
                .Include(x => x.Pokemon)
                .Include(x => x.PrimaryAbility)
                .Include(x => x.SecondaryAbility)
                .Include(x => x.HiddenAbility)
                .ToList();
        }

        public PokemonEggGroupDetail GetPokemonWithEggGroups(string pokemonId)
        {
            return this._dataContext.PokemonEggGroupDetails
                .Include(x => x.Pokemon)
                .Include(x => x.PrimaryEggGroup)
                .Include(x => x.SecondaryEggGroup)
                .ToList()
                .Find(x => x.Pokemon.Id == pokemonId);
        }

        public List<Pokemon> GetSurroundingPokemon(string pokemonId)
        {
            List<Pokemon> surroundingPokemon = new List<Pokemon>();
            List<string> pokemonIds = this.GetPokemonIds();
            string previousId, nextId;
            int index = pokemonIds.FindIndex(x => x == pokemonId);

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

        public List<string> GetPokemonIds()
        {
            List<string> ids = new List<string>();

            foreach(var p in this.GetAllPokemon())
            {
                if(p.Id.IndexOf('-') == -1)
                {
                    ids.Add(p.Id);
                }
            }

            return ids;
        }

        public PokemonEggGroupDetail GetPokemonWithEggGroupsNoIncludes(string pokemonId)
        {
            return this._dataContext.PokemonEggGroupDetails
                .ToList()
                .Find(x => x.PokemonId == pokemonId);
        }

        public PokemonEggGroupDetail GetPokemonWithEggGroupsWithIncomplete(string pokemonId)
        {
            return this._dataContext.PokemonEggGroupDetails
                .Include(x => x.Pokemon)
                .Include(x => x.PrimaryEggGroup)
                .Include(x => x.SecondaryEggGroup)
                .ToList()
                .Find(x => x.Pokemon.Id == pokemonId);
        }

        public List<PokemonEggGroupDetail> GetAllPokemonWithEggGroups()
        {
            return this._dataContext.PokemonEggGroupDetails
                .Include(x => x.Pokemon)
                .Include(x => x.PrimaryEggGroup)
                .Include(x => x.SecondaryEggGroup)
                .Where(x => x.Pokemon.IsComplete == true)
                .ToList();
        }

        public List<PokemonEggGroupDetail> GetAllPokemonWithEggGroupsAndIncomplete()
        {
            return this._dataContext.PokemonEggGroupDetails
                .Include(x => x.Pokemon)
                .Include(x => x.PrimaryEggGroup)
                .Include(x => x.SecondaryEggGroup)
                .ToList();
        }

        public BaseStat GetPokemonBaseStats(string pokemonId)
        {
            return this._dataContext.BaseStats
                .Include(x => x.Pokemon)
                .ToList()
                .Find(x => x.Pokemon.Id == pokemonId);
        }

        public BaseStat GetPokemonBaseStatsNoIncludes(string pokemonId)
        {
            return this._dataContext.BaseStats
                .ToList()
                .Find(x => x.PokemonId == pokemonId);
        }

        public EVYield GetPokemonEVYields(string pokemonId)
        {
            return this._dataContext.EVYields
                .Include(x => x.Pokemon)
                .ToList()
                .Find(x => x.Pokemon.Id == pokemonId);
        }

        public List<Pokemon> GetAllPokemonWithClassifications()
        {
            return this._dataContext.Pokemon
                .Include(x => x.Classification)
                .Where(x => x.IsComplete == true)
                .ToList();
        }

        public BaseStat GetBaseStat(string pokemonId)
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

        public EVYield GetEVYield(string pokemonId)
        {
            return this._dataContext.EVYields
                .Include(x => x.Pokemon)
                .ToList()
                .Find(x => x.Pokemon.Id == pokemonId);
        }

        public EVYield GetEVYieldNoIncludes(string pokemonId)
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

        public List<TypeChart> GetTypeChart()
        {
            List<TypeChart> typeChart = this._dataContext.TypeCharts
                .Include(x => x.Attack)
                .Include(x => x.Defend)
                .OrderBy(x => x.Attack.Id)
                .ThenBy(x => x.Defend.Id)
                .ToList();

            return typeChart;
        }

        public TypeEffectivenessViewModel GetTypeChartPokemon(string pokemonId)
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

        public List<BaseHappiness> GetBaseHappinesses()
        {
            return this._dataContext.BaseHappiness.OrderBy(x => x.Happiness).Where(x => x.IsArchived == false).ToList();
        }

        public ShinyHuntingTechnique GetShinyHuntingTechnique(int id)
        {
            return this._dataContext.ShinyHuntingTechniques.ToList().Find(x => x.Id == id);
        }

        public List<ShinyHuntingTechnique> GetShinyHuntingTechniques()
        {
            return this._dataContext.ShinyHuntingTechniques.Where(x => !x.IsArchived).ToList();
        }

        public Classification GetClassification(int id)
        {
            return this._dataContext.Classifications.ToList().Find(x => x.Id == id);
        }

        public List<Classification> GetClassifications()
        {
            return this._dataContext.Classifications.OrderBy(x => x.Name).Where(x => x.IsArchived == false).ToList();
        }

        public List<Classification> GetClassificationsWithArchive()
        {
            return this._dataContext.Classifications.OrderBy(x => x.Name).ToList();
        }

        public ShinyHunt GetShinyHunt(int id)
        {
            return this._dataContext.ShinyHunts.Include(x => x.User)
                .Include(x => x.Pokemon)
                .Include(x => x.Generation)
                .Include(x => x.ShinyHuntingTechnique)
                .ToList()
                .Find(x => x.Id == id);
        }

        public List<ShinyHunt> GetShinyHunter(string username)
        {
            return this._dataContext.ShinyHunts
                .Include(x => x.User)
                .Include(x => x.Pokemon)
                .Include(x => x.Generation)
                .Include(x => x.ShinyHuntingTechnique)
                .Where(x => x.User.Username == username && !x.IsArchived)
                .OrderBy(x => x.User.Username)
                .ToList();
        }

        public List<ShinyHunt> GetShinyHunters()
        {
            return this._dataContext.ShinyHunts
                .Include(x => x.User)
                .Include(x => x.Pokemon)
                .Include(x => x.Generation)
                .Include(x => x.ShinyHuntingTechnique)
                .Where(x => !x.IsArchived)
                .OrderBy(x => x.User.Username)
                .ToList();
        }

        public List<ShinyHunt> GetShinyHuntersWithArchive()
        {
            return this._dataContext.ShinyHunts
                .Include(x => x.User)
                .Include(x => x.Pokemon)
                .Include(x => x.ShinyHuntingTechnique)
                .Include(x => x.Generation)
                .OrderBy(x => x.User.Username)
                .ToList();
        }

        public List<ShinyHuntingTechnique> GetShinyHuntingTechniquesWithArchive()
        {
            return this._dataContext.ShinyHuntingTechniques.OrderBy(x => x.Name).ToList();
        }

        public List<CaptureRate> GetCaptureRates()
        {
            return this._dataContext.CaptureRates.OrderBy(x => x.CatchRate).Where(x => x.IsArchived == false).ToList();
        }

        public List<EggCycle> GetEggCycles()
        {
            return this._dataContext.EggCycles.OrderBy(x => x.CycleCount).Where(x => x.IsArchived == false).ToList();
        }

        public List<ExperienceGrowth> GetExperienceGrowths()
        {
            return this._dataContext.ExperienceGrowths.OrderBy(x => x.Name).Where(x => x.IsArchived == false).ToList();
        }

        public List<GenderRatio> GetGenderRatios()
        {
            return this._dataContext.GenderRatios.Where(x => x.IsArchived == false).ToList();
        }

        public List<EvolutionMethod> GetEvolutionMethods()
        {
            return this._dataContext.EvolutionMethods.OrderBy(x => x.Name).ToList();
        }

        public List<Generation> GetGenerations()
        {
            return this._dataContext.Generations.Where(x => x.IsArchived == false).ToList();
        }

        public List<Generation> GetGenerationsWithArchive()
        {
            return this._dataContext.Generations.ToList();
        }

        public Generation GetGeneration(string id)
        {
            return this._dataContext.Generations.ToList().Find(x => x.Id == id);
        }

        public Generation GetGenerationByPokemon(string id)
        {
            return this.GetGeneration(this.GetPokemonById(id).GenerationId);
        }

        public User GetUserWithUsername(string username)
        {
            return this._dataContext.Users.ToList().Find(x => x.Username == username);
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

        public void AddUpdate(Update update)
        {
            this._dataContext.Updates.Add(update);
            this._dataContext.SaveChanges();
        }

        public void AddUser(User user)
        {
            this._dataContext.Users.Add(user);
            this._dataContext.SaveChanges();
        }

        public void AddGeneration(Generation generation)
        {
            this._dataContext.Generations.Add(generation);
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

        public void AddEggGroup(EggGroup eggGroup)
        {
            this._dataContext.EggGroups.Add(eggGroup);
            this._dataContext.SaveChanges();
        }

        public void AddAbility(Ability ability)
        {
            this._dataContext.Abilities.Add(ability);
            this._dataContext.SaveChanges();
        }

        public void AddPokemon(Pokemon pokemon)
        {
            this._dataContext.Pokemon.Add(pokemon);
            this._dataContext.SaveChanges();
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

        public void UpdatePokemonTypeDetail(PokemonTypeDetail pokemonTypeDetail)
        {
            this._dataContext.PokemonTypeDetails.Update(pokemonTypeDetail);
            this._dataContext.SaveChanges();
        }

        public void UpdatePokemonAbilityDetail(PokemonAbilityDetail pokemonAbilityDetail)
        {
            this._dataContext.PokemonAbilityDetails.Update(pokemonAbilityDetail);
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

        public void UpdateClassification(Classification classification)
        {
            this._dataContext.Classifications.Update(classification);
            this._dataContext.SaveChanges();
        }

        public void ArchiveUser(int id)
        {
            User user = this.GetUserById(id);
            user.IsArchived = true;
            this.UpdateUser(user);
        }

        public void UpdateLegendaryType(LegendaryType legendaryType)
        {
            this._dataContext.LegendaryTypes.Update(legendaryType);
            this._dataContext.SaveChanges();
        }

        public void ArchiveLegendaryType(int id)
        {
            LegendaryType legendaryType = this.GetLegendaryType(id);
            legendaryType.IsArchived = true;
            this.UpdateLegendaryType(legendaryType);
        }

        public void ArchiveShinyHunt(int id)
        {
            ShinyHunt shinyHunt = this.GetShinyHunt(id);
            shinyHunt.IsArchived = true;
            this.UpdateShinyHunt(shinyHunt);
        }

        public void ArchiveForm(int id)
        {
            Form form = this.GetForm(id);
            form.IsArchived = true;
            this.UpdateForm(form);
        }

        public void ArchiveGeneration(string id)
        {
            Generation generation = this.GetGeneration(id);
            generation.IsArchived = true;
            this.UpdateGeneration(generation);
        }

        public void ArchiveType(int id)
        {
            Type type = this.GetType(id);
            type.IsArchived = true;
            this.UpdateType(type);
        }

        public void ArchiveEggGroup(int id)
        {
            EggGroup eggGroup = this.GetEggGroup(id);
            eggGroup.IsArchived = true;
            this.UpdateEggGroup(eggGroup);
        }

        public void ArchiveAbility(int id)
        {
            Ability ability = this.GetAbility(id);
            ability.IsArchived = true;
            this.UpdateAbility(ability);
        }

        public void ArchiveShinyHuntingTechnique(int id)
        {
            ShinyHuntingTechnique shinyHuntingTechnique = this.GetShinyHuntingTechnique(id);
            shinyHuntingTechnique.IsArchived = true;
            this.UpdateShinyHuntingTechnique(shinyHuntingTechnique);
        }

        public void ArchiveClassification(int id)
        {
            Classification classification = this.GetClassification(id);
            classification.IsArchived = true;
            this.UpdateClassification(classification);
        }

        public void UnarchiveGeneration(string id)
        {
            Generation generation = this.GetGeneration(id);
            generation.IsArchived = false;
            this.UpdateGeneration(generation);
        }

        public void UnarchiveType(int id)
        {
            Type type = this.GetType(id);
            type.IsArchived = false;
            this.UpdateType(type);
        }

        public void UnarchiveForm(int id)
        {
            Form form = this.GetForm(id);
            form.IsArchived = false;
            this.UpdateForm(form);
        }

        public void UnarchiveShinyHunt(int id)
        {
            ShinyHunt shinyHunt = this.GetShinyHunt(id);
            shinyHunt.IsArchived = false;
            this.UpdateShinyHunt(shinyHunt);
        }

        public void UnarchiveAbility(int id)
        {
            Ability ability = this.GetAbility(id);
            ability.IsArchived = false;
            this.UpdateAbility(ability);
        }

        public void UnarchiveEggGroup(int id)
        {
            EggGroup eggGroup = this.GetEggGroup(id);
            eggGroup.IsArchived = false;
            this.UpdateEggGroup(eggGroup);
        }

        public void UnarchiveClassification(int id)
        {
            Classification classification = this.GetClassification(id);
            classification.IsArchived = false;
            this.UpdateClassification(classification);
        }

        public void UnarchiveLegendaryType(int id)
        {
            LegendaryType legendaryType = this.GetLegendaryType(id);
            legendaryType.IsArchived = false;
            this.UpdateLegendaryType(legendaryType);
        }

        public void UnarchiveShinyHuntingTechnique(int id)
        {
            ShinyHuntingTechnique shinyHuntingTechnique = this.GetShinyHuntingTechnique(id);
            shinyHuntingTechnique.IsArchived = false;
            this.UpdateShinyHuntingTechnique(shinyHuntingTechnique);
        }

        public void UnarchiveUser(int id)
        {
            User user = this.GetUserById(id);
            user.IsArchived = false;
            this.UpdateUser(user);
        }

        public void DeletePokemon(string id)
        {
            Pokemon pokemon = this.GetPokemonById(id);
            this._dataContext.Pokemon.Remove(pokemon);
            this._dataContext.SaveChanges();
        }

        public void DeleteGeneration(string id)
        {
            Generation generation = this.GetGeneration(id);
            this._dataContext.Generations.Remove(generation);
            this._dataContext.SaveChanges();
        }

        public void DeleteType(int id)
        {
            Type type = this.GetType(id);
            this._dataContext.Types.Remove(type);
            this._dataContext.SaveChanges();
        }

        public void DeleteAbility(int id)
        {
            Ability ability = this.GetAbility(id);
            this._dataContext.Abilities.Remove(ability);
            this._dataContext.SaveChanges();
        }

        public void DeleteUser(int id)
        {
            User user = this.GetUserById(id);
            this._dataContext.Users.Remove(user);
            this._dataContext.SaveChanges();
        }

        public void DeleteLegendaryType(int id)
        {
            LegendaryType legendaryType = this.GetLegendaryType(id);
            this._dataContext.LegendaryTypes.Remove(legendaryType);
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

        public void DeleteForm(int id)
        {
            Form form = this.GetForm(id);
            this._dataContext.Forms.Remove(form);
            this._dataContext.SaveChanges();
        }

        public void DeleteClassification(int id)
        {
            Classification classification = this.GetClassification(id);
            this._dataContext.Classifications.Remove(classification);
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
    }
}