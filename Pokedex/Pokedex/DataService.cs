using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public List<Ability> GetPokemonAbilities(string pokemonId)
        {
            PokemonAbilityDetail abilityDetail = this._dataContext.PokemonAbilityDetails.Include(a => a.Pokemon).Include(a => a.PrimaryAbility).Include(a => a.SecondaryAbility).Include(a => a.HiddenAbility).ToList().Find(a => a.Pokemon.Id == pokemonId);
            List<Ability> abilities = new List<Ability>();
            abilities.Add(this.GetAbility(abilityDetail.PrimaryAbility.Id));
            if (abilityDetail.SecondaryAbility != null)
            {
                abilities.Add(this.GetAbility(abilityDetail.SecondaryAbility.Id));
            }

            if (abilityDetail.HiddenAbility != null)
            {
                abilities.Add(this.GetAbility(abilityDetail.HiddenAbility.Id));
            }

            return abilities;
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

        public List<Type> GetTypeChartTypes()
        {
            return this._dataContext.Types.Where(x => x.IsArchived == false).ToList();
        }

        public List<Type> GetPokemonTypes(string pokemonId)
        {
            PokemonTypeDetail typeDetail = this._dataContext.PokemonTypeDetails.Include(t => t.Pokemon).Include(t => t.PrimaryType).Include(t => t.SecondaryType).ToList().Find(t => t.Pokemon.Id == pokemonId);
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
                .Find(e => e.Id == id);
        }

        public List<EggGroup> GetEggGroups()
        {
            return this._dataContext.EggGroups.OrderBy(x => x.Name).Where(x => x.IsArchived == false).ToList();
        }

        public List<EggGroup> GetEggGroupsWithArchive()
        {
            return this._dataContext.EggGroups.OrderBy(x => x.Name).ToList();
        }

        public List<EggGroup> GetPokemonEggGroups(string pokemonId)
        {
            PokemonEggGroupDetail eggGroupDetail = this._dataContext.PokemonEggGroupDetails.Include(e => e.Pokemon).Include(e => e.PrimaryEggGroup).Include(e => e.SecondaryEggGroup).ToList().Find(e => e.Pokemon.Id == pokemonId);
            List<EggGroup> eggGroups = new List<EggGroup>();
            eggGroups.Add(this.GetEggGroup(eggGroupDetail.PrimaryEggGroup.Id));
            if (eggGroupDetail.SecondaryEggGroup != null)
            {
                eggGroups.Add(this.GetEggGroup(eggGroupDetail.SecondaryEggGroup.Id));
            }

            return eggGroups;
        }

        public List<Evolution> GetEvolutions()
        {
            return this._dataContext.Evolutions
                .Include(x => x.PreevolutionPokemon)
                .Include(x => x.EvolutionPokemon)
                .Include(e => e.EvolutionMethod)
                .ToList();
        }

        public Evolution GetPreEvolution(string pokemonId)
        {
            return this.GetEvolutions().Find(e => e.EvolutionPokemon.Id == pokemonId);
        }

        public List<Evolution> GetPokemonEvolutions(string pokemonId)
        {
            List<Evolution> evolutions = this.GetEvolutions()
                .Where(e => e.PreevolutionPokemon.Id == pokemonId)
                .OrderBy(p => p.EvolutionPokemon.Id.Length)
                .ThenBy(p => p.EvolutionPokemon.Id)
                .ToList();
            return evolutions;
        }

        public Form GetForm(int id)
        {
            return this._dataContext.Forms.ToList().Find(f => f.Id == id);
        }

        public List<PokemonFormDetail> GetPokemonForms(string pokemonId)
        {
            return this._dataContext.PokemonFormDetails
                .Include(f => f.Form)
                .Include(f => f.OriginalPokemon)
                .Include(f => f.AltFormPokemon)
                .Where(f => f.OriginalPokemon.Id == pokemonId)
                .OrderBy(p => p.AltFormPokemon.Id.Substring(p.AltFormPokemon.Id.IndexOf("-") + 1).Length)
                .ThenBy(p => p.AltFormPokemon.Id.Substring(p.AltFormPokemon.Id.IndexOf("-") + 1))
                .ToList();
        }

        public string GetPokemonFormName(string pokemonId)
        {
            PokemonFormDetail formDetail = this._dataContext.PokemonFormDetails
                .Include(f => f.Form)
                .Include(f => f.OriginalPokemon)
                .Include(f => f.AltFormPokemon)
                .ToList()
                .Find(f => f.AltFormPokemon.Id == pokemonId);
            return formDetail.Form.Name;
        }

        public Pokemon GetPokemon(string name)
        {
            return this._dataContext.Pokemon
                .Include(p => p.EggCycle)
                .Include(p => p.GenderRatio)
                .Include(p => p.Classification)
                .Include(p => p.Generation)
                .Include(p => p.ExperienceGrowth)
                .Include(p => p.CaptureRate)
                .Include(p => p.BaseHappiness)
                .ToList()
                .Find(x => x.Name == name);
        }

        public Pokemon GetPokemonById(string id)
        {
            return this._dataContext.Pokemon
                .Include(p => p.EggCycle)
                .Include(p => p.GenderRatio)
                .Include(p => p.Classification)
                .Include(p => p.Generation)
                .Include(p => p.ExperienceGrowth)
                .Include(p => p.CaptureRate)
                .Include(p => p.BaseHappiness)
                .ToList()
                .Find(x => x.Id == id);
        }

        public List<Pokemon> GetAllPokemon()
        {
            return this._dataContext.Pokemon
                .Include(p => p.EggCycle)
                .Include(p => p.GenderRatio)
                .Include(p => p.Classification)
                .Include(p => p.Generation)
                .Include(p => p.ExperienceGrowth)
                .Include(p => p.CaptureRate)
                .Include(p => p.BaseHappiness)
                .OrderBy(p => p.Id.Length)
                .ThenBy(p => p.Id)
                .Where(x => x.IsArchived == false)
                .ToList();
        }

        public List<Pokemon> GetAltForms(Pokemon pokemon)
        {
            List<PokemonFormDetail> pokemonFormList = this._dataContext.PokemonFormDetails
                .Include(p => p.AltFormPokemon)
                    .ThenInclude(x => x.EggCycle)
                .Include(p => p.AltFormPokemon)
                    .ThenInclude(x => x.GenderRatio)
                .Include(p => p.AltFormPokemon)
                    .ThenInclude(x => x.Classification)
                .Include(p => p.AltFormPokemon)
                    .ThenInclude(x => x.Generation)
                .Include(p => p.AltFormPokemon)
                    .ThenInclude(x => x.ExperienceGrowth)
                .Include(p => p.AltFormPokemon)
                    .ThenInclude(x => x.CaptureRate)
                .Include(p => p.AltFormPokemon)
                    .ThenInclude(x => x.BaseHappiness)
                .Include(p => p.OriginalPokemon)
                    .ThenInclude(x => x.EggCycle)
                .Include(p => p.OriginalPokemon)
                    .ThenInclude(x => x.GenderRatio)
                .Include(p => p.OriginalPokemon)
                    .ThenInclude(x => x.Classification)
                .Include(p => p.OriginalPokemon)
                    .ThenInclude(x => x.Generation)
                .Include(p => p.OriginalPokemon)
                    .ThenInclude(x => x.ExperienceGrowth)
                .Include(p => p.OriginalPokemon)
                    .ThenInclude(x => x.CaptureRate)
                .Include(p => p.OriginalPokemon)
                    .ThenInclude(x => x.BaseHappiness).Where(p => p.OriginalPokemon == pokemon).ToList();
            List<Pokemon> pokemonList = new List<Pokemon>();
            foreach (var p in pokemonFormList)
            {
                pokemonList.Add(p.AltFormPokemon);
            }

            return pokemonList;
        }

        public List<PokemonTypeDetail> GetPokemonWithTypes()
        {
            List<PokemonTypeDetail> pokemon = this._dataContext.PokemonTypeDetails.Include(p => p.Pokemon).Include("Pokemon.EggCycle").Include("Pokemon.BaseHappiness").Include("Pokemon.CaptureRate").Include("Pokemon.ExperienceGrowth").Include("Pokemon.Generation").Include("Pokemon.Classification").Include("Pokemon.GenderRatio").Include(p => p.PrimaryType).Include(p => p.SecondaryType).ToList();
            List<PokemonTypeDetail> altForms = pokemon.Where(p => p.Pokemon.Id.Contains("-")).ToList();
            pokemon = pokemon.Except(altForms).ToList();

            pokemon = pokemon.OrderBy(p => p.Pokemon.Id.Length).ThenBy(p => p.Pokemon.Id).ToList();

            return pokemon;
        }

        public PokemonTypeDetail GetPokemonWithTypes(string pokemonId)
        {
            return this._dataContext.PokemonTypeDetails.Include(p => p.Pokemon).Include(p => p.PrimaryType).Include(p => p.SecondaryType).ToList().Find(x => x.Pokemon.Id == pokemonId);
        }

        public List<PokemonTypeDetail> GetAllPokemonWithTypes()
        {
            return this._dataContext.PokemonTypeDetails.Include(p => p.Pokemon).Include(p => p.PrimaryType).Include(p => p.SecondaryType).ToList();
        }

        public PokemonAbilityDetail GetPokemonWithAbilities(string pokemonId)
        {
            return this._dataContext.PokemonAbilityDetails.Include(p => p.Pokemon).Include(p => p.PrimaryAbility).Include(p => p.SecondaryAbility).Include(p => p.HiddenAbility).ToList().Find(x => x.Pokemon.Id == pokemonId);
        }

        public List<PokemonAbilityDetail> GetAllPokemonWithAbilities()
        {
            return this._dataContext.PokemonAbilityDetails.Include(p => p.Pokemon).Include(p => p.PrimaryAbility).Include(p => p.SecondaryAbility).Include(p => p.HiddenAbility).ToList();
        }

        public PokemonEggGroupDetail GetPokemonWithEggGroups(string pokemonId)
        {
            return this._dataContext.PokemonEggGroupDetails.Include(p => p.Pokemon).Include(p => p.PrimaryEggGroup).Include(p => p.SecondaryEggGroup).ToList().Find(x => x.Pokemon.Id == pokemonId);
        }

        public List<PokemonEggGroupDetail> GetAllPokemonWithEggGroups()
        {
            return this._dataContext.PokemonEggGroupDetails.Include(p => p.Pokemon).Include(p => p.PrimaryEggGroup).Include(p => p.SecondaryEggGroup).ToList();
        }

        public List<Pokemon> GetAllPokemonWithClassifications()
        {
            return this._dataContext.Pokemon.Include(p => p.Classification).ToList();
        }

        public BaseStat GetBaseStat(string pokemonId)
        {
            return this._dataContext.BaseStats
                .Include(b => b.Pokemon)
                .ToList()
                .Find(b => b.Pokemon.Id == pokemonId);
        }

        public EVYield GetEVYield(string pokemonId)
        {
            return this._dataContext.EVYields
                .Include(e => e.Pokemon)
                .ToList()
                .Find(e => e.Pokemon.Id == pokemonId);
        }

        public List<TypeChart> GetTypeChart()
        {
            List<TypeChart> typeChart = this._dataContext.TypeCharts
                .Include(t => t.Attack)
                .Include(t => t.Defend)
                .OrderBy(t => t.Attack.Id)
                .ThenBy(t => t.Defend.Id).ToList();

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
                    .Include(t => t.Attack)
                    .Include(t => t.Defend)
                    .Where(t => t.Defend == type)
                    .ToList();
                foreach (var t in typeList)
                {
                    if (typeChart.Exists(c => c.Attack.Name == t.Name))
                    {
                        attackType = t.Name;
                        effectiveValue = typeChart.Find(c => c.Attack.Name == attackType).Effective.ToString("0.####");
                        if (effectiveValue == "0")
                        {
                            strongAgainst.Remove(attackType);
                            weakAgainst.Remove(attackType);
                            immuneTo.Add(attackType);
                        }
                        else if (effectiveValue == "0.5")
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
                        else if (effectiveValue == "2")
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
            return this._dataContext.ShinyHunts.Include(x => x.User).Include(x => x.Pokemon).Include(x => x.Generation).Include(x => x.ShinyHuntingTechnique).ToList().Find(x => x.Id == id);
        }

        public List<ShinyHunt> GetShinyHunter(string username)
        {
            return this._dataContext.ShinyHunts.Include(x => x.User).Include(x => x.Pokemon).Include(x => x.Generation).Include(x => x.ShinyHuntingTechnique).Where(x => x.User.Username == username && !x.IsArchived).OrderBy(x => x.User.Username).ToList();
        }

        public List<ShinyHunt> GetShinyHunters()
        {
            return this._dataContext.ShinyHunts.Include(x => x.User).Include(x => x.Pokemon).Include(x => x.Generation).Include(x => x.ShinyHuntingTechnique).Where(x => !x.IsArchived).OrderBy(x => x.User.Username).ToList();
        }

        public List<ShinyHunt> GetShinyHuntersWithArchive()
        {
            return this._dataContext.ShinyHunts.Include(x => x.User).Include(x => x.Pokemon).Include(x => x.ShinyHuntingTechnique).Include(x => x.Generation).OrderBy(x => x.User.Username).ToList();
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

        public void AddPokemonTyping(PokemonTypeDetail typing)
        {
            this._dataContext.PokemonTypeDetails.Add(typing);
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

        public void UpdateUser(User user)
        {
            this._dataContext.Users.Update(user);
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

        public void ArchiveShinyHunt(int id)
        {
            ShinyHunt shinyHunt = this.GetShinyHunt(id);
            shinyHunt.IsArchived = true;
            this.UpdateShinyHunt(shinyHunt);
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

        public void DeleteEggGroup(int id)
        {
            EggGroup eggGroup = this.GetEggGroup(id);
            this._dataContext.EggGroups.Remove(eggGroup);
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