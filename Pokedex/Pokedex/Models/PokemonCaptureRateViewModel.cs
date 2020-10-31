using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the capture rate view model.
    /// </summary>
    public class PokemonCaptureRateViewModel : PokemonCaptureRateDetail
    {
        /// <summary>
        /// Gets or sets a list of all capture rates.
        /// </summary>
        public List<CaptureRate> AllCaptureRates { get; set; }
    }
}
