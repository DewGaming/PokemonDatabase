using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class EvolutionViewModel : Evolution
    {
        public List<EvolutionMethod> AllEvolutionMethods { get; set; }
    }
}