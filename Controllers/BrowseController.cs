using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Matriarchy.Data;
using Matriarchy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;



namespace Matriarchy.Controllers
{
    [AllowAnonymous]
    public class BrowseController : Controller
    {

        private readonly MvcMatriarchyContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;
        private int m_intPageSize = 10;

        //**********************************************************************************************************
        // Constructor
        //
        // Set up the Controller and initalize modual level variables,
        //
        // Inputs:
        // MvcMatriarchyContext context                    view into the apllication data in the database
        //                                            assign it to modual property _context
        // UserManager<ApplicationUser> userManager   User for getting User ID and permissions
        //**********************************************************************************************************
        public BrowseController(MvcMatriarchyContext context, UserManager<ApplicationUser> userManager,
                                ILogger<BrowseController> logger)
        {
            try
            {
                _context = context;
                _userManager = userManager;
                _logger = logger;
            }
            catch
            {

            }
        }

        //**********************************************************************************************************
        // Service
        //
        // Get the list of service providers and display them on the web page
        // This page is not paginated, but it may need to be.
        //
        // Inputs: 
        // int v_intPageNumber = 1             Page number for pagination 
        //**********************************************************************************************************
        public async Task<IActionResult> Service(int v_intPageNumber = 1)
        {
            try
            {
                var l_rsServices = from m in _context.Services select m;

                l_rsServices.OrderBy(s => s.Description);

                return View(await PaginatedList<Service>.CreateAsync(l_rsServices.AsNoTracking(), v_intPageNumber, m_intPageSize));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }


        public IActionResult CountyCovid(string v_strCounty)
        {
            try
            {
                ViewBag.v_strCounty = v_strCounty;
                ViewBag.v_strImage = "~/images/Covid/" + v_strCounty + ".png";
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }


        public async Task<IActionResult> Counties(int v_intPageNumber = 1)
        {
            try
            {
                IQueryable<County> l_rsCounties;

                l_rsCounties = from m in _context.Counties.FromSql("Call GetCounties()") select m;

                return View(await PaginatedList<County>.CreateAsync(l_rsCounties.AsNoTracking(), v_intPageNumber, m_intPageSize));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }


        public async Task<IActionResult> Companies(int v_intPageNumber = 1)
        {
            try
            {
                IQueryable<Company> l_rsCompanies;

                //var l_rsCompanies = from m in _context.Companies select m;
                l_rsCompanies = from m in _context.Companies.FromSql("Call GetCompanies()") select m;

                return View(await PaginatedList<Company>.CreateAsync(l_rsCompanies.AsNoTracking(), v_intPageNumber, m_intPageSize));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        public async Task<IActionResult> CompanyPlans(int v_intCompanyID, string v_strTitle, int v_intPageNumber = 1)
        {
            try
            {
                IQueryable<CompanyPlans> l_rsCompanyPlans;

                //var l_rsCompanies = from m in _context.Companies select m;
                l_rsCompanyPlans = from m in _context.Company_Plans.FromSql("Call GetCompanyPlans({0})", v_intCompanyID) select m;

                return View(await PaginatedList<CompanyPlans>.CreateAsync(l_rsCompanyPlans.AsNoTracking(), v_intPageNumber, m_intPageSize));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        public async Task<IActionResult> CountyPlans(int v_intCountyID, string v_strTitle, int v_intPageNumber = 1)
        {
            try
            {
                IQueryable<CountyPlans> l_rsCountyPlans;

                l_rsCountyPlans = from m in _context.County_Plans.FromSql("Call GetCountyPlans({0})", v_intCountyID) select m;

                return View(await PaginatedList<CountyPlans>.CreateAsync(l_rsCountyPlans.AsNoTracking(), v_intPageNumber, m_intPageSize, v_intCountyID, v_strTitle));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        public async Task<IActionResult> PlanDetail(int v_intPlanID, string v_strTitle, int v_intPageNumber = 1)
        {
            try
            {
                IQueryable<PlanDetail> l_rsPlanDetail;

                l_rsPlanDetail = from m in _context.PlanDetails.FromSql("Call GetPlanDetails({0})", v_intPlanID) select m;

                return View(await PaginatedList<PlanDetail>.CreateAsync(l_rsPlanDetail.AsNoTracking(), v_intPageNumber, m_intPageSize, v_intPlanID, v_strTitle));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        //**********************************************************************************************************
        // ServiceMovies
        //
        // Get the list of the Movies provided by the given service provider, if the user is logged in they will be 
        // able to rate the item in the list.
        //
        // Inputs: 
        // int v_intServiceCD                            The service code that is being viewed
        // int v_intPageNumber = 1                       Page number for pagination 
        // int v_intMovieCD = 0                          The movie code used for updating rating
        // int v_intRating = 0                           The value of the rating being updated, 0 mean no update
        // string v_strClear = ""                        Flag for removing the rating from the item
        // string v_strTitle = ""                        Name of the Service provider to display on the page
        // 
        //**********************************************************************************************************
        public async Task<IActionResult> ServiceMovies(int v_intServiceCD, int v_intPageNumber = 1, int v_intMovieCD = 0, 
                                                       int v_intRating = 0, string v_strClear = "", string v_strTitle = "")
        {
            try
            {
                IQueryable<ServiceMovie> l_rsServiceMovie;

                // If the user is logged in they can rate the item
                if (User.Identity.IsAuthenticated)
                {
                    var l_clsUser = await _userManager.FindByEmailAsync(User.Identity.Name);

                    // are they updating a rating  
                    if ((v_intMovieCD != 0) && (v_intRating != 0))
                    {
                        var l_intRowsAffected = _context.Database.ExecuteSqlCommand("Call RateMovie({0},{1},{2})", l_clsUser.UserID, v_intMovieCD, v_intRating);
                    }
                    else if (v_strClear == "Clear")
                    { // Are they clearing a rating
                        var l_intRowsAffected = _context.Database.ExecuteSqlCommand("Call UnRateMovie({0},{1})", l_clsUser.UserID, v_intMovieCD);
                    }
                    // get updated records to show changes
                    l_rsServiceMovie = from m in _context.GetServiceMovies.FromSql("Call GetServiceMoviesFavorites({0}, {1})", v_intServiceCD, l_clsUser.UserID) select m;
                }
                else
                {   // get list to show on the page
                    l_rsServiceMovie = from m in _context.GetServiceMovies.FromSql("Call GetServiceMovies({0})", v_intServiceCD) select m;
                }
                //serviceMovie = serviceMovie.OrderByDescending(m => m.v_intRating);
                return View(await PaginatedList<ServiceMovie>.CreateAsync(l_rsServiceMovie.AsNoTracking(), v_intPageNumber, m_intPageSize, v_intServiceCD, v_strTitle));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        //**********************************************************************************************************
        // ServiceSeries
        //
        // Get the list of the Series provided by the given service provider, if the user is logged in they will be 
        // able to rate the item in the list.
        //
        // Inputs: 
        // int v_intServiceCD                            The service code that is being viewed
        // int v_intPageNumber = 1                       Page number for pagination 
        // int v_intSerieCD = 0                          The Serie code used for updating rating
        // int v_intRating = 0                           The value of the rating being updated, 0 mean no update
        // string v_strClear = ""                        Flag for removing the rating from the item
        // string v_strTitle = ""                        Name of the Service provider to display on the page
        // 
        //**********************************************************************************************************
        public async Task<IActionResult> ServiceSeries(int v_intServiceCD, int v_intPageNumber = 1, int v_intSerieCD = 0, 
                                                       int v_intRating = 0, string v_strClear = "", string v_strTitle = "")
        {
            try
            {
                IQueryable<ServiceSerie> l_rsServiceSerie;

                // If the user is logged in they can rate the item
                if (User.Identity.IsAuthenticated)
                {
                    var l_clsUser = await _userManager.FindByEmailAsync(User.Identity.Name);

                    // are they updating a rating  
                    if ((v_intSerieCD != 0) && (v_intRating != 0))
                    {
                        var l_intRowsAffected = _context.Database.ExecuteSqlCommand("Call RateSerie({0},{1},{2})", l_clsUser.UserID, v_intSerieCD, v_intRating);
                    }
                    else if (v_strClear == "Clear")
                    {// Are they clearing a rating
                        var l_intRowsAffected = _context.Database.ExecuteSqlCommand("Call UnRateSerie({0},{1})", l_clsUser.UserID, v_intSerieCD);
                    }

                    // get updated records to show changes
                    l_rsServiceSerie = from m in _context.GetServiceSeries.FromSql("Call GetServiceSeriesFavorites({0}, {1})", v_intServiceCD, l_clsUser.UserID) select m;

                }
                else
                { // get list to show on the page
                    l_rsServiceSerie = from m in _context.GetServiceSeries.FromSql("Call GetServiceSeries({0})", v_intServiceCD) select m;
                }
                //l_rsServiceSerie = l_rsServiceSerie.OrderByDescending(m => m.Rating);
                return View(await PaginatedList<ServiceSerie>.CreateAsync(l_rsServiceSerie.AsNoTracking(), v_intPageNumber, m_intPageSize, v_intServiceCD, v_strTitle));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }
        //**********************************************************************************************************
        // ServiceNetworks
        //
        // Get the list of the Networks provided by the given service provider, if the user is logged in they will be 
        // able to rate the item in the list.
        //
        // Inputs: 
        // int v_intServiceCD                            The service code that is being viewed
        // int v_intPageNumber = 1                       Page number for pagination 
        // int v_intNetworkCD = 0                        The Network code used for updating rating
        // int v_intRating = 0                           The value of the rating being updated, 0 mean no update
        // string v_strClear = ""                        Flag for removing the rating from the item
        // string v_strTitle = ""                        Name of the Service provider to display on the page
        // 
        //**********************************************************************************************************
        public async Task<IActionResult> ServiceNetworks(int v_intServiceCD, int v_intPageNumber = 1, int v_intNetworkCD = 0,  
                                                         int v_intRating = 0, string v_strClear = "", string v_strTitle = "")
        {
            try
            {
                IQueryable<ServiceNetwork> l_rsNetworks;

                // If the user is logged in they can rate the item
                if (User.Identity.IsAuthenticated)
                {
                    var l_clsUser = await _userManager.FindByEmailAsync(User.Identity.Name);

                    // are they updating a rating  
                    if ((v_intNetworkCD != 0) && (v_intRating != 0))
                    {
                        var l_intRowsAffected = _context.Database.ExecuteSqlCommand("Call RateNetwork({0},{1},{2})", l_clsUser.UserID, v_intNetworkCD, v_intRating);
                    }
                    else if (v_strClear == "Clear")
                    {// Are they clearing a rating
                        var l_intRowsAffected = _context.Database.ExecuteSqlCommand("Call UnRateNetwork({0},{1})", l_clsUser.UserID, v_intNetworkCD);
                    }

                    // get updated records to show changes
                    l_rsNetworks = from m in _context.GetServiceNetworks.FromSql("Call GetServiceNetworksFavorites({0}, {1})", v_intServiceCD, l_clsUser.UserID) select m;

                }
                else
                { // get list to show on the page
                    l_rsNetworks = from m in _context.GetServiceNetworks.FromSql("Call GetServiceNetworks({0})", v_intServiceCD) select m;
                }
                //networks = serviceNetwork.OrderByDescending(m => m.Rating);
                return View(await PaginatedList<ServiceNetwork>.CreateAsync(l_rsNetworks.AsNoTracking(), v_intPageNumber, m_intPageSize, v_intServiceCD, v_strTitle));
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
        // Get the list of the all of the Movies, if the user is logged in they will be 
        // able to rate the item in the list.
        //
        // Inputs: 
        // string v_strSearchString                      The users can filter the list to help find an item
        // int v_intPageNumber = 1                       Page number for pagination 
        // int v_intMovieCD = 0                          The movie code used for updating rating
        // int v_intRating = 0                           The value of the rating being updated, 0 mean no update
        // string v_strClear = ""                        Flag for removing the rating from the item
        // string v_strTitle = ""                        Name of the Service provider to display on the page
        // 
        //**********************************************************************************************************
        public async Task<IActionResult> Movies(string v_strSearchString, int v_intPageNumber = 1, int v_intMovieCD = 0, 
                                                   int v_intRating = 0, string v_strClear = "", string v_strTitle = "", string v_strClearFilter = "")
        {
            try
            {
                IQueryable<MovieRating> l_rsMovies;

                if (v_strClearFilter == "Yes") v_strSearchString = "";

                // If the user is logged in they can rate the item
                if (User.Identity.IsAuthenticated)
                {
                    var l_clsUser = await _userManager.FindByEmailAsync(User.Identity.Name);

                    // are they updating a rating  
                    if ((v_intMovieCD != 0) && (v_intRating != 0))
                    {
                        var l_intRowsAffected = _context.Database.ExecuteSqlCommand("Call RateMovie({0},{1},{2})", l_clsUser.UserID, v_intMovieCD, v_intRating);
                    }
                    else if (v_strClear == "Clear")
                    {
                        var l_intRowsAffected = _context.Database.ExecuteSqlCommand("Call UnRateMovie({0},{1})", l_clsUser.UserID, v_intMovieCD);
                    }

                    // get updated records to show changes
                    l_rsMovies = from m in _context.GetMovieFavorites.FromSql("Call GetMoviesFavorites({0})", l_clsUser.UserID) select m;

                }
                else
                { // get list to show on the page
                    l_rsMovies = from m in _context.GetMovieFavorites.FromSql("Call GetMovies()") select m;
                }

                //Filter the results if the user has provided a filter
                if (!String.IsNullOrEmpty(v_strSearchString))
                {
                    l_rsMovies = l_rsMovies.Where(s => s.Description.ToUpper().Contains(v_strSearchString.ToUpper()));
                    ViewBag.v_strSearchString = v_strSearchString;
                }
                return View(await PaginatedList<MovieRating>.CreateAsync(l_rsMovies.AsNoTracking(), v_intPageNumber, m_intPageSize, v_strSearchString, v_strTitle));
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
        // Get the list of the all of the Series, if the user is logged in they will be 
        // able to rate the item in the list.
        //
        // Inputs: 
        // string v_strSearchString                      The users can filter the list to help find an item
        // int v_intPageNumber = 1                       Page number for pagination 
        // int v_intSerieCD = 0                          The serie code used for updating rating
        // int v_intRating = 0                           The value of the rating being updated, 0 mean no update
        // string v_strClear = ""                        Flag for removing the rating from the item
        // string v_strTitle = ""                        Name of the Service provider to display on the page
        // 
        //**********************************************************************************************************

        public async Task<IActionResult> Series(string v_strSearchString, int v_intPageNumber = 1, int v_intSerieCD = 0, int v_intRating = 0, 
                                                string v_strClear = "", string v_strTitle = "", string v_strClearFilter = "")
        {
            try
            {
                if (v_strClearFilter == "Yes") v_strSearchString = "";

                IQueryable<SerieRating> l_rsSeries;

                // If the user is logged in they can rate the item
                if (User.Identity.IsAuthenticated)
                {
                    var l_clsUser = await _userManager.FindByEmailAsync(User.Identity.Name);

                    // are they updating a rating  
                    if ((v_intSerieCD != 0) && (v_intRating != 0))
                    {
                        var l_intRowsAffected = _context.Database.ExecuteSqlCommand("Call RateSerie({0},{1},{2})", l_clsUser.UserID, v_intSerieCD, v_intRating);
                    }
                    else if (v_strClear == "Clear")
                    {// Are they clearing a rating
                        var l_intRowsAffected = _context.Database.ExecuteSqlCommand("Call UnRateSerie({0},{1})", l_clsUser.UserID, v_intSerieCD);
                    }

                    // get updated records to show changes
                    l_rsSeries = from m in _context.GetSerieFavorites.FromSql("Call GetSeriesFavorites({0})", l_clsUser.UserID) select m;

                }
                else
                { // get list to show on the page
                    l_rsSeries = from m in _context.GetSerieFavorites.FromSql("Call GetSeries()") select m;
                }

                //Filter the results if the user has provided a filter
                if (!String.IsNullOrEmpty(v_strSearchString))
                {
                    l_rsSeries = l_rsSeries.Where(s => s.Description.ToUpper().Contains(v_strSearchString.ToUpper()));
                    ViewBag.v_strSearchString = v_strSearchString;
                }

                return View(await PaginatedList<SerieRating>.CreateAsync(l_rsSeries.AsNoTracking(), v_intPageNumber, m_intPageSize, v_strSearchString, v_strTitle));
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
        // Get the list of the all of the Networks, if the user is logged in they will be 
        // able to rate the item in the list.
        //
        // Inputs: 
        // string v_strSearchString                      The users can filter the list to help find an item
        // int v_intPageNumber = 1                       Page number for pagination 
        // int v_intNetworkCD = 0                       The serie code used for updating rating
        // int v_intRating = 0                           The value of the rating being updated, 0 mean no update
        // string v_strClear = ""                        Flag for removing the rating from the item
        // string v_strTitle = ""                        Name of the Service provider to display on the page
        // 
        //**********************************************************************************************************
        public async Task<IActionResult> Networks(string v_strSearchString, int v_intPageNumber = 1, int v_intNetworkCD = 0, int v_intRating = 0,
                                                string v_strClear = "", string v_strTitle = "", string v_strClearFilter = "")
        {
            try
            {
                if (v_strClearFilter == "Yes") v_strSearchString = "";

                IQueryable<NetworkRating> l_rsNetworks;

                // If the user is logged in they can rate the item
                if (User.Identity.IsAuthenticated)
                {
                    var l_clsUser = await _userManager.FindByEmailAsync(User.Identity.Name);

                    // are they updating a rating  
                    if ((v_intNetworkCD != 0) && (v_intRating != 0))
                    {
                        var l_intRowsAffected = _context.Database.ExecuteSqlCommand("Call RateNetwork({0},{1},{2})", l_clsUser.UserID, v_intNetworkCD, v_intRating);

                    }
                    else if (v_strClear == "Clear")
                    {// Are they clearing a rating
                        var l_intRowsAffected = _context.Database.ExecuteSqlCommand("Call UnRateNetwork({0},{1})", l_clsUser.UserID, v_intNetworkCD);
                    }
                    // get updated records to show changes
                    l_rsNetworks = from m in _context.GetNetworkFavorites.FromSql("Call GetNetworksFavorites({0})", l_clsUser.UserID) select m;
                }
                else
                { // get list to show on the page
                    l_rsNetworks = from m in _context.GetNetworkFavorites.FromSql("Call GetNetworks()") select m;
                }

                //Filter the results if the user has provided a filter
                if (!String.IsNullOrEmpty(v_strSearchString))
                {
                    l_rsNetworks = l_rsNetworks.Where(s => s.Description.ToUpper().Contains(v_strSearchString.ToUpper()));
                    ViewBag.v_strSearchString = v_strSearchString;
                }


                //l_rsNetworks = serviceNetwork.OrderByDescending(m => m.Rating);
                return View(await PaginatedList<NetworkRating>.CreateAsync(l_rsNetworks.AsNoTracking(), v_intPageNumber, m_intPageSize, v_strSearchString, v_strTitle));
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
        // int v_intReturnPage      the page in the list to return to after viewing providers
        // int v_intMovieCD         Item which is being examined, used to add movie to service provider offerings
        // string v_strTitle        Used to display the Movie name on the screen
        //**********************************************************************************************************
        public async Task<IActionResult> MovieProviders(int v_intMovieCD, int v_intReturnPage=1, int v_intPageNumber = 1,  string v_strTitle = "", string v_strSearchString = "")
        {
            try
            {
                //Store the current page from the list in the view bag, so when the provider page returns to the list, it can return to where it came from
                ViewBag.returnPage = v_intReturnPage;
                ViewBag.v_strSearchString = v_strSearchString;
                
                IQueryable<ServiceMoviesRightJoinServices> l_rsMovieProviders;

                //refresh list
                l_rsMovieProviders = from m in _context.ServiceMoviesRightJoinServices.FromSql("Call ServiceMoviesRightJoinServices({0})", v_intMovieCD) select m;

                return View(await PaginatedList<ServiceMoviesRightJoinServices>.CreateAsync(l_rsMovieProviders.AsNoTracking(), v_intPageNumber, m_intPageSize, v_intMovieCD, v_strTitle));
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
        // int v_intReturnPage      the page in the list to return to after viewing providers
        // int v_intNetworkCD       Item which is being examined, used to add network to service provider offerings
        // string v_strTitle        Used to display the Network name on the screen
        //**********************************************************************************************************

        public async Task<IActionResult> NetworkProviders(int v_intNetworkCD, int v_intReturnPage = 1, int v_intPageNumber = 1,  string v_strTitle = "", string v_strSearchString = "")
        {
            try
            {
                //Store the current page from the list in the view bag, so when the provider page returns to the list, it can return to where it came from
                ViewBag.returnPage = v_intReturnPage;
                ViewBag.v_strSearchString = v_strSearchString;

                IQueryable<ServiceNetworksRightJoinServices> l_rsNetworkProviders;

                //refresh list
                l_rsNetworkProviders = from m in _context.ServiceNetworksRightJoinServices.FromSql("Call ServiceNetworksRightJoinServices({0})", v_intNetworkCD) select m;
                return View(await PaginatedList<ServiceNetworksRightJoinServices>.CreateAsync(l_rsNetworkProviders.AsNoTracking(), v_intPageNumber, m_intPageSize, v_intNetworkCD, v_strTitle));
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
        // int v_intReturnPage      the page in the list to return to after viewing providers
        // int v_intSerieCD         Item which is being examined, used to add network to service provider offerings
        // string v_strTitle        Used to display the Network name on the screen
        //**********************************************************************************************************
        public async Task<IActionResult> SerieProviders(int v_intSerieCD, int v_intReturnPage = 1, int v_intPageNumber = 1, string v_strTitle = "", string v_strSearchString = "")
        {
            try
            {
                //Store the current page from the list in the view bag, so when the provider page returns to the list, it can return to where it came from
                ViewBag.returnPage = v_intReturnPage;
                ViewBag.v_strSearchString = v_strSearchString;

                IQueryable<ServiceSeriesRightJoinServices> l_rsSerieProviders;

                //refresh list
                l_rsSerieProviders = from m in _context.ServiceSeriesRightJoinServices.FromSql("Call ServiceSeriesRightJoinServices({0})", v_intSerieCD) select m;

                return View(await PaginatedList<ServiceSeriesRightJoinServices>.CreateAsync(l_rsSerieProviders.AsNoTracking(), v_intPageNumber, m_intPageSize, v_intSerieCD, v_strTitle));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }

        }

    }
}