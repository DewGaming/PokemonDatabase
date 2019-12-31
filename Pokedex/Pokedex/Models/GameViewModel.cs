using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class GameViewModel : Game
    {
        public List<Generation> AllGenerations { get; set; }
    }
}