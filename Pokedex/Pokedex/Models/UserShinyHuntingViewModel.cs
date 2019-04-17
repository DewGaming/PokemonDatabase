using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class UserShinyHuntingViewModel
    {
        public List<ShinyHuntingTechnique> AllShinyHuntingTechniques { get; set; }
        public List<UserShinyHuntingTechniqueDetail> AllShinyHunters { get; set; }
    }
}