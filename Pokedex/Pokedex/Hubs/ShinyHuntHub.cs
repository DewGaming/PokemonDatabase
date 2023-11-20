using Microsoft.AspNetCore.SignalR;
using Pokedex.DataAccess.Models;
using System.Threading.Tasks;

namespace Pokedex
{
    /// <summary>
    /// The class that is used to represent the shiny hunt hub.
    /// </summary>
    public class ShinyHuntHub : Hub
    {
        private readonly DataService dataService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShinyHuntHub"/> class.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        public ShinyHuntHub(DataContext dataContext)
        {
            // Instantiate an instance of the data service.
            this.dataService = new DataService(dataContext);
        }

        /// <summary>
        /// Updates the encounter count for a given shiny hunt.
        /// </summary>
        /// <param name="shinyHuntId">The Id of the Shiny Hunt having their encounters updated.</param>
        /// <param name="encounters">The current amount of encounters.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task UpdateEncounters(int shinyHuntId, int encounters)
        {
            await this.Clients.All.SendAsync("SendEncounters", shinyHuntId, encounters);
        }

        /// <summary>
        /// Sends the shiny hunt's pinned status.
        /// </summary>
        /// <param name="shinyHuntId">The Id of the Shiny Hunt having their encounters updated.</param>
        /// <param name="isPinned">If the hunt is pinned.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task UpdatePinStatus(int shinyHuntId, bool isPinned)
        {
            await this.Clients.All.SendAsync("SendPinStatus", shinyHuntId, isPinned);
        }
    }
}
