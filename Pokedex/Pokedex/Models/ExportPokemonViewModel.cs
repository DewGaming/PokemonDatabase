using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class ExportPokemonViewModel
    {
        public int TeamId { get; set; }

        public string ExportString { get; set; }
    }
}