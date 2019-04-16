using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PokemonDatabase.DataAccess.Models
{
    public class User   
    {
        public int Id { get; set; }

        [Required, Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required, Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required, StringLength(25)]
        public string Username { get; set; }

        [Required, DataType(DataType.EmailAddress), Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required, Display(Name = "Is Owner")]
        public bool IsOwner { get; set; }

        [Required, Display(Name = "Is Admin")]
        public bool IsAdmin { get; set; }

        [Required]
        public bool IsArchived { get; set; }
    }
}