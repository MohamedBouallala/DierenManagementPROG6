using DAL;
using DierenManagement.DbContextFile;
using DierenManagement.Models;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DierenManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AnimalController : Controller
    {
        AnimalManagementDbContext _context;

        public AnimalController(AnimalManagementDbContext context)
        {
            this._context = context;

        }

        [HttpGet]
        public ActionResult Index()
        {
            ICollection<Animal> animals = _context.Animals.ToList();

            return View(animals);
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            Animal? animal = _context.Animals.FirstOrDefault(animal => animal.Id == id);

            return View(animal);
        }

        //[Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            List<AnimalType> animalType = Enum.GetValues(typeof(AnimalType)).Cast<AnimalType>().ToList();
            ViewBag.AnimalType = animalType;

            return View();
        }

        // POST: AnimalController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles ="Admin")]
        public ActionResult Create(Animal animal)
        {
            try
            {
                _context.Animals.Add(animal);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AnimalController/Edit/5
        //[Authorize(Roles ="Admin")]
        public ActionResult Edit(int id)
        {
            Animal? animal = _context.Animals.FirstOrDefault(a => a.Id == id);
            List<AnimalType> animalTypes = Enum.GetValues(typeof(AnimalType)).Cast<AnimalType>().ToList();
            ViewBag.AnimalType = animalTypes;

            return View(animal);
        }

        // POST: AnimalController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles ="Admin")]
        public ActionResult Edit(Animal currentAnimal)
        {

            if (currentAnimal != null)
            {

                Animal? animalToUpdate = _context.Animals.FirstOrDefault(a => a.Id == currentAnimal.Id);

                if (animalToUpdate != null)
                {
                    _context.Entry(animalToUpdate).CurrentValues.SetValues(currentAnimal);
                    _context.SaveChanges();
                }

                return RedirectToAction(nameof(Index));

            }
            else
            {
                return View();
            }

        }

        // GET: AnimalController/Delete/5
        //[Authorize(Roles ="Admin")]
        public ActionResult Delete(int id)
        {
            Animal? animal = _context.Animals.FirstOrDefault(anim => anim.Id == id);
            return View(animal);
        }

        // POST: AnimalController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteButton(int id)
        {
            try
            {
                Animal? animal = _context.Animals.FirstOrDefault(a => a.Id == id);

                _context.Animals.Remove(animal);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(nameof(Delete));
            }
        }
    }
}
