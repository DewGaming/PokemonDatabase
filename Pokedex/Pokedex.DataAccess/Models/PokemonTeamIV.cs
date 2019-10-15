using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class PokemonTeamIV
    {
        public PokemonTeamIV()
        {
            Health = 31;
            Attack = 31;
            Defense = 31;
            SpecialAttack = 31;
            SpecialDefense = 31;
            Speed = 31;
        }
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