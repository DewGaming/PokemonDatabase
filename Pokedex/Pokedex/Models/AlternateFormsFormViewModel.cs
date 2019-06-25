using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class AlternateFormsFormViewModel
    {
        public PokemonFormDetail pokemonFormDetail { get; set; }

        public List<Form> AllForms { get; set; }
    }
}