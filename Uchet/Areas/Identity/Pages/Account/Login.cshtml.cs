﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Uchet.Data;

namespace Uchet.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;



        public LoginModel(SignInManager<IdentityUser> signInManager, 
            ILogger<LoginModel> logger,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
            _db = db;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Электронная почта:")]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Пароль")]
            public string Password { get; set; }

            [Display(Name = "Запомнить меня!")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (_roleManager.Roles.Count() == 0)
            {
                await _roleManager.CreateAsync(new IdentityRole //владелец сервиса - Я
                {
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                });
                await _roleManager.CreateAsync(new IdentityRole //Владелец магазина, их может быть, и должно быть много)
                {
                    Name = "Owner",
                    NormalizedName = "OWNER"
                });

                await _roleManager.CreateAsync(new IdentityRole //Внутри интерфейса владельца магазина будет кнопка добавить кассира
                {
                    Name = "Cashier",
                    NormalizedName = "CASHIER"
                });

            }

            var user = await _userManager.FindByEmailAsync(Input.Email);

            if (!(_userManager.IsInRoleAsync(user, "Owner")).Result)
            {
                await _userManager.AddToRoleAsync(user, "Owner");
            }

            var admin = await _userManager.FindByEmailAsync("viskhan11@gmail.com");
            if (!(_userManager.IsInRoleAsync(admin, "Admin")).Result)
            {
                await _userManager.AddToRoleAsync(admin, "Admin");
                await _userManager.AddToRoleAsync(admin, "Cashier");
            }

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var user1 = await _userManager.FindByEmailAsync(Input.Email);
                    var roles = await _userManager.GetRolesAsync(user1);
                    var isAdmin = roles.Where(r => r.Contains("Admin")).FirstOrDefault();
                    var isOwner = roles.Where(r => r.Contains("Owner")).FirstOrDefault();

                    if (isAdmin != null)
                    {
                        _logger.LogInformation("Admin logged in."); //потом тут можно в лог добавлять id Admin'ов вместо просто слова
                        return LocalRedirect("~/AdminPanel/Index");
                    }
                    else if (isOwner != null) 
                    {
                        _logger.LogInformation("Owner logged in."); //потом тут можно в лог добавлять id конкретных владельцев вместо просто слова
                        return LocalRedirect("~/OwnerPanel/Index");
                    }
                    else
                    {
                        _logger.LogInformation("Cashier logged in."); //потом тут можно в лог добавлять id конкретных кассиров вместо просто слова
                        return LocalRedirect("~/CashierPanel/Index");
                    }
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
