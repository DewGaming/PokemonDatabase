using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class ExperienceGrowthViewModel
    {
        public List<ExperienceGrowth> AllExperienceGrowths { get; set; }

        public List<Pokemon> AllPokemon { get; set; }
    }
}