using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class FormItemViewModel : FormItem
    {
        public List<Pokemon> AllPokemon { get; set; }
    }
}