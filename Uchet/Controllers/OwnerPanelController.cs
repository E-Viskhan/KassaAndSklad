using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Uchet.Controllers
{
    public class OwnerPanelController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult CreateShop() {
            return View();
        }

        [Authorize]
        public IActionResult AddGood()
        {
            return View();
        }

        [Authorize]
        public IActionResult AddEmployee()
        {
            return View();
        }
    }
}