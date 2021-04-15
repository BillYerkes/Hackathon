using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Canoe.Data;
using Canoe.Models;
using Microsoft.AspNetCore.Http;

namespace Canoe.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /User/
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Models.User user)
        {
            if (ModelState.IsValid)
            {
                if (user.IsValid(user.UserName, user.Password))
                {
                    //FormsAuthentication.SetAuthCookie(user.UserName, user.RememberMe);

                    HttpContext.Session.SetInt32("AccessLevel", user.AccessLevelCD);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Login data is incorrect!");
                }
            }
            return View(user);
        }
        public ActionResult Logout()
        {
            //FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        public int AccessLevel()
        {
            int x;
            x = 25;
            return x;
        }
    }
}