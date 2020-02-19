using System.Collections.Generic;

namespace Pokedex
{
    public class AppConfig
    {
        public string AppName { get; set; }

        public string AppVersion { get; set; }

        public string WebUrl { get; set; }

        public string FTPUrl { get; set; }

        public string PokemonImageUrl { get; set; }

        public string FaviconImageUrl { get; set; }

        public string GeneralImageUrl { get; set; }

        public string HomePageImageUrl { get; set; }

        public string PokemonImageFTPUrl { get; set; }

        public string FaviconImageFtpUrl { get; set; }

        public string FTPUsername { get; set; }

        public string FTPPassword { get; set; }

        public string EmailAddress { get; set; }

        public string EmailAddressPassword { get; set; }

        public List<string> CommentCategories { get; set; }

        public List<string> PageCategories { get; set; }
    }
}