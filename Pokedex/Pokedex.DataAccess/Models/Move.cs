using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class Move   
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int GameId { get; set; }
        public Game Game { get; set; }
        [Required]
        public int MoveCategoryId { get; set; }
        public MoveCategory MoveCategory { get; set; }
        [Required]
        public int MoveTypeId { get; set; }
        public Type MoveType { get; set; }
        public byte? BasePower { get; set; }
        [Required]
        public byte PP { get; set; }
        public byte? Accuracy { get; set; }
    }
}