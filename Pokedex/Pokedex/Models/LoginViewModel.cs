using System.ComponentModel.DataAnnotations;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the Create Account page.
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// Gets or sets the user's username.
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the user's password.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
