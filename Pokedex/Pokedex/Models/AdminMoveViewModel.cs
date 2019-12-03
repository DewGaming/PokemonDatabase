using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class AdminMoveViewModel
    {
        public List<Move> AllMoves { get; set; }

        public List<Type> AllTypes { get; set; }
    }
}