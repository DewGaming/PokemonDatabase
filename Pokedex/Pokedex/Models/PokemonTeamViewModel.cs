using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class PokemonTeamViewModel : PokemonTeam
    {
        public AppConfig AppConfig { get; set; }
    }
}