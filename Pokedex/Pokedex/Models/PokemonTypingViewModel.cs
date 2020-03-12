using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class PokemonTypingViewModel : PokemonTypeDetail
    {
        public List<Type> AllTypes { get; set; }
    }
}
