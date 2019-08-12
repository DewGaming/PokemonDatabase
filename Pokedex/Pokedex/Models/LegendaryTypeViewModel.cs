using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class LegendaryTypeViewModel
    {
        public List<LegendaryType> AllLegendaryTypes { get; set; }

        public List<PokemonLegendaryDetail> AllPokemon { get; set; }
    }
}