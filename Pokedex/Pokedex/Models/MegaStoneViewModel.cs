using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class FormItemViewModel
    {
        public List<FormItem> AllFormItems { get; set; }

        public List<PokemonTypeDetail> AllPokemon { get; set; }
    }
}