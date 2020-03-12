using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the Send Message without Comment page.
    /// </summary>
    public class MessageViewModel : Message
    {
        /// <summary>
        /// Gets or sets a list of all users.
        /// </summary>
        public List<User> AllUsers { get; set; }
    }
}
