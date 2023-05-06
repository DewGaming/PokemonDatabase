using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the create pokemon team page view model.
    /// </summary>
    public class StartShinyHuntViewModel : ShinyHunt
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StartShinyHuntViewModel"/> class.
        /// </summary>
        public StartShinyHuntViewModel()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StartShinyHuntViewModel"/> class.
        /// </summary>
        /// <param name="shinyHunt">The shiny hunt.</param>
        public StartShinyHuntViewModel(ShinyHunt shinyHunt)
            : base(shinyHunt)
        {
        }

        /// <summary>
        /// Gets or sets a list of all games.
        /// </summary>
        public List<Game> AllGames { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokemon.
        /// </summary>
        public List<Pokemon> AllPokemon { get; set; }

        /// <summary>
        /// Gets or sets a list of all hunting methods.
        /// </summary>
        public List<HuntingMethod> AllHuntingMethods { get; set; }

        /// <summary>
        /// Gets or sets the application's config.
        /// </summary>
        public AppConfig AppConfig { get; set; }
    }
}
