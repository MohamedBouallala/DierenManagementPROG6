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
        //AnimalManagementDbContext _context;

        private IAnimalRepository _animalRepository;

        public AnimalController(/*AnimalManagementDbContext context,*/ IAnimalRepository repository)
        {
            //this._context = context;
            _animalRepository = repository;
        }

        [HttpGet]
        public ActionResult Index()
        {
            ICollection<Animal> animals = _animalRepository.GetAllAnimals().ToList();

            return View(animals);
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            //Animal? animal = _context.Animals.FirstOrDefault(animal => animal.Id == id);
            Animal? animal = _animalRepository.GetAnimalById(id);


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
                _animalRepository.Add(animal);
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
            //Animal? animal = _context.Animals.FirstOrDefault(a => a.Id == id);
            Animal animal = _animalRepository.GetAnimalById(id);

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

            //Animal? animalToUpdate = _context.Animals.FirstOrDefault(a => a.Id == currentAnimal.Id);

            //Animal animalToUpdate = _animalRepository.GetAnimalById(currentAnimal.Id);

            if (currentAnimal != null)
            {
                _animalRepository.Update(currentAnimal);
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
            //Animal? animal = _context.Animals.FirstOrDefault(anim => anim.Id == id);
            Animal animal = _animalRepository.GetAnimalById(id);


            return View(animal);
        }

        // POST: AnimalController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteButton(int id)
        {
            try
            {
                //Animal? animal = _context.Animals.FirstOrDefault(a => a.Id == id);
                //_context.Remove(animal);

                Animal animal = _animalRepository.GetAnimalById(id);
                _animalRepository.Delete(animal);

                //_context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(nameof(Delete));
            }
        }
    }
}
