using System;
using System.ComponentModel.DataAnnotations;
using PokemonDatabase.DataAccess.Models;

namespace PokemonDatabase.DataAccess.Models
{
    public class PokemonTypeDetail  
    {
        public int Id { get; set; }
        [Required]
        public Pokemon Pokemon { get; set; }
        [Required]
        public Type PrimaryType { get; set; }
        public Type SecondaryType { get; set; }
    }
}