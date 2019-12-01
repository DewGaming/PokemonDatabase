using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class TypeChart
    {
        public int Id { get; set; }
        [Required]
        public decimal Effective { get; set; }
        [Required]
        public int AttackId { get; set; }
        public Type Attack { get; set; }
        [Required]
        public int DefendId { get; set; }
        public Type Defend { get; set; }
    }
}