using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace SAC_Web_Application.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Club Committee";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Contact us";

            return View();
        }
        public IActionResult Information()
        {
            ViewData["Message"] = "Training Information";

            return View();
        }
        public IActionResult Subscriptions()
        {
            ViewData["Message"] = "Registration page.";

            return View();
        }
        public IActionResult Events()
        {
            ViewData["Message"] = "This page will be a list of all events members can take part in";

            return View();
        }
        public IActionResult Gallery()
        {
            ViewData["Message"] = "Sligo Athletics Club Photos";

            return View();
        }
        
        public IActionResult Admin()
        {
            ViewData["Message"] = "Administration Home Page";

            return View();
        }
        public IActionResult EventRegistration()
        {
            ViewData["Message"] = "Register for your event here";

            return View();
        }
        public IActionResult GroupEmail()
        {
            ViewData["Message"] = "Send an email to a group of members";

            return View();
        }
        public IActionResult Coaches()
        {
            ViewData["Message"] = "List of coaches";

            return View();
        }

     
        public IActionResult Error()
        {
            return View();
        }
    }
}
