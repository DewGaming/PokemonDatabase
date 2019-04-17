using System;
using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class Classification   
    {
        public int Id { get; set; }
        [StringLength(25)]
        [Required]
        public string Name { get; set; }
        [Required]
        public bool IsArchived { get; set; }
    }
}
