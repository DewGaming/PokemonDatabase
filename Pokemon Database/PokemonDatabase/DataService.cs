using System.Collections.Generic;
using System.Linq;
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
            return _dataContext.Abilities.ToList();
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

        public List<Type> GetTypes()
        {
            return _dataContext.Types.ToList();
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
            return _dataContext.EggGroups.ToList();
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
        
        public Evolution GetEvolution(int id)
        {
            return this.GetEvolutions().Find(e => e.Id == id);
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

        public List<decimal> GetEffectiveness(Type type, List<Type> typeList)
        {
            List<decimal> effectiveness = new List<decimal>();
            List<TypeChart> typeChart = _dataContext.TypeCharts.Where(t => t.Defend == type).ToList();
            foreach(var t in typeList)
            {
                effectiveness.Add(typeChart.Find(c => c.Attack == t).Effective);
            }

            return effectiveness;
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
            List<decimal> primaryTypeEffectiveness = new List<decimal>();
            List<decimal> secondaryTypeEffectiveness = new List<decimal>();
            List<decimal> pokemonTypeEffectiveness = new List<decimal>();
            List<string> strongAgainst = new List<string>();
            List<string> superStrongAgainst = new List<string>();
            List<string> weakAgainst = new List<string>();
            List<string> superWeakAgainst = new List<string>();
            List<string> immuneTo = new List<string>();

            primaryTypeEffectiveness = GetEffectiveness(pokemonTypes[0], typeList);
            if (pokemonTypes.Count > 1)
            {
                secondaryTypeEffectiveness = GetEffectiveness(pokemonTypes[1], typeList);
                pokemonTypeEffectiveness = primaryTypeEffectiveness.Select((dValue, index) => dValue * secondaryTypeEffectiveness[index]).ToList();
            }
            else
            {
                pokemonTypeEffectiveness = primaryTypeEffectiveness;
            }

            for (var i = 0; i < typeList.Count - 1; i++)
            {
                string effectiveValue = pokemonTypeEffectiveness[i].ToString("0.####");
            
                if (effectiveValue != "1")
                {
                    if (effectiveValue == "0")
                    {
                        immuneTo.Add(typeList[i].Name);
                    }
                    else if (effectiveValue == "0.25")
                    {
                        superStrongAgainst.Add(typeList[i].Name + " Quad");
                    }
                    else if (effectiveValue == "0.5")
                    {
                        strongAgainst.Add(typeList[i].Name);
                    }
                    else if (effectiveValue == "2")
                    {
                        weakAgainst.Add(typeList[i].Name);
                    }
                    else if (effectiveValue == "4")
                    {
                        superWeakAgainst.Add(typeList[i].Name + " Quad");
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
            return _dataContext.Users.Where(x => x.IsArchived == false).ToList();
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
    }
}