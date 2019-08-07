using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class FormViewModel
    {
        public List<Form> AllForms { get; set; }

        public List<PokemonFormDetail> AllPokemon { get; set; }
    }
}