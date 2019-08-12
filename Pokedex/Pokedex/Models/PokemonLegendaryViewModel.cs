using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class PokemonLegendaryViewModel : PokemonLegendaryDetail
    {
        public List<LegendaryType> AllLegendaryTypes { get; set; }
    }
}