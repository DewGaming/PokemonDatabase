using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class BaseHappinessViewModel
    {
        public List<BaseHappiness> AllBaseHappinesses { get; set; }

        public List<Pokemon> AllPokemon { get; set; }
    }
}