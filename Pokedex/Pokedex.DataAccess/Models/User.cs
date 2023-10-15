using System;
using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(25)]
        [Display(Name = "Username (Case Sensitive)")]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        [Display(Name = "Is Owner")]
        public bool IsOwner { get; set; }

        [Required]
        [Display(Name = "Is Tester")]
        public bool IsTester { get; set; }

        [Required]
        [Display(Name = "Show Shiny Alternate Forms")]
        public bool ShowShinyAltForms { get; set; }

        [Required]
        [Display(Name = "Show Shiny Gender Differences")]
        public bool ShowShinyGenderDifferences { get; set; }

        [Required]
        [Display(Name = "Hide Captured Shiny Pokemon")]
        public bool HideCapturedShinyPokemon { get; set; }
    }
}