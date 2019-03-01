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

        public List<PokemonDatabase.Models.Type> GetTypes()
        {
            return _dataContext.Types.ToList();
        }

        public List<EggGroup> GetEggGroups()
        {
            return _dataContext.EggGroups.ToList();
        }
    }
}