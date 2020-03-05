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
        public DateTime LastVisit { get; set; }

        [Required]
        [Display(Name = "Is Owner")]
        public bool IsOwner { get; set; }

        [Required]
        [Display(Name = "Is Admin")]
        public bool IsAdmin { get; set; }
    }
}