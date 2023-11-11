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
        /// Gets or sets the pokemon's legendary type, if applicable.
        /// </summary>
        public LegendaryType LegendaryType { get; set; }

        /// <summary>
        /// Gets or sets the pokemon's base happinesses.
        /// </summary>
        public List<PokemonBaseHappinessDetail> BaseHappinesses { get; set; }

        /// <summary>
        /// Gets or sets the pokemon's base stats.
        /// </summary>
        public List<BaseStat> BaseStats { get; set; }

        /// <summary>
        /// Gets or sets the pokemon's EV yields.
        /// </summary>
        public List<EVYield> EVYields { get; set; }

        /// <summary>
        /// Gets or sets the pokemon's typings.
        /// </summary>
        public List<PokemonTypeDetail> Typings { get; set; }

        /// <summary>
        /// Gets or sets the pokemon's abilities.
        /// </summary>
        public List<PokemonAbilityDetail> Abilities { get; set; }

        /// <summary>
        /// Gets or sets the pokemon's egg groups.
        /// </summary>
        public List<PokemonEggGroupDetail> EggGroups { get; set; }

        /// <summary>
        /// Gets or sets the pokemon's egg groups.
        /// </summary>
        public List<PokemonCaptureRateDetail> CaptureRates { get; set; }

        /// <summary>
        /// Gets or sets the pokemon's pre evolutions, if applicable.
        /// </summary>
        public List<Evolution> PreEvolutions { get; set; }

        /// <summary>
        /// Gets or sets the pokemon's evolutions, if applicable.
        /// </summary>
        public List<Evolution> Evolutions { get; set; }

        /// <summary>
        /// Gets or sets the pokemon's type chart.
        /// </summary>
        public List<PokemonTypeChartViewModel> Effectiveness { get; set; }

        /// <summary>
        /// Gets or sets the pokemon that surround the viewed pokemon in the pokedex.
        /// </summary>
        public List<Pokemon> SurroundingPokemon { get; set; }

        /// <summary>
        /// Gets or sets the games this pokemon is available in.
        /// </summary>
        public List<Game> GamesAvailableIn { get; set; }

        /// <summary>
        /// Gets or sets the regional dexes this pokemon appears in.
        /// </summary>
        public List<RegionalDex> RegionalDexes { get; set; }

        /// <summary>
        /// Gets or sets the regional dex entries for this pokemon.
        /// </summary>
        public List<RegionalDexEntry> RegionalDexEntries { get; set; }

        /// <summary>
        /// Gets or sets the pokemon before any edits.
        /// </summary>
        public Pokemon OriginalPokemon { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a pokemon has a shiny image.
        /// </summary>
        public bool HasShiny { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a pokemon has a home image.
        /// </summary>
        public bool HasHome { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a pokemon has a female gender difference image.
        /// </summary>
        public bool HasFemaleGenderDifference { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a pokemon has a male gender difference image.
        /// </summary>
        public bool HasMaleGenderDifference { get; set; }

        /// <summary>
        /// Gets or sets the application's configuration.
        /// </summary>
        public AppConfig AppConfig { get; set; }
    }
}
