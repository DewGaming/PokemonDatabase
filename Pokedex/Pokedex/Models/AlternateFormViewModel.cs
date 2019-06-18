using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class AlternateFormViewModel
    {
        public List<Pokemon> AllPokemon { get; set; }

        public List<Form> AllForms { get; set; }

        public List<Generation> AllGenerations { get; set; }

        [Display(Name = "Original Pokemon")]
        [Required]
        public string OriginalPokemonId { get; set; }

        [Display(Name = "Alternate Form Name")]
        [Required]
        public int FormId { get; set; }

        [Display(Name = "Game(s) of Origin")]
        [Required]
        public string GenerationId { get; set; }

        [Display(Name = "Height (In Meters)")]
        [Required]
        public decimal Height { get; set; }

        [Display(Name = "Weight (In Kilograms)")]
        [Required]
        public decimal Weight { get; set; }

        [Display(Name = "Experience Yield")]
        [Required]
        public int ExpYield { get; set; }
    }
}