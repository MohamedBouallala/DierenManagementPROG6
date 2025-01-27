using DierenManagement.Models;
using Domain;

namespace DierenManagement.ViewModels
{
    public class BookingViewModel
    {
        public DateOnly Date { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }

        //public int AnimalId { get; set; }
        //public Animal Animal { get; set; }

        public List<Animal> Animals { get; set; } = new List<Animal>();

        // dit heb ik toegevoegd zou dat het handig is met alle dieren + geselecteerde dieren te werken (controller)
        public List<Animal> listWithSelectedAnimals { get; set; } = new List<Animal>(); 
    }
}
