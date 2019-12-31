using System;
using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class Generation   
    {
        public int Id { get; set; }
        [Required, StringLength(10)]
        public string Region { get; set; }
    }
}