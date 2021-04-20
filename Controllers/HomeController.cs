using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Matriarchy.Models;
using Microsoft.Extensions.Logging;
using System;

namespace Matriarchy.Controllers
{
    public class HomeController : Controller
    {

        private readonly ILogger _logger;
        //**********************************************************************************************************
        // Constructor
        //
        // Set up the Controller and initalize modual level variables,
        //
        // Inputs:
        // ILogger<ReportsController> logger    Logger for logging errors
        //**********************************************************************************************************
        public HomeController( ILogger<HomeController> logger)
        {
            try
            {
                _logger = logger;
            }
            catch
            {

            }
        }

        //**********************************************************************************************************
        //
        //
        //**********************************************************************************************************

        public IActionResult Index()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }
        //**********************************************************************************************************
        //
        //
        //**********************************************************************************************************

        public IActionResult About()
        {
            try
            {
                ViewData["Message"] = "The Matriarchy web application is a tool to help uninsured residents " +
                                      "of New York state find health insurance to match their needs.  The application " +
                                      "all provides some historic information about Covid cases by county.";

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }
        //**********************************************************************************************************
        //
        //
        //**********************************************************************************************************

        public IActionResult Contact()
        {
            try
            {
                ViewData["Message"] = "We can help you find the health insurance for you.";
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }

        }
        //**********************************************************************************************************
        //
        //
        //**********************************************************************************************************

        public IActionResult Error()
        {
            try
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }


    }
}
