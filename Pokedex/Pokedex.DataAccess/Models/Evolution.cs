using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class Evolution
    {
        public int Id { get; set; }
        [StringLength(200), Display(Name = "Evolution Details (If Any)")]
        public string EvolutionDetails { get; set; }
        [Required, Display(Name = "Evolution Methods")]
        public int? EvolutionMethodId { get; set; }
        public EvolutionMethod EvolutionMethod { get; set; }
        [Required, Display(Name = "Pokedex Number of Pre-Evolution")]
        public int PreevolutionPokemonId { get; set; }
        public Pokemon PreevolutionPokemon { get; set; }
        [Required, Display(Name = "Pokedex Number of Evolution")]
        public int EvolutionPokemonId { get; set; }
        public Pokemon EvolutionPokemon { get; set; }
    }
}