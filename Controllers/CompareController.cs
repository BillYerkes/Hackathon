using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Matriarchy.Data;
using Matriarchy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;

namespace Matriarchy.Controllers
{
    [Authorize(Roles = "Admin,Registered")]
    public class CompareController : Controller
    {

        private readonly MvcMatriarchyContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;
        private readonly int m_intPageSize = 10;

        //**********************************************************************************************************
        // ServiceOfferingCount
        //
        // Generate the report/list of a summary of what each service provider offers, the number of movies,
        // networks, and series
        //
        // Inputs:
        // int v_intPageNumber = 1       User for pagination of the results
        //**********************************************************************************************************
        public CompareController(MvcMatriarchyContext context, UserManager<ApplicationUser> userManager, 
                                 ILogger<ReportsController> logger)
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
        // Compare
        //
        // Generate the report/list of a summary of what each service provider offers, the number of movies,
        // networks, and series
        //
        // Inputs:
        // int v_intPageNumber = 1       User for pagination of the results
        //**********************************************************************************************************
        public async Task<IActionResult> Compare(int v_intPageNumber = 1)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    // Get the current logged in user,
                    var l_clsUser = await _userManager.FindByEmailAsync(User.Identity.Name);

                    // Get the Compare Service record set for the given user
                    var l_rsCompareServices = from m in _context.CompareServices.FromSql("Call CompareServices({0})", l_clsUser.UserID) select m;

                    int l_intRecommendedServiceID = 0;
                    // We just want the first Service ID, which is the one with the highest score
                    foreach (CompareServices m in l_rsCompareServices)
                    {
                        l_intRecommendedServiceID = m.ID;
                        break;
                    }

                    // updated the user profile with the service id of the highest rated service for the user
                    var l_intRowsAffected = _context.Database.ExecuteSqlCommand("Call RecommendedService({0},{1})", l_clsUser.Id, l_intRecommendedServiceID);


                    return View(await PaginatedList<CompareServices>.CreateAsync(l_rsCompareServices.AsNoTracking(), v_intPageNumber, m_intPageSize));
                }
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
            //  The user should be authenticated to get into the controller, this if may not be needed.
        }
    }
}