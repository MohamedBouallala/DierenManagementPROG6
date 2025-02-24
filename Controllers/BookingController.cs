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

namespace DierenManagement.Controllers
{
    public class BookingController : Controller
    {
        AnimalManagementDbContext _context;

        List<string> discountDetails = new List<string>(); // dit moet later weg!!

        public BookingController(AnimalManagementDbContext context)
        {
            this._context = context;
        }


        public ActionResult Index() // select a date view
        {
            return View();
        }

        [HttpGet]
        public ActionResult Animals(BookingViewModel model) // all animals view
        {
            //de datum geselecteerd in index komt hier in.
            //model.Animals = _context.Animals.ToList();

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


            // ik moet hier later de animals die de user al heeft geboekt terug uithalen. en dan vergelijken met model.animal

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
                    CardRulesNotViolated(model);
                }

                CanAnimalsBeBooked(model);
                
                int totaalPrice = 0;

                foreach (var animal in listWithSelectedAnimals)
                {
                    totaalPrice += animal.Price;
                }

                decimal priceInDecimal = (decimal)totaalPrice;

                decimal discountPercentage = DiscountCalculator(model);

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

        public bool CanAnimalsBeBooked(BookingViewModel model)
        {
            List<string> errors = new List<string>();

            if (IsthereACornivoreAndFarmAnimal(model.listWithSelectedAnimals))
            {
                //throw new Exception("Sorry you can't book a Carnivore and a ForestAnimal at the same time");
                errors.Add("Sorry you can't book a Carnivore and a Farm Animal at the same time");

            }
            if (IsAnimalBookedOnaBadDay(model))
            {
                //throw new Exception("Sorry you can't book a Penguin during the weekends");
                errors.Add("Sorry you can't book a Penguin during the weekends");

            }
            if (IsThereDesertAnimalBookedOnBadMonth(model))
            {
                //throw new Exception("Sorry you can't book a desert animal between the months October to February");
                errors.Add("Sorry you can't book a desert animal between the months October to February");

            }
            if (IsThereSnowAnimalBookedOnBadMonth(model))
            {
                //throw new Exception("Sorry you can't book a Snow animal between the months June to August");
                errors.Add("Sorry you can't book a Snow animal between the months June to August");

            }
            if (errors.Any())
            {
                throw new Exception(string.Join("<br/>", errors));
            }
            return true;

        }

        public bool IsthereACornivoreAndFarmAnimal(List<Animal> animals)
        {
            //List<Animal> selectedAnimals = animals.Where(a => a.IsSelected == true).ToList();

            bool thereIsAcarnivore = false;
            bool thereIsAForestAnimal = false;

            foreach (Animal animal in animals)
            {
                if (animal.AnimalType == AnimalType.Carnivore)
                {
                    thereIsAcarnivore = true;
                }
                if(animal.AnimalType == AnimalType.FarmAnimal)
                {
                    thereIsAForestAnimal = true;
                }

            }

            if (thereIsAcarnivore && thereIsAForestAnimal)  
            {
                //throw new Exception("Sorry you can't book a Carnivore and a ForestAnimal at the same time");
                return true;
            }
            else
            {
                return false;
            }
            
        }

