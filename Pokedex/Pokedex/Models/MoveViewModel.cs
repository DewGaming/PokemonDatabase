using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class MoveViewModel : Move
    {
        public List<Type> AllTypes { get; set; }

        public List<MoveCategory> AllMoveCategories { get; set; }
    }
}