using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class MarkGameDetail
    {
        public int Id { get; set; }
        [Required]
        public int GameId { get; set; }
        public Game Game { get; set; }
        [Required]
        public int MarkId { get; set; }
        public Mark Mark { get; set; }
    }
}