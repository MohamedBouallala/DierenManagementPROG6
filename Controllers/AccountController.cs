using DierenManagement.Models;
using DierenManagement.ViewModels;
using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace DierenManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager,SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Register() // view
        {
            var model = new RegisterViewModel();
            ViewBag.LoyaltyCards = Enum.GetValues(typeof(LoyaltyCard)).Cast<LoyaltyCard>().ToList();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {

                var user = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Address = model.Address,
                    Email = model.Email,
                    NormalizedEmail = model.Email.ToUpper(),
                    PhoneNumber = model.PhoneNumber,
                    UserName = model.Email
                    

                };

                var generatedPassword = GeneratePassword();

                model.GeneratedPassword = generatedPassword;

                user.Password = generatedPassword;
                user.ConfirmPassword = generatedPassword;

                var result = await _userManager.CreateAsync(user,generatedPassword);

                if (result.Succeeded)
                {
                    //_logger

                    var roleResult = await _userManager.AddToRoleAsync(user, "Client"); // "Client" hard coded.

                    if (roleResult.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return View("Register", model);
                    }

                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }


            }

                return View(model);

        }

        private string GeneratePassword()
        {

            var option = new PasswordOptions()
            {
                RequiredLength = 8,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,

            };


            Random random = new Random();

            var password = new StringBuilder();


            if (option.RequireDigit)
            {
                password.Append(random.Next(0, 20));
            }
            if (option.RequireLowercase)
            {
                password.Append((char)random.Next('a', 'z' + 1));
            }
            if (option.RequireUppercase)
            {
                password.Append((char)random.Next('A', 'Z' + 1));
            }

            while (password.Length < option.RequiredLength)
            {
                char nextChar = (char)random.Next(33, 126);
                password.Append(nextChar);
            }

            return password.ToString();

        }

        public IActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogIn(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {

                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Animal");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "invalid Login attempt.");
                    return View();
                }
            }
            return View();
        }


    }

}
