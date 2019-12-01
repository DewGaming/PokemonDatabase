using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class PokemonTeamDetailViewModel : PokemonTeamDetail
    {
        public PokemonTeamEV EVs { get; set; }

        public PokemonTeamIV IVs { get; set; }

        public PokemonTeamMoveset Moveset { get; set; }

        public AppConfig AppConfig { get; set; }
    }
}