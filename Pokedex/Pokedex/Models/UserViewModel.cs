using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class UserViewModel
    {
        public List<User> UserList { get; set; }

        public List<User> UsersWithShinyHunts { get; set; }

        public List<User> UsersWithPokemonTeams { get; set; }
    }
}