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
        public HomeController( ILogger<ReportsController> logger)
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
                ViewData["Message"] = "The Matriarchy streaming TV services evaluator application is a senior class project for " +
                                      "UMKC CSC 451R Software Engineering Capstone class.  The goal is to allow users to be " +
                                      "able to easily compare different Streaming TV services by evaluating their favorite " +
                                      "TV shows, networks and movies, which the application will then turn into a recomendation.";

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
                ViewData["Message"] = "We can help you find your Streaming Serice, get in our Matriarchy!";
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