        public bool IsAnimalBookedOnaBadDay(BookingViewModel booking) // penguin
        {
            foreach(var animal in booking.listWithSelectedAnimals)
            {
                if (animal.Name == "Penguin" && (booking.Date.DayOfWeek == DayOfWeek.Saturday || booking.Date.DayOfWeek == DayOfWeek.Sunday))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsThereDesertAnimalBookedOnBadMonth(BookingViewModel booking)
        {
            foreach (var animal in booking.listWithSelectedAnimals)
            {
                if (animal.AnimalType == AnimalType.DesertAnimal && ((booking.Date.Month >= 10 && booking.Date.Month <= 12) || booking.Date.Month >= 1 && booking.Date.Month <= 2))
                {
                    return true;
                }
            }
            
            return false ;
        }
        public bool IsThereSnowAnimalBookedOnBadMonth(BookingViewModel booking)
        {
            foreach (var animal in booking.listWithSelectedAnimals)
            {
                if (animal.AnimalType == AnimalType.Snow && booking.Date.Month >= 6 && booking.Date.Month <= 8)
                {
                    return true;
                }
            }

            return false;
        }

        public bool CardRulesNotViolated(BookingViewModel booking)
        {
            if (booking.User.LoyaltyCard == LoyaltyCard.White && (booking.listWithSelectedAnimals.Count() > 3 ||
                booking.listWithSelectedAnimals.Any(a => a.AnimalType == AnimalType.VIP)))
            {
                throw new Exception("Sorry you have a White Loyalty Card which means you can select only 3 animals and you can't select a VIP animal");
            }
            else if (booking.User.LoyaltyCard == LoyaltyCard.Silver && (booking.listWithSelectedAnimals.Count() > 4 ||
                booking.listWithSelectedAnimals.Any(a => a.AnimalType == AnimalType.VIP)))
            {
                throw new Exception("Sorry you have a Silver Loyalty Card which means you can select only 4 animals and you can't select a VIP animal");

            }
            else if (booking.User.LoyaltyCard == LoyaltyCard.Gold && booking.listWithSelectedAnimals.Any(a => a.AnimalType == AnimalType.VIP))
            {
                throw new Exception("Sorry you have a Gold Loyalty Card which means you can't select a VIP animal");
            }
            //voor platinum card hoeft niks
            return true;
        }

        public decimal DiscountCalculator(BookingViewModel booking)
        {         

            Random random = new Random();

            decimal totalDiscount =  0m;

            // 10% korting voor 3 dieren van hetzelfde type
            var groupedAnimals = booking.listWithSelectedAnimals.GroupBy(a => a.AnimalType);

            if (groupedAnimals.Any(g =>g.Count() >= 3))
            {
                totalDiscount += 10m;
                discountDetails.Add("10% Discount for 3 animals of the same type");
            }

            // 1 op 6 kans op 50% korting als 'Eend' in de boeking zit

            if (booking.listWithSelectedAnimals.Any(a => a.Name == "Duck"))
            {
                int chance = random.Next(1,7);
                if (chance == 5)
                {
                    totalDiscount += 50m;
                    discountDetails.Add("1 in 6 chance of a 50% discount if 'Duck' is included in the booking");
                }
            }

            //15% korting voor boekingen op maandag of dinsdag

            if (booking.Date.DayOfWeek == DayOfWeek.Monday || booking.Date.DayOfWeek == DayOfWeek.Tuesday)
            {
                totalDiscount += 15m;
                discountDetails.Add("15% discount for bookings on Monday or Tuesday");
            }

            //Extra korting voor letters in dierennamen

            var uniquLetters = new HashSet<char>();

            foreach (var animal in booking.listWithSelectedAnimals)
            {
                foreach (char c in animal.Name.ToUpper())
                {
                    if (char.IsLetter(c))
                    {
                        uniquLetters.Add(c);
                    }
                }
            }

            totalDiscount += uniquLetters.Count * 2m;
            discountDetails.Add("Extra discount for unique letters in animal names");

            //10% korting voor klanten met een klantenkaart

            string userName = User.Identity.Name; // GetId werkt niet

            User? user = _context.Users.FirstOrDefault(a => a.UserName == userName);

            if (user.LoyaltyCard != LoyaltyCard.White)
            {
                totalDiscount += 10m;
                discountDetails.Add("10 % discount for customers with a loyalty card");
            }

            //Maximaal 60% korting

            totalDiscount = Math.Min(totalDiscount, 60m);

            return totalDiscount;

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


        public ActionResult Bookings() // Gebruik Groep by. check of je bij ellke datum of je alle dieren der bij krijgt.
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
