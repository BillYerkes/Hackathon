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
    //User must be in the Admin role to use this controller
    [Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        
        private readonly MvcMatriarchyContext _context;
        private readonly ILogger _logger;
        private int m_intPageSize = 15;

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
        public ReportsController(MvcMatriarchyContext context, ILogger<ReportsController> logger)
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
        // ServiceRecommendations
        //
        // Generate the report/list of services ordered the best possible match based upon the user perferences,
        // the ratings they may have assigned to movies, series, and networks.
        //
        // Inputs:
        // None
        //**********************************************************************************************************
        public async Task<IActionResult> ServiceRecommendations()
        {
            try
            {
                var l_rsReport = from m in _context.ReportServiceRecommendations.FromSql("Call ReportServiceRecommendations()") select m;

                return View(await l_rsReport.ToListAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        //**********************************************************************************************************
        // UserFavoriteSeries
        //
        // Generate the report/list of the top 10 series, based upon ALL user perferences
        //
        // Inputs: None
        //**********************************************************************************************************
        public async Task<IActionResult> UserFavoriteSeries()
        {
            try
            {
                IQueryable<ReportUserFavorites> l_rsReport;

                ViewBag.Title = "Serie";
                ViewBag.Header = "Top 10 Series";

                l_rsReport = from m in _context.ReportFavoriteMovies.FromSql("Call ReportFavoriteSeries()") select m;

                return View(await l_rsReport.ToListAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        //**********************************************************************************************************
        // UserFavoriteNetworks
        //
        // Generate the report/list of the top 10 networks, based upon ALL user perferences
        //
        // Inputs: None
        //**********************************************************************************************************
        public async Task<IActionResult> UserFavoriteNetworks()
        {
            try
            {
                IQueryable<ReportUserFavorites> l_rsReport;

                ViewBag.Title = "Network";
                ViewBag.Header = "Top 10 Networks";

                l_rsReport = from m in _context.ReportFavoriteMovies.FromSql("Call ReportFavoriteNetworks()") select m;

                return View(await l_rsReport.ToListAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }
        //**********************************************************************************************************
        // UserFavoriteMovies
        //
        // Generate the report/list of the top 10 movies
        //
        // Inputs: None
        //**********************************************************************************************************
        public async Task<IActionResult> UserFavoriteMovies()
        {
            try
            {
                IQueryable<ReportUserFavorites> l_rsReport;

                // put the title and header in the view bag, so it can be displayed on the User Favorites Report Page
                ViewBag.Title = "Movie";
                ViewBag.Header = "Top 10 Movies";

                l_rsReport = from m in _context.ReportFavoriteMovies.FromSql("Call ReportFavoriteMovies()") select m;

                return View(await l_rsReport.ToListAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        //**********************************************************************************************************
        // FailedLoginAttempts
        //
        // Generate the report/list of the failed login attempts grouped by the email address / user id
        //
        // Inputs:
        // int v_intPageNumber = 1       User for pagination of the results
        //**********************************************************************************************************
        public async Task<IActionResult> FailedLoginAttempts(int v_intPageNumber = 1)
        {
            try
            {
                IQueryable<ReportFailedLoginAttempts> l_rsReport;

                l_rsReport = from m in _context.ReportFailedLoginAttempts.FromSql("Call ReportFailedLoginAttempts()") select m;

                return View(await PaginatedList<ReportFailedLoginAttempts>.CreateAsync(l_rsReport.AsNoTracking(), v_intPageNumber, m_intPageSize));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        //**********************************************************************************************************
        // ServiceOfferingCount
        //
        // Generate the report/list of a summary of what each service provider offers, the number of movies,
        // networks, and series
        //
        // Inputs:
        // int v_intPageNumber = 1       User for pagination of the results
        //**********************************************************************************************************

        public async Task<IActionResult> ServiceOfferingCount(int v_intPageNumber = 1)
        {
            try
            {
                IQueryable<ReportServiceOfferingCount> l_rsReport;

                l_rsReport = from m in _context.ReportServiceOfferingCount.FromSql("Call ReportServiceOfferingCount()") select m;

                return View(await PaginatedList<ReportServiceOfferingCount>.CreateAsync(l_rsReport.AsNoTracking(), v_intPageNumber, m_intPageSize));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}