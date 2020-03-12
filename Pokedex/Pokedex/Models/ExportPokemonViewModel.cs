namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the ajax call to grab a user's team.
    /// </summary>
    public class ExportPokemonViewModel
    {
        /// <summary>
        /// Gets or sets the team id.
        /// </summary>
        public int TeamId { get; set; }

        /// <summary>
        /// Gets or sets the team's export string.
        /// </summary>
        public string ExportString { get; set; }
    }
}
