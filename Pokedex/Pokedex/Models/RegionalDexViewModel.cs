using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the Admin Regional Dex page.
    /// </summary>
    public class RegionalDexViewModel : RegionalDex
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegionalDexViewModel"/> class.
        /// </summary>
        public RegionalDexViewModel()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegionalDexViewModel"/> class.
        /// </summary>
        /// <param name="regionalDex">The regional dex.</param>
        public RegionalDexViewModel(RegionalDex regionalDex)
            : base(regionalDex)
        {
        }

        /// <summary>
        /// Gets or sets a list of all games.
        /// </summary>
        public List<Game> AllGames { get; set; }
    }
}
