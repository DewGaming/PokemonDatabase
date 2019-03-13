using System;
using System.ComponentModel.DataAnnotations;

namespace PokemonDatabase.DataAccess.Models
{
    public class PokemonAbilityDetail  
    {
        public int Id { get; set; }
        [Required]
        public Pokemon Pokemon { get; set; }
        [Required]
        public Ability PrimaryAbility { get; set; }
        public Ability SecondaryAbility { get; set; }
        public Ability HiddenAbility { get; set; }
    }
}