using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using PokemonDatabase.DataAccess.Models;
using PokemonDatabase.Models;

namespace PokemonDatabase
{
    public class DataService
    {
        private readonly DataContext _dataContext;

        public DataService(DataContext dataContext)
        {
            _dataContext = dataContext;
            //List<Pokemon> _pokemonList = _dataContext.Pokemon
            //    .Include(p => p.EggCycle)
            //    .Include(p => p.GenderRatio)
            //    .Include(p => p.Classification)
            //    .Include(p => p.Generation)
            //    .Include(p => p.ExperienceGrowth)
            //    .Include(p => p.CaptureRate)
            //    .Include(p => p.BaseHappiness)
            //    .ToList();
            //foreach(var pokemon in _pokemonList.Where(p => p.Id.Contains('-')))
            //{
            //    pokemon.Name += " (" + this.GetPokemonFormName(pokemon.Id) + ")";
            //}
        }

        public Ability GetAbility(int id)
        {
            return _dataContext.Abilities
                .ToList()
                .Find(x => x.Id == id);
        }

        public Ability GetAbilityDescription(string Name)
        {
            return _dataContext.Abilities
                .ToList()
                .Find(x => x.Name == Name);
        }

        public List<Ability> GetAbilities()
        {
            return _dataContext.Abilities.OrderBy(x => x.Name).Where(x => x.IsArchived == false).ToList();
        }

        public List<Ability> GetPokemonAbilities(string PokemonId)
        {
            PokemonAbilityDetail abilityDetail = _dataContext.PokemonAbilityDetails.Include(a => a.Pokemon).Include(a => a.PrimaryAbility).Include(a => a.SecondaryAbility).Include(a => a.HiddenAbility).ToList().Find(a => a.Pokemon.Id == PokemonId);
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
            return _dataContext.Types
                .ToList()
                .Find(x => x.Id == id);
        }

        public List<Type> GetTypesWithArchive()
        {
            return _dataContext.Types.OrderBy(x => x.Name).ToList();
        }

        public List<Type> GetTypes()
        {
            return _dataContext.Types.OrderBy(x => x.Name).Where(x => x.IsArchived == false).ToList();
        }

        public List<Type> GetPokemonTypes(string PokemonId)
        {
            PokemonTypeDetail typeDetail = _dataContext.PokemonTypeDetails.Include(t => t.Pokemon).Include(t => t.PrimaryType).Include(t => t.SecondaryType).ToList().Find(t => t.Pokemon.Id == PokemonId);
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
            return _dataContext.EggGroups
                .ToList()
                .Find(e => e.Id == id);
        }

        public List<EggGroup> GetEggGroups()
        {
            return _dataContext.EggGroups.OrderBy(x => x.Name).Where(x => x.IsArchived == false).ToList();
        }

        public List<EggGroup> GetPokemonEggGroups(string PokemonId)
        {
            PokemonEggGroupDetail eggGroupDetail = _dataContext.PokemonEggGroupDetails.Include(e => e.Pokemon).Include(e => e.PrimaryEggGroup).Include(e => e.SecondaryEggGroup).ToList().Find(e => e.Pokemon.Id == PokemonId);
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
            return _dataContext.Evolutions
                .Include(e => e.EvolutionMethod)
                .ToList();
        }

        public Evolution GetPreEvolution(string PokemonId)
        {
            return this.GetEvolutions().Find(e => e.EvolutionPokemon.Id == PokemonId);
        }

        public List<Evolution> GetPokemonEvolutions(string PokemonId)
        {
            List<Evolution> evolutions = this.GetEvolutions()
                .Where(e => e.PreevolutionPokemon.Id == PokemonId)
                .OrderBy(p => p.EvolutionPokemon.Id.Length)
                .ThenBy(p => p.EvolutionPokemon.Id)
                .ToList();
            return evolutions;
        }

        public Form GetForm(int id)
        {
            return _dataContext.Forms.ToList().Find(f => f.Id == id);
        }

        public List<PokemonFormDetail> GetPokemonForms(string PokemonId)
        {
            return _dataContext.PokemonFormDetails
                .Include(f => f.Form)
                .Include(f => f.OriginalPokemon)
                .Include(f => f.AltFormPokemon)
                .Where(f => f.OriginalPokemon.Id == PokemonId)
                .OrderBy(p => p.AltFormPokemon.Id.Substring(p.AltFormPokemon.Id.IndexOf("-") + 1).Length)
                .ThenBy(p => p.AltFormPokemon.Id.Substring(p.AltFormPokemon.Id.IndexOf("-") + 1))
                .ToList();
        }

