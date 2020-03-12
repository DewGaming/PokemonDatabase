using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class BasePokemonViewModel : Pokemon
    {
        public Pokemon Pokemon { get; set; }

        public List<EggCycle> AllEggCycles { get; set; }

        public List<GenderRatioViewModel> AllGenderRatios { get; set; }

        public List<Classification> AllClassifications { get; set; }

        public List<Game> AllGames { get; set; }

        public List<ExperienceGrowth> AllExperienceGrowths { get; set; }

        public List<CaptureRate> AllCaptureRates { get; set; }

        public List<BaseHappiness> AllBaseHappinesses { get; set; }
    }
}