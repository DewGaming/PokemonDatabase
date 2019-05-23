using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class NewPasswordViewModel
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Old Password")]
        public string OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(255, ErrorMessage = "Password must be between 5 and 255 characters", MinimumLength = 5)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword")]
        public string NewPasswordConfirm { get; set; }
    }
}