        public string GetPokemonFormName(string PokemonId)
        {
            PokemonFormDetail formDetail = _dataContext.PokemonFormDetails
                .Include(f => f.Form)
                .Include(f => f.OriginalPokemon)
                .Include(f => f.AltFormPokemon)
                .ToList()
                .Find(f => f.AltFormPokemon.Id == PokemonId);
            return formDetail.Form.Name;
        }

        public Pokemon GetPokemon(string Name)
        {
            return _dataContext.Pokemon
                .Include(p => p.EggCycle)
                .Include(p => p.GenderRatio)
                .Include(p => p.Classification)
                .Include(p => p.Generation)
                .Include(p => p.ExperienceGrowth)
                .Include(p => p.CaptureRate)
                .Include(p => p.BaseHappiness)
                .ToList()
                .Find(x => x.Name == Name);
        }

        public Pokemon GetPokemonById(string Id)
        {
            return _dataContext.Pokemon
                .Include(p => p.EggCycle)
                .Include(p => p.GenderRatio)
                .Include(p => p.Classification)
                .Include(p => p.Generation)
                .Include(p => p.ExperienceGrowth)
                .Include(p => p.CaptureRate)
                .Include(p => p.BaseHappiness)
                .ToList()
                .Find(x => x.Id == Id);
        }

        public List<Pokemon> GetAllPokemon()
        {
            return _dataContext.Pokemon
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
                .ToList();;
        }

        public List<Pokemon> GetAltForms(Pokemon pokemon)
        {
            List<PokemonFormDetail> pokemonFormList = _dataContext.PokemonFormDetails.Include(p => p.AltFormPokemon).Where(p => p.OriginalPokemon == pokemon).ToList();
            List<Pokemon> pokemonList = new List<Pokemon>();
            foreach (var p in pokemonFormList)
            {
                pokemonList.Add(p.AltFormPokemon);
            }
            return pokemonList;
        }

        public List<PokemonTypeDetail> GetPokemonWithTypes()
        {
            List<PokemonTypeDetail> pokemon = _dataContext.PokemonTypeDetails.Include(p => p.Pokemon).Include("Pokemon.EggCycle").Include("Pokemon.BaseHappiness").Include("Pokemon.CaptureRate").Include("Pokemon.ExperienceGrowth").Include("Pokemon.Generation").Include("Pokemon.Classification").Include("Pokemon.GenderRatio").Include(p => p.PrimaryType).Include(p => p.SecondaryType).ToList();
            List<PokemonTypeDetail> altForms = pokemon.Where(p => p.Pokemon.Id.Contains("-")).ToList();
            pokemon = pokemon.Except(altForms).ToList();

            pokemon = pokemon.OrderBy(p => p.Pokemon.Id.Length).ThenBy(p => p.Pokemon.Id).ToList();

            return pokemon;
        }

        public PokemonTypeDetail GetPokemonWithTypes(string pokemonId)
        {
            return _dataContext.PokemonTypeDetails.Include(p => p.Pokemon).Include(p => p.PrimaryType).Include(p => p.SecondaryType).ToList().Find(x => x.Pokemon.Id == pokemonId);
        }

        public List<PokemonTypeDetail> GetAllPokemonWithTypes()
        {
            return _dataContext.PokemonTypeDetails.Include(p => p.Pokemon).Include(p => p.PrimaryType).Include(p => p.SecondaryType).ToList();
        }

        public PokemonAbilityDetail GetPokemonWithAbilities(string pokemonId)
        {
            return _dataContext.PokemonAbilityDetails.Include(p => p.Pokemon).Include(p => p.PrimaryAbility).Include(p => p.SecondaryAbility).Include(p => p.HiddenAbility).ToList().Find(x => x.Pokemon.Id == pokemonId);
        }

        public List<PokemonAbilityDetail> GetAllPokemonWithAbilities()
        {
            return _dataContext.PokemonAbilityDetails.Include(p => p.Pokemon).Include(p => p.PrimaryAbility).Include(p => p.SecondaryAbility).Include(p => p.HiddenAbility).ToList();
        }

