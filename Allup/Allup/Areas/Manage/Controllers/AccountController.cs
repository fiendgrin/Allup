using Allup.Areas.Manage.ViewModels.AccountVMs;
using Allup.Models;
using Microsoft.AspNetCore.Authorization;
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
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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

            if (await _userManager.Users.AnyAsync(u => u.NormalizedUserName == registerVM.UserName.Trim().ToUpperInvariant()))
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

            return RedirectToAction(nameof(Login));
        }

        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }

            AppUser appUser = await _userManager.FindByEmailAsync(loginVM.Email);

            if (appUser == null)
            {
                ModelState.AddModelError("", "Email or Password are incorrect");
                return View(loginVM);
            }

            //if (await _userManager.CheckPasswordAsync(appUser,loginVM.Password))
            //{
            //    ModelState.AddModelError("", "Email or Password are incorrect");
            //    return View(loginVM);
            //}

            if (!appUser.IsActive)
            {
                return Unauthorized();
            }

            Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager.PasswordSignInAsync(appUser, loginVM.Password, loginVM.RememberMe, true);

            if (!signInResult.Succeeded)
            {
                ModelState.AddModelError("", "Email or password are incorrect");
                return View(loginVM);
            }

            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelError("", "Your Account is blocked");
                return View(loginVM);
            }

            return RedirectToAction("Index", "Dashboard", new { area = "manage" });
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Profile()
        {
            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);

            if (appUser == null) return BadRequest();

            ProfileVM profileVM = new ProfileVM
            {
                Name = appUser.Name,
                SurName = appUser.SurName,
                Email = appUser.Email,
                UserName = appUser.UserName
            };


            return View(profileVM);
        }


        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Profile(ProfileVM profileVM)
        {
            if (!ModelState.IsValid) return View(profileVM);

            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);

            if (appUser.NormalizedUserName != profileVM.UserName.Trim().ToUpperInvariant())
            {
                appUser.UserName = profileVM.UserName.Trim();
            }

            if (appUser.NormalizedEmail != profileVM.Email.Trim().ToUpperInvariant())
            {

                appUser.Email = profileVM.Email.Trim();
            }

            appUser.SurName = profileVM.SurName;
            appUser.Name = profileVM.Name;

            IdentityResult identityResult =  await _userManager.UpdateAsync(appUser);

            if (!identityResult.Succeeded)
            {
                foreach (IdentityError identityError in identityResult.Errors)
                {
                    ModelState.AddModelError("", identityError.Description);
                }
                return View(profileVM);
            }

            if (!string.IsNullOrWhiteSpace(profileVM.CurrentPassword))
            {
                if (await _userManager.CheckPasswordAsync(appUser,profileVM.CurrentPassword))
                {
                    ModelState.AddModelError("CurrentPassword", "Current Password is Incorrect");
                    return View(profileVM);
                }

                string token = await _userManager.GeneratePasswordResetTokenAsync(appUser);

               identityResult = await _userManager.ResetPasswordAsync(appUser, token, profileVM.NewPassword);

                if (!identityResult.Succeeded)
                {
                    foreach (IdentityError identityError in identityResult.Errors)
                    {
                        ModelState.AddModelError("", identityError.Description);
                    }
                    return View(profileVM);
                }
            }

            await _signInManager.SignInAsync(appUser, true);

            return RedirectToAction("Index", "Dashboard", new { area = "manage" });
        }

        public async Task<IActionResult> Logout(ProfileVM profileVM) 
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
    }
}
