using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Matriarchy.Data;
using Matriarchy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;

namespace Matriarchy.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ManageServicesController : Controller
    {

        private readonly MvcMatriarchyContext _context;
        private readonly ILogger _logger;
        private readonly int m_intPageSize = 10;

        //**********************************************************************************************************
        // Constructor
        //
        // Set up the Controller and initalize modual level variables,
        //
        // Inputs:
        // MvcMatriarchyContext context     view into the apllication data in the database
        //                             assign it to modual property _context
        //**********************************************************************************************************

        public ManageServicesController(MvcMatriarchyContext context, ILogger<AccountController> logger)
        {
            try
            {
                _context = context;
                _logger = logger;
            }
            catch
            {

            }
        }


        //**********************************************************************************************************
        // List
        //
        // List of the Service Providers
        //
        // Inputs:
        // int v_intPageNumber     what page to move to, of the list of items
        //**********************************************************************************************************
        public async Task<IActionResult> List(int v_intPageNumber = 1)
        {
            try
            {
                if (TempData["v_intPageNumber"] != null) v_intPageNumber = (int)TempData["v_intPageNumber"];

                var l_rsServices = from m in _context.Services select m;

                return View(await PaginatedList<Service>.CreateAsync(l_rsServices, v_intPageNumber, m_intPageSize, 0));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }
        //**********************************************************************************************************
        // Movies
        //
        // List of movies offered by given service provider.  Provides the admins with the ability to update
        // which movies are offered by the provider
        //
        // Inputs:
        // int v_intPageNumber = 1      what page to move to, of the list of items
        // int v_intServiceCD           Service provider filtered on
        // int v_intMovieCD = 0         used for adding a movie to a provider along with service cd
        // string v_strAddRemove = ""   used to determine if we need to add or remove a item to the service provider offerings
        // int v_intMovieServiceID = 0  used for removing an item from the service provider's offerings
        // string v_strTitle = ""       Name of the service provider
        //**********************************************************************************************************
        public async Task<IActionResult> Movies(int v_intServiceCD=0, int v_intPageNumber = 1, int v_intMovieCD = 0, 
                                                string v_strAddRemove = "", int v_intMovieServiceID = 0, string v_strTitle = "")
        {
            try
            {
                if (TempData["v_intPageNumber"] != null) v_intPageNumber = (int)TempData["v_intPageNumber"];
                if (TempData["v_strTitle"] != null) v_strTitle = (string)TempData["v_strTitle"];
                if (TempData["v_intServiceCD"] != null) v_intServiceCD = (int)TempData["v_intServiceCD"];

                if (v_intServiceCD != 0)
                {
                    IQueryable<ServiceMoviesRightJoinMovies> l_rsServiceMovie;

                    // Add or Remove item from Service Provider Offerings
                    if (v_strAddRemove == "Add")
                    {
                        var l_intRowsAffected = _context.Database.ExecuteSqlCommand("Call AddServiceMovie({0},{1})", v_intMovieCD, v_intServiceCD);
                    } //remove serivce provider
                    else if (v_strAddRemove == "Remove")
                    {
                        var l_intRowsAffected = _context.Database.ExecuteSqlCommand("Call RemoveServiceMovie({0})", v_intMovieServiceID);
                    }

                    //refresh list
                    l_rsServiceMovie = from m in _context.ServiceMoviesRightJoinMovies.FromSql("Call ServiceMoviesRightJoinMovies({0})", v_intServiceCD) select m;


                    return View(await PaginatedList<ServiceMoviesRightJoinMovies>.CreateAsync(l_rsServiceMovie.AsNoTracking(), v_intPageNumber, m_intPageSize, v_intServiceCD, v_strTitle));

                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }


        //**********************************************************************************************************
        // Networks
        //
        // List of Networks offered by given service provider.  Provides the admins with the ability to update
        // which movies are offered by the provider
        //
        // Inputs:
        // int v_intPageNumber = 1        what page to move to, of the list of items
        // int v_intServiceCD             Service provider filtered on
        // int v_intNetworkCD = 0         used for adding a network to a provider along with service cd
        // string v_strAddRemove = ""     used to determine if we need to add or remove a item to the service provider offerings
        // int v_intNetworkServiceID = 0  used for removing an item from the service provider's offerings
        // string v_strTitle = ""         Name of the service provider
        //**********************************************************************************************************
        public async Task<IActionResult> Networks(int v_intServiceCD = 0, int v_intPageNumber = 1, int v_intNetworkCD = 0, 
                                                  string v_strAddRemove = "", int v_intNetworkServiceID = 0, string v_strTitle = "")
        {
            try
            {
                if (TempData["v_intPageNumber"] != null) v_intPageNumber = (int)TempData["v_intPageNumber"];
                if (TempData["v_strTitle"] != null) v_strTitle = (string)TempData["v_strTitle"];
                if (TempData["v_intServiceCD"] != null) v_intServiceCD = (int)TempData["v_intServiceCD"];

                if (v_intServiceCD != 0)
                {
                    IQueryable<ServiceNetworksRightJoinNetworks> l_rsServiceNetwork;

                    // Add or Remove item from Service Provider Offerings
                    if (v_strAddRemove == "Add")
                    {
                        var l_intRowsAffected = _context.Database.ExecuteSqlCommand("Call AddServiceNetwork({0},{1})", v_intNetworkCD, v_intServiceCD);
                    }
                    else if (v_strAddRemove == "Remove")
                    {
                        var l_intRowsAffected = _context.Database.ExecuteSqlCommand("Call RemoveServiceNetwork({0})", v_intNetworkServiceID);
                    }
                    //refresh list
                    l_rsServiceNetwork = from m in _context.ServiceNetworksRightJoinNetworks.FromSql("Call ServiceNetworksRightJoinNetworks({0})", v_intServiceCD) select m;

                    return View(await PaginatedList<ServiceNetworksRightJoinNetworks>.CreateAsync(l_rsServiceNetwork.AsNoTracking(), v_intPageNumber, m_intPageSize, v_intServiceCD, v_strTitle));
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }

        }


        //**********************************************************************************************************
        // Series
        //
        // List of Series offered by given service provider.  Provides the admins with the ability to update
        // which movies are offered by the provider
        //
        // Inputs:
        // int v_intPageNumber = 1        what page to move to, of the list of items
        // int v_intServiceCD             Service provider filtered on
        // int v_intSerieCD = 0           NOT USED
        // string v_strAddRemove = ""     used to determine if we need to add or remove a item to the service provider offerings
        // int v_intNetworkServiceID = 0  used for removing an item from the service provider's offerings
        // string v_strTitle = ""         Name of the service provider
        //**********************************************************************************************************
        public async Task<IActionResult> Series(int v_intServiceCD = 0, int v_intPageNumber = 1, int v_intSerieCD = 0, 
                                                string v_strAddRemove = "", int v_intSerieServiceID = 0, string v_strTitle = "")
        {
            try
            {
                if (TempData["v_intPageNumber"] != null) v_intPageNumber = (int)TempData["v_intPageNumber"];
                if (TempData["v_strTitle"] != null) v_strTitle = (string)TempData["v_strTitle"];
                if (TempData["v_intServiceCD"] != null) v_intServiceCD = (int)TempData["v_intServiceCD"];

                if (v_intServiceCD != 0)
                {
                    IQueryable<ServiceSeriesRightJoinSeries> l_rsServiceSerie;

                    // To add a series, we need to go to the edit screen and provide what season will be offered
                    if (v_strAddRemove == "Remove")
                    {
                        var l_intRowsAffected = _context.Database.ExecuteSqlCommand("Call RemoveServiceSerie({0})", v_intSerieServiceID);
                    }

                    // get the updated list of records
                    l_rsServiceSerie = from m in _context.ServiceSeriesRightJoinSeries.FromSql("Call ServiceSeriesRightJoinSeries({0})", v_intServiceCD) select m;

                    return View(await PaginatedList<ServiceSeriesRightJoinSeries>.CreateAsync(l_rsServiceSerie.AsNoTracking(), v_intPageNumber, m_intPageSize, v_intServiceCD, v_strTitle));
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }


        //**********************************************************************************************************
        // SerieAdd
        //
        // Add a new Series to the pool of avaiable series
        //
        // Inputs:
        // string v_strTitle             Name of Title on the page
        // int v_intPageNumber           current page in the list so we can come back to the same spot
        // int v_intServiceCD            The service CD value so we can create the same filter
        // int v_intSerieCD
        // string v_strSeriesName        Name of the Serie
        // int v_intID = 0               ID of record
        // string v_strDescription = ""  Serie Seasons
        // string v_strAction = ""       Prep - setting up the view or Update - processing the change
        //**********************************************************************************************************   
        [HttpPost]
        public async Task<IActionResult> SerieAdd(string v_strTitle, int v_intPageNumber, int v_intServiceCD, int v_intSerieCD = 0, 
                                                  string v_strSeriesName = "",int v_intID = 0, string v_strDescription = "", string v_strAction = "")
        {
            try
            {
                SeasonEdit l_clsSeasonEdit = new SeasonEdit();
                l_clsSeasonEdit.ID = v_intID;
                l_clsSeasonEdit.Description = v_strDescription;
                l_clsSeasonEdit.Title = v_strTitle;
                l_clsSeasonEdit.pageNumber = v_intPageNumber;
                l_clsSeasonEdit.ServiceCD = v_intServiceCD;
                l_clsSeasonEdit.SerieCD = v_intSerieCD;
                l_clsSeasonEdit.SeriesName = v_strSeriesName;


                if (v_strAction == "Prep")
                {
                    return View(l_clsSeasonEdit);

                }
                else if (v_strAction == "Update")
                {
                    try
                    {
                        if (ModelState.IsValid)
                        {
                            var l_clsSeason = new SerieServiceSeason();
                            l_clsSeason.ID = v_intID;
                            l_clsSeason.Description = v_strDescription;
                            l_clsSeason.SerieCD = v_intSerieCD;
                            l_clsSeason.ServiceCD = v_intServiceCD;

                            _context.Update(l_clsSeason);
                            await _context.SaveChangesAsync();

                            //Find the new item we just added
                            IQueryable<ServiceSeriesRightJoinSeries> l_rsServiceSerie;
                            l_rsServiceSerie = from m in _context.ServiceSeriesRightJoinSeries.FromSql("Call ServiceSeriesRightJoinSeries({0})", v_intServiceCD) select m;

                            int i = 0;

                            foreach (ServiceSeriesRightJoinSeries m in l_rsServiceSerie)
                            {
                                i++;
                                if (m.Description == v_strSeriesName)
                                { break; }
                            }

                            // compute the page number
                            v_intPageNumber = ((i - 1) / m_intPageSize) + 1;

                            TempData["v_intPageNumber"] = v_intPageNumber;
                            TempData["v_strTitle"] = v_strTitle;
                            TempData["v_intServiceCD"] = v_intServiceCD;

                            return RedirectToAction("Series");
                        }
                        return View(l_clsSeasonEdit);
                    }
                    catch (DbUpdateConcurrencyException e)
                    {
                        ViewBag.Error = e.InnerException.Message;
                        return View(l_clsSeasonEdit);
                    }
                    catch (DbUpdateException e)
                    {
                        ModelState.AddModelError("", e.InnerException.Message);
                        return View(l_clsSeasonEdit);
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        //**********************************************************************************************************
        // SerieEdit  Prep Edit the Name of the Serie
        //
        //
        // Inputs:
        // string v_strTitle             Name of the Network so we can reset the title to the page
        // int v_intPageNumber           current page in the list so we can come back to the same spot
        // int v_intServiceCD            The service CD value so we can create the same filter
        // int v_intID = 0               ID of record
        // string v_strDescription = ""  Name of Serie
        // string v_strAction = ""       Prep - setting up the view or Update - processing the change
        //**********************************************************************************************************
        [HttpPost]
        public async Task<IActionResult> SerieEdit(string v_strTitle, int v_intPageNumber, int v_intServiceCD,
                                                    int v_intID = 0, string v_strDescription = "", string v_strAction = "")
        {
            try
            {
                //create the object for the edit view
                SerieEdit l_clsSerieEdit = new SerieEdit();
                l_clsSerieEdit.ID = v_intID;
                l_clsSerieEdit.Description = v_strDescription;
                l_clsSerieEdit.Title = v_strTitle;
                l_clsSerieEdit.pageNumber = v_intPageNumber;
                l_clsSerieEdit.ServiceCD = v_intServiceCD;

                if (v_strAction == "Prep")
                {
                    //need and id to edit
                    if (v_intID == 0)
                    {
                        return NotFound();
                    }

                    //check to see if the id exist in the db
                    var l_clsSerie = await _context.Series.FindAsync(v_intID);
                    if (l_clsSerie == null)
                    {
                        return NotFound();
                    }
                    return View(l_clsSerieEdit);
                }
                else if (v_strAction == "Update")
                {
                    try
                    {
                        if (ModelState.IsValid)
                        {
                            Serie l_clsSerie = new Serie();
                            l_clsSerie.ID = v_intID;
                            l_clsSerie.Description = v_strDescription;
                            _context.Update(l_clsSerie);
                            await _context.SaveChangesAsync();

                            //Find the new item we just added
                            IQueryable<ServiceSeriesRightJoinSeries> l_rsServiceSerie;
                            l_rsServiceSerie = from m in _context.ServiceSeriesRightJoinSeries.FromSql("Call ServiceSeriesRightJoinSeries({0})", v_intServiceCD) select m;

                            int i = 0;

                            foreach (ServiceSeriesRightJoinSeries m in l_rsServiceSerie)
                            {
                                i++;
                                if (m.Description == l_clsSerie.Description)
                                { break; }
                            }

                            // compute the page number
                            v_intPageNumber = ((i - 1) / m_intPageSize) + 1;

                            TempData["v_intPageNumber"] = v_intPageNumber;
                            TempData["v_strTitle"] = v_strTitle;
                            TempData["v_intServiceCD"] = v_intServiceCD;
                            return RedirectToAction("Series");
                        }
                        return View(l_clsSerieEdit);
                    }
                    catch (DbUpdateException e)
                    {
                        if ((uint)e.HResult == 0x80131500) //duplicate record
                        {
                            ModelState.AddModelError("", "The serie: " + l_clsSerieEdit.Description + ", is already in the system.");
                        }
                        else
                        {
                            ModelState.AddModelError("", e.InnerException.Message);
                        }
                        return View(l_clsSerieEdit);
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        //**********************************************************************************************************
        // MovieEdit   Edit the Name of the Movie
        //
        // Inputs:
        // string v_strTitle             Name of the Network so we can reset the title to the page
        // int v_intPageNumber           current page in the list so we can come back to the same spot
        // int v_intServiceCD            The service CD value so we can create the same filter
        // int v_intID = 0               ID of record
        // string v_strDescription = ""  Name of Movie
        // string v_strAction = ""       Prep - setting up the view or Update - processing the change
        //**********************************************************************************************************
        [HttpPost]
        public async Task<IActionResult> MovieEdit( string v_strTitle, int v_intPageNumber, int v_intServiceCD,
                                                    int v_intID = 0, string v_strDescription = "", string v_strAction = "")
        {
            try
            {
                //create the object for the edit view
                var l_clsMovieEdit = new MovieEdit();
                l_clsMovieEdit.ID = v_intID;
                l_clsMovieEdit.Description = v_strDescription;
                l_clsMovieEdit.Title = v_strTitle;
                l_clsMovieEdit.pageNumber = v_intPageNumber;
                l_clsMovieEdit.ServiceCD = v_intServiceCD;


                if (v_strAction == "Prep")
                {
                    //need and id to edit
                    if (v_intID == 0)
                    {
                        return NotFound();
                    }

                    //check to see if the id exist in the db
                    var l_clsMovie = await _context.Movies.FindAsync(v_intID);
                    if (l_clsMovie == null)
                    {
                        return NotFound();
                    }

                    //Update field with value from database
                    l_clsMovieEdit.Description = l_clsMovie.Description;

                    return View(l_clsMovieEdit);
                }
                else if (v_strAction == "Update")
                {
                    try
                    {
                        if (ModelState.IsValid)
                        {
                            var l_clsMovie = new Movie();
                            l_clsMovie.ID = v_intID;
                            l_clsMovie.Description = v_strDescription;

                            _context.Update(l_clsMovie);
                            await _context.SaveChangesAsync();

                            //Find the new item we just added
                            IQueryable<ServiceMoviesRightJoinMovies> l_rsServiceMovies;
                            l_rsServiceMovies = from m in _context.ServiceMoviesRightJoinMovies.FromSql("Call ServiceMoviesRightJoinMovies({0})", v_intServiceCD) select m;

                            int i = 0;

                            foreach (ServiceMoviesRightJoinMovies m in l_rsServiceMovies)
                            {
                                i++;
                                if (m.Description == l_clsMovie.Description)
                                { break; }
                            }

                            // compute the page number
                            v_intPageNumber = ((i - 1) / m_intPageSize) + 1;

                            TempData["v_intPageNumber"] = v_intPageNumber;
                            TempData["v_strTitle"] = v_strTitle;
                            TempData["v_intServiceCD"] = v_intServiceCD;
                            return RedirectToAction("Movies");
                        }

                        return View(l_clsMovieEdit);
                    }

                    catch (DbUpdateConcurrencyException e)
                    {
                        ModelState.AddModelError("", e.InnerException.Message);
                        return View(l_clsMovieEdit);
                    }
                    catch (DbUpdateException e)
                    {
                        if ((uint)e.HResult == 0x80131500) //duplicate record
                        {
                            ModelState.AddModelError("", "The movie: " + l_clsMovieEdit.Description + ", is already in the system.");
                        }
                        else
                        {
                            ModelState.AddModelError("", e.InnerException.Message);
                        }
                        return View(l_clsMovieEdit);
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        //**********************************************************************************************************
        // NetworkEdit   Edit the Name of the Network
        //
        //
        // Inputs:
        // string v_strTitle             Name of the Network so we can reset the title to the page
        // int v_intPageNumber           current page in the list so we can come back to the same spot
        // int v_intServiceCD            The service CD value so we can create the same filter
        // int v_intID = 0               ID of record
        // string v_strDescription = ""  Name of Network
        // string v_strAction = ""       Prep - setting up the view or Update - processing the change
        //**********************************************************************************************************
        [HttpPost]
        public async Task<IActionResult> NetworkEdit(string v_strTitle, int v_intPageNumber, int v_intServiceCD,
                                                    int v_intID = 0, string v_strDescription = "", string v_strAction = "")
        {
            try
            {
                //create the object for the edit view
                NetworkEdit l_clsNetworkEdit = new NetworkEdit();
                l_clsNetworkEdit.ID = v_intID;
                l_clsNetworkEdit.Description = v_strDescription;
                l_clsNetworkEdit.Title = v_strTitle;
                l_clsNetworkEdit.pageNumber = v_intPageNumber;
                l_clsNetworkEdit.ServiceCD = v_intServiceCD;

                if (v_strAction == "Prep")
                {
                    //need and id to edit
                    if (v_intID == 0)
                    {
                        return NotFound();
                    }

                    //check to see if the id exist in the db
                    var l_clsNetwork = await _context.Networks.FindAsync(v_intID);
                    if (l_clsNetwork == null)
                    {
                        return NotFound();
                    }

                    //Update field with value from database
                    l_clsNetworkEdit.Description = v_strDescription;

                    return View(l_clsNetworkEdit);
                }
                else if (v_strAction == "Update")
                {
                    try
                    {
                        if (ModelState.IsValid)
                        {
                            var l_clsNetwork = new Network();
                            l_clsNetwork.ID = v_intID;
                            l_clsNetwork.Description = v_strDescription;

                            _context.Update(l_clsNetwork);
                            await _context.SaveChangesAsync();

                            //Find the new item we just added
                            IQueryable<ServiceNetworksRightJoinNetworks> l_rsServiceNetworks;
                            l_rsServiceNetworks = from m in _context.ServiceNetworksRightJoinNetworks.FromSql("Call ServiceNetworksRightJoinNetworks({0})", v_intServiceCD) select m;

                            int i = 0;

                            foreach (ServiceNetworksRightJoinNetworks m in l_rsServiceNetworks)
                            {
                                i++;
                                if (m.Description == l_clsNetwork.Description)
                                { break; }
                            }

                            // compute the page number
                            v_intPageNumber = ((i - 1) / m_intPageSize) + 1;

                            TempData["v_intPageNumber"] = v_intPageNumber;
                            TempData["v_strTitle"] = v_strTitle;
                            TempData["v_intServiceCD"] = v_intServiceCD;

                            return RedirectToAction("Networks");
                        }
                        return View(l_clsNetworkEdit);
                    }
                    catch (DbUpdateConcurrencyException e)
                    {
                        ViewBag.Error = e.InnerException.Message;
                        return View(l_clsNetworkEdit);
                    }
                    catch (DbUpdateException e)
                    {
                        if ((uint)e.HResult == 0x80131500) //duplicate record
                        {
                            ModelState.AddModelError("", "The network: " + l_clsNetworkEdit.Description + ", is already in the system.");
                        }
                        else
                        {
                            ModelState.AddModelError("", e.InnerException.Message);
                        }
                        return View(l_clsNetworkEdit);
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }
        //**********************************************************************************************************
        // SeasonEdit
        //
        // Change the Seasons offered by a given service provider
        //
        // Inputs:
        // string v_strTitle             Name of the Serie so we can reset the title to the page
        // int v_intPageNumber           current page in the list so we can come back to the same spot
        // int v_intServiceCD            The service CD value so we can create the same filter
        // int v_intSerieCD              
        // int v_intID = 0               ID of new record
        // string v_strSeriesName = ""   Name of Series
        // string v_strDescription = ""  Serie Seasons
        // string v_strAction = ""       Prep - setting up the view or Update - processing the change
        //**********************************************************************************************************
        [HttpPost]
        public async Task<IActionResult> SeasonEdit(string v_strTitle, int v_intPageNumber, int v_intServiceCD, int v_intSerieCD = 0,
                                                   string v_strSerieName = "", int v_intID = 0, string v_strDescription = "", string v_strAction = "")
        {
            try
            {
                // Create the object to edit
                var l_clsSeasonEdit = new SeasonEdit();
                l_clsSeasonEdit.ID = v_intID;
                l_clsSeasonEdit.SerieCD = v_intSerieCD;
                l_clsSeasonEdit.ServiceCD = v_intServiceCD;
                l_clsSeasonEdit.Description = v_strDescription;
                l_clsSeasonEdit.Title = v_strTitle;
                l_clsSeasonEdit.pageNumber = v_intPageNumber;
                l_clsSeasonEdit.SeriesName = v_strSerieName;

                if (v_strAction == "Prep")
                {
                    //need and id to edit
                    if (v_intID == 0)
                    {
                        return NotFound();
                    }

                    //check to see if the id exist in the db
                    var l_clsSeasons = await _context.SerieServiceSeasons.FindAsync(v_intID);
                    if (l_clsSeasons == null)
                    {
                        return NotFound();
                    }
                    // Open form to edit 
                    return View(l_clsSeasonEdit);
                }
                else if (v_strAction == "Update")
                {
                    try
                    {
                        if (ModelState.IsValid)
                        {
                            var l_clsSeason = new SerieServiceSeason();
                            l_clsSeason.ID = v_intID;
                            l_clsSeason.Description = v_strDescription;
                            l_clsSeason.SerieCD = v_intSerieCD;
                            l_clsSeason.ServiceCD = v_intServiceCD;


                            _context.Update(l_clsSeason);
                            await _context.SaveChangesAsync();

                            TempData["v_intPageNumber"] = v_intPageNumber;
                            TempData["v_strTitle"] = v_strTitle;
                            TempData["v_intServiceCD"] = v_intServiceCD;

                            return RedirectToAction("Series");

                        }
                        return View(l_clsSeasonEdit);
                    }
                    catch (DbUpdateConcurrencyException e)
                    {
                        ViewBag.Error = e.InnerException.Message;
                        return View(l_clsSeasonEdit);
                    }
                    catch (DbUpdateException e)
                    {
                        ModelState.AddModelError("", e.InnerException.Message);
                        return View(l_clsSeasonEdit);
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }

        }

        //**********************************************************************************************************
        // NetworkCreate 
        //
        // Add a new Network to the pool of networks avaiable
        //
        // Inputs:
        // string v_strTitle             Name of the Network so we can reset the title to the page
        // int v_intPageNumber           current page in the list so we can come back to the same spot
        // int v_intServiceCD            The service CD value so we can create the same filter
        // int v_intID = 0               ID of new record
        // string v_strDescription = ""  Name of Series
        // string v_strAction = ""       Prep - setting up the view or Update - processing the change
        //**********************************************************************************************************
        [HttpPost]
        public async Task<IActionResult> NetworkCreate(string v_strTitle, int v_intPageNumber, int v_intServiceCD,
                                                       int v_intID = 0, string v_strDescription = "", string v_strAction = "")
        {
            try
            {
                NetworkEdit l_clsNetworkEdit = new NetworkEdit();
                l_clsNetworkEdit.pageNumber = v_intPageNumber;
                l_clsNetworkEdit.ServiceCD = v_intServiceCD;
                l_clsNetworkEdit.Title = v_strTitle;
                l_clsNetworkEdit.Description = v_strDescription;

                if (v_strAction == "Prep")
                {
                    return View(l_clsNetworkEdit);
                }
                else if (v_strAction == "Update")
                {
                    try
                    {
                        if (ModelState.IsValid)
                        {
                            var l_clsNetwork = new Network();
                            l_clsNetwork.Description = v_strDescription;

                            _context.Add(l_clsNetwork);
                            await _context.SaveChangesAsync();

                            IQueryable<ServiceNetworksRightJoinNetworks> l_rsServiceNetworks;
                            l_rsServiceNetworks = from m in _context.ServiceNetworksRightJoinNetworks.FromSql("Call ServiceNetworksRightJoinNetworks({0})", v_intServiceCD) select m;

                            int i = 0;
                            foreach (ServiceNetworksRightJoinNetworks m in l_rsServiceNetworks)
                            {
                                i++;
                                if (m.Description == l_clsNetwork.Description)
                                { break; }
                            }

                            v_intPageNumber = ((i - 1) / m_intPageSize) + 1;

                            TempData["v_intPageNumber"] = v_intPageNumber;
                            TempData["v_intServiceCD"] = v_intServiceCD;
                            TempData["v_strTitle"] = v_strTitle;
                            return RedirectToAction("Networks");
                        }
                        return View(l_clsNetworkEdit);
                    }
                    catch (DbUpdateConcurrencyException e)
                    {
                        ViewBag.Error = e.InnerException.Message;
                        return View(l_clsNetworkEdit);
                    }
                    catch (DbUpdateException e)
                    {
                        if ((uint)e.HResult == 0x80131500) //duplicate record
                        {
                            ModelState.AddModelError("", "The network: " + l_clsNetworkEdit.Description + ", is already in the system.");
                        }
                        else
                        {
                            ModelState.AddModelError("", e.InnerException.Message);
                        }
                        return View(l_clsNetworkEdit);
                    }

                }
                else
                {
                    return NotFound();
                }

                
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }
        //**********************************************************************************************************
        // MovieCreate 
        //
        // Add a new movie to the pool of movies avaiable
        //
        // Inputs:
        // string v_strTitle             Name of the Service so we can reset the title to the page
        // int v_intPageNumber           current page in the list so we can come back to the same spot
        // int v_intServiceCD            The service CD value so we can create the same filter
        // int v_intID = 0               ID of new record
        // string v_strDescription = ""  Name of Movie
        // string v_strAction = ""       Prep - setting up the view or Update - processing the change
        //**********************************************************************************************************
        [HttpPost]
        public async Task<IActionResult> MovieCreate(string v_strTitle, int v_intPageNumber, int v_intServiceCD,
                                                       int v_intID = 0, string v_strDescription = "", string v_strAction = "")
        {
            try
            {
                MovieEdit l_clsMovieEdit = new MovieEdit();

                l_clsMovieEdit.pageNumber = v_intPageNumber;
                l_clsMovieEdit.ServiceCD = v_intServiceCD;
                l_clsMovieEdit.Title = v_strTitle;
                l_clsMovieEdit.Description = v_strDescription;

                if (v_strAction == "Prep")
                {
                    return View(l_clsMovieEdit);
                }
                else if (v_strAction == "Update")
                {
                    try
                    {
                        if (ModelState.IsValid)
                        {
                            var l_clsMovie = new Movie();
                            l_clsMovie.Description = v_strDescription;

                            _context.Add(l_clsMovie);
                            await _context.SaveChangesAsync();

                            //Find the new item we just added
                            IQueryable<ServiceMoviesRightJoinMovies> l_rsServiceMovies;
                            l_rsServiceMovies = from m in _context.ServiceMoviesRightJoinMovies.FromSql("Call ServiceMoviesRightJoinMovies({0})", v_intServiceCD) select m;

                            int i = 0;

                            foreach (ServiceMoviesRightJoinMovies m in l_rsServiceMovies)
                            {
                                i++;
                                if (m.Description == l_clsMovie.Description)
                                { break; }
                            }

                            // compute the page number
                            v_intPageNumber = ((i - 1) / m_intPageSize) + 1; ;

                            TempData["v_intPageNumber"] = v_intPageNumber;
                            TempData["v_intServiceCD"] = v_intServiceCD;
                            TempData["v_strTitle"] = v_strTitle;
                            return RedirectToAction("Movies");
                        }
                        return View(l_clsMovieEdit);
                    }
                    catch (DbUpdateConcurrencyException e)
                    {
                        ViewBag.Error = e.InnerException.Message;
                        return View(l_clsMovieEdit);
                    }
                    catch (DbUpdateException e)
                    {
                        if ((uint)e.HResult == 0x80131500) //duplicate record
                        {
                            ModelState.AddModelError("", "The network: " + l_clsMovieEdit.Description + ", is already in the system.");
                        }
                        else
                        {
                            ModelState.AddModelError("", e.InnerException.Message);
                        }
                        return View(l_clsMovieEdit);
                    }
                }
                else
                {
                    return NotFound();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }
        //**********************************************************************************************************
        // SerieCreate
        //
        // Add a new Series to the pool of avaiable series
        //
        // Inputs:
        // string v_strTitle             Name of the Service so we can reset the title to the page
        // int v_intPageNumber           current page in the list so we can come back to the same spot
        // int v_intServiceCD            The service CD value so we can create the same filter
        // int v_intID = 0               ID of new record
        // string v_strDescription = ""  Name of Series
        // string v_strAction = ""       Prep - setting up the view or Update - processing the change
        //**********************************************************************************************************
        [HttpPost]
        public async Task<IActionResult> SerieCreate(string v_strTitle, int v_intPageNumber, int v_intServiceCD,
                                                    int v_intID = 0, string v_strDescription = "", string v_strAction = "")
        {
            try
            {
                SerieEdit l_clsSerieEdit = new SerieEdit();

                l_clsSerieEdit.pageNumber = v_intPageNumber;
                l_clsSerieEdit.ServiceCD = v_intServiceCD;
                l_clsSerieEdit.Title = v_strTitle;
                l_clsSerieEdit.Description = v_strDescription;

                if (v_strAction == "Prep")
                {
                    return View(l_clsSerieEdit);
                }
                else if (v_strAction == "Update")
                {
                    try
                    {
                        if (ModelState.IsValid)
                        {
                            Serie l_clsSerie = new Serie();
                            l_clsSerie.Description = v_strDescription;

                            _context.Add(l_clsSerie);
                            await _context.SaveChangesAsync();

                            //Find the new item we just added
                            IQueryable<ServiceSeriesRightJoinSeries> l_rsServiceSeries;
                            l_rsServiceSeries = from m in _context.ServiceSeriesRightJoinSeries.FromSql("Call ServiceSeriesRightJoinSeries({0})", v_intServiceCD) select m;

                            int i = 0;

                            foreach (ServiceSeriesRightJoinSeries m in l_rsServiceSeries)
                            {
                                i++;
                                if (m.Description == l_clsSerie.Description)
                                { break; }
                            }

                            // compute the page number
                            v_intPageNumber = ((i - 1) / m_intPageSize) + 1;

                            TempData["v_intPageNumber"] = v_intPageNumber;
                            TempData["v_intServiceCD"] = v_intServiceCD;
                            TempData["v_strTitle"] = v_strTitle;
                            return RedirectToAction("Series");
                        }
                        return View(l_clsSerieEdit);
                    }
                    catch (DbUpdateConcurrencyException e)
                    {
                        ViewBag.Error = e.InnerException.Message;
                        return View(l_clsSerieEdit);
                    }
                    catch (DbUpdateException e)
                    {
                        if ((uint)e.HResult == 0x80131500) //duplicate record
                        {
                            ModelState.AddModelError("", "The series: " + l_clsSerieEdit.Description + ", is already in the system.");
                        }
                        else
                        {
                            ModelState.AddModelError("", e.InnerException.Message);
                        }
                        return View(l_clsSerieEdit);
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }
        //**********************************************************************************************************
        // Create    Create new Service Provider
        //
        // int v_intPageNumber           current page in the list so we can come back to the same spot
        // string v_strDescription = ""  Name of the Service Provider
        // v_decPrice                    Price of the Service Provider
        // string v_strAction = ""       Prep - setting up the view or Update - processing the change
        //**********************************************************************************************************        
        [HttpPost]
        public async Task<IActionResult> Create(int v_intPageNumber, string v_strDescription = "", decimal v_decPrice = (decimal)0.0, string v_strAction = "")
        {
            try
            {
                ServiceEdit l_clsServiceEdit = new ServiceEdit();
                l_clsServiceEdit.pageNumber = v_intPageNumber;
                l_clsServiceEdit.Description = v_strDescription;

                if (v_strAction == "Prep")
                {
                    return View(l_clsServiceEdit);
                }
                else if (v_strAction == "Update")
                {
                    try
                    {
                        if (ModelState.IsValid)
                        {
                            Service l_clsService = new Service();
                            l_clsService.Description = v_strDescription;
                            l_clsService.Price = v_decPrice;

                            _context.Add(l_clsService);
                            await _context.SaveChangesAsync();

                            //Find the new item we just added
                            IQueryable<Service> l_rsServices;
                            l_rsServices = from m in _context.Services select m;

                            int i = 0;

                            foreach (Service m in l_rsServices)
                            {
                                i++;
                                if (m.Description == l_clsService.Description)
                                { break; }
                            }

                            // compute the page number
                            v_intPageNumber = ((i - 1) / m_intPageSize) + 1;

                            TempData["v_intPageNumber"] = v_intPageNumber;
                            return RedirectToAction("List");
                        }
                        return View(l_clsServiceEdit);
                    }
                    catch (DbUpdateConcurrencyException e)
                    {
                        ViewBag.Error = e.InnerException.Message;
                        return View(l_clsServiceEdit);
                    }
                    catch (DbUpdateException e)
                    {
                        if ((uint)e.HResult == 0x80131500) //duplicate record
                        {
                            ModelState.AddModelError("", "The service provider: " + l_clsServiceEdit.Description + ", is already in the system.");
                        }
                        else
                        {
                            ModelState.AddModelError("", e.InnerException.Message);
                        }
                        return View(l_clsServiceEdit);
                    }
                }
                else
                {
                    return NotFound();
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        //**********************************************************************************************************
        // Edit      Edit the Service Provider
        //
        // int v_intPageNumber           current page in the list so we can come back to the same spot
        // v_intID                       ID of the item being edited
        // string v_strDescription = ""  Name of the Service Provider
        // v_decPrice                    Price of the Service Provider
        // string v_strAction = ""       Prep - setting up the view or Update - processing the change
        //**********************************************************************************************************
        [HttpPost]
        public async Task<IActionResult> Edit(int v_intPageNumber, int v_intID = 0, string v_strDescription = "", 
                                              decimal v_decPrice = (decimal)0.0, string v_strAction = "")
        {
            try
            {
                //create the object for the edit view
                ServiceEdit l_clsServiceEdit = new ServiceEdit();
                l_clsServiceEdit.ID = v_intID;
                l_clsServiceEdit.Description = v_strDescription;
                l_clsServiceEdit.Price = v_decPrice;
                l_clsServiceEdit.pageNumber = v_intPageNumber;

                if (v_strAction == "Prep")
                {
                    //need and id to edit
                    if (v_intID == 0)
                    {
                        return NotFound();
                    }

                    //check to see if the id exist in the db
                    Service l_clsService = await _context.Services.FindAsync(v_intID);
                    if (l_clsService == null)
                    {
                        return NotFound();
                    }

                    //Update field with value from database
                    l_clsServiceEdit.Description = l_clsService.Description;
                    l_clsServiceEdit.Price = l_clsService.Price;

                    return View(l_clsServiceEdit);
                }
                else if (v_strAction == "Update")
                {
                    try
                    {
                        if (ModelState.IsValid)
                        {
                            Service l_clsService = new Service();
                            l_clsService.Description = v_strDescription;
                            l_clsService.Price = v_decPrice;
                            l_clsService.ID = v_intID;

                            _context.Update(l_clsService);
                            await _context.SaveChangesAsync();

                            //Find the new item we just added
                            IQueryable<Service> l_rsServices;
                            l_rsServices = from m in _context.Services select m;

                            int i = 0;

                            foreach (Service m in l_rsServices)
                            {
                                i++;
                                if (m.Description == l_clsService.Description)
                                { break; }
                            }

                            // compute the page number
                            v_intPageNumber = ((i - 1) / m_intPageSize) + 1;

                            TempData["v_intPageNumber"] = v_intPageNumber;
                            return RedirectToAction("List");
                        }
                        return View(l_clsServiceEdit);
                    }
                    catch (DbUpdateConcurrencyException e)
                    {
                        ViewBag.Error = e.InnerException.Message;
                        return View(l_clsServiceEdit);
                    }
                    catch (DbUpdateException e)
                    {
                        if ((uint)e.HResult == 0x80131500) //duplicate record
                        {
                            ModelState.AddModelError("", "The service: " + l_clsServiceEdit.Description + ", is already in the system.");
                        }
                        else
                        {
                            ModelState.AddModelError("", e.InnerException.Message);
                        }
                        return View(l_clsServiceEdit);
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}