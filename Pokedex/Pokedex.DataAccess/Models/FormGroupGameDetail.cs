using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class FormGroupGameDetail
    {
        public int Id { get; set; }
        [Required]
        public int GameId { get; set; }
        public Game Game { get; set; }
        [Required]
        public int FormGroupId { get; set; }
        public FormGroup FormGroup { get; set; }
    }
}