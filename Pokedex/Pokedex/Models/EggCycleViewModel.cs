using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class EggCycleViewModel
    {
        public List<EggCycle> AllEggCycles { get; set; }

        public List<Pokemon> AllPokemon { get; set; }
    }
}