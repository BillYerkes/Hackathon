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



    }
}