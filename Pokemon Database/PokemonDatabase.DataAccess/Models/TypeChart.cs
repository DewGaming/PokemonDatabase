using System;
using System.ComponentModel.DataAnnotations;

namespace PokemonDatabase.DataAccess.Models
{
    public class TypeChart
    {
        public int Id { get; set; }
        [Required]
        public decimal Effective { get; set; }
        [Required]
        public Type Attack { get; set; }
        [Required]
        public Type Defend { get; set; }
    }
}