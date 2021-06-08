using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class PokemonLocationDetail
    {
        public int Id { get; set; }
        [Required]
        public double ChanceOfEncounter { get; set; }
        [Required]
        public int PokemonId { get; set; }
        public Pokemon Pokemon { get; set; }
        [Required]
        public int LocationId { get; set; }
        public Location Location { get; set; }
    }
}