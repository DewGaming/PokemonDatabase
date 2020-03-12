using System.ComponentModel.DataAnnotations;

namespace Pokedex.Models
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(25)]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(255, ErrorMessage = "Password must be between 1 and 255 characters", MinimumLength = 1)]

        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password")]
        public string PasswordConfirm { get; set; }
    }
}
