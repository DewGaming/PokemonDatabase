using System;
using System.ComponentModel.DataAnnotations;

namespace PokemonDatabase.DataAccess.Models
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
        [Required]
        public Int16 SpecialAttack { get; set; }
        [Required]
        public Int16 SpecialDefense { get; set; }
        [Required]
        public Int16 Speed { get; set; }
        [Required]
        public Int16 StatTotal { get; set; }
        public Pokemon Pokemon { get; set; }
    }
}