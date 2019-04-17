using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class TypeChartViewModel
    {
        public List<TypeChart> typeChart { get; set; }
        public List<Type> types { get; set; }
    }
}