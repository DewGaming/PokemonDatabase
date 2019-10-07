using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pokedex.DataAccess.Models
{
    public class CaptureRate   
    {
        public int Id { get; set; }
        [Required]
        public Int16 CatchRate { get; set; }
        [Required, NotMapped]
        public decimal ChanceOfCapture { get { return Decimal.Round(CatchRate * 100 / 765m, 1); } }
        [Required]
        public bool IsArchived { get; set; }
    }
}