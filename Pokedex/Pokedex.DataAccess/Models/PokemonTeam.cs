using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class PokemonTeam   
    {
        public int Id { get; set; }

        [Required]
        public int? UserId { get; set; }

        public User User { get; set; }

        [Required]
        public string FirstPokemonId { get; set; }
        public Pokemon FirstPokemon { get; set; }

        public string SecondPokemonId { get; set; }
        public Pokemon SecondPokemon { get; set; }

        public string ThirdPokemonId { get; set; }
        public Pokemon ThirdPokemon { get; set; }

        public string FourthPokemonId { get; set; }
        public Pokemon FourthPokemon { get; set; }

        public string FifthPokemonId { get; set; }
        public Pokemon FifthPokemon { get; set; }

        public string SixthPokemonId { get; set; }
        public Pokemon SixthPokemon { get; set; }
    }
}