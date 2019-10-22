using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class AdminPokemonViewModel
    {
        public DropdownViewModel DropdownViewModel { get; set; }

        public Pokemon Pokemon { get; set; }
    }
}