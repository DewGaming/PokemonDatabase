using System.Collections.Generic;
using PokemonDatabase.DataAccess.Models;

namespace PokemonDatabase.Models
{
    public class TypeEffectivenessViewModel
    {
        public List<string> strongAgainst { get; set; }
        public List<string> weakAgainst { get; set; }
        public List<string> immuneTo { get; set; }
    }
}