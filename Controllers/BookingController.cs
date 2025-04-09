using DierenManagement.DbContextFile;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DierenManagement.ViewModels;
using Microsoft.AspNetCore.Identity;
using DierenManagement.Models;
using Domain;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Interface;
using BusinessLayer;

namespace DierenManagement.Controllers
{
    public class BookingController : Controller
    {
        AnimalManagementDbContext _context;

        private readonly IBookingValidator _BookingValidator; //= new BookingValidation();

        public BookingController(AnimalManagementDbContext context, IBookingValidator bookingValidator)
        {
            this._context = context;
            this._BookingValidator = bookingValidator;
        }


        public ActionResult Index() // select a date view
        {
            return View();
        }

        [HttpGet]
        public ActionResult Animals(BookingViewModel model) // all animals view
        {
            //de datum geselecteerd in index komt hier in.

            List<Animal> animals = _context.Animals.ToList();

            //hier haal ik dieren die al geboekt zijn op de geselcteerde datum.
            List<Animal> animalsAlreadyBookedOnTheDate = _context.bookings
                .Where(b=>b.Date == model.Date)
                .Select(b=>b.Animal)
                .ToList();

            // hier filter de dieren

            List<Animal> availableAnimals = animals
                .Where(a => !animalsAlreadyBookedOnTheDate.Any(ab => ab.Id == a.Id))
                .ToList();

            model.Animals = availableAnimals;

            string userName = User.Identity.Name; // GetId werkt niet

            var user = _context.Users.FirstOrDefault(u => u.UserName == userName);

            if (user != null)
            {
                model.UserId = user.Id;
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult BookingDetails(BookingViewModel model) // userID, date , list met dieren
        {
            try
            {                
                List<Animal> listWithSelectedAnimals = model.Animals.Where(a => a.IsSelected == true).ToList();
                
                model.listWithSelectedAnimals = listWithSelectedAnimals;

                User? user = _context.Users.FirstOrDefault(u => u.Id == model.UserId);
                model.User = user;

                if (User.Identity.IsAuthenticated)
                {
                    _BookingValidator.CardRulesNotViolated(model.ConvertToBookingDto());
                }


                _BookingValidator.CanAnimalsBeBooked(model.ConvertToBookingDto());

                
                int totaalPrice = 0;

                foreach (var animal in listWithSelectedAnimals)
                {
                    totaalPrice += animal.Price;
                }

                decimal priceInDecimal = (decimal)totaalPrice;

                var (discountPercentage, discountDetails) = _BookingValidator.DiscountCalculator(model.ConvertToBookingDto());

                decimal discountAmount = (priceInDecimal * discountPercentage) / 100;

                decimal finalPrice = priceInDecimal - discountAmount;

                ViewBag.DiscountDetails = discountDetails;

                ViewBag.Totaal = finalPrice; // met korting

                if (model.listWithSelectedAnimals.Count() == 0)
                {
                    return View("Animals", model);
                }
                else
                {
                    return View("BookingData", model);
                }


            }
            catch (Exception ex) 
            {
                ViewBag.Error = ex.Message;
                return View(nameof(Animals),model);
            }          

        }

        public ActionResult ConfirmBookingButton(BookingViewModel model)
        {

            foreach (var listSelectedAnimals in model.listWithSelectedAnimals)
            {
                Booking booking = new Booking()
                {
                    Date = model.Date,
                    UserId = model.User.Id,
                    AnimalId = listSelectedAnimals.Id
                };

                _context.bookings.Add(booking);
            }
            _context.SaveChanges();

            return View("EndBookingView", model);
        }


        public ActionResult Bookings()
        {

            string userName = User.Identity.Name; // GetId werkt niet

            var user = _context.Users.FirstOrDefault(u => u.UserName == userName);

            List<Booking> bookings = _context.bookings.Where(b => b.UserId == user.Id).Include(b => b.Animal).ToList();

            List<BookingViewModel> groepedBookings = bookings.GroupBy(b => b.Date)
                .Select(g => new BookingViewModel
                {
                    User = user,
                    Date = g.Key,
                    Animals = g.Select(b => b.Animal).ToList(),
                }).ToList();


            return View("AllBookings",groepedBookings);
        }

        public ActionResult DeleteBooking(BookingViewModel model) // hier komt de userId en de Datum van een boeking
        {
            List<Booking> bookingToDelete = _context.bookings.Where(b => b.UserId == model.UserId && b.Date == model.Date)
                .ToList();

            if (bookingToDelete != null)
            {
                foreach (Booking booking in bookingToDelete)
                {
                    _context.bookings.Remove(booking);
                    _context.SaveChanges();
                }
            }


            string userName = User.Identity.Name; // GetId werkt niet
            var user = _context.Users.FirstOrDefault(u => u.UserName == userName);

            List<Booking> bookings = _context.bookings.Where(b => b.UserId == user.Id).Include(b => b.Animal).ToList();

            List<BookingViewModel> groepedBookings = bookings.GroupBy(b => b.Date)
                .Select(g => new BookingViewModel
                {
                    User = user,
                    Date = g.Key,
                    Animals = g.Select(b => b.Animal).ToList(),
                }).ToList();
            
            return View("AllBookings", groepedBookings);
        }

        [Authorize(Roles ="Admin")]
        public ActionResult AnimalsBookingDetails()
        {
            List<Booking> bookings = _context.bookings.Include(b => b.Animal).ToList();

            List<BookingViewModel> groupedBookings = bookings.GroupBy(b => b.Date)
                .Select(g => new BookingViewModel
                {
                    Date = g.Key,
                    Animals = g.Select(b => b.Animal).ToList(),
                }).ToList();


            return View(groupedBookings);
        }

    }
}
