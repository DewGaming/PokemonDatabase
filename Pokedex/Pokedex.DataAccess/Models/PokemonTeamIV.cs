using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class PokemonTeamIV
    {
        public int Id { get; set; }

        [Required]
        public byte Health { get; set; }

        [Required]
        public byte Attack { get; set; }

        [Required]
        public byte Defense { get; set; }

        [Required]
        [Display(Name = "Special Attack")]
        public byte SpecialAttack { get; set; }

        [Required]
        [Display(Name = "Special Defense")]
        public byte SpecialDefense { get; set; }

        [Required]
        public byte Speed { get; set; }
    }
}