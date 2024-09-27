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
        /// Updates the attributes for a given shiny hunt.
        /// </summary>
        /// <param name="shinyHuntId">The Id of the Shiny Hunt having their encounters updated.</param>
        /// <param name="encounters">The current amount of encounters.</param>
        /// <param name="phases">The current amount of phases.</param>
        /// <param name="increment">The current increment amount.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task UpdateHuntAttributes(int shinyHuntId, int encounters, int phases, int increment)
        {
            await this.Clients.All.SendAsync("SendHuntAttributes", shinyHuntId, encounters, phases, increment);
        }

        /// <summary>
        /// Sends the shiny hunt's pinned status.
        /// </summary>
        /// <param name="shinyHuntId">The Id of the Shiny Hunt having their pin status updated.</param>
        /// <param name="isPinned">If the hunt is pinned.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task UpdatePinStatus(int shinyHuntId, bool isPinned)
        {
            await this.Clients.All.SendAsync("SendPinStatus", shinyHuntId, isPinned);
        }

        /// <summary>
        /// Delete the shiny hunt from the shiny hunt page.
        /// </summary>
        /// <param name="shinyHuntId">The Id of the Shiny Hunt being deleted.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task DeleteShinyHunt(int shinyHuntId)
        {
            await this.Clients.All.SendAsync("RemoveShinyHunt", shinyHuntId);
        }
    }
}
