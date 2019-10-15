using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class BattleItem
    {
        public int Id { get; set; }
        [StringLength(20), Required]
        public string Name { get; set; }
        [StringLength(500), Required]
        public string Description { get; set; }
    }
}