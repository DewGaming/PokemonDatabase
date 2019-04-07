using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PokemonDatabase.DataAccess.Models;

namespace PokemonDatabase.Models
{
    public class LoginViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
    
        [Required]
        [DataType(DataType.Password)]
        public string Password{ get; set; }
    }
}