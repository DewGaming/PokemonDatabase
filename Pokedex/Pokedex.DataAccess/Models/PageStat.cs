using System;
using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class PageStat
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int ViewCount { get; set; }

        [Required]
        public DateTime LastVisit { get; set; }
    }
}