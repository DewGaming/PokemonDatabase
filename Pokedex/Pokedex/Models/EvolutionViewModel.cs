using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class EvolutionViewModel : Evolution
    {
        public List<EvolutionMethod> AllEvolutionMethods { get; set; }

        public List<Pokemon> AllPokemon { get; set; }
    }
}