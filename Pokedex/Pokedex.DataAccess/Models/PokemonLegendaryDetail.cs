using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class PokemonLegendaryDetail
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Legendary Type")]
        public int LegendaryTypeId { get; set; }
        public LegendaryType LegendaryType { get; set; }
        [Required]
        public int PokemonId { get; set; }
        public Pokemon Pokemon { get; set; }
    }
}