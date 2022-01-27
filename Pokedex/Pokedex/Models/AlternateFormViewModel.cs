using Pokedex.DataAccess.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the alternate form view model.
    /// </summary>
    public class AlternateFormViewModel
    {
        /// <summary>
        /// Gets or sets the alternate form's weight.
        /// </summary>
        public List<Form> AllForms { get; set; }

        /// <summary>
        /// Gets or sets a list of all games.
        /// </summary>
        public List<Game> AllGames { get; set; }

        /// <summary>
        /// Gets or sets a list of all classifications.
        /// </summary>
        public List<Classification> AllClassifications { get; set; }

        /// <summary>
        /// Gets or sets the alternate form's original pokemon's id.
        /// </summary>
        [Display(Name = "Original Pokemon")]
        [Required]
        public int OriginalPokemonId { get; set; }

        /// <summary>
        /// Gets or sets the alternate form's original pokemon.
        /// </summary>
        public Pokemon OriginalPokemon { get; set; }

        /// <summary>
        /// Gets or sets the alternate form's classification id.
        /// </summary>
        [Display(Name = "Classification")]
        [Required]
        public int ClassificationId { get; set; }

        /// <summary>
        /// Gets or sets the alternate form's form id.
        /// </summary>
        [Display(Name = "Alternate Form Name")]
        [Required]
        public int FormId { get; set; }

        /// <summary>
        /// Gets or sets the alternate form's game of origin.
        /// </summary>
        [Display(Name = "Game(s) of Origin")]
        [Required]
        public int GameId { get; set; }

        /// <summary>
        /// Gets or sets the alternate form's height.
        /// </summary>
        [Display(Name = "Height (In Meters)")]
        [Required]
        public decimal Height { get; set; }

        /// <summary>
        /// Gets or sets the alternate form's weight.
        /// </summary>
        [Display(Name = "Weight (In Kilograms)")]
        [Required]
        public decimal Weight { get; set; }
    }
}
