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

    public class MatriarchyController : Controller
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
        public MatriarchyController(MvcMatriarchyContext context, UserManager<ApplicationUser> userManager,
                                ILogger<MatriarchyController> logger)
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

    }
}