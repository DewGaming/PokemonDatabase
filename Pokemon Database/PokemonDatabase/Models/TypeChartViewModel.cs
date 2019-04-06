using System.Collections.Generic;
using PokemonDatabase.DataAccess.Models;

namespace PokemonDatabase.Models
{
    public class TypeChartViewModel
    {
        public List<TypeChart> typeChart { get; set; }
        public List<Type> types { get; set; }
    }
}