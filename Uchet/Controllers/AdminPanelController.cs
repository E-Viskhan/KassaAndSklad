using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Uchet.Data;

namespace Uchet.Controllers
{
    public class AdminPanelController : Controller
    {
        private ApplicationDbContext db;
        private RoleManager<IdentityRole> roleManager;

        public AdminPanelController(ApplicationDbContext context, RoleManager<IdentityRole> _roleManager)
        {
            db = context;
            roleManager = _roleManager;
        }


        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View(await db.Shops.ToListAsync());
        }

        public IActionResult AllUsers()
        {
            var users = db.Users.ToList();
            return View(users);
        }
        public IActionResult AllShops()
        {
            //db.Add(new IdentityRole("NewRole"));
            //var role = await roleManager.FindByNameAsync("NewRole");
            //await roleManager.DeleteAsync(role)
            var shops = db.Shops.ToList();;
            return View(shops);
        }
    }
}