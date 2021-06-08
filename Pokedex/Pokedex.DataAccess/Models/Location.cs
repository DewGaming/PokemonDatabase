using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class Location
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int RegionId { get; set; }
        public Region Region { get; set; }
    }
}