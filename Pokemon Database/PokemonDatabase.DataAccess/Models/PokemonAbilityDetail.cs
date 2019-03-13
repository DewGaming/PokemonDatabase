using System;
using System.ComponentModel.DataAnnotations;

namespace PokemonDatabase.DataAccess.Models
{
    public class PokemonAbilityDetail  
    {
        public int Id { get; set; }
        [Required]
        public virtual Pokemon Pokemon { get; set; }
        [Required]
        public virtual Ability PrimaryAbility { get; set; }
        public virtual Ability SecondaryAbility { get; set; }
        public virtual Ability HiddenAbility { get; set; }
    }
}