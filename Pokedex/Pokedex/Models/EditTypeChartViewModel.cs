using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class EditTypeChartViewModel
    {
        public List<TypeChart> TypeChart { get; set; }

        public List<Type> Types { get; set; }

        public int TypeId { get; set; }
    }
}