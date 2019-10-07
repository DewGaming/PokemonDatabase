using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class PokemonTeamDetail
    {
        public int Id { get; set; }

        public string Nickname { get; set; }

        [Required]
        public bool IsShiny { get; set; }

        [StringLength(6)]
        public string Gender { get; set; }

        [Required]
        public string PokemonId { get; set; }
        public Pokemon Pokemon { get; set; }

        [Required]
        public int? PokemonTypeDetailId { get; set; }

        public PokemonTypeDetail PokemonTypeDetail { get; set; }

        [Required]
        public int? AbilityId { get; set; }

        public Ability Ability { get; set; }

        public int? PokemonTeamEVId { get; set; }

        public PokemonTeamEV PokemonTeamEV { get; set; }

        public int? PokemonTeamIVId { get; set; }

        public PokemonTeamIV PokemonTeamIV { get; set; }
    }
}