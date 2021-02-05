using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Pokedex
{
    /// <summary>
    /// The class that is used to represent the Program.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The method that starts the program.
        /// </summary>
        /// <param name="args">The arguments used for the program.</param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Creates the host builder for the program.
        /// </summary>
        /// <param name="args">The arguments used for the program.</param>
        /// <returns>Returns the created host builder.</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) => Host
            .CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
    }
}
