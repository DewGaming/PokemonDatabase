namespace Pokedex
{
    /// <summary>
    /// The class that is used to represent the application configuration.
    /// </summary>
    public class AppConfig
    {
        /// <summary>
        /// Gets or sets the application name.
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// Gets or sets the web Url.
        /// </summary>
        public string WebUrl { get; set; }

        /// <summary>
        /// Gets or sets the FTP Url.
        /// </summary>
        public string FTPUrl { get; set; }

        /// <summary>
        /// Gets or sets the pokemon image Url.
        /// </summary>
        public string PokemonImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the pokemon's shiny image Url.
        /// </summary>
        public string ShinyPokemonImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the favicon image Url.
        /// </summary>
        public string FaviconImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the general image Url.
        /// </summary>
        public string GeneralImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the home page image Url.
        /// </summary>
        public string HomePageImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the pokemon image FTP Url.
        /// </summary>
        public string PokemonImageFTPUrl { get; set; }

        /// <summary>
        /// Gets or sets the pokemon's shiny image FTP Url.
        /// </summary>
        public string ShinyPokemonImageFTPUrl { get; set; }

        /// <summary>
        /// Gets or sets the favicon image FTP Url.
        /// </summary>
        public string FaviconImageFTPUrl { get; set; }

        /// <summary>
        /// Gets or sets the FTP server username.
        /// </summary>
        public string FTPUsername { get; set; }

        /// <summary>
        /// Gets or sets the FTP server password.
        /// </summary>
        public string FTPPassword { get; set; }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the email address password.
        /// </summary>
        public string EmailAddressPassword { get; set; }
    }
}
