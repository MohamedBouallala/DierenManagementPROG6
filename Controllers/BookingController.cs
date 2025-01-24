using DierenManagement.DbContextFile;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DierenManagement.ViewModels;
using Microsoft.AspNetCore.Identity;
using DierenManagement.Models;
using Domain;

namespace DierenManagement.Controllers
{
    public class BookingController : Controller
    {
        AnimalManagementDbContext _context;

        public BookingController(AnimalManagementDbContext context)
        {
            this._context = context;
        }


        // GET: BookingController
        public ActionResult Index() // select a date view
        {
            return View();
        }

        
        public ActionResult Animals(BookingViewModel model) // all animals view
        {
            //de datum geselecteerd in index komt hier in.
            model.Animals = _context.Animals.ToList();

            // ik moet hier later de animals die de user al heeft geboekt terug uithalen. en dan vergelijken met model.animal

            string userName = User.Identity.Name; // GetId werkt niet

            var user = _context.Users.FirstOrDefault(u => u.UserName == userName);

            if (user != null)
            {
                model.UserId = user.Id;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult BookButton(BookingViewModel model) // userID, date , list met dieren
        {
            List<Animal> listWithSelectedAnimals = model.Animals.Where(a => a.IsSelected == true).ToList();

            foreach (var listSelectedAnimals in listWithSelectedAnimals)
            {
                Booking booking = new Booking()
                {
                    Date = model.Date,
                    UserId = model.UserId,
                    AnimalId = listSelectedAnimals.Id
                };
                
                _context.bookings.Add(booking);
            }
            _context.SaveChanges();
            
            return RedirectToAction("Index");
           // return View(); // naar stap 2 view.
        }

        // POST: BookingController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BookingData(BookingViewModel model) // Booking met data view maken
        {
            List<Animal> listWithSelectedAnimals = model.Animals.Where(a => a.IsSelected == true).ToList();

            var user = _context.Users.FirstOrDefault(u => u.Id == model.UserId);

            model.User = user;
            model.Animals = listWithSelectedAnimals;

            int totaal = 0;

            foreach (var animal in listWithSelectedAnimals)
            {
                totaal += animal.Price;
            }
            ViewBag.Totaal = totaal;

            return View(model);
        }

        // GET: BookingController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: BookingController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: BookingController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: BookingController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
