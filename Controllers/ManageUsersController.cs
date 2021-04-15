using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Matriarchy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;


namespace Matriarchy.Controllers
{

    [Authorize(Roles = "Admin")]
    public class ManageUsersController : Controller
    {
        
        private UserManager<ApplicationUser> userManager;
        private RoleManager<IdentityRole> roleManager;
        private readonly ILogger _logger;
        private readonly int m_intPageSize = 10;

        //**********************************************************************************************************
        // Constructor
        //
        // Set up the Controller and initalize modual level variables,
        //
        // Inputs:
        // userManager      reference to the Identity UserManager object
        //            
        //**********************************************************************************************************
        public ManageUsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
                                     ILogger<ReportsController> logger)
        {
            try
            {
                this.userManager = userManager;
                this.roleManager = roleManager;
                _logger = logger;
            }
            catch
            {

            }
        }
        //**********************************************************************************************************
        // List
        //
        // Return list of User
        //
        // Inputs:
        // string id         id of the User
        //**********************************************************************************************************
        public async Task<IActionResult> List(string v_strSearchString, int v_intPageNumber = 1, string v_strClearFilter="")
        {
            try
            {
                var l_clsUser = userManager.Users;

                if (v_strClearFilter == "Yes") v_strSearchString = "";

                if (!String.IsNullOrEmpty(v_strSearchString))
                {
                    ViewBag.v_strSearchString = v_strSearchString;
                    l_clsUser = l_clsUser.Where(s => s.UserName.ToUpper().Contains(v_strSearchString.ToUpper()));
                }

                l_clsUser.OrderBy(s => s.Email);

                return View(await PaginatedList<ApplicationUser>.CreateAsync(l_clsUser, v_intPageNumber, m_intPageSize, v_strSearchString));

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }

        }
        //**********************************************************************************************************
        // ManageUserRoles
        //
        // Allow the Admins to update the roles assigned to the user 
        // Prepares the information to be edit
        //
        // Inputs:
        // string           v_strUserID       User ID 
        //**********************************************************************************************************
        public async Task<IActionResult> ManageUserRoles(string v_strUserID, List<UserRoles> v_lstUserRoles, string v_strActionUR)
        {
            try
            {
                if (v_strActionUR == "Prep")
                {
                    // Put the user id in the view bag to be displayed on the page
                    ViewBag.userID = v_strUserID;

                    // Find the user we are going to update
                    var l_clsUser = await userManager.FindByIdAsync(v_strUserID);
                    if (l_clsUser == null)
                    {
                        ViewBag.ErrorMessage = $"User with Id = {v_strUserID} cannot be found.";
                        return View("NotFound");
                    }
                    // create a list to store the roles
                    var l_lstRoles = new List<UserRoles>();

                    // get ALL of the roles that are available 
                    foreach (var l_clsRole in roleManager.Roles)
                    {
                        // create user role to add to list
                        var l_clsUserRole = new UserRoles
                        {
                            RoleId = l_clsRole.Id,
                            RoleName = l_clsRole.Name
                        };
                        // if the user is in the role, set to true, else set to false
                        if (await userManager.IsInRoleAsync(l_clsUser, l_clsRole.Name))
                        {
                            l_clsUserRole.IsSelected = true;
                        }
                        else
                        {
                            l_clsUserRole.IsSelected = false;
                        }

                        // add the role to the list
                        l_lstRoles.Add(l_clsUserRole);

                    }
                    return View(l_lstRoles);

                }
                else if (v_strActionUR == "Update")
                {
                    var l_clsUser = await userManager.FindByIdAsync(v_strUserID);
                    if (l_clsUser == null)
                    {
                        ViewBag.ErrorMessage = $"User with Id = {v_strUserID} cannot be found.";
                        return View("NotFound");
                    }

                    var v_ilstRoles = await userManager.GetRolesAsync(l_clsUser);
                    var l_clsResult = await userManager.RemoveFromRolesAsync(l_clsUser, v_ilstRoles);

                    if (!l_clsResult.Succeeded)
                    {
                        ModelState.AddModelError("", "Cannot remove user existing roles.");
                        return View(v_lstUserRoles);
                    }

                    l_clsResult = await userManager.AddToRolesAsync(l_clsUser, v_lstUserRoles.Where(x => x.IsSelected).Select(y => y.RoleName));
                    if (!l_clsResult.Succeeded)
                    {
                        ModelState.AddModelError("", "Cannot add selected roles to user.");
                        return View(v_lstUserRoles);
                    }

                    TempData["v_strUserID"] = v_strUserID;
                    TempData["v_strAction"] = "Prep";

                    return RedirectToAction("EditUser");

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
        // EditUser
        //
        // Allow the Admins to edit properities for user accounts
        // Prepares the information to be edit
        //
        // Inputs:
        // string v_strID         id of the User
        //**********************************************************************************************************
        public async Task<IActionResult> EditUser(string v_strUserID, EditUser v_clsEditUser, string v_strAction = "")
        {
            try
            {
                if (TempData["v_strAction"] != null) v_strAction = (string)TempData["v_strAction"];
                if (TempData["v_strUserID"] != null) v_strUserID = (string)TempData["v_strUserID"];


                if (v_strAction == "Prep")
                {
                    //Find the user to edit
                    var l_clsUser = await userManager.FindByIdAsync(v_strUserID);
                    if (l_clsUser == null)
                    {
                        ViewBag.ErrorMessage = $"User with Id = {v_strUserID} cannot be found.";
                        return View("NotFound");
                    }

                    //Populate the list of roles assigned to the user
                    IList<string> IuserRoles = await userManager.GetRolesAsync(l_clsUser);
                    List<string> userRoles = new List<string>(IuserRoles.Select(x => (string)x));

                    //Populate the properties of the edit user object for the edit page
                    var l_clsEditUser = new EditUser
                    {
                        Id = l_clsUser.Id,
                        Email = l_clsUser.Email,
                        AccessFailedCount = l_clsUser.AccessFailedCount,
                        EmailConfirmed = l_clsUser.EmailConfirmed,
                        LockoutEnabled = l_clsUser.LockoutEnabled,
                        LockoutEnd = l_clsUser.LockoutEnd,
                        Roles = userRoles
                    };
                    return View(l_clsEditUser);


                }
                else if (v_strAction == "Update")
                {
                    //Find the user record from the Identity Table
                    var l_clsUser = await userManager.FindByIdAsync(v_clsEditUser.Id);
                    if (l_clsUser == null)
                    {
                        ViewBag.ErrorMessage = $"User with Id = {v_clsEditUser.Id} cannot be found.";
                        return View("NotFound");
                    }
                    else
                    { //Populate the User Object with the updates from the form
                        l_clsUser.Email = v_clsEditUser.Email;  //keep the user name and email the same
                        l_clsUser.UserName = v_clsEditUser.Email;
                        l_clsUser.AccessFailedCount = v_clsEditUser.AccessFailedCount;
                        l_clsUser.PhoneNumber = v_clsEditUser.PhoneNumber;

                        var l_clsResult = await userManager.UpdateAsync(l_clsUser);

                        //if it worked return to the list
                        if (l_clsResult.Succeeded)
                        {
                            return RedirectToAction("List");
                        }
                        //else display the errors
                        foreach (var error in l_clsResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }

                        return View(v_clsEditUser);
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