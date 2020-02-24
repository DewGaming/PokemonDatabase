using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class MessageViewModel : Message
    {
        public List<User> AllUsers { get; set; }
    }
}