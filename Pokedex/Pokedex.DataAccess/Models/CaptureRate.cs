using System;
using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class CaptureRate   
    {
        public int Id { get; set; }
        [Required]
        public Int16 CatchRate { get; set; }
        [Required]
        public decimal ChanceOfCapture { get { return Decimal.Round(CatchRate * 100 / 765m, 1); } }
        [Required]
        public bool IsArchived { get; set; }
    }
}