using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PokemonDatabase.DataAccess.Models;

namespace PokemonDatabase
{
    public class DataService
    {
        private readonly DataContext _dataContext;

        public DataService(DataContext dataContext)
        {
           _dataContext = dataContext;
        }

        public Ability GetAbility(int Id)
        {
            return _dataContext.Abilities.ToList().Find(x => x.Id == Id);
        }

        public List<Ability> GetAbilities(string PokemonId)
        {
            PokemonAbilityDetail abilityDetail = _dataContext.PokemonAbilityDetails.Include(a => a.Pokemon).Include(a => a.PrimaryAbility).Include(a => a.SecondaryAbility).Include(a => a.HiddenAbility).ToList().Find(a => a.Pokemon.Id == PokemonId);
            List<Ability> abilities = new List<Ability>();
            abilities.Add(this.GetAbility(abilityDetail.PrimaryAbility.Id));
            if (abilityDetail.SecondaryAbility != null)
            {
                abilities.Add(this.GetAbility((int)abilityDetail.SecondaryAbility.Id));
            }
            
            if (abilityDetail.HiddenAbility != null)
            {
                abilities.Add(this.GetAbility((int)abilityDetail.HiddenAbility.Id));
            }

            return abilities;
        }

        public DataAccess.Models.Type GetType(int Id)
        {
            return _dataContext.Types.ToList().Find(x => x.Id == Id);
        }

        public List<DataAccess.Models.Type> GetTypes()
        {
            return _dataContext.Types.ToList();
        }

        public List<DataAccess.Models.Type> GetTyping(string PokemonId)
        {
            PokemonTypeDetail typeDetail = _dataContext.PokemonTypeDetails.Include(t => t.Pokemon).Include(t => t.PrimaryType).Include(t => t.SecondaryType).ToList().Find(t => t.Pokemon.Id == PokemonId);
            List<DataAccess.Models.Type> types = new List<DataAccess.Models.Type>();
            types.Add(this.GetType(typeDetail.PrimaryType.Id));
            if (typeDetail.SecondaryType != null)
            {
                types.Add(this.GetType((int)typeDetail.SecondaryType.Id));
            }

            return types;
        }

        public EggGroup GetEggGroup(int Id)
        {
            return _dataContext.EggGroups.ToList().Find(x => x.Id == Id);
        }

        public BaseHappiness GetBaseHappiness(int Id)
        {
            return _dataContext.BaseHappiness.ToList().Find(x => x.Id == Id);
        }

        public Classification GetClassification(int Id)
        {
            return _dataContext.Classifications.ToList().Find(x => x.Id == Id);
        }

        public CaptureRate GetCaptureRate(int Id)
        {
            return _dataContext.CaptureRates.ToList().Find(x => x.Id == Id);
        }

        public EggCycle GetEggCycle(int Id)
        {
            return _dataContext.EggCycles.ToList().Find(x => x.Id == Id);
        }

        public ExperienceGrowth GetExperienceGrowth(int Id)
        {
            return _dataContext.ExperienceGrowths.ToList().Find(x => x.Id == Id);
        }

        public GenderRatio GetGenderRatio(int Id)
        {
            return _dataContext.GenderRatios.ToList().Find(x => x.Id == Id);
        }

        public Generation GetGeneration(string Id)
        {
            return _dataContext.Generations.ToList().Find(x => x.Id == Id);
        }

        public List<Pokemon> GrabPokemon()
        {
            List<Pokemon> pokemonList = _dataContext.Pokemon.Include(p => p.EggCycle).Include(p => p.GenderRatio).Include(p => p.Classification).Include(p => p.Generation).Include(p => p.ExperienceGrowth).Include(p => p.CaptureRate).Include(p => p.BaseHappiness).ToList();

            return pokemonList;
        }

        public Pokemon GetPokemon(string Name)
        {
            Pokemon pokemon = this.GrabPokemon().Find(x => x.Name == Name);

            return pokemon;
        }

        public List<Pokemon> GetAllPokemon()
        {
            List<Pokemon> pokemon = this.GrabPokemon();
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

        public Form GetForm(int Id)
        {
            return _dataContext.Forms.ToList().Find(x => x.Id == Id);
        }
    }
}