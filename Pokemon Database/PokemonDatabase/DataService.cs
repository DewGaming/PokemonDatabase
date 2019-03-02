using System;
using System.Collections.Generic;
using System.Linq;
using PokemonDatabase.Models;

namespace PokemonDatabase
{
    public class DataService
    {
        private readonly DataContext _dataContext;

        public DataService(DataContext dataContext)
        {
           _dataContext = dataContext;
        }

        public List<Ability> GetAbilities()
        {
            return _dataContext.Abilities.ToList();
        }

        public Ability GetAbility(int ID)
        {
            return _dataContext.Abilities.ToList().Find(x => x.ID == ID);
        }

        public PokemonDatabase.Models.Type GetType(int ID)
        {
            return _dataContext.Types.ToList().Find(x => x.ID == ID);
        }

        public List<PokemonDatabase.Models.Type> GetTypes()
        {
            return _dataContext.Types.ToList();
        }

        public EggGroup GetEggGroup(int ID)
        {
            return _dataContext.EggGroups.ToList().Find(x => x.ID == ID);
        }

        public BaseHappiness GetBaseHappiness(int ID)
        {
            return _dataContext.BaseHappiness.ToList().Find(x => x.ID == ID);
        }

        public Classification GetClassification(int ID)
        {
            return _dataContext.Classifications.ToList().Find(x => x.ID == ID);
        }

        public CaptureRate GetCaptureRate(int ID)
        {
            return _dataContext.CaptureRates.ToList().Find(x => x.ID == ID);
        }

        public EggCycle GetEggCycle(int ID)
        {
            return _dataContext.EggCycles.ToList().Find(x => x.ID == ID);
        }

        public ExperienceGrowth GetExperienceGrowth(int ID)
        {
            return _dataContext.ExperienceGrowths.ToList().Find(x => x.ID == ID);
        }

        public GenderRatio GetGenderRatio(int ID)
        {
            return _dataContext.GenderRatios.ToList().Find(x => x.ID == ID);
        }

        public Generation GetGeneration(string ID)
        {
            return _dataContext.Generations.ToList().Find(x => x.ID == ID);
        }

        public Pokemon GetPokemon(string Name)
        {
            Pokemon pokemon = _dataContext.Pokemon.ToList().Find(x => x.Name == Name);
            
            pokemon.Classification = this.GetClassification(pokemon.ClassificationID);

            return pokemon;
        }

        public List<Pokemon> GetAllPokemon()
        {
            List<Pokemon> pokemon = _dataContext.Pokemon.ToList();
            List<Pokemon> altForms = pokemon.Where(p => p.ID.Contains("-")).ToList();
            pokemon = pokemon.Except(altForms).ToList();

            List<Classification> classification = _dataContext.Classifications.ToList();
            
            foreach(Pokemon p in pokemon)
            {
                p.Classification = classification.Find(c => c.ID == p.ClassificationID);
            }

            pokemon = pokemon.OrderBy(p => p.ID.Length).ThenBy(p => p.ID).ToList();

            return pokemon;
        }

        public BaseStat GetBaseStat(string ID)
        {
            return _dataContext.BaseStats.ToList().Find(x => x.PokemonID == ID);
        }

        public EVYield GetEVYield(string ID)
        {
            return _dataContext.EVYields.ToList().Find(x => x.PokemonID == ID);
        }

        public Form GetForm(int ID)
        {
            return _dataContext.Forms.ToList().Find(x => x.ID == ID);
        }

        public List<TypeChart> GetTypeChart()
        {
            List<TypeChart> typeChart = _dataContext.TypeChart.ToList();
            List<PokemonDatabase.Models.Type> typeList = this.GetTypes();
            foreach (TypeChart t in typeChart)
            {
                t.Attack = typeList.Find(c => c.ID == t.AttackID);
                t.Defend = typeList.Find(c => c.ID == t.DefendID);
            }
            return typeChart;
        }
    }
}