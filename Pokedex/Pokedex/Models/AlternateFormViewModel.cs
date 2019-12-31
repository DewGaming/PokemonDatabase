using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class AlternateFormViewModel
    {
        public List<Form> AllForms { get; set; }

        public List<Game> AllGames { get; set; }

        public List<Classification> AllClassifications { get; set; }

        [Display(Name = "Original Pokemon")]
        [Required]
        public int OriginalPokemonId { get; set; }
        public Pokemon OriginalPokemon { get; set; }

        [Display(Name = "Classification")]
        [Required]
        public int ClassificationId { get; set; }

        [Display(Name = "Alternate Form Name")]
        [Required]
        public int FormId { get; set; }

        [Display(Name = "Game(s) of Origin")]
        [Required]
        public int GameId { get; set; }

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