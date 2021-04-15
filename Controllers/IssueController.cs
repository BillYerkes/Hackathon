using System;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Matriarchy.Data;
using Matriarchy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Matriarchy.Controllers
{
    public class IssueController : Controller
    {
        private  MvcMatriarchyContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;
        private int m_intPageSize = 10;


        //**********************************************************************************************************
        // IssueController
        //
        //
        // Inputs:   None
        //**********************************************************************************************************

        public IssueController(MvcMatriarchyContext context, UserManager<ApplicationUser> userManager,
                                ILogger<IssueController> logger)
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

        public IActionResult Index()
        {
            return View();
        }

        //**********************************************************************************************************
        // Create
        //
        //
        // Inputs:   None
        //**********************************************************************************************************
        public IActionResult Create()
        {
            return View();
        }

        //**********************************************************************************************************
        // List     List of Issues
        //
        // Inputs:   
        // int v_intUserID = 0                  Filter on user issues
        // int v_intPageNumber = 1              Used for pagination
        //**********************************************************************************************************
        [Authorize(Roles = "Admin,Registered")]
        public async Task<IActionResult> UserIssues(int v_intUserID = 0, int v_intPageNumber = 1, int v_intManageUserReturnPage = 0, string v_strManageuserSearchString = "")
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {// user needs to be logged in
                    IQueryable<GetAllIssues>   l_rsIssues;
                    var l_clsUser = await _userManager.FindByEmailAsync(User.Identity.Name);
                    ViewBag.v_intUserID = v_intUserID;
                    ViewBag.UserEmail = l_clsUser.Email;
                    ViewBag.v_intManageUserReturnPage = v_intManageUserReturnPage;
                    ViewBag.v_strManageuserSearchString = v_strManageuserSearchString;

                    //if the user is an admin they can see anyones issues, else they can only see their own issues
                    if (User.IsInRole("Admin"))
                    {
                        if (v_intUserID == 0) v_intUserID = l_clsUser.UserID;
                        l_rsIssues = from m in _context.GetAllIssues.FromSql("Call GetUserIssues({0})", v_intUserID) select m;
                    }
                    else
                        l_rsIssues = from m in _context.GetAllIssues.FromSql("Call GetUserIssues({0})", l_clsUser.UserID) select m;

                    return View(await PaginatedList<GetAllIssues>.CreateAsync(l_rsIssues, v_intPageNumber, m_intPageSize));
                }
                else
                { //should not get here but just in case
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
        // AllIssues     List of All Issues
        //
        // Inputs:   
        // int v_intPageNumber = 1              Used for pagination
        //**********************************************************************************************************
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AllIssues(int v_intPageNumber = 1)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {// user needs to be logged in
                    IQueryable<GetAllIssues> l_rsIssues;
                    var l_clsUser = await _userManager.FindByEmailAsync(User.Identity.Name);

                    if (User.IsInRole("Admin"))
                    { //if we want all the issues and the user is an admin
                        l_rsIssues = from m in _context.GetAllIssues.FromSql("Call GetAllIssues()") select m;
                    }
                    else
                    { //should not get here but just in case
                        return NotFound();
                    }
                    return View(await PaginatedList<GetAllIssues>.CreateAsync(l_rsIssues, v_intPageNumber, m_intPageSize));
                }
                else
                { //should not get here but just in case
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
        // Create
        //
        //
        // Inputs:   None
        //**********************************************************************************************************
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserCd, IssueDateTime, Subject, Description ")] Issue v_clsIssue )
        {
            try
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (User.Identity.IsAuthenticated)
                        {
                            var l_clsUser = await _userManager.FindByEmailAsync(User.Identity.Name);
                            v_clsIssue.UserCD = l_clsUser.UserID;
                            v_clsIssue.IssueDateTime = DateTime.Now; //default value in mysql not working
                        }
                        else
                        {
                            v_clsIssue.UserCD = 0;
                            v_clsIssue.IssueDateTime = DateTime.Now; //default value in mysql not working
                        }

                            _context.Add(v_clsIssue);
                        await _context.SaveChangesAsync();
                        return View("ThankYou");
                    }
                    return View(v_clsIssue);
                }
                catch (DbUpdateConcurrencyException e)
                {
                    ViewBag.Error = e.InnerException.Message;
                    return View(v_clsIssue);
                }
                catch (DbUpdateException e)
                {
                    ModelState.AddModelError("", e.InnerException.Message);
                    return View(v_clsIssue);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        //**********************************************************************************************************
        // Details
        //
        //
        // Inputs:   None
        //**********************************************************************************************************


        public async Task<IActionResult> Details(int? v_intIssueID, int v_intReturnPage = 0, string v_strController = "", string v_strAction = "",
                                                 int v_intManageUserReturnPage = 0, string v_strManageuserSearchString = "")
        {
            try
            {
                ViewBag.v_intReturnPage = v_intReturnPage;
                ViewBag.v_strController = v_strController;
                ViewBag.v_strAction = v_strAction;
                ViewBag.v_intManageUserReturnPage = v_intManageUserReturnPage;
                ViewBag.v_strManageuserSearchString = v_strManageuserSearchString;


                if (User.Identity.IsAuthenticated)
                {
                    //need and id to edit
                    if (v_intIssueID == null)
                    {
                        return NotFound();
                    }

                    //check to see if the id exist in the db
                    var l_clsIssue = await _context.Issues.FindAsync(v_intIssueID);
                    if (l_clsIssue == null)
                    {
                        return NotFound();
                    }

                    if (User.IsInRole("Admin"))
                    {
                        return View(l_clsIssue);
                    }
                    else
                    {
                        var l_clsUser = await _userManager.FindByEmailAsync(User.Identity.Name);
                        if (l_clsUser.UserID == l_clsIssue.UserCD)
                            return View(l_clsIssue);  //Non admin users can only view their own issues
                        else
                            return NotFound();
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