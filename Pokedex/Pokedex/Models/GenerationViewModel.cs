using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class GenerationViewModel
    {
        public List<Generation> AllGenerations { get; set; }

        public List<Game> AllGames { get; set; }
    }
}