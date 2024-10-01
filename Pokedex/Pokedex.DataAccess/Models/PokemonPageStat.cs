using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pokedex.DataAccess.Models
{
    public class PokemonPageStat
    {
        public int Id { get; set; }
        [Required]
        public int PokemonId { get; set; }
        public Pokemon Pokemon { get; set; }
        public int? FormId { get; set; }
        public Form Form { get; set; }

        [Required]
        public DateTime VisitTime { get; set; }

        [Column(TypeName="Date")]
        public DateTime VisitDate { get; set; }
    }
}