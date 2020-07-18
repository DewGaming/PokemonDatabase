using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class Comment   
    {
        public int Id { get; set; }
        [StringLength(500), Required, Display(Name = "Comment")]
        public string Name { get; set; }
        [Required]
        public int? CategoryId { get; set; }
        public CommentCategory Category { get; set; }
        public int? PageId { get; set; }
        public CommentPage Page { get; set; }
        public string PokemonName { get; set; }
        public string OtherPage { get; set; }
        public int? CommentorId { get; set; }
        public User Commentor { get; set; }
        [Required]
        public bool IsCompleted { get; set; }
    }
}