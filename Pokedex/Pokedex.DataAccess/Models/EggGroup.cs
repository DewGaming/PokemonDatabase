using System;
using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class EggGroup   
    {
        public int Id { get; set; }
        [StringLength(15), Required]
        public string Name { get; set; }
        [Required]
        public bool IsArchived { get; set; }
    }
}