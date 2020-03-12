using System.ComponentModel.DataAnnotations;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the new password view model.
    /// </summary>
    public class NewPasswordViewModel
    {
        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the user's old password.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Old Password")]
        public string OldPassword { get; set; }

        /// <summary>
        /// Gets or sets the user's new password.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [StringLength(255, ErrorMessage = "Password must be between 5 and 255 characters", MinimumLength = 5)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        /// <summary>
        /// Gets or sets the user's new password's confirmation.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword")]
        public string NewPasswordConfirm { get; set; }
    }
}
