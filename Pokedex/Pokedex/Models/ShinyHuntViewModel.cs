using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class ShinyHuntViewModel
    {
        public List<ShinyHuntingTechnique> AllShinyHuntingTechniques { get; set; }
        public List<ShinyHunt> AllShinyHunters { get; set; }
    }
}