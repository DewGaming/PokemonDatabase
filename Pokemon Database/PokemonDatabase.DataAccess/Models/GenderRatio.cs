using System;
using System.ComponentModel.DataAnnotations;

namespace PokemonDatabase.DataAccess.Models
{
    public class GenderRatio   
    {
        public int Id { get; set; }
        [Required]
        public double MaleRatio { get; set; }
        [Required]
        public double FemaleRatio { get; set; }
    }
}