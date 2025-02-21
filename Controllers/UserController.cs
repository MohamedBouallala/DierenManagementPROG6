using DierenManagement.ViewModels;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DierenManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UserController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: UserController
        public ActionResult Index()
        {
            var users =  _userManager.Users.ToList();

            var model = users.Select(user => new UserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                LoyaltyCard = user.LoyaltyCard
            }).ToList();

            return View(model);
        }

        // GET: UserController/Details/5
        public async Task<ActionResult> Details(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            UserViewModel model = new UserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                Password = user.Password,
                LoyaltyCard = user.LoyaltyCard
            };

            return View(model);
        }

        // GET: UserController/Edit/5
        public async Task<ActionResult> Edit(string id) // view
        {
            var user = await _userManager.FindByIdAsync(id);

            var model = new UserViewModel()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                LoyaltyCard = user.LoyaltyCard,
                Password = user.Password

            };

            ViewBag.LoyaltyCards = Enum.GetValues(typeof(LoyaltyCard)).Cast<LoyaltyCard>().ToList();

            return View(model);
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <ActionResult> Edit(string id, UserViewModel model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;
                user.Address = model.Address;
                user.UserName = model.Email;
                user.LoyaltyCard = model.LoyaltyCard;

                var result = await _userManager.UpdateAsync(user);
               // var passwordResult = await _userManager.ChangePasswordAsync(user, user.Password, model.Password); // dit werkt niet password hashed.

                if (result.Succeeded) 
                {
                     return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                ViewBag.LoyaltyCards = Enum.GetValues(typeof(LoyaltyCard)).Cast<LoyaltyCard>().ToList();
                return View(model);
            }
            catch
            {
                ViewBag.LoyaltyCards = Enum.GetValues(typeof(LoyaltyCard)).Cast<LoyaltyCard>().ToList();
                return View(model);
            }
        }

        // GET: UserController/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            var model = new UserViewModel() 
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
            };

            return View(model);
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <ActionResult> DeleteButton(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
