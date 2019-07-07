using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class TypingEffectivenessViewModel
    {
        public int TypeId { get; set; }

        public string  TypeName { get; set; }

        public decimal Effectiveness { get; set; }
    }
}