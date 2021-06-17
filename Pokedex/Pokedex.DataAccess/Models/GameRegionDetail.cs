using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class GameRegionDetail
    {
        public int Id { get; set; }
        [Required]
        public int GameId { get; set; }
        public Game Game { get; set; }
        [Required]
        public int RegionId { get; set; }
        public Region Region { get; set; }
    }
}