using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class TypeEffectivenessViewModel
    {
        public List<string> strongAgainst { get; set; }
        public List<string> weakAgainst { get; set; }
        public List<string> immuneTo { get; set; }
    }
}