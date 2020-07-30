using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pokedex.DataAccess.Models
{
    public class EVYield   
    {
        public int Id { get; set; }
        [Required]
        public Int16 Health { get; set; }
        [Required]
        public Int16 Attack { get; set; }
        [Required]
        public Int16 Defense { get; set; }
        [Display(Name = "Special Attack"), Required]
        public Int16 SpecialAttack { get; set; }
        [Display(Name = "Special Defense"), Required]
        public Int16 SpecialDefense { get; set; }
        [Required]
        public Int16 Speed { get; set; }
        [Required, NotMapped]
        public Int16 EVTotal { get { return (Int16)(Health + Attack + Defense + SpecialAttack + SpecialDefense + Speed); } }
        [Required]
        public int PokemonId { get; set; }
        public Pokemon Pokemon { get; set; }
        [Required]
        public int GenerationId { get; set; }
        public Generation Generation { get; set; }
    }
}