using DierenManagement.DbContextFile;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DierenManagement.ViewModels;
using Microsoft.AspNetCore.Identity;
using DierenManagement.Models;
using Domain;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
            try
            {                
                List<Animal> listWithSelectedAnimals = model.Animals.Where(a => a.IsSelected == true).ToList();
                
                model.listWithSelectedAnimals = listWithSelectedAnimals;

                User? user = _context.Users.FirstOrDefault(u => u.Id == model.UserId);
                model.User = user;

                CardRulesNotViolated(model);
                CanAnimalsBeBooked(model);

                // een nieuwe methode om opteslaan die naar de BookingData view gaat

                //foreach (var listSelectedAnimals in listWithSelectedAnimals)   
                //{
                //    Booking booking = new Booking()
                //    {
                //        Date = model.Date,
                //        UserId = model.UserId,
                //        AnimalId = listSelectedAnimals.Id
                //    };

                //    _context.bookings.Add(booking);
                //}
                //_context.SaveChanges();
                
                int totaal = 0;

                foreach (var animal in listWithSelectedAnimals)
                {
                    totaal += animal.Price;
                }
                ViewBag.Totaal = totaal;

                return View("BookingData", model);


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

            if (IsthereACornivoreAndForestAnimal(model.listWithSelectedAnimals))
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

        public bool IsthereACornivoreAndForestAnimal(List<Animal> animals)
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

        public bool IsAnimalBookedOnaBadDay(BookingViewModel booking)
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


    }
}
