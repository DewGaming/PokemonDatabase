using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class Comment   
    {
        public int Id { get; set; }
        [StringLength(500), Required, Display(Name = "Comment")]
        public string Name { get; set; }
        [Required]
        public string CommentType { get; set; }
        public string CommentedPage { get; set; }
        public string PokemonName { get; set; }
        public User Commentor { get; set; }
        public int? CommentorId { get; set; }
        [Required]
        public bool IsCompleted { get; set; }
    }
}