using System;
using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class BaseStat   
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
        [Required]
        public Int16 StatTotal { get { return (Int16)(Health + Attack + Defense + SpecialAttack + SpecialDefense + Speed); } }
        public string PokemonId { get; set; }
        public Pokemon Pokemon { get; set; }
    }
}