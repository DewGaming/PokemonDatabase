using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pokedex.DataAccess.Models
{
    public class PokemonTeam
    {
        public int Id { get; set; }
        
        [Required, Display(Name = "Pokemon Team Name")]
        public string PokemonTeamName { get; set; }
        
        [Display(Name = "Origin Game(s) (Optional)")]
        public string GenerationId { get; set; }

        public Generation Generation { get; set; }

        [Required]
        public int? UserId { get; set; }

        public User User { get; set; }

        public int? FirstPokemonId { get; set; }
        public PokemonTeamDetail FirstPokemon { get; set; }

        public int? SecondPokemonId { get; set; }
        public PokemonTeamDetail SecondPokemon { get; set; }

        public int? ThirdPokemonId { get; set; }
        public PokemonTeamDetail ThirdPokemon { get; set; }

        public int? FourthPokemonId { get; set; }
        public PokemonTeamDetail FourthPokemon { get; set; }

        public int? FifthPokemonId { get; set; }
        public PokemonTeamDetail FifthPokemon { get; set; }

        public int? SixthPokemonId { get; set; }
        public PokemonTeamDetail SixthPokemon { get; set; }

        public void InsertPokemon(PokemonTeamDetail pokemon)
        {
            if(this.FirstPokemonId == null)
            {
                this.FirstPokemonId = pokemon.Id;
            }
            else if(this.SecondPokemonId == null)
            {
                this.SecondPokemonId = pokemon.Id;
            }
            else if(this.ThirdPokemonId == null)
            {
                this.ThirdPokemonId = pokemon.Id;
            }
            else if(this.FourthPokemonId == null)
            {
                this.FourthPokemonId = pokemon.Id;
            }
            else if(this.FifthPokemonId == null)
            {
                this.FifthPokemonId = pokemon.Id;
            }
            else if(this.SixthPokemonId == null)
            {
                this.SixthPokemonId = pokemon.Id;
            }
        }

        public List<int> GrabPokemonTeamDetailIds()
        {
            List<int> pokemonTeamDetailIds = new List<int>();
            if(this.FirstPokemonId != null)
            {
                pokemonTeamDetailIds.Add((int)this.FirstPokemonId);
            }

            if(this.SecondPokemonId != null)
            {
                pokemonTeamDetailIds.Add((int)this.SecondPokemonId);
            }

            if(this.ThirdPokemonId != null)
            {
                pokemonTeamDetailIds.Add((int)this.ThirdPokemonId);
            }

            if(this.FourthPokemonId != null)
            {
                pokemonTeamDetailIds.Add((int)this.FourthPokemonId);
            }

            if(this.FifthPokemonId != null)
            {
                pokemonTeamDetailIds.Add((int)this.FifthPokemonId);
            }

            if(this.SixthPokemonId != null)
            {
                pokemonTeamDetailIds.Add((int)this.SixthPokemonId);
            }

            return pokemonTeamDetailIds;
        }

        [NotMapped]
        public List<PokemonTeamDetail> GrabPokemonTeamDetails
        {
            get
            {
                List<PokemonTeamDetail> pokemonTeamDetails = new List<PokemonTeamDetail>();
                if(this.FirstPokemonId != null)
                {
                    pokemonTeamDetails.Add(this.FirstPokemon);
                }

                if(this.SecondPokemonId != null)
                {
                    pokemonTeamDetails.Add(this.SecondPokemon);
                }

                if(this.ThirdPokemonId != null)
                {
                    pokemonTeamDetails.Add(this.ThirdPokemon);
                }

                if(this.FourthPokemonId != null)
                {
                    pokemonTeamDetails.Add(this.FourthPokemon);
                }

                if(this.FifthPokemonId != null)
                {
                    pokemonTeamDetails.Add(this.FifthPokemon);
                }

                if(this.SixthPokemonId != null)
                {
                    pokemonTeamDetails.Add(this.SixthPokemon);
                }

                return pokemonTeamDetails;
            }
        }
    }
}