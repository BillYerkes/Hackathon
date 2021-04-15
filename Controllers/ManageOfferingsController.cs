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
    public class ManageOfferingsController : Controller
    {

        private readonly MvcMatriarchyContext _context;
        private readonly ILogger _logger;
        private int m_intPageSize = 10;
        //**********************************************************************************************************
        // Constructor
        //
        // Set up the Controller and initalize modual level variables,
        //
        // Inputs:
        // MvcMatriarchyContext context     view into the apllication data in the database
        //                             assign it to modual property _context
        //**********************************************************************************************************
        public ManageOfferingsController(MvcMatriarchyContext context, ILogger<ManageOfferingsController> logger)
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
        // Movies
        //
        // List of All movies which can be offered
        //
        // Inputs:
        // int v_intPageNumber     what page to move to, of the list of items
        // string v_strMoveToItem = ""   Used to move back to the item in list after we when to view its providers
        //**********************************************************************************************************
        public async Task<IActionResult> Movies(int v_intPageNumber = 1, string v_strMoveToItem = "")
        {
            try
            {
                if (TempData["v_intPageNumber"] != null) v_intPageNumber = (int)TempData["v_intPageNumber"];

                IQueryable<Movie> l_rsMovies;
                l_rsMovies = from m in _context.Movies.FromSql("Call GetMovies()") select m;


                // Move to the item in the list 
                if (v_strMoveToItem != "")
                {
                    int i = 0;

                    foreach (Movie m in l_rsMovies)
                    {
                        i++;
                        if (m.Description == v_strMoveToItem)
                        { break; }
                    }

                    // compute the page number
                    v_intPageNumber = ((i - 1) / m_intPageSize) + 1;
                }

                return View(await PaginatedList<Movie>.CreateAsync(l_rsMovies.AsNoTracking(), v_intPageNumber, m_intPageSize));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        //**********************************************************************************************************
        // MovieProviders
        //
        // List of Service Providers for a given Movie
        // Also executes the functions of Adding and Removing a Movie from the offersing of a Service Provider
        //
        // Inputs:
        // int v_intPageNumber      what page to move to, of the list of items
        // string AddRemove    Add or Remove the given service provider for this movie
        // int MovieCD         Item which is being examined, used to add movie to service provider offerings
        // int ServiceCD       The service provider to be added 
        // int MovieServiceID  Unique ID for remvoing the movie fromt he service provider's offerings (MovieCD+ServiceCD do not work on delete)
        // string v_strTitle        Used to display the Movie name on the screen
        //**********************************************************************************************************
        [HttpPost]
        public async Task<IActionResult> MovieProviders(int v_intMovieCD = 0, int v_intPageNumber = 1 , int v_intServiceCD = 0, 
                                                        string v_strAddRemove = "", int v_intMovieServiceID = 0, string v_strTitle = "")
        {
            try
            {
                if (TempData["v_intPageNumber"] != null) v_intPageNumber = (int)TempData["v_intPageNumber"];
                if (TempData["v_strTitle"] != null)      v_strTitle = (string)TempData["v_strTitle"];
                if (TempData["v_intMovieCD"] != null)    v_intMovieCD = (int)TempData["v_intMovieCD"];

                if (v_intMovieCD != 0)
                {
                    IQueryable<ServiceMoviesRightJoinServices> l_rsMovieProviders;

                    // Add Provider 
                    if (v_strAddRemove == "Add")
                    {
                        var rowsAffected = _context.Database.ExecuteSqlCommand("Call AddServiceMovie({0},{1})", v_intMovieCD, v_intServiceCD);
                    } // remove provider
                    else if (v_strAddRemove == "Remove")
                    {
                        var rowsAffected = _context.Database.ExecuteSqlCommand("Call RemoveServiceMovie({0})", v_intMovieServiceID);
                    }

                    //refresh list
                    l_rsMovieProviders = from m in _context.ServiceMoviesRightJoinServices.FromSql("Call ServiceMoviesRightJoinServices({0})", v_intMovieCD) select m;

                    return View(await PaginatedList<ServiceMoviesRightJoinServices>.CreateAsync(l_rsMovieProviders.AsNoTracking(), v_intPageNumber, m_intPageSize, v_intMovieCD, v_strTitle));
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
        // List of All networks which can be offered
        //
        // Inputs:
        // int v_intPageNumber           what page to move to, of the list of items
        // string v_strMoveToItem = ""   Used to move back to the item in list after we when to view its providers
        //**********************************************************************************************************
        public async Task<IActionResult> Networks(int v_intPageNumber = 1, string v_strMoveToItem = "")
        {
            try
            {
                if (TempData["v_intPageNumber"] != null) v_intPageNumber = (int)TempData["v_intPageNumber"];

                IQueryable<Network> l_rsNetworks;
                l_rsNetworks = from m in _context.Networks.FromSql("Call GetNetworks()") select m;

                // Move to the item in the list 
                if (v_strMoveToItem != "")
                {
                    int i = 0;

                    foreach (Network m in l_rsNetworks)
                    {
                        i++;
                        if (m.Description == v_strMoveToItem)
                        { break; }
                    }

                    // compute the page number
                    v_intPageNumber = ((i - 1) / m_intPageSize) + 1;
                }


                return View(await PaginatedList<Network>.CreateAsync(l_rsNetworks.AsNoTracking(), v_intPageNumber, m_intPageSize));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        //**********************************************************************************************************
        // NetworkProviders
        //
        // List of Service Providers for a given Network
        // Also executes the functions of Adding and Removing a Network from the offersing of a Service Provider
        //
        // Inputs:
        // int v_intPageNumber      what page to move to, of the list of items
        // string AddRemove    Add or Remove the given service provider for this network
        // int NetworkCD       Item which is being examined, used to add network to service provider offerings
        // int ServiceCD       The service provider to be added 
        // int MovieServiceID  Unique ID for remvoing the movie fromt he service provider's offerings (Network+ServiceCD do not work on delete)
        // string Title        Used to display the Network name on the screen
        //**********************************************************************************************************
        [HttpPost]
        public async Task<IActionResult> NetworkProviders(int v_intNetworkCD = 0, int v_intPageNumber = 1, int v_intServiceCD = 0, 
                                                          string v_strAddRemove = "", int v_intNetworkServiceID = 0, string v_strTitle = "")
        {
            try
            {
                if (TempData["v_intPageNumber"] != null) v_intPageNumber = (int)TempData["v_intPageNumber"];
                if (TempData["v_strTitle"] != null)      v_strTitle = (string)TempData["v_strTitle"];
                if (TempData["v_intNetworkCD"] != null)  v_intNetworkCD = (int)TempData["v_intNetworkCD"];

                if (v_intNetworkCD != 0)
                {
                    IQueryable<ServiceNetworksRightJoinServices> l_rsNetworkProviders;

                    //Add provider
                    if (v_strAddRemove == "Add")
                    {
                        var rowsAffected = _context.Database.ExecuteSqlCommand("Call AddServiceNetwork({0},{1})", v_intNetworkCD, v_intServiceCD);
                    }// remove service provider
                    else if (v_strAddRemove == "Remove")
                    {
                        var rowsAffected = _context.Database.ExecuteSqlCommand("Call RemoveServiceNetwork({0})", v_intNetworkServiceID);
                    }

                    //refresh list
                    l_rsNetworkProviders = from m in _context.ServiceNetworksRightJoinServices.FromSql("Call ServiceNetworksRightJoinServices({0})", v_intNetworkCD) select m;

                    return View(await PaginatedList<ServiceNetworksRightJoinServices>.CreateAsync(l_rsNetworkProviders.AsNoTracking(), v_intPageNumber, m_intPageSize, v_intNetworkCD, v_strTitle));
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
        // List of All series which can be offered
        //
        // Inputs:
        // int v_intPageNumber     what page to move to, of the list of items
        // string v_strMoveToItem = ""   Used to move back to the item in list after we when to view its providers
        //**********************************************************************************************************

        public async Task<IActionResult> Series(int v_intPageNumber = 1, string v_strMoveToItem = "")
        {
            try
            {
                if (TempData["v_intPageNumber"] != null) v_intPageNumber = (int)TempData["v_intPageNumber"];

                IQueryable<Serie> l_rsSeries;
                l_rsSeries = from m in _context.Series.FromSql("Call GetSeries()") select m;

                // Move to the item in the list 
                if (v_strMoveToItem != "")
                {
                    int i = 0;

                    foreach (Serie m in l_rsSeries)
                    {
                        i++;
                        if (m.Description == v_strMoveToItem)
                        { break; }
                    }

                    // compute the page number
                    v_intPageNumber = ((i - 1) / m_intPageSize) + 1;
                }


                return View(await PaginatedList<Serie>.CreateAsync(l_rsSeries.AsNoTracking(), v_intPageNumber, m_intPageSize));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        //**********************************************************************************************************
        // SerieProviders
        //
        // List of Service Providers for a given Series
        // Also executes the functions of Adding and Removing a Serie from the offersing of a Service Provider
        //
        // Inputs:
        // int v_intPageNumber      what page to move to, of the list of items
        // string AddRemove    Add or Remove the given service provider for this serie
        // int SerieCD         Item which is being examined, used to add network to service provider offerings
        // int ServiceCD       The service provider to be added 
        // int SerieServiceID  Unique ID for remvoing the movie fromt he service provider's offerings (SerieCD,ServiceCD do not work on delete)
        // string Title        Used to display the Network name on the screen
        //**********************************************************************************************************
        public async Task<IActionResult> SerieProviders(int v_intSerieCD = 0, int v_intPageNumber = 1, int v_intServiceCD = 0, 
                                                        string v_strAddRemove = "", int v_intSerieServiceID = 0, string v_strTitle = "")
        {
            try
           {
                if (TempData["v_intPageNumber"] != null) v_intPageNumber = (int)TempData["v_intPageNumber"];
                if (TempData["v_strTitle"] != null) v_strTitle = (string)TempData["v_strTitle"];
                if (TempData["v_intSerieCD"] != null) v_intSerieCD = (int)TempData["v_intSerieCD"];

                if (v_intSerieCD != 0)
                {

                    IQueryable<ServiceSeriesRightJoinServices> l_rsSerieProviders;

                    // To add a series, we need to go to the edit screen and provide what season will be offered        
                    //remove serivce provider
                    if (v_strAddRemove == "Remove")
                    {
                        var l_intRowsAffected = _context.Database.ExecuteSqlCommand("Call RemoveServiceSerie({0})", v_intSerieServiceID);
                    }

                    //refresh list
                    l_rsSerieProviders = from m in _context.ServiceSeriesRightJoinServices.FromSql("Call ServiceSeriesRightJoinServices({0})", v_intSerieCD) select m;

                    return View(await PaginatedList<ServiceSeriesRightJoinServices>.CreateAsync(l_rsSerieProviders.AsNoTracking(), v_intPageNumber, m_intPageSize, v_intSerieCD, v_strTitle));
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
                                                  string v_strSeriesName = "", int v_intID = 0, string v_strDescription = "", string v_strAction = "")
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
                            IQueryable<ServiceSeriesRightJoinServices> l_rsSerieProviders;
                            
                            l_rsSerieProviders = from m in _context.ServiceSeriesRightJoinServices.FromSql("Call ServiceSeriesRightJoinServices({0})", v_intSerieCD) select m;

                            int i = 0;

                            foreach (ServiceSeriesRightJoinServices m in l_rsSerieProviders)
                            {
                                i++;
                                if (m.ID == v_intServiceCD)
                                { break; }
                            }

                            // compute the page number
                            v_intPageNumber = ((i - 1) / m_intPageSize) + 1;

                            TempData["v_intPageNumber"] = v_intPageNumber;
                            TempData["v_strTitle"] = v_strTitle;
                            TempData["v_intSerieCD"] = v_intSerieCD;

                            return RedirectToAction("SerieProviders");
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
        // string v_strDescription = ""  Serie Seasons
        // string v_strAction = ""       Prep - setting up the view or Update - processing the change
        //**********************************************************************************************************
        public async Task<IActionResult> SeasonEdit(string v_strTitle, int v_intPageNumber, int v_intServiceCD, int v_intSerieCD,
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
                    //If the id is null we have a problem
                    if (v_intID == 0)
                    {
                        return NotFound();
                    }

                    //populate the object to edit with data from the database
                    var l_clsSeasons = await _context.SerieServiceSeasons.FindAsync(v_intID);

                    //If we cannot find the record in the table, we have a problem
                    if (l_clsSeasons == null)
                    {
                        return NotFound();
                    }

                    // Open form to edit 
                    return View(l_clsSeasonEdit);

                }
                else if (v_strAction == "Update")
                {
                    try //Update the record in the Datbase
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
                            TempData["v_strTitle"] = v_strSerieName; //Title on Serie Provider scree is serie name
                            TempData["v_intSerieCD"] = v_intSerieCD;

                            return RedirectToAction("SerieProviders");
                                                     
                        }
                        return View(l_clsSeasonEdit);
                    }   //If the update failed we need to notife the user
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
        // Add a new network to the pool of networks avaiable
        //
        // int v_intPageNumber           current page in the list so we can come back to the same spot
        // int v_intID = 0               ID of new record
        // string v_strDescription = ""  Name of Series
        // string v_strAction = ""       Prep - setting up the view or Update - processing the change
        //**********************************************************************************************************
        [HttpPost]
        public async Task<IActionResult> NetworkCreate(int v_intPageNumber, string v_strDescription = "",  string v_strAction = "")
        {
            try
            {
                NetworkEdit l_clsNeworkEdit = new NetworkEdit();
                l_clsNeworkEdit.pageNumber = v_intPageNumber;
                l_clsNeworkEdit.Description = v_strDescription;
                if (v_strAction == "Prep")
                {
                    return View(l_clsNeworkEdit);
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

                            IQueryable<Network> l_rsNetworks;
                            l_rsNetworks = from m in _context.Networks.FromSql("Call GetNetworks()") select m;

                            int i = 0;

                            foreach (Network m in l_rsNetworks)
                            {
                                i++;
                                if (m.Description == l_clsNetwork.Description)
                                { break; }
                            }
                            v_intPageNumber = ((i - 1) / m_intPageSize) + 1;

                            TempData["v_intPageNumber"] = v_intPageNumber;
                            return RedirectToAction("Networks");
                        }
                        return View(l_clsNeworkEdit);
                    }
                    catch (DbUpdateConcurrencyException e)
                    {
                        ViewBag.Error = e.InnerException.Message;
                        return View(l_clsNeworkEdit);
                    }
                    catch (DbUpdateException e)
                    {
                        ModelState.AddModelError("", e.InnerException.Message);
                        return View(l_clsNeworkEdit);
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
        // Add a new movie to the pool of movie avaiable
        //
        // int v_intPageNumber           current page in the list so we can come back to the same spot
        // int v_intID = 0               ID of new record
        // string v_strDescription = ""  Name of Movie
        // string v_strAction = ""       Prep - setting up the view or Update - processing the change
        //**********************************************************************************************************
        [HttpPost]
        public async Task<IActionResult> MovieCreate(int v_intPageNumber, string v_strDescription = "", string v_strAction = "")
        {
            try
            {
                MovieEdit l_clsMovieEdit = new MovieEdit();
                l_clsMovieEdit.pageNumber = v_intPageNumber;
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
                            var result = await _context.SaveChangesAsync();

                            // Move to the new item in teh list
                            IQueryable<Movie> movies;
                            movies = from m in _context.Movies.FromSql("Call GetMovies()") select m;

                            int i = 0;
                            // Find the new movie we just added
                            foreach (Movie m in movies)
                            {
                                i++;
                                if (m.Description == l_clsMovieEdit.Description)
                                { break; }
                            }
                            //Compute the page number
                            v_intPageNumber = ((i - 1) / m_intPageSize) + 1;

                            TempData["v_intPageNumber"] = v_intPageNumber;
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
        // SerieCreate 
        //
        // Add a new Series to the pool of avaiable series
        //
        // int v_intPageNumber           current page in the list so we can come back to the same spot
        // int v_intID = 0               ID of new record
        // string v_strDescription = ""  Name of Series
        // string v_strAction = ""       Prep - setting up the view or Update - processing the change
        //**********************************************************************************************************   
        [HttpPost]
        public async Task<IActionResult> SerieCreate(int v_intPageNumber, string v_strDescription = "", string v_strAction = "")
        {
            try
            {
                SerieEdit l_clsSerieEdit = new SerieEdit();
                l_clsSerieEdit.pageNumber = v_intPageNumber;
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
                            IQueryable<Serie> series;
                            series = from m in _context.Series.FromSql("Call GetSeries()") select m;

                            int i = 0;

                            foreach (Serie m in series)
                            {
                                i++;
                                if (m.Description == l_clsSerie.Description)
                                { break; }
                            }

                            // compute the page number
                            v_intPageNumber = ((i - 1) / m_intPageSize) + 1;

                            TempData["v_intPageNumber"] = v_intPageNumber;
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
        // SerieEdit  Prep Edit the Name of the Serie
        //
        //
        // Inputs:
        // int v_intPageNumber           current page in the list so we can come back to the same spot
        // int v_intID = 0               ID of record
        // string v_strDescription = ""  Name of Serie
        // string v_strAction = ""       Prep - setting up the view or Update - processing the change
        //**********************************************************************************************************
        [HttpPost]
        public async Task<IActionResult> SerieEdit(int v_intPageNumber, int v_intID = 0, string v_strDescription = "", string v_strAction = "")
        {
            try
            {
                //create the object for the edit view
                SerieEdit l_clsSerieEdit = new SerieEdit();
                l_clsSerieEdit.ID = v_intID;
                l_clsSerieEdit.Description = v_strDescription;
                l_clsSerieEdit.Title = "";  // not used, need when we are coming from a service provider view
                l_clsSerieEdit.pageNumber = v_intPageNumber;
                l_clsSerieEdit.ServiceCD = 0; // not used, need when we are coming from a service provider view

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
                            IQueryable<Serie> series;
                            series = from m in _context.Series.FromSql("Call GetSeries()") select m;

                            int i = 0;

                            foreach (Serie m in series)
                            {
                                i++;
                                if (m.Description == l_clsSerie.Description)
                                { break; }
                            }

                            // compute the page number
                            v_intPageNumber = ((i - 1) / m_intPageSize) + 1;

                            TempData["v_intPageNumber"] = v_intPageNumber;
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
        // int v_intPageNumber           current page in the list so we can come back to the same spot
        // int v_intID = 0               ID of record
        // string v_strDescription = ""  Name of Movie
        // string v_strAction = ""       Prep - setting up the view or Update - processing the change
        //**********************************************************************************************************
        [HttpPost]
        public async Task<IActionResult> MovieEdit( int v_intPageNumber, int v_intID = 0, string v_strDescription="", string v_strAction="")
        {
            try
            {
                //create the object for the edit view
                MovieEdit l_clsMovieEdit = new MovieEdit();
                l_clsMovieEdit.ID = v_intID;
                l_clsMovieEdit.Description = v_strDescription;
                l_clsMovieEdit.Title = "";   // not needed
                l_clsMovieEdit.pageNumber = v_intPageNumber;
                l_clsMovieEdit.ServiceCD = 0; // not needed

                if (v_strAction == "Prep")
                {
                    //need and id to edit
                    if (v_intID == 0)
                    {
                        return NotFound();
                    }

                    //check to see if the id exist in the db
                    Movie l_clsMovie = await _context.Movies.FindAsync(v_intID);
                    if (l_clsMovie == null)
                    {
                        return NotFound();
                    }

                    //Update field with value from database
                    l_clsMovieEdit.Description = l_clsMovie.Description;
                    
                    return View(l_clsMovieEdit);
                }
                else if(v_strAction == "Update")
                {
                    try
                    {
                        if (ModelState.IsValid)
                        {
                            Movie l_clsMovie = new Movie();
                            l_clsMovie.ID = v_intID;
                            l_clsMovie.Description = v_strDescription;

                            _context.Update(l_clsMovie);
                            await _context.SaveChangesAsync();

                            //Find the new item we just added
                            IQueryable<Movie> l_rsMovies;
                            l_rsMovies = from m in _context.Movies.FromSql("Call GetMovies()") select m;

                            int i = 0;

                            foreach (Movie m in l_rsMovies)
                            {
                                i++;
                                if (m.Description == l_clsMovie.Description)
                                { break; }
                            }

                            // compute the page number
                            v_intPageNumber = ((i - 1) / m_intPageSize) + 1;

                            TempData["v_intPageNumber"] = v_intPageNumber;
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
        // int v_intPageNumber           current page in the list so we can come back to the same spot
        // int v_intID = 0               ID of record
        // string v_strDescription = ""  Name of Network
        // string v_strAction = ""       Prep - setting up the view or Update - processing the change
        //**********************************************************************************************************
        [HttpPost]
        public async Task<IActionResult> NetworkEdit(int v_intPageNumber, int v_intID = 0, string v_strDescription = "", string v_strAction = "")
        {
            try
            {
                //create the object for the edit view
                var l_clsNetworkEdit = new NetworkEdit();
                l_clsNetworkEdit.ID = v_intID;
                l_clsNetworkEdit.Description = v_strDescription;
                l_clsNetworkEdit.Title = "";     // not needed from this view
                l_clsNetworkEdit.pageNumber = v_intPageNumber;
                l_clsNetworkEdit.ServiceCD = 0;  // not needed from this view

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
                            IQueryable<Network> l_rsNetworks;
                            l_rsNetworks = from m in _context.Networks.FromSql("Call GetNetworks()") select m;

                            int i = 0;

                            foreach (Network m in l_rsNetworks)
                            {
                                i++;
                                if (m.Description == l_clsNetwork.Description)
                                { break; }
                            }

                            // compute the page number
                            v_intPageNumber = ((i - 1) / m_intPageSize) + 1;

                            TempData["v_intPageNumber"] = v_intPageNumber;
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
    }
}