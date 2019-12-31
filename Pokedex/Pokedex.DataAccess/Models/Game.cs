using System;
using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class Game   
    {
        public int Id { get; set; }
        [StringLength(50)]
        [Required]
        public string Name { get; set; }
        [StringLength(5)]
        [Required]
        public string Abbreviation { get; set; }
        [Display(Name = "Release Date")]
        [Required]
        public DateTime ReleaseDate { get; set; }
        [Required]
        public int GenerationId { get; set; }
        public Generation Generation { get; set; }
    }
}