        public PokemonEggGroupDetail GetPokemonWithEggGroups(string pokemonId)
        {
            return _dataContext.PokemonEggGroupDetails.Include(p => p.Pokemon).Include(p => p.PrimaryEggGroup).Include(p => p.SecondaryEggGroup).ToList().Find(x => x.Pokemon.Id == pokemonId);
        }

        public List<PokemonEggGroupDetail> GetAllPokemonWithEggGroups()
        {
            return _dataContext.PokemonEggGroupDetails.Include(p => p.Pokemon).Include(p => p.PrimaryEggGroup).Include(p => p.SecondaryEggGroup).ToList();
        }

        public BaseStat GetBaseStat(string PokemonId)
        {
            return _dataContext.BaseStats
                .Include(b => b.Pokemon)
                .ToList()
                .Find(b => b.Pokemon.Id == PokemonId);
        }

        public EVYield GetEVYield(string PokemonId)
        {
            return _dataContext.EVYields
                .Include(e => e.Pokemon)
                .ToList()
                .Find(e => e.Pokemon.Id == PokemonId);
        }

        public List<TypeChart> GetTypeChart()
        {
            List<TypeChart> typeChart = _dataContext.TypeCharts
                .Include(t => t.Attack)
                .Include(t => t.Defend)
                .OrderBy(t => t.Attack.Id)
                .ThenBy(t => t.Defend.Id).ToList();

            return typeChart;
        }

