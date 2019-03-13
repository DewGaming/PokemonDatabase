using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PokemonDatabase.DataAccess.Models
{
    public class Ability   
    {
        public int Id { get; set; }
        [StringLength(20)]
        [Required]
        public string Name { get; set; }
        [StringLength(150)]
        [Required]
        public string Description { get; set; }
    }
}