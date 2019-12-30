using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class GenderRatiosViewModel
    {
        public List<GenderRatio> AllGenderRatios { get; set; }

        public List<Pokemon> AllPokemon { get; set; }
    }
}