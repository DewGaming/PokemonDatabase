using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pokedex.DataAccess.Models
{
    public class PokemonTeamEV
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

        [Required, Display(Name = "Total EVs"), NotMapped]
        public Int16 EVTotal { get { return (Int16)(Health + Attack + Defense + SpecialAttack + SpecialDefense + Speed); } }
    }
}