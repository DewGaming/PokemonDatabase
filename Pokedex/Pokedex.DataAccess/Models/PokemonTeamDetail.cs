using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class PokemonTeamDetail
    {
        public int Id { get; set; }

        public string Nickname { get; set; }

        [Required]
        public bool IsShiny { get; set; }

        public string Gender { get; set; }

        [Required]
        public byte Level { get; set; }

        [Required]
        public int PokemonId { get; set; }
        public Pokemon Pokemon { get; set; }

        public int? AbilityId { get; set; }

        public Ability Ability { get; set; }

        public string BattleItemString { get; set; }
        
        public int? NatureId { get; set; }

        public Nature Nature { get; set; }

        public int? PokemonTeamEVId { get; set; }

        public PokemonTeamEV PokemonTeamEV { get; set; }

        public int? PokemonTeamIVId { get; set; }

        public PokemonTeamIV PokemonTeamIV { get; set; }

        public int? PokemonTeamMovesetId { get; set; }

        public PokemonTeamMoveset PokemonTeamMoveset { get; set; }

        public int? TeraTypeId { get; set; }

        public Type TeraType { get; set; }
    }
}