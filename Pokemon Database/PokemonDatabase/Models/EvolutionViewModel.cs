using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PokemonDatabase.DataAccess.Models;

namespace PokemonDatabase.Models
{
    public class EvolutionViewModel : Evolution
    {
        public List<EvolutionMethod> AllEvolutionMethods { get; set; }
    }
}