using App.MvcWebUI.Entities;
using App.MvcWebUI.Models;
using App.MvcWebUI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;

namespace App.MvcWebUI.Controllers
{

    public class AccountController : Controller
    {
        private UserManager<CustomIdentityUser> _userManager;
        private RoleManager<CustomIdentityRole> _roleManager;
        private SignInManager<CustomIdentityUser> _signInManager;

        public AccountController(UserManager<CustomIdentityUser> userManager,
            RoleManager<CustomIdentityRole> roleManager,
            SignInManager<CustomIdentityUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }
        public IActionResult Register()
        {

            return View();
        }
        [Authorize(Policy = "Add")]
        public IActionResult RegisterEditor()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        async public Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                CustomIdentityUser user = new CustomIdentityUser
                {
                    UserName = registerViewModel.Username,
                    Email = registerViewModel.Email,
                };

                IdentityResult result = _userManager.CreateAsync(user, registerViewModel.Password).Result;


                if (result.Succeeded)
                {
                    if (!_roleManager.RoleExistsAsync("Admin").Result)
                    {
                        CustomIdentityRole role = new CustomIdentityRole
                        {
                            Name = "Admin"
                        };
                        //claims to role


                        IdentityResult roleResult = _roleManager.CreateAsync(role).Result;


                        _roleManager.AddClaimAsync(role, new Claim("Edit", "true")).Wait();
                        _roleManager.AddClaimAsync(role, new Claim("Add", "true")).Wait();
                        _roleManager.AddClaimAsync(role, new Claim("Delete", "true")).Wait();
                        _roleManager.AddClaimAsync(role, new Claim("CreateRole", "true")).Wait();


                        if (!roleResult.Succeeded)
                        {
                            ModelState.AddModelError("", "We can not add the role!");
                            return View(registerViewModel);
                        }
                    }


                    _userManager.AddToRoleAsync(user, "Admin").Wait();
                    return RedirectToAction("Login", "Account");

                }
            }
            return View(registerViewModel);
        }
        [Authorize(Policy = "Add")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        async public Task<IActionResult> RegisterEditor(RegisterViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                CustomIdentityUser user = new CustomIdentityUser
                {
                    UserName = registerViewModel.Username,
                    Email = registerViewModel.Email,
                };
                // hemen identity(user,role,policy ola biler) yaranmasiyla bagli melumat verir
                IdentityResult result = _userManager.CreateAsync(user, registerViewModel.Password).Result;

                if (result.Succeeded)
                {
                    if (!_roleManager.RoleExistsAsync("Editor").Result)
                    {
                        CustomIdentityRole role = new CustomIdentityRole
                        {
                            Name = "Editor"
                        };

                        //add claim to role

                        IdentityResult roleResult = _roleManager.CreateAsync(role).Result;


                        _roleManager.AddClaimAsync(role, new Claim("Edit", "true")).Wait();
                        _roleManager.AddClaimAsync(role, new Claim("Add", "true")).Wait();
                        _roleManager.AddClaimAsync(role, new Claim("Delete", "true")).Wait();


                        if (!roleResult.Succeeded)
                        {
                            ModelState.AddModelError("", "We can not add the role!");
                            return View(registerViewModel);
                        }
                    }


                    _userManager.AddToRoleAsync(user, "Editor").Wait();
                    await _userManager.AddClaimAsync(user, new Claim("Editor", "true"));

                    return RedirectToAction("Login", "Account");

                }
            }
            return View(registerViewModel);
        }

        [Authorize(Policy = "CreateRole")]

        public IActionResult CreateRole()
        {

            var model = new CreateRoleViewModel
            {
                RoleName = "",
            };

            model.Claims = ClaimsService.Claims.Select(c => new SelectListItem
            {
                Text = c,
                Value = c
            }).ToList();

            return View(model);
        }

        [Authorize(Policy = "CreateRole")]
        [HttpPost]
        public IActionResult CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                CustomIdentityRole role = new CustomIdentityRole
                {
                    Name = model.RoleName
                };
                IdentityResult result = _roleManager.CreateAsync(role).Result;

                if (result.Succeeded)
                {

                    List<SelectListItem> userClaims = model.Claims.Where(c => c.Selected).ToList();
                    foreach (var claim in userClaims)
                        _roleManager.AddClaimAsync(role, new Claim(claim.Text, claim.Value)).Wait();

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }
            return View(model);
        }

        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = _signInManager.PasswordSignInAsync(loginViewModel.UserName,
                    loginViewModel.Password, loginViewModel.RememberMe, false).Result;
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Admin");
                }
                ModelState.AddModelError("", "Invalid Login");
            }

            return View(loginViewModel);
        }


        public IActionResult LogOff()
        {
            _signInManager.SignOutAsync().Wait();
            return RedirectToAction("Login");
        }

    }
}
