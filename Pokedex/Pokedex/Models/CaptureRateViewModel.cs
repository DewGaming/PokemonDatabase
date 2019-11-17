using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class CaptureRateViewModel
    {
        public List<CaptureRate> AllCaptureRates { get; set; }

        public List<Pokemon> AllPokemon { get; set; }
    }
}