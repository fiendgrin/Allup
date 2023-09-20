using Allup.Areas.Manage.ViewModels.AccountVMs;
using Allup.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Allup.Areas.Manage.Controllers
{
    [Area("manage")]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM) 
        {
            if (!ModelState.IsValid)
            {
                return View(registerVM);
            }

            AppUser appUser = new AppUser 
            {
                UserName = registerVM.UserName,
                Email = registerVM.Email
            };

            if (await _userManager.Users.AnyAsync(u=>u.NormalizedUserName == registerVM.UserName.Trim().ToUpperInvariant()))
            {
                ModelState.AddModelError("UserName", $"'{registerVM.UserName}' Already Exists");
                return View(registerVM);
            }

            if (await _userManager.Users.AnyAsync(u => u.NormalizedEmail == registerVM.Email.Trim().ToUpperInvariant()))
            {
                ModelState.AddModelError("Email", $"'{registerVM.Email}' Already Exists");
                return View(registerVM);
            }

            IdentityResult identityResult = await _userManager.CreateAsync(appUser, registerVM.Password);

            if (!identityResult.Succeeded) 
            {
                foreach (IdentityError identityError in identityResult.Errors)
                {
                    ModelState.AddModelError("", identityError.Description);
                }
                return View(registerVM);
            }

            await _userManager.AddToRoleAsync(appUser, "Admin");

            return Ok();
        }

        //public async Task<IActionResult> CreateSuperAdmin() 
        //{
        //    AppUser appUser = new AppUser
        //    {
        //        Email="superadmin@gmail.com",
        //        UserName = "superadmin"
        //    };

        //    await _userManager.CreateAsync(appUser, "SuperAdmin666");
        //    await _userManager.AddToRoleAsync(appUser, "SuperAdmin");


        //    return Ok("Super Admin Created");
        //}


        //public async Task<IActionResult> CreateRole(RegisterVM registerVM) 
        //{
        //    await _roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
        //    await _roleManager.CreateAsync(new IdentityRole("Admin"));
        //    await _roleManager.CreateAsync(new IdentityRole("Member"));

        //    return Ok("Roles Created");
        //}
    }
}
