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
            return _dataContext.Types.ToList().Find(x => x.ID == ID);;
        }

        public EggGroup GetEggGroup(int ID)
        {
            return _dataContext.EggGroups.ToList().Find(x => x.ID == ID);;
        }

        public BaseHappiness GetBaseHappiness(int ID)
        {
            return _dataContext.BaseHappiness.ToList().Find(x => x.ID == ID);
        }

        public Classification GetClassification(int ID)
        {
            return _dataContext.Classifications.ToList().Find(x => x.ID == ID);;
        }

        public CaptureRate GetCaptureRate(int ID)
        {
            return _dataContext.CaptureRates.ToList().Find(x => x.ID == ID);
        }

        public EggCycle GetEggCycle(int ID)
        {
            return _dataContext.EggCycles.ToList().Find(x => x.ID == ID);;
        }
    }
}