        public TypeEffectivenessViewModel GetTypeChartPokemon(string pokemonId)
        {
            List<Type> typeList = GetTypes();
            List<Type> pokemonTypes = GetPokemonTypes(pokemonId);
            List<string> strongAgainst = new List<string>();
            List<string> superStrongAgainst = new List<string>();
            List<string> weakAgainst = new List<string>();
            List<string> superWeakAgainst = new List<string>();
            List<string> immuneTo = new List<string>();
            List<TypeChart> typeChart;
            string effectiveValue, attackType;

            foreach(var type in pokemonTypes)
            {
                typeChart = _dataContext.TypeCharts.Where(t => t.Defend == type).ToList();
                foreach(var t in typeList)
                {
                    if (typeChart.Exists(c => c.Attack == t))
                    {
                        attackType = typeChart.Find(c => c.Attack == t).Attack.Name;
                        effectiveValue = typeChart.Find(c => c.Attack == t).Effective.ToString("0.####");
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
                immuneTo = immuneTo,
                strongAgainst = strongAgainstList,
                weakAgainst = weakAgainstList
            };

            return effectivenessChart;
        }

        public List<BaseHappiness> GetBaseHappinesses()
        {
            return _dataContext.BaseHappiness.OrderBy(x => x.Happiness).Where(x => x.IsArchived == false).ToList();
        }

        public List<Classification> GetClassifications()
        {
            return _dataContext.Classifications.OrderBy(x => x.Name).Where(x => x.IsArchived == false).ToList();
        }

        public List<CaptureRate> GetCaptureRates()
        {
            return _dataContext.CaptureRates.OrderBy(x => x.CatchRate).Where(x => x.IsArchived == false).ToList();
        }

        public List<EggCycle> GetEggCycles()
        {
            return _dataContext.EggCycles.OrderBy(x => x.CycleCount).Where(x => x.IsArchived == false).ToList();
        }

        public List<ExperienceGrowth> GetExperienceGrowths()
        {
            return _dataContext.ExperienceGrowths.OrderBy(x => x.Name).Where(x => x.IsArchived == false).ToList();
        }

        public List<GenderRatio> GetGenderRatios()
        {
            return _dataContext.GenderRatios.Where(x => x.IsArchived == false).ToList();
        }

        public List<Generation> GetGenerations()
        {
            return _dataContext.Generations.Where(x => x.IsArchived == false).ToList();
        }

        public Generation GetGeneration(string id)
        {
            return _dataContext.Generations.ToList().Find(x => x.Id == id);
        }

        public User GetUser(string email)
        {
            return _dataContext.Users.ToList().Find(x => x.EmailAddress == email);
        }

        public User GetUserById(int id)
        {
            return _dataContext.Users.ToList().Find(x => x.Id == id);
        }

        public List<User> GetUsers()
        {
            return _dataContext.Users.ToList();
        }

        public void AddUser(User user)
        {
            _dataContext.Users.Add(user);
            _dataContext.SaveChanges();
        }

        public void AddGeneration(Generation generation)
        {
            _dataContext.Generations.Add(generation);
            _dataContext.SaveChanges();
        }

        public void AddType(Type type)
        {
            _dataContext.Types.Add(type);
            _dataContext.SaveChanges();
        }

        public void AddEggGroup(EggGroup eggGroup)
        {
            _dataContext.EggGroups.Add(eggGroup);
            _dataContext.SaveChanges();
        }

        public void AddAbility(Ability ability)
        {
            _dataContext.Abilities.Add(ability);
            _dataContext.SaveChanges();
        }

        public void AddPokemon(Pokemon pokemon)
        {
            _dataContext.Pokemon.Add(pokemon);
            _dataContext.SaveChanges();
        }

        public void AddPokemonTyping(PokemonTypeDetail typing)
        {
            _dataContext.PokemonTypeDetails.Add(typing);
            _dataContext.SaveChanges();
        }

        public void AddPokemonAbilities(PokemonAbilityDetail abilities)
        {
            _dataContext.PokemonAbilityDetails.Add(abilities);
            _dataContext.SaveChanges();
        }

        public void AddPokemonEggGroups(PokemonEggGroupDetail eggGroups)
        {
            _dataContext.PokemonEggGroupDetails.Add(eggGroups);
            _dataContext.SaveChanges();
        }

        public void AddPokemonBaseStat(BaseStat baseStat)
        {
            _dataContext.BaseStats.Add(baseStat);
            _dataContext.SaveChanges();
        }

        public void AddPokemonEVYield(EVYield evYield)
        {
            _dataContext.EVYields.Add(evYield);
            _dataContext.SaveChanges();
        }

        public void UpdateUser(User user)
        {
            _dataContext.Users.Update(user);
            _dataContext.SaveChanges();
        }

        public void UpdateGeneration(Generation generation)
        {
            _dataContext.Generations.Update(generation);
            _dataContext.SaveChanges();
        }

        public void UpdateType(Type type)
        {
            _dataContext.Types.Update(type);
            _dataContext.SaveChanges();
        }

        public void UpdateEggGroup(EggGroup eggGroup)
        {
            _dataContext.EggGroups.Update(eggGroup);
            _dataContext.SaveChanges();
        }

        public void UpdateAbility(Ability ability)
        {
            _dataContext.Abilities.Update(ability);
            _dataContext.SaveChanges();
        }

        public void DeleteUser(int id)
        {
            User user = this.GetUserById(id);
            user.IsArchived = true;
            this.UpdateUser(user);
        }

        public void DeleteGeneration(string id)
        {
            Generation generation = this.GetGeneration(id);
            generation.IsArchived = true;
            this.UpdateGeneration(generation);
        }

        public void DeleteType(int id)
        {
            Type type = this.GetType(id);
            type.IsArchived = true;
            this.UpdateType(type);
        }

        public void DeleteEggGroup(int id)
        {
            EggGroup eggGroup = this.GetEggGroup(id);
            eggGroup.IsArchived = true;
            this.UpdateEggGroup(eggGroup);
        }

        public void DeleteAbility(int id)
        {
            Ability ability = this.GetAbility(id);
            ability.IsArchived = true;
            this.UpdateAbility(ability);
        }

        public void RestoreGeneration(string id)
        {
            Generation generation = this.GetGeneration(id);
            generation.IsArchived = false;
            this.UpdateGeneration(generation);
        }

        public void RestoreType(int id)
        {
            Type type = this.GetType(id);
            type.IsArchived = false;
            this.UpdateType(type);
        }

        public void RestoreAbility(int id)
        {
            Ability ability = this.GetAbility(id);
            ability.IsArchived = false;
            this.UpdateAbility(ability);
        }

        public void RestoreEggGroup(int id)
        {
            EggGroup eggGroup = this.GetEggGroup(id);
            eggGroup.IsArchived = false;
            this.UpdateEggGroup(eggGroup);
        }

        public void RestoreUser(int id)
        {
            User user = this.GetUserById(id);
            user.IsArchived = false;
            this.UpdateUser(user);
        }
    }
}