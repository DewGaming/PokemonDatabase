using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class PokemonBaseHappinessDetail
    {
        public int Id { get; set; }
        [Required, Display(Name = "Base Happiness")]
        public int BaseHappinessId { get; set; }
        public BaseHappiness BaseHappiness { get; set; }
        [Required]
        public int GenerationId { get; set; }
        public Generation Generation { get; set; }
        [Required]
        public int PokemonId { get; set; }
        public Pokemon Pokemon { get; set; }
    }
}