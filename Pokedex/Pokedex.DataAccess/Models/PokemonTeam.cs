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

        public void InsertPokemon(PokemonTeamDetail pokemon)
        {
            if(this.FirstPokemonId == null)
            {
                this.FirstPokemonId = pokemon.Id;
            }
            else if(this.SecondPokemonId == null)
            {
                this.SecondPokemonId = pokemon.Id;
            }
            else if(this.ThirdPokemonId == null)
            {
                this.ThirdPokemonId = pokemon.Id;
            }
            else if(this.FourthPokemonId == null)
            {
                this.FourthPokemonId = pokemon.Id;
            }
            else if(this.FifthPokemonId == null)
            {
                this.FifthPokemonId = pokemon.Id;
            }
            else if(this.SixthPokemonId == null)
            {
                this.SixthPokemonId = pokemon.Id;
            }
        }
    }
}