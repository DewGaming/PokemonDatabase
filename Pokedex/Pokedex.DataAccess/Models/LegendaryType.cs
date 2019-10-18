using System;
using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class LegendaryType   
    {
        [Display(Name = "Type of Legendary")]
        public int Id { get; set; }
        [StringLength(20), Display(Name = "Legendary Name"), Required]
        public string Type { get; set; }
    }
}