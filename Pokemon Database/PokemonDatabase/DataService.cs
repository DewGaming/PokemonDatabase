using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PokemonDatabase.DataAccess.Models;

namespace PokemonDatabase
{
    public class DataService
    {
        private readonly List<Pokemon> _pokemonList;

        private readonly DataContext _dataContext;

        public DataService(DataContext dataContext)
        {
           _dataContext = dataContext;
           _pokemonList = _dataContext.Pokemon.Include(p => p.EggCycle).Include(p => p.GenderRatio).Include(p => p.Classification).Include(p => p.Generation).Include(p => p.ExperienceGrowth).Include(p => p.CaptureRate).Include(p => p.BaseHappiness).ToList();
            foreach(var pokemon in _pokemonList.Where(p => p.Id.Contains('-')))
            {
                pokemon.Name += " (" + this.GetPokemonForm(pokemon.Id).Name + ")";
            }
        }

        public Ability GetAbility(int Id)
        {
            return _dataContext.Abilities.ToList().Find(x => x.Id == Id);
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

        public Type GetType(int Id)
        {
            return _dataContext.Types.ToList().Find(x => x.Id == Id);
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

        public EggGroup GetEggGroup(int Id)
        {
            return _dataContext.EggGroups.ToList().Find(e => e.Id == Id);
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
            return _dataContext.Evolutions.Include(e => e.EvolutionMethod).ToList();
        }
        
        public Evolution GetEvolution(int Id)
        {
            return this.GetEvolutions().Find(e => e.Id ==Id);
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

        public Form GetForm(int Id)
        {
            return _dataContext.Forms.ToList().Find(f => f.Id == Id);
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

        public PokemonFormDetail GetOriginalForm(string PokemonId)
        {
            return _dataContext.PokemonFormDetails
                               .Include(f => f.Form)
                               .Include(f => f.OriginalPokemon)
                               .Include(f => f.AltFormPokemon)
                               .ToList()
                               .Find(f => f.AltFormPokemon.Id == PokemonId);
        }

        public Form GetPokemonForm(string PokemonId)
        {
            PokemonFormDetail formDetail = _dataContext.PokemonFormDetails
                                                       .Include(f => f.Form)
                                                       .Include(f => f.OriginalPokemon)
                                                       .Include(f => f.AltFormPokemon)
                                                       .ToList()
                                                       .Find(f => f.AltFormPokemon.Id == PokemonId);
            return formDetail.Form;
        }

        public Pokemon GetPokemon(string Name)
        {
            return _pokemonList.Find(x => x.Name == Name);
        }

        public List<Pokemon> GetAllPokemon()
        {
            List<Pokemon> pokemon = _pokemonList;
            List<Pokemon> altForms = pokemon.Where(p => p.Id.Contains("-")).ToList();
            pokemon = pokemon.Except(altForms).ToList();

            pokemon = pokemon.OrderBy(p => p.Id.Length).ThenBy(p => p.Id).ToList();

            return pokemon;
        }

        public BaseStat GetBaseStat(string PokemonId)
        {
            return _dataContext.BaseStats.Include(b => b.Pokemon).ToList().Find(b => b.Pokemon.Id == PokemonId);
        }

        public EVYield GetEVYield(string PokemonId)
        {
            return _dataContext.EVYields.Include(e => e.Pokemon).ToList().Find(e => e.Pokemon.Id == PokemonId);
        }
    }
}