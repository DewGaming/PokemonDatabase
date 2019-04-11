using System.Collections.Generic;
using PokemonDatabase.DataAccess.Models;

namespace PokemonDatabase.Models
{
    public class PokemonTypingViewModel : PokemonTypeDetail
    {
        public List<Type> AllTypes { get; set; }
    }
}