using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the user view model.
    /// </summary>
    public class UserViewModel
    {
        /// <summary>
        /// Gets or sets the list of users.
        /// </summary>
        public List<User> UserList { get; set; }

        /// <summary>
        /// Gets or sets the list of users with shiny hunts.
        /// </summary>
        public List<User> UsersWithShinyHunts { get; set; }

        /// <summary>
        /// Gets or sets the list of users with pokemon teams.
        /// </summary>
        public List<User> UsersWithPokemonTeams { get; set; }
    }
}
