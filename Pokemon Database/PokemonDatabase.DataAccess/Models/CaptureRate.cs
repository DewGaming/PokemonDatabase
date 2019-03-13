using System;
using System.ComponentModel.DataAnnotations;

namespace PokemonDatabase.DataAccess.Models
{
    public class CaptureRate   
    {
        public int Id { get; set; }
        [Required]
        public Int16 CatchRate { get; set; }
        [Required]
        public decimal ChanceOfCapture { get; set; }
    }
}