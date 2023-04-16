using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent a pokemon's type chart view model.
    /// </summary>
    public class TypeEffectivenessViewModel
    {
        /// <summary>
        /// Gets or sets a list of types strong against the pokemon.
        /// </summary>
        public List<string> StrongAgainst { get; set; }

        /// <summary>
        /// Gets or sets a list of types super strong against the pokemon.
        /// </summary>
        public List<string> SuperStrongAgainst { get; set; }

        /// <summary>
        /// Gets or sets a list of types weak against the pokemon.
        /// </summary>
        public List<string> WeakAgainst { get; set; }

        /// <summary>
        /// Gets or sets a list of types super weak against the pokemon.
        /// </summary>
        public List<string> SuperWeakAgainst { get; set; }

        /// <summary>
        /// Gets or sets a list of types immune to the pokemon.
        /// </summary>
        public List<string> ImmuneTo { get; set; }

        /// <summary>
        /// Gets or sets the application configuration.
        /// </summary>
        public AppConfig AppConfig { get; set; }
    }
}
