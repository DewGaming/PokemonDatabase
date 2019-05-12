using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class BeginShinyHuntViewModel : ShinyHunt
    {
        public List<ShinyHuntingTechnique> AllShinyHuntingTechniques { get; set; }

        public List<Pokemon> AllPokemon { get; set; }

        public List<Generation> AllGenerations { get; set; }
    }
}