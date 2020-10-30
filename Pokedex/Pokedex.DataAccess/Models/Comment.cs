using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class Comment   
    {
        public int Id { get; set; }
        [StringLength(500), Required(ErrorMessage = "Comment is required"), Display(Name = "Comment")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Category is required")]
        public int? CategoryId { get; set; }
        public CommentCategory Category { get; set; }
        [Required(ErrorMessage = "Page is required")]
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