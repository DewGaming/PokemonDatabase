using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the complete pokemon team page view model.
    /// </summary>
    public class EditShinyHuntViewModel : ShinyHunt
    {
        /// <summary>
        /// Gets or sets the pokemon.
        /// </summary>
        public Pokemon PokemonHunted { get; set; }

        /// <summary>
        /// Gets or sets the game.
        /// </summary>
        public Game GameHuntedIn { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokeballs.
        /// </summary>
        public List<Pokeball> AllPokeballs { get; set; }

        /// <summary>
        /// Gets or sets a list of all gender possibilities.
        /// </summary>
        public List<string> AllGenders { get; set; }

        /// <summary>
        /// Gets or sets a list of all marks.
        /// </summary>
        public List<Mark> AllMarks { get; set; }

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
