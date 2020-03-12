using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the base pokemon's view model.
    /// </summary>
    public class BasePokemonViewModel : Pokemon
    {
        /// <summary>
        /// Gets or sets the pokemon.
        /// </summary>
        public Pokemon Pokemon { get; set; }

        /// <summary>
        /// Gets or sets a list of all egg cycles.
        /// </summary>
        public List<EggCycle> AllEggCycles { get; set; }

        /// <summary>
        /// Gets or sets a list of all gender ratios.
        /// </summary>
        public List<GenderRatioViewModel> AllGenderRatios { get; set; }

        /// <summary>
        /// Gets or sets a list of all classifications.
        /// </summary>
        public List<Classification> AllClassifications { get; set; }

        /// <summary>
        /// Gets or sets a list of all games.
        /// </summary>
        public List<Game> AllGames { get; set; }

        /// <summary>
        /// Gets or sets a list of all experience growths.
        /// </summary>
        public List<ExperienceGrowth> AllExperienceGrowths { get; set; }

        /// <summary>
        /// Gets or sets a list of all capture rates.
        /// </summary>
        public List<CaptureRate> AllCaptureRates { get; set; }

        /// <summary>
        /// Gets or sets a list of all base happiness.
        /// </summary>
        public List<BaseHappiness> AllBaseHappinesses { get; set; }
    }
}
