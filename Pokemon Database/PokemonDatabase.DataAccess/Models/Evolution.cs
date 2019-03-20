using System;
using System.ComponentModel.DataAnnotations;

namespace PokemonDatabase.DataAccess.Models
{
    public class Evolution   
    {
        public int Id { get; set; }
        [StringLength(200)]
        public string EvolutionDetails { get; set; }
        [Required]
        public EvolutionMethod EvolutionMethod { get; set; }
        [Required]
        public Pokemon PreevolutionPokemon { get; set; }
        [Required]
        public Pokemon EvolutionPokemon { get; set; }
    }
}