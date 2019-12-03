using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class Move   
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int MoveTypeId { get; set; }
        public Type MoveType { get; set; }
        [Required]
        public byte BasePower { get; set; }
        [Required]
        public byte PP { get; set; }
        public byte Accuracy { get; set; }
    }
}