using System;
using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class ShinyHuntingTechnique   
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [StringLength(5), Required]
        public string Abbreviation { get; set; }
        [Required]
        public string Technique { get; set; }
        [Required]
        public bool IsArchived { get; set; }
    }
}