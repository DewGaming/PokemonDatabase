using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class EvolutionMethodViewModel
    {
        public List<EvolutionMethod> AllEvolutionMethods { get; set; }

        public List<Evolution> AllEvolutions { get; set; }
    }
}