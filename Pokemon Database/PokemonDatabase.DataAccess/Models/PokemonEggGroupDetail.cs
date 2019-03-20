using System;
using System.ComponentModel.DataAnnotations;

namespace PokemonDatabase.DataAccess.Models
{
    public class PokemonEggGroupDetail 
    {
        public int Id { get; set; }
        [Required]
        public Pokemon Pokemon { get; set; }
        [Required]
        public EggGroup PrimaryEggGroup { get; set; }
        public EggGroup SecondaryEggGroup { get; set; }
    }
}