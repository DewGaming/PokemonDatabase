using System;
using System.ComponentModel.DataAnnotations;

namespace PokemonDatabase.DataAccess.Models
{
    public class PokemonFormDetail  
    {
        public int Id { get; set; }
        [Required]
        public Form Form { get; set; }
        [Required]
        public Pokemon OriginalPokemon { get; set; }
        [Required]
        public Pokemon AltFormPokemon { get; set; }
    }
}