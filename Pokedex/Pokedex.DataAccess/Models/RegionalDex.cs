using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class RegionalDex
    {
        public RegionalDex()
        { }

        public RegionalDex(RegionalDex regionalDex)
        {
            Id = regionalDex.Id;
            Name = regionalDex.Name;
            GameId = regionalDex.GameId;
            Game = regionalDex.Game;
        }

        public int Id { get; set; }
        [Required, Display(Name = "Regional Dex Name (Will Show in Pokemon Page)")]
        public string Name { get; set; }
        [Required, Display(Name = "Regional Dex's Game")]
        public int GameId { get; set; }
        public Game Game { get; set; }
    }
}