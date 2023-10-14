using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class RegionalDexEntry
    {
        public RegionalDexEntry()
        { }

        public RegionalDexEntry(RegionalDexEntry regionalDexEntry)
        {
            Id = regionalDexEntry.Id;
            RegionalPokedexNumber = regionalDexEntry.RegionalPokedexNumber;
            RegionalDexId = regionalDexEntry.RegionalDexId;
            RegionalDex = regionalDexEntry.RegionalDex;
            PokemonId = regionalDexEntry.PokemonId;
            Pokemon = regionalDexEntry.Pokemon;
        }

        public int Id { get; set; }
        public int RegionalPokedexNumber { get; set; }
        public int RegionalDexId { get; set; }
        public RegionalDex RegionalDex { get; set; }
        public int PokemonId { get; set; }
        public Pokemon Pokemon { get; set; }
    }
}