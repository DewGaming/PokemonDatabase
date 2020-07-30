using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the pokemon view model.
    /// </summary>
    public class PokemonViewModel
    {
        /// <summary>
        /// Gets or sets the pokemon being viewed.
        /// </summary>
        public Pokemon Pokemon { get; set; }

        /// <summary>
        /// Gets or sets the pokemon's form, if applicable.
        /// </summary>
        public Form Form { get; set; }

        /// <summary>
        /// Gets or sets the pokemon's base stats.
        /// </summary>
        public BaseStat BaseStats { get; set; }

        /// <summary>
        /// Gets or sets the pokemon's EV yields.
        /// </summary>
        public List<EVYield> EVYields { get; set; }

        /// <summary>
        /// Gets or sets the pokemon's primary type.
        /// </summary>
        public Type PrimaryType { get; set; }

        /// <summary>
        /// Gets or sets the pokemon's secondary type, if applicable.
        /// </summary>
        public Type SecondaryType { get; set; }

        /// <summary>
        /// Gets or sets the pokemon's primary ability.
        /// </summary>
        public Ability PrimaryAbility { get; set; }

        /// <summary>
        /// Gets or sets the pokemon's secondary ability, if applicable.
        /// </summary>
        public Ability SecondaryAbility { get; set; }

        /// <summary>
        /// Gets or sets the pokemon's hidden ability, if applicable.
        /// </summary>
        public Ability HiddenAbility { get; set; }

        /// <summary>
        /// Gets or sets the pokemon's special event ability, if applicable.
        /// </summary>
        public Ability SpecialEventAbility { get; set; }

        /// <summary>
        /// Gets or sets the pokemon's primary egg group.
        /// </summary>
        public EggGroup PrimaryEggGroup { get; set; }

        /// <summary>
        /// Gets or sets the pokemon's secondary egg group, if applicable.
        /// </summary>
        public EggGroup SecondaryEggGroup { get; set; }

        /// <summary>
        /// Gets or sets the pokemon's pre evolution, if applicable.
        /// </summary>
        public Evolution PreEvolution { get; set; }

        /// <summary>
        /// Gets or sets the pokemon's evolutions, if applicable.
        /// </summary>
        public List<Evolution> Evolutions { get; set; }

        /// <summary>
        /// Gets or sets the pokemon's type chart.
        /// </summary>
        public List<TypeChart> Effectiveness { get; set; }

        /// <summary>
        /// Gets or sets the pokemon that surround the viewed pokemon in the pokedex.
        /// </summary>
        public List<Pokemon> SurroundingPokemon { get; set; }

        /// <summary>
        /// Gets or sets the games this pokemon is available in.
        /// </summary>
        public List<Game> GamesAvailableIn { get; set; }

        /// <summary>
        /// Gets or sets the pokemon before any edits.
        /// </summary>
        public Pokemon OriginalPokemon { get; set; }

        /// <summary>
        /// Gets or sets the application's configuration.
        /// </summary>
        public AppConfig AppConfig { get; set; }
    }
}
