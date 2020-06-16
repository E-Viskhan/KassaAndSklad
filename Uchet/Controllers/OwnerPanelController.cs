using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Uchet.Data;
using Uchet.Models;

namespace Uchet.Controllers
{
    public class OwnerPanelController : Controller
    {
        private ApplicationDbContext db;
        private readonly UserManager<IdentityUser> _userManager;

        public OwnerPanelController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            db = context;
            _userManager = userManager;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult CreateShop() {//Просто возвращает страницу создания магазина
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateShop(Shop shop) //обрабатывает добавление нового магазина
        {
            var user = await _userManager.GetUserAsync(User);
            var email = user.Email;
            shop.OwnerEmail = email;
            db.Shops.Add(shop);
            await db.SaveChangesAsync();
            return RedirectToAction("MyShops");
        }

        public async Task<IActionResult> MyShops() //выводит все магазины конкретного пользователя
        {
            var shops = db.Shops.ToList();
            var user = await _userManager.GetUserAsync(User);
            var email = user.Email;
            var MyShops = from s in shops
                          where s.OwnerEmail == email
                          select s;
            return View(MyShops);
        }

        [Authorize]
        public async Task<IActionResult> AddProduct()
        {
            //получаем список магазинов текущего пользвателя
            var shops = db.Shops.ToList();
            var user = await _userManager.GetUserAsync(User);
            var email = user.Email;
            var MyShops = from s in shops
                          where s.OwnerEmail == email
                          select s;
            //получили список магазинов

            //отправляем на страницу добавления продукта список магазинов данного пользователя
            //чтобы он мог выбрать конкретно из своего списока магазинов, куда добавить товар
            ViewBag.Shops = new SelectList(MyShops, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(Product product)
        {

            ////получаем список магазинов текущего пользвателя
            //var shops = db.Shops.ToList();
            //var user = await _userManager.GetUserAsync(User);
            //var email = user.Email;
            //var MyShops = from s in shops
            //              where s.OwnerEmail == email
            //              select s;
            ////получили список магазинов
            db.Add(product);
            await db.SaveChangesAsync();
            //Shop shop = MyShops.FirstOrDefault(c => c.Id == product.ShopId);
            return RedirectToAction("AllProducts");
        }

        [Authorize]
        public IActionResult AddEmployee()
        {
            return View();
        }
    }
}