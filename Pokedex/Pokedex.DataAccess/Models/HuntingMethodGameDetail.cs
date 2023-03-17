using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class HuntingMethodGameDetail
    {
        public int Id { get; set; }
        [Required]
        public int GameId { get; set; }
        public Game Game { get; set; }
        [Required]
        public int HuntingMethodId { get; set; }
        public HuntingMethod HuntingMethod { get; set; }
    }
}