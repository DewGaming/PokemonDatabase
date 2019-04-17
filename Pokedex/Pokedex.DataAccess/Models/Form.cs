using System;
using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class Form   
    {
        public int Id { get; set; }
        [StringLength(25), Required]
        public string Name { get; set; }
    }
}