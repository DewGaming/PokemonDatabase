using System.Collections.Generic;

namespace Pokedex.Models
{
    public class TypeEffectivenessViewModel
    {
        public List<string> StrongAgainst { get; set; }

        public List<string> WeakAgainst { get; set; }

        public List<string> ImmuneTo { get; set; }
    }
}