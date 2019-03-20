using System;
using System.ComponentModel.DataAnnotations;

namespace PokemonDatabase.DataAccess.Models
{
    public class Generation   
    {
        [StringLength(4)]
        public string Id { get; set; }
        [StringLength(6)]
        [Required]
        public string Region { get; set; }
        [StringLength(50)]
        [Required]
        public string Games { get; set; }
        [StringLength(5)]
        [Required]
        public string Abbreviation { get; set; }
    }
}