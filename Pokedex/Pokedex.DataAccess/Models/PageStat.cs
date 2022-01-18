using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pokedex.DataAccess.Models
{
    public class PageStat
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime VisitTime { get; set; }

        [Column(TypeName="Date")]
        public DateTime VisitDate { get; set; }
    }
}