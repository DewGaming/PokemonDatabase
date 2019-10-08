using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class PokemonTeam
    {
        public int Id { get; set; }
        
        [Required, Display(Name = "Pokemon Team Name")]
        public string PokemonTeamName { get; set; }
        
        [Display(Name = "Origin Game(s) (Optional)")]
        public string GenerationId { get; set; }

        public Generation Generation { get; set; }

        [Required]
        public int? UserId { get; set; }

        public User User { get; set; }

        public int? FirstPokemonId { get; set; }
        public PokemonTeamDetail FirstPokemon { get; set; }

        public int? SecondPokemonId { get; set; }
        public PokemonTeamDetail SecondPokemon { get; set; }

        public int? ThirdPokemonId { get; set; }
        public PokemonTeamDetail ThirdPokemon { get; set; }

        public int? FourthPokemonId { get; set; }
        public PokemonTeamDetail FourthPokemon { get; set; }

        public int? FifthPokemonId { get; set; }
        public PokemonTeamDetail FifthPokemon { get; set; }

        public int? SixthPokemonId { get; set; }
        public PokemonTeamDetail SixthPokemon { get; set; }
    }
}