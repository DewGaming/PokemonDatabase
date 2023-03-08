using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MoreLinq;
using Pokedex.DataAccess.Models;
using Pokedex.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Pokedex.Controllers
{
    /// <summary>
    /// The class that handles all ShinyHunt related requests.
    /// </summary>
    [Authorize]
    [Route("")]
    public class ShinyHuntController : Controller
    {
        private readonly DataService dataService;

        private readonly AppConfig appConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShinyHuntController"/> class.
        /// </summary>
        /// <param name="appConfig">The application's configuration.</param>
        /// <param name="dataContext">The pokemon database context.</param>
        public ShinyHuntController(IOptions<AppConfig> appConfig, DataContext dataContext)
        {
            // Instantiate an instance of the data service.
            this.appConfig = appConfig.Value;
            this.dataService = new DataService(dataContext);
        }

        /// <summary>
        /// Starts a shiny hunt.
        /// </summary>
        /// <returns>The shiny hunt page.</returns>
        [HttpGet]
        [Route("start_hunt")]
        public IActionResult StartHunt()
        {
            List<Game> gamesList = this.dataService.GetObjects<Game>("ReleaseDate, Id").Where(x => x.ReleaseDate <= DateTime.Now && x.GenerationId >= 2).ToList();
            List<Game> selectableGames = new List<Game>();
            List<Game> uniqueGames = gamesList.OrderBy(x => x.ReleaseDate).ThenBy(x => x.Id).DistinctBy(y => y.ReleaseDate).ToList();
            for (var i = 0; i < uniqueGames.Count; i++)
            {
                if (uniqueGames[i].Name == "Fire Red")
                {
                    selectableGames.Add(uniqueGames[i]);
                    selectableGames.Add(this.dataService.GetObjectByPropertyValue<Game>("Name", "Leaf Green"));
                }
                else if (i == uniqueGames.Count - 1)
                {
                    selectableGames.Add(new Game()
                    {
                        Id = uniqueGames[i].Id,
                        Name = string.Join(" / ", gamesList.Where(x => x.ReleaseDate >= uniqueGames[i].ReleaseDate).Select(x => x.Name)),
                        GenerationId = uniqueGames[i].GenerationId,
                    });
                }
                else
                {
                    List<Game> games = gamesList.Where(x => x.ReleaseDate >= uniqueGames[i].ReleaseDate && x.ReleaseDate < uniqueGames[i + 1].ReleaseDate && !selectableGames.Any(y => y.ReleaseDate == x.ReleaseDate)).ToList();
                    if (games.Count == 0)
                    {
                        selectableGames.Add(uniqueGames[i]);
                    }
                    else
                    {
                        selectableGames.Add(new Game()
                        {
                            Id = uniqueGames[i].Id,
                            Name = string.Join(" / ", games.ConvertAll(x => x.Name)),
                            GenerationId = uniqueGames[i].GenerationId,
                        });
                    }
                }
            }

            StartShinyHuntViewModel model = new StartShinyHuntViewModel()
            {
                AllGames = selectableGames,
                UserId = this.dataService.GetObjectByPropertyValue<User>("Username", this.User.Identity.Name).Id,
                AppConfig = this.appConfig,
            };

            return this.View(model);
        }

        /// <summary>
        /// Starts a shiny hunt.
        /// </summary>
        /// <param name="shinyHunt">The started shiny hunt.</param>
        /// <returns>The user's shiny hunt page.</returns>
        [HttpPost]
        [Route("start_hunt")]
        public IActionResult StartHunt(StartShinyHuntViewModel shinyHunt)
        {
            if (!this.ModelState.IsValid)
            {
                List<Game> gamesList = this.dataService.GetObjects<Game>("ReleaseDate, Id").Where(x => x.ReleaseDate <= DateTime.Now && x.GenerationId >= 2).ToList();
                List<Game> selectableGames = new List<Game>();
                List<Game> uniqueGames = gamesList.OrderBy(x => x.ReleaseDate).ThenBy(x => x.Id).DistinctBy(y => y.ReleaseDate).ToList();
                for (var i = 0; i < uniqueGames.Count; i++)
                {
                    if (uniqueGames[i].Name == "Fire Red")
                    {
                        selectableGames.Add(uniqueGames[i]);
                        selectableGames.Add(this.dataService.GetObjectByPropertyValue<Game>("Name", "Leaf Green"));
                    }
                    else if (i == uniqueGames.Count - 1)
                    {
                        selectableGames.Add(new Game()
                        {
                            Id = uniqueGames[i].Id,
                            Name = string.Join(" / ", gamesList.Where(x => x.ReleaseDate >= uniqueGames[i].ReleaseDate).Select(x => x.Name)),
                            GenerationId = uniqueGames[i].GenerationId,
                        });
                    }
                    else
                    {
                        List<Game> games = gamesList.Where(x => x.ReleaseDate >= uniqueGames[i].ReleaseDate && x.ReleaseDate < uniqueGames[i + 1].ReleaseDate && !selectableGames.Any(y => y.ReleaseDate == x.ReleaseDate)).ToList();
                        if (games.Count == 0)
                        {
                            selectableGames.Add(uniqueGames[i]);
                        }
                        else
                        {
                            selectableGames.Add(new Game()
                            {
                                Id = uniqueGames[i].Id,
                                Name = string.Join(" / ", games.ConvertAll(x => x.Name)),
                                GenerationId = uniqueGames[i].GenerationId,
                            });
                        }
                    }
                }

                StartShinyHuntViewModel model = new StartShinyHuntViewModel()
                {
                    AllGames = selectableGames,
                    UserId = this.dataService.GetObjectByPropertyValue<User>("Username", this.User.Identity.Name).Id,
                    AppConfig = this.appConfig,
                };

                return this.View(model);
            }

            this.dataService.AddObject(shinyHunt);

            return this.RedirectToAction("ShinyHunts", "User");
        }
    }